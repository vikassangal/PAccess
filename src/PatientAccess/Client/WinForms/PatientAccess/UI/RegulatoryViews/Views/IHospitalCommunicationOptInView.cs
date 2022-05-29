

using PatientAccess.UI.RegulatoryViews.Presenters;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IHospitalCommunicationOptInView
    {
        void OptIn();
        void OptOut();
        void UnSelected();
        void ShowMe();
        void HideMe();
        void EnableMe();
        void DisableMe();
        HospitalCommunicationOptInPresenter Presenter { set; }
    }
}