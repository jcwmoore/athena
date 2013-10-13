using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Practices.Unity;
using Ninject;

namespace IoC.Tests
{
    [TestFixture]
    public class SimpleDIPerformanceFixture
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
        public void IoCSimpleDITest()
        {
            var container = new Container();
            container.Register<MockObject>().To<MockObjectIoC>().AsTransient();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Simple DI, Total milliseconds elapsed: {1}", "IoC", (end - start).TotalMilliseconds));
        }

        [Test] // this is not really a valid test because I can't register an object into HaveBox
        public void HaveBoxSimpleDITest()
        {
            var container = new HaveBox.Container();            
            container.Configure(x => x.For<HaveBox.Container>().Use<HaveBox.Container>().AsSingleton());
            container.Configure(x => x.For<MockObject>().Use<MockObjectHaveBox>());
            //container.Configure(x => );
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.GetInstance<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Simple DI, Total milliseconds elapsed: {1}", "HaveBox", (end - start).TotalMilliseconds));
        }

        //[Test]
        //public void UnitySimpleDITest()
        //{
        //    var container = new UnityContainer();
        //    container.RegisterType<MockObject, MockObjectUnity>();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Resolve<MockObject>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} Simple DI, Total milliseconds elapsed: {1}", "Unity", (end - start).TotalMilliseconds));
        //}

        [Test]
        public void TinyIoCSimpleDITest()
        {
            var container = new TinyIoC.TinyIoCContainer();
            container.Register<MockObject, MockObjectTiny>().AsMultiInstance();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Simple DI, Total milliseconds elapsed: {1}", "TinyIoC", (end - start).TotalMilliseconds));
        }

        //[Test]
        //public void NinjectSimpleDITest()
        //{
        //    var container = new Ninject.StandardKernel();
        //    container.Bind<MockObject>().To<MockObjectNinject>().InTransientScope();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Get<MockObject>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} Simple DI, Total milliseconds elapsed: {1}", "Ninject", (end - start).TotalMilliseconds));
        //}

        #endregion

        private abstract class MockObject { }

        private class MockObjectIoC : MockObject
        {
            public MockObjectIoC(Container c)
            {

            }
        }

        private class MockObjectUnity : MockObject
        {
            public MockObjectUnity(IUnityContainer c)
            {

            }
        }

        private class MockObjectTiny : MockObject
        {
            public MockObjectTiny(TinyIoC.TinyIoCContainer c)
            {

            }
        }

        private class MockObjectNinject : MockObject
        {
            public MockObjectNinject(IKernel c)
            {

            }
        }

        private class MockObjectHaveBox : MockObject
        {
            public MockObjectHaveBox(HaveBox.Container c)
            {

            }
        }
    }
}
