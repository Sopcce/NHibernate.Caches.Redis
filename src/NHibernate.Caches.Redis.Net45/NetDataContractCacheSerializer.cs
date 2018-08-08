using System.Runtime.Serialization;

namespace NHibernate.Caches.Redis.Net45
{
    public class NetDataContractCacheSerializer : XmlRedisCacheSerializerBase
    {
        protected override XmlObjectSerializer CreateSerializer()
        {
            var serializer = new NetDataContractSerializer();
            return serializer;
        }
    }
}
