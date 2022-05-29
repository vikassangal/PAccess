 
namespace PatientAccess.UI.RegulatoryViews.Views
{ 
    public interface IRegulatoryView
    {
        void EnableRightToRestrict();
        void DisableRightToRestrict();
        void SetRightToRestrict();
        void UnSetRightToRestrict();
        void AutoPopulateShareDataWithPublicHIEForRightToRestrict(bool rightToRestrictChecked);
        void SetCOBReceivedLocationForPreregistrationAccount();
        void SetCOBReceivedLocationForRegistrationAccount();
        void SetIMFMReceivedLocationForPreregistrationAccount();
        void SetIMFMReceivedLocationForRegistrationAccount();
        void HospCommOptIn();
        void HospCommOptOut();
        void EnableHospComm();
        void DisableHospComm();
        void PatientPortalOptIn();
        void PatientPortalOptOut();
        void EnablePatientPortal();
        void DisablePatientPortal();
        void UpdateHospitalCommunicationView();
        void UpdatePatientPortalView();
        void SetEmailReasonToPatientDeclined();
        void DisableEmailReason();
        void EnableEmailReason();
        void DisableEmail();
        void EnableEmail();
        void SetEmailAddressAsNormal();
        void SetEmailReasonAsNormal();
        void PopulatePatientPortalOptInValue();
        void PopulateHospCommunicationOptInValue();
        void UnSelectAuthorizePatientPortalUser();
        void EnableAuthorizeAdditionalPortalUser();
        void DisableAuthorizeAdditionalPortalUser();
        void SetHIEShareDataAndPCPFlagLocation();
    }
}