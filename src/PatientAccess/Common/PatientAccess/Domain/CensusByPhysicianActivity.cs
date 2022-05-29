using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByPhysicianActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByPhysicianActivity
    [Serializable]
    public class CensusByPhysicianActivity : Activity
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
        public CensusByPhysicianActivity()
        {
            this.Description    = "Census by P&hysician";
            this.ContextDescription  = "Census by Physician";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
