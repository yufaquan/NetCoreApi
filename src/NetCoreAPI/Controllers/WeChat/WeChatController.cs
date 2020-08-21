using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Request;

namespace NetCoreAPI.Controllers.WeChat
{
    /// <summary>
    /// 微信
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class WeChatController : ControllerBase
    {
        public static readonly string Token = Config.SenparcWeixinSetting.Token;//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey;//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = Config.SenparcWeixinSetting.WeixinAppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string Secret = Config.SenparcWeixinSetting.WeixinAppSecret;//与微信公众账号后台的Secret设置保持一致，区分大小写。

        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://base.api.yufaquan.cn/WeChat/Get
        /// </summary>
        /// <param name="signature">微信加密签名，signature结合了开发者填写的token参数和请求中的timestamp参数、nonce参数。</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <param name="echostr">随机字符串</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Get(string signature, string timestamp, string nonce,  string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }


        /// <summary>
        /// 微信后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://base.api.yufaquan.cn/WeChat/Post
        /// </summary>
        /// <param name="postModel">微信公众服务器Post过来的加密参数集合</param>
        /// <param name="echostr">随机字符串</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Post(PostModel postModel, string echostr)
        {
            if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + postModel.Signature + "," + CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
                    "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
        }

        /// <summary>
        /// 获取微信接口通用token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetToken()
        {
            return new JsonResult(HttpResult.Success(new { Token = CommonApi.GetToken(AppId, Secret) }));
        }


        /// <summary>
        /// 获取微信接口通用token （每次调用都会获取一个不同的access_token）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCacheToken()
        {
            return new JsonResult(HttpResult.Success(new { Token = CommonApi.GetToken(AppId, Secret) }));
        }


        /// <summary>
        /// 直接通过appid获取菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMenu()
        {
            return new JsonResult(HttpResult.Success(new { Menu = CommonApi.GetMenu(AppId) }));
        }

    }
}
