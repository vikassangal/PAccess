using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PatientLiabilityWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for PatientLiabilityWorklistActivity
    [Serializable]
    public class PatientLiabilityWorklistActivity : Activity
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
        public PatientLiabilityWorklistActivity()
        {
            this.Description    = "Patient &Liability Worklist";
            this.ContextDescription  = "Patient Liability Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
