﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace NHibernate.Caches.Redis.Tests
{
    public class RedisCacheProviderOptionsTests
    {
        [Test]
        void the_copy_constructor_copies_all_event_handlers()
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
