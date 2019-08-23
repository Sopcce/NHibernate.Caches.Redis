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
    public class LockFailedEventArgs
    {
        /// <summary>
        /// RegionName
        /// </summary>
        public string RegionName { get; private set; }
        /// <summary>
        /// Key
        /// </summary>
        public object Key { get; private set; }
        /// <summary>
        /// LockKey
        /// </summary>
        public string LockKey { get; private set; }
        /// <summary>
        /// LockTimeout
        /// </summary>
        public TimeSpan LockTimeout { get; private set; }
        /// <summary>
        /// AcquireLockTimeout
        /// </summary>
        public TimeSpan AcquireLockTimeout { get; private set; }

        internal LockFailedEventArgs(string regionName, object key, string lockKey, TimeSpan lockTimeout, TimeSpan acquireLockTimeout)
        {
            this.RegionName = regionName;
            this.Key = key;
            this.LockKey = lockKey;
            this.LockTimeout = lockTimeout;
            this.AcquireLockTimeout = acquireLockTimeout;
        }
    }
}
