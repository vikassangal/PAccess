using PatientAccess.BrokerInterfaces;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// This interface will be used and fully populated when all non UI related logic is moved from the <see cref="PatientsAccountsView"/> to the 
    /// <see cref="PatientAccountsViewPresenter"/>. The inheritance from <see cref="IPatientsAccountsPresenter"/> will be removed when the view refactoring is completed. 
    /// </summary>
    public interface IEMPISearchFieldsDialog
    {
        PatientSearchCriteria SearchCriteria { get; set; }
        string SSNText { get; }
        PhoneNumberControl PhoneNumberControl { get; }
        string YearText { get; }
        string MonthText { get; set; }
        string DayText { get; set; }

        void SetSSNNormalColor();
        void SetSSNPreferredColor();
        void SetSSNErrorColor();
        void SetFocusToSSN();

        void SetNormalColorToYear();
        void SetNormalColorToDay();
        void SetNormalColorToMonth();

        void SetPreferredColorToMonth();
        void SetPreferredColorToDay();
        void SetPreferredColorToYear();

        void SetErrorColorToYear();
        void SetErrorColorToMonth();
        void SetErrorColorToDay();
        void SetDobErrBgColor();
        void SetDobPreferredColor();
        void SetDOBNormalColor();
        void SetFocusToYear();
        void SetFocusToMonth();
        void SetFocusToDay();



    }
}
