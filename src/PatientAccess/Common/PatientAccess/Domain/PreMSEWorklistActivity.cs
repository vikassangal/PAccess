using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PreMSEWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for PreMSEWorklistActivity
    [Serializable]
    public class PreMSEWorklistActivity : Activity
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
        public PreMSEWorklistActivity()
        {
            this.Description    = "Pre-&MSE Worklist";
            this.ContextDescription  = "Pre-MSE Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
