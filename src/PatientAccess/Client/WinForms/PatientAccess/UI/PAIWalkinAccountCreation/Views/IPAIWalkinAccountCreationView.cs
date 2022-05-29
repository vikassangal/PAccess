using System.Collections;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Views
{
    public interface IPAIWalkinAccountCreationView
    {
        Account ModelAccount { get; }
        string AdmitDate { get; set; }
        string AdmitDateText { get; }
        string AdmitTime { get; set; }
        string Age { set; get; }
        string DateOfBirth { get; set; }
        string DateOfBirthText { get; }
        ScheduleCode ScheduleCode { set; }
        void PopulateProcedure();
        void PopulateChiefComplaint();
        void PopulateScheduleCodeComboBox( ArrayList scheduleCodes );
        void PopulateGender();
        void PopulateFirstName();
        void PopulateLastName();
        void PopulateMiddleInitial();
        void SetNormalColorForDateOfBirth();
        void SetAdmitDateError();
        void SetAdmitTimeError();
        void SetDateOfBirthErrBgColor();
        void SetFocusToAdmitTime();
        void DisplayMessageForMedicareAdvise();
        SSNControl SSNView { get; }
    }
}
