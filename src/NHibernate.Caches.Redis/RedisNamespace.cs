using System;
using System.Threading;
namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisNamespace
    {
        private readonly string prefix;
        private readonly string _setOfActiveKeysKey;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        public RedisNamespace(string prefix)
        {
            this.prefix = prefix;
            this._setOfActiveKeysKey = prefix + ":keys";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetSetOfActiveKeysKey()
        {
            return _setOfActiveKeysKey;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKey(object key)
        {
            return prefix + ":" + key;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetLockKey(object key)
        {
            return GetKey(key) + ":lock";
        }

      
    }
}