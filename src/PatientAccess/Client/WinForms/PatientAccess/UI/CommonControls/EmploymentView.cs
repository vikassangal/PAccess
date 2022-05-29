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
using PatientAccess.UI.EmploymentViews;

namespace PatientAccess.UI.CommonControls
{
    /// <summary>
    /// Summary description for EmploymentView.
    /// </summary>
    public class EmploymentView : ControlView, IEmploymentView
    {
        #region Events
        public event EventHandler EmploymentViewChangedEvent;
        public event CancelEventHandler EmploymentStatusValidating;
        #endregion

        #region Event Handlers

        private void btnEditEmployer_Click( object sender, EventArgs e )
        {
            this.OnEditEmployer();

            if( this.EmploymentViewChangedEvent != null )
            {
                this.EmploymentViewChangedEvent(this, null);
            }
        }

        private void OnEditEmployer()
        {
            IInsuranceBroker broker = 
                BrokerFactory.BrokerOfType<IInsuranceBroker>();

             InsurancePlan plan = Model_Account.Insurance.CoverageFor( 
                 this.CoverageOrderID ).InsurancePlan;

           int coveredGroupCount = broker.GetCoveredGroupCountFor(plan.PlanID, plan.EffectiveOn, plan.ApprovedOn, Model_Account.Facility.Oid, Model_Account.AdmitDate );

            FormSelectEmployer formSelectEmployer = new FormSelectEmployer();

            if( Model_Account != null && Model_Account.Insurance!= null && 
                ( Model_Account.Insurance.CoverageFor( 
                this.CoverageOrderID ).InsurancePlan.PlanID != null ) &&
                coveredGroupCount > 0 )                
            {
                formSelectEmployer.Account =  Model_Account;
                formSelectEmployer.SetCoveredGroupsAsDefaultTabPage();
                formSelectEmployer.UpdateView();
            }

			try
			{
                if(Model_Account != null && Model_Account.Activity != null)
                {
                    formSelectEmployer.Activity = Model_Account.Activity;
                }
				formSelectEmployer.ShowDialog( this );

				if( formSelectEmployer.DialogResult == DialogResult.OK )
				{
					if( Model_Employment == null )
					{
						Model_Employment = savedEmployment;
					}

					//Model_Employment.Employer = formSelectEmployer.Model_Employer;
					if( formSelectEmployer.Model_Employer != null )
					{
						Model_Employment.Employer = formSelectEmployer.Model_Employer;
					}
					else
					{
						Model_Employment.Employer = formSelectEmployer.SelectedEmployer;
					}

                    if (Model_Employment.Employer != null)
                    {
                        if (formSelectEmployer.SelectedContactPoint.Address.Address1.Length > 0)
                        {
                            Model_Employment.Employer.PartyContactPoint = formSelectEmployer.SelectedContactPoint;
                        }
                        else if ((formSelectEmployer.SelectedEmployerContactPoint.Address.Address1.Length > 0) &&
                            (!formSelectEmployer.SelectNoAddressChecked))
                        {
                            Model_Employment.Employer.PartyContactPoint =
                                formSelectEmployer.SelectedEmployerContactPoint;
                        }
                    }

                    this.BuildEmploymentAddress();
                    this.UpdateEmploymentPhone();
				}
			}
			finally
			{
				formSelectEmployer.Dispose();
			}
        }

