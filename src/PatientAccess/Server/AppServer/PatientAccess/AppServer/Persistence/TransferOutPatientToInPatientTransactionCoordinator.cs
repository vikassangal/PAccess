using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	[Serializable]
    internal class TransferOutPatientToInPatientTransactionCoordinator : TransactionCoordinator
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
                // Since this txn uses the same txn as the INtoOUT we can reuse the
                // same strategy.
                TransferInPatientToOutPatientInsertStrategy strategy 
                    = new TransferInPatientToOutPatientInsertStrategy();
                strategy.TransactionFileId = TRANSFER_OUTPATIENT_INPATIENT_TRANSACTION_ID;
                strategy.SecurityCode = this.AppUser.PBARSecurityCode;

                i_SqlBuilderStrategy[0] = strategy;

                InsuranceInsertStrategy insuranceStrategy;
                for( int i = 1; i <= Account.Insurance.Coverages.Count; i++ )
                {
                    Coverage coverage = Account.Insurance.CoverageFor(i);
                    insuranceStrategy = new InsuranceInsertStrategy( coverage.Priority );
                    insuranceStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                    insuranceStrategy.OrignalTransactionId = TRANSFER_OUTPATIENT_INPATIENT_TRANSACTION_ID;
                    if( i == Account.Insurance.Coverages.Count )
                    {
                        insuranceStrategy.LastTransactionInGroup = 
                            LAST_TRANSACTION_IN_GROUP_FLAG;
                    }
                    i_SqlBuilderStrategy[i] = insuranceStrategy; 
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
		public TransferOutPatientToInPatientTransactionCoordinator()
		{
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
			
		}
        public TransferOutPatientToInPatientTransactionCoordinator(User user) 
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
        private const int NUMBER_OF_INSERT_STRATEGIES = 3;
        public const string TRANSFER_OUTPATIENT_INPATIENT_TRANSACTION_ID = "OT";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        #endregion
	}
}
