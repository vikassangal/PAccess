using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class CoverageView : ControlView
    {
        #region Event Handlers
        public delegate void CoveragesDelegate(ICollection coverages);
        public delegate void CoverageDelegate(Coverage aCoverages);
        public event CoveragesDelegate CoverageUpdatedEvent;
        public event CoverageDelegate CoverageResetClickedEvent;
        public event CoverageDelegate PlanSelectedEvent;

    

        private void CoverageView_Load( object sender, EventArgs e )
        {
            btnSetAsPrimary.Enabled = false;
            btnEdit.Enabled = false;
            this.findAPlanView.IsPrimary = this.isPrimary;
        }

        private void FireCoverageUpdatedEvent()
        {
            this.planSummaryView.UpdateView();

            if( CoverageUpdatedEvent != null )
            {
                CoverageUpdatedEvent(this.Account.Insurance.Coverages);
            }
        }

        private void FindAPlanView_PlanSelectedEvent( object sender, SelectInsuranceArgs args )
        {
            InsurancePlan plan  = args.SelectedPlan as InsurancePlan;
            Employer emp        = args.SelectedEmployer as Employer;
            Coverage coverage   = null;

            if (plan == null)
            {
                return;
            }

            coverage = Coverage.CoverageFor( plan, new Insured() );

            if (coverage == null)
            {
                return;
            }

            if( this.isPrimary )
            {
                coverage.CoverageOrder  = new CoverageOrder( CoverageOrder.PRIMARY_OID, "Primary" );
            }
            else
            {
                coverage.CoverageOrder  = new CoverageOrder( CoverageOrder.SECONDARY_OID, "Secondary" );
            }
            
            if( coverage.Insured == null )
            {
                coverage.Insured = new Insured();
            }

            if( coverage.Insured.Employment == null  )
            {
                coverage.Insured.Employment = new Employment();
            }

            coverage.Insured.Employment.Employer = emp; 
            if( coverage.Insured.Employment.Employer != null )
            {
                ContactPoint employerCP = coverage.Insured.Employment.Employer.ContactPointWith( TypeOfContactPoint.NewEmployerContactPointType() );
                 coverage.Insured.Employment.Employer.PartyContactPoint = employerCP;
            }                       

            PlanSelectedEvent( coverage );
        }

        private void btnReset_Click( object sender, EventArgs e )
        {
            if( this.Model_Coverage != null )
            {
                CoverageResetClickedEvent(this.Model_Coverage);
            }

            ResetView();
            this.UpdateModel();

            this.RuleEngine.RegisterEvent<MedicarePatientHasHMO>(
                this.HandleRuleHmoIsSecondary );
            this.RuleEngine.EvaluateRule<MedicarePatientHasHMO>(
                this.Account.Insurance );
            this.RuleEngine.UnregisterEvent<MedicarePatientHasHMO>(
                this.HandleRuleHmoIsSecondary );

        }


        private void HandleRuleHmoIsSecondary( object sender, EventArgs e )
        {

            MessageBox.Show( UIErrorMessages.MEDICARE_WITH_MEDICARE_HMO,
                             "Warning",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Exclamation,
                             MessageBoxDefaultButton.Button1 );
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
        private void SetAsPrimaryButtonClickHandler( object sender, EventArgs e )
        {
            if( Model_Coverage != null )
            {
                if (Account.Insurance.PrimaryCoverage == null)
                {
                    Account.DeletedSecondaryCoverage = Model_Coverage.DeepCopy();
                 }
                Account.Insurance.SetAsPrimary( Model_Coverage );                
                Account.FinancialClass = null;
            }
            FireCoverageUpdatedEvent();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            insuranceDetailsDialog                = new InsuranceDetails();
            try
            {
                insuranceDetailsDialog.insuranceDetailsView.Model_Coverage = this.Model_Coverage;
                insuranceDetailsDialog.insuranceDetailsView.Account        = this.Account;
                insuranceDetailsDialog.insuranceDetailsView.Active_Tab     = PLAN_DETAILS_PAGE;

                insuranceDetailsDialog.UpdateView();

                this.Model_Coverage = insuranceDetailsDialog.insuranceDetailsView.Model_Coverage;

                if( insuranceDetailsDialog.ShowDialog( this ) == DialogResult.OK )
                {
                    UpdateView();
                }            

                this.FireCoverageUpdatedEvent();
            }
            finally
            {
                insuranceDetailsDialog.Dispose();
            }
        }

        #endregion

        #region Methods
        public void ResetView()
        {
            //Account        = null;
            this.Model_Coverage = null;

          //  lblPriority.Text = String.Empty;
            lblPlanID.Text   = String.Empty;
            lblPlanName.Text = String.Empty;

            btnSetAsPrimary.Enabled = false;
            btnEdit.Enabled = false;

            planSummaryView.ResetView();
            findAPlanView.ResetView();
            insuredSummaryView.ResetView();

            UpdateView();
        } 

        public override void UpdateView()
        {
            btnSetAsPrimary.Enabled    = false;

            insuredSummaryView.Model   = this.Model_Coverage;
            insuredSummaryView.Account = this.Account;

            planSummaryView.Model      = this.Model_Coverage;
            planSummaryView.Account    = this.Account;

            this.findAPlanView.PatientAccount = this.Account;
            
            if( Model_Coverage != null && Model_Coverage.CoverageOrder != null )
            {
                btnSetAsPrimary.Enabled = true;
//                lblPriority.Text = Model_Coverage.CoverageOrder.Description;

               // SetCoverageButtonState();
                if( !isPrimary )
                {
                    btnSetAsPrimary.Enabled = (this.Model != null);
                    Model_Coverage.CoverageOrder = new CoverageOrder( CoverageOrder.SECONDARY_OID, "Secondary");
                }
                else
                {
                    Model_Coverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "Primary");                    
                }

                if( Model_Coverage != null && Model_Coverage.InsurancePlan != null )
                {
                    lblPlanID.Text   = Model_Coverage.InsurancePlan.PlanID;
                    lblPlanName.Text = Model_Coverage.InsurancePlan.PlanName;
                }
                btnEdit.Enabled = true;
            }
            else
            {
                if( isPrimary )
                {
                    btnSetAsPrimary.Visible = false;
                    if ( Account.HasInValidPrimaryPlanID )
                    {
                        findAPlanView.MtbPlanSearchEntry.UnMaskedText = Account.EMPIPrimaryInvalidPlanID;
                    }
                }
                else
                {
                    btnSetAsPrimary.Enabled = false;
                    if ( Account.HasInValidSecondaryPlanID )
                    {
                        findAPlanView.MtbPlanSearchEntry.UnMaskedText = Account.EMPISecondaryInvalidPlanID;
                    }
                }
                btnEdit.Enabled = false;
            }

            insuredSummaryView.UpdateView();
            planSummaryView.UpdateView();
        }

        public override void UpdateModel()
        {
        }

        public void SetDefaultFocus()
        {
            this.findAPlanView.SetDefaultFocus();
        }
        #endregion

        #region Properties
        public Coverage Model_Coverage
        {
            private get
            {
                return (Coverage)base.Model;
            }
            set
            {
                base.Model = value;
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
        #endregion

        #region Private Methods
        protected override void Dispose( bool disposing )
        {
            if ( IsHandleCreated )
            {
                Application.DoEvents();
            }

            if ( disposing )
            {
                if ( components != null ) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblStaticPriority = new System.Windows.Forms.Label();
            this.lblPriority = new System.Windows.Forms.Label();
            this.lblStaticPlanId = new System.Windows.Forms.Label();
            this.lblPlanID = new System.Windows.Forms.Label();
            this.btnSetAsPrimary = new LoggingButton();
            this.btnReset = new LoggingButton();
            this.lblStaticPlanName = new System.Windows.Forms.Label();
            this.lblPlanName = new System.Windows.Forms.Label();
            this.planSummaryView = new PatientAccess.UI.InsuranceViews.PlanSummaryView();
            this.insuredSummaryView = new PatientAccess.UI.InsuranceViews.InsuredSummaryView();
            this.findAPlanView = new PatientAccess.UI.InsuranceViews.FindAPlanView();
            this.btnEdit = new LoggingButton();
            this.SuspendLayout();
            // 
            // lblStaticPriority
            // 
            this.lblStaticPriority.Location = new System.Drawing.Point(10, 6);
            this.lblStaticPriority.Name = "lblStaticPriority";
            this.lblStaticPriority.Size = new System.Drawing.Size(43, 23);
            this.lblStaticPriority.TabIndex = 0;
            this.lblStaticPriority.Text = "Priority:";
            this.lblStaticPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPriority
            // 
            this.lblPriority.Location = new System.Drawing.Point(69, 6);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(150, 20);
            this.lblPriority.TabIndex = 0;
            this.lblPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticPlanId
            // 
            this.lblStaticPlanId.Location = new System.Drawing.Point(10, 28);
            this.lblStaticPlanId.Name = "lblStaticPlanId";
            this.lblStaticPlanId.Size = new System.Drawing.Size(48, 23);
            this.lblStaticPlanId.TabIndex = 0;
            this.lblStaticPlanId.Text = "Plan ID:";
            this.lblStaticPlanId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanID
            // 
            this.lblPlanID.Location = new System.Drawing.Point(69, 28);
            this.lblPlanID.Name = "lblPlanID";
            this.lblPlanID.Size = new System.Drawing.Size(150, 20);
            this.lblPlanID.TabIndex = 0;
            this.lblPlanID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSetAsPrimary
            // 
            this.btnSetAsPrimary.Location = new System.Drawing.Point(232, 16);
            this.btnSetAsPrimary.Name = "btnSetAsPrimary";
            this.btnSetAsPrimary.Size = new System.Drawing.Size(90, 23);
            this.btnSetAsPrimary.TabIndex = 1;
            this.btnSetAsPrimary.Text = "Set as Prim&ary";
            this.btnSetAsPrimary.Click += new System.EventHandler(this.SetAsPrimaryButtonClickHandler);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(640, 148);
            this.btnReset.Name = "btnReset";
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Clear All";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblStaticPlanName
            // 
            this.lblStaticPlanName.Location = new System.Drawing.Point(10, 53);
            this.lblStaticPlanName.Name = "lblStaticPlanName";
            this.lblStaticPlanName.Size = new System.Drawing.Size(65, 23);
            this.lblStaticPlanName.TabIndex = 0;
            this.lblStaticPlanName.Text = "Plan Name:";
            this.lblStaticPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPlanName
            // 
            this.lblPlanName.Location = new System.Drawing.Point(69, 53);
            this.lblPlanName.Name = "lblPlanName";
            this.lblPlanName.Size = new System.Drawing.Size(285, 20);
            this.lblPlanName.TabIndex = 0;
            this.lblPlanName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPlanName.UseMnemonic = false;
            // 
            // planSummaryView
            // 
            this.planSummaryView.Account = null;
            this.planSummaryView.Location = new System.Drawing.Point(8, 88);
            this.planSummaryView.Model = null;
            this.planSummaryView.Model_Coverage = null;
            this.planSummaryView.Name = "planSummaryView";
            this.planSummaryView.Size = new System.Drawing.Size(300, 90);
            this.planSummaryView.TabIndex = 0;
            this.planSummaryView.TabStop = false;
            // 
            // insuredSummaryView
            // 
            this.insuredSummaryView.Account = null;
            this.insuredSummaryView.Location = new System.Drawing.Point(328, 88);
            this.insuredSummaryView.Model = null;
            this.insuredSummaryView.Model_Coverage = null;
            this.insuredSummaryView.Name = "insuredSummaryView";
            this.insuredSummaryView.Size = new System.Drawing.Size(300, 90);
            this.insuredSummaryView.TabIndex = 0;
            this.insuredSummaryView.TabStop = false;
            // 
            // findAPlanView
            // 
            this.findAPlanView.IsPrimary = true;
            this.findAPlanView.Location = new System.Drawing.Point(364, 6);
            this.findAPlanView.Model = null;
            this.findAPlanView.Name = "findAPlanView";
            this.findAPlanView.Size = new System.Drawing.Size(264, 75);
            this.findAPlanView.TabIndex = 2;
            this.findAPlanView.PlanSelectedEvent += new System.EventHandler<SelectInsuranceArgs>(this.FindAPlanView_PlanSelectedEvent);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(640, 112);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Edit...";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // CoverageView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.findAPlanView);
            this.Controls.Add(this.insuredSummaryView);
            this.Controls.Add(this.planSummaryView);
            this.Controls.Add(this.lblPlanName);
            this.Controls.Add(this.lblStaticPlanName);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSetAsPrimary);
            this.Controls.Add(this.lblPlanID);
            this.Controls.Add(this.lblStaticPlanId);
            this.Controls.Add(this.lblPriority);
            this.Controls.Add(this.lblStaticPriority);
            this.Name = "CoverageView";
            this.Size = new System.Drawing.Size(730, 185);
            this.Load += new System.EventHandler(this.CoverageView_Load);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public CoverageView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
        }

        public CoverageView( bool isItPrimary ) : this()
        {
            isPrimary = isItPrimary;

            if( isPrimary )
            {
                btnSetAsPrimary.Visible = false;
                lblPriority.Text = "Primary";
            }
            else
            {
                lblPriority.Text = "Secondary";
            }
        }

        #endregion

        #region Data Elements
        private Container                     components = null;
        // Static text labels on left side of form
        private Label                          lblStaticPriority;
        private Label                          lblStaticPlanId;
        private Label                          lblStaticPlanName;
        // Dynamic labels updated with insurance information
        private Label                          lblPriority;
        public  Label                          lblPlanID;
        public  Label                          lblPlanName;

        private LoggingButton                         btnSetAsPrimary;
        private LoggingButton                         btnReset;
        // Nested view objects
        private PlanSummaryView     planSummaryView;
        private FindAPlanView       findAPlanView;
        private InsuredSummaryView  insuredSummaryView;
        private LoggingButton btnEdit;
        private InsuranceDetails                                    insuranceDetailsDialog;
        private Account                                             i_Account;
        private bool                                                isPrimary = true;
        private RuleEngine i_RuleEngine;

        #endregion

        #region Constants
        private const int                       PLAN_DETAILS_PAGE = 0;
        #endregion

    }
}