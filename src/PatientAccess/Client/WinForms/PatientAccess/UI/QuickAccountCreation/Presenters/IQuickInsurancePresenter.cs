using PatientAccess.Domain;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    public interface IQuickInsurancePresenter
    {
        #region Operations

        void UpdateView();
        void ResetView();
        void ResetCoverage(); 
        void Edit();
        void FindAPlanViewPlanSelected( SelectInsuranceArgs args );

        #endregion Operations
    }
}
