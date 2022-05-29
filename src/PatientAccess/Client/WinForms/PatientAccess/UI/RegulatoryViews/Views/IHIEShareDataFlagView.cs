using System.Collections.Generic;
using PatientAccess.Domain;
namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IHIEShareDataFlagView
    {
        void ShowShareDataWithPublicHIE();
        void HideShareDataWithPublicHIE();
        YesNoFlag ShareDataWithPublicHIE { set; get; }
        YesNoFlag ShareDataWithPCP { set; get; }
        void PopulateShareDataWithPublicHIE(IEnumerable<YesNoFlag> shareDataWithPublicHieFlags);
        void HideShareDataWithPCP();
        void ShowShareDataWithPCP();
        void PopulateShareDataWithPCP(IEnumerable<YesNoFlag> shareDataWithPCPFlags);
        void DisableShareDatawithPublicHIE();
        void EnableShareDataWithPublicHIE();
        bool IsShareDataPublicHIEEnabled { get; }
        void SetShareDataWithPublicHIEToNormalColor();
        void SetNotifyPCPDataRequiredBgColor();
        void SetNotifyPCPToNormalColor();
        bool ShowHIEMessage { get; }

        void SetShareDataWithPublicHIEAsRequired();
    }
}