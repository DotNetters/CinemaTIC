using Cinematic.Domain;
using Cinematic.Domain.Contracts;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinematic.Web.IoC
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        IUnityContainer container = null;

        public UnityDependencyResolver()
        {
            container = new UnityContainer().LoadConfiguration();
        }

        public object GetService(Type serviceType)
        {
            object instance;

            try
            {
                instance = container.Resolve(serviceType);
                if (serviceType.IsAbstract || serviceType.IsInterface)
                {
                    return null;
                }
                return instance;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            IEnumerable<object> instanceList = container.ResolveAll(serviceType);
            return instanceList;
        }
    }
}