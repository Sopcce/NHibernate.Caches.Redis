using System.Configuration;

namespace NHibernate.Caches.Redis.Net45
{
    public class RedisCacheProviderSection : ConfigurationSection
    {
        [ConfigurationProperty("caches", IsDefaultCollection = false)]
        public RedisCacheElementCollection Caches
        {
            get
            {
                return (RedisCacheElementCollection)base["caches"];
            }
        }
    }
}
