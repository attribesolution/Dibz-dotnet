using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;


[assembly: OwinStartup(typeof(DIBZ.Services.Startup))]
namespace DIBZ.Services
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            AppDomain.CurrentDomain.Load(typeof(DIBZHub).Assembly.FullName);
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            //// Make long polling connections wait a maximum of 110 seconds for a
            //// response. When that time expires, trigger a timeout command and
            //// make the client reconnect.
            //GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            //// Wait a maximum of 30 seconds after a transport connection is lost
            //// before raising the Disconnected event to terminate the SignalR connection.
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

            //// For transports other than long polling, send a keepalive packet every
            //// 10 seconds. 
            //// This value must be no more than 1/3 of the DisconnectTimeout value.
            //GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

            app.MapSignalR(hubConfiguration);
        }
    }
}
