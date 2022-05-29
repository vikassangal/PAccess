using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByBloodlessActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByBloodlessActivity
    [Serializable]
    public class CensusByBloodlessActivity : Activity
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
        public CensusByBloodlessActivity()
        {
            this.Description    = "Census by &Bloodless";
            this.ContextDescription  = "Census by Bloodless";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
