using System;
using Extensions.Exceptions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for DischargeTransactionCoordinator.
    /// </summary>
    [Serializable]
    internal class DischargeTransactionCoordinator : TransactionCoordinator
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
                if( this.Account.KindOfVisit.IsInpatient )
                {
                    DischargeInsertStrategy strategy = new DischargeInsertStrategy();
                    strategy.TransactionFileId = DISCHARGE_TRANSACTION_ID;
                    strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                    this.NumberOfInsurances = 0;
                    this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
                    this.NumberOfOtherRecs = 0;
                    i_SqlBuilderStrategy[0] = strategy;
                }
                else if ( this.Account.KindOfVisit.IsOutpatient ||
                    this.Account.KindOfVisit.IsEmergencyPatient ||
                    this.Account.KindOfVisit.IsRecurringPatient ||
                    this.Account.KindOfVisit.IsNonPatient )
                {
                    EditRecurringDischargeInsertStrategy strategy = new EditRecurringDischargeInsertStrategy();
                    // override the class defaults since this is an EO transaction.
                    this.NumberOfInsurances = 0;
                    this.NumberOfNonInsurances = 0;
                    this.NumberOfOtherRecs = 1;
                    strategy.TransactionFileId = END_OUTPATIENT_VISIT_ID;
                    strategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
                    i_SqlBuilderStrategy[0] = strategy;
                }
                else
                {
                    throw new BrokerException("Attempting to discharge/end visit of patient of Invalid type",
                        null,Severity.High);
                }
                
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
        public DischargeTransactionCoordinator()
        {
            //this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public DischargeTransactionCoordinator(User user) 
            : base( user )
        {
            //this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        } 

        #endregion

        #region Data Elements
        private SqlBuilderStrategy[] i_SqlBuilderStrategy = new SqlBuilderStrategy[NUMBER_OF_INSERT_STRATEGIES];
        #endregion

        #region Constants
        private const int NUMBER_OF_INSERT_STRATEGIES = 1;
        private const string DISCHARGE_TRANSACTION_ID           = "DI";
        private const string END_OUTPATIENT_VISIT_ID            = "EO";
        private const int NUMBER_OF_NON_INSURANCE_TRANSACTIONS = 1;
        #endregion
    }
}
