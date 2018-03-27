using System.Diagnostics;
using Microsoft.Owin;
using Milou.Web.ClientResources.WebTests.Integration;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Milou.Web.ClientResources.WebTests.Integration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Debug.WriteLine("Running OWIN startup");

            CustomOwinStartup.Configuration(app);
            ConfigureAuth(app);
        }
    }
}