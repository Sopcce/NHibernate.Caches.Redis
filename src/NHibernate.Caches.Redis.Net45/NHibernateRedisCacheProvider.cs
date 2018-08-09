using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Web;
using StackExchange.Redis;

namespace NHibernate.Caches.Redis.Net45
{
    public class NHibernateRedisCacheProvider : RedisCacheProvider
    {
        protected override RedisCache BuildCache(string regionName, IDictionary<string, string> properties, RedisCacheElement configElement, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
        {
            options.OnException = (e) =>
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[NHibernateRedisCache.SkipNHibernateCacheKey] = true;
                }
                else
                {
                    CallContext.SetData(NHibernateRedisCache.SkipNHibernateCacheKey, true);
                }

            };

            return new NHibernateRedisCache(regionName, properties, configElement, connectionMultiplexer, options);
        }
    }
}
