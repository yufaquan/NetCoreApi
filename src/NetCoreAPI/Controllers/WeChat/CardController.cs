using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using Senparc.Weixin;

namespace NetCoreAPI.Controllers.WeChat
{
    /// <summary>
    /// 卡券
    /// </summary>
    [ApiController]
    public class CardController : WeChatApiBase
    {
        public static readonly string AppId = Config.SenparcWeixinSetting.WeixinAppId;

        /// <summary>
        /// 获取卡券列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Get()
        {
            var reslut= CardApi.CardBatchGet(AppId, 0, 50,new List<string>() {"CARD_STATUS_NOT_VERIFY", "CARD_STATUS_VERIFY_OK" });
            return new JsonResult(HttpResult.Success(reslut));
        }
        /// <summary>
        /// 创建卡券
        /// </summary>
        /// <param name="info">卡券信息(yitianwan)</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create([FromBody] Card_CashData info)
        {
            //Card_CashData info = new Card_CashData();
            info.least_cost = 100000;
            info.reduce_cost = 100000;
            info.base_info = new Card_BaseInfoBase();
            info.base_info.brand_name = "测试商户名称";
            info.base_info.can_share = false;
            info.base_info.code_type = Senparc.Weixin.MP.Card_CodeType.CODE_TYPE_QRCODE;
            info.base_info.color = "Color040";
            //info.base_info.custom_url = "https://yufaquan.cn";
            //info.base_info.custom_url_name = "自定义跳转地址测试";
            //info.base_info.custom_url_sub_title = "点击进入自定义跳转地址";
            info.base_info.date_info = new Card_BaseInfo_DateInfo();
            info.base_info.date_info.fixed_begin_term = 0;
            info.base_info.date_info.fixed_term = 7;
            info.base_info.date_info.type = "2";
            info.base_info.description = "长文本描述\r\n可以换行。最长一千字。";
            info.base_info.get_limit = 1;
            //info.base_info.location_id_list=
            info.base_info.logo_url = "http://mmbiz.qpic.cn/mmbiz/iaL1LJM1mF9aRKPZJkmG8xXhiaHqkKSVMMWeN3hLut7X7hicFNjakmxibMLGWpXrEXB33367o7zHN0CwngnQY7zb7g/0";
            info.base_info.notice = "此处为使用提醒消费时从这付款";
            info.base_info.sku = new Card_BaseInfo_Sku();
            info.base_info.sku.quantity=100;
            info.base_info.sku.total_quantity = 100;
            info.base_info.sub_title = "券名副标题上限18字";
            info.base_info.title = "券名1000元代金券";
            info.base_info.use_all_locations = true;
            info.base_info.use_limit = 1;
            //info.base_info.promotion_url = "https://yufaquan.cn";
            //info.base_info.promotion_url_name = "自定义外链跳转地址测试";
            //info.base_info.promotion_url_sub_title = "点击进入自定义跳转外链地址";
            
            var result = CardApi.CreateCard(AppId, info);
            return new JsonResult(HttpResult.Success(result));
        }
    }
}
