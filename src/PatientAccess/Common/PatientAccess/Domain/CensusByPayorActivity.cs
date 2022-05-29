using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByPayorActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByPayorActivity
    [Serializable]
    public class CensusByPayorActivity : Activity
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
        public CensusByPayorActivity()
        {
            this.Description    = "Census by &Payor";
            this.ContextDescription  = "Census by Payor";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
