using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for EditAddressView.
    /// </summary>
    [Serializable]
    public class EditAddressView : ControlView
    {
        #region Event Handlers

        //public event EventHandler ValidateAddressLength;
		public event EventHandler EnableOKButton;

        private void AddressStreetRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args    = (PropertyChangedArgs) e;
            Control aControl            = args.Context as Control;

            if( aControl == this.mtbStreet )
            {
                UIColors.SetRequiredBgColor(aControl);
                Refresh();
            }       
   
            if( aControl == null )
            {
                UIColors.SetRequiredBgColor(this.mtbStreet);
                Refresh();
            }
        }

        private void AddressCityRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args    = (PropertyChangedArgs) e;
            Control aControl            = args.Context as Control;

            if( aControl == this.mtbCity )
            {
                UIColors.SetRequiredBgColor(aControl);
                Refresh();
            }
            
            if( aControl == null )
            {
                UIColors.SetRequiredBgColor(this.mtbCity);
                Refresh();
            }
        }

        private void mtbStreet_TextChanged(object sender, EventArgs e)
        { 
            this.lblStreetLength.Text = this.mtbStreet.Text.Length.ToString();
            this.ValidateFieldLength();

        }

        private void mtbCity_TextChanged(object sender, EventArgs e)
        {            
            this.lblCityLength.Text = this.mtbCity.Text.Length.ToString();
            this.ValidateFieldLength();
        }

		private void mtbCity_Validating(object sender, CancelEventArgs e)
		{
			this.lblCityLength.Text = this.mtbCity.Text.Length.ToString();
			this.Model_Address.City = this.mtbCity.Text;

			this.runRules();
			this.ValidateFieldLength();
		}

		private void mtbStreet_Validating(object sender, CancelEventArgs e)
		{
			this.lblStreetLength.Text = this.mtbStreet.Text.Length.ToString();
			this.Model_Address.Address1 = this.mtbStreet.Text;

			this.runRules();
			this.ValidateFieldLength();
		}
		#endregion

        #region Methods
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
            this.mtbStreet.TextChanged -= new EventHandler(this.mtbStreet_TextChanged);
			this.mtbCity.TextChanged -= new EventHandler(this.mtbCity_TextChanged);

            this.mtbStreet.UnMaskedText = String.Empty;
            this.mtbCity.UnMaskedText = String.Empty;

			this.mtbStreet.TextChanged += new EventHandler(this.mtbStreet_TextChanged);
			this.mtbCity.TextChanged += new EventHandler(this.mtbCity_TextChanged);

			if( this.Model_Address != null )
			{
				if( this.Model_Address.State != null )
				{
					this.lblStateText.Text = this.Model_Address.State.Code;
				}
				
				if( this.Model_Address.ZipCode.PostalCode != null )
				{
					this.lblZipText.Text = this.Model_Address.ZipCode.FormattedPostalCodeFor( this.Model_Address.IsUnitedStatesAddress() );
				}

				this.PopulateStreetInformation();
				this.PopulateCityInformation();	
			}
			
            //this.loadRules();

            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStreetRequired), this.Model, this.mtbStreet, new EventHandler(AddressStreetRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, new EventHandler(AddressCityRequiredEventHandler));

            // run the rules

            this.runRules();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {   

        }

		public void SetCursorPosition()
		{
			if( this.streetInvalid || 
				this.mtbStreet.Text.Trim().Length > 25 )
			{
				this.mtbStreet.Focus();
			}
			else
			{
				this.mtbCity.Focus();
			}
		}

		public Address TruncatedAddress()
		{
			Address truncatedAddress = Model_Address.DeepCopy() as Address;
			truncatedAddress.City = this.mtbCity.Text;
			truncatedAddress.Address1 = this.mtbStreet.Text;
			return truncatedAddress;
			//this.ValidateAddressLength( this, new LooseArgs( truncatedAddress ) );
		}
        #endregion

        #region Properties
        public bool validCharacterLimit
        {
            get
            {
                return IValidCharacterLimit;
            }
            set
            {
                IValidCharacterLimit = value;
            }
        }

        private Address Model_Address
        {
            get
            {
                return (Address)this.Model;
            }
        }
        #endregion

        #region Private Methods
		private void PopulateStreetInformation()
		{
			if( !string.IsNullOrEmpty(Model_Address.Address1) )
			{	
				string addrLine2 = String.Empty;

				if( !string.IsNullOrEmpty(Model_Address.Address2) )
				{
					addrLine2 = Model_Address.Address2;
				}

				this.mtbStreet.Text = Model_Address.Address1 + " " + addrLine2;
				this.mtbStreet.Text = this.mtbStreet.Text.Trim();
				if( this.mtbStreet.Text.Trim().Length <= 25 )
				{
					this.labelStreetNonEdit.Text = this.mtbStreet.Text.Trim();
					this.labelStreetNonEdit.Show();
					this.mtbStreet.Hide();
					this.lblStreetLength.Hide();
					this.lblStreetLimit.Hide();
					this.streetInvalid = false;
				}
				else
				{
					this.labelStreetNonEdit.Hide();
					this.mtbStreet.Show();
					this.lblStreetLength.Show();
					this.lblStreetLimit.Show();
					this.mtbStreet.Text = Model_Address.Address1;
					streetInvalid = true;
				}
			}
		}

		private void PopulateCityInformation()
		{
			if( this.Model_Address.City.Length <= 15 )
			{
				this.labelCityNonEdit.Text = this.Model_Address.City;
				this.mtbCity.Text = this.Model_Address.City;
				this.labelCityNonEdit.Show();
				this.mtbCity.Hide();
				this.lblCityLength.Hide();
				this.lblCityLimit.Hide();
			}
			else
			{
				this.labelCityNonEdit.Hide();
				this.mtbCity.Show();
				this.lblCityLength.Show();
				this.lblCityLimit.Show();
				this.mtbCity.Text = Model_Address.City;
			}

		}


        private void runRules()
        {   

            UIColors.SetNormalBgColor(this.mtbStreet);
            UIColors.SetNormalBgColor(this.mtbCity);

            this.Refresh();

            Account anAccount = AccountView.GetInstance().Model_Account;

            RuleEngine.GetInstance().EvaluateRule(typeof(AddressStreetRequired), anAccount);
            RuleEngine.GetInstance().EvaluateRule(typeof(AddressCityRequired), anAccount);
        }

        private void ValidateFieldLength()
        {
			YesNoFlag yesNo = new YesNoFlag();

			if( ( this.mtbCity.UnMaskedText.Length > 0 && this.mtbCity.UnMaskedText.Length <= 15 ) &&
				( this.mtbStreet.UnMaskedText.Length > 0 && this.mtbStreet.UnMaskedText.Length <= 25 ) )
			{
				yesNo.SetYes();
			}
			else
			{
				yesNo.SetNo();
			}
			
			this.EnableOKButton( this, new LooseArgs( yesNo ) );
        }

        /// <summary>
        /// ActivatePreRegistrationView_Disposed - on disposing, remove any event handlers we have
        /// wired to rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditAddressView_Disposed(object sender, EventArgs e)
        {
            // unwire the events

            this.unRegisterEvents();
        }

        private void unRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStreetRequired), this.Model, new EventHandler(AddressStreetRequiredEventHandler));
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressCityRequired), this.Model, new EventHandler(AddressCityRequiredEventHandler));       

            this.Disposed -= new EventHandler(this.EditAddressView_Disposed);
   
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mtbStreet );
            MaskedEditTextBoxBuilder.ConfigureAddressCity( mtbCity );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.lblDescription = new System.Windows.Forms.Label();
			this.lblStreet = new System.Windows.Forms.Label();
			this.lblCity = new System.Windows.Forms.Label();
			this.lblState = new System.Windows.Forms.Label();
			this.lblZip = new System.Windows.Forms.Label();
			this.mtbStreet = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.mtbCity = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.lblStateText = new System.Windows.Forms.Label();
			this.lblZipText = new System.Windows.Forms.Label();
			this.lblStreetLimit = new System.Windows.Forms.Label();
			this.lblCityLimit = new System.Windows.Forms.Label();
			this.lblStreetLength = new System.Windows.Forms.Label();
			this.lblCityLength = new System.Windows.Forms.Label();
			this.labelStreetNonEdit = new System.Windows.Forms.Label();
			this.labelCityNonEdit = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(6, 4);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(440, 25);
			this.lblDescription.TabIndex = 0;
			this.lblDescription.Text = "The selected address contains more characters than the system can store.  Abbrevi" +
				"ate the address to fit the number of available characters for the field.";
			// 
			// lblStreet
			// 
			this.lblStreet.Location = new System.Drawing.Point(8, 42);
			this.lblStreet.Name = "lblStreet";
			this.lblStreet.Size = new System.Drawing.Size(42, 14);
			this.lblStreet.TabIndex = 0;
			this.lblStreet.Text = "Street:";
			// 
			// lblCity
			// 
			this.lblCity.Location = new System.Drawing.Point(8, 88);
			this.lblCity.Name = "lblCity";
			this.lblCity.Size = new System.Drawing.Size(30, 14);
			this.lblCity.TabIndex = 0;
			this.lblCity.Text = "City:";
			// 
			// lblState
			// 
			this.lblState.Location = new System.Drawing.Point(8, 128);
			this.lblState.Name = "lblState";
			this.lblState.Size = new System.Drawing.Size(81, 14);
			this.lblState.TabIndex = 0;
			this.lblState.Text = "State/Province:";
			// 
			// lblZip
			// 
			this.lblZip.Location = new System.Drawing.Point(8, 147);
			this.lblZip.Name = "lblZip";
			this.lblZip.Size = new System.Drawing.Size(86, 14);
			this.lblZip.TabIndex = 0;
			this.lblZip.Text = "Zip/Postal code:";
			// 
			// mtbStreet
			// 
			this.mtbStreet.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mtbStreet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.mtbStreet.Location = new System.Drawing.Point(93, 37);
			this.mtbStreet.Mask = string.Empty;
			this.mtbStreet.MaxLength = 128;
			this.mtbStreet.Name = "mtbStreet";
			this.mtbStreet.Size = new System.Drawing.Size(360, 20);
			this.mtbStreet.TabIndex = 1;
			this.mtbStreet.Validating += new System.ComponentModel.CancelEventHandler(this.mtbStreet_Validating);
			this.mtbStreet.TextChanged += new System.EventHandler(this.mtbStreet_TextChanged);
			// 
			// mtbCity
			// 
			this.mtbCity.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.mtbCity.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.mtbCity.Location = new System.Drawing.Point(93, 83);
			this.mtbCity.Mask = string.Empty;
			this.mtbCity.MaxLength = 64;
			this.mtbCity.Name = "mtbCity";
			this.mtbCity.Size = new System.Drawing.Size(266, 20);
			this.mtbCity.TabIndex = 2;
			this.mtbCity.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCity_Validating);
			this.mtbCity.TextChanged += new System.EventHandler(this.mtbCity_TextChanged);
			// 
			// lblStateText
			// 
			this.lblStateText.Location = new System.Drawing.Point(93, 128);
			this.lblStateText.Name = "lblStateText";
			this.lblStateText.Size = new System.Drawing.Size(310, 11);
			this.lblStateText.TabIndex = 0;
			// 
			// lblZipText
			// 
			this.lblZipText.Location = new System.Drawing.Point(93, 147);
			this.lblZipText.Name = "lblZipText";
			this.lblZipText.Size = new System.Drawing.Size(310, 12);
			this.lblZipText.TabIndex = 0;
			// 
			// lblStreetLimit
			// 
			this.lblStreetLimit.Location = new System.Drawing.Point(93, 60);
			this.lblStreetLimit.Name = "lblStreetLimit";
			this.lblStreetLimit.Size = new System.Drawing.Size(213, 13);
			this.lblStreetLimit.TabIndex = 0;
			this.lblStreetLimit.Text = "Character limit = 25  Current characters = ";
			// 
			// lblCityLimit
			// 
			this.lblCityLimit.Location = new System.Drawing.Point(93, 106);
			this.lblCityLimit.Name = "lblCityLimit";
			this.lblCityLimit.Size = new System.Drawing.Size(213, 13);
			this.lblCityLimit.TabIndex = 0;
			this.lblCityLimit.Text = "Character limit = 15  Current characters =  ";
			// 
			// lblStreetLength
			// 
			this.lblStreetLength.Location = new System.Drawing.Point(299, 60);
			this.lblStreetLength.Name = "lblStreetLength";
			this.lblStreetLength.Size = new System.Drawing.Size(50, 13);
			this.lblStreetLength.TabIndex = 0;
			this.lblStreetLength.Text = "1";
			// 
			// lblCityLength
			// 
			this.lblCityLength.Location = new System.Drawing.Point(299, 106);
			this.lblCityLength.Name = "lblCityLength";
			this.lblCityLength.Size = new System.Drawing.Size(50, 13);
			this.lblCityLength.TabIndex = 0;
			this.lblCityLength.Text = "1";
			// 
			// labelStreetNonEdit
			// 
			this.labelStreetNonEdit.Location = new System.Drawing.Point(93, 42);
			this.labelStreetNonEdit.Name = "labelStreetNonEdit";
			this.labelStreetNonEdit.Size = new System.Drawing.Size(360, 14);
			this.labelStreetNonEdit.TabIndex = 0;
			// 
			// labelCityNonEdit
			// 
			this.labelCityNonEdit.Location = new System.Drawing.Point(93, 88);
			this.labelCityNonEdit.Name = "labelCityNonEdit";
			this.labelCityNonEdit.Size = new System.Drawing.Size(266, 14);
			this.labelCityNonEdit.TabIndex = 0;
			// 
			// EditAddressView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.labelCityNonEdit);
			this.Controls.Add(this.labelStreetNonEdit);
			this.Controls.Add(this.lblStreetLength);
			this.Controls.Add(this.lblCityLength);
			this.Controls.Add(this.lblZipText);
			this.Controls.Add(this.lblStateText);
			this.Controls.Add(this.mtbCity);
			this.Controls.Add(this.mtbStreet);
			this.Controls.Add(this.lblZip);
			this.Controls.Add(this.lblState);
			this.Controls.Add(this.lblCity);
			this.Controls.Add(this.lblStreet);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.lblStreetLimit);
			this.Controls.Add(this.lblCityLimit);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "EditAddressView";
			this.Size = new System.Drawing.Size(461, 172);
			this.Disposed += new System.EventHandler(this.EditAddressView_Disposed);
			this.ResumeLayout(false);

		}
        #endregion
        #endregion

        #region Private Properties

        #endregion

        #region Construction and Finalization
        public EditAddressView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            // TODO: Add any initialization after the InitializeComponent call
			this.labelCityNonEdit.Hide();
			this.labelStreetNonEdit.Hide();
        }
        #endregion

        #region Data Elements
        private Container components = null;
        private Label lblDescription;
        private Label lblStreet;
        private Label lblCity;
        private Label lblState;
        private Label lblZip;
        private MaskedEditTextBox mtbStreet;
        private MaskedEditTextBox mtbCity;
        private Label lblStateText;
        private Label lblZipText;
        private Label lblStreetLimit;
        private Label lblCityLimit;
        private Label lblStreetLength;
        private Label lblCityLength;
		private Label labelCityNonEdit;
		private Label labelStreetNonEdit;
        private bool IValidCharacterLimit;
		private bool streetInvalid = false;
        #endregion

        #region Constants
        #endregion
    }
}
