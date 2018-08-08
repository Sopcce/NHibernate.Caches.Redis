using System;

namespace NHibernate.Caches.Redis.Net45
{
    public class RedisCacheExceptionEventArgs
    {
        public Exception Exception { get; private set; }
        public bool Throw { get; set; }

        public RedisCacheExceptionEventArgs(Exception exception)
        {
            this.Exception = exception;
            this.Throw = false;
        }
    }
}
