using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Actions
{
    [Serializable]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class GenericCompositeAction : CompositeAction
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
        }
        #endregion

        #region Properties
        public  Account AccountContext
        {
            get
            {                              
                return null;
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public GenericCompositeAction()
        {
        }
        public GenericCompositeAction(Account anAccount)
        {
            this.Context = anAccount as object;
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
