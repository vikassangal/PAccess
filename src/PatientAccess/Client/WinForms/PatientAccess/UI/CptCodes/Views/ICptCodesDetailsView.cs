using System.Collections.Generic;
using PatientAccess.UI.CptCodes.Presenters;

namespace PatientAccess.UI.CptCodes.Views
{
    public interface ICptCodesDetailsView
    {
        Dictionary<int, string> CptCodes { get; }
        CptCodesDetailsPresenter Presenter { set; }
        
        void CloseView();
        void SetNormalColor(CptFields cptField);
        void SetErrorColor(CptFields cptField);
        void SetFocus(CptFields cptField);
        void ShowAsDialog();
        void ClearCptCodes();
        void SetCptCodes(string[] textCptCodes);
    }
}