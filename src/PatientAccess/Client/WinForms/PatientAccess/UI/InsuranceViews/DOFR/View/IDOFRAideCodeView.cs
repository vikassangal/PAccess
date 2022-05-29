using PatientAccess.UI.InsuranceViews.DOFR.Presenter;
using System.Collections;

namespace PatientAccess.UI.InsuranceViews.DOFR.View
{
    public interface IDOFRAidCodeView
    {
        DOFRAidCodePresenter DOFRAidCodePresenter { get; set; }
        bool ShowMe { set; }
        bool rbExpansionChecked { get; set; }
        bool rbNonExpansionChecked { get; set; }
        void LoadAidCodes(ArrayList aidCodes);
        void ClearAidCodes();
        void SetNormalBgColor();
        void MakeRequiredBgColor();
        string SetSelectedAidCode { set; }
    }
}