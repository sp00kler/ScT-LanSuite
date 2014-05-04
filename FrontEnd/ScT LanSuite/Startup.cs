using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ScT_LanSuite.Startup))]
namespace ScT_LanSuite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigureChat(app);
        }
    }
}
