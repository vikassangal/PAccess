using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.AddressViews
{
    /// <summary>
    /// Summary description for AddressEntryView.
    /// </summary>
    [Serializable]
    public class AddressEntryView : ControlView
    {
        #region Event Handlers

        public event EventHandler VerifyAddress;
        public event EventHandler DataModified;
        public event EventHandler NonUSAddress;
        public event EventHandler AddressEntryCancelled;
		public event EventHandler SetFormToOriginalSize;
		public event EventHandler ResetMatchingAddresses;
		public event EventHandler VerificationButtonEnabled;

        #region Dean Bortell Defect 34738 used on ln 829
        //delegate definition for anonymous method
        //to address cross threading issue
        delegate void FillComboBoxDelegate();
        #endregion

        /// <summary>
        /// btnCancel_Click - restore the original values for this address
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if( this.AddressEntryCancelled != null )
            {
                this.AddressEntryCancelled( this, new LooseArgs( this.i_OriginalAddress ) );
            }            
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if( this.btnVerify.Text == "OK" )
            {
                Address searchAddress = GetSearchAddress();
                // Raise VerifyAddress Event
                this.NonUSAddress( this, new LooseArgs( searchAddress ) );
            }
            else
            {
					this.Cursor = Cursors.WaitCursor;
					if( this.isFormExtended )
					{
						this.ResetMatchingAddresses( this, null );
					}

					UIColors.SetNormalBgColor( this.axZipCode );
					UIColors.SetNormalBgColor( this.mtbCity );
					UIColors.SetNormalBgColor( this.comboBox_States );
					this.OnlyStreetAndCountryRequiredRule();

                    Address searchAddress = GetSearchAddress();
                    // Raise VerifyAddress Event
                    this.VerifyAddress( this, new LooseArgs( searchAddress ) );
					isFormExtended = true;
					this.Cursor = Cursors.Default;
            }
        }

        private void comboBox_Countries_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }
        private bool CountryIsCanada
        {
            get
            {
                return (selectedCountry != null && (selectedCountry.Code == CANADA_REFCODE));
            }
        }
        private bool CountryIsMexico
        {
            get
            {
                return (selectedCountry != null && (selectedCountry.Code == MEXICO_REFCODE));
            }
        }
        private bool CountryIsBlank
        {
            get
            {
                return (selectedCountry != null && (selectedCountry.Code == String.Empty));
            }
        }
        private void comboBox_Countries_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IgnoreChecked = false;

            selectedCountry = this.comboBox_Countries.SelectedItem as Country;

            if( selectedCountry != null )
            {
                if( this.Model_Address != null )
                {
                    this.Model_Address.Country = selectedCountry;
                }

                if( selectedCountry.Code != String.Empty && CountryIsUSorUSTerritory )
                {
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                    this.axZipCode.MaxLength = 10;
                    this.axZipCode.Mask = "     -";
                    //OTD 36129 - remove mask literal to avoid confusion in MaskedEditTextBox
                    this.axZipCode.UnMaskedText = this.axZipCode.UnMaskedText.Replace( "-", string.Empty ); 
                    this.btnVerify.Text = "&Verify";

                    if ( IsCaliforniaFacility )
                    {
                        if ( this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_INTERNATIONAL )
                        {
                            this.PopulateZipCodeStatusControlWithBlank( false );
                            this.EnableZipCodeAndStatus( true );
                            cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindStringExact( ZipCodeStatus.DESC_KNOWN );
                            this.axZipCode.UnMaskedText = String.Empty;
                        }
                        else
                        {
                            // initial loading from model address
                            this.PopulateZipCodeStatusControlWithBlank(false);
                            ZipCodeStatus zipCodeStatus = this.Model_Address.ZipCode.GetZipCodeStatusFor( State.CALIFORNIA_CODE );
                            cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindString( zipCodeStatus.ToString() );
                            this.SetControlsStateFor( zipCodeStatus );
                        }
                    }
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                }
                else
                {
                    MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );
                    this.axZipCode.Mask = String.Empty;
                    this.axZipCode.MaxLength = 9;
                    this.btnVerify.Text = "OK";
                    
                    if ( IsCaliforniaFacility )
                    {
                        this.PopulateZipCodeStatusControlWithBlank( true );
                        cmbZipCodeStatus.Enabled = false;
                        this.axZipCode.Enabled = true;
                        cmbZipCodeStatus.SelectedIndex = -1;
                        
                    }

                    if( this.isFormExtended )
                    {
                        this.ChangeFormToOriginalSize();
                    }
                }

                SetStateForNonUSCountry();
                if( CountryIsUSorUSTerritory && !this.isFormExtended )
                {
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                    this.OnlyStreetAndCountryRequiredRule();
                    this.RegisterStreetRule();
                }
                else if( selectedCountry != null &&
                    selectedCountry.Code != String.Empty &&
                    !CountryIsUSorUSTerritory )
                {
                    //Register street and city as required.
                    this.OnlyStreetCityAndCountryRequiredRules();
                }
                else
                {
                    this.UnregisterStreetRule();
                }

                if( selectedCountry != null )
                {
                    this.lblCountryDesc.Text = selectedCountry.Description;
                }

                this.runRules();
                this.IsVerificationValid();
                this.DataModified( this, e );
                if (countryIsUSorUSTerritory)
                {
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                }
                else
                {
                    {
                        MaskedEditTextBoxBuilder.ConfigureNonUSZipCode(axZipCode);
                    }
                }
            }
        }

        private void mtbStreet_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

		private void mtbStreet_TextChanged(object sender, EventArgs e)
		{
			this.IsVerificationValid();
			this.DataModified( this, e );
		}

        private void cmbZipCodeStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( selectedCountry != null
                && CountryIsUSorUSTerritory )
            {
                ZipCodeStatus zipCodeStatus = ( ZipCodeStatus ) cmbZipCodeStatus.SelectedItem;
                if ( zipCodeStatus.IsZipStatusUnknownOrHomeless )
                {
                    this.axZipCode.UnMaskedText = zipCodeStatus.GetZipCodeFor( State.CALIFORNIA_CODE );
                }

                // Clear zip code only when the status has changed from 'Unknown' / 'Homeless' 
                // / 'International'(California facility only) OTD 36329 
                // to 'Known' else retain the known value in the zip code field
                if ( zipCodeStatus.Description == ZipCodeStatus.DESC_KNOWN
                    && ( this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_HOMELESS
                         || this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_UNKNOWN 
                         || (this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_INTERNATIONAL
                            && IsCaliforniaFacility) ) )
                {
                    this.axZipCode.UnMaskedText = String.Empty;
                }

                this.SetControlsStateFor( zipCodeStatus );

                if ( this.Model_Address != null && this.Model_Address.ZipCode != null )
                {
                    this.Model_Address.ZipCode.PostalCode = this.axZipCode.UnMaskedText;
                }
            }
            if (countryIsUSorUSTerritory)
            {
                MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
            }
            else
            {
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode(axZipCode);
            }
        }

		private void axZipCode_TextChanged(object sender, EventArgs e)
		{
            bool validZip = true;

            if( ( CountryIsUSorUSTerritory )
                && ( !IsCaliforniaFacility
                    || this.cmbZipCodeStatus.SelectedItem == null
                    || ( (ZipCodeStatus)this.cmbZipCodeStatus.SelectedItem ).Description == ZipCodeStatus.DESC_KNOWN ) )
            {
                try
                {
                    if( this.axZipCode.UnMaskedText != string.Empty )
                    {
                        int zipCode = Convert.ToInt32( this.axZipCode.UnMaskedText );
                    }
                }
                catch
                {
                    validZip = false;
                }
            }

            if( validZip
                && CountryIsUSorUSTerritory
                && ( !IsCaliforniaFacility
                    || this.cmbZipCodeStatus.SelectedItem == null
                    || ( (ZipCodeStatus)this.cmbZipCodeStatus.SelectedItem ).Description == ZipCodeStatus.DESC_KNOWN ) )
            {
                MaskedEditTextBoxBuilder.ConfigureUSZipCode( axZipCode );
            }
            else  // non-numeric characters in zip field 
                  //  or international country (or) california facility and selected zipcodestatus is Unknown / Homeless
            {
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );
            }

            this.IsVerificationValid();
			this.DataModified( this, e );
		}

        private void mtbCity_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

		private void mtbCity_TextChanged(object sender, EventArgs e)
		{
			this.IsVerificationValid();
			this.DataModified( this, e );
		}

        private void comboBox_States_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void comboBox_States_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( this.Model_Address != null )
            {
                this.Model_Address.State = (State)this.comboBox_States.SelectedItem;
                this.runRules();
				this.IsVerificationValid();
                this.DataModified( this, e );
            }
        }

        private void AddressCountryRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.comboBox_Countries);
			this.btnVerify.Enabled = false;
            Refresh();
        }

        public virtual void AddressStreetRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args    = (PropertyChangedArgs) e;
            Control aControl            = args.Context as Control;

            if( aControl == this.mtbStreet )
            {
                UIColors.SetRequiredBgColor(aControl);
            }

            if( aControl == null )
            {
                UIColors.SetRequiredBgColor(this.mtbStreet);
            }
        }

        public void AddressCityRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args    = (PropertyChangedArgs) e;
            Control aControl            = args.Context as Control;

            if( aControl == this.mtbCity )
            {
                UIColors.SetRequiredBgColor(aControl);
            }

            if( aControl == null )
            {
                UIColors.SetRequiredBgColor(this.mtbCity);
            }
        }

        public void AddressStateRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.comboBox_States);
        }

        private void AddressZipRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.axZipCode);
        }

        /// <summary>
        /// All ruless passed - enable the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllRulesPassedEventHandler(object sender, EventArgs e)
        {
			Country country = this.comboBox_Countries.SelectedItem as Country;
			if( country != null &&
				country.Code != String.Empty )
			{
				this.btnVerify.Enabled  = true;
			}
			else
			{
				this.btnVerify.Enabled  = false;
			}
        }

        private void AddressEntryView_Enter(object sender, EventArgs e)
        {
            this.registerEvents();
        }

        private void AddressEntryView_Leave(object sender, EventArgs e)
        {
            this.unRegisterEvents();
        }

		private void comboBox_Countries_Validating(object sender, CancelEventArgs e)
		{
			if( this.btnVerify.Focused &&
				this.btnVerify.Enabled && 
				this.IgnoreChecked )
			{
				return;
			}

			this.CheckExpression();

			if( this.selectedCountry != null &&
				this.selectedCountry.Code != String.Empty &&
                CountryIsUSorUSTerritory )
			{
				this.CheckTextTruncate();
			}

			this.runRules();
			this.IsVerificationValid();
		}

		private void comboBox_States_Validating(object sender, CancelEventArgs e)
		{
			if( this.btnVerify.Focused &&
				this.btnVerify.Enabled && 
				this.IgnoreChecked )
			{
				return;
			}

			if( this.Model_Address != null )
			{
				this.runRules();
				this.IsVerificationValid();
				
				if( this.IgnoreChecked )
				{
					this.DataModified( this, e );
				}
			}
		}

		private void mtbStreet_Validating(object sender, CancelEventArgs e)
		{
			if( this.btnVerify.Focused &&
				this.btnVerify.Enabled && 
				this.IgnoreChecked )
			{
				return;
			}

			if( this.Model_Address != null )
			{
				this.mtbStreet.Text = this.mtbStreet.Text.Trim();
				this.Model_Address.Address1 = this.mtbStreet.Text;
				this.runRules();
				this.IsVerificationValid();

				if( this.IgnoreChecked )
				{
					this.DataModified( this, e );
				}
			}
		}

		private void axZipCode_Validating(object sender, CancelEventArgs e)
		{
			if( this.btnVerify.Focused &&
				this.btnVerify.Enabled && 
				this.IgnoreChecked )
			{
				return;

			}

            UIColors.SetNormalBgColor( axZipCode );
			if( this.Model_Address != null )
			{
				this.Model_Address.ZipCode.PostalCode = this.axZipCode.UnMaskedText;
				this.runRules();
				this.IsZipCodeNumeric();
                this.ValidateZipCode();
				this.IsVerificationValid();

				if( this.IgnoreChecked )
				{
					this.DataModified( this, e );
				}
			}
		}

		private void mtbCity_Validating(object sender, CancelEventArgs e)
		{
			if( this.btnVerify.Focused &&
				this.btnVerify.Enabled && 
				this.IgnoreChecked )
			{
				return;
			}

			if( this.Model_Address != null )
			{
				this.mtbCity.Text = this.mtbCity.Text.Trim();
				this.Model_Address.City = this.mtbCity.Text;
				this.runRules();
				this.IsVerificationValid();
				
				if( this.IgnoreChecked )
				{
					this.DataModified( this, e );
				}
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

        private void DisplayZipCodeStatusControl( bool displayZipCodeStatus )
        {
            if ( displayZipCodeStatus )
            {
                this.cmbZipCodeStatus.Visible = true;
                this.cmbZipCodeStatus.Enabled = true;
                this.axZipCode.Location = new Point(284, 77);
            }
            else
            {
                this.cmbZipCodeStatus.Visible = false;
                this.axZipCode.Location = new Point(92, 77);
            }
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            AddressBroker = new AddressBrokerProxy();

            CountryComboHelper = new ReferenceValueComboBox( comboBox_Countries );
            StateComboHelper = new ReferenceValueComboBox( comboBox_States );
            
            InitializeCountries();

            InitializeStates();
            
            if( this.Model_Address != null )
            {

                // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

                StackTrace st = new StackTrace();
                string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
                // this is being logged using the BreadCrumbLogger in namespace UI.Logging
                BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

                // ********************************************************************************************************


                this.i_OriginalAddress = new Address(this.Model_Address.Address1, this.Model_Address.Address2,
                    this.Model_Address.City, new ZipCode( this.Model_Address.ZipCode.PostalCode ), this.Model_Address.State,
                    this.Model_Address.Country);

                if( this.Model_Address.Country != null )
                {
                    if( Model_Address.Country.GetType() == typeof( UnknownCountry ) )
                    {
                        //TODO:  populate label with country text
                        this.lblCountryDesc.Text = Model_Address.Country.Description;
                        this.comboBox_Countries.SelectedIndex = -1;
                        this.btnVerify.Enabled = false;
                    }
                    else
                    {
                        if (Model_Address.Country.Code != String.Empty)
                        {
                            CountryComboHelper.SetSelectedObject(AddressBroker.CountryWith(User.GetCurrent().Facility.Oid, Model_Address.Country.Code));
                        }
                        else
                        {
                            this.comboBox_Countries.SelectedIndex = -1;
                            this.btnVerify.Enabled = false;
                        }
                    }
                }

				if( ( this.Model_Address.Country == null ||
					  this.Model_Address.Country.Code == String.Empty ) &&
					  this.Model_Address.Address1 != null &&
					  this.Model_Address.Address1 == String.Empty &&
					  this.Model_Address.City != null &&
					  this.Model_Address.City == String.Empty )
				{
					int selectedIndex = -1;
					selectedIndex = this.comboBox_Countries.FindString( USA_DESCRIPTION	);
					if( selectedIndex != -1 )
					{
						this.comboBox_Countries.SelectedIndex = selectedIndex;
					}
				}

                this.mtbStreet.Text = Model_Address.Address1 + Model_Address.Address2;
                    
                if( IsCaliforniaFacility )
                {
                    this.GetZipCodeStatusesCollection();
                }
                this.DisplayZipCodeStatusControl( IsCaliforniaFacility );

                this.SetZipCode();
                
                this.mtbCity.Text = Model_Address.City;

                if( this.Model_Address.State == null )
                {
                    this.comboBox_States.SelectedIndex = -1;
                }
                else
                {
                    this.comboBox_States.SelectedItem = this.Model_Address.State;
                }

                UIColors.SetNormalBgColor( comboBox_Countries );
                UIColors.SetNormalBgColor( comboBox_States );
                UIColors.SetNormalBgColor( mtbStreet );
                UIColors.SetNormalBgColor( axZipCode );
                UIColors.SetNormalBgColor( mtbCity );
                Refresh();
            }
            else
            {
                this.InitializeCountries();
                this.btnVerify.Enabled = false;
            }

            this.registerEvents();

            // run the rules
            this.runRules();
			this.IsVerificationValid();
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        private void SetZipCode()
        {
            string primary = String.Empty;
            string extended = String.Empty;
            int primaryAsInt = 0;
            int extendedAsInt = 0;

            try  
			{
                if ( Model_Address.ZipCode.ZipCodePrimary.Trim() != String.Empty )
                {
                    primaryAsInt = Convert.ToInt32( Model_Address.ZipCode.ZipCodePrimary );
                }
			}
			catch 
			{
                primaryAsInt = 1; // Set dummy value if ZipCodePrimary was alpha-numeric
            }


            if( primaryAsInt != 0  )
            {
                primary = Model_Address.ZipCode.ZipCodePrimary;
            }

            try  
			{
                if ( Model_Address.ZipCode.ZipCodeExtended.Trim() != String.Empty )
                {
                    extendedAsInt = Convert.ToInt32( Model_Address.ZipCode.ZipCodeExtended );
                }
			}
			catch 
			{
                extendedAsInt = 1; // Set dummy value if ZipCodeExtended was alpha-numeric
            }

            if( extendedAsInt != 0  )
            {
                extended = Model_Address.ZipCode.ZipCodeExtended;
            }

            this.axZipCode.UnMaskedText = String.Concat( primary, extended );

            if( IsCaliforniaFacility )      // if facility is in California
            {

                if (CountryIsUSorUSTerritory)
                {
                    ZipCodeStatus zipCodeStatus = this.Model_Address.ZipCode.GetZipCodeStatusFor(State.CALIFORNIA_CODE);
                    bool isInternationalAddress = (zipCodeStatus.Description == String.Empty) ? true : false;
                    this.PopulateZipCodeStatusControlWithBlank(isInternationalAddress);

                    if (this.axZipCode.UnMaskedText.Trim() == String.Empty)
                    {
                        cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindStringExact(ZipCodeStatus.DESC_KNOWN);
                    }
                    else
                    {
                        cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindString(zipCodeStatus.ToString());
                    }

                    this.SetControlsStateFor(zipCodeStatus);
                }
                else
                {
                    ZipCodeStatus zipCodeStatus = new ZipCodeStatus();
                    cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindString(zipCodeStatus.ToString());
                    cmbZipCodeStatus.Enabled = false;
                }
            }
        }

        public virtual bool AllFieldsValid()
        {
            // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

            StackTrace st = new StackTrace();
            string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
            // this is being logged using the BreadCrumbLogger in namespace UI.Logging
            BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

            // ********************************************************************************************************

            //mark all empty fields as required.
            if( this.comboBox_Countries.SelectedIndex < 0 )
            {
                this.comboBox_Countries.Select();
                return false;
            }

            if( this.mtbStreet.Text == String.Empty )
            {
                this.mtbStreet.Select();
                return false;
            }

            if( this.axZipCode.UnMaskedText.Trim() == String.Empty ||
                !HadValidZipCodeLength)
            {
                this.axZipCode.Select();
                return false;
            }

            if( this.mtbCity.Text == String.Empty )
            {
                this.mtbCity.Select();
                return false;
            }

            if( this.comboBox_States.SelectedIndex < 1 )
            {
                this.comboBox_States.Select();
                return false;
            }

            return true;
        }

        public Address VerificationAddress()
        {
            return GetSearchAddress();
        }

		public virtual void AllFieldsRequiredRules()
		{
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressCityRequired), this.Model, this.mtbCity, AddressCityRequiredEventHandler);
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressZipRequired), this.Model, new EventHandler(AddressZipRequiredEventHandler));
			this.runRules();
		}

        public virtual void OnlyStreetAndCountryRequiredRule()
		{
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressCityRequired), this.Model, AddressCityRequiredEventHandler);
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStateRequired), this.Model, AddressStateRequiredEventHandler);
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressZipRequired), this.Model, AddressZipRequiredEventHandler);
			this.runRules();
		}

        public virtual void OnlyStreetCityAndCountryRequiredRules()
		{
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressCityRequired), this.Model, this.mtbCity, AddressCityRequiredEventHandler);
			
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStateRequired), this.Model, AddressStateRequiredEventHandler);
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressZipRequired), this.Model, AddressZipRequiredEventHandler);
			this.runRules();
		}

        public virtual void RegisterStreetRule()
		{
			RuleEngine.GetInstance().RegisterEvent( typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
		}

        public virtual void UnregisterStreetRule()
		{
			RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStreetRequired), this.Model, AddressStreetRequiredEventHandler);
		}
        #endregion

        #region Properties

        private Address Model_Address
        {
            get
            {
                return (Address)this.Model;
            }
        }

        public Address OriginalAddress
        {
            get
            {
                return i_OriginalAddress;
            }
        }

		public bool	IgnoreChecked
		{
		    private get
			{
				return i_IgnoreChecked;
			}
			set
			{
				i_IgnoreChecked = value;
			}
		}

        private bool IsCaliforniaFacility
		{
			get
			{
                return i_IsCaliforniaFacility;
			}
			set
			{
				i_IsCaliforniaFacility = value;
			}
		}

        private Facility Facility { get; set; }

        private RuleEngine RuleEngine { get; set; }

        #endregion

        #region Private Methods
        private void SetControlsStateFor( ZipCodeStatus zipCodeStatus )
        {
            bool isInternationalAddress = ( zipCodeStatus.Description == String.Empty ) ? true : false;

            if ( zipCodeStatus.IsZipStatusUnknownOrHomeless || isInternationalAddress || !countryIsUSorUSTerritory )
            {
                this.EnableZipCodeAndStatus( false );
                this.btnVerify.Text = "OK";
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );

                if( this.isFormExtended )
                {
                    this.ChangeFormToOriginalSize();
                }

                if( !isInternationalAddress )
                {
                    this.cmbZipCodeStatus.Enabled = true;
                }
            }
            else
            {
                this.EnableZipCodeAndStatus( true );
                this.btnVerify.Text = "&Verify";
                MaskedEditTextBoxBuilder.ConfigureUSZipCode( axZipCode );
            }
        }

		private void CheckExpression()
		{
			if( this.IsZipCodeNumeric() && 
				this.selectedCountry != null &&
				CountryIsUSorUSTerritory )
			{
                MaskedEditTextBoxBuilder.ConfigureUSZipCode( axZipCode );
            }
			else
			{
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );
            }
		}

		private void ValidateZipCode()
		{
            if( ( ( this.selectedCountry != null && CountryIsUSorUSTerritory ) ||
                ( this.Model_Address.IsUnitedStatesAddress() ) ) &&
                this.axZipCode.UnMaskedText.Length > 0 &&
                !HadValidZipCodeLength)
            {
                UIColors.SetErrorBgColor( axZipCode );
                MessageBox.Show( UIErrorMessages.ZIP_CODE_LENGTH,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                this.axZipCode.Focus();
                UIColors.SetErrorBgColor( axZipCode );
            }
		}

		protected virtual void IsVerificationValid()
		{
			bool buttonEnabled = false;
			YesNoFlag yesNo = new YesNoFlag();
			yesNo.SetNo();
			State selectedState = (State)this.comboBox_States.SelectedItem;

			if( selectedCountry != null &&
				CountryIsUSorUSTerritory &&
				this.mtbStreet.Text.Trim().Length > 0 &&
                (HadValidZipCodeLength ||
                 ( this.mtbCity.Text.Trim().Length > 0 && 
                   selectedState != null &&
                   selectedState.Code.Trim() != String.Empty ) ) )
			{
				buttonEnabled = true;
				yesNo.SetYes();
			}
			else if( selectedCountry != null &&
				selectedCountry.Code != String.Empty && 
				!CountryIsUSorUSTerritory &&
				this.mtbStreet.Text.Trim().Length > 0 &&
                this.mtbCity.Text.Trim().Length > 0 &&
                selectedState != null &&
                selectedState.Code.Trim() != String.Empty &&
                HadValidZipCodeLength
            )

			{
				buttonEnabled = true;
				yesNo.SetYes();
			}
			else
			{
				yesNo.SetNo();
			}

			this.btnVerify.Enabled = buttonEnabled;
            this.FireVerificationButtonEnabledEvent( yesNo );
		}


        protected void FireVerificationButtonEnabledEvent( YesNoFlag yesNo )
        {
            this.VerificationButtonEnabled(this, new LooseArgs(yesNo));
        }


        private void ChangeFormToOriginalSize()
		{
			this.SetFormToOriginalSize( this, null );
			this.isFormExtended = false;
		}


        protected void runRules()
        {   
            UIColors.SetNormalBgColor(this.comboBox_Countries);
            UIColors.SetNormalBgColor(this.mtbStreet);
            UIColors.SetNormalBgColor(this.mtbCity);
            UIColors.SetNormalBgColor(this.comboBox_States);
            this.Refresh();

            // run the single country code required rule... this rule does not run as part of the
            // final finish button click!
            RuleEngine.EvaluateRule(typeof(AddressCountryRequired), this.Model_Address);

            // run the rest of the address rules
			RuleEngine.EvaluateRule(typeof(AddressFieldsRequired),this.Model_Address);
        }

        private void InitializeCountries()
        {
            if (InvokeRequired)
                Invoke( new FillComboBoxDelegate(InitializeCountries));
            else
            {
                if (comboBox_Countries.Items.Count == 0)
                {
                    #region Dean Bortell Mods Defect 34738 July 31, 2007
                    //There were a few controls causing this action on background threads.
                    //this caused cross-threading issues so I used inline anonymous 
                    //methods to make the binding and filling itself happen on the 
                    //gui thread.

                    CountryComboHelper.PopulateWithCollection(AddressBroker.AllCountries(User.GetCurrent().Facility.Oid));

                    #endregion
                }
            }
        }

        private void InitializeStates()
        {
            // Previously this method did not have InvokeRequired. I have added it to be consistent with InitializeCountries
            // and the comboBox_Countries control.
            //
            // The StateComboHelper control has never raised a cross-thread issue, but the comboBox_Countries has, which means
            // sooner or later so should this comboBox_States since they are both called at the same points in the code.
            //
            if (InvokeRequired)
                Invoke(new FillComboBoxDelegate(InitializeStates));
            else
            {
                if (comboBox_States.Items.Count == 0)
                {
                    StateComboHelper.PopulateWithCollection(AddressBroker.AllStates(User.GetCurrent().Facility.Oid));
                }
            }
        }

        private void EnableZipCodeAndStatus( bool shouldEnable )
        {
            cmbZipCodeStatus.Enabled = shouldEnable;
            this.axZipCode.Enabled = shouldEnable;
        }
        
        private void GetZipCodeStatusesCollection()
        {
            this.ZipCodeStatusCollection = ( ArrayList ) ZipCodeStatus.AllZipCodeStatuses();
        }

        private void PopulateZipCodeStatusControlWithBlank( bool isInternationalAddress )
        {
            cmbZipCodeStatus.Items.Clear();

            if( cmbZipCodeStatus.Items.Count > 0 )
            {
                return;
            }

            if( this.ZipCodeStatusCollection == null )
            {
                MessageBox.Show( "No zipCode statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            if( isInternationalAddress )
            {
                cmbZipCodeStatus.Items.Clear();
                cmbZipCodeStatus.Items.Add( new ZipCodeStatus() ); // blank Zip Code Status entry
            }

            foreach( ZipCodeStatus zipCodeStatus in this.ZipCodeStatusCollection )
            {
                cmbZipCodeStatus.Items.Add( zipCodeStatus );
            }
        }

        private void SetIsCaliforniaFacility()
        {
            this.Facility.SetFacilityStateCode();
            IsCaliforniaFacility = this.Facility.IsFacilityInState( State.CALIFORNIA_CODE ) ? true : false;
        }

        private void SetStateForNonUSCountry()
        {
             
            if (comboBox_States.SelectedItem == null || comboBox_States.SelectedIndex == -1
                                                     || comboBox_States.SelectedIndex == 0)
            {
                if (!(CountryIsCanada || CountryIsMexico || CountryIsUSorUSTerritory || CountryIsBlank))

                {
                    comboBox_States.SelectedIndex = this.comboBox_States.FindString("OTHER");
                }
            }
        }

        private bool IsZipCodeNumeric()
        {
            bool validZip = true;
            if ((CountryIsUSorUSTerritory)
                && (!IsCaliforniaFacility
                    || this.cmbZipCodeStatus.SelectedItem == null
                    || ((ZipCodeStatus) this.cmbZipCodeStatus.SelectedItem).Description == ZipCodeStatus.DESC_KNOWN))
            {
                try
                {
                    int zipCode = Convert.ToInt32(this.axZipCode.UnMaskedText);
                }
                catch
                {

                    if (this.axZipCode.UnMaskedText != string.Empty)
                    {
                        validZip = false;
                    }
                }
            }

            if( selectedCountry != null )
            {
				if( CountryIsUSorUSTerritory && !validZip )
                {
                    UIColors.SetErrorBgColor( this.axZipCode );
					MessageBox.Show( UIErrorMessages.ZIP_CODE_INVALID,
						"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                    this.axZipCode.Focus();
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                    return false;
                }
                else
                {
                    return true;
                }
            }
			return validZip;
        }

        private void CheckTextTruncate()
        {
            if( this.mtbCity.TextLength > 15 )
            {
                string city = this.mtbCity.Text;
                this.mtbCity.Text = city.Substring(0,15);
            }

            if( this.mtbStreet.TextLength > 25 )
            {
                string street = this.mtbStreet.Text;
                this.mtbStreet.Text = street.Substring(0,25);
            }
        }

        private Address GetSearchAddress()
        {
            Address searchAddress = 
                new Address();


            // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

            StackTrace st = new StackTrace();
            string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
            // this is being logged using the BreadCrumbLogger in namespace UI.Logging
            BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

            // ********************************************************************************************************


            State state = new State();
            if( comboBox_States.SelectedItem != null && !comboBox_States.SelectedItem.Equals( String.Empty ) )
            {
                state = (State)comboBox_States.SelectedItem;
            }

            searchAddress.Address1 = this.mtbStreet.Text.Trim();
            searchAddress.Address2 = String.Empty;
            searchAddress.City = this.mtbCity.Text.Trim();

            Country country = null;
            if( comboBox_Countries.SelectedItem != null && !comboBox_Countries.SelectedItem.Equals( String.Empty ) )
            {
                country = (Country)comboBox_Countries.SelectedItem;
            }

            searchAddress.Country = country;
            searchAddress.ZipCode = new ZipCode();
            searchAddress.ZipCode.PostalCode = this.axZipCode.UnMaskedText;
            if ( cmbZipCodeStatus.SelectedItem != null )
            {
                searchAddress.ZipCode.ZipCodeStatus = ( ZipCodeStatus )cmbZipCodeStatus.SelectedItem;
            }
            else
            {
                searchAddress.ZipCode.ZipCodeStatus = new ZipCodeStatus();
            }
            searchAddress.State = state;

            return searchAddress;
        }

        /// <summary>
        /// ActivatePreRegistrationView_Disposed - on disposing, remove any event handlers we have
        /// wired to rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressEntryView_Disposed(object sender, EventArgs e)
        {
            // unwire the events

            this.unRegisterEvents();
        }

        /// <summary>
        /// registerEvents - wire up handlers for rules that might fail
        /// {
        /// }
        /// </summary>
        private void registerEvents()
        {
            if( i_Registered )
            {
                return;
            }

            i_Registered = true;

            RuleEngine.GetInstance().RegisterEvent( typeof(AddressFieldsRequired), this.Model, null );            

            RuleEngine.GetInstance().RegisterEvent( typeof(AddressCountryRequired), this.Model, new EventHandler(AddressCountryRequiredEventHandler));

            // wire the 'global' rules engine events

            RuleEngine.GetInstance().RegisterGlobalEvent( RuleEngine.ALL_RULES_PASSED, AllRulesPassedEventHandler );

            this.Disposed += this.AddressEntryView_Disposed;
        }

        /// <summary>
        /// unegisterEvents - unwire handlers for rules that might fail
        /// </summary>
        private void unRegisterEvents()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressCountryRequired), this.Model, AddressCountryRequiredEventHandler);

            // wire the 'global' rules engine events

            RuleEngine.GetInstance().UnregisterGlobalEvent(RuleEngine.ALL_RULES_PASSED, AllRulesPassedEventHandler);

            this.Disposed -= this.AddressEntryView_Disposed;
        }

		public virtual bool AllRequiredFieldsValid()
		{
            //mark all empty fields as required.
			if( this.comboBox_Countries.SelectedIndex < 0 )
			{
				return false;
			}

			if( this.mtbStreet.Text == String.Empty )
			{
				return false;
			}

			if( this.axZipCode.UnMaskedText.Trim() == String.Empty ||
                !HadValidZipCodeLength)
			{
				return false;
			}

			if( this.mtbCity.Text == String.Empty )
			{
				return false;
			}

			if( this.comboBox_States.SelectedIndex < 1 )
			{
				return false;
			}

			return true;
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
            this.lblCountry = new System.Windows.Forms.Label();
            this.lblStreet = new System.Windows.Forms.Label();
            this.lblZip = new System.Windows.Forms.Label();
            this.lblCity = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.comboBox_Countries = new System.Windows.Forms.ComboBox();
            this.mtbStreet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCity = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.comboBox_States = new System.Windows.Forms.ComboBox();
            this.btnCancel = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnVerify = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblCountryDesc = new System.Windows.Forms.Label();
            this.axZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.cmbZipCodeStatus = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCountry
            // 
            this.lblCountry.Location = new System.Drawing.Point(5, 28);
            this.lblCountry.Name = "lblCountry";
            this.lblCountry.Size = new System.Drawing.Size(48, 14);
            this.lblCountry.TabIndex = 0;
            this.lblCountry.Text = "Country:";
            // 
            // lblStreet
            // 
            this.lblStreet.Location = new System.Drawing.Point(5, 54);
            this.lblStreet.Name = "lblStreet";
            this.lblStreet.Size = new System.Drawing.Size(39, 14);
            this.lblStreet.TabIndex = 0;
            this.lblStreet.Text = "Street:";
            // 
            // lblZip
            // 
            this.lblZip.Location = new System.Drawing.Point(5, 79);
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size(85, 14);
            this.lblZip.TabIndex = 0;
            this.lblZip.Text = "Zip/Postal code:";
            // 
            // lblCity
            // 
            this.lblCity.Location = new System.Drawing.Point(5, 104);
            this.lblCity.Name = "lblCity";
            this.lblCity.Size = new System.Drawing.Size(28, 14);
            this.lblCity.TabIndex = 0;
            this.lblCity.Text = "City:";
            // 
            // lblState
            // 
            this.lblState.Location = new System.Drawing.Point(5, 129);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(82, 14);
            this.lblState.TabIndex = 0;
            this.lblState.Text = "State/Province:";
            // 
            // comboBox_Countries
            // 
            this.comboBox_Countries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Countries.Location = new System.Drawing.Point(92, 25);
            this.comboBox_Countries.Name = "comboBox_Countries";
            this.comboBox_Countries.Size = new System.Drawing.Size(187, 21);
            this.comboBox_Countries.TabIndex = 1;
            this.comboBox_Countries.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_Countries_Validating);
            this.comboBox_Countries.SelectedIndexChanged += new System.EventHandler(this.comboBox_Countries_SelectedIndexChanged);
            this.comboBox_Countries.DropDown += new System.EventHandler(this.comboBox_Countries_DropDown);
            // 
            // mtbStreet
            // 
            this.mtbStreet.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbStreet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbStreet.Location = new System.Drawing.Point(92, 52);
            this.mtbStreet.Mask = "";
            this.mtbStreet.MaxLength = 25;
            this.mtbStreet.Name = "mtbStreet";
            this.mtbStreet.Size = new System.Drawing.Size(354, 20);
            this.mtbStreet.TabIndex = 2;
            this.mtbStreet.Enter += new System.EventHandler(this.mtbStreet_Enter);
            this.mtbStreet.Validating += new System.ComponentModel.CancelEventHandler(this.mtbStreet_Validating);
            this.mtbStreet.TextChanged += new System.EventHandler(this.mtbStreet_TextChanged);
            // 
            // mtbCity
            // 
            this.mtbCity.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbCity.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbCity.Location = new System.Drawing.Point(92, 102);
            this.mtbCity.Mask = "";
            this.mtbCity.MaxLength = 15;
            this.mtbCity.Name = "mtbCity";
            this.mtbCity.Size = new System.Drawing.Size(292, 20);
            this.mtbCity.TabIndex = 5;
            this.mtbCity.Enter += new System.EventHandler(this.mtbCity_Enter);
            this.mtbCity.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCity_Validating);
            this.mtbCity.TextChanged += new System.EventHandler(this.mtbCity_TextChanged);
            // 
            // comboBox_States
            // 
            this.comboBox_States.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_States.Location = new System.Drawing.Point(92, 126);
            this.comboBox_States.Name = "comboBox_States";
            this.comboBox_States.Size = new System.Drawing.Size(187, 21);
            this.comboBox_States.TabIndex = 6;
            this.comboBox_States.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_States_Validating);
            this.comboBox_States.SelectedIndexChanged += new System.EventHandler(this.comboBox_States_SelectedIndexChanged);
            this.comboBox_States.DropDown += new System.EventHandler(this.comboBox_States_DropDown);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 2);
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.BackColor = System.Drawing.SystemColors.Control;
            this.btnVerify.Enabled = false;
            this.btnVerify.Location = new System.Drawing.Point(307, 2);
            this.btnVerify.Message = null;
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.TabIndex = 1;
            this.btnVerify.Text = "&Verify";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // lblCountryDesc
            // 
            this.lblCountryDesc.Location = new System.Drawing.Point(287, 28);
            this.lblCountryDesc.Name = "lblCountryDesc";
            this.lblCountryDesc.Size = new System.Drawing.Size(156, 14);
            this.lblCountryDesc.TabIndex = 0;
            this.lblCountryDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // axZipCode
            // 
            this.axZipCode.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.axZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.axZipCode.Location = new System.Drawing.Point(284, 77);
            this.axZipCode.Mask = "";
            this.axZipCode.MaxLength = 9;
            this.axZipCode.Name = "axZipCode";
            this.axZipCode.Size = new System.Drawing.Size(100, 20);
            this.axZipCode.TabIndex = 4;
            this.axZipCode.Validating += new System.ComponentModel.CancelEventHandler(this.axZipCode_Validating);
            this.axZipCode.TextChanged += new System.EventHandler(this.axZipCode_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(228)))), ((int)(((byte)(243)))));
            this.panel1.Controls.Add(this.btnVerify);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Location = new System.Drawing.Point(0, 158);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(468, 25);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cmbZipCodeStatus);
            this.panel2.Controls.Add(this.lblInfo);
            this.panel2.Controls.Add(this.lblCountry);
            this.panel2.Controls.Add(this.lblStreet);
            this.panel2.Controls.Add(this.lblZip);
            this.panel2.Controls.Add(this.lblCity);
            this.panel2.Controls.Add(this.lblState);
            this.panel2.Controls.Add(this.comboBox_Countries);
            this.panel2.Controls.Add(this.mtbStreet);
            this.panel2.Controls.Add(this.mtbCity);
            this.panel2.Controls.Add(this.comboBox_States);
            this.panel2.Controls.Add(this.lblCountryDesc);
            this.panel2.Controls.Add(this.axZipCode);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(466, 153);
            this.panel2.TabIndex = 0;
            // 
            // lblInfo
            // 
            this.lblInfo.Location = new System.Drawing.Point(5, 4);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(448, 13);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "For US address verification, provide Street and either ZIP or City with State, or" +
                " all fields.";
            // 
            // cmbZipCodeStatus
            // 
            this.cmbZipCodeStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbZipCodeStatus.Location = new System.Drawing.Point(92, 77);
            this.cmbZipCodeStatus.Name = "cmbZipCodeStatus";
            this.cmbZipCodeStatus.Size = new System.Drawing.Size(187, 21);
            this.cmbZipCodeStatus.TabIndex = 3;
            this.cmbZipCodeStatus.Visible = false;
            this.cmbZipCodeStatus.SelectedIndexChanged += new System.EventHandler(this.cmbZipCodeStatus_SelectedIndexChanged);
            // 
            // AddressEntryView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "AddressEntryView";
            this.Size = new System.Drawing.Size(468, 184);
            this.Enter += new System.EventHandler(this.AddressEntryView_Enter);
            this.Leave += new System.EventHandler(this.AddressEntryView_Leave);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties

        private bool HadValidZipCodeLength
        {
            get
            {
                if (CountryIsUSorUSTerritory)
                {
                    return this.axZipCode.UnMaskedText.Length >= 5;
                }
                else
                {
                    return this.axZipCode.UnMaskedText.Length >= 1;
                }
            }
        }

        private IAddressBroker AddressBroker
        {
            get
            {
                return i_Broker;
            }
            set
            {
                i_Broker = value;
            }
        }

        private ArrayList ZipCodeStatusCollection
        {
            get
            {
                return i_zipCodeStatusCollection;
            }
            set
            {
                i_zipCodeStatusCollection = value;
            }
        }

        private ReferenceValueComboBox CountryComboHelper
        {
            get
            {
                return i_CountryComboHelper;
            }
            set
            {
                i_CountryComboHelper = value;
            }
        }
        
        private ReferenceValueComboBox StateComboHelper
        {
            get
            {
                return i_StateComboHelper;
            }
            set
            {
                i_StateComboHelper = value;
            }
        }

        protected bool CountryIsUSorUSTerritory
        {
            get
            {
                return countryIsUSorUSTerritory = ( selectedCountry != null 
                                                    && ( selectedCountry.Code == USA_REFCODE ||
                                                         Country.IsTerritoryOfCountry( selectedCountry.Code, USA_REFCODE ) )
                                                    || ( this.Model_Address != null &&
                                                         this.Model_Address.IsUnitedStatesAddress() ) );
            }
        }
        #endregion

        #region Construction and Finalization
        public AddressEntryView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            this.btnVerify.Message = "Click verify address";
        }

        public AddressEntryView(Facility facility, RuleEngine ruleEngine): this()
        {
            Facility = facility;
            RuleEngine = ruleEngine;
            SetIsCaliforniaFacility();
        }

        #endregion

        #region Data Elements

        private LoggingButton                                    btnCancel;
        public LoggingButton                                    btnVerify;

        private Panel                      panel1;
        private Panel                      panel2;

        private Container                 components = null;

        protected ComboBox                   comboBox_Countries;
        protected ComboBox                   comboBox_States;        
        private ComboBox                   cmbZipCodeStatus;

        private Label                      lblCountry;
        private Label                      lblStreet;
        private Label                      lblZip;
        private Label                      lblCity;
        private Label                      lblState;
        private Label                      lblCountryDesc;
        private Label                      lblInfo;

        private MaskedEditTextBox        mtbStreet;
        protected   MaskedEditTextBox mtbCity;
        public MaskedEditTextBox axZipCode;

        private ReferenceValueComboBox                          i_CountryComboHelper;
        private ReferenceValueComboBox                          i_StateComboHelper;

        private IAddressBroker                                  i_Broker;

        protected Country                                         selectedCountry;

        private Address                                         i_OriginalAddress;

        private bool                                            i_Registered = false;
		private bool											isFormExtended = false;
		private bool											i_IgnoreChecked = false;
        private bool                                            i_IsCaliforniaFacility = false;
        private bool                                            countryIsUSorUSTerritory = false;
        private ArrayList                                       i_zipCodeStatusCollection = new ArrayList();
        #endregion

        #region Constants
        string USA_REFCODE = "USA";
		string USA_DESCRIPTION = "United States";
        string CANADA_REFCODE = "CAN";
        string MEXICO_REFCODE = "MEX"; 
        #endregion
	}
}
