using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.Logging;

namespace PatientAccess.UI.EmploymentViews
{
    /// <summary>
    /// Summary description for EmploymentView.
    /// </summary>
    public class PatientEmploymentView : ControlView, IEmploymentView
    {
        #region Event Handlers

        public event EventHandler PatientEmploymentChanged;

        public event CancelEventHandler EmploymentStatusValidating;

        /// <summary>
        /// On disposing, remove any event handlers we have wired to rules
        /// </summary>
        private void PatientEmploymentView_Disposed( object sender, EventArgs e )
        {
            UnRegisterEvents();
        }

        private void PatientEmploymentView_Leave( object sender, EventArgs e )
        {
            UpdatePhoneNumberOnModel();
        }

        private void cmbEmploymentStatus_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
        }

        private void cmbEmploymentStatus_Validating( object sender, CancelEventArgs e )
        {
            if ( EmploymentStatusValidating != null )
            {
                EmploymentStatusValidating( this, null );
                RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentStatusRequired ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentStatusPreferred ), Model );
            }

            if ( cmbEmploymentStatus.Text.Trim() != String.Empty &&
                cmbEmploymentStatus.Text != NOT_EMPLOYED )
            {
                // Display 'Select Employer' screen if 
                // a) You are selecting the Employment status for a new patient for the first time (or)
                // b) A valid Employment status is selected but no employer information is available (or)
                // c) You are changing an existing Employment status to a new valid status.

                if ((lblEmployerAddress.Text != string.Empty) && priorEmploymentStatus == null || 
                     ( ( priorEmploymentStatus != null ) &&
                       ( cmbEmploymentStatus.Text.Trim() != priorEmploymentStatus.Description.Trim() ) ) )
                {
                    OnSelectEmployer();
                }
                priorEmploymentStatus = Model_Account.Patient.Employment.Status;
            }
        }

        private void cmbEmploymentStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( !loadingModelData )
            {
                // Get the employer statu and set the corresponding controls state
                selectedStatus = cmbEmploymentStatus.SelectedItem as EmploymentStatus;

                if ( selectedStatus != null )
                {
                    // TLG 08/02/2006 CR0371 - Employer name defaulted to Employer Status if the status is:
                    //   a. Retired
                    //   b. Not-Employed
                    //   c. Self-Employed

                    if ( lblEmployerAddress.Text.Replace( "-", " " ).Replace( "\r\n", "" ) == EmploymentStatus.NewNotEmployed().Description.Replace( "-", " " )
                        || lblEmployerAddress.Text.Replace( "-", " " ).Replace( "\r\n", "" ) == EmploymentStatus.NewRetired().Description.Replace( "-", " " )
                        || lblEmployerAddress.Text.Replace( "-", " " ).Replace( "\r\n", "" ) == EmploymentStatus.NewSelfEmployed().Description.Replace( "-", " " ) )
                    {
                        Model_Account.Patient.Employment.Employer = new Employer();
                        lblEmployerAddress.Text = string.Empty;
                    }

                    // if PBAR had a name initially, put it back

                    if ( selectedStatus.Code == EmploymentStatus.NOT_EMPLOYED_CODE )
                    {
                        Model_Account.Patient.Employment.Employer = new Employer { Name = selectedStatus.Description };
                        lblEmployerAddress.Text = string.Empty;
                        lblEmployerAddress.Text = selectedStatus.Description;

                        phoneNumberControl.AreaCode = string.Empty;
                        phoneNumberControl.PhoneNumber = string.Empty;
                        phoneNumberControl.Model = new PhoneNumber();

                        mtbIndustry.Text = string.Empty;
                        Model_Account.Patient.Employment.Occupation = string.Empty;
                        mtbEmployeeID.Text = string.Empty;
                        Model_Account.Patient.Employment.EmployeeID = string.Empty;

                        if ( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address != null )
                        {
                            savedAddressOfEmployment = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address;
                            Model_Account.Patient.Employment.Employer.PartyContactPoint.Address = null;
                        }
                    }
                    else if ( selectedStatus.Code == EmploymentStatus.RETIRED_CODE )
                    {
                        Model_Account.Patient.Employment.Employer = new Employer();
                        Model_Account.Patient.Employment.Employer.Name = selectedStatus.Description;
                        lblEmployerAddress.Text = string.Empty;
                        lblEmployerAddress.Text = selectedStatus.Description;

                        phoneNumberControl.AreaCode = string.Empty;
                        phoneNumberControl.PhoneNumber = string.Empty;

                        mtbIndustry.Text = string.Empty;
                        mtbEmployeeID.Text = string.Empty;
                    }
                    else if ( selectedStatus.Code == EmploymentStatus.SELF_EMPLOYED_CODE )
                    {
                        Model_Account.Patient.Employment.Employer = new Employer();
                        Model_Account.Patient.Employment.Employer.Name = selectedStatus.Description;
                        lblEmployerAddress.Text = string.Empty;
                        lblEmployerAddress.Text = selectedStatus.Description;

                        phoneNumberControl.AreaCode = string.Empty;
                        phoneNumberControl.PhoneNumber = string.Empty;

                        mtbIndustry.Text = string.Empty;
                        mtbEmployeeID.Text = string.Empty;
                    }
                    
                    if ( Model_Account.Patient.Employment != null )
                    {
                        Model_Account.Patient.Employment.Status = selectedStatus;
                    }

                }
                else
                {
                    if ( Model_Account.Patient.Employment != null )
                    {
                        Model_Account.Patient.Employment.Status = null;

                        if ( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address != null )
                        {
                            savedAddressOfEmployment = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address;
                            Model_Account.Patient.Employment.Employer.PartyContactPoint.Address = null;
                        }
                    }
                }
                employmentViewPresenter.SetControlStatesOnEmploymentStatus();
                CheckForRequiredFields();

                if ( PatientEmploymentChanged != null )
                {
                    PatientEmploymentChanged( this, null );
                }
            }
        }

        private void mtbEmployeeID_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            Model_Account.Patient.Employment.EmployeeID = mtb.Text.Trim();
        }

        private void mtbIndustry_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            Model_Account.Patient.Employment.Occupation = mtb.UnMaskedText.Trim();
            CheckForRequiredFields();
        }

        private void SelectEmployerButtonClick( object sender, EventArgs e )
        {
            BreadCrumbLogger.GetInstance.Log( "Selecting employer for " + Model_Account.Patient.FormattedName );
            OnSelectEmployer();
        }

        private void buttonClear_Click( object sender, EventArgs e )
        {
            ClearEmployerAddress();

            Model_Account.Patient.Employment = new Employment { Status = cmbEmploymentStatus.SelectedItem as EmploymentStatus };

            CheckForRequiredFields();

            if ( PatientEmploymentChanged != null )
            {
                PatientEmploymentChanged( this, null );
            }
        }
        #endregion

        #region Rules Event Handlers

        private void EmploymentStatusRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( cmbEmploymentStatus );
        }

        private void EmploymentStatusPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbEmploymentStatus );
        }

        public void OCCIndustryPreferredEventHandler( object sender, EventArgs e )
        {
            if ( mtbIndustry.CanFocus )
            {
                UIColors.SetPreferredBgColor( mtbIndustry );
            }
        }
        #endregion

        #region Methods


        public override void UpdateView()
        {
            if ( loadingModelData )
            {
                PopulateEmploymentStatusControl();

                if ( Model_Account.Patient.Employment.Status != null )
                {
                    savedEmploymentStatus = Model_Account.Patient.Employment.Status;
                    cmbEmploymentStatus.SelectedItem = Model_Account.Patient.Employment.Status;

                    BuildEmploymentAddress();
                    mtbIndustry.UnMaskedText = Model_Account.Patient.Employment.Occupation;
                    mtbEmployeeID.Text = Model_Account.Patient.Employment.EmployeeID;
                    
                    if ( Model_Account.Patient.Employment.Status.Code == EmploymentStatus.NOT_EMPLOYED_CODE ||
                        Model_Account.Patient.Employment.Status.Code.Trim() == string.Empty )
                    {
                        phoneNumberControl.AreaCode = string.Empty;
                        phoneNumberControl.PhoneNumber = string.Empty;
                        mtbIndustry.Text = string.Empty;
                        mtbEmployeeID.Text = string.Empty;
                    }
                }
                else
                {
                    BuildEmploymentAddress();
                }
                employmentViewPresenter.SetControlStatesOnEmploymentStatus();
                RegisterEvents();
            }
            else
            {
                BuildEmploymentAddress();
            }
            loadingModelData = false;
            CheckForRequiredFields();
        }

        public void EnableClearButton(bool state)
        {
            buttonClear.Enabled = state;
        }

        public void SetControlState(bool state)
        {
            btnSelectEmployer.Enabled = state;
            phoneNumberControl.ToggleEnabled(state);
            mtbIndustry.Enabled = state;
            mtbEmployeeID.Enabled = state;
        }
        #endregion

        #region Properties

        public object AddressArea
        {
            get
            {
                return lblEmployerAddress;
            }
        }

        public Account Model_Account
        {
            private get
            {
                return ( Account )Model;
            }
            set
            {
                Model = value;
            }
        }

        public PatientAccessComboBox ComboBox
        {
            get
            {
                return cmbEmploymentStatus;
            }
        }

        public Employment Model_Employment
        {
            get
            {
                if (Model_Account != null && Model_Account.Patient!=null 
                        && Model_Account.Patient.Employment!=null)
                {

                    return Model_Account.Patient.Employment;
                }
                
                return null;

            }
            set
            {
                if (Model_Account != null && Model_Account.Patient != null)
                {
                    Model_Account.Patient.Employment = value;
                }
            }

        }
        private RuleEngine RuleEngine
        {
            get { return i_RuleEngine ?? ( i_RuleEngine = RuleEngine.GetInstance() ); }
        }

        #endregion

        #region Private Methods

        private void ClearEmployerAddress()
        {
            lblEmployerAddress.Text = String.Empty;

            phoneNumberControl.AreaCode = string.Empty;
            phoneNumberControl.PhoneNumber = string.Empty;
            phoneNumberControl.Model = new PhoneNumber( string.Empty, string.Empty );
            phoneNumberControl.SetNormalColor();

            mtbIndustry.Text = String.Empty;
            UIColors.SetNormalBgColor( mtbIndustry );
            mtbEmployeeID.Text = String.Empty;

            if ( Model_Account != null && Model_Account.Patient != null &&
                Model_Account.Patient.Employment != null )
            {
                employmentViewPresenter.SetControlStatesOnEmploymentStatus();
            }

            Refresh();
        }

        private void BuildEmploymentAddress()
        {
            if ( Model_Account.Patient.Employment.Employer == null ||
                Model_Account.Patient.Employment.Employer.PartyContactPoint == null )
            {
                return;
            }
            const string NEWLINE_TAG = "\r\n";
            const string CITYSTATE_DELIMITER = ", ";
            const string SPACE_DELIMITER = " ";
            StringBuilder msg = new StringBuilder();

            lblEmployerAddress.ResetText();
            Address addr = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address;

            msg.Append( Model_Account.Patient.Employment.Employer.Name );

            if ( addr != null )
            {
                if ( addr.Address1 != null && addr.Address1.Length > 1 )
                {
                    msg.Append( NEWLINE_TAG );
                    msg.Append( addr.Address1 );
                }
                if ( addr.Address2 != null && addr.Address2.Length > 1 )
                {
                    msg.Append( NEWLINE_TAG );
                    msg.Append( addr.Address2 );
                }
                string statestr = String.Empty;
                if ( addr.State != null && addr.State.ToString().Length > 1 )
                {   // Defect 3727: State was appearing as incomplete string causing display of incomplete string
                    statestr = addr.State.ToString();

                    if ( ( addr.City.Length + statestr.Length + addr.ZipCode.PostalCode.Length ) > 0 )
                    {
                        msg.Append( NEWLINE_TAG );
                        if ( addr.City.Length > 0 )
                        {
                            msg.Append( addr.City );
                        }
                        if ( addr.State.PrintString.Length > 0 )
                        {
                            msg.Append( CITYSTATE_DELIMITER );
                            msg.Append( addr.State );
                        }
                        if ( addr.ZipCode.PostalCode.Length > 0 )
                        {
                            msg.Append( SPACE_DELIMITER );
                            msg.Append( addr.ZipCode.FormattedPostalCodeFor( addr.IsUnitedStatesAddress() ) );
                        }
                    }
                    if ( addr.Country != null && addr.Country.PrintString.Length > 0 )
                    {
                        msg.Append( NEWLINE_TAG );
                        msg.Append( addr.Country.PrintString );
                    }
                }
            }

            if ( Model_Account.Patient.Employment.Employer.PartyContactPoint.PhoneNumber != null )
            {
                phoneNumberControl.Model = Model_Account.Patient.Employment.Employer.PartyContactPoint.PhoneNumber;
            }
            lblEmployerAddress.Text = msg.ToString();
        }

        private void UpdatePhoneNumberOnModel()
        {
            if ( Model_Account != null && Model_Account.Patient != null &&
                 Model_Account.Patient.Employment != null &&
                 Model_Account.Patient.Employment.Employer != null &&
                 Model_Account.Patient.Employment.Employer.PartyContactPoint != null &&
                 Model_Account.Patient.Employment.Employer.PartyContactPoint.PhoneNumber != null )
            {
                Model_Account.Patient.Employment.Employer.PartyContactPoint.PhoneNumber = phoneNumberControl.Model;
            }
        }

        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            UIColors.SetNormalBgColor( cmbEmploymentStatus );
            phoneNumberControl.SetNormalColor();
            UIColors.SetNormalBgColor( mtbIndustry );

            Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentStatusRequired ), Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( EmploymentStatusPreferred ), Model );

            phoneNumberControl.RunRules();
        }

        private void OnSelectEmployer()
        {
            FormSelectEmployer formSelectEmployer = new FormSelectEmployer { Activity = Model_Account.Activity };

            try
            {
                formSelectEmployer.ShowDialog( this );

                if ( formSelectEmployer.DialogResult == DialogResult.OK )
                {
                    if ( Model_Account.Patient.Employment == null )
                    {
                        Model_Account.Patient.Employment = new Employment();
                    }

                    Model_Account.Patient.Employment.Employer = formSelectEmployer.SelectedEmployer;

                    if ( formSelectEmployer.SelectedContactPoint.Address.Address1.Length > 0 )
                    {
                        Model_Account.Patient.Employment.Employer.PartyContactPoint = formSelectEmployer.SelectedContactPoint;
                    }

                    UpdateView();

                    if ( PatientEmploymentChanged != null )
                    {
                        PatientEmploymentChanged( this, null );
                    }
                }
            }
            finally
            {
                formSelectEmployer.Dispose();
                phoneNumberControl.FocusAreaCode();
                SelectNextControl( phoneNumberControl, true, true, false, true );
            }
        }

        private void PopulateEmploymentStatusControl()
        {
            IEmploymentStatusBroker broker = new EmploymentStatusBrokerProxy();
            ICollection employmentStatuses = broker.AllTypesOfEmploymentStatuses( User.GetCurrent().Facility.Oid );

            cmbEmploymentStatus.Items.Clear();

            foreach ( EmploymentStatus employmentStatus in employmentStatuses )
            {
                cmbEmploymentStatus.Items.Add( employmentStatus );
            }
            SetControlState( false );
        }

        

        private void RegisterEvents()
        {
            if ( i_Registered )
            {
                return;
            }

            i_Registered = true;

            RuleEngine.GetInstance().RegisterEvent( typeof( EmploymentStatusRequired ),
                Model, new EventHandler( EmploymentStatusRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( EmploymentStatusPreferred ),
                Model, new EventHandler( EmploymentStatusPreferredEventHandler ) );
        }

        private void UnRegisterEvents()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( EmploymentStatusRequired ),
                Model, EmploymentStatusRequiredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( EmploymentStatusPreferred ),
                Model, EmploymentStatusPreferredEventHandler );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployeeID( mtbEmployeeID );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PatientEmploymentView ) );
            this.grpPatientEmployment = new System.Windows.Forms.GroupBox();
            this.buttonClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblEmployerAddress = new System.Windows.Forms.Label();
            this.mtbEmployeeID = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbIndustry = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblEmployeeID = new System.Windows.Forms.Label();
            this.lblIndustry = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.btnSelectEmployer = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblStaticRetiredOn = new System.Windows.Forms.Label();
            this.cmbEmploymentStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.grpPatientEmployment.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpPatientEmployment
            // 
            this.grpPatientEmployment.Controls.Add( this.phoneNumberControl );
            this.grpPatientEmployment.Controls.Add( this.buttonClear );
            this.grpPatientEmployment.Controls.Add( this.lblEmployerAddress );
            this.grpPatientEmployment.Controls.Add( this.mtbEmployeeID );
            this.grpPatientEmployment.Controls.Add( this.mtbIndustry );
            this.grpPatientEmployment.Controls.Add( this.lblEmployeeID );
            this.grpPatientEmployment.Controls.Add( this.lblIndustry );
            this.grpPatientEmployment.Controls.Add( this.lblPhone );
            this.grpPatientEmployment.Controls.Add( this.btnSelectEmployer );
            this.grpPatientEmployment.Controls.Add( this.lblStaticRetiredOn );
            this.grpPatientEmployment.Controls.Add( this.cmbEmploymentStatus );
            this.grpPatientEmployment.Controls.Add( this.lblStatus );
            this.grpPatientEmployment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPatientEmployment.Location = new System.Drawing.Point( 0, 0 );
            this.grpPatientEmployment.Name = "grpPatientEmployment";
            this.grpPatientEmployment.Size = new System.Drawing.Size( 312, 280 );
            this.grpPatientEmployment.TabIndex = 1;
            this.grpPatientEmployment.TabStop = false;
            this.grpPatientEmployment.Text = "Patient employment";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point( 115, 50 );
            this.buttonClear.Message = null;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size( 65, 23 );
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new System.EventHandler( this.buttonClear_Click );
            // 
            // lblEmployerAddress
            // 
            this.lblEmployerAddress.Location = new System.Drawing.Point( 8, 82 );
            this.lblEmployerAddress.Name = "lblEmployerAddress";
            this.lblEmployerAddress.Size = new System.Drawing.Size( 296, 88 );
            this.lblEmployerAddress.TabIndex = 0;
            // 
            // mtbEmployeeID
            // 
            this.mtbEmployeeID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEmployeeID.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEmployeeID.Location = new System.Drawing.Point( 87, 242 );
            this.mtbEmployeeID.MaxLength = 11;
            this.mtbEmployeeID.Name = "mtbEmployeeID";
            this.mtbEmployeeID.Size = new System.Drawing.Size( 114, 20 );
            this.mtbEmployeeID.TabIndex = 6;
            this.mtbEmployeeID.Validating += new System.ComponentModel.CancelEventHandler( this.mtbEmployeeID_Validating );
            // 
            // mtbIndustry
            // 
            this.mtbIndustry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbIndustry.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbIndustry.Location = new System.Drawing.Point( 87, 210 );
            this.mtbIndustry.Mask = "";
            this.mtbIndustry.MaxLength = 14;
            this.mtbIndustry.Name = "mtbIndustry";
            this.mtbIndustry.Size = new System.Drawing.Size( 182, 20 );
            this.mtbIndustry.TabIndex = 5;
            this.mtbIndustry.Validating += new System.ComponentModel.CancelEventHandler( this.mtbIndustry_Validating );
            // 
            // lblEmployeeID
            // 
            this.lblEmployeeID.Location = new System.Drawing.Point( 15, 245 );
            this.lblEmployeeID.Name = "lblEmployeeID";
            this.lblEmployeeID.Size = new System.Drawing.Size( 75, 23 );
            this.lblEmployeeID.TabIndex = 0;
            this.lblEmployeeID.Text = "Employee ID:";
            // 
            // lblIndustry
            // 
            this.lblIndustry.Location = new System.Drawing.Point( 15, 213 );
            this.lblIndustry.Name = "lblIndustry";
            this.lblIndustry.Size = new System.Drawing.Size( 75, 23 );
            this.lblIndustry.TabIndex = 0;
            this.lblIndustry.Text = "Occ/Industry:";
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point( 15, 181 );
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size( 47, 23 );
            this.lblPhone.TabIndex = 0;
            this.lblPhone.Text = "Phone:";
            // 
            // btnSelectEmployer
            // 
            this.btnSelectEmployer.Location = new System.Drawing.Point( 8, 50 );
            this.btnSelectEmployer.Message = null;
            this.btnSelectEmployer.Name = "btnSelectEmployer";
            this.btnSelectEmployer.Size = new System.Drawing.Size( 100, 23 );
            this.btnSelectEmployer.TabIndex = 2;
            this.btnSelectEmployer.Text = "Select Employer...";
            this.btnSelectEmployer.Click += new System.EventHandler( this.SelectEmployerButtonClick );
            // 
            // lblStaticRetiredOn
            // 
            this.lblStaticRetiredOn.Location = new System.Drawing.Point( 8, 53 );
            this.lblStaticRetiredOn.Name = "lblStaticRetiredOn";
            this.lblStaticRetiredOn.Size = new System.Drawing.Size( 60, 23 );
            this.lblStaticRetiredOn.TabIndex = 0;
            this.lblStaticRetiredOn.Text = "Retired on:";
            // 
            // cmbEmploymentStatus
            // 
            this.cmbEmploymentStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmploymentStatus.Location = new System.Drawing.Point( 56, 18 );
            this.cmbEmploymentStatus.Name = "cmbEmploymentStatus";
            this.cmbEmploymentStatus.Size = new System.Drawing.Size( 245, 21 );
            this.cmbEmploymentStatus.TabIndex = 1;
            this.cmbEmploymentStatus.Validating += new System.ComponentModel.CancelEventHandler( this.cmbEmploymentStatus_Validating );
            this.cmbEmploymentStatus.SelectedIndexChanged += new System.EventHandler( this.cmbEmploymentStatus_SelectedIndexChanged );
            this.cmbEmploymentStatus.DropDown += new System.EventHandler( this.cmbEmploymentStatus_DropDown );
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point( 8, 21 );
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size( 60, 23 );
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status:";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = "";
            this.phoneNumberControl.Location = new System.Drawing.Point( 84, 175 );
            this.phoneNumberControl.Model = ( ( PatientAccess.Domain.Parties.PhoneNumber )( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = "";
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 4;
            // 
            // PatientEmploymentView
            // 
            this.Controls.Add( this.grpPatientEmployment );
            this.Name = "PatientEmploymentView";
            this.Size = new System.Drawing.Size( 312, 280 );
            this.Disposed += new System.EventHandler( this.PatientEmploymentView_Disposed );
            this.Leave += new EventHandler( PatientEmploymentView_Leave );
            this.grpPatientEmployment.ResumeLayout( false );
            this.grpPatientEmployment.PerformLayout();
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public PatientEmploymentView()
        {
            loadingModelData = true;
            employmentViewPresenter = new EmploymentViewPresenter(this);
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
        private Container components = null;

        private LoggingButton btnSelectEmployer;
        private LoggingButton buttonClear;

        private PatientAccessComboBox cmbEmploymentStatus;

        private GroupBox grpPatientEmployment;

        private Label lblEmployeeID;
        private Label lblIndustry;
        private Label lblPhone;
        private Label lblStatus;
        private Label lblStaticRetiredOn;
        private Label lblEmployerAddress;
        private MaskedEditTextBox mtbIndustry;
        private MaskedEditTextBox mtbEmployeeID;

        private bool loadingModelData;
        private bool i_Registered;
        private Address savedAddressOfEmployment;
        private EmploymentStatus selectedStatus;
        private EmploymentStatus savedEmploymentStatus;
        private RuleEngine i_RuleEngine;
        private EmploymentViewPresenter employmentViewPresenter;
        private EmploymentStatus priorEmploymentStatus;
        #endregion
        private PhoneNumberControl phoneNumberControl;

        #region Constants
        private const string NOT_EMPLOYED = "NOT-EMPLOYED";
        #endregion


        
    }
}
