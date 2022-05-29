using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BenefitsVerificationIncomplete : LeafAction
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
            this.Context = "PrimaryInsuranceVerification";
        }
        #endregion

        #region Properties

        
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public BenefitsVerificationIncomplete() {}
        public BenefitsVerificationIncomplete(Account context)
        {
            this.Context = context;
        }

        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
