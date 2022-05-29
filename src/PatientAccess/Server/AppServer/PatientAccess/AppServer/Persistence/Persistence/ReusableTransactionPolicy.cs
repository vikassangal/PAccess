using System;
using System.Data.SqlClient;

namespace Extensions.Persistence
{
    //TODO: Create XML summary comment for ReusableTransactionPolicy
    [Serializable]
    public class ReusableTransactionPolicy : ExecutionPolicy
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void Commit( SqlTransaction txn )
        {
        }

        public override void Close( SqlCommand cmd )
        {
            if( null != cmd )
            {
                cmd.Dispose();
            }
        }

        public override SqlCommand CommandFor( string storedProcName )
        {
            SqlCommand cmd = base.CommandFor( storedProcName );
            cmd.Transaction = this.CurrentTransaction();
            return cmd;
        }

        public override SafeReader ExecuteReader( SqlCommand cmd )
        {
            return new SafeReader( cmd.ExecuteReader() );
        }

        public override SqlConnection NewConnection()
        {
            return this.CurrentTransaction().Connection;
        }
        
        public override SqlTransaction NewTransaction()
        {
            return this.CurrentTransaction();
        }

        public override void Rollback( SqlTransaction txn )
        {
        }

        public override SqlTransaction TransactionFor( SqlCommand cmd )
        {
            SqlTransaction txn = this.CurrentTransaction();
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
        private SqlTransaction CurrentTransaction()
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