        private void cmbEmploymentStatus_SelectedIndexChanged( object sender, EventArgs e )
        {
            EmploymentStatus selectedStatus = this.cmbEmploymentStatus.SelectedItem as EmploymentStatus;
            
            if (this.priorEmploymentStatus != null &&
                selectedStatus != null &&
                this.priorEmploymentStatus.Code == selectedStatus.Code)
            {
                return;
            }

            SetPhoneNormalBgColor();
            SetEmploymentNormalBgColor();

            if( selectedStatus != null )
            {
                // TLG 08/02/2006 CR0371 - Employer name defaulted to Employer Status if the status is:
                //   a. Retired
                //   b. Not-Employed
                //   c. Self-Employed
           
                if( selectedStatus.Code == EmploymentStatus.NOT_EMPLOYED_CODE 
                    || selectedStatus.Code == EmploymentStatus.RETIRED_CODE
                    || selectedStatus.Code == EmploymentStatus.SELF_EMPLOYED_CODE )
                {
                    if (this.priorEmploymentStatus != null)
                    {
                        this.Model_Employment.Employer = new Employer();
                        this.Model_Employment.Employer.Name = selectedStatus.Description;
                        this.txtEmployerAddress.Text = selectedStatus.Description;
                        this.phoneNumberControl.AreaCode = string.Empty;
                        this.phoneNumberControl.PhoneNumber = string.Empty;
                    }
                }               
                else if( selectedStatus.Code.Trim() == string.Empty )
                {
                    if (this.Model_Employment != null && this.Model_Employment.Employer != null && this.Model_Employment.Employer.Name != null)
                    {
                        this.Model_Employment.Employer.Name = string.Empty;
                    }
                }
                else
                {
                    if ( this.txtEmployerAddress.Text.Replace("-", " ").Replace("\r\n", string.Empty) == EmploymentStatus.NOT_EMPLOYED_DESC
                         || this.txtEmployerAddress.Text.Replace("-", " ").Replace("\r\n", string.Empty) == EmploymentStatus.RETIRED_DESC 
                         || this.txtEmployerAddress.Text.Replace("-", " ").Replace("\r\n", string.Empty) == EmploymentStatus.SELF_EMPLOYED_DESC ) 
                    {
                        this.txtEmployerAddress.Text = string.Empty;

                        if (this.Model_Employment.Employer.Name.Replace("-", " ") == EmploymentStatus.NOT_EMPLOYED_DESC
                            || this.Model_Employment.Employer.Name.Replace("-", " ") == EmploymentStatus.RETIRED_DESC
                            || this.Model_Employment.Employer.Name.Replace("-", " ") == EmploymentStatus.SELF_EMPLOYED_DESC)
                        {
                            this.Model_Employment.Employer.Name = string.Empty;
                        }                        
                    }
                }
                
                if( this.Model_Employment != null )
                {
                    this.Model_Employment.Status = selectedStatus;
                    this.priorEmploymentStatus = this.Model_Employment.Status;
                }

                BuildEmploymentAddress();
                UpdateEmploymentPhone();               
            }
            else
            {
                if( Model_Employment != null )
                {
                    Model_Employment.Status = null;

                    if( Model_Employment.Employer != null
                        && Model_Employment.Employer.PartyContactPoint != null
                        && Model_Employment.Employer.PartyContactPoint.Address != null )
                    {
                        Model_Employment.Employer.PartyContactPoint.Address = null;
                    }
                }
            }
            employmentView.SetControlStatesOnEmploymentStatus();

            // OTD# 37701 fix for - SR39518: Warning message to validate insurance
            // on employment change comes up twice when 'Clear All' is clicked
            if( !isResetClickedOnInsuredTab )
            {
                if( this.EmploymentViewChangedEvent != null )
                {
                    this.EmploymentViewChangedEvent(this, null);
                }
            }
        }

