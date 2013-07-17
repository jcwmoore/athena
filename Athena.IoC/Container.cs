using System;
using System.Collections.Generic;
using System.Linq;

namespace Athena.IoC
{
    public sealed partial class Container : IDisposable
    {
        internal static readonly string DEFAULT_INSTANCE_NAME = Guid.NewGuid().ToString();
        private readonly Dictionary<Type, Dictionary<string, ResolutionInfoBase>> _resolutionMap = new Dictionary<Type, Dictionary<string, ResolutionInfoBase>>();

        public Container()
        {
            Register<Container>().To(this);
        }

        /// <summary>
        /// Begins the registration syntax, must invoke the To operation to persist the registration
        /// </summary>
        public DemeterRegistationContext<TType> Register<TType>()
        {
            return new RegistrationContext<TType>(this);
        }

        /// <summary>
        /// Returns the default implementation of <typeparamref name="TType"/> 
        /// </summary>
        public TType Resolve<TType>()
        {
            return Resolve<TType>(DEFAULT_INSTANCE_NAME);
        }

        /// <summary>
        /// Returns the named implementation of <typeparamref name="TType"/>
        /// </summary>
        public TType Resolve<TType>(string name)
        {
            var t = typeof(TType);
            if (!_resolutionMap.ContainsKey(t))
            {
                if (t.IsInterface || t.IsAbstract)
                {
                    throw new ResolutionException(string.Format("The requested type is not available: {0}", typeof(TType).Name), typeof(TType));
                }
                ((RegistrationContext<TType>)Register<TType>().Named(name)).ToDynamically<TType>(); // we will try to register the new type and see what happens
            }
            if (!_resolutionMap[t].ContainsKey(name))
            {
                throw new ResolutionException(string.Format("The requested type, {0}, is not available with name {1}", typeof(TType).Name, name), typeof(TType));
            }
            return ((ResolutionInfo<TType>)_resolutionMap[t][name]).LifeCycleManager.GetConcrete();
        }

        /// <summary>
        /// Returns a lazily loaded collection of all registrations of <typeparamref name="TType"/> with the
        /// default implementation as the first in the collection
        /// </summary>
        public IEnumerable<TType> ResolveAll<TType>()
        {
            if (_resolutionMap.ContainsKey(typeof(TType)) && _resolutionMap[typeof(TType)].ContainsKey(DEFAULT_INSTANCE_NAME))
            {
                yield return Resolve<TType>();
            }
            var names = _resolutionMap[typeof(TType)].Keys.Where(k => k != DEFAULT_INSTANCE_NAME).ToList();
            foreach (var n in names)
            {
                yield return Resolve<TType>(n);
            }
        }

        /// <summary>
        /// releases all objects held by the container and clears all registrations
        /// </summary>
        public void Dispose()
        {
            foreach (var obj in _resolutionMap.Where(m => m.Key != typeof(Container)).SelectMany(m => m.Value.Values))
            {
                obj.ObjectManager.Dispose();
            }
            _resolutionMap.Clear();
        }

        private TInterface ReflectionBuild<TInterface, TType>(ResolutionInfoBase ri) where TType : TInterface
        {
            if (ri.ConstructorInfo == null)
            {
                ri.ConstructorInfo = typeof(TType).GetConstructors()
                                                  .OrderByDescending(c => c.GetParameters().Length)
                                                  .FirstOrDefault(c => c.GetParameters().All(p => _resolutionMap.ContainsKey(p.ParameterType)));
                if (ri.ConstructorInfo != null)
                {
                    ri.Parameters = ri.ConstructorInfo.GetParameters();
                }
                else
                {
                    throw new ResolutionException(string.Format("Unable to resolve dependencies for type: {0}", typeof(TType).Name), typeof(TType));
                }
            }
            var parameters = ri.Parameters.Select(p => _resolutionMap[p.ParameterType][DEFAULT_INSTANCE_NAME].ObjectManager.GetObject()).ToArray();
            return (TInterface)ri.ConstructorInfo.Invoke(parameters);
        }


        internal class TransientLifecycle<TInterface> : IDemeterLifeCycleManager<TInterface>
        {
            protected readonly ResolutionInfo<TInterface> _info;
            public TransientLifecycle(ResolutionInfo<TInterface> info)
            {
                _info = info;
            }

            public virtual TInterface GetConcrete()
            {
                return _info.BuildInstance(_info.Container);
            }

            public object GetObject()
            {
                return GetConcrete();
            }

            public virtual void Dispose() { }
        }

        internal abstract class SingletonLifecycleBase<TInterface, TImplementation> : TransientLifecycle<TInterface> where TImplementation : TInterface
        {
            protected TImplementation _instance;
            public SingletonLifecycleBase(ResolutionInfo<TInterface> info, TImplementation instance)
                : base(info)
            {
                _instance = instance;
            }

