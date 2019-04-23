using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Data
{
    public class WalletContext
    {
        public string Tag { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Passsword { get; set; }

        public int? TxCount { get; set; }

        public double? Balance { get; set; }


    }
}
