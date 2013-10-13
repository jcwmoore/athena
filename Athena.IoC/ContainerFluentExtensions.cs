using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC
{
    public static class IoCRegistationContextExtensions
    {
        /// <summary>
        /// Registers a type to its self
        /// </summary>
        public static IoCRegistration<TInterface, TInterface> ToItsSelf<TInterface>(this IoCRegistationContext<TInterface> rc) where TInterface : class
        {
            return rc.To<TInterface>();
        }

        /// <summary>
        /// Assigns a name to this registration
        /// </summary>
        /// <param name="name">Name for the registration</param>
        /// <returns>Original object for Fluent API</returns>
        public static IoCRegistationContext<TInterface> Named<TInterface>(this IoCRegistationContext<TInterface> rc, string name)
        {
            rc.ResolutionInfo.Name = name;
            return rc;
        }
    }

    public static class IoCRegistrationExtensions
    {
        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        public static void AsSingleton<TInterface, TImplementation>(this IoCRegistration<TInterface, TImplementation> jr) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonObjectLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, null);
        }

        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        /// <param name="obj">Singleton object</param>
        public static void AsSingleton<TInterface, TImplementation>(this IoCRegistration<TInterface, TImplementation> jr, TImplementation obj) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonObjectLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, obj);
        }

        /// <summary>
        /// Specifies this Registration as a Singleton
        /// </summary>
        /// <param name="obj">Singleton object</param>
        public static void AsValueSingleton<TInterface, TImplementation>(this IoCRegistration<TInterface, TImplementation> jr, TImplementation obj) where TImplementation : struct, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new Container.SingletonStructLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, obj);
        }

        /// <summary>
        /// Specifies this Registration as a Transient
        /// </summary>
        public static void AsTransient<TInterface, TImplementation>(this IoCRegistration<TInterface, TImplementation> jr) where TImplementation : TInterface
        {
            if (typeof(TImplementation).IsAbstract && jr.ResolutionInfo.IsReflectionBuild)
            {
                throw new RegistrationException(string.Format("Can not register {0} as transient, a defined constructor must be provided", typeof(TInterface).Name), typeof(TImplementation));
            }
            jr.ResolutionInfo.LifeCycleManager = new Container.TransientLifecycle<TInterface>(jr.ResolutionInfo);
        }
    }
}
