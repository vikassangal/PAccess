using PatientAccess.Domain;

namespace PatientAccess.UI.FinancialCounselingViews.PaymentViews
{
    public interface IPaymentCollectedView
    {
        Account Model { get; set; }
        void PopulateMonthlyDueDate();
        void ShowMonthlyDueDate();
        void DoNotShowMonthlyDueDate();
        void MakeMonthlyDueDateRequired();
        void MakeMonthlyDueDateNormal();
        string MonthlyDueDate { get; }
        void SetErrorBgColorForMonthlyDueDate();
        void SetFocusToMonthlyDueDate();
    }
}
