using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using Extensions.Exceptions;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.Utilities;
using log4net;
using TimeoutException = Extensions.DB2Persistence.TimeoutException;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements strategy related data saving into PBAR.
    /// </summary>

    [Serializable]
    internal class TransactionBroker : AbstractPBARBroker, ITransactionBroker
    {
        #region Event Handlers
        #endregion

        #region Methods

        /// <summary>
        /// Insert the sql statments into PBAR Database
        /// </summary>
        /// <param name="txnCoord"></param>
        public void ExecuteTransaction(ITransactionCoordinator txnCoord)
        {
            c_log.InfoFormat("Beginning save of TXN- Activit:{0} Facility:{1} MRN:{2} Acct:{3} WorkstationID:{4}",
                    txnCoord.Account.Activity.GetType(),
                    txnCoord.Account.Facility.Code,
                    txnCoord.Account.Patient.MedicalRecordNumber,
                    txnCoord.Account.AccountNumber,
                    txnCoord.Account.Activity.AppUser.WorkstationID
                    );

            this.TransactionCoordinator = txnCoord;

            txnCoord.CreateSQL();
            
            ExecuteTransactionTest(txnCoord);

            this.Execute(txnCoord.SqlStatements,
                txnCoord.Account.Facility,
                txnCoord.Account,
                txnCoord.IsTransactionHeaderRequired);

            // there are some txns such as the Bed Swap that requires special post
            // transaction actions. This virtual method executes these steps. it is 
            // empty in the base TxnCoordinator
            txnCoord.AfterTxn();

            c_log.InfoFormat( "Completed save of TXN- Activit:{0} Facility:{1} MRN:{2} Acct:{3} WorkstationID:{4}",
                    txnCoord.Account.Activity.GetType(),
                    txnCoord.Account.Facility.Code,
                    txnCoord.Account.Patient.MedicalRecordNumber,
                    txnCoord.Account.AccountNumber,
                    txnCoord.Account.Activity.AppUser.WorkstationID
                    );
        }

        public void ExecuteTransactionTest(ITransactionCoordinator txnCoord)
        {
            
            iDB2Transaction testTxn = null;
            iDB2Connection testCon = null;
            ITransactionBroker testTransactionBroker = null;

            bool shouldPerformCheck = true;
            
            bool.TryParse( ConfigurationManager.AppSettings[PERFORM_PRESAVECHECK], out shouldPerformCheck );

            if( shouldPerformCheck )
            {

                try
                {
                    testTxn = this.NewTransaction(txnCoord.Account.Facility);

                    testTransactionBroker =  new TransactionBroker( testTxn );
                    testCon = testTxn.Connection;

                    testTransactionBroker.TestSQL( txnCoord.SqlStatements,
                        txnCoord.Account.Facility,
                        txnCoord.Account,
                        txnCoord.IsTransactionHeaderRequired );

                    testTxn.Rollback();                    
                }
                finally
                {
                    if(testTxn != null)
                    {
                        testTxn.Dispose();
                    }
                    if(testCon != null && testCon.State == ConnectionState.Open)
                    {
                        testCon.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Submit the sql to a set of dummy tables to insure that the SQL in syntactically correct.
        /// </summary>
        /// <param name="sqlStatements"></param>
        /// <param name="facility"></param>
        /// <param name="anAccount"></param>
        /// <param name="IsTransactionHeaderRequired"></param>
        public void TestSQL(ArrayList sqlStatements, Facility facility, Account anAccount, bool IsTransactionHeaderRequired)
        {
            try
            {
                string facilityCode = facility.Code;

                List<string> testSQLList = new List<string>();

                foreach (string sqlStatement in sqlStatements)
                {
                    StringBuilder sqlStatementWithHeader1 = new StringBuilder(sqlStatement);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.PATIENT_RECORD_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.LOG_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.PRIMARY_INSURANCE_RECORD_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.SECONDARY_INSURANCE_RECORD_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.GUARANTOR_TRANSACTION_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.OTHER_RECORD_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.ACCOUNT_TWO_PRIMARY_INSURANCE_RECORD_NUMBER, DUMMYID);
                    sqlStatementWithHeader1.Replace(SqlBuilderStrategy.ACCOUNT_TWO_SECONDARY_INSURANCE_RECORD_NUMBER, DUMMYID);
                    if (sqlStatementWithHeader1.Length > 0 &&
                        sqlStatementWithHeader1.ToString().IndexOf(ORIGINAL_INSERT_STATEMENT) > -1)
                    {
                        // only perform the test for the insert of the transaction tables.
                        sqlStatementWithHeader1.Replace(ORIGINAL_INSERT_STATEMENT, NEW_INSERT_STATEMENT);
                        testSQLList.Add(sqlStatementWithHeader1.ToString());
                    }
                }

                foreach (string sqlStatement in testSQLList)
                {
                    aDb2Command = this.CommandFor(sqlStatement);
                    aDb2Command.CommandType = CommandType.Text;

                    try
                    {
                        aDb2Command.ExecuteNonQuery();
                    }
                    catch( iDB2Exception )
                    {
                        c_log.Error( "Error in SQL detected for statement" + sqlStatement );
                        if( sqlStatement.Length > 8192 )
                        {
                            c_log.Error( sqlStatement.Substring( 8196 ) );
                        }
                        throw;
                    }
                    finally
                    {
                        this.Close( aDb2Command );
                    }
                }
            }
            catch (iDB2Exception ex)
            {
                throw new TransactionUpdateException("Test of transaction failed:" + ex.Message + " " + ex.StackTrace);
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        public void Execute( ArrayList sqlStatements, Facility facility, Account anAccount, bool IsTransactionHeaderRequired )
        {
            IDbConnection conn = null;

            string facilityCode = facility.Code;
            
            List<string> sqlStatementsWithHeader = new List<string>();

            if(( IsTransactionHeaderRequired ) &&
                (IsNewTransaction( sqlStatements ))
                )
                                                 
            {
                ITransactionBroker innerTxnBroker = new TransactionBroker( this.ConnectionString );
                
                innerTxnBroker.SelectTransactionHeader( 
                    this.TransactionCoordinator.NumberOfInsurances,
                    this.TransactionCoordinator.NumberOfNonInsurances,
                    this.TransactionCoordinator.NumberOfOtherRecs,
                    this.TransactionCoordinator.Account.Facility,
                    this);
            }

            try
            {
                if( this.Transaction == null )
                {
                    aDb2Command = new iDB2Command();
                    aDb2Command.Connection = base.NewConnection( facility );
                    dbTransaction = base.TransactionFor( aDb2Command );
                }

                if( ( IsTransactionHeaderRequired ) &&
                     ( IsNewTransaction( sqlStatements ) ) )
                {
                    int insHeaderCounter = this.TransactionKeys.InsuranceRecordNumber;
                    foreach( string sqlStatement in sqlStatements )
                    {
                        StringBuilder sqlStatementWithHeaderSB = new StringBuilder( sqlStatement );
                        sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.PATIENT_RECORD_NUMBER, this.TransactionKeys.PatientRecordNumber.ToString() );
                        sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.LOG_NUMBER, this.TransactionKeys.LogNumber.ToString() );
                        if( sqlStatementWithHeaderSB.ToString().IndexOf( SqlBuilderStrategy.PRIMARY_INSURANCE_RECORD_NUMBER ) > 0 )
                        {
                            sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.PRIMARY_INSURANCE_RECORD_NUMBER, insHeaderCounter.ToString() );
                            insHeaderCounter++;
                        }
                        if( sqlStatementWithHeaderSB.ToString().IndexOf( SqlBuilderStrategy.SECONDARY_INSURANCE_RECORD_NUMBER ) > 0 )
                        {
                            sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.SECONDARY_INSURANCE_RECORD_NUMBER, insHeaderCounter.ToString() );
                            insHeaderCounter++;
                        }
                        sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.GUARANTOR_TRANSACTION_NUMBER, ( this.TransactionKeys.PatientRecordNumber + 1 ).ToString() );
                        sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.OTHER_RECORD_NUMBER, ( this.TransactionKeys.OtherRecordNumber ).ToString() );
                        if( sqlStatementWithHeaderSB.ToString().IndexOf( SqlBuilderStrategy.ACCOUNT_TWO_PRIMARY_INSURANCE_RECORD_NUMBER ) > 0 )
                        {
                            sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.ACCOUNT_TWO_PRIMARY_INSURANCE_RECORD_NUMBER, insHeaderCounter.ToString() );
                            insHeaderCounter++;
                        }

                        if( sqlStatementWithHeaderSB.ToString().IndexOf( SqlBuilderStrategy.ACCOUNT_TWO_SECONDARY_INSURANCE_RECORD_NUMBER ) > 0 )
                        {
                            sqlStatementWithHeaderSB.Replace( SqlBuilderStrategy.ACCOUNT_TWO_SECONDARY_INSURANCE_RECORD_NUMBER, insHeaderCounter.ToString() );
                            insHeaderCounter++;
                        }
                        sqlStatementsWithHeader.Add( sqlStatementWithHeaderSB.ToString() );
                    }
                }

                foreach( object sqlStatement in sqlStatementsWithHeader )
                {
                    if( sqlStatement.ToString().Trim().Length > 0 )
                    {
                        // because of the size of the insurance table the entire string will not fit in
                        // a single log entry so split it at the values. 
                        if( sqlStatement.ToString().Contains( "HPADAPID" ) )
                        {
                            string statement = sqlStatement.ToString();
                            c_log.Info( statement.Substring( 0, statement.IndexOf( "VALUE" ) ) );
                            c_log.Info( statement.Substring( statement.IndexOf( "VALUE" ) ) );
                        }
                        else
                        {
                            c_log.Info( sqlStatement.ToString() );
                        }

                        aDb2Command.CommandText = sqlStatement.ToString();

                        aDb2Command.ExecuteNonQuery();
                    }
                }

                // mark the account lock with the appropriate setting
                IBMUtilities ibmUtility = new IBMUtilities( facility );
                if( anAccount.IsNew )
                {
                    ibmUtility.LockFlaggingAccount( anAccount.AccountNumber,
                        anAccount.Activity.AppUser.PBAREmployeeID,
                        anAccount.Activity.AppUser.WorkstationID,
                        anAccount.Facility );
                }
                else if( anAccount.IsLocked )
                {
                    ibmUtility.FinishWithAccountLock( anAccount.AccountNumber,
                        anAccount.Activity.AppUser.PBAREmployeeID,
                        anAccount.Activity.AppUser.WorkstationID,
                        anAccount.Facility );
                }

                if( this.Transaction == null )
                {
                    base.Commit( dbTransaction );
                }
            }
            catch( TimeoutException timeOutException )
            {
                this.RollBackBlock( dbTransaction );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "The Transaction has timed out", timeOutException, c_log );
            }
            catch( iDB2SQLErrorException ibmexception )
            {
                string message = String.Format( ERROR_INVALID_SQL,
                        anAccount.AccountNumber, anAccount.Patient.MedicalRecordNumber, anAccount.Facility.Code );
                // if there are errors of these types then the Data is allow the SQL to 
                // succeed. Throw the error up where it can be handled appropriately. 
                // by throwing this trype of message the object that calls this method
                // will not enqueue it to be retried. 
                if( ibmexception.MessageCode == -406 || // Conversion error
                    ibmexception.MessageCode == -404 || // value to long
                    ibmexception.MessageCode == -101 )  // Invalid token
                {
                    // 2-15-08 - kjs - If you roll back a transaction the connection gets closed
                    // on the object but the actual connection is still open. You have to manually
                    // close the connection. This is only true for IBM.
                    conn = dbTransaction.Connection;
                    base.Rollback( dbTransaction );
                    if( conn != null )
                    {
                        conn.Close();
                    }

                    c_log.Error( message + " " + ibmexception.Message + " " + ibmexception.MessageDetails,
                        ibmexception );
                    throw new TransactionUpdateException( message + ibmexception.StackTrace,
                        Severity.High );
                }
                else
                {
                    this.RollBackBlock( dbTransaction );
                    throw BrokerExceptionFactory.BrokerExceptionFrom( message, ibmexception, c_log );
                }
            }
            catch( TransactionUpdateException ex )
            {
                // 2-15-08 - kjs - If you roll back a transaction the connection gets closed
                // on the object but the actual connection is still open. You have to manually
                // close the connection. This is only true for IBM.
                conn = dbTransaction.Connection;
                base.Rollback( dbTransaction );
                if( conn != null )
                {
                    conn.Close();
                }

                throw BrokerExceptionFactory.BrokerExceptionFrom( ex, c_log );
            }
            catch( Exception e )
            {
                this.RollBackBlock( dbTransaction );
                string message = "An exception of type " + e.GetType() +
                    " was encountered while inserting the data - AccountNumber = " + anAccount.AccountNumber + ";Facility = " + anAccount.Facility.Code + ". Neither record was written to database.";
                throw BrokerExceptionFactory.BrokerExceptionFrom( message, e, c_log );
            }
            finally
            {
                base.Close( aDb2Command );
            }
        }        
        
        public void SelectTransactionHeader( 
            int numberOfInsurances, 
            int numberOfNonInsurances, 
            int numberOfOthers,
            Facility facility,
            ITransactionBroker broker)
        {
            iDB2Command cmd = null;

            try
            {
                c_log.InfoFormat( "BEGIN Getting RR# for: {0}", facility.Code  );

                cmd = this.CommandFor( "CALL " + SP_SELECT_TRANSACTION_HEADER +
                    "(" + PARAM_INSURANCE_TRANSACTION + 
                    "," + PARAM_NONINSURANCE_TRANSACTION + 
                    "," + PARAM_OTHER_TRANSACTION + 
                    "," + PARAM_HSP_NUMBER + 
                    "," + PARAM_RRNUM + 
                    "," + PARAM_LOGNUM +
                    "," + PARAM_INSRRNUM +
                    "," + PARAM_OTHERRRNUM + ")",
                    CommandType.Text,
                    facility
                    );
                cmd.Parameters[PARAM_INSURANCE_TRANSACTION].Value = numberOfInsurances;
                cmd.Parameters[PARAM_NONINSURANCE_TRANSACTION].Value = numberOfNonInsurances;
                cmd.Parameters[PARAM_OTHER_TRANSACTION].Value = numberOfOthers;
                cmd.Parameters[PARAM_HSP_NUMBER].Value = facility.Oid;

                cmd.Parameters[PARAM_RRNUM].Direction = ParameterDirection.Output;
                cmd.Parameters[PARAM_LOGNUM].Direction = ParameterDirection.Output;
                cmd.Parameters[PARAM_INSRRNUM].Direction = ParameterDirection.Output;
                cmd.Parameters[PARAM_OTHERRRNUM].Direction = ParameterDirection.Output;

                iDB2Connection db2Conn = cmd.Connection;

                if (db2Conn != null)
                {
                    c_log.InfoFormat("Getting Transaction Headers using job: {0}", db2Conn.JobName);
                }

                //txn = base.TransactionFor( cmd );
                cmd.ExecuteNonQuery();
                
                // GOO 2/14/07 - convert this proc call to use output variables
                patientRecordNumber = Convert.ToInt32(cmd.Parameters[PARAM_RRNUM].Value);
                logNumber = Convert.ToInt32( cmd.Parameters[PARAM_LOGNUM].Value );
                insuranceRecordNumber = Convert.ToInt32( cmd.Parameters[PARAM_INSRRNUM].Value );
                otherRecordNumber = Convert.ToInt32( cmd.Parameters[PARAM_OTHERRRNUM].Value );

                // do this now so that the transaction finishes as fast as possible
                // the closes in the finally are only for errors.
                base.Close(cmd);

                // GOO - Restructured Stored Proc so we no longer need to 
                // decrement the returned values. The returned values are now
                // the starting points
                broker.TransactionKeys = new TransactionKeys( 
                    patientRecordNumber,
                    logNumber,
                    insuranceRecordNumber,
                    otherRecordNumber,
                    daysSinceAdmission );


                string logString = broker.LogTransactionHeaderNumbersFor( 
                    facility.Code, numberOfNonInsurances, 
                    numberOfInsurances, numberOfOthers,
                    broker);

                c_log.Info( logString );
            }
            catch( Exception ex )
            {
                //base.Rollback( txn );
                throw BrokerExceptionFactory.BrokerExceptionFrom( "Unhandled Exception.", ex, c_log );
            }
            finally
            {
                if (cmd.Connection != null && cmd.Connection.State == ConnectionState.Open)
                {
                    base.Close( cmd );
                }
            }
        }

       
        public bool IsNewTransaction( ArrayList sqlStatements )
        {
                              
            foreach ( object sqlStatement in sqlStatements )
            {
                if ( (sqlStatement.ToString().IndexOf(SqlBuilderStrategy.PATIENT_RECORD_NUMBER) > 0) ||
                    (sqlStatement.ToString().IndexOf(SqlBuilderStrategy.LOG_NUMBER) > 0) ||
                    (sqlStatement.ToString().IndexOf(SqlBuilderStrategy.SECONDARY_INSURANCE_RECORD_NUMBER) > 0) ||
                    (sqlStatement.ToString().IndexOf(SqlBuilderStrategy.GUARANTOR_TRANSACTION_NUMBER) > 0) ||
                    (sqlStatement.ToString().IndexOf(SqlBuilderStrategy.OTHER_RECORD_NUMBER) > 0)                    
                    )
                {
                    return true;
                }

            }
            return false;
        }
        #endregion

        #region Properties
        public ArrayList SqlBuilderStrategies
        {
            get
            {
                return i_SqlBuilderStrategies;
            }
            set
            {
                i_SqlBuilderStrategies = value;
            }
        }

        public TransactionKeys TransactionKeys
        {
            set
            {
                i_TransactionKeys = value;
            }
            get
            {
                return i_TransactionKeys;
            }
        }
       
        #endregion

        #region Private Methods
        public byte[] Base64DecodeByteArrayFor(string inputStr) 
        { 
            byte[] decodedByteArray = 
                Convert.FromBase64CharArray(inputStr.ToCharArray(), 
                0, inputStr.Length); 
            return (decodedByteArray); 
        }      

       

        

        private void RollBackBlock( IDbTransaction dbTransaction )
        {
            try
            {
                if( this.Transaction == null )
                {
                    IDbConnection conn = dbTransaction.Connection;
                    base.Rollback( dbTransaction );
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            catch ( iDB2Exception db2ex )
            {
                if ( base.Transaction.Connection != null )
                {
                    string message = "An exception of type " + db2ex.GetType() +
                        " was encountered while attempting to roll back the transaction.";
                    throw BrokerExceptionFactory.BrokerExceptionFrom( message, db2ex, c_log );
                }
            }
        }

        private string FormatRecordNumbersFor( int recordNumber, int requiredNumbers, string logString )
        {
            string formattedString = String.Empty;
            if( requiredNumbers != 0 )
            {
                string firstNumber = recordNumber.ToString();
                string remainingNumbers = String.Empty;
                for( int i = 1; i < requiredNumbers; i++ )
                {
                    int nextNumber = recordNumber + i;
                    remainingNumbers = remainingNumbers + LABEL_COMMA + nextNumber;
                }

                formattedString = 
                    String.Format( logString + LABEL_EQUALS + firstNumber + remainingNumbers + LABEL_SEMICOLON );
            }
            return formattedString;
        }

        public string LogTransactionHeaderNumbersFor( 
            string facilityCode, 
            int numberOfNonInsurances, 
            int numberOfInsurances, 
            int numberOfOthers,
            ITransactionBroker broker)
        {
            string patientRecordLog = this.FormatRecordNumbersFor(
                broker.TransactionKeys.PatientRecordNumber, numberOfNonInsurances, "PatientRecordNumbers" );

            string insuranceRecordLog = this.FormatRecordNumbersFor( 
                broker.TransactionKeys.InsuranceRecordNumber, numberOfInsurances, "InsuranceRecordNumbers" );

            string otherRecordLog = this.FormatRecordNumbersFor( 
                broker.TransactionKeys.OtherRecordNumber, numberOfOthers, "OtherRecordNumbers" );

            string logNumberString = "LogNumber" + LABEL_EQUALS + broker.TransactionKeys.LogNumber;

            string logString = String.Format( "Relative Record Numbers used for Facility - {0}{1}{2}{3}{4}{5} ", 
                facilityCode, LABEL_COLON, patientRecordLog, insuranceRecordLog, otherRecordLog, logNumberString);

            return logString;
        }
        #endregion

        #region Public Properties
        public ITransactionCoordinator TransactionCoordinator
        {
            get
            {
                return i_TransactionCoordinator;
            }
            set
            {
                i_TransactionCoordinator = value;
            }
        }
     
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TransactionBroker()
            : base()
        {
        }

        public TransactionBroker( string cxnString )
            : base( cxnString )
        {
        }

        private TransactionBroker( IDbTransaction txn )
            : base( txn )
        {
        }

        #endregion

        #region Data Elements
        private ArrayList i_SqlBuilderStrategies = new ArrayList();
        private TransactionKeys i_TransactionKeys;
        private int patientRecordNumber = 0;
        private int logNumber = 0;
        private int insuranceRecordNumber = 0;
        private int otherRecordNumber = 0;
        private iDB2Command aDb2Command;
        private IDbTransaction dbTransaction;
        private int daysSinceAdmission = 365;
        private ITransactionCoordinator i_TransactionCoordinator = null;

        //private static readonly log4net.ILog c_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog c_log = LogManager.GetLogger( typeof( TransactionBroker ) );
        public int pisStr = 0;

        #endregion

        #region Constants

        private const string
            
            SP_SELECT_TRANSACTION_HEADER    = "SELECTTXTIONHEADERINFO2",
            SP_SELECT_ACCT_LAST_UPDATE_INFO = "SELECTACCOUNTLASTUPDATEINFO",

            COL_LPLMD                   = "LASTMAINTENANCEDATE",
            COL_LPLML                   = "LASTMAINTENANCELOGNUMBER",
            COL_LPLUL                   = "UPDATELOGNUMBER",

            PARAM_HSP_NUMBER                = "@P_HSP",
            PARAM_NONINSURANCE_TRANSACTION  = "@P_NONINSURANCETXN",
            PARAM_INSURANCE_TRANSACTION     = "@P_INSURANCETXN",
            PARAM_OTHER_TRANSACTION         = "@P_OTHERTXN",
            PARAM_RRNUM                     = "@O_RRNUM",
            PARAM_LOGNUM                    = "@O_LOGNUM",
            PARAM_INSRRNUM                  = "@O_INSRRNUM",
            PARAM_OTHERRRNUM                = "@O_OTHERRRNUM",

            PARAM_HSPNUMBER         = "@P_HSP",
            PARAM_MRN               = "@P_MRN",
            PARAM_ACCOUNTNUMBER     = "@P_AccountNumber";

        private const string        
            PERFORM_PRESAVECHECK        = "PERFORM_PRESAVECHECK";

        private const string    
            ORIGINAL_INSERT_STATEMENT   = "INSERT INTO HPAD",
            NEW_INSERT_STATEMENT        = "INSERT INTO PACC";

        private const string
            ERROR_INVALID_TRANSACTION = "Account has already been updated by another transaction: ACCNO={0} MRN={1} HSP={2}",
            ERROR_INVALID_SQL         = "The SQL generated for this txn caused an error ACCNO={0} MRN={1} HSP={2}";

        private const string 
            LABEL_COMMA     = ", ",
            LABEL_EQUALS    = " = ",
            LABEL_SEMICOLON = "; ",
            LABEL_COLON     = " : ", 
            DUMMYID         = "1";
        #endregion
    }
}
