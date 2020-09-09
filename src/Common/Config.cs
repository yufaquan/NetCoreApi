using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 系统配置类
    /// </summary>
    public static class Config
    {
        private static string isOpenRedis = "true";
        private static string redisConnectionString = "127.0.0.1:6379,password=123456";
        private static string mysqlConnectionStrng = "Data Source=yufaquan.cn;Port=3306;Database=netcoreapi;Uid=yufaquan;Pwd=passWord@Yfq;Allow Zero Datetime=false;charset=utf8;pooling=true;";
        private static JWTInfo jwtInfo = new JWTInfo() { Issuer = "YFQ", SigningKey = "YFQAPI's Secret Key" };
        private static string visitTokenSY = "yufaquanzhendehenshuaia";
        private static string userTokenSY = "yufaquanqueshihenshuaine";
        private static string userTokenSign = "yufquanzhendeshichaojideshuaihaoma.shuidoubibuliaolezhende.";

        /// <summary>
        /// 是否使用Redis
        /// </summary>
        public static string IsOpenRedis { get => isOpenRedis.ToLower(); set => isOpenRedis = value; }
        /// <summary>
        /// Redis链接字符串
        /// </summary>
        public static string RedisConnectionString { get => redisConnectionString; set => redisConnectionString = value; }
        /// <summary>
        /// MySql链接字符串
        /// </summary>
        public static string MysqlConnectionStrng { get => mysqlConnectionStrng; set => mysqlConnectionStrng = value; }
        public static JWTInfo JWTInfo { get => jwtInfo; set => jwtInfo = value; }
        /// <summary>
        /// 访问Token的mc
        /// </summary>
        public static string TokenName { get => "yvanToken"; }
        /// <summary>
        /// 访问Token的秘钥
        /// </summary>
        public static string VisitTokenSY { get => visitTokenSY; set => visitTokenSY = value; }
        /// <summary>
        /// 用户Token的秘钥
        /// </summary>
        public static string UserTokenSY { get => userTokenSY; set => userTokenSY = value; }
        /// <summary>
        /// 用户Token的签名
        /// </summary>
        public static string UserTokenSign { get => userTokenSign; set => userTokenSign = value; }
        /// <summary>
        /// 加密解密的默认密钥
        /// </summary>
        public static string SecuritySign { get => "Yvan_YuFaquan_1998_ACE"; }

        /// <summary>
        /// 可访问访问密钥
        /// </summary>
        public static Dictionary<string,string> VisitTos { get; set; }
    }

    public class JWTInfo
    {
        public string Issuer { get; set; }
        public string SigningKey { get; set; }
    }
}
