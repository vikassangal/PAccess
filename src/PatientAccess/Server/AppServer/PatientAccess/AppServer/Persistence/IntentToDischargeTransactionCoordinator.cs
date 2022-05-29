using System;
using PatientAccess.Domain;
using PatientAccess.Persistence.Factories;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// This class will be used to call the required InsertStratgy for 
    /// Intent to Disharge/Pre-Discharge.
    /// </summary>
	[Serializable]
	internal class IntentToDischargeTransactionCoordinator : TransactionCoordinator
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
                PatientInsertStrategy strategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
                strategy.TransactionFileId = PREDISCHARGE_TRANSACTION_ID;
                strategy.PreDischargeFlag = PREDISCHARGE_FLAG;
                strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                i_SqlBuilderStrategy[0] = strategy;

                GuarantorInsertStrategy guarantorInsertStrategy = new GuarantorInsertStrategy();
                guarantorInsertStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                guarantorInsertStrategy.LastTransactionInGroup = LAST_TRANSACTION_IN_GROUP_FLAG;
                i_SqlBuilderStrategy[1] = guarantorInsertStrategy;

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
        public IntentToDischargeTransactionCoordinator( Account anAccount )
            : base( anAccount )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
            
        }

        public IntentToDischargeTransactionCoordinator() : base()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }

        public IntentToDischargeTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }  
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        const int NUMBER_OF_INSERT_STRATEGIES = 2;

        private const string PREDISCHARGE_TRANSACTION_ID      = "PG";
        private const string PREDISCHARGE_FLAG = "D";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 2;
        #endregion
    }
}
