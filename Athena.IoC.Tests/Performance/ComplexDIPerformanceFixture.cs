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
    public class ComplexDIPerformanceFixture
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
            var container = new Container();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = new MockObjectIoC(container, new MockDIObject3(new MockDIObject2(new MockDIObject1())));
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "Base Line", (end - start).TotalMilliseconds));
        }

        [Test]
        public void IoCComplexDITest()
        {
            var container = new Container();
            container.Register<MockDIObject1>().ToItsSelf().AsTransient();
            container.Register<MockDIObject2>().ToItsSelf().AsTransient();
            container.Register<MockDIObject3>().ToItsSelf().AsTransient();
            container.Register<MockObject>().To<MockObjectIoC>().AsTransient();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "IoC", (end - start).TotalMilliseconds));
        }

        [Test]
        public void HaveBoxSimpleDITest()
        {
            var container = new HaveBox.Container();
            container.Configure(x => x.For<HaveBox.Container>().Use<HaveBox.Container>().AsSingleton());
            container.Configure(x => x.For<MockDIObject1>().Use<MockDIObject1>());
            container.Configure(x => x.For<MockDIObject2>().Use<MockDIObject2>());
            container.Configure(x => x.For<MockDIObject3>().Use<MockDIObject3>());
            container.Configure(x => x.For<MockObject>().Use<MockObjectHaveBox>());
            //container.Configure(x => );
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.GetInstance<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "HaveBox", (end - start).TotalMilliseconds));
        }

        //[Test]
        //public void UnityComplexDITest()
        //{
        //    var container = new UnityContainer();
        //    container.RegisterType<MockObject, MockObjectUnity>();
        //    container.RegisterType<MockDIObject1, MockDIObject1>();
        //    container.RegisterType<MockDIObject2, MockDIObject2>();
        //    container.RegisterType<MockDIObject3, MockDIObject3>();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Resolve<MockObject>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "Unity", (end - start).TotalMilliseconds));
        //}

        [Test]
        public void TinyIoCComplexDITest()
        {
            var container = new TinyIoC.TinyIoCContainer();
            container.Register<MockObject, MockObjectTiny>().AsMultiInstance();
            container.Register<MockDIObject1, MockDIObject1>().AsMultiInstance();
            container.Register<MockDIObject2, MockDIObject2>().AsMultiInstance();
            container.Register<MockDIObject3, MockDIObject3>().AsMultiInstance();
            var start = DateTime.Now;
            for (int i = 0; i < _iterations; i++)
            {
                var t = container.Resolve<MockObject>();
            }
            var end = DateTime.Now;
            Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "TinyIoC", (end - start).TotalMilliseconds));
        }

        //[Test] //this test is very slow!
        //public void NinjectComplexDITest()
        //{
        //    var container = new Ninject.StandardKernel();
        //    container.Bind<MockObject>().To<MockObjectNinject>().InTransientScope();
        //    container.Bind<MockDIObject1>().ToSelf().InTransientScope();
        //    container.Bind<MockDIObject2>().ToSelf().InTransientScope();
        //    container.Bind<MockDIObject3>().ToSelf().InTransientScope();
        //    var start = DateTime.Now;
        //    for (int i = 0; i < _iterations; i++)
        //    {
        //        var t = container.Get<MockObject>();
        //    }
        //    var end = DateTime.Now;
        //    Console.WriteLine(string.Format("{0} Complex DI, Total milliseconds elapsed: {1}", "Ninject", (end - start).TotalMilliseconds));
        //}

        #endregion

        private abstract class MockObject { }

        private class MockDIObject1 { }

        private class MockDIObject2
        {
            public MockDIObject2(MockDIObject1 o1)
            {

            }
        }

        private class MockDIObject3
        {
            public MockDIObject3(MockDIObject2 o2)
            {

            }
        }

        private class MockObjectIoC : MockObject
        {
            public MockObjectIoC(Container c, MockDIObject3 o3)
            {

            }
        }

        private class MockObjectUnity : MockObject
        {
            public MockObjectUnity(IUnityContainer c, MockDIObject3 o3)
            {

            }
        }

        private class MockObjectTiny : MockObject
        {
            public MockObjectTiny(TinyIoC.TinyIoCContainer c, MockDIObject3 o3)
            {

            }
        }

        private class MockObjectNinject : MockObject
        {
            public MockObjectNinject(IKernel c, MockDIObject3 o3)
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
