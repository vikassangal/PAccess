using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for DisabilityEntitlementPage2View.
    /// </summary>
    public class DisabilityEntitlementPage2View : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void DisabilityEntitlementPage2View_Load(object sender, EventArgs e)
        {
            if( (Model_MedicareSecondaryPayor.MedicareEntitlement as
                DisabilityEntitlement).FamilyMemberEmployment == null )
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment = new Employment();
            }
        }

        private void btnCopyFromInsured_Click(object sender, EventArgs e)
        {
            if( Model_Account.PrimaryInsured.Employment != null )
            {
                if( Model_Account.Patient.Employment == null ||
                    Model_Account.Patient.Employment.Employer == null ||
                    Model_Account.Patient.Employment.Employer.PartyContactPoint == null ||
                    Model_Account.Patient.Employment.Employer.PartyContactPoint.Address == null )
                {
                    return;
                }

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment = (Employment) Model_Account.Patient.Employment.Clone();

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment.Status = EmploymentStatus.NewFullTimeEmployed();

                if( Model_Account.Patient.Employment.Employer.Name.Length > 0 )
                {
                    mskEmployerName.Text = Model_Account.Patient.Employment.Employer.Name;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.Name = mskEmployerName.Text;
                    UIColors.SetNormalBgColor( mskEmployerName );
                }
                if( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.Address1.Length > 0 )
                {
                    mskEmployerAddress.Text = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.Address1;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.Address1 = mskEmployerAddress.Text;
                    UIColors.SetNormalBgColor( mskEmployerAddress );
                }
                if( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.City.Length > 0 )
                {
                    mskEmployerCity.Text = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.City;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.City = mskEmployerCity.Text;
                    UIColors.SetNormalBgColor( mskEmployerCity );
                }
                if( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary.Length > 0 )
                {
                    mskZipCode.Text = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary = mskZipCode.Text;
                    UIColors.SetNormalBgColor( mskZipCode );
                }
                if( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended.Length > 0 )
                {
                    mskZipPlusFour.Text = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended = mskZipPlusFour.Text;
                }
                if( Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.State != null )
                {
                    State theState = Model_Account.Patient.Employment.Employer.PartyContactPoint.Address.State;
                    (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.State = (State) theState.Clone();

                    cbStates.SelectedIndex = cbStates.FindString( theState.ToString().ToUpper() );
                    if( cbStates.SelectedIndex > 1 )
                    {
                        UIColors.SetNormalBgColor( cbStates );
                    }
                }
            }
        }

        private void mskEmployerName_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            (Model_MedicareSecondaryPayor.MedicareEntitlement as
                        DisabilityEntitlement).FamilyMemberEmployment.Employer.Name  = mtb.Text;
        }

        private void mskEmployerName_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }
        
        private void mskEmployerName_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                FormCanTransition();
            }
        }

        private void mskEmployerName_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            (Model_MedicareSecondaryPayor.MedicareEntitlement as
                DisabilityEntitlement).FamilyMemberEmployment.Employer.Name = mtb.Text.Trim();

            if( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if( (bool) Tag == true )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void mskEmployerAddress_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskEmployerAddress_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                FormCanTransition();
            }
        }

        private void mskEmployerAddress_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            (Model_MedicareSecondaryPayor.MedicareEntitlement as
                DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.Address1 = mtb.Text.Trim();

            if( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if( (bool) Tag == true )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void mskEmployerCity_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskEmployerCity_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
                FormCanTransition();
            }
        }

        private void mskEmployerCity_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            (Model_MedicareSecondaryPayor.MedicareEntitlement as
                DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.City = mtb.Text.Trim();

            if( mtb.Text.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
            }
            else if( (bool) Tag == true )
            {
                formWasChanged = true;
                Tag = false;
                parentForm.ClearLinkLabels();
            }
        }

        private void cbStates_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cbStates_Leave(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex < 1 )
            {
                UIColors.SetRequiredBgColor( cb );
                FormCanTransition();
            }
        }

        private void cbStates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex > 0 )
            {
                UIColors.SetNormalBgColor( cb );

                State state = cb.SelectedItem as State;
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.State = (State) state.Clone();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
            FormCanTransition();
        }

        private void mskZipCode_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mskZipCode_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Length == 0 )
            {
                UIColors.SetRequiredBgColor( mtb );
                FormCanTransition();
            }
            else if( mtb.UnMaskedText.Length == 1 )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            if( mtb.UnMaskedText.Length == 5 )
            {
                FormCanTransition();
            }
        }

        private void mskZipCode_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if( mtb.Text.Length != 5 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( "The primary zip code must be 5 digits.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodePrimary = mtb.Text;
                UIColors.SetNormalBgColor( mtb );
                Refresh();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
            FormCanTransition();
        }

        private void mskZipPlusFour_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if( mtb.Text.Length != 4 )
            {
                mtb.Focus();
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( "The extended zip code must be 4 digits.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
            }
            else
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).FamilyMemberEmployment.Employer.PartyContactPoint.Address.ZipCode.ZipCodeExtended = mtb.Text;
                UIColors.SetNormalBgColor( mtb );
                Refresh();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion3Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question3Response = true;
                SetQuestion4State( true );

                Model_MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.SetYes();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion3No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question3Response = true;
                SetQuestion4State( false );
                ClearPage3Data();
                SetFamilyEmployerControlState( false );

                Model_MedicareSecondaryPayor.MedicareEntitlement.GroupHealthPlanCoverage.SetNo();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion4Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question4Response = true;
                DisplayFamilyMemberEmploymentInfo( true );

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).GHPCoverageOtherThanSpouse.SetYes();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion4No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question4Response = true;
                SetFamilyEmployerControlState( false );

                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).GHPCoverageOtherThanSpouse.SetNo();

                //ClearPage3Data();
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
            InitializeStates();

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

                    DisabilityEntitlement disabilityEntitlement =  
                        Model_Account.MedicareSecondaryPayor.MedicareEntitlement as 
                        DisabilityEntitlement;

                    flag = disabilityEntitlement.GroupHealthPlanCoverage;

                    if( flag.Code.Equals( "Y" ) )
                    {
                        rbQuestion3Yes.Checked = true;

                        flag = (Model_Account.MedicareSecondaryPayor.MedicareEntitlement as
                            DisabilityEntitlement).GHPCoverageOtherThanSpouse;

                        if( flag.Code.Equals( "Y" ) )
                        {
                            rbQuestion4Yes.Checked = true;

                            Employment familyMemberEmployment =  disabilityEntitlement.FamilyMemberEmployment;
                            UpdateSpouseEmployerInformation( familyMemberEmployment );
                            DisplayFamilyMemberEmploymentInfo( true );

                        }
                        else if( flag.Code.Equals( "N" ) )
                        {
                            rbQuestion4No.Checked = true;
                        }
                    }
                    else if( flag.Code.Equals( "N" ) )
                    {
                        rbQuestion3No.Checked = true;
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
        private void UpdateSpouseEmployerInformation( Employment familyMemberEmployment )
        {
            this.mskEmployerName.Text =  familyMemberEmployment.Employer.Name;
            Address employerAddress = familyMemberEmployment.Employer.PartyContactPoint.Address;

            mskEmployerAddress.Text =  employerAddress.Address1;
            mskEmployerCity.Text = employerAddress.City;
            cbStates.Text = employerAddress.State.Description;
            mskZipCode.Text = employerAddress.ZipCode.ZipCodePrimary;
            mskZipPlusFour.Text = employerAddress.ZipCode.ZipCodeExtended; 
        }


        /// <summary>
        /// Clears the data on page 3 so the summary results come out correctly.  If the
        /// user comes from page 3 back to page 2 and makes a change that causes page 3
        /// not to be relevant, the data that was set on that page must be removed so it
        /// is not analyzed for the summary.
        /// </summary>
        private void ClearPage3Data()
        {
            (Model_MedicareSecondaryPayor.MedicareEntitlement as
                DisabilityEntitlement).GHPLimitExceeded.SetBlank();
            DisabilityEntitlementPage3View.formActivating = true;
        }

        private void DisplayFamilyMemberEmploymentInfo( bool state )
        {
            SetFamilyEmployerControlState( state );

            if( state )
            {
                    if( mskEmployerName.Text.Length == 0 )
                    {
                        UIColors.SetRequiredBgColor( mskEmployerName );
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mskEmployerName );
                    }

                    if( mskEmployerAddress.Text.Length == 0 )
                    {
                        UIColors.SetRequiredBgColor( mskEmployerAddress );
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mskEmployerAddress );
                    }

                    if( mskEmployerCity.Text.Length == 0 )
                    {
                        UIColors.SetRequiredBgColor( mskEmployerCity );
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mskEmployerCity );
                    }

                    if( cbStates.SelectedIndex < 1 )
                    {
                        UIColors.SetRequiredBgColor( cbStates );
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( cbStates );
                    }

                    if( mskZipCode.Text.Length == 0 )
                    {
                        UIColors.SetRequiredBgColor( mskZipCode );
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mskZipCode );
                    }

                btnCopyFromInsured.Enabled = (DisabilityEntitlementPage1View.enableEmploymentButton &&
                    Model_Account.PrimaryInsured.Employment != null);
            }
        }

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            if( question3Response && question4Response )
            {
                if( rbQuestion4Yes.Checked )
                {   // GHP is primary
                    if( EmployerControlsValid() == false )
                    {
                        SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                        SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                        return;
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
                {   // Medicare is primary
                    response = MSPEventCode.YesStimulus();
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
                }
            }
            else if( question3Response )
            {
                if( rbQuestion3No.Checked )
                {
                    response = MSPEventCode.NoStimulus();
                    SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
                }
                else
                {
                    SendMessage( parentForm.Handle, CONTINUE_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
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
            if( cbStates.Items.Count == 0 )
            {
                try
                {
                    IAddressBroker broker = new AddressBrokerProxy();
                    ICollection states = broker.AllStates(User.GetCurrent().Facility.Oid);

                    if( states == null )
                    {
                        MessageBox.Show( "IAddressBroker.AllStates() returned empty list.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error );
                        return;
                    }

                    foreach( State state in states )
                    {
                        cbStates.Items.Add( state );
                    }
                }
                catch( Exception ex )
                {
                    MessageBox.Show( "IAddressBroker failed: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }

        private void ResetView()
        {
            SetFamilyEmployerControlState( false );
            rbQuestion3No.Checked = false;
            rbQuestion3Yes.Checked = false;
            rbQuestion4Yes.Checked = false;
            rbQuestion4No.Checked = false;
            mskZipCode.Text = String.Empty;
            mskEmployerName.Text = String.Empty;
            mskEmployerCity.Text = String.Empty;
            mskZipPlusFour.Text = String.Empty;
            mskEmployerAddress.Text = String.Empty;
            if( cbStates.Items.Count > 0 )
            {
                cbStates.SelectedIndex = 0;
            }
            question3Response = false;
            question4Response = false;
            formActivating = false;
        }
        /// <summary>
        /// Set the Enable property of groupBox, clear text, and reset background colors.
        /// </summary>
        private void SetFamilyEmployerControlState( bool state )
        {
            if( state == false )
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
            }
            grpSpouseEmployer.Enabled = state;
        }

        /// <summary>
        /// Enable or disable the controls for question 4
        /// </summary>
        private void SetQuestion4State( bool state )
        {
            if( state == false )
            {
                (Model_MedicareSecondaryPayor.MedicareEntitlement as
                    DisabilityEntitlement).GHPCoverageOtherThanSpouse.SetBlank();
            }

            panel2.Enabled         = state;
            rbQuestion4Yes.Checked = false;
            rbQuestion4No.Checked  = false;
            question4Response = false;
        }

        /// <summary>
        /// Validates the employer fields and returns true if invalid since
        /// </summary>
        /// <returns></returns>
        private bool EmployerControlsValid()
        {
            bool result = true;

            if( mskEmployerName.Text.Length == 0 )
            {
                result = false;
            }
            else if( mskEmployerAddress.Text.Length == 0 )
            {
                result = false;
            }
            else if( mskEmployerCity.Text.Length == 0 )
            {
                result = false;
            }
            else if( cbStates.SelectedIndex < 1 )
            {
                result = false;
            }
            else if( mskZipCode.Text.Length != 5 )
            {
                result = false;
            }
            return result;
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblQuestion3a = new System.Windows.Forms.Label();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.rbQuestion3No = new System.Windows.Forms.RadioButton();
            this.rbQuestion3Yes = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblQuestion4a = new System.Windows.Forms.Label();
            this.lblQuestion4 = new System.Windows.Forms.Label();
            this.rbQuestion4Yes = new System.Windows.Forms.RadioButton();
            this.rbQuestion4No = new System.Windows.Forms.RadioButton();
            this.txtTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.grpSpouseEmployer = new System.Windows.Forms.GroupBox();
            this.mskEmployerName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mskEmployerAddress = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnCopyFromInsured = new LoggingButton();
            this.cbStates = new System.Windows.Forms.ComboBox();
            this.mskZipCode = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticState = new System.Windows.Forms.Label();
            this.lblStaticCity = new System.Windows.Forms.Label();
            this.mskEmployerCity = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticZipCode = new System.Windows.Forms.Label();
            this.lblHyphen = new System.Windows.Forms.Label();
            this.lblStaticAddress = new System.Windows.Forms.Label();
            this.mskZipPlusFour = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEmployerName = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grpSpouseEmployer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblQuestion3a);
            this.panel1.Controls.Add(this.lblQuestion3);
            this.panel1.Controls.Add(this.rbQuestion3No);
            this.panel1.Controls.Add(this.rbQuestion3Yes);
            this.panel1.Location = new System.Drawing.Point(16, 64);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 32);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
            // 
            // lblQuestion3a
            // 
            this.lblQuestion3a.Location = new System.Drawing.Point(11, 14);
            this.lblQuestion3a.Name = "lblQuestion3a";
            this.lblQuestion3a.Size = new System.Drawing.Size(200, 23);
            this.lblQuestion3a.TabIndex = 0;
            this.lblQuestion3a.Text = "family member\'s current employment?";
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(392, 40);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3. Do you have group health plan (GHP) coverage based on your own, or a ";
            // 
            // rbQuestion3No
            // 
            this.rbQuestion3No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion3No.Name = "rbQuestion3No";
            this.rbQuestion3No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion3No.TabIndex = 2;
            this.rbQuestion3No.TabStop = true;
            this.rbQuestion3No.Text = "No";
            this.rbQuestion3No.CheckedChanged += new System.EventHandler(this.rbQuestion3No_CheckedChanged);
            // 
            // rbQuestion3Yes
            // 
            this.rbQuestion3Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion3Yes.Name = "rbQuestion3Yes";
            this.rbQuestion3Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion3Yes.TabIndex = 1;
            this.rbQuestion3Yes.TabStop = true;
            this.rbQuestion3Yes.Text = "Yes";
            this.rbQuestion3Yes.CheckedChanged += new System.EventHandler(this.rbQuestion3Yes_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblQuestion4a);
            this.panel2.Controls.Add(this.lblQuestion4);
            this.panel2.Controls.Add(this.rbQuestion4Yes);
            this.panel2.Controls.Add(this.rbQuestion4No);
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(16, 144);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 32);
            this.panel2.TabIndex = 2;
            this.panel2.TabStop = true;
            // 
            // lblQuestion4a
            // 
            this.lblQuestion4a.Location = new System.Drawing.Point(11, 14);
            this.lblQuestion4a.Name = "lblQuestion4a";
            this.lblQuestion4a.TabIndex = 0;
            this.lblQuestion4a.Text = "your spouse?";
            // 
            // lblQuestion4
            // 
            this.lblQuestion4.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion4.Name = "lblQuestion4";
            this.lblQuestion4.Size = new System.Drawing.Size(408, 23);
            this.lblQuestion4.TabIndex = 0;
            this.lblQuestion4.Text = "4. Are you covered under the group health plan of a family member other than";
            // 
            // rbQuestion4Yes
            // 
            this.rbQuestion4Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion4Yes.Name = "rbQuestion4Yes";
            this.rbQuestion4Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion4Yes.TabIndex = 1;
            this.rbQuestion4Yes.TabStop = true;
            this.rbQuestion4Yes.Text = "Yes";
            this.rbQuestion4Yes.CheckedChanged += new System.EventHandler(this.rbQuestion4Yes_CheckedChanged);
            // 
            // rbQuestion4No
            // 
            this.rbQuestion4No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion4No.Name = "rbQuestion4No";
            this.rbQuestion4No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion4No.TabIndex = 2;
            this.rbQuestion4No.TabStop = true;
            this.rbQuestion4No.Text = "No";
            this.rbQuestion4No.CheckedChanged += new System.EventHandler(this.rbQuestion4No_CheckedChanged);
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
            this.txtTitle.Size = new System.Drawing.Size(240, 23);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Text = "Medicare Entitlement - Disability";
            // 
            // grpSpouseEmployer
            // 
            this.grpSpouseEmployer.Controls.Add(this.mskEmployerName);
            this.grpSpouseEmployer.Controls.Add(this.mskEmployerAddress);
            this.grpSpouseEmployer.Controls.Add(this.btnCopyFromInsured);
            this.grpSpouseEmployer.Controls.Add(this.cbStates);
            this.grpSpouseEmployer.Controls.Add(this.mskZipCode);
            this.grpSpouseEmployer.Controls.Add(this.lblStaticState);
            this.grpSpouseEmployer.Controls.Add(this.lblStaticCity);
            this.grpSpouseEmployer.Controls.Add(this.mskEmployerCity);
            this.grpSpouseEmployer.Controls.Add(this.lblStaticZipCode);
            this.grpSpouseEmployer.Controls.Add(this.lblHyphen);
            this.grpSpouseEmployer.Controls.Add(this.lblStaticAddress);
            this.grpSpouseEmployer.Controls.Add(this.mskZipPlusFour);
            this.grpSpouseEmployer.Controls.Add(this.lblStaticEmployerName);
            this.grpSpouseEmployer.Enabled = false;
            this.grpSpouseEmployer.Location = new System.Drawing.Point(27, 190);
            this.grpSpouseEmployer.Name = "grpSpouseEmployer";
            this.grpSpouseEmployer.Size = new System.Drawing.Size(397, 168);
            this.grpSpouseEmployer.TabIndex = 3;
            this.grpSpouseEmployer.TabStop = false;
            this.grpSpouseEmployer.Text = "Family member\'s employer";
            // 
            // mskEmployerName
            // 
            this.mskEmployerName.BackColor = System.Drawing.Color.White;
            this.mskEmployerName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerName.Location = new System.Drawing.Point(92, 64);
            this.mskEmployerName.Mask = "";
            this.mskEmployerName.MaxLength = Employer.PBAR_EMP_NAME_LENGTH;
            this.mskEmployerName.Name = "mskEmployerName";
            this.mskEmployerName.Size = new System.Drawing.Size(295, 20);
            this.mskEmployerName.TabIndex = 2;
            this.mskEmployerName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerName.Leave += new System.EventHandler(this.mskEmployerName_Leave);
            this.mskEmployerName.Validating += new System.ComponentModel.CancelEventHandler(this.mskEmployerName_Validating);
            this.mskEmployerName.TextChanged += new System.EventHandler(this.mskEmployerName_TextChanged);
            this.mskEmployerName.Enter += new System.EventHandler(this.mskEmployerName_Enter);
            // 
            // mskEmployerAddress
            // 
            this.mskEmployerAddress.BackColor = System.Drawing.Color.White;
            this.mskEmployerAddress.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerAddress.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerAddress.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerAddress.Location = new System.Drawing.Point(92, 88);
            this.mskEmployerAddress.Mask = "";
            this.mskEmployerAddress.MaxLength = 25;
            this.mskEmployerAddress.Name = "mskEmployerAddress";
            this.mskEmployerAddress.Size = new System.Drawing.Size(188, 20);
            this.mskEmployerAddress.TabIndex = 3;
            this.mskEmployerAddress.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerAddress.Leave += new System.EventHandler(this.mskEmployerAddress_Leave);
            this.mskEmployerAddress.TextChanged += new System.EventHandler(this.mskEmployerAddress_TextChanged);
            this.mskEmployerAddress.Enter += new System.EventHandler(this.mskEmployerAddress_Enter);
            // 
            // btnCopyFromInsured
            // 
            this.btnCopyFromInsured.Enabled = false;
            this.btnCopyFromInsured.Location = new System.Drawing.Point(136, 24);
            this.btnCopyFromInsured.Name = "btnCopyFromInsured";
            this.btnCopyFromInsured.Size = new System.Drawing.Size(113, 23);
            this.btnCopyFromInsured.TabIndex = 1;
            this.btnCopyFromInsured.Text = "&Copy From Insured";
            this.btnCopyFromInsured.Click += new System.EventHandler(this.btnCopyFromInsured_Click);
            // 
            // cbStates
            // 
            this.cbStates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStates.Location = new System.Drawing.Point(47, 138);
            this.cbStates.Name = "cbStates";
            this.cbStates.Size = new System.Drawing.Size(165, 21);
            this.cbStates.TabIndex = 5;
            this.cbStates.DropDown += new System.EventHandler(this.cbStates_DropDown);
            this.cbStates.SelectedIndexChanged += new System.EventHandler(this.cbStates_SelectedIndexChanged);
            this.cbStates.Leave += new System.EventHandler(this.cbStates_Leave);
            // 
            // mskZipCode
            // 
            this.mskZipCode.BackColor = System.Drawing.Color.White;
            this.mskZipCode.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskZipCode.KeyPressExpression = "^\\d*$";
            this.mskZipCode.Location = new System.Drawing.Point(280, 138);
            this.mskZipCode.Mask = "";
            this.mskZipCode.MaxLength = 5;
            this.mskZipCode.Name = "mskZipCode";
            this.mskZipCode.Size = new System.Drawing.Size(38, 20);
            this.mskZipCode.TabIndex = 6;
            this.mskZipCode.ValidationExpression = "^\\d*$";
            this.mskZipCode.Leave += new System.EventHandler(this.mskZipCode_Leave);
            this.mskZipCode.TextChanged += new System.EventHandler(this.mskZipCode_TextChanged);
            this.mskZipCode.Enter += new System.EventHandler(this.mskZipCode_Enter);
            // 
            // lblStaticState
            // 
            this.lblStaticState.Location = new System.Drawing.Point(9, 140);
            this.lblStaticState.Name = "lblStaticState";
            this.lblStaticState.Size = new System.Drawing.Size(40, 23);
            this.lblStaticState.TabIndex = 0;
            this.lblStaticState.Text = "State:";
            // 
            // lblStaticCity
            // 
            this.lblStaticCity.Location = new System.Drawing.Point(8, 115);
            this.lblStaticCity.Name = "lblStaticCity";
            this.lblStaticCity.Size = new System.Drawing.Size(32, 23);
            this.lblStaticCity.TabIndex = 0;
            this.lblStaticCity.Text = "City:";
            // 
            // mskEmployerCity
            // 
            this.mskEmployerCity.BackColor = System.Drawing.Color.White;
            this.mskEmployerCity.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mskEmployerCity.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskEmployerCity.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerCity.Location = new System.Drawing.Point(92, 112);
            this.mskEmployerCity.Mask = "";
            this.mskEmployerCity.MaxLength = 17;
            this.mskEmployerCity.Name = "mskEmployerCity";
            this.mskEmployerCity.Size = new System.Drawing.Size(130, 20);
            this.mskEmployerCity.TabIndex = 4;
            this.mskEmployerCity.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mskEmployerCity.Leave += new System.EventHandler(this.mskEmployerCity_Leave);
            this.mskEmployerCity.TextChanged += new System.EventHandler(this.mskEmployerCity_TextChanged);
            this.mskEmployerCity.Enter += new System.EventHandler(this.mskEmployerCity_Enter);
            // 
            // lblStaticZipCode
            // 
            this.lblStaticZipCode.Location = new System.Drawing.Point(224, 140);
            this.lblStaticZipCode.Name = "lblStaticZipCode";
            this.lblStaticZipCode.Size = new System.Drawing.Size(56, 23);
            this.lblStaticZipCode.TabIndex = 0;
            this.lblStaticZipCode.Text = "Zip Code:";
            // 
            // lblHyphen
            // 
            this.lblHyphen.Location = new System.Drawing.Point(320, 140);
            this.lblHyphen.Name = "lblHyphen";
            this.lblHyphen.Size = new System.Drawing.Size(13, 23);
            this.lblHyphen.TabIndex = 0;
            this.lblHyphen.Text = "-";
            // 
            // lblStaticAddress
            // 
            this.lblStaticAddress.Location = new System.Drawing.Point(8, 91);
            this.lblStaticAddress.Name = "lblStaticAddress";
            this.lblStaticAddress.Size = new System.Drawing.Size(53, 23);
            this.lblStaticAddress.TabIndex = 0;
            this.lblStaticAddress.Text = "Address:";
            // 
            // mskZipPlusFour
            // 
            this.mskZipPlusFour.BackColor = System.Drawing.Color.White;
            this.mskZipPlusFour.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskZipPlusFour.KeyPressExpression = "^\\d*$";
            this.mskZipPlusFour.Location = new System.Drawing.Point(336, 138);
            this.mskZipPlusFour.Mask = "";
            this.mskZipPlusFour.MaxLength = 4;
            this.mskZipPlusFour.Name = "mskZipPlusFour";
            this.mskZipPlusFour.Size = new System.Drawing.Size(32, 20);
            this.mskZipPlusFour.TabIndex = 7;
            this.mskZipPlusFour.ValidationExpression = "^\\d*$";
            this.mskZipPlusFour.Leave += new System.EventHandler(this.mskZipPlusFour_Leave);
            // 
            // lblStaticEmployerName
            // 
            this.lblStaticEmployerName.Location = new System.Drawing.Point(8, 67);
            this.lblStaticEmployerName.Name = "lblStaticEmployerName";
            this.lblStaticEmployerName.Size = new System.Drawing.Size(87, 23);
            this.lblStaticEmployerName.TabIndex = 0;
            this.lblStaticEmployerName.Text = "Employer name:";
            // 
            // DisabilityEntitlementPage2View
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.grpSpouseEmployer);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.txtTitle);
            this.Name = "DisabilityEntitlementPage2View";
            this.Size = new System.Drawing.Size(680, 520);
            this.Load += new System.EventHandler(this.DisabilityEntitlementPage2View_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.grpSpouseEmployer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public DisabilityEntitlementPage2View( MSPDialog form )
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

        private LoggingButton                 btnCopyFromInsured;

        private ComboBox               cbStates;

        private GroupBox               grpSpouseEmployer;

        private Label                  lblQuestion3;
        private Label                  lblQuestion3a;
        private Label                  lblQuestion4;
        private Label                  lblQuestion4a;
        private Label                  lblStaticState;
        private Label                  lblStaticCity;
        private Label                  lblStaticZipCode;
        private Label                  lblHyphen;
        private Label                  lblStaticAddress;
        private Label                  lblStaticEmployerName;

        private Panel                  panel1;
        private Panel                  panel2;

        private RadioButton            rbQuestion3No;
        private RadioButton            rbQuestion3Yes;
        private RadioButton            rbQuestion4Yes;
        private RadioButton            rbQuestion4No;

        private MaskedEditTextBox    mskZipCode;
        private MaskedEditTextBox    mskEmployerName;
        private MaskedEditTextBox    mskEmployerCity;
        private MaskedEditTextBox    mskZipPlusFour;
        private MaskedEditTextBox    mskEmployerAddress;

        private NonSelectableTextBox  txtTitle;

        private Account                                     i_account;
        private MSPDialog                                   parentForm;
        public static bool                                  formActivating;
        private static bool                                  formWasChanged;
        private bool                                        question3Response;
        private bool                                        question4Response;
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
        #endregion        
    }
}
