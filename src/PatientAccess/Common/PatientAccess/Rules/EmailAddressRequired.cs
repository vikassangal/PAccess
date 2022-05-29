using System;
using Extensions.UI.Builder;
using PatientAccess.Annotations;
using PatientAccess.Domain;

namespace PatientAccess.Rules
{
    [Serializable]
    [UsedImplicitly]
    public class EmailAddressRequired : LeafRule
    {
        [NonSerialized]
        private IPatientPortalOptInFeatureManager patientPortalOptInFeatureManager;

        private IEmailAddressFeatureManager emailAddressFeatureManager;
        private IHospitalCommunicationOptInFeatureManager hospitalCommunicationOptInFeatureManager;
        private IEmailReasonFeatureManager emailReasonFeatureManager  ;

        private Account anAccount;

        private IPatientPortalOptInFeatureManager PatientPortalOptInFeatureManager
        {
            get { return patientPortalOptInFeatureManager; }
            set { patientPortalOptInFeatureManager = value; }
        }

        private IEmailAddressFeatureManager EmailAddressFeatureManager
        {
            get { return emailAddressFeatureManager; }
            set { emailAddressFeatureManager = value; }
        }

        private IEmailReasonFeatureManager EmailReasonFeatureManager
        {
            get { return emailReasonFeatureManager; }
            set { emailReasonFeatureManager = value; }
        }
        private IHospitalCommunicationOptInFeatureManager HospitalCommunicationOptInFeatureManager
        {
            get { return hospitalCommunicationOptInFeatureManager; }
            set { hospitalCommunicationOptInFeatureManager = value; }
        }
        #region Event Handlers
        public event EventHandler EmailAddressRequiredEvent;
        #endregion

        #region Methods
        public override bool RegisterHandler(EventHandler eventHandler)
        {
            EmailAddressRequiredEvent += eventHandler;
            return true;
        }
        public override bool UnregisterHandler(EventHandler eventHandler)
        {
            EmailAddressRequiredEvent -= eventHandler;
            return true;
        }
        public override void UnregisterHandlers()
        {
            EmailAddressRequiredEvent = null;
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
            EmailReasonFeatureManager = new EmailReasonFeatureManager();

            if (IsEmailAddressEnabled &&
                (IsOptInFlagsYes || HasValidEmailReason ) &&
                String.IsNullOrEmpty(anAccount.Patient.GetEmailAddress()))
            {
                if (FireEvents && EmailAddressRequiredEvent != null)
                {
                    EmailAddressRequiredEvent(this, null);
                }

                return false;
            }
            return true;
        }

        private bool EmailAddressRequiredForActivity
        {
            get
            {
                return (IsRegistrationAccount || IsDiagnosticAccount || IsPreRegistrationAccount ||
                        IsTransferActivity);
            }
        }

        private bool IsTransferActivity
        {
            get
            {
                return anAccount.Activity.IsTransferERToOutpatientActivity() ||
                       anAccount.Activity.IsTransferOutToInActivity();
            }
        }
        private bool IsPreRegistrationAccount
        {
            get
            {
                return (anAccount.KindOfVisit.IsPreRegistrationPatient);
            }
        }

        private bool HasValidEmailReason
        {
            get
            {
                return (IsEmailReasonAsEmailProvided &&
                        EmailReasonFeatureManager.ShouldFeatureBeVisibleForAccountCreatedDate(anAccount) &&
                        !anAccount.HideEmailReason);
            }
        }

        private bool IsEmailAddressEnabled
        {
            get
            {
                return (EmailAddressRequiredForActivity &&
                        EmailAddressFeatureManager.ShouldFeatureBeEnabled(anAccount) && HasValidCOSSigned );
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

        private bool HasValidCOSSigned
        {
            get { return (IsPreRegistrationAccount || IsTransferActivity || !anAccount.COSSigned.IsRefused); }
        }
        private bool IsDiagnosticAccount
        {
            get
            {
                return (anAccount.Activity.IsDiagnosticRegistrationActivity() ||
                        anAccount.IsDiagnosticRegistrationMaintenanceAccount());
            }
        }

        private bool IsEmailReasonAsEmailProvided
        {
            get
            {
                return (anAccount.Patient.EmailReason.IsProvided);
            }
        }

        private bool IsOptInFlagsYes
        {
            get { return (IsPatientPortalOptinYes || IsHospitalCommunicationOptInYes); }
        }


        private bool IsPatientPortalOptinYes
        {
            get
            {
                return ((PatientPortalOptInFeatureManager.ShouldFeatureBeEnabled(anAccount)) &&
                        anAccount.PatientPortalOptIn.IsYes);
            }
        }

        private bool IsHospitalCommunicationOptInYes
        {
            get
            {
                return ((HospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(anAccount)) && HospitalCommunicationOptInFeatureManager.IsValidActivityForEmailAddress(anAccount) &&
                        anAccount.Patient.HospitalCommunicationOptIn.IsYes);
            }
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


