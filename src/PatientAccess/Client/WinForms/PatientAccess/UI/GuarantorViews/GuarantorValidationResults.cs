using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.GuarantorViews
{
    /// <summary>
    /// Summary description for GuarantorValidationResults.
    /// </summary>
    [Serializable]
    public class GuarantorValidationResults : ControlView
    {
		#region Events

		public event EventHandler EnableOKButton;

		#endregion 

        #region Rule Event Handlers

        void phoneNumberControl_Validating( object sender, CancelEventArgs e )
        {
			if( this.phoneNumberControl.AreaCode.Trim().Length == 0 
                && this.phoneNumberControl.PhoneNumber.Trim().Length == 0 )
			{
				this.GuarantorPhoneNumber = new PhoneNumber();
			}
			else
			{
                this.GuarantorPhoneNumber = this.phoneNumberControl.Model;
			}			

            this.ValidateRequiredFields();
        }

        private void btnAlerts_Click(object sender, EventArgs e)
        {
			GuarantorValidationAlerts gva = new GuarantorValidationAlerts();
			gva.Model = this.Model_Guarantor;
			if( gva.SingletonCreated )
			{
				try
				{
					gva.ShowDialog();
				}
				finally
				{
					gva.Dispose();
				}
			}

//			FormGuarantorValidationActions formGuarantorValidationActions = new FormGuarantorValidationActions();
//
//			formGuarantorValidationActions.Model = this.Model_Guarantor;
//			formGuarantorValidationActions.UpdateView();
//			formGuarantorValidationActions.ShowDialog( this );
        }

        private void btnCopySentName_Click(object sender, EventArgs e)
        {
			this.mtbRecordAsLastname.Text = this.lblSentLastname.Text;
			this.mtbRecordAsFirstname.Text = this.lblSentFirstname.Text;
			this.mtbRecordAsMiddleInitial.Text = this.lblSentMiddleInitial.Text;
			this.ValidateRequiredFields();
        }

        private void btnCopyReturnedName_Click(object sender, EventArgs e)
        {
			this.mtbRecordAsLastname.Text = this.lblReturnedLastname.Text;
			this.mtbRecordAsFirstname.Text = this.lblReturnedFirstname.Text;
			this.mtbRecordAsMiddleInitial.Text = this.lblReturnedMiddleInitial.Text;
			this.ValidateRequiredFields();
        }

        private void mtbRecordAsLastname_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }

		private void mtbRecordAsLastname_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;
			this.ValidateRequiredFields();
		}

		private void mtbRecordAsFirstname_Validating(object sender, CancelEventArgs e)
		{
			this.ValidateRequiredFields();
		}

        private void btnCopySSNSent_Click(object sender, EventArgs e)
        {
			this.mtbSSNRecordAs.UnMaskedText = this.Model_Guarantor.SocialSecurityNumber.ToString();
            this.GuarantorSSN = this.Model_Guarantor.SocialSecurityNumber;
			this.mtbSSNRecordAs.Focus();
        }

        private void btnCopySSNRecv_Click(object sender, EventArgs e)
        {
			this.mtbSSNRecordAs.UnMaskedText = this.Model_Guarantor.CreditReport.ServiceSSN;
            this.GuarantorSSN = new SocialSecurityNumber( this.mtbSSNRecordAs.UnMaskedText );
			this.mtbSSNRecordAs.Focus();
        }

		
		private void mtbSSNRecordAs_Validating(object sender, CancelEventArgs e)
		{
			//MaskedEditTextBox mtb = sender as MaskedEditTextBox;

			if( mtbSSNRecordAs.UnMaskedText.Length > 0 && mtbSSNRecordAs.UnMaskedText.Length < 11 )
			{   // Prevent cursor from advancing to the next control
				UIColors.SetErrorBgColor( mtbSSNRecordAs );
				MessageBox.Show( UIErrorMessages.SSN_INVALID, "Error",
					MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
					MessageBoxDefaultButton.Button1 );
				//mtbSSNRecordAs.Focus();
				//return;
			}
			else
			{
				UIColors.SetNormalBgColor( this.mtbSSNRecordAs );
				if( mtbSSNRecordAs.Text.Length == 0 )
				{
					this.GuarantorSSN = new SocialSecurityNumber();
				}
				else
				{
					this.GuarantorSSN = new SocialSecurityNumber( this.mtbSSNRecordAs.UnMaskedText );
				}
				//Model_Account.Patient.SocialSecurityNumber = new SocialSecurityNumber( ssnControlText );
			}

			this.ValidateRequiredFields();
		}

        private void mtbSSNRecordAs_Enter(object sender, EventArgs e)
        {
//            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
//            UIColors.SetNormalBgColor( mtb );
        }

		private void btnValidate_Click(object sender, EventArgs e)
		{
            ListBox lb = sender as ListBox;
            this.btnValidate.Enabled = true;
            Address selectedAddress = null;

            selectedAddress = (Address)this.listBox_Addresses.SelectedItem;
            						
            if( selectedAddress != null )
            {
                this.ValidateAndDisplaySelectedAddress( selectedAddress );
            }
		}

        private void btnCopyPhoneSent_Click(object sender, EventArgs e)
        {
            this.phoneNumberControl.Model = this.SentContactPoint.PhoneNumber;
            this.GuarantorPhoneNumber = this.phoneNumberControl.Model;
            this.phoneNumberControl.FocusAreaCode();
            this.ValidateRequiredFields();
        }

        private void btnCopyPhoneRecv_Click(object sender, EventArgs e)
        {
			this.phoneNumberControl.Model = this.Model_Guarantor.CreditReport.ServicePhoneNumber;
            this.GuarantorPhoneNumber = this.phoneNumberControl.Model;
            this.phoneNumberControl.FocusAreaCode();
            this.ValidateRequiredFields();
        }

        private void btnCopyAddress_Click(object sender, EventArgs e)
        {
			this.GuarantorAddress = this.SentContactPoint.Address;
			this.lblAddressRecordAs.Text = this.lblMailingAddress.Text;
			this.ValidateRequiredFields();
        }

        private void listBox_Addresses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this.listBox_Addresses.SelectedIndex >= 0 )
            {
                this.btnValidate.Enabled = true;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {	
			this.GuarantorName = new Name( String.Empty, String.Empty, String.Empty );

			int hawkAlertCnt = this.Model_Guarantor.CreditReport.ServiceHawkAlerts.Count;
			if( hawkAlertCnt < 1 )
			{
				this.btnAlerts.Enabled = false;
			}
			this.lblAlerts.Text = ALERTS_MSG + hawkAlertCnt + " alerts.";
			
		    this.PopulateSentGuarantorInfo();
		    this.PopulateReceivedGuarantorInfo();
		    this.EnableDisableNameCopyButton();
		    this.EnableDisableSSNCopyButton();
		    this.EnableDisablePhoneCopyButton();

            //this.OriginalContactPoint = this.guarantorSentContactPoint;

			this.ValidateRequiredFields();

            RunRules();
        }

        public override void UpdateModel()
        {
        }

		public bool IsValidPhoneNumber()
		{

			return ( this.phoneNumberControl.PhoneNumber.Trim().Length == 7 );
		}

		public bool IsValidSSN()
		{
			return ( this.mtbSSNRecordAs.UnMaskedText.Length == 9 );
		}
        #endregion

        #region Properties
		public Guarantor Model_Guarantor
		{
			private get
			{
				return (Guarantor)this.Model;
			}
			set
			{
				this.Model = value;
			}
		}

		public PhoneNumber GuarantorPhoneNumber
		{
			get
			{
				return i_PhoneNumber;
			}
			set
			{
				this.i_PhoneNumber = value;
			}
		}

		public Name GuarantorName
		{
			get
			{
				return i_Name;
			}
			set
			{
				this.i_Name = value;
			}
		}

		public SocialSecurityNumber GuarantorSSN
		{
			get
			{
				return i_SSN;
			}
			set
			{
				this.i_SSN = value;
			}
		}

		public ContactPoint SentContactPoint
		{
			private get
			{
				return i_SentContactPoint;
			}
			set
			{
				this.i_SentContactPoint = value;
			}
		}

		public Address GuarantorAddress
		{
			get
			{
				return i_GuarantorAddress;
			}
			set
			{
				i_GuarantorAddress = value;
			}

		}

        private RuleEngine RuleEngine
        {
            get
            {
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
		private void ValidateRequiredFields()
		{
            if( GuarantorName == null )
            {
                return;
            }

			YesNoFlag yesNo = new YesNoFlag();

			if( this.mtbRecordAsLastname.Text.Trim() != String.Empty &&
				this.mtbRecordAsFirstname.Text.Trim() != String.Empty &&
				GuarantorAddress != null &&
				( this.mtbSSNRecordAs.UnMaskedText.Length == 0 || this.mtbSSNRecordAs.UnMaskedText.Length == 9 ) &&
				this.phoneNumberControl.PhoneNumber.Trim().Length == 7 )
			{
				GuarantorName.LastName = this.mtbRecordAsLastname.Text;
				GuarantorName.FirstName = this.mtbRecordAsFirstname.Text;
				GuarantorName.MiddleInitial = this.mtbRecordAsMiddleInitial.Text;

				yesNo.SetYes();
				this.EnableOKButton( this, new LooseArgs( yesNo ) );
			}
			else
			{
				yesNo.SetNo();

                if( this.EnableOKButton != null )
                {
                    this.EnableOKButton( this, new LooseArgs( yesNo ) );
                }				
			}
		}

		private void ValidateAndDisplaySelectedAddress( Address selectedAddress )
		{
            var currentUser = User.GetCurrent();
            var facility = currentUser.Facility;

            var formAddressVerification = new EmployerFormAddressVerification( facility, RuleEngine );

			formAddressVerification.Model = selectedAddress;
			formAddressVerification.UpdateView();

			try
			{
				formAddressVerification.ShowDialog( this );

				if( formAddressVerification.DialogResult == DialogResult.OK )
				{
					this.GuarantorAddress = formAddressVerification.i_AddressSelected;
					this.lblAddressRecordAs.Text = GuarantorAddress.AsMailingLabel();
					this.ValidateRequiredFields();
				}
			}
			finally
			{
				formAddressVerification.Dispose();
			}
		}

		private void EnableDisableNameCopyButton()
		{
			if( ( this.Model_Guarantor.Name.LastName.Trim()        == this.Model_Guarantor.CreditReport.ServiceLastName.Trim() ) &&
				( this.Model_Guarantor.Name.FirstName.Trim()       == this.Model_Guarantor.CreditReport.ServiceFirstName.Trim() ) &&
				( this.Model_Guarantor.Name.MiddleInitial.Trim()   == this.Model_Guarantor.CreditReport.ServiceMiddleName.Trim() ) )
			{
				this.btnCopyReturnedName.Enabled        = false;
				this.btnCopySentName.Enabled            = false;
				this.mtbRecordAsLastname.Text           = this.Model_Guarantor.Name.LastName.Trim();
				this.mtbRecordAsFirstname.Text          = this.Model_Guarantor.Name.FirstName.Trim();
				this.mtbRecordAsMiddleInitial.Text      = this.Model_Guarantor.Name.MiddleInitial.Trim();

				this.mtbRecordAsLastname.Enabled        = false;
				this.mtbRecordAsFirstname.Enabled       = false;
				this.mtbRecordAsMiddleInitial.Enabled   = false;
				return;
			}

			if( ( this.Model_Guarantor.CreditReport.ServiceLastName.Trim() == String.Empty ) &&
				( this.Model_Guarantor.CreditReport.ServiceFirstName.Trim() == String.Empty ) &&
				( this.Model_Guarantor.CreditReport.ServiceMiddleName.Trim() == String.Empty ) )
			{
				this.btnCopyReturnedName.Enabled = false;
			}

			if( ( this.Model_Guarantor.Name.LastName.Trim() == String.Empty ) &&
				( this.Model_Guarantor.Name.FirstName.Trim() == String.Empty ) &&
				( this.Model_Guarantor.Name.MiddleInitial.Trim() == String.Empty ) )
			{
				this.btnCopySentName.Enabled = false;
			}
		}

		private void EnableDisablePhoneCopyButton()
		{
			if( this.SentContactPoint.PhoneNumber.AsUnformattedString().Trim() != String.Empty &&
				this.Model_Guarantor.CreditReport.ServicePhoneNumber.AsUnformattedString().Trim() != String.Empty &&
				this.SentContactPoint.PhoneNumber.AsUnformattedString().Trim() == 
				this.Model_Guarantor.CreditReport.ServicePhoneNumber.AsUnformattedString().Trim() )
			{				
				this.btnCopyPhoneSent.Enabled = false;
				this.btnCopyPhoneRecv.Enabled = false;
                this.phoneNumberControl.Model = this.Model_Guarantor.CreditReport.ServicePhoneNumber;
                this.GuarantorPhoneNumber = this.phoneNumberControl.Model;
                this.phoneNumberControl.ToggleEnabled( false );
				return;
			}

			if(  this.SentContactPoint.PhoneNumber.AsFormattedString() == string.Empty )
			{
				this.btnCopyPhoneSent.Enabled = false;
			}

            if( this.Model_Guarantor.CreditReport.ServicePhoneNumber.AsUnformattedString() == string.Empty )
            {
                this.btnCopyPhoneRecv.Enabled = false;
            }
		}

		private void EnableDisableSSNCopyButton()
		{
            if( this.Model_Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() == 
                this.Model_Guarantor.CreditReport.ServiceSSN.Trim() ) 
            {
                this.btnCopySSNSent.Enabled = false;
                this.btnCopySSNRecv.Enabled = false;
				if( this.Model_Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() != String.Empty )
				{
					this.mtbSSNRecordAs.Text = this.Model_Guarantor.SocialSecurityNumber.DisplayString.Trim();
				}

				if( this.Model_Guarantor.CreditReport.ServiceSSN.Length > 0 )
				{
					
					this.GuarantorSSN = new SocialSecurityNumber( this.mtbSSNRecordAs.UnMaskedText );
					this.mtbSSNRecordAs.Enabled = false;
				}
				else
				{
					this.mtbSSNRecordAs.Enabled = true;
				}
				
                return;
            }
            else
            {
                this.btnCopySSNSent.Enabled = true;
                this.btnCopySSNRecv.Enabled = true;
            }

			if( this.Model_Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber.Trim() == String.Empty )
			{
				this.btnCopySSNSent.Enabled = false;
			}

			if( this.Model_Guarantor.CreditReport.ServiceSSN.Trim() == String.Empty )
			{
				this.btnCopySSNRecv.Enabled = false;
			}
		}

		private void PopulateSentGuarantorInfo()
		{
            if( this.Model_Guarantor == null )
            {
                return;
            }

			string phoneNbr = String.Empty;

            if( this.Model_Guarantor.Name != null )
            {
                this.lblSentLastname.Text           = this.Model_Guarantor.Name.LastName;
                this.lblSentFirstname.Text          = this.Model_Guarantor.Name.FirstName; 
                this.lblSentMiddleInitial.Text      = this.Model_Guarantor.Name.MiddleInitial;
            }

            if( this.Model_Guarantor.SocialSecurityNumber != null )
            {
                if(  this.Model_Guarantor.SocialSecurityNumber.UnformattedSocialSecurityNumber == string.Empty )
                {
                    this.lblSSNSent.Text = string.Empty;
                }
                else
                {
                    this.lblSSNSent.Text = this.Model_Guarantor.SocialSecurityNumber.DisplayString;
                }
            }
			
			ContactPoint cp  = new ContactPoint( TypeOfContactPoint.NewMailingContactPointType() );
			SentContactPoint = this.Model_Guarantor.ContactPointWith( cp.TypeOfContactPoint );

            if( SentContactPoint != null )
            {
                if( SentContactPoint.PhoneNumber.ToString().Length == 0 )
                {
                    this.lblPhoneSent.Text          = string.Empty;
                }
                else
                {
                    this.lblPhoneSent.Text          = SentContactPoint.PhoneNumber.AsFormattedString();
                }

                if( SentContactPoint.Address != null )
                {
                    this.lblMailingAddress.Text         = SentContactPoint.Address.AsMailingLabel();
                }                
            }
		}

		private void PopulateReceivedGuarantorInfo()
		{
			string phoneNbr = String.Empty;
			string ssn = String.Empty;

			this.lblReturnedLastname.Text       = this.Model_Guarantor.CreditReport.ServiceLastName;
			this.lblReturnedFirstname.Text      = this.Model_Guarantor.CreditReport.ServiceFirstName; 
			this.lblReturnedMiddleInitial.Text  = this.Model_Guarantor.CreditReport.ServiceMiddleName;

			if( this.Model_Guarantor != null &&
				this.Model_Guarantor.CreditReport != null &&
				this.Model_Guarantor.CreditReport.ServicePhoneNumber != null &&
				this.Model_Guarantor.CreditReport.ServicePhoneNumber.AsUnformattedString() != String.Empty )
			{
				phoneNbr = this.Model_Guarantor.CreditReport.ServicePhoneNumber.AsFormattedString();
			}

			this.lblPhoneReturned.Text          = phoneNbr;

			if( this.Model_Guarantor != null &&
				this.Model_Guarantor.CreditReport != null &&
				this.Model_Guarantor.CreditReport.ServiceSSN != null &&
			    this.Model_Guarantor.CreditReport.ServiceSSN.Length > 0 )
			{
				ssn = this.Model_Guarantor.CreditReport.ServiceSSN.Substring(0,3) + "-" +
					this.Model_Guarantor.CreditReport.ServiceSSN.Substring(3,2) + "-" +
					this.Model_Guarantor.CreditReport.ServiceSSN.Substring(5,4);
			}
			this.lblSSNReturned.Text            = ssn;

			this.PopulateAddressListReceived( this.Model_Guarantor.CreditReport.ServiceAddresses );
		}

		private void PopulateAddressListReceived( ArrayList addressList )
		{
            if( addressList != null )
            {
                this.listBox_Addresses.Items.Clear();
                if( addressList.Count > 0 )
                {
                    foreach( Address address in addressList )
                    {
                        listBox_Addresses.Items.Add( address );
                    }
                    if( listBox_Addresses.Items.Count > 0 )
                    {
                        listBox_Addresses.SetSelected( 0, true);
                    }
                    this.listBox_Addresses.ScrollAlwaysVisible = true;
                    //this.ShowCover = false;
                }
                else
                {
                    this.listBox_Addresses.ScrollAlwaysVisible = false;
                    //this.ShowCover = true;
                }
                this.listBox_Addresses.Focus();
            }
		}

        private void listBox_Addresses_DrawItem(object sender, DrawItemEventArgs e)
        {
            if( e.Index >= 0 )
            {
                RenderInListItem( e, (Address)listBox_Addresses.Items[ e.Index ] );
            }
        }

        private void RenderInListItem( DrawItemEventArgs e, Address address )
        {
            Rectangle bounds = e.Bounds;
            Color textColor = SystemColors.ControlText;

            // render background and select text color based on selected and focus states
            if( (e.State & DrawItemState.Selected) == DrawItemState.Selected )
            {
                if( (e.State & DrawItemState.Focus) == DrawItemState.Focus )
                {
                    e.DrawBackground();
                    e.DrawFocusRectangle();
                }
                else
                {
                    using( Brush b = new SolidBrush( SystemColors.InactiveCaption ) )
                    {
                        e.Graphics.FillRectangle(b, e.Bounds);
                    }
                }
                textColor = SystemColors.HighlightText;
            }
            else
            {
                e.DrawBackground();
            }

            using(SolidBrush textBrush = new SolidBrush( textColor ))
            {
                int top = 2;

                renderLine( e, address.Address1, textBrush, ref top );
                string addr2 = address.Address2;
                string postCode;

                if( addr2.Length > 0 )
                {
                    renderLine( e, address.Address2, textBrush, ref top );
                }
                if( address.ZipCode.PostalCode.Length == 9 )
                {
                    postCode = String.Format("{0}-{1}", address.ZipCode.ZipCodePrimary, address.ZipCode.ZipCodeExtended );
                }
                else
                {
                    postCode = address.ZipCode.PostalCode;
                }
                string cityLine = String.Format
                    ( "{0}, {1} {2}"
                    , address.City
                    , address.State
                    , postCode
                    );
                renderLine( e, cityLine, textBrush, ref top );                
            }
        }

        private void renderLine(DrawItemEventArgs e, string text, SolidBrush TextBrush, ref int top)
        {
            if( top+e.Font.Height < e.Bounds.Height )
            {
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    TextBrush,
                    e.Bounds.Left,
                    e.Bounds.Top + top);

                top += e.Font.Height;
            }
        }

        private void RunRules()
        {
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( GuarantorValidationResults ) );
            this.chheader = new System.Windows.Forms.ColumnHeader();
            this.chType = new System.Windows.Forms.ColumnHeader();
            this.grpName = new System.Windows.Forms.GroupBox();
            this.lblReturnedMiddleInitial = new System.Windows.Forms.Label();
            this.lblSentMiddleInitial = new System.Windows.Forms.Label();
            this.mtbRecordAsMiddleInitial = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblReturnedFirstname = new System.Windows.Forms.Label();
            this.lblSentFirstname = new System.Windows.Forms.Label();
            this.mtbRecordAsFirstname = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticComma = new System.Windows.Forms.Label();
            this.mtbRecordAsLastname = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticRecordAs = new System.Windows.Forms.Label();
            this.lblReturnedLastname = new System.Windows.Forms.Label();
            this.lblSentLastname = new System.Windows.Forms.Label();
            this.lblStaticNameReturned = new System.Windows.Forms.Label();
            this.lblStaticNameSent = new System.Windows.Forms.Label();
            this.btnCopyReturnedName = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCopySentName = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblAlerts = new System.Windows.Forms.Label();
            this.btnAlerts = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStaticMessages = new System.Windows.Forms.Label();
            this.grpSSN = new System.Windows.Forms.GroupBox();
            this.lblSSNReturned = new System.Windows.Forms.Label();
            this.lblSSNSent = new System.Windows.Forms.Label();
            this.mtbSSNRecordAs = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticSSNRecordAs = new System.Windows.Forms.Label();
            this.lblStaticSSNReturned = new System.Windows.Forms.Label();
            this.lblStaticSSNSent = new System.Windows.Forms.Label();
            this.btnCopySSNRecv = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCopySSNSent = new PatientAccess.UI.CommonControls.LoggingButton();
            this.grpPhone = new System.Windows.Forms.GroupBox();
            this.lblStaticPhoneRecordAs = new System.Windows.Forms.Label();
            this.lblPhoneReturned = new System.Windows.Forms.Label();
            this.lblPhoneSent = new System.Windows.Forms.Label();
            this.lblStaticPhoneRecv = new System.Windows.Forms.Label();
            this.lblStaticPhoneSent = new System.Windows.Forms.Label();
            this.btnCopyPhoneRecv = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCopyPhoneSent = new PatientAccess.UI.CommonControls.LoggingButton();
            this.grpAddress = new System.Windows.Forms.GroupBox();
            this.listBox_Addresses = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblListHeader = new System.Windows.Forms.Label();
            this.lblAddressRecordAs = new System.Windows.Forms.Label();
            this.lblStaticAddrRecordAs = new System.Windows.Forms.Label();
            this.lblStaticInstructions = new System.Windows.Forms.Label();
            this.btnValidate = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStaticReturned = new System.Windows.Forms.Label();
            this.lblMailingAddress = new System.Windows.Forms.Label();
            this.btnCopyAddress = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStaticSent = new System.Windows.Forms.Label();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.grpName.SuspendLayout();
            this.grpSSN.SuspendLayout();
            this.grpPhone.SuspendLayout();
            this.grpAddress.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chheader
            // 
            this.chheader.Text = "";
            this.chheader.Width = 0;
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 367;
            // 
            // grpName
            // 
            this.grpName.Controls.Add( this.lblReturnedMiddleInitial );
            this.grpName.Controls.Add( this.lblSentMiddleInitial );
            this.grpName.Controls.Add( this.mtbRecordAsMiddleInitial );
            this.grpName.Controls.Add( this.lblReturnedFirstname );
            this.grpName.Controls.Add( this.lblSentFirstname );
            this.grpName.Controls.Add( this.mtbRecordAsFirstname );
            this.grpName.Controls.Add( this.lblStaticComma );
            this.grpName.Controls.Add( this.mtbRecordAsLastname );
            this.grpName.Controls.Add( this.lblStaticRecordAs );
            this.grpName.Controls.Add( this.lblReturnedLastname );
            this.grpName.Controls.Add( this.lblSentLastname );
            this.grpName.Controls.Add( this.lblStaticNameReturned );
            this.grpName.Controls.Add( this.lblStaticNameSent );
            this.grpName.Controls.Add( this.btnCopyReturnedName );
            this.grpName.Controls.Add( this.btnCopySentName );
            this.grpName.Location = new System.Drawing.Point( 272, 5 );
            this.grpName.Name = "grpName";
            this.grpName.Size = new System.Drawing.Size( 544, 111 );
            this.grpName.TabIndex = 2;
            this.grpName.TabStop = false;
            this.grpName.Text = "Name";
            // 
            // lblReturnedMiddleInitial
            // 
            this.lblReturnedMiddleInitial.Location = new System.Drawing.Point( 513, 51 );
            this.lblReturnedMiddleInitial.Name = "lblReturnedMiddleInitial";
            this.lblReturnedMiddleInitial.Size = new System.Drawing.Size( 18, 15 );
            this.lblReturnedMiddleInitial.TabIndex = 0;
            // 
            // lblSentMiddleInitial
            // 
            this.lblSentMiddleInitial.Location = new System.Drawing.Point( 513, 23 );
            this.lblSentMiddleInitial.Name = "lblSentMiddleInitial";
            this.lblSentMiddleInitial.Size = new System.Drawing.Size( 18, 15 );
            this.lblSentMiddleInitial.TabIndex = 0;
            // 
            // mtbRecordAsMiddleInitial
            // 
            this.mtbRecordAsMiddleInitial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRecordAsMiddleInitial.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRecordAsMiddleInitial.KeyPressExpression = "^[a-zA-Z]*";
            this.mtbRecordAsMiddleInitial.Location = new System.Drawing.Point( 510, 77 );
            this.mtbRecordAsMiddleInitial.Mask = "";
            this.mtbRecordAsMiddleInitial.MaxLength = 1;
            this.mtbRecordAsMiddleInitial.Name = "mtbRecordAsMiddleInitial";
            this.mtbRecordAsMiddleInitial.Size = new System.Drawing.Size( 18, 20 );
            this.mtbRecordAsMiddleInitial.TabIndex = 5;
            this.mtbRecordAsMiddleInitial.ValidationExpression = "^[a-zA-Z]*";
            // 
            // lblReturnedFirstname
            // 
            this.lblReturnedFirstname.Location = new System.Drawing.Point( 376, 51 );
            this.lblReturnedFirstname.Name = "lblReturnedFirstname";
            this.lblReturnedFirstname.Size = new System.Drawing.Size( 125, 15 );
            this.lblReturnedFirstname.TabIndex = 0;
            // 
            // lblSentFirstname
            // 
            this.lblSentFirstname.Location = new System.Drawing.Point( 374, 23 );
            this.lblSentFirstname.Name = "lblSentFirstname";
            this.lblSentFirstname.Size = new System.Drawing.Size( 125, 15 );
            this.lblSentFirstname.TabIndex = 0;
            // 
            // mtbRecordAsFirstname
            // 
            this.mtbRecordAsFirstname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRecordAsFirstname.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRecordAsFirstname.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbRecordAsFirstname.Location = new System.Drawing.Point( 374, 77 );
            this.mtbRecordAsFirstname.Mask = "";
            this.mtbRecordAsFirstname.MaxLength = 15;
            this.mtbRecordAsFirstname.Name = "mtbRecordAsFirstname";
            this.mtbRecordAsFirstname.Size = new System.Drawing.Size( 125, 20 );
            this.mtbRecordAsFirstname.TabIndex = 4;
            this.mtbRecordAsFirstname.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbRecordAsFirstname.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRecordAsFirstname_Validating );
            // 
            // lblStaticComma
            // 
            this.lblStaticComma.Location = new System.Drawing.Point( 359, 76 );
            this.lblStaticComma.Name = "lblStaticComma";
            this.lblStaticComma.Size = new System.Drawing.Size( 10, 23 );
            this.lblStaticComma.TabIndex = 0;
            this.lblStaticComma.Text = ",";
            // 
            // mtbRecordAsLastname
            // 
            this.mtbRecordAsLastname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRecordAsLastname.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRecordAsLastname.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbRecordAsLastname.Location = new System.Drawing.Point( 147, 77 );
            this.mtbRecordAsLastname.Mask = "";
            this.mtbRecordAsLastname.MaxLength = 25;
            this.mtbRecordAsLastname.Name = "mtbRecordAsLastname";
            this.mtbRecordAsLastname.Size = new System.Drawing.Size( 200, 20 );
            this.mtbRecordAsLastname.TabIndex = 3;
            this.mtbRecordAsLastname.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\-]*$";
            this.mtbRecordAsLastname.Enter += new System.EventHandler( this.mtbRecordAsLastname_Enter );
            this.mtbRecordAsLastname.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRecordAsLastname_Validating );
            // 
            // lblStaticRecordAs
            // 
            this.lblStaticRecordAs.Location = new System.Drawing.Point( 84, 82 );
            this.lblStaticRecordAs.Name = "lblStaticRecordAs";
            this.lblStaticRecordAs.Size = new System.Drawing.Size( 59, 16 );
            this.lblStaticRecordAs.TabIndex = 0;
            this.lblStaticRecordAs.Text = "Record as:";
            // 
            // lblReturnedLastname
            // 
            this.lblReturnedLastname.Location = new System.Drawing.Point( 147, 51 );
            this.lblReturnedLastname.Name = "lblReturnedLastname";
            this.lblReturnedLastname.Size = new System.Drawing.Size( 200, 15 );
            this.lblReturnedLastname.TabIndex = 0;
            // 
            // lblSentLastname
            // 
            this.lblSentLastname.Location = new System.Drawing.Point( 147, 23 );
            this.lblSentLastname.Name = "lblSentLastname";
            this.lblSentLastname.Size = new System.Drawing.Size( 200, 15 );
            this.lblSentLastname.TabIndex = 0;
            // 
            // lblStaticNameReturned
            // 
            this.lblStaticNameReturned.Location = new System.Drawing.Point( 84, 51 );
            this.lblStaticNameReturned.Name = "lblStaticNameReturned";
            this.lblStaticNameReturned.Size = new System.Drawing.Size( 56, 15 );
            this.lblStaticNameReturned.TabIndex = 0;
            this.lblStaticNameReturned.Text = "Returned:";
            // 
            // lblStaticNameSent
            // 
            this.lblStaticNameSent.Location = new System.Drawing.Point( 84, 24 );
            this.lblStaticNameSent.Name = "lblStaticNameSent";
            this.lblStaticNameSent.Size = new System.Drawing.Size( 32, 16 );
            this.lblStaticNameSent.TabIndex = 0;
            this.lblStaticNameSent.Text = "Sent:";
            // 
            // btnCopyReturnedName
            // 
            this.btnCopyReturnedName.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopyReturnedName.Location = new System.Drawing.Point( 9, 47 );
            this.btnCopyReturnedName.Message = null;
            this.btnCopyReturnedName.Name = "btnCopyReturnedName";
            this.btnCopyReturnedName.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopyReturnedName.TabIndex = 2;
            this.btnCopyReturnedName.Text = "Copy";
            this.btnCopyReturnedName.UseVisualStyleBackColor = false;
            this.btnCopyReturnedName.Click += new System.EventHandler( this.btnCopyReturnedName_Click );
            // 
            // btnCopySentName
            // 
            this.btnCopySentName.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopySentName.Location = new System.Drawing.Point( 9, 20 );
            this.btnCopySentName.Message = null;
            this.btnCopySentName.Name = "btnCopySentName";
            this.btnCopySentName.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopySentName.TabIndex = 1;
            this.btnCopySentName.Text = "Copy";
            this.btnCopySentName.UseVisualStyleBackColor = false;
            this.btnCopySentName.Click += new System.EventHandler( this.btnCopySentName_Click );
            // 
            // lblAlerts
            // 
            this.lblAlerts.Location = new System.Drawing.Point( 4, 40 );
            this.lblAlerts.Name = "lblAlerts";
            this.lblAlerts.Size = new System.Drawing.Size( 204, 14 );
            this.lblAlerts.TabIndex = 0;
            this.lblAlerts.Text = "This validation generated no alerts.";
            // 
            // btnAlerts
            // 
            this.btnAlerts.BackColor = System.Drawing.SystemColors.Control;
            this.btnAlerts.Location = new System.Drawing.Point( 147, 8 );
            this.btnAlerts.Message = null;
            this.btnAlerts.Name = "btnAlerts";
            this.btnAlerts.Size = new System.Drawing.Size( 75, 23 );
            this.btnAlerts.TabIndex = 1;
            this.btnAlerts.Text = "A&lerts";
            this.btnAlerts.UseVisualStyleBackColor = false;
            this.btnAlerts.Click += new System.EventHandler( this.btnAlerts_Click );
            // 
            // lblStaticMessages
            // 
            this.lblStaticMessages.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.lblStaticMessages.Location = new System.Drawing.Point( 5, 11 );
            this.lblStaticMessages.Name = "lblStaticMessages";
            this.lblStaticMessages.Size = new System.Drawing.Size( 72, 15 );
            this.lblStaticMessages.TabIndex = 0;
            this.lblStaticMessages.Text = "Messages";
            // 
            // grpSSN
            // 
            this.grpSSN.Controls.Add( this.lblSSNReturned );
            this.grpSSN.Controls.Add( this.lblSSNSent );
            this.grpSSN.Controls.Add( this.mtbSSNRecordAs );
            this.grpSSN.Controls.Add( this.lblStaticSSNRecordAs );
            this.grpSSN.Controls.Add( this.lblStaticSSNReturned );
            this.grpSSN.Controls.Add( this.lblStaticSSNSent );
            this.grpSSN.Controls.Add( this.btnCopySSNRecv );
            this.grpSSN.Controls.Add( this.btnCopySSNSent );
            this.grpSSN.Location = new System.Drawing.Point( 8, 125 );
            this.grpSSN.Name = "grpSSN";
            this.grpSSN.Size = new System.Drawing.Size( 235, 110 );
            this.grpSSN.TabIndex = 3;
            this.grpSSN.TabStop = false;
            this.grpSSN.Text = "SSN";
            // 
            // lblSSNReturned
            // 
            this.lblSSNReturned.Location = new System.Drawing.Point( 153, 53 );
            this.lblSSNReturned.Name = "lblSSNReturned";
            this.lblSSNReturned.Size = new System.Drawing.Size( 70, 15 );
            this.lblSSNReturned.TabIndex = 0;
            // 
            // lblSSNSent
            // 
            this.lblSSNSent.Location = new System.Drawing.Point( 153, 26 );
            this.lblSSNSent.Name = "lblSSNSent";
            this.lblSSNSent.Size = new System.Drawing.Size( 70, 15 );
            this.lblSSNSent.TabIndex = 0;
            // 
            // mtbSSNRecordAs
            // 
            this.mtbSSNRecordAs.Enabled = false;
            this.mtbSSNRecordAs.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbSSNRecordAs.KeyPressExpression = "^\\d*";
            this.mtbSSNRecordAs.Location = new System.Drawing.Point( 153, 79 );
            this.mtbSSNRecordAs.Mask = "   -  -";
            this.mtbSSNRecordAs.MaxLength = 11;
            this.mtbSSNRecordAs.Name = "mtbSSNRecordAs";
            this.mtbSSNRecordAs.Size = new System.Drawing.Size( 70, 20 );
            this.mtbSSNRecordAs.TabIndex = 2;
            this.mtbSSNRecordAs.ValidationExpression = "^\\d*";
            this.mtbSSNRecordAs.Enter += new System.EventHandler( this.mtbSSNRecordAs_Enter );
            this.mtbSSNRecordAs.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSSNRecordAs_Validating );
            // 
            // lblStaticSSNRecordAs
            // 
            this.lblStaticSSNRecordAs.Location = new System.Drawing.Point( 83, 83 );
            this.lblStaticSSNRecordAs.Name = "lblStaticSSNRecordAs";
            this.lblStaticSSNRecordAs.Size = new System.Drawing.Size( 59, 14 );
            this.lblStaticSSNRecordAs.TabIndex = 0;
            this.lblStaticSSNRecordAs.Text = "Record as:";
            // 
            // lblStaticSSNReturned
            // 
            this.lblStaticSSNReturned.Location = new System.Drawing.Point( 83, 53 );
            this.lblStaticSSNReturned.Name = "lblStaticSSNReturned";
            this.lblStaticSSNReturned.Size = new System.Drawing.Size( 56, 15 );
            this.lblStaticSSNReturned.TabIndex = 0;
            this.lblStaticSSNReturned.Text = "Returned:";
            // 
            // lblStaticSSNSent
            // 
            this.lblStaticSSNSent.Location = new System.Drawing.Point( 83, 26 );
            this.lblStaticSSNSent.Name = "lblStaticSSNSent";
            this.lblStaticSSNSent.Size = new System.Drawing.Size( 32, 12 );
            this.lblStaticSSNSent.TabIndex = 0;
            this.lblStaticSSNSent.Text = "Sent:";
            // 
            // btnCopySSNRecv
            // 
            this.btnCopySSNRecv.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopySSNRecv.Location = new System.Drawing.Point( 9, 48 );
            this.btnCopySSNRecv.Message = null;
            this.btnCopySSNRecv.Name = "btnCopySSNRecv";
            this.btnCopySSNRecv.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopySSNRecv.TabIndex = 1;
            this.btnCopySSNRecv.Text = "Copy";
            this.btnCopySSNRecv.UseVisualStyleBackColor = false;
            this.btnCopySSNRecv.Click += new System.EventHandler( this.btnCopySSNRecv_Click );
            // 
            // btnCopySSNSent
            // 
            this.btnCopySSNSent.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopySSNSent.Location = new System.Drawing.Point( 9, 21 );
            this.btnCopySSNSent.Message = null;
            this.btnCopySSNSent.Name = "btnCopySSNSent";
            this.btnCopySSNSent.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopySSNSent.TabIndex = 0;
            this.btnCopySSNSent.Text = "Copy";
            this.btnCopySSNSent.UseVisualStyleBackColor = false;
            this.btnCopySSNSent.Click += new System.EventHandler( this.btnCopySSNSent_Click );
            // 
            // grpPhone
            // 
            this.grpPhone.Controls.Add( this.phoneNumberControl );
            this.grpPhone.Controls.Add( this.lblStaticPhoneRecordAs );
            this.grpPhone.Controls.Add( this.lblPhoneReturned );
            this.grpPhone.Controls.Add( this.lblPhoneSent );
            this.grpPhone.Controls.Add( this.lblStaticPhoneRecv );
            this.grpPhone.Controls.Add( this.lblStaticPhoneSent );
            this.grpPhone.Controls.Add( this.btnCopyPhoneRecv );
            this.grpPhone.Controls.Add( this.btnCopyPhoneSent );
            this.grpPhone.Location = new System.Drawing.Point( 272, 125 );
            this.grpPhone.Name = "grpPhone";
            this.grpPhone.Size = new System.Drawing.Size( 271, 110 );
            this.grpPhone.TabIndex = 4;
            this.grpPhone.TabStop = false;
            this.grpPhone.Text = "Phone";
            // 
            // lblStaticPhoneRecordAs
            // 
            this.lblStaticPhoneRecordAs.Location = new System.Drawing.Point( 82, 84 );
            this.lblStaticPhoneRecordAs.Name = "lblStaticPhoneRecordAs";
            this.lblStaticPhoneRecordAs.Size = new System.Drawing.Size( 59, 17 );
            this.lblStaticPhoneRecordAs.TabIndex = 0;
            this.lblStaticPhoneRecordAs.Text = "Record as:";
            // 
            // lblPhoneReturned
            // 
            this.lblPhoneReturned.Location = new System.Drawing.Point( 153, 53 );
            this.lblPhoneReturned.Name = "lblPhoneReturned";
            this.lblPhoneReturned.Size = new System.Drawing.Size( 100, 15 );
            this.lblPhoneReturned.TabIndex = 0;
            // 
            // lblPhoneSent
            // 
            this.lblPhoneSent.Location = new System.Drawing.Point( 153, 24 );
            this.lblPhoneSent.Name = "lblPhoneSent";
            this.lblPhoneSent.Size = new System.Drawing.Size( 100, 15 );
            this.lblPhoneSent.TabIndex = 0;
            // 
            // lblStaticPhoneRecv
            // 
            this.lblStaticPhoneRecv.Location = new System.Drawing.Point( 82, 54 );
            this.lblStaticPhoneRecv.Name = "lblStaticPhoneRecv";
            this.lblStaticPhoneRecv.Size = new System.Drawing.Size( 56, 17 );
            this.lblStaticPhoneRecv.TabIndex = 0;
            this.lblStaticPhoneRecv.Text = "Returned:";
            // 
            // lblStaticPhoneSent
            // 
            this.lblStaticPhoneSent.Location = new System.Drawing.Point( 82, 26 );
            this.lblStaticPhoneSent.Name = "lblStaticPhoneSent";
            this.lblStaticPhoneSent.Size = new System.Drawing.Size( 32, 18 );
            this.lblStaticPhoneSent.TabIndex = 0;
            this.lblStaticPhoneSent.Text = "Sent:";
            // 
            // btnCopyPhoneRecv
            // 
            this.btnCopyPhoneRecv.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopyPhoneRecv.Location = new System.Drawing.Point( 9, 48 );
            this.btnCopyPhoneRecv.Message = null;
            this.btnCopyPhoneRecv.Name = "btnCopyPhoneRecv";
            this.btnCopyPhoneRecv.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopyPhoneRecv.TabIndex = 1;
            this.btnCopyPhoneRecv.Text = "Copy";
            this.btnCopyPhoneRecv.UseVisualStyleBackColor = false;
            this.btnCopyPhoneRecv.Click += new System.EventHandler( this.btnCopyPhoneRecv_Click );
            // 
            // btnCopyPhoneSent
            // 
            this.btnCopyPhoneSent.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopyPhoneSent.Location = new System.Drawing.Point( 9, 21 );
            this.btnCopyPhoneSent.Message = null;
            this.btnCopyPhoneSent.Name = "btnCopyPhoneSent";
            this.btnCopyPhoneSent.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopyPhoneSent.TabIndex = 0;
            this.btnCopyPhoneSent.Text = "Copy";
            this.btnCopyPhoneSent.UseVisualStyleBackColor = false;
            this.btnCopyPhoneSent.Click += new System.EventHandler( this.btnCopyPhoneSent_Click );
            // 
            // grpAddress
            // 
            this.grpAddress.Controls.Add( this.btnCopyAddress );
            this.grpAddress.Controls.Add( this.listBox_Addresses );
            this.grpAddress.Controls.Add( this.label1 );
            this.grpAddress.Controls.Add( this.lblListHeader );
            this.grpAddress.Controls.Add( this.lblAddressRecordAs );
            this.grpAddress.Controls.Add( this.lblStaticAddrRecordAs );
            this.grpAddress.Controls.Add( this.lblStaticInstructions );
            this.grpAddress.Controls.Add( this.btnValidate );
            this.grpAddress.Controls.Add( this.lblStaticReturned );
            this.grpAddress.Controls.Add( this.lblMailingAddress );
            this.grpAddress.Controls.Add( this.lblStaticSent );
            this.grpAddress.Location = new System.Drawing.Point( 8, 243 );
            this.grpAddress.Name = "grpAddress";
            this.grpAddress.Size = new System.Drawing.Size( 864, 242 );
            this.grpAddress.TabIndex = 5;
            this.grpAddress.TabStop = false;
            this.grpAddress.Text = "Address";
            // 
            // listBox_Addresses
            // 
            this.listBox_Addresses.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox_Addresses.IntegralHeight = false;
            this.listBox_Addresses.ItemHeight = 45;
            this.listBox_Addresses.Location = new System.Drawing.Point( 264, 97 );
            this.listBox_Addresses.Name = "listBox_Addresses";
            this.listBox_Addresses.ScrollAlwaysVisible = true;
            this.listBox_Addresses.Size = new System.Drawing.Size( 367, 139 );
            this.listBox_Addresses.TabIndex = 0;
            this.listBox_Addresses.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.listBox_Addresses_DrawItem );
            this.listBox_Addresses.SelectedIndexChanged += new System.EventHandler( this.listBox_Addresses_SelectedIndexChanged );
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point( 274, 80 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 181, 14 );
            this.label1.TabIndex = 5;
            this.label1.Text = "Addresses Returned from Agency";
            // 
            // lblListHeader
            // 
            this.lblListHeader.BackColor = System.Drawing.SystemColors.Control;
            this.lblListHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblListHeader.Location = new System.Drawing.Point( 264, 74 );
            this.lblListHeader.Name = "lblListHeader";
            this.lblListHeader.Size = new System.Drawing.Size( 367, 23 );
            this.lblListHeader.TabIndex = 4;
            this.lblListHeader.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblAddressRecordAs
            // 
            this.lblAddressRecordAs.Location = new System.Drawing.Point( 656, 80 );
            this.lblAddressRecordAs.Name = "lblAddressRecordAs";
            this.lblAddressRecordAs.Size = new System.Drawing.Size( 193, 102 );
            this.lblAddressRecordAs.TabIndex = 0;
            // 
            // lblStaticAddrRecordAs
            // 
            this.lblStaticAddrRecordAs.Location = new System.Drawing.Point( 656, 24 );
            this.lblStaticAddrRecordAs.Name = "lblStaticAddrRecordAs";
            this.lblStaticAddrRecordAs.Size = new System.Drawing.Size( 64, 16 );
            this.lblStaticAddrRecordAs.TabIndex = 0;
            this.lblStaticAddrRecordAs.Text = "Record as:";
            // 
            // lblStaticInstructions
            // 
            this.lblStaticInstructions.Location = new System.Drawing.Point( 366, 48 );
            this.lblStaticInstructions.Name = "lblStaticInstructions";
            this.lblStaticInstructions.Size = new System.Drawing.Size( 260, 18 );
            this.lblStaticInstructions.TabIndex = 0;
            this.lblStaticInstructions.Text = "To copy an address, select it, then click Copy.";
            // 
            // btnValidate
            // 
            this.btnValidate.BackColor = System.Drawing.SystemColors.Control;
            this.btnValidate.Enabled = false;
            this.btnValidate.Location = new System.Drawing.Point( 264, 43 );
            this.btnValidate.Message = null;
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size( 95, 22 );
            this.btnValidate.TabIndex = 1;
            this.btnValidate.Text = "Validate && Copy";
            this.btnValidate.UseVisualStyleBackColor = false;
            this.btnValidate.Click += new System.EventHandler( this.btnValidate_Click );
            // 
            // lblStaticReturned
            // 
            this.lblStaticReturned.Location = new System.Drawing.Point( 264, 24 );
            this.lblStaticReturned.Name = "lblStaticReturned";
            this.lblStaticReturned.Size = new System.Drawing.Size( 56, 16 );
            this.lblStaticReturned.TabIndex = 0;
            this.lblStaticReturned.Text = "Returned:";
            // 
            // lblMailingAddress
            // 
            this.lblMailingAddress.Location = new System.Drawing.Point( 12, 80 );
            this.lblMailingAddress.Name = "lblMailingAddress";
            this.lblMailingAddress.Size = new System.Drawing.Size( 210, 100 );
            this.lblMailingAddress.TabIndex = 0;
            // 
            // btnCopyAddress
            // 
            this.btnCopyAddress.BackColor = System.Drawing.SystemColors.Control;
            this.btnCopyAddress.Location = new System.Drawing.Point( 9, 44 );
            this.btnCopyAddress.Message = null;
            this.btnCopyAddress.Name = "btnCopyAddress";
            this.btnCopyAddress.Size = new System.Drawing.Size( 50, 20 );
            this.btnCopyAddress.TabIndex = 0;
            this.btnCopyAddress.Text = "Copy";
            this.btnCopyAddress.UseVisualStyleBackColor = false;
            this.btnCopyAddress.Click += new System.EventHandler( this.btnCopyAddress_Click );
            // 
            // lblStaticSent
            // 
            this.lblStaticSent.Location = new System.Drawing.Point( 9, 21 );
            this.lblStaticSent.Name = "lblStaticSent";
            this.lblStaticSent.Size = new System.Drawing.Size( 32, 13 );
            this.lblStaticSent.TabIndex = 0;
            this.lblStaticSent.Text = "Sent:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb( ( (int)( ( (byte)( 255 ) ) ) ), ( (int)( ( (byte)( 224 ) ) ) ), ( (int)( ( (byte)( 142 ) ) ) ) );
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add( this.lblAlerts );
            this.panel1.Controls.Add( this.btnAlerts );
            this.panel1.Controls.Add( this.lblStaticMessages );
            this.panel1.Location = new System.Drawing.Point( 10, 10 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 232, 105 );
            this.panel1.TabIndex = 6;
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 147, 77 );
            this.phoneNumberControl.Model = ( (PatientAccess.Domain.Parties.PhoneNumber)( resources.GetObject( "phoneNumberControl1.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 2;
            this.phoneNumberControl.Validating += new CancelEventHandler( phoneNumberControl_Validating );
            // 
            // GuarantorValidationResults
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.grpAddress );
            this.Controls.Add( this.grpPhone );
            this.Controls.Add( this.grpSSN );
            this.Controls.Add( this.grpName );
            this.Name = "GuarantorValidationResults";
            this.Size = new System.Drawing.Size( 880, 495 );
            this.grpName.ResumeLayout( false );
            this.grpName.PerformLayout();
            this.grpSSN.ResumeLayout( false );
            this.grpSSN.PerformLayout();
            this.grpPhone.ResumeLayout( false );
            this.grpAddress.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.ResumeLayout( false );

		}

        #endregion

        #region Construction and Finalization
        public GuarantorValidationResults()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            base.EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container                 components = null;
        private LoggingButton                     btnAlerts;
        private LoggingButton                     btnCopySentName;
        private LoggingButton                     btnCopyReturnedName;
        private LoggingButton                     btnCopySSNSent;
        private LoggingButton                     btnCopySSNRecv;
        private LoggingButton                     btnCopyPhoneSent;
        private LoggingButton                     btnCopyPhoneRecv;
        private LoggingButton                     btnValidate;

        private ColumnHeader               columnHeader;
        private GroupBox                   grpAddress;
        private GroupBox                   grpName;
        private GroupBox                   grpSSN;
        private GroupBox                   grpPhone;

        private Label                      lblAlerts;
        private Label                      lblSentLastname;
        private Label                      lblReturnedLastname;
        private Label                      lblStaticRecordAs;
        private Label                      lblStaticComma;
        private Label                      lblSentFirstname;
        private Label                      lblSentMiddleInitial;
        private Label                      lblReturnedMiddleInitial;
        private Label                      lblStaticSSNSent;
        private Label                      lblStaticSSNReturned;
        private Label                      lblStaticSSNRecordAs;
        private Label                      lblSSNSent;
        private Label                      lblSSNReturned;
        private Label                      lblStaticNameSent;
        private Label                      lblStaticNameReturned;
        private Label                      lblStaticPhoneSent;
        private Label                      lblStaticPhoneRecv;
        private Label                      lblPhoneSent;
        private Label                      lblPhoneReturned;
        private Label                      lblStaticPhoneRecordAs;
        private Label                      lblReturnedFirstname;

        private MaskedEditTextBox        mtbRecordAsLastname;
        private MaskedEditTextBox        mtbRecordAsMiddleInitial;
        private MaskedEditTextBox mtbSSNRecordAs;
        private MaskedEditTextBox        mtbRecordAsFirstname;

        private Label                      lblStaticMessages;
        private Label                      lblStaticSent;
        private Label                      lblMailingAddress;
        private Label                      lblStaticReturned;
        private Label                      lblStaticInstructions;
        private Label                      lblStaticAddrRecordAs;

        private RuleEngine                                      i_RuleEngine;
        private StringCollection                                stringCollection = new StringCollection();
		private LoggingButton btnCopyAddress;
		private Label lblAddressRecordAs;
        private Label lblListHeader;
        private Panel panel1;
        private Label label1;
		//private Address											i_AddressSelected = null;
		private Address											i_GuarantorAddress = null;
		private ContactPoint									i_SentContactPoint = null;
		private PhoneNumber										i_PhoneNumber = null;
		private Name											i_Name = null;
        private ListBox                    listBox_Addresses;
		private SocialSecurityNumber							i_SSN = null;

        private ColumnHeader               chType;
        private ColumnHeader               chheader;

        #endregion
        private PhoneNumberControl phoneNumberControl;

        #region Constants
		private const string ALERTS_MSG = "This validation generated ";
		#endregion
	}
}
