using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using Cinematic.Contracts;
using Cinematic.DAL;
using Microsoft.Practices.Unity;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Cinematic.Web.App_Start.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Cinematic.Web.App_Start.UnityWebActivator), "Shutdown")]

namespace Cinematic.Web.App_Start
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            var container = UnityConfig.GetConfiguredContainer();

            container.RegisterType<IDataContext, CinematicEFDataContext>(new PerRequestLifetimeManager());
            container.RegisterType<ISeatManager, SeatManager>();
            container.RegisterType<ISessionManager, SessionManager>();
            container.RegisterType<IPriceManager, PriceManager>();
            container.RegisterType<ITicketManager, TicketManager>();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}