using BtcWebSrvApp.Business;
using BtcWebSrvApp.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BtcWebSrvApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        

        public WebApiApplication() : base()
        {
            
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            //di did not injected into WebApiApplication class, I do not know why :(

            //start sync local hot wallets with db
            Task.Run(async()=>await new SyncTaskStarter(new WebAppConnectionStringProvider()).StartSync());
            
            //start sendbtc queue resolve process
            Task.Run(() => SendBtcQueue.Start());

            
        }
    }
}
