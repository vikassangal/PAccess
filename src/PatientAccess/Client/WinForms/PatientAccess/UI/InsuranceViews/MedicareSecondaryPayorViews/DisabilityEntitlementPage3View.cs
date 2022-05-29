using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for DisabilityEntitlementPage3View.
    /// </summary>
    public class DisabilityEntitlementPage3View : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void btnEditInsurance_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURANCE );
        }

        private void btnEditInsured_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURED );
        }

        private void btnEditPayor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.PAYORDETAILS );
        }

        private void rbQuestion5Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question5Response = true;
                DisplayGroupHealthPlanInfo( true );

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).GHPLimitExceeded.SetYes();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion5No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question5Response = true;
                DisplayGroupHealthPlanInfo( false );

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).GHPLimitExceeded.SetNo();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );

            if( (bool) Tag == true && FormChanged )
            {   // User went back and made a change
                ResetView();
            }
            else if( formActivating )
            {
                ResetView();

                if( (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as DisabilityEntitlement ) == null )
                {
                    return;
                }
                else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( DisabilityEntitlement ) ) )
                {   // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls
                    YesNoFlag flag = new YesNoFlag();

                    flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).GHPLimitExceeded;

                    if( flag.Code.Equals( "Y" ) )
                    {
                        rbQuestion5Yes.Checked = true;
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        rbQuestion5No.Checked = true;
                    }
                }
            }
            FormCanTransition();
        }
        #endregion

        #region Properties
        [Browsable(false)]
        private MedicareSecondaryPayor Model_MedicareSecondaryPayor
        {
            get
            {
                return (MedicareSecondaryPayor) this.Model;
            }
        }

        [Browsable(false)]
        public Account Model_Account
        {
            private get
            {
                return (Account) this.i_account;
            }
            set
            {
                i_account = value;
            }
        }

        [Browsable(false)]
        public bool FormChanged
        {
            get
            {
                return formWasChanged;
            }
            set
            {
                formWasChanged = value;
            }
        }

        [Browsable(false)]
        public int Response
        {
            get
            {
                return response;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Populate the controls with the Group Plan Health information
        /// </summary>
        private void DisplayGroupHealthPlanInfo( bool state )
        {
            SetGroupPlanGroupBoxState( state );

            Coverage coverage = GetPrimaryCoverage();

            if( state && Model_Account.PrimaryInsured != null && coverage != null )
            {
                RelationshipType relaType = Model_Account.Patient.RelationshipWith( coverage.Insured );
                if( relaType != null )
                {
                    string relationShip = relaType.ToString();

                    if( relationShip != String.Empty )
                    {
                        lblRelationship.Text = relationShip;
                    }
                    else
                    {
                        lblRelationship.Text = NOT_AVAILABLE;
                    }
                }
                else
                {
                    lblRelationship.Text = NOT_AVAILABLE;
                }
                if( Model_Account.PrimaryInsured.FormattedName != String.Empty )
                {
                    lblPolicyHolder.Text = Model_Account.PrimaryInsured.FormattedName;
                }
                else
                {
                    lblPolicyHolder.Text = NOT_AVAILABLE;
                }              
                if( coverage != null )
                {
                    if( coverage.GetType().Equals( typeof( WorkersCompensationCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as WorkersCompensationCoverage).PolicyNumber;
                        lblGroupID.Text = NOT_AVAILABLE;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicaidCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as GovernmentMedicaidCoverage).PolicyCINNumber;
                        lblGroupID.Text = NOT_AVAILABLE;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicareCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as GovernmentMedicareCoverage).MBINumber;
                        lblGroupID.Text = NOT_AVAILABLE;
                    }
                    else if( coverage.GetType().Equals( typeof( CommercialCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as CommercialCoverage).CertSSNID;
                        lblGroupID.Text = (coverage as CommercialCoverage).GroupNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( CoverageForCommercialOther ) ) )
                    {
                        lblPolicyID.Text = (coverage as CoverageForCommercialOther).CertSSNID;                    
                        lblGroupID.Text = (coverage as CoverageForCommercialOther).GroupNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as OtherCoverage).CertSSNID;
                        lblGroupID.Text = (coverage as OtherCoverage).GroupNumber;
                    }
                    else
                    {
                        lblPolicyID.Text = NOT_AVAILABLE;
                    }               

                    if( coverage.BillingInformation.Address != null )
                    {
                        string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                        if( mailingLabel != String.Empty )
                        {
                            lblInsuranceAddress.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                                Environment.NewLine, mailingLabel );
                        }
                        else
                        {
                            lblInsuranceAddress.Text = coverage.InsurancePlan.PlanName;
                        }
                    }
                    else
                    {
                        lblInsuranceAddress.Text = coverage.InsurancePlan.PlanName;
                    }
                }
                else
                {
                    lblInsuranceAddress.Text = NOT_AVAILABLE;
                }
            }
        }

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            if( question5Response )
            {
                if( rbQuestion5Yes.Checked )
                {
                    response = MSPEventCode.YesStimulus();
                }
                else
                {
                    response = MSPEventCode.NoStimulus();
                }
            }
            if( question5Response )
            {
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
            }
        }

        /// <summary>
        /// Get the primary insurance coverage that is not Medicare
        /// </summary>
        private Coverage GetPrimaryCoverage()
        {
            Coverage primaryCoverage = null;
            ICollection coverageCollection = Model_Account.Insurance.Coverages;

            if( coverageCollection == null )
            {
                return null;
            }

            foreach( Coverage coverage in coverageCollection )
            {
                if( coverage == null )
                {
                    continue;
                }
                else if( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    primaryCoverage = coverage;
                    break;
                }
            }
            return primaryCoverage;
        }

        private void ResetView()
        {
            rbQuestion5Yes.Checked = false;
            rbQuestion5No.Checked = false;
            question5Response = false;
            formActivating = false;
        }
        /// <summary>
        /// Enable or disable the Group Plan Health information controls
        /// based on state the boolean flag;
        /// </summary>
        private void SetGroupPlanGroupBoxState( bool state )
        {
            if( state == false )
            {
                lblGroupID.Text          = String.Empty;
                lblInsuranceAddress.Text = String.Empty;
                lblPolicyHolder.Text     = String.Empty;
                lblPolicyID.Text         = String.Empty;
                lblRelationship.Text     = String.Empty;
            }
            grpHealthPlan.Enabled = state;
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbQuestion5No = new System.Windows.Forms.RadioButton();
            this.rbQuestion5Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.grpHealthPlan = new System.Windows.Forms.GroupBox();
            this.btnEditPayor = new LoggingButton();
            this.lblGroupID = new System.Windows.Forms.Label();
            this.lblStaticGroupID = new System.Windows.Forms.Label();
            this.lblPolicyID = new System.Windows.Forms.Label();
            this.lblStaticPolicyID = new System.Windows.Forms.Label();
            this.btnEditInsurance = new LoggingButton();
            this.lblInsuranceAddress = new System.Windows.Forms.Label();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.btnEditInsured = new LoggingButton();
            this.lblRelationship = new System.Windows.Forms.Label();
            this.lblStaticRelationship = new System.Windows.Forms.Label();
            this.lblPolicyHolder = new System.Windows.Forms.Label();
            this.lblStaticPolicy = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.grpHealthPlan.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTitle
            // 
            this.txtTitle.BackColor = System.Drawing.Color.White;
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(16, 16);
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(232, 23);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Text = "Medicare Entitlement - Disability";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbQuestion5No);
            this.panel1.Controls.Add(this.rbQuestion5Yes);
            this.panel1.Controls.Add(this.lblQuestion5);
            this.panel1.Location = new System.Drawing.Point(16, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // rbQuestion5No
            // 
            this.rbQuestion5No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion5No.Name = "rbQuestion5No";
            this.rbQuestion5No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5No.TabIndex = 2;
            this.rbQuestion5No.TabStop = true;
            this.rbQuestion5No.Text = "No";
            this.rbQuestion5No.CheckedChanged += new System.EventHandler(this.rbQuestion5No_CheckedChanged);
            // 
            // rbQuestion5Yes
            // 
            this.rbQuestion5Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion5Yes.Name = "rbQuestion5Yes";
            this.rbQuestion5Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5Yes.TabIndex = 1;
            this.rbQuestion5Yes.TabStop = true;
            this.rbQuestion5Yes.Text = "Yes";
            this.rbQuestion5Yes.CheckedChanged += new System.EventHandler(this.rbQuestion5Yes_CheckedChanged);
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(8, 0);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(424, 23);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5. Does the employer that sponsors your GHP employ 100 or more employees?";
            // 
            // grpHealthPlan
            // 
            this.grpHealthPlan.Controls.Add(this.btnEditPayor);
            this.grpHealthPlan.Controls.Add(this.lblGroupID);
            this.grpHealthPlan.Controls.Add(this.lblStaticGroupID);
            this.grpHealthPlan.Controls.Add(this.lblPolicyID);
            this.grpHealthPlan.Controls.Add(this.lblStaticPolicyID);
            this.grpHealthPlan.Controls.Add(this.btnEditInsurance);
            this.grpHealthPlan.Controls.Add(this.lblInsuranceAddress);
            this.grpHealthPlan.Controls.Add(this.lblLine2);
            this.grpHealthPlan.Controls.Add(this.lblLine1);
            this.grpHealthPlan.Controls.Add(this.btnEditInsured);
            this.grpHealthPlan.Controls.Add(this.lblRelationship);
            this.grpHealthPlan.Controls.Add(this.lblStaticRelationship);
            this.grpHealthPlan.Controls.Add(this.lblPolicyHolder);
            this.grpHealthPlan.Controls.Add(this.lblStaticPolicy);
            this.grpHealthPlan.Enabled = false;
            this.grpHealthPlan.Location = new System.Drawing.Point(24, 120);
            this.grpHealthPlan.Name = "grpHealthPlan";
            this.grpHealthPlan.Size = new System.Drawing.Size(504, 225);
            this.grpHealthPlan.TabIndex = 2;
            this.grpHealthPlan.TabStop = false;
            this.grpHealthPlan.Text = "Group Health Plan";
            // 
            // btnEditPayor
            // 
            this.btnEditPayor.Location = new System.Drawing.Point(309, 160);
            this.btnEditPayor.Name = "btnEditPayor";
            this.btnEditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnEditPayor.TabIndex = 3;
            this.btnEditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnEditPayor.Click += new System.EventHandler(this.btnEditPayor_Click);
            // 
            // lblGroupID
            // 
            this.lblGroupID.Location = new System.Drawing.Point(59, 192);
            this.lblGroupID.Name = "lblGroupID";
            this.lblGroupID.Size = new System.Drawing.Size(240, 23);
            this.lblGroupID.TabIndex = 0;
            // 
            // lblStaticGroupID
            // 
            this.lblStaticGroupID.Location = new System.Drawing.Point(8, 192);
            this.lblStaticGroupID.Name = "lblStaticGroupID";
            this.lblStaticGroupID.Size = new System.Drawing.Size(54, 23);
            this.lblStaticGroupID.TabIndex = 0;
            this.lblStaticGroupID.Text = "Group ID:";
            // 
            // lblPolicyID
            // 
            this.lblPolicyID.Location = new System.Drawing.Point(59, 160);
            this.lblPolicyID.Name = "lblPolicyID";
            this.lblPolicyID.Size = new System.Drawing.Size(240, 23);
            this.lblPolicyID.TabIndex = 0;
            // 
            // lblStaticPolicyID
            // 
            this.lblStaticPolicyID.Location = new System.Drawing.Point(8, 160);
            this.lblStaticPolicyID.Name = "lblStaticPolicyID";
            this.lblStaticPolicyID.Size = new System.Drawing.Size(56, 23);
            this.lblStaticPolicyID.TabIndex = 0;
            this.lblStaticPolicyID.Text = "Policy ID:";
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point(309, 94);
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsurance.TabIndex = 2;
            this.btnEditInsurance.Text = "Edit I&nsurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler(this.btnEditInsurance_Click);
            // 
            // lblInsuranceAddress
            // 
            this.lblInsuranceAddress.Location = new System.Drawing.Point(12, 94);
            this.lblInsuranceAddress.Name = "lblInsuranceAddress";
            this.lblInsuranceAddress.Size = new System.Drawing.Size(275, 40);
            this.lblInsuranceAddress.TabIndex = 0;
            // 
            // lblLine2
            // 
            this.lblLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine2.Location = new System.Drawing.Point(0, 145);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(502, 1);
            this.lblLine2.TabIndex = 0;
            // 
            // lblLine1
            // 
            this.lblLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine1.Location = new System.Drawing.Point(0, 81);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(502, 1);
            this.lblLine1.TabIndex = 0;
            this.lblLine1.Text = "label1";
            // 
            // btnEditInsured
            // 
            this.btnEditInsured.Location = new System.Drawing.Point(309, 51);
            this.btnEditInsured.Name = "btnEditInsured";
            this.btnEditInsured.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsured.TabIndex = 1;
            this.btnEditInsured.Text = "Edit &Insured && Cancel MSP";
            this.btnEditInsured.Click += new System.EventHandler(this.btnEditInsured_Click);
            // 
            // lblRelationship
            // 
            this.lblRelationship.Location = new System.Drawing.Point(123, 49);
            this.lblRelationship.Name = "lblRelationship";
            this.lblRelationship.Size = new System.Drawing.Size(173, 23);
            this.lblRelationship.TabIndex = 0;
            // 
            // lblStaticRelationship
            // 
            this.lblStaticRelationship.Location = new System.Drawing.Point(8, 49);
            this.lblStaticRelationship.Name = "lblStaticRelationship";
            this.lblStaticRelationship.Size = new System.Drawing.Size(122, 23);
            this.lblStaticRelationship.TabIndex = 0;
            this.lblStaticRelationship.Text = "Relationship to Patient:";
            // 
            // lblPolicyHolder
            // 
            this.lblPolicyHolder.Location = new System.Drawing.Point(76, 21);
            this.lblPolicyHolder.Name = "lblPolicyHolder";
            this.lblPolicyHolder.Size = new System.Drawing.Size(412, 23);
            this.lblPolicyHolder.TabIndex = 0;
            // 
            // lblStaticPolicy
            // 
            this.lblStaticPolicy.Location = new System.Drawing.Point(8, 21);
            this.lblStaticPolicy.Name = "lblStaticPolicy";
            this.lblStaticPolicy.Size = new System.Drawing.Size(72, 23);
            this.lblStaticPolicy.TabIndex = 0;
            this.lblStaticPolicy.Text = "Policy holder:";
            // 
            // DisabilityEntitlementPage3View
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpHealthPlan);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtTitle);
            this.Name = "DisabilityEntitlementPage3View";
            this.Size = new System.Drawing.Size(680, 520);
            this.panel1.ResumeLayout(false);
            this.grpHealthPlan.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Construction
        public DisabilityEntitlementPage3View( MSPDialog form )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            parentForm = form;
            formActivating = true;  // Used in setting radio button states
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton                 btnEditInsured;
        private LoggingButton                 btnEditInsurance;
        private LoggingButton                 btnEditPayor;

        private Label                  lblQuestion5;
        private Label                  lblStaticPolicy;
        private Label                  lblPolicyHolder;
        private Label                  lblStaticRelationship;
        private Label                  lblRelationship;

        private Panel                  panel1;

        private RadioButton            rbQuestion5Yes;
        private RadioButton            rbQuestion5No;

        private GroupBox               grpHealthPlan;

        private Label                  lblLine1;
        private Label                  lblLine2;
        private Label                  lblInsuranceAddress;
        private Label                  lblStaticPolicyID;
        private Label                  lblPolicyID;
        private Label                  lblStaticGroupID;
        private Label                  lblGroupID;

        private NonSelectableTextBox txtTitle;

        private Account                                     i_account;
        private MSPDialog                                   parentForm;
        public static bool                                  formActivating;
        private static bool                                  formWasChanged;
        private bool                                        question5Response;
        private int                                         response;
        #endregion

        #region Constants
        const Int32                                         WM_USER                  = 0x400;
        const Int32                                         CONTINUE_BUTTON_DISABLED = WM_USER + 1;
        const Int32                                         CONTINUE_BUTTON_ENABLED  = WM_USER + 2;
        const Int32                                         CONTINUE_BUTTON_FOCUS    = WM_USER + 3;
        const Int32                                         NEXT_BUTTON_DISABLED     = WM_USER + 4;
        const Int32                                         NEXT_BUTTON_ENABLED      = WM_USER + 5;
        const Int32                                         NEXT_BUTTON_FOCUS        = WM_USER + 6;

        private const string NOT_AVAILABLE = "Not available";
        #endregion
    }
}
