using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	[Serializable]
    internal class TransferInPatientToOutPatientTransactionCoordinator : TransactionCoordinator
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
                TransferInPatientToOutPatientInsertStrategy xferStrategy 
                    = new TransferInPatientToOutPatientInsertStrategy();
                xferStrategy.TransactionFileId = TRANSFER_INPATIENT_OUTPATIENT_TRANSACTION_ID;
                xferStrategy.SecurityCode = this.AppUser.PBARSecurityCode;
                i_SqlBuilderStrategy[0] = xferStrategy;
                
                InsuranceInsertStrategy strategy;
                for( int i = 1; i <= Account.Insurance.Coverages.Count; i++ )
                {
                    Coverage coverage = Account.Insurance.CoverageFor(i);
                    strategy = new InsuranceInsertStrategy( coverage.Priority );
                    strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                    strategy.OrignalTransactionId = TRANSFER_INPATIENT_OUTPATIENT_TRANSACTION_ID;
                    if( i == Account.Insurance.Coverages.Count )
                    {
                        strategy.LastTransactionInGroup = 
                            LAST_TRANSACTION_IN_GROUP_FLAG;
                    }
                    i_SqlBuilderStrategy[i] = strategy; 
                }
                
                
                return i_SqlBuilderStrategy;
            }
            set
            {
                i_SqlBuilderStrategy = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction And Finalization
		public TransferInPatientToOutPatientTransactionCoordinator()
		{
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
			
		}
        public TransferInPatientToOutPatientTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        } 

        #endregion 

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy 
            = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        public const string TRANSFER_INPATIENT_OUTPATIENT_TRANSACTION_ID = "IO";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        private const int NUMBER_OF_INSERT_STRATEGIES = 3;

        #endregion
	}
}
