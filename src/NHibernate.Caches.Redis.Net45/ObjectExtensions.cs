using System;

namespace NHibernate.Caches.Redis.Net45
{
    internal static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T source)
            where T : class
        {
            if (source == null) throw new ArgumentNullException();
            return source;
        }

        public static T ThrowIfNull<T>(this T source, string paramName)
        {
            if (source == null) throw new ArgumentNullException(paramName);
            return source;
        }
    }
}
