﻿using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Web;
using StackExchange.Redis;

namespace NHibernate.Caches.Redis.Net45
{
    /// <summary>
    /// Allow NHibernate not to continue to timeout for every operation when Redis server is unavailable
    /// https://github.com/TheCloudlessSky/NHibernate.Caches.Redis
    /// </summary>
    public class NHibernateRedisCache : RedisCache
    {
        public const string SkipNHibernateCacheKey = "__SkipNHibernateCache__";

        public NHibernateRedisCache(string regionName, IDictionary<string, string> properties, RedisCacheElement element, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
            : base(regionName, properties, element, connectionMultiplexer, options)
        {

        }

        public override object Get(object key)
        {
            if (HasFailedForThisHttpRequest()) return null;
            return base.Get(key);
        }

        public override void Put(object key, object value)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Put(key, value);
        }

        public override void Remove(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Remove(key);
        }

        public override void Clear()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Clear();
        }

        public override void Destroy()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Destroy();
        }

        public override void Lock(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Lock(key);
        }

        public override void Unlock(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Unlock(key);
        }
        
        private bool HasFailedForThisHttpRequest()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Items.Contains(SkipNHibernateCacheKey);
            }
            else
            {
                return CallContext.GetData(SkipNHibernateCacheKey) != null;
            }
        }
    }

}
