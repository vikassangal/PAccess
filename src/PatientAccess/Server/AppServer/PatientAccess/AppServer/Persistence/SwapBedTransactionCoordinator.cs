using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
    internal class SwapBedTransactionCoordinator : TransactionCoordinator
    {

        #region Event Handlers
        #endregion

        #region Methods
        
        #endregion

        #region Public Properties

        public override SqlBuilderStrategy[] InsertStrategies
        {
            get
            {
                SwapBedInsertStrategy strategy = new SwapBedInsertStrategy();
                strategy.TransactionFileId = BED_SWAP_TRANSACTION_ID;
                if(this.AppUser.PBARSecurityCode != null)
                {
                    strategy.SecurityCode = this.AppUser.PBARSecurityCode;
                }
                i_SqlBuilderStrategy[0] = strategy;
                InsuranceInsertStrategy insStrategy;
                int  ACCOUNT_ONE_NUM_OF_INS = this.Account.Insurance.Coverages.Count ;

                InsuranceInsertStrategy insStrategy2;
                int  ACCOUNT_TWO_NUM_OF_INS = this.AccountTwo.Insurance.Coverages.Count ;
                if(this.Account.IsSetInsuranceVerifiedToDefault())
                {
                   
                    for( int i = 1; i <= this.Account.Insurance.Coverages.Count; i++ )
                    {
                        Coverage coverage = this.Account.Insurance.CoverageFor(i);
                        insStrategy = new InsuranceInsertStrategy( coverage.Priority );
                        insStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                        insStrategy.OrignalTransactionId = BED_SWAP_TRANSACTION_ID;
                        insStrategy.AccountNumber = (int )this.Account.AccountNumber ;
                        if((i == Account.Insurance.Coverages.Count )&&
                            !(this.AccountTwo.IsSetInsuranceVerifiedToDefault()))     
                        {        
                            insStrategy.LastTransactionInGroup = 
                                LAST_TRANSACTION_IN_GROUP_FLAG;
                        }

                        i_SqlBuilderStrategy[i] = insStrategy; 
                    }
                }
                if(this.AccountTwo.IsSetInsuranceVerifiedToDefault())
                {
                   
                    for( int i = 1; i <= this.AccountTwo.Insurance.Coverages.Count; i++ )
                    {
                        Coverage coverage2 = AccountTwo.Insurance.CoverageFor(i);
                        insStrategy2 = new InsuranceInsertStrategy( coverage2.Priority );
                        insStrategy2.UserSecurityCode = this.AppUser.PBARSecurityCode;
                        insStrategy2.OrignalTransactionId = BED_SWAP_TRANSACTION_ID;
                        insStrategy2.AccountNumber = (int )AccountTwo.AccountNumber ;
                        if( i == AccountTwo.Insurance.Coverages.Count )
                        {
                            insStrategy2.LastTransactionInGroup = 
                                LAST_TRANSACTION_IN_GROUP_FLAG;
                            
                        }
                        i_SqlBuilderStrategy[i + ACCOUNT_ONE_NUM_OF_INS] = insStrategy2; 
                    }
                }
                return i_SqlBuilderStrategy;
            }
            set
            {
                i_SqlBuilderStrategy = value;
            }
        }

        public override void CreateSQL(  )
        {

            Account account = this.Account;  
            // Retrieve PBARSecurityCode and set it to the AppUser
            string pbarEmployeeID = this.AppUser.PBAREmployeeID;
            string pbarSecurityCode = String.Empty;
            if( !( pbarEmployeeID == null || pbarEmployeeID.Equals( String.Empty ) ) )
            {
                ISecurityCodeBroker securityCodeBroker = BrokerFactory.BrokerOfType< ISecurityCodeBroker >() ;
                pbarSecurityCode = securityCodeBroker.GetPrintedSecurityCode( pbarEmployeeID, account.Facility );
            }
            this.AppUser.PBARSecurityCode = pbarSecurityCode; 
            this.ReOrderInsurances();
            this.ReOrderInsurancesForAccountTwo();
            ArrayList aSqlStatements  = new ArrayList();   

            SqlBuilderStrategy[] strategies = this.InsertStrategies;

            foreach( SqlBuilderStrategy sqlBuildStrategy in strategies )
            {
                if( sqlBuildStrategy != null )
                {
                    if(sqlBuildStrategy.GetType() != typeof(InsuranceInsertStrategy))                    
                    {
                        aSqlStatements = sqlBuildStrategy.BuildSqlFrom(
                            this.Account, this.AccountTwo, null );
                        foreach(string sqlSt in aSqlStatements)
                        {
                            this.Add( sqlSt );
                        }
                    }
                    else
                    {
                        if(sqlBuildStrategy.GetType() == typeof(InsuranceInsertStrategy))  
                        {
                            InsuranceInsertStrategy insStrategy = (InsuranceInsertStrategy)sqlBuildStrategy;
                            if(insStrategy.AccountNumber == this.Account.AccountNumber)
                            {
                                aSqlStatements = sqlBuildStrategy.BuildSqlFrom(
                                    this.Account, null );
                                foreach(string sqlSt in aSqlStatements)
                                {
                                    this.Add( sqlSt );
                                }
                            }
                            else  if(insStrategy.AccountNumber == this.AccountTwo.AccountNumber)
                            {
                                ArrayList tempSqlStatements = sqlBuildStrategy.BuildSqlFrom(
                                    this.AccountTwo, null );
                                foreach( string insSqlStmt in   tempSqlStatements)
                                {
                                    string sqlStatementWithHeader3 =  insSqlStmt.Replace(SqlBuilderStrategy.PRIMARY_INSURANCE_RECORD_NUMBER ,SqlBuilderStrategy.ACCOUNT_TWO_PRIMARY_INSURANCE_RECORD_NUMBER);                          
                                    string sqlStatementWithHeader4 =  sqlStatementWithHeader3.Replace(SqlBuilderStrategy.SECONDARY_INSURANCE_RECORD_NUMBER ,SqlBuilderStrategy.ACCOUNT_TWO_SECONDARY_INSURANCE_RECORD_NUMBER);                                                              
                                    this.Add( sqlStatementWithHeader4) ;
                                }                        
                            }                           
                        }
                    }
                }
            }
            //Facility facility = this.Account.Facility;
            //aTransactionBroker.Execute( SqlStatements, facility, this.Account, this.IsTransactionHeaderRequired );
            
        }

        public override void AfterTxn()
        {
            User appUser = this.Account.Activity.AppUser;
            IAccountBroker accountBroker = BrokerFactory.BrokerOfType< IAccountBroker >() ;

            accountBroker.ReleaseLockOn( this.AccountTwo.AccountNumber, appUser.PBAREmployeeID, 
                appUser.WorkstationID, this.AccountTwo.Facility.Oid );
            accountBroker.FinishLockingOn( this.AccountTwo.AccountNumber, appUser.PBAREmployeeID, 
                appUser.WorkstationID, this.AccountTwo.Facility.Oid );
        }
        
       
        private int GetNumberOfInsurancesFor(Account account)
        {
            int numInsurances = 0;
            if(account.IsSetInsuranceVerifiedToDefault())
            {
                if( ( account.Insurance != null ) && 
                    ( account.Insurance.Coverages != null ) )
                {
                    numInsurances = account.Insurance.Coverages.Count;
                }
                if( numInsurances == 1 && 
                    account.IsNew == false &&
                    account.DeletedSecondaryCoverage != null)
                {
                    numInsurances++;
                }
            }
            return numInsurances ;

        }

        private  void ReOrderInsurancesForAccountTwo()
        {
            
            if( this.AccountTwo.IsNew || this.AccountTwo.Insurance.Coverages.Count == 2 )
            {
                // if this is new account just use the primary coverages.
                // ignore a deleted coverage since we don't really have 
                // delete it.
                // OR
                // if there are already 2 coverages in the list just use them
                // and ignore a deleted one if it exists.
                for( int i = 1; i <= this.AccountTwo.Insurance.Coverages.Count; i++ )
                {
                    Coverage coverage = this.AccountTwo.Insurance.CoverageFor(i);
                    coverage.Priority = i;
                }
                return;
            }
            else if ( this.AccountTwo.Insurance.Coverages.Count == 1 )
            {
                Coverage primageCoverage = this.AccountTwo.Insurance.CoverageFor(1);
                primageCoverage.Priority = 1;

                if( this.AccountTwo.DeletedSecondaryCoverage != null )
                {
                    this.AccountTwo.DeletedSecondaryCoverage.Priority = 0;
                    this.AccountTwo.Insurance.AddCoverage(this.AccountTwo.DeletedSecondaryCoverage);
                }
            }
            
        }
        public override int NumberOfInsurances
        {
            get
            {
                this.i_NumberOfInsurances = GetNumberOfInsurancesFor(this.Account) +
                    GetNumberOfInsurancesFor(this.AccountTwo);              

                return this.i_NumberOfInsurances;

            }
            set
            {
                this.i_NumberOfInsurances = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction And Finalization
        public SwapBedTransactionCoordinator( Account anAccountOne, Account anAccountTwo )
        {
            this.Account = anAccountOne;
            this.AccountTwo = anAccountTwo;
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public SwapBedTransactionCoordinator()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public SwapBedTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        } 
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        private int i_NumberOfInsurances= 0;
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 5;
        private const string BED_SWAP_TRANSACTION_ID = "DT";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        #endregion
    }
}


