using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Ninject;

namespace Athena.IoC.Tests.Performance
{
    [TestFixture]
    public class DefinedConstructionPerformanceFixture
    {
        private readonly int _iterations = 1000000;

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
        public void DemeterDefinedTest()
        {
            var container = new Container();
            container.Register<MockObject>().To<MockObject>().ConstructAs(j => new MockObject()).AsTransient();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Defined Constructor, Total milliseconds elapsed: {1}", "Demeter", (end - start).TotalMilliseconds));
        }

        [Test]
        public void UnityDefinedTest()
        {
            var container = new UnityContainer();
            container.RegisterType<MockObject, MockObject>(new InjectionFactory(u => new MockObject()));
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Defined Constructor, Total milliseconds elapsed: {1}", "Unity", (end - start).TotalMilliseconds));
        }

        [Test]
        public void TinyIoCDefinedTest()
        {
            var container = new TinyIoC.TinyIoCContainer();
            container.Register<MockObject, MockObject>().UsingConstructor(() => new MockObject()).AsMultiInstance();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Defined Constructor, Total milliseconds elapsed: {1}", "TinyIoC", (end - start).TotalMilliseconds));
        }

        #endregion

        private class MockObject { }
    }
}
