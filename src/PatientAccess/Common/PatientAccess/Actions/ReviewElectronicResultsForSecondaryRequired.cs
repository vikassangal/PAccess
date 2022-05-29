using System;
using System.Diagnostics;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ReviewElectronicResultsForSecondaryRequired : LeafAction
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

            this.Context = "SecondaryInsuranceVerification";
        }
        #endregion

        #region Properties

        // Override the base priority (50)... the lower the priority value, the more important

        public override int Priority
        {
            get
            {
                return PRIORITY_HIGH;
            }
        }

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
        public ReviewElectronicResultsForSecondaryRequired()
        {
        }
        public ReviewElectronicResultsForSecondaryRequired(Account anAccount)
        {
            this.Context = anAccount as object;
        }

        #endregion

        #region Data Elements
        
        //        System.Type avType = null;
        //        System.Type pavpaType = null;

        #endregion

        #region Constants

        public static int PRIORITY_HIGH = 10;

        #endregion
    }
}
