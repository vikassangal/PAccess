using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace PatientAccess.Persistence.AccountCopy
{

    /// <summary>
    /// Make a copy of the an account without Insurance and Guarantor
    /// information to avoid EMTALA issues.
    /// </summary>
    class PreMseAccountCopyStrategy : BinaryCloneAccountCopyStrategy
    {
		#region Protected Methods 

        /// <summary>
        /// Edits the diagnosis.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditDiagnosisUsing(Account newAccount, Account oldAccount)
        {
            if(oldAccount.Activity is PreMSERegisterActivity)
            {
            base.EditDiagnosisUsing(newAccount, oldAccount);
            
            newAccount.KindOfVisit = VisitType.Emergency;
            }
            else if (oldAccount.Activity is UCCPreMSERegistrationActivity)
            {

             base.EditDiagnosisUsing(newAccount, oldAccount);

             newAccount.KindOfVisit = VisitType.UCCOutpatient;

            }
        }

        /// <summary>
        /// Edits the general information using.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditGeneralInformationUsing(Account newAccount, Account oldAccount)
        {

            base.EditGeneralInformationUsing(newAccount, oldAccount);

            newAccount.PreMSECopiedAccountNumber = oldAccount.AccountNumber;

        }

        /// <summary>
        /// Edits the guarantor.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditGuarantorUsing(Account newAccount, Account oldAccount)
        {

            // We don't want this on the account as it could provide evidence for
            // and EMTALA challenge
            newAccount.Guarantor = new Guarantor();

        }

        /// <summary>
        /// Edits the insurance.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditInsuranceUsing(Account newAccount, Account oldAccount)
        {
            PullPriorVisitInsuranceToPreMseFeatureManager pullPriorVisitInsuranceToPreMseFeatureManager =
                new PullPriorVisitInsuranceToPreMseFeatureManager();
            var featureEnabled = pullPriorVisitInsuranceToPreMseFeatureManager
                .IsPullPriorVisitInsuranceToPreMseEnabledForDate(
                    newAccount);

            // We don't want this on the account as it could provide evidence for
            // and EMTALA challenge
            if (!(newAccount.Activity is PreMSERegisterActivity) || !featureEnabled)
            {
                newAccount.Insurance = new Insurance();
                newAccount.MedicareSecondaryPayor = new MedicareSecondaryPayor();
            }
        }

        /// <summary>
        /// Edits the regulatory.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditRegulatoryUsing(Account newAccount, Account oldAccount)
        {
            base.EditRegulatoryUsing(newAccount, oldAccount);

            newAccount.OptOutHealthInformation = false;
            newAccount.OptOutLocation = false;
            newAccount.OptOutName = false;
            newAccount.OptOutReligion = false;

        }

		#endregion Protected Methods 
    }

}
