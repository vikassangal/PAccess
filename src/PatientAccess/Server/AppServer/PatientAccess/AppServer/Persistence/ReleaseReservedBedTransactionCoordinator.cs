using System;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
    internal class ReleaseReservedBedTransactionCoordinator : TransactionCoordinator
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
                ReleaseReservedBedUpdateStrategy strategy
                    = new ReleaseReservedBedUpdateStrategy();
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
        public ReleaseReservedBedTransactionCoordinator()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
            this.IsTransactionHeaderRequired = false;
        }

        public ReleaseReservedBedTransactionCoordinator(User user): base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
            this.IsTransactionHeaderRequired = false;
        }
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 0;
        #endregion

	}
}
