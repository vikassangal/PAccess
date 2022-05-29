using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for ViewAccountActivity.
	/// </summary>
	//TODO: Create XML summary comment for ViewAccountActivity
    [Serializable]
    public class ViewAccountActivity : Activity
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
        public ViewAccountActivity()
        {
            this.Description    = "&View Account";
            this.ContextDescription  = "View Account";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
