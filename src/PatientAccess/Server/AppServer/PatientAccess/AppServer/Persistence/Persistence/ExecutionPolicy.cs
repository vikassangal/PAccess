using System;
using System.Data;
using System.Data.SqlClient;

namespace Extensions.Persistence
{
    //TODO: Create XML summary comment for ExecutionPolicy
    [Serializable]
    public abstract class ExecutionPolicy : object
    {
        #region Event Handlers
        #endregion

        #region Methods
        public abstract void Close( SqlCommand cmd );

        public virtual void Close( IDataReader reader )
        {
            if( null != reader )
            {
                reader.Close();
                reader.Dispose();
            }
        }

        public abstract void Commit( SqlTransaction txn );

        public virtual SqlCommand CommandFor( string storedProcName )
        {
            SqlCommand cmd = new SqlCommand( storedProcName, this.NewConnection() );
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public abstract SafeReader ExecuteReader( SqlCommand cmd );
        public abstract SqlConnection  NewConnection();
        public abstract SqlTransaction NewTransaction();
        public abstract void Rollback( SqlTransaction txn );
        public abstract SqlTransaction TransactionFor( SqlCommand cmd );
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        protected AbstractBroker Broker
        {
            get
            {
                return i_Broker;
            }
            private set
            {
                i_Broker = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public ExecutionPolicy( AbstractBroker broker )
        {
            this.Broker = broker;
        }
        #endregion

        #region Data Elements
        private AbstractBroker i_Broker;
        #endregion
    }
}
