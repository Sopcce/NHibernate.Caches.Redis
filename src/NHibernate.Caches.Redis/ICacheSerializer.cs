using StackExchange.Redis;
namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        RedisValue Serialize(object value);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object Deserialize(RedisValue value);
    }
}