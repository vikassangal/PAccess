using System;

namespace PatientAccess.Domain.QuickAccountCreation
{
    [Serializable]
    public class QuickAccountMaintenanceActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
           return false ;
        }
        // Patient Type cannot change for this Activity
        public override bool CanPatientTypeChange()
        {
            return false;
        }

        public override bool CanCreateNewPatient()
        {
            return true;
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
        public QuickAccountMaintenanceActivity()
        {
            Description = "Quick Account Maintenance(QAC)";
            ContextDescription = "Edit/Maintain Quick Account";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
