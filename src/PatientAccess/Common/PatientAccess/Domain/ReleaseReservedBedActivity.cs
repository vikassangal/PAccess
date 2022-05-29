using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class ReleaseReservedBedActivity : Activity
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
        public ReleaseReservedBedActivity()
        {
            this.Description    = "Release bed when PBAR is not available";
            this.ContextDescription  = "Release bed when PBAR is not available";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}

