using System;

namespace PatientAccess.Domain.UCCRegistration
{
    /// <summary>
    /// Summary description for UCCPreMSERegistrationActivity.
    /// </summary>
    //TODO: Create XML summary comment for PreMSERegisterActivity
    [Serializable]
    public class UCCPreMSERegistrationActivity : Activity
    {

        #region Methods
        public override bool ReadOnlyAccount()
        {
            return true;
        }

        public override bool CanCreateNewPatient()
        {
            return true;
        }
        #endregion


        #region Construction and Finalization
        public UCCPreMSERegistrationActivity()
        {
            this.Description = "Urgent Care Pre-MSE";
            this.ContextDescription = "Urgent Care Pre-MSE";
        }
        #endregion


    }
}
