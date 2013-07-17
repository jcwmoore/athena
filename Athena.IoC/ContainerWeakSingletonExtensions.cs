using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.IoC
{
    public static class WeakSingletonLifeCycleExtensions
    {
        /// <summary>
        /// Specifies this Registration as a weakly backed Singleton, objects managed with this lifecycle will be reconstructed as necessary
        /// </summary>
        public static void AsWeakSingleton<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new WeakSingletonLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, null);
        }

        /// <summary>
        /// Specifies this Registration as a weakly backed Singleton with an initial value, objects managed with this lifecycle will be reconstructed as necessary
        /// </summary>
        public static void AsWeakSingleton<TInterface, TImplementation>(this DemeterRegistration<TInterface, TImplementation> jr, TImplementation def) where TImplementation : class, TInterface
        {
            jr.ResolutionInfo.LifeCycleManager = new WeakSingletonLifecycle<TInterface, TImplementation>(jr.ResolutionInfo, def);
        }

        internal class WeakSingletonLifecycle<TInterface, TImplementation> : IDemeterLifeCycleManager<TInterface> where TImplementation : class, TInterface
        {
            private readonly Container.ResolutionInfo<TInterface> _info;
            private WeakReference _instance;

            public WeakSingletonLifecycle(Container.ResolutionInfo<TInterface> info, TImplementation instance)
            {
                _info = info;
                _instance = new WeakReference(instance);
            }

            public TInterface GetConcrete()
            {
                TImplementation obj = (TImplementation)_instance.Target;
                if (!_instance.IsAlive)
                {
                    obj = (TImplementation)_info.BuildInstance(_info.Container);
                    _instance.Target = obj;
                }
                return obj;
            }

            public object GetObject()
            {
                return GetConcrete();
            }

            public void Dispose()
            {
            }
        }
    }
}
