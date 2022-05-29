using System;
using System.Configuration;
using System.Data;
using Extensions.Exceptions;
using Extensions.PersistenceCommon;
using IBM.Data.DB2.iSeries;

namespace Extensions.DB2Persistence
{
    /// <summary>
    /// AbstractBroker serves as a base class for all classes that provide
    /// data services to an application.  Subclasses will typically define
    /// a custom interface for retrieving and storing data.  The base class
    /// provides interfaces for services required by all concrete subclasses.
    /// </summary>
	[Serializable]
	abstract public class AbstractBroker : MarshalByRefObject, IAbstractBroker
	{
		#region Constants
		private const string CLASS_ID_CONCURRENCY = "75003";
		private const string 
			TIMEOUT_SQLSTATE_EXCEPTION = "57005",
			CONTEXT_PREFIX    = "PersistenceError.",
			CONTEXT_PROCEDURE = "ProcedureName",
			CXN_PROPERTY_NAME = "DB2ConnectionTemplate";
		
        private const string
			ERROR_MSG_CONCURRENCY       = "Concurrency error: {0}.",
			ERROR_MSG_GENERAL_SQL_ERROR = "General SQL Server Error : {0}",
			ERROR_MSG_TIMEOUT	= "Timeout Error from Server: {0}";
		
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
			set
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
		public IDbTransaction Transaction
		{
			get
			{
				return i_Transaction;
			}
			set
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
		protected IDataAdapter AdapterFor( string storedProcName )
		{
			IDbDataAdapter adapter = new iDB2DataAdapter( storedProcName, 
				(iDB2Connection)this.Policy.NewConnection() );
			adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
			return adapter;
		}

		/// <summary>
		/// Close the provided command based on my current policy.
		/// </summary>
		/// <param name="cmd">
		/// A command to close.
		/// </param>
		protected void Close( IDbCommand cmd )
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
        /// <param name="cmdType"></param>
        /// <returns>
        /// An instance of System.Data.SqlClient.SqlCommand.
        /// </returns>
        protected virtual iDB2Command CommandFor( string storedProcName, CommandType cmdType )
        {
            iDB2Command cmd = (iDB2Command)this.Policy.CommandFor( storedProcName, cmdType );
            if( cmdType == CommandType.Text )
            {
                cmd.DeriveParameters();
            }
            return cmd;
        }

        protected virtual iDB2Command CommandFor( string cmdText )
        {
            return (iDB2Command)this.Policy.CommandFor( cmdText );
        }

        /// <summary>
		/// Commit the provided transaction based on my current policy.
		/// </summary>
		/// <param name="txn"></param>
		protected void Commit( IDbTransaction txn )
		{
			this.Policy.Commit( txn );
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
			IDbCommand cmd )
		{
			error.AddContextItem( CONTEXT_PREFIX + CONTEXT_PROCEDURE, cmd.CommandText );
			foreach( IDbDataParameter parameter in cmd.Parameters )
			{
				error.AddContextItem( CONTEXT_PREFIX + parameter.ParameterName, 
					parameter.Value.ToString() );
			}
			return error;
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
		protected SafeReader ExecuteReader( IDbCommand cmd )
		{
			//SafeReader reader = null;
			try
			{
				return this.Policy.ExecuteReader( cmd );
			}
			catch( iDB2Exception sqlError )
			{
				EnterpriseException error = null;
				if( this.IsConcurrencyError( sqlError ) )
				{
					error = new RecordContentionException( 
						String.Format( ERROR_MSG_CONCURRENCY, sqlError.Message ), 
						sqlError, 
						Severity.Medium );
				}
				else if(this.IsTimeoutError(sqlError) )
				{
					error = new TimeoutException(
						String.Format(ERROR_MSG_TIMEOUT, sqlError.Message ),
						sqlError,
						Severity.High );
				}
				else
				{
					error = new DatabaseServiceException( 
						String.Format( ERROR_MSG_GENERAL_SQL_ERROR, sqlError.Message ), 
						sqlError, 
						Severity.Catastrophic );
				}
				this.LoadExceptionContextFrom( error, cmd );
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
		private bool IsConcurrencyError( iDB2Exception error )
		{
			return error.SqlState.Equals(CLASS_ID_CONCURRENCY);
			//return false;
		}

        private bool IsTimeoutError (iDB2Exception error)
		{
			if(error.MessageCode == -666 && error.SqlState.Equals(TIMEOUT_SQLSTATE_EXCEPTION))
				return true;
			return false;
		}

		/// <summary>
		/// Answer a new open, connection based on my current policy.
		/// </summary>
		/// <returns></returns>
		protected iDB2Connection NewConnection()
		{
			return (iDB2Connection)this.Policy.NewConnection();
		}

		protected iDB2Transaction NewTransaction()
		{
			return (iDB2Transaction)this.Policy.NewTransaction();
		}

		/// <summary>
		/// Roll back the specified transaction based on my current policy.
		/// </summary>
		/// <param name="txn">
		/// The transaction to roll back.
		/// </param>
		protected void Rollback( IDbTransaction txn )
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
		protected IDbTransaction TransactionFor( IDbCommand cmd )
		{
			return this.Policy.TransactionFor( cmd );
		}
		#endregion

		#region Private Properties
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
		public AbstractBroker( IDbTransaction txn )
		{
			this.Transaction = txn;
			this.Policy = new ReusableTransactionPolicy( this );
		}
		#endregion

		#region Data Elements
		private string          i_ConnectionString;
		private ExecutionPolicy i_Policy;
		private IDbTransaction  i_Transaction;
		#endregion
	}
}
