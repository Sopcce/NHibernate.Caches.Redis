using StackExchange.Redis;

namespace NHibernate.Caches.Redis.Net45
{
    public interface ICacheSerializer
    {
        RedisValue Serialize(object value);
        object Deserialize(RedisValue value);
    }
}