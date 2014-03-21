using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LCIATest.Startup))]
namespace LCIATest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
