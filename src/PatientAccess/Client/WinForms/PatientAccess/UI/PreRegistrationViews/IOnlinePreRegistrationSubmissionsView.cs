using System;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.UI.WorklistViews;

namespace PatientAccess.UI.PreRegistrationViews
{
    public interface IOnlinePreRegistrationSubmissionsView
    {
        IOnlinePreRegistrationSubmissionsPresenter Presenter { get; set; }
        FilterWorklistView FilterWorklistView { get; set; }
        void ShowNoAccessView();
        void ShowProgressPanel();
        void ShowNoAccessMessage();
        void ShowDefaultCursor();
        void ShowWaitCursor();
        void ShowAppStartingCursor();
        void ShowNullFacilityMessage();
        bool ViewDisposed();
        void ShowRemotingTimeOutMesssage();
        void SetNotEnabledButtonState();

        void ShowNewPatientAndAccountButtons();
        void HideNewPatientAndAccountButtons();

        void UpdateOnlinePreRegistrationsList( IEnumerable<OnlinePreRegistrationItem> preRegistrations );
        void RefreshPreRegistrationSubmissionsList( Object state );
        void SetNewPatientButtonState();
        void EnableCreateNewPatientButton();
        void ShowCreateNewPatientButton();
        void ShowNoOnlineSubmissionsMessage();
        void ClearNoOnlineSubmissionsText();
        void HideCreateNewPatientButton();
        void DisableCreateNewPatientButton();
        void HideNoOnlineSubmissionsMessage();

        void EnableCreateNewAccountButton();
        void ShowCreateNewAccountButton();
        void ShowPatientSearchTimeOutMessage();
        void UpdateMatchingPatientsList( IEnumerable<PatientSearchResult> matchingPatients );
        void SetNewAccountButtonState();
        void ShowNoMatchingPatientsMessage();
        void HideCreateNewAccountButton();
        void DisableCreateNewAccountButton();
        void HideProgressPanel();
        void HideNoAccessPanel();
        void HideNoMatchingPatientsMessage();
        void HidePatientSearchProgressPanel();
        void ShowPatientSearchProgressPanel();
        void ShowPreRegistrationSubmissionIsLockedMessage();
        void ShowPreRegistrationSubmissionNotAvailableMessage();
        void ShowMatchingPatientsPanel();
        void ShowPreRegPanel();
        void SetDefaultButtonState();
    }
}
