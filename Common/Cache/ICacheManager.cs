using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache
{
    /// <summary>
    /// Redis缓存接口
    /// </summary>
    public interface ICacheManager
    {

        //获取 Reids 缓存值
        string GetValue(string key);

        //获取值，并序列化
        TEntity Get<TEntity>(string key);

        //
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSliding">滑动过期时长</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        bool Set(string key, object value ,TimeSpan expiresSliding, TimeSpan expiressAbsoulte);

        //判断是否存在
        bool Exists(string key);

        //移除某一个缓存值
        void Remove(string key);

        //全部清除
        void Clear();
    }
}
