using System;
using System.Data;

//using System.Data.SqlClient;

namespace Extensions.DB2Persistence
{
    //TODO: Create XML summary comment for ReusableTransactionPolicy
    [Serializable]
    public class ReusableTransactionPolicy : ExecutionPolicy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void Commit( IDbTransaction txn )
        {
        }

        public override void Close( IDbCommand cmd )
        {
            if( null != cmd )
            {
                cmd.Dispose();
            }
        }

        public override IDbCommand CommandFor( string storedProcName )
        {
            IDbCommand cmd = base.CommandFor( storedProcName );
            cmd.Transaction = this.CurrentTransaction();
            

            return cmd;
        }

        public override IDbCommand CommandFor( string storedProcName, CommandType cmdType )
        {
            IDbCommand cmd = base.CommandFor( storedProcName, cmdType );
            cmd.Transaction = this.CurrentTransaction();
            
            return cmd;
        }

        public override SafeReader ExecuteReader( IDbCommand cmd )
        {
            return new SafeReader( cmd.ExecuteReader() );
        }

        public override IDbConnection NewConnection()
        {
            return this.CurrentTransaction().Connection;
        }
        
        public override IDbTransaction NewTransaction()
        {
            return this.CurrentTransaction();
        }

        public override void Rollback( IDbTransaction txn )
        {
        }

        public override IDbTransaction TransactionFor( IDbCommand cmd )
        {
            IDbTransaction txn = this.CurrentTransaction();
            if( null != cmd && 
                null != cmd.Connection && 
                null == cmd.Transaction )
            {
                cmd.Transaction = txn;
            }
            return txn;
        }
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        private IDbTransaction CurrentTransaction()
        {
            return this.Broker.Transaction;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ReusableTransactionPolicy( AbstractBroker broker )
            : base( broker )
        {
        }
        #endregion

        #region Data Elements
        #endregion
    }
}
