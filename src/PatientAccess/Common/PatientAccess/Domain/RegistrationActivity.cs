using System;

namespace PatientAccess.Domain
{
    [Serializable]
    public class RegistrationActivity : Activity
    {
        #region Event Handlers
        #endregion

        #region Methods
                
        public override bool ReadOnlyAccount()
        {
            if( this.AssociatedActivityType == typeof(ActivatePreRegistrationActivity)  )
            {
                return false;
            }
            else
            {
                return true;
            }
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
        public RegistrationActivity()
        {
            this.Description    = "&Register Inpatient or Outpatient";     
            this.ContextDescription  = "Register Inpatient or Outpatient";
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}
