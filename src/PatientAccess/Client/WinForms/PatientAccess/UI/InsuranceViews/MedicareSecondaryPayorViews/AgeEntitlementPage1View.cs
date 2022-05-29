using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for AgeEntitlementPage1View.
    /// </summary>
    public class AgeEntitlementPage1View : ControlView
    {
        [DllImport( "User32.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam );

        #region Event Handlers
        private void AgeEntitlementPage1View_Load( object sender, EventArgs e )
        {
            if ( ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment == null )
            {
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment = new Employment();
            }
            if ( ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment == null )
            {
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment = new Employment();
            }
        }

        private void btnEditEmployment_Click( object sender, EventArgs e )
        {
            parentForm.RaiseTabSelectedEvent( ( int )AccountView.ScreenIndexes.EMPLOYMENT );
        }

        private void mskRetirementDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
        }

        private void mskRetirementDate_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            FormCanTransition();
        }

        private void mskRetirementDate_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            dateFieldError = false;

            if ( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( mtb.Text.Length != 10 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                dateFieldError = true;
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                try
                {
                    DateTime retirementDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                    ITimeBroker broker = ProxyFactory.GetTimeBroker();
                    DateTime todaysDate = broker.TimeAt( Model_Account.Facility.GMTOffset,
                                                         Model_Account.Facility.DSTOffset );

                    if ( retirementDate > todaysDate )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        dateFieldError = true;
                        MessageBox.Show( UIErrorMessages.DATE_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else if ( DateValidator.IsValidDate( retirementDate ) == false )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        dateFieldError = true;
                        MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else
                    {
                        ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.RetiredDate = retirementDate;
                        UIColors.SetNormalBgColor( mtb );
                        Refresh();

                        if ( ( bool )Tag )
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                    }
                }
                catch
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    mtb.Focus();
                    UIColors.SetErrorBgColor( mtb );
                    dateFieldError = true;
                    MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
            FormCanTransition();
        }

        private void rbQuestion1Yes_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question1Response = true;
                SetRetirementControlState( false );
                DisplayPrimaryEmploymentInfo( true );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.Status = EmploymentStatus.NewFullTimeEmployed();

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.RetiredDate = DateTime.MinValue;

                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion1Retired_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question1Response = true;
                SetRetirementControlState( true );
                DisplayPrimaryEmploymentInfo( false );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.Status = EmploymentStatus.NewRetired();

                if ( Model_Account.RetirementDate != DateTime.MinValue )
                {
                    DateTime date = Model_Account.RetirementDate;
                    mskRetirementDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                    UIColors.SetNormalBgColor( mskRetirementDate );
                }

                ClearPage2Data();
                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion1Never_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question1Response = true;
                SetRetirementControlState( false );
                DisplayPrimaryEmploymentInfo( false );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.Status = EmploymentStatus.NewNotEmployed();

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.RetiredDate = DateTime.MinValue;

                ClearPage2Data();
                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Yes_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question2Response = true;
                DisplaySpouseEmploymentInfo( true );
                SetSpouseRetiredControlState( false );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Status = EmploymentStatus.NewFullTimeEmployed();

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.RetiredDate = DateTime.MinValue;

                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Retired_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question2Response = true;
                DisplaySpouseEmploymentInfo( false );
                SetSpouseRetiredControlState( true );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Status = EmploymentStatus.NewRetired();

                ClearPage2Data();
                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Never_CheckedChanged( object sender, EventArgs e )
        {
            RadioButton rb = ( RadioButton )sender;

            if ( rb.Checked )
            {
                question2Response = true;
                DisplaySpouseEmploymentInfo( false );
                SetSpouseRetiredControlState( false );

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Status = EmploymentStatus.NewNotEmployed();

                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.RetiredDate = DateTime.MinValue;

                ClearPage2Data();
                FormCanTransition();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void mskSpouseRetirementDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskSpouseRetirementDate_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            FormCanTransition();
        }

        private void mskSpouseRetirementDate_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            spouseDateFieldError = false;

            if ( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( mtb.Text.Length != 10 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                spouseDateFieldError = true;
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                try
                {   // Check the date entered is not in the future
                    DateTime spouseRetirementDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                    if ( spouseRetirementDate > DateTime.Today )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        spouseDateFieldError = true;
                        MessageBox.Show( UIErrorMessages.DATE_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else if ( DateValidator.IsValidDate( spouseRetirementDate ) == false )
                    {
                        mtb.Focus();
                        UIColors.SetErrorBgColor( mtb );
                        spouseDateFieldError = true;
                        MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                    else
                    {
                        ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.RetiredDate = spouseRetirementDate;
                        UIColors.SetNormalBgColor( mtb );
                        Refresh();

                        if ( ( bool )Tag )
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                    }
                }
                catch
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    mtb.Focus();
                    UIColors.SetErrorBgColor( mtb );
                    spouseDateFieldError = true;
                    MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
            }
            FormCanTransition();
        }

        private void mskEmployerName_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskEmployerName_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                FormCanTransition();
            }
        }

        private void mskEmployerName_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.Name = mtb.Text.Trim();

            if ( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( ( bool )Tag )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void mskEmployerAddress_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskEmployerAddress_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                FormCanTransition();
            }
        }

        private void mskEmployerAddress_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.PartyContactPoint.Address.Address1 = mtb.Text.Trim();

            if ( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( ( bool )Tag )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void mskEmployerCity_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskEmployerCity_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if ( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                FormCanTransition();
            }
        }

        private void mskEmployerCity_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.PartyContactPoint.Address.City = mtb.Text.Trim();

            if ( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( ( bool )Tag )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void cbStates_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = ( ComboBox )sender;
            UIColors.SetNormalBgColor( cb );
        }

        private void cbStates_Leave( object sender, EventArgs e )
        {
            ComboBox cb = ( ComboBox )sender;
            if ( cb.SelectedIndex < 1 )
            {
                UIColors.SetRequiredBgColor( cb );
                FormCanTransition();
            }
        }

        private void cbStates_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = ( ComboBox )sender;
            if ( cb.SelectedIndex > 0 )
            {
                UIColors.SetNormalBgColor( cb );

                State state = ( State )cb.SelectedItem;
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.PartyContactPoint.Address.State = ( State )state.Clone();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
            FormCanTransition();
        }

        private void mskZipCode_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskZipCode_TextChanged( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if ( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            if ( mtb.UnMaskedText.Length == 5 )
            {
                FormCanTransition();
            }
        }

        private void mskZipCode_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if ( mtb.Text.Length != 5 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( "The primary zip code must be 5 digits.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary = mtb.Text;
                UIColors.SetNormalBgColor( mtb );
                Refresh();

                if ( ( bool )Tag )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
            FormCanTransition();
        }

        private void mskZipPlusFour_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = ( MaskedEditTextBox )sender;
            if ( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if ( mtb.Text.Length != 4 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( "The extended zip code must be 4 digits.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended = mtb.Text;
                UIColors.SetNormalBgColor( mtb );
                Refresh();

                if ( ( bool )Tag )
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
            InitializeStates();

            if ( ( bool )Tag && FormChanged )
            {   // User went back and made a change
                ResetView();
            }
            else if ( formActivating )
            {
                ResetView();
                DisplayPrimaryEmploymentInfo( false );

                if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as AgeEntitlement ) == null )
                {
                    return;
                }
                if ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement.GetType().Equals( typeof( AgeEntitlement ) ) )
                {   // If the user didn't change the entitlement type in the previous screen, 
                    // then put the current data selections on the controls

                    if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                         as AgeEntitlement ).PatientEmployment != null )
                    {
                        EmploymentStatus patientEmpStatus = ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                                                              as AgeEntitlement ).PatientEmployment ).Status;

                        if ( patientEmpStatus != null )
                        {
                            if ( patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                                patientEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                            {
                                rbQuestion1Yes.Checked = true;
                            }
                            else if ( patientEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                            {
                                DateTime date = Model_Account.RetirementDate;

                                if ( date != DateTime.MinValue )
                                {
                                    ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                     AgeEntitlement ).PatientEmployment.RetiredDate = date;
                                    ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).PatientEmployment.RetiredDate = date;

                                    mskRetirementDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                                    UIColors.SetNormalBgColor( mskRetirementDate );
                                }

                                rbQuestion1Retired.Checked = true;
                            }
                            else if ( patientEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                            {
                                rbQuestion1Never.Checked = true;
                            }
                        }
                    }

                    if ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                         as AgeEntitlement ).SpouseEmployment != null )
                    {
                        EmploymentStatus spouseEmpStatus = ( ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                                                             as AgeEntitlement ).SpouseEmployment ).Status;

                        if ( spouseEmpStatus != null )
                        {
                            if ( spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) ||
                                spouseEmpStatus.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) )
                            {
                                rbQuestion2Yes.Checked = true;
                                Employment spouseEmployment = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement
                                                               as AgeEntitlement ).SpouseEmployment;
                                UpdateSpouseEmployerInformation( spouseEmployment );

                                DisplaySpouseEmploymentInfo( true );
                            }
                            else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.RETIRED_CODE ) )
                            {
                                DateTime date = ( Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                                                 AgeEntitlement ).SpouseEmployment.RetiredDate;

                                if ( date != DateTime.MinValue )
                                {
                                    ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment.RetiredDate = date;

                                    mskSpouseRetirementDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                                    UIColors.SetNormalBgColor( mskSpouseRetirementDate );
                                }
                                rbQuestion2Retired.Checked = true;
                            }
                            else if ( spouseEmpStatus.Code.Equals( EmploymentStatus.NOT_EMPLOYED_CODE ) )
                            {
                                rbQuestion2Never.Checked = true;
                            }
                        }
                    }
                }
            }
            FormCanTransition();
        }
        #endregion

        #region Properties
        [Browsable( false )]
        private MedicareSecondaryPayor Model_MedicareSecondaryPayor
        {
            get
            {
                return ( MedicareSecondaryPayor )Model;
            }
        }

        [Browsable( false )]
        public Account Model_Account
        {
            private get
            {
                return ( Account )i_account;
            }
            set
            {
                i_account = value;
            }
        }

        [Browsable( false )]
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

        [Browsable( false )]
        public int Response
        {
            get
            {
                return response;
            }
        }
        #endregion

        #region Private Methods
        private string BuildEmploymentAddress()
        {
            if ( Model_Account.Patient.Employment == null ||
                Model_Account.Patient.Employment.Employer.PartyContactPoint.Address == null )
            {
                return String.Empty;
            }

            const string CITYSTATE_DELIMITER = ", ";
            const string SPACE_DELIMITER = " ";
            StringBuilder msg = new StringBuilder();

            lblEmployerInfo.ResetText();
            Address addr = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address;

            if ( addr == null )
            {
                return String.Empty;
            }

            if ( Model_Account.Patient.Employment.Employer != null )
            {
                msg.Append( Model_Account.Patient.Employment.Employer.Name );
            }

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
        /// Clears the data on page 2 so the summary results come out correctly.  If the
        /// user comes from page 2 back to page 1 and makes a change that causes page 2
        /// not to be relevant, the data that was set on that page must be removed so it
        /// is not analyzed for the summary.
        /// </summary>
        private void ClearPage2Data()
        {
            if ( ( rbQuestion1Retired.Checked || rbQuestion1Never.Checked ) &&
                ( rbQuestion2Retired.Checked || rbQuestion2Never.Checked ) )
            {
                Model_MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.SetBlank();
                ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).GHPLimitExceeded.SetBlank();
                AgeEntitlementPage2View.formActivating = true;
            }
        }

        private void DisplayPrimaryEmploymentInfo( bool state )
        {
            SetEmploymentGroupBoxState( state );

            if ( state )
            {
                string employer = BuildEmploymentAddress();

                if ( employer != String.Empty )
                {
                    lblEmployerInfo.Text = employer;
                }
                else
                {
                    lblEmployerInfo.Text = "Not available";
                }
            }
        }

        private void DisplaySpouseEmploymentInfo( bool state )
        {
            SetSpouseEmployerControlState( state );

            if ( state )
            {
                if ( mskEmployerName.Text.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskEmployerName );
                }
                else
                {
                    UIColors.SetNormalBgColor( mskEmployerName );
                }

                if ( mskEmployerAddress.Text.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskEmployerAddress );
                }
                else
                {
                    UIColors.SetNormalBgColor( mskEmployerAddress );
                }

                if ( mskEmployerCity.Text.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskEmployerCity );
                }
                else
                {
                    UIColors.SetNormalBgColor( mskEmployerCity );
                }

                if ( cbStates.SelectedIndex < 1 )
                {
                    UIColors.SetRequiredBgColor( cbStates );
                }
                else
                {
                    UIColors.SetNormalBgColor( cbStates );
                }

                if ( mskZipCode.Text.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskZipCode );
                }
                else
                {
                    UIColors.SetNormalBgColor( mskZipCode );
                }
            }
        }

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            if ( dateFieldError || spouseDateFieldError )
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            }
            else if ( question1Response && question2Response )
            {
                if ( rbQuestion1Retired.Checked && mskRetirementDate.UnMaskedText.Length != 8 )
                {
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                }
                else if ( rbQuestion2Retired.Checked && mskSpouseRetirementDate.UnMaskedText.Length != 8 )
                {
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                }
                else if ( rbQuestion1Yes.Checked || rbQuestion2Yes.Checked )
                {   // Either patient or spouse is employed
                    if ( rbQuestion2Yes.Checked && SpouseEmployerControlsValid() == false )
                    {
                        SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                        SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    }
                    else
                    {
                        response = MSPEventCode.YesStimulus();
                        SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                        SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                        SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
                    }
                }
                else
                {   // Neither is employed, so Medicare is primary
                    response = MSPEventCode.NoStimulus();
                    SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
                }
            }
            else
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            }
        }

        private void InitializeStates()
        {
            if ( cbStates.Items.Count == 0 )
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
                        cbStates.Items.Add( state );
                    }
                }
                catch ( Exception ex )
                {
                    MessageBox.Show( "IAddressBroker failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }

        private void ResetView()
        {
            SetEmploymentGroupBoxState( false );
            rbQuestion1Yes.Checked = false;
            rbQuestion1Retired.Checked = false;
            rbQuestion1Never.Checked = false;
            rbQuestion2Yes.Checked = false;
            rbQuestion2Retired.Checked = false;
            rbQuestion2Never.Checked = false;
            mskZipCode.Text = String.Empty;
            mskZipPlusFour.Text = String.Empty;
            mskEmployerName.Text = String.Empty;
            mskEmployerAddress.Text = String.Empty;
            mskEmployerCity.Text = String.Empty;
            if ( cbStates.Items.Count > 0 )
            {
                cbStates.SelectedIndex = 0;
            }
            mskRetirementDate.UnMaskedText = String.Empty;
            mskSpouseRetirementDate.UnMaskedText = String.Empty;
            question1Response = false;
            question2Response = false;
            formActivating = false;
        }

        private void SetEmploymentGroupBoxState( bool state )
        {
            if ( state == false )
            {
                lblEmployerInfo.Text = String.Empty;
            }
            grpEmployer.Enabled = state;
        }

        private void SetRetirementControlState( bool state )
        {
            lblRetirmentDate.Enabled = state;
            mskRetirementDate.Enabled = state;

            if ( state )
            {
                if ( mskRetirementDate.UnMaskedText.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskRetirementDate );
                }
            }
            else
            {
                mskRetirementDate.Text = String.Empty;
                mskRetirementDate.UnMaskedText = String.Empty;
                UIColors.SetNormalBgColor( mskRetirementDate );
                Refresh();
            }
        }

        private void SetSpouseRetiredControlState( bool state )
        {
            lblStaticRetirementDate.Enabled = state;
            mskSpouseRetirementDate.Enabled = state;

            if ( state )
            {
                if ( mskSpouseRetirementDate.UnMaskedText.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskSpouseRetirementDate );
                }
            }
            else
            {
                mskSpouseRetirementDate.Text = String.Empty;
                mskSpouseRetirementDate.UnMaskedText = String.Empty;
                UIColors.SetNormalBgColor( mskSpouseRetirementDate );
                Refresh();
            }
        }

        /// <summary>
        /// Validates the employer fields and returns true if invalid.
        /// </summary>
        /// <returns>bool</returns>
        private bool SpouseEmployerControlsValid()
        {
            bool result = true;

            if ( mskEmployerName.Text.Length == 0 )
            {
                result = false;
            }
            else if ( mskEmployerAddress.Text.Length == 0 )
            {
                result = false;
            }
            else if ( mskEmployerCity.Text.Length == 0 )
            {
                result = false;
            }
            else if ( cbStates.SelectedIndex < 1 )
            {
                result = false;
            }
            else if ( mskZipCode.Text.Length != 5 )
            {
                result = false;
            }
            return result;
        }

        private void SetSpouseEmployerControlState( bool state )
        {
            if ( state == false )
            {
                mskEmployerName.Text = String.Empty;
                UIColors.SetNormalBgColor( mskEmployerName );

                mskEmployerAddress.Text = String.Empty;
                UIColors.SetNormalBgColor( mskEmployerAddress );

                mskEmployerCity.Text = String.Empty;
                UIColors.SetNormalBgColor( mskEmployerCity );

                UIColors.SetNormalBgColor( cbStates );
                cbStates.SelectedIndex = -1;

                mskZipCode.Text = String.Empty;
                UIColors.SetNormalBgColor( mskZipCode );

                mskZipPlusFour.Text = String.Empty;
                UIColors.SetNormalBgColor( mskZipPlusFour );
                Refresh();
            }
            else
            {
                if ( ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment == null )
                {
                    ( ( AgeEntitlement )Model_MedicareSecondaryPayor.MedicareEntitlement ).SpouseEmployment = new Employment();
                }
            }

            grpSpouseEmployer.Enabled = state;
        }

        private void UpdateSpouseEmployerInformation( Employment spouseEmployment )
        {
            mskEmployerName.Text = spouseEmployment.Employer.Name;
            Address employerAddress = spouseEmployment.Employer.PartyContactPoint.Address;

            mskEmployerAddress.Text = employerAddress.Address1;
            mskEmployerCity.Text = employerAddress.City;
            cbStates.Text = employerAddress.State.Description;
            mskZipCode.Text = employerAddress.ZipCode.ZipCodePrimary;
            mskZipPlusFour.Text = employerAddress.ZipCode.ZipCodeExtended;
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureEmployerName( mskEmployerName );
            MaskedEditTextBoxBuilder.ConfigureAddressStreet( mskEmployerAddress );
            MaskedEditTextBoxBuilder.ConfigureAddressCity( mskEmployerCity );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mskZipCode );
            MaskedEditTextBoxBuilder.ConfigureUnMaskedUSZipCode( mskZipPlusFour );
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblSpouseEmployed = new System.Windows.Forms.Label();
            this.rbQuestion2Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion2Retired = new System.Windows.Forms.RadioButton();
            this.rbQuestion2Never = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblCurrentlyEmployed = new System.Windows.Forms.Label();
            this.rbQuestion1Never = new System.Windows.Forms.RadioButton();
            this.rbQuestion1Retired = new System.Windows.Forms.RadioButton();
            this.rbQuestion1Yes = new System.Windows.Forms.RadioButton();
            this.lblRetirmentDate = new System.Windows.Forms.Label();
            this.txtTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.mskRetirementDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.grpEmployer = new System.Windows.Forms.GroupBox();
            this.lblEmployerInfo = new System.Windows.Forms.Label();
            this.btnEditEmployment = new LoggingButton();
            this.lblStaticRetirementDate = new System.Windows.Forms.Label();
            this.mskSpouseRetirementDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEmployerName = new System.Windows.Forms.Label();
            this.mskEmployerName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticAddress = new System.Windows.Forms.Label();
            this.mskEmployerAddress = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticCity = new System.Windows.Forms.Label();
            this.mskEmployerCity = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticState = new System.Windows.Forms.Label();
            this.lblStaticZipCode = new System.Windows.Forms.Label();
            this.lblHyphen = new System.Windows.Forms.Label();
            this.mskZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mskZipPlusFour = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cbStates = new System.Windows.Forms.ComboBox();
            this.grpSpouseEmployer = new System.Windows.Forms.GroupBox();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpEmployer.SuspendLayout();
            this.grpSpouseEmployer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add( this.lblSpouseEmployed );
            this.panel2.Controls.Add( this.rbQuestion2Yes );
            this.panel2.Controls.Add( this.rbQuestion2Retired );
            this.panel2.Controls.Add( this.rbQuestion2Never );
            this.panel2.Location = new System.Drawing.Point( 16, 232 );
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size( 600, 24 );
            this.panel2.TabIndex = 4;
            this.panel2.TabStop = true;
            // 
            // lblSpouseEmployed
            // 
            this.lblSpouseEmployed.Location = new System.Drawing.Point( 0, 0 );
            this.lblSpouseEmployed.Name = "lblSpouseEmployed";
            this.lblSpouseEmployed.Size = new System.Drawing.Size( 201, 23 );
            this.lblSpouseEmployed.TabIndex = 0;
            this.lblSpouseEmployed.Text = "2. Is your spouse currently employed?";
            // 
            // rbQuestion2Yes
            // 
            this.rbQuestion2Yes.Location = new System.Drawing.Point( 320, 0 );
            this.rbQuestion2Yes.Name = "rbQuestion2Yes";
            this.rbQuestion2Yes.Size = new System.Drawing.Size( 56, 24 );
            this.rbQuestion2Yes.TabIndex = 1;
            this.rbQuestion2Yes.TabStop = true;
            this.rbQuestion2Yes.Text = "Yes";
            this.rbQuestion2Yes.CheckedChanged += new System.EventHandler( this.rbQuestion2Yes_CheckedChanged );
            // 
            // rbQuestion2Retired
            // 
            this.rbQuestion2Retired.Location = new System.Drawing.Point( 378, 0 );
            this.rbQuestion2Retired.Name = "rbQuestion2Retired";
            this.rbQuestion2Retired.Size = new System.Drawing.Size( 88, 24 );
            this.rbQuestion2Retired.TabIndex = 2;
            this.rbQuestion2Retired.TabStop = true;
            this.rbQuestion2Retired.Text = "No - Retired";
            this.rbQuestion2Retired.CheckedChanged += new System.EventHandler( this.rbQuestion2Retired_CheckedChanged );
            // 
            // rbQuestion2Never
            // 
            this.rbQuestion2Never.Location = new System.Drawing.Point( 472, 0 );
            this.rbQuestion2Never.Name = "rbQuestion2Never";
            this.rbQuestion2Never.Size = new System.Drawing.Size( 135, 24 );
            this.rbQuestion2Never.TabIndex = 3;
            this.rbQuestion2Never.TabStop = true;
            this.rbQuestion2Never.Text = "No - Never employed";
            this.rbQuestion2Never.CheckedChanged += new System.EventHandler( this.rbQuestion2Never_CheckedChanged );
            // 
            // panel1
            // 
            this.panel1.Controls.Add( this.lblCurrentlyEmployed );
            this.panel1.Controls.Add( this.rbQuestion1Never );
            this.panel1.Controls.Add( this.rbQuestion1Retired );
            this.panel1.Controls.Add( this.rbQuestion1Yes );
            this.panel1.Location = new System.Drawing.Point( 16, 64 );
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size( 600, 24 );
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // lblCurrentlyEmployed
            // 
            this.lblCurrentlyEmployed.Location = new System.Drawing.Point( 0, 0 );
            this.lblCurrentlyEmployed.Name = "lblCurrentlyEmployed";
            this.lblCurrentlyEmployed.Size = new System.Drawing.Size( 216, 23 );
            this.lblCurrentlyEmployed.TabIndex = 0;
            this.lblCurrentlyEmployed.Text = "1. Are you currently employed?";
            // 
            // rbQuestion1Never
            // 
            this.rbQuestion1Never.Location = new System.Drawing.Point( 472, 0 );
            this.rbQuestion1Never.Name = "rbQuestion1Never";
            this.rbQuestion1Never.Size = new System.Drawing.Size( 130, 24 );
            this.rbQuestion1Never.TabIndex = 3;
            this.rbQuestion1Never.TabStop = true;
            this.rbQuestion1Never.Text = "No - Never employed";
            this.rbQuestion1Never.CheckedChanged += new System.EventHandler( this.rbQuestion1Never_CheckedChanged );
            // 
            // rbQuestion1Retired
            // 
            this.rbQuestion1Retired.Location = new System.Drawing.Point( 378, 0 );
            this.rbQuestion1Retired.Name = "rbQuestion1Retired";
            this.rbQuestion1Retired.Size = new System.Drawing.Size( 86, 24 );
            this.rbQuestion1Retired.TabIndex = 2;
            this.rbQuestion1Retired.TabStop = true;
            this.rbQuestion1Retired.Text = "No - Retired";
            this.rbQuestion1Retired.CheckedChanged += new System.EventHandler( this.rbQuestion1Retired_CheckedChanged );
            // 
            // rbQuestion1Yes
            // 
            this.rbQuestion1Yes.Location = new System.Drawing.Point( 320, 0 );
            this.rbQuestion1Yes.Name = "rbQuestion1Yes";
            this.rbQuestion1Yes.Size = new System.Drawing.Size( 50, 24 );
            this.rbQuestion1Yes.TabIndex = 1;
            this.rbQuestion1Yes.TabStop = true;
            this.rbQuestion1Yes.Text = "Yes";
            this.rbQuestion1Yes.CheckedChanged += new System.EventHandler( this.rbQuestion1Yes_CheckedChanged );
            // 
            // lblRetirmentDate
            // 
            this.lblRetirmentDate.Enabled = false;
            this.lblRetirmentDate.Location = new System.Drawing.Point( 27, 96 );
            this.lblRetirmentDate.Name = "lblRetirmentDate";
            this.lblRetirmentDate.TabIndex = 0;
            this.lblRetirmentDate.Text = "Date of retirement:";
            // 
            // txtTitle
            // 
            this.txtTitle.BackColor = System.Drawing.Color.White;
            this.txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTitle.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.txtTitle.Location = new System.Drawing.Point( 16, 16 );
            this.txtTitle.Multiline = true;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size( 152, 23 );
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Text = "Entitlement by Age";
            // 
            // mskRetirementDate
            // 
            this.mskRetirementDate.BackColor = System.Drawing.Color.White;
            this.mskRetirementDate.Enabled = false;
            this.mskRetirementDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskRetirementDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mskRetirementDate.Location = new System.Drawing.Point( 122, 96 );
            this.mskRetirementDate.Mask = "  /  /";
            this.mskRetirementDate.MaxLength = 10;
            this.mskRetirementDate.Name = "mskRetirementDate";
            this.mskRetirementDate.Size = new System.Drawing.Size( 70, 20 );
            this.mskRetirementDate.TabIndex = 2;
            this.mskRetirementDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mskRetirementDate.Leave += new System.EventHandler( this.mskRetirementDate_Leave );
            this.mskRetirementDate.TextChanged += new System.EventHandler( this.mskRetirementDate_TextChanged );
            this.mskRetirementDate.Enter += new System.EventHandler( this.mskRetirementDate_Enter );
            // 
            // grpEmployer
            // 
            this.grpEmployer.Controls.Add( this.lblEmployerInfo );
            this.grpEmployer.Controls.Add( this.btnEditEmployment );
            this.grpEmployer.Enabled = false;
            this.grpEmployer.Location = new System.Drawing.Point( 27, 136 );
            this.grpEmployer.Name = "grpEmployer";
            this.grpEmployer.Size = new System.Drawing.Size( 397, 75 );
            this.grpEmployer.TabIndex = 3;
            this.grpEmployer.TabStop = false;
            this.grpEmployer.Text = "Employer";
            // 
            // lblEmployerInfo
            // 
            this.lblEmployerInfo.Location = new System.Drawing.Point( 12, 21 );
            this.lblEmployerInfo.Name = "lblEmployerInfo";
            this.lblEmployerInfo.Size = new System.Drawing.Size( 182, 40 );
            this.lblEmployerInfo.TabIndex = 0;
            // 
            // btnEditEmployment
            // 
            this.btnEditEmployment.Location = new System.Drawing.Point( 204, 21 );
            this.btnEditEmployment.Name = "btnEditEmployment";
            this.btnEditEmployment.Size = new System.Drawing.Size( 180, 23 );
            this.btnEditEmployment.TabIndex = 1;
            this.btnEditEmployment.Text = "Edit E&mployment && Cancel MSP";
            this.btnEditEmployment.Click += new System.EventHandler( this.btnEditEmployment_Click );
            // 
            // lblStaticRetirementDate
            // 
            this.lblStaticRetirementDate.Enabled = false;
            this.lblStaticRetirementDate.Location = new System.Drawing.Point( 27, 264 );
            this.lblStaticRetirementDate.Name = "lblStaticRetirementDate";
            this.lblStaticRetirementDate.Size = new System.Drawing.Size( 98, 23 );
            this.lblStaticRetirementDate.TabIndex = 0;
            this.lblStaticRetirementDate.Text = "Date of retirement:";
            // 
            // mskSpouseRetirementDate
            // 
            this.mskSpouseRetirementDate.BackColor = System.Drawing.Color.White;
            this.mskSpouseRetirementDate.Enabled = false;
            this.mskSpouseRetirementDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskSpouseRetirementDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mskSpouseRetirementDate.Location = new System.Drawing.Point( 123, 261 );
            this.mskSpouseRetirementDate.Mask = "  /  /";
            this.mskSpouseRetirementDate.MaxLength = 10;
            this.mskSpouseRetirementDate.Name = "mskSpouseRetirementDate";
            this.mskSpouseRetirementDate.Size = new System.Drawing.Size( 70, 20 );
            this.mskSpouseRetirementDate.TabIndex = 5;
            this.mskSpouseRetirementDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mskSpouseRetirementDate.Leave += new System.EventHandler( this.mskSpouseRetirementDate_Leave );
            this.mskSpouseRetirementDate.TextChanged += new System.EventHandler( this.mskSpouseRetirementDate_TextChanged );
            this.mskSpouseRetirementDate.Enter += new System.EventHandler( this.mskSpouseRetirementDate_Enter );
            // 
            // lblStaticEmployerName
            // 
            this.lblStaticEmployerName.Location = new System.Drawing.Point( 8, 18 );
            this.lblStaticEmployerName.Name = "lblStaticEmployerName";
            this.lblStaticEmployerName.Size = new System.Drawing.Size( 87, 23 );
            this.lblStaticEmployerName.TabIndex = 0;
            this.lblStaticEmployerName.Text = "Employer name:";
            // 
            // mskEmployerName
            // 
            this.mskEmployerName.BackColor = System.Drawing.Color.White;
            this.mskEmployerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerName.Location = new System.Drawing.Point( 92, 15 );
            this.mskEmployerName.Mask = "";
            this.mskEmployerName.MaxLength = Employer.PBAR_EMP_NAME_LENGTH;
            this.mskEmployerName.Name = "mskEmployerName";
            this.mskEmployerName.Size = new System.Drawing.Size( 295, 20 );
            this.mskEmployerName.TabIndex = 1;
            this.mskEmployerName.Leave += new System.EventHandler( this.mskEmployerName_Leave );
            this.mskEmployerName.TextChanged += new System.EventHandler( this.mskEmployerName_TextChanged );
            this.mskEmployerName.Enter += new System.EventHandler( this.mskEmployerName_Enter );
            // 
            // lblStaticAddress
            // 
            this.lblStaticAddress.Location = new System.Drawing.Point( 8, 44 );
            this.lblStaticAddress.Name = "lblStaticAddress";
            this.lblStaticAddress.Size = new System.Drawing.Size( 53, 23 );
            this.lblStaticAddress.TabIndex = 0;
            this.lblStaticAddress.Text = "Address:";
            // 
            // mskEmployerAddress
            // 
            this.mskEmployerAddress.BackColor = System.Drawing.Color.White;
            this.mskEmployerAddress.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerAddress.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerAddress.Location = new System.Drawing.Point( 92, 41 );
            this.mskEmployerAddress.Mask = "";
            this.mskEmployerAddress.MaxLength = 25;
            this.mskEmployerAddress.Name = "mskEmployerAddress";
            this.mskEmployerAddress.Size = new System.Drawing.Size( 188, 20 );
            this.mskEmployerAddress.TabIndex = 2;
            this.mskEmployerAddress.Leave += new System.EventHandler( this.mskEmployerAddress_Leave );
            this.mskEmployerAddress.TextChanged += new System.EventHandler( this.mskEmployerAddress_TextChanged );
            this.mskEmployerAddress.Enter += new System.EventHandler( this.mskEmployerAddress_Enter );
            // 
            // lblStaticCity
            // 
            this.lblStaticCity.Location = new System.Drawing.Point( 8, 70 );
            this.lblStaticCity.Name = "lblStaticCity";
            this.lblStaticCity.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticCity.TabIndex = 0;
            this.lblStaticCity.Text = "City:";
            // 
            // mskEmployerCity
            // 
            this.mskEmployerCity.BackColor = System.Drawing.Color.White;
            this.mskEmployerCity.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerCity.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerCity.Location = new System.Drawing.Point( 92, 67 );
            this.mskEmployerCity.Mask = "";
            this.mskEmployerCity.MaxLength = 17;
            this.mskEmployerCity.Name = "mskEmployerCity";
            this.mskEmployerCity.Size = new System.Drawing.Size( 130, 20 );
            this.mskEmployerCity.TabIndex = 3;
            this.mskEmployerCity.Leave += new System.EventHandler( this.mskEmployerCity_Leave );
            this.mskEmployerCity.TextChanged += new System.EventHandler( this.mskEmployerCity_TextChanged );
            this.mskEmployerCity.Enter += new System.EventHandler( this.mskEmployerCity_Enter );
            // 
            // lblStaticState
            // 
            this.lblStaticState.Location = new System.Drawing.Point( 9, 98 );
            this.lblStaticState.Name = "lblStaticState";
            this.lblStaticState.Size = new System.Drawing.Size( 40, 23 );
            this.lblStaticState.TabIndex = 0;
            this.lblStaticState.Text = "State:";
            // 
            // lblStaticZipCode
            // 
            this.lblStaticZipCode.Location = new System.Drawing.Point( 224, 98 );
            this.lblStaticZipCode.Name = "lblStaticZipCode";
            this.lblStaticZipCode.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticZipCode.TabIndex = 0;
            this.lblStaticZipCode.Text = "Zip Code:";
            // 
            // lblHyphen
            // 
            this.lblHyphen.Location = new System.Drawing.Point( 320, 98 );
            this.lblHyphen.Name = "lblHyphen";
            this.lblHyphen.Size = new System.Drawing.Size( 13, 23 );
            this.lblHyphen.TabIndex = 0;
            this.lblHyphen.Text = "-";
            // 
            // mskZipCode
            // 
            this.mskZipCode.BackColor = System.Drawing.Color.White;
            this.mskZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskZipCode.Location = new System.Drawing.Point( 280, 96 );
            this.mskZipCode.Mask = "";
            this.mskZipCode.MaxLength = 5;
            this.mskZipCode.Name = "mskZipCode";
            this.mskZipCode.Size = new System.Drawing.Size( 38, 20 );
            this.mskZipCode.TabIndex = 5;
            this.mskZipCode.Leave += new System.EventHandler( this.mskZipCode_Leave );
            this.mskZipCode.TextChanged += new System.EventHandler( this.mskZipCode_TextChanged );
            this.mskZipCode.Enter += new System.EventHandler( this.mskZipCode_Enter );
            // 
            // mskZipPlusFour
            // 
            this.mskZipPlusFour.BackColor = System.Drawing.Color.White;
            this.mskZipPlusFour.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskZipPlusFour.Location = new System.Drawing.Point( 336, 96 );
            this.mskZipPlusFour.Mask = "";
            this.mskZipPlusFour.MaxLength = 4;
            this.mskZipPlusFour.Name = "mskZipPlusFour";
            this.mskZipPlusFour.Size = new System.Drawing.Size( 32, 20 );
            this.mskZipPlusFour.TabIndex = 6;
            this.mskZipPlusFour.Leave += new System.EventHandler( this.mskZipPlusFour_Leave );
            // 
            // cbStates
            // 
            this.cbStates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStates.Location = new System.Drawing.Point( 47, 96 );
            this.cbStates.Name = "cbStates";
            this.cbStates.Size = new System.Drawing.Size( 165, 21 );
            this.cbStates.TabIndex = 4;
            this.cbStates.DropDown += new System.EventHandler( this.cbStates_DropDown );
            this.cbStates.SelectedIndexChanged += new System.EventHandler( this.cbStates_SelectedIndexChanged );
            this.cbStates.Leave += new System.EventHandler( this.cbStates_Leave );
            // 
            // grpSpouseEmployer
            // 
            this.grpSpouseEmployer.Controls.Add( this.cbStates );
            this.grpSpouseEmployer.Controls.Add( this.mskZipCode );
            this.grpSpouseEmployer.Controls.Add( this.mskEmployerName );
            this.grpSpouseEmployer.Controls.Add( this.lblStaticState );
            this.grpSpouseEmployer.Controls.Add( this.lblStaticCity );
            this.grpSpouseEmployer.Controls.Add( this.mskEmployerCity );
            this.grpSpouseEmployer.Controls.Add( this.lblStaticZipCode );
            this.grpSpouseEmployer.Controls.Add( this.lblHyphen );
            this.grpSpouseEmployer.Controls.Add( this.lblStaticAddress );
            this.grpSpouseEmployer.Controls.Add( this.mskZipPlusFour );
            this.grpSpouseEmployer.Controls.Add( this.lblStaticEmployerName );
            this.grpSpouseEmployer.Controls.Add( this.mskEmployerAddress );
            this.grpSpouseEmployer.Enabled = false;
            this.grpSpouseEmployer.Location = new System.Drawing.Point( 27, 307 );
            this.grpSpouseEmployer.Name = "grpSpouseEmployer";
            this.grpSpouseEmployer.Size = new System.Drawing.Size( 397, 124 );
            this.grpSpouseEmployer.TabIndex = 6;
            this.grpSpouseEmployer.TabStop = false;
            this.grpSpouseEmployer.Text = "Spouse\'s employer";
            // 
            // AgeEntitlementPage1View
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.grpSpouseEmployer );
            this.Controls.Add( this.mskRetirementDate );
            this.Controls.Add( this.mskSpouseRetirementDate );
            this.Controls.Add( this.lblStaticRetirementDate );
            this.Controls.Add( this.grpEmployer );
            this.Controls.Add( this.panel2 );
            this.Controls.Add( this.panel1 );
            this.Controls.Add( this.lblRetirmentDate );
            this.Controls.Add( this.txtTitle );
            this.Name = "AgeEntitlementPage1View";
            this.Size = new System.Drawing.Size( 682, 520 );
            this.Load += new System.EventHandler( this.AgeEntitlementPage1View_Load );
            this.panel2.ResumeLayout( false );
            this.panel1.ResumeLayout( false );
            this.grpEmployer.ResumeLayout( false );
            this.grpSpouseEmployer.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        #region Construction
        public AgeEntitlementPage1View( MSPDialog form )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            ConfigureControls();
            EnableThemesOn( this );
            parentForm = form;
            formActivating = true;  // Used in setting radio button states
            employment = new Employment();
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private LoggingButton btnEditEmployment;

        private ComboBox cbStates;

        private GroupBox grpEmployer;
        private GroupBox grpSpouseEmployer;

        private Panel panel1;
        private Panel panel2;

        private Label lblSpouseEmployed;
        private Label lblCurrentlyEmployed;
        private Label lblRetirmentDate;
        private Label lblStaticEmployerName;
        private Label lblStaticAddress;
        private Label lblStaticCity;
        private Label lblStaticState;
        private Label lblStaticZipCode;
        private Label lblHyphen;
        private Label lblStaticRetirementDate;
        private Label lblEmployerInfo;

        private RadioButton rbQuestion1Yes;
        private RadioButton rbQuestion1Retired;
        private RadioButton rbQuestion1Never;
        private RadioButton rbQuestion2Yes;
        private RadioButton rbQuestion2Retired;
        private RadioButton rbQuestion2Never;

        private MaskedEditTextBox mskRetirementDate;
        private MaskedEditTextBox mskSpouseRetirementDate;
        private MaskedEditTextBox mskZipCode;
        private MaskedEditTextBox mskZipPlusFour;
        private MaskedEditTextBox mskEmployerName;
        private MaskedEditTextBox mskEmployerAddress;
        private MaskedEditTextBox mskEmployerCity;

        private NonSelectableTextBox txtTitle;

        private Account i_account;
        private Employment employment;
        private MSPDialog parentForm;
        private bool formActivating;
        private static bool formWasChanged;
        private bool question1Response;
        private bool question2Response;
        private bool dateFieldError;
        private bool spouseDateFieldError;
        private int response;
        #endregion

        #region Constants
        const Int32 WM_USER = 0x400;
        const Int32 CONTINUE_BUTTON_DISABLED = WM_USER + 1;
        const Int32 CONTINUE_BUTTON_ENABLED = WM_USER + 2;
        const Int32 CONTINUE_BUTTON_FOCUS = WM_USER + 3;
        const Int32 NEXT_BUTTON_DISABLED = WM_USER + 4;
        const Int32 NEXT_BUTTON_ENABLED = WM_USER + 5;
        const Int32 NEXT_BUTTON_FOCUS = WM_USER + 6;
        #endregion
    }
}
