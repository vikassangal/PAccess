using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByADTActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByADTActivity
    [Serializable]
    public class CensusByADTActivity : Activity
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
        public CensusByADTActivity()
        {
            this.Description    = "Census by A-D-&T";
            this.ContextDescription  = "Census by A-D-T";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
