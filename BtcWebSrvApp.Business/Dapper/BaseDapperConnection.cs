using BtcWebSrvApp.API;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace BtcWebSrvApp.Business.Dapper
{
    public class BaseDapperConnection
    {
        public BaseDapperConnection(IConnectionStringProvider provider)
        {
            ConnectionString = provider.ConnectionString;
        }
        public string ConnectionString { get; private set; }


        public void Execute(Action<IDbConnection> action)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                try
                {
                    db.Open();

                    action(db);

                }
                finally
                {
                    if (db.State != ConnectionState.Closed)
                        db.Close();
                }
            }
        }

        public T ExecuteQuery<T>(Func<IDbConnection, T> action)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                try{
                    db.Open();
                    

                    return action(db);

                }
                finally
                {
                    if (db.State != ConnectionState.Closed)
                        db.Close();
                }
            }
        }
        public T ExecuteQueryOpenTrunc<T>(Func<IDbConnection, T> action, out IDbTransaction trunc)
        {
            trunc = null;

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
             
                try
                {
                    db.Open();

                    trunc = db.BeginTransaction();

                    return action(db);

                }
                finally
                {
                    if(trunc==null)
                        if (db.State != ConnectionState.Closed)
                            db.Close();
                }
            }
        }
        public Task<T> ExecuteQueryAsync<T>(Func<IDbConnection,Task<T>> action)
        {
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                db.Open();


                return action(db);

            }
        }
    }
}
