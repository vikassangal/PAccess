using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.Factories;
using PatientAccess.UI.ShortRegistration;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// DisabilityEntitlementPage3 - captures other family member employment info
    /// </summary>
    [Serializable]
    public class DisabilityEntitlementPage3 : WizardPage
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        private void btnEditInsurance_Click( object sender, EventArgs e )
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
        /// runRules - evaluate any requirements (fields entered, questions answered, etc) for this page;
        /// determines if the page can navigate
        /// </summary>
        /// <returns></returns>
        private void runRules( object sender, EventArgs e )
        {
            bool blnRC = runRules();

            if ( blnRC )
            {
                CanPageNavigate();
            }
        }

        /// <summary>
        /// checkBoxYesNoGroup6_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup6_RadioChanged( object sender, EventArgs e )
        {
            if ( ( ( RadioButton )sender ).Name == "rbYes"
                && ( ( RadioButton )sender ).Checked )
            {
                pnlQuestion7.Enabled = true;
                pnlQuestion7c.Enabled = true;

                runRules();
            }
            else
            {
                DisplayInsurance = false;
                checkBoxYesNoGroup7.rbYes.Checked = false;
                checkBoxYesNoGroup7.rbNo.Checked = false;

                pnlQuestion7.Enabled = false;

                mtbQ7EmployerName.UnMaskedText = string.Empty;
                mtbQ7Address.UnMaskedText = string.Empty;
                mtbQ7City.UnMaskedText = string.Empty;
                mtbQ7ZipCode.UnMaskedText = string.Empty;
                cmbQ7State.SelectedIndex = 0;
            }

            CanPageNavigate();
        }

        /// <summary>
        /// checkBoxYesNoGroup7_RadioChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxYesNoGroup7_RadioChanged( object sender, EventArgs e )
        {
            displayInsurance();

            CanPageNavigate();
        }

        /// <summary>
        /// DisabilityEntitlementPage3_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisabilityEntitlementPage3_EnabledChanged( object sender, EventArgs e )
        {
            if ( Enabled )
            {
                UpdateView();
            }
        }

        /// <summary>
        /// DisabilityEntitlementPage3_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisabilityEntitlementPage3_Load( object sender, EventArgs e )
        {
            ShowLink = false;

            MyWizardMessages.Message1 = "Entitlement by Disability";
            MyWizardMessages.TextFont1 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize1 = 8.25;
            MyWizardMessages.FontStyle1 = FontStyle.Bold;

            MyWizardMessages.Message2 = string.Empty;

            MyWizardMessages.TextFont2 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize2 = 8.25;

            MyWizardMessages.ShowMessages();

            pnlQuestion7.Enabled = false;
            pnlInsurance.Enabled = false;

            // set the DisplayInsurance flag so the Page 2 may analyze the value before UpdateView has been
            // called on this page

            if ( Model_Account != null
                && Model_Account.MedicareSecondaryPayor != null
                && Model_Account.MedicareSecondaryPayor.MedicareEntitlement != null
                && Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType() == typeof( DisabilityEntitlement ) )
            {
                if ( ( ( DisabilityEntitlement )Model_Account.MedicareSecondaryPayor.MedicareEntitlement ).FamilyMemberGHPEmploysMoreThanXFlag.Code
                    == YesNoFlag.CODE_YES )
                {
                    DisplayInsurance = true;
                }
            }
        }

        /// <summary>
        /// Cancel - is the delegate for the Cancel button click event
        /// </summary>
        private void Cancel()
        {
            MyWizardContainer.Cancel();

            if ( MSPCancelled != null )
            {
                MSPCancelled( this, null );
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

            checkBoxYesNoGroup6.rbNo.Checked = false;
            checkBoxYesNoGroup6.rbYes.Checked = false;

            checkBoxYesNoGroup7.rbNo.Checked = false;
            checkBoxYesNoGroup7.rbYes.Checked = false;

            mtbQ7Address.UnMaskedText = string.Empty;
            mtbQ7City.UnMaskedText = string.Empty;
            mtbQ7EmployerName.UnMaskedText = string.Empty;
            mtbQ7ZipCode.UnMaskedText = string.Empty;
            mtbQ7ZipExtension.UnMaskedText = string.Empty;

            pnlQuestion7.Enabled = false;
            pnlInsurance.Enabled = false;

            UIColors.SetNormalBgColor( mtbQ7Address );
            UIColors.SetNormalBgColor( mtbQ7City );
            UIColors.SetNormalBgColor( mtbQ7EmployerName );
            UIColors.SetNormalBgColor( mtbQ7ZipCode );
            UIColors.SetNormalBgColor( mtbQ7ZipExtension );
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>
        private bool CheckForSummary()
        {
            bool rc = CanNavigate;

            MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if ( rc )
            {
                MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
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

            // check for next button

            if ( checkBoxYesNoGroup6.rbNo.Checked
                ||
                ( checkBoxYesNoGroup6.rbYes.Checked
                && ( checkBoxYesNoGroup7.rbNo.Checked
                || checkBoxYesNoGroup7.rbYes.Checked )
                )
                )
            {
                if ( runRules() )
                {
                    canNav = true;
                }
            }

            CanNavigate = canNav;

            CheckForSummary();

            return canNav;
        }

        /// <summary>
        /// UpdateView - set the items on the page based on the Domain
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();

            if ( !blnLoaded )
            {
                blnLoaded = true;

                loadStatesCombo();

                if ( Model_Account == null
                    || Model_Account.MedicareSecondaryPayor == null
                    || ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as DisabilityEntitlement ) == null )
                {
                    return;
                }

                if ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( DisabilityEntitlement ) ) )
                {
                    DisabilityEntitlement entitlement = Model_Account.MedicareSecondaryPayor.MedicareEntitlement as DisabilityEntitlement;

                    // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls

                    if ( entitlement.FamilyMemberGHPFlag.Code == YesNoFlag.CODE_YES )
                    {
                        checkBoxYesNoGroup6.rbYes.Checked = true;

                        if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                            as DisabilityEntitlement ).FamilyMemberEmployment != null )
                        {
                            Employment familyMemberEmployment = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                                as DisabilityEntitlement ).FamilyMemberEmployment;

                            displayFamilyMemberEmployment( familyMemberEmployment, true );
                        }
                    }
                    else if ( entitlement.FamilyMemberGHPFlag.Code == YesNoFlag.CODE_NO )
                    {
                        checkBoxYesNoGroup6.rbNo.Checked = true;
                    }

                    if ( entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES )
                    {
                        checkBoxYesNoGroup7.rbYes.Checked = true;
                    }
                    else if ( entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_NO )
                    {
                        checkBoxYesNoGroup7.rbNo.Checked = true;
                    }
                }
            }

            displayInsurance();

            CanPageNavigate();
        }

        /// <summary>
        /// UpdateModel - update the Domain based on the items on the page
        /// </summary>
        public override void UpdateModel()
        {
            base.UpdateModel();

            DisabilityEntitlement entitlement = null;

            if ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement == null )
            {
                entitlement = new DisabilityEntitlement();
            }
            else
            {
                if ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType()
                    != typeof( DisabilityEntitlement ) )
                {
                    entitlement = new DisabilityEntitlement();
                }
                else
                    entitlement = ( DisabilityEntitlement )Model_Account.MedicareSecondaryPayor.MedicareEntitlement;
            }

            if ( checkBoxYesNoGroup6.rbYes.Checked )
            {
                entitlement.FamilyMemberGHPFlag.Code = YesNoFlag.CODE_YES;

                entitlement.FamilyMemberEmployment = new Employment();

                entitlement.FamilyMemberEmployment.Employer.Name = mtbQ7EmployerName.Text;
                Address addr = new Address( mtbQ7Address.Text, string.Empty, mtbQ7City.Text,
                    new ZipCode( mtbQ7ZipCode.Text + mtbQ7ZipExtension.Text ), cmbQ7State.SelectedItem as State,
                    new Country() );

                entitlement.FamilyMemberEmployment.Employer.PartyContactPoint.Address = addr;

                entitlement.FamilyMemberEmployment.Status = EmploymentStatus.NewFullTimeEmployed();

                if ( checkBoxYesNoGroup7.rbYes.Checked )
                {
                    entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_YES;
                }
                else if ( checkBoxYesNoGroup7.rbNo.Checked )
                {
                    entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_NO;
                }
                else
                {
                    entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_BLANK;
                }
            }
            else
            {
                entitlement.FamilyMemberGHPEmploysMoreThanXFlag.Code = YesNoFlag.CODE_BLANK;

                if ( checkBoxYesNoGroup6.rbNo.Checked )
                {
                    entitlement.FamilyMemberGHPFlag.Code = YesNoFlag.CODE_NO;
                    entitlement.FamilyMemberEmployment = new Employment();
                    entitlement.FamilyMemberEmployment.Status = EmploymentStatus.NewNotEmployed();
                }
                else
                {
                    entitlement.FamilyMemberGHPFlag.Code = YesNoFlag.CODE_BLANK;
                    entitlement.FamilyMemberEmployment = new Employment();
                    entitlement.FamilyMemberEmployment.Status = EmploymentStatus.NewNotEmployed();
                }
            }
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {
            MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( Cancel ) );
            MyWizardButtons.AddNavigation( "< &Back", "DisabilityEntitlementPage2" );
            MyWizardButtons.AddNavigation( "&Next >", string.Empty );
            MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );

            MyWizardButtons.SetPanel();
        }

        #endregion

        #region Properties

        public bool DisplayInsurance
        {
            get
            {
                return i_DisplayInsurance;
            }
            private set
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
        /// displayFamiliyMemberEmployment - display the employment info for the employer whose GHP 
        /// the patient is covered under
        /// </summary>
        /// <param name="employment"></param>
        /// <param name="display"></param>
        private void displayFamilyMemberEmployment( Employment employment, bool display )
        {
            if ( display )
            {
                mtbQ7EmployerName.Text = employment.Employer.Name;

                Address employerAddress = employment.Employer.PartyContactPoint.Address;

                mtbQ7Address.Text = employerAddress.Address1;
                mtbQ7City.Text = employerAddress.City;
                cmbQ7State.SelectedItem = employerAddress.State;
                mtbQ7ZipCode.Text = employerAddress.ZipCode.ZipCodePrimary;
                mtbQ7ZipExtension.Text = employerAddress.ZipCode.ZipCodeExtended;
            }
            else
            {
                mtbQ7EmployerName.Text = string.Empty;
                mtbQ7Address.Text = string.Empty;
                mtbQ7City.Text = string.Empty;
                cmbQ7State.SelectedIndex = 0;
                mtbQ7ZipCode.Text = string.Empty;
                mtbQ7ZipExtension.Text = string.Empty;
            }
        }

        /// <summary>
        /// displayInsurance - display primary insurance info for the patient
        /// </summary>
        private void displayInsurance()
        {
            bool display = false;
            DisplayInsurance = false;


            if ( checkBoxYesNoGroup7.rbYes.Checked )
            {
                display = true;
                DisplayInsurance = true;
            }

            WizardPage aPage = MyWizardContainer.GetWizardPage( "DisabilityEntitlementPage2" );

            if ( aPage != null )
            {
                if ( ( ( DisabilityEntitlementPage2 )aPage ).DisplayInsurance )
                {
                    display = true;
                }
            }

            if ( display )
            {
                pnlInsurance.Enabled = true;

                if ( ParentForm != null )
                {
                    Coverage primaryCoverage = ( ( MSP2Dialog )ParentForm ).GetPrimaryCoverage();

                    if ( primaryCoverage != null )
                    {
                        if ( primaryCoverage.Insured != null )
                        {
                            lblPrimaryInsuredText.Text = primaryCoverage.Insured.FormattedName;
                        }

                        if ( primaryCoverage.InsurancePlan != null )
                        {
                            lblPrimaryPayorText.Text = primaryCoverage.InsurancePlan.Payor.Name;
                        }
                    }

                    Coverage secondaryCoverage = ( ( MSP2Dialog )ParentForm ).GetSecondaryCoverage();

                    if ( secondaryCoverage != null )
                    {
                        if ( secondaryCoverage.Insured != null )
                        {
                            lblSecondaryInsuredText.Text = secondaryCoverage.Insured.FormattedName;
                        }

                        if ( secondaryCoverage.InsurancePlan != null )
                        {
                            lblSecondaryPayorText.Text = secondaryCoverage.InsurancePlan.Payor.Name;
                        }
                    }
                }
            }
            else
            {
                pnlInsurance.Enabled = false;
                DisplayInsurance = false;

                lblPrimaryInsuredText.Text = string.Empty;
                lblPrimaryPayorText.Text = string.Empty;

                lblSecondaryInsuredText.Text = string.Empty;
                lblSecondaryPayorText.Text = string.Empty;
            }
        }

        /// <summary>
        /// loadStatesCombo - populate the combo with states, provinces, etc
        /// </summary>
        private void loadStatesCombo()
        {
            if ( cmbQ7State.Items.Count == 0 )
            {
                try
                {
                    IAddressBroker broker = new AddressBrokerProxy();
                    ICollection states = broker.AllStates(User.GetCurrent().Facility.Oid);

                    if ( states == null )
                    {
                        MessageBox.Show( "IAddressBroker.AllStates() returned empty list.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error );
                        return;
                    }

                    foreach ( State state in states )
                    {
                        cmbQ7State.Items.Add( state );
                    }
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "IAddressBroker failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }

        /// <summary>
        /// runRules - evaluate any requirements (fields entered, questions answered, etc) for this page;
        /// determines if the page can navigate
        /// </summary>
        /// <returns></returns>
        private bool runRules()
        {
            bool blnRC = true;

            UIColors.SetNormalBgColor( mtbQ7EmployerName );
            UIColors.SetNormalBgColor( mtbQ7Address );
            UIColors.SetNormalBgColor( mtbQ7City );
            UIColors.SetNormalBgColor( cmbQ7State );
            UIColors.SetNormalBgColor( mtbQ7ZipCode );

            Refresh();

            if ( !checkBoxYesNoGroup6.rbYes.Checked )
            {
                return true;
            }

            if ( mtbQ7EmployerName.Enabled
                && mtbQ7EmployerName.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ7EmployerName );
                blnRC = false;
            }

            if ( mtbQ7Address.Enabled
                && mtbQ7Address.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ7Address );
                blnRC = false;
            }

            if ( mtbQ7City.Enabled
                && mtbQ7City.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ7City );
                blnRC = false;
            }

            if ( cmbQ7State.Enabled
                && ( cmbQ7State.SelectedItem == null ||
                cmbQ7State.SelectedIndex < 1 ) )
            {
                UIColors.SetRequiredBgColor( cmbQ7State );
                blnRC = false;
            }

            if ( mtbQ7ZipCode.Enabled )
            {
                if ( mtbQ7ZipCode.UnMaskedText.Trim() == string.Empty )
                {
                    UIColors.SetRequiredBgColor( mtbQ7ZipCode );
                    blnRC = false;

                }
                else if ( mtbQ7ZipCode.UnMaskedText.Trim().Length != 5 )
                {
                    UIColors.SetErrorBgColor( mtbQ7ZipCode );
                    MessageBox.Show( "The primary zip code must be 5 digits.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbQ7ZipCode.Focus();
                    blnRC = false;
                }
            }

            return blnRC;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployerName( mtbQ7EmployerName );
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mtbQ7Address );
            MaskedEditTextBoxBuilder.ConfigureAddressCity( mtbQ7City );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mtbQ7ZipCode );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mtbQ7ZipExtension );
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQuestion7 = new System.Windows.Forms.Panel();
            this.lblQuestion7c = new System.Windows.Forms.Label();
            this.lblQuestion7b = new System.Windows.Forms.Label();
            this.checkBoxYesNoGroup7 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.pnlDividerQ7 = new System.Windows.Forms.Panel();
            this.lblQuestion7 = new System.Windows.Forms.Label();
            this.pnlQuestion7c = new System.Windows.Forms.Panel();
            this.gbQ7OtherMemberEmployer = new System.Windows.Forms.GroupBox();
            this.cmbQ7State = new System.Windows.Forms.ComboBox();
            this.mtbQ7ZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbQ7EmployerName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ7State = new System.Windows.Forms.Label();
            this.lblQ7City = new System.Windows.Forms.Label();
            this.mtbQ7City = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ7Zip = new System.Windows.Forms.Label();
            this.lblQ7Hyphen = new System.Windows.Forms.Label();
            this.lblQ7Address = new System.Windows.Forms.Label();
            this.mtbQ7ZipExtension = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ7EmployerName = new System.Windows.Forms.Label();
            this.mtbQ7Address = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.pnlQuestion6 = new System.Windows.Forms.Panel();
            this.pnlDividerQ6 = new System.Windows.Forms.Panel();
            this.checkBoxYesNoGroup6 = new PatientAccess.UI.InsuranceViews.MSP2.CheckBoxYesNoGroup();
            this.lblQuestion6 = new System.Windows.Forms.Label();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
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
            this.pnlQuestion7.SuspendLayout();
            this.pnlQuestion7c.SuspendLayout();
            this.gbQ7OtherMemberEmployer.SuspendLayout();
            this.pnlQuestion6.SuspendLayout();
            this.pnlInsurance.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add( this.pnlInsurance );
            this.pnlWizardPageBody.Controls.Add( this.pnlDivider1 );
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion6 );
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion7 );
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion7, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion6, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlDivider1, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlInsurance, 0 );
            // 
            // pnlQuestion7
            // 
            this.pnlQuestion7.Controls.Add( this.lblQuestion7c );
            this.pnlQuestion7.Controls.Add( this.lblQuestion7b );
            this.pnlQuestion7.Controls.Add( this.checkBoxYesNoGroup7 );
            this.pnlQuestion7.Controls.Add( this.pnlDividerQ7 );
            this.pnlQuestion7.Controls.Add( this.lblQuestion7 );
            this.pnlQuestion7.Controls.Add( this.pnlQuestion7c );
            this.pnlQuestion7.Location = new System.Drawing.Point( 8, 124 );
            this.pnlQuestion7.Name = "pnlQuestion7";
            this.pnlQuestion7.Size = new System.Drawing.Size( 667, 233 );
            this.pnlQuestion7.TabIndex = 2;
            // 
            // lblQuestion7c
            // 
            this.lblQuestion7c.Location = new System.Drawing.Point( 19, 40 );
            this.lblQuestion7c.Name = "lblQuestion7c";
            this.lblQuestion7c.Size = new System.Drawing.Size( 483, 18 );
            this.lblQuestion7c.TabIndex = 0;
            this.lblQuestion7c.Text = "employ 100 or more employees?";
            // 
            // lblQuestion7b
            // 
            this.lblQuestion7b.Location = new System.Drawing.Point( 19, 24 );
            this.lblQuestion7b.Name = "lblQuestion7b";
            this.lblQuestion7b.Size = new System.Drawing.Size( 483, 18 );
            this.lblQuestion7b.TabIndex = 0;
            this.lblQuestion7b.Text = "does your family member\'s employer, that sponsors or contributes to the GHP, ";
            // 
            // checkBoxYesNoGroup7
            // 
            this.checkBoxYesNoGroup7.Location = new System.Drawing.Point( 536, 0 );
            this.checkBoxYesNoGroup7.Name = "checkBoxYesNoGroup7";
            this.checkBoxYesNoGroup7.Size = new System.Drawing.Size( 131, 35 );
            this.checkBoxYesNoGroup7.TabIndex = 1;
            this.checkBoxYesNoGroup7.RadioChanged += new System.EventHandler( this.checkBoxYesNoGroup7_RadioChanged );
            // 
            // pnlDividerQ7
            // 
            this.pnlDividerQ7.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ7.Location = new System.Drawing.Point( 5, 229 );
            this.pnlDividerQ7.Name = "pnlDividerQ7";
            this.pnlDividerQ7.Size = new System.Drawing.Size( 656, 1 );
            this.pnlDividerQ7.TabIndex = 0;
            // 
            // lblQuestion7
            // 
            this.lblQuestion7.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion7.Name = "lblQuestion7";
            this.lblQuestion7.Size = new System.Drawing.Size( 483, 18 );
            this.lblQuestion7.TabIndex = 0;
            this.lblQuestion7.Text = "7. If you have GHP coverage based on a family member\'s current employment,";
            // 
            // pnlQuestion7c
            // 
            this.pnlQuestion7c.Controls.Add( this.gbQ7OtherMemberEmployer );
            this.pnlQuestion7c.Location = new System.Drawing.Point( 4, 71 );
            this.pnlQuestion7c.Name = "pnlQuestion7c";
            this.pnlQuestion7c.Size = new System.Drawing.Size( 429, 141 );
            this.pnlQuestion7c.TabIndex = 2;
            // 
            // gbQ7OtherMemberEmployer
            // 
            this.gbQ7OtherMemberEmployer.Controls.Add( this.cmbQ7State );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.mtbQ7ZipCode );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.mtbQ7EmployerName );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7State );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7City );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.mtbQ7City );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7Zip );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7Hyphen );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7Address );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.mtbQ7ZipExtension );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.lblQ7EmployerName );
            this.gbQ7OtherMemberEmployer.Controls.Add( this.mtbQ7Address );
            this.gbQ7OtherMemberEmployer.Location = new System.Drawing.Point( 9, 9 );
            this.gbQ7OtherMemberEmployer.Name = "gbQ7OtherMemberEmployer";
            this.gbQ7OtherMemberEmployer.Size = new System.Drawing.Size( 412, 124 );
            this.gbQ7OtherMemberEmployer.TabIndex = 3;
            this.gbQ7OtherMemberEmployer.TabStop = false;
            this.gbQ7OtherMemberEmployer.Text = "Family member\'s employer";
            // 
            // cmbQ7State
            // 
            this.cmbQ7State.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQ7State.Location = new System.Drawing.Point( 47, 96 );
            this.cmbQ7State.Name = "cmbQ7State";
            this.cmbQ7State.Size = new System.Drawing.Size( 165, 21 );
            this.cmbQ7State.TabIndex = 4;
            this.cmbQ7State.SelectedIndexChanged += new System.EventHandler( this.runRules );
            // 
            // mtbQ7ZipCode
            // 
            this.mtbQ7ZipCode.BackColor = System.Drawing.Color.White;
            this.mtbQ7ZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ7ZipCode.Location = new System.Drawing.Point( 280, 96 );
            this.mtbQ7ZipCode.Mask = string.Empty;
            this.mtbQ7ZipCode.MaxLength = 5;
            this.mtbQ7ZipCode.Name = "mtbQ7ZipCode";
            this.mtbQ7ZipCode.Size = new System.Drawing.Size( 38, 20 );
            this.mtbQ7ZipCode.TabIndex = 5;
            this.mtbQ7ZipCode.Leave += new System.EventHandler( this.runRules );
            this.mtbQ7ZipCode.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // mtbQ7EmployerName
            // 
            this.mtbQ7EmployerName.BackColor = System.Drawing.Color.White;
            this.mtbQ7EmployerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ7EmployerName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ7EmployerName.Location = new System.Drawing.Point( 107, 15 );
            this.mtbQ7EmployerName.Mask = string.Empty;
            this.mtbQ7EmployerName.MaxLength = Employer.PBAR_EMP_NAME_LENGTH;
            this.mtbQ7EmployerName.Name = "mtbQ7EmployerName";
            this.mtbQ7EmployerName.Size = new System.Drawing.Size( 295, 20 );
            this.mtbQ7EmployerName.TabIndex = 1;
            this.mtbQ7EmployerName.Leave += new System.EventHandler( this.runRules );
            this.mtbQ7EmployerName.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // lblQ7State
            // 
            this.lblQ7State.Location = new System.Drawing.Point( 9, 98 );
            this.lblQ7State.Name = "lblQ7State";
            this.lblQ7State.Size = new System.Drawing.Size( 40, 16 );
            this.lblQ7State.TabIndex = 0;
            this.lblQ7State.Text = "State:";
            // 
            // lblQ7City
            // 
            this.lblQ7City.Location = new System.Drawing.Point( 8, 70 );
            this.lblQ7City.Name = "lblQ7City";
            this.lblQ7City.Size = new System.Drawing.Size( 32, 17 );
            this.lblQ7City.TabIndex = 0;
            this.lblQ7City.Text = "City:";
            // 
            // mtbQ7City
            // 
            this.mtbQ7City.BackColor = System.Drawing.Color.White;
            this.mtbQ7City.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ7City.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ7City.Location = new System.Drawing.Point( 107, 67 );
            this.mtbQ7City.Mask = string.Empty;
            this.mtbQ7City.MaxLength = 17;
            this.mtbQ7City.Name = "mtbQ7City";
            this.mtbQ7City.Size = new System.Drawing.Size( 130, 20 );
            this.mtbQ7City.TabIndex = 3;
            this.mtbQ7City.Leave += new System.EventHandler( this.runRules );
            this.mtbQ7City.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // lblQ7Zip
            // 
            this.lblQ7Zip.Location = new System.Drawing.Point( 224, 98 );
            this.lblQ7Zip.Name = "lblQ7Zip";
            this.lblQ7Zip.Size = new System.Drawing.Size( 56, 23 );
            this.lblQ7Zip.TabIndex = 0;
            this.lblQ7Zip.Text = "Zip Code:";
            // 
            // lblQ7Hyphen
            // 
            this.lblQ7Hyphen.Location = new System.Drawing.Point( 323, 98 );
            this.lblQ7Hyphen.Name = "lblQ7Hyphen";
            this.lblQ7Hyphen.Size = new System.Drawing.Size( 8, 23 );
            this.lblQ7Hyphen.TabIndex = 0;
            this.lblQ7Hyphen.Text = "-";
            // 
            // lblQ7Address
            // 
            this.lblQ7Address.Location = new System.Drawing.Point( 8, 44 );
            this.lblQ7Address.Name = "lblQ7Address";
            this.lblQ7Address.Size = new System.Drawing.Size( 53, 17 );
            this.lblQ7Address.TabIndex = 0;
            this.lblQ7Address.Text = "Address:";
            // 
            // mtbQ7ZipExtension
            // 
            this.mtbQ7ZipExtension.BackColor = System.Drawing.Color.White;
            this.mtbQ7ZipExtension.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ7ZipExtension.Location = new System.Drawing.Point( 336, 96 );
            this.mtbQ7ZipExtension.Mask = string.Empty;
            this.mtbQ7ZipExtension.MaxLength = 4;
            this.mtbQ7ZipExtension.Name = "mtbQ7ZipExtension";
            this.mtbQ7ZipExtension.Size = new System.Drawing.Size( 32, 20 );
            this.mtbQ7ZipExtension.TabIndex = 6;
            // 
            // lblQ7EmployerName
            // 
            this.lblQ7EmployerName.Location = new System.Drawing.Point( 8, 18 );
            this.lblQ7EmployerName.Name = "lblQ7EmployerName";
            this.lblQ7EmployerName.Size = new System.Drawing.Size( 87, 23 );
            this.lblQ7EmployerName.TabIndex = 0;
            this.lblQ7EmployerName.Text = "Employer name:";
            // 
            // mtbQ7Address
            // 
            this.mtbQ7Address.BackColor = System.Drawing.Color.White;
            this.mtbQ7Address.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ7Address.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ7Address.Location = new System.Drawing.Point( 107, 41 );
            this.mtbQ7Address.Mask = string.Empty;
            this.mtbQ7Address.MaxLength = 25;
            this.mtbQ7Address.Name = "mtbQ7Address";
            this.mtbQ7Address.Size = new System.Drawing.Size( 188, 20 );
            this.mtbQ7Address.TabIndex = 2;
            this.mtbQ7Address.Leave += new System.EventHandler( this.runRules );
            this.mtbQ7Address.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // pnlQuestion6
            // 
            this.pnlQuestion6.Controls.Add( this.pnlDividerQ6 );
            this.pnlQuestion6.Controls.Add( this.checkBoxYesNoGroup6 );
            this.pnlQuestion6.Controls.Add( this.lblQuestion6 );
            this.pnlQuestion6.Location = new System.Drawing.Point( 8, 63 );
            this.pnlQuestion6.Name = "pnlQuestion6";
            this.pnlQuestion6.Size = new System.Drawing.Size( 667, 53 );
            this.pnlQuestion6.TabIndex = 1;
            // 
            // pnlDividerQ6
            // 
            this.pnlDividerQ6.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ6.Location = new System.Drawing.Point( 9, 47 );
            this.pnlDividerQ6.Name = "pnlDividerQ6";
            this.pnlDividerQ6.Size = new System.Drawing.Size( 656, 1 );
            this.pnlDividerQ6.TabIndex = 0;
            // 
            // checkBoxYesNoGroup6
            // 
            this.checkBoxYesNoGroup6.Location = new System.Drawing.Point( 536, -1 );
            this.checkBoxYesNoGroup6.Name = "checkBoxYesNoGroup6";
            this.checkBoxYesNoGroup6.Size = new System.Drawing.Size( 131, 35 );
            this.checkBoxYesNoGroup6.TabIndex = 1;
            this.checkBoxYesNoGroup6.RadioChanged += new System.EventHandler( this.checkBoxYesNoGroup6_RadioChanged );
            // 
            // lblQuestion6
            // 
            this.lblQuestion6.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion6.Name = "lblQuestion6";
            this.lblQuestion6.Size = new System.Drawing.Size( 421, 13 );
            this.lblQuestion6.TabIndex = 0;
            this.lblQuestion6.Text = "6.  Are you covered under the GHP of a family member other than your spouse?";
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point( 7, 55 );
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size( 670, 2 );
            this.pnlDivider1.TabIndex = 0;
            // 
            // pnlInsurance
            // 
            this.pnlInsurance.Controls.Add( this.btnEditInsurance );
            this.pnlInsurance.Controls.Add( this.lblSecondaryInsuredText );
            this.pnlInsurance.Controls.Add( this.lblSecondaryPayorText );
            this.pnlInsurance.Controls.Add( this.lblPrimaryInsuredText );
            this.pnlInsurance.Controls.Add( this.lblPrimaryPayorText );
            this.pnlInsurance.Controls.Add( this.lblSecondaryInsured );
            this.pnlInsurance.Controls.Add( this.lblSecondaryPayor );
            this.pnlInsurance.Controls.Add( this.lblPrimaryInsured );
            this.pnlInsurance.Controls.Add( this.lblPrimaryPayor );
            this.pnlInsurance.Controls.Add( this.pnlInsuranceDivider );
            this.pnlInsurance.Location = new System.Drawing.Point( 7, 432 );
            this.pnlInsurance.Name = "pnlInsurance";
            this.pnlInsurance.Size = new System.Drawing.Size( 667, 117 );
            this.pnlInsurance.TabIndex = 9;
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point( 465, 25 );
            this.btnEditInsurance.Message = null;
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size( 180, 23 );
            this.btnEditInsurance.TabIndex = 13;
            this.btnEditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler( this.btnEditInsurance_Click );
            // 
            // lblSecondaryInsuredText
            // 
            this.lblSecondaryInsuredText.Location = new System.Drawing.Point( 104, 92 );
            this.lblSecondaryInsuredText.Name = "lblSecondaryInsuredText";
            this.lblSecondaryInsuredText.Size = new System.Drawing.Size( 344, 13 );
            this.lblSecondaryInsuredText.TabIndex = 0;
            // 
            // lblSecondaryPayorText
            // 
            this.lblSecondaryPayorText.Location = new System.Drawing.Point( 104, 73 );
            this.lblSecondaryPayorText.Name = "lblSecondaryPayorText";
            this.lblSecondaryPayorText.Size = new System.Drawing.Size( 344, 13 );
            this.lblSecondaryPayorText.TabIndex = 0;
            // 
            // lblPrimaryInsuredText
            // 
            this.lblPrimaryInsuredText.Location = new System.Drawing.Point( 104, 44 );
            this.lblPrimaryInsuredText.Name = "lblPrimaryInsuredText";
            this.lblPrimaryInsuredText.Size = new System.Drawing.Size( 344, 13 );
            this.lblPrimaryInsuredText.TabIndex = 0;
            // 
            // lblPrimaryPayorText
            // 
            this.lblPrimaryPayorText.Location = new System.Drawing.Point( 104, 26 );
            this.lblPrimaryPayorText.Name = "lblPrimaryPayorText";
            this.lblPrimaryPayorText.Size = new System.Drawing.Size( 344, 13 );
            this.lblPrimaryPayorText.TabIndex = 0;
            // 
            // lblSecondaryInsured
            // 
            this.lblSecondaryInsured.Location = new System.Drawing.Point( 56, 92 );
            this.lblSecondaryInsured.Name = "lblSecondaryInsured";
            this.lblSecondaryInsured.Size = new System.Drawing.Size( 45, 13 );
            this.lblSecondaryInsured.TabIndex = 0;
            this.lblSecondaryInsured.Text = "Insured:";
            // 
            // lblSecondaryPayor
            // 
            this.lblSecondaryPayor.Location = new System.Drawing.Point( 10, 73 );
            this.lblSecondaryPayor.Name = "lblSecondaryPayor";
            this.lblSecondaryPayor.Size = new System.Drawing.Size( 95, 13 );
            this.lblSecondaryPayor.TabIndex = 0;
            this.lblSecondaryPayor.Text = "Secondary Payor:";
            // 
            // lblPrimaryInsured
            // 
            this.lblPrimaryInsured.Location = new System.Drawing.Point( 56, 44 );
            this.lblPrimaryInsured.Name = "lblPrimaryInsured";
            this.lblPrimaryInsured.Size = new System.Drawing.Size( 45, 13 );
            this.lblPrimaryInsured.TabIndex = 0;
            this.lblPrimaryInsured.Text = "Insured:";
            // 
            // lblPrimaryPayor
            // 
            this.lblPrimaryPayor.Location = new System.Drawing.Point( 26, 26 );
            this.lblPrimaryPayor.Name = "lblPrimaryPayor";
            this.lblPrimaryPayor.Size = new System.Drawing.Size( 81, 13 );
            this.lblPrimaryPayor.TabIndex = 0;
            this.lblPrimaryPayor.Text = "Primary Payor:";
            // 
            // pnlInsuranceDivider
            // 
            this.pnlInsuranceDivider.BackColor = System.Drawing.Color.Black;
            this.pnlInsuranceDivider.Location = new System.Drawing.Point( 5, 11 );
            this.pnlInsuranceDivider.Name = "pnlInsuranceDivider";
            this.pnlInsuranceDivider.Size = new System.Drawing.Size( 656, 2 );
            this.pnlInsuranceDivider.TabIndex = 0;
            // 
            // DisabilityEntitlementPage3
            // 
            this.Name = "DisabilityEntitlementPage3";
            this.EnabledChanged += new System.EventHandler( this.DisabilityEntitlementPage3_EnabledChanged );
            this.Load += new System.EventHandler( this.DisabilityEntitlementPage3_Load );
            this.pnlWizardPageBody.ResumeLayout( false );
            this.pnlQuestion7.ResumeLayout( false );
            this.pnlQuestion7c.ResumeLayout( false );
            this.gbQ7OtherMemberEmployer.ResumeLayout( false );
            this.pnlQuestion6.ResumeLayout( false );
            this.pnlInsurance.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage3()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            // TODO: Add any initialization after the InitializeComponent call
        }

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage3( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
        }

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage3( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
        }

        public DisabilityEntitlementPage3( string pageName, WizardContainer wizardContainer, Account anAccount )
            : base( pageName, wizardContainer, anAccount )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

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
        private Panel pnlQuestion6;
        private Panel pnlDividerQ6;
        private Panel pnlQuestion7;
        private Panel pnlQuestion7c;
        private Panel pnlDividerQ7;
        private Panel pnlInsurance;
        private Panel pnlInsuranceDivider;

        private GroupBox gbQ7OtherMemberEmployer;

        private ComboBox cmbQ7State;

        private MaskedEditTextBox mtbQ7EmployerName;
        private MaskedEditTextBox mtbQ7Address;
        private MaskedEditTextBox mtbQ7City;
        private MaskedEditTextBox mtbQ7ZipCode;
        private MaskedEditTextBox mtbQ7ZipExtension;

        private Label lblQuestion6;
        private Label lblQuestion7;
        private Label lblQuestion7b;
        private Label lblQuestion7c;
        private Label lblQ7EmployerName;
        private Label lblQ7Address;
        private Label lblQ7City;
        private Label lblQ7State;
        private Label lblQ7Zip;
        private Label lblQ7Hyphen;
        private Label lblSecondaryInsuredText;
        private Label lblSecondaryPayorText;
        private Label lblPrimaryInsuredText;
        private Label lblPrimaryPayorText;
        private Label lblSecondaryInsured;
        private Label lblSecondaryPayor;
        private Label lblPrimaryInsured;
        private Label lblPrimaryPayor;

        private CheckBoxYesNoGroup checkBoxYesNoGroup6;
        private CheckBoxYesNoGroup checkBoxYesNoGroup7;

        private LoggingButton btnEditInsurance;

        private bool i_DisplayInsurance;
        private bool blnLoaded;

        #endregion


        #region Constants
        #endregion
    }
}

