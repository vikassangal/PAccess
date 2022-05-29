using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class GuarantorEmailAddressPreferred : LeafRule
    {
        private Account anAccount;

        #region Event Handlers
        public event EventHandler GuarantorEmailAddressPreferredEvent;

        #endregion

        #region Methods
        private IEmailAddressFeatureManager emailAddressFeatureManager;

        private IEmailAddressFeatureManager EmailAddressFeatureManager
        {
            get { return emailAddressFeatureManager; }
            set { emailAddressFeatureManager = value; }
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
            if (IsEmailAddressPreferredforRegistrationandPreRegistrationActivities()
                && EmailAddressFeatureManager.ShouldFeatureBeEnabled(anAccount)
                && String.IsNullOrEmpty(anAccount.Guarantor.GetEmailAddress()))
                
            {
                if (FireEvents && GuarantorEmailAddressPreferredEvent != null)
                {
                    GuarantorEmailAddressPreferredEvent(this, null);
                }

                return false;
            }
            return true;
        }

        private bool IsEmailAddressPreferredforRegistrationandPreRegistrationActivities()
        {
            return (IsDiagnosticPreregistrationAccount || IsDiagnosticAccount || IsRegistrationAccount || IsPreRegistrationAccount);
        }

        private bool IsDiagnosticPreregistrationAccount
        {
            
            get
            {
                return (anAccount.Activity.IsDiagnosticPreRegistrationActivity() ||
                        anAccount.IsDiagnosticPreRegistrationMaintenanceAccount());
            }
        }
        private bool IsDiagnosticAccount
        {
            get
            {
                return (anAccount.Activity.IsDiagnosticRegistrationActivity() ||
                        anAccount.IsDiagnosticRegistrationMaintenanceAccount());
            }
        }

        private bool IsRegistrationAccount
        {
            
            get
            {
                return (anAccount.Activity.IsRegistrationActivity() ||
                        anAccount.Activity.IsAdmitNewbornActivity() ||
                        anAccount.Activity.IsPostMSEActivity() ||
                        anAccount.Activity.IsUccPostMSEActivity() ||
                        anAccount.Activity.IsMaintenanceActivity());
            }
        }
        private bool IsPreRegistrationAccount
        {

            get
            {
                return (anAccount.Activity.IsPreRegistrationActivity() ||
                        anAccount.Activity.IsPreAdmitNewbornActivity() ||
                        anAccount.Activity.IsEditPreMseActivity());
            }
        }
 

        public override bool RegisterHandler(EventHandler eventHandler)
        {
            GuarantorEmailAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            GuarantorEmailAddressPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            GuarantorEmailAddressPreferredEvent = null;
        }
        #endregion
    }
}
