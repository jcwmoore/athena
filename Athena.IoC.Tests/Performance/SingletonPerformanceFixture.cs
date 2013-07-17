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
    public class SingletonPerformanceFixture
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
        public void DemeterSingletonTest()
        {
            var container = new Container();
            container.Register<MockObject>().To(new MockObject()).AsSingleton();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "Demeter", (end - start).TotalMilliseconds));
        }

        [Test]
        public void HaveBoxSingletonTest()
        {
            var container = new HaveBox.Container();
            container.Configure(x => x.For<MockObject>().Use<MockObject>().AsSingleton());
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.GetInstance<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "HaveBox", (end - start).TotalMilliseconds));
        }

        [Test]
        public void UnitySingletonTest()
        {
            var container = new UnityContainer();
            container.RegisterInstance<MockObject>(new MockObject());
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "Unity", (end - start).TotalMilliseconds));
        }

        [Test]
        public void TinyIoCSingletonTest()
        {
            var container = new TinyIoC.TinyIoCContainer();
            container.Register<MockObject>(new MockObject());
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "TinyIoC", (end - start).TotalMilliseconds));
        }

        [Test]
        public void NinjectSingletonTest()
        {
            var container = new Ninject.StandardKernel();
            container.Bind<MockObject>().ToConstant(new MockObject());
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Get<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "Ninject", (end - start).TotalMilliseconds));
        }

        #endregion

        private class MockObject { }
    }
}
