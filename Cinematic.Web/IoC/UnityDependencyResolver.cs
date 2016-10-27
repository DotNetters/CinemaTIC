using Cinematic.DAL;
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
            // Configuración mediante XML
            //container = new UnityContainer().LoadConfiguration();

            // Configuración mediante código
            container = new UnityContainer();

            container.RegisterType<IDataContext, CinematicEFDataContext>(new PerRequestLifetimeManager());
            container.RegisterType<ISeatManager, SeatManager>();
            container.RegisterType<ISessionManager, SessionManager>();
            container.RegisterType<IPriceManager, PriceManager>();
            container.RegisterType<ITicketManager, TicketManager>();
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