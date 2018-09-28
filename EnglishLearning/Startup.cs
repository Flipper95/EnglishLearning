using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EnglishLearning.Startup))]
namespace EnglishLearning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
