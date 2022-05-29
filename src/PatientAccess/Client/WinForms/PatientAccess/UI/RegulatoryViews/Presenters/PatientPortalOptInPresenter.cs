using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.RegulatoryViews.ViewImpl;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class PatientPortalOptInPresenter
    {
        public readonly IPatientPortalOptInView PatientPortalOptInView = new PatientPortalOptInView();

        public PatientPortalOptInPresenter(IPatientPortalOptInView portalOptInView, IMessageBoxAdapter messageBoxAdapter,
            Account account,
            PatientPortalOptInFeatureManager featureManager, RuleEngine ruleEngine)
        {
            Guard.ThrowIfArgumentIsNull(PatientPortalOptInView, "PatientPortalOptInView");
            Guard.ThrowIfArgumentIsNull(account, "Account");
            Guard.ThrowIfArgumentIsNull(patientPortalOptInFeatureManager, "PatientPortalOptInFeatureManager");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");

            PatientPortalOptInView = portalOptInView;
            Account = account;
            patientPortalOptInFeatureManager = featureManager;
            MessageBoxAdapter = messageBoxAdapter;
            RuleEngine = ruleEngine;
            PatientPortalOptInView.PatientPortalOptInPresenter = this;
           
        }
        public Account Account { get; private set; }
        public void ValidateEmailAddress()
        {
            if (Account != null && Account.Patient != null)
            {
                var mailingContactPoint =
                    Account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());

                if (mailingContactPoint == null || mailingContactPoint.EmailAddress == null ||
                    String.IsNullOrEmpty(mailingContactPoint.EmailAddress.ToString()))
                {
                    if (IsFeatureEnabled() && Account.PatientPortalOptIn.IsYes)
                    {
                        if (Account.Activity.IsTransferERToOutpatientActivity() ||
                            Account.Activity.IsTransferOutToInActivity())
                        {

                            MessageBoxAdapter.ShowMessageBox(
                                UIErrorMessages.PATIENT_PORTAL_OPTIN_EMAIL_REQUIRED_MSG_For_TRANSFER,
                                UIErrorMessages.WARNING,
                                MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                MessageBoxAdapterDefaultButton.Button1);

                        }
                        else
                        {
                            MessageBoxAdapter.ShowMessageBox(UIErrorMessages.PATIENT_PORTAL_OPTIN_EMAIL_REQUIRED_MSG,
                                UIErrorMessages.WARNING,
                                MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                                MessageBoxAdapterDefaultButton.Button1);
                        }
                    }
                }
            }
        }
        public void OptIn()
        {
            Account.PatientPortalOptIn.SetYes();
            EvaluateEmailAddress();
        }
        public void OptOut()
        {
            Account.PatientPortalOptIn.SetNo();
            EvaluateEmailAddress();
        }

        public void Unable()
        {
            Account.PatientPortalOptIn.SetUnable();
            EvaluateEmailAddress();
        }

        private void EvaluateEmailAddress()
        {
            if (Account != null)
            {
                if (Account.Patient != null)
                {
                    RuleEngine.EvaluateRule(typeof(EmailAddressRequired), Account);
                    RuleEngine.EvaluateRule(typeof(EmailAddressPreferred), Account);
                }
            }
        }

        public void UpdateView()
        {
            if (IsFeatureEnabled())
            {
                PatientPortalOptInView.ShowMe();
                UpdateOptInSelectionOnTheView();
            }
            else
            {
                PatientPortalOptInView.HideMe();
            }
        }


        public void EnableMe()
        {
            PatientPortalOptInView.EnableMe();
        }
        public void DisableMe()
        {
            PatientPortalOptInView.DisableMe();
        }
        public bool IsFeatureEnabled()
        {
            return patientPortalOptInFeatureManager.ShouldFeatureBeEnabled(Account);
        }
        
        public void UpdateOptInSelectionOnTheView(string portalOptIn)
        { 
            switch ( portalOptIn )
            {
                case YesNoFlag.CODE_YES:
                    PatientPortalOptInView.OptIn();
                    Account.PatientPortalOptIn.SetYes();
                break;
                case YesNoFlag.CODE_NO:
                    PatientPortalOptInView.OptOut();
                    Account.PatientPortalOptIn.SetNo();
                break;
                case YesNoFlag.CODE_UNABLE:
                    PatientPortalOptInView.Unable();
                    Account.PatientPortalOptIn.SetUnable();
                    break;
                default:
                    PatientPortalOptInView.UnSelected();
                    Account.PatientPortalOptIn.SetBlank();
                break;
            }
        }
        private void UpdateOptInSelectionOnTheView()
        {
            switch (Account.PatientPortalOptIn.Code)
            {
                case YesNoFlag.CODE_YES:
                    PatientPortalOptInView.OptIn();
                    break;

                case YesNoFlag.CODE_NO:
                    PatientPortalOptInView.OptOut();
                    break;

                case YesNoFlag.CODE_UNABLE:
                    PatientPortalOptInView.Unable();
                    break;

                default:
                    PatientPortalOptInView.UnSelected();
                    break;
            }
        }
        
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }
        private RuleEngine RuleEngine { get; set; }
       
        private readonly IPatientPortalOptInFeatureManager patientPortalOptInFeatureManager =
        new PatientPortalOptInFeatureManager();
        
    }
}
