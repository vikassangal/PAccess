using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class AdmitNewbornActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return false;
        }

        public override bool CanCreateNewPatient()
        {
            return false;
        }
        // Patient Type cannot change for this Activity
        public override bool CanPatientTypeChange()
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

        public AdmitNewbornActivity()
        {
            this.Description    = "Register &Newborn";
            this.ContextDescription  = "Register Newborn";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
