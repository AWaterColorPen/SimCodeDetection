using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SimCodeDetectionWeb.Startup))]
namespace SimCodeDetectionWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
