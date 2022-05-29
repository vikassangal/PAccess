using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
    public partial class AddressEntryWithCountyView : ControlView
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
            if (this.AddressEntryCancelled != null)
            {
                this.AddressEntryCancelled(this, new LooseArgs(this.i_OriginalAddress));
            }
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if (this.btnVerify.Text == "OK")
            {
                Address searchAddress = GetSearchAddress();
                // Raise VerifyAddress Event
                this.NonUSAddress(this, new LooseArgs(searchAddress));
            }
            else
            {
                this.Cursor = Cursors.WaitCursor;
                if (this.isFormExtended)
                {
                    this.ResetMatchingAddresses(this, null);
                }

                UIColors.SetNormalBgColor(this.axZipCode);
                UIColors.SetNormalBgColor(this.mtbCity);
                UIColors.SetNormalBgColor(this.comboBox_States);
                UIColors.SetNormalBgColor(this.comboBox_Counties);
                this.OnlyStreetAndCountryRequiredRule();

                Address searchAddress = GetSearchAddress();
                // Raise VerifyAddress Event
                this.VerifyAddress(this, new LooseArgs(searchAddress));
                isFormExtended = true;
                this.Cursor = Cursors.Default;
            }
        }

        private void comboBox_Countries_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor(cb);
        }
        private void EnableCounties(bool enable)
        {
            this.comboBox_Counties.Enabled = enable;
        }
        private void ResetCounties()
        {
            this.comboBox_Counties.SelectedIndex = -1;
        }
        private void comboBox_Countries_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IgnoreChecked = false;

            selectedCountry = this.comboBox_Countries.SelectedItem as Country;

            if (CountryIsUS)
            {
                ShowOnlyUSStates();
                EnableCounties(true);
            }

            else //NON US Country
            {
                if (!loading)
                {
                    Model_Address.State = new State();
                }

                ShowNonUSStates();
                EnableCounties(false);
                ResetCounties();
            }
           
            if (selectedCountry != null)
            {
                if (this.Model_Address != null)
                {
                    this.Model_Address.Country = selectedCountry;
                }
                //US Country
                if (selectedCountry.Code != String.Empty && CountryIsUSorUSTerritory)
                {
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                    this.axZipCode.MaxLength = 10;
                    this.axZipCode.Mask = "     -";
                    //OTD 36129 - remove mask literal to avoid confusion in MaskedEditTextBox
                    this.axZipCode.UnMaskedText = this.axZipCode.UnMaskedText.Replace("-", string.Empty);
                    this.btnVerify.Text = "&Verify";

                    if (IsCaliforniaFacility)
                    {
                        if (this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_INTERNATIONAL)
                        {
                            this.PopulateZipCodeStatusControlWithBlank();
                            this.EnableZipCodeAndStatus(true);
                            cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindStringExact(ZipCodeStatus.DESC_KNOWN);
                            this.axZipCode.UnMaskedText = String.Empty;
                        }
                        else
                        {
                            this.PopulateZipCodeStatusControlWithBlank();
                            // initial loading from model address
                            ZipCodeStatus zipCodeStatus = this.Model_Address.ZipCode.GetZipCodeStatusFor(State.CALIFORNIA_CODE);
                            cmbZipCodeStatus.SelectedIndex = cmbZipCodeStatus.FindString(zipCodeStatus.ToString());
                            this.SetControlsStateFor(zipCodeStatus);
                        }
                    }
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                }
                else  //NON US Country
                {
                    MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );
                    this.axZipCode.Mask = String.Empty;
                    this.axZipCode.MaxLength = 9;
                    this.btnVerify.Text = "OK";

                    if (IsCaliforniaFacility)
                    {
                        this.axZipCode.UnMaskedText = this.axZipCode.UnMaskedText.Replace("-", string.Empty);
                        this.PopulateZipCodeStatusControlWithBlank();
                        cmbZipCodeStatus.Enabled = false;
                        this.axZipCode.Enabled = true;
                        cmbZipCodeStatus.SelectedIndex = -1;
                    }

                    if (this.isFormExtended)
                    {
                        this.ChangeFormToOriginalSize();
                    }
                }

                if (CountryIsUSorUSTerritory && !this.isFormExtended)
                {
                    this.OnlyStreetAndCountryRequiredRule();
                    this.RegisterStreetRule();
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                }
                else if (selectedCountry != null &&
                    selectedCountry.Code != String.Empty &&
                    !CountryIsUSorUSTerritory)
                {
                    //Register street and city as required.
                    this.OnlyStreetCityAndCountryRequiredRules();
                }
                else
                {
                    this.UnregisterStreetRule();
                }

                if (selectedCountry != null)
                {
                    this.lblCountryDesc.Text = selectedCountry.Description;
                }

                this.runRules();
                this.IsVerificationValid();
                this.DataModified(this, e);
                if (countryIsUSorUSTerritory)
                {
                    MaskedEditTextBoxBuilder.ConfigureUSZipCode(axZipCode);
                }
                else
                {
                    MaskedEditTextBoxBuilder.ConfigureNonUSZipCode(axZipCode);
                }
            }
        }

        private void mtbStreet_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor(mtb);
            Refresh();
        }

        private void mtbStreet_TextChanged(object sender, EventArgs e)
        {
            this.IsVerificationValid();
            this.DataModified(this, e);
        }

        private void cmbZipCodeStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedCountry != null
                && CountryIsUSorUSTerritory)
            {
                ZipCodeStatus zipCodeStatus = (ZipCodeStatus)cmbZipCodeStatus.SelectedItem;
                if (zipCodeStatus.IsZipStatusUnknownOrHomeless)
                {
                    this.axZipCode.UnMaskedText = zipCodeStatus.GetZipCodeFor(State.CALIFORNIA_CODE);
                }

                // Clear zip code only when the status has changed from 'Unknown' / 'Homeless' 
                // / 'International'(California facility only) OTD 36329 
                // to 'Known' else retain the known value in the zip code field
                if (zipCodeStatus.Description == ZipCodeStatus.DESC_KNOWN
                    && (this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_HOMELESS
                         || this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_UNKNOWN
                         || (this.axZipCode.UnMaskedText == ZipCodeStatus.CONST_INTERNATIONAL
                            && IsCaliforniaFacility)))
                {
                    this.axZipCode.UnMaskedText = String.Empty;
                }

                this.SetControlsStateFor(zipCodeStatus);

                if (this.Model_Address != null && this.Model_Address.ZipCode != null)
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

            if ((CountryIsUSorUSTerritory)
                && (!IsCaliforniaFacility
                    || this.cmbZipCodeStatus.SelectedItem == null
                    || ((ZipCodeStatus)this.cmbZipCodeStatus.SelectedItem).Description == ZipCodeStatus.DESC_KNOWN))
            {
                try
                {
                    if (this.axZipCode.UnMaskedText != string.Empty)
                    {
                        int zipCode = Convert.ToInt32(this.axZipCode.UnMaskedText);
                    }
                }
                catch
                {
                    validZip = false;
                }
            }

            if (validZip
                && CountryIsUSorUSTerritory
                && ( this.cmbZipCodeStatus.SelectedItem == null
                    || ((ZipCodeStatus)this.cmbZipCodeStatus.SelectedItem).Description == ZipCodeStatus.DESC_KNOWN))
            {
                MaskedEditTextBoxBuilder.ConfigureUSZipCode( axZipCode );
            }
            else  // non-numeric characters in zip field 
            //  or international country (or) california facility and selected zipcodestatus is Unknown / Homeless
            {
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );
            }

            this.IsVerificationValid();
            this.DataModified(this, e);
        }

        private void mtbCity_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor(mtb);
            Refresh();
        }

        private void mtbCity_TextChanged(object sender, EventArgs e)
        {
            this.IsVerificationValid();
            this.DataModified(this, e);
        }

        private void comboBox_States_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedState = (State) comboBox_States.SelectedItem;

            if (this.Model_Address != null)
            {
                this.Model_Address.State = selectedState;

                this.runRules();
                this.IsVerificationValid();
                this.DataModified(this, e);
            }

            if (selectedState != null && selectedState.Code != null)
            {
                if (CountryIsUS)
                {
                    if (selectedState.Code.Equals(string.Empty))
                    {
                        InitializeCounties();
                    }

                    else
                    {
                        UpdateCounties(selectedState);
                    }
                }
                else
                {
                    EnableCounties(false);
                    ResetCounties();
                }
            }
        }

        private void comboBox_Counties_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            //UIColors.SetNormalBgColor(cb);
        }

        private void comboBox_Counties_SelectedIndexChanged( object sender, EventArgs e )
        {
            if (this.Model_Address != null)
            {
                this.Model_Address.County = ( County )this.comboBox_Counties.SelectedItem;
                this.i_SelectedCounty = (County)this.comboBox_Counties.SelectedItem;
                UIColors.SetNormalBgColor(comboBox_Counties);
                this.runRules();
                this.IsVerificationValid();
                this.DataModified(this, e);
            }
        }

        private void AddressCountryRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.comboBox_Countries);
            this.btnVerify.Enabled = false;
            Refresh();
        }

        private void AddressStreetRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if (aControl == this.mtbStreet)
            {
                UIColors.SetRequiredBgColor(aControl);
            }

            if (aControl == null)
            {
                UIColors.SetRequiredBgColor(this.mtbStreet);
            }
        }

        private void AddressCityRequiredEventHandler(object sender, EventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            Control aControl = args.Context as Control;

            if (aControl == this.mtbCity)
            {
                UIColors.SetRequiredBgColor(aControl);
            }

            if (aControl == null)
            {
                UIColors.SetRequiredBgColor(this.mtbCity);
            }
        }

        private void AddressStateRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.comboBox_States);
        }

        private void AddressZipRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.axZipCode);
        }
        private void AddressCountyRequiredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetRequiredBgColor(this.comboBox_Counties);
        }
        /// <summary>
        /// All ruless passed - enable the OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AllRulesPassedEventHandler(object sender, EventArgs e)
        {
            Country country = this.comboBox_Countries.SelectedItem as Country;
            if (country != null &&
                country.Code != String.Empty)
            {
                this.btnVerify.Enabled = true;
            }
            else
            {
                this.btnVerify.Enabled = false;
            }
        }

        private void AddressEntryWithCountyView_Enter(object sender, EventArgs e)
        {
            this.registerEvents();
        }

        private void AddressEntryWithCountyView_Leave(object sender, EventArgs e)
        {
            this.unRegisterEvents();
        }

        private void comboBox_Countries_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;
            }

            this.CheckExpression();

            if (this.selectedCountry != null &&
                this.selectedCountry.Code != String.Empty &&
                CountryIsUSorUSTerritory)
            {
                this.CheckTextTruncate();
            }

            this.runRules();
            this.IsVerificationValid();
        }

        private void comboBox_States_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;
            }

            if (this.Model_Address != null)
            {
                this.runRules();
                this.IsVerificationValid();

                if (this.IgnoreChecked)
                {
                    this.DataModified(this, e);
                }
            }
        }

        private void comboBox_Counties_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;
            }

            if (this.Model_Address != null)
            {
                UIColors.SetNormalBgColor(comboBox_Counties);
                this.runRules();
                this.IsVerificationValid();

                if (this.IgnoreChecked)
                {
                    this.DataModified(this, e);
                }
            }
        }

        private void mtbStreet_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;
            }

            if (this.Model_Address != null)
            {
                this.mtbStreet.Text = this.mtbStreet.Text.Trim();
                this.Model_Address.Address1 = this.mtbStreet.Text;
                this.runRules();
                this.IsVerificationValid();

                if (this.IgnoreChecked)
                {
                    this.DataModified(this, e);
                }
            }
        }

        private void mtbStreet2_Validating(object sender, CancelEventArgs e)
        {
            if (this.Model_Address != null)
            {
                this.mtbStreet2.Text = this.mtbStreet2.Text.Trim();
                this.Model_Address.Address2 = this.mtbStreet2.Text;
            }
        }

        private void axZipCode_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;

            }

            UIColors.SetNormalBgColor(axZipCode);
            if (this.Model_Address != null)
            {
                this.Model_Address.ZipCode.PostalCode = this.axZipCode.UnMaskedText;
                this.runRules();
                this.IsZipCodeNumeric();
                this.ValidateZipCode();
                this.IsVerificationValid();

                if (this.IgnoreChecked)
                {
                    this.DataModified(this, e);
                }
            }
        }

        private void mtbCity_Validating(object sender, CancelEventArgs e)
        {
            if (this.btnVerify.Focused &&
                this.btnVerify.Enabled &&
                this.IgnoreChecked)
            {
                return;
            }

            if (this.Model_Address != null)
            {
                this.mtbCity.Text = this.mtbCity.Text.Trim();
                this.Model_Address.City = this.mtbCity.Text;
                this.runRules();
                this.IsVerificationValid();

                if (this.IgnoreChecked)
                {
                    this.DataModified(this, e);
                }
            }
        }
        #endregion

        #region Methods

        private void ShowNonUSStates()
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate
            {
                
                comboBox_States.Items.Clear();
                var USstates = AddressBroker.AllNonUSStates(User.GetCurrent().Facility.Oid);
                StateComboHelper.PopulateWithCollection(USstates.ToArray());
               
                SetStateForNonUSCountry();
            });
        }

        private void ShowOnlyUSStates()
        {
            ControlExtensions.UseInvokeIfNeeded( this, delegate
                {
                    var USstates = AddressBroker.AllUSStates(User.GetCurrent().Facility.Oid);

                    StateComboHelper.PopulateWithCollection( USstates.ToArray() );
                    comboBox_States.Items.Insert(0, new State());
                    SetState();
                } );
        }

        private void DisplayZipCodeStatusControl(bool displayZipCodeStatus)
        {
            if (displayZipCodeStatus)
            {
                this.cmbZipCodeStatus.Visible = true;
                this.axZipCode.Location = new Point(284, 104);
                this.cmbZipCodeStatus.Enabled = CountryIsUS;
            }
            else
            {
                this.cmbZipCodeStatus.Visible = false;
                this.axZipCode.Location = new Point(92, 104);
            }
        }

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            AddressBroker = new AddressBrokerProxy();
            loading = true;
            CountryComboHelper = new ReferenceValueComboBox(comboBox_Countries);
            StateComboHelper = new ReferenceValueComboBox(comboBox_States);
            CountyComboHelper = new ReferenceValueComboBox(comboBox_Counties);

            InitializeCountries();
            InitializeCounties();
            InitializeStates();

            if (this.Model_Address != null)
            {

                // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

                StackTrace st = new StackTrace();
                string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
                // this is being logged using the BreadCrumbLogger in namespace UI.Logging
                BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

                // ********************************************************************************************************


                this.i_OriginalAddress = new Address(this.Model_Address.Address1, this.Model_Address.Address2,
                    this.Model_Address.City, new ZipCode(this.Model_Address.ZipCode.PostalCode), this.Model_Address.State,
                    this.Model_Address.Country, this.Model_Address.County );

                if (this.Model_Address.Country != null)
                {
                    if (Model_Address.Country.GetType() == typeof(UnknownCountry))
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
                            this.comboBox_Countries.SelectedItem = this.Model_Address.Country;
                        }
                        else
                        {
                            this.comboBox_Countries.SelectedIndex = -1;
                            this.btnVerify.Enabled = false;
                        }
                    }
                }

                if ((this.Model_Address.Country == null ||
                      this.Model_Address.Country.Code == String.Empty) &&
                      this.Model_Address.Address1 != null &&
                      this.Model_Address.Address1 == String.Empty &&
                      this.Model_Address.City != null &&
                      this.Model_Address.City == String.Empty)
                {
                    int selectedIndex = -1;
                    selectedIndex = this.comboBox_Countries.FindString(USA_DESCRIPTION);
                    if (selectedIndex != -1)
                    {
                        this.comboBox_Countries.SelectedIndex = selectedIndex;
                    }
                }

                this.mtbStreet.Text = Model_Address.Address1;

                this.mtbStreet2.Text = Model_Address.Address2;

                if (IsCaliforniaFacility)
                {
                    this.GetZipCodeStatusesCollection();
                }
                this.DisplayZipCodeStatusControl(IsCaliforniaFacility);

             this.SetZipCode();

                this.mtbCity.Text = Model_Address.City;

                if (this.Model_Address.State == null)
                {
                    this.comboBox_States.SelectedIndex = -1;
                }
                else
                {
                    this.comboBox_States.SelectedItem = this.Model_Address.State;
                }

                if (this.Model_Address.County == null)
                {
                    this.comboBox_Counties.SelectedIndex = -1;
                }
                else
                {
                    this.comboBox_Counties.SelectedItem = this.Model_Address.County;
                }

                UIColors.SetNormalBgColor(comboBox_Countries);
                UIColors.SetNormalBgColor(comboBox_States);
                UIColors.SetNormalBgColor(comboBox_Counties);
                UIColors.SetNormalBgColor(mtbStreet);
                UIColors.SetNormalBgColor(axZipCode);
                UIColors.SetNormalBgColor(mtbCity);
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
            loading = false;
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        private void SetState()
        {
            if (Model_Address.State == null)
            {
                comboBox_States.SelectedIndex = -1;
            }

            else
            {
                comboBox_States.SelectedItem = Model_Address.State;
            }
        }

        private void SetStateForNonUSCountry()
        {
            if (!comboBox_States.Items.Contains(Model_Address.State))
            {
                comboBox_States.SelectedIndex = -1;
            }
            if (Model_Address.State != null && Model_Address.State.Code != string.Empty)
            {
                comboBox_States.SelectedItem = Model_Address.State;
            }
            if (comboBox_States.SelectedItem == null || comboBox_States.SelectedIndex == -1)
            {
                if (CountryIsCanada || CountryIsMexico)
                {
                    comboBox_States.SelectedIndex = -1;
                }
                else
                {
                    comboBox_States.SelectedIndex = this.comboBox_States.FindString("OTHER");
                }
            }
        }

        private void SetCounty()
        {
            if (Model_Address.County == null)
            {
                comboBox_Counties.SelectedIndex = -1;
            }

            else
            {
                comboBox_Counties.SelectedItem = Model_Address.County;
            }
        }

        private void SetZipCode()
        {
            string primary = String.Empty;
            string extended = String.Empty;
            int primaryAsInt = 0;
            int extendedAsInt = 0;

            try
            {
                if (Model_Address.ZipCode.ZipCodePrimary.Trim() != String.Empty)
                {
                    primaryAsInt = Convert.ToInt32(Model_Address.ZipCode.ZipCodePrimary);
                }
            }
            catch
            {
                primaryAsInt = 1; // Set dummy value if ZipCodePrimary was alpha-numeric
            }


            if (primaryAsInt != 0)
            {
                primary = Model_Address.ZipCode.ZipCodePrimary;
            }

            try
            {
                if (Model_Address.ZipCode.ZipCodeExtended.Trim() != String.Empty)
                {
                    extendedAsInt = Convert.ToInt32(Model_Address.ZipCode.ZipCodeExtended);
                }
            }
            catch
            {
                extendedAsInt = 1; // Set dummy value if ZipCodeExtended was alpha-numeric
            }

            if (extendedAsInt != 0)
            {
                extended = Model_Address.ZipCode.ZipCodeExtended;
            }

            this.axZipCode.UnMaskedText = String.Concat(primary, extended);

            if (IsCaliforniaFacility)      // if facility is in California
            {
                if (CountryIsUSorUSTerritory)
                {
                    ZipCodeStatus zipCodeStatus = this.Model_Address.ZipCode.GetZipCodeStatusFor(State.CALIFORNIA_CODE);
                   
                    this.PopulateZipCodeStatusControlWithBlank();

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

        public bool AllFieldsValid()
        {
            // ****** PLEASE DO NOT REMOVE - CROSS-THREAD DEBUGGING - THIS WILL BE REMOVED ONCE THE BUG IS FIXED ******

            StackTrace st = new StackTrace();
            string stackOutput = StackTracer.LogTraceLog(st, this.comboBox_Countries.InvokeRequired);
            // this is being logged using the BreadCrumbLogger in namespace UI.Logging
            BreadCrumbLogger.GetInstance.Log(String.Format(stackOutput));

            // ********************************************************************************************************

            //mark all empty fields as required.
            if (this.comboBox_Countries.SelectedIndex < 0)
            {
                this.comboBox_Countries.Select();
                return false;
            }

            if (this.mtbStreet.Text == String.Empty)
            {
                this.mtbStreet.Select();
                return false;
            }

            if (this.axZipCode.UnMaskedText.Trim() == String.Empty ||
                !HadValidZipCodeLength )
            {
                this.axZipCode.Select();
                return false;
            }

            if (this.mtbCity.Text == String.Empty)
            {
                this.mtbCity.Select();
                return false;
            }

            if (this.CountyRequiredForCurrentActivity && this.CountryIsUS && this.CapturePhysicalAddress && this.IgnoreChecked && this.comboBox_Counties.SelectedIndex < 1)
            {
                this.comboBox_Counties.Select();
                return false;
            }
			
            if (this.comboBox_States.SelectedIndex < 1)
            {
                this.comboBox_States.Select();
                return false;
            }

            return true;
        }

        public Address VerificationAddress()
        {
            Address address = GetSearchAddress();
            return address;
        }


        public void AllFieldsRequiredRules()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, AddressCityRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStateRequired), this.Model, new EventHandler(AddressStateRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressZipRequired), this.Model, new EventHandler(AddressZipRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCountyRequired), this.Model, this.CapturePhysicalAddress, AddressCountyRequiredEventHandler);
            this.runRules();
        }

        public void OnlyStreetAndCountryRequiredRule()
        {
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressCityRequired), this.Model, AddressCityRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStateRequired), this.Model, AddressStateRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressZipRequired), this.Model, AddressZipRequiredEventHandler);
            
            this.runRules();
        }

        private void OnlyStreetCityAndCountryRequiredRules()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCityRequired), this.Model, this.mtbCity, AddressCityRequiredEventHandler);

            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressStateRequired), this.Model, AddressStateRequiredEventHandler);
            RuleEngine.GetInstance().UnregisterEvent(typeof(AddressZipRequired), this.Model, AddressZipRequiredEventHandler);
           
            this.runRules();
        }

        private void RegisterStreetRule()
        {
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressStreetRequired), this.Model, this.mtbStreet, AddressStreetRequiredEventHandler);
        }

        private void UnregisterStreetRule()
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

        public County SelectedCounty
        {
            get
            {
                return i_SelectedCounty;
            }

            set
            {
                i_SelectedCounty = value;
            }
        }

        public bool IgnoreChecked
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
        #endregion

        #region Private Methods
        private void SetControlsStateFor(ZipCodeStatus zipCodeStatus)
        {
            bool isInternationalAddress = (zipCodeStatus.Description == String.Empty) ? true : false;

            if (zipCodeStatus.IsZipStatusUnknownOrHomeless || isInternationalAddress || !countryIsUSorUSTerritory)
            {
                this.EnableZipCodeAndStatus(false);
                this.btnVerify.Text = "OK";
                MaskedEditTextBoxBuilder.ConfigureNonUSZipCode( axZipCode );

                if (this.isFormExtended)
                {
                    this.ChangeFormToOriginalSize();
                }

                if (!isInternationalAddress || CountryIsUS)
                {
                    this.cmbZipCodeStatus.Enabled = true;
                }
                else
                {
                    this.cmbZipCodeStatus.Enabled = false;
                }
            }
            else
            {
                this.EnableZipCodeAndStatus(true);
                this.btnVerify.Text = "&Verify";
                MaskedEditTextBoxBuilder.ConfigureUSZipCode( axZipCode );
            }
        }

        private void CheckExpression()
        {
            if (this.IsZipCodeNumeric() &&
                this.selectedCountry != null &&
                CountryIsUSorUSTerritory)
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
            if (((this.selectedCountry != null && CountryIsUSorUSTerritory) ||
                (this.Model_Address.IsUnitedStatesAddress())) &&
                this.axZipCode.UnMaskedText.Length > 0 &&
                !HadValidZipCodeLength )
            {
                UIColors.SetErrorBgColor(axZipCode);
                MessageBox.Show(UIErrorMessages.ZIP_CODE_LENGTH,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                this.axZipCode.Focus();
                UIColors.SetErrorBgColor(axZipCode);
            }
        }
        
        private void IsVerificationValid()
        {
            bool buttonEnabled = false;
            YesNoFlag yesNo = new YesNoFlag();
            yesNo.SetNo();
            State selectedState = (State)this.comboBox_States.SelectedItem;

            if (selectedCountry != null &&
                CountryIsUSorUSTerritory &&
                this.mtbStreet.Text.Trim().Length > 0 &&
                (HadValidZipCodeLength ||
                (this.mtbCity.Text.Trim().Length > 0 &&
                selectedState != null &&
                selectedState.Code.Trim() != String.Empty)))
            {
                buttonEnabled = true;
                yesNo.SetYes();
            }
            else if (selectedCountry != null &&
                selectedCountry.Code != String.Empty &&
                !CountryIsUSorUSTerritory &&
                this.mtbStreet.Text.Trim().Length > 0 &&
                this.mtbCity.Text.Trim().Length > 0
                && selectedState !=null
                && selectedState.Code.Trim() != String.Empty
                && HadValidZipCodeLength
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
            this.VerificationButtonEnabled(this, new LooseArgs(yesNo));
        }

        private void ChangeFormToOriginalSize()
        {
            this.SetFormToOriginalSize(this, null);
            this.isFormExtended = false;
        }


        private void runRules()
        {
            UIColors.SetNormalBgColor(this.comboBox_Countries);
            UIColors.SetNormalBgColor(this.mtbStreet);
            UIColors.SetNormalBgColor(this.mtbCity);
            UIColors.SetNormalBgColor(this.comboBox_States);
            UIColors.SetNormalBgColor(this.comboBox_Counties);
            this.Refresh();

            // run the single country code required rule... this rule does not run as part of the
            // final finish button click!
            RuleEngine.GetInstance().EvaluateRule(typeof(AddressCountryRequired), this.Model_Address);

            // run the rest of the address rules
            RuleEngine.GetInstance().EvaluateRule(typeof(AddressFieldsRequired), this.Model_Address);
            RuleEngine.GetInstance().EvaluateRule(typeof(AddressCountyRequired), this.Model_Address,this.CapturePhysicalAddress);
        }

        private void InitializeCountries()
        {
            if (InvokeRequired)
                Invoke(new FillComboBoxDelegate(InitializeCountries));
            else
            {
                if (comboBox_Countries.Items.Count == 0)
                {
                    #region Dean Bortell Mods Defect 34738 July 31, 2007
                    //There were a few controls causing this action on background threads.
                    //this caused cross-threading issues so I used inline anonymous 
                    //methods to make the binding and filling itself happen on the 
                    //gui thread.

                    CountryComboHelper.PopulateWithCollection(AddressBroker.AllCountries(Facility.Oid));
                    if (CaptureMailingAddress && comboBox_Countries.Items.Contains( new Country() ))
                    {
                        comboBox_Countries.Items.Remove(new Country() );
                    }

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

        private void InitializeCounties()
        {
            if ( InvokeRequired )
                Invoke( new FillComboBoxDelegate( InitializeCounties ));
            else
            {
                    CountyComboHelper.DisplayMember = "Key" + " " + "Description";
                    CountyComboHelper.PopulateWithCollection( AddressBroker.AllCountiesFor( Facility.Oid ) );
                
            }
        }

        private void UpdateCounties(State selectedState)
        {
            ControlExtensions.UseInvokeIfNeeded(this, delegate
                {
                    CountyComboHelper.DisplayMember = "Key" + " " + "Description";

                    var facilityId = Facility.Oid;

                    var counties = AddressBroker.GetCountiesForAState(selectedState.Code, facilityId);

                    CountyComboHelper.PopulateWithCollection(counties.ToArray() );
                    comboBox_Counties.Items.Insert(0, new County());
                    SetCounty();
                });
        }


        private void EnableZipCodeAndStatus(bool shouldEnable)
        {
            cmbZipCodeStatus.Enabled = shouldEnable;
            this.axZipCode.Enabled = shouldEnable;
        }

        private void GetZipCodeStatusesCollection()
        {
            this.ZipCodeStatusCollection = (ArrayList)ZipCodeStatus.AllZipCodeStatuses();
        }

        private void PopulateZipCodeStatusControlWithBlank()
        {
            cmbZipCodeStatus.Items.Clear();

            if (cmbZipCodeStatus.Items.Count > 0)
            {
                return;
            }

            if (this.ZipCodeStatusCollection == null)
            {
                MessageBox.Show("No zipCode statuses were found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!CountryIsUS)
            {
                cmbZipCodeStatus.Items.Add(new ZipCodeStatus()); // blank Zip Code Status entry
            }

            foreach (ZipCodeStatus zipCodeStatus in this.ZipCodeStatusCollection)
            {
                cmbZipCodeStatus.Items.Add(zipCodeStatus);
            }
        }

        private void SetIsCaliforniaFacility()
        {
            Facility.SetFacilityStateCode();
            IsCaliforniaFacility = Facility.IsFacilityInState(State.CALIFORNIA_CODE) ? true : false;
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
                    if (this.axZipCode.UnMaskedText != string.Empty)
                    {
                        int zipCode = Convert.ToInt32(this.axZipCode.UnMaskedText);
                    }
                }
                catch
                {
                    validZip = false;
                }
            }

        
        if (selectedCountry != null)
            {
                if (CountryIsUSorUSTerritory && !validZip)
                {
                    UIColors.SetErrorBgColor(this.axZipCode);
                    MessageBox.Show(UIErrorMessages.ZIP_CODE_INVALID,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    this.axZipCode.Focus();
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
            if (this.mtbCity.TextLength > 15)
            {
                string city = this.mtbCity.Text;
                this.mtbCity.Text = city.Substring(0, 15);
            }

            if (this.mtbStreet.TextLength > 45)
            {
                string street = this.mtbStreet.Text;
                this.mtbStreet.Text = street.Substring(0, 45);
            }

            if ( mtbStreet2.TextLength > 30 )
            {
                string street2 = mtbStreet2.Text;
                mtbStreet2.Text = street2.Substring(0, 30);
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
            if (comboBox_States.SelectedItem != null && !comboBox_States.SelectedItem.Equals(String.Empty))
            {
                state = (State)comboBox_States.SelectedItem;
            }

            searchAddress.Address1 = this.mtbStreet.Text.Trim();
            searchAddress.Address2 = this.mtbStreet2.Text.Trim();
            searchAddress.City = this.mtbCity.Text.Trim();

            Country country = null;
            if (comboBox_Countries.SelectedItem != null && !comboBox_Countries.SelectedItem.Equals(String.Empty))
            {
                country = (Country)comboBox_Countries.SelectedItem;
            }

            searchAddress.Country = country;
            searchAddress.ZipCode = new ZipCode();
            searchAddress.ZipCode.PostalCode = this.axZipCode.UnMaskedText;
            if (cmbZipCodeStatus.SelectedItem != null)
            {
                searchAddress.ZipCode.ZipCodeStatus = (ZipCodeStatus)cmbZipCodeStatus.SelectedItem;
            }
            else
            {
                searchAddress.ZipCode.ZipCodeStatus = new ZipCodeStatus();
            }
            searchAddress.State = state;

            County county = null;
            if (comboBox_Counties.SelectedItem != null && !comboBox_Counties.SelectedItem.Equals(String.Empty))
            {
                county = (County)comboBox_Counties.SelectedItem;
            }
            searchAddress.County = county;

            return searchAddress;
        }

        /// <summary>
        /// ActivatePreRegistrationView_Disposed - on disposing, remove any event handlers we have
        /// wired to rules
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddressEntryWithCountyView_Disposed(object sender, EventArgs e)
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
            if (i_Registered)
            {
                return;
            }

            i_Registered = true;

            RuleEngine.GetInstance().RegisterEvent(typeof(AddressFieldsRequired), this.Model, null);

            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCountryRequired), this.Model, new EventHandler(AddressCountryRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent(typeof(AddressCountyRequired), this.Model,this.CapturePhysicalAddress, new EventHandler(AddressCountyRequiredEventHandler));

            // wire the 'global' rules engine events

            RuleEngine.GetInstance().RegisterGlobalEvent(RuleEngine.ALL_RULES_PASSED, AllRulesPassedEventHandler);

            this.Disposed += this.AddressEntryWithCountyView_Disposed;
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

            this.Disposed -= this.AddressEntryWithCountyView_Disposed;
        }

        public bool AllRequiredFieldsValid()
        {
            //mark all empty fields as required.
            if (this.comboBox_Countries.SelectedIndex < 0)
            {
                return false;
            }

            if (this.mtbStreet.Text == String.Empty)
            {
                return false;
            }

            if (this.axZipCode.UnMaskedText.Trim() == String.Empty ||
                !HadValidZipCodeLength )
            {
                return false;
            }

            if (this.mtbCity.Text == String.Empty)
            {
                return false;
            }

            if (this.comboBox_States.SelectedIndex < 1)
            {
                return false;
            }
			
            if (this.CountyRequiredForCurrentActivity && this.CountryIsUS && CapturePhysicalAddress && 
                IgnoreChecked && this.comboBox_Counties.SelectedIndex < 1)
            {
                return false;
            }
            return true;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mtbStreet );
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mtbStreet2 );
            MaskedEditTextBoxBuilder.ConfigureAddressCity( mtbCity );
        }

        #endregion

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

        #region Private Properties
        public bool CapturePhysicalAddress { set; private get; }
        public bool CaptureMailingAddress { set; private get; }
        public bool CountyRequiredForCurrentActivity { set; private get; }
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

        private ReferenceValueComboBox CountyComboHelper
        {
            get
            {
                return i_CountyComboHelper;
            }
            set
            {
                i_CountyComboHelper = value;
            }
        }

        private bool CountryIsUSorUSTerritory
        {
            get
            {
                return countryIsUSorUSTerritory = (selectedCountry != null
                                                    && (selectedCountry.Code == USA_REFCODE ||
                                                         Country.IsTerritoryOfCountry(selectedCountry.Code, USA_REFCODE))
                                                    || (this.Model_Address != null &&
                                                         this.Model_Address.IsUnitedStatesAddress()));
            }
        }

        private bool CountryIsUS
        {
            get
            {
                return  (selectedCountry != null && (selectedCountry.Code == USA_REFCODE));
            }
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

        #endregion

        #region Construction and Finalization
        public AddressEntryWithCountyView() : this( User.GetCurrent().Facility )
        {
            
        }

        private AddressEntryWithCountyView(Facility facility)
        {
            Facility = facility;
            
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            ConfigureControls();

            this.btnVerify.Message = "Click verify address";
            this.SetIsCaliforniaFacility();
        }
        #endregion

        #region Data Elements

        private ReferenceValueComboBox i_CountyComboHelper;
        private ReferenceValueComboBox i_CountryComboHelper;
        private ReferenceValueComboBox i_StateComboHelper;

        private IAddressBroker i_Broker;

        private Country selectedCountry;
        private County i_SelectedCounty = null;

        private Address i_OriginalAddress;

        private bool i_Registered = false;
        private bool isFormExtended = false;
        private bool i_IgnoreChecked = false;
        private bool i_IsCaliforniaFacility = false;
        private bool countryIsUSorUSTerritory = false;
        private bool loading = true;
        private ArrayList i_zipCodeStatusCollection = new ArrayList();
        #endregion

        #region Constants
        string USA_REFCODE = "USA";
        string USA_DESCRIPTION = "United States";
        string CANADA_REFCODE = "CAN";
        string MEXICO_REFCODE = "MEX"; 

        private Facility Facility { get; set; }

        #endregion
    }
}
