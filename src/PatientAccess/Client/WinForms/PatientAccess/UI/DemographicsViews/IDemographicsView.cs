using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{
    public interface IDemographicsView
    {
        Account ModelAccount { get; set; }
        DateTime PreRegAdmitDate { get; set; }
        bool LoadingModelData { get; set; }
        DateTime GetPreopDateFromUI();
        DateTime GetDateAndTimeFrom( string dateText, string timeText );
        void SetPreOpDateFocus();
        void SetPreOpDateErrBgColor();
        void SetPreOpDateNormalBgColor();
        void EnablePreOpDate( bool blnEnable );
        void SetBlankPreOpDate();
        void SetPreopDateFromModel();
        void ShowPreOpDateIncompleteErrorMessage();
        void ShowPreOpDateInvalidErrorMessage();
        void ShowPreOpDateAfterAdmitDateErrorMessage();
        void AutoSetPreOpDateWithAdmitDate();
        DateTime GetAdmitDateFromUI();
        string GetPreOpDateUnmaskedText();
        void RegisterPreOpDateEvent();
    }
}
