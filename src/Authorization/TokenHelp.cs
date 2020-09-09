using Authorization.Model;
using Common;
using Common.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            Current.VisitToKey = to;
            return MySecurity.SEncryptString(visit.ToJsonString(), Config.VisitTokenSY);
            //return SecurityHelp.Init.Encrypto(visit.ToJsonString());
        }

        /// <summary>
        /// 解析访问token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public static VisitToken ReadVisitTokenByTokenStr(string tokenStr)
        {
            //解密
            var token_json = MySecurity.SDecryptString(tokenStr, Config.VisitTokenSY);
            //var token_json = SecurityHelp.Init.Decrypto(tokenStr);
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
            if (visitToken.From!=_from || visitToken.ZZ!=_zz || Config.VisitTos.Count <= 0 || !Config.VisitTos.ContainsKey(visitToken.To))
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
            Current.VisitToKey = visitToken.To;
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
            var jmStr = MySecurity.SEncryptString(ut.ToJsonString(), Config.UserTokenSY);
            //var jmStr = SecurityHelp.Init.Encrypto(ut.ToJsonString());
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
        /// 延长UserToken的过期时间
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="jmStr">UserToken</param>
        public static void ExtensionTime(int userId,string jmStr)
        {
            //若存在 则覆盖
            ICacheManager cache = CacheService.GetCacheManager();
            var cacheKey = GetUserCacheKey(userId);
            if (cache.Exists(cacheKey))
            {
                cache.Remove(cacheKey);
            }
            cache.Set(cacheKey, jmStr, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
        }

        /// <summary>
        /// 延长UserToken的过期时间
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="jmStr">UserToken</param>
        public static async Task ExtensionTimeAsync(int userId, string jmStr)
        {
            await Task.Run(()=> {
                //若存在 则覆盖
                ICacheManager cache = CacheService.GetCacheManager();
                var cacheKey = GetUserCacheKey(userId);
                if (cache.Exists(cacheKey))
                {
                    cache.Remove(cacheKey);
                }
                cache.Set(cacheKey, jmStr, TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));
            });
        }

        /// <summary>
        /// 解析用户token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public static UserToken ReadUserTokenByTokenStr(string tokenStr)
        {
            var json = MySecurity.SDecryptString(tokenStr, Config.UserTokenSY);
            //var json = SecurityHelp.Init.Decrypto(tokenStr);
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
            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                errorMessage = "UserToken为空。";
                return false;
            }
            try
            {
                ut = ReadUserTokenByTokenStr(tokenStr);
            }
            catch (Exception ex)
            {
                errorMessage = "UserToken解析失败。";
                return false;
            }
            if (ut==null)
            {
                errorMessage = "UserToken解析失败。";
                return false;
            }
            //验证token签名
            if (ut.Sign!=Config.UserTokenSign)
            {
                errorMessage = "UserToken异常。";
                return false;
            }
            var cacheKey = GetUserCacheKey(ut.UserId);
            //获取缓存 判断是否存在以及是否刷新
            ICacheManager cache = CacheService.GetCacheManager();
            if (!cache.Exists(cacheKey))
            {
                errorMessage = "UserToken不存在或已到期。";
                return false;
            }
            var cacheValue = cache.GetValue(cacheKey);
            if (cacheValue!=tokenStr)
            {
                errorMessage = "UserToken已失效。";
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
            //加密
            return MySecurity.SEncryptString(key);
            //return SecurityHelp.Init.Encrypto(key);
        }
    }
}
