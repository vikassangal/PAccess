using System;

namespace PatientAccess.Domain.QuickAccountCreation
{
    [Serializable]
    public class QuickAccountCreationActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override bool ReadOnlyAccount()
        {
            if ( AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
            {
                return false;
            }

            return true;
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
        public QuickAccountCreationActivity()
        {
            Description = "Quick Account Creation";
            ContextDescription = "Quick Account Creation";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
