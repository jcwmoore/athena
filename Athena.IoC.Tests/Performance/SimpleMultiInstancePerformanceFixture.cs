using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Ninject;


namespace IoC.Tests.Performance
{
    [TestFixture]
    public class SimpleMultiInstancePerformanceFixture
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
        public void BaseLineMultiInstanceTest()
        {
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = new MockObject1();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} MultiInstance, Total milliseconds elapsed: {1}", "Base Line", (end - start).TotalMilliseconds));
        }

        [Test]
        public void IoCMultiInstanceTest()
        {
            var container = new Container();
            container.Register<MockObject1>().ToItsSelf().AsTransient();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject1>();
                var u = container.Resolve<MockObject2>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} MultiInstance, Total milliseconds elapsed: {1}", "IoC", (end - start).TotalMilliseconds));
        }

        [Test]
        public void HaveBoxMultiInstanceTest()
        {
            var container = new HaveBox.Container();
            container.Configure(x => x.For<MockObject1>().Use<MockObject1>());
            container.Configure(x => x.For<MockObject2>().Use<MockObject2>());
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.GetInstance<MockObject1>();
                var u = container.GetInstance<MockObject2>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Singleton, Total milliseconds elapsed: {1}", "HaveBox", (end - start).TotalMilliseconds));
        }

        //[Test]
        //public void UnityMultiInstanceTest()
        //{
        //    var container = new UnityContainer();
        //    container.RegisterType<MockObject1, MockObject1>();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Resolve<MockObject1>();
        //        var u = container.Resolve<MockObject2>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} MultiInstance, Total milliseconds elapsed: {1}", "Unity", (end - start).TotalMilliseconds));
        //}

        [Test]
        public void TinyIoCMultiInstanceTest()
        {
            var container = new TinyIoC.TinyIoCContainer();
            container.Register<MockObject1, MockObject1>().AsMultiInstance();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject1>();
                var u = container.Resolve<MockObject2>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} MultiInstance, Total milliseconds elapsed: {1}", "TinyIoC", (end - start).TotalMilliseconds));
        }

        //[Test]
        //public void NinjectMultiInstanceTest()
        //{
        //    var container = new Ninject.StandardKernel();
        //    container.Bind<MockObject1>().To<MockObject1>().InTransientScope();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Get<MockObject1>();
        //        var u = container.Get<MockObject2>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} MultiInstance, Total milliseconds elapsed: {1}", "Ninject", (end - start).TotalMilliseconds));
        //}

        #endregion

        private class MockObject1 { }
        private class MockObject2 { }
    }
}
