using System;


namespace PatientAccess.Domain
{
    [Serializable]
    public class PreAdmitNewbornActivity : Activity
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
        public PreAdmitNewbornActivity()
        {
            Description = "Pre-Admit Newborn";
            ContextDescription = "Pre-Admit Newborn";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
