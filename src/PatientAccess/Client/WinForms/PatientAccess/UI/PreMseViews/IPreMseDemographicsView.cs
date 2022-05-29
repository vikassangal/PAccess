using PatientAccess.Domain;

namespace PatientAccess.UI.PreMSEViews
{
    public interface IPreMseDemographicsView
    {
        void PopulateOtherLanguage();
        void ClearOtherLanguage();
        bool OtherLanguageVisibleAndEnabled { set; }
        Account ModelAccount { get; }
        void MakeOtherLanguageRequired();
    }
}