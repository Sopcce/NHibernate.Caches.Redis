using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;


namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class RedisCacheProviderOptionsTests
    {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void The_copy_constructor_copies_all_event_handlers()
        {
            var sut = new RedisCacheProviderOptions();
            var order = new List<string>();
            sut.Exception += (s, e) => order.Add("a");
            sut.Exception += (s, e) => order.Add("b");
            sut.Exception += (s, e) => order.Add("c");

            var clone = sut.ShallowCloneAndValidate();
            clone.OnException(null, new ExceptionEventArgs("foo", RedisCacheMethod.Unknown, new Exception()));

            Assert.AreEqual(new[] { "a", "b", "c" }, order);
        }
    }
}