        private void cmbEmploymentStatus_Validating(object sender, CancelEventArgs e)
        {
            if( this.cmbEmploymentStatus.Text.Trim() != string.Empty
                && this.cmbEmploymentStatus.Text != NOT_EMPLOYED
                && this.cmbEmploymentStatus.SelectedItem as EmploymentStatus != this.prevValidatingStatus )
            {
                this.prevValidatingStatus = this.Model_Employment.Status;
                OnEditEmployer();
            }

            if( this.EmploymentStatusValidating != null )
            {
                this.EmploymentStatusValidating( this, null );        
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            this.phoneNumberControl.AreaCode = string.Empty;
            this.phoneNumberControl.PhoneNumber = string.Empty;
            this.txtEmployerAddress.Clear();
           
            this.Model_Employment           = new Employment();
            this.Model_Employment.Status = this.cmbEmploymentStatus.SelectedItem as EmploymentStatus;

            if( this.EmploymentViewChangedEvent != null )
            {
                this.EmploymentViewChangedEvent(this, null);
            }        

        }

        private void phoneNumberControl_Changed(object sender, EventArgs e)
        {
            if (this.EmploymentViewChangedEvent != null)
            {
                this.EmploymentViewChangedEvent(this, null);
            }
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( this.loadingModelData )
            {
                this.loadingModelData = false;
                this.InitializeEmploymentStatus();
            }

            if( this.Model_Employment != null )
            {                
                if( Model_Employment.Status == null )
                {
                    Model_Employment.Status = new EmploymentStatus();
                }

                // unplug SelectedIndexChanged event handler temporarily
                // and call the method explicitly
                // since copy to insured from may not trigger event
                this.priorEmploymentStatus = null;
                this.cmbEmploymentStatus.SelectedIndexChanged -= new EventHandler(this.cmbEmploymentStatus_SelectedIndexChanged);                    
                this.cmbEmploymentStatus.SelectedItem = Model_Employment.Status;
                this.cmbEmploymentStatus.SelectedIndexChanged += new EventHandler(this.cmbEmploymentStatus_SelectedIndexChanged);
                    
                this.cmbEmploymentStatus_SelectedIndexChanged(null, null);
            }        
        }

        private void UpdateEmploymentPhone()
        {
            if (Model_Employment.Employer != null &&
                Model_Employment.Employer.PartyContactPoint != null
                && Model_Employment.Employer.PartyContactPoint.PhoneNumber != null)
            {
                this.phoneNumberControl.Model = Model_Employment.Employer.PartyContactPoint.PhoneNumber;
                this.phoneNumberControl.RunRules();
            }
        }

        public void ResetView( bool isResetFromInsuredTab )
        {
            if( Model_Employment != null )
            {
                savedEmployment = Model_Employment;
                //Model_Employment = null;
                Model_Employment = new Employment();
            }

            this.phoneNumberControl.AreaCode = string.Empty;
            this.phoneNumberControl.PhoneNumber = string.Empty;
            SetPhoneNormalBgColor();
            txtEmployerAddress.Clear();
            
            // OTD# 37701 fix for - SR39518: Warning message to validate insurance
            // on employment change comes up twice when 'Clear All' is clicked
            isResetClickedOnInsuredTab = isResetFromInsuredTab;
            ResetEmploymentStatusComboBox();
            isResetClickedOnInsuredTab = false;
            UpdateView();
        }

        public void SetControlState(bool state)
        {
            btnEditEmployer.Enabled = state;
            phoneNumberControl.ToggleEnabled(state);
        }


        public void EnableClearButton(bool state)
        {
            buttonClear.Enabled = state;
        }
        
        #endregion

        #region Properties

        public PhoneNumberControl PhoneNumberControl
        {
            get
            {
                return this.phoneNumberControl;
            }
        }

        public Account Model_Account
        {
            private get
            {
                return this.i_Model_Account;
            }
            set
            {
                this.i_Model_Account = value;
            }
        }

        public Employment Model_Employment
        {
            get
            {
                if( this.Model != null )
                {
                    return (Employment)this.Model;
                }
                else
                    return null;
                
            }
            set
            {
                this.Model = value;
            }
        }

        public long CoverageOrderID
        {
            private get
            {
                return this.i_CoverageOrderID;
            }
            set
            {
                this.i_CoverageOrderID = value;
            }
        }

        public PatientAccessComboBox EmploymentStatusComboBox
        {   // Used by the preferred or required field state routine 
            get
            {
                return this.cmbEmploymentStatus;
            }
        }

        public object EmployerField
        {
            get
            {
                return this.txtEmployerAddress;
            }
        }

        #endregion

        #region Private Methods
        private void ResetEmploymentStatusComboBox()
        {
            cmbEmploymentStatus.SelectedIndex = -1;
            btnEditEmployer.Enabled = false;
        }

        private void SetPhoneNormalBgColor()
        {
            phoneNumberControl.SetNormalColor();
            Refresh();
        }

        private void SetEmploymentNormalBgColor()
        {
            UIColors.SetNormalBgColor( txtEmployerAddress );
            Refresh();
        }

        private void InitializeEmploymentStatus()
        {
            IEmploymentStatusBroker broker = new EmploymentStatusBrokerProxy() ; 
            ICollection employmentStatuses = broker.AllTypesOfEmploymentStatuses( User.GetCurrent().Facility.Oid );

            this.cmbEmploymentStatus.Items.Clear();
            foreach( EmploymentStatus employmentStatus in employmentStatuses )
            {
                this.cmbEmploymentStatus.Items.Add( employmentStatus );
            }
        }

        private void BuildEmploymentAddress()
        {
            if( Model_Employment == null
                || Model_Employment.Employer == null
                || Model_Employment.Employer.PartyContactPoint == null
                || Model_Employment.Employer.PartyContactPoint.Address == null )
            {
                txtEmployerAddress.ResetText();
                return;
            }
            const string NEWLINE_TAG         = "\r\n";
            const string CITYSTATE_DELIMITER = ", ";
            const string SPACE_DELIMITER     = " ";
            StringBuilder msg = new StringBuilder();

            txtEmployerAddress.ResetText();
            Address addr = Model_Employment.Employer.PartyContactPoint.Address;

            if( addr == null )
            {
                return;
            }

            if( Model_Employment.Employer != null 
                && Model_Employment.Employer.Name != null )
            {
                msg.Append( Model_Employment.Employer.Name );
                msg.Append( NEWLINE_TAG );
            }

            msg.Append( addr.Address1 );

            if( addr.Address2 != null && addr.Address2.Length > 0 )
            {
                msg.Append( NEWLINE_TAG );
                msg.Append( addr.Address2 );
            }

            string statestr = String.Empty;
            if( addr.State != null )
            {
                statestr = addr.State.PrintString;
            }

            if( (addr.City.Length + statestr.Length + addr.ZipCode.PostalCode.Length) > 0 )
            {
                msg.Append( NEWLINE_TAG );
                if( addr.City.Length > 0 )
                {
                    msg.Append( addr.City );
                }

                if( addr.State != null
                    && addr.State.PrintString.Length > 0 )
                {
                    msg.Append( CITYSTATE_DELIMITER );
                    msg.Append( addr.State );
                }

                if( addr.ZipCode.PostalCode.Length > 0 )
                {
                    msg.Append( SPACE_DELIMITER );
                    msg.Append( addr.ZipCode.ZipCodePrimary + addr.ZipCode.ZipCodeExtended );
                }
            }

            if( addr.Country != null && addr.Country.PrintString.Length > 0 )
            {
                msg.Append( NEWLINE_TAG );
                msg.Append( addr.Country.PrintString );
            }

            if( Model_Employment.Employer.PartyContactPoint != null
                && Model_Employment.Employer.PartyContactPoint.PhoneNumber != null )
            {
                phoneNumberControl.Model = Model_Employment.Employer.PartyContactPoint.PhoneNumber;
            }

            txtEmployerAddress.Text = msg.ToString();
            SetEmploymentNormalBgColor();
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( EmploymentView ) );
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.midPanel = new System.Windows.Forms.Panel();
            this.txtEmployerAddress = new System.Windows.Forms.TextBox();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.lblStaticPhone = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.Panel();
            this.buttonClear = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditEmployer = new PatientAccess.UI.CommonControls.LoggingButton();
            this.cmbEmploymentStatus = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblStaticStatus = new System.Windows.Forms.Label();
            this.phoneNumberControl = new PatientAccess.UI.CommonControls.PhoneNumberControl();
            this.groupBox.SuspendLayout();
            this.midPanel.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add( this.midPanel );
            this.groupBox.Controls.Add( this.bottomPanel );
            this.groupBox.Controls.Add( this.topPanel );
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.groupBox.Location = new System.Drawing.Point( 0, 0 );
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size( 360, 225 );
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Employment";
            // 
            // midPanel
            // 
            this.midPanel.Controls.Add( this.txtEmployerAddress );
            this.midPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.midPanel.Location = new System.Drawing.Point( 3, 81 );
            this.midPanel.Name = "midPanel";
            this.midPanel.Size = new System.Drawing.Size( 354, 91 );
            this.midPanel.TabIndex = 0;
            // 
            // txtEmployerAddress
            // 
            this.txtEmployerAddress.BackColor = System.Drawing.Color.White;
            this.txtEmployerAddress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEmployerAddress.Location = new System.Drawing.Point( 5, 0 );
            this.txtEmployerAddress.Multiline = true;
            this.txtEmployerAddress.Name = "txtEmployerAddress";
            this.txtEmployerAddress.ReadOnly = true;
            this.txtEmployerAddress.Size = new System.Drawing.Size( 345, 91 );
            this.txtEmployerAddress.TabIndex = 0;
            this.txtEmployerAddress.TabStop = false;
            this.txtEmployerAddress.WordWrap = false;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add( this.phoneNumberControl );
            this.bottomPanel.Controls.Add( this.lblStaticPhone );
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.bottomPanel.Location = new System.Drawing.Point( 3, 172 );
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size( 354, 50 );
            this.bottomPanel.TabIndex = 1;
            // 
            // lblStaticPhone
            // 
            this.lblStaticPhone.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStaticPhone.Location = new System.Drawing.Point( 5, 19 );
            this.lblStaticPhone.Name = "lblStaticPhone";
            this.lblStaticPhone.Size = new System.Drawing.Size( 42, 23 );
            this.lblStaticPhone.TabIndex = 0;
            this.lblStaticPhone.Text = "Phone:";
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add( this.buttonClear );
            this.topPanel.Controls.Add( this.btnEditEmployer );
            this.topPanel.Controls.Add( this.cmbEmploymentStatus );
            this.topPanel.Controls.Add( this.lblStaticStatus );
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point( 3, 16 );
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size( 354, 65 );
            this.topPanel.TabIndex = 0;
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point( 120, 32 );
            this.buttonClear.Message = null;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size( 64, 23 );
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new System.EventHandler( this.buttonClear_Click );
            // 
            // btnEditEmployer
            // 
            this.btnEditEmployer.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.btnEditEmployer.Location = new System.Drawing.Point( 6, 32 );
            this.btnEditEmployer.Message = null;
            this.btnEditEmployer.Name = "btnEditEmployer";
            this.btnEditEmployer.Size = new System.Drawing.Size( 110, 23 );
            this.btnEditEmployer.TabIndex = 2;
            this.btnEditEmployer.Text = "Select E&mployer...";
            this.btnEditEmployer.Click += new System.EventHandler( this.btnEditEmployer_Click );
            // 
            // cmbEmploymentStatus
            // 
            this.cmbEmploymentStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEmploymentStatus.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.cmbEmploymentStatus.Location = new System.Drawing.Point( 48, 0 );
            this.cmbEmploymentStatus.Name = "cmbEmploymentStatus";
            this.cmbEmploymentStatus.Size = new System.Drawing.Size( 185, 21 );
            this.cmbEmploymentStatus.TabIndex = 1;
            this.cmbEmploymentStatus.Validating += new System.ComponentModel.CancelEventHandler( this.cmbEmploymentStatus_Validating );
            this.cmbEmploymentStatus.SelectedIndexChanged += new System.EventHandler( this.cmbEmploymentStatus_SelectedIndexChanged );
            // 
            // lblStaticStatus
            // 
            this.lblStaticStatus.Location = new System.Drawing.Point( 5, 3 );
            this.lblStaticStatus.Name = "lblStaticStatus";
            this.lblStaticStatus.Size = new System.Drawing.Size( 41, 22 );
            this.lblStaticStatus.TabIndex = 0;
            this.lblStaticStatus.Text = "Status:";
            // 
            // phoneNumberControl
            // 
            this.phoneNumberControl.AreaCode = string.Empty;
            this.phoneNumberControl.Location = new System.Drawing.Point( 46, 11 );
            this.phoneNumberControl.Model = ( (PatientAccess.Domain.Parties.PhoneNumber)( resources.GetObject( "phoneNumberControl.Model" ) ) );
            this.phoneNumberControl.Name = "phoneNumberControl";
            this.phoneNumberControl.PhoneNumber = string.Empty;
            this.phoneNumberControl.Size = new System.Drawing.Size( 94, 27 );
            this.phoneNumberControl.TabIndex = 2;
            this.phoneNumberControl.AreaCodeChanged += new EventHandler(phoneNumberControl_Changed);
            this.phoneNumberControl.PhoneNumberChanged += new EventHandler(phoneNumberControl_Changed); 
            // 
            // EmploymentView
            // 
            this.Controls.Add( this.groupBox );
            this.Name = "EmploymentView";
            this.Size = new System.Drawing.Size( 360, 225 );
            this.groupBox.ResumeLayout( false );
            this.midPanel.ResumeLayout( false );
            this.midPanel.PerformLayout();
            this.bottomPanel.ResumeLayout( false );
            this.topPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public EmploymentView()
        {
            loadingModelData = true;
            employmentView= new EmploymentViewPresenter(this);
            // This call is required by the Windows.Forms Form Designer.
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

        private Container          components = null;

        private LoggingButton              btnEditEmployer;
        private LoggingButton              buttonClear;

        private PatientAccessComboBox                    cmbEmploymentStatus;

        private GroupBox            groupBox;

        private Label               lblStaticStatus;
        private Label               lblStaticPhone;

        private Panel               topPanel;
        private Panel               bottomPanel;
        private Panel               midPanel;

        private TextBox txtEmployerAddress;

        private bool                                     isResetClickedOnInsuredTab = false;
        private bool                                     loadingModelData;
        private Employment                               savedEmployment;
        private Account                                  i_Model_Account;
        private long                                     i_CoverageOrderID;   
        private EmploymentStatus                         priorEmploymentStatus = new EmploymentStatus();
        private EmploymentStatus                         prevValidatingStatus = new EmploymentStatus();
        private EmploymentViewPresenter                  employmentView;
        #endregion

        #region Constants

        private const string NOT_EMPLOYED                = "NOT-EMPLOYED";
        private PhoneNumberControl phoneNumberControl;

        #endregion



    }
}
