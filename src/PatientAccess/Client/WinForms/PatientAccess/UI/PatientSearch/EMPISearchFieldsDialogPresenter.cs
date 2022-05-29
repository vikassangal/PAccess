using System;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.PatientSearch
{
    /// <summary>
    /// 
    /// </summary>
    public class EMPISearchFieldsDialogPresenter
    {
     
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EMPISearchFieldsDialogPresenter"/> class.
        /// </summary>

        public EMPISearchFieldsDialogPresenter(IEMPISearchFieldsDialog view, PatientSearchCriteria searchCriteria)
        {
            PatientSearchView = view;
            SearchCriteria = searchCriteria;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the diagnosis view.
        /// </summary>
        /// <value>The diagnosis view.</value>
        private IEMPISearchFieldsDialog PatientSearchView { get; set; }
        private PatientSearchCriteria SearchCriteria { get; set; }

        #endregion Properties

        #region Public Methods

        public void UpdateSearchCriteria()
        {
            SearchCriteria.PhoneNumber.AreaCode = PatientSearchView.PhoneNumberControl.AreaCode;
            SearchCriteria.PhoneNumber.Number = PatientSearchView.PhoneNumberControl.PhoneNumber;
            SearchCriteria.SocialSecurityNumber = new SocialSecurityNumber(PatientSearchView.SSNText);
            if (!String.IsNullOrEmpty(PatientSearchView.YearText) && !String.IsNullOrEmpty(PatientSearchView.MonthText) &&
                !String.IsNullOrEmpty(PatientSearchView.DayText))
            {
                SearchCriteria.YearOfBirth = Convert.ToInt64(PatientSearchView.YearText);
                SearchCriteria.MonthOfBirth = Convert.ToInt64(PatientSearchView.MonthText);
                SearchCriteria.DayOfBirth = Convert.ToInt64(PatientSearchView.DayText);
            }
        }

        public void ShowInvalidSSNErrorMessage()
        {
            string errorMsg = PatientSearchCriteria.ERR_MSG_PARTIAL_SSN;
            PatientSearchView.SetSSNErrorColor();
            MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }
        public bool ValidateSsn()
        {
            SetSSNNormalColor();
            if (String.IsNullOrEmpty(PatientSearchView.SSNText))
            {
                SetSSNPreferredColor();
                return true;
            }
            SocialSecurityNumber ssn = new SocialSecurityNumber(PatientSearchView.SSNText);
            if (!String.IsNullOrEmpty( PatientSearchView.SSNText ) && !ssn.IsComplete)

            {
                PatientSearchView.SetSSNErrorColor(); 
                PatientSearchView.SetFocusToSSN();
                return false;
            }
            return true;
        }

        public void SetPhoneNumberPreferredColor()
        {
            if (String.IsNullOrEmpty(PatientSearchView.PhoneNumberControl.PhoneNumber))
            {
                PatientSearchView.PhoneNumberControl.SetPhoneNumberPreferredColor();
            }
            if (String.IsNullOrEmpty(PatientSearchView.PhoneNumberControl.AreaCode))
            {
                PatientSearchView.PhoneNumberControl.SetAreaCodePreferredColor();
            }
        }
        public void ValidateMonth()
        {
           PatientSearchView.SetNormalColorToMonth();

            if (!String.IsNullOrEmpty(PatientSearchView.MonthText) && PatientSearchView.MonthText != "0" &&
                PatientSearchView.MonthText.Length == 1)
            {
                PatientSearchView.MonthText = "0" + PatientSearchView.MonthText;
            }
            else if (!String.IsNullOrEmpty(PatientSearchView.MonthText) &&
                     Convert.ToInt16(PatientSearchView.MonthText) == 0)
            {
                PatientSearchView.MonthText = String.Empty;
            }
            if (String.IsNullOrEmpty(PatientSearchView.MonthText))
            {
                PatientSearchView.SetPreferredColorToMonth();
            }
        }

        public void ValidateDay()
        {
            PatientSearchView.SetNormalColorToDay();

            if (!String.IsNullOrEmpty(PatientSearchView.DayText) && PatientSearchView.DayText != "0" &&
                PatientSearchView.DayText.Length == 1)
            {
                PatientSearchView.DayText = "0" + PatientSearchView.DayText;
            }
            else if (!String.IsNullOrEmpty(PatientSearchView.DayText) && Convert.ToInt16(PatientSearchView.DayText) == 0)
            {
                PatientSearchView.DayText = String.Empty;
            }
            if (String.IsNullOrEmpty(PatientSearchView.DayText))
            {
                PatientSearchView.SetPreferredColorToDay();
            }
        }

        public void ValidateYear()
        {
            PatientSearchView.SetNormalColorToYear();

            if (!String.IsNullOrEmpty(PatientSearchView.YearText) && PatientSearchView.YearText.Length < 4)
            {
                PatientSearchView.SetErrorColorToYear();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
                PatientSearchView.SetFocusToYear();
                return;
            }
            if (String.IsNullOrEmpty(PatientSearchView.YearText))
            {
                PatientSearchView.SetPreferredColorToYear();
            }
        }
        public bool  ValidateDobEntry()
        {
            PatientSearchView.SetDOBNormalColor();
            DateTime currentDate = DateTime.Today; 
            int currentYear = currentDate.Year;

            string dobMonth = PatientSearchView.MonthText;
            string dobDay = PatientSearchView.DayText;
            string dobYear = PatientSearchView.YearText;
            var DOBLength = dobDay.Length + dobMonth.Length + dobYear.Length;
            string createDate; 

            if (dobYear.Length > 0 && dobYear.Length < 4)
            {
                PatientSearchView.SetErrorColorToYear();
                MessageBox.Show(UIErrorMessages.DOB_YEAR_DIGITS_ERRMSG,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                PatientSearchView.SetFocusToYear();
                return false;
            }
            else if (DOBLength > 0 && DOBLength < 8)
            {
                MessageBox.Show(UIErrorMessages.DOB_INCOMPLETE_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (dobMonth.Length == 0)
                {
                    PatientSearchView.SetErrorColorToMonth();
                    PatientSearchView.SetFocusToMonth();
                }
                else
                if (dobDay.Length == 0)
                {
                    PatientSearchView.SetErrorColorToDay();
                    PatientSearchView.SetFocusToDay();
                }
                else
                if (dobYear.Length == 0)
                {
                    PatientSearchView.SetErrorColorToYear();
                    PatientSearchView.SetFocusToYear();
                }
                return false;
            }

            if ((dobYear != String.Empty && Convert.ToInt16(dobYear) > currentYear) || (dobYear != string.Empty &&
                Convert.ToInt16(dobYear) == currentYear && dobMonth != String.Empty &&
                Convert.ToInt16(dobMonth) > currentDate.Month))
            {
                PatientSearchView.SetDobErrBgColor();
                MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                PatientSearchView.SetFocusToYear();
                return false;
            }

            if (dobMonth != String.Empty && dobDay != String.Empty && dobYear != String.Empty)
            {
                createDate = dobMonth + "/" + dobDay + "/" + dobYear;
                try
                {
                    DateTime dateOfBirth = Convert.ToDateTime(createDate);

                    if (dateOfBirth > DateTime.Today)
                    {
                        PatientSearchView.SetDobErrBgColor();
                        MessageBox.Show(UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                        PatientSearchView.SetFocusToYear();
                        return false;
                    }
                    else if (DateValidator.IsValidDate(dateOfBirth) == false)
                    {   // Invalid leap year will trigger this
                        PatientSearchView.SetDobErrBgColor();
                        MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1);
                        PatientSearchView.SetFocusToYear();
                        return false;
                    }
                    return true;
                }
                catch
                {   // DateTime throws ArgumentOutOfRange exception when there's an
                    // invalid year, month, or day.
                    PatientSearchView.SetDobErrBgColor();
                    MessageBox.Show(UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                     MessageBoxDefaultButton.Button1);
                    PatientSearchView.SetFocusToYear();
                    return false; 
                }
            }

            if (DOBLength == 0 )
            {
               PatientSearchView.SetDobPreferredColor(); 
            }
            return true;
        }
        #endregion Public Methods

        #region Private Methods
        private void SetSSNPreferredColor()
        {
            PatientSearchView.SetSSNPreferredColor();
        }

        private void SetSSNNormalColor()
        {
            PatientSearchView.SetSSNNormalColor();
        }
        #endregion Private Methods

    }
}