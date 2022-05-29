using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class InsDetailPlanDetails : ControlView
    {
        #region Events
        public event EventHandler ResetButtonClicked;
        #endregion

        #region Rule Event Handlers

        #endregion

        #region Event Handlers
        private void billingInformationView_Load( object sender, EventArgs e )
        {
        }

        private void InsDetailPlanDetails_Leave( object sender, EventArgs e )
        {
            if( this.currentView != null && this.currentView is MedicaidPayorView )
            {
                ( (MedicaidPayorView)this.currentView ).blnLeaveRun = true;
            }
            UpdateModel();
            if( this.currentView != null && this.currentView is MedicaidPayorView )
            {
                ( (MedicaidPayorView)this.currentView ).blnLeaveRun = false;
            }
        }

        /// <summary>
        /// OnBillingAddressSelectedEvent - the method will pass the BillingAddressSelected  on
        /// to the AddressView
        /// </summary>
        private void billingInformationView_BillingAddressSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs)e;
            BillingInformation aBillingInformation = (BillingInformation)args.Context;

            this.addressView.Model = aBillingInformation;
            this.addressView.UpdateView();

            this.RunRules();
        }

        // the AddressView changed, so pass the info on to the BillingInformationView

        private void addressView_AddressChangedEventHandler( object sender, EventArgs e )
        {
            ContactPoint modelContactPoint = this.addressView.Model_ContactPoint;

            if( modelContactPoint != null )
            {
                billingInformationView.Model_Coverage.BillingInformation.Address = modelContactPoint.Address;
                billingInformationView.UpdateView();
            }

            this.RunRules();
        }

        private void addressView_PhoneNumberChanged( object sender, EventArgs e )
        {
            billingInformationView.Model_Coverage.BillingInformation.PhoneNumber =
                this.addressView.Model_ContactPoint.PhoneNumber;
        }

        private void ResetButtonClick( object sender, EventArgs e )
        {
            this.UpdateView();
        }

        private void btnReset_Click( object sender, EventArgs e )
        {
            if( ResetButtonClicked != null )
            {
                ResetButtonClicked( sender, e );
            }

            ResetView();

            this.RunRules();
            this.billingInformationView.RunRules();
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;
                //selectedInsured = null;
                RegisterRulesEvents();
            }

            if( Model_Coverage != null )
            {
                DisplayPayorView( Model_Coverage.InsurancePlan.GetType() );
                billingInformationView.Model = Model_Coverage;
            }

            if( Model_Coverage != null
                && Model_Coverage.BillingInformation != null )
            {
                addressView.Model = Model_Coverage.BillingInformation;
                addressView.PatientAccount = AccountView.GetInstance().Model_Account ?? Account;
            }

            addressView.UpdateView();
            billingInformationView.UpdateView();

            this.RunRules();
        }

        public override void UpdateModel()
        {
            if( currentView != null )
            {
                currentView.UpdateModel();
            }
            billingInformationView.UpdateModel();
            addressView.UpdateModel();
        }

        private void ResetView()
        {
            billingInformationView.ResetView();
            Model_Coverage.BillingInformation.Address = null;

            addressView.Model = Model_Coverage.BillingInformation;
            RegisterRulesEvents();
            addressView.ResetView();

            //Model_Coverage = null;
            UpdateModel();
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (Coverage)this.Model;
            }
        }

        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        public string EditBtnResetText
        {
            set
            {
                this.btnReset.Text = value;
            }
        }
        #endregion

        #region Private Methods

        private void RegisterRulesEvents()
        {
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingAddressPreferred ), this.addressView.Model, new EventHandler( this.addressView.AddressPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingAddressRequired ), this.addressView.Model, new EventHandler( this.addressView.AddressRequiredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingPhonePreferred ), this.Model_Coverage, new EventHandler( this.addressView.PhonePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( BillingPhoneRequired ), this.Model_Coverage, new EventHandler( this.addressView.PhoneRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {
            // UNREGISTER EVENTS                                    
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingAddressPreferred ), this.addressView.Model, new EventHandler( this.addressView.AddressPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingAddressRequired ), this.addressView.Model, new EventHandler( this.addressView.AddressRequiredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingPhonePreferred ), this.Model_Coverage, new EventHandler( this.addressView.PhonePreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( BillingPhoneRequired ), this.Model_Coverage, new EventHandler( this.addressView.PhoneRequiredEventHandler ) );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            RegisterRulesEvents();
            this.Refresh();

            RuleEngine.GetInstance().EvaluateRule( typeof( BillingAddressPreferred ), this.addressView.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingAddressRequired ), this.addressView.Model );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingPhonePreferred ), this.Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( BillingPhoneRequired ), this.Model_Coverage );

        }

        private void PopulateViewTable()
        {
            commercialPayorView = CreateCommercialPayorView();
            govOtherPayorView = CreateGovernmentOtherPayorView();
            medicaidPayorView = CreateMedicaidPayorView();
            medicarePayorView = CreateMedicarePayorView();
            otherPayorView = CreateOtherPayorView();
            selfPayPayorView = CreateSelfPayPayorView();
            workersCompPayorView = CreateWorkersCompPayorView();

            viewTable.Add( typeof( CommercialInsurancePlan ), commercialPayorView );
            viewTable.Add( typeof( GovernmentMedicaidInsurancePlan ), medicaidPayorView );
            viewTable.Add( typeof( GovernmentMedicareInsurancePlan ), medicarePayorView );
            viewTable.Add( typeof( GovernmentOtherInsurancePlan ), govOtherPayorView );
            viewTable.Add( typeof( OtherInsurancePlan ), otherPayorView );
            viewTable.Add( typeof( SelfPayInsurancePlan ), selfPayPayorView );
            viewTable.Add( typeof( WorkersCompensationInsurancePlan ), workersCompPayorView );
        }

        private void DisplayPayorView( Type coverageType )
        {
            // TLG 10/24/2005 - issue: type coverageType parm passed in the type definition of the 
            // Coverage.InsurancePlan ( i.e. Model_Coverage.InsurancePlan.GetType() ); this is being
            // used to type the Model_Coverage; the issue is that the InsurancePlan type does not
            // necessarily correspond to the Coverage type ( e.g. for ACO/30015, the InsurancePlan type 
            // is Commercial but the Coverage type is GovernmentOther?

            if( currentView != null )
            {
                currentView.Visible = false;
                currentView.Hide();
            }
            currentView = (ControlView)viewTable[coverageType];

            this.Model_Coverage.Account = this.Account;

            if( coverageType == typeof( CommercialInsurancePlan ) )
            {
                commercialPayorView.Model_Coverage = (CommercialCoverage)this.Model_Coverage;
                commercialPayorView.Account = this.Account;
            }
            else if( coverageType == typeof( GovernmentMedicaidInsurancePlan ) )
            {
                medicaidPayorView.Model_Coverage = (GovernmentMedicaidCoverage)this.Model_Coverage;
                medicaidPayorView.Account = this.Account;
            }
            else if( coverageType == typeof( GovernmentMedicareInsurancePlan ) )
            {
                medicarePayorView.Model_Coverage = (GovernmentMedicareCoverage)this.Model_Coverage;
                medicarePayorView.Account = this.Account;
            }
            else if( coverageType == typeof( GovernmentOtherInsurancePlan ) )
            {
                govOtherPayorView.Model_Coverage = (GovernmentOtherCoverage)this.Model_Coverage;
                govOtherPayorView.Account = this.Account;
            }
            else if( coverageType == typeof( OtherInsurancePlan ) )
            {
                otherPayorView.Model_Coverage = (OtherCoverage)this.Model_Coverage;
                otherPayorView.Account = this.Account;
            }
            else if( coverageType == typeof( SelfPayInsurancePlan ) )
            {
                selfPayPayorView.Model = (SelfPayCoverage)this.Model_Coverage;
            }
            else if( coverageType == typeof( WorkersCompensationInsurancePlan ) )
            {
                workersCompPayorView.Model_Coverage = (WorkersCompensationCoverage)this.Model_Coverage;
                workersCompPayorView.Account = this.Account;
            }
            currentView.Visible = true;
            currentView.Show();
            currentView.UpdateView();
            currentView.Focus();
        }

        private CommercialPayorView CreateCommercialPayorView()
        {
            this.commercialPayorView = new CommercialPayorView( this );
            this.commercialPayorView.Location = new Point( 0, 0 );
            this.commercialPayorView.Model = null;
            this.commercialPayorView.Name = "commercialPayorView";
            this.commercialPayorView.Size = new Size( 880, 215 );
            this.commercialPayorView.TabIndex = 0;
            this.commercialPayorView.Visible = false;
            this.commercialPayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.Controls.Add( this.commercialPayorView );
            this.topPanel.Controls.Add( this.commercialPayorView );
            return this.commercialPayorView;
        }

        private GovernmentOtherPayorView CreateGovernmentOtherPayorView()
        {
            this.govOtherPayorView = new GovernmentOtherPayorView( this );
            this.govOtherPayorView.Location = new Point( 0, 0 );
            this.govOtherPayorView.Model = null;
            this.govOtherPayorView.Name = "governmentOtherPayorView";
            this.govOtherPayorView.Size = new Size( 880, 215 );
            this.govOtherPayorView.TabIndex = 0;
            this.govOtherPayorView.Visible = false;
            this.govOtherPayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.Controls.Add( this.govOtherPayorView );
            this.topPanel.Controls.Add( this.govOtherPayorView );
            return this.govOtherPayorView;
        }

        private MedicaidPayorView CreateMedicaidPayorView()
        {
            this.medicaidPayorView = new MedicaidPayorView( this );
            this.medicaidPayorView.Location = new Point( 0, 0 );
            this.medicaidPayorView.Model = null;
            this.medicaidPayorView.Name = "medicaidPayorView";
            this.medicaidPayorView.Size = new Size( 880, 215 );
            this.medicaidPayorView.TabIndex = 0;
            this.medicaidPayorView.Visible = false;
            this.medicaidPayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.topPanel.Controls.Add( this.medicaidPayorView );
            return this.medicaidPayorView;
        }

        private MedicarePayorView CreateMedicarePayorView()
        {
            this.medicarePayorView = new MedicarePayorView( this );
            this.medicarePayorView.Location = new Point( 0, 0 );
            this.medicarePayorView.Model = null;
            this.medicarePayorView.Name = "medicarePayorView";
            this.medicarePayorView.Size = new Size( 880, 215 );
            this.medicarePayorView.TabIndex = 0;
            this.medicarePayorView.Visible = false;
            this.medicarePayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.Controls.Add( this.medicarePayorView );
            this.topPanel.Controls.Add( this.medicarePayorView );
            return this.medicarePayorView;
        }

        private OtherPayorView CreateOtherPayorView()
        {
            this.otherPayorView = new OtherPayorView( this );
            this.otherPayorView.Location = new Point( 0, 0 );
            this.otherPayorView.Model = null;
            this.otherPayorView.Name = "otherPayorView";
            this.otherPayorView.Size = new Size( 880, 215 );
            this.otherPayorView.TabIndex = 0;
            this.otherPayorView.Visible = false;
            this.otherPayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.Controls.Add( this.otherPayorView );
            this.topPanel.Controls.Add( this.otherPayorView );
            return this.otherPayorView;
        }

        private SelfPayPayorView CreateSelfPayPayorView()
        {
            this.selfPayPayorView = new SelfPayPayorView();
            this.selfPayPayorView.Location = new Point( 0, 0 );
            this.selfPayPayorView.Model = null;
            this.selfPayPayorView.Name = "selfPayPayorView";
            this.selfPayPayorView.Size = new Size( 880, 215 );
            this.selfPayPayorView.TabIndex = 0;
            this.selfPayPayorView.Visible = false;
            this.Controls.Add( this.selfPayPayorView );
            this.topPanel.Controls.Add( this.selfPayPayorView );
            return this.selfPayPayorView;
        }

        private WorkersCompPayorView CreateWorkersCompPayorView()
        {
            this.workersCompPayorView = new WorkersCompPayorView( this );
            this.workersCompPayorView.Location = new Point( 0, 0 );
            this.workersCompPayorView.Model = null;
            this.workersCompPayorView.Name = "workersCompPayorView";
            this.workersCompPayorView.Size = new Size( 880, 215 );
            this.workersCompPayorView.TabIndex = 0;
            this.workersCompPayorView.Visible = false;
            this.workersCompPayorView.ResetButtonClicked += new EventHandler( this.ResetButtonClick );
            this.topPanel.Controls.Add( this.workersCompPayorView );
            return this.workersCompPayorView;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.topPanel = new System.Windows.Forms.Panel();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.addressView = new PatientAccess.UI.AddressViews.AddressView();
            this.billingInformationView = new PatientAccess.UI.InsuranceViews.BillingInformationView();
            this.btnReset = new LoggingButton();
            this.billingLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.topPanel.Location = new System.Drawing.Point( 0, 0 );
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size( 880, 215 );
            this.topPanel.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add( this.addressView );
            this.bottomPanel.Controls.Add( this.billingInformationView );
            this.bottomPanel.Controls.Add( this.btnReset );
            this.bottomPanel.Controls.Add( this.billingLineLabel );
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point( 0, 215 );
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size( 880, 235 );
            this.bottomPanel.TabIndex = 1;
            // 
            // addressView
            // 
            this.addressView.Context = "Billing";
            this.addressView.Font = new System.Drawing.Font( "Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.addressView.KindOfTargetParty = null;
            this.addressView.Location = new System.Drawing.Point( 490, 24 );
            this.addressView.Model = null;
            this.addressView.Mode = AddressViews.AddressView.AddressMode.PHONE;
            this.addressView.ShowStatus = false;
            this.addressView.Name = "addressView";
            this.addressView.PatientAccount = null;
            this.addressView.Size = new System.Drawing.Size( 257, 154 );
            this.addressView.TabIndex = 3;
            this.addressView.IsAddressWithStreet2 = false;
            this.addressView.AddressChanged += new System.EventHandler( this.addressView_AddressChangedEventHandler );
            this.addressView.PhoneNumberChanged += new System.EventHandler( this.addressView_PhoneNumberChanged );
            // 
            // billingInformationView
            // 
            this.billingInformationView.Font = new System.Drawing.Font( "Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.billingInformationView.Location = new System.Drawing.Point( 12, 24 );
            this.billingInformationView.Model = null;
            this.billingInformationView.Model_Coverage = null;
            this.billingInformationView.Name = "billingInformationView";
            this.billingInformationView.Size = new System.Drawing.Size( 452, 160 );
            this.billingInformationView.TabIndex = 2;
            this.billingInformationView.Load += new System.EventHandler( this.billingInformationView_Load );
            this.billingInformationView.BillingAddressSelectedEvent += new System.EventHandler( this.billingInformationView_BillingAddressSelectedEvent );
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point( 768, 186 );
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler( this.btnReset_Click );
            // 
            // billingLineLabel
            // 
            this.billingLineLabel.Caption = "Billing Information";
            this.billingLineLabel.Location = new System.Drawing.Point( 8, 0 );
            this.billingLineLabel.Name = "billingLineLabel";
            this.billingLineLabel.Size = new System.Drawing.Size( 864, 18 );
            this.billingLineLabel.TabIndex = 0;
            this.billingLineLabel.TabStop = false;
            // 
            // InsDetailPlanDetails
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.bottomPanel );
            this.Controls.Add( this.topPanel );
            this.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.Name = "InsDetailPlanDetails";
            this.Size = new System.Drawing.Size( 880, 450 );
            this.Leave += new System.EventHandler( this.InsDetailPlanDetails_Leave );
            this.bottomPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public InsDetailPlanDetails()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );
            // Instantiate the coverage plan views and store them in the view hash table
            PopulateViewTable();
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                this.UnRegisterRulesEvents();

                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private Container components = null;

        private LoggingButton btnReset;

        private Panel topPanel;
        private Panel bottomPanel;

        private LineLabel billingLineLabel;
        private AddressView addressView;

        // 7 Views stored in the form view hash table that appear in the top half of the parent form:
        private MedicaidPayorView medicaidPayorView;
        private MedicarePayorView medicarePayorView;
        private SelfPayPayorView selfPayPayorView;
        private WorkersCompPayorView workersCompPayorView;
        private CommercialPayorView commercialPayorView;
        private GovernmentOtherPayorView govOtherPayorView;
        private OtherPayorView otherPayorView;

        // Views nested in the lower half of the parent form:
        private BillingInformationView billingInformationView;
        private Hashtable viewTable = new Hashtable( NUM_PAYOR_VIEWS );
        private Account i_Account;
        private ControlView currentView;
        //private Insured                                                 selectedInsured;
        private bool loadingModelData = true;
        #endregion

        #region Constants
        private static int NUM_PAYOR_VIEWS = 7;    // Number of buckets in the form view hash table
        #endregion
    }
}
