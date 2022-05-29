using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.GuarantorViews.Views;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;

namespace PatientAccess.UI.GuarantorViews.Presenters
{
    public class GuarantorDateOfBirthPresenter
    {
        private DateTime dobDate;

        #region Construction and Finalization

        public GuarantorDateOfBirthPresenter(IGuarantorDateOfBirthView view, Account account, IRuleEngine ruleEngine, IMessageBoxAdapter messageBoxAdapter)
        {
            Guard.ThrowIfArgumentIsNull(view, "guarantorView");
            Guard.ThrowIfArgumentIsNull(account, "account");
            Guard.ThrowIfArgumentIsNull(ruleEngine, "ruleEngine");

            View = view;
            Account = account;
            GuarantorDateOfBirthFeatureManager = new GuarantorDateOfBirthFeatureManager();
            RuleEngine = ruleEngine;
            MessageBoxAdapter = messageBoxAdapter;
        }

        #endregion

        #region Public Methods

        private void GuarantorDateOfBirthRequiredEventHandler(object sender, EventArgs e)
        {
            View.SetRequireedColor();
        }

        private void GuarantorDateOfBirthPreferredEventhandler(object sender, EventArgs e)
        {
            View.SetPreferredColor();
        }

        public void HandleGuarantorDateOfBirth()
        {
            bool ShowGuarantorDateOfBirth = GuarantorDateOfBirthFeatureManager.
                ShouldWeEnableGuarantorDateOfBirthFeature(Account);
            
            if (ShowGuarantorDateOfBirth)
            {
                View.ShowMe();
                View.Populate(Account.Guarantor.DateOfBirth);
            }
            else
            {
                View.HideMe();
                View.Populate(DateTime.MinValue);
                dobDate = DateTime.MinValue;
                UpdateGuarantorDateOfBirth();
            }

            RunGuarantorDateOfBirthRules();
        }

        public bool ValidateDateOfBirth()
        {
            String DOBEntered = View.UnmaskedText;
            if (DOBEntered.Length == 0)
            {
                dobDate = DateTime.MinValue;
                UpdateGuarantorDateOfBirth();
                return true;
            }

            if (DOBEntered.Length != 8)
            {
                View.FocusMe();
                View.SetErrorColor();
                MessageBoxAdapter.Show(UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }

            try
            {
                string month = DOBEntered.Substring(0, 2);
                string day = DOBEntered.Substring(2, 2);
                string year = DOBEntered.Substring(4, 4);

                dobDate = new DateTime(Convert.ToInt32(year),
                                        Convert.ToInt32(month),
                                        Convert.ToInt32(day));

                if (dobDate > DateTime.Today)
                {
                    // Remove the Admit Time Leave handler so error isn't generated
                    // when user comes back to the time field to correct the error.
                    View.FocusMe();
                    View.SetErrorColor();
                    MessageBoxAdapter.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }
                if (dobDate != null &&
                    !dobDate.Equals(DateTime.MinValue) &&
                    dobDate < EARLIEST_DATEOFBIRTH)
                {
                    View.SetErrorColor();
                    MessageBoxAdapter.Show(UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    View.FocusMe();
                    return false;
                }
                if (DateValidator.IsValidDate(dobDate) == false)
                {
                    View.FocusMe();
                    View.SetErrorColor();
                    MessageBoxAdapter.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    return false;
                }
            }
            catch
            {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                // an invalid year, month, or day.  Simply set field to error color.
                View.FocusMe();
                View.SetErrorColor();
                MessageBoxAdapter.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                 MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                 MessageBoxDefaultButton.Button1);
                return false;
            }
            UpdateGuarantorDateOfBirth();
            return true;
        }
        public void RunGuarantorDateOfBirthRules()
        {
            View.SetNormalColor();
            RuleEngine.OneShotRuleEvaluation<GuarantorDateOfBirthRequired>(Account, GuarantorDateOfBirthRequiredEventHandler);
            RuleEngine.OneShotRuleEvaluation<GuarantorDateOfBirthPreferred>(Account, GuarantorDateOfBirthPreferredEventhandler);
        }

        public void UpdateGuarantorDateOfBirth()
        {
         
            Account.Guarantor.DateOfBirth = dobDate;
            RunGuarantorDateOfBirthRules();
        }
  
        #endregion

        #region Data Elements

        public IGuarantorDateOfBirthView View { get; private set; }

        private Account Account { get; set; }

        private IGuarantorDateOfBirthFeatureManager GuarantorDateOfBirthFeatureManager { get; set; }

        private IRuleEngine RuleEngine { get; set; }

        private IMessageBoxAdapter MessageBoxAdapter { get; set; }

        private readonly DateTime EARLIEST_DATEOFBIRTH = new DateTime(1800, 01, 01);
        #endregion
    }
}