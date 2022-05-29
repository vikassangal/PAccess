using PatientAccess.UI.CptCodes.Presenters;

namespace PatientAccess.UI.CptCodes.Views
{
    public interface ICptCodesView
    {
        CptCodesPresenter CptCodesPresenter { set; }
        
        void CheckYes();
        void CheckNo();
        void UnSelected();
        void ShowMe();
        void HideMe();
        void DisableViewButton();
        void EnableViewButton();
    }
}