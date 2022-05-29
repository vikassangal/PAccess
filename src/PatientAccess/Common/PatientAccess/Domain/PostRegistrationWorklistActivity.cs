using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PostRegistrationWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for PostRegistrationWorklistActivity
    [Serializable]
    public class PostRegistrationWorklistActivity : Activity
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
        public PostRegistrationWorklistActivity()
        {
            this.Description    = "&Postregistration Worklist";
            this.ContextDescription  = "Postregistration Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
