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
    /// LiabilityInsurer - capture info related to non-work related accident, including whether or not the 
    /// patient has no-fault and/or liability insurance
    /// </summary>
    [Serializable]
    public class LiabilityInsurerPage : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// checkBoxYesNoGroup1_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup1_RadioChanged( object sender, EventArgs e )
        {
            if ( ( ( RadioButton )sender ).Name == "rbYes"
                && ( ( RadioButton )sender ).Checked )
            {
                displayAccidentDate();

                pnlQuestion1b.Enabled = true;
                pnlQuestion2.Enabled = true;
                pnlQuestion3.Enabled = true;

                MyWizardButtons.DisableNavigation( "&Next >" );
            }
            else
            {
                checkBoxYesNoGroup2.rbYes.Checked = false;
                checkBoxYesNoGroup2.rbNo.Checked = false;

                checkBoxYesNoGroup3.rbYes.Checked = false;
                checkBoxYesNoGroup3.rbNo.Checked = false;

                pnlQuestion1b.Enabled = false;
                pnlQuestion2.Enabled = false;
                pnlQuestion3.Enabled = false;
            }

            CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup2_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup2_RadioChanged( object sender, EventArgs e )
        {
            if ( ( ( RadioButton )sender ).Name == "rbYes"
                && ( ( RadioButton )sender ).Checked )
            {
                gbQ2NoFaultInsurer.Enabled = true;
                displayNoFaultInsurer( true );
            }
            else
            {
                gbQ2NoFaultInsurer.Enabled = false;
                displayNoFaultInsurer( false );
            }

            CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup3_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup3_RadioChanged( object sender, EventArgs e )
        {
            if ( ( ( RadioButton )sender ).Name == "rbYes"
                && ( ( RadioButton )sender ).Checked )
            {
                gbQ3LiabilityInsurer.Enabled = true;
                displayLiabilityInsurer( true );
            }
            else
            {
                gbQ3LiabilityInsurer.Enabled = false;
                displayLiabilityInsurer( false );
            }

            CanPageNavigate();
        }

        /// <summary>
        /// btnQ1EditDiagnosis_Click - the user has opted to abandon the MSP wizard and edit the diagnosis
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ1EditDiagnosis_Click( object sender, EventArgs e )
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
        /// btnQ2MoreInfo_Click - dislay the custom 'messagebox' with more info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ2MoreInfo_Click( object sender, EventArgs e )
        {
            CloseMessageBox box = new CloseMessageBox( "Is no-fault insurance available?",
                "No-fault insurance is insurance that pays for health care services resulting from "
                + "injury to you or damage to your property regardless of who is at fault for causing the accident." );

            box.Show();
        }

        /// <summary>
        /// btnQ2EditInsured_Click - the user has opted to abandon the MSP wizard and edit the insured
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ2EditInsured_Click( object sender, EventArgs e )
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
        /// btnQ2EditPayor_Click - the user has opted to abandon the MSP wizard and edit the payor
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ2EditPayor_Click( object sender, EventArgs e )
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
        /// btnQ2EditInsurance_Click - the user has opted to abandon the MSP wizard and edit the employment
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ2EditInsurance_Click( object sender, EventArgs e )
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
        /// btnQ2MoreInfo_Click - dislay the custom 'messagebox' with more info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ3MoreInfo_Click( object sender, EventArgs e )
        {
            CloseMessageBox box = new CloseMessageBox( "Is liability insurance available?",
                "Liability insurance is insurance that protects against claims based on negligence, inappropriate  "
                + "action or inaction, which results in injury to someone or damage to property." );

            box.Show();
        }

        /// <summary>
        /// btnQ3EditInsurance_Click - the user has opted to abandon the MSP wizard and edit the insurance
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ3EditInsurance_Click( object sender, EventArgs e )
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
        /// btnQ3EditPayor_Click - the user has opted to abandon the MSP wizard and edit the payor
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQ3EditPayor_Click( object sender, EventArgs e )
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
        /// LiabilityInsurerPage_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiabilityInsurerPage_EnabledChanged( object sender, EventArgs e )
        {
            if ( Enabled )
            {
                UpdateView();
            }
        }

        /// <summary>
        /// LiabilityInsurerPage_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiabilityInsurerPage_Load( object sender, EventArgs e )
        {
            LinkName = "Liability Insurer";
            MyWizardMessages.Message1 = "Liability Insurer";
            MyWizardMessages.TextFont1 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize1 = 8.25;
            MyWizardMessages.FontStyle1 = FontStyle.Bold;

            MyWizardMessages.Message2 = "Complete each question in this section as it pertains to the patient's reason for today's visit.";

            MyWizardMessages.TextFont2 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize2 = 8.25;

            MyWizardMessages.ShowMessages();

            pnlQuestion1b.Enabled = false;
            pnlQuestion2.Enabled = false;
            gbQ2NoFaultInsurer.Enabled = false;
            pnlQuestion3.Enabled = false;
            gbQ3LiabilityInsurer.Enabled = false;
        }

        /// <summary>
        /// displayLiabilityInsurer - display the primary coverage info for the patient
        /// </summary>
        /// <param name="display"></param>
        private void displayLiabilityInsurer( bool display )
        {
            if ( display )
            {
                if ( ParentForm != null )
                {
                    Coverage coverage = ( ( MSP2Dialog )ParentForm ).GetPrimaryCoverage();

                    if ( coverage == null )
                    {
                        lblQ3LiabilityInfo.Text = "Not available";
                        lblQ3ClaimNumberText.Text = "Not available";
                        return;
                    }
                    if ( coverage.BillingInformation.Address != null )
                    {
                        string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();

                        if ( mailingLabel != String.Empty )
                        {
                            lblQ3LiabilityInfo.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                                                                     Environment.NewLine, mailingLabel );
                        }
                        else
                        {
                            lblQ3LiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                        }
                    }
                    else
                    {
                        lblQ3LiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                    }

                    if ( coverage.GetType().Equals( typeof( CommercialCoverage ) ) )
                    {
                        lblQ3ClaimNumberText.Text = ( ( CommercialCoverage )coverage ).AutoInsuranceClaimNumber;
                    }
                    else if ( coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                    {
                        lblQ3ClaimNumberText.Text = ( ( OtherCoverage )coverage ).AutoInsuranceClaimNumber;
                    }
                    else if ( coverage.GetType().Equals( typeof( WorkersCompensationCoverage ) ) )
                    {
                        lblQ3ClaimNumberText.Text = ( ( WorkersCompensationCoverage )coverage ).ClaimNumberForIncident;
                    }
                    else
                    {
                        lblQ3ClaimNumberText.Text = "Not available";
                    }
                }
            }
            else
            {
                // clear the fields

                lblQ3LiabilityInfo.Text = string.Empty;
                lblQ3ClaimNumberText.Text = string.Empty;
            }
        }

        /// <summary>
        /// displayNoFaultInsurer - display the primary coverage for the patient
        /// </summary>
        /// <param name="display"></param>
        private void displayNoFaultInsurer( bool display )
        {
            if ( display )
            {
                if ( ParentForm != null )
                {
                    Coverage coverage = ( ( MSP2Dialog )ParentForm ).GetPrimaryCoverage();

                    if ( coverage == null )
                    {
                        lblQ2NoFaultInfo.Text = "Not available";
                        lblQ2InsuredText.Text = "Not available";
                        lblQ2ClaimNumberText.Text = "Not available";

                        return;
                    }

                    if ( coverage.BillingInformation.Address != null )
                    {
                        string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                        if ( mailingLabel != String.Empty )
                        {
                            lblQ2NoFaultInfo.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                                                                   Environment.NewLine, mailingLabel );
                        }
                        else
                        {
                            lblQ2NoFaultInfo.Text = coverage.InsurancePlan.PlanName;
                        }
                    }
                    else
                    {
                        lblQ2NoFaultInfo.Text = coverage.InsurancePlan.PlanName;
                    }

                    if ( coverage.Insured == null
                        || coverage.Insured.FormattedName == string.Empty )
                    {
                        lblQ2InsuredText.Text = "Not available";
                    }
                    else
                    {
                        lblQ2InsuredText.Text = coverage.Insured.FormattedName;
                    }

                    if ( coverage.GetType().Equals( typeof( CommercialCoverage ) ) ||
                        coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                    {
                        string claimNumber = ( ( CommercialCoverage )coverage ).AutoInsuranceClaimNumber;

                        if ( claimNumber != String.Empty )
                        {
                            lblQ2ClaimNumberText.Text = claimNumber;
                        }
                        else
                        {
                            lblQ2ClaimNumberText.Text = "Claim number missing";
                        }
                    }
                    else if ( coverage.GetType().Equals( typeof( WorkersCompensationCoverage ) ) )
                    {
                        string claimNumber = ( ( WorkersCompensationCoverage )coverage ).ClaimNumberForIncident;

                        lblQ2ClaimNumberText.Text = ( claimNumber != String.Empty ) ? claimNumber : "Claim number missing";
                    }
                    else
                    {
                        lblQ2ClaimNumberText.Text = "Not available";
                    }
                }
            }
            else
            {
                // clear the fields

                lblQ2InsuredText.Text = string.Empty;
                lblQ2NoFaultInfo.Text = string.Empty;
                lblQ2ClaimNumberText.Text = string.Empty;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        public override void ResetPage()
        {
            base.ResetPage();

            checkBoxYesNoGroup1.rbYes.Checked = false;
            checkBoxYesNoGroup1.rbNo.Checked = false;
            checkBoxYesNoGroup2.rbYes.Checked = false;
            checkBoxYesNoGroup2.rbNo.Checked = false;
            checkBoxYesNoGroup3.rbYes.Checked = false;
            checkBoxYesNoGroup3.rbNo.Checked = false;
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        private bool CheckForSummary()
        {
            bool rc = false;

            MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if ( CanNavigate )
            {
                MedicareEntitlementPage aPage = MyWizardContainer.GetWizardPage( "MedicareEntitlementPage" )
                    as MedicareEntitlementPage;

                if ( aPage != null )
                {
                    if ( aPage.EntitlementCanNavigate() )
                    {
                        rc = true;
                        MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                        MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
                    }
                }
            }

            HasSummary = rc;
            return rc;
        }

        /// <summary>
        /// CanPageNavigate - determine if all requirements are met (fields entered, questions answered, etc).
        /// If so, set navigation to the next page in the wizard.
        /// </summary>
        /// <returns></returns>
        private bool CanPageNavigate()
        {
            bool canNav = false;

            MyWizardButtons.UpdateNavigation( "&Next >", string.Empty );
            MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            // ensure that all checkboxes have been designated

            if ( ( checkBoxYesNoGroup1.rbYes.Checked
                || checkBoxYesNoGroup1.rbNo.Checked )
                &&
                ( checkBoxYesNoGroup2.Enabled == false
                || checkBoxYesNoGroup2.rbYes.Checked
                || checkBoxYesNoGroup2.rbNo.Checked )
                &&
                ( checkBoxYesNoGroup3.Enabled == false
                || checkBoxYesNoGroup3.rbYes.Checked
                || checkBoxYesNoGroup3.rbNo.Checked )
                )
            {
                canNav = true;
                MyWizardButtons.UpdateNavigation( "&Next >", "MedicareEntitlementPage" );
                MyWizardButtons.SetAcceptButton( "&Next >" );
            }

            CanNavigate = canNav;

            CheckForSummary();

            return canNav;
        }

        /// <summary>
        /// Cancel - is the delegate for the Cancel button click event
        /// </summary>
        private void Cancel()
        {
            if ( MSPCancelled != null )
            {
                MSPCancelled( this, null );
            }

            MyWizardContainer.Cancel();
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {
            if ( Model_Account == null )
            {
                return;
            }

            if ( !blnLoaded )
            {
                blnLoaded = true;

                if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.Code == YesNoFlag.CODE_YES )
                {   // Other buttons are disabled if first one is "No"

                    checkBoxYesNoGroup1.rbYes.Checked = true;

                    lblQ1DateOfAccidentText.Text = GetAccidentDateForDisplay();

                    if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NoFaultInsuranceAvailable.Code == YesNoFlag.CODE_YES )
                    {
                        checkBoxYesNoGroup2.rbYes.Checked = true;
                    }
                    else if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NoFaultInsuranceAvailable.Code == YesNoFlag.CODE_NO )
                    {
                        checkBoxYesNoGroup2.rbNo.Checked = true;
                    }

                    if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.LiabilityInsuranceAvailable.Code == YesNoFlag.CODE_YES )
                    {
                        checkBoxYesNoGroup3.rbYes.Checked = true;
                    }
                    else if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.LiabilityInsuranceAvailable.Code == YesNoFlag.CODE_NO )
                    {
                        checkBoxYesNoGroup3.rbNo.Checked = true;
                    }
                }
                else if ( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.Code == YesNoFlag.CODE_NO )
                {
                    checkBoxYesNoGroup1.rbNo.Checked = true;
                }
            }

            CanPageNavigate();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel();

            LiabilityInsurer li = new LiabilityInsurer();

            if ( checkBoxYesNoGroup1.rbYes.Checked )
            {
                li.NonWorkRelated.Code = YesNoFlag.CODE_YES;

                if ( lblQ1DateOfAccidentText.Text.Trim().Length == 10 )
                {
                    li.AccidentDate = DateTime.Parse( lblQ1DateOfAccidentText.Text );
                }

                if ( checkBoxYesNoGroup2.rbYes.Checked )
                {
                    li.NoFaultInsuranceAvailable.Code = YesNoFlag.CODE_YES;
                }
                else if ( checkBoxYesNoGroup2.rbNo.Checked )
                {
                    li.NoFaultInsuranceAvailable.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    li.NoFaultInsuranceAvailable.Code = YesNoFlag.CODE_BLANK;
                }

                if ( checkBoxYesNoGroup3.rbYes.Checked )
                {
                    li.LiabilityInsuranceAvailable.Code = YesNoFlag.CODE_YES;
                }
                else if ( checkBoxYesNoGroup3.rbNo.Checked )
                {
                    li.LiabilityInsuranceAvailable.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    li.LiabilityInsuranceAvailable.Code = YesNoFlag.CODE_BLANK;
                }
            }
            else if ( checkBoxYesNoGroup1.rbNo.Checked )
            {
                li.NonWorkRelated.Code = YesNoFlag.CODE_NO;
            }
            else
            {
                li.NonWorkRelated.Code = YesNoFlag.CODE_BLANK;
            }

            Model_Account.MedicareSecondaryPayor.LiabilityInsurer = li;
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {
            MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( Cancel ) );
            MyWizardButtons.AddNavigation( "< &Back", "SpecialProgramsPage" );
            MyWizardButtons.AddNavigation( "&Next >", string.Empty );
            MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );

            MyWizardButtons.SetPanel();
        }

        internal string GetAccidentDateForDisplay()
        {
            DateTime date = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentDate;
            if ( date != DateTime.MinValue )
            {
                return String.Format( "{0:D2}/{1:D2}/{2:D4}", date.Month, date.Day, date.Year );
            }

            return string.Empty;
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

        private void displayAccidentDate()
        {
            lblQ1DateOfAccidentText.Text = string.Empty;

            if ( Model_Account != null
                && Model_Account.Diagnosis != null
                && Model_Account.Diagnosis.Condition != null )
            {
                if ( Model_Account.Diagnosis.Condition.GetType() == typeof( Accident )
                    || Model_Account.Diagnosis.Condition.GetType() == typeof( Crime ) )
                {
                    TimeAndLocationBoundCondition condition = ( TimeAndLocationBoundCondition )Model_Account.Diagnosis.Condition;

                    if ( condition.OccurredOn != DateTime.MinValue )
                    {
                        lblQ1DateOfAccidentText.Text = String.Format( "{0:D2}/{1:D2}/{2:D4}",
                            condition.OccurredOn.Month, condition.OccurredOn.Day, condition.OccurredOn.Year );
                    }

                }
            }
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQuestion1 = new System.Windows.Forms.Panel();
            this.pnlDividerQ1 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup1 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.pnlQuestion1b = new System.Windows.Forms.Panel();
            this.btnQ1EditDiagnosis = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ1DateOfAccidentText = new System.Windows.Forms.Label();
            this.lblQ1DateOfAccident = new System.Windows.Forms.Label();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlQuestion2 = new System.Windows.Forms.Panel();
            this.btnQ2MoreInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.checkBoxYesNoGroup2 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.pnlDividerQ2 = new System.Windows.Forms.Panel();
            this.gbQ2NoFaultInsurer = new System.Windows.Forms.GroupBox();
            this.lblQ2InsuredText = new System.Windows.Forms.Label();
            this.lblQ2Insured = new System.Windows.Forms.Label();
            this.btnQ2EditInsured = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ2NoFaultInfo = new System.Windows.Forms.Label();
            this.lblQ2ClaimNumberText = new System.Windows.Forms.Label();
            this.btnQ2EditPayor = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ2ClaimNumber = new System.Windows.Forms.Label();
            this.lblQ2Line = new System.Windows.Forms.Label();
            this.btnQ2EditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pnlQuestion3 = new System.Windows.Forms.Panel();
            this.btnQ3MoreInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.checkBoxYesNoGroup3 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.gbQ3LiabilityInsurer = new System.Windows.Forms.GroupBox();
            this.btnQ3EditPayor = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ3LiabilityInfo = new System.Windows.Forms.Label();
            this.lblQ3ClaimNumberText = new System.Windows.Forms.Label();
            this.lblQ3ClaimNumber = new System.Windows.Forms.Label();
            this.lblQ3Line = new System.Windows.Forms.Label();
            this.btnQ3EditInsurance = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pnlWizardPageBody.SuspendLayout();
            this.pnlQuestion1.SuspendLayout();
            this.pnlQuestion1b.SuspendLayout();
            this.pnlQuestion2.SuspendLayout();
            this.gbQ2NoFaultInsurer.SuspendLayout();
            this.pnlQuestion3.SuspendLayout();
            this.gbQ3LiabilityInsurer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion3 );
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion2 );
            this.pnlWizardPageBody.Controls.Add( this.pnlDivider1 );
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion1 );
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion1, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlDivider1, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion2, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion3, 0 );
            // 
            // pnlQuestion1
            // 
            this.pnlQuestion1.Controls.Add( this.pnlDividerQ1 );
            this.pnlQuestion1.Controls.Add( this.checkBoxYesNoGroup1 );
            this.pnlQuestion1.Controls.Add( this.lblQuestion1 );
            this.pnlQuestion1.Controls.Add( this.pnlQuestion1b );
            this.pnlQuestion1.Location = new System.Drawing.Point( 7, 63 );
            this.pnlQuestion1.Name = "pnlQuestion1";
            this.pnlQuestion1.Size = new System.Drawing.Size( 667, 67 );
            this.pnlQuestion1.TabIndex = 1;
            // 
            // pnlDividerQ1
            // 
            this.pnlDividerQ1.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ1.Location = new System.Drawing.Point( 5, 64 );
            this.pnlDividerQ1.Name = "pnlDividerQ1";
            this.pnlDividerQ1.Size = new System.Drawing.Size( 656, 1 );
            this.pnlDividerQ1.TabIndex = 0;
            // 
            // checkBoxYesNoGroup1
            // 
            this.checkBoxYesNoGroup1.Location = new System.Drawing.Point( 526, 1 );
            this.checkBoxYesNoGroup1.Name = "checkBoxYesNoGroup1";
            this.checkBoxYesNoGroup1.Size = new System.Drawing.Size( 125, 35 );
            this.checkBoxYesNoGroup1.TabIndex = 1;
            this.checkBoxYesNoGroup1.RadioChanged += new System.EventHandler( this.checkBoxYesNoGroup1_RadioChanged );
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size( 344, 14 );
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1.  Was illness/injury due to a non-work-related accident?";
            // 
            // pnlQuestion1b
            // 
            this.pnlQuestion1b.Controls.Add( this.btnQ1EditDiagnosis );
            this.pnlQuestion1b.Controls.Add( this.lblQ1DateOfAccidentText );
            this.pnlQuestion1b.Controls.Add( this.lblQ1DateOfAccident );
            this.pnlQuestion1b.Location = new System.Drawing.Point( 21, 25 );
            this.pnlQuestion1b.Name = "pnlQuestion1b";
            this.pnlQuestion1b.Size = new System.Drawing.Size( 442, 35 );
            this.pnlQuestion1b.TabIndex = 2;
            // 
            // btnQ1EditDiagnosis
            // 
            this.btnQ1EditDiagnosis.Location = new System.Drawing.Point( 257, 8 );
            this.btnQ1EditDiagnosis.Message = null;
            this.btnQ1EditDiagnosis.Name = "btnQ1EditDiagnosis";
            this.btnQ1EditDiagnosis.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ1EditDiagnosis.TabIndex = 1;
            this.btnQ1EditDiagnosis.Text = "Edit Diagn&osis && Cancel MSP";
            this.btnQ1EditDiagnosis.Click += new System.EventHandler( this.btnQ1EditDiagnosis_Click );
            // 
            // lblQ1DateOfAccidentText
            // 
            this.lblQ1DateOfAccidentText.Location = new System.Drawing.Point( 105, 12 );
            this.lblQ1DateOfAccidentText.Name = "lblQ1DateOfAccidentText";
            this.lblQ1DateOfAccidentText.Size = new System.Drawing.Size( 70, 14 );
            this.lblQ1DateOfAccidentText.TabIndex = 0;
            // 
            // lblQ1DateOfAccident
            // 
            this.lblQ1DateOfAccident.Location = new System.Drawing.Point( 14, 12 );
            this.lblQ1DateOfAccident.Name = "lblQ1DateOfAccident";
            this.lblQ1DateOfAccident.Size = new System.Drawing.Size( 90, 13 );
            this.lblQ1DateOfAccident.TabIndex = 0;
            this.lblQ1DateOfAccident.Text = "Date of accident:";
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point( 9, 55 );
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size( 656, 2 );
            this.pnlDivider1.TabIndex = 0;
            // 
            // pnlQuestion2
            // 
            this.pnlQuestion2.Controls.Add( this.btnQ2MoreInfo );
            this.pnlQuestion2.Controls.Add( this.checkBoxYesNoGroup2 );
            this.pnlQuestion2.Controls.Add( this.lblQuestion2 );
            this.pnlQuestion2.Controls.Add( this.pnlDividerQ2 );
            this.pnlQuestion2.Controls.Add( this.gbQ2NoFaultInsurer );
            this.pnlQuestion2.Location = new System.Drawing.Point( 4, 137 );
            this.pnlQuestion2.Name = "pnlQuestion2";
            this.pnlQuestion2.Size = new System.Drawing.Size( 670, 190 );
            this.pnlQuestion2.TabIndex = 2;
            // 
            // btnQ2MoreInfo
            // 
            this.btnQ2MoreInfo.Location = new System.Drawing.Point( 314, 3 );
            this.btnQ2MoreInfo.Message = null;
            this.btnQ2MoreInfo.Size = new System.Drawing.Size( 75, 23 );
            this.btnQ2MoreInfo.Name = "btnQ2MoreInfo";
            this.btnQ2MoreInfo.TabIndex = 1;
            this.btnQ2MoreInfo.Text = "More info";
            this.btnQ2MoreInfo.Click += new System.EventHandler( this.btnQ2MoreInfo_Click );
            // 
            // checkBoxYesNoGroup2
            // 
            this.checkBoxYesNoGroup2.Location = new System.Drawing.Point( 526, 2 );
            this.checkBoxYesNoGroup2.Name = "checkBoxYesNoGroup2";
            this.checkBoxYesNoGroup2.Size = new System.Drawing.Size( 125, 35 );
            this.checkBoxYesNoGroup2.TabIndex = 2;
            this.checkBoxYesNoGroup2.RadioChanged += new System.EventHandler( this.checkBoxYesNoGroup2_RadioChanged );
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size( 201, 14 );
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2.  Is no-fault insurance available?";
            // 
            // pnlDividerQ2
            // 
            this.pnlDividerQ2.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ2.Location = new System.Drawing.Point( 7, 187 );
            this.pnlDividerQ2.Name = "pnlDividerQ2";
            this.pnlDividerQ2.Size = new System.Drawing.Size( 656, 1 );
            this.pnlDividerQ2.TabIndex = 0;
            // 
            // gbQ2NoFaultInsurer
            // 
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2InsuredText );
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2Insured );
            this.gbQ2NoFaultInsurer.Controls.Add( this.btnQ2EditInsured );
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2NoFaultInfo );
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2ClaimNumberText );
            this.gbQ2NoFaultInsurer.Controls.Add( this.btnQ2EditPayor );
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2ClaimNumber );
            this.gbQ2NoFaultInsurer.Controls.Add( this.lblQ2Line );
            this.gbQ2NoFaultInsurer.Controls.Add( this.btnQ2EditInsurance );
            this.gbQ2NoFaultInsurer.Location = new System.Drawing.Point( 7, 26 );
            this.gbQ2NoFaultInsurer.Name = "gbQ2NoFaultInsurer";
            this.gbQ2NoFaultInsurer.Size = new System.Drawing.Size( 469, 155 );
            this.gbQ2NoFaultInsurer.TabIndex = 3;
            this.gbQ2NoFaultInsurer.TabStop = false;
            this.gbQ2NoFaultInsurer.Text = "No-fault insurer";
            // 
            // lblQ2InsuredText
            // 
            this.lblQ2InsuredText.Location = new System.Drawing.Point( 128, 75 );
            this.lblQ2InsuredText.Name = "lblQ2InsuredText";
            this.lblQ2InsuredText.Size = new System.Drawing.Size( 324, 17 );
            this.lblQ2InsuredText.TabIndex = 0;
            // 
            // lblQ2Insured
            // 
            this.lblQ2Insured.Location = new System.Drawing.Point( 8, 75 );
            this.lblQ2Insured.Name = "lblQ2Insured";
            this.lblQ2Insured.Size = new System.Drawing.Size( 128, 18 );
            this.lblQ2Insured.TabIndex = 0;
            this.lblQ2Insured.Text = "Insured:";
            // 
            // btnQ2EditInsured
            // 
            this.btnQ2EditInsured.Location = new System.Drawing.Point( 59, 124 );
            this.btnQ2EditInsured.Message = null;
            this.btnQ2EditInsured.Name = "btnQ2EditInsured";
            this.btnQ2EditInsured.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ2EditInsured.TabIndex = 2;
            this.btnQ2EditInsured.Text = "Edit &Insured && Cancel MSP";
            this.btnQ2EditInsured.Click += new System.EventHandler( this.btnQ2EditInsured_Click );
            // 
            // lblQ2NoFaultInfo
            // 
            this.lblQ2NoFaultInfo.Location = new System.Drawing.Point( 12, 21 );
            this.lblQ2NoFaultInfo.Name = "lblQ2NoFaultInfo";
            this.lblQ2NoFaultInfo.Size = new System.Drawing.Size( 250, 40 );
            this.lblQ2NoFaultInfo.TabIndex = 0;
            // 
            // lblQ2ClaimNumberText
            // 
            this.lblQ2ClaimNumberText.Location = new System.Drawing.Point( 128, 101 );
            this.lblQ2ClaimNumberText.Name = "lblQ2ClaimNumberText";
            this.lblQ2ClaimNumberText.Size = new System.Drawing.Size( 135, 16 );
            this.lblQ2ClaimNumberText.TabIndex = 0;
            // 
            // btnQ2EditPayor
            // 
            this.btnQ2EditPayor.Location = new System.Drawing.Point( 246, 124 );
            this.btnQ2EditPayor.Message = null;
            this.btnQ2EditPayor.Name = "btnQ2EditPayor";
            this.btnQ2EditPayor.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ2EditPayor.TabIndex = 3;
            this.btnQ2EditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnQ2EditPayor.Click += new System.EventHandler( this.btnQ2EditPayor_Click );
            // 
            // lblQ2ClaimNumber
            // 
            this.lblQ2ClaimNumber.Location = new System.Drawing.Point( 8, 101 );
            this.lblQ2ClaimNumber.Name = "lblQ2ClaimNumber";
            this.lblQ2ClaimNumber.Size = new System.Drawing.Size( 128, 17 );
            this.lblQ2ClaimNumber.TabIndex = 0;
            this.lblQ2ClaimNumber.Text = "Insurance claim number:";
            // 
            // lblQ2Line
            // 
            this.lblQ2Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblQ2Line.Location = new System.Drawing.Point( 8, 70 );
            this.lblQ2Line.Name = "lblQ2Line";
            this.lblQ2Line.Size = new System.Drawing.Size( 451, 3 );
            this.lblQ2Line.TabIndex = 0;
            // 
            // btnQ2EditInsurance
            // 
            this.btnQ2EditInsurance.Location = new System.Drawing.Point( 273, 21 );
            this.btnQ2EditInsurance.Message = null;
            this.btnQ2EditInsurance.Name = "btnQ2EditInsurance";
            this.btnQ2EditInsurance.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ2EditInsurance.TabIndex = 1;
            this.btnQ2EditInsurance.Text = "Edi&t Insurance && Cancel MSP";
            this.btnQ2EditInsurance.Click += new System.EventHandler( this.btnQ2EditInsurance_Click );
            // 
            // pnlQuestion3
            // 
            this.pnlQuestion3.Controls.Add( this.btnQ3MoreInfo );
            this.pnlQuestion3.Controls.Add( this.checkBoxYesNoGroup3 );
            this.pnlQuestion3.Controls.Add( this.lblQuestion3 );
            this.pnlQuestion3.Controls.Add( this.gbQ3LiabilityInsurer );
            this.pnlQuestion3.Location = new System.Drawing.Point( 7, 334 );
            this.pnlQuestion3.Name = "pnlQuestion3";
            this.pnlQuestion3.Size = new System.Drawing.Size( 668, 158 );
            this.pnlQuestion3.TabIndex = 3;
            // 
            // btnQ3MoreInfo
            // 
            this.btnQ3MoreInfo.Location = new System.Drawing.Point( 314, 3 );
            this.btnQ3MoreInfo.Message = null;
            this.btnQ3MoreInfo.Size = new System.Drawing.Size( 75, 23 );
            this.btnQ3MoreInfo.Name = "btnQ3MoreInfo";
            this.btnQ3MoreInfo.TabIndex = 1;
            this.btnQ3MoreInfo.Text = "More info";
            this.btnQ3MoreInfo.Click += new System.EventHandler( this.btnQ3MoreInfo_Click );
            // 
            // checkBoxYesNoGroup3
            // 
            this.checkBoxYesNoGroup3.Location = new System.Drawing.Point( 526, 2 );
            this.checkBoxYesNoGroup3.Name = "checkBoxYesNoGroup3";
            this.checkBoxYesNoGroup3.Size = new System.Drawing.Size( 125, 35 );
            this.checkBoxYesNoGroup3.TabIndex = 2;
            this.checkBoxYesNoGroup3.RadioChanged += new System.EventHandler( this.checkBoxYesNoGroup3_RadioChanged );
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size( 201, 14 );
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3.  Is liability insurance available?";
            // 
            // gbQ3LiabilityInsurer
            // 
            this.gbQ3LiabilityInsurer.Controls.Add( this.btnQ3EditPayor );
            this.gbQ3LiabilityInsurer.Controls.Add( this.lblQ3LiabilityInfo );
            this.gbQ3LiabilityInsurer.Controls.Add( this.lblQ3ClaimNumberText );
            this.gbQ3LiabilityInsurer.Controls.Add( this.lblQ3ClaimNumber );
            this.gbQ3LiabilityInsurer.Controls.Add( this.lblQ3Line );
            this.gbQ3LiabilityInsurer.Controls.Add( this.btnQ3EditInsurance );
            this.gbQ3LiabilityInsurer.Location = new System.Drawing.Point( 7, 26 );
            this.gbQ3LiabilityInsurer.Name = "gbQ3LiabilityInsurer";
            this.gbQ3LiabilityInsurer.Size = new System.Drawing.Size( 469, 115 );
            this.gbQ3LiabilityInsurer.TabIndex = 3;
            this.gbQ3LiabilityInsurer.TabStop = false;
            this.gbQ3LiabilityInsurer.Text = "Liability insurer";
            // 
            // btnQ3EditPayor
            // 
            this.btnQ3EditPayor.Location = new System.Drawing.Point( 273, 83 );
            this.btnQ3EditPayor.Message = null;
            this.btnQ3EditPayor.Name = "btnQ3EditPayor";
            this.btnQ3EditPayor.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ3EditPayor.TabIndex = 2;
            this.btnQ3EditPayor.Text = "Edit Pa&yor Details && Cancel MSP";
            this.btnQ3EditPayor.Click += new System.EventHandler( this.btnQ3EditPayor_Click );
            // 
            // lblQ3LiabilityInfo
            // 
            this.lblQ3LiabilityInfo.Location = new System.Drawing.Point( 12, 21 );
            this.lblQ3LiabilityInfo.Name = "lblQ3LiabilityInfo";
            this.lblQ3LiabilityInfo.Size = new System.Drawing.Size( 250, 40 );
            this.lblQ3LiabilityInfo.TabIndex = 0;
            // 
            // lblQ3ClaimNumberText
            // 
            this.lblQ3ClaimNumberText.Location = new System.Drawing.Point( 128, 84 );
            this.lblQ3ClaimNumberText.Name = "lblQ3ClaimNumberText";
            this.lblQ3ClaimNumberText.Size = new System.Drawing.Size( 135, 16 );
            this.lblQ3ClaimNumberText.TabIndex = 0;
            // 
            // lblQ3ClaimNumber
            // 
            this.lblQ3ClaimNumber.Location = new System.Drawing.Point( 8, 84 );
            this.lblQ3ClaimNumber.Name = "lblQ3ClaimNumber";
            this.lblQ3ClaimNumber.Size = new System.Drawing.Size( 128, 17 );
            this.lblQ3ClaimNumber.TabIndex = 0;
            this.lblQ3ClaimNumber.Text = "Insurance claim number:";
            // 
            // lblQ3Line
            // 
            this.lblQ3Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblQ3Line.Location = new System.Drawing.Point( 8, 70 );
            this.lblQ3Line.Name = "lblQ3Line";
            this.lblQ3Line.Size = new System.Drawing.Size( 451, 3 );
            this.lblQ3Line.TabIndex = 0;
            // 
            // btnQ3EditInsurance
            // 
            this.btnQ3EditInsurance.Location = new System.Drawing.Point( 273, 21 );
            this.btnQ3EditInsurance.Message = null;
            this.btnQ3EditInsurance.Name = "btnQ3EditInsurance";
            this.btnQ3EditInsurance.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ3EditInsurance.TabIndex = 1;
            this.btnQ3EditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnQ3EditInsurance.Click += new System.EventHandler( this.btnQ3EditInsurance_Click );
            // 
            // LiabilityInsurerPage
            // 
            this.EnabledChanged += new System.EventHandler( this.LiabilityInsurerPage_EnabledChanged );
            this.Load += new System.EventHandler( this.LiabilityInsurerPage_Load );
            this.Name = "LiabilityInsurerPage";
            this.pnlWizardPageBody.ResumeLayout( false );
            this.pnlQuestion1.ResumeLayout( false );
            this.pnlQuestion1b.ResumeLayout( false );
            this.pnlQuestion2.ResumeLayout( false );
            this.gbQ2NoFaultInsurer.ResumeLayout( false );
            this.pnlQuestion3.ResumeLayout( false );
            this.gbQ3LiabilityInsurer.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public LiabilityInsurerPage()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        public LiabilityInsurerPage( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public LiabilityInsurerPage( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            EnableThemesOn( this );
        }

        public LiabilityInsurerPage( string pageName, WizardContainer wizardContainer, Account anAccount )
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

        private IContainer components = null;

        private Panel pnlDivider1;
        private Panel pnlQuestion1;
        private Panel pnlQuestion1b;
        private Panel pnlDividerQ1;
        private Panel pnlQuestion2;
        private Panel pnlDividerQ2;
        private Panel pnlQuestion3;

        private Label lblQuestion1;
        private Label lblQ1DateOfAccident;
        private Label lblQ1DateOfAccidentText;
        private Label lblQuestion2;
        private Label lblQ2ClaimNumberText;
        private Label lblQ2InsuredText;
        private Label lblQ2Insured;
        private Label lblQ2NoFaultInfo;
        private Label lblQ2ClaimNumber;
        private Label lblQ2Line;
        private Label lblQuestion3;
        private Label lblQ3LiabilityInfo;
        private Label lblQ3ClaimNumberText;
        private Label lblQ3ClaimNumber;
        private Label lblQ3Line;

        private CheckBoxYesNoGroup checkBoxYesNoGroup1;
        private CheckBoxYesNoGroup checkBoxYesNoGroup2;
        private CheckBoxYesNoGroup checkBoxYesNoGroup3;

        private LoggingButton btnQ1EditDiagnosis;
        private LoggingButton btnQ2MoreInfo;
        private LoggingButton btnQ2EditPayor;
        private LoggingButton btnQ2EditInsurance;
        private LoggingButton btnQ2EditInsured;
        private LoggingButton btnQ3MoreInfo;
        private LoggingButton btnQ3EditPayor;
        private LoggingButton btnQ3EditInsurance;

        private GroupBox gbQ2NoFaultInsurer;
        private GroupBox gbQ3LiabilityInsurer;

        private bool blnLoaded;

        #endregion

        #region Constants
        #endregion

    }
}

