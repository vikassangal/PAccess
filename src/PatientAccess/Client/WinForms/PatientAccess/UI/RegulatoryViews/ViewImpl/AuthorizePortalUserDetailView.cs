using System;
using System.ComponentModel;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.Factories;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.HelperClasses;
using PatientAccess.Utilities;
namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class AuthorizePortalUserDetailView : ControlView, IAuthorizePortalUserDetailView
    {
        #region Constructor
        private readonly Regex emailKeyPressExpression;
        private readonly Regex emailValidationExpression;
        public IAuthorizePortalUserDetailPresenter AuthorizePortalUserDetailPresenter;

        public AuthorizePortalUserDetailView()
        {
            InitializeComponent();
            ConfigureControls();
            emailKeyPressExpression = new Regex(RegularExpressions.EmailValidCharactersExpression);
            emailValidationExpression = new Regex(RegularExpressions.EmailValidFormatExpression);
            MaskedEditTextBoxBuilder.ConfigureEmail(mtbEmailAddress);
        }

        #endregion

        public override void UpdateView()
        {
            AuthorizePortalUserDetailPresenter = new AuthorizePortalUserDetailPresenter(this, PortalUser);
            AuthorizePortalUserDetailPresenter.UpdateView();
        }
        #region Proterties

        public AuthorizedAdditionalPortalUser PortalUser { get; set; }
        public int AuthPortalUserSequenceNumber { get; set; }
         
        #endregion

        #region Private Methods
        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName(mtbFirstName);
            MaskedEditTextBoxBuilder.ConfigureFirstNameAndLastName(mtbLastName);
        }

        public void SetRequiredColorFirstName()
        {
            UIColors.SetRequiredBgColor(mtbFirstName);
        }

        public void SetNormalColorFirstName()
        {
            UIColors.SetNormalBgColor(mtbFirstName);
        }

        public void SetRequiredColorLastName()
        {
            UIColors.SetRequiredBgColor(mtbLastName);
        }
        public void SetNormalColorLastName()
        {
            UIColors.SetNormalBgColor(mtbLastName);
        }
        public void SetRequiredColorEmail()
        {
            UIColors.SetRequiredBgColor(mtbEmailAddress);
        }
        public void SetNormalColorEmail()
        {
            UIColors.SetNormalBgColor(mtbEmailAddress);
        }
        public void SetRequiredColorDob()
        {
            UIColors.SetRequiredBgColor(mtbDateOfBirth);
        }
        public void SetNormalColorDob()
        {
            UIColors.SetNormalBgColor(mtbDateOfBirth);
        }
        public void SetErrorColorDob()
        {
            UIColors.SetErrorBgColor(mtbDateOfBirth);
        }
        public void SetErrorColorEmail()
        {
            UIColors.SetErrorBgColor(mtbEmailAddress);
        }

        private void mtbFirstName_Validating(object sender, CancelEventArgs e)
        {
            AuthorizePortalUserDetailPresenter.CheckAllFieldsValuesAreEntered();
        }

        private void mtbLastName_Validating(object sender, CancelEventArgs e)
        {
            AuthorizePortalUserDetailPresenter.CheckAllFieldsValuesAreEntered();
        }

        private void mtbDateOfBirth_Validating(object sender, CancelEventArgs e)
        {

            if (!DateTimeUtilities.ValidateDateOfBirth(mtbDateOfBirth))
            {
                mtbDateOfBirth.Focus();
            }
            else
            {
                //if DOB is cleared after once entered we need below code
                if (mtbDateOfBirth.UnMaskedText.Length == 0)
                {
                    DOB = String.Empty;
                }
                
                AuthorizePortalUserDetailPresenter.CheckAllFieldsValuesAreEntered();
            }
        }

        private void mtbEmailAddress_Validating(object sender, CancelEventArgs e)
        {
            var mtb = (MaskedEditTextBox) sender;

            // check if only valid email special characters have been entered or pasted
            if (mtb.Text != string.Empty && emailKeyPressExpression.IsMatch(mtb.Text) == false)
            {
                // Prevent cursor from advancing to the next control
                e.Cancel = true;
                SetErrorColorEmail();
                MessageBox.Show(UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
                return;
            }

            // check if entered email is in the correct email format
            if (mtb.Text != string.Empty &&
                (emailValidationExpression.IsMatch(mtb.Text) == false ||
                 EmailAddressPresenter.IsGenericEmailAddress(mtb.Text))
            )
            {
                // Prevent cursor from advancing to the next control 
                e.Cancel = true;
                SetErrorColorEmail();
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, UIErrorMessages.ERROR,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
                return;
            }

            AuthorizePortalUserDetailPresenter.CheckAllFieldsValuesAreEntered();
        }
        
        #endregion


        public string FirstName
        {
            get { return mtbFirstName.Text; }
            set { mtbFirstName.Text = value; }
        }

        public string LastName
        {
            get { return mtbLastName.Text; }
            set { mtbLastName.Text = value; }
        }

        public string DOB
        {
            get { return mtbDateOfBirth.UnMaskedText; }
            set { mtbDateOfBirth.UnMaskedText = value; }
        }
        public string EmailAddress
        {
            get { return mtbEmailAddress.UnMaskedText; }
            set { mtbEmailAddress.UnMaskedText = value; }
        }
        public YesNoFlag RemoveUserFlag
        {
            get
            {
                var result = new YesNoFlag();
                if (chkRemoveUser.Checked)
                {
                    result.SetYes();
                }
                else
                {
                    result.SetNo();
                }

                return result;
            }

            set { chkRemoveUser.Checked = value.IsYes; }
        }


        public bool HasNoFirstName
        {
            get { return String.IsNullOrEmpty(FirstName); }
        }
        public bool HasNoLastName
        {
            get { return String.IsNullOrEmpty(LastName); }
        }
        public bool HasNoEmail
        {
            get { return String.IsNullOrEmpty(EmailAddress); }
        }
        public bool HasNoDob
        {
            get
            {
                return DOB == string.Empty ||
                       DOB == "01010001";
            }
        }

    }
}
