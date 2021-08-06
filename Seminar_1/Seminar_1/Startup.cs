using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Seminar_1.Hubs;

[assembly: OwinStartup(typeof(Seminar_1.Startup))]

namespace Seminar_1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            //var idProvider = new CustomUserIdProvider();

            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => idProvider);
            app.MapSignalR();
        }
    }
}
