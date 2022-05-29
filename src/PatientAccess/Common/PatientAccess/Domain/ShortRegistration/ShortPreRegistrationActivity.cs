using System;

namespace PatientAccess.Domain.ShortRegistration
{
    [Serializable]
    public class ShortPreRegistrationActivity : Activity
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
        public ShortPreRegistrationActivity()
        {
            Description = "Preregister Diagnostic Outpatient (Short Prereg)";
            ContextDescription = "Preregister Diagnostic Outpatient (Short Prereg)";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
