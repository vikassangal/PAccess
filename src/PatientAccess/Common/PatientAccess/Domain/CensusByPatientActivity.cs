using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByPatientActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByPatientActivity
    [Serializable]
    public class CensusByPatientActivity : Activity
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
        public CensusByPatientActivity()
        {
            this.Description    = "Census by &Patient";
            this.ContextDescription  = "Census by Patient";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
