using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for FormAddressWithCountyVerification.
    /// </summary>
    [Serializable]
    public partial class FormAddressWithCountyVerification : TimeOutFormView
    {
        #region Event Handlers
        private void FormAddressWithCountyVerification_Load(object sender, EventArgs e)
        {
            this.UpdateView();
        }

        private void addressEntryView1_VerificationButtonEnabled(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            YesNoFlag yesNo = args.Context as YesNoFlag;

            if (yesNo.Code == YesNoFlag.CODE_YES)
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
            this.Height = 290;
        }

        private void addressEntryView1_ResetMatchingAddresses(object sender, EventArgs e)
        {
            this.cbxIgnore.Checked = false;
            this.lblMessage.Text = String.Empty;
            this.matchingAddressView1.ResetMatchingAddressView();
            this.Height = 486;
        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            Address addressOriginal = this.addressEntryView1.OriginalAddress;

            PopulateAddress(addressOriginal);
        }

        private void cbxIgnore_Click(object sender, EventArgs e)
        {
            if (this.cbxIgnore.Checked)
            {
                this.addressEntryView1.IgnoreChecked = true;
                this.addressEntryView1.AllFieldsRequiredRules();
                this.matchingAddressView1.Enabled = false;

                this.matchingAddressView1.Ignoring = true;
                this.matchingAddressView1.IgnoreMatchingAddresses();

                if (!this.addressEntryView1.AllFieldsValid())
                {
                    this.AcceptButton = this.addressEntryView1.btnVerify;
                    this.btnOk.Enabled = false;
                }
                else
                {
                    this.PopulateAddress(this.addressEntryView1.VerificationAddress());
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

                if (!this.matchingAddressView1.AddressListPopulated())
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

            this.isPanel3Extended = false;
            this.Height = 486;
            this.panel3.Visible = false;
            this.btnOk.Location = new Point(319, 430);
            this.btnCancelEdit.Location = new Point(399, 430);
        }

        private void OnDataModification(object sender, EventArgs e)
        {
            if (this.cbxIgnore.Checked)
            {
                if (!this.addressEntryView1.AllRequiredFieldsValid())
                {
                    this.btnOk.Enabled = false;
                    this.AcceptButton = this.addressEntryView1.btnVerify;
                }
                else
                {
                    if (!isPanel3Extended)
                    {
                        this.PopulateAddress(this.addressEntryView1.VerificationAddress());
                    }

                    if (i_AddressSelected != null &&
                        (!isPanel3Extended || (editAddressYesNo.Code == YesNoFlag.CODE_YES)))
                    {
                        this.btnOk.Enabled = true;
                        this.AcceptButton = this.btnOk;
                    }
                }
            }
        }

        private void OnNonUSAddress(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            Address addressSearch = args.Context as Address;

            PopulateAddress(addressSearch);
            this.DialogResult = DialogResult.OK;
        }

        private void OnAddressEntryCancelled(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            Address addressOriginal = args.Context as Address;
            PopulateAddress(addressOriginal);
        }

        private void OnAddressVerification(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            Address addressSearch = args.Context as Address;
            ICollection addresses = null;

            addresses = this.GetAddressesFromService( addressSearch, addresses );

            this.matchingAddressView1.Model_Addresses = addresses;
            this.matchingAddressView1.Ignoring = false;
            this.matchingAddressView1.UpdateView();
            this.matchingAddressView1.SetTabOrder();

            if ((addresses == null || addresses.Count < 1) && !this.cbxIgnore.Checked)
            {
                this.btnOk.Enabled = false;
            }

            this.matchingAddressView1.Enabled = true;
            this.cbxIgnore.Checked = false;

            this.Height = 486;
            this.panel3.Visible = false;
            this.btnOk.Location = new Point(319, 430);
            this.btnCancelEdit.Location = new Point(399, 430);
            this.isPanel3Extended = false;

            this.SetTabOrderForMatchingAddresses();

            if (addresses != null &&
                addresses.Count > 0)
            {
                this.btnOk.Enabled = true;
                this.AcceptButton = this.btnOk;
            }
        }


        private ICollection GetAddressesFromService( Address addressSearch, ICollection addresses ) 
        {

            try
            {

                IDataValidationBroker dataValidationBroker = BrokerFactory.BrokerOfType<IDataValidationBroker>();
                User appUser = User.GetCurrent();
                AddressValidationResult validateResult =
                    dataValidationBroker.ValidAddressesMatching(addressSearch, 
                                                                appUser.SecurityUser.UPN,
                                                                appUser.Facility.Code);

                addresses = (ICollection)validateResult.Addresses;

                if (validateResult.ReturnMessage != null &&
                    ((validateResult.ReturnMessage.ToUpper().IndexOf(SUCCESSFUL_CASS) == -1) ||
                     (validateResult.ReturnMessage.ToUpper().IndexOf(SUCCESSFUL_CASS) != -1 && validateResult.Addresses.Count > 1)))
                {
                    this.lblMessage.Text = validateResult.ReturnMessage;
                }
                else
                {
                    this.lblMessage.Text = "";
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains(UIErrorMessages.EDV_UNAVAILABLE_ERROR))
                {
                    this.lblMessage.Text = UIErrorMessages.EDV_UNAVAILABLE_MESSAGE;
                }
                else
                {
                    this.lblMessage.Text = UIErrorMessages.DATAVALIDATION_NOT_AVAILABLE;
                }

                this.Cursor = Cursors.Default;
            }

            return addresses;

        }


        private void OnAddressSelected(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            Address matchingAddressSelected = args.Context as Address;
            ValidateSelectedAddress(matchingAddressSelected);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.cbxIgnore.Checked &&
                !this.isPanel3Extended)
            {
                this.PopulateAddress(this.addressEntryView1.VerificationAddress());
            }

            i_AddressSelected.County = this.addressEntryView1.SelectedCounty;
            ValidateSelectedAddress(i_AddressSelected);
            if (isPanel3Extended)
            {
                panel3.Visible = true;
                this.Height = 663;
            }
            if (!this.btnOk.Enabled)
            {
                this.DialogResult = DialogResult.None;
            }
        }

        private void editAddressView1_EnableOKButton(object sender, EventArgs e)
        {
            LooseArgs args = (LooseArgs)e;
            editAddressYesNo = args.Context as YesNoFlag;

            if (editAddressYesNo.Code == YesNoFlag.CODE_YES)
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
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if (Model_Address != null)
            {
                addressEntryView1.Model = Model_Address;
                addressEntryView1.CapturePhysicalAddress = this.CapturePhysicalAddress;
                addressEntryView1.CaptureMailingAddress = this.CaptureMailingAddress;
                addressEntryView1.CountyRequiredForCurrentActivity = this.CountyRequiredForCurrentActivity;
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
        public bool CapturePhysicalAddress { set; private get; }
        public bool CaptureMailingAddress { set; private get; }
        public bool CountyRequiredForCurrentActivity { set; private get; }
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
            this.editAddressView1.Enabled = true;
            this.editAddressView1.TabIndex = 4;
            this.editAddressView1.TabStop = true;
            this.btnOk.TabIndex = 5;
            this.btnCancelEdit.TabIndex = 6;
        }

        private bool AddressLengthIsValid(Address address)
        {
            if (address != null &&
                address.Address1.Length + address.Address2.Length > 75 ||
                address.City.Length > 15)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        private void ValidateSelectedAddress(Address selectedAddress)
        {
            if (selectedAddress == null)
            {
                return;
            }

            if (!AddressLengthIsValid(selectedAddress))
            {
                this.editAddressView1.Model = selectedAddress;
                this.editAddressView1.UpdateView();

                //Resize window to full view.
                this.btnOk.Location = new Point(319, 605);
                this.btnCancelEdit.Location = new Point(399, 605);
                this.AcceptButton = this.addressEntryView1.btnVerify;
                this.PopulateAddress(selectedAddress);
                this.SetTabOrderForEditAddress();
                isPanel3Extended = true;
                this.btnOk.Enabled = false;
                this.editAddressView1.SetCursorPosition();
            }
            else
            {
                this.Height = 486;
                this.panel3.Visible = false;
                this.btnOk.Location = new Point(319, 430);
                this.btnCancelEdit.Location = new Point(399, 430);
                this.btnOk.Enabled = true;
                this.AcceptButton = this.btnOk;
                this.PopulateAddress(selectedAddress);
                isPanel3Extended = false;
                this.SetTabOrderForMatchingAddresses();
            }

            this.Refresh();
        }

        private void PopulateAddress(Address address)
        {
            if (address == null)
            {
                return;
            }

            i_AddressSelected.Country = address.Country;
            i_AddressSelected.Address1 = address.Address1;
            i_AddressSelected.Address2 = address.Address2;
            i_AddressSelected.ZipCode.PostalCode = address.ZipCode.PostalCode;
            i_AddressSelected.City = address.City;
            i_AddressSelected.State = address.State;
            i_AddressSelected.County = address.County;
            addressEntryView1.SelectedCounty = address.County;
        }
        #endregion

        #region Construction and Finalization
        public FormAddressWithCountyVerification()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
            
            i_AddressSelected = new Address( String.Empty, String.Empty, string.Empty, new ZipCode( String.Empty ), null, null );

			this.panel1.BackColor = Color.FromArgb(((Byte)(209)), ((Byte)(228)), ((Byte)(243)));
            this.Height = 290;
            this.btnOk.Enabled = false;
            base.EnableThemesOn( this );
			this.Location = new Point(250, 50);

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

        #region Data Elements

        private int i_CityLength;
        private int i_StreetLength;
        private Account i_Account;
        public Address i_AddressSelected;
        private Address addressValidation = null;
        private bool isPanel3Extended = false;
        private YesNoFlag editAddressYesNo = new YesNoFlag("Y");
        #endregion

        #region Constants

        private const string SUCCESSFUL_CASS = "SUCCESSFUL CASS";

        #endregion
    }
}
