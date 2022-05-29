using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ShortRegistrationWithOfflineActivity : Activity
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
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ShortRegistrationWithOfflineActivity()
        {
            this.Description = "Enter Diagnostic Registration with Offline Account Number...";
            this.ContextDescription = "Enter Diagnostic Registration with Offline Account Number...";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
