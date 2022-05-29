using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class PreAdmitNewbornWithOfflineActivity : Activity
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
        public PreAdmitNewbornWithOfflineActivity()
        {
            Description = "Enter Pre-Admit Newborn Registration with Offline Account Number...";
            ContextDescription = "Enter Pre-Admit Newborn Registration with Offline Account Number...";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion




    }
}
