using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Bussiness;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using WeChatRelated.MP;
using WeChatRelated.Other;

namespace NetCoreAPI.Controllers.WeChat
{
    /// <summary>
    /// 微信支付
    /// </summary>
    [ApiController]
    public class PayV3Controller : WeChatApiBase
    {
        private string appid = Config.SenparcWeixinSetting.WeixinAppId;
        private string appSecret= Config.SenparcWeixinSetting.WeixinAppSecret;
        private static TenPayV3Info _tenPayV3Info;

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        public static TenPayV3Info TenPayV3Info
        {
            get
            {
                if (_tenPayV3Info == null)
                {
                    var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

                    _tenPayV3Info =
                        TenPayV3InfoCollection.Data[key];
                }
                return _tenPayV3Info;
            }
        }

        private readonly IServiceProvider _serviceProvider;
        public PayV3Controller(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
       
        #region 支付
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResult JsApi(int productId)
        {
            string errorMessage = string.Empty;
            try
            {
                //获取产品信息
                var products = ProductModel.GetFakeProductList();
                var product = products.FirstOrDefault(z => z.Id == productId);
                if (product == null)
                {
                    return HttpResult.WeChatError("商品信息不存在！", null);
                }

                var openId = Common.Current.WxOpenId;

                //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
                string sp_billno = string.Format("{0}{1}{2}", TenPayV3Info.MchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmssfff"),
                        TenPayV3Util.BuildRandomStr(6));
                //注意：以上订单号仅作为演示使用，如果访问量比较大，建议增加订单流水号的去重检查。

                var timeStamp = TenPayV3Util.GetTimestamp();
                var nonceStr = TenPayV3Util.GetNoncestr();

                var body = product.Name;
                var price = (int)(product.Price * 100);//单位：分
                var xmlDataInfo = new TenPayV3UnifiedorderRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, body, sp_billno, price, HttpContext.UserHostAddress()?.ToString(), TenPayV3Info.TenPayV3Notify, TenPayV3Type.JSAPI, openId, TenPayV3Info.Key, nonceStr);
                xmlDataInfo.NotifyUrl = "https://yufaquan.cn/wx/PayV3/PayNotifyUrl";

                var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口
                                                                //JsSdkUiPackage jsPackage = new JsSdkUiPackage(TenPayV3Info.AppId, timeStamp, nonceStr,);
                var package = string.Format("prepay_id={0}", result.prepay_id);

                //TO DO 
                //记录订单信息，留给退款申请接口测试使用
                //HttpContext.Session.SetString("BillNo", sp_billno);
                //HttpContext.Session.SetString("BillFee", price.ToString());


                var res = new
                {
                    product,
                    appId = TenPayV3Info.AppId,
                    timeStamp,
                    nonceStr,
                    package,
                    paySign = TenPayV3.GetJsPaySign(TenPayV3Info.AppId, timeStamp, nonceStr, package, TenPayV3Info.Key)
                };
                return HttpResult.Success(res);
            }
            catch (Exception ex)
            {
                //var msg = ex.Message;
                //msg += "<br>" + ex.StackTrace;
                //msg += "<br>==Source==<br>" + ex.Source;

                //if (ex.InnerException != null)
                //{
                //    msg += "<br>===InnerException===<br>" + ex.InnerException.Message;
                //}
                return HttpResult.WeChatError(ex.Message, null);
            }
        }

