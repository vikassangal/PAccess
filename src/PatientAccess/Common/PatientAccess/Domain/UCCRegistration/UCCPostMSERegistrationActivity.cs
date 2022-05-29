using System;

namespace PatientAccess.Domain.UCCRegistration
{
    /// <summary>
    /// Summary description for UCCPostMSERegistrationActivity.
    /// </summary>
    //TODO: Create XML summary comment for UCCPostMSERegistrationActivity
    [Serializable]
    public class UCCPostMseRegistrationActivity : Activity
    {

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return false;
        }
        // Patient Type cannot change for this Activity
        public override bool CanPatientTypeChange()
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


        #region Construction and Finalization
        public UCCPostMseRegistrationActivity()
        {
            this.Description = "Urgent Care Post-MSE";
            this.ContextDescription = "Urgent Care Post-MSE";
        }
        #endregion


    }
}
