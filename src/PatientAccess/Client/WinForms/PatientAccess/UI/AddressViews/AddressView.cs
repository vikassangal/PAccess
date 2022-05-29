using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews.Presenters;
using PatientAccess.UI.AddressViews.Views;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Email.Presenters;
using PatientAccess.UI.CommonControls.Email.Views;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using PatientAccess.Utilities;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for AddressView.
    /// </summary>
    public class AddressView : ControlView, ICellPhoneConsentView, IEmailAddressView 
    {
        #region Events
        public event EventHandler AddressChanged;
        public event EventHandler AreaCodeChanged;
        public event EventHandler PhoneNumberChanged;
        public event EventHandler CellPhoneNumberChanged;
        public event EventHandler EmailChanged;
        public event EventHandler CellPhoneConsentChanged; 
        
        #endregion

        #region Event Handlers

        private void phoneNumberControl_AreaCodeChanged( object sender, EventArgs e )
        {
            if ( Model_ContactPoint != null &&
                Model_ContactPoint.PhoneNumber == null )
            {
                Model_ContactPoint.PhoneNumber = new PhoneNumber {AreaCode = phoneNumberControl.Model.AreaCode};
            }

            if ( AreaCodeChanged != null )
            {
                AreaCodeChanged( this, e );
            }
        }

        private void phoneNumberControl_PhoneNumberChanged( object sender, EventArgs e )
        {
            if ( Model_ContactPoint != null &&
                Model_ContactPoint.PhoneNumber == null )
            {
                Model_ContactPoint.PhoneNumber = new PhoneNumber {Number = phoneNumberControl.Model.Number};
            }

            if ( PhoneNumberChanged != null )
            {
                PhoneNumberChanged( this, e );
            }
        }

        private void cellPhoneNumberControl_AreaCodeChanged( object sender, EventArgs e )
        {
                Model_ContactPoint.CellPhoneNumber.AreaCode = cellPhoneNumberControl.Model.AreaCode;
                CellPhoneConsentPresenter.CellPhoneNumberChanged();
        }

        private void cellPhoneNumberControl_PhoneNumberChanged(object sender, EventArgs e)
        {
            CellPhoneConsentPresenter.CellPhoneNumberChanged();
        }

        public void CellPhoneConsentUpdated()
        {
            if (CellPhoneConsentChanged != null)
            {
                CellPhoneConsentChanged(this, null);
            }
        }
 
        public void CellPhoneNumberUpdated()
        {
            if (CellPhoneNumberChanged != null)
            {
                CellPhoneNumberChanged(this, null);
            }
        }
        

        public void AddressRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( lblAddress );
        }

        public void AddressPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( lblAddress );
        }

        public void AreaCodeRequiredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetAreaCodeRequiredColor();
        }

        public void AreaCodePreferredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetAreaCodePreferredColor();
        }

        public void PhoneRequiredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetPhoneNumberRequiredColor();
        }

        public void PhonePreferredEventHandler( object sender, EventArgs e )
        {
            phoneNumberControl.SetPhoneNumberPreferredColor();
        }

        private void AddressView_Load( object sender, EventArgs e )
        {
            HideControlsBasedOnProperties();
        }

        private void btnClear_Click( object sender, EventArgs e )
        {
            ResetView();
            NotifyContactPointChange();
        }

        private void btnEdit_Click( object sender, EventArgs e )
        {
            ArrayList addresses = new ArrayList();

            btnEdit.Message = String.Format( "Click Edit Address {0}", Model_ContactPoint.TypeOfContactPoint.Description );

            if ( IsAddressWithCounty )
            {
                FormAddressWithCountyVerification formAddressWithCountyVerification = new FormAddressWithCountyVerification();

                if ( Model_ContactPoint != null && Model_ContactPoint.Address != null )
                {
                    formAddressWithCountyVerification.Model = Model_ContactPoint.Address.Clone();
                    formAddressWithCountyVerification.CapturePhysicalAddress = this.CapturePhysicalAddress;
                    formAddressWithCountyVerification.CaptureMailingAddress = this.CaptureMailingAddress;
                    formAddressWithCountyVerification.CountyRequiredForCurrentActivity = this.CountyRequiredForCurrentActivity;
                }
                else if ( cmbCopyFrom.SelectedIndex == BLANK_CONTACTPOINT )
                {
                    formAddressWithCountyVerification.Model = null;
                }

                try
                {
                    formAddressWithCountyVerification.ShowDialog( this );

                    addresses.Add( formAddressWithCountyVerification.i_AddressSelected );
                }
                finally
                {
                    formAddressWithCountyVerification.Dispose();
                }
            }
            else
            {
                var currentUser = User.GetCurrent();
                var facility = currentUser.Facility;
                var ruleEngine = RuleEngine.GetInstance();
                if ( !isAddressWithStreet2 )
                {
                    var formAddressVerification = new FormAddressVerification(facility, ruleEngine);
                    GetFormAddressWithStreetView(formAddressVerification, addresses);
                }
                else
                {
                    var formAddressVerification = new FormAddressWithStreet2Verification(facility, ruleEngine);
                    GetFormAddressWithStreetView(formAddressVerification, addresses);
                }
                
            }

            foreach ( Address address in addresses )
            {
                if ( Model_ContactPoint != null ) Model_ContactPoint.Address = address;
                break;
            }

            UIColors.SetNormalBgColor( lblAddress );
            lblAddress.Text = string.Empty;

            if ( Model_ContactPoint != null && Model_ContactPoint.Address != null )
            {
                string addressText = String.Empty;
                addressText = this.Context == Address.PatientMailing
                    ? Model_ContactPoint.Address.AsMailingLabelWithCountry()
                    : Model_ContactPoint.Address.AsMailingLabel();

                if ( addressText != String.Empty )
                {
                    lblAddress.Text = addressText;
                }
            }

            if ( AddressChanged != null )
            {
                AddressChanged( this, null );
            }
        }

        private void GetFormAddressWithStreetView(FormAddressWithStreet2Verification formAddressVerification, ArrayList addresses)
        {
            if (Model_ContactPoint != null && Model_ContactPoint.Address != null)
            {
                formAddressVerification.Model = Model_ContactPoint.Address.Clone();
            }
            else if (cmbCopyFrom.SelectedIndex == BLANK_CONTACTPOINT)
            {
                formAddressVerification.Model = null;
            }

            try
            {
                formAddressVerification.ShowDialog(this);

                addresses.Add(formAddressVerification.i_AddressSelected);
            }
            finally
            {
                formAddressVerification.Dispose();
            }
        }

        private void GetFormAddressWithStreetView(FormAddressVerification formAddressVerification, ArrayList addresses)
        {
           if (Model_ContactPoint != null && Model_ContactPoint.Address != null)
            {
                formAddressVerification.Model = Model_ContactPoint.Address.Clone();
            }
            else if (cmbCopyFrom.SelectedIndex == BLANK_CONTACTPOINT)
            {
                formAddressVerification.Model = null;
            }

            try
            {
                formAddressVerification.ShowDialog(this);

                addresses.Add(formAddressVerification.i_AddressSelected);
            }
            finally
            {
                formAddressVerification.Dispose();
            }
        }

        private void cmbCopyFrom_Click(object sender, EventArgs e)
        {
            SetCopyFromValues();
        }

        private void cmbCopyFrom_GotFocus(object sender, EventArgs e)
        {
            SetCopyFromValues();
        }

        private void cmbCopyFrom_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( cmbCopyFrom.SelectedIndex != BLANK_CONTACTPOINT )
            {
                if ( cmbCopyFrom.Items.Count > 1 && cmbCopyFrom.SelectedIndex > BLANK_CONTACTPOINT )
                {
                    ContactPoint selectedContactPoint = ( ContactPoint )( ( ( DictionaryEntry )cmbCopyFrom.SelectedItem ).Value );

                    StringBuilder sbMsg = new StringBuilder( 500 );
                    if ( selectedContactPoint.Address != null )
                    {
                        if (!IsAddressWithStreet2)
                        {
                            if (selectedContactPoint.Address.Address1.Length > 25)
                            {
                                selectedContactPoint.Address.Address1 =
                                    selectedContactPoint.Address.Address1.Substring(0, 25);
                            }
                            if (selectedContactPoint.Address.Address2.Length > 0)
                            {
                                selectedContactPoint.Address.Address2 = string.Empty;

                            }
                            sbMsg.Append(selectedContactPoint.Address.OneLineAddressLabelWithoutStreet2());
                        }
                        else
                        {
                            sbMsg.Append(selectedContactPoint.Address.OneLineAddressLabel());
                        }
                    }
                    if ( selectedContactPoint.PhoneNumber != null )
                    {
                        sbMsg.Append( " " );
                        sbMsg.Append( selectedContactPoint.PhoneNumber.AsFormattedString() );
                    }
                    if ( selectedContactPoint.CellPhoneNumber != null )
                    {
                        sbMsg.Append( " " );
                        sbMsg.Append( selectedContactPoint.CellPhoneNumber.AsFormattedString() );
                    }
                    if ( selectedContactPoint.EmailAddress != null )
                    {
                        sbMsg.Append( " " );
                        sbMsg.Append( selectedContactPoint.EmailAddress.ToString() );
                    }
                    BreadCrumbLogger.GetInstance.Log( String.Format( "Contact Info copied from {0}", sbMsg ) );

                    PhoneNumber phone = ( selectedContactPoint.PhoneNumber == null ) ? ( new PhoneNumber() ) : ( ( PhoneNumber )selectedContactPoint.PhoneNumber.Clone() );
                    PhoneNumber cell = ( selectedContactPoint.CellPhoneNumber == null ) ? ( new PhoneNumber() ) : ( ( PhoneNumber )selectedContactPoint.CellPhoneNumber.Clone() );

                    UpdateContactPoint( selectedContactPoint.Address, phone, cell, selectedContactPoint.EmailAddress );
                    UpdateView();

                    NotifyContactPointChange();
                }

                cmbCopyFrom.SelectedIndex = BLANK_CONTACTPOINT;
            }

        }

        private void mtbEmailAddress_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );

            // check if only valid email special characters have been entered or pasted
            if ( mtb.Text != string.Empty && emailKeyPressExpression.IsMatch( mtb.Text ) == false )
            {   // Prevent cursor from advancing to the next control
                e.Cancel = true;
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
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
                UIColors.SetErrorBgColor(mtb);
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                mtb.Focus();
            }
            else
            {
                if (mtb.Text == String.Empty)
                {
                    Model_ContactPoint.EmailAddress = new EmailAddress();
                }
                else
                {
                    Model_ContactPoint.EmailAddress = new EmailAddress(mtb.Text);
                }

                if (EmailChanged != null)
                {
                    EmailChanged(this, null);
                }
            }
        }
       
        public void GuarantorConsentRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor( cmbCellConsent );
        }

        public void GuarantorConsentPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor(cmbCellConsent);
        }
        public void EmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            SetEmailAddressAsRequired();
        }

        public void SetEmailAddressAsRequired()
        {
            UIColors.SetRequiredBgColor(mtbEmail);
        }

        public void EmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            SetEmailAddressAsPreferred();
        }

        public void SetEmailAddressAsPreferred()
        {
            UIColors.SetPreferredBgColor(mtbEmail);
        }

        public void SetEmailAddressAsNormal()
        {
            UIColors.SetNormalBgColor(mtbEmail);
        }

        public void GuarantorEmailAddressRequiredEventHandler(object sender, EventArgs e)
        {
            SetGuarantorEmailAddressAsRequired();
        }

        public void SetGuarantorEmailAddressAsRequired()
        {
            UIColors.SetRequiredBgColor(mtbEmail);
        }

        public void GuarantorEmailAddressPreferredEventhandler(object sender, EventArgs e)
        {
            SetGuarantorEmailAddressAsPreferred();
        }

        public void SetGuarantorEmailAddressAsPreferred()
        {
            UIColors.SetPreferredBgColor(mtbEmail);
        }

        public void UnregisterEventHandlers()
        {
            RuleEngine.GetInstance()
                .UnregisterEvent(typeof(GuarantorConsentRequired), PatientAccount,
                    GuarantorConsentRequiredEventHandler);
            RuleEngine.GetInstance()
                .UnregisterEvent(typeof(GuarantorConsentPreferred), PatientAccount.Guarantor,
                    GuarantorConsentPreferredEventHandler);
            RuleEngine.GetInstance()
                .UnregisterEvent(typeof(EmailAddressRequired), Model_Account, EmailAddressRequiredEventHandler);
            RuleEngine.GetInstance()
                .UnregisterEvent(typeof(EmailAddressPreferred), Model_Account, EmailAddressPreferredEventhandler);
            RuleEngine.GetInstance()
                .UnregisterEvent(typeof(GuarantorEmailAddressPreferred), PatientAccount.Guarantor,
                    GuarantorEmailAddressPreferredEventhandler);
        }

        #endregion

        #region Methods
        /// <summary>
        /// Populate change from Model to UI
        /// </summary>
        public override void UpdateView()
        {
            Loading = true;
            ClearControls();
            RegisterEventHandlers();
            var cellPhoneConsentBroker = BrokerFactory.BrokerOfType<ICellPhoneConsentBroker>();
            CellPhoneConsentPresenter = new CellPhoneConsentPresenter(this, PatientAccount, Model_ContactPoint, cellPhoneConsentBroker);
            EmailAddressPresenter = new EmailAddressPresenter(this, PatientAccount, RuleEngine.GetInstance());
            SetCopyFromValues();
            
            if ( Model_ContactPoint != null )
            {
                if (Model_ContactPoint.Address != null)
                {
                    var addressText = String.Empty;
                    addressText = this.Context == Address.PatientMailing
                        ? Model_ContactPoint.Address.AsMailingLabelWithCountry()
                        : Model_ContactPoint.Address.AsMailingLabel();

                    if (addressText != String.Empty)
                    {
                        lblAddress.Text = addressText;
                    }
                }

                if ( Model_ContactPoint.PhoneNumber != null &&
                    ( Mode == AddressMode.PHONE ||
                      Mode == AddressMode.PHONECELL ||
                      Mode == AddressMode.PHONECELLEMAIL ||
                      Mode == AddressMode.PHONECELLCONSENTEMAIL) )
                {
                    phoneNumberControl.Model = Model_ContactPoint.PhoneNumber;
                }

                if ( Model_ContactPoint.CellPhoneNumber != null &&
                    ( Mode == AddressMode.PHONECELL ||
                      Mode == AddressMode.PHONECELLEMAIL ||
                      Mode == AddressMode.PHONECELLCONSENTEMAIL))
                {
                    cellPhoneNumberControl.Model = Model_ContactPoint.CellPhoneNumber;
                    if (CellPhoneNumberChanged != null)
                    {
                        CellPhoneNumberChanged(this, null);
                    }
                }

                if ( ( Mode == AddressMode.PHONECELLEMAIL ||
                    Mode == AddressMode.PHONECELLCONSENTEMAIL) && 
                    Model_ContactPoint.EmailAddress != null )
                {
                    mtbEmail.UnMaskedText = Model_ContactPoint.EmailAddress.ToString();
                }
                 if (( Mode == AddressMode.PHONECELLCONSENTEMAIL ) &&
                     ( Model_ContactPoint.CellPhoneNumber != null &&
                    Model_ContactPoint.CellPhoneConsent != null))
                {
                    cmbCellConsent.SelectedItem = Model_ContactPoint.CellPhoneConsent;
                }
               
               CellPhoneConsentPresenter.Initialize();
               
            }
            Loading = false;
            phoneNumberControl.RunRules();
            cellPhoneNumberControl.RunRules();
            if (!cmbCellConsent.Enabled)
            {
                cmbCellConsent.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        public void SetGroupBoxText( string text )
        {
            gbAddress.Text = text;
        }
        public void DoNotShowEmailAddress()
        {
            mtbEmail.Visible = false;
            lblEmail.Visible = false;
        }

        /// <summary>
        /// Reset CopyFrom combobox
        /// </summary>
        public void ResetView()
        {
            UpdateContactPoint( new Address(), new PhoneNumber(), new PhoneNumber(), new EmailAddress() );
            UpdateView();
        }
        public void SetEmailAddressToNormalColor()
        {
            UIColors.SetNormalBgColor(mtbEmail);
        }

        public void SetCellPhoneConsentNormal()
        {
            UIColors.SetNormalBgColor( cmbCellConsent );
        }
        
        public void ResetCellPhoneConsentValue()
        {
            if (!Loading)
            {
                cmbCellConsent.SelectedIndex = -1;
            }
            CellPhoneConsentPresenter.EnableOrDisableConsent();
        }

        public void ClearConsentSelectionValues()
        {
            cmbCellConsent.Items.Clear();
        }
      
       public void PopulateConsentSelections(IEnumerable<CellPhoneConsent> consentsValues)
        {
            var guarantorCellPhoneConsentFeatureManager = new GuarantorCellPhoneConsentFeatureManager();
            var featureIsEnabledToDefaultForCOSSigned =
                guarantorCellPhoneConsentFeatureManager.IsCellPhoneConsentRuleForCOSEnabledforaccount(PatientAccount);
            if (featureIsEnabledToDefaultForCOSSigned)
            {
                foreach (var consent in consentsValues)
                {
                    if (PatientAccount.COSSigned.IsYes)
                    {
                        if (consent.Code == CellPhoneConsent.WRITTEN_CONSENT ||
                            consent.Code == CellPhoneConsent.DECLINE_CONSENT ||
                            consent.Code == CellPhoneConsent.REVOKE_CONSENT ||
                            consent.Code == "")
                        {
                            cmbCellConsent.Items.Add(consent);
                        }
                    }
                    else
                    {
                        cmbCellConsent.Items.Add(consent);
                    }
                }
            }
            else
            {
                foreach (var consent in consentsValues)
                {
                   cmbCellConsent.Items.Add(consent);
                }
            }

            if (Model_ContactPoint.CellPhoneConsent != null && Model_ContactPoint.CellPhoneConsent.Code != string.Empty)
            { 
                cmbCellConsent.SelectedItem = Model_ContactPoint.CellPhoneConsent;
                if(cmbCellConsent.SelectedItem == null)
                {
                    Model_ContactPoint.CellPhoneConsent = new CellPhoneConsent();
                    cmbCellConsent.SelectedIndex = 0;
                }
            }
            else
            {
                cmbCellConsent.SelectedIndex = 0;
            }
            if (PatientAccount.COSSigned.IsYes &&
                Model_ContactPoint.CellPhoneConsent.Code == string.Empty &&
                featureIsEnabledToDefaultForCOSSigned)
            {
                cmbCellConsent.SelectedIndex = 1;
            }
        }

        
        public void ValidateEmailAddress()
        {
            if (!IsEmailAddressValid())
            {
                SetEmailAsError();
            }
        }

        private bool IsEmailAddressValid()
        {
            var EmailAddressText = mtbEmail.Text;
            UIColors.SetNormalBgColor(mtbEmail);
            // check if only valid email special characters have been entered or pasted
            if (EmailAddressText != string.Empty && emailKeyPressExpression.IsMatch(EmailAddressText) == false)
            {

                UIColors.SetErrorBgColor(mtbEmail);
                MessageBox.Show(UIErrorMessages.ONLY_VALID_EMAIL_CHARACTERS_ALLOWED, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                return false;
            }

            // check if entered email is in the correct email format
            if (EmailAddressText != string.Empty &&
                 (emailValidationExpression.IsMatch(EmailAddressText) == false ||
                  EmailAddressPresenter.IsGenericEmailAddress(EmailAddressText))
                 )
            {
                // Prevent cursor from advancing to the next control

                UIColors.SetErrorBgColor(mtbEmail);
                MessageBox.Show(UIErrorMessages.EMAIL_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

                return false;
            }
            else
            {
                if (EmailAddressText == String.Empty)
                {
                    Model_ContactPoint.EmailAddress = new EmailAddress();
                }
                else
                {
                    Model_ContactPoint.EmailAddress = new EmailAddress(EmailAddressText);
                }

                if (EmailChanged != null)
                {
                    EmailChanged(this, null);
                }
            }
            return true;
        }


        #endregion

        #region Properties

        public bool CapturePhysicalAddress { set; private get; }
        public bool CaptureMailingAddress { set; private get; }

        public bool CountyRequiredForCurrentActivity { private get; set; }

        public string GroupBoxText
        {
            // Allows the setting of custom text
            set
            {
                gbAddress.Text = value;
            }
        }

        public AddressMode Mode
        {
            private get
            {
                return i_Mode;
            }
            set
            {
                i_Mode = value;
            }
        }

        public bool ShowStatus
        {
            private get
            {
                return i_ShowStatus;
            }
            set
            {
                i_ShowStatus = value;
            }
        }

        public ContactPoint Model_ContactPoint
        {
            get
            {
                if ( Model != null )
                {
                   return ( ContactPoint )Model;
                }
                
                return null;
            }
            set
            {
                Model = value;
            }
        }
        
        public Account PatientAccount
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        public Type KindOfTargetParty
        {
            get
            {
                return i_KindOfTargetParty;
            }
            set
            {
                i_KindOfTargetParty = value;
            }
        }

        public string Context
        {
            get
            {
                return i_Context;
            }
            set
            {
                i_Context = value;
            }
        }

        public ComboBox ComboBox
        {
            get
            {
                return cmbCopyFrom;
            }
        }

        public string EditAddressButtonText
        {
            get
            {
                return btnEdit.Text;
            }
            // Allows the setting of custom text
            set
            {
                btnEdit.Text = value;
            }
        }

        public string ClearButtonText
        {   // Allows the setting of custom text
            set
            {
                btnClear.Text = value;
            }
        }

        public bool IsAddressWithCounty
        {
            private get
            {
                return isAddressWithCounty;
            }
            set
            {
                isAddressWithCounty = value;
            }
        }

        public bool IsAddressWithStreet2
        {
            private get { return isAddressWithStreet2; }
            set { isAddressWithStreet2 = value; }
        }

        public CellPhoneConsent CellPhoneConsent
        {
            get
            {
                return cmbCellConsent.SelectedItem as CellPhoneConsent ?? new CellPhoneConsent();
            }
        }
        
        public int MobileAreaCodeLength
        {
            get
            {
                return cellPhoneNumberControl.AreaCode.Trim().Length;
            }
        }

        public int MobilePhoneNumberLength
        {
            get
            {
                return cellPhoneNumberControl.PhoneNumber.Trim().Length;
            }
        }
        
        #endregion

        #region Private Methods

        private void HideControlsBasedOnProperties()
        {
            int h = 0;
            width = 265;
            switch ( Mode )
            {
                case AddressMode.PHONECELLEMAIL:
                    h = 270;
                    lblCellConsent.Visible = false;
                    cmbCellConsent.Visible = false;
                    break;

               case AddressMode.PHONECELL:
                    lblEmail.Visible = false;
                    mtbEmail.Visible = false;
                    lblCellConsent.Visible = false;
                    cmbCellConsent.Visible = false;
                    lblPhone.Location = new Point(9, 18);
                    phoneNumberControl.Location = new Point(55, 13);
                    lblCell.Location = new Point(9, 41);
                    cellPhoneNumberControl.Location = new Point(55, 38);
                    panelContact.Size = new Size( 259, 69 );
                    panelStatus.Location = new Point( 3, 209 );
                    h = 265;
                    break;

                case AddressMode.PHONE:
                    lblEmail.Visible = false;
                    mtbEmail.Visible = false;
                    lblCell.Visible = false;
                    lblCellConsent.Visible = false;
                    cmbCellConsent.Visible = false;
                    cellPhoneNumberControl.SetVisible( false );
                    panelContact.Size = new Size( 259, 36 );
                    panelStatus.Location = new Point( 3, 176 );
                    h = 235;
                    break;

                case AddressMode.PHONECELLCONSENTEMAIL:
                        lblAddress.Size = new Size(240, 77);
                        SetPhoneCellConsentEmailProperties();
                        h = 292;
                        break; 

                case AddressMode.NONE:
                    lblEmail.Visible = false;
                    mtbEmail.Visible = false;
                    lblCell.Visible = false;
                    cellPhoneNumberControl.SetVisible( false );
                    lblPhone.Visible = false;
                    phoneNumberControl.SetVisible( false );
                    panelContact.Size = new Size( 259, 0 );
                    panelStatus.Location = new Point( 3, 143 );
                    h = 178;
                    break;

                default:
                    break;
            }

            if (ShowStatus)
            {
                lblStatus.Visible = true;
                lblStaticStatus.Visible = false;
                panelStatus.Visible = true;
            }
            else
            {
                lblStatus.Visible = false;
                lblStaticStatus.Visible = false;
                panelStatus.Visible = false;
                h = h - 25;
            }

            gbAddress.Size = new Size( width, h );
            Size = new Size( width, h );
        }

        private void SetPhoneCellConsentEmailProperties()
        {
            panelContact.Controls.Add(this.lblCellConsent);
            panelContact.Controls.Add(this.cmbCellConsent);
            lblPhone.Location = new Point(9, 18);
            phoneNumberControl.Location = new Point(55, 18);
            lblCell.Location = new Point(9, 44);
            cellPhoneNumberControl.Location = new Point(55, 43);
            panelStatus.Visible = false;
            cmbCellConsent.Visible = true;
            cmbCellConsent.Enabled = false;
            lblCellConsent.Location = new Point(9, 74);
            cmbCellConsent.Location = new Point(114, 72);
            lblEmail.Location = new Point(9, 99);
            mtbEmail.Location = new Point(58, 98);
            panelContact.Size = new Size(300, 127);
            ShowStatus = false;
            width = 300;
        }

        private void SetPhoneCellEmailProperties()
        {
            lblCellConsent.Visible = false;
            cmbCellConsent.Visible = false;
            lblPhone.Location = new Point(9, 18);
            phoneNumberControl.Location = new Point(55, 13);
            lblCell.Location = new Point(9, 41);
            cellPhoneNumberControl.Location = new Point(55, 38);
            panelStatus.Visible = false;
            lblEmail.Location = new Point(9, 68);
            mtbEmail.Location = new Point(58, 67);
            panelContact.Size = new Size(259, 127);
            ShowStatus = false;
        }

        public void SetCopyFromValues()
        {
            UIColors.SetNormalBgColor( cmbCopyFrom );
            cmbCopyFrom.Items.Clear();

            cmbCopyFrom.ValueMember = "Value";
            cmbCopyFrom.DisplayMember = "Key";

            cmbCopyFrom.Items.Add( String.Empty );
            cmbCopyFrom.SelectedItem = String.Empty;
            cmbCopyFrom.SelectedIndex = BLANK_CONTACTPOINT;

            if ( PatientAccount != null )
            {
                foreach ( DictionaryEntry entry in PatientAccount.ContactPointsForCopyingToWithContext( Context ) )
                {
                    cmbCopyFrom.Items.Add( entry );
                }
            }
        }

        private void UpdateContactPoint(Address address, PhoneNumber phone, PhoneNumber cell, EmailAddress email)
        {
            if (Model_ContactPoint != null)
            {
                Model_ContactPoint.Address = address;

                if ( Model_ContactPoint.Address.Country == null ||
                    String.IsNullOrEmpty(Model_ContactPoint.Address.Country.Code) &&
                    Model_ContactPoint.TypeOfContactPoint.Description.ToUpper() ==
                    TypeOfContactPoint.MAILING_CONTACTPOINT_DESCRIPTION)
                {
                    Model_ContactPoint.Address.Country = Country.NewUnitedStatesCountry();
                }

                Model_ContactPoint.PhoneNumber = phone;
                Model_ContactPoint.CellPhoneNumber = cell;
                Model_ContactPoint.EmailAddress = email;
            }
        }

        private void NotifyContactPointChange()
        {
            if ( AddressChanged != null )
            {
                AddressChanged( this, null );
            }

            if ( PhoneNumberChanged != null )
            {
                PhoneNumberChanged( this, null );
            }

            if ( CellPhoneNumberChanged != null )
            {
                CellPhoneNumberChanged( this, null );
            }

            if ( EmailChanged != null )
            {
                EmailChanged( this, null );
            }
        }

        private void ClearControls()
        {
            UIColors.SetNormalBgColor( lblAddress );

            phoneNumberControl.SetNormalColor();
            cellPhoneNumberControl.SetNormalColor();

            UIColors.SetNormalBgColor( mtbEmail );

            lblAddress.Text = string.Empty;
            phoneNumberControl.AreaCode = string.Empty;
            phoneNumberControl.PhoneNumber = string.Empty;
            cellPhoneNumberControl.AreaCode = string.Empty;
            cellPhoneNumberControl.PhoneNumber = string.Empty;
            mtbEmail.UnMaskedText = string.Empty;
            cmbCellConsent.Text = string.Empty;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmail( mtbEmail );
        }

        private void cmbCellConsent_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCellPhoneConsent();
        }

        private void UpdateCellPhoneConsent()
        {
            CellPhoneConsentPresenter.UpdateConsent();
        }
        
        private void cmbCellConsent_Validating(object sender, CancelEventArgs e)
        {
            CellPhoneConsentPresenter.CellPhoneConsentChanged();
        }
        
        private void AddressView_Disposed(object sender, EventArgs e)
        {
            UnregisterEventHandlers();
        }
  
        private void RegisterEventHandlers()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorConsentRequired), PatientAccount, GuarantorConsentRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorConsentPreferred), PatientAccount.Guarantor, GuarantorConsentPreferredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(EmailAddressRequired), PatientAccount, EmailAddressRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(EmailAddressPreferred), PatientAccount, EmailAddressPreferredEventhandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(GuarantorEmailAddressPreferred), PatientAccount.Guarantor, GuarantorEmailAddressPreferredEventhandler);
           
        }

        public void Enable()
        {
            cmbCellConsent.Enabled = true;
        }

        public void Disable()
        {
            cmbCellConsent.Enabled = false;
        }

        public void ValidateCellPhoneConsent()
        {
            CellPhoneConsentPresenter.ValidateConsent();
        }

    

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddressView));
            this.gbAddress = new System.Windows.Forms.GroupBox();
            this.lblCopy = new System.Windows.Forms.Label();
            this.cmbCopyFrom = new System.Windows.Forms.ComboBox();
            this.btnClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEdit = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAddress = new System.Windows.Forms.Label();
            this.panelContact = new System.Windows.Forms.Panel();
            this.lblCellConsent = new System.Windows.Forms.Label();
            this.cmbCellConsent = new System.Windows.Forms.ComboBox();
            this.cellPhoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblCell = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.mtbEmail = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStaticStatus = new System.Windows.Forms.Label();
            this.gbAddress.SuspendLayout();
            this.panelContact.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbAddress
            // 
            this.gbAddress.Controls.Add(this.lblCopy);
            this.gbAddress.Controls.Add(this.cmbCopyFrom);
            this.gbAddress.Controls.Add(this.btnClear);
            this.gbAddress.Controls.Add(this.btnEdit);
            this.gbAddress.Controls.Add(this.lblAddress);
            this.gbAddress.Controls.Add(this.panelContact);
            this.gbAddress.Controls.Add(this.panelStatus);
            this.gbAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbAddress.Location = new System.Drawing.Point(0, 0);
            this.gbAddress.Name = "gbAddress";
            this.gbAddress.Size = new System.Drawing.Size(265, 270);
            this.gbAddress.TabIndex = 0;
            this.gbAddress.TabStop = false;
            this.gbAddress.Text = "Address";
            // 
            // lblCopy
            // 
            this.lblCopy.Location = new System.Drawing.Point(9, 23);
            this.lblCopy.Name = "lblCopy";
            this.lblCopy.Size = new System.Drawing.Size(60, 23);
            this.lblCopy.TabIndex = 2;
            this.lblCopy.Text = "Copy from:";
            // 
            // cmbCopyFrom
            // 
            this.cmbCopyFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCopyFrom.Location = new System.Drawing.Point(69, 20);
            this.cmbCopyFrom.Name = "cmbCopyFrom";
            this.cmbCopyFrom.Size = new System.Drawing.Size(179, 21);
            this.cmbCopyFrom.TabIndex = 3;
            this.cmbCopyFrom.SelectedIndexChanged += new System.EventHandler(this.cmbCopyFrom_SelectedIndexChanged);
            this.cmbCopyFrom.Click += new System.EventHandler(this.cmbCopyFrom_Click);
            this.cmbCopyFrom.GotFocus += new System.EventHandler(this.cmbCopyFrom_GotFocus);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(112, 46);
            this.btnClear.Message = null;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(9, 46);
            this.btnEdit.Message = null;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(87, 23);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit Address...";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblAddress
            // 
            this.lblAddress.Location = new System.Drawing.Point(9, 75);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(240, 92);
            this.lblAddress.TabIndex = 6;
            // 
            // panelContact
            // 
            this.panelContact.Controls.Add(this.lblCellConsent);
            this.panelContact.Controls.Add(this.cmbCellConsent);
            this.panelContact.Controls.Add(this.cellPhoneNumberControl);
            this.panelContact.Controls.Add(this.phoneNumberControl);
            this.panelContact.Controls.Add(this.lblPhone);
            this.panelContact.Controls.Add(this.lblCell);
            this.panelContact.Controls.Add(this.lblEmail);
            this.panelContact.Controls.Add(this.mtbEmail);
            this.panelContact.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelContact.Location = new System.Drawing.Point(3, 143);
            this.panelContact.Name = "panelContact";
            this.panelContact.Size = new System.Drawing.Size(262, 103);
            this.panelContact.TabIndex = 7;
            // 
            // lblCellConsent
            // 
            this.lblCellConsent.Location = new System.Drawing.Point(9, 50);
            this.lblCellConsent.Name = "lblCellConsent";
            this.lblCellConsent.Size = new System.Drawing.Size(105, 17);
            this.lblCellConsent.TabIndex = 15;
            this.lblCellConsent.Text = "Cell Phone Consent:";
            // 
            // cmbCellConsent
            // 
            this.cmbCellConsent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCellConsent.Enabled = false;
            this.cmbCellConsent.Location = new System.Drawing.Point(100, 46);
            this.cmbCellConsent.Name = "cmbCellConsent";
            this.cmbCellConsent.Size = new System.Drawing.Size(178, 21);
            this.cmbCellConsent.TabIndex = 13;
            this.cmbCellConsent.Visible = false;
            this.cmbCellConsent.SelectedIndexChanged += new System.EventHandler(this.cmbCellConsent_SelectedIndexChanged);
            this.cmbCellConsent.Validating += new System.ComponentModel.CancelEventHandler(this.cmbCellConsent_Validating);
            // 
            // cellPhoneNumberControl
            // 
            this.cellPhoneNumberControl.AreaCode = "";
            this.cellPhoneNumberControl.Location = new System.Drawing.Point(55, 22);
            this.cellPhoneNumberControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cellPhoneNumberControl.Model = ((PatientAccess.Domain.Parties.PhoneNumber)(resources.GetObject("cellPhoneNumberControl.Model")));
            this.cellPhoneNumberControl.Name = "cellPhoneNumberControl";
            this.cellPhoneNumberControl.PhoneNumber = "";
            this.cellPhoneNumberControl.Size = new System.Drawing.Size(94, 27);
            this.cellPhoneNumberControl.TabIndex = 11;
            this.cellPhoneNumberControl.AreaCodeChanged += new System.EventHandler(this.cellPhoneNumberControl_AreaCodeChanged);
            this.cellPhoneNumberControl.PhoneNumberChanged += new System.EventHandler(this.cellPhoneNumberControl_PhoneNumberChanged);
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point(55, 1);
            this.phoneNumberControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.phoneNumberControl.Model = ((PatientAccess.Domain.Parties.PhoneNumber)(resources.GetObject("phoneNumberControl.Model")));
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size(94, 27);
            this.phoneNumberControl.TabIndex = 9;
            this.phoneNumberControl.AreaCodeChanged += new System.EventHandler(this.phoneNumberControl_AreaCodeChanged);
            this.phoneNumberControl.PhoneNumberChanged += new System.EventHandler(this.phoneNumberControl_PhoneNumberChanged);
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point(9, 6);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(40, 23);
            this.lblPhone.TabIndex = 8;
            this.lblPhone.Text = "Phone:";
            // 
            // lblCell
            // 
            this.lblCell.Location = new System.Drawing.Point(9, 28);
            this.lblCell.Name = "lblCell";
            this.lblCell.Size = new System.Drawing.Size(40, 23);
            this.lblCell.TabIndex = 10;
            this.lblCell.Text = "Cell:";
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(9, 49);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(40, 23);
            this.lblEmail.TabIndex = 12;
            this.lblEmail.Text = "Email:";
            // 
            // mtbEmail
            // 
            this.mtbEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmail.Location = new System.Drawing.Point(58, 49);
            this.mtbEmail.Mask = "";
            this.mtbEmail.MaxLength = 64;
            this.mtbEmail.Name = "mtbEmail";
            this.mtbEmail.Size = new System.Drawing.Size(158, 20);
            this.mtbEmail.TabIndex = 14;
            this.mtbEmail.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEmailAddress_Validating);
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.lblStatus);
            this.panelStatus.Controls.Add(this.lblStaticStatus);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(3, 241);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(259, 26);
            this.panelStatus.TabIndex = 14;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(55, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(196, 16);
            this.lblStatus.TabIndex = 15;
            // 
            // lblStaticStatus
            // 
            this.lblStaticStatus.Location = new System.Drawing.Point(8, 8);
            this.lblStaticStatus.Name = "lblStaticStatus";
            this.lblStaticStatus.Size = new System.Drawing.Size(41, 16);
            this.lblStaticStatus.TabIndex = 16;
            this.lblStaticStatus.Text = "Status:";
            // 
            // AddressView
            // 
            this.Controls.Add(this.gbAddress);
            this.Name = "AddressView";
            this.Size = new System.Drawing.Size(265, 270);
            this.Load += new System.EventHandler(this.AddressView_Load);
            this.Disposed += new System.EventHandler(this.AddressView_Disposed);
            this.gbAddress.ResumeLayout(false);
            this.panelContact.ResumeLayout(false);
            this.panelContact.PerformLayout();
            this.panelStatus.ResumeLayout(false);
            this.ResumeLayout(false);

        }

       
        #endregion

        #endregion

        #region Private Properties

        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? (i_RuleEngine = RuleEngine.GetInstance()); }
        }

        private Account Model_Account
        {
            get { return (Account)Model; }
            set { Model = value; }
        }

        #endregion

        #region Construction and Finalization

        public AddressView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            

                emailValidationExpression = new Regex( RegularExpressions.EmailValidFormatExpression );
                emailKeyPressExpression = new Regex( RegularExpressions.EmailValidCharactersExpression );
                ConfigureControls();
        }

        //TODO-AC-SR352 delete this
        public AddressView( AddressMode addressMode )
        {
            Mode = addressMode;
            InitializeComponent();

            ConfigureControls();
        }

        //TODO-AC-SR352 delete this
        public AddressView( AddressMode addressMode, bool showStatus )
        {
            Mode = addressMode;
            ShowStatus = showStatus;
            InitializeComponent();

            ConfigureControls();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements

        private Container components = null;
        private GroupBox gbAddress;

        private Label lblCopy;
        private ComboBox cmbCopyFrom;
        private LoggingButton btnEdit;
        private LoggingButton btnClear;
        private Label lblAddress;

        private Label lblStaticStatus;
        private Label lblStatus;
        private Panel panelStatus;

        private readonly Regex emailValidationExpression;
        private readonly Regex emailKeyPressExpression;

        private Account i_Account;
        private Type i_KindOfTargetParty;
        private string i_Context;

        private AddressMode i_Mode = AddressMode.NONE;
        private bool i_ShowStatus = true;
        private bool isAddressWithCounty;
        private bool isAddressWithStreet2;

        private RuleEngine i_RuleEngine;
        private Panel panelContact;
        private Label lblCellConsent;
        private ComboBox cmbCellConsent;
        public PhoneNumberControl cellPhoneNumberControl;
        private PhoneNumberControl phoneNumberControl;
        private Label lblPhone;
        private Label lblCell;
        private Label lblEmail;
        private MaskedEditTextBox mtbEmail;
        private CellPhoneConsentPresenter CellPhoneConsentPresenter { get; set; }
        private EmailAddressPresenter EmailAddressPresenter { get; set; }
        private bool Loading;
        private int width;

        public enum AddressMode
        {
            NONE,
            PHONE,          // showing phone
            PHONECELL,      // showing phone, cell
            PHONECELLEMAIL, // showing phone, cell and email
            PHONECELLCONSENTEMAIL  //showing phone, cell, cosent and email
        };
        #endregion

        #region Constants

        const int BLANK_CONTACTPOINT = 0;
        #endregion
        public void SetEmailAsError()
        {
            UIColors.SetErrorBgColor(mtbEmail);
            mtbEmail.Focus();
        }


    }
}