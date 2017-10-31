using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FechamentoSeal.Startup))]
namespace FechamentoSeal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
