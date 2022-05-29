using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.ShortRegistration.DemographicsViews
{
    public interface IShortDemographicsView
    {
        Account ModelAccount { get; set; }
        DateTime PreRegAdmitDate { get; set; }
        bool LoadingModelData { get; set; }
        DateTime GetDateAndTimeFrom( string dateText, string timeText );
        DateTime GetAdmitDateFromUI();

        void PopulateOtherLanguage();
        void ClearOtherLanguage();
        bool OtherLanguageVisibleAndEnabled { set; }
        void MakeOtherLanguageRequired();

    }
}
