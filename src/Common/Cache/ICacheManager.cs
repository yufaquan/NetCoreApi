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

        /// <summary>
        /// 获取 Reids 缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 获取值，并序列化
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true：存在</returns>
        bool Exists(string key);

        /// <summary>
        /// 移除某一个缓存值
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// 全部清除
        /// </summary>
        void Clear();
    }
}
