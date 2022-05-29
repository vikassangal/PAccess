using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// ESRDEntitlementPage1 - the first Entitlement by ESRD page; captures if the patient is covered by GHP
    /// from their or spouse's employment
    /// </summary>
    [Serializable]
    public class ESRDEntitlementPage1 : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// btnInsEditInsurance_Click - the user has elected to abandon the MSP wizard and edit the insurance info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsEditInsurance_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.INSURANCE );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.INSURANCE );
            }
        }

        /// <summary>
        /// btnInsEditInsurance_Click - the user has elected to abandon the MSP wizard and edit the primary insured info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditInsured_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.INSURED );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.INSURED );
            }
        }

        /// <summary>
        /// btnEditInsurance_Click - the user has elected to abandon the MSP wizard and edit the insurance info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditInsurance_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.INSURANCE );
            }
            else
            {
                ( ( MSP2Dialog ) ParentForm).RaiseTabSelectedEvent( ( int ) AccountView.ScreenIndexes.INSURANCE );
            }
        }

        /// <summary>
        /// btnInsEditInsurance_Click - the user has elected to abandon the MSP wizard and edit the primary payor info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditPayor_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.PAYORDETAILS );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.PAYORDETAILS );
            }
        }

        /// <summary>
        /// btnEditEmployment_Click - the user has elected to abandon the MSP wizard and edit the Guarantor employment
        /// info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditEmployment_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.SHORTGUARANTOR );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.GUARANTOR );
            }
        }

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup1_RadioChanged(object sender, EventArgs e)
        {        
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                
                this.grpHealthPlan.Enabled      = true;
                this.grpEmployer.Enabled        = true;
                this.pnlInsurance.Enabled       = true;
                this.DisplayInsurance           = true;
                this.displayInsurance( true );
                this.ResetPreviouslySelectedPages(true);
                this.previousGHPSelection = true;

            }      
            else
            {
                this.grpHealthPlan.Enabled      = false;
                this.grpEmployer.Enabled        = false;
                this.pnlInsurance.Enabled       = false;
                this.displayInsurance( false );
                this.ResetPreviouslySelectedPages(false);
                this.previousGHPSelection = false;
            }
            this.CanPageNavigate();
            this.MyWizardLinks.SetPanel();
        }
        private void ResetESRDPage2()
        {
            var esrdPage = this.MyWizardContainer.GetWizardPage("ESRDEntitlementPage2")
                           as ESRDEntitlementPage2;
            if (esrdPage == null) return;
            esrdPage.ResetPage();
            esrdPage.DisablePage();
        }

        private void ResetSumamryPage()
        {
            var sumamryPage = this.MyWizardContainer.GetWizardPage("SummaryPage")
                              as SummaryPage;
            if (sumamryPage != null)
            {
                sumamryPage.DisablePage();
            }
        }

        private void ResetPreviouslySelectedPages(bool currentSelection)
        {
            if (currentSelection == this.previousGHPSelection) return;
            ResetESRDPage2();
            ResetSumamryPage();
        }
        /// <summary>
        /// ESRDEntitlementPage1_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ESRDEntitlementPage1_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled )
            {
                this.UpdateView();
            }
        }

        /// <summary>
        /// ESRDEntitlementPage1_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ESRDEntitlementPage1_Load(object sender, EventArgs e)
        {
            this.LinkName                           = "Entitlement by ESRD";
            this.MyWizardMessages.Message1          = "Entitlement by ESRD";            
            this.MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1         = 8.25;
            this.MyWizardMessages.FontStyle1        = FontStyle.Bold;

            this.MyWizardMessages.Message2          = string.Empty;

            this.MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize2         = 8.25;

            this.MyWizardMessages.ShowMessages();

            this.pnlInsurance.Enabled               = false;
            this.grpHealthPlan.Enabled              = false;
            this.grpEmployer.Enabled                = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        public override void ResetPage()
        {
            base.ResetPage ();
        
            this.checkBoxYesNoGroup1.rbNo.Checked       = false;
            this.checkBoxYesNoGroup1.rbYes.Checked      = false;

            this.grpHealthPlan.Enabled              = false;
            this.grpEmployer.Enabled                = false;
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        public bool CheckForSummary()
        {
            bool rc = false;

            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if( this.CanNavigate )
            {
                if (this.checkBoxYesNoGroup1.rbNo.Checked)
                {
                    rc = true;
                }

            }

            this.HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// Cancel - is the delegate for the Cancel button click event
        /// </summary>
        private void Cancel()
        {
            this.MyWizardContainer.Cancel();

            if( this.MSPCancelled != null )
            {
                this.MSPCancelled(this, null);
            }
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {       
            bool canNav = false;

            this.MyWizardButtons.UpdateNavigation("&Next >", string.Empty);
            this.MyWizardButtons.UpdateNavigation("&Continue to Summary", string.Empty);

            if( this.checkBoxYesNoGroup1.rbNo.Checked )
            {
                canNav = true;
                this.MyWizardButtons.UpdateNavigation("&Next >", "ESRDEntitlementPage2");
                this.MyWizardButtons.SetAcceptButton("&Next >");          
            }
            else if( this.checkBoxYesNoGroup1.rbYes.Checked )
            {
                this.MyWizardButtons.UpdateNavigation("&Next >", "ESRDEntitlementPage2");
                this.MyWizardButtons.SetAcceptButton( "&Next >" );
                canNav = true;
            }

            this.CanNavigate = canNav;

            this.CheckForSummary();

            return canNav;
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();

            if( !blnLoaded )
            {
                blnLoaded       = true;   
             
                if( Model_Account == null
                    || Model_Account.MedicareSecondaryPayor == null
                    || Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
                {
                    return;
                }

                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( ESRDEntitlement ) ) )
                {   
                    ESRDEntitlement esrdEntitlement = Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as ESRDEntitlement;

                    if( esrdEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES )
                    {
                        this.checkBoxYesNoGroup1.rbYes.Checked  = true;
                    }
                    else if( esrdEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_NO )
                    {
                        this.checkBoxYesNoGroup1.rbNo.Checked   = true;
                    }
                }
            }            

            this.CanPageNavigate();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel ();

            ESRDEntitlement entitlement = null;

            if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                entitlement = new ESRDEntitlement();
            }
            else
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType()
                    != typeof(ESRDEntitlement) )
                {
                    entitlement = new ESRDEntitlement();
                }
                else
                    entitlement = this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement as ESRDEntitlement;
            }

            if( this.checkBoxYesNoGroup1.rbYes.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_YES;
            }
            else if( this.checkBoxYesNoGroup1.rbNo.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_BLANK;
            }

            this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = entitlement;
        }

        
        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );
            this.MyWizardButtons.AddNavigation( "< &Back", "MedicareEntitlementPage" );
            this.MyWizardButtons.AddNavigation( "&Next >", string.Empty );            
            this.MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            this.MyWizardButtons.SetPanel();
        }

        #endregion

        #region Properties
        public CheckBoxYesNoGroup  CheckBoxYesNoGroup1
        {
            get { return checkBoxYesNoGroup1; }
        }

        private bool DisplayInsurance
        {
            get
            {
                return i_DisplayInsurance;
            }
            set
            {
                i_DisplayInsurance = value;
            }
        }

        private bool IsShortRegAccount
        {
            get
            {
                return Model_Account.IsShortRegistered ||
                       AccountView.IsShortRegAccount;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// displayInsurance - display the patient's primary insurance info
        /// </summary>
        /// <param name="display"></param>
        private void displayInsurance( bool display )
        {
            if( display )
            {           
                this.pnlInsurance.Enabled           = true;
                Coverage primaryCoverage = null;
                if( ((MSP2Dialog)ParentForm) != null)
                {
                    primaryCoverage = ((MSP2Dialog) ParentForm).GetPrimaryCoverage();
                }
                if( primaryCoverage != null )
                {
                    if( primaryCoverage.Insured != null )
                    {
                        this.lblPrimaryInsuredText.Text     = primaryCoverage.Insured.FormattedName;
                    }
                    
                    if( primaryCoverage.InsurancePlan != null )
                    {
                        this.lblPrimaryPayorText.Text       = primaryCoverage.InsurancePlan.Payor.Name;
                    }                    
                }
                Coverage secondaryCoverage = null;
                if (((MSP2Dialog)ParentForm) != null)
                {
                    secondaryCoverage = ((MSP2Dialog) ParentForm).GetSecondaryCoverage();
                }
                if( secondaryCoverage != null )
                {
                    if( secondaryCoverage.Insured != null )
                    {
                        this.lblSecondaryInsuredText.Text     = secondaryCoverage.Insured.FormattedName;
                    }
                    
                    if( secondaryCoverage.InsurancePlan != null )
                    {
                        this.lblSecondaryPayorText.Text       = secondaryCoverage.InsurancePlan.Payor.Name;
                    }                    
                }

                displayGroupHealthPlanInfo( true );
                displayEmploymentInfo( true );
            }
            else
            {
                this.pnlInsurance.Enabled                   = false;
                this.DisplayInsurance                       = false;

                this.lblPrimaryInsuredText.Text             = string.Empty;
                this.lblPrimaryPayorText.Text               = string.Empty;

                this.lblSecondaryInsuredText.Text           = string.Empty;
                this.lblSecondaryPayorText.Text             = string.Empty;

                displayGroupHealthPlanInfo( false );
                displayEmploymentInfo( false );
            }                
        }

        /// <summary>
        /// Display GHP coverage address
        /// </summary>
        private void displayEmploymentInfo( bool display )
        {
            if( display )
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
            else
            {
                this.lblEmployerInfo.Text       = string.Empty;
            }
        }

        /// <summary>
        /// Populate the controls with the Group Plan Health information
        /// </summary>
        private void displayGroupHealthPlanInfo( bool display )
        {
            Coverage coverage = null;
            if (((MSP2Dialog)ParentForm) != null)
            {
                coverage = ((MSP2Dialog) ParentForm).GetPrimaryCoverage();
            }
            if( display == false )
            {
                this.lblRelationship.Text       = string.Empty;
                this.lblPolicyHolder.Text       = string.Empty;
                this.lblPolicyID.Text           = string.Empty;
                this.lblGroupID.Text            = string.Empty;
                this.lblInsuranceAddress.Text   = string.Empty;

                return;
            }
            else if( Model_Account.PrimaryInsured != null && coverage != null )
            {                
                RelationshipType relaType = null;

                if( Model_Account.PrimaryInsured.Relationships != null
                    && Model_Account.PrimaryInsured.Relationships.Count > 0 )
                {
                    foreach( Relationship r in Model_Account.PrimaryInsured.Relationships )
                    {
                        relaType = r.Type;
                        break;
                    }
                }
                

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

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpEmployer = new System.Windows.Forms.GroupBox();
            this.lblEmployerInfo = new System.Windows.Forms.Label();
            this.btnEditEmployment = new PatientAccess.UI.CommonControls.LoggingButton();
            this.grpHealthPlan = new System.Windows.Forms.GroupBox();
            this.lblInsuranceAddress = new System.Windows.Forms.Label();
            this.btnEditInsured = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnEditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.btnEditPayor = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblGroupID = new System.Windows.Forms.Label();
            this.lblPolicyID = new System.Windows.Forms.Label();
            this.lblStaticGroupID = new System.Windows.Forms.Label();
            this.lblStaticPolicyID = new System.Windows.Forms.Label();
            this.lblRelationship = new System.Windows.Forms.Label();
            this.lblStaticRelationship = new System.Windows.Forms.Label();
            this.lblPolicyHolder = new System.Windows.Forms.Label();
            this.lblStaticPolicy = new System.Windows.Forms.Label();
            this.pnlQuestion1 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup1 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuesion1b = new System.Windows.Forms.Label();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlInsurance = new System.Windows.Forms.Panel();
            this.btnInsEditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblSecondaryInsuredText = new System.Windows.Forms.Label();
            this.lblSecondaryPayorText = new System.Windows.Forms.Label();
            this.lblPrimaryInsuredText = new System.Windows.Forms.Label();
            this.lblPrimaryPayorText = new System.Windows.Forms.Label();
            this.lblSecondaryInsured = new System.Windows.Forms.Label();
            this.lblSecondaryPayor = new System.Windows.Forms.Label();
            this.lblPrimaryInsured = new System.Windows.Forms.Label();
            this.lblPrimaryPayor = new System.Windows.Forms.Label();
            this.pnlInsuranceDivider = new System.Windows.Forms.Panel();
            this.pnlWizardPageBody.SuspendLayout();
            this.grpEmployer.SuspendLayout();
            this.grpHealthPlan.SuspendLayout();
            this.pnlQuestion1.SuspendLayout();
            this.pnlInsurance.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.pnlInsurance);
            this.pnlWizardPageBody.Controls.Add(this.pnlDivider1);
            this.pnlWizardPageBody.Controls.Add(this.grpEmployer);
            this.pnlWizardPageBody.Controls.Add(this.grpHealthPlan);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion1);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.grpHealthPlan, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.grpEmployer, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlDivider1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlInsurance, 0);
            // 
            // grpEmployer
            // 
            this.grpEmployer.Controls.Add(this.lblEmployerInfo);
            this.grpEmployer.Controls.Add(this.btnEditEmployment);
            this.grpEmployer.Enabled = false;
            this.grpEmployer.Location = new System.Drawing.Point(21, 345);
            this.grpEmployer.Name = "grpEmployer";
            this.grpEmployer.Size = new System.Drawing.Size(504, 75);
            this.grpEmployer.TabIndex = 6;
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
            this.btnEditEmployment.Message = null;
            this.btnEditEmployment.Name = "btnEditEmployment";
            this.btnEditEmployment.Size = new System.Drawing.Size(180, 23);
            this.btnEditEmployment.TabIndex = 1;
            this.btnEditEmployment.Text = "Edit &Guarantor && Cancel MSP";
            this.btnEditEmployment.Click += new System.EventHandler(this.btnEditEmployment_Click);
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
            this.grpHealthPlan.Location = new System.Drawing.Point(20, 131);
            this.grpHealthPlan.Name = "grpHealthPlan";
            this.grpHealthPlan.Size = new System.Drawing.Size(504, 207);
            this.grpHealthPlan.TabIndex = 5;
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
            this.btnEditInsured.Message = null;
            this.btnEditInsured.Name = "btnEditInsured";
            this.btnEditInsured.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsured.TabIndex = 1;
            this.btnEditInsured.Text = "Edit &Insured && Cancel MSP";
            this.btnEditInsured.Click += new System.EventHandler(this.btnEditInsured_Click);
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point(310, 94);
            this.btnEditInsurance.Message = null;
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsurance.TabIndex = 2;
            this.btnEditInsurance.Text = "Edi&t Insurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler(this.btnEditInsurance_Click);
            // 
            // lblLine2
            // 
            this.lblLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine2.Location = new System.Drawing.Point(8, 145);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(488, 1);
            this.lblLine2.TabIndex = 0;
            this.lblLine2.Text = "label1";
            // 
            // lblLine1
            // 
            this.lblLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine1.Location = new System.Drawing.Point(8, 81);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(488, 1);
            this.lblLine1.TabIndex = 0;
            // 
            // btnEditPayor
            // 
            this.btnEditPayor.Location = new System.Drawing.Point(310, 160);
            this.btnEditPayor.Message = null;
            this.btnEditPayor.Name = "btnEditPayor";
            this.btnEditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnEditPayor.TabIndex = 3;
            this.btnEditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnEditPayor.Click += new System.EventHandler(this.btnEditPayor_Click);
            // 
            // lblGroupID
            // 
            this.lblGroupID.Location = new System.Drawing.Point(59, 182);
            this.lblGroupID.Name = "lblGroupID";
            this.lblGroupID.Size = new System.Drawing.Size(240, 16);
            this.lblGroupID.TabIndex = 0;
            // 
            // lblPolicyID
            // 
            this.lblPolicyID.Location = new System.Drawing.Point(59, 160);
            this.lblPolicyID.Name = "lblPolicyID";
            this.lblPolicyID.Size = new System.Drawing.Size(240, 16);
            this.lblPolicyID.TabIndex = 0;
            // 
            // lblStaticGroupID
            // 
            this.lblStaticGroupID.Location = new System.Drawing.Point(8, 182);
            this.lblStaticGroupID.Name = "lblStaticGroupID";
            this.lblStaticGroupID.Size = new System.Drawing.Size(54, 15);
            this.lblStaticGroupID.TabIndex = 0;
            this.lblStaticGroupID.Text = "Group ID:";
            // 
            // lblStaticPolicyID
            // 
            this.lblStaticPolicyID.Location = new System.Drawing.Point(8, 160);
            this.lblStaticPolicyID.Name = "lblStaticPolicyID";
            this.lblStaticPolicyID.Size = new System.Drawing.Size(56, 14);
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
            // pnlQuestion1
            // 
            this.pnlQuestion1.Controls.Add(this.checkBoxYesNoGroup1);
            this.pnlQuestion1.Controls.Add(this.lblQuesion1b);
            this.pnlQuestion1.Controls.Add(this.lblQuestion1);
            this.pnlQuestion1.Location = new System.Drawing.Point(8, 73);
            this.pnlQuestion1.Name = "pnlQuestion1";
            this.pnlQuestion1.Size = new System.Drawing.Size(666, 50);
            this.pnlQuestion1.TabIndex = 4;
            this.pnlQuestion1.TabStop = true;
            // 
            // checkBoxYesNoGroup1
            // 
            this.checkBoxYesNoGroup1.Location = new System.Drawing.Point(541, 0);
            this.checkBoxYesNoGroup1.Name = "checkBoxYesNoGroup1";
            this.checkBoxYesNoGroup1.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup1.TabIndex = 2;
            this.checkBoxYesNoGroup1.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup1_RadioChanged);
            // 
            // lblQuesion1b
            // 
            this.lblQuesion1b.Location = new System.Drawing.Point(22, 23);
            this.lblQuesion1b.Name = "lblQuesion1b";
            this.lblQuesion1b.Size = new System.Drawing.Size(432, 15);
            this.lblQuesion1b.TabIndex = 1;
            this.lblQuesion1b.Text = "your spouse\'s, or a family member\'s employment?";
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size(432, 15);
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1. Do you have group health plan (GHP) coverage based on your own, ";
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point(7, 56);
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size(668, 2);
            this.pnlDivider1.TabIndex = 7;
            // 
            // pnlInsurance
            // 
            this.pnlInsurance.Controls.Add(this.btnInsEditInsurance);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsured);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayor);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsured);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayor);
            this.pnlInsurance.Controls.Add(this.pnlInsuranceDivider);
            this.pnlInsurance.Location = new System.Drawing.Point(8, 433);
            this.pnlInsurance.Name = "pnlInsurance";
            this.pnlInsurance.Size = new System.Drawing.Size(667, 117);
            this.pnlInsurance.TabIndex = 8;
            // 
            // btnInsEditInsurance
            // 
            this.btnInsEditInsurance.Location = new System.Drawing.Point(465, 25);
            this.btnInsEditInsurance.Message = null;
            this.btnInsEditInsurance.Name = "btnInsEditInsurance";
            this.btnInsEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnInsEditInsurance.TabIndex = 13;
            this.btnInsEditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnInsEditInsurance.Click += new System.EventHandler(this.btnInsEditInsurance_Click);
            // 
            // lblSecondaryInsuredText
            // 
            this.lblSecondaryInsuredText.Location = new System.Drawing.Point(104, 92);
            this.lblSecondaryInsuredText.Name = "lblSecondaryInsuredText";
            this.lblSecondaryInsuredText.Size = new System.Drawing.Size(344, 13);
            this.lblSecondaryInsuredText.TabIndex = 12;
            // 
            // lblSecondaryPayorText
            // 
            this.lblSecondaryPayorText.Location = new System.Drawing.Point(104, 73);
            this.lblSecondaryPayorText.Name = "lblSecondaryPayorText";
            this.lblSecondaryPayorText.Size = new System.Drawing.Size(344, 13);
            this.lblSecondaryPayorText.TabIndex = 11;
            // 
            // lblPrimaryInsuredText
            // 
            this.lblPrimaryInsuredText.Location = new System.Drawing.Point(104, 44);
            this.lblPrimaryInsuredText.Name = "lblPrimaryInsuredText";
            this.lblPrimaryInsuredText.Size = new System.Drawing.Size(344, 13);
            this.lblPrimaryInsuredText.TabIndex = 10;
            // 
            // lblPrimaryPayorText
            // 
            this.lblPrimaryPayorText.Location = new System.Drawing.Point(104, 26);
            this.lblPrimaryPayorText.Name = "lblPrimaryPayorText";
            this.lblPrimaryPayorText.Size = new System.Drawing.Size(344, 13);
            this.lblPrimaryPayorText.TabIndex = 9;
            // 
            // lblSecondaryInsured
            // 
            this.lblSecondaryInsured.Location = new System.Drawing.Point(56, 92);
            this.lblSecondaryInsured.Name = "lblSecondaryInsured";
            this.lblSecondaryInsured.Size = new System.Drawing.Size(45, 13);
            this.lblSecondaryInsured.TabIndex = 8;
            this.lblSecondaryInsured.Text = "Insured:";
            // 
            // lblSecondaryPayor
            // 
            this.lblSecondaryPayor.Location = new System.Drawing.Point(10, 73);
            this.lblSecondaryPayor.Name = "lblSecondaryPayor";
            this.lblSecondaryPayor.Size = new System.Drawing.Size(95, 13);
            this.lblSecondaryPayor.TabIndex = 7;
            this.lblSecondaryPayor.Text = "Secondary Payor:";
            // 
            // lblPrimaryInsured
            // 
            this.lblPrimaryInsured.Location = new System.Drawing.Point(56, 44);
            this.lblPrimaryInsured.Name = "lblPrimaryInsured";
            this.lblPrimaryInsured.Size = new System.Drawing.Size(45, 13);
            this.lblPrimaryInsured.TabIndex = 6;
            this.lblPrimaryInsured.Text = "Insured:";
            // 
            // lblPrimaryPayor
            // 
            this.lblPrimaryPayor.Location = new System.Drawing.Point(26, 26);
            this.lblPrimaryPayor.Name = "lblPrimaryPayor";
            this.lblPrimaryPayor.Size = new System.Drawing.Size(81, 13);
            this.lblPrimaryPayor.TabIndex = 5;
            this.lblPrimaryPayor.Text = "Primary Payor:";
            // 
            // pnlInsuranceDivider
            // 
            this.pnlInsuranceDivider.BackColor = System.Drawing.Color.Black;
            this.pnlInsuranceDivider.Location = new System.Drawing.Point(5, 11);
            this.pnlInsuranceDivider.Name = "pnlInsuranceDivider";
            this.pnlInsuranceDivider.Size = new System.Drawing.Size(656, 2);
            this.pnlInsuranceDivider.TabIndex = 4;
            // 
            // ESRDEntitlementPage1
            // 
            this.Name = "ESRDEntitlementPage1";
            this.EnabledChanged += new System.EventHandler(this.ESRDEntitlementPage1_EnabledChanged);
            this.Load += new System.EventHandler(this.ESRDEntitlementPage1_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.grpEmployer.ResumeLayout(false);
            this.grpHealthPlan.ResumeLayout(false);
            this.pnlQuestion1.ResumeLayout(false);
            this.pnlInsurance.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ESRDEntitlementPage1()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public ESRDEntitlementPage1( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public ESRDEntitlementPage1( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public ESRDEntitlementPage1( string pageName, WizardContainer wizardContainer, Account anAccount )
            : base( pageName, wizardContainer, anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
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

        #region Data Elements
        
        private IContainer                                components = null;

        private CheckBoxYesNoGroup         checkBoxYesNoGroup1;

        private GroupBox                                   grpEmployer;
        private GroupBox                                   grpHealthPlan;

        private Label                                      lblEmployerInfo;
        private Label                                      lblInsuranceAddress;

        private LoggingButton                   btnEditEmployment;
        private LoggingButton                   btnEditInsured;
        private LoggingButton                   btnEditInsurance;
        private LoggingButton                   btnEditPayor;
        private LoggingButton                   btnInsEditInsurance;

        private Panel                                      pnlQuestion1;
        private Panel                                      pnlDivider1;
        private Panel                                      pnlInsurance;
        private Panel                                      pnlInsuranceDivider;

        private Label                                      lblLine2;
        private Label                                      lblLine1;        
        private Label                                      lblGroupID;
        private Label                                      lblPolicyID;
        private Label                                      lblStaticGroupID;
        private Label                                      lblStaticPolicyID;
        private Label                                      lblRelationship;
        private Label                                      lblStaticRelationship;
        private Label                                      lblPolicyHolder;
        private Label                                      lblStaticPolicy;        
        private Label                                      lblQuestion1;
        private Label                                      lblQuesion1b;
        private Label                                      lblSecondaryInsuredText;
        private Label                                      lblSecondaryPayorText;
        private Label                                      lblPrimaryInsuredText;
        private Label                                      lblPrimaryPayorText;
        private Label                                      lblSecondaryInsured;
        private Label                                      lblSecondaryPayor;
        private Label                                      lblPrimaryInsured;
        private Label                                      lblPrimaryPayor;
        
        private bool                                                            blnLoaded = false;
        private bool                                                            i_DisplayInsurance;
        private bool previousGHPSelection;

        #endregion

        #region Constants
        
        private const string                                            NOT_AVAILABLE = "Not available";

        #endregion
    }
}

