using System;

namespace PatientAccess.Domain.QuickAccountCreation
{
    [Serializable]
    public class QACRegistrationWithOfflineActivity : Activity
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
        public QACRegistrationWithOfflineActivity()
        {
            this.Description    = "Enter QACE Registration with Offline Account Number...";
            this.ContextDescription  = "Enter QAC Registration with Offline Account Number...";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
