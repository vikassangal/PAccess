using System;
using PatientAccess.Domain;
using PatientAccess.Persistence.Factories;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// This class will be used to call the required InsertStratgy for 
    /// Pre-Registration.
    /// </summary>
    [Serializable]
    internal class PreregistrationTransactionCoordinator : TransactionCoordinator
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
                PatientInsertStrategy patientInsertStrategy = SqlBuilderStrategyFactory.CreatePatientInsertStrategy();
                patientInsertStrategy.TransactionFileId = PRE_REGISTRATION_TRANSACTION_ID;
                patientInsertStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;

                i_SqlBuilderStrategy[0] = patientInsertStrategy;    

                GuarantorInsertStrategy guarantorInsertStrategy = 
                    new GuarantorInsertStrategy();
                guarantorInsertStrategy.UserSecurityCode = 
                    this.AppUser.PBARSecurityCode;
                if( Account.Insurance.Coverages.Count == 0 )
                {
                    guarantorInsertStrategy.LastTransactionInGroup = 
                        LAST_TRANSACTION_IN_GROUP_FLAG;
                }
                i_SqlBuilderStrategy[1] = guarantorInsertStrategy; 

                InsuranceInsertStrategy strategy;
                for( int i = 1; i <= Account.Insurance.Coverages.Count; i++ )
                {
                    Coverage coverage = Account.Insurance.CoverageFor(i);
                    strategy = new InsuranceInsertStrategy( coverage.Priority );
                    strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                    //strategy.PreRegistrationFlag = PRE_REGISTRATION_FLAG;
                    strategy.OrignalTransactionId = PRE_REGISTRATION_TRANSACTION_ID;
                    if( i == Account.Insurance.Coverages.Count )
                    {
                        strategy.LastTransactionInGroup = 
                            LAST_TRANSACTION_IN_GROUP_FLAG;
                    }
                    i_SqlBuilderStrategy[i + 1] = strategy; 
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

        #region Construction and Finalization
        public PreregistrationTransactionCoordinator( Account anAccount )
            : base( anAccount )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }

        public PreregistrationTransactionCoordinator() : base()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public PreregistrationTransactionCoordinator(User user) 
            : base( user )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }  
        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        const int NUMBER_OF_INSERT_STRATEGIES = 4;
        const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 2;

        private const string
            PRE_REGISTRATION_TRANSACTION_ID = "PC",
            PRE_REGISTRATION_FLAG = "P";
        #endregion
    }
}
