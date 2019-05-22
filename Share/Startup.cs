using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Share.Startup))]
namespace Share
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