            public override TInterface GetConcrete()
            {
                return _instance;
            }

            public override void Dispose()
            {
                if (_instance as IDisposable != null)
                {
                    ((IDisposable)_instance).Dispose();
                }
            }
        }

        internal class SingletonStructLifecycle<TInterface, TImplementation> : SingletonLifecycleBase<TInterface, TImplementation> where TImplementation : struct, TInterface
        {
            public SingletonStructLifecycle(ResolutionInfo<TInterface> info, TImplementation instance) : base(info, instance) { }
        }

        internal class SingletonObjectLifecycle<TInterface, TImplementation> : SingletonLifecycleBase<TInterface, TImplementation> where TImplementation : class, TInterface
        {
            public SingletonObjectLifecycle(ResolutionInfo<TInterface> info, TImplementation instance)
                : base(info, instance)
            {
                if (instance == null)
                {
                    _instance = (TImplementation)_info.BuildInstance(_info.Container);
                }
            }
        }

        private class RegistrationContext<TInterface> : DemeterRegistationContext<TInterface>
        {
            private readonly Container _container;
            private readonly ResolutionInfo<TInterface> _info = new ResolutionInfo<TInterface>();
            internal override ResolutionInfo<TInterface> ResolutionInfo { get { return _info; } }

            public RegistrationContext(Container container)
            {
                _container = container;
                _info.ParentContainer = _container;
                _info.Name = DEFAULT_INSTANCE_NAME;
                _info.Container = _container;
            }

            private void BuildBase<TImplementation>() where TImplementation : TInterface
            {
                _info.BuildInstance = (j) => _container.ReflectionBuild<TInterface, TImplementation>(_info);
                _info.IsReflectionBuild = true;
                if (!_container._resolutionMap.ContainsKey(typeof(TInterface)))
                {
                    _container._resolutionMap.Add(typeof(TInterface), new Dictionary<string, ResolutionInfoBase>());
                }
                if (_container._resolutionMap[typeof(TInterface)].ContainsKey(_info.Name))
                {
                    _container._resolutionMap[typeof(TInterface)].Remove(_info.Name);
                }
                _container._resolutionMap[typeof(TInterface)].Add(_info.Name, _info);
            }

            public override DemeterRegistration<TInterface, TImplementation> As<TImplementation>(Func<Container, TImplementation> fun)
            {
                BuildBase<TImplementation>();
                _info.IsReflectionBuild = false;
                _info.BuildInstance = (c) => fun(c);
                _info.LifeCycleManager = new TransientLifecycle<TInterface>(_info);
                return new Registration<TInterface, TImplementation>(_info);
            }

            public override DemeterRegistration<TInterface, TImplementation> To<TImplementation>()
            {
                BuildBase<TImplementation>();
                if (typeof(TImplementation).IsAbstract)
                {
                    throw new RegistrationException(string.Format("Cannot register {0} to abstract type {1}", typeof(TInterface).Name, typeof(TImplementation).Name), typeof(TImplementation));
                }
                _info.LifeCycleManager = new TransientLifecycle<TInterface>(_info);
                return new Registration<TInterface, TImplementation>(_info);
            }

            public DemeterRegistration<TInterface, TImplementation> ToDynamically<TImplementation>() where TImplementation : TInterface
            {
                if (typeof(TImplementation).IsAbstract)
                {
                    throw new RegistrationException(string.Format("Cannot register {0} to abstract type {1}", typeof(TInterface).Name, typeof(TImplementation).Name), typeof(TImplementation));
                }
                _info.LifeCycleManager = new TransientLifecycle<TInterface>(_info);
                BuildBase<TImplementation>();
                return new Registration<TInterface, TImplementation>(_info);
            }

            public override DemeterRegistration<TInterface, TImplementation> To<TImplementation>(TImplementation ti)
            {
                BuildBase<TImplementation>();
                _info.LifeCycleManager = new SingletonObjectLifecycle<TInterface, TImplementation>(_info, ti);
                return new Registration<TInterface, TImplementation>(_info);
            }

            public override DemeterRegistration<TInterface, TImplementation> ToValue<TImplementation>(TImplementation ti)
            {
                BuildBase<TImplementation>();
                _info.LifeCycleManager = new SingletonStructLifecycle<TInterface, TImplementation>(_info, ti);
                return new Registration<TInterface, TImplementation>(_info);
            }
        }

        private class Registration<TInterface, TImplementation> : DemeterRegistration<TInterface, TImplementation> where TImplementation : TInterface
        {
            private readonly ResolutionInfo<TInterface> _info;
            private Func<Container, TImplementation> _fun;
            internal override ResolutionInfo<TInterface> ResolutionInfo { get { return _info; } }

            public Registration(ResolutionInfo<TInterface> info)
            {
                _info = info;
            }

