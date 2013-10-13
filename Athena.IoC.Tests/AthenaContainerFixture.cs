using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace IoC.Tests
{
    [TestFixture]
    public class IoCFixture
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

        [Test(Description = "You can register an object as a singleton")]
        public void ResolveExistingSingletonTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();
            container.Register<MockSimpleObject>().To(obj);

            var res = container.Resolve<MockSimpleObject>();
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You can change the singleton object by overriding the registration")]
        public void ResolveReregisteredSingletonTest()
        {
            var container = new Container();
            var obj1 = new MockSimpleObject();
            var obj2 = new MockSimpleObject();
            container.Register<MockSimpleObject>().To(obj1);
            container.Register<MockSimpleObject>().To(obj2);

            var res = container.Resolve<MockSimpleObject>();
            Assert.That(res, Is.SameAs(obj2));
        }

        [Test(Description = "You can register a structure instance as a singleton")]
        public void ResolveReregisteredSingletonStructureTest()
        {
            var container = new Container();
            var s1 = new MyStucture();
            var obj = new MockSimpleObject();
            s1.Object = obj;
            container.Register<MyStucture>().ToValue(s1);

            var res = container.Resolve<MyStucture>();
            Assert.That(res, Is.Not.SameAs(s1));
            Assert.That(res.Object, Is.SameAs(s1.Object));
            Assert.That(res, Is.EqualTo(s1));
        }

        [Test(Description = "You can instruct the container how to build the objects")]
        public void ResolveUsingFuncTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();

            container.Register<MockSimpleObject>().To<MockSimpleObject>().ConstructAs(j => obj);
            var res = container.Resolve<MockSimpleObject>();
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You can name a registration to allow multiple registrations for a single type")]
        public void ResolveNamedInstanceTest()
        {
            var container = new Container();
            var obj = new MockSimpleObject();

            container.Register<MockSimpleObject>().To<MockSimpleObject>();
            container.Register<MockSimpleObject>().Named("test").To<MockSimpleObject>().ConstructAs(j => obj);
            var res = container.Resolve<MockSimpleObject>("test");
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You can resolve the container, the container is auto registered for you as a singleton")]
        public void ResolveContainerTest()
        {
            var container = new Container();
            var res = container.Resolve<Container>();
            Assert.That(res, Is.SameAs(container));
        }

        [Test(Description = "You can resolve an object with a dependency on the container, the container is auto registered for you")]
        public void ResolveWithContainerOnlyTest()
        {
            var container = new Container();
            container.Register<MockContainerObject>().To<MockContainerObject>();
            var res = container.Resolve<MockContainerObject>();
            Assert.That(res.Container, Is.SameAs(container));
        }

        [Test(Description = "You can change registrations by performing them again")]
        public void OverrideRegistrationTest()
        {
            var container = new Container();
            var obj = new MockContainerObject(container);
            container.Register<MockContainerObject>().To<MockContainerObject>();
            var res1 = container.Resolve<MockContainerObject>();
            container.Register<MockContainerObject>().To(obj);
            var res2 = container.Resolve<MockContainerObject>();
            Assert.That(res1.Container, Is.SameAs(container));
            Assert.That(res1, Is.Not.SameAs(obj));
            Assert.That(res2, Is.SameAs(obj));
        }

        [Test(Description = "You can resolve an interface that is registered to a type, basic IoC")]
        public void ResolveComplexTypeWithCompleteRegistrationTest()
        {
            var container = new Container();
            var so = container.Resolve<MockSimpleObject>();
            var co = container.Resolve<MockContainerObject>();
            container.Register<MockSimpleObject>().To(so);
            container.Register<MockContainerObject>().To(co);
            container.Register<IMyInterface>().To<MyClass>();
            var res = container.Resolve<IMyInterface>() as MyClass;
            Assert.That(res, Is.Not.Null);
            Assert.That(res.CObject, Is.SameAs(co));
            Assert.That(res.Object, Is.SameAs(so));
        }

        [Test(Description = "You cannot resolve a concrete type if the container cannot satisfy dependencies")]
        [ExpectedException(typeof(ResolutionException))]
        public void ResolveClassWithUnregisteredParameters()
        {
            var container = new Container();
            container.Resolve<MyDependencyClass>();
            Assert.Fail();
        }

        [Test(Description = "You can resolve a concrete type if the container can satisfy dependencies")]
        public void ResolveUnregisteredClassTest()
        {
            var container = new Container();
            var res = container.Resolve<MockSimpleObject>();
            Assert.That(res, Is.Not.Null);
        }

        [Test(Description = "You can resolve a named concrete type if the container can satisfy dependencies")]
        public void ResolveUnregisteredClassWithNameTest()
        {
            var container = new Container();
            var res = container.Resolve<MockSimpleObject>("test");
            Assert.That(res, Is.Not.Null);
        }

        [Test(Description = "The container will select the constructor with the most parameters that it can satisfy (Greedy)")]
        public void ResolveGreedyConstructorTest()
        {
            var container = new Container();
            container.Register<MockSimpleObject>().ToItsSelf();
            var res = container.Resolve<GreedyObject>();
            Assert.That(res, Is.Not.Null);
            Assert.That(res.Obj1, Is.Not.Null);
            Assert.That(res.Obj2, Is.Not.Null);
        }

        [Test(Description = "You cannot resolve an object with unknown dependencies")]
        [ExpectedException(typeof(ResolutionException))]
        public void AutoRegisterSimpleDIFailsTest()
        {
            var container = new Container();
            var res = container.Resolve<SimpleDIObject>();
            Assert.Fail("Should not be auto registering dependencies");
        }

        [Test(Description = "You can resolve a concrete type if the container can satisfy dependencies")]
        public void ContainerRemembersAutoRegisterTest()
        {
            var container = new Container();
            container.Resolve<MockSimpleObject>();
            var res = container.Resolve<SimpleDIObject>();
            Assert.That(res, Is.Not.Null);
        }

        [Test(Description = "You can register an interface to itself as a singleton")]
        public void ContainerRegisterInterfaceSingletonTest()
        {
            var container = new Container();
            IMyInterface obj = new MyClass(new MockSimpleObject(), new MockContainerObject(container));
            container.Register<IMyInterface>().To(obj);
            var res = container.Resolve<IMyInterface>();
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You can register an interface to itself as a singleton")]
        public void ContainerQuickRegisterInterfaceSingletonTest()
        {
            var container = new Container();
            IMyInterface obj = new MyClass(new MockSimpleObject(), new MockContainerObject(container));
            container.Register(obj);
            var res = container.Resolve<IMyInterface>();
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You can register an interface to itself if you tell the container how to build it")]
        public void ContainerRegisterInterfaceWithBuilderTest()
        {
            var container = new Container();
            container.Register<IMyInterface>().As(c => (IMyInterface)new MyClass(new MockSimpleObject(), new MockContainerObject(c)));
            var res = container.Resolve<IMyInterface>();
            Assert.That(res, Is.Not.Null);
        }

        [Test(Description = "You cannot register an interface to itself as a transient")]
        [ExpectedException(typeof(RegistrationException))]
        public void ContainerRegisterInterfaceTransientTest()
        {
            var container = new Container();
            IMyInterface obj = new MyClass(new MockSimpleObject(), new MockContainerObject(container));
            container.Register<IMyInterface>().To(obj).AsTransient();
            Assert.Fail();
        }

        [Test(Description = "You can register an interface to itself as a transient if you define a custom builder")]
        public void ContainerRegisterInterfaceTransientWithCustomBuilderTest()
        {
            var container = new Container();
            IMyInterface obj = new MyClass(new MockSimpleObject(), new MockContainerObject(container));
            container.Register<IMyInterface>().To(obj).ConstructAs(c => obj).AsTransient();
            var res = container.Resolve<IMyInterface>();
            Assert.That(res, Is.SameAs(obj));
        }

        [Test(Description = "You cannot register an abstract class to its self")]
        [ExpectedException(typeof(RegistrationException))]
        public void RegisteredAbstractClassTest()
        {
            var container = new Container();
            container.Register<MyAbstractClass>().To<MyAbstractClass>();
            Assert.Fail();
        }

        [Test(Description = "You cannot resolve an interface without registering it")]
        [ExpectedException(typeof(ResolutionException))]
        public void UnregisteredInterfacedTest()
        {
            var container = new Container();
            container.Resolve<IMyInterface>();
            Assert.Fail();
        }

        [Test(Description = "You cannot register an interface to its self")]
        [ExpectedException(typeof(RegistrationException))]
        public void RegisteredInterfaceTest()
        {
            var container = new Container();
            container.Register<IMyInterface>().To<IMyInterface>();
            Assert.Fail();
        }

        [Test(Description = "You can resolve all registrations, default is returned first in IEnumerable")]
        public void ResolveAllWithDefaultImplementationTest()
        {
            var container = new Container();
            var first = new MockSimpleObject();
            var other = new MockSimpleObject();
            container.Register<MockSimpleObject>().To(first);
            container.Register<MockSimpleObject>().Named("test1").ToItsSelf();
            container.Register<MockSimpleObject>().Named("test3").ToItsSelf();
            container.Register<MockSimpleObject>().Named("test2").To(other);

            var result = container.ResolveAll<MockSimpleObject>().ToList();
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.First(), Is.SameAs(first));
            Assert.That(result.Contains(other));
        }

        [Test(Description = "You can resolve all registrations, no guarantee of ordering")]
        public void ResolveAllWithoutDefaultImplementationTest()
        {
            var container = new Container();
            var first = new MockSimpleObject();
            var other = new MockSimpleObject();
            container.Register<MockSimpleObject>().Named("test").To(first);
            container.Register<MockSimpleObject>().Named("test1").ToItsSelf();
            container.Register<MockSimpleObject>().Named("test3").ToItsSelf();
            container.Register<MockSimpleObject>().Named("test2").To(other);

            var result = container.ResolveAll<MockSimpleObject>().ToList();
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(result.Contains(first));
            Assert.That(result.Contains(other));
        }

        [Test(Description = "You can dispose the container")]
        public void DisposeReuseTest()
        {
            var container = new Container();
            container.Register<MockContainerObject>().ToItsSelf().AsSingleton();
            var target = container.Resolve<MockContainerObject>();
            Assert.That(target.Disposed, Is.False);
            container.Dispose();
            Assert.That(target.Disposed, Is.True);
        }

        [Test(Description = "You can not use a disposed container")]
        [ExpectedException(typeof(ResolutionException))]
        public void DisposeTest()
        {
            var container = new Container();
            container.Register<MockContainerObject>().ToItsSelf().AsSingleton();
            container.Dispose();
            container.Resolve<MockContainerObject>();
        }

        #endregion

        #region Mock Classes
        private interface IMyInterface
        { }

        private abstract class MyAbstractClass { }

        private class MyClass : IMyInterface
        {
            public readonly MockSimpleObject Object;
            public readonly MockContainerObject CObject;

            public MyClass(MockSimpleObject obj, MockContainerObject cobj)
            {
                Object = obj;
                CObject = cobj;
            }
        }

        private class MyDependencyClass
        {
            public MyDependencyClass(IMyInterface mi)
            {

            }
        }

        private class MockSimpleObject
        {
        }

        private class SimpleDIObject
        {
            public SimpleDIObject(MockSimpleObject obj)
            {

            }
        }

        private class GreedyObject
        {
            public MockSimpleObject Obj1 { get; set; }
            public MockSimpleObject Obj2 { get; set; }

            public GreedyObject(MockSimpleObject o1)
            {
                Obj1 = o1;
                Obj2 = null;
            }

            public GreedyObject(MockSimpleObject o1, MockSimpleObject o2)
            {
                Obj1 = o1;
                Obj2 = o2;
            }
        }

        private struct MyStucture
        {
            public MockSimpleObject Object { get; set; }
        }

        private class MockContainerObject : IDisposable
        {
            public bool Disposed { get; private set; }
            public readonly Container Container;

            public MockContainerObject(Container container)
            {
                Container = container;
                Disposed = false;
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }
        #endregion
    }
}
