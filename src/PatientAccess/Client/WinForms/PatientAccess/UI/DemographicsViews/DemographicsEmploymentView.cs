using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.EmploymentViews;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for DemographicsEmploymentView.
    /// </summary>
    public class DemographicsEmploymentView : ControlView, IDemographicsEmploymentView
    {
        #region Events
        public event EventHandler EnableInsuranceTab;
        #endregion

        #region Event Handlers

        private void DemographicsEmploymentView_Enter( object sender, EventArgs e )
        {
            IAccountView accountView = AccountView.GetInstance();

            // Display message where the patient is over 65 and if the user selects a 
            // non-Medicare Primary payor and the secondary payor is not entered or null.
            if ( accountView.IsMedicareAdvisedForPatient() )
            {
                accountView.MedicareOver65Checked = true;

                DialogResult warningResult = MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_QUESTION,
                    UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning );

                if ( warningResult == DialogResult.Yes )
                {
                    if ( EnableInsuranceTab != null )
                    {
                        EnableInsuranceTab( this, new LooseArgs( Model_Account ) );
                    }
                }
            }
        }

        private void patientEmploymentView_PatientEmploymentChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( (Control)this.patientEmploymentView.AddressArea );

            RuleEngine.GetInstance().EvaluateRule( typeof( OnEmploymentForm ), this.Model_Account );

        }

        private void DemographicsEmploymentView_Leave( object sender, EventArgs e )
        {
            this.blnLeaveRun = true;
            RuleEngine.GetInstance().EvaluateRule( typeof( OnEmploymentForm ), this.Model_Account );
            this.blnLeaveRun = false;

            patientEmploymentView.UpdateModel();
        }

        private void driversLicenseView_DriversLicenseNumberChanged( object sender, EventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( DriversLicenseStateRequired ), this.Model );
            this.passportView.CheckForValidDriverLicense();
        }

        private void passportView_PassportNumberChanged( object sender, EventArgs e )
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( PassportCountryRequired ), this.Model );
        }

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void DemographicsEmploymentView_Disposed( object sender, EventArgs e )
        {
            this.unregisterEvents();
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void LanguagePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbLanguage );
        }

        /// <summary>
        /// Event handlers for Required/Preferred fields
        /// </summary>
        private void LanguageRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbLanguage );
        }
        public void MakeOtherLanguageRequired()
        {
            UIColors.SetRequiredBgColor( mtbOtherLanguage );
        }

        private void DriversLicenseStateRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( this.driversLicenseView.ComboBox );
        }

        private void EmployerRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( (Control)this.patientEmploymentView.AddressArea );
        }

        private void EmployerAddressPreferredHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( (Control)this.patientEmploymentView.AddressArea );
        }

        private void EmployerAddressRequiredHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( (Control)this.patientEmploymentView.AddressArea );
        }

        private void cmbLanguage_SelectedIndexChanged( object sender, EventArgs e )
        {
            Language language =  cmbLanguage.SelectedItem as Language;
            if ( language != null )
            {
                UpdateSelectedLanguage( language );
            }
        }
        private void UpdateSelectedLanguage( Language language )
        {
            if ( language != null )
            {
                Model_Account.Patient.Language = language;
                if ( !this.blnLeaveRun )
                {
                    UIColors.SetNormalBgColor( this.cmbLanguage );
                    this.Refresh();
                    RuleEngine.GetInstance().EvaluateRule( typeof( InvalidLanguageCode ), this.Model );
                    RuleEngine.GetInstance().EvaluateRule( typeof( InvalidLanguageCodeChange ), this.Model );
                    RuleEngine.GetInstance().EvaluateRule( typeof( LanguagePreferred ), this.Model );
                    RuleEngine.GetInstance().EvaluateRule( typeof( LanguageRequired ), this.Model );
                }
                Presenter.SelectedLanguageChanged( Model_Account.Patient.Language );
            }
        }
        private void ReligionSelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;

            if ( cb.SelectedIndex != -1 )
            {
                Model_Account.Patient.Religion = cb.SelectedItem as Religion;
            }
        }

        private void PlaceOfWorshipIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;

            if ( cb.SelectedIndex != -1 )
            {
                Model_Account.Patient.PlaceOfWorship = cb.SelectedItem as PlaceOfWorship;
            }
        }

        private void mtbPlaceOfBirth_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if ( mtb.UnMaskedText == String.Empty )
            {
                Model_Account.Patient.PlaceOfBirth = String.Empty;
            }
            else
            {
                Model_Account.Patient.PlaceOfBirth = mtb.Text.Trim();
            }
        }

        private void RadioButtonClergyClick( object sender, EventArgs e )
        {
            RadioButton radio = sender as RadioButton;

            switch ( radio.Text.ToUpper().Substring( 0, 1 ) )
            {
                case CLERGY_VISIT_YES:
                    Model_Account.ClergyVisit.SetYes( radio.Text.ToUpper() );
                    break;
                case CLERGY_VISIT_NO:
                    Model_Account.ClergyVisit.SetNo( radio.Text.ToUpper() );
                    break;
                case CLERGY_VISIT_UNSPECIFIED:
                    Model_Account.ClergyVisit.SetBlank( radio.Text.ToUpper() );
                    break;
            }
        }

        private void radioClergyYes_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                radioClergyNo.Focus();
            }
            else if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                radioClergyUnspecified.Focus();
            }
        }

        private void radioClergyNo_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                radioClergyYes.Focus();
            }
            else if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                radioClergyUnspecified.Focus();
            }
        }

        private void radioClergyUnspecified_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Right || e.KeyData == Keys.Down )
            {
                radioClergyYes.Focus();
            }
            else if ( e.KeyData == Keys.Left || e.KeyData == Keys.Up )
            {
                radioClergyNo.Focus();
            }
        }

        private void ValuablesCollected_IndexChanged( object sender, EventArgs e )
        {

            this.Model_Account.ValuablesAreTaken = (YesNoFlag)this.cboValuablesCollected.SelectedItem;

        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void cmbLanguage_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor(this.cmbLanguage);
            this.Refresh();
            RuleEngine.GetInstance().EvaluateRule(typeof(InvalidLanguageCode), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(InvalidLanguageCodeChange), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(LanguagePreferred), this.Model);
            RuleEngine.GetInstance().EvaluateRule(typeof(LanguageRequired), this.Model);
        }
        private void cmbReligion_Validating( object sender, CancelEventArgs e )
        {
            if ( !this.blnLeaveRun )
            {
                UIColors.SetNormalBgColor( this.cmbReligion );
                this.Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidReligionCode ), this.Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidReligionCodeChange ), this.Model );
            }
        }
        private void cmbPlaceOfWorship_Validating( object sender, CancelEventArgs e )
        {
            if ( !this.blnLeaveRun )
            {
                UIColors.SetNormalBgColor( this.cmbPlaceOfWorship );
                this.Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPlaceOfWorshipCode ), this.Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidPlaceOfWorshipCodeChange ), this.Model );
            }
        }
        private void patientEmploymentView_EmploymentStatusValidating( object sender, CancelEventArgs e )
        {
            if ( !this.blnLeaveRun )
            {
                UIColors.SetNormalBgColor( this.patientEmploymentView.ComboBox );
                this.Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmploymentStatusCode ), this.Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidEmploymentStatusCodeChange ), this.Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }

        private void InvalidLanguageCodeChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.cmbLanguage );
        }
        private void InvalidReligionCodeChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.cmbReligion );
        }
        private void InvalidPlaceOfWorshipCodeChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.cmbPlaceOfWorship );
        }
        private void InvalidEmploymentStatusCodeChangeEventHandler( object sender, EventArgs e )
        {
            this.ProcessInvalidCodeEvent( this.patientEmploymentView.ComboBox );
        }

        //-----------------------------------------------------------------

        private void InvalidLanguageCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbLanguage );
        }
        private void InvalidReligionCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbReligion );
        }
        private void InvalidPlaceOfWorshipCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.cmbPlaceOfWorship );
        }
        private void InvalidEmploymentStatusCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( this.patientEmploymentView.ComboBox );
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            Presenter = new DemographicsEmploymentPresenter( this, Model_Account, RuleEngine.GetInstance() );
            if ( loadingModelData )
            {
                loadingModelData = false;
                PopulateLanguageControl();
                PopulateReligionControl();
                PopulatePlacesOfWorshipControl();
            }

            if ( Model_Account != null && Model_Account.Patient != null )
            {
                if ( Model_Account.Patient.DriversLicense != null )
                {
                    driversLicenseView.Model = Model_Account;
                    driversLicenseView.UpdateView();
                    driversLicenseView.CheckForNewBornActivity();
                }
                if ( Model_Account.Patient.Passport != null )
                {
                    passportView.Model = Model_Account;
                    passportView.UpdateView();
                    passportView.CheckForNewBornActivity();
                    passportView.CheckForValidDriverLicense();
                }
                if ( Model_Account.Patient.Employment != null )
                {
                    patientEmploymentView.Model = Model_Account;
                    patientEmploymentView.UpdateView();
                }
                if ( Model_Account.Patient.Language != null )
                {
                    cmbLanguage.SelectedItem = Model_Account.Patient.Language;
                }
                if ( Model_Account.Patient.Religion != null )
                {
                    cmbReligion.SelectedItem = Model_Account.Patient.Religion;
                }
                if ( Model_Account.Patient.PlaceOfWorship != null )
                {
                    cmbPlaceOfWorship.SelectedItem = Model_Account.Patient.PlaceOfWorship;
                }
                if ( Model_Account.Patient.PlaceOfBirth != null )
                {
                    mtbPlaceOfBirth.Text = Model_Account.Patient.PlaceOfBirth.Trim();
                }

                switch ( Model_Account.ClergyVisit.Code.ToUpper() )
                {
                    case CLERGY_VISIT_YES:
                        radioClergyYes.Checked = true;
                        break;
                    case CLERGY_VISIT_NO:
                        radioClergyNo.Checked = true;
                        break;
                    case CLERGY_VISIT_UNSPECIFIED:
                        radioClergyUnspecified.Checked = true;
                        break;
                }
            }
            else
            {
                mtbPlaceOfBirth.ResetText();

                radioClergyYes.Checked         = false;
                radioClergyNo.Checked          = false;
                radioClergyUnspecified.Checked = false;
            }

            if ( Model_Account.Activity!= null && Model_Account.Activity.IsNewBornRelatedActivity())
            {
                driversLicenseView.Enabled = false;
                passportView.Enabled = false;
                patientEmploymentView.Enabled = false;
                //OT#9199: Make sure the event gets fired so all of the proper information gets propogated to PBAR
                patientEmploymentView.ComboBox.SelectedIndex = 0;
                patientEmploymentView.ComboBox.SelectedItem = EmploymentStatus.NewNotEmployed() as object;
            }
            this.PopulateValuablesCollectedList();
            this.registerEvents();
            this.RunRules();
        }

        private void PopulateValuablesCollectedList()
        {
            this.cboValuablesCollected.Items.Clear();

            YesNoFlag blank = new YesNoFlag();
            blank.SetBlank( "" );
            this.cboValuablesCollected.Items.Add( blank );

            YesNoFlag yes = new YesNoFlag();
            yes.SetYes();
            this.cboValuablesCollected.Items.Add( yes );

            YesNoFlag no = new YesNoFlag();
            no.SetNo();
            this.cboValuablesCollected.Items.Add( no );

            if ( this.Model_Account.ValuablesAreTaken.Code == YesNoFlag.CODE_YES )
            {
                this.cboValuablesCollected.SelectedIndex = 1;
            }
            else if ( this.Model_Account.ValuablesAreTaken.Code == YesNoFlag.CODE_NO )
            {
                this.cboValuablesCollected.SelectedIndex = 2;
            }
            else
            {
                this.cboValuablesCollected.SelectedIndex = 0;
            }
        }
        public void PopulateOtherLanguage()
        {
            if ( Model_Account != null && Model_Account.Patient != null && Model_Account.Patient.Language != null )
            {
                if ( Model_Account.Patient.OtherLanguage != null )
                {
                    mtbOtherLanguage.Text = Model_Account.Patient.OtherLanguage.Trim();
                }
                else
                {
                    mtbOtherLanguage.Text = string.Empty;
                }
            }
        }
        public void ClearOtherLanguage()
        {
            mtbOtherLanguage.Text = string.Empty;
        }
        public bool OtherLanguageVisibleAndEnabled
        {
            set
            {
                mtbOtherLanguage.Visible = value;
                lblSpecify.Visible = value;
                mtbOtherLanguage.Enabled = value;
                lblSpecify.Enabled = value;
            }
        }
        #endregion

        #region Properties
        public Account Model_Account
        {
            private get
            {
                return (Account)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        private RuleEngine RuleEngine
        {
            get
            {
                if ( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }

        private IDemographicsEmploymentPresenter Presenter { get; set; }

        #endregion

        #region Private Methods
        /// <summary>
        /// RunRules - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void RunRules()
        {
            UIColors.SetNormalBgColor( this.cmbLanguage );
            UIColors.SetNormalBgColor( this.cmbReligion );
            UIColors.SetNormalBgColor( this.cmbPlaceOfWorship );
            UIColors.SetNormalBgColor( this.patientEmploymentView.ComboBox );
            UIColors.SetNormalBgColor( (Control)this.patientEmploymentView.AddressArea );

            RuleEngine.GetInstance().EvaluateRule( typeof( OnEmploymentForm ), this.Model );
        }

        private void PopulateLanguageControl()
        {
            DemographicsBrokerProxy broker = new DemographicsBrokerProxy();
            ICollection languageCollection = broker.AllLanguages( User.GetCurrent().Facility.Oid );

            cmbLanguage.Items.Clear();

            if ( languageCollection == null )
            {
                MessageBox.Show( "No languages were found", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            foreach ( Language ms in languageCollection )
            {
                cmbLanguage.Items.Add( ms );
            }
        }

        private void PopulatePlacesOfWorshipControl()
        {
            IReligionBroker broker = BrokerFactory.BrokerOfType<IReligionBroker>();
            IList religionCollection = broker.AllPlacesOfWorshipFor( User.GetCurrent().Facility.Oid );

            cmbPlaceOfWorship.Items.Clear();

            if ( religionCollection == null )
            {
                MessageBox.Show( "No religions were found", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            foreach ( PlaceOfWorship ms in religionCollection )
            {
                cmbPlaceOfWorship.Items.Add( ms );
            }
        }

        private void PopulateReligionControl()
        {
            IReligionBroker broker = BrokerFactory.BrokerOfType<IReligionBroker>();
            ICollection religionCollection = broker.AllReligions( User.GetCurrent().Facility.Oid );

            cmbReligion.Items.Clear();

            if ( religionCollection == null )
            {
                MessageBox.Show( "No religions were found", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            foreach ( Religion ms in religionCollection )
            {
                cmbReligion.Items.Add( ms );
            }
        }

        private void registerEvents()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( LanguagePreferred ), this.Model, new EventHandler( LanguagePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( LanguageRequired ), this.Model, new EventHandler( LanguageRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( DriversLicenseStateRequired ), this.Model, new EventHandler( DriversLicenseStateRequiredEventHandler ) );
            //RuleEngine.GetInstance().RegisterEvent( typeof(PassportCountryRequired), this.Model, new EventHandler(PassportCountryRequiredEventHandler));
            RuleEngine.GetInstance().RegisterEvent( typeof( EmployerRequired ), this.Model_Account, new EventHandler( EmployerRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( EmployerAddressRequired ), this.Model_Account, new EventHandler( EmployerAddressRequiredHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidLanguageCode ), this.Model, new EventHandler( InvalidLanguageCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidLanguageCodeChange ), this.Model, new EventHandler( InvalidLanguageCodeChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidReligionCode ), this.Model, new EventHandler( InvalidReligionCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidReligionCodeChange ), this.Model, new EventHandler( InvalidReligionCodeChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidPlaceOfWorshipCode ), this.Model, new EventHandler( InvalidPlaceOfWorshipCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidPlaceOfWorshipCodeChange ), this.Model, new EventHandler( InvalidPlaceOfWorshipCodeChangeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmploymentStatusCode ), this.Model, new EventHandler( InvalidEmploymentStatusCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidEmploymentStatusCodeChange ), this.Model, new EventHandler( InvalidEmploymentStatusCodeChangeEventHandler ) );
            Presenter.RegisterOtherLanguageRequiredRule();

        }

        private void unregisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( LanguagePreferred ), this.Model, LanguagePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( LanguageRequired ), this.Model, LanguageRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( DriversLicenseStateRequired ), this.Model, DriversLicenseStateRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( EmployerRequired ), this.Model_Account, EmployerRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( EmployerAddressRequired ), this.Model_Account, EmployerAddressRequiredHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidLanguageCode ), this.Model, InvalidLanguageCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidLanguageCodeChange ), this.Model, InvalidLanguageCodeChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidReligionCode ), this.Model, InvalidReligionCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidReligionCodeChange ), this.Model, InvalidReligionCodeChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidPlaceOfWorshipCode ), this.Model, InvalidPlaceOfWorshipCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidPlaceOfWorshipCodeChange ), this.Model, InvalidPlaceOfWorshipCodeChangeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmploymentStatusCode ), this.Model, InvalidEmploymentStatusCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidEmploymentStatusCodeChange ), this.Model, InvalidEmploymentStatusCodeChangeEventHandler );
            Presenter.UnRegisterOtherLanguageRequiredRule();
        }

        private void mtbSpecify_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( mtbOtherLanguage );
            Presenter.UpdateOtherLanguage( mtbOtherLanguage.Text );

        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureOtherLanguage( mtbOtherLanguage );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblPlaceOfBirth = new System.Windows.Forms.Label();
            this.mtbPlaceOfBirth = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbLanguage = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblReligion = new System.Windows.Forms.Label();
            this.cmbReligion = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblClergyVisit = new System.Windows.Forms.Label();
            this.radioClergyYes = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.radioClergyNo = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.radioClergyUnspecified = new PatientAccess.UI.CommonControls.RadioButtonKeyHandler();
            this.lblPlaceOfWorship = new System.Windows.Forms.Label();
            this.cmbPlaceOfWorship = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.patientEmploymentView = new PatientAccess.UI.EmploymentViews.PatientEmploymentView();
            this.driversLicenseView = new PatientAccess.UI.CommonControls.DriversLicenseView();
            this.passportView = new PatientAccess.UI.CommonControls.PassportView();
            this.cboValuablesCollected = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblValuablesCollected = new System.Windows.Forms.Label();
            this.panelRadioButtons = new System.Windows.Forms.Panel();
            this.lblSpecify = new System.Windows.Forms.Label();
            this.mtbOtherLanguage = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.SuspendLayout();
            // 
            // lblLanguage
            // 
            this.lblLanguage.Location = new System.Drawing.Point( 16, 330 );
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size( 75, 23 );
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Language:";
            // 
            // lblPlaceOfBirth
            // 
            this.lblPlaceOfBirth.Location = new System.Drawing.Point( 16, 185 );
            this.lblPlaceOfBirth.Name = "lblPlaceOfBirth";
            this.lblPlaceOfBirth.Size = new System.Drawing.Size( 75, 23 );
            this.lblPlaceOfBirth.TabIndex = 0;
            this.lblPlaceOfBirth.Text = "Place of birth:";
            // 
            // mtbPlaceOfBirth
            // 
            this.mtbPlaceOfBirth.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPlaceOfBirth.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.Default;
            this.mtbPlaceOfBirth.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPlaceOfBirth.Location = new System.Drawing.Point( 120, 182 );
            this.mtbPlaceOfBirth.Mask = "";
            this.mtbPlaceOfBirth.MaxLength = 15;
            this.mtbPlaceOfBirth.Name = "mtbPlaceOfBirth";
            this.mtbPlaceOfBirth.Size = new System.Drawing.Size( 184, 20 );
            this.mtbPlaceOfBirth.TabIndex = 4;
            this.mtbPlaceOfBirth.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPlaceOfBirth.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPlaceOfBirth_Validating );
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.Location = new System.Drawing.Point( 120, 324 );
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size( 170, 21 );
            this.cmbLanguage.TabIndex = 11;
            this.cmbLanguage.Validating += new System.ComponentModel.CancelEventHandler( this.cmbLanguage_Validating );
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler( this.cmbLanguage_SelectedIndexChanged );
            // 
            // lblReligion
            // 
            this.lblReligion.Location = new System.Drawing.Point( 16, 215 );
            this.lblReligion.Name = "lblReligion";
            this.lblReligion.Size = new System.Drawing.Size( 75, 23 );
            this.lblReligion.TabIndex = 0;
            this.lblReligion.Text = "Religion:";
            // 
            // cmbReligion
            // 
            this.cmbReligion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReligion.Location = new System.Drawing.Point( 120, 209 );
            this.cmbReligion.Name = "cmbReligion";
            this.cmbReligion.Size = new System.Drawing.Size( 135, 21 );
            this.cmbReligion.TabIndex = 5;
            this.cmbReligion.Validating += new System.ComponentModel.CancelEventHandler( this.cmbReligion_Validating );
            this.cmbReligion.SelectedIndexChanged += new System.EventHandler( this.ReligionSelectedIndexChanged );
            // 
            // lblClergyVisit
            // 
            this.lblClergyVisit.Location = new System.Drawing.Point( 16, 245 );
            this.lblClergyVisit.Name = "lblClergyVisit";
            this.lblClergyVisit.Size = new System.Drawing.Size( 90, 23 );
            this.lblClergyVisit.TabIndex = 0;
            this.lblClergyVisit.Text = "Clergy may visit:";
            // 
            // radioClergyYes
            // 
            this.radioClergyYes.Location = new System.Drawing.Point( 120, 239 );
            this.radioClergyYes.Name = "radioClergyYes";
            this.radioClergyYes.Size = new System.Drawing.Size( 42, 24 );
            this.radioClergyYes.TabIndex = 6;
            this.radioClergyYes.Text = "Yes";
            this.radioClergyYes.Click += new System.EventHandler( this.RadioButtonClergyClick );
            this.radioClergyYes.KeyDown += new System.Windows.Forms.KeyEventHandler( this.radioClergyYes_KeyDown );
            // 
            // radioClergyNo
            // 
            this.radioClergyNo.Location = new System.Drawing.Point( 179, 239 );
            this.radioClergyNo.Name = "radioClergyNo";
            this.radioClergyNo.Size = new System.Drawing.Size( 40, 24 );
            this.radioClergyNo.TabIndex = 7;
            this.radioClergyNo.Text = "No";
            this.radioClergyNo.Click += new System.EventHandler( this.RadioButtonClergyClick );
            this.radioClergyNo.KeyDown += new System.Windows.Forms.KeyEventHandler( this.radioClergyNo_KeyDown );
            // 
            // radioClergyUnspecified
            // 
            this.radioClergyUnspecified.Location = new System.Drawing.Point( 236, 239 );
            this.radioClergyUnspecified.Name = "radioClergyUnspecified";
            this.radioClergyUnspecified.Size = new System.Drawing.Size( 83, 24 );
            this.radioClergyUnspecified.TabIndex = 8;
            this.radioClergyUnspecified.Text = "Unspecified";
            this.radioClergyUnspecified.Click += new System.EventHandler( this.RadioButtonClergyClick );
            this.radioClergyUnspecified.KeyDown += new System.Windows.Forms.KeyEventHandler( this.radioClergyUnspecified_KeyDown );
            // 
            // lblPlaceOfWorship
            // 
            this.lblPlaceOfWorship.Location = new System.Drawing.Point( 16, 272 );
            this.lblPlaceOfWorship.Name = "lblPlaceOfWorship";
            this.lblPlaceOfWorship.Size = new System.Drawing.Size( 90, 18 );
            this.lblPlaceOfWorship.TabIndex = 0;
            this.lblPlaceOfWorship.Text = "Place of worship:";
            // 
            // cmbPlaceOfWorship
            // 
            this.cmbPlaceOfWorship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlaceOfWorship.Location = new System.Drawing.Point( 120, 268 );
            this.cmbPlaceOfWorship.Name = "cmbPlaceOfWorship";
            this.cmbPlaceOfWorship.Size = new System.Drawing.Size( 208, 21 );
            this.cmbPlaceOfWorship.TabIndex = 9;
            this.cmbPlaceOfWorship.Validating += new System.ComponentModel.CancelEventHandler( this.cmbPlaceOfWorship_Validating );
            this.cmbPlaceOfWorship.SelectedIndexChanged += new System.EventHandler( this.PlaceOfWorshipIndexChanged );
            // 
            // patientEmploymentView
            // 
            this.patientEmploymentView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.patientEmploymentView.Location = new System.Drawing.Point( 388, 9 );
            this.patientEmploymentView.Model = null;
            this.patientEmploymentView.Model_Account = null;
            this.patientEmploymentView.Name = "patientEmploymentView";
            this.patientEmploymentView.Size = new System.Drawing.Size( 325, 275 );
            this.patientEmploymentView.TabIndex = 18;
            this.patientEmploymentView.PatientEmploymentChanged += new System.EventHandler( this.patientEmploymentView_PatientEmploymentChanged );
            this.patientEmploymentView.EmploymentStatusValidating += new System.ComponentModel.CancelEventHandler( this.patientEmploymentView_EmploymentStatusValidating );
            // 
            // driversLicenseView
            // 
            this.driversLicenseView.Location = new System.Drawing.Point( 19, 3 );
            this.driversLicenseView.Model = null;
            this.driversLicenseView.Model_Account = null;
            this.driversLicenseView.Name = "driversLicenseView";
            this.driversLicenseView.Size = new System.Drawing.Size( 300, 84 );
            this.driversLicenseView.TabIndex = 1;
            this.driversLicenseView.DriversLicenseNumberChanged += new System.EventHandler( this.driversLicenseView_DriversLicenseNumberChanged );
            // 
            // passportView
            // 
            this.passportView.Location = new System.Drawing.Point( 19, 93 );
            this.passportView.Model = null;
            this.passportView.Model_Account = null;
            this.passportView.Name = "passportView";
            this.passportView.Size = new System.Drawing.Size( 300, 83 );
            this.passportView.TabIndex = 3;
            this.passportView.PassportNumberChanged += new System.EventHandler( this.passportView_PassportNumberChanged );
            // 
            // cboValuablesCollected
            // 
            this.cboValuablesCollected.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValuablesCollected.Location = new System.Drawing.Point( 120, 296 );
            this.cboValuablesCollected.Name = "cboValuablesCollected";
            this.cboValuablesCollected.Size = new System.Drawing.Size( 52, 21 );
            this.cboValuablesCollected.TabIndex = 10;
            this.cboValuablesCollected.SelectedIndexChanged += new System.EventHandler( this.ValuablesCollected_IndexChanged );
            // 
            // lblValuablesCollected
            // 
            this.lblValuablesCollected.Location = new System.Drawing.Point( 16, 305 );
            this.lblValuablesCollected.Name = "lblValuablesCollected";
            this.lblValuablesCollected.Size = new System.Drawing.Size( 112, 15 );
            this.lblValuablesCollected.TabIndex = 0;
            this.lblValuablesCollected.Text = "Valuables collected:";
            // 
            // panelRadioButtons
            // 
            this.panelRadioButtons.Location = new System.Drawing.Point( 120, 236 );
            this.panelRadioButtons.Name = "panelRadioButtons";
            this.panelRadioButtons.Size = new System.Drawing.Size( 194, 30 );
            this.panelRadioButtons.TabIndex = 6;
            this.panelRadioButtons.TabStop = true;
            // 
            // lblSpecify
            // 
            this.lblSpecify.Location = new System.Drawing.Point( 16, 357 );
            this.lblSpecify.Name = "lblSpecify";
            this.lblSpecify.Size = new System.Drawing.Size( 75, 23 );
            this.lblSpecify.TabIndex = 10;
            this.lblSpecify.Text = "Specify:";
            // 
            // mtbOtherLanguage
            // 
            this.mtbOtherLanguage.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbOtherLanguage.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOtherLanguage.KeyPressExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbOtherLanguage.Location = new System.Drawing.Point( 120, 352 );
            this.mtbOtherLanguage.Mask = "";
            this.mtbOtherLanguage.MaxLength = 20;
            this.mtbOtherLanguage.Name = "mtbOtherLanguage";
            this.mtbOtherLanguage.Size = new System.Drawing.Size( 184, 20 );
            this.mtbOtherLanguage.TabIndex = 12;
            this.mtbOtherLanguage.ValidationExpression = "^[a-zA-Z][a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbOtherLanguage.Validating += new System.ComponentModel.CancelEventHandler( this.mtbSpecify_Validating );
            // 
            // DemographicsEmploymentView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.cmbPlaceOfWorship );
            this.Controls.Add( this.cboValuablesCollected );
            this.Controls.Add( this.mtbOtherLanguage );
            this.Controls.Add( this.lblSpecify );
            this.Controls.Add( this.radioClergyNo );
            this.Controls.Add( this.radioClergyUnspecified );
            this.Controls.Add( this.radioClergyYes );
            this.Controls.Add( this.panelRadioButtons );
            this.Controls.Add( this.lblValuablesCollected );
            this.Controls.Add( this.driversLicenseView );
            this.Controls.Add( this.passportView );
            this.Controls.Add( this.patientEmploymentView );
            this.Controls.Add( this.lblPlaceOfWorship );
            this.Controls.Add( this.lblClergyVisit );
            this.Controls.Add( this.cmbReligion );
            this.Controls.Add( this.lblReligion );
            this.Controls.Add( this.cmbLanguage );
            this.Controls.Add( this.mtbPlaceOfBirth );
            this.Controls.Add( this.lblPlaceOfBirth );
            this.Controls.Add( this.lblLanguage );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.Location = new System.Drawing.Point( 16, 253 );
            this.Name = "DemographicsEmploymentView";
            this.Size = new System.Drawing.Size( 961, 385 );
            this.Disposed += new System.EventHandler( this.DemographicsEmploymentView_Disposed );
            this.Leave += new System.EventHandler( this.DemographicsEmploymentView_Leave );
            this.Enter += new System.EventHandler( this.DemographicsEmploymentView_Enter );
            this.ResumeLayout( false );
            this.PerformLayout();

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public DemographicsEmploymentView()
        {
            // This call is required by the Windows.Forms Form Designer.
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private PatientAccessComboBox cmbLanguage;
        private PatientAccessComboBox cmbPlaceOfWorship;
        private PatientAccessComboBox cmbReligion;

        private Label lblClergyVisit;
        private Label lblLanguage;
        private Label lblPlaceOfBirth;
        private Label lblPlaceOfWorship;

        private Label lblValuablesCollected;
        private PatientAccessComboBox cboValuablesCollected;

        private Label lblReligion;

        private Panel panelRadioButtons;

        private MaskedEditTextBox mtbPlaceOfBirth;

        private RadioButtonKeyHandler radioClergyNo;
        private RadioButtonKeyHandler radioClergyUnspecified;
        private RadioButtonKeyHandler radioClergyYes;
        private DriversLicenseView driversLicenseView;
        private PassportView passportView;

        private PatientEmploymentView patientEmploymentView;

        private bool loadingModelData = true;
        private RuleEngine i_RuleEngine;

        private bool blnLeaveRun = false;

        #endregion

        #region Constants
        private const string CLERGY_VISIT_YES         = "Y";
        private const string CLERGY_VISIT_NO          = "N";
        private Label lblSpecify;
        private MaskedEditTextBox mtbOtherLanguage;
        private const string CLERGY_VISIT_UNSPECIFIED = " ";
        #endregion




    }
}
