using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ARS.ITCS.ControlPanel.Startup))]
namespace ARS.ITCS.ControlPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
