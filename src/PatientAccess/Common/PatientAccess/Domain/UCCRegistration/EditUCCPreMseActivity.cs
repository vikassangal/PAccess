using System;

namespace PatientAccess.Domain
{
    /// <summary>
    /// Summary description for Edit UCC PreMSE RegisterActivity.
    /// </summary>
    [Serializable]
    public class EditUCCPreMSEActivity : Activity
    {

        #region Methods
        public override bool ReadOnlyAccount()
        {

            return false;
        }

        public override bool CanCreateNewPatient()
        {
            return false;
        }

        #endregion

        #region Construction and Finalization
        public EditUCCPreMSEActivity()
        {
            this.Description = "Edit ED Patient UCC Pre-MSE";
            this.ContextDescription = "Edit ED Patient UCC Pre-MSE";
        }
        #endregion


    }
}
