using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.PreRegistrationViews
{
    public interface IOnlinePreRegistrationSubmissionsPresenter
    {
        void ShowPreRegistrationSubmissionsRequestEvent( object sender, EventArgs e );
        void PrintReportEvent();
        void FindMatchingPatients(OnlinePreRegistrationItem preRegistrationItem );

        void CreateAndLoadNewPatient();
        void ResetButtonClickEvent( object sender, EventArgs e );
        void PopulatePreRegistrationSubmissions();
        void OnViewDispose();
        PatientSearchResult SelectedPatientSearchResult { get; set; }
        OnlinePreRegistrationItem SelectSubmissionItem { get; set; }
        void CreateAndLoadNewAccountForExistingPatient();
        void OnViewLeave();
    }
}
