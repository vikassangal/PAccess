using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for EditAccountActivity.
	/// </summary>
	//TODO: Create XML summary comment for EditAccountActivity
    [Serializable]
    public class EditAccountActivity : Activity
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
        public EditAccountActivity()
        {
            this.Description    = @"&Edit/Maintain Account";
            this.ContextDescription  = "Edit/Maintain Account";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
