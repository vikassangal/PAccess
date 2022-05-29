using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
    internal class EditRecurringDischargeTransactionCoordinator : TransactionCoordinator
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        public override SqlBuilderStrategy[] InsertStrategies
        {
            get
            {
                EditRecurringDischargeInsertStrategy strategy = 
                    new EditRecurringDischargeInsertStrategy();
                strategy.TransactionFileId = EDIT_RECURRING_DISCHARGE_TRANSACTION_ID;
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

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public EditRecurringDischargeTransactionCoordinator()
        {
            this.NumberOfNonInsurances = 0;
            this.NumberOfInsurances = 0;
            this.NumberOfOtherRecs = NUMBER_OF_OTHER_RECS;
        }
        public EditRecurringDischargeTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = 0;
            this.NumberOfInsurances = 0;
            this.NumberOfOtherRecs = NUMBER_OF_OTHER_RECS;
        } 
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const string EDIT_RECURRING_DISCHARGE_TRANSACTION_ID = "EO";
        private const int NUMBER_OF_OTHER_RECS = 1;
        #endregion
    }
}
