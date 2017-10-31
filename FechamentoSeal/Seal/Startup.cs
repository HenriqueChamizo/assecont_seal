using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Seal.Startup))]
namespace Seal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
