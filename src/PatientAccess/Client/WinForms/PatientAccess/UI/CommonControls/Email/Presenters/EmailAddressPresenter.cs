using System;
using System.Collections.ObjectModel;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls.Email.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.CommonControls.Email.Presenters
{
    public class EmailAddressPresenter
    {

        #region Construction and Finalization

        public EmailAddressPresenter(IEmailAddressView addressView, Account account, IRuleEngine ruleEngine)
        {
            Guard.ThrowIfArgumentIsNull(addressView, "addressView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "ruleEngine");

            AddressView = addressView;
            Account = account;
            EmailAddressFeatureManager = new EmailAddressFeatureManager();
            RuleEngine = ruleEngine;
        }

        #endregion
        
        #region Public Methods

        private void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            AddressView.SetEmailAddressAsRequired();
        }

        private void EmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            AddressView.SetEmailAddressAsPreferred();
        }
 
        private void GuarantorEmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            AddressView.SetGuarantorEmailAddressAsPreferred();
        }

        public void HandleEmailAddress()
        {
            var makeEmailAddressVisible = EmailAddressFeatureManager.ShouldFeatureBeEnabled(Account);

            if (!makeEmailAddressVisible)
            {
                AddressView.DoNotShowEmailAddress();
            }
        }

        public void RunPatientEmailAddressRules()
        {
            AddressView.SetEmailAddressToNormalColor();
            RuleEngine.OneShotRuleEvaluation<EmailAddressRequired>(Account, EmailAddressRequiredEventHandler);
            RuleEngine.OneShotRuleEvaluation<EmailAddressPreferred>(Account, EmailAddressPreferredEventhandler);
        }

        public void RunGurantorEmailAddressRules()
        {
            AddressView.SetEmailAddressToNormalColor();
            RuleEngine.OneShotRuleEvaluation<GuarantorEmailAddressPreferred>(Account, GuarantorEmailAddressPreferredEventhandler);
        }

        public void UpdatePatientEmailAddress()
        {
            ContactPoint mailingContactPoint =
                Account.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            mailingContactPoint.EmailAddress = AddressView.Model_ContactPoint.EmailAddress;
            RunPatientEmailAddressRules();
        }

        public void UpdateGuarantorEmailAddress()
        {
            ContactPoint mailingContactPoint = Account.Guarantor.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType());
            mailingContactPoint.EmailAddress = AddressView.Model_ContactPoint.EmailAddress;
            RunGurantorEmailAddressRules();
        }

        public static bool IsGenericEmailAddress(String emailAddress)
        {
           return GenericEmailAddresses.Contains(emailAddress);
        }
        #endregion

        #region Data Elements

        public IEmailAddressView AddressView { get; private set; }

        private Account Account { get; set; }

        private IEmailAddressFeatureManager EmailAddressFeatureManager { get; set; }

        private IRuleEngine RuleEngine { get; set; }

        private static readonly Collection<String> GenericEmailAddresses = new Collection<String>
        {
            "NONE@NONE.COM"
        };

        #endregion
    }
}