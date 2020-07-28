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
        private static string isOpenRedis="true";
        private static string redisConnectionString = "127.0.0.1:6379,password=123456";
        private static string mysqlConnectionStrng = "";

        public static string IsOpenRedis { get => isOpenRedis.ToLower(); set => isOpenRedis = value; }
        public static string RedisConnectionString { get => redisConnectionString; set => redisConnectionString = value; }
        public static string MysqlConnectionStrng { get => mysqlConnectionStrng; set => mysqlConnectionStrng = value; }
    }
}
