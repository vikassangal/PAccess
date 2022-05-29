using PatientAccess.Domain;
namespace PatientAccess.UI.InsuranceViews.Views
{
    public interface IMBINumberView
    {
        string MBINumber { get; set; }
        void SetMBINumberError();
        void SetMBINumberNormalColor();
        void SetHICNumberNormalColor();
        void setFocusToMBINumber();
        void EnbleMBINumber();
        void DisableMBINumber();
        Account Account { get; set; }
    }
}