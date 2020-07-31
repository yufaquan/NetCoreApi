using Authorization.Model;
using Common;
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
            var visit = new VisitTokenModel() { ETime=TimeSpan.FromHours(12), From= _from, STime=DateTime.Now, To= to, ZZ= _zz };
            return CryptoHelper.Encrypt(visit.ToJsonString(), Config.VisitTokenSY);
        }

        /// <summary>
        /// 解析访问token
        /// </summary>
        /// <param name="tokenStr"></param>
        /// <returns></returns>
        public static VisitTokenModel ReadVisitTokenByTokenStr(string tokenStr)
        {
            return tokenStr.JsonToModel<VisitTokenModel>();
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
            var visitToken = ReadVisitTokenByTokenStr(tokenStr);
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
    }
}
