using System;
using System.Collections.Generic;
using System.Linq;
using BtcWebSrvApp.API;
using BtcWebSrvApp.Data;
using Dapper;


namespace BtcWebSrvApp.Business.Dapper
{
    public class WalletContextOperations : BaseDapperConnection
    {
        public WalletContextOperations(IConnectionStringProvider provider) : base(provider)
        {
        }
        public bool Exist(WalletContext wc)
        {
            return ExecuteQuery(db => db.Query<WalletAddress>("select top 1 * from dbo.[wallet] where wallettag=@tag and nodeurl=@url", new { tag = wc.Tag, url = wc.Url })?.FirstOrDefault() != null);
        }

        public void Add(WalletContext wc)
        {
            Execute(db =>
            {
                db.Execute("INSERT INTO [dbo].[wallet] ([wallettag],[nodeurl],[rpcuser],[rpcpassword]) VALUES(@wallettag, @nodeurl, @rpcuser, @rpcpassword)", new { wallettag=wc.Tag, nodeurl=wc.Url, rpcuser=wc.User, rpcpassword=wc.Passsword });
            });
        }
        public IEnumerable<WalletContext> GetAll()
        {
            return ExecuteQuery(db => db.Query<dynamic>("select * from dbo.[wallet]")).Select(i => new WalletContext()
            {
                Tag=i.wallettag,
                Url= i.nodeurl,
                User= i.rpcuser,
                Passsword= i.rpcpassword,
                Balance=i.balance,
                TxCount=i.txcount
            }).ToArray();
        }
        

        internal void UpdateInfo(WalletContext wc, double? balance, int txcount)
        {
            Execute(db =>
            {
                db.Execute("update [dbo].[wallet] set [balance] = @balance, [txcount]=@txcount where wallettag=@tag and nodeurl=@url", new { balance, txcount, tag = wc.Tag, url = wc.Url });
            });
        }
    }
}
