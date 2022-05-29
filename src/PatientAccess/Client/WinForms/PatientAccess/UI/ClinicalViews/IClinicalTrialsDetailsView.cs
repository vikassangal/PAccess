using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.UI.ClinicalViews
{
    public interface IClinicalTrialsDetailsView
    {
        void ShowMe();
        ClinicalTrialsDetailsPresenter Presenter { get; set; }
        void Update( IEnumerable<ConsentedResearchStudy> patientsStudies, IEnumerable<ResearchStudy> studySelectionList );
        void CloseMe();
        bool SaveCommandEnabled { get; set; }
        bool EnrollCommandsEnabled { get; set; }
        bool RemoveCommandEnabled { get; set; }
        bool ShowExpiredStudies { get; set; }
    }
}