using PatientAccess.Domain.Specialized;

namespace PatientAccess.Persistence.Specialized
{

    /// <summary>
    /// 
    /// </summary>
    public class ClinicalTrialsPatientInsertStrategy : PatientInsertStrategy
    {

        /// <summary>
        /// Sets the account clinical trial board flag.
        /// </summary>
        /// <value>The account clinical trial board flag.</value>
        private string AccountClinicalTrialBoardFlag
        {
            set
            {
                this.patientDetailsOrderedList[ClinicalTrialsConstants.KEY_CLINICAL_TRIAL_FLAG_TRANSACTION_FIELD] =
                    value;
            }
        }

        /// <summary>
        /// Updates the column values using.
        /// </summary>
        /// <param name="account">The account.</param>
        public override void UpdateColumnValuesUsing( Domain.Account account )
        {
            
            base.UpdateColumnValuesUsing( account );

            if( account.HasExtendedProperty( ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS ) )
            {
                this.AccountClinicalTrialBoardFlag =
                    account[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS].ToString();
            }

        }

    }
}
