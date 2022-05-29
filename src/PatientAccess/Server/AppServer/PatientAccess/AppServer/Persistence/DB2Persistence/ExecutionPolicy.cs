using System;
using System.Configuration;
using System.Data;
using IBM.Data.DB2.iSeries;

namespace Extensions.DB2Persistence
{
    //TODO: Create XML summary comment for ExecutionPolicy
    [Serializable]
    public abstract class ExecutionPolicy : object
    {
		private const string TIMEOUT_PROPERTY_NAME = "CommandTimeout";
		private const int DEFAULT_TIMEOUT = 30;
        #region Event Handlers
        #endregion

        #region Methods
        public abstract void Close( IDbCommand cmd );

        public virtual void Close( IDataReader reader )
        {
            if( null != reader )
            {
                reader.Close();
                reader.Dispose();
            }
        }

        public abstract void Commit( IDbTransaction txn );

        public virtual IDbCommand CommandFor( string storedProcName )
        {
            IDbCommand cmd = new iDB2Command( storedProcName, 
                (iDB2Connection)this.NewConnection() );
			cmd.CommandTimeout = GetTimeout();
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public virtual IDbCommand CommandFor( string cmdtext, CommandType cmdType )
        {
            iDB2Command cmd = 
                new iDB2Command( cmdtext,cmdType,
                this.NewConnection());
            cmd.CommandTimeout = GetTimeout();

            return cmd;
        }

        public abstract SafeReader ExecuteReader( IDbCommand cmd );
        public abstract IDbConnection  NewConnection();
        public abstract IDbTransaction NewTransaction();
        public abstract void Rollback( IDbTransaction txn );
        public abstract IDbTransaction TransactionFor( IDbCommand cmd );
        #endregion

        #region Properties
        #endregion

        #region Private Methods
		private int GetTimeout()
		{
			int timeout = DEFAULT_TIMEOUT;

            string timeoutStr = ConfigurationManager.AppSettings[TIMEOUT_PROPERTY_NAME];
			if(timeoutStr != null)
				timeout = int.Parse(timeoutStr);

			return timeout;
		}
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
        protected ExecutionPolicy( AbstractBroker broker )
        {
            this.Broker = broker;
        }
        #endregion

        #region Data Elements
        private AbstractBroker i_Broker;
        #endregion
    }
}
