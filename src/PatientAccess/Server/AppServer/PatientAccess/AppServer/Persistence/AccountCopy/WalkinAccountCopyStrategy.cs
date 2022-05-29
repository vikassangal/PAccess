using PatientAccess.Domain;

namespace PatientAccess.Persistence.AccountCopy
{
    /// <summary>
    ///WalkinAccountCopyStrategy
    /// </summary>
    class WalkinAccountCopyStrategy : BinaryCloneAccountCopyStrategy
    {
        #region Constants
        #endregion Constants

        #region Protected Methods

        /// <summary>
        /// Edits demographics -
        /// For WalkinAccountCopyStrategy, we do not need to copy forward the patient's Physical Contact Point information.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        protected override void EditDemographicsUsing( Account newAccount, Account oldAccount )
        {
            base.EditDemographicsUsing( newAccount, oldAccount );
            newAccount.IsShortRegistered = false;
            newAccount.IsQuickRegistered = false;
            newAccount.IsPAIWalkinRegistered = true;
        }

        /// <summary>
        /// Edits the insurance.
        /// </summary>
        /// <param name="newAccount">The new account.</param>
        /// <param name="oldAccount">The old account.</param>
        /// <returns></returns>
        protected override void EditInsuranceUsing( Account newAccount, Account oldAccount )
        {
            base.EditInsuranceUsing( newAccount, oldAccount );
            newAccount.MedicareSecondaryPayor =
                    MedicareSecondaryPayor.GetPartiallyCopiedForwardMSPFrom( oldAccount.MedicareSecondaryPayor );

        }

        #endregion Protected Methods

        #region Private Methods
        #endregion Private Methods
    }
}
