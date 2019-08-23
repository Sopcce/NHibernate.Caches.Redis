using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class ShouldRetryAcquireLockArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string RegionName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public object Key { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string LockKey { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string LockValue { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan LockTimeout { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan AcquireLockTimeout { get; private set; }

        internal ShouldRetryAcquireLockArgs(string regionName, object key, string lockKey, string lockValue, TimeSpan lockTimeout, TimeSpan acquireLockTimeout)
        {
            this.RegionName = regionName;
            this.Key = key;
            this.LockKey = lockKey;
            this.LockValue = lockValue;
            this.LockTimeout = lockTimeout;
            this.AcquireLockTimeout = acquireLockTimeout;
        }
    }
}
