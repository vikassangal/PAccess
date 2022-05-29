using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.UI.DiagnosisViews
{
    public interface IEDLogView 
    {
        void ClearFields();
        void DoNotShow();
        void ShowDisabled();
        void ShowEnabled();
        bool HasData();
        void MakeModeOfArrivalRequired();
        void SetModeOfArrival(ModeOfArrival modeOfArrival);
        void PopulateArrivalModes(ArrayList arrivalModes);
        void PopulateReferralTypes(ArrayList referralTypes);
        void SetReferralType(ReferralType referralType);
        void SetReferralFacility(ReferralFacility referralFacility);
        void PopulateReferralFacilities(ICollection referralFacilities);
        void SetReadmitCode(ReAdmitCode admitCode);
        void PopulateReadmitCode(ICollection readmitCodes);
        void SetArrivalTime(string displayedTimeFormat);
        void ClearArrivalTimeMask();
        void ShowEnabledForUCCPostMse();
    }
}