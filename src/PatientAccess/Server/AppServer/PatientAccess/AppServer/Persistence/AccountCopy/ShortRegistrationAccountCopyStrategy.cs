using PatientAccess.Domain;

namespace PatientAccess.Persistence.AccountCopy
{
    /// <summary>
    /// Given the mother's account, create a newborn account
    /// </summary>
    class ShortRegistrationAccountCopyStrategy : BinaryCloneAccountCopyStrategy
    {
        #region Constants
        #endregion Constants

        #region Protected Methods

        /// <summary>
        /// Edits demographics -
        /// For Short Registration, we do not need to copy forward the patient's Physical Contact Point information.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditDemographicsUsing( Account newAccount, Account oldAccount )
        {
            base.EditDemographicsUsing( newAccount, oldAccount );
            newAccount.IsShortRegistered = true;
        }

        /// <summary>
        /// Edits the guarantor.
        /// For Short Registration, we do not need to copy forward the 
        /// Guarantor's Gender, Employment and Email Address information.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditGuarantorUsing( Account newAccount, Account oldAccount )
        {
            base.EditGuarantorUsing( newAccount, oldAccount );

            newAccount.Guarantor.Sex = new Gender();
            newAccount.Guarantor.Employment = new Employment();

            
        }

        #endregion Protected Methods

        #region Private Methods
        #endregion Private Methods
    }
}
