using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.InsuranceViews.MSP2
{   
    /// <summary>
    /// AgeEntitlementPage2 - the second Entitlement by Age page; captures wether or not the patient has Group
    /// Health Plan (GHP) coverage through their or their spouse's employer
    /// </summary>
    
    [Serializable]
    public class AgeEntitlementPage2 : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// checkBoxYesNoGroup4_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void checkBoxYesNoGroup4_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                this.pnlInsurance.Enabled   = true;
                this.displayInsurance( true );
            }      
            else
            {
                if( !this.checkBoxYesNoGroup5.rbYes.Checked )
                {
                    this.pnlInsurance.Enabled           = false;
                    this.displayInsurance( false );
                }
            }

            this.CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup5_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void checkBoxYesNoGroup5_RadioChanged(object sender, EventArgs e)
        {
            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                this.pnlInsurance.Enabled               = true;
                this.displayInsurance( true );
            }
            else
            {
                if( !this.checkBoxYesNoGroup4.rbYes.Checked )
                {
                    this.pnlInsurance.Enabled           = false;
                    this.displayInsurance( false );
                }
            }

            this.CanPageNavigate();
        }

        /// <summary>
        /// rbQ3No_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void rbQ3No_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlQuestion4.Enabled                   = false;
            this.checkBoxYesNoGroup4.rbNo.Checked       = false;
            this.checkBoxYesNoGroup4.rbYes.Checked      = false;
            this.pnlQuestion5.Enabled                   = false;
            this.checkBoxYesNoGroup5.rbNo.Checked       = false;
            this.checkBoxYesNoGroup5.rbYes.Checked      = false;

            this.CanPageNavigate();
        }

        /// <summary>
        /// rbQ3YesSelf_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void rbQ3YesSelf_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlQuestion4.Enabled                   = true;
            this.pnlQuestion5.Enabled                   = false;
            this.checkBoxYesNoGroup5.rbNo.Checked       = false;
            this.checkBoxYesNoGroup5.rbYes.Checked      = false;

            this.CanPageNavigate();
        }

        /// <summary>
        /// rbQ3YesSpouse_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void rbQ3YesSpouse_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlQuestion4.Enabled                   = false;
            this.checkBoxYesNoGroup4.rbNo.Checked       = false;
            this.checkBoxYesNoGroup4.rbYes.Checked      = false;
            this.pnlQuestion5.Enabled                   = true;

            this.CanPageNavigate();
        }
        
        /// <summary>
        /// rbQ3YesBoth_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void rbQ3YesBoth_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlQuestion4.Enabled                   = true;
            this.pnlQuestion5.Enabled                   = true;

            this.CanPageNavigate();
        }

        /// <summary>
        /// btnEditInsurance_Click - the user has opted to abandon the MSP wizard and edit the insurance
        /// info for the patient.
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
        /// AgeEntitlementPage2_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void AgeEntitlementPage2_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled )
            {
                this.UpdateView();
            }
        }

        /// <summary>
        /// AgeEntitlementPage2_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        
        private void AgeEntitlementPage2_Load(object sender, EventArgs e)
        {
            this.ShowLink                           = false;

            this.MyWizardMessages.Message1          = "Entitlement by Age";            
            this.MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1         = 8.25;
            this.MyWizardMessages.FontStyle1        = FontStyle.Bold;

            this.MyWizardMessages.Message2          = "";

            this.MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize2         = 8.25;

            this.MyWizardMessages.ShowMessages();

            this.pnlQuestion4.Enabled               = false;
            this.pnlQuestion5.Enabled               = false;
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

        #endregion

        #region Methods

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        public override void ResetPage()
        {
            base.ResetPage ();
        
            this.rbQ3No.Checked                     = false;
            this.rbQ3YesBoth.Checked                = false;
            this.rbQ3YesSelf.Checked                = false;
            this.rbQ3YesSpouse.Checked              = false;

            this.checkBoxYesNoGroup4.rbNo.Checked   = false;
            this.checkBoxYesNoGroup4.rbYes.Checked  = false;

            this.checkBoxYesNoGroup5.rbNo.Checked   = false;
            this.checkBoxYesNoGroup5.rbYes.Checked  = false;  
          
            this.pnlQuestion4.Enabled               = false;
            this.pnlQuestion5.Enabled               = false;
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
                this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                this.MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
            }

            this.HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {
            this.MyWizardButtons.UpdateNavigation( "&Next >", string.Empty );
            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            bool blnCanNav              = false;

            if( this.rbQ3No.Checked )
            {
                blnCanNav               = true;
            }

            if( !blnCanNav
                && this.rbQ3YesSelf.Checked
                && ( this.checkBoxYesNoGroup4.rbYes.Checked
                || this.checkBoxYesNoGroup4.rbNo.Checked )
                )
            {
                blnCanNav               = true;
            }

            if( !blnCanNav
                && this.rbQ3YesSpouse.Checked
                && ( this.checkBoxYesNoGroup5.rbYes.Checked
                || this.checkBoxYesNoGroup5.rbNo.Checked )
                )
            {
                blnCanNav               = true;
            }

            if( !blnCanNav
                && this.rbQ3YesBoth.Checked                
                && ( this.checkBoxYesNoGroup4.rbYes.Checked
                || this.checkBoxYesNoGroup4.rbNo.Checked )
                && ( this.checkBoxYesNoGroup5.rbYes.Checked
                || this.checkBoxYesNoGroup5.rbNo.Checked )
                )
            {
                blnCanNav               = true;
            }

            this.CanNavigate = blnCanNav; 

            this.CheckForSummary();
        
            return blnCanNav;
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
                    || (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ) == null )
                {
                    return;
                }

                if( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType()
                    == typeof( AgeEntitlement ) ) 
                {  
                    AgeEntitlement ageEntitlement = 
                     Model_Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement;

                    if( ageEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES )                        
                    {
                        if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SELF_OID )
                        {
                            this.rbQ3YesSelf.Checked                    = true;
                        }
                        else if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.SPOUSE_OID )
                        {
                            this.rbQ3YesSpouse.Checked                  = true;
                        }
                        else if( ageEntitlement.GroupHealthPlanType.Oid == GroupHealthPlanType.BOTH_OID )
                        {
                            this.rbQ3YesBoth.Checked                    = true;
                        }
                        
                    }
                    else if(ageEntitlement.GroupHealthPlanCoverage.Code == YesNoFlag.CODE_NO )
                    {
                        this.rbQ3No.Checked                         = true;
                    }
                    
                    if( ageEntitlement.GHPEmploysX.Code == YesNoFlag.CODE_YES )
                    {
                        this.checkBoxYesNoGroup4.rbYes.Checked      = true;
                    }
                    else if ( ageEntitlement.GHPEmploysX.Code == YesNoFlag.CODE_NO )
                    {
                        this.checkBoxYesNoGroup4.rbNo.Checked       = true;
                    }

                    if( ageEntitlement.GHPSpouseEmploysX.Code == YesNoFlag.CODE_YES )
                    {
                        this.checkBoxYesNoGroup5.rbYes.Checked      = true;
                    }
                    else if ( ageEntitlement.GHPSpouseEmploysX.Code == YesNoFlag.CODE_NO )
                    {
                        this.checkBoxYesNoGroup5.rbNo.Checked       = true;
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
            base.UpdateModel();

            AgeEntitlement entitlement = null;

            if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                entitlement = new AgeEntitlement();
            }
            else
            {
                if( this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType()
                    != typeof(AgeEntitlement) )
                {
                    entitlement = new AgeEntitlement();
                }
                else
                    entitlement = this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement;
            }

            if( this.rbQ3YesSelf.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_YES;
                entitlement.GroupHealthPlanType.Oid = GroupHealthPlanType.SELF_OID;
            }
            else if( this.rbQ3YesSpouse.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_YES;
                entitlement.GroupHealthPlanType.Oid = GroupHealthPlanType.SPOUSE_OID;
            }
            else if( this.rbQ3YesBoth.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_YES;
                entitlement.GroupHealthPlanType.Oid = GroupHealthPlanType.BOTH_OID;
            }
            else if( this.rbQ3No.Checked )
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.GroupHealthPlanCoverage.Code = YesNoFlag.CODE_BLANK;
            }

            if( this.checkBoxYesNoGroup4.rbYes.Checked )
            {
                entitlement.GHPEmploysX.Code    = YesNoFlag.CODE_YES;
            }
            else if( this.checkBoxYesNoGroup4.rbNo.Checked )
            {
                entitlement.GHPEmploysX.Code    = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.GHPEmploysX.Code    = YesNoFlag.CODE_BLANK;
            }

            if( this.checkBoxYesNoGroup5.rbYes.Checked )
            {
                entitlement.GHPSpouseEmploysX.Code    = YesNoFlag.CODE_YES;
            }
            else if( this.checkBoxYesNoGroup5.rbNo.Checked )
            {
                entitlement.GHPSpouseEmploysX.Code    = YesNoFlag.CODE_NO;
            }
            else
            {
                entitlement.GHPSpouseEmploysX.Code    = YesNoFlag.CODE_BLANK;
            }

            this.Model_Account.MedicareSecondaryPayor.MedicareEntitlement = entitlement;
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {            
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );
            this.MyWizardButtons.AddNavigation( "< &Back", GetAgeEntitlementPageName() );
            this.MyWizardButtons.AddNavigation( "&Next >", string.Empty );            
            this.MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            this.MyWizardButtons.SetPanel();
        }

        private bool IsShortRegAccount
        {
            get
            {
                return Model_Account.IsShortRegistered ||
                       AccountView.IsShortRegAccount;
            }
        }

        private string GetAgeEntitlementPageName()
        {
            if ( IsShortRegAccount )
            {
                return "ShortAgeEntitlementPage1";
            }

            return "AgeEntitlementPage1";
        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        /// <summary>
        /// displayInsurance - display (or remove the display of) primary insurance info
        /// </summary>
        /// <param name="display"></param>
        private void displayInsurance( bool display )
        {
            if( display )
            {           
                this.pnlInsurance.Enabled           = true;
                this.btnEditInsurance.Enabled       = true;

                Coverage primaryCoverage = ((MSP2Dialog)ParentForm).GetPrimaryCoverage();

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

                Coverage secondaryCoverage = ((MSP2Dialog)ParentForm).GetSecondaryCoverage();

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
            }
            else
            {
                this.pnlInsurance.Enabled                   = false;
                this.btnEditInsurance.Enabled               = false;

                this.lblPrimaryInsuredText.Text             = string.Empty;
                this.lblPrimaryPayorText.Text               = string.Empty;

                this.lblSecondaryInsuredText.Text           = string.Empty;
                this.lblSecondaryPayorText.Text             = string.Empty;
            }                
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQuestion3 = new System.Windows.Forms.Panel();
            this.lblQuestion3b = new System.Windows.Forms.Label();
            this.pnlQ3RadioButtons = new System.Windows.Forms.Panel();
            this.rbQ3YesBoth = new System.Windows.Forms.RadioButton();
            this.rbQ3YesSpouse = new System.Windows.Forms.RadioButton();
            this.rbQ3No = new System.Windows.Forms.RadioButton();
            this.rbQ3YesSelf = new System.Windows.Forms.RadioButton();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.pnlDividerQ3 = new System.Windows.Forms.Panel();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlQuestion4 = new System.Windows.Forms.Panel();
            this.pnlDividerQ4 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup4 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion4b = new System.Windows.Forms.Label();
            this.lblQuestion4 = new System.Windows.Forms.Label();
            this.pnlQuestion5 = new System.Windows.Forms.Panel();
            this.lblQuestion5c = new System.Windows.Forms.Label();
            this.pnlQ5Divider = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup5 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion5b = new System.Windows.Forms.Label();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.pnlInsurance = new System.Windows.Forms.Panel();
            this.btnEditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
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
            this.pnlQuestion3.SuspendLayout();
            this.pnlQ3RadioButtons.SuspendLayout();
            this.pnlQuestion4.SuspendLayout();
            this.pnlQuestion5.SuspendLayout();
            this.pnlInsurance.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.pnlInsurance);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion5);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion4);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion3);
            this.pnlWizardPageBody.Controls.Add(this.pnlDivider1);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlDivider1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion3, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion4, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion5, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlInsurance, 0);
            // 
            // pnlQuestion3
            // 
            this.pnlQuestion3.Controls.Add(this.lblQuestion3b);
            this.pnlQuestion3.Controls.Add(this.pnlQ3RadioButtons);
            this.pnlQuestion3.Controls.Add(this.lblQuestion3);
            this.pnlQuestion3.Controls.Add(this.pnlDividerQ3);
            this.pnlQuestion3.Location = new System.Drawing.Point(6, 71);
            this.pnlQuestion3.Name = "pnlQuestion3";
            this.pnlQuestion3.Size = new System.Drawing.Size(667, 75);
            this.pnlQuestion3.TabIndex = 3;
            // 
            // lblQuestion3b
            // 
            this.lblQuestion3b.Location = new System.Drawing.Point(24, 24);
            this.lblQuestion3b.Name = "lblQuestion3b";
            this.lblQuestion3b.Size = new System.Drawing.Size(182, 16);
            this.lblQuestion3b.TabIndex = 2;
            this.lblQuestion3b.Text = "or a spouse\'s current employment?";
            // 
            // pnlQ3RadioButtons
            // 
            this.pnlQ3RadioButtons.Controls.Add(this.rbQ3YesBoth);
            this.pnlQ3RadioButtons.Controls.Add(this.rbQ3YesSpouse);
            this.pnlQ3RadioButtons.Controls.Add(this.rbQ3No);
            this.pnlQ3RadioButtons.Controls.Add(this.rbQ3YesSelf);
            this.pnlQ3RadioButtons.Location = new System.Drawing.Point(485, -7);
            this.pnlQ3RadioButtons.Name = "pnlQ3RadioButtons";
            this.pnlQ3RadioButtons.Size = new System.Drawing.Size(174, 75);
            this.pnlQ3RadioButtons.TabIndex = 1;
            this.pnlQ3RadioButtons.TabStop = true;
            // 
            // rbQ3YesBoth
            // 
            this.rbQ3YesBoth.Location = new System.Drawing.Point(9, 49);
            this.rbQ3YesBoth.Name = "rbQ3YesBoth";
            this.rbQ3YesBoth.Size = new System.Drawing.Size(94, 24);
            this.rbQ3YesBoth.TabIndex = 3;
            this.rbQ3YesBoth.Text = "Yes - Both";
            this.rbQ3YesBoth.CheckedChanged += new System.EventHandler(this.rbQ3YesBoth_CheckedChanged);
            // 
            // rbQ3YesSpouse
            // 
            this.rbQ3YesSpouse.Location = new System.Drawing.Point(9, 29);
            this.rbQ3YesSpouse.Name = "rbQ3YesSpouse";
            this.rbQ3YesSpouse.Size = new System.Drawing.Size(99, 24);
            this.rbQ3YesSpouse.TabIndex = 2;
            this.rbQ3YesSpouse.Text = "Yes - Spouse";
            this.rbQ3YesSpouse.CheckedChanged += new System.EventHandler(this.rbQ3YesSpouse_CheckedChanged);
            // 
            // rbQ3No
            // 
            this.rbQ3No.Location = new System.Drawing.Point(121, 7);
            this.rbQ3No.Name = "rbQ3No";
            this.rbQ3No.Size = new System.Drawing.Size(48, 24);
            this.rbQ3No.TabIndex = 4;
            this.rbQ3No.TabStop = true;
            this.rbQ3No.Text = "No";
            this.rbQ3No.CheckedChanged += new System.EventHandler(this.rbQ3No_CheckedChanged);
            // 
            // rbQ3YesSelf
            // 
            this.rbQ3YesSelf.Location = new System.Drawing.Point(9, 9);
            this.rbQ3YesSelf.Name = "rbQ3YesSelf";
            this.rbQ3YesSelf.Size = new System.Drawing.Size(91, 24);
            this.rbQ3YesSelf.TabIndex = 1;
            this.rbQ3YesSelf.TabStop = true;
            this.rbQ3YesSelf.Text = "Yes - Self";
            this.rbQ3YesSelf.CheckedChanged += new System.EventHandler(this.rbQ3YesSelf_CheckedChanged);
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(365, 16);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3.  Do you have group health plan (GHP) coverage based on your own";
            // 
            // pnlDividerQ3
            // 
            this.pnlDividerQ3.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ3.Location = new System.Drawing.Point(4, 71);
            this.pnlDividerQ3.Name = "pnlDividerQ3";
            this.pnlDividerQ3.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ3.TabIndex = 0;
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point(12, 55);
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size(656, 2);
            this.pnlDivider1.TabIndex = 2;
            // 
            // pnlQuestion4
            // 
            this.pnlQuestion4.Controls.Add(this.pnlDividerQ4);
            this.pnlQuestion4.Controls.Add(this.checkBoxYesNoGroup4);
            this.pnlQuestion4.Controls.Add(this.lblQuestion4b);
            this.pnlQuestion4.Controls.Add(this.lblQuestion4);
            this.pnlQuestion4.Location = new System.Drawing.Point(1, 153);
            this.pnlQuestion4.Name = "pnlQuestion4";
            this.pnlQuestion4.Size = new System.Drawing.Size(674, 73);
            this.pnlQuestion4.TabIndex = 4;
            // 
            // pnlDividerQ4
            // 
            this.pnlDividerQ4.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ4.Location = new System.Drawing.Point(9, 65);
            this.pnlDividerQ4.Name = "pnlDividerQ4";
            this.pnlDividerQ4.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ4.TabIndex = 3;
            // 
            // checkBoxYesNoGroup4
            // 
            this.checkBoxYesNoGroup4.Location = new System.Drawing.Point(542, -1);
            this.checkBoxYesNoGroup4.Name = "checkBoxYesNoGroup4";
            this.checkBoxYesNoGroup4.Size = new System.Drawing.Size(131, 35);
            this.checkBoxYesNoGroup4.TabIndex = 2;
            this.checkBoxYesNoGroup4.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup4_RadioChanged);
            // 
            // lblQuestion4b
            // 
            this.lblQuestion4b.Location = new System.Drawing.Point(23, 24);
            this.lblQuestion4b.Name = "lblQuestion4b";
            this.lblQuestion4b.Size = new System.Drawing.Size(421, 13);
            this.lblQuestion4b.TabIndex = 1;
            this.lblQuestion4b.Text = "employer, that sponsors or contributes to the GHP, employ 20 or more employees?";
            // 
            // lblQuestion4
            // 
            this.lblQuestion4.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion4.Name = "lblQuestion4";
            this.lblQuestion4.Size = new System.Drawing.Size(421, 13);
            this.lblQuestion4.TabIndex = 0;
            this.lblQuestion4.Text = "4.  If you have GHP coverage based on your own current employment, does your";
            // 
            // pnlQuestion5
            // 
            this.pnlQuestion5.Controls.Add(this.lblQuestion5c);
            this.pnlQuestion5.Controls.Add(this.pnlQ5Divider);
            this.pnlQuestion5.Controls.Add(this.checkBoxYesNoGroup5);
            this.pnlQuestion5.Controls.Add(this.lblQuestion5b);
            this.pnlQuestion5.Controls.Add(this.lblQuestion5);
            this.pnlQuestion5.Location = new System.Drawing.Point(4, 233);
            this.pnlQuestion5.Name = "pnlQuestion5";
            this.pnlQuestion5.Size = new System.Drawing.Size(674, 73);
            this.pnlQuestion5.TabIndex = 5;
            // 
            // lblQuestion5c
            // 
            this.lblQuestion5c.Location = new System.Drawing.Point(24, 39);
            this.lblQuestion5c.Name = "lblQuestion5c";
            this.lblQuestion5c.Size = new System.Drawing.Size(421, 13);
            this.lblQuestion5c.TabIndex = 4;
            this.lblQuestion5c.Text = "employ 20 or more employees?";
            // 
            // pnlQ5Divider
            // 
            this.pnlQ5Divider.BackColor = System.Drawing.Color.Black;
            this.pnlQ5Divider.Location = new System.Drawing.Point(9, 65);
            this.pnlQ5Divider.Name = "pnlQ5Divider";
            this.pnlQ5Divider.Size = new System.Drawing.Size(656, 1);
            this.pnlQ5Divider.TabIndex = 3;
            // 
            // checkBoxYesNoGroup5
            // 
            this.checkBoxYesNoGroup5.Location = new System.Drawing.Point(542, 0);
            this.checkBoxYesNoGroup5.Name = "checkBoxYesNoGroup5";
            this.checkBoxYesNoGroup5.Size = new System.Drawing.Size(131, 35);
            this.checkBoxYesNoGroup5.TabIndex = 2;
            this.checkBoxYesNoGroup5.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup5_RadioChanged);
            // 
            // lblQuestion5b
            // 
            this.lblQuestion5b.Location = new System.Drawing.Point(23, 24);
            this.lblQuestion5b.Name = "lblQuestion5b";
            this.lblQuestion5b.Size = new System.Drawing.Size(421, 13);
            this.lblQuestion5b.TabIndex = 1;
            this.lblQuestion5b.Text = "does your spouse\'s employer, that sponsors or contributes to the GHP, ";
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(8, 8);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(421, 13);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5.  If you have GHP coverage based on your spouse\'s current employment, ";
            // 
            // pnlInsurance
            // 
            this.pnlInsurance.Controls.Add(this.btnEditInsurance);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsuredText);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayorText);
            this.pnlInsurance.Controls.Add(this.lblSecondaryInsured);
            this.pnlInsurance.Controls.Add(this.lblSecondaryPayor);
            this.pnlInsurance.Controls.Add(this.lblPrimaryInsured);
            this.pnlInsurance.Controls.Add(this.lblPrimaryPayor);
            this.pnlInsurance.Controls.Add(this.pnlInsuranceDivider);
            this.pnlInsurance.Location = new System.Drawing.Point(7, 431);
            this.pnlInsurance.Name = "pnlInsurance";
            this.pnlInsurance.Size = new System.Drawing.Size(667, 117);
            this.pnlInsurance.TabIndex = 6;
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point(465, 25);
            this.btnEditInsurance.Message = null;
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsurance.TabIndex = 13;
            this.btnEditInsurance.Enabled = false;
            this.btnEditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler(this.btnEditInsurance_Click);
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
            // AgeEntitlementPage2
            // 
            this.Name = "AgeEntitlementPage2";
            this.EnabledChanged += new System.EventHandler(this.AgeEntitlementPage2_EnabledChanged);
            this.Load += new System.EventHandler(this.AgeEntitlementPage2_Load);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.pnlQuestion3.ResumeLayout(false);
            this.pnlQ3RadioButtons.ResumeLayout(false);
            this.pnlQuestion4.ResumeLayout(false);
            this.pnlQuestion5.ResumeLayout(false);
            this.pnlInsurance.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public AgeEntitlementPage2()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call
        }

        public AgeEntitlementPage2( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public AgeEntitlementPage2( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public AgeEntitlementPage2( string pageName, WizardContainer wizardContainer, Account anAccount )
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
        
        private IContainer                            components = null;
        
        private Panel                                  pnlQ3RadioButtons;
        private Panel                                  pnlDivider1;
        private Panel                                  pnlQuestion3;
        private Panel                                  pnlDividerQ3;
        private Panel                                  pnlQuestion4;
        private Panel                                  pnlDividerQ4;
        private Panel                                  pnlQuestion5;
        private Panel                                  pnlQ5Divider;
        private Panel                                  pnlInsurance;
        private Panel                                  pnlInsuranceDivider;
        
        private RadioButton                            rbQ3YesSelf;
        private RadioButton                            rbQ3YesSpouse;        
        private RadioButton                            rbQ3YesBoth;
        private RadioButton                            rbQ3No;

        private LoggingButton               btnEditInsurance;
        
        private CheckBoxYesNoGroup     checkBoxYesNoGroup4;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup5;

        private Label                                  lblQuestion3b;
        private Label                                  lblQuestion3;
        private Label                                  lblQuestion4;
        private Label                                  lblQuestion4b;
        private Label                                  lblQuestion5b;
        private Label                                  lblQuestion5;
        private Label                                  lblQuestion5c;
        private Label                                  lblPrimaryInsured;
        private Label                                  lblSecondaryInsured;
        private Label                                  lblSecondaryPayor;
        private Label                                  lblPrimaryPayor;
        private Label                                  lblPrimaryPayorText;
        private Label                                  lblPrimaryInsuredText;
        private Label                                  lblSecondaryPayorText;
        private Label                                  lblSecondaryInsuredText; 

        private bool                                                        blnLoaded = false;

        #endregion

        #region Constants
        #endregion
    }
}

