using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PreRegistrationWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for PreRegistrationWorklistActivity
    [Serializable]
    public class PreRegistrationWorklistActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
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
        public PreRegistrationWorklistActivity()
        {
            this.Description    = "Pre&registration Worklist";
            this.ContextDescription  = "Preregistration Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
