namespace PatientAccess.UI.DemographicsViews
{

    public interface IDemographicsEmploymentView
    {
        void PopulateOtherLanguage();
        void ClearOtherLanguage();
        bool OtherLanguageVisibleAndEnabled { set; }
//        Account Model_Account { get; set; }
        void MakeOtherLanguageRequired();
    }
}