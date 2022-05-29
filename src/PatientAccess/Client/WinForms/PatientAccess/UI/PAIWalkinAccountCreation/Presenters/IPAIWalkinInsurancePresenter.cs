using PatientAccess.Domain;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Presenters
{
    public interface IPAIWalkinInsurancePresenter
    {
        void UpdateView();
        void ResetView();
        void ResetCoverage();
        void Edit();
        void FindAPlanViewPlanSelected( SelectInsuranceArgs args );
    }
}
