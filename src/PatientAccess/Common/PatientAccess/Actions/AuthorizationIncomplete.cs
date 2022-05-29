using System;
using System.Diagnostics;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AuthorizationIncomplete : LeafAction
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

            this.Context = "PrimaryInsuranceVerification";
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
        public AuthorizationIncomplete()
        {
        }
        public AuthorizationIncomplete(Account anAccount)
        {
            this.Context = anAccount;
        }

        #endregion

        #region Data Elements
        
//        System.Type avType = null;
//        System.Type pavpaType = null;

        #endregion

        #region Constants
        #endregion
    }
}
