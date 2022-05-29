using System;

namespace PatientAccess.Domain
{
    //TODO: Create XML summary comment for TransferOutToInActivity
    [Serializable]
    public class TransferOutToInActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return false;
        }  
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TransferOutToInActivity()
        {
            this.Description    = "Transfer &Outpatient to Inpatient";
            this.ContextDescription  = "Transfer Outpatient to Inpatient";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
