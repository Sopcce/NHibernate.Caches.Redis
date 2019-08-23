using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;


namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// Allow NHibernate not to continue to timeout for every operation when Redis server is unavailable
    /// https://github.com/TheCloudlessSky/NHibernate.Caches.Redis
    /// redis 不可用是记录数据到内存中
    /// </summary>
    public class RequestRecoveryRedisCache : RedisCache
    {
        public const string SkipNHibernateCacheKey = "__SkipNHibernateCache__";



        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="properties"></param>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="options"></param>
        public RequestRecoveryRedisCache(RedisCacheConfiguration configuration,
            IDictionary<string, string> properties,
            ConnectionMultiplexer connectionMultiplexer,
            RedisCacheProviderOptions options)
            : base(configuration, connectionMultiplexer, options)
        {
            AsyncLocal<HttpContext> _httpContextCurrent = new AsyncLocal<HttpContext>();
            _httpContextCurrent.Value.Items[RequestRecoveryRedisCache.SkipNHibernateCacheKey] = true;
        }



        public  object Get(object key)
        {
            if (HasFailedForThisHttpRequest()) return null;
            return base.Get(key);
        }

        public  void Put(object key, object value)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Put(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public  void Remove(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Remove(key);
        }

        public  void Clear()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Clear();
        }

        public  void Destroy()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Destroy();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public  void Lock(object key)
        {
             base.Lock(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockValue"></param>
        public  void Unlock(object key, object lockValue)
        {
            throw new NotImplementedException();
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        //public void Unlock(object key)
        //{
        //    base.Unlock(key);
        //}

        /// <summary>
        /// 
        /// </summary>
        public  string RegionName { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        private bool HasFailedForThisHttpRequest()
        {
            //if (HttpContext.Current != null)
            //{
            //    return HttpContext.Current.Items.Contains(SkipNHibernateCacheKey);
            //}
            //else
            //{
            //    return CallContext.GetData(SkipNHibernateCacheKey) != null;
            //}

            return true;


        }
    }


}