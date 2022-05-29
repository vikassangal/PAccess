using PatientAccess.UI.RegulatoryViews.Presenters;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IPatientPortalOptInView
    {
        void OptIn();
        void OptOut();
        void UnSelected();
        void ShowMe();
        void HideMe();
        void EnableMe();
        void DisableMe();
        void Unable();
        PatientPortalOptInPresenter PatientPortalOptInPresenter { get; set; }
     }
}