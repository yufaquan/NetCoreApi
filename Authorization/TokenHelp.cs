using Authorization.Model;
using Common;
using Common.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authorization
{
    public class TokenHelp
    {
        private const string _from = "yvanApi";
        private const string _zz = "yufaquan";
        /// <summary>
        /// 生成访问token
        /// </summary>
        /// <param name="to">给谁</param>
        /// <returns></returns>
        public static string WriteVisitToken(string to = "everyOne")
        {
            var visit = new VisitToken() { ETime=TimeSpan.FromHours(12), From= _from, STime=DateTime.Now, To= to, ZZ= _zz };
            return CryptoHelper.Encrypt(visit.ToJsonString(), Config.VisitTokenSY);
        }

        /// <summary>
        /// 解析访问token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public static VisitToken ReadVisitTokenByTokenStr(string tokenStr)
        {
            //解密
            var token_json = CryptoHelper.Decrypt(tokenStr, Config.VisitTokenSY);
            return token_json.JsonToModel<VisitToken>();
        }

        /// <summary>
        /// 校验访问token
        /// </summary>
        /// <param name="tokenStr">token字符串</param>
        /// <param name="verifyTime">是否校验时间</param>
        /// <param name="errorMessage">校验未通过时的错误信息</param>
        /// <returns>true：校验通过；false：校验未通过。</returns>
        public static bool VerifyVisitToken(string tokenStr,bool verifyTime,out string errorMessage)
        {
            VisitToken visitToken ;
            try
            {
                visitToken = ReadVisitTokenByTokenStr(tokenStr);
            }
            catch (Exception ex)
            {
                errorMessage = "Token解析失败。";
                return false;
            }
            if (visitToken == null)
            {
                errorMessage = "Token解析失败。";
                return false;
            }
            if (visitToken.From!=_from || visitToken.ZZ!=_zz)
            {
                errorMessage = "非法访问。"; 
                return false;
            }
            if (verifyTime)
            {
                //校验时间
                var dt = DateTime.Now;
                var sjc = dt - visitToken.STime;
                if (sjc > visitToken.ETime)
                {
                    errorMessage = "token已过期。";
                    return false;
                }
            }
            errorMessage = string.Empty;
            return true;
        }

        /// <summary>
        /// 生成用户token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string WriteUserToken(int userId)
        {
            var ut = new UserToken() { ITime = DateTime.Now, UserId = userId,Sign=Config.UserTokenSign };
            ICacheManager cache = CacheService.GetCacheManager();
            var jmStr= CryptoHelper.Encrypt(ut.ToJsonString(), Config.UserTokenSY);
            //判断是否已存在缓存
            //若存在 则覆盖
            var cacheKey = GetUserCacheKey(userId);
            if (cache.Exists(cacheKey))
            {
                cache.Remove(cacheKey);
            }
            cache.Set(cacheKey, jmStr, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
            return jmStr;
        }

        /// <summary>
        /// 解析用户token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public static UserToken ReadUserTokenByTokenStr(string tokenStr)
        {
            var json = CryptoHelper.Decrypt(tokenStr, Config.UserTokenSY);
            return json.JsonToModel<UserToken>();
        }

        /// <summary>
        /// 校验用户Token
        /// </summary>
        /// <param name="tokenStr">用户token</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true：校验通过；false：校验未通过。</returns>
        public static bool VerifyUserToken(string tokenStr,out int? userId,out string errorMessage)
        {
            UserToken ut ;
            userId = null;
            try
            {
                ut = ReadUserTokenByTokenStr(tokenStr);
            }
            catch (Exception ex)
            {
                errorMessage = "userToken解析失败。";
                return false;
            }
            if (ut==null)
            {
                errorMessage = "userToken解析失败。";
                return false;
            }
            //验证token签名
            if (ut.Sign!=Config.UserTokenSign)
            {
                errorMessage = "userToken异常。";
                return false;
            }
            var cacheKey = GetUserCacheKey(ut.UserId);
            //获取缓存 判断是否存在以及是否刷新
            ICacheManager cache = CacheService.GetCacheManager();
            if (!cache.Exists(cacheKey))
            {
                errorMessage = "userToken不存在或已到期。";
                return false;
            }
            var cacheValue = cache.GetValue(cacheKey);
            if (cacheValue!=tokenStr)
            {
                errorMessage = "userToken已失效。";
                return false;
            }
            errorMessage = string.Empty;
            userId = ut.UserId;
            return true;
        }

        /// <summary>
        /// 获取用户缓存key
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static string GetUserCacheKey(int userId)
        {
            string key = new { id = userId, key = Config.UserTokenSY }.ToJsonString();
            return CryptoHelper.Encrypt(key, Config.UserTokenSY);
        }
    }
}
