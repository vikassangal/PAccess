using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PreMSERegisterActivity.
	/// </summary>
	//TODO: Create XML summary comment for PreMSERegisterActivity
    [Serializable]
    public class PreMSERegisterActivity : Activity
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
        public PreMSERegisterActivity()
        {
            this.Description    = "Register ED Patient Pre-&MSE";
            this.ContextDescription  = "Register ED Patient Pre-MSE";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
