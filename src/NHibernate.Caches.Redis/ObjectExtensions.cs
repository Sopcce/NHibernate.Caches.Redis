using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// AcquireLockTimeout
    /// </summary>
    internal static class AcquireLockTimeout
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ThrowIfNull<T>(this T source)
            where T : class
        {
            if (source == null) throw new ArgumentNullException();
            return source;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static T ThrowIfNull<T>(this T source, string paramName)
        {
            if (source == null) throw new ArgumentNullException(paramName);
            return source;
        }
    }
}
