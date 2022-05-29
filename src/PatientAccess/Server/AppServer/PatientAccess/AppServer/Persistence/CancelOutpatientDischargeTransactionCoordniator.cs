using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    internal class CancelOutpatientDischargeTransactionCoordinator : TransactionCoordinator
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
                CancelInsertStrategy strategy = new CancelInsertStrategy();
                strategy.TransactionFileId = CANCEL_DISCHARGE_TRANSACTION_ID;
                strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                i_SqlBuilderStrategy[0] = strategy;
                
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
        public CancelOutpatientDischargeTransactionCoordinator()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
            this.NumberOfOtherRecs = NUMBER_OF_OTHER_TRANSACTIONS;
            this.NumberOfInsurances = NUMBER_OF_INSURANCE_TRANSACTIONS;
        }

        public CancelOutpatientDischargeTransactionCoordinator(User user)
            : base(user)
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
            this.NumberOfOtherRecs = NUMBER_OF_OTHER_TRANSACTIONS;
            this.NumberOfInsurances = NUMBER_OF_INSURANCE_TRANSACTIONS;

        }
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const string CANCEL_DISCHARGE_TRANSACTION_ID = "CD";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        private const int NUMBER_OF_INSURANCE_TRANSACTIONS = 0;
        private const int NUMBER_OF_OTHER_TRANSACTIONS = 0;
        #endregion
    }
}

