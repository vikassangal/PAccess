using System;
using System.Diagnostics;
using Extensions.UI.Builder;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
	/// <summary>
	/// Summary description for ViewGuarentorVerification.
	/// </summary>
	//TODO: Create XML summary comment for ViewGuarentorVerification
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ViewGuarentorVerification : LeafAction
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void Execute()
        {
            IAccountView accountView = ActionHelper.LoadAccountView();

            accountView.SetModel(AccountContext);
            accountView.ActivateGuarantorTab();
        }
        #endregion

        #region Properties
        public  Account AccountContext
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
        public ViewGuarentorVerification()
        {
        }
        public ViewGuarentorVerification(Account anAccount)
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
