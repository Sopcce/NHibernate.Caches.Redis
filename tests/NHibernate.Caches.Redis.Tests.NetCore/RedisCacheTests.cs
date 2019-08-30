using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
 

namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>

    [TestClass]
    public class RedisCacheTests : RedisTest
    {
        private readonly RedisCacheProviderOptions options;
        /// <summary>
        /// 
        /// </summary>
        public RedisCacheTests()
        {
            options = CreateTestProviderOptions();
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Configure_cache_expiration()
        {
            var configuration = new RedisCacheConfiguration("region") { Expiration = TimeSpan.FromMinutes(99) };
            var sut = new RedisCache(configuration, ConnectionMultiplexer, options);

            sut.Put(999, new Person("Foo", 10));

            var cacheKey = sut.CacheNamespace.GetKey(999);
            var expiry = Redis.KeyTimeToLive(cacheKey);
            //Assert.That(expiry.Value, Is.InRange(TimeSpan.FromMinutes(98), TimeSpan.FromMinutes(99)));
        }

         [TestMethod]
        void Configure_cache_lock_timeout()
        {
            var configuration = new RedisCacheConfiguration("region") { LockTimeout = TimeSpan.FromSeconds(123) };
            var sut = new RedisCache(configuration, ConnectionMultiplexer, options);
            const string key = "123";

            sut.Lock(key);
            var lockKey = sut.CacheNamespace.GetLockKey(key);

            var expiry = Redis.KeyTimeToLive(lockKey);
            //Assert.InRange(expiry.Value, low: TimeSpan.FromSeconds(120), high: TimeSpan.FromSeconds(123));
            //Assert.That(expiry.Value, Is.InRange(TimeSpan.FromSeconds(120), TimeSpan.FromSeconds(123)));

        }
        /// <summary>
        /// 
        /// </summary>
       [TestMethod]
        void Put_adds_the_item_to_the_cache()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);

            sut.Put(999, new Person("Foo", 10));

            var cacheKey = sut.CacheNamespace.GetKey(999);
            var data = Redis.StringGet(cacheKey);
            var person = (Person)options.Serializer.Deserialize(data);
            Assert.AreEqual("Foo", person.Name);
            Assert.AreEqual(10, person.Age);
        }

       [TestMethod]
        void Put_sets_an_expiration_on_the_item()
        {
            var config = new RedisCacheConfiguration("region") { Expiration = TimeSpan.FromSeconds(30) };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);

            sut.Put(999, new Person("Foo", 10));

            var cacheKey = sut.CacheNamespace.GetKey(999);
            var ttl = Redis.KeyTimeToLive(cacheKey);
            //Assert.InRange(ttl.Value, TimeSpan.FromSeconds(29), TimeSpan.FromSeconds(30));
            //Assert.That(ttl.Value, Is.InRange(TimeSpan.FromSeconds(29), TimeSpan.FromSeconds(30)));
        }

       [TestMethod]
        void Get_should_deserialize_data()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            sut.Put(999, new Person("Foo", 10));

            var person = sut.Get(999) as Person;

            Assert.IsNotNull(person);
            Assert.AreEqual("Foo", person.Name);
            Assert.AreEqual(10, person.Age);
        }
        /// <summary>
        /// Make sure key reference is removed from :keys via GET after expiry
        /// TODO:Add
        /// </summary>
       [TestMethod]
        void Get_for_expired_item_removes_it_from_keys()
        {
            var config = new RedisCacheConfiguration("region")
            {
                Expiration = TimeSpan.FromMilliseconds(100),
                SlidingExpiration = RedisCacheConfiguration.NoSlidingExpiration
            };
            var firstKey = 1;
            var secondKey = 2;
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            var setOfActiveKeysKey = sut.CacheNamespace.GetSetOfActiveKeysKey();
            var firstCacheKey = sut.CacheNamespace.GetKey(firstKey);
            var secondCacheKey = sut.CacheNamespace.GetKey(secondKey);
            sut.Put(firstKey, new Person("John Doe", 10));
            Assert.IsTrue(GetDatabase().SetContains(setOfActiveKeysKey, firstCacheKey));//first key reference exists in :keys
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            sut.Put(secondKey, new Person("John Doe The Second", 12));
            Assert.IsNull(sut.Get(firstKey));//this should trigger deletion of first key reference from :keys
            Assert.IsFalse(GetDatabase().SetContains(setOfActiveKeysKey, firstCacheKey));//first key reference is removed from :keys
            Assert.IsTrue(GetDatabase().SetContains(setOfActiveKeysKey, secondCacheKey));//first key reference exists in :keys
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Assert.IsNull(sut.Get(secondKey));//this should trigger deletion of second key reference from :keys
            Assert.IsFalse(GetDatabase().SetContains(setOfActiveKeysKey, secondCacheKey));//second key reference is removed from :keys
        }
       [TestMethod]
        void Get_should_return_null_if_not_exists()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);

            var person = sut.Get(99999) as Person;

            Assert.IsNull(person);
        }

       [TestMethod]
        void Get_after_cache_has_been_cleared_returns_null()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);

            sut.Put(999, new Person("John Doe", 20));
            sut.Clear();
            var result = sut.Get(999);

            Assert.IsNull(result);
        }

       [TestMethod]
        void Get_after_item_has_expired_returns_null()
        {
            var config = new RedisCacheConfiguration("region") { Expiration = TimeSpan.FromMilliseconds(500) };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            sut.Put(1, new Person("John Doe", 20));

            Thread.Sleep(TimeSpan.FromMilliseconds(600));
            var result = sut.Get(1);

            Assert.IsNull(result);
        }

       [TestMethod]
        void Get_after_item_has_expired_removes_the_key_from_set_of_all_keys()
        {
            const int key = 1;
            var config = new RedisCacheConfiguration("region")
            {
                Expiration = TimeSpan.FromMilliseconds(500),
                SlidingExpiration = RedisCacheConfiguration.NoSlidingExpiration
            };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            sut.Put(key, new Person("John Doe", 20));

            Thread.Sleep(TimeSpan.FromMilliseconds(600));
            var result = sut.Get(key);

            var setOfActiveKeysKey = sut.CacheNamespace.GetSetOfActiveKeysKey();
            var cacheKey = sut.CacheNamespace.GetKey(key);
            var isKeyStillTracked = Redis.SetContains(setOfActiveKeysKey, cacheKey);
            Assert.IsFalse(isKeyStillTracked);
        }

       [TestMethod]
        void Get_when_sliding_expiration_not_set_does_not_extend_the_expiration()
        {
            var config = new RedisCacheConfiguration("region")
            {
                Expiration = TimeSpan.FromMilliseconds(500),
                SlidingExpiration = RedisCacheConfiguration.NoSlidingExpiration
            };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            sut.Put(1, new Person("John Doe", 10));

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            var result = sut.Get(1);

            var cacheKey = sut.CacheNamespace.GetKey(1);
            var expiry = Redis.KeyTimeToLive(cacheKey);
            //Assert.InRange(expiry.Value, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300));

            //Assert.That(expiry.Value, Is.InRange(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300)));
        }

       [TestMethod]
        void Get_when_sliding_expiration_set_and_time_to_live_is_greater_than_expiration_does_not_reset_the_expiration()
        {
            var config = new RedisCacheConfiguration("region")
            {
                Expiration = TimeSpan.FromMilliseconds(500),
                SlidingExpiration = TimeSpan.FromMilliseconds(100)
            };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            sut.Put(1, new Person("John Doe", 10));

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            var result = sut.Get(1);

            var cacheKey = sut.CacheNamespace.GetKey(1);
            var expiry = Redis.KeyTimeToLive(cacheKey);
            //Assert.InRange(expiry.Value, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300));
            //Assert.That(expiry.Value, Is.InRange(TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(300)));
        }

       [TestMethod]
        void Get_when_sliding_expiration_and_time_to_live_is_less_than_expiration_resets_the_expiration()
        {
            var config = new RedisCacheConfiguration("region")
            {
                Expiration = TimeSpan.FromMilliseconds(500),
                SlidingExpiration = TimeSpan.FromMilliseconds(400)
            };
            var sut = new RedisCache(config, ConnectionMultiplexer, options);
            sut.Put(1, new Person("John Doe", 10));

            Thread.Sleep(TimeSpan.FromMilliseconds(200));
            var result = sut.Get(1);

            var cacheKey = sut.CacheNamespace.GetKey(1);
            var expiry = Redis.KeyTimeToLive(cacheKey);
            //Assert.InRange(expiry.Value, TimeSpan.FromMilliseconds(480), TimeSpan.FromMilliseconds(500));
            //Assert.That(expiry.Value, Is.InRange(TimeSpan.FromMilliseconds(480), TimeSpan.FromMilliseconds(500)));
        }

       [TestMethod]
        void Put_and_Get_into_different_cache_regions()
        {
            const int key = 1;
            var sut1 = new RedisCache("region_A", ConnectionMultiplexer, options);
            var sut2 = new RedisCache("region_B", ConnectionMultiplexer, options);

            sut1.Put(key, new Person("A", 1));
            sut2.Put(key, new Person("B", 1));

            Assert.AreEqual("A", ((Person)sut1.Get(1)).Name);
            Assert.AreEqual("B", ((Person)sut2.Get(1)).Name);
        }

       [TestMethod]
        void Remove_should_remove_from_cache()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            sut.Put(999, new Person("Foo", 10));

            sut.Remove(999);

            var result = sut.Get(999);
            Assert.IsNull(result);
        }

       [TestMethod]
        void Clear_should_remove_all_items_from_cache()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            sut.Put(1, new Person("A", 1));
            sut.Put(2, new Person("B", 2));
            sut.Put(3, new Person("C", 3));
            sut.Put(4, new Person("D", 4));

            sut.Clear();

            Assert.IsNull(sut.Get(1));
            Assert.IsNull(sut.Get(2));
            Assert.IsNull(sut.Get(3));
            Assert.IsNull(sut.Get(4));
        }

       [TestMethod]
        void Destroy_should_not_clear()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            sut.Put(1, new Person("John Doe", 20));

            sut.Destroy();

            var result = sut.Get(1) as Person;


            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual(20, result.Age);
        }

       [TestMethod]
        void Lock_and_Unlock_concurrently_with_same_cache_client()
        {
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            sut.Put(1, new Person("Foo", 1));

            var results = new ConcurrentQueue<string>();
            const int numberOfClients = 5;

            var tasks = new List<Task>();
            for (var i = 1; i <= numberOfClients; i++)
            {
                int clientNumber = i;
                var t = Task.Factory.StartNew(() =>
                {
                    var key = "1";
                    sut.Lock(key);
                    results.Enqueue(clientNumber + " lock");

                    // Artificial concurrency.
                    Thread.Sleep(100);

                    results.Enqueue(clientNumber + " unlock");
                    sut.Unlock(key);
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());

            // Each Lock should be followed by its associated Unlock.
            var listResults = results.ToList();
            for (var i = 1; i <= numberOfClients; i++)
            {
                var lockIndex = listResults.IndexOf(i + " lock");
                Assert.AreEqual(i + " lock", listResults[lockIndex]);
                Assert.AreEqual(i + " unlock", listResults[lockIndex + 1]);
            }
        }

       [TestMethod]
        void Lock_and_Unlock_concurrently_with_different_cache_clients()
        {
            var mainCache = new RedisCache("region", ConnectionMultiplexer, options);
            mainCache.Put(1, new Person("Foo", 1));

            var results = new ConcurrentQueue<string>();
            const int numberOfClients = 5;

            var tasks = new List<Task>();
            for (var i = 1; i <= numberOfClients; i++)
            {
                int clientNumber = i;
                var t = Task.Factory.StartNew(() =>
                {
                    var cacheX = new RedisCache("region", ConnectionMultiplexer, options);
                    var key = "1";
                    cacheX.Lock(key);
                    results.Enqueue(clientNumber + " lock");

                    // Artificial concurrency.
                    Thread.Sleep(100);

                    results.Enqueue(clientNumber + " unlock");
                    cacheX.Unlock(key);
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());

            // Each Lock should be followed by its associated Unlock.
            var listResults = results.ToList();
            for (var i = 1; i <= numberOfClients; i++)
            {
                var lockIndex = listResults.IndexOf(i + " lock");
                Assert.AreEqual(i + " lock", listResults[lockIndex]);
                Assert.AreEqual(i + " unlock", listResults[lockIndex + 1]);
            }
        }

       [TestMethod]
        void Unlock_when_not_locally_locked_triggers_the_unlock_failed_event()
        {
            var unlockFailedCounter = 0;
            options.UnlockFailed += (sender, e) =>
            {
                if (e.LockKey == null && e.LockValue == null)
                {
                    unlockFailedCounter++;
                }
            };
            var sut = new RedisCache("region", ConnectionMultiplexer, options);

            sut.Unlock(123);

            Assert.AreEqual(1, unlockFailedCounter);
        }

       [TestMethod]
        void Unlock_when_locked_locally_but_not_locked_in_redis_triggers_the_unlock_failed_event()
        {
            var unlockFailedCounter = 0;
            options.UnlockFailed += (sender, e) =>
            {
                if (e.LockKey != null && e.LockValue != null)
                {
                    unlockFailedCounter++;
                }
            };
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            const int key = 123;

            sut.Lock(key);
            var lockKey = sut.CacheNamespace.GetLockKey(key);
            Redis.KeyDelete(lockKey);
            sut.Unlock(key);

            Assert.AreEqual(1, unlockFailedCounter);
        }

       [TestMethod]
        void Lock_when_failed_to_acquire_lock_triggers_the_unlock_failed_event()
        {
            var lockFailedCounter = 0;
            options.LockFailed += (sender, e) =>
            {
                lockFailedCounter++;
            };
            options.AcquireLockRetryStrategy = new DoNotRetryAcquireLockRetryStrategy();
            var sut = new RedisCache("region", ConnectionMultiplexer, options);
            const int key = 123;

            sut.Lock(key);
            sut.Lock(key);

            Assert.AreEqual(1, lockFailedCounter);
        }
    }
}
