using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.InsuranceViews.DOFR.View
{
    public interface IDOFRInitiateView
    {
        DOFRInitiatePresenter DOFRInitiatePresenter { get; set; }
        bool EnableInitiateButton { set; }
        bool ShowMe { set; }
    }
}