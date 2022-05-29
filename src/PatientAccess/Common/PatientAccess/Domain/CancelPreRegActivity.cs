using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CancelPreRegActivity.
	/// </summary>
	//TODO: Create XML summary comment for CancelPreRegActivity
    [Serializable]
    public class CancelPreRegActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
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
        public CancelPreRegActivity()
        {
            this.Description    = "&Cancel Preregistration";
            this.ContextDescription  = "Cancel Preregistration";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
