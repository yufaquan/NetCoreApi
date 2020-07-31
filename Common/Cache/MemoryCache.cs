using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache
{
    public class MyMemoryCache:ICacheManager
    {
        private readonly MemoryCache _cache;

        public MyMemoryCache()
        {
            this._cache =new MemoryCache(new MemoryCacheOptions());;
        }

        public void Clear()
        {
            
        }

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            object cached;
            return _cache.TryGetValue(key, out cached);
        }

        public TEntity Get<TEntity>(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _cache.Get<string>(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _cache.Set(key, value,
                    new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte)
                    );

            return Exists(key);
        }


    }
}
