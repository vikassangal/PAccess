using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// SpecialProgramsPage - determines if the patient is covered by a special program (someone other than Medicare)
    /// e.g. Black Lung, Veteran's admin, Worker's Comp
    /// </summary>
    
    [Serializable]
	public class SpecialProgramsPage : WizardPage
	{
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// mtbQ1DateBegan_Leave - check the format of the date entered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ1DateBegan_Leave(object sender, EventArgs e)
        {
            this.runRules();
        }

        private void runRules()
        {
            MaskedEditTextBox mtb = this.mtbQ1DateBegan;
          
            UIColors.SetNormalBgColor( mtb );
            Refresh();

            if(  mtb.UnMaskedText != string.Empty
                && mtb.Text.Length != 0
                && mtb.Text.Length != 10 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );

                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                return;
            }
            else if( ServicesBeganDateRequired() )
            {
                UIColors.SetRequiredBgColor(mtbQ1DateBegan);
            }
            else
            {
                if(  mtb.UnMaskedText != string.Empty
                    && mtb.Text.Length != 0 )
                {
                
                    try
                    {   // Check the date entered is not in the future
                        DateTime benefitsDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                            Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                            Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                        if( !DateValidator.IsValidDate( benefitsDate ) )                         
                        {
                            mtb.Focus();
                            UIColors.SetErrorBgColor( mtb );
                            MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1 );
                            return;
                        }
                    }
                    catch
                    {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                        // an invalid year, month, or day.  Simply set field to error color.
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        return;
                    }
                }
            }

            this.CanPageNavigate();
        }
        private bool ServicesBeganDateRequired()
        {

            return (mtbQ1DateBegan.Enabled
                    && checkBoxYesNoGroup1.rbYes.Checked
                    && mtbQ1DateBegan.UnMaskedText.Trim().Length == 0);

        }

        /// <summary>
        /// SpecialProgramsPage_GotFocus - default focus so that a radio can be defaulted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpecialProgramsPage_GotFocus(object sender, EventArgs e)
        {
            this.checkBoxYesNoGroup1.Focus();
        }

        /// <summary>
        /// SpecialProgramsPage_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpecialProgramsPage_Load(object sender, EventArgs e)
        {
            this.LinkName                           = "Special Programs";
            this.MyWizardMessages.Message1          = "Special Programs";            
            this.MyWizardMessages.TextFont1         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize1         = 8.25;
            this.MyWizardMessages.FontStyle1        = FontStyle.Bold;

            this.MyWizardMessages.Message2          = "If the patient has Black Lung benefits, obtain the date services began even if the reason for today's " +
                "visit is not due to Black Lung." ;

            this.MyWizardMessages.TextFont2         = "Microsoft Sans Serif";
            this.MyWizardMessages.TextSize2         = 8.25;

            this.MyWizardMessages.ShowMessages();

            this.pnlQuestion1b.Enabled              = false;
            this.pnlQuestion2.Enabled               = false;
            this.gbQuestion5.Enabled                = false;         
        }

        /// <summary>
        /// SpecialProgramsPage_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpecialProgramsPage_EnabledChanged(object sender, EventArgs e)
        {
            if( this.Enabled )
            {
                this.UpdateView();
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
            this.runRules();

            if( ((RadioButton)sender).Name == "rbYes"
                && ((RadioButton)sender).Checked )
            {
                this.pnlQuestion1b.Enabled              = true;
                this.pnlQuestion2.Enabled               = true;

                UIColors.SetRequiredBgColor( this.mtbQ1DateBegan );

                if( !this.checkBoxYesNoGroup2.rbNo.Checked
                    && !this.checkBoxYesNoGroup2.rbYes.Checked )
                {
                    this.MyWizardButtons.DisableNavigation( "&Next >" );
                }
            }
            else
            {
                this.pnlQuestion1b.Enabled              = false;
                this.pnlQuestion2.Enabled               = false;

                this.mtbQ1DateBegan.UnMaskedText        = string.Empty;
            }
        }

        /// <summary>
        /// checkBoxYesNoGroup2_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup2_RadioChanged(object sender, EventArgs e)
        {
            this.CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup3_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup3_RadioChanged(object sender, EventArgs e)
        {
            this.CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup4_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup4_RadioChanged(object sender, EventArgs e)
        {
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
                this.gbQuestion5.Enabled            = true;
                this.displayWorkersCompInfo( true );                 
            }
            else
            {
                this.gbQuestion5.Enabled            = false;
                this.displayWorkersCompInfo( false );
            }



            this.CanPageNavigate();
        }

        /// <summary>
        /// btnQ5EditGuarantor_Click - the user has opted to abandon the MSP wizard and edit the Guarantor
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ5EditGuarantor_Click(object sender, EventArgs e)
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
        /// btnQ5EditPayor_Click - the user has opted to abandon the MSP wizard and edit the payor
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ5EditPayor_Click(object sender, EventArgs e)
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
        /// btnQ5EditDiagnosis_Click - the user has opted to abandon the MSP wizard and edit the diagnosis
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ5EditDiagnosis_Click(object sender, EventArgs e)
        {
            if ( ParentForm == null ) return;

            if ( IsShortRegAccount )
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )ShortAccountView.ShortRegistrationScreenIndexes.SHORTDIAGNOSIS );
            }
            else
            {
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.DIAGNOSIS );
            }
        }

        /// <summary>
        /// btnQ5EditInsurance_Click - the user has opted to abandon the MSP wizard and edit the insurance
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ5EditInsurance_Click(object sender, EventArgs e)
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

        #endregion

        #region Methods

        public override void ResetPage()
        {
            base.ResetPage ();

            this.checkBoxYesNoGroup1.rbYes.Checked      = false;
            this.checkBoxYesNoGroup1.rbNo.Checked       = false;

            this.checkBoxYesNoGroup2.rbYes.Checked      = false;
            this.checkBoxYesNoGroup2.rbNo.Checked       = false;

            this.checkBoxYesNoGroup3.rbYes.Checked      = false;
            this.checkBoxYesNoGroup3.rbNo.Checked       = false;

            this.checkBoxYesNoGroup4.rbYes.Checked      = false;
            this.checkBoxYesNoGroup4.rbNo.Checked       = false;

            this.checkBoxYesNoGroup5.rbYes.Checked      = false;
            this.checkBoxYesNoGroup5.rbNo.Checked       = false;

            this.mtbQ1DateBegan.UnMaskedText            = string.Empty;
            
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {
            bool canNav = false;

            this.MyWizardButtons.UpdateNavigation( "&Next >", string.Empty );
            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            // ensure that all checkboxes have been designated

            if( (   ( this.mtbQ1DateBegan.Enabled
                && this.checkBoxYesNoGroup1.rbYes.Checked
                && this.mtbQ1DateBegan.UnMaskedText.Trim().Length == 8 )                        
                || this.checkBoxYesNoGroup1.rbNo.Checked)
                &&
                ( this.checkBoxYesNoGroup2.Enabled == false
                || this.checkBoxYesNoGroup2.rbYes.Checked
                || this.checkBoxYesNoGroup2.rbNo.Checked)
                &&
                (this.checkBoxYesNoGroup3.rbYes.Checked
                || this.checkBoxYesNoGroup3.rbNo.Checked)
                &&
                (this.checkBoxYesNoGroup4.rbYes.Checked
                || this.checkBoxYesNoGroup4.rbNo.Checked)
                &&
                (this.checkBoxYesNoGroup5.rbYes.Checked
                || this.checkBoxYesNoGroup5.rbNo.Checked)
                )
            {
                canNav = true;

                if( this.checkBoxYesNoGroup5.rbYes.Checked )
                {
                    // Clear out any existing LiabilityInsurer

                    this.Model_Account.MedicareSecondaryPayor.LiabilityInsurer = new LiabilityInsurer();

                    LiabilityInsurerPage liPage = this.MyWizardContainer.GetWizardPage("LiabilityInsurerPage")
                        as LiabilityInsurerPage;
                    if( liPage != null )
                    {
                        liPage.ResetPage();
                        liPage.Enabled = false;
                    }

                    WizardPage aPage = this.MyWizardContainer.GetWizardPage("MedicareEntitlementPage");
                    if( aPage != null )
                    {
                        aPage.MyWizardButtons.UpdateNavigation( "< &Back", "SpecialProgramsPage" );
                    }

                    this.MyWizardButtons.UpdateNavigation( "&Next >", "MedicareEntitlementPage" );
                    this.MyWizardButtons.SetAcceptButton( "&Next >" );
                }
                else
                {
                    WizardPage aPage = this.MyWizardContainer.GetWizardPage("MedicareEntitlementPage");
                    if( aPage != null )
                    {
                        aPage.MyWizardButtons.UpdateNavigation( "< &Back", "LiabilityInsurerPage" );
                    }

                    this.MyWizardButtons.UpdateNavigation("&Next >", "LiabilityInsurerPage");
                    this.MyWizardButtons.SetAcceptButton( "&Next >" );
                    
                }
            }                        

            this.CanNavigate = canNav;

            this.CheckForSummary();

            return canNav;
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        private bool CheckForSummary()
        {
            bool rc = false;

            this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if( this.CanNavigate )
            {
                MedicareEntitlementPage aPage = this.MyWizardContainer.GetWizardPage("MedicareEntitlementPage")
                    as MedicareEntitlementPage;

                if( aPage != null )
                {
                    if( aPage.EntitlementCanNavigate() )
                    {
                        rc = true;
                        this.MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                        this.MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
                    }
                }
            }

            this.HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView ();           

            if( this.Model_Account == null )
            {
                return;
            }

            if( !blnLoaded )
            {
                blnLoaded       = true;
            
                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.Code == YesNoFlag.CODE_YES )
                {
                    this.checkBoxYesNoGroup1.rbYes.Checked      = true;

                    if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate != DateTime.MinValue )
                    {
                        DateTime date = Model_Account.MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate;
                        this.mtbQ1DateBegan.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                    }

                    if( Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code == YesNoFlag.CODE_YES )
                    {
                        this.checkBoxYesNoGroup2.rbYes.Checked  = true;
                    }
                    else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code == YesNoFlag.CODE_NO )
                    {
                        this.checkBoxYesNoGroup2.rbNo.Checked   = true;
                    }
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.Code == YesNoFlag.CODE_NO )
                {
                    this.checkBoxYesNoGroup1.rbNo.Checked       = true;
                }

                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.Code == YesNoFlag.CODE_YES )
                {
                    this.checkBoxYesNoGroup3.rbYes.Checked      = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.Code == YesNoFlag.CODE_NO )
                {
                    this.checkBoxYesNoGroup3.rbNo.Checked       = true;
                }

                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.Code == YesNoFlag.CODE_YES )
                {
                    this.checkBoxYesNoGroup4.rbYes.Checked      = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.Code == YesNoFlag.CODE_NO )
                {
                    this.checkBoxYesNoGroup4.rbNo.Checked       = true;
                }

                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated.Code == YesNoFlag.CODE_YES )
                {
                    this.checkBoxYesNoGroup5.rbYes.Checked      = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated.Code == YesNoFlag.CODE_NO )
                {
                    this.checkBoxYesNoGroup5.rbNo.Checked       = true;
                }
            }

            this.CanPageNavigate();       
 
            this.runRules();
   
            this.Refresh();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel();

            SpecialProgram sp = new SpecialProgram();

            if( this.checkBoxYesNoGroup1.rbYes.Checked )
            {
                sp.BlackLungBenefits.Code = YesNoFlag.CODE_YES;
                sp.BLBenefitsStartDate = DateTime.Parse(this.mtbQ1DateBegan.Text);
                
                if( this.checkBoxYesNoGroup2.rbYes.Checked )
                {
                    sp.VisitForBlackLung.Code = YesNoFlag.CODE_YES;
                }
                else if( this.checkBoxYesNoGroup2.rbNo.Checked )
                {
                    sp.VisitForBlackLung.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    sp.VisitForBlackLung.Code = YesNoFlag.CODE_BLANK;
                }
            }
            else if( this.checkBoxYesNoGroup1.rbNo.Checked )
            {
                sp.BlackLungBenefits.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                sp.BlackLungBenefits.Code = YesNoFlag.CODE_BLANK;
            }

            if( this.checkBoxYesNoGroup3.rbYes.Checked )
            {
                sp.GovernmentProgram.Code = YesNoFlag.CODE_YES;
            }
            else if( this.checkBoxYesNoGroup3.rbNo.Checked )
            {
                sp.GovernmentProgram.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                sp.GovernmentProgram.Code = YesNoFlag.CODE_BLANK;
            }

            if( this.checkBoxYesNoGroup4.rbYes.Checked )
            {
                sp.DVAAuthorized.Code = YesNoFlag.CODE_YES;
            }
            else if( this.checkBoxYesNoGroup4.rbNo.Checked )
            {
                sp.DVAAuthorized.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                sp.DVAAuthorized.Code = YesNoFlag.CODE_BLANK;
            }

            if( this.checkBoxYesNoGroup5.rbYes.Checked )
            {
                sp.WorkRelated.Code = YesNoFlag.CODE_YES;                
            }
            else if( this.checkBoxYesNoGroup5.rbNo.Checked )
            {
                sp.WorkRelated.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                sp.WorkRelated.Code = YesNoFlag.CODE_BLANK;
            }

            this.Model_Account.MedicareSecondaryPayor.SpecialProgram = sp;
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {                        
            this.MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( this.Cancel ) );
            this.MyWizardButtons.AddNavigation( "< &Back", "WelcomePage" );
            this.MyWizardButtons.AddNavigation( "&Next >", string.Empty );            
            this.MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );
            
            this.MyWizardButtons.SetPanel();
        }

        /// <summary>
        /// Cancel - handle the Cancel button click
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

        #region Properties

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
        /// displayWorkersCompInfo - Populate the controls with the patient's WC info
        /// </summary>
        
        private void displayWorkersCompInfo( bool state )
        {
            if( state )
            {
                if( Model_Account.Diagnosis != null )
                {
                    if( Model_Account.Diagnosis.Condition.GetType() == typeof( Illness ) )
                    {
                       // do nothing...
                    }
                    else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Crime ) )
                    {
                        DateTime date = (Model_Account.Diagnosis.Condition as Crime).OccurredOn;
                        this.lblQ5DateText.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );
                    }
                    else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Accident ) )
                    {
                        DateTime date = (Model_Account.Diagnosis.Condition as Accident).OccurredOn;
                        this.lblQ5DateText.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );
                    }
                }
                if( Model_Account.Guarantor.Employment != null )
                {
                    string guarantorEmp 
                        = Model_Account.Guarantor.Employment.Employer.PartyContactPoint.Address.AsMailingLabel();

                    if( guarantorEmp != String.Empty )
                    {
                        this.lblQ5GuarantorText.Text = String.Format("{0}{1}{2}", 
                            Model_Account.Guarantor.Employment.Employer.Name, 
                            Environment.NewLine, guarantorEmp );
                    }
                    else
                    {
                        this.lblQ5GuarantorText.Text = "Not available";
                    }
                }
                else
                {
                    this.lblQ5GuarantorText.Text = "Not employed";
                }

                Coverage coverage = ((MSP2Dialog)ParentForm).GetPrimaryCoverage();

                if( coverage != null )
                {
                    if( coverage.GetType().Equals( typeof( WorkersCompensationCoverage ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as WorkersCompensationCoverage).PolicyNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicaidCoverage ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as GovernmentMedicaidCoverage).PolicyCINNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicareCoverage ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as GovernmentMedicareCoverage).MBINumber;
                    }
                    else if( coverage.GetType().Equals( typeof( CommercialCoverage ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as CommercialCoverage).CertSSNID;
                    }
                    else if( coverage.GetType().Equals( typeof( CoverageForCommercialOther ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as CoverageForCommercialOther).CertSSNID;
                    }
                    else if( coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                    {
                        this.lblQ5PolicyText.Text = (coverage as OtherCoverage).CertSSNID;
                    }
                    else
                    {
                        this.lblQ5PolicyText.Text = "Not available";
                    }

                    if( coverage.BillingInformation.Address != null )
                    {
                        string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                        if( mailingLabel != String.Empty )
                        {
                            this.lblQ5InsuranceText.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                                Environment.NewLine, mailingLabel );
                        }
                        else
                        {
                            this.lblQ5InsuranceText.Text = coverage.InsurancePlan.PlanName;
                        }
                    }
                    else
                    {
                        this.lblQ5InsuranceText.Text = coverage.InsurancePlan.PlanName;
                    }
                    //                    lblPolicyID.Text  = coverage.InsurancePlan.PlanID;
                }
                else
                {
                    this.lblQ5InsuranceText.Text    = "Not available";
                    this.lblQ5PolicyText.Text       = "Not available";
                }
            }
            else
            {
                // clear the fields

                this.lblQ5DateText.Text                 = string.Empty;
                this.lblQ5GuarantorText.Text            = string.Empty;
                this.lblQ5InsuranceText.Text            = string.Empty;
                this.lblQ5PolicyText.Text               = string.Empty;
            }
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {            
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlQuestion1 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup1 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.pnlDividerQ1 = new System.Windows.Forms.Panel();
            this.pnlQuestion1b = new System.Windows.Forms.Panel();
            this.mtbQ1DateBegan = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ1DateBegan = new System.Windows.Forms.Label();
            this.pnlQuestion2 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup2 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.pnlDividerQ2 = new System.Windows.Forms.Panel();
            this.pnlQuestion3 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup3 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.pnlDividerQ3 = new System.Windows.Forms.Panel();
            this.pnlQuestion4 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup4 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion4a = new System.Windows.Forms.Label();
            this.pnlDividerQ4 = new System.Windows.Forms.Panel();
            this.lblQuestion4b = new System.Windows.Forms.Label();
            this.pnlQuestion5 = new System.Windows.Forms.Panel();
            this.pnlQ5Divider3 = new System.Windows.Forms.Panel();
            this.gbQuestion5 = new System.Windows.Forms.GroupBox();
            this.pnlQ5Divider1 = new System.Windows.Forms.Panel();
            this.pnlQ5Divider2 = new System.Windows.Forms.Panel();
            this.btnQ5EditGuarantor = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnQ5EditPayor = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnQ5EditDiagnosis = new PatientAccess.UI.CommonControls.LoggingButton();
            this.btnQ5EditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ5GuarantorText = new System.Windows.Forms.Label();
            this.lblQ5Guarantor = new System.Windows.Forms.Label();
            this.lblQ5PolicyText = new System.Windows.Forms.Label();
            this.lblQ5Policy = new System.Windows.Forms.Label();
            this.lblQ5DateText = new System.Windows.Forms.Label();
            this.lblQ5Date = new System.Windows.Forms.Label();
            this.lblQ5InsuranceText = new System.Windows.Forms.Label();
            this.checkBoxYesNoGroup5 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.pnlDividerQ5 = new System.Windows.Forms.Panel();
            this.pnlWizardPageBody.SuspendLayout();
            this.pnlQuestion1.SuspendLayout();
            this.pnlQuestion1b.SuspendLayout();
            this.pnlQuestion2.SuspendLayout();
            this.pnlQuestion3.SuspendLayout();
            this.pnlQuestion4.SuspendLayout();
            this.pnlQuestion5.SuspendLayout();
            this.gbQuestion5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion5);
            this.pnlWizardPageBody.Controls.Add(this.lblQuestion4b);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion4);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion3);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion1);
            this.pnlWizardPageBody.Controls.Add(this.pnlDivider1);
            this.pnlWizardPageBody.Controls.Add(this.pnlQuestion2);
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion2, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlDivider1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion1, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion3, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion4, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.lblQuestion4b, 0);
            this.pnlWizardPageBody.Controls.SetChildIndex(this.pnlQuestion5, 0);
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point(17, 54);
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size(640, 2);
            this.pnlDivider1.TabIndex = 0;
            // 
            // pnlQuestion1
            // 
            this.pnlQuestion1.Controls.Add(this.checkBoxYesNoGroup1);
            this.pnlQuestion1.Controls.Add(this.lblQuestion1);
            this.pnlQuestion1.Controls.Add(this.pnlDividerQ1);
            this.pnlQuestion1.Controls.Add(this.pnlQuestion1b);
            this.pnlQuestion1.Location = new System.Drawing.Point(8, 60);
            this.pnlQuestion1.Name = "pnlQuestion1";
            this.pnlQuestion1.Size = new System.Drawing.Size(666, 67);
            this.pnlQuestion1.TabIndex = 2;
            this.pnlQuestion1.TabStop = true;
            // 
            // checkBoxYesNoGroup1
            // 
            this.checkBoxYesNoGroup1.Location = new System.Drawing.Point(521, 1);
            this.checkBoxYesNoGroup1.Name = "checkBoxYesNoGroup1";
            this.checkBoxYesNoGroup1.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup1.TabIndex = 1;
            this.checkBoxYesNoGroup1.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup1_RadioChanged);
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size(375, 16);
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1.  Are you receiving Black Lung (BL) benefits?";
            // 
            // pnlDividerQ1
            // 
            this.pnlDividerQ1.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ1.Location = new System.Drawing.Point(5, 65);
            this.pnlDividerQ1.Name = "pnlDividerQ1";
            this.pnlDividerQ1.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ1.TabIndex = 0;
            // 
            // pnlQuestion1b
            // 
            this.pnlQuestion1b.Controls.Add(this.mtbQ1DateBegan);
            this.pnlQuestion1b.Controls.Add(this.lblQ1DateBegan);
            this.pnlQuestion1b.Location = new System.Drawing.Point(11, 30);
            this.pnlQuestion1b.Name = "pnlQuestion1b";
            this.pnlQuestion1b.Size = new System.Drawing.Size(237, 35);
            this.pnlQuestion1b.TabIndex = 2;
            // 
            // mtbQ1DateBegan
            // 
            this.mtbQ1DateBegan.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ1DateBegan.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbQ1DateBegan.Location = new System.Drawing.Point(122, 8);
            this.mtbQ1DateBegan.Mask = "  /  /";
            this.mtbQ1DateBegan.MaxLength = 10;
            this.mtbQ1DateBegan.Name = "mtbQ1DateBegan";
            this.mtbQ1DateBegan.Size = new System.Drawing.Size(70, 20);
            this.mtbQ1DateBegan.TabIndex = 1;
            this.mtbQ1DateBegan.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbQ1DateBegan.Leave += new System.EventHandler(this.mtbQ1DateBegan_Leave);
            // 
            // lblQ1DateBegan
            // 
            this.lblQ1DateBegan.Location = new System.Drawing.Point(14, 12);
            this.lblQ1DateBegan.Name = "lblQ1DateBegan";
            this.lblQ1DateBegan.Size = new System.Drawing.Size(115, 13);
            this.lblQ1DateBegan.TabIndex = 0;
            this.lblQ1DateBegan.Text = "Date services began:";
            // 
            // pnlQuestion2
            // 
            this.pnlQuestion2.Controls.Add(this.checkBoxYesNoGroup2);
            this.pnlQuestion2.Controls.Add(this.lblQuestion2);
            this.pnlQuestion2.Controls.Add(this.pnlDividerQ2);
            this.pnlQuestion2.Location = new System.Drawing.Point(7, 134);
            this.pnlQuestion2.Name = "pnlQuestion2";
            this.pnlQuestion2.Size = new System.Drawing.Size(667, 41);
            this.pnlQuestion2.TabIndex = 2;
            this.pnlQuestion2.TabStop = true;
            // 
            // checkBoxYesNoGroup2
            // 
            this.checkBoxYesNoGroup2.Location = new System.Drawing.Point(521, 1);
            this.checkBoxYesNoGroup2.Name = "checkBoxYesNoGroup2";
            this.checkBoxYesNoGroup2.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup2.TabIndex = 1;
            this.checkBoxYesNoGroup2.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup2_RadioChanged);
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(375, 16);
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2.  Is the reason for today\'s visit due to Black Lung?";
            // 
            // pnlDividerQ2
            // 
            this.pnlDividerQ2.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ2.Location = new System.Drawing.Point(5, 39);
            this.pnlDividerQ2.Name = "pnlDividerQ2";
            this.pnlDividerQ2.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ2.TabIndex = 0;
            // 
            // pnlQuestion3
            // 
            this.pnlQuestion3.Controls.Add(this.checkBoxYesNoGroup3);
            this.pnlQuestion3.Controls.Add(this.lblQuestion3);
            this.pnlQuestion3.Controls.Add(this.pnlDividerQ3);
            this.pnlQuestion3.Location = new System.Drawing.Point(7, 182);
            this.pnlQuestion3.Name = "pnlQuestion3";
            this.pnlQuestion3.Size = new System.Drawing.Size(668, 41);
            this.pnlQuestion3.TabIndex = 3;
            this.pnlQuestion3.TabStop = true;
            // 
            // checkBoxYesNoGroup3
            // 
            this.checkBoxYesNoGroup3.Location = new System.Drawing.Point(521, 1);
            this.checkBoxYesNoGroup3.Name = "checkBoxYesNoGroup3";
            this.checkBoxYesNoGroup3.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup3.TabIndex = 1;
            this.checkBoxYesNoGroup3.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup3_RadioChanged);
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(440, 16);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3.  Are the services to be paid by a government program such as a research grant?" +
                "";
            // 
            // pnlDividerQ3
            // 
            this.pnlDividerQ3.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ3.Location = new System.Drawing.Point(5, 39);
            this.pnlDividerQ3.Name = "pnlDividerQ3";
            this.pnlDividerQ3.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ3.TabIndex = 0;
            // 
            // pnlQuestion4
            // 
            this.pnlQuestion4.Controls.Add(this.checkBoxYesNoGroup4);
            this.pnlQuestion4.Controls.Add(this.lblQuestion4a);
            this.pnlQuestion4.Controls.Add(this.pnlDividerQ4);
            this.pnlQuestion4.Location = new System.Drawing.Point(7, 230);
            this.pnlQuestion4.Name = "pnlQuestion4";
            this.pnlQuestion4.Size = new System.Drawing.Size(668, 56);
            this.pnlQuestion4.TabIndex = 4;
            this.pnlQuestion4.TabStop = true;
            // 
            // checkBoxYesNoGroup4
            // 
            this.checkBoxYesNoGroup4.Location = new System.Drawing.Point(521, 1);
            this.checkBoxYesNoGroup4.Name = "checkBoxYesNoGroup4";
            this.checkBoxYesNoGroup4.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup4.TabIndex = 1;
            this.checkBoxYesNoGroup4.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup4_RadioChanged);
            // 
            // lblQuestion4a
            // 
            this.lblQuestion4a.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion4a.Name = "lblQuestion4a";
            this.lblQuestion4a.Size = new System.Drawing.Size(459, 13);
            this.lblQuestion4a.TabIndex = 0;
            this.lblQuestion4a.Text = "4.  Has the Department of Veteran Affairs (DVA) authorized and agreed to pay for";
            // 
            // pnlDividerQ4
            // 
            this.pnlDividerQ4.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ4.Location = new System.Drawing.Point(5, 54);
            this.pnlDividerQ4.Name = "pnlDividerQ4";
            this.pnlDividerQ4.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ4.TabIndex = 0;
            // 
            // lblQuestion4b
            // 
            this.lblQuestion4b.Location = new System.Drawing.Point(15, 258);
            this.lblQuestion4b.Name = "lblQuestion4b";
            this.lblQuestion4b.Size = new System.Drawing.Size(459, 13);
            this.lblQuestion4b.TabIndex = 0;
            this.lblQuestion4b.Text = "     your care at this facility?";
            // 
            // pnlQuestion5
            // 
            this.pnlQuestion5.Controls.Add(this.pnlQ5Divider3);
            this.pnlQuestion5.Controls.Add(this.gbQuestion5);
            this.pnlQuestion5.Controls.Add(this.checkBoxYesNoGroup5);
            this.pnlQuestion5.Controls.Add(this.lblQuestion5);
            this.pnlQuestion5.Controls.Add(this.pnlDividerQ5);
            this.pnlQuestion5.Location = new System.Drawing.Point(7, 292);
            this.pnlQuestion5.Name = "pnlQuestion5";
            this.pnlQuestion5.Size = new System.Drawing.Size(668, 251);
            this.pnlQuestion5.TabIndex = 5;
            this.pnlQuestion5.TabStop = true;
            // 
            // pnlQ5Divider3
            // 
            this.pnlQ5Divider3.BackColor = System.Drawing.Color.Black;
            this.pnlQ5Divider3.Location = new System.Drawing.Point(17, 184);
            this.pnlQ5Divider3.Name = "pnlQ5Divider3";
            this.pnlQ5Divider3.Size = new System.Drawing.Size(447, 1);
            this.pnlQ5Divider3.TabIndex = 0;
            // 
            // gbQuestion5
            // 
            this.gbQuestion5.Controls.Add(this.pnlQ5Divider1);
            this.gbQuestion5.Controls.Add(this.pnlQ5Divider2);
            this.gbQuestion5.Controls.Add(this.btnQ5EditGuarantor);
            this.gbQuestion5.Controls.Add(this.btnQ5EditPayor);
            this.gbQuestion5.Controls.Add(this.btnQ5EditDiagnosis);
            this.gbQuestion5.Controls.Add(this.btnQ5EditInsurance);
            this.gbQuestion5.Controls.Add(this.lblQ5GuarantorText);
            this.gbQuestion5.Controls.Add(this.lblQ5Guarantor);
            this.gbQuestion5.Controls.Add(this.lblQ5PolicyText);
            this.gbQuestion5.Controls.Add(this.lblQ5Policy);
            this.gbQuestion5.Controls.Add(this.lblQ5DateText);
            this.gbQuestion5.Controls.Add(this.lblQ5Date);
            this.gbQuestion5.Controls.Add(this.lblQ5InsuranceText);
            this.gbQuestion5.Location = new System.Drawing.Point(8, 47);
            this.gbQuestion5.Name = "gbQuestion5";
            this.gbQuestion5.Size = new System.Drawing.Size(466, 196);
            this.gbQuestion5.TabIndex = 2;
            this.gbQuestion5.TabStop = false;
            this.gbQuestion5.Text = "Worker\'s Compensation";
            // 
            // pnlQ5Divider1
            // 
            this.pnlQ5Divider1.BackColor = System.Drawing.Color.Black;
            this.pnlQ5Divider1.Location = new System.Drawing.Point(9, 60);
            this.pnlQ5Divider1.Name = "pnlQ5Divider1";
            this.pnlQ5Divider1.Size = new System.Drawing.Size(447, 1);
            this.pnlQ5Divider1.TabIndex = 0;
            // 
            // pnlQ5Divider2
            // 
            this.pnlQ5Divider2.BackColor = System.Drawing.Color.Black;
            this.pnlQ5Divider2.Location = new System.Drawing.Point(10, 98);
            this.pnlQ5Divider2.Name = "pnlQ5Divider2";
            this.pnlQ5Divider2.Size = new System.Drawing.Size(447, 1);
            this.pnlQ5Divider2.TabIndex = 0;
            // 
            // btnQ5EditGuarantor
            // 
            this.btnQ5EditGuarantor.Location = new System.Drawing.Point(271, 156);
            this.btnQ5EditGuarantor.Message = null;
            this.btnQ5EditGuarantor.Name = "btnQ5EditGuarantor";
            this.btnQ5EditGuarantor.Size = new System.Drawing.Size(180, 23);
            this.btnQ5EditGuarantor.TabIndex = 4;
            this.btnQ5EditGuarantor.Text = "Edit &Guarantor && Cancel MSP";
            this.btnQ5EditGuarantor.Click += new System.EventHandler(this.btnQ5EditGuarantor_Click);
            // 
            // btnQ5EditPayor
            // 
            this.btnQ5EditPayor.Location = new System.Drawing.Point(271, 106);
            this.btnQ5EditPayor.Message = null;
            this.btnQ5EditPayor.Name = "btnQ5EditPayor";
            this.btnQ5EditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnQ5EditPayor.TabIndex = 3;
            this.btnQ5EditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnQ5EditPayor.Click += new System.EventHandler(this.btnQ5EditPayor_Click);
            // 
            // btnQ5EditDiagnosis
            // 
            this.btnQ5EditDiagnosis.Location = new System.Drawing.Point(271, 68);
            this.btnQ5EditDiagnosis.Message = null;
            this.btnQ5EditDiagnosis.Name = "btnQ5EditDiagnosis";
            this.btnQ5EditDiagnosis.Size = new System.Drawing.Size(180, 23);
            this.btnQ5EditDiagnosis.TabIndex = 2;
            this.btnQ5EditDiagnosis.Text = "Edit Diagn&osis && Cancel MSP";
            this.btnQ5EditDiagnosis.Click += new System.EventHandler(this.btnQ5EditDiagnosis_Click);
            // 
            // btnQ5EditInsurance
            // 
            this.btnQ5EditInsurance.Location = new System.Drawing.Point(271, 25);
            this.btnQ5EditInsurance.Message = null;
            this.btnQ5EditInsurance.Name = "btnQ5EditInsurance";
            this.btnQ5EditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnQ5EditInsurance.TabIndex = 1;
            this.btnQ5EditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnQ5EditInsurance.Click += new System.EventHandler(this.btnQ5EditInsurance_Click);
            // 
            // lblQ5GuarantorText
            // 
            this.lblQ5GuarantorText.Location = new System.Drawing.Point(65, 145);
            this.lblQ5GuarantorText.Name = "lblQ5GuarantorText";
            this.lblQ5GuarantorText.Size = new System.Drawing.Size(174, 41);
            this.lblQ5GuarantorText.TabIndex = 0;
            // 
            // lblQ5Guarantor
            // 
            this.lblQ5Guarantor.Location = new System.Drawing.Point(8, 145);
            this.lblQ5Guarantor.Name = "lblQ5Guarantor";
            this.lblQ5Guarantor.Size = new System.Drawing.Size(58, 23);
            this.lblQ5Guarantor.TabIndex = 0;
            this.lblQ5Guarantor.Text = "Guarantor:";
            // 
            // lblQ5PolicyText
            // 
            this.lblQ5PolicyText.Location = new System.Drawing.Point(111, 103);
            this.lblQ5PolicyText.Name = "lblQ5PolicyText";
            this.lblQ5PolicyText.Size = new System.Drawing.Size(120, 23);
            this.lblQ5PolicyText.TabIndex = 0;
            // 
            // lblQ5Policy
            // 
            this.lblQ5Policy.Location = new System.Drawing.Point(8, 103);
            this.lblQ5Policy.Name = "lblQ5Policy";
            this.lblQ5Policy.Size = new System.Drawing.Size(110, 23);
            this.lblQ5Policy.TabIndex = 0;
            this.lblQ5Policy.Text = "Policy or ID number:";
            // 
            // lblQ5DateText
            // 
            this.lblQ5DateText.Location = new System.Drawing.Point(111, 72);
            this.lblQ5DateText.Name = "lblQ5DateText";
            this.lblQ5DateText.Size = new System.Drawing.Size(120, 23);
            this.lblQ5DateText.TabIndex = 0;
            // 
            // lblQ5Date
            // 
            this.lblQ5Date.Location = new System.Drawing.Point(8, 72);
            this.lblQ5Date.Name = "lblQ5Date";
            this.lblQ5Date.Size = new System.Drawing.Size(112, 23);
            this.lblQ5Date.TabIndex = 0;
            this.lblQ5Date.Text = "Date of illness/injury:";
            // 
            // lblQ5InsuranceText
            // 
            this.lblQ5InsuranceText.Location = new System.Drawing.Point(8, 19);
            this.lblQ5InsuranceText.Name = "lblQ5InsuranceText";
            this.lblQ5InsuranceText.Size = new System.Drawing.Size(225, 40);
            this.lblQ5InsuranceText.TabIndex = 0;
            // 
            // checkBoxYesNoGroup5
            // 
            this.checkBoxYesNoGroup5.Location = new System.Drawing.Point(521, 1);
            this.checkBoxYesNoGroup5.Name = "checkBoxYesNoGroup5";
            this.checkBoxYesNoGroup5.Size = new System.Drawing.Size(125, 35);
            this.checkBoxYesNoGroup5.TabIndex = 1;
            this.checkBoxYesNoGroup5.RadioChanged += new System.EventHandler(this.checkBoxYesNoGroup5_RadioChanged);
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(8, 9);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(459, 13);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5.  Was the illness/injury due to a work-related accident/condition?";
            // 
            // pnlDividerQ5
            // 
            this.pnlDividerQ5.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ5.Location = new System.Drawing.Point(5, 39);
            this.pnlDividerQ5.Name = "pnlDividerQ5";
            this.pnlDividerQ5.Size = new System.Drawing.Size(656, 1);
            this.pnlDividerQ5.TabIndex = 0;
            // 
            // SpecialProgramsPage
            // 
            this.Name = "SpecialProgramsPage";
            this.PageName = "SpecialProgramsPage";
            this.EnabledChanged += new System.EventHandler(this.SpecialProgramsPage_EnabledChanged);
            this.Load += new System.EventHandler(this.SpecialProgramsPage_Load);
            this.GotFocus += new System.EventHandler(this.SpecialProgramsPage_GotFocus);
            this.pnlWizardPageBody.ResumeLayout(false);
            this.pnlQuestion1.ResumeLayout(false);
            this.pnlQuestion1b.ResumeLayout(false);
            this.pnlQuestion2.ResumeLayout(false);
            this.pnlQuestion3.ResumeLayout(false);
            this.pnlQuestion4.ResumeLayout(false);
            this.pnlQuestion5.ResumeLayout(false);
            this.gbQuestion5.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public SpecialProgramsPage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }


        public SpecialProgramsPage( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 

            EnableThemesOn( this );
        }

        public SpecialProgramsPage( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public SpecialProgramsPage( string pageName, WizardContainer wizardContainer, Account anAccount )
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

        private Panel                                  pnlDivider1;
        private Panel                                  pnlQuestion1;
        private Panel                                  pnlQuestion1b;
        private Panel                                  pnlQuestion2;        
        private Panel                                  pnlQuestion3;
        private Panel                                  pnlDividerQ2;
        private Panel                                  pnlDividerQ1;        
        private Panel                                  pnlDividerQ3;
        private Panel                                  pnlQuestion4;
        private Panel                                  pnlDividerQ4;
        private Panel                                  pnlQuestion5;
        private Panel                                  pnlQ5Divider3;
        private Panel                                  pnlQ5Divider2;
        private Panel                                  pnlQ5Divider1;
        private Panel                                  pnlDividerQ5;        

        private Label                                  lblQuestion1;
        private Label                                  lblQ1DateBegan;
        private Label                                  lblQuestion2;
        private Label                                  lblQuestion3;
        private Label                                  lblQuestion4a;
        private Label                                  lblQuestion4b;
        private Label                                  lblQuestion5;
        private Label                                  lblQ5Date;
        private Label                                  lblQ5DateText;        
        private Label                                  lblQ5Policy;
        private Label                                  lblQ5PolicyText;
        private Label                                  lblQ5Guarantor;
        private Label                                  lblQ5GuarantorText;                
        private Label                                  lblQ5InsuranceText;

        private MaskedEditTextBox                                           mtbQ1DateBegan;

        private CheckBoxYesNoGroup     checkBoxYesNoGroup1;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup2;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup3;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup4;
        private CheckBoxYesNoGroup     checkBoxYesNoGroup5;
        
        private GroupBox                               gbQuestion5;

        private LoggingButton               btnQ5EditGuarantor;
        private LoggingButton               btnQ5EditPayor;
        private LoggingButton               btnQ5EditDiagnosis;
        private LoggingButton               btnQ5EditInsurance;

        private bool                                                        blnLoaded = false;

        #endregion

        #region Constants
        #endregion

    }
}

