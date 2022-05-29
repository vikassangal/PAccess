using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;

namespace PatientAccess.UI.Specialized
{
    /// <summary>
    /// 
    /// </summary>
    public class ClinicalTrialsBoardPatientAccessView : PatientAccessView
    {
		#region Constants 

        private const string DEFAULT_CTB_VALUE = " ";

		#endregion Constants 

		#region Methods 

        /// <summary>
        /// Handles the ActivityStarted event of the PatientAccessView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void PatientAccessView_ActivityStarted(object sender, EventArgs args)
        {

            base.PatientAccessView_ActivityStarted( sender, args );
            
            Account anAccount;
            
            if( !TryCastToAccount( args, out anAccount ) )
            {
                return;
            }

            if( TryAddTrialsFlagForRegistrationActivityTo( anAccount ) )
            {
                return;
            }

            TryRemoveTrialsFlagForActivityOn( anAccount );

        }


        /// <summary>
        /// Tries the add trials flag for registration activity to an account.
        /// </summary>
        /// <param name="anAccount">An account.</param>
        /// <returns></returns>
        private static bool TryAddTrialsFlagForRegistrationActivityTo( Account anAccount )
        {
            bool isHandled =
                    (anAccount.Activity is RegistrationActivity ||
                     anAccount.Activity is RegistrationWithOfflineActivity) &&
                    !anAccount.Patient.IsNew;
            
            // Copy the clinical trials flag from the patient record to the account
            if( isHandled )
            {
                anAccount[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS] =
                    anAccount.Patient[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS];
            }

            return isHandled;
        }


        /// <summary>
        /// Tries the cast to account.
        /// </summary>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <param name="anAccount">An account.</param>
        /// <returns></returns>
        private static bool TryCastToAccount( EventArgs args, out Account anAccount )
        {
            LooseArgs looseArguments = args as LooseArgs;
            
            anAccount = null;

            if( looseArguments == null)
            {
                return false;
            }

            anAccount = looseArguments.Context as Account;

            return ( anAccount != null );
        }


        /// <summary>
        /// If we don't do this, we can accidentally copy the flag value forward
        /// </summary>
        /// <param name="anAccount">An account.</param>
        /// <returns></returns>
        private static bool TryRemoveTrialsFlagForActivityOn( Account anAccount )
        {
            bool isHandled =
                    anAccount.Activity is AdmitNewbornActivity ||
                    anAccount.Activity is AdmitNewbornWithOfflineActivity ||
                    anAccount.Activity is PreAdmitNewbornActivity ||
                    anAccount.Activity is PreAdmitNewbornWithOfflineActivity ||
                    anAccount.Activity is PreRegistrationActivity ||
                    anAccount.Activity is PreRegistrationWithOfflineActivity ||
                    anAccount.Activity is PreRegistrationWorklistActivity ||
                    anAccount.Activity is PreMSERegisterActivity ||
                    anAccount.Activity is PreMSERegistrationWithOfflineActivity ||
                    anAccount.Activity is PreMSEWorklistActivity;

            // Clear out the carried-over flag from the mother's account
            if( isHandled )
            {
                anAccount[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS] = 
                    DEFAULT_CTB_VALUE;
            }

            return isHandled;
        }

		#endregion Methods 
    }
}
