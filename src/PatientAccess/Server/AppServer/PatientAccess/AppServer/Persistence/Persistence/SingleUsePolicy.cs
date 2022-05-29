using System;
using System.Data;
using System.Data.SqlClient;

namespace Extensions.Persistence
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
        public override void Close( SqlCommand cmd )
        {
            if( null != cmd )
            {
                cmd.Connection.Close();
                cmd.Dispose();
            }
        }

        public override void Commit( SqlTransaction txn )
        {
            if( null != txn )
            {
                txn.Commit();
                txn.Dispose();
            }
        }

        public override SafeReader ExecuteReader( SqlCommand cmd )
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
            return new SafeReader( cmd.ExecuteReader( action ) );
        }

        public override SqlConnection NewConnection()
        {
            SqlConnection cxn = new SqlConnection( this.Broker.ConnectionString );
            cxn.Open();
            return cxn;
        }

        public override SqlTransaction NewTransaction()
        {
            return this.NewConnection().BeginTransaction();
        }


        public override void Rollback( SqlTransaction txn )
        {
            if( null != txn )
            {
                txn.Rollback();
                txn.Dispose();
            }
        }

        public override SqlTransaction TransactionFor( SqlCommand cmd )
        {
            SqlTransaction txn = null;
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
