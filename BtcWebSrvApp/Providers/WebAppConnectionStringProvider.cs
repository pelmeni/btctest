using BtcWebSrvApp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtcWebSrvApp.Providers
{
    public class WebAppConnectionStringProvider : IConnectionStringProvider
    {
        public string ConnectionString => Properties.Settings.Default.BtcSrvDb;
    }
}