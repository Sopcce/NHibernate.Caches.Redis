using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisCacheProviderRequests : RedisCacheProvider
    {


        /// <summary>
        /// 到redis 不可用是，处理redis 请求
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="properties"></param>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override RedisCache BuildCache(RedisCacheConfiguration configuration,
          IDictionary<string, string> properties,
          ConnectionMultiplexer connectionMultiplexer,
          RedisCacheProviderOptions options)
        {
            //options.OnException = (e) =>
            //{
            //  if (HttpContext.Current != null)
            //  {
            //    HttpContext.Current.Items[RequestRecoveryRedisCache.SkipNHibernateCacheKey] = true;
            //  }
            //  else
            //  {
            //    CallContext.SetData(RequestRecoveryRedisCache.SkipNHibernateCacheKey, true);
            //  }

            //};
            //var evtArg = new ExceptionEventArgs(configuration.RegionName, RedisCacheMethod.Clear, e);
            //options.OnException(this, evtArg);
            options.OnException(null, new ExceptionEventArgs(configuration.RegionName, RedisCacheMethod.Unknown, new Exception()));

            return new RequestRecoveryRedisCache(configuration, properties, connectionMultiplexer, options);
        }

    }
}
