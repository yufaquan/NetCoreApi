using Common;
using Common.Cache;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeChatRelated
{
    /// <summary>
    /// 微信的公用方法
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 存储网页授权token
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
       public static bool SetCache_OAuthAccessToken(object value)
        {
            if (string.IsNullOrWhiteSpace(Current.WxOpenId) || value==null)
            {
                return false;
            }
            return CacheService.GetCacheManager().Set(Current.WxOpenId+"_OAuthAccessToken", value, new TimeSpan(11, 50, 59), new TimeSpan(0));
        }
        /// <summary>
        /// 获取网页授权token
        /// </summary>
        /// <returns></returns>
        public static OAuthAccessTokenResult GetCache_OAuthAccessToken()
        {
            if (string.IsNullOrWhiteSpace(Current.WxOpenId))
            {
                return null;
            }
            return CacheService.GetCacheManager().Get<OAuthAccessTokenResult>(Current.WxOpenId + "_OAuthAccessToken");
        }
    }
}
