using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

namespace System.Web.Mvc
{
    public static class UnityConfig
    {
        public static void RegisterIoc()
        {
            var container = new Microsoft.Practices.Unity.UnityContainer();
            // build out your container here...
            System.Web.Mvc.DependencyResolver.SetResolver(new UnityMvcResolver(container));
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new UnityApiResolver(container);

        }

        private class UnityMvcResolver : System.Web.Mvc.IDependencyResolver
        {
            private Microsoft.Practices.Unity.IUnityContainer _container;

            public UnityMvcResolver(Microsoft.Practices.Unity.IUnityContainer container)
            {
                _container = container;
            }

            public object GetService(Type serviceType)
            {
                try
                {
                    return _container.Resolve(serviceType);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                try
                {
                    return _container.ResolveAll(serviceType);
                }
                catch (Exception)
                {
                    return new object[0];
                }
            }
        }

        private class UnityApiResolver : System.Web.Http.Dependencies.IDependencyResolver
        {
            private Microsoft.Practices.Unity.IUnityContainer _container;

            public UnityApiResolver(Microsoft.Practices.Unity.IUnityContainer container)
            {
                _container = container;
            }

            public System.Web.Http.Dependencies.IDependencyScope BeginScope()
            {
                return this;
            }

            public object GetService(Type serviceType)
            {
                try
                {
                    return _container.Resolve(serviceType);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                try
                {
                    return _container.ResolveAll(serviceType);
                }
                catch (Exception)
                {
                    return new object[0];
                }
            }

            public void Dispose()
            {
            }
        }
    }
}
