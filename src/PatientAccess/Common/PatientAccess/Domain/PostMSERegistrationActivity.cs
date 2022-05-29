using System;

namespace PatientAccess.Domain
{
	/// <summary>
	/// Summary description for PostMSERegistrationActivity.
	/// </summary>
//TODO: Create XML summary comment for PostMSERegistrationActivity
	[Serializable]
	public class PostMSERegistrationActivity : Activity
	{
		#region Event Handlers
		#endregion

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

		#region Private Methods
		#endregion

		#region Private Properties
		#endregion

		#region Construction and Finalization
		public PostMSERegistrationActivity()
		{
            this.Description    = "Register ED Patient P&ost-MSE";
            this.ContextDescription  = "Register ED Patient Post-MSE";
		}
		#endregion

		#region Data Elements
		#endregion

		#region Constants
		#endregion
	}
}
