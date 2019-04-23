using BtcWebSrvApp.API;
using BtcWebSrvApp.Business;
using BtcWebSrvApp.Business.Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Helpers;
using System.Web.Http;

namespace BtcWebSrvApp.Controllers
{
    public class BtcSrvController : ApiController
    {
        private readonly WalletContextOperations _wcops;
        private readonly WalletAddressOperations _waops;
        private readonly TransactionOperations _trops;
        private DateTime _getLastLastCalledUtc;
        static int send_cnt_calls=0;
        public BtcSrvController(WalletContextOperations wcops, WalletAddressOperations waops, TransactionOperations trops)
        {
            this._wcops = wcops;
            this._waops = waops;
            this._trops = trops;

            _getLastLastCalledUtc = DateTime.UtcNow;

        }
        [HttpGet]
        [Route("api/btcsrv/getlast")]
        public IHttpActionResult GetLast()
        {
            try
            {
                Debug.WriteLine("call to api/btcsrv/getlast");

                var result = _trops
                    .GetList("receive", _getLastLastCalledUtc, 3)
                    .Select(i => new BtcTransaction()
                    {
                        address = i.address,
                        amount = i.amount,
                        confirmation = i.confirmations,
                        date = i.txtime
                    }).ToArray();

                _getLastLastCalledUtc = DateTime.UtcNow;

                return Json(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpPost]
        [Route("api/btcsrv/sendbtc")]
        public void SendBtc([FromBody]SendBtcParams data)
        {
            Debug.WriteLine("call to api/btcsrv/sendbtc");

            try
            {
                
                if (!Regex.IsMatch(data.toaddress, "[13][a-km-zA-HJ-NP-Z1-9]{25,34}$"))
                    throw new ApplicationException("btc address is not valid");


                var wallets_btcops = _wcops.GetAll().Select(i => new BtcCliOperations(i)).ToArray();

                var task = new Action(()=> {

                    foreach (var wbop in wallets_btcops)
                    {
                        try
                        {
                            
                            var tx=wbop.SendToAddress(data.toaddress, data.amount);

                            //TODO: validate tx we got the correct answer
                                                     
                            Interlocked.Increment(ref send_cnt_calls);

                            Debug.WriteLine($"call to api/btcsrv/sendbtc completed ({send_cnt_calls}-{tx})");

                            break; //exit when send btc w/o error

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());

                            Debug.WriteLine("call to api/btcsrv/sendbtc failed");
                        }
                    }
    
                });

                SendBtcQueue.EnqueueTask(task);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
