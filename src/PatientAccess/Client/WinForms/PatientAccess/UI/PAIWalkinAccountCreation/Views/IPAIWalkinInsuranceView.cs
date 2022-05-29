using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Views
{
    public interface IPAIWalkinInsuranceView
    {
        Account PatientAccount { set; }
        Coverage ModelCoverage { get; set; }
        void PopulatePlan( string planID, string planName );
        void EnableEditButton();
        void DisableEditButton();
        void ResetFindPlanView();
        InsuranceDetails InsuranceDetailsDialog { get; set; }
        void FireCoverageUpdatedEvent();
        void PlanSelected( Coverage coverage );
    }
}
