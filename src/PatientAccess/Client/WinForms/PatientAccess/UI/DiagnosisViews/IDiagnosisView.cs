using PatientAccess.Domain;

namespace PatientAccess.UI.DiagnosisViews
{

    public interface IDiagnosisView
    {
        void PopulateProcedure();
        void ClearProcedureField();
        void ShowProcedureDisabled();
        void ShowProcedureEnabled();
        bool HasProcedureData();
        Account Model { get; set; }
    }
}