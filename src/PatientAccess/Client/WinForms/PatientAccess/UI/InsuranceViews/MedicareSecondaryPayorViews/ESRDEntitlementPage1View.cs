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
    /// Summary description for ESRDEntitlementPage1View.
    /// </summary>
    public class ESRDEntitlementPage1View : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void rbQuestion1Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                DisplayGroupHealthPlanInfo( true );
                DisplayEmploymentInfo( true );

                Model_MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.SetYes();
                GHPSelectd = true;
                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion1No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                GHPSelectd = false;
                DisplayGroupHealthPlanInfo( false );
                DisplayEmploymentInfo( false );

                Model_MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.SetNo();

                ClearPage2Data();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void btnEditInsured_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURED );
        }

        private void btnEditInsurance_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURANCE );
        }

        private void btnEditPayor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.PAYORDETAILS );
        }

        private void btnEditEmployment_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.GUARANTOR );
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

                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
                {
                    return;
                }
                else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( ESRDEntitlement ) ) )
                {   // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls
                    if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.Code.Equals( "Y" ) )
                    {
                        rbQuestion1Yes.Checked = true;
                    }
                    else if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.Code.Equals( "N" ) )
                    {
                        rbQuestion1No.Checked = true;
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
        /// Clears the data on page 2 so the summary results come out correctly.  If the
        /// user comes from page 2 back to page 1 and makes a change that causes page 2
        /// not to be relevant, the data that was set on that page must be removed so it
        /// is not analyzed for the summary.
        /// </summary>
        private void ClearPage2Data()
        {
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).KidneyTransplant.SetBlank();
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).DialysisTreatment.SetBlank();
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).WithinCoordinationPeriod.SetBlank();
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).ESRDandAgeOrDisability.SetBlank();
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).BasedOnESRD.SetBlank();
            (Model_MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement).BasedOnAgeOrDisability.SetBlank();
            ESRDEntitlementPage2View.formActivating = true;
        }
        
        /// <summary>
        /// Display GHP coverage address
        /// </summary>
        private void DisplayEmploymentInfo( bool state )
        {
            SetEmploymentGroupBoxState( state );

            if( state == true )
            {
                if( Model_Account.Guarantor.Employment != null )
                {
                    string guarantor = Model_Account.Guarantor.Employment.Employer.PartyContactPoint.Address.AsMailingLabel();

                    if( guarantor != String.Empty )
                    {
                        lblEmployerInfo.Text = Model_Account.Guarantor.Employment.Employer.Name + 
                                               "\n" + guarantor;
                    }
                    else
                    {
                        lblEmployerInfo.Text = "No employer available";
                    }
                }
            }
        }

        /// <summary>
        /// Populate the controls with the Group Plan Health information
        /// </summary>
        private void DisplayGroupHealthPlanInfo( bool state )
        {
            SetGroupPlanGroupBoxState( state );

            Coverage coverage = GetPrimaryCoverage();

            if( state == false )
            {
                return;
            }
            else if( Model_Account.PrimaryInsured != null && coverage != null )
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
                    lblRelationship.Text = "Not available";
                }
                if( Model_Account.PrimaryInsured.FormattedName != String.Empty )
                {
                    lblPolicyHolder.Text = Model_Account.PrimaryInsured.FormattedName;
                }
                else
                {
                    lblPolicyHolder.Text = NOT_AVAILABLE;
                }
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

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            if( rbQuestion1Yes.Checked )
            {
                response = MSPEventCode.YesStimulus();
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
            }
            else if( rbQuestion1No.Checked )
            {
                response = MSPEventCode.NoStimulus();
                SendMessage(parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero);
                SendMessage(parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero);
                SendMessage(parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
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
            rbQuestion1Yes.Checked = false;
            rbQuestion1No.Checked = false;
            formActivating = false;
        }
        /// <summary>
        /// Set the Enabled property on groupBox and clear label text.
        /// </summary>
        private void SetEmploymentGroupBoxState( bool state )
        {
            if( state == false )
            {
                lblEmployerInfo.Text = String.Empty;
            }
            grpEmployer.Enabled = state;
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
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.rbQuestion1Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion1No = new System.Windows.Forms.RadioButton();
            this.grpHealthPlan = new System.Windows.Forms.GroupBox();
            this.lblInsuranceAddress = new System.Windows.Forms.Label();
            this.btnEditInsured = new LoggingButton();
            this.btnEditInsurance = new LoggingButton();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.btnEditPayor = new LoggingButton();
            this.lblGroupID = new System.Windows.Forms.Label();
            this.lblPolicyID = new System.Windows.Forms.Label();
            this.lblStaticGroupID = new System.Windows.Forms.Label();
            this.lblStaticPolicyID = new System.Windows.Forms.Label();
            this.lblRelationship = new System.Windows.Forms.Label();
            this.lblStaticRelationship = new System.Windows.Forms.Label();
            this.lblPolicyHolder = new System.Windows.Forms.Label();
            this.lblStaticPolicy = new System.Windows.Forms.Label();
            this.grpEmployer = new System.Windows.Forms.GroupBox();
            this.lblEmployerInfo = new System.Windows.Forms.Label();
            this.btnEditEmployment = new LoggingButton();
            this.panel1.SuspendLayout();
            this.grpHealthPlan.SuspendLayout();
            this.grpEmployer.SuspendLayout();
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
            this.txtTitle.Size = new System.Drawing.Size(160, 23);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Text = "Entitlement by ESRD";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.rbQuestion1Yes);
            this.panel1.Controls.Add(this.rbQuestion1No);
            this.panel1.Location = new System.Drawing.Point(16, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(288, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "1. Do you have group health plan (GHP) coverage?";
            // 
            // rbQuestion1Yes
            // 
            this.rbQuestion1Yes.Location = new System.Drawing.Point(470, 0);
            this.rbQuestion1Yes.Name = "rbQuestion1Yes";
            this.rbQuestion1Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion1Yes.TabIndex = 1;
            this.rbQuestion1Yes.TabStop = true;
            this.rbQuestion1Yes.Text = "Yes";
            this.rbQuestion1Yes.CheckedChanged += new System.EventHandler(this.rbQuestion1Yes_CheckedChanged);
            // 
            // rbQuestion1No
            // 
            this.rbQuestion1No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion1No.Name = "rbQuestion1No";
            this.rbQuestion1No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion1No.TabIndex = 2;
            this.rbQuestion1No.TabStop = true;
            this.rbQuestion1No.Text = "No";
            this.rbQuestion1No.CheckedChanged += new System.EventHandler(this.rbQuestion1No_CheckedChanged);
            // 
            // grpHealthPlan
            // 
            this.grpHealthPlan.Controls.Add(this.lblInsuranceAddress);
            this.grpHealthPlan.Controls.Add(this.btnEditInsured);
            this.grpHealthPlan.Controls.Add(this.btnEditInsurance);
            this.grpHealthPlan.Controls.Add(this.lblLine2);
            this.grpHealthPlan.Controls.Add(this.lblLine1);
            this.grpHealthPlan.Controls.Add(this.btnEditPayor);
            this.grpHealthPlan.Controls.Add(this.lblGroupID);
            this.grpHealthPlan.Controls.Add(this.lblPolicyID);
            this.grpHealthPlan.Controls.Add(this.lblStaticGroupID);
            this.grpHealthPlan.Controls.Add(this.lblStaticPolicyID);
            this.grpHealthPlan.Controls.Add(this.lblRelationship);
            this.grpHealthPlan.Controls.Add(this.lblStaticRelationship);
            this.grpHealthPlan.Controls.Add(this.lblPolicyHolder);
            this.grpHealthPlan.Controls.Add(this.lblStaticPolicy);
            this.grpHealthPlan.Enabled = false;
            this.grpHealthPlan.Location = new System.Drawing.Point(27, 110);
            this.grpHealthPlan.Name = "grpHealthPlan";
            this.grpHealthPlan.Size = new System.Drawing.Size(504, 225);
            this.grpHealthPlan.TabIndex = 2;
            this.grpHealthPlan.TabStop = false;
            this.grpHealthPlan.Text = "Group Health Plan";
            // 
            // lblInsuranceAddress
            // 
            this.lblInsuranceAddress.Location = new System.Drawing.Point(8, 94);
            this.lblInsuranceAddress.Name = "lblInsuranceAddress";
            this.lblInsuranceAddress.Size = new System.Drawing.Size(250, 40);
            this.lblInsuranceAddress.TabIndex = 0;
            // 
            // btnEditInsured
            // 
            this.btnEditInsured.Location = new System.Drawing.Point(310, 53);
            this.btnEditInsured.Name = "btnEditInsured";
            this.btnEditInsured.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsured.TabIndex = 1;
            this.btnEditInsured.Text = "Edit &Insured && Cancel MSP";
            this.btnEditInsured.Click += new System.EventHandler(this.btnEditInsured_Click);
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point(310, 94);
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsurance.TabIndex = 2;
            this.btnEditInsurance.Text = "Edit I&nsurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler(this.btnEditInsurance_Click);
            // 
            // lblLine2
            // 
            this.lblLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine2.Location = new System.Drawing.Point(0, 145);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(502, 1);
            this.lblLine2.TabIndex = 0;
            this.lblLine2.Text = "label1";
            // 
            // lblLine1
            // 
            this.lblLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine1.Location = new System.Drawing.Point(0, 81);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(502, 1);
            this.lblLine1.TabIndex = 0;
            // 
            // btnEditPayor
            // 
            this.btnEditPayor.Location = new System.Drawing.Point(310, 160);
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
            // lblPolicyID
            // 
            this.lblPolicyID.Location = new System.Drawing.Point(59, 160);
            this.lblPolicyID.Name = "lblPolicyID";
            this.lblPolicyID.Size = new System.Drawing.Size(240, 23);
            this.lblPolicyID.TabIndex = 0;
            // 
            // lblStaticGroupID
            // 
            this.lblStaticGroupID.Location = new System.Drawing.Point(8, 192);
            this.lblStaticGroupID.Name = "lblStaticGroupID";
            this.lblStaticGroupID.Size = new System.Drawing.Size(54, 23);
            this.lblStaticGroupID.TabIndex = 0;
            this.lblStaticGroupID.Text = "Group ID:";
            // 
            // lblStaticPolicyID
            // 
            this.lblStaticPolicyID.Location = new System.Drawing.Point(8, 160);
            this.lblStaticPolicyID.Name = "lblStaticPolicyID";
            this.lblStaticPolicyID.Size = new System.Drawing.Size(56, 23);
            this.lblStaticPolicyID.TabIndex = 0;
            this.lblStaticPolicyID.Text = "Policy ID:";
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
            // grpEmployer
            // 
            this.grpEmployer.Controls.Add(this.lblEmployerInfo);
            this.grpEmployer.Controls.Add(this.btnEditEmployment);
            this.grpEmployer.Enabled = false;
            this.grpEmployer.Location = new System.Drawing.Point(27, 347);
            this.grpEmployer.Name = "grpEmployer";
            this.grpEmployer.Size = new System.Drawing.Size(504, 75);
            this.grpEmployer.TabIndex = 3;
            this.grpEmployer.TabStop = false;
            this.grpEmployer.Text = "Name and address of employer, if any, from which you received GHP coverage:";
            // 
            // lblEmployerInfo
            // 
            this.lblEmployerInfo.Location = new System.Drawing.Point(12, 21);
            this.lblEmployerInfo.Name = "lblEmployerInfo";
            this.lblEmployerInfo.Size = new System.Drawing.Size(284, 40);
            this.lblEmployerInfo.TabIndex = 0;
            // 
            // btnEditEmployment
            // 
            this.btnEditEmployment.Location = new System.Drawing.Point(310, 21);
            this.btnEditEmployment.Name = "btnEditEmployment";
            this.btnEditEmployment.Size = new System.Drawing.Size(180, 23);
            this.btnEditEmployment.TabIndex = 1;
            this.btnEditEmployment.Text = "Edit &Guarantor && Cancel MSP";
            this.btnEditEmployment.Click += new System.EventHandler(this.btnEditEmployment_Click);
            // 
            // ESRDEntitlementPage1View
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpEmployer);
            this.Controls.Add(this.grpHealthPlan);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.panel1);
            this.Name = "ESRDEntitlementPage1View";
            this.Size = new System.Drawing.Size(680, 520);
            this.panel1.ResumeLayout(false);
            this.grpHealthPlan.ResumeLayout(false);
            this.grpEmployer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public ESRDEntitlementPage1View( MSPDialog form )
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
        private Container             components = null;

        private LoggingButton                 btnEditPayor;
        private LoggingButton                 btnEditInsured;
        private LoggingButton                 btnEditEmployment;
        private LoggingButton                 btnEditInsurance;

        private GroupBox               grpEmployer;
        private GroupBox               grpHealthPlan;

        private Label                  label2;
        private Label                  lblLine2;
        private Label                  lblLine1;
        private Label                  lblGroupID;
        private Label                  lblPolicyID;
        private Label                  lblStaticGroupID;
        private Label                  lblStaticPolicyID;
        private Label                  lblRelationship;
        private Label                  lblStaticRelationship;
        private Label                  lblPolicyHolder;
        private Label                  lblStaticPolicy;
        private Label                  lblInsuranceAddress;
        private Label                  lblEmployerInfo;

        private Panel                  panel1;

        private RadioButton            rbQuestion1Yes;
        private RadioButton            rbQuestion1No;

        private NonSelectableTextBox txtTitle;

        private Account                                     i_account;
        private MSPDialog                                   parentForm;
        private bool                                        formActivating;
        private static bool                                  formWasChanged;
        public static bool GHPSelectd;
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
