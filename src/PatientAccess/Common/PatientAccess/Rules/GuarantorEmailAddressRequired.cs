using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class GuarantorEmailAddressRequired : LeafRule
    {
        [NonSerialized]
        
        private IEmailAddressFeatureManager emailAddressFeatureManager;

        private Account anAccount;

        private IEmailAddressFeatureManager EmailAddressFeatureManager
        {
            get { return emailAddressFeatureManager; }
            set { emailAddressFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler GuarantorEmailAddressRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            GuarantorEmailAddressRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            GuarantorEmailAddressRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            GuarantorEmailAddressRequiredEvent = null;
        }
        public override bool CanBeAppliedTo(object context)
        {
            if (context == null ||
                context.GetType() != typeof(Account))
            {
                return true;
            }
            anAccount = context as Account;
            if (anAccount == null)
            {
                return true;
            }
            
            EmailAddressFeatureManager = new EmailAddressFeatureManager();

            if (  EmailAddressFeatureManager.ShouldFeatureBeEnabled(anAccount) &&
                String.IsNullOrEmpty(anAccount.Guarantor.GetEmailAddress() )
                )
            {
                if (FireEvents && GuarantorEmailAddressRequiredEvent != null)
                {
                    GuarantorEmailAddressRequiredEvent(this, null);
                }

                return false;
            }
            return true;
        }

       

        public override void ApplyTo(object context)
        {

        }

        public override bool ShouldStopProcessing()
        {
            return false;
        }
        #endregion
    }
}


