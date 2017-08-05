using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureTraining.Startup))]
namespace AzureTraining
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
