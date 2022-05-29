using PatientAccess.Domain;
namespace PatientAccess.UI.InsuranceViews.Views
{
    public interface IMBIView
    {
        string MBINumber { get; set; }
        void SetMBINumberError();
        void setFocusToMBINumber();
        void EnbleMBINumber();
        void DisableMBINumber();
        Account Account { get; set; }
        void ResetMBINumber();
        void CheckForRequiredFields();
    }
}