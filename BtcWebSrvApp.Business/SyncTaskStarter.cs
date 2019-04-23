using BtcWebSrvApp.API;
using BtcWebSrvApp.Business.Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcWebSrvApp.Business
{
    public  class SyncTaskStarter
    {
        public SyncTaskStarter(IConnectionStringProvider provider)
        {

            var wcops = new WalletContextOperations(provider);

            var waops = new WalletAddressOperations(provider);

            var trops = new TransactionOperations(provider);

            _syops = new SyncOperations(wcops, trops, waops);

        }
        private  readonly SyncOperations _syops;


        public  async Task StartSync()
        {
            await Task.Run(async ()=>
            {
                while (true)
                {
                    try
                    {
                        _syops.SyncWalletsInfo();


                        await _syops.SyncAddressesAsync();

                        Debug.WriteLine("sync addresses completed");

                        await _syops.SyncTransactionsAsync();

                        Debug.WriteLine("sync transactions completed");

                        await Task.Delay(30 * 1000);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            });
        }
    }
}
