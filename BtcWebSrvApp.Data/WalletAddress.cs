using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Data
{
    public class WalletAddress
    {
        public string address { get; set; }

        public string wallettag { get; set; }

        public string walleturl { get; set; }

        public double balance { get; set; }

        public DateTime updated { get; set; }

        public DateTime created { get; set; }
    }
}
