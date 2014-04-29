using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Company.Commerce.Web.Startup))]
namespace Company.Commerce.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
