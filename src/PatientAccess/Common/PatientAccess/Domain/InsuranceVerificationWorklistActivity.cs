using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for InsuranceVerificationWorklistActivity.
	/// </summary>
	//TODO: Create XML summary comment for InsuranceVerificationWorklistActivity
    [Serializable]
    public class InsuranceVerificationWorklistActivity : Activity
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
        public InsuranceVerificationWorklistActivity()
        {
            this.Description    = "Insurance &Verification Worklist";
            this.ContextDescription  = "Insurance Verification Worklist";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }}
