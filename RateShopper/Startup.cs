using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RateShopper.Startup))]
namespace RateShopper
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
