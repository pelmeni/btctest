using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BtcWebSrvApp.API
{
    public class SendBtcParams
    {
        public string toaddress { get; set; }

        public double amount { get; set; }
    }
}