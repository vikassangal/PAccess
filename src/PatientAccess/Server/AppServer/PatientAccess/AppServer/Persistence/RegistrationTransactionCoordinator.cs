using System;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.Factories;

namespace PatientAccess.Persistence
{
	/// <summary>
	/// Summary description for RegistrationTransactionCoordinator.
	/// </summary>
    [Serializable]
    internal class RegistrationTransactionCoordinator : TransactionCoordinator
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
                if( this.Account.KindOfVisit.IsInpatient )
                {
                    patientInsertStrategy.TransactionFileId = ADMISSION_TRANSACTION_CODE;
                }
                else
                {
                    patientInsertStrategy.TransactionFileId = REGISTRATION_TRANSACTION_CODE;
                    
                }
                patientInsertStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;

                i_SqlBuilderStrategy[0] = patientInsertStrategy;

                GuarantorInsertStrategy guarantorInsertStrategy = new GuarantorInsertStrategy();
                guarantorInsertStrategy.UserSecurityCode = this.AppUser.PBARSecurityCode;
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
                    if( this.Account.KindOfVisit.IsInpatient )
                    {
                        strategy.OrignalTransactionId = ADMISSION_TRANSACTION_CODE;
                    }
                    else
                    {
                        strategy.OrignalTransactionId = REGISTRATION_TRANSACTION_CODE;
                    }

                    // if this is an admission then set this field. Else leave it blank
                    // if isNew == false then this must be a pre-reg activation
                   if( //this.Account.KindOfVisit.IsInPatient() == true &&
                       this.Account.IsNew == false && // which means that is already existed so it must have been pre-reg'ed
                        (this.Account.Activity.GetType() == typeof(RegistrationActivity)
                            ||
                            this.Account.Activity.GetType() == typeof( ShortRegistrationActivity ))
                       ||
                       this.Account.Activity.GetType() == typeof( ActivatePreRegistrationActivity ) )
                    {
                        strategy.PreRegistrationFlag = PRE_REGISTRATION_FLAG;
                    }
                    
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
        public RegistrationTransactionCoordinator( Account anAccount )
            : base( anAccount )
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }

        public RegistrationTransactionCoordinator() : base()
        {
            this.NumberOfNonInsurances = NUMBER_OF_NON_INSURANCE_TRANSACTIONS;
        }
        public RegistrationTransactionCoordinator(User user) 
            : base(user )
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
            REGISTRATION_TRANSACTION_CODE               = "PF",
            ADMISSION_TRANSACTION_CODE                  = "PD",
            PRE_REGISTRATION_FLAG                       = "P";
        #endregion
    }
}
