using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;
using System;
using PatientAccess.UI.RegulatoryViews.Views;


namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class HospitalCommunicationOptInPresenter
    {
        public HospitalCommunicationOptInPresenter(IHospitalCommunicationOptInView view,
               IMessageBoxAdapter messageBoxAdapter, Account account,
               HospitalCommunicationOptInFeatureManager featureManager, RuleEngine ruleEngine)
        {
            Guard.ThrowIfArgumentIsNull(view, "HospitalCommunicationOptInView");
            Guard.ThrowIfArgumentIsNull(account, "Account");
            Guard.ThrowIfArgumentIsNull(hospitalCommunicationOptInFeatureManager,
                "HospitalCommunicationOptInFeatureManager");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "RuleEngine");

            View = view;
            Account = account;
            hospitalCommunicationOptInFeatureManager = featureManager;
            MessageBoxAdapter = messageBoxAdapter;
            RuleEngine = ruleEngine;
            View.Presenter = this;
        }

        public Account Account { get; private set; }
        private RuleEngine RuleEngine { get; set; }
        private IMessageBoxAdapter MessageBoxAdapter { get; set; }
        private readonly IHospitalCommunicationOptInFeatureManager hospitalCommunicationOptInFeatureManager =
            new HospitalCommunicationOptInFeatureManager();
        public void ValidateEmailAddress()
        {
            if (Account != null && Account.Patient != null)
            {
                var mailingContactPoint =
                    Account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());

                if (mailingContactPoint == null || mailingContactPoint.EmailAddress == null ||
                    String.IsNullOrEmpty(mailingContactPoint.EmailAddress.ToString()))
                {
                    if (IsFeatureEnabled && Account.Patient.HospitalCommunicationOptIn.IsYes)
                    {
                        MessageBoxAdapter.ShowMessageBox(UIErrorMessages.PATIENT_HOSPCOMM_OPTIN_EMAIL_REQUIRED_MSG,
                            UIErrorMessages.WARNING,
                            MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                            MessageBoxAdapterDefaultButton.Button1);
                    }
                }
            }
        }
        public bool IsFeatureEnabled
        {
            get { return hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(Account); }
        }
        public void OptIn()
        {
            Account.Patient.HospitalCommunicationOptIn.SetYes();
            EvaluateEmailAddress();
        }
        public void OptOut()
        {
            Account.Patient.HospitalCommunicationOptIn.SetNo();
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
        public void EnableMe()
        {
            View.EnableMe();
        }
        public void DisableMe()
        {
            View.DisableMe();
        }
        public void UpdateView()
        {
            if (hospitalCommunicationOptInFeatureManager.ShouldFeatureBeEnabled(Account))
            {
                View.ShowMe();
                UpdateOptInSelectionOnTheView();
            }
            else
            {
                View.HideMe();
            }
        }
        public void UpdateOptInSelectionOnTheView(string hcOptIn)
        {
            switch (hcOptIn)
            {
                case YesNoFlag.CODE_YES:
                    View.OptIn();
                    Account.Patient.HospitalCommunicationOptIn.SetYes();
                    break;
                case YesNoFlag.CODE_NO:
                    View.OptOut();
                    Account.Patient.HospitalCommunicationOptIn.SetNo();
                    break;
                default:
                    View.UnSelected();
                    Account.Patient.HospitalCommunicationOptIn.SetBlank();
                    break;
            }
           
        }
        private void UpdateOptInSelectionOnTheView()
        {
            switch (Account.Patient.HospitalCommunicationOptIn.Code)
            {
                case YesNoFlag.CODE_YES:
                    View.OptIn();
                    break;

                case YesNoFlag.CODE_NO:
                    View.OptOut();
                    break;

                default:
                    View.UnSelected();
                    break;
            }
        }

        #region Data Elements
        public IHospitalCommunicationOptInView View { get; private set; }
        
        #endregion
    }
}
