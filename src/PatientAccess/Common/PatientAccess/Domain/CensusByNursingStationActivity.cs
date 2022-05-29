using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for CensusByNursingStationActivity.
	/// </summary>
	//TODO: Create XML summary comment for CensusByNursingStationActivity
    [Serializable]
    public class CensusByNursingStationActivity : Activity
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
        public CensusByNursingStationActivity()
        {
            this.Description    = "Census by &Nursing Station";
            this.ContextDescription  = "Census by Nursing Station";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
