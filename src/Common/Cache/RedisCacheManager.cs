using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Cache
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly string redisConnenctionString;
        public volatile ConnectionMultiplexer redisConnection;
        private readonly object redisConnectionLock = new object();
        public RedisCacheManager()
        {
            string redisConfiguration = Config.RedisConnectionString;//获取连接字符串

            if (string.IsNullOrWhiteSpace(redisConfiguration))
            {
                throw new ArgumentException("redis config is empty", nameof(redisConfiguration));
            }
            this.redisConnenctionString = redisConfiguration;
            this.redisConnection = GetRedisConnection();
        }

        /// <summary>
        /// 核心代码，获取连接实例
        /// 通过双if 夹lock的方式，实现单例模式
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetRedisConnection()
        {
            //如果已经连接实例，直接返回
            if (this.redisConnection != null && this.redisConnection.IsConnected)
            {
                return this.redisConnection;
            }
            //加锁，防止异步编程中，出现单例无效的问题
            lock (redisConnectionLock)
            {
                if (this.redisConnection != null)
                {
                    //释放redis连接
                    this.redisConnection.Dispose();
                }
                try
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        AllowAdmin = true,
                        ConnectTimeout = 15000,
                        SyncTimeout = 5000,
                        //ResponseTimeout = 15000,
                        Password = Config.RedisPwd,//Redis数据库密码
                        EndPoints = { Config.RedisHost }// connectionString 为IP:Port 如”192.168.2.110:6379”
                    };
                    //this.redisConnection = ConnectionMultiplexer.Connect(redisConnenctionString);
                    this.redisConnection = ConnectionMultiplexer.Connect(config);
                }
                catch (Exception)
                {
                    throw new Exception("Redis服务未启用，请开启该服务");
                }
            }
            return this.redisConnection;
        }
 
        public void Clear()
        {
            foreach (var endPoint in this.GetRedisConnection().GetEndPoints())
            {
                var server = this.GetRedisConnection().GetServer(endPoint);
                foreach (var key in server.Keys())
                {
                    redisConnection.GetDatabase().KeyDelete(key);
                }
            }
        }

        public bool Exists(string key)
        {
            return redisConnection.GetDatabase().KeyExists(key);
        }

        public string GetValue(string key)
        {
            return Get<string>(key);
        }

        public TEntity Get<TEntity>(string key)
        {
            var value = redisConnection.GetDatabase().StringGet(key);
            if (value.HasValue)
            {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<TEntity>(value);
            }
            else
            {
                return default(TEntity);
            }
        }

        public bool Remove(string key)
        {
            return redisConnection.GetDatabase().KeyDelete(key);
        }

        public bool Set(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            if (value != null)
            {
                //序列化，将object值生成RedisValue
                return redisConnection.GetDatabase().StringSet(key, SerializeHelper.Serialize(value),expiressAbsoulte);
            }
            return false;
        }

        public bool SetValue(string key, byte[] value)
        {
            return redisConnection.GetDatabase().StringSet(key, value, TimeSpan.FromSeconds(120));
        }

    }
}
