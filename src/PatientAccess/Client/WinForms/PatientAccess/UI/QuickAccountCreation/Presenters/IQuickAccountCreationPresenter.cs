using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    public interface IQuickAccountCreationPresenter
    {
        #region Operations

        void PopulateGenderControl( GenderControl genderControl );
        void UpdateGender( Gender gender );
        void PopulateScheduleCodeComboBox();
        void UpdateAppointment( ScheduleCode scheduleCode );
        void SetScheduleCode();
        void PopulateFirstName();
        void PopulateLastName();
        void PopulateMiddleInitial();
        void UpdateFirstName( string firstName );
        void UpdateLastName( string lastName );
        void UpdateMiddleInitial( string middleInitial );
        void SetAdmitDateOnUIFromTheModel();
        void SetAdmitTimeOnUIFromTheModel();
        void AdmitDateUnaltered();
        bool VerifyAdmitDate();
        void RunAdmitDateValidationRules();
        bool UpdateAdmitTime();
        bool PopulateDateOfBirth();
        void SetDateOfBirthAndAge();
        bool UpdateDateOfBirth();
        void UpdateAdmitDate();

        #endregion Operations
    }
}
