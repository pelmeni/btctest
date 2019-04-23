using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Data
{
    public class Transaction
    {
        public string id { get; set; }
        public DateTime txtime { get; set; }
        public string category { get; set; }
        public string wallettag { get; set; }
        public string address { get; set; }
        public double amount { get; set; }
        public double? fee { get; set; }
        public int confirmations { get; set; }
        public DateTime created { get; set; }
    }
}
