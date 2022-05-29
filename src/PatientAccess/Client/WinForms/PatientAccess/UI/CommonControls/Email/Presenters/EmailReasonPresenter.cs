using System;
using System.Collections.Generic;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls.Email.Views;
using PatientAccess.Utilities;


namespace PatientAccess.UI.CommonControls.Email.Presenters
{
    public class EmailReasonPresenter
    {
       
        #region Construction and Finalization

        public EmailReasonPresenter(IEmailReasonView emailReasonView, Account account, IEmailReasonBroker emailReasonBroker)
        {
            Guard.ThrowIfArgumentIsNull(emailReasonView, "emailReasonView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(emailReasonBroker, "emailReasonBroker");

            View = emailReasonView;
            Account = account;
            EmailReasonBroker = emailReasonBroker;
        }

        #endregion

        #region Public Methods
        public void PopulateEmailReason()
        {
            View.ClearEmailReasonSelectionValues();
            LoadEmailReasonValues();
            View.PopulateEmailReasonSelections(EmailReasonsValues);
        }

        public void UpdateEmailReason()
        {
            // Update Account. Mailing Contact point . EmailReason 
            Account.Patient.EmailReason = View.EmailReason;
            ValidateEmailReason();
        }

        private void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            View.SetEmailAddressAsRequired();
        }
        private void EmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            View.SetEmailAddressAsPreferred();
        }
        private void EmailReasonRequiredEventhandler(object sender, EventArgs e)
        {
            View.SetEmailReasonToRequired();
        }

        private void EmailReasonPreferredEventHandler(object sender, EventArgs e)
        {
            View.SetEmailReasonToPreferred();
        }
        public void ValidateEmailReason()
        {
            View.SetEmailReasonNormal();
            View.SetEmailAddressAsNormal();
            RuleEngine.GetInstance().OneShotRuleEvaluation<EmailReasonRequired>(Account, EmailReasonRequiredEventhandler);
            RuleEngine.GetInstance().OneShotRuleEvaluation<EmailReasonPreferred>(Account, EmailReasonPreferredEventHandler);
            RuleEngine.GetInstance().OneShotRuleEvaluation<EmailAddressRequired>(Account, EmailAddressRequiredEventHandler);
            RuleEngine.GetInstance().OneShotRuleEvaluation<EmailAddressPreferred>(Account, EmailAddressPreferredEventhandler); 

        }

        #endregion

        #region Private Methods

        private void LoadEmailReasonValues()
        {
            EmailReasonsValues = EmailReasonBroker.AllEmailReasons();
        }


        #endregion

        #region Data Elements

        private IEmailReasonView View { get; set; }
        private Account Account { get; set; }
        private ICollection<EmailReason> EmailReasonsValues { get; set; }
        private IEmailReasonBroker EmailReasonBroker { get; set; }

        #endregion
    }
}