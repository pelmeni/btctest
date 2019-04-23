using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BtcWebSrvApp.API;
using BtcWebSrvApp.Business;
using BtcWebSrvApp.Business.Dapper;
using BtcWebSrvApp.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetWalletInfo_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";
            var ops = new BtcCliOperations(wc1);
            var r = ops.GetDefaultWalletInfo();
        }
        [TestMethod]
        public void GetNewAddressForLabel_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";
            var ops = new BtcCliOperations(wc1);
            var r = ops.GetNewAddressForLabel();
        }
        [TestMethod]
        public void GetAddresses_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";
            var ops = new BtcCliOperations(wc1);
            var r = ops.GetAddresses();
        }
        [TestMethod]
        public void GetDefaultWalletBalance_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";
            var ops = new BtcCliOperations(wc1);

            var dwb = ops.GetDefaultWalletBalance();
        }
        [TestMethod]
        public void SendToAddress_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";
            var ops = new BtcCliOperations(wc1);

            var total = ops.GetDefaultWalletBalance();

            var addrs = ops.GetAddresses();

            var addr = addrs.ToArray();

            var testAmount = total / addrs.Count();

            ops.SendToAddress(addr[0],testAmount.Value);
        }


        [TestMethod]
        public void Interaction01_Call_Success()
        {
            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.56.101:11111";
            wc1.User = "btcuser";
            wc1.Passsword = "btcpwd";

            var wc2 = new WalletContext();
            wc2.Tag = "btccliwebsrv2";
            wc2.Url = "http://192.168.56.101:11112";
            wc2.User = "btcuser";
            wc2.Passsword = "btcpwd";

            var ops1 = new BtcCliOperations(wc1);

            var ops2 = new BtcCliOperations(wc2);

            var i = 0;
            try { i = ops1.GetAddresses().Count(); } catch (Exception ex) { Debug.WriteLine(ex.ToString()); }

            while (i < 5)
            {
                try
                {
                    var a = ops1.GetNewAddressForLabel();
                    i++;
                    Debug.WriteLine($"gen address {a} for wallet with tag:{wc1.Tag}");
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex.ToString());
                }
            }
            i = 0;
            try { i = ops2.GetAddresses().Count(); } catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            while (i < 5)
            {
                try
                {
                    var a = ops2.GetNewAddressForLabel();
                    i++;
                    Debug.WriteLine($"gen address {a} for wallet with tag:{wc2.Tag}");
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex.ToString());
                }
            }

            Debug.WriteLine($"wallet with tag:{wc1.Tag} addresses:");
            foreach (var a in ops1.GetAddresses())
            {
                Debug.WriteLine($"{a}");
            }
            Debug.WriteLine($"wallet with tag:{wc2.Tag} addresses:");
            foreach (var a in ops2.GetAddresses())
            {
                Debug.WriteLine($"{a}");
            }

            //var total = ops.GetDefaultWalletBalance();

            //var addrs = ops.GetAddresses();

            //var addr = addrs.ToArray();

            //var testAmount = total / addrs.Count();

            //ops.SendToAddress(addr[0], testAmount.Value);
        }


        [TestMethod]
        public void InitWalletsInfo_Call_Success()
        {
            //add hot wallets manually

            var wc1 = new WalletContext();
            wc1.Tag = "btccliwebsrv";
            wc1.Url = "http://192.168.0.45:11111";
            wc1.User = "btcusr";
            wc1.Passsword = "btcpwd";




            var wc2 = new WalletContext();
            wc2.Tag = "btccliwebsrv2";
            wc2.Url = "http://192.168.0.45:11112";
            wc2.User = "btcusr";
            wc2.Passsword = "btcpwd";

            var provider = new WebAppConnectionStringProvider();

            var wcops = new WalletContextOperations(provider);

            var waops = new WalletAddressOperations(provider);

            var trops = new TransactionOperations(provider);

            if (!wcops.Exist(wc1))
                wcops.Add(wc1);

            if (!wcops.Exist(wc2))
                wcops.Add(wc2);

            //sync hot wallet's adresses
            var wallets = wcops.GetAll();

            foreach(var w in wallets)
            {
                var btcops = new BtcCliOperations(w);

                try {

                   var walletAddressCount = 0;

                   try { walletAddressCount = btcops.GetAddresses().Count(); } catch (Exception ex) {/*if no addresses in wallet*/ Debug.WriteLine(ex.ToString()); }

                    //add initial addresses in wallets if 
                    while (walletAddressCount < 5)
                    {
                        try
                        {
                            var a = btcops.GetNewAddressForLabel();


                            walletAddressCount++;

                            Debug.WriteLine($"gen address {a} for wallet with tag:{w.Tag}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }

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
                        if (!waops.Exist(a))
                            waops.Add(new WalletAddress()
                            {
                                address = a,
                                wallettag = w.Tag,
                                walleturl = w.Url,
                                updated = DateTime.Now
                            });
                    }

                    //fill test balances if zeroes and node total exits
                    double? total = null;

                    try
                    {
                        total = btcops.GetDefaultWalletBalance();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }

                    if (!total.HasValue || total.Value==0.0)
                    {
                        btcops.GenTestCoins();
                    }
                    var retries = 3;
                    while (total==0.0 && retries-- >= 3)
                    {
                        try
                        {
                            total = btcops.GetDefaultWalletBalance();

                            break;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }

                    if (total.HasValue && total.Value > 0)
                        foreach (var wadr in waops.GetAll(w.Tag, w.Url))
                        {
                            if (wadr.balance != 0)
                                continue;

                            var testAmount = total / addrs.Count();

                            try
                            {
                                btcops.SendToAddress(wadr.address, testAmount.Value);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }
                        }


                    //sync transactions with db
                    List<Transaction> trList = new List<Transaction>();
                    try
                    {
                        trList = btcops.ListTransactions().ToList();

                        trops.AddOrUpdateConfirmations(trList);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            }

            //test sending coins 
            foreach (var fromwallet in wallets)
            {
                var btcops = new BtcCliOperations(fromwallet);

                foreach (var to_wallet in wallets.Where(i => i.Tag != fromwallet.Tag && i.Url != fromwallet.Url).ToArray())
                {

                    var fromwallet_balance = btcops.GetDefaultWalletBalance();

                    if (fromwallet_balance == 0)
                        continue;

                    var testAmount = 0.001;

                    var to_addresses = waops.GetAll(to_wallet.Tag, to_wallet.Url);

                    foreach (var a in to_addresses)
                    {
                        try
                        {
                            btcops.SendToAddress(a.address, testAmount);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                }
            }

        }

    }
}
