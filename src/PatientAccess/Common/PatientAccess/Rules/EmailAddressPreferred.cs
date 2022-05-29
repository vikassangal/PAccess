using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class EmailAddressPreferred : LeafRule
    {
        private Account anAccount;
        #region Event Handlers

        public event EventHandler EmailAddressPreferredEvent;
       
        #endregion

        #region Methods
        private IEmailAddressFeatureManager emailAddressFeatureManager;
        private IPatientPortalOptInFeatureManager patientPortalOptInFeatureManager;
        private IHospitalCommunicationOptInFeatureManager hospitalCommunicationOptInFeatureManager; 
        private IEmailAddressFeatureManager EmailAddressFeatureManager
        {
            get { return emailAddressFeatureManager; }
            set { emailAddressFeatureManager = value; }
        }

        private IHospitalCommunicationOptInFeatureManager HospitalCommunicationOptInFeatureManager
        {
            get { return hospitalCommunicationOptInFeatureManager; }
            set { hospitalCommunicationOptInFeatureManager = value; }
        }

        private IPatientPortalOptInFeatureManager PatientPortalOptInFeatureManager
        {
            get { return patientPortalOptInFeatureManager; }
            set { patientPortalOptInFeatureManager = value; }
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
            HospitalCommunicationOptInFeatureManager = new HospitalCommunicationOptInFeatureManager();
            PatientPortalOptInFeatureManager = new PatientPortalOptInFeatureManager(); 

            if ((IsEmailReasonBlank || anAccount.HideEmailReason) && 
                ApplyPreferredRuleForRegistrationAccount && 
                EmailAddressFeatureManager.ShouldFeatureBeEnabled(anAccount) &&
                String.IsNullOrEmpty(anAccount.Patient.GetEmailAddress())
                )
            {
                if (FireEvents && EmailAddressPreferredEvent != null)
                {
                    EmailAddressPreferredEvent(this, null);
                }

                return false;
            }
            return true;
        }

        private bool IsEmailReasonBlank
        {
            get { return (anAccount.Patient.EmailReason.Code == EmailReason.BLANK); }
        }
        
        private bool ApplyPreferredRuleForRegistrationAccount
        {
            get
            {
                return (EmailAddressPreferredForActivity && IsPatientPortalOptInBlank && IsHospitalCommunicationOptInBlank);
            }
        }

        private bool IsPatientPortalOptInBlank
        {
            get
            {
                return ! PatientPortalOptInFeatureManager.ShouldFeatureBeEnabled(anAccount) ||
                       anAccount.PatientPortalOptIn.IsBlank;
            }
        }

        private bool IsHospitalCommunicationOptInBlank
        {
            get
            {
                return ! hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(anAccount) ||
                       anAccount.Patient.HospitalCommunicationOptIn.IsBlank;
            }
        }

        private bool EmailAddressPreferredForActivity
        {
            get
            {
                return (IsRegistrationAccount || IsDiagnosticPreregistrationAccount || IsDiagnosticAccount || IsPreRegistrationAccount);
            }
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
        private bool IsPreRegistrationAccount
        {
            get
            {
                return (anAccount.KindOfVisit.IsPreRegistrationPatient);
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
       
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmailAddressPreferredEvent += eventHandler;
            return true;
        }

        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmailAddressPreferredEvent -= eventHandler;
            return true;
        }

        public override void UnregisterHandlers()
        {
            EmailAddressPreferredEvent = null;
        }
       
        #endregion
    }
}
