using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for CancelPreRegTransactionCoordinator.
	/// </summary>
	//TODO: Create XML summary comment for CancelPreRegTransactionCoordinator
    [Serializable]
    internal class CancelPreRegTransactionCoordinator : TransactionCoordinator
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override SqlBuilderStrategy[] InsertStrategies
        {
            get
            {
                CancelInsertStrategy strategy = new CancelInsertStrategy();
                strategy.TransactionFileId = CANCEL_PREREG_TRANSACTION_ID;
                strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
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

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CancelPreRegTransactionCoordinator()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public CancelPreRegTransactionCoordinator(User user) 
            : base(user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }  

        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const string CANCEL_PREREG_TRANSACTION_ID = "CP";
        const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        #endregion
    }
}
