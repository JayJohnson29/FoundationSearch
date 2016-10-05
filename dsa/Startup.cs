using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dsa.Startup))]
namespace dsa
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
