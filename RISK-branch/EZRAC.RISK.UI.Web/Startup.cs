using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EZRAC.Risk.UI.Web.Startup))]
namespace EZRAC.Risk.UI.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