        /// <summary>
        /// JS-SDK支付回调地址（在统一下单接口中设置notify_url）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResult PayNotifyUrl()
        {
            try
            {
                ResponseHandler resHandler = new ResponseHandler(HttpContext);

                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");


                resHandler.SetKey(TenPayV3Info.Key);
                //验证请求是否从微信发过来（安全）
                if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
                {
                    //正确的订单处理
                    //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！
                }
                else
                {
                    //错误的订单处理
                }

                /* 这里可以进行订单处理的逻辑 */

                //发送支付成功的模板消息
                try
                {
                    string appId = Config.SenparcWeixinSetting.TenPayV3_AppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
                    string openId = resHandler.GetParameter("openid");
                    var templateData = new Weixin_PaySuccess("https://yufaquan.cn", "购买商品", "状态：" + return_code);

                    Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息参数", appId + " , " + openId);

                    var result = TemplateApi.SendTemplateMessage(appId, openId, templateData);
                }
                catch (Exception ex)
                {
                    Senparc.Weixin.WeixinTrace.SendCustomLog("支付成功模板消息", ex.ToString());
                }

                #region 记录日志


                #endregion


                var res = new
                {
                    return_code,
                    return_msg
                };
                return HttpResult.Success(res);
            }
            catch (Exception ex)
            {
                WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));
                throw;
            }
        }


        #endregion

        #region 订单及退款

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrderQuery()
        {
            string nonceStr = TenPayV3Util.GetNoncestr();

            TenPayV3OrderQueryRequestData data = new TenPayV3OrderQueryRequestData(TenPayV3Info.AppId,TenPayV3Info.MchId,"微信订单号",nonceStr,"商家订单号", TenPayV3Info.Key);

            var result = TenPayV3.OrderQuery(data);
            string openid = result.openid;

            return Content(openid);
        }

        /// <summary>
        /// 关闭订单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseOrder()
        {
            string nonceStr = TenPayV3Util.GetNoncestr();

            TenPayV3CloseOrderRequestData data = new TenPayV3CloseOrderRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, "商家订单号", TenPayV3Info.Key,nonceStr);
            var result = TenPayV3.CloseOrder(data);
            
            return Content(result.ResultXml);
        }

        /// <summary>
        /// 退款申请接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResult Refund()
        {
            try
            {
                WeixinTrace.SendCustomLog("进入退款流程", "1");

                string nonceStr = TenPayV3Util.GetNoncestr();

                string outTradeNo = HttpContext.Session.GetString("BillNo");

                WeixinTrace.SendCustomLog("进入退款流程", "2 outTradeNo：" + outTradeNo);
                
                string outRefundNo = "OutRefunNo-" + SystemTime.Now.Ticks;
                int totalFee = int.Parse(HttpContext.Session.GetString("BillFee"));
                int refundFee = totalFee;
                string opUserId = TenPayV3Info.MchId;
                var notifyUrl = "https://yufaquan.cn/wx/PayV3/RefundNotifyUrl";
                var dataInfo = new TenPayV3RefundRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, TenPayV3Info.Key,
                    null, nonceStr, null, outTradeNo, outRefundNo, totalFee, refundFee, opUserId, null, notifyUrl: notifyUrl);

                #region 旧方法
                //var cert = @"D:\cert\apiclient_cert_SenparcRobot.p12";//根据自己的证书位置修改
                //var password = TenPayV3Info.MchId;//默认为商户号，建议修改
                //var result = TenPayV3.Refund(dataInfo, cert, password);
                #endregion

                #region 新方法（Senparc.Weixin v6.4.4+）
                var result = TenPayV3.Refund(_serviceProvider, dataInfo);//证书地址、密码，在配置文件中设置，并在注册微信支付信息时自动记录
                #endregion

                WeixinTrace.SendCustomLog("进入退款流程", "3 Result：" + result.ToJson());

                return HttpResult.Success($"退款结果：{result.result_code} {result.err_code_des}。", null);
            }
            catch (Exception ex)
            {
                WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));

                throw;
            }


        }

        /// <summary>
        /// 退款通知地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RefundNotifyUrl()
        {
            WeixinTrace.SendCustomLog("RefundNotifyUrl被访问", "IP" + HttpContext.UserHostAddress()?.ToString());

            string responseCode = "FAIL";
            string responseMsg = "FAIL";
            try
            {
                ResponseHandler resHandler = new ResponseHandler(null);

                string return_code = resHandler.GetParameter("return_code");
                string return_msg = resHandler.GetParameter("return_msg");

                WeixinTrace.SendCustomLog("跟踪RefundNotifyUrl信息", resHandler.ParseXML());

                if (return_code == "SUCCESS")
                {
                    responseCode = "SUCCESS";
                    responseMsg = "OK";

                    string appId = resHandler.GetParameter("appid");
                    string mch_id = resHandler.GetParameter("mch_id");
                    string nonce_str = resHandler.GetParameter("nonce_str");
                    string req_info = resHandler.GetParameter("req_info");

                    var decodeReqInfo = TenPayV3Util.DecodeRefundReqInfo(req_info, TenPayV3Info.Key);
                    var decodeDoc = XDocument.Parse(decodeReqInfo);

                    //获取接口中需要用到的信息
                    string transaction_id = decodeDoc.Root.Element("transaction_id").Value;
                    string out_trade_no = decodeDoc.Root.Element("out_trade_no").Value;
                    string refund_id = decodeDoc.Root.Element("refund_id").Value;
                    string out_refund_no = decodeDoc.Root.Element("out_refund_no").Value;
                    int total_fee = int.Parse(decodeDoc.Root.Element("total_fee").Value);
                    int? settlement_total_fee = decodeDoc.Root.Element("settlement_total_fee") != null
                            ? int.Parse(decodeDoc.Root.Element("settlement_total_fee").Value)
                            : null as int?;
                    int refund_fee = int.Parse(decodeDoc.Root.Element("refund_fee").Value);
                    int tosettlement_refund_feetal_fee = int.Parse(decodeDoc.Root.Element("settlement_refund_fee").Value);
                    string refund_status = decodeDoc.Root.Element("refund_status").Value;
                    string success_time = decodeDoc.Root.Element("success_time").Value;
                    string refund_recv_accout = decodeDoc.Root.Element("refund_recv_accout").Value;
                    string refund_account = decodeDoc.Root.Element("refund_account").Value;
                    string refund_request_source = decodeDoc.Root.Element("refund_request_source").Value;


                    WeixinTrace.SendCustomLog("RefundNotifyUrl被访问", "验证通过");

                    //进行后续业务处理

                }
            }
            catch (Exception ex)
            {
                responseMsg = ex.Message;
                WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));
            }

            string xml = string.Format(@"<xml>
<return_code><![CDATA[{0}]]></return_code>
<return_msg><![CDATA[{1}]]></return_msg>
</xml>", responseCode, responseMsg);
            return Content(xml, "text/xml");
        }

        #endregion
    }


}
