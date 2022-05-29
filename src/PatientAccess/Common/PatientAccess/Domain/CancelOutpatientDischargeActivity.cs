using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CancelOutpatientDischargeActivity : Activity
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
        public CancelOutpatientDischargeActivity()
        {
            this.Description    = "&Cancel Outpatient Discharge";
            this.ContextDescription  = "Cancel Outpatient Discharge";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
