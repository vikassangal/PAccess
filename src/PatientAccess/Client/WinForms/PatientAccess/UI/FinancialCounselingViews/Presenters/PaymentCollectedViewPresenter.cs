using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.FinancialCounselingViews.PaymentViews;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.FinancialCounselingViews.Presenters
{
    /// <summary>
    /// 
    /// </summary>
    public class PaymentCollectedViewPresenter : IPaymentCollectedViewPresenter
    {
        public PaymentCollectedViewPresenter(IPaymentCollectedView view, MessageBoxAdapter messageBoxAdapter)
        {
            View = view;
            MonthlyDueDateFeatureManager = new MonthlyDueDateFeatureManager();
            RuleEngine = RuleEngine.GetInstance();
            MessageBox = messageBoxAdapter;
        }
        
        #region IPatientCollectedViewPresenter Members

        public void SetMonthlyDueDate()
        {
            ModelAccount.DayOfMonthPaymentDue = View.MonthlyDueDate;
            EvaluateMonthlyDueDate();
        }

        public void UpdateMonthlyDueDate()
        {
            if (ModelAccount != null && ModelAccount.Facility != null &&
                MonthlyDueDateFeatureManager.IsMonthlyDueDateEnabled(ModelAccount.Facility))
            {
                View.ShowMonthlyDueDate();
                View.PopulateMonthlyDueDate();
            }
            else
            {
                View.DoNotShowMonthlyDueDate();
            }

            EvaluateMonthlyDueDate();
        }

        #endregion
        #region private Members
        public bool IsValidMonthlyDueDate
        {
            get
            {
                if (!String.IsNullOrEmpty(View.MonthlyDueDate) && (Convert.ToInt32(View.MonthlyDueDate) > 28))
                {
                    View.SetErrorBgColorForMonthlyDueDate();
                    MessageBox.ShowMessageBox(UIErrorMessages.INVALID_MONTHLY_DUE_DATE,
                        UIErrorMessages.ERROR,
                        MessageBoxAdapterButtons.OK, MessageBoxAdapterIcon.Exclamation,
                        MessageBoxAdapterDefaultButton.Button1);
                    View.SetFocusToMonthlyDueDate();
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public void EvaluateMonthlyDueDate()
        {
            View.MakeMonthlyDueDateNormal();
            RuleEngine.OneShotRuleEvaluation<MonthlyDueDateRequired>(ModelAccount, MonthlyDueDateRequiredEvent);
        }

        private void MonthlyDueDateRequiredEvent(object sender, EventArgs e)
        {
            View.MakeMonthlyDueDateRequired();
        }

        #endregion private Members
        #region Properties
        private IPaymentCollectedView View { get; set; }
        private RuleEngine RuleEngine { get; set; }
        private Account ModelAccount
        {
            get
            {
                return View.Model;
            }
        }

        private IMonthlyDueDateFeatureManager MonthlyDueDateFeatureManager { get; set; }
        private IMessageBoxAdapter MessageBox { get; set; }
        #endregion Properties
    }
}