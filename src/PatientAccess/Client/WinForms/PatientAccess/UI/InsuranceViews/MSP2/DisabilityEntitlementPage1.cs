using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.CommonControls.Wizard;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.MSP2
{
    /// <summary>
    /// DisabilityEntitlementPage1 - the first Entitlement by disability page; captures employment info
    /// for the patient and spouse, if applicable
    /// </summary>

    [Serializable]
    public class DisabilityEntitlementPage1 : WizardPage, IDisabilityEntitlementPage1View
    {
        #region Events

        public event EventHandler MSPCancelled;

        #endregion

        #region Event Handlers

        /// <summary>
        /// btnQ2MoreInfo_Click - display the custom 'messagebox' with more info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnQ2MoreInfo_Click( object sender, EventArgs e )
        {
            CloseMessageBox2 box = new CloseMessageBox2( "If the patient is not married, choose No - (Spouse) Never employed." );

            box.Show();
        }

        /// <summary>
        /// call the common runRules method to determine if items should be shaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runRules( object sender, EventArgs e )
        {
            bool blnRC = runRules();

            if ( blnRC )
            {
                CanPageNavigate();
            }
        }

        /// <summary>
        /// btnQ1EditEmployment_Click - the user has opted to abandon the MSP wizard and edit the employment
        /// info for the patient.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnQ1EditEmployment_Click( object sender, EventArgs e )
        {
            if ( ParentForm != null )
                ( ( MSP2Dialog )ParentForm ).RaiseTabSelectedEvent( AccountView.EMPLOYMENT );
        }

        /// <summary>
        /// mtbQ1DateOfRetirement_Leave - check the date format 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ1DateOfRetirement_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            checkDate( mtb );
        }

        private void checkDate( MaskedEditTextBox mtb )
        {
            UIColors.SetNormalBgColor( mtb );
            Refresh();

            if ( mtb.UnMaskedText != string.Empty
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

            if ( mtb.UnMaskedText != string.Empty
                 && mtb.Text.Length != 0 )
            {

                try
                {   // Check the date entered is not in the future
                    DateTime benefitsDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                                                          Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                                                          Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                    if ( !DateValidator.IsValidDate( benefitsDate ) )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                                         MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                         MessageBoxDefaultButton.Button1 );
                        return;
                    }

                    if ( DateValidator.IsFutureDate( benefitsDate ) )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DATE_CANNOT_BE_FUTURE_DATE, "Error",
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

            CanPageNavigate();
        }

        /// <summary>
        /// mtbQ2DateOfRetirement_Leave - check the date format 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mtbQ2DateOfRetirement_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            checkDate( mtb );
        }

        /// <summary>
        /// rbQ1Yes_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ1Yes_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion1b.Enabled = false;
            pnlQuestion1c.Enabled = true;

            mtbQ1DateOfRetirement.UnMaskedText = string.Empty;

            displayPatientEmployment( true );

            CanPageNavigate();
        }

        /// <summary>
        /// rbQ1NoRetired_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ1NoRetired_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion1b.Enabled = true;
            pnlQuestion1c.Enabled = false;

            CanPageNavigate();
        }

        /// <summary>
        /// rbQ1NoNever_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ1NoNever_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion1b.Enabled = false;
            pnlQuestion1c.Enabled = false;

            mtbQ1DateOfRetirement.UnMaskedText = string.Empty;

            CanPageNavigate();
        }

        /// <summary>
        /// rbQ2Yes_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ2Yes_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion2b.Enabled = false;
            pnlQuestion2c.Enabled = true;

            mtbQ2DateOfRetirement.UnMaskedText = string.Empty;
            mtbQ2EmployerName.UnMaskedText = string.Empty;
            mtbQ2Address.UnMaskedText = string.Empty;
            mtbQ2City.UnMaskedText = string.Empty;
            mtbQ2ZipCode.UnMaskedText = string.Empty;
            cmbQ2State.SelectedIndex = 0;

            CanPageNavigate();
        }

        /// <summary>
        /// rbQ2NoRetired_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ2NoRetired_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion2b.Enabled = true;
            pnlQuestion2c.Enabled = false;

            mtbQ2EmployerName.UnMaskedText = string.Empty;
            mtbQ2Address.UnMaskedText = string.Empty;
            mtbQ2City.UnMaskedText = string.Empty;
            mtbQ2ZipCode.UnMaskedText = string.Empty;

            CanPageNavigate();
        }

        /// <summary>
        /// rbQ2NoNever_CheckedChanged - appropriately enable/disable other questions; determine if we can
        /// navigate from this page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbQ2NoNever_CheckedChanged( object sender, EventArgs e )
        {
            pnlQuestion2b.Enabled = false;
            pnlQuestion2c.Enabled = false;

            mtbQ2DateOfRetirement.UnMaskedText = string.Empty;
            mtbQ2EmployerName.UnMaskedText = string.Empty;
            mtbQ2Address.UnMaskedText = string.Empty;
            mtbQ2City.UnMaskedText = string.Empty;
            mtbQ2ZipCode.UnMaskedText = string.Empty;

            CanPageNavigate();
        }

        /// <summary>
        /// pnlQ1RadioButtons_GotFocus - default the radio button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlQ1RadioButtons_GotFocus( object sender, EventArgs e )
        {
            if ( !rbQ1Yes.Checked
                && !rbQ1NoRetired.Checked
                && !rbQ1NoNever.Checked )
            {
                rbQ1Yes.Checked = true;
            }
        }

        /// <summary>
        /// pnlQ2RadioButtons_GotFocus - default the radio button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlQ2RadioButtons_GotFocus( object sender, EventArgs e )
        {
            if ( !rbQ2Yes.Checked
                && !rbQ2NoRetired.Checked
                && !rbQ2NoNever.Checked )
            {
                rbQ2Yes.Checked = true;
            }
        }

        /// <summary>
        /// DisabilityEntitlementPage1_EnabledChanged - invoke UpdateView if the page is enabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisabilityEntitlementPage1_EnabledChanged( object sender, EventArgs e )
        {
            if ( Enabled )
            {
                UpdateView();
            }
        }

        /// <summary>
        /// DisabilityEntitlementPage1_Load - load up the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisabilityEntitlementPage1_Load( object sender, EventArgs e )
        {
            LinkName = "Entitlement by Disability";
            MyWizardMessages.Message1 = "Entitlement by Disability";
            MyWizardMessages.TextFont1 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize1 = 8.25;
            MyWizardMessages.FontStyle1 = FontStyle.Bold;

            MyWizardMessages.Message2 = "";

            MyWizardMessages.TextFont2 = "Microsoft Sans Serif";
            MyWizardMessages.TextSize2 = 8.25;

            MyWizardMessages.ShowMessages();

            pnlQuestion1b.Enabled = false;
            pnlQuestion1c.Enabled = false;

            pnlQuestion2b.Enabled = false;
            pnlQuestion2c.Enabled = false;
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

        public void  DisablePage()
        {
            Enabled = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// ResetPage - set the page back to an un-initialized state
        /// </summary>
        public override void ResetPage()
        {
            base.ResetPage();

            rbQ1NoNever.Checked = false;
            rbQ1NoRetired.Checked = false;
            rbQ1Yes.Checked = false;

            rbQ2NoNever.Checked = false;
            rbQ2NoRetired.Checked = false;
            rbQ2Yes.Checked = false;

            cmbQ2State.SelectedIndex = -1;

            mtbQ1DateOfRetirement.UnMaskedText = string.Empty;
            mtbQ2Address.UnMaskedText = string.Empty;
            mtbQ2City.UnMaskedText = string.Empty;
            mtbQ2DateOfRetirement.UnMaskedText = string.Empty;
            mtbQ2EmployerName.UnMaskedText = string.Empty;
            mtbQ2ZipCode.UnMaskedText = string.Empty;
            mtbQ2ZipExtension.UnMaskedText = string.Empty;

            pnlQuestion1b.Enabled = false;
            pnlQuestion1c.Enabled = false;

            pnlQuestion2b.Enabled = false;
            pnlQuestion2c.Enabled = false;

            UIColors.SetNormalBgColor( cmbQ2State );
            UIColors.SetNormalBgColor( mtbQ1DateOfRetirement );
            UIColors.SetNormalBgColor( mtbQ2Address );
            UIColors.SetNormalBgColor( mtbQ2City );
            UIColors.SetNormalBgColor( mtbQ2DateOfRetirement );
            UIColors.SetNormalBgColor( mtbQ2EmployerName );
            UIColors.SetNormalBgColor( mtbQ2ZipCode );
            UIColors.SetNormalBgColor( mtbQ2ZipExtension );
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

            if (
                (
                rbQ1NoNever.Checked
                || rbQ1Yes.Checked
                || ( rbQ1NoRetired.Checked
                && mtbQ1DateOfRetirement.UnMaskedText.Trim() != string.Empty )
                )
                &&
                (
                rbQ2NoNever.Checked
                || rbQ2Yes.Checked
                || ( rbQ2NoRetired.Checked && mtbQ2DateOfRetirement.UnMaskedText.Trim() != string.Empty )

                )
                )
            {
                if ( runRules() )
                {
                    canNav = true;
                }
            }
            else
            {
                runRules();
            }

            CanNavigate = canNav;

            if ( canNav )
            {
                MyWizardButtons.UpdateNavigation( "&Next >", "DisabilityEntitlementPage2" );
                MyWizardButtons.SetAcceptButton( "&Next >" );
            }

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
                    // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls

                    if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as DisabilityEntitlement ).PatientEmployment != null )
                    {
                        EmploymentStatus patientEmpStatus = ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                            as DisabilityEntitlement ).PatientEmployment ).Status;

                        if ( patientEmpStatus != null )
                        {
                            displayPatientEmployment( true );

                            if ( patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                                patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                            {
                                rbQ1Yes.Checked = true;
                            }
                            else if ( patientEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                            {
                                DateTime date = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                    DisabilityEntitlement ).PatientEmployment.RetiredDate;

                                if ( date != DateTime.MinValue )
                                {
                                    mtbQ1DateOfRetirement.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                                }

                                rbQ1NoRetired.Checked = true;
                            }
                            else if ( patientEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                            {
                                rbQ1NoNever.Checked = true;
                            }
                        }
                    }

                    if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                        as DisabilityEntitlement ).SpouseEmployment != null )
                    {
                        Employment spouseEmployment = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                            as DisabilityEntitlement ).SpouseEmployment;

                        EmploymentStatus spouseEmpStatus = spouseEmployment.Status;

                        if ( spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                            spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                        {
                            rbQ2Yes.Checked = true;

                            displaySpouseEmployment( spouseEmployment, true );
                        }
                        else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                        {
                            DateTime date = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                DisabilityEntitlement ).SpouseEmployment.RetiredDate;

                            if ( date != DateTime.MinValue )
                            {
                                mtbQ2DateOfRetirement.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                            }

                            rbQ2NoRetired.Checked = true;
                        }
                        else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                        {
                            rbQ2NoNever.Checked = true;
                        }
                    }
                }
            }


            // if question 1 has not yet been answered, check occurrence codes to see if the patient is retired

            if ( !rbQ1NoNever.Checked
                && !rbQ1NoRetired.Checked
                && !rbQ1Yes.Checked )
            {
                if ( Model_Account.OccurrenceCodes != null
                    && Model_Account.OccurrenceCodes.Count > 0 )
                {
                    foreach ( OccurrenceCode oc in Model_Account.OccurrenceCodes )
                    {
                        if ( oc.Code == OccurrenceCode.OCCURRENCECODE_RETIREDATE )
                        {
                            rbQ1NoRetired.Checked = true;
                            mtbQ1DateOfRetirement.UnMaskedText = oc.OccurrenceDate.ToString( "MMddyyyy" );
                            UIColors.SetNormalBgColor( mtbQ1DateOfRetirement );
                        }
                    }
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

            if ( rbQ1Yes.Checked )
            {
                entitlement.PatientEmployment = ( Employment )Model_Account.Patient.Employment.DeepCopy();
                entitlement.PatientEmployment.Status = EmploymentStatus.NewFullTimeEmployed();
            }
            else
            {
                entitlement.PatientEmployment = new Employment();

                if ( rbQ1NoRetired.Checked )
                {
                    entitlement.PatientEmployment.Status = EmploymentStatus.NewRetired();
                    entitlement.PatientEmployment.RetiredDate = DateTime.Parse( mtbQ1DateOfRetirement.Text );
                }
                else if ( rbQ1NoNever.Checked )
                {
                    entitlement.PatientEmployment.Status = EmploymentStatus.NewNotEmployed();
                }
            }

            entitlement.SpouseEmployment = new Employment();

            if ( rbQ2Yes.Checked )
            {
                entitlement.SpouseEmployment.Employer.Name = mtbQ2EmployerName.Text;
                Address addr = new Address( mtbQ2Address.Text, string.Empty, mtbQ2City.Text,
                    new ZipCode( mtbQ2ZipCode.Text + mtbQ2ZipExtension.Text ), cmbQ2State.SelectedItem as State,
                    new Country() );

                entitlement.SpouseEmployment.Employer.PartyContactPoint.Address = addr;
                entitlement.SpouseEmployment.Status = EmploymentStatus.NewFullTimeEmployed();
            }
            else
            {
                if ( rbQ2NoRetired.Checked )
                {
                    entitlement.SpouseEmployment.Status = EmploymentStatus.NewRetired();
                    entitlement.SpouseEmployment.RetiredDate = DateTime.Parse( mtbQ2DateOfRetirement.Text );
                }
                else if ( rbQ2NoNever.Checked )
                {
                    entitlement.SpouseEmployment.Status = EmploymentStatus.NewNotEmployed();
                }
            }

            Model_Account.MedicareSecondaryPayor.MedicareEntitlement = entitlement;
        }

        /// <summary>
        /// AddButtons - add the buttons and default links for this page
        /// </summary>
        public void AddButtons()
        {
            MyWizardButtons.AddNavigation( "Cancel", new FunctionDelegate( Cancel ) );
            MyWizardButtons.AddNavigation( "< &Back", "MedicareEntitlementPage" );
            MyWizardButtons.AddNavigation( "&Next >", string.Empty );
            MyWizardButtons.AddNavigation( "&Continue to Summary", string.Empty );

            MyWizardButtons.SetPanel();
        }

        /// <summary>
        /// CheckForSummary - determine if the Summary button can be enabled
        /// </summary>
        /// <returns></returns>

        public bool CheckForSummary()
        {
            bool rc = CanNavigate;

            MyWizardButtons.UpdateNavigation( "&Continue to Summary", string.Empty );

            if ( rc )
            {
                DisabilityEntitlementPage2 page2 = MyWizardContainer.GetWizardPage( "DisabilityEntitlementPage2" )
                    as DisabilityEntitlementPage2;

                DisabilityEntitlementPage3 page3 = MyWizardContainer.GetWizardPage( "DisabilityEntitlementPage3" )
                    as DisabilityEntitlementPage3;

                if ( page2 != null
                    && page3 != null )
                {
                    if ( page2.HasSummary
                        || page3.HasSummary )
                    {
                        MyWizardButtons.UpdateNavigation( "&Continue to Summary", "SummaryPage" );
                        MyWizardButtons.SetAcceptButton( "&Continue to Summary" );
                        rc = true;
                    }
                }
            }

            HasSummary = rc;
            return rc;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods

        /// <summary>
        /// displayPatientEmployment - display the patient's employment
        /// </summary>
        /// <param name="display"></param>
        private void displayPatientEmployment( bool display )
        {
            if ( display )
            {
                string employer = buildEmploymentAddress();

                if ( employer != String.Empty )
                {
                    lblQ1EmployerInfo.Text = employer;
                }
                else
                {
                    lblQ1EmployerInfo.Text = "Not available";
                }
            }
            else
            {
                lblQ1EmployerInfo.Text = string.Empty;
            }
        }

        /// <summary>
        /// displaySpouseEmployment - display the patient's spouse's employment
        /// </summary>
        /// <param name="spouseEmployment"></param>
        /// <param name="display"></param>
        private void displaySpouseEmployment( Employment spouseEmployment, bool display )
        {
            if ( display )
            {
                mtbQ2EmployerName.Text = spouseEmployment.Employer.Name;

                Address employerAddress = spouseEmployment.Employer.PartyContactPoint.Address;

                mtbQ2Address.Text = employerAddress.Address1;
                mtbQ2City.Text = employerAddress.City;
                cmbQ2State.SelectedItem = employerAddress.State;
                mtbQ2ZipCode.Text = employerAddress.ZipCode.ZipCodePrimary;
                mtbQ2ZipExtension.Text = employerAddress.ZipCode.ZipCodeExtended;
            }
            else
            {
                mtbQ2EmployerName.Text = string.Empty;
                mtbQ2Address.Text = string.Empty;
                mtbQ2City.Text = string.Empty;
                cmbQ2State.SelectedIndex = 0;
                mtbQ2ZipCode.Text = string.Empty;
                mtbQ2ZipExtension.Text = string.Empty;
            }
        }

        /// <summary>
        /// buildEmploymentAddress - build out text to display on the page that represents the patient's employment
        /// </summary>
        /// <returns></returns>
        private string buildEmploymentAddress()
        {
            if ( Model_Account == null
                || Model_Account.Patient == null
                || Model_Account.Patient.Employment == null
                || Model_Account.Patient.Employment.Employer == null
                || Model_Account.Patient.Employment.Employer.PartyContactPoint == null
                || Model_Account.Patient.Employment.Employer.PartyContactPoint.Address == null )
            {
                return string.Empty;
            }

            const string CITYSTATE_DELIMITER = ", ";
            const string SPACE_DELIMITER = " ";
            StringBuilder msg = new StringBuilder();

            lblQ1EmployerInfo.ResetText();

            Address addr = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address;

            msg.Append( Model_Account.Patient.Employment.Employer.Name );

            if ( addr.Address1 != null && addr.Address1.Length > 1 )
            {
                msg.Append( Environment.NewLine );
                msg.Append( addr.Address1 );
            }

            if ( addr.Address2 != null && addr.Address2.Length > 1 )
            {
                msg.Append( Environment.NewLine );
                msg.Append( addr.Address2 );
            }

            if ( addr.State != null && addr.State.ToString().Length > 1 )
            {
                string statestr = addr.State.ToString();

                if ( ( addr.City.Length + statestr.Length + addr.ZipCode.PostalCode.Length ) > 0 )
                {
                    msg.Append( Environment.NewLine );
                    if ( addr.City.Length > 0 )
                    {
                        msg.Append( addr.City );
                    }
                    if ( addr.State.PrintString.Length > 0 )
                    {
                        msg.Append( CITYSTATE_DELIMITER );
                        msg.Append( addr.State );
                    }
                    if ( addr.ZipCode.PostalCode.Length > 0 )
                    {
                        msg.Append( SPACE_DELIMITER );
                        msg.Append( addr.ZipCode.PostalCode );
                    }
                }
                if ( addr.Country != null && addr.Country.PrintString.Length > 0 )
                {
                    msg.Append( Environment.NewLine );
                    msg.Append( addr.Country.PrintString );
                }
            }

            return msg.ToString();
        }

        /// <summary>
        /// loadStatesCombo - populate the combo with states, provinces, etc
        /// </summary>
        private void loadStatesCombo()
        {
            if ( cmbQ2State.Items.Count == 0 )
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
                        cmbQ2State.Items.Add( state );
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

            UIColors.SetNormalBgColor( mtbQ1DateOfRetirement );
            UIColors.SetNormalBgColor( mtbQ2DateOfRetirement );
            UIColors.SetNormalBgColor( mtbQ2EmployerName );
            UIColors.SetNormalBgColor( mtbQ2Address );
            UIColors.SetNormalBgColor( mtbQ2City );
            UIColors.SetNormalBgColor( cmbQ2State );
            UIColors.SetNormalBgColor( mtbQ2ZipCode );

            Refresh();

            if ( mtbQ1DateOfRetirement.Enabled
                && mtbQ1DateOfRetirement.UnMaskedText.Trim().Length != 8 )
            {
                UIColors.SetRequiredBgColor( mtbQ1DateOfRetirement );
                blnRC = false;
            }

            if ( mtbQ2DateOfRetirement.Enabled
                && mtbQ2DateOfRetirement.UnMaskedText.Trim().Length != 8 )
            {
                UIColors.SetRequiredBgColor( mtbQ2DateOfRetirement );
                blnRC = false;
            }

            if ( mtbQ2EmployerName.Enabled
                && mtbQ2EmployerName.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ2EmployerName );
                blnRC = false;
            }

            if ( mtbQ2Address.Enabled
                && mtbQ2Address.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ2Address );
                blnRC = false;
            }

            if ( mtbQ2City.Enabled
                && mtbQ2City.UnMaskedText.Trim() == string.Empty )
            {
                UIColors.SetRequiredBgColor( mtbQ2City );
                blnRC = false;
            }

            if ( cmbQ2State.Enabled
                && ( cmbQ2State.SelectedItem == null ||
                cmbQ2State.SelectedIndex < 1 ) )
            {
                UIColors.SetRequiredBgColor( cmbQ2State );
                blnRC = false;
            }

            if ( mtbQ2ZipCode.Enabled )
            {
                if ( mtbQ2ZipCode.UnMaskedText.Trim() == string.Empty )
                {
                    UIColors.SetRequiredBgColor( mtbQ2ZipCode );
                    blnRC = false;
                }
                else if ( mtbQ2ZipCode.UnMaskedText.Trim().Length != 5 )
                {
                    UIColors.SetErrorBgColor( mtbQ2ZipCode );
                    MessageBox.Show( "The primary zip code must be 5 digits.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtbQ2ZipCode.Focus();
                    blnRC = false;
                }
            }

            return blnRC;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployerName( mtbQ2EmployerName );
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mtbQ2Address );
            MaskedEditTextBoxBuilder.ConfigureAddressCity( mtbQ2City );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mtbQ2ZipCode );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mtbQ2ZipExtension );
        }

        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlQuestion2 = new System.Windows.Forms.Panel();
            this.btnQ2MoreInfo = new PatientAccess.UI.CommonControls.LoggingButton();
            this.pnlQuestion2b = new System.Windows.Forms.Panel();
            this.mtbQ2DateOfRetirement = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ2DateOfRetirement = new System.Windows.Forms.Label();
            this.pnlDividerQ2 = new System.Windows.Forms.Panel();
            this.pnlQ2RadioButtons = new System.Windows.Forms.Panel();
            this.rbQ2NoNever = new System.Windows.Forms.RadioButton();
            this.rbQ2NoRetired = new System.Windows.Forms.RadioButton();
            this.rbQ2Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.pnlQuestion2c = new System.Windows.Forms.Panel();
            this.gbQ2SpouseEmployer = new System.Windows.Forms.GroupBox();
            this.cmbQ2State = new System.Windows.Forms.ComboBox();
            this.mtbQ2ZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbQ2EmployerName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ2State = new System.Windows.Forms.Label();
            this.lblQ2City = new System.Windows.Forms.Label();
            this.mtbQ2City = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ2Zip = new System.Windows.Forms.Label();
            this.lblQ2Hyphen = new System.Windows.Forms.Label();
            this.lblQ2Address = new System.Windows.Forms.Label();
            this.mtbQ2ZipExtension = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblQ2EmployerName = new System.Windows.Forms.Label();
            this.mtbQ2Address = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.pnlQuestion1 = new System.Windows.Forms.Panel();
            this.pnlQuestion1b = new System.Windows.Forms.Panel();
            this.lblQ1DateOfRetirement = new System.Windows.Forms.Label();
            this.mtbQ1DateOfRetirement = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.pnlQ1RadioButtons = new System.Windows.Forms.Panel();
            this.rbQ1NoNever = new System.Windows.Forms.RadioButton();
            this.rbQ1NoRetired = new System.Windows.Forms.RadioButton();
            this.rbQ1Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.pnlDividerQ1 = new System.Windows.Forms.Panel();
            this.pnlQuestion1c = new System.Windows.Forms.Panel();
            this.gbQ1Employer = new System.Windows.Forms.GroupBox();
            this.btnQ1EditEmployment = new PatientAccess.UI.CommonControls.LoggingButton();
            this.lblQ1EmployerInfo = new System.Windows.Forms.Label();
            this.pnlDivider1 = new System.Windows.Forms.Panel();
            this.pnlWizardPageBody.SuspendLayout();
            this.pnlQuestion2.SuspendLayout();
            this.pnlQuestion2b.SuspendLayout();
            this.pnlQ2RadioButtons.SuspendLayout();
            this.pnlQuestion2c.SuspendLayout();
            this.gbQ2SpouseEmployer.SuspendLayout();
            this.pnlQuestion1.SuspendLayout();
            this.pnlQuestion1b.SuspendLayout();
            this.pnlQ1RadioButtons.SuspendLayout();
            this.pnlQuestion1c.SuspendLayout();
            this.gbQ1Employer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlWizardPageBody
            // 
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion2 );
            this.pnlWizardPageBody.Controls.Add( this.pnlQuestion1 );
            this.pnlWizardPageBody.Controls.Add( this.pnlDivider1 );
            this.pnlWizardPageBody.Name = "pnlWizardPageBody";
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlDivider1, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion1, 0 );
            this.pnlWizardPageBody.Controls.SetChildIndex( this.pnlQuestion2, 0 );
            // 
            // pnlQuestion2
            // 
            this.pnlQuestion2.Controls.Add( this.btnQ2MoreInfo );
            this.pnlQuestion2.Controls.Add( this.pnlQuestion2b );
            this.pnlQuestion2.Controls.Add( this.pnlDividerQ2 );
            this.pnlQuestion2.Controls.Add( this.pnlQ2RadioButtons );
            this.pnlQuestion2.Controls.Add( this.lblQuestion2 );
            this.pnlQuestion2.Controls.Add( this.pnlQuestion2c );
            this.pnlQuestion2.Location = new System.Drawing.Point( 8, 229 );
            this.pnlQuestion2.Name = "pnlQuestion2";
            this.pnlQuestion2.Size = new System.Drawing.Size( 667, 251 );
            this.pnlQuestion2.TabIndex = 5;
            // 
            // btnQ2MoreInfo
            // 
            this.btnQ2MoreInfo.Location = new System.Drawing.Point( 260, 4 );
            this.btnQ2MoreInfo.Message = null;
            this.btnQ2MoreInfo.Name = "btnQ2MoreInfo";
            this.btnQ2MoreInfo.TabIndex = 1;
            this.btnQ2MoreInfo.Text = "More info";
            this.btnQ2MoreInfo.Click += new System.EventHandler( this.btnQ2MoreInfo_Click );
            // 
            // pnlQuestion2b
            // 
            this.pnlQuestion2b.Controls.Add( this.mtbQ2DateOfRetirement );
            this.pnlQuestion2b.Controls.Add( this.lblQ2DateOfRetirement );
            this.pnlQuestion2b.Location = new System.Drawing.Point( 14, 25 );
            this.pnlQuestion2b.Name = "pnlQuestion2b";
            this.pnlQuestion2b.Size = new System.Drawing.Size( 192, 26 );
            this.pnlQuestion2b.TabIndex = 9;
            // 
            // mtbQ2DateOfRetirement
            // 
            this.mtbQ2DateOfRetirement.BackColor = System.Drawing.Color.White;
            this.mtbQ2DateOfRetirement.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2DateOfRetirement.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbQ2DateOfRetirement.Location = new System.Drawing.Point( 105, 4 );
            this.mtbQ2DateOfRetirement.Mask = "  /  /";
            this.mtbQ2DateOfRetirement.MaxLength = 10;
            this.mtbQ2DateOfRetirement.Name = "mtbQ2DateOfRetirement";
            this.mtbQ2DateOfRetirement.Size = new System.Drawing.Size( 70, 20 );
            this.mtbQ2DateOfRetirement.TabIndex = 1;
            this.mtbQ2DateOfRetirement.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbQ2DateOfRetirement.Leave += new System.EventHandler( mtbQ2DateOfRetirement_Leave );
            this.mtbQ2DateOfRetirement.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // lblQ2DateOfRetirement
            // 
            this.lblQ2DateOfRetirement.Enabled = false;
            this.lblQ2DateOfRetirement.Location = new System.Drawing.Point( 7, 5 );
            this.lblQ2DateOfRetirement.Name = "lblQ2DateOfRetirement";
            this.lblQ2DateOfRetirement.Size = new System.Drawing.Size( 98, 23 );
            this.lblQ2DateOfRetirement.TabIndex = 7;
            this.lblQ2DateOfRetirement.Text = "Date of retirement:";
            // 
            // pnlDividerQ2
            // 
            this.pnlDividerQ2.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ2.Location = new System.Drawing.Point( 5, 241 );
            this.pnlDividerQ2.Name = "pnlDividerQ2";
            this.pnlDividerQ2.Size = new System.Drawing.Size( 656, 2 );
            this.pnlDividerQ2.TabIndex = 0;
            // 
            // pnlQ2RadioButtons
            // 
            this.pnlQ2RadioButtons.Controls.Add( this.rbQ2NoNever );
            this.pnlQ2RadioButtons.Controls.Add( this.rbQ2NoRetired );
            this.pnlQ2RadioButtons.Controls.Add( this.rbQ2Yes );
            this.pnlQ2RadioButtons.Location = new System.Drawing.Point( 408, 0 );
            this.pnlQ2RadioButtons.Name = "pnlQ2RadioButtons";
            this.pnlQ2RadioButtons.Size = new System.Drawing.Size( 257, 57 );
            this.pnlQ2RadioButtons.TabIndex = 1;
            this.pnlQ2RadioButtons.TabStop = true;
            this.pnlQ2RadioButtons.GotFocus += new System.EventHandler( this.pnlQ2RadioButtons_GotFocus );
            // 
            // rbQ2NoNever
            // 
            this.rbQ2NoNever.Location = new System.Drawing.Point( 94, 33 );
            this.rbQ2NoNever.Name = "rbQ2NoNever";
            this.rbQ2NoNever.Size = new System.Drawing.Size( 187, 24 );
            this.rbQ2NoNever.TabIndex = 4;
            this.rbQ2NoNever.TabStop = true;
            this.rbQ2NoNever.Text = "No - (Spouse) Never employed";
            this.rbQ2NoNever.CheckedChanged += new System.EventHandler( this.rbQ2NoNever_CheckedChanged );
            // 
            // rbQ2NoRetired
            // 
            this.rbQ2NoRetired.Location = new System.Drawing.Point( 94, 9 );
            this.rbQ2NoRetired.Name = "rbQ2NoRetired";
            this.rbQ2NoRetired.Size = new System.Drawing.Size( 138, 24 );
            this.rbQ2NoRetired.TabIndex = 3;
            this.rbQ2NoRetired.TabStop = true;
            this.rbQ2NoRetired.Text = "No - (Spouse) Retired";
            this.rbQ2NoRetired.CheckedChanged += new System.EventHandler( this.rbQ2NoRetired_CheckedChanged );
            // 
            // rbQ2Yes
            // 
            this.rbQ2Yes.Location = new System.Drawing.Point( 23, 9 );
            this.rbQ2Yes.Name = "rbQ2Yes";
            this.rbQ2Yes.Size = new System.Drawing.Size( 47, 24 );
            this.rbQ2Yes.TabIndex = 2;
            this.rbQ2Yes.TabStop = true;
            this.rbQ2Yes.Text = "Yes";
            this.rbQ2Yes.CheckedChanged += new System.EventHandler( this.rbQ2Yes_CheckedChanged );
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point( 8, 8 );
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size( 253, 18 );
            this.lblQuestion2.TabIndex = 8;
            this.lblQuestion2.Text = "2. If married, is your spouse currently employed?";
            // 
            // pnlQuestion2c
            // 
            this.pnlQuestion2c.Controls.Add( this.gbQ2SpouseEmployer );
            this.pnlQuestion2c.Location = new System.Drawing.Point( 4, 60 );
            this.pnlQuestion2c.Name = "pnlQuestion2c";
            this.pnlQuestion2c.Size = new System.Drawing.Size( 429, 141 );
            this.pnlQuestion2c.TabIndex = 2;
            // 
            // gbQ2SpouseEmployer
            // 
            this.gbQ2SpouseEmployer.Controls.Add( this.cmbQ2State );
            this.gbQ2SpouseEmployer.Controls.Add( this.mtbQ2ZipCode );
            this.gbQ2SpouseEmployer.Controls.Add( this.mtbQ2EmployerName );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2State );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2City );
            this.gbQ2SpouseEmployer.Controls.Add( this.mtbQ2City );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2Zip );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2Hyphen );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2Address );
            this.gbQ2SpouseEmployer.Controls.Add( this.mtbQ2ZipExtension );
            this.gbQ2SpouseEmployer.Controls.Add( this.lblQ2EmployerName );
            this.gbQ2SpouseEmployer.Controls.Add( this.mtbQ2Address );
            this.gbQ2SpouseEmployer.Location = new System.Drawing.Point( 9, 9 );
            this.gbQ2SpouseEmployer.Name = "gbQ2SpouseEmployer";
            this.gbQ2SpouseEmployer.Size = new System.Drawing.Size( 412, 124 );
            this.gbQ2SpouseEmployer.TabIndex = 2;
            this.gbQ2SpouseEmployer.TabStop = false;
            this.gbQ2SpouseEmployer.Text = "Spouse\'s employer";
            // 
            // cmbQ2State
            // 
            this.cmbQ2State.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQ2State.Location = new System.Drawing.Point( 47, 96 );
            this.cmbQ2State.Name = "cmbQ2State";
            this.cmbQ2State.Size = new System.Drawing.Size( 165, 21 );
            this.cmbQ2State.TabIndex = 4;
            this.cmbQ2State.EnabledChanged += new System.EventHandler( this.runRules );
            this.cmbQ2State.SelectedIndexChanged += new System.EventHandler( this.runRules );
            // 
            // mtbQ2ZipCode
            // 
            this.mtbQ2ZipCode.BackColor = System.Drawing.Color.White;
            this.mtbQ2ZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2ZipCode.Location = new System.Drawing.Point( 280, 96 );
            this.mtbQ2ZipCode.Mask = "";
            this.mtbQ2ZipCode.MaxLength = 5;
            this.mtbQ2ZipCode.Name = "mtbQ2ZipCode";
            this.mtbQ2ZipCode.Size = new System.Drawing.Size( 38, 20 );
            this.mtbQ2ZipCode.TabIndex = 5;
            this.mtbQ2ZipCode.Leave += new System.EventHandler( this.runRules );
            this.mtbQ2ZipCode.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // mtbQ2EmployerName
            // 
            this.mtbQ2EmployerName.BackColor = System.Drawing.Color.White;
            this.mtbQ2EmployerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ2EmployerName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2EmployerName.Location = new System.Drawing.Point( 107, 15 );
            this.mtbQ2EmployerName.Mask = "";
            this.mtbQ2EmployerName.MaxLength = Employer.PBAR_EMP_NAME_LENGTH;
            this.mtbQ2EmployerName.Name = "mtbQ2EmployerName";
            this.mtbQ2EmployerName.Size = new System.Drawing.Size( 295, 20 );
            this.mtbQ2EmployerName.TabIndex = 1;
            this.mtbQ2EmployerName.Leave += new System.EventHandler( this.runRules );
            this.mtbQ2EmployerName.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // lblQ2State
            // 
            this.lblQ2State.Location = new System.Drawing.Point( 9, 98 );
            this.lblQ2State.Name = "lblQ2State";
            this.lblQ2State.Size = new System.Drawing.Size( 40, 16 );
            this.lblQ2State.TabIndex = 0;
            this.lblQ2State.Text = "State:";
            // 
            // lblQ2City
            // 
            this.lblQ2City.Location = new System.Drawing.Point( 8, 70 );
            this.lblQ2City.Name = "lblQ2City";
            this.lblQ2City.Size = new System.Drawing.Size( 32, 17 );
            this.lblQ2City.TabIndex = 0;
            this.lblQ2City.Text = "City:";
            // 
            // mtbQ2City
            // 
            this.mtbQ2City.BackColor = System.Drawing.Color.White;
            this.mtbQ2City.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ2City.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2City.Location = new System.Drawing.Point( 107, 67 );
            this.mtbQ2City.Mask = "";
            this.mtbQ2City.MaxLength = 17;
            this.mtbQ2City.Name = "mtbQ2City";
            this.mtbQ2City.Size = new System.Drawing.Size( 130, 20 );
            this.mtbQ2City.TabIndex = 3;
            this.mtbQ2City.Leave += new System.EventHandler( this.runRules );
            this.mtbQ2City.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // lblQ2Zip
            // 
            this.lblQ2Zip.Location = new System.Drawing.Point( 224, 98 );
            this.lblQ2Zip.Name = "lblQ2Zip";
            this.lblQ2Zip.Size = new System.Drawing.Size( 56, 23 );
            this.lblQ2Zip.TabIndex = 0;
            this.lblQ2Zip.Text = "Zip Code:";
            // 
            // lblQ2Hyphen
            // 
            this.lblQ2Hyphen.Location = new System.Drawing.Point( 323, 98 );
            this.lblQ2Hyphen.Name = "lblQ2Hyphen";
            this.lblQ2Hyphen.Size = new System.Drawing.Size( 8, 23 );
            this.lblQ2Hyphen.TabIndex = 0;
            this.lblQ2Hyphen.Text = "-";
            // 
            // lblQ2Address
            // 
            this.lblQ2Address.Location = new System.Drawing.Point( 8, 44 );
            this.lblQ2Address.Name = "lblQ2Address";
            this.lblQ2Address.Size = new System.Drawing.Size( 53, 23 );
            this.lblQ2Address.TabIndex = 0;
            this.lblQ2Address.Text = "Address:";
            // 
            // mtbQ2ZipExtension
            // 
            this.mtbQ2ZipExtension.BackColor = System.Drawing.Color.White;
            this.mtbQ2ZipExtension.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2ZipExtension.Location = new System.Drawing.Point( 336, 96 );
            this.mtbQ2ZipExtension.Mask = "";
            this.mtbQ2ZipExtension.MaxLength = 4;
            this.mtbQ2ZipExtension.Name = "mtbQ2ZipExtension";
            this.mtbQ2ZipExtension.Size = new System.Drawing.Size( 32, 20 );
            this.mtbQ2ZipExtension.TabIndex = 6;
            // 
            // lblQ2EmployerName
            // 
            this.lblQ2EmployerName.Location = new System.Drawing.Point( 8, 18 );
            this.lblQ2EmployerName.Name = "lblQ2EmployerName";
            this.lblQ2EmployerName.Size = new System.Drawing.Size( 87, 23 );
            this.lblQ2EmployerName.TabIndex = 0;
            this.lblQ2EmployerName.Text = "Employer name:";
            // 
            // mtbQ2Address
            // 
            this.mtbQ2Address.BackColor = System.Drawing.Color.White;
            this.mtbQ2Address.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbQ2Address.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ2Address.Location = new System.Drawing.Point( 107, 41 );
            this.mtbQ2Address.Mask = "";
            this.mtbQ2Address.MaxLength = 25;
            this.mtbQ2Address.Name = "mtbQ2Address";
            this.mtbQ2Address.Size = new System.Drawing.Size( 188, 20 );
            this.mtbQ2Address.TabIndex = 2;
            this.mtbQ2Address.Leave += new System.EventHandler( this.runRules );
            this.mtbQ2Address.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // pnlQuestion1
            // 
            this.pnlQuestion1.Controls.Add( this.pnlQuestion1b );
            this.pnlQuestion1.Controls.Add( this.pnlQ1RadioButtons );
            this.pnlQuestion1.Controls.Add( this.lblQuestion1 );
            this.pnlQuestion1.Controls.Add( this.pnlDividerQ1 );
            this.pnlQuestion1.Controls.Add( this.pnlQuestion1c );
            this.pnlQuestion1.Location = new System.Drawing.Point( 8, 72 );
            this.pnlQuestion1.Name = "pnlQuestion1";
            this.pnlQuestion1.Size = new System.Drawing.Size( 667, 150 );
            this.pnlQuestion1.TabIndex = 4;
            // 
            // pnlQuestion1b
            // 
            this.pnlQuestion1b.Controls.Add( this.lblQ1DateOfRetirement );
            this.pnlQuestion1b.Controls.Add( this.mtbQ1DateOfRetirement );
            this.pnlQuestion1b.Location = new System.Drawing.Point( 6, 19 );
            this.pnlQuestion1b.Name = "pnlQuestion1b";
            this.pnlQuestion1b.Size = new System.Drawing.Size( 389, 32 );
            this.pnlQuestion1b.TabIndex = 3;
            // 
            // lblQ1DateOfRetirement
            // 
            this.lblQ1DateOfRetirement.Location = new System.Drawing.Point( 13, 9 );
            this.lblQ1DateOfRetirement.Name = "lblQ1DateOfRetirement";
            this.lblQ1DateOfRetirement.Size = new System.Drawing.Size( 98, 14 );
            this.lblQ1DateOfRetirement.TabIndex = 2;
            this.lblQ1DateOfRetirement.Text = "Date of retirement:";
            // 
            // mtbQ1DateOfRetirement
            // 
            this.mtbQ1DateOfRetirement.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbQ1DateOfRetirement.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbQ1DateOfRetirement.Location = new System.Drawing.Point( 111, 7 );
            this.mtbQ1DateOfRetirement.Mask = "  /  /";
            this.mtbQ1DateOfRetirement.MaxLength = 10;
            this.mtbQ1DateOfRetirement.Name = "mtbQ1DateOfRetirement";
            this.mtbQ1DateOfRetirement.Size = new System.Drawing.Size( 70, 20 );
            this.mtbQ1DateOfRetirement.TabIndex = 1;
            this.mtbQ1DateOfRetirement.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbQ1DateOfRetirement.Leave += new System.EventHandler( mtbQ1DateOfRetirement_Leave );
            this.mtbQ1DateOfRetirement.EnabledChanged += new System.EventHandler( this.runRules );
            // 
            // pnlQ1RadioButtons
            // 
            this.pnlQ1RadioButtons.Controls.Add( this.rbQ1NoNever );
            this.pnlQ1RadioButtons.Controls.Add( this.rbQ1NoRetired );
            this.pnlQ1RadioButtons.Controls.Add( this.rbQ1Yes );
            this.pnlQ1RadioButtons.Location = new System.Drawing.Point( 408, -7 );
            this.pnlQ1RadioButtons.Name = "pnlQ1RadioButtons";
            this.pnlQ1RadioButtons.Size = new System.Drawing.Size( 251, 63 );
            this.pnlQ1RadioButtons.TabIndex = 1;
            this.pnlQ1RadioButtons.TabStop = true;
            this.pnlQ1RadioButtons.GotFocus += new System.EventHandler( this.pnlQ1RadioButtons_GotFocus );
            // 
            // rbQ1NoNever
            // 
            this.rbQ1NoNever.Location = new System.Drawing.Point( 94, 33 );
            this.rbQ1NoNever.Name = "rbQ1NoNever";
            this.rbQ1NoNever.Size = new System.Drawing.Size( 130, 24 );
            this.rbQ1NoNever.TabIndex = 3;
            this.rbQ1NoNever.TabStop = true;
            this.rbQ1NoNever.Text = "No - Never employed";
            this.rbQ1NoNever.CheckedChanged += new System.EventHandler( this.rbQ1NoNever_CheckedChanged );
            // 
            // rbQ1NoRetired
            // 
            this.rbQ1NoRetired.Location = new System.Drawing.Point( 94, 9 );
            this.rbQ1NoRetired.Name = "rbQ1NoRetired";
            this.rbQ1NoRetired.Size = new System.Drawing.Size( 85, 24 );
            this.rbQ1NoRetired.TabIndex = 2;
            this.rbQ1NoRetired.TabStop = true;
            this.rbQ1NoRetired.Text = "No - Retired";
            this.rbQ1NoRetired.CheckedChanged += new System.EventHandler( this.rbQ1NoRetired_CheckedChanged );
            // 
            // rbQ1Yes
            // 
            this.rbQ1Yes.Location = new System.Drawing.Point( 23, 9 );
            this.rbQ1Yes.Name = "rbQ1Yes";
            this.rbQ1Yes.Size = new System.Drawing.Size( 47, 24 );
            this.rbQ1Yes.TabIndex = 1;
            this.rbQ1Yes.TabStop = true;
            this.rbQ1Yes.Text = "Yes";
            this.rbQ1Yes.CheckedChanged += new System.EventHandler( this.rbQ1Yes_CheckedChanged );
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point( 8, 3 );
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size( 167, 16 );
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1. Are you currently employed?";
            // 
            // pnlDividerQ1
            // 
            this.pnlDividerQ1.BackColor = System.Drawing.Color.Black;
            this.pnlDividerQ1.Location = new System.Drawing.Point( 4, 143 );
            this.pnlDividerQ1.Name = "pnlDividerQ1";
            this.pnlDividerQ1.Size = new System.Drawing.Size( 656, 1 );
            this.pnlDividerQ1.TabIndex = 0;
            // 
            // pnlQuestion1c
            // 
            this.pnlQuestion1c.Controls.Add( this.gbQ1Employer );
            this.pnlQuestion1c.Location = new System.Drawing.Point( 1, 54 );
            this.pnlQuestion1c.Name = "pnlQuestion1c";
            this.pnlQuestion1c.Size = new System.Drawing.Size( 441, 77 );
            this.pnlQuestion1c.TabIndex = 2;
            // 
            // gbQ1Employer
            // 
            this.gbQ1Employer.Controls.Add( this.btnQ1EditEmployment );
            this.gbQ1Employer.Controls.Add( this.lblQ1EmployerInfo );
            this.gbQ1Employer.Location = new System.Drawing.Point( 9, 2 );
            this.gbQ1Employer.Name = "gbQ1Employer";
            this.gbQ1Employer.Size = new System.Drawing.Size( 425, 74 );
            this.gbQ1Employer.TabIndex = 2;
            this.gbQ1Employer.TabStop = false;
            this.gbQ1Employer.Text = "Employer";
            // 
            // btnQ1EditEmployment
            // 
            this.btnQ1EditEmployment.Location = new System.Drawing.Point( 206, 24 );
            this.btnQ1EditEmployment.Message = null;
            this.btnQ1EditEmployment.Name = "btnQ1EditEmployment";
            this.btnQ1EditEmployment.Size = new System.Drawing.Size( 180, 23 );
            this.btnQ1EditEmployment.TabIndex = 2;
            this.btnQ1EditEmployment.Text = "Edit E&mployment && Cancel MSP";
            this.btnQ1EditEmployment.Click += new System.EventHandler( this.btnQ1EditEmployment_Click );
            // 
            // lblQ1EmployerInfo
            // 
            this.lblQ1EmployerInfo.Location = new System.Drawing.Point( 9, 24 );
            this.lblQ1EmployerInfo.Name = "lblQ1EmployerInfo";
            this.lblQ1EmployerInfo.Size = new System.Drawing.Size( 182, 40 );
            this.lblQ1EmployerInfo.TabIndex = 1;
            // 
            // pnlDivider1
            // 
            this.pnlDivider1.BackColor = System.Drawing.Color.Black;
            this.pnlDivider1.Location = new System.Drawing.Point( 13, 56 );
            this.pnlDivider1.Name = "pnlDivider1";
            this.pnlDivider1.Size = new System.Drawing.Size( 656, 2 );
            this.pnlDivider1.TabIndex = 3;
            // 
            // DisabilityEntitlementPage1
            // 
            this.Name = "DisabilityEntitlementPage1";
            this.EnabledChanged += new System.EventHandler( this.DisabilityEntitlementPage1_EnabledChanged );
            this.Load += new System.EventHandler( this.DisabilityEntitlementPage1_Load );
            this.pnlWizardPageBody.ResumeLayout( false );
            this.pnlQuestion2.ResumeLayout( false );
            this.pnlQuestion2b.ResumeLayout( false );
            this.pnlQ2RadioButtons.ResumeLayout( false );
            this.pnlQuestion2c.ResumeLayout( false );
            this.gbQ2SpouseEmployer.ResumeLayout( false );
            this.pnlQuestion1.ResumeLayout( false );
            this.pnlQuestion1b.ResumeLayout( false );
            this.pnlQ1RadioButtons.ResumeLayout( false );
            this.pnlQuestion1c.ResumeLayout( false );
            this.gbQ1Employer.ResumeLayout( false );
            this.ResumeLayout( false );
        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage1()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
        }

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage1( WizardContainer wizardContainer )
            : base( wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
        }

        //TODO-AC-SR352-Delete if not needed
        public DisabilityEntitlementPage1( string pageName, WizardContainer wizardContainer )
            : base( pageName, wizardContainer )
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            ConfigureControls();

            EnableThemesOn( this );
        }

        public DisabilityEntitlementPage1( string pageName, WizardContainer wizardContainer, Account anAccount )
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
        private Panel pnlQuestion1;
        private Panel pnlQuestion1b;
        private Panel pnlQuestion1c;
        private Panel pnlQ1RadioButtons;
        private Panel pnlDividerQ1;
        private Panel pnlQuestion2;
        private Panel pnlQuestion2b;
        private Panel pnlQuestion2c;
        private Panel pnlQ2RadioButtons;
        private Panel pnlDividerQ2;

        private MaskedEditTextBox mtbQ1DateOfRetirement;
        private MaskedEditTextBox mtbQ2DateOfRetirement;
        private MaskedEditTextBox mtbQ2EmployerName;
        private MaskedEditTextBox mtbQ2Address;
        private MaskedEditTextBox mtbQ2City;
        private MaskedEditTextBox mtbQ2ZipCode;
        private MaskedEditTextBox mtbQ2ZipExtension;

        private RadioButton rbQ1NoNever;
        private RadioButton rbQ1NoRetired;
        private RadioButton rbQ1Yes;
        private RadioButton rbQ2NoNever;
        private RadioButton rbQ2NoRetired;
        private RadioButton rbQ2Yes;

        private Label lblQuestion1;
        private Label lblQ1DateOfRetirement;
        private Label lblQ1EmployerInfo;
        private Label lblQ2DateOfRetirement;
        private Label lblQuestion2;
        private Label lblQ2EmployerName;
        private Label lblQ2Address;
        private Label lblQ2City;
        private Label lblQ2State;
        private Label lblQ2Zip;
        private Label lblQ2Hyphen;

        private GroupBox gbQ1Employer;
        private GroupBox gbQ2SpouseEmployer;

        private ComboBox cmbQ2State;

        private LoggingButton btnQ1EditEmployment;
        private LoggingButton btnQ2MoreInfo;

        private bool blnLoaded;

        #endregion

        #region Constants
        #endregion
    }
}

