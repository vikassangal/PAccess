
namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public interface IHIEShareDataPresenter
    {
        void UpdateView();
        void UpdateShareDataWithPublicHIE();
        void UpdateShareDataWithPCP();
        void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool rightToRestrictChecked);
        void AutoPopulateShareDataWithPublicHIEForShareDatawithPCP(bool selectedShareDataWithPCP);
        void HandleNotifyPCP();
    }
}
