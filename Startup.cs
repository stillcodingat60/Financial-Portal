using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Financial_Portal.Startup))]
namespace Financial_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
