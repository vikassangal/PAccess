using System;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient
{
    public interface IEmergencyPatientToOutPatientStep1View
    {
        string AccountNumber { set; }
        string AdmitDate { set; }
        string AdmitSource { set; }
        string AdmitTime { set; }
        string ChiefComplaint { set; }
        string FinancialClass { set; }
        string HospitalService { set; }
        string LocationLabel { set; }
        LocationView LocationView1 { get; set; }
        string OriginalChiefComplaint { set; }
        PatientContextView PatientContextView1 { get; set; }
        string PatientName { set; }
        string PrimaryPlan { set; }
        string PatientType { set; }
        string TransferToPatientType { set; }
        string TransferDate { get; set; }
        string TransferTime { get; set; }
        string UserContextView { get; set; }
        AdmitSource AdmitSourcesSelectedItem { set; }
        int AdmitSourcesSelectedIndex { set; }
        HospitalClinic HospitalClinicSelectedItem { set; }
        int HospitalClinicsSelectedIndex { set; }
        HospitalService HospitalServiceSelectedItem { set; }
        int HospitalServiceSelectedIndex { set; }
        void DisplayInfoMessage(string message);
        void EnableNextButton(bool enable);
        void DisableControls();
        bool IsTransferDateValid();
        DateTime GetTransferDateTime();

        void SetAdmitSourceRequired();
        void AddAdmitSources(AdmitSource admitSource);
        void AddClinicCode(HospitalClinic clinicCode);
        void AddHospitalService(HospitalService hospitalService);
        void SetAlternateCareFacilityRequired();
        void SetHospitalServiceListSorted();
        void ClearAdmitSourcesComboBox();
        void ClearClinicCodesComboBox();
        void ClearHospitalServicesComboBox();
        void SetClinicOneRequired();
        void SetHospitalServiceRequired();
        void SetChiefComplaintRequired();
        void SetLocationRequired();
        void SetTransferDateRequired();
        void SetTransferTimeRequired();
        void DisplayTransferDateIsFutureDateMessage();
        void DisplayTransferTimeIsFutureTimeMessage(EventArgs e);
        void DisplayTransferDateTimeBeforeAdmitDateTimeMessage(EventArgs e);
        void SetEmailAddressRequired();
    }
}