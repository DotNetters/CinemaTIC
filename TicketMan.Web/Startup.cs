using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketMan.Web.Startup))]
namespace TicketMan.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
