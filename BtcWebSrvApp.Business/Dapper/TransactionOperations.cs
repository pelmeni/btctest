using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BtcWebSrvApp.API;
using BtcWebSrvApp.Data;
using Dapper;


namespace BtcWebSrvApp.Business.Dapper
{
    public class TransactionOperations : BaseDapperConnection
    {
        public TransactionOperations(IConnectionStringProvider provider) : base(provider)
        {
        }
        public bool Exist(string id, string category)
        {
            return ExecuteQuery(db => db.Query<Transaction>("select top 1 id from [dbo].[transaction] where id = @id and category=@category", new { id, category })?.FirstOrDefault() != null);
        }

        public void Add(Transaction transaction)
        {
            Execute(db =>{db.Execute("INSERT INTO [dbo].[transaction]([id],[txtime],[category],[wallettag],[address],[amount],[fee],[confirmations]) VALUES(@id, @txtime, @category, @wallettag, @address, @amount, @fee, @confirmations)"
                    , transaction);});
        }
        public void AddOrUpdateConfirmations(IEnumerable<Transaction> transactions)
        {
            var errors = new StringBuilder();

            foreach(var t in transactions)
            {
                try
                {
                    if (!Exist(t.id, t.category))
                        Add(t);
                    else
                        UpdateConfirmationsIfDiffers(t.id, t.category, t.confirmations);
                }
                catch(Exception ex)
                {
                    errors.AppendLine(ex.ToString());
                }
            }
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ApplicationException(errors.ToString());
        }

        public Transaction Get(string id, string category)
        {
            return ExecuteQuery(db => db.Query<Transaction>("select * from [dbo].[transaction] where id = @id and category = @category", new { id, category })?.FirstOrDefault());
        }
        public void UpdateConfirmationsIfDiffers(string id, string category, int confirmations)
        {
            Execute(db =>
            {
                db.Execute("UPDATE [dbo].[transaction] set confirmations=@confirmations WHERE id=@id and confirmations<>@confirmations", new { id, category, confirmations });
            });
        }
        public IEnumerable<Transaction> GetList(string category)
        {
            return ExecuteQuery(db => db.Query<Transaction>("select * from [dbo].[transaction] where category = @category", new { category })?.ToArray());
        }
        public IEnumerable<Transaction> GetList(string category, DateTime limitTime, int limitConfs)
        {
            return ExecuteQuery(db => db.Query<Transaction>("select * from [dbo].[transaction] where category = @category and confirmations< @limitconfs union select * from [dbo].[transaction] where category = @category and created> @limitTime", new { category, limitTime, limitConfs })?.ToArray());
        }
    }
}
