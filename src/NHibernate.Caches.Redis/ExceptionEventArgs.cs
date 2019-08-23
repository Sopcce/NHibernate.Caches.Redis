using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string RegionName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public RedisCacheMethod Method { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Throw { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="method"></param>
        /// <param name="exception"></param>
        public ExceptionEventArgs(string regionName, RedisCacheMethod method, Exception exception)
        {
            this.RegionName = regionName;
            this.Exception = exception;
            this.Method = method;
            this.Throw = false;
        }

   
  }
}
