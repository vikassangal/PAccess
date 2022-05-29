using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	[Serializable]
    internal class TransferToNewLocationTransactionCoordinator : TransactionCoordinator
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
                TransferToNewLocationInsertStrategy strategy = new TransferToNewLocationInsertStrategy();
                strategy.TransactionFileId = TRANSFER_TO_NEW_LOCATION_TRANSACTION_ID;
                strategy.SecurityCode = this.AppUser.PBARSecurityCode;
                i_SqlBuilderStrategy[0] = strategy;    

                return i_SqlBuilderStrategy;                
            }
            set
            {
                i_SqlBuilderStrategy = value;
            }
        }

        public override int NumberOfInsurances
        {
            get
            {
                // Since there is no Insurance Transaction Insert done.
                return 0;
            }
            set
            {
                base.NumberOfInsurances = value;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction And Finalization
		public TransferToNewLocationTransactionCoordinator()
		{
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
			
		}
        public TransferToNewLocationTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        } 
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const string TRANSFER_TO_NEW_LOCATION_TRANSACTION_ID = "IT";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        #endregion
	}
}
