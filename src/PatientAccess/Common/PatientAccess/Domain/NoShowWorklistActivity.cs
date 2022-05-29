using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for NoShowWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for NoShowWorklistActivity
    [Serializable]
    public class NoShowWorklistActivity : Activity
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
        public NoShowWorklistActivity()
        {
            this.Description    = "&No Show Worklist";
            this.ContextDescription  = "No Show Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
