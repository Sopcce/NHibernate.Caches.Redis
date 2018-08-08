using System;
using System.Runtime.Serialization;

namespace NHibernate.Caches.Redis.Net45
{
    [Serializable]
    public class RedisCacheGenerationException : Exception
    {
        public RedisCacheGenerationException()
        {

        }

        public RedisCacheGenerationException(string message)
            : base(message)
        {

        }

        public RedisCacheGenerationException(string message, Exception inner)
            : base(message, inner)
        {

        }

        protected RedisCacheGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
