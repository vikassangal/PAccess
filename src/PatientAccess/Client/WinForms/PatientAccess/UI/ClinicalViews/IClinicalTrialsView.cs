using PatientAccess.Domain;

namespace PatientAccess.UI.ClinicalViews
{
    public interface IClinicalTrialsView
    {
        void PopulateClinicalResearchField();
        void ShowClinicalResearchFieldsAsVisible( bool show );
        void ShowClinicalResearchFieldDisabled();
        void ShowClinicalResearchFieldEnabled();
        YesNoFlag IsPatientInClinicalResearchStudy { get; set; }
        bool ViewDetailsCommandVisible { get; set; }
        bool ViewDetailsCommandEnabled { get; set; }
        bool GetConfirmationForDiscardingPatientStudies();
    }
}