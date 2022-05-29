using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PAIWalkinAccountCreation.Presenters
{
    public interface IPAIWalkinAccountCreationPresenter
    {
        void PopulateGenderControl( GenderControl genderControl );
        void UpdateGender( Gender gender );
        void UpdateAppointment( ScheduleCode scheduleCode );
        void UpdateFirstName( string firstName );
        void UpdateLastName( string lastName );
        void UpdateMiddleInitial( string middleInitial );
        bool VerifyAdmitDate();
        void RunAdmitDateValidationRules();
        bool UpdateAdmitTime(); 
        bool UpdateDateOfBirth();
        void UpdateAdmitDate();
        void UpdateProcedureField( string procedureText );
        void UpdateChiefComplaintField( string complaintText );
        void UpdateView();
    }
}
