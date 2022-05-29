using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Extensions.Exceptions;

namespace Extensions.Persistence
{
    /// <summary>
    /// AbstractBroker serves as a base class for all classes that provide
    /// data services to an application.  Subclasses will typically define
    /// a custom interface for retrieving and storing data.  The base class
    /// provides interfaces for services required by all concrete subclasses.
    /// </summary>
    [Serializable]
    abstract public class AbstractBroker : MarshalByRefObject
    {
        #region Constants

        private const int CLASS_ID_CONCURRENCY = 99999,
                           DEFAULT_TIMEOUT      = 30;
        private const string 
            CONTEXT_PREFIX          = "PersistenceError.",
            CONTEXT_PROCEDURE       = "ProcedureName",
            CXN_PROPERTY_NAME       = "ConnectionString",
            NULL_INDICATOR          = "<Null>",
            SETTING_DEFAULT_TIMEOUT = "SQL.Timeout";
        
        private const string
            ERROR_MSG_CONCURRENCY       = "Concurrency error: {0}.",
            ERROR_MSG_GENERAL_SQL_ERROR = "General SQL Server Error : {0}";
        
        #endregion

        #region Methods
        #endregion

        #region Properties
        /// <summary>
        /// Provides the default connection string used by the broker.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return i_ConnectionString;
            }
            private set
            {
                i_ConnectionString = value;
            }
        }

        /// <summary>
        /// The policy enforcing rules regarding database related objects
        /// and methods to ensure transactional integrity and connection
        /// stability.
        /// </summary>
        private ExecutionPolicy Policy
        {
            get
            {
                return i_Policy;
            }
            set
            {
                i_Policy = value;
            }
        }

        /// <summary>
        /// Transaction currently associated with the broker if one is present.
        /// </summary>
        public SqlTransaction Transaction
        {
            get
            {
                return i_Transaction;
            }
            private set
            {
                i_Transaction = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method provides a SqlAdapter object that will execute a given
        /// stored procedure.  The command includes a predefined connection
        /// that is open and ready for execution.  The consumer must close
        /// the database connection explicitly.  The consumer must also
        /// provide any parameter information required by the stored procedure.
        /// </summary>
        /// <param name="storedProcName">
        /// The name of a stored procedure in the default database.
        /// </param>
        /// <returns>
        /// An instance of System.Data.SqlClient.SqlCommand.
        /// </returns>
        protected SqlDataAdapter AdapterFor( string storedProcName )
        {
            SqlDataAdapter adapter = new SqlDataAdapter( storedProcName, 
                                                         this.Policy.NewConnection() );
            adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            return adapter;
        }

        /// <summary>
        /// Close the provided command based on my current policy.
        /// </summary>
        /// <param name="cmd">
        /// A command to close.
        /// </param>
        protected void Close( SqlCommand cmd )
        {
            this.Policy.Close( cmd );
        }

        /// <summary>
        /// Close the provided reader based on my current policy.
        /// </summary>
        /// <param name="reader">
        /// A reader to close.
        /// </param>
        protected void Close( IDataReader reader )
        {
            this.Policy.Close( reader );
        }

        /// <summary>
        /// Answer a command object that will execute a given
        /// stored procedure.  The command includes a predefined connection
        /// that is open and ready for execution.  The consumer must close
        /// the database connection explicitly.  The consumer must also
        /// provide any parameter information required by the stored procedure.
        /// </summary>
        /// <param name="storedProcName">
        /// The name of a stored procedure in the default database.
        /// </param>
        /// <returns>
        /// An instance of System.Data.SqlClient.SqlCommand.
        /// </returns>
        protected SqlCommand CommandFor( string storedProcName )
        {
            SqlCommand cmd = this.Policy.CommandFor( storedProcName );
            cmd.CommandTimeout = this.Timeout;
            return cmd;
        }

        /// <summary>
        /// Commit the provided transaction based on my current policy.
        /// </summary>
        /// <param name="txn"></param>
        protected void Commit( SqlTransaction txn )
        {
            this.Policy.Commit( txn );
        }

        /// <summary>
        /// Answer an EnterpriseException loaded with context information.
        /// </summary>
        /// <param name="sqlError">
        /// An original Sql exception that was thrown by the database engine.
        /// </param>
        /// <param name="cmd">
        /// A command object with context information.
        /// </param>
        /// <returns>
        /// An Enterprise Exception with context loaded.
        /// </returns>
        private EnterpriseException CreateEnterpriseExceptionUsing( SqlException sqlError, SqlCommand cmd )
        {
            EnterpriseException error = null;
            if( this.IsConcurrencyError( sqlError ) )
            {
                error = new RecordContentionException( 
                    String.Format( ERROR_MSG_CONCURRENCY, sqlError.Message ), 
                    sqlError, 
                    Severity.Medium );
            }
            else
            {
                error = new DatabaseServiceException( 
                    String.Format( ERROR_MSG_GENERAL_SQL_ERROR, sqlError.Message ), 
                    sqlError, 
                    Severity.Catastrophic );
            }
            this.LoadExceptionContextFrom( error, cmd );

            return error;
        }

        
        /// <summary>
        /// Answer a number of records affected by the execution of a command.
        /// </summary>
        /// <param name="cmd">
        /// A command to use as the source of the resulting data.
        /// </param>
        /// <returns>
        /// Number of records affected
        /// </returns>
        protected int ExecuteNonQuery( SqlCommand cmd )
        {
            int recordsAffected = 0;
            try
            {
                recordsAffected = cmd.ExecuteNonQuery();
            }
            catch( SqlException sqlError )
            {
                EnterpriseException error = this.CreateEnterpriseExceptionUsing( sqlError, cmd );
                throw error;
            }

            return recordsAffected;
        }

        /// <summary>
        /// Answer a reader resulting from the execution of a command based 
        /// on my current policy.
        /// </summary>
        /// <param name="cmd">
        /// A command to use as the source of the resulting data.
        /// </param>
        /// <returns>
        /// A SafeReader.
        /// </returns>
        protected SafeReader ExecuteReader( SqlCommand cmd )
        {
            //SafeReader reader = null;
            try
            {
                return this.Policy.ExecuteReader( cmd );
            }
            catch( SqlException sqlError )
            {
                EnterpriseException error = this.CreateEnterpriseExceptionUsing( sqlError, cmd );
                throw error;
            }
        }

        /// <summary>
        /// Answer true if the exception provided represents a concurrency error.
        /// </summary>
        /// <param name="error">
        /// The exception to check.
        /// </param>
        /// <returns>
        /// True if the exception is a concurrency error.
        /// </returns>
        private bool IsConcurrencyError( SqlException error )
        {
            return error.Number == CLASS_ID_CONCURRENCY;
        }

        /// <summary>
        /// Answer an EnterpriseException loaded with context information.
        /// </summary>
        /// <param name="error">
        /// An exception that will be loaded with context information.
        /// </param>
        /// <param name="cmd">
        /// A command object with context information.
        /// </param>
        /// <returns>
        /// A loaded exception.
        /// </returns>
        private EnterpriseException LoadExceptionContextFrom( EnterpriseException error,
            SqlCommand cmd )
        {
            if( cmd != null )
            {
                error.AddContextItem( CONTEXT_PREFIX + CONTEXT_PROCEDURE, cmd.CommandText );
                foreach( SqlParameter parameter in cmd.Parameters )
                {
                    if( parameter.Value != null )
                    {
                        error.AddContextItem( CONTEXT_PREFIX + parameter.ParameterName, 
                            parameter.Value.ToString() );
                    }
                    else
                    {
                        error.AddContextItem( CONTEXT_PREFIX + parameter.ParameterName, 
                            NULL_INDICATOR);
                    }
                }
            }
            return error;
        }

        /// <summary>
        /// Answer a new open, connection based on my current policy.
        /// </summary>
        /// <returns></returns>
        protected SqlConnection NewConnection()
        {
            return this.Policy.NewConnection();
        }

        protected SqlTransaction NewTransaction()
        {
            return this.Policy.NewTransaction();
        }

        /// <summary>
        /// Roll back the specified transaction based on my current policy.
        /// </summary>
        /// <param name="txn">
        /// The transaction to roll back.
        /// </param>
        protected void Rollback( SqlTransaction txn )
        {
            this.Policy.Rollback( txn );
        }

        /// <summary>
        /// Answer a new transaction associated with the provided command
        /// based on my current execution policy.
        /// </summary>
        /// <param name="cmd">
        /// The command which will be associated with the transaction.
        /// The connection associated with the command should be open.
        /// </param>
        /// <returns>
        /// A new transaction.
        /// </returns>
        protected SqlTransaction TransactionFor( SqlCommand cmd )
        {
            return this.Policy.TransactionFor( cmd );
        }
        #endregion

        #region Private Properties

        private int Timeout
        {
            get
            {
                int timeout;

                var parsingSucceeded = int.TryParse( ConfigurationManager.AppSettings[SETTING_DEFAULT_TIMEOUT], out timeout );

                if ( !parsingSucceeded )
                {
                    timeout = DEFAULT_TIMEOUT;
                }
                
                return timeout;
            }
        }
        #endregion

        #region Construction and Finalization
        /// <summary>
        /// Construct a new instance of the broker that will read the connection
        /// information from the AppSettings value, CXN_PROPERTY_NAME.
        /// </summary>
        public AbstractBroker() :
            this( ConfigurationManager.ConnectionStrings[CXN_PROPERTY_NAME].ConnectionString )
        {
        }

        /// <summary>
        /// Construct and initialize an instance of the broker using a supplied
        /// connection string.
        /// </summary>
        /// <param name="cxnString">
        /// The connection string required for the broker to connection to 
        /// its underlying persistence mechanism.
        /// </param>
        public AbstractBroker( string cxnString )
        {
            this.ConnectionString = cxnString;
            this.Policy = new SingleUsePolicy( this );
        }

        /// <summary>
        /// Construct and initialize an instance of the broker using a supplied
        /// open connection.
        /// </summary>
        /// <param name="txn">
        /// An existing transaction into which all commands will enlist.
        /// </param>
        public AbstractBroker( SqlTransaction txn )
        {
            this.Transaction = txn;
            this.Policy = new ReusableTransactionPolicy( this );
        }
        #endregion

        #region Data Elements
        private string          i_ConnectionString;
        private ExecutionPolicy i_Policy;
        private SqlTransaction  i_Transaction;
        #endregion
    }
}
