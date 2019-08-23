using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RedisCacheException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public string RegionName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RedisCacheException()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RedisCacheException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public RedisCacheException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RedisCacheException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedisCacheException(string regionName, string message, Exception innerException)
            : this(message, innerException)
        {
            this.RegionName = regionName;
        }
    }
}
