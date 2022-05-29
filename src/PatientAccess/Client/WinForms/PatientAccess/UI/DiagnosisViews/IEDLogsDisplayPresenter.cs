using PatientAccess.Domain;

namespace PatientAccess.UI.DiagnosisViews
{
    public interface IEDLogsDisplayPresenter
    {
        void PopulateEdLogData();
        void UpdateSelectedModeOfArrival( ModeOfArrival selectedModeOfArrival );
        void UpdateEDLogDisplay( VisitType visitType, bool isViewBeingUpdated, bool edLogFieldsHaveData );
        void RegisterEDLogRules();
        void UnRegisterEDLogRules();
    }
}