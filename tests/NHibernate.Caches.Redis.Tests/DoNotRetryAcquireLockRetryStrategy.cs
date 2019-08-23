using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class DoNotRetryAcquireLockRetryStrategy : IAcquireLockRetryStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ShouldRetryAcquireLock GetShouldRetry()
        {
            return e => false;
        }
    }
}
