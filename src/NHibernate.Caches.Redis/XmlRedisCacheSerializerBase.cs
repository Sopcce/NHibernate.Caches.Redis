using System.IO;
using System.Runtime.Serialization;
using StackExchange.Redis;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class XmlRedisCacheSerializerBase : ICacheSerializer
    {
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public RedisValue Serialize(object value)
        {
            var serializer = CreateSerializer();
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                serializer.WriteObject(stream, value);
                stream.Position = 0;
                var result = reader.ReadToEnd();
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Deserialize(RedisValue value)
        {
            if (value.IsNull) return null;

            var serializer = CreateSerializer();
            using (var stream = new MemoryStream())
            {
                byte[] data = value;
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var result = serializer.ReadObject(stream);
                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract XmlObjectSerializer CreateSerializer();
    }
}