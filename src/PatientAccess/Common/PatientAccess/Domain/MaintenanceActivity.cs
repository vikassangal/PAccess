using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class MaintenanceActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return false;
        }  
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is subject to HSV plan financial class restriction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is subject to HSV plan financial class restriction; otherwise, <c>false</c>.
        /// </value>
        public override bool IsSubjectToHSVPlanFinancialClassRestriction
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MaintenanceActivity()
        {
            this.Description    = "&Edit/Maintain Account";
            this.ContextDescription  = "Edit/Maintain Account";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}