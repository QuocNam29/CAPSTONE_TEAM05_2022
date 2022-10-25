using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CAP_TEAM05_2022.Startup))]
namespace CAP_TEAM05_2022
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
