using System;
using PatientAccess.Domain;

namespace PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient
{
    public interface IOutPatientToEmergencyPatientStep1View
    {
        string AccountNumber { set; }
        string AdmitDate { set; }
        string AdmitSource { get; set; }
        string AdmitTime { set; }
        string ChiefComplaint { set; }
        string OriginalChiefComplaint { set; }
        string HospitalService { set; }

        PatientContextView PatientContextView1 { get; set; }
        string PatientName { get;  set; }

        string PatientType { set; }
        string LocationLabel { set; }
        string TransferToPatientType { set; }
        string TransferDate { get; set; }
        string TransferTime { get; set; }
        string UserContextView { get; set; }
        AdmitSource AdmitSourcesSelectedItem { get; set; }
        HospitalService HospitalServiceSelectedItem { get; set; }
        int AdmitSourcesSelectedIndex { set; }
        int HospitalServicesSelectedIndex { set;}
        void DisplayInfoMessage(string message);
        void EnableNextButton(bool enable);
        void DisableControls();

        DateTime GetTransferDateTime();
        bool IsTransferDateValid();
        void SetAdmitSourceRequired();
        void AddAdmitSources(AdmitSource admitSource);
        void AddHospitalService(HospitalService hospitalService);
        void SetHospitalServiceListSorted();
        void ClearAdmitSourcesComboBox();
        void ClearHospitalServiceComboBox();
        void SetHospitalServiceRequired();
        void SetChiefComplaintRequired();

        void SetTransferDateRequired();
        void SetTransferTimeRequired();
        void DisplayTransferDateIsFutureDateMessage();
        void DisplayTransferTimeIsFutureTimeMessage(EventArgs e);
        void DisplayTransferDateTimeBeforeAdmitDateTimeMessage(EventArgs e);
    }
}