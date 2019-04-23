using System;
using System.Collections.Generic;
using System.Linq;
using BtcWebSrvApp.API;
using BtcWebSrvApp.Data;
using Dapper;


namespace BtcWebSrvApp.Business.Dapper
{
    public class WalletAddressOperations : BaseDapperConnection
    {
        public WalletAddressOperations(IConnectionStringProvider provider) : base(provider)
        {
        }
        public bool Exist(string address)
        {
            return ExecuteQuery(db => db.Query<WalletAddress>("select top 1 address from dbo.[walletaddress] where address = @address", new { address })?.FirstOrDefault() != null);
        }
        public void Add(WalletAddress wa)
        {
            try
            {
                Execute(db =>
            {

                db.Execute("INSERT INTO [dbo].[walletaddress] ([address],[wallettag],[walleturl],[balance],[updated]) VALUES(@address, @wallettag, @walleturl, @balance, @updated)",
                    new { address = wa.address, wallettag = wa.wallettag, walleturl = wa.walleturl, balance = wa.balance, updated = DateTime.Now });
            });
            }catch(Exception ex)
            {

            }
        }
        public WalletAddress Get(string address)
        {
            return ExecuteQuery(db => db.Query<WalletAddress>("select * from [dbo].[walletaddress] where address = @address", new { address })?.FirstOrDefault());
        }
        public void UpdateBalance(string address, double balance)
        {
            Execute(db =>
            {
                db.Execute("update [dbo].[walletaddress] set [balance] = @balance, [updated]=getdate() [address]=@address", new { address, balance });
            });
        }
        public IEnumerable<WalletAddress> GetAll(string wallettag, string nodeurl)
        {
            return ExecuteQuery(db => db.Query<WalletAddress>("select * from dbo.[walletaddress] where wallettag=@wallettag and walleturl=@nodeurl",new { wallettag, nodeurl})).ToArray();
        }
        public void AddIfNotExist(WalletAddress wa)
        {
            if (!Exist(wa.address))
                Add(new WalletAddress()
                {
                    address = wa.address,
                    wallettag = wa.wallettag,
                    walleturl = wa.walleturl,
                    updated = DateTime.Now
                });
        }

       
    }
}
