using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.API
{
    public class BtcTransaction
    {
            public DateTime date { get; set; }
            public string address { get; set; }
            public double amount { get; set; }
            public int confirmation { get; set; }
    }
}
