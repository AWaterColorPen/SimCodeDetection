using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoreWeb.Startup))]
namespace CoreWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
