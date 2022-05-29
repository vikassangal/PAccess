using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.InsuranceVerificationViews;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
	public class InsuranceDetailsView : ControlView
	{
        #region Event handlers

        private void ResetButtonEventHandler( object sender, EventArgs e )
        {
            //Model_Coverage = null;
        }

        private void InsuranceDetailsView_Load( object sender, EventArgs e )
        {
            if( Model_Coverage != null )
            {
                savedModelCoverage = Model_Coverage.DeepCopy() as Coverage;
            }
            else
            {
                savedModelCoverage = null;
            }

            if( ( this.Account != null ) && ( this.Account.MedicalGroupIPA != null ) )
            {
                savedMedicalGroupIPA = this.Account.MedicalGroupIPA.DeepCopy() as MedicalGroupIPA;
            }
            else
            {
                savedMedicalGroupIPA = null;
            }

            tabControl.SelectedIndex = Active_Tab;
            //UpdateView();
        }
        private void TabControlSelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateView();
        }

        public void CancelThis()
        {

            try
            {

                // Have to turn change-tracking off to avoid creating a false event
                // Somebody, please, switch the implementation so the model is only
                // modified AFTER the okay button is clicked.
                Extensions.Model.IsTrackingEnabled = false;

                this.Model_Coverage = this.savedModelCoverage;

                this.Account.MedicalGroupIPA = this.savedMedicalGroupIPA;

                if( Model_Coverage != null )
                {

                    if( Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                    {
                        this.Account.Insurance.RemovePrimaryCoverage();
                        this.Account.Insurance.AddCoverage( this.Model_Coverage );
                    }
                    else
                    {
                        this.Account.Insurance.RemoveSecondaryCoverage();
                        this.Account.Insurance.AddCoverage( this.Model_Coverage );
                    }
                }

            }//try
            finally
            {

                Extensions.Model.IsTrackingEnabled = true;

            }//finally

        }
        private void btnCancel_Click( object sender, EventArgs e )
        {   
           this.CancelThis();
        }

	    private bool FinancialClassIsSignedOverMedicare()
	    {
	        return Account != null && Account.FinancialClass != null && Account.FinancialClass.IsSignedOverMedicare();
	    }
 
        private void BackButtonClick( object sender, EventArgs e )
        {
            switch( tabControl.SelectedIndex )
            {
                case INSURED_DETAILS_PAGE:
                    tabControl.SelectedIndex = PLAN_DETAILS_PAGE;
                    break;
                case VERIFICATION_DETAILS_PAGE:
                    tabControl.SelectedIndex = INSURED_DETAILS_PAGE;
                    break;
                case INSURANCE_AUTHORIZATION_PAGE:
                    tabControl.SelectedIndex = VERIFICATION_DETAILS_PAGE;
                    break;
                default:
                    tabControl.SelectedIndex = PLAN_DETAILS_PAGE;
                    break;
            }
        }

        private void NextButtonClick( object sender, EventArgs e )
        {
            switch( tabControl.SelectedIndex )
            {
                case PLAN_DETAILS_PAGE:
                    tabControl.SelectedIndex = INSURED_DETAILS_PAGE;
                    break;
                case INSURED_DETAILS_PAGE:
                    tabControl.SelectedIndex = VERIFICATION_DETAILS_PAGE;
                    break;
                case VERIFICATION_DETAILS_PAGE:
                    tabControl.SelectedIndex = INSURANCE_AUTHORIZATION_PAGE;
                    break;
               
                default:
                    tabControl.SelectedIndex = VERIFICATION_DETAILS_PAGE;
                    break;
            }
        }

        private void btnOK_Click( object sender, EventArgs e )
        {
            // Get the data from the view            
            if( insDetailInsuredView.CheckValidations() )
            {
                if( Model_Coverage != null )
                {
                    insDetailInsuredView.UpdateModel();
                    insDetailPlanDetails.UpdateModel();
                    insuranceVerificationView.UpdateModel();
                    authorizationView.UpdateModel();
                
                    //Model_Coverage.Insured = insDetailInsuredView.Model_Insured;
                    //Model_Coverage.InsurancePlan = insDetailPlanDetails.Model_Coverage.InsurancePlan;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            if( Model_Coverage != null )
            {
                insDetailPlanDetails.Model_Coverage = Model_Coverage;
                insDetailPlanDetails.Account = this.Account;
                insDetailPlanDetails.UpdateView();  

                insDetailInsuredView.Model_Coverage = Model_Coverage;
                insDetailInsuredView.Model_Insured = Model_Coverage.Insured;
                insDetailInsuredView.Model_Account = this.Account;
                insDetailInsuredView.UpdateView();

                insuranceVerificationView.Model_Coverage = Model_Coverage;
                insuranceVerificationView.Model_Account = this.Account;

                if (this.tabControl.SelectedIndex == 2)
                {
                    insuranceVerificationView.UpdateView( true );
                }
                else
                {
                    insuranceVerificationView.UpdateView( false );
                }

                authorizationView.Model = Model_Coverage;
                authorizationView.Account = this.Account;
                authorizationView.UpdateView();

                if( Model_Coverage.CoverageOrder != null )
                {
                    lblPriority.Text = Model_Coverage.CoverageOrder.Description;
                }
                else
                {
                    lblPriority.Text = "None";
                }
                if( Model_Coverage.InsurancePlan != null )
                {
                    string planText = Model_Coverage.InsurancePlan.PlanID + " " 
                        + Model_Coverage.InsurancePlan.PlanName.ToUpper();
                    
                    lblPlan.Text = planText;

                    if( Model_Coverage.InsurancePlan.PlanCategory != null )
                    {
                        string categoryText = Model_Coverage.InsurancePlan.PlanCategory.Description;
                        if( lblCategory.Text == "Self-pay" )
                        {
                            this.insDetailPlanDetails.EditBtnResetText = "&Clear All";
                        }
                        lblCategory.Text = @categoryText;
                    }
                    else
                    {
                        lblCategory.Text = "None";
                    }
                    if( Model_Coverage.InsurancePlan.Payor != null )
                    {
                        string payorText = Model_Coverage.InsurancePlan.Payor.Name.ToUpper();                        
                        lblPayor.Text = @payorText;
                    }
                    else
                    {
                        lblPayor.Text = "NONE";
                    }
                }
                else
                {
                    lblPlan.Text = "NONE";
                }
            }

            if( tabControl.SelectedTab == tabPlanDetailsPage )
            {                              
                DisplayPlanDetailsPage();
            }
            else if( tabControl.SelectedTab == tabInsuredPage )
            {                
                DisplayInsuredDetailsPage();
            }
            else if( tabControl.SelectedTab == tabVerificationPage )
            {                
                DisplayVerificationDetailsPage();
            }
            else if( tabControl.SelectedTab == tabAuthorizationPage )
            {                
                DisplayAuthorizationDetailsPage();
            }
            else
            {                
                DisplayPlanDetailsPage();
            }
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }
        #endregion

        #region Properties

        public Coverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            get
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
        public int Active_Tab
        {
            private get
            {
                return activeTab;
            }
            set
            {
                activeTab = value;
            }
        }

        #endregion

        #region Construction and Finalization
        
		public InsuranceDetailsView()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

            base.EnableThemesOn( this );
            btnBack.Enabled = false;
            activeTab = 1; //PLAN_DETAILS_PAGE;			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
        
        #endregion

        #region Private methods
        
        /// <summary>
        /// Set proper tab button states & display the plan details page.
        /// </summary>
        private void DisplayPlanDetailsPage()
        {
            btnBack.Enabled = false;
            btnNext.Enabled = true;
        }

        /// <summary>
        /// Set proper tab button states & display the insured details page.
        /// </summary>
        private void DisplayInsuredDetailsPage()
        {
            btnBack.Enabled = true;
            btnNext.Enabled = true;
        }

        /// <summary>
        /// Set proper tab button states & display the insured details page.
        /// </summary>
        private void DisplayVerificationDetailsPage()
        {
            btnBack.Enabled = true;
            btnNext.Enabled = true;
        }
        
        /// <summary>
        /// Set proper tab button states & display the insured details page.
        /// </summary>
        private void DisplayAuthorizationDetailsPage()
        {
            btnBack.Enabled = true;
            btnNext.Enabled = false;
        }


		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mainPanel = new System.Windows.Forms.Panel();
            this.labelPanel = new System.Windows.Forms.Panel();
            this.lblPayor = new System.Windows.Forms.Label();
            this.lblPlan = new System.Windows.Forms.Label();
            this.lblStaticPayor = new System.Windows.Forms.Label();
            this.lblPriority = new System.Windows.Forms.Label();
            this.lblStaticPlan = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblStaticCategory = new System.Windows.Forms.Label();
            this.lblStaticPriority = new System.Windows.Forms.Label();
            this.tabPanel = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPlanDetailsPage = new System.Windows.Forms.TabPage();
            this.insDetailPlanDetails = new PatientAccess.UI.InsuranceViews.InsDetailPlanDetails();
            this.tabInsuredPage = new System.Windows.Forms.TabPage();
            this.insDetailInsuredView = new PatientAccess.UI.InsuranceViews.InsDetailInsuredView();
            this.tabVerificationPage = new System.Windows.Forms.TabPage();
            this.tabAuthorizationPage = new System.Windows.Forms.TabPage();
            this.insuranceVerificationView = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.InsuranceVerificationView();
            this.authorizationView = new PatientAccess.UI.InsuranceViews.AuthorizationView();
            this.insuranceDetailsTabView = new PatientAccess.UI.InsuranceViews.InsuranceDetailsTabView();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.btnCancel = new LoggingButton();
            this.btnBack = new LoggingButton();
            this.btnNext = new LoggingButton();
            this.btnOK = new LoggingButton();
            this.mainPanel.SuspendLayout();
            this.labelPanel.SuspendLayout();
            this.tabPanel.SuspendLayout();
            this.insuranceDetailsTabView.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPlanDetailsPage.SuspendLayout();
            this.tabInsuredPage.SuspendLayout();
            this.tabVerificationPage.SuspendLayout();
            this.tabAuthorizationPage.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.labelPanel);
            this.mainPanel.Controls.Add(this.tabPanel);
            this.mainPanel.Controls.Add(this.buttonPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.DockPadding.Left = 5;
            this.mainPanel.DockPadding.Right = 5;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(910, 555);
            this.mainPanel.TabIndex = 2;
            this.mainPanel.TabStop = true;
            // 
            // labelPanel
            // 
            this.labelPanel.Controls.Add(this.lblPayor);
            this.labelPanel.Controls.Add(this.lblPlan);
            this.labelPanel.Controls.Add(this.lblStaticPayor);
            this.labelPanel.Controls.Add(this.lblPriority);
            this.labelPanel.Controls.Add(this.lblStaticPlan);
            this.labelPanel.Controls.Add(this.lblCategory);
            this.labelPanel.Controls.Add(this.lblStaticCategory);
            this.labelPanel.Controls.Add(this.lblStaticPriority);
            this.labelPanel.DockPadding.Left = 10;
            this.labelPanel.DockPadding.Right = 10;
            this.labelPanel.Location = new System.Drawing.Point(10, 6);
            this.labelPanel.Name = "labelPanel";
            this.labelPanel.Size = new System.Drawing.Size(893, 33);
            this.labelPanel.TabIndex = 0;
            // 
            // lblPayor
            // 
            this.lblPayor.Location = new System.Drawing.Point(425, 0);
            this.lblPayor.Name = "lblPayor";
            this.lblPayor.Size = new System.Drawing.Size(155, 45);
            this.lblPayor.TabIndex = 7;
            this.lblPayor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPayor.UseMnemonic = false;
            // 
            // lblPlan
            // 
            this.lblPlan.Location = new System.Drawing.Point(619, 0);
            this.lblPlan.Name = "lblPlan";
            this.lblPlan.Size = new System.Drawing.Size(263, 45);
            this.lblPlan.TabIndex = 5;
            this.lblPlan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblPlan.UseMnemonic = false;
            // 
            // lblStaticPayor
            // 
            this.lblStaticPayor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticPayor.Location = new System.Drawing.Point(343, 0);
            this.lblStaticPayor.Name = "lblStaticPayor";
            this.lblStaticPayor.Size = new System.Drawing.Size(77, 23);
            this.lblStaticPayor.TabIndex = 6;
            this.lblStaticPayor.Text = "Payor/broker:";
            this.lblStaticPayor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPriority
            // 
            this.lblPriority.Location = new System.Drawing.Point(83, 0);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(51, 23);
            this.lblPriority.TabIndex = 1;
            this.lblPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticPlan
            // 
            this.lblStaticPlan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticPlan.Location = new System.Drawing.Point(585, 0);
            this.lblStaticPlan.Name = "lblStaticPlan";
            this.lblStaticPlan.Size = new System.Drawing.Size(31, 23);
            this.lblStaticPlan.TabIndex = 4;
            this.lblStaticPlan.Text = "Plan:";
            this.lblStaticPlan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCategory
            // 
            this.lblCategory.Location = new System.Drawing.Point(201, 0);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(135, 20);
            this.lblCategory.TabIndex = 3;
            this.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticCategory
            // 
            this.lblStaticCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticCategory.Location = new System.Drawing.Point(141, 0);
            this.lblStaticCategory.Name = "lblStaticCategory";
            this.lblStaticCategory.Size = new System.Drawing.Size(56, 23);
            this.lblStaticCategory.TabIndex = 2;
            this.lblStaticCategory.Text = "Category:";
            this.lblStaticCategory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStaticPriority
            // 
            this.lblStaticPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblStaticPriority.Location = new System.Drawing.Point(0, 0);
            this.lblStaticPriority.Name = "lblStaticPriority";
            this.lblStaticPriority.Size = new System.Drawing.Size(78, 23);
            this.lblStaticPriority.TabIndex = 0;
            this.lblStaticPriority.Text = "Payor priority:";
            this.lblStaticPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPanel
            // 
            //this.tabPanel.Controls.Add(this.tabControl);
            this.tabPanel.Controls.Add(this.insuranceDetailsTabView);
            this.tabPanel.Location = new System.Drawing.Point(10, 40);
            this.tabPanel.Name = "tabPanel";
            this.tabPanel.Size = new System.Drawing.Size(893, 481);
            this.tabPanel.TabIndex = 1;
            this.tabPanel.TabStop = true;
            //
            // insuranceDetailsTabView
            //
            this.insuranceDetailsTabView.Controls.Add(this.tabControl);
            this.insuranceDetailsTabView.Location = new System.Drawing.Point(0, 0);
            this.insuranceDetailsTabView.Name = "insuranceDetailsTabView";
            this.insuranceDetailsTabView.Size = new System.Drawing.Size(893, 481);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPlanDetailsPage);
            this.tabControl.Controls.Add(this.tabInsuredPage);
            this.tabControl.Controls.Add(this.tabVerificationPage);
            this.tabControl.Controls.Add(this.tabAuthorizationPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(893, 481);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControlSelectedIndexChanged);
            // 
            // tabPlanDetailsPage
            // 
            this.tabPlanDetailsPage.BackColor = System.Drawing.Color.White;
            this.tabPlanDetailsPage.Controls.Add(this.insDetailPlanDetails);
            this.tabPlanDetailsPage.Location = new System.Drawing.Point(4, 22);
            this.tabPlanDetailsPage.Name = "tabPlanDetailsPage";
            this.tabPlanDetailsPage.Size = new System.Drawing.Size(885, 455);
            this.tabPlanDetailsPage.TabIndex = 4;
            this.tabPlanDetailsPage.Text = "Payor Details";
            // 
            // insDetailPlanDetails
            // 
            this.insDetailPlanDetails.Account = null;
            this.insDetailPlanDetails.BackColor = System.Drawing.Color.White;
            this.insDetailPlanDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.insDetailPlanDetails.Location = new System.Drawing.Point(0, 0);
            this.insDetailPlanDetails.Model = null;
            this.insDetailPlanDetails.Model_Coverage = null;
            this.insDetailPlanDetails.Name = "insDetailPlanDetails";
            this.insDetailPlanDetails.Size = new System.Drawing.Size(872, 450);
            this.insDetailPlanDetails.TabIndex = 0;
            this.insDetailPlanDetails.ResetButtonClicked += new System.EventHandler(this.ResetButtonEventHandler);
            // 
            // tabInsuredPage
            // 
            this.tabInsuredPage.Controls.Add(this.insDetailInsuredView);
            this.tabInsuredPage.BackColor = System.Drawing.Color.White;
            this.tabInsuredPage.Location = new System.Drawing.Point(4, 22);
            this.tabInsuredPage.Name = "tabInsuredPage";
            this.tabInsuredPage.Size = new System.Drawing.Size(885, 455);
            this.tabInsuredPage.TabIndex = 5;
            this.tabInsuredPage.Text = "Insured";
            this.tabInsuredPage.Visible = false;
            // 
            // insDetailInsuredView
            // 
            this.insDetailInsuredView.BackColor = System.Drawing.Color.White;
            this.insDetailInsuredView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.insDetailInsuredView.Location = new System.Drawing.Point(5, 5);
            this.insDetailInsuredView.Model = null;
            this.insDetailInsuredView.Model_Account = null;
            this.insDetailInsuredView.Model_Coverage = null;
            this.insDetailInsuredView.Model_Insured = null;
            this.insDetailInsuredView.Name = "insDetailInsuredView";
            this.insDetailInsuredView.Size = new System.Drawing.Size(880, 450);
            this.insDetailInsuredView.TabIndex = 0;
            // 
            // tabVerificationPage
            // 
            this.tabVerificationPage.BackColor = System.Drawing.Color.White;
            this.tabVerificationPage.Controls.Add(this.insuranceVerificationView);
            this.tabVerificationPage.Location = new System.Drawing.Point(4, 22);
            this.tabVerificationPage.Name = "tabVerificationPage";
            this.tabVerificationPage.Size = new System.Drawing.Size(885, 455);
            this.tabVerificationPage.TabIndex = 6;
            this.tabVerificationPage.Text = "Verification";
            this.tabVerificationPage.Visible = false;
            // 
            // insuranceVerificationView
            // 
            this.insuranceVerificationView.AutoScroll = true;
            this.insuranceVerificationView.BackColor = System.Drawing.Color.White;
            this.insuranceVerificationView.Location = new System.Drawing.Point(0, 0);
            this.insuranceVerificationView.Model = null;
            this.insuranceVerificationView.Model_Account = null;
            this.insuranceVerificationView.Model_Coverage = null;
            this.insuranceVerificationView.Model_Insured = null;
            this.insuranceVerificationView.Name = "insuranceVerificationView";
            this.insuranceVerificationView.Size = new System.Drawing.Size(885, 455);
            this.insuranceVerificationView.TabIndex = 0;
            // 
            // tabAuthorizationPage
            // 
            this.tabAuthorizationPage.BackColor = System.Drawing.Color.White;
            this.tabAuthorizationPage.Controls.Add( this.authorizationView );
            this.tabAuthorizationPage.Location = new System.Drawing.Point( 4, 22 );
            this.tabAuthorizationPage.Name = "tabAuthorizationPage";
            this.tabAuthorizationPage.Size = new System.Drawing.Size( 885, 455 );
            this.tabAuthorizationPage.TabIndex = 6;
            this.tabAuthorizationPage.Text = "Authorization";
            this.tabAuthorizationPage.Visible = false;
            // 
            // authorizationView
            // 
            this.authorizationView.BackColor = System.Drawing.Color.White;
            this.authorizationView.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (System.Byte)( 0 ) ) );
            this.authorizationView.Location = new System.Drawing.Point( 0, 0 );
            this.authorizationView.Model = null;
            this.authorizationView.Account = null;
            //this.authorizationView.Model_Insured = null;
            this.authorizationView.Name = "authorizationView";
            this.authorizationView.Size = new System.Drawing.Size( 885, 455 );
            this.authorizationView.TabIndex = 0;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Controls.Add(this.btnBack);
            this.buttonPanel.Controls.Add(this.btnNext);
            this.buttonPanel.Controls.Add(this.btnOK);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(5, 520);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(900, 35);
            this.buttonPanel.TabIndex = 2;
            this.buttonPanel.TabStop = true;
            // 
            // btnCancel
            // 
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(570, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(658, 7);
            this.btnBack.Name = "btnBack";
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "< &Back";
            this.btnBack.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(738, 7);
            this.btnNext.Name = "btnNext";
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "&Next >";
            this.btnNext.Click += new System.EventHandler(this.NextButtonClick);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(818, 7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 24);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // InsuranceDetailsView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.Controls.Add(this.mainPanel);
            this.Name = "InsuranceDetailsView";
            this.Size = new System.Drawing.Size(910, 555);
            this.Load += new System.EventHandler(this.InsuranceDetailsView_Load);
           
            this.mainPanel.ResumeLayout(false);
            this.labelPanel.ResumeLayout(false);
            this.tabPanel.ResumeLayout(false);
            this.insuranceDetailsTabView.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabPlanDetailsPage.ResumeLayout(false);
            this.tabVerificationPage.ResumeLayout(false);
            this.tabAuthorizationPage.ResumeLayout( false );
            this.tabInsuredPage.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

        #endregion

        #region Data elements

        private Panel          mainPanel;
        private Panel          labelPanel;
        private Panel          buttonPanel;

        private Label          lblPayor;
        private Label          lblPlan;
        private Label          lblStaticPayor;
        private Label          lblPriority;
        private Label          lblStaticPlan;
        private Label          lblCategory;
        private Label          lblStaticCategory;
        private Label          lblStaticPriority;

        private Panel          tabPanel;
        private TabControl     tabControl;
        private TabPage        tabPlanDetailsPage;        
        private TabPage        tabInsuredPage;
        private TabPage        tabVerificationPage;
        private TabPage        tabAuthorizationPage;

        private AuthorizationView                   authorizationView;
        private InsuranceVerificationView           insuranceVerificationView;
        private InsDetailPlanDetails                insDetailPlanDetails;
        private InsDetailInsuredView                insDetailInsuredView;
        private InsuranceDetailsTabView             insuranceDetailsTabView;
        
        private LoggingButton         btnCancel;
        private LoggingButton         btnBack;
        private LoggingButton         btnNext;
        private LoggingButton         btnOK;

        private IContainer    components = null;

        private int                                 activeTab;
        private Account                             i_Account;
        private Coverage                            savedModelCoverage;
        private MedicalGroupIPA                     savedMedicalGroupIPA;

        #endregion

        #region Constants

        // The MSP Wizard has a button the invokes this dialog and displays the
        // Insured & Payor Details tabPages, so the tab index must be accessible.

        public const int                        PLAN_DETAILS_PAGE          = 0;
        public const int                        INSURED_DETAILS_PAGE       = 1;
        public const int                        VERIFICATION_DETAILS_PAGE  = 2;
        public const int                        INSURANCE_AUTHORIZATION_PAGE = 3;
        
        #endregion
	}
}

