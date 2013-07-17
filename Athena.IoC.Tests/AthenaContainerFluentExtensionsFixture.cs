using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Athena.IoC.Tests
{
    [TestFixture]
    public class DemeterFluentExtensionsFixture
    {
        #region SetUp / TearDown

        [SetUp]
        public void Init()
        { }

        [TearDown]
        public void Dispose()
        { }

        #endregion

        #region Tests

        [Test]
        public void ResolveUndefinedSingletonTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();
            container.Register<MockSimpleObject>().To<MockSimpleObject>().AsSingleton();

            var res1 = container.Resolve<MockSimpleObject>();
            Assert.That(res1, Is.Not.Null);
            var res2 = container.Resolve<MockSimpleObject>();
            Assert.That(res2, Is.Not.Null);
            Assert.That(res1, Is.SameAs(res2));
        }

        [Test]
        public void ResolveDefinedSingletonTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();
            container.Register<MockSimpleObject>().To<MockSimpleObject>().AsSingleton(obj);

            var res1 = container.Resolve<MockSimpleObject>();
            Assert.That(res1, Is.SameAs(obj));
        }

        [Test]
        public void ChangeSingletonToTransientTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();
            container.Register<MockSimpleObject>().To(obj).AsTransient();

            var res1 = container.Resolve<MockSimpleObject>();
            Assert.That(res1, Is.Not.Null);
            Assert.That(res1, Is.Not.SameAs(obj));
        }

        private class MockSimpleObject
        {
        }

        #endregion
    }
}
