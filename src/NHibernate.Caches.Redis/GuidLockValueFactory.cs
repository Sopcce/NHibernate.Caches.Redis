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
    public class GuidLockValueFactory : ILockValueFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLockValue()
        {
            return "lock-" + Guid.NewGuid();            
        }
    }
}
