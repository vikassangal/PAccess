using System;
using System.Diagnostics;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class UnknownPatientGender : LeafAction
    {
        #region Event Handlers
        #endregion

        #region Methods
        /// <summary>
        /// This routine gets the AccountView singleton instance and sets the desired tab
        /// on the view.  It fires an event which results in displaying the AccountView 
        /// with the account data.
        /// </summary>
        public override void Execute()
        {
            //            IAccountView accountView = Actions.ActionHelper.LoadAccountView();
            //
            //            accountView.SetModel(AccountContext);
            //            accountView.ActivateDemographicsTab();

            this.Context = "Demographics";
        } 
        #endregion

        #region Properties
        public Account AccountContext
        {
            get
            {
                IAccount iAccount = Context as IAccount;
                Debug.Assert( iAccount != null );
                return iAccount.AsAccount();
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public UnknownPatientGender()
        {
        }
        public UnknownPatientGender(Account anAccount)
        {
            this.Context = anAccount as object;
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
