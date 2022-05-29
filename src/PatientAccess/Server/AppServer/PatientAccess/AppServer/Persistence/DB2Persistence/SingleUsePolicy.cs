using System;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Extensions.DB2Persistence
{
    //TODO: Create XML summary comment for SingleUsePolicy
    [Serializable]
    public class SingleUsePolicy : ExecutionPolicy
    {
        #region Constants
        #endregion

        #region Event Handlers
        #endregion

        #region Methods
        public override void Close( IDbCommand cmd )
        {
            if( null != cmd )
            {
                if (cmd.Connection != null &&
                    cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Connection.Close();
                }
                cmd.Dispose();
            }
        }

        public override void Commit( IDbTransaction txn )
        {
            if( null != txn )
            {
                txn.Commit();
                txn.Dispose();
            }
        }

        public override SafeReader ExecuteReader( IDbCommand cmd )
        {
            CommandBehavior action;
            if( null != cmd.Transaction )
            {
                action = CommandBehavior.Default;
            }
            else
            {
                action = CommandBehavior.CloseConnection;
            }
            return new SafeReader(cmd.ExecuteReader(action));
        }

        public override IDbConnection NewConnection()
        { 
            IDbConnection cxn = new iDB2Connection( this.Broker.ConnectionString );
            cxn.Open();
            return cxn; 
        }

        public override IDbTransaction NewTransaction()
        {
            return this.NewConnection().BeginTransaction();
        }


        public override void Rollback( IDbTransaction txn )
        {
            if( null != txn )
            {
                // 2-14-2007 - kjs - If you call rollback on an IBM transaction
                // it renderes the connection invalid. It does this by loosing 
                // the connection. You have to manually close this connection or
                // else you will leave a connection hung open.

                IDbConnection conn = txn.Connection;
                
                txn.Rollback();
                txn.Dispose();
                if(conn != null)
                {
                    conn.Close();
                }
            }
        }

        public override IDbTransaction TransactionFor( IDbCommand cmd )
        {
            IDbTransaction txn = null;
            if( null != cmd && null != cmd.Connection )
            {
                txn = cmd.Connection.BeginTransaction();
                cmd.Transaction = txn;
            }
            return txn;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public SingleUsePolicy( AbstractBroker broker )
            : base( broker )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
