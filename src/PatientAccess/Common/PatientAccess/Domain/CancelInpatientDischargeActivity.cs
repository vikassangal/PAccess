using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class CancelInpatientDischargeActivity : Activity
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
        public CancelInpatientDischargeActivity()
        {
            this.Description    = "&Cancel Inpatient Discharge";
            this.ContextDescription  = "Cancel Inpatient Discharge";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
