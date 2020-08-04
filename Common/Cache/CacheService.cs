using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache
{
    public class CacheService
    {
        /// <summary>
        /// 获取缓存服务
        /// </summary>
        /// <returns></returns>
        public static ICacheManager GetCacheManager()
        {
            if (Config.IsOpenRedis.ToLower()=="true")
            {
                return new RedisCacheManager();
            }
            else
            {
                return new MyMemoryCache();
            }
        }
    }
}
