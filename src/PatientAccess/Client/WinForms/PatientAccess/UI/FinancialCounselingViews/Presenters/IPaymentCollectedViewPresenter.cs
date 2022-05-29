
namespace PatientAccess.UI.FinancialCounselingViews.Presenters
{
    public interface IPaymentCollectedViewPresenter
    {
        void UpdateMonthlyDueDate();
        void SetMonthlyDueDate();
        void EvaluateMonthlyDueDate();
        bool IsValidMonthlyDueDate { get; }
    }
}