using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class PreRegistrationActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return true;
        }  

        public override bool CanCreateNewPatient()
        {
            return true;
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
        public PreRegistrationActivity()
        {
            this.Description    = "&Preregister Inpatient or Outpatient";
            this.ContextDescription  = "Preregister Inpatient or Outpatient";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
