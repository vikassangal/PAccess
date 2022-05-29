using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.InsuranceViews.DOFR.View
{
    public interface IDOFRInsuracePartIPAView
    {
        DOFRInsuracePartIPAViewPresenter DOFRInsuracePartIPAViewPresenter { get; set; }
        bool ShowMe { set; }
        bool rbYesChecked { get; set; }
        bool rbNoChecked { get; set; }
        bool rbYesEnabled { get; set; }
        bool rbNoEnabled { get; set; }
        void SetNormalBgColor();
        void MakeRequiredBgColor();
    }
}