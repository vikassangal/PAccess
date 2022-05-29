using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for FormAddressVerification.
    /// </summary>
    [Serializable]
    public class FormAddressWithStreet2Verification : TimeOutFormView 
    {
        #region Event Handlers
		private void FormAddressVerification_Load(object sender, EventArgs e)
		{
			this.UpdateView();
		}

        private void addressEntryView1_VerificationButtonEnabled(object sender, EventArgs e)
		{
			LooseArgs args = (LooseArgs) e;
			YesNoFlag yesNo = args.Context as YesNoFlag;

			if( yesNo.Code == YesNoFlag.CODE_YES )
			{
				this.AcceptButton = this.addressEntryView1.btnVerify;
			}
			else
			{
				this.AcceptButton = this.btnOk;
			}
		}

		private void addressEntryView1_SetFormToOriginalSize(object sender, EventArgs e)
		{
			this.Height = 260;
		}

		private void addressEntryView1_ResetMatchingAddresses(object sender, EventArgs e)
		{
			this.cbxIgnore.Checked = false;
			this.lblMessage.Text = String.Empty;
			this.matchingAddressView1.ResetMatchingAddressView();
			this.Height = 460;
		}

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            Address addressOriginal = this.addressEntryView1.OriginalAddress;

            PopulateAddress( addressOriginal );
        }

        private void cbxIgnore_Click( object sender, EventArgs e )
        {
            if( this.cbxIgnore.Checked )
            {   
				this.addressEntryView1.IgnoreChecked = true;
				this.addressEntryView1.AllFieldsRequiredRules();
                this.matchingAddressView1.Enabled = false;
                
				this.matchingAddressView1.Ignoring = true;
				this.matchingAddressView1.IgnoreMatchingAddresses();

                if( !this.addressEntryView1.AllFieldsValid() )
                {
                    this.AcceptButton = this.addressEntryView1.btnVerify;
					this.btnOk.Enabled = false;
                }
                else
                {
                    this.PopulateAddress( this.addressEntryView1.VerificationAddress() );
                    this.btnOk.Enabled = true;
					this.AcceptButton = this.btnOk;
                }
            }
            else
            {
				this.addressEntryView1.IgnoreChecked = false;
				this.addressEntryView1.OnlyStreetAndCountryRequiredRule();
                this.matchingAddressView1.Ignoring = false;
				this.matchingAddressView1.Enabled = true;
                this.matchingAddressView1.PopulateAddressListBox();

                if( !this.matchingAddressView1.AddressListPopulated() )
                {
                    this.btnOk.Enabled = false;
					this.AcceptButton = this.addressEntryView1.btnVerify;
                }
                else
                {
					this.btnOk.Enabled = true;
					this.matchingAddressView1.SelectFirstListBoxItem();
                    this.matchingAddressView1.Enabled = true;
                }

				this.matchingAddressView1.Refresh();
				this.matchingAddressView1.SetTabOrder();
            }
			
			//this.cbxIgnore.Focus();
			this.isPanel3Extended = false;
			this.Height = 460;
			this.panel3.Visible = false;
			this.btnOk.Location = new Point( 319,399 );
            this.btnCancelEdit.Location = new Point(399, 399);
        }

        private void OnDataModification( object sender, EventArgs e )
        {
            if( this.cbxIgnore.Checked )
            {
				//if( !this.addressEntryView1.AllFieldsValid() )
				if( !this.addressEntryView1.AllRequiredFieldsValid() )
				{
					this.btnOk.Enabled = false;
					this.AcceptButton = this.addressEntryView1.btnVerify;
				}
				else
				{
					if( !isPanel3Extended )
					{
						this.PopulateAddress( this.addressEntryView1.VerificationAddress() );
					}

//					if( ( addressValidation != null &&
//						( addressValidation.City.Length > 0 && addressValidation.City.Length <= 15 ) &&
//						( addressValidation.Address1.Length > 0 && addressValidation.Address1.Length <= 25 ) ) )
				    if( i_AddressSelected != null && 
						( !isPanel3Extended || ( editAddressYesNo.Code == YesNoFlag.CODE_YES ) ) )
					{
						this.btnOk.Enabled = true;
						this.AcceptButton = this.btnOk;
					}
				}
            }
        }

        private void OnNonUSAddress( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            Address addressSearch = args.Context as Address;

            PopulateAddress( addressSearch );
            this.DialogResult = DialogResult.OK;
        }

        private void OnAddressEntryCancelled( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            Address addressOriginal = args.Context as Address;

            PopulateAddress( addressOriginal );
        }

        private void OnAddressVerification( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            Address addressSearch = args.Context as Address;
            ICollection addresses = null;

            try
            {
                IDataValidationBroker dvBroker = BrokerFactory.BrokerOfType<IDataValidationBroker>();
                
                User appUser = User.GetCurrent();
                AddressValidationResult validateResult = 
                    ( AddressValidationResult )dvBroker.ValidAddressesMatching( addressSearch, appUser.SecurityUser.UPN, 
                        appUser.Facility.Code );
                addresses = (ICollection)validateResult.Addresses;
        
                if( validateResult.ReturnMessage != null && 
					( ( validateResult.ReturnMessage.ToUpper().IndexOf( SUCCESSFUL_CASS ) == -1 ) || 
					  ( validateResult.ReturnMessage.ToUpper().IndexOf( SUCCESSFUL_CASS ) != -1 && validateResult.Addresses.Count > 1 ) ) )
                {
                    this.lblMessage.Text = validateResult.ReturnMessage;
                }
                else
                {
                    this.lblMessage.Text = string.Empty;
                }
            }
            catch( Exception ex )
           {
               if( ex.ToString().Contains( UIErrorMessages.EDV_UNAVAILABLE_ERROR ))
               {
                   this.lblMessage.Text = UIErrorMessages.EDV_UNAVAILABLE_MESSAGE;
               }
               else
               {
                    this.lblMessage.Text = UIErrorMessages.DATAVALIDATION_NOT_AVAILABLE;
               }

                //this.lblMessage.Text = ex.Message;
				this.Cursor = Cursors.Default;
            }

            this.matchingAddressView1.Model_Addresses = addresses;
			this.matchingAddressView1.Ignoring = false;
            this.matchingAddressView1.UpdateView();
            this.matchingAddressView1.SetTabOrder();

            if( ( addresses == null || addresses.Count < 1 ) && !this.cbxIgnore.Checked)
            {
                this.btnOk.Enabled = false;
            }

            this.matchingAddressView1.Enabled = true;
            this.cbxIgnore.Checked = false;

                this.Height = 460;
                this.panel3.Visible = false;
                this.btnOk.Location = new Point( 319,399 );
                this.btnCancelEdit.Location = new Point( 399,399 );
				this.isPanel3Extended = false;

                this.SetTabOrderForMatchingAddresses();

			if( addresses != null &&
				addresses.Count > 0 )
			{
				this.btnOk.Enabled = true;
				this.AcceptButton = this.btnOk;
			}
        }

        private void OnAddressSelected( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            Address matchingAddressSelected = args.Context as Address;

            ValidateSelectedAddress( matchingAddressSelected );
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			if( this.cbxIgnore.Checked && 
				!this.isPanel3Extended )
			{
				this.PopulateAddress( this.addressEntryView1.VerificationAddress() );
			}

            ValidateSelectedAddress( i_AddressSelected );
            if (isPanel3Extended)
            {
                panel3.Visible = true;
                this.Height = 630;
            }
			if( !this.btnOk.Enabled )
			{
				this.DialogResult = DialogResult.None;
			}
        }

		private void editAddressView1_EnableOKButton(object sender, EventArgs e)
		{
			LooseArgs args = (LooseArgs)e;
			editAddressYesNo = args.Context as YesNoFlag;

			if( editAddressYesNo.Code == YesNoFlag.CODE_YES )
			{
				addressValidation = this.editAddressView1.TruncatedAddress();
				this.PopulateAddress(addressValidation);
				this.btnOk.Enabled = true;
				this.AcceptButton = this.btnOk;
			}
			else
			{
				this.btnOk.Enabled = false;
				this.AcceptButton = this.addressEntryView1.btnVerify;
			}
		}
        #endregion

        #region Methods
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

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if( Model_Address != null )
            {
                addressEntryView1.Model = Model_Address;
            }
            addressEntryView1.UpdateView();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {

        }
        #endregion

        #region Properties

        protected virtual AddressEntryWithStreet2View TheAddressView
        {
            get
            {
                return new AddressEntryWithStreet2View(this.Facility, this.RuleEngine);
            }
        }

        public RuleEngine RuleEngine { get; set; }

        public Facility Facility { get; set; }

        private Address Model_Address
        {
            get
            {
                return (Address)this.Model;
            }
           
        }

        public int cityLength
        {
            get
            {
                return i_CityLength;
            }
            set
            {
                i_CityLength = value;
            }
        }

        public int streetLength
        {
            get
            {
                return i_StreetLength;
            }
            set
            {
                i_StreetLength = value;
            }
        }

		public Account anAccount
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
        #endregion

        #region Private Methods
		private void SetTabOrderForMatchingAddresses()
		{
			this.matchingAddressView1.TabIndex = 2;
			this.matchingAddressView1.TabStop = true;
			this.cbxIgnore.TabIndex = 3;
			this.cbxIgnore.TabStop = true;
			this.editAddressView1.Enabled = false;
			this.editAddressView1.TabIndex = 0;
			this.editAddressView1.TabStop = false;
			this.btnOk.TabIndex = 4;
			this.btnCancelEdit.TabIndex = 5;
			this.btnCancelEdit.Enabled = true;
			this.CancelButton = this.btnCancelEdit;
		}

		private void SetTabOrderForEditAddress()
		{
			//this.matchingAddressView1.TabIndex = 2;
			//this.cbxIgnore.TabIndex = 3;
			this.editAddressView1.Enabled = true;
			this.editAddressView1.TabIndex = 4;
			this.editAddressView1.TabStop = true;
			this.btnOk.TabIndex = 5;
			this.btnCancelEdit.TabIndex = 6;
		}

        private bool AddressLengthIsValid( Address address )
        {
            if( address != null &&
                address.Address1.Length + address.Address2.Length > 75 || 
                address.City.Length > 15 )
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private void ValidateSelectedAddress( Address selectedAddress)
        {   
            if( selectedAddress == null )
            {
                return;
            }

            if( !AddressLengthIsValid(selectedAddress) )
            {
                this.editAddressView1.Model = selectedAddress;
                this.editAddressView1.UpdateView();

                //Resize window to full view.
                this.btnOk.Location = new Point( 319,571 );
				this.btnCancelEdit.Location = new Point( 399,571 );
                this.AcceptButton = this.addressEntryView1.btnVerify;
				this.PopulateAddress(selectedAddress);
				this.SetTabOrderForEditAddress();
				isPanel3Extended = true;
				this.btnOk.Enabled = false;
				this.editAddressView1.SetCursorPosition();
            }
            else
            {
                this.Height = 460;
                this.panel3.Visible = false;
                this.btnOk.Location = new Point( 319,399 );
                this.btnCancelEdit.Location = new Point( 399,399 );
                this.btnOk.Enabled = true;
				this.AcceptButton = this.btnOk;
                this.PopulateAddress(selectedAddress);
				isPanel3Extended = false;
				this.SetTabOrderForMatchingAddresses();
            }

            this.Refresh();
        }

        private void PopulateAddress( Address address)
        {
            if( address == null )
            {
                return;
            }

            i_AddressSelected.Country = address.Country;
            i_AddressSelected.Address1 = address.Address1;
            i_AddressSelected.Address2 = address.Address2;
            i_AddressSelected.ZipCode.PostalCode = address.ZipCode.PostalCode;
            i_AddressSelected.City = address.City;
            i_AddressSelected.State = address.State;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
		private void InitializeComponent()
		{
            this.panel1 = new System.Windows.Forms.Panel();
            this.matchingAddressView1 = new PatientAccess.UI.AddressViews.MatchingAddressView();
            this.lblMatching = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cbxIgnore = new System.Windows.Forms.CheckBox();
            this.editAddressView1 = new PatientAccess.UI.AddressViews.EditAddressView();
            this.btnOk = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnCancelEdit = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // addressEntryView1
            // 
            this.addressEntryView1.BackColor = Color.FromArgb(((Byte)(209)), ((Byte)(228)), ((Byte)(243)));
            this.addressEntryView1.IgnoreChecked = false;
            this.addressEntryView1.Location = new Point(0, 0);
            this.addressEntryView1.Model = null;
            this.addressEntryView1.Name = "addressEntryView1";
            this.addressEntryView1.Size = new Size(466, 212);
            this.addressEntryView1.TabIndex = 1;
            this.addressEntryView1.ResetMatchingAddresses += new EventHandler(this.addressEntryView1_ResetMatchingAddresses);
            this.addressEntryView1.DataModified += new EventHandler(this.OnDataModification);
            this.addressEntryView1.SetFormToOriginalSize += new EventHandler(this.addressEntryView1_SetFormToOriginalSize);
            this.addressEntryView1.AddressEntryCancelled += new EventHandler(this.OnAddressEntryCancelled);
            this.addressEntryView1.VerificationButtonEnabled += new EventHandler(this.addressEntryView1_VerificationButtonEnabled);
            this.addressEntryView1.VerifyAddress += new EventHandler(this.OnAddressVerification);
            this.addressEntryView1.NonUSAddress += new EventHandler(this.OnNonUSAddress);
            // 
            // panel1
            // 
            this.panel1.BackColor = Color.White;
            this.panel1.Controls.Add(this.addressEntryView1);
            this.panel1.Location = new Point(6, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(467, 215);
            this.panel1.TabIndex = 0;
            // 
            // matchingAddressView1
            // 
            this.matchingAddressView1.CoverMessage = "No addresses match your entry.  Use the address entered (if complete), or enter a" +
            " new address to verify.";
            this.matchingAddressView1.CoverPadding = 30;
            this.matchingAddressView1.Ignoring = false;
            this.matchingAddressView1.Location = new Point(7, 37);
            this.matchingAddressView1.Model = null;
            this.matchingAddressView1.Name = "matchingAddressView1";
            this.matchingAddressView1.ShowCover = true;
            this.matchingAddressView1.Size = new Size(452, 97);
            this.matchingAddressView1.TabIndex = 1;
            this.matchingAddressView1.AddressSelected += new EventHandler(this.OnAddressSelected);
            // 
            // lblMatching
            // 
            this.lblMatching.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((Byte)(0)));
            this.lblMatching.Location = new Point(6, 5);
            this.lblMatching.Name = "lblMatching";
            this.lblMatching.Size = new Size(156, 14);
            this.lblMatching.TabIndex = 0;
            this.lblMatching.Text = "Matching USPS Addresses";
            // 
            // lblMessage
            // 
            this.lblMessage.ForeColor = Color.FromArgb(((Byte)(204)), ((Byte)(0)), ((Byte)(0)));
            this.lblMessage.Location = new Point(16, 22);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new Size(437, 13);
            this.lblMessage.TabIndex = 0;
            // 
            // cbxIgnore
            // 
            this.cbxIgnore.Location = new Point(7, 137);
            this.cbxIgnore.Name = "cbxIgnore";
            this.cbxIgnore.Size = new Size(425, 16);
            this.cbxIgnore.TabIndex = 2;
            this.cbxIgnore.Text = "Ignore results and use the address entered (all fields required)";
            this.cbxIgnore.Click += new EventHandler(this.cbxIgnore_Click);
            // 
            // editAddressView1
            // 
            this.editAddressView1.BackColor = Color.White;
            this.editAddressView1.Dock = DockStyle.Fill;
            this.editAddressView1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            this.editAddressView1.Location = new Point(0, 0);
            this.editAddressView1.Model = null;
            this.editAddressView1.Name = "editAddressView1";
            this.editAddressView1.Size = new Size(465, 164);
            this.editAddressView1.TabIndex = 1;
            this.editAddressView1.validCharacterLimit = false;
            this.editAddressView1.EnableOKButton += new EventHandler(this.editAddressView1_EnableOKButton);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = SystemColors.Control;
            
            this.btnOk.Location = new Point(314, 568);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(75, 21);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new EventHandler(this.btnOk_Click);
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.BackColor = SystemColors.Control;

            this.btnCancelEdit.Location = new Point(394, 568);
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new Size(75, 21);
            this.btnCancelEdit.TabIndex = 4;
            this.btnCancelEdit.Text = "Cancel";
            this.btnCancelEdit.Click += new EventHandler(this.btnCancelEdit_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = Color.White;
            this.panel2.BorderStyle = BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblMatching);
            this.panel2.Controls.Add(this.lblMessage);
            this.panel2.Controls.Add(this.cbxIgnore);
            this.panel2.Controls.Add(this.matchingAddressView1);
            this.panel2.Location = new Point(6, 233);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(467, 158);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.BackColor = Color.White;
            this.panel3.BorderStyle = BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.editAddressView1);
            this.panel3.Location = new Point(6, 400);
            this.panel3.Name = "panel3";
            this.panel3.Size = new Size(467, 166);
            this.panel3.TabIndex = 2;
            // 
            // FormAddressVerification
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new Size(5, 13);
            this.BackColor = Color.FromArgb(((Byte)(209)), ((Byte)(228)), ((Byte)(243)));
            this.ClientSize = new Size(479, 600);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancelEdit);
            this.Controls.Add(this.panel1);

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.Name = "FormAddressVerification";
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Text = "Address Entry";
            this.Load += new EventHandler(this.FormAddressVerification_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

		}

        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements
        private Container components = null;
        private AddressEntryWithStreet2View addressEntryView1;
        private Panel panel1;
        private MatchingAddressView matchingAddressView1;
        private Label lblMatching;
        private Label lblMessage;
        private CheckBox cbxIgnore;
        private EditAddressView editAddressView1;
        private LoggingButton btnOk;
        private LoggingButton btnCancelEdit;
        private Panel panel2;
        private Panel panel3;
        private int i_CityLength;
        private int i_StreetLength;
		private Account i_Account;
        public  Address i_AddressSelected;
		private Address addressValidation = null;
		private bool	isPanel3Extended = false;
		private YesNoFlag editAddressYesNo = new YesNoFlag("Y");
        #endregion

        #region Construction and Finalization
        public FormAddressWithStreet2Verification()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            SetupControl();
            
           
            //this.Closing += new System.ComponentModel.CancelEventHandler(this.btnClosing_Click);
        }

        public FormAddressWithStreet2Verification(Facility facility, RuleEngine ruleEngine)
        {
            Facility = facility;
            RuleEngine = ruleEngine;
            addressEntryView1 = TheAddressView;
            InitializeComponent();
            SetupControl();
        }

        private void SetupControl()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.btnOk.DialogResult = DialogResult.OK;
            this.btnCancelEdit.DialogResult = DialogResult.Cancel;

            i_AddressSelected = new Address( String.Empty, String.Empty, string.Empty, new ZipCode( String.Empty ), null, null );

            this.panel1.BackColor = Color.FromArgb( 209, 228, 243 );
            this.Height = 260;
            this.btnOk.Enabled = false;
            base.EnableThemesOn( this );
            this.Location = new Point( 250, 50 );

            this.matchingAddressView1.TabIndex = 0;
            this.matchingAddressView1.TabStop = false;
            this.cbxIgnore.TabIndex = 0;
            this.cbxIgnore.TabStop = false;
            this.editAddressView1.Enabled = false;
            this.editAddressView1.TabIndex = 0;
            this.editAddressView1.TabStop = false;
            this.btnOk.TabIndex = 0;
            this.btnCancelEdit.TabIndex = 0;
            this.btnCancelEdit.Enabled = false;
        }

        #endregion

        

        #region Constants
		string SUCCESSFUL_CASS = "SUCCESSFUL CASS";

        #endregion

        }
}
