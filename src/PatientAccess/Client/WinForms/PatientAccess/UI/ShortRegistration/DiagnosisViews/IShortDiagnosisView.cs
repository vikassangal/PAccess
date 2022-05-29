using PatientAccess.Domain;

namespace PatientAccess.UI.ShortRegistration.DiagnosisViews
{

    public interface IShortDiagnosisView
    {
        void PopulateProcedure();
        void ClearProcedureField();
        void ShowProcedureDisabled();
        void ShowProcedureEnabled();
        bool HasProcedureData();
        Account Model { get; set; }
    }
}