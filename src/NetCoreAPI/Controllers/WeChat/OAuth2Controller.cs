using System;
using System.Linq;
using Bussiness;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using WeChatRelated.MP;

namespace NetCoreAPI.Controllers.WeChat
{
    /// <summary>
    /// 微信网页授权
    /// </summary>
    [ApiController]
    public class OAuth2Controller : WeChatApiBase
    {
        private string appid = Config.SenparcWeixinSetting.WeixinAppId;
        private string appSecret= Config.SenparcWeixinSetting.WeixinAppSecret;

        /// <summary>
        /// 获取微信用户信息
        /// OAuthScope.snsapi_userinfo
        /// 授权回调填写前端地址，此处用code换取用户信息即可
        /// </summary>
        /// <param name="code"></param>
        /// <returns>HttpResult</returns>
        [HttpGet]
        public HttpResult GetWeChatUserInfo(string code)
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(code))
            {
                errorMessage = "您拒绝了授权！";
                return HttpResult.WeChatError(errorMessage, null);
            }

            //if (state != HttpContext.Session.GetString("State"))
            //{
            //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
            //    //建议用完之后就清空，将其一次性使用
            //    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
            //    errorMessage = "验证失败！请从正规途径进入！";
            //    return HttpResult.WeChatError(errorMessage, null);
            //}

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(appid, appSecret, code);
            }
            catch (Exception ex)
            {
                return HttpResult.WeChatError("授权失败,"+ex.Message, null);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return HttpResult.WeChatError("错误：" + result.errmsg, null);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            //HttpContext.Session.SetString("OAuthAccessTokenStartTime", SystemTime.Now.ToString());
            //HttpContext.Session.SetString("OAuthAccessToken", result.ToJson());
            WeChatRelated.Common.SetCache_OAuthAccessToken(result);

            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            try
            {
                OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
                return HttpResult.Success(userInfo);
            }
            catch (ErrorJsonResultException ex)
            {
                return HttpResult.WeChatError("授权失败," + ex.Message, null);
            }
        }

        /// <summary>
        /// 获取系统用户信息
        /// OAuthScope.snsapi_userinfo
        /// 会根据openid自动创建用户信息，如已存在则只会更新昵称、性别和头像
        /// 授权回调填写前端地址，此处用code换取用户信息即可
        /// </summary>
        /// <param name="code"></param>
        /// <returns>HttpResult</returns>
        [HttpGet]
        public HttpResult GetUserInfo(string code)
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(code))
            {
                errorMessage = "您拒绝了授权！";
                return HttpResult.WeChatError(errorMessage, null);
            }

            //if (state != HttpContext.Session.GetString("State"))
            //{
            //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
            //    //建议用完之后就清空，将其一次性使用
            //    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
            //    errorMessage = "验证失败！请从正规途径进入！";
            //    return HttpResult.WeChatError(errorMessage, null);
            //}

            OAuthAccessTokenResult result = null;

            //通过，用code换取access_token
            try
            {
                result = OAuthApi.GetAccessToken(appid, appSecret, code);
            }
            catch (Exception ex)
            {
                return HttpResult.WeChatError("授权失败," + ex.Message, null);
            }
            if (result.errcode != ReturnCode.请求成功)
            {
                return HttpResult.WeChatError("错误：" + result.errmsg, null);
            }
            //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
            //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
            //HttpContext.Session.SetString("OAuthAccessTokenStartTime", SystemTime.Now.ToString());
            //HttpContext.Session.SetString("OAuthAccessToken", result.ToJson());
            WeChatRelated.Common.SetCache_OAuthAccessToken(result);

            //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
            OAuthUserInfo userInfo = null;
            try
            {
                userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);
            }
            catch (ErrorJsonResultException ex)
            {
                return HttpResult.WeChatError("获取用户信息失败," + ex.Message, null);
            }
            var user= new UserService().InsertOrUpdateByWXMPUserInfo(userInfo);
            if (user!=null)
            {
                return HttpResult.Success(user);
            }
            else
            {
                return HttpResult.WeChatError("获取失败，请稍候再试。" , null);
            }
        }


    }
}
