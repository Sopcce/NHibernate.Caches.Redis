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
    public class NetDataContractCacheSerializer : XmlRedisCacheSerializerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override XmlObjectSerializer CreateSerializer()
        {
            //  var serializer = new NetDataContractSerializer();
            var serializer = new DataContractSerializer(this.GetType());

            return serializer;
        }

    }
}
