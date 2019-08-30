using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>
     [TestClass]
    public class IntegrationTests : IntegrationTestBase
    {
        /// <summary>
        /// 实体缓存
        /// </summary>
         [TestMethod]
        public void Entity_cache()
        {
            using (var sf = CreateSessionFactory())
            {
                object personId = null;

                UsingSession(sf, session =>
                {
                    personId = session.Save(new Person("Foo", 1));

                    // Put occurs on the next fetch from the DB.
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCacheHitCount);
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCacheMissCount);
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCachePutCount);
                });

                sf.Statistics.Clear();

                UsingSession(sf, session =>
                {
                    session.Get<Person>(personId);
                    Assert.AreEqual(1, sf.Statistics.SecondLevelCacheMissCount);
                    Assert.AreEqual(1, sf.Statistics.SecondLevelCachePutCount);
                });

                sf.Statistics.Clear();

                UsingSession(sf, session =>
                {
                    session.Get<Person>(personId);
                    Assert.AreEqual(1, sf.Statistics.SecondLevelCacheHitCount);
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCacheMissCount);
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCachePutCount);
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
         [TestMethod]
        void SessionFactory_Dispose_should_not_clear_cache()
        {
            using (var sf = CreateSessionFactory())
            {
                UsingSession(sf, session =>
                {
                    session.Save(new Person("Foo", 10));
                });

                UsingSession(sf, session =>
                {
                    session.QueryOver<Person>()
                        .Cacheable()
                        .List();

                    Assert.AreEqual(1, sf.Statistics.QueryCacheMissCount);
                    Assert.AreEqual(1, sf.Statistics.SecondLevelCachePutCount);
                    Assert.AreEqual(1, sf.Statistics.QueryCachePutCount);
                });
            }

            using (var sf = CreateSessionFactory())
            {
                UsingSession(sf, session =>
                {
                    session.QueryOver<Person>()
                        .Cacheable()
                        .List();

                    Assert.AreEqual(1, sf.Statistics.SecondLevelCacheHitCount);
                    Assert.AreEqual(1, sf.Statistics.QueryCacheHitCount);
                    Assert.AreEqual(0, sf.Statistics.SecondLevelCachePutCount);
                    Assert.AreEqual(0, sf.Statistics.QueryCachePutCount);
                });
            }
        }
    }
}
