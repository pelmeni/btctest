using BtcWebSrvApp.API;
using BtcWebSrvApp.Business.Dapper;
using BtcWebSrvApp.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Business
{
    public class SyncOperations
    {
        private readonly WalletContextOperations _wcops;
        private readonly TransactionOperations _trops;
        private readonly WalletAddressOperations _waops;

        public SyncOperations(WalletContextOperations wcops, TransactionOperations trops, WalletAddressOperations waops)
        {
            this._wcops = wcops;
            this._trops = trops;
            this._waops = waops;
        }
        public void SyncAddresses()
        {
            var wallets = _wcops.GetAll();

            foreach (var w in wallets)
            {
                var btcops = new BtcCliOperations(w);

                try
                {
                    var walletAddressCount = 0;

                    try { walletAddressCount = btcops.GetAddresses().Count(); } catch (Exception ex) {/*if no addresses in wallet*/ Debug.WriteLine(ex.ToString()); }

                    //sync with db

                    IEnumerable<string> addrs = new List<string>();

                    try
                    {
                        addrs = btcops.GetAddresses();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    foreach (var a in addrs)
                    {
                            _waops.AddIfNotExist(new WalletAddress()
                            {
                                address = a,
                                wallettag = w.Tag,
                                walleturl = w.Url,
                                updated = DateTime.Now
                            });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
        public async Task SyncAddressesAsync()
        {
            await Task.Run(() => SyncAddresses());
        }
        public void SyncTransactions()
        {
            //sync transactions with db

            var wallets = _wcops.GetAll();

            foreach (var w in wallets)
            {
                var btcops = new BtcCliOperations(w);

                var trList = new List<Transaction>();

                try
                {
                    trList = btcops.ListTransactions().ToList();

                    _trops.AddOrUpdateConfirmations(trList);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

            }
        }
        public void SyncWalletsInfo()
        {
            //sync transactions with db

            var wallets = _wcops.GetAll();

            foreach (var w in wallets)
            {
                var btcops = new BtcCliOperations(w);

               try
                {
                    var b = btcops.GetDefaultWalletBalance();

                    var txcnt = btcops.GetDefaultWalletTxCount();

                    _wcops.UpdateInfo(w, b, txcnt);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

            }
        }
        public async Task SyncTransactionsAsync()
        {
            await Task.Run(() => SyncTransactions());
        }
    }
}
