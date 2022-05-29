using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorDateOfBirthPreferred.
    /// </summary>
    [Serializable]
    [UsedImplicitly]
    public class GuarantorDateOfBirthPreferred : LeafRule
    {
        #region Event Handlers
        public event EventHandler GuarantorDateOfBirthPreferredEventhandler;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            GuarantorDateOfBirthPreferredEventhandler += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            GuarantorDateOfBirthPreferredEventhandler -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            this.GuarantorDateOfBirthPreferredEventhandler = null;
        }

        public override bool ShouldStopProcessing()
        {
            return true;
        }

        public override void ApplyTo(object context)
        {
        }

        public override bool CanBeAppliedTo(object context)
        {
            bool result = true;

            if (context == null || context.GetType() != typeof (Account))
            {
                return true;
            }

            Account anAccount = context as Account;
            if (anAccount == null)
            {
                return false;
            }
            GuarantorDateOfBirthFeatureManager = new GuarantorDateOfBirthFeatureManager();

            if (anAccount.Guarantor != null &&
                GuarantorDateOfBirthFeatureManager.ShouldWeEnableGuarantorDateOfBirthFeature(anAccount) &&
                anAccount.Guarantor.DateOfBirth.Equals(DateTime.MinValue))
            {
                if (this.FireEvents && GuarantorDateOfBirthPreferredEventhandler != null)
                {
                    GuarantorDateOfBirthPreferredEventhandler(this, null);
                }
                result = false;
            }
            return result;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        private IGuarantorDateOfBirthFeatureManager GuarantorDateOfBirthFeatureManager
        {
            get { return _GuarantorDateOfBirthFeatureManager; }
            set { _GuarantorDateOfBirthFeatureManager = value; }
        }

        #endregion

        #region Data Elements
        private IGuarantorDateOfBirthFeatureManager _GuarantorDateOfBirthFeatureManager;
        #endregion

        #region Constants
        #endregion
    }
}
