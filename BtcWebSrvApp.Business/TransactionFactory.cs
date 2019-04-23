using BtcWebSrvApp.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Business
{
    public class TransactionFactory
    {
        private TransactionFactory() { }

        public static Transaction From(JToken tx)
        {
            var rawTxId = tx["txid"].ToString();

            var rawAddress = tx["address"]?.Value<string>();

            var rawCategory = tx["category"].Value<string>();

            var rawAmount = tx["amount"].Value<double>();

            var rawFee = tx["fee"]?.Value<double>();

            var rawConfirmations = tx["confirmations"].Value<int>();

            var rawLabel = tx["label"]?.Value<string>();

            var rawTimeRecievedSecondsSinceEpoch = tx["timereceived"].Value<int>();

            //in utc
            var rawTimeRecievedAsDatetime = new DateTime(1970, 1, 1).ToUniversalTime().AddSeconds(rawTimeRecievedSecondsSinceEpoch);

            var tr = new Transaction()
            {
                address = rawAddress,
                amount = rawAmount,
                category = rawCategory,
                confirmations = rawConfirmations,
                fee = rawFee,
                id = rawTxId,
                txtime = rawTimeRecievedAsDatetime,
                wallettag = rawLabel
            };

            return tr;
        }
        
    }
}
