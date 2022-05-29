using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByReligionActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByReligionActivity
    [Serializable]
    public class CensusByReligionActivity : Activity
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
        public CensusByReligionActivity()
        {
            this.Description        = "Census by &Religion";
            this.ContextDescription      = "Census by Religion";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
