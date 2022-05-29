using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for Edit MSE RegisterActivity.
    /// </summary>
    [Serializable]
    public class EditPreMseActivity : Activity
	{
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            // TLG 04/17/2007 - With Kevin and Deepa, determined the original value (true) was a bug

            return false;
        }

        public override bool CanCreateNewPatient()
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
        public EditPreMseActivity()
		{
            this.Description    = "Edit ED Patient Pre-&MSE";
            this.ContextDescription  = "Edit ED Patient Pre-MSE";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