            public override DemeterRegistration<TInterface, TImplementation> ConstructAs(Func<Container, TImplementation> fun)
            {
                _fun = fun;
                _info.BuildInstance = (c) => _fun(c);
                _info.IsReflectionBuild = false;
                return this;
            }
        }

        internal abstract class ResolutionInfoBase
        {
            public Container Container { get; set; }
            public string Name { get; set; }
            public abstract IDemeterObjectManager ObjectManager { get; }
            public Container ParentContainer { get; set; }
            public System.Reflection.ConstructorInfo ConstructorInfo { get; set; }
            public System.Reflection.ParameterInfo[] Parameters { get; set; }
        }

        internal class ResolutionInfo<TInterface> : ResolutionInfoBase
        {
            public TInterface Singleton { get; set; }
            public bool IsReflectionBuild { get; set; }
            public Func<Container, TInterface> BuildInstance { get; set; }
            public IDemeterLifeCycleManager<TInterface> LifeCycleManager { get; set; }
            public override IDemeterObjectManager ObjectManager { get { return LifeCycleManager; } }
        }
    }

    #region Exceptions
    /// <summary>
    /// This exception is thrown when the container is unable to complete the requested type Resolution
    /// </summary>
    public sealed class ResolutionException : Exception
    {
        public Type AttemptedType { get; set; }
        internal ResolutionException(string message, Type attemptedType) : this(message, attemptedType, null) { }
        internal ResolutionException(string message, Type attemptedType, Exception inner)
            : base(message, inner)
        {
            AttemptedType = attemptedType;
        }
    }

    /// <summary>
    /// This exception is thrown when the container fails to complete the requested registration
    /// </summary>
    public sealed class RegistrationException : Exception
    {
        public Type AttemptedType { get; set; }
        internal RegistrationException(string message, Type attemptedType) : this(message, attemptedType, null) { }
        internal RegistrationException(string message, Type attemptedType, Exception inner)
            : base(message, inner)
        {
            AttemptedType = attemptedType;
        }
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// This is the pre registration object, any pre-registration operations (i.e. naming) will happen on this object
    /// </summary>
    /// <typeparam name="TInterface">Interface/Abstract type</typeparam>
    public abstract class DemeterRegistationContext<TInterface>
    {
        internal abstract Container.ResolutionInfo<TInterface> ResolutionInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract DemeterRegistration<TInterface, TImplementation> As<TImplementation>(Func<Container, TImplementation> fun) where TImplementation : TInterface;

        /// <summary>
        /// Assigns the registration to a concert type as a transient
        /// </summary>
        public abstract DemeterRegistration<TInterface, TImplementation> To<TImplementation>() where TImplementation : class, TInterface;

        /// <summary>
        /// Assigns the registration to a concert type with a predefined object as a singleton
        /// </summary>
        public abstract DemeterRegistration<TInterface, TImplementation> To<TImplementation>(TImplementation ti) where TImplementation : class, TInterface;

        /// <summary>
        /// Assigns the registration to a concert type with a predefined object as a singleton
        /// </summary>
        public abstract DemeterRegistration<TInterface, TImplementation> ToValue<TImplementation>(TImplementation ti) where TImplementation : struct, TInterface;
    }

    /// <summary>
    /// Post registration object, once you have this object the registration is persisted and you can adjust 
    /// any post registration configuration options, i.e. life cycle management.
    /// </summary>
    /// <typeparam name="TInterface">Interface/Abstract type</typeparam>
    /// <typeparam name="TImplementation">Concrete type</typeparam>
    public abstract class DemeterRegistration<TInterface, TImplementation> where TImplementation : TInterface
    {
        internal abstract Container.ResolutionInfo<TInterface> ResolutionInfo { get; }

        /// <summary>
        /// Provides the registration with a direct method for building an instance
        /// </summary>
        /// <returns>the original object</returns>
        public abstract DemeterRegistration<TInterface, TImplementation> ConstructAs(Func<Container, TImplementation> fun);
    }

    /// <summary>
    /// This is the base (non-generic) interface for object life cycle managers, custom life cycles should always implement <see cref="IDemeterLifeCycleManager"/>
    /// </summary>
    public interface IDemeterObjectManager : IDisposable
    {
        /// <summary>
        /// returns a non-generic object instance
        /// </summary>
        object GetObject();
    }

    /// <summary>
    /// Interface for all life cycle managers.  The life cycle manager for a <see cref="ResolutionInfo"/> can be set by an extension method on the <see cref="DemeterRegistration"/> class.
    /// </summary>
    /// <typeparam name="TInterface">Interface type, this is not the same as the concrete implementation type</typeparam>
    public interface IDemeterLifeCycleManager<TInterface> : IDemeterObjectManager
    {
        /// <summary>
        /// Provides a generic object instance, as the interface type, for this Lifecycle
        /// </summary>
        TInterface GetConcrete();
    }
    #endregion
}
