using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for SpecialProgramView.
    /// </summary>
    public class SpecialProgramView : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void btnEditInsurance_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURANCE );
        }

        private void btnEditDiagnosis_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.DIAGNOSIS );
        }

        private void btnEditPayor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.PAYORDETAILS );
        }

        private void btnEditGuarantor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.GUARANTOR );
        }

        private void mskDateBegan_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText.Length == 0 )
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

        private void mskDateBegan_Leave(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            dateFieldError = false;

            if( mtb.UnMaskedText == String.Empty )
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if( mtb.Text.Length != 10 )
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
                {   // Check the date entered is not in the future
                    benefitsDate = new DateTime( Convert.ToInt32( mtb.Text.Substring( 6, 4 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 0, 2 ) ),
                        Convert.ToInt32( mtb.Text.Substring( 3, 2 ) ) );

                    if( DateValidator.IsValidDate( benefitsDate ) == false )
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
                        Model_MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate = benefitsDate;
                        UIColors.SetNormalBgColor( mtb );
                        Refresh();
                        if( (bool) Tag == true )
                        {
                            formWasChanged = true;
                            Tag = false;
                            parentForm.ClearLinkLabels();
                        }
                        return;
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

        private void rbQuestion1Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question1Response = true;
                SetRetirementControlState( true );
                SetQuestion2State( true );
                DisplayWorkersCompInfo( rbQuestion5Yes.Checked );

                Model_MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.SetYes();

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
                question1Response = true;
                SetRetirementControlState( false );
                SetQuestion2State( false );
                SetRetirementControlState( false );
                DisplayWorkersCompInfo( false );

                Model_MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.SetNo();
                Model_MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate = DateTime.MinValue;

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question2Response = true;

                Model_MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.SetYes();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question2Response = true;

                Model_MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.SetNo();

                FormCanTransition();

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
                DisplayWorkersCompInfo( rbQuestion5Yes.Checked );

                Model_MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.SetYes();

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
                DisplayWorkersCompInfo( false );

                Model_MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.SetNo();

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
                DisplayWorkersCompInfo( rbQuestion5Yes.Checked );

                Model_MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.SetYes();

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
                DisplayWorkersCompInfo( false );

                Model_MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.SetNo();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion5Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question5Response = true;
                DisplayWorkersCompInfo( true );

                Model_MedicareSecondaryPayor.SpecialProgram.WorkRelated.SetYes();

                ClearLiabilityPageData();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion5No_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question5Response = true;
                DisplayWorkersCompInfo( false );

                Model_MedicareSecondaryPayor.SpecialProgram.WorkRelated.SetNo();

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

            if( (bool) Tag == true && FormChanged )
            {   // User went back and made a change
                ResetView();
                return;
            }
            else if( formActivating )
            {
                ResetView();
                SetQuestion2State( false );

                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.Code.Equals( "Y" ) )
                {
                    rbQuestion1Yes.Checked = true;

                    if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate != DateTime.MinValue )
                    {
                        DateTime date = Model_Account.MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate;
                        Model_MedicareSecondaryPayor.SpecialProgram.BLBenefitsStartDate = date;
                        mskDateBegan.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );
                        UIColors.SetNormalBgColor( mskDateBegan );
                    }
                    if( Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code.Equals( "Y" ) )
                    {
                        rbQuestion2Yes.Checked = true;
                    }
                    else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.Code.Equals( "N" ) )
                    {
                        rbQuestion2No.Checked = true;
                    }
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.BlackLungBenefits.Code.Equals( "N" ) )
                {
                    rbQuestion1No.Checked = true;
                }
                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.Code.Equals( "Y" ) )
                {
                    rbQuestion3Yes.Checked = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.GovernmentProgram.Code.Equals( "N" ) )
                {
                    rbQuestion3No.Checked = true;
                }
                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.Code.Equals( "Y" ) )
                {
                    rbQuestion4Yes.Checked = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.DVAAuthorized.Code.Equals( "N" ) )
                {
                    rbQuestion4No.Checked = true;
                }
                if( Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated.Code.Equals( "Y" ) )
                {
                    rbQuestion5Yes.Checked = true;
                }
                else if( Model_Account.MedicareSecondaryPayor.SpecialProgram.WorkRelated.Code.Equals( "N" ) )
                {
                    rbQuestion5No.Checked = true;
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
        private void ClearLiabilityPageData()
        {
            Model_MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.SetBlank();
            Model_MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.SetBlank();
            Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentType = new TypeOfAccident();
            LiabilityInsurerView.formActivating = true;
        }
        /// <summary>
        /// Populate the controls with the patient's illness or injury complain info
        /// </summary>
        private void DisplayWorkersCompInfo( bool state )
        {
            EnableGroupBox( state );

            if( state == true )
            {
                if( Model_Account.Diagnosis != null )
                {
                    if( Model_Account.Diagnosis.Condition.GetType() == typeof( Illness ) )
                    {
                        DateTime date = (Model_Account.Diagnosis.Condition as Illness).Onset;
                        lblDate.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );
                    }
                    else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Crime ) )
                    {
                        DateTime date = (Model_Account.Diagnosis.Condition as Crime).OccurredOn;
                        lblDate.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );
                    }
                    else if( Model_Account.Diagnosis.Condition.GetType() == typeof( Accident ) )
                    {
                        DateTime date = (Model_Account.Diagnosis.Condition as Accident).OccurredOn;
                        lblDate.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );
                    }
                    else
                    {
                        lblDate.Text  = "Unknown condition";
                    }
                }
                if( Model_Account.Guarantor.Employment != null )
                {
                    string guarantor = Model_Account.Guarantor.ContactPointWith(
                        TypeOfContactPoint.NewMailingContactPointType()).Address.AsMailingLabel();

                    if( guarantor != String.Empty )
                    {
                        lblGuarantorInfo.Text = String.Format("{0}{1}{2}", 
                            Model_Account.Guarantor.Employment.Employer.Name, 
                            Environment.NewLine, guarantor );
                    }
                    else
                    {
                        lblGuarantorInfo.Text = "Not available";
                    }
                }
                else
                {
                    lblGuarantorInfo.Text = "Not employed";
                }

                Coverage coverage = GetPrimaryCoverage();

                if( coverage != null )
                {
                    if( coverage.GetType().Equals( typeof( WorkersCompensationCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as WorkersCompensationCoverage).PolicyNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicaidCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as GovernmentMedicaidCoverage).PolicyCINNumber;
                    }
                    else if( coverage.GetType().Equals( typeof( GovernmentMedicareCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as GovernmentMedicareCoverage).MBINumber;
                    }
                    else if( coverage.GetType().Equals( typeof( CommercialCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as CommercialCoverage).CertSSNID;
                    }
                    else if( coverage.GetType().Equals( typeof( CoverageForCommercialOther ) ) )
                    {
                        lblPolicyID.Text = (coverage as CoverageForCommercialOther).CertSSNID;
                    }
                    else if( coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                    {
                        lblPolicyID.Text = (coverage as OtherCoverage).CertSSNID;
                    }
                    else
                    {
                        lblPolicyID.Text = "Not available";
                    }

                    if( coverage.BillingInformation.Address != null )
                    {
                        string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                        if( mailingLabel != String.Empty )
                        {
                            lblInsuranceInfo.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                                Environment.NewLine, mailingLabel );
                        }
                        else
                        {
                            lblInsuranceInfo.Text = coverage.InsurancePlan.PlanName;
                        }
                    }
                    else
                    {
                        lblInsuranceInfo.Text = coverage.InsurancePlan.PlanName;
                    }
//                    lblPolicyID.Text  = coverage.InsurancePlan.PlanID;
                }
                else
                {
                    lblInsuranceInfo.Text = "Not available";
                    lblPolicyID.Text  = "Not available";
                }
            }
        }

        private void EnableGroupBox( bool state )
        {
            if( state == false )
            {
                lblDate.Text          = String.Empty;
                lblPolicyID.Text  = String.Empty;
                lblInsuranceInfo.Text = String.Empty;
                lblGuarantorInfo.Text = String.Empty;
            }
            grpWorkersCompensation.Enabled = state;
        }

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            bool result = false;

            if( question1Response && question3Response &&
                question4Response && question5Response )
            {
                if( rbQuestion1Yes.Checked && question2Response == false )
                {
                    result = false;
                }
                else if( rbQuestion1Yes.Checked && mskDateBegan.UnMaskedText.Length != 8  )
                {
                    result = false;
                }
                else if( rbQuestion5Yes.Checked )
                {   // Workers compensation is primary
                    response = MSPEventCode.YesStimulus();
                    result   = true;
                }
                else
                {
                    response = MSPEventCode.NoStimulus();
                    result   = true;
                }
            }
            if( dateFieldError )
            {
                result = false;
            }
            if( result )
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_ENABLED, IntPtr.Zero, IntPtr.Zero );
                SendMessage( parentForm.Handle, NEXT_BUTTON_FOCUS, IntPtr.Zero, IntPtr.Zero );
            }
            else
            {
                SendMessage( parentForm.Handle, NEXT_BUTTON_DISABLED, IntPtr.Zero, IntPtr.Zero );
            }
        }

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
            rbQuestion1No.Checked = false;
            rbQuestion1Yes.Checked = false;
            rbQuestion2Yes.Checked = false;
            rbQuestion2No.Checked = false;
            rbQuestion3No.Checked = false;
            rbQuestion3Yes.Checked = false;
            rbQuestion4No.Checked = false;
            rbQuestion4Yes.Checked = false;
            rbQuestion5No.Checked = false;
            rbQuestion5Yes.Checked = false;
            question1Response = false;
            question2Response = false;
            question3Response = false;
            question4Response = false;
            question5Response = false;
            formActivating = false;
        }

        private void SetQuestion2State( bool state )
        {
            if( state == false )
            {
                Model_MedicareSecondaryPayor.SpecialProgram.VisitForBlackLung.SetBlank();
            }

            panel2.Enabled         = state;
            rbQuestion2Yes.Checked = false;
            rbQuestion2No.Checked  = false;
            question2Response      = false;
        }


        private void SetRetirementControlState( bool state )
        {
            lblDateBegan.Enabled = state;
            mskDateBegan.Enabled = state;
            dateFieldError       = false;

            if( state )
            {
                if( mskDateBegan.UnMaskedText.Length == 0 )
                {
                    UIColors.SetRequiredBgColor( mskDateBegan );
                }
            }
            else
            {
                mskDateBegan.Text         = String.Empty;
                mskDateBegan.UnMaskedText = String.Empty;
                UIColors.SetNormalBgColor( mskDateBegan );
                Refresh();
            }
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbQuestion3Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.rbQuestion3No = new System.Windows.Forms.RadioButton();
            this.lblTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbQuestion1No = new System.Windows.Forms.RadioButton();
            this.rbQuestion1Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion1 = new System.Windows.Forms.Label();
            this.grpWorkersCompensation = new System.Windows.Forms.GroupBox();
            this.lblGuarantorInfo = new System.Windows.Forms.Label();
            this.lblInsuranceInfo = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.lblPolicyID = new System.Windows.Forms.Label();
            this.lblStaticGuarantor = new System.Windows.Forms.Label();
            this.lblStaticPolicyNumber = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStaticInjuryDate = new System.Windows.Forms.Label();
            this.btnEditGuarantor = new LoggingButton();
            this.btnEditPayor = new LoggingButton();
            this.btnEditDiagnosis = new LoggingButton();
            this.btnEditInsurance = new LoggingButton();
            this.lblDateBegan = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblQuestion3a = new System.Windows.Forms.Label();
            this.rbQuestion4No = new System.Windows.Forms.RadioButton();
            this.rbQuestion4Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rbQuestion5No = new System.Windows.Forms.RadioButton();
            this.rbQuestion5Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion5 = new System.Windows.Forms.Label();
            this.mskDateBegan = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbQuestion2No = new System.Windows.Forms.RadioButton();
            this.rbQuestion2Yes = new System.Windows.Forms.RadioButton();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpWorkersCompensation.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbQuestion3Yes);
            this.panel3.Controls.Add(this.lblQuestion3);
            this.panel3.Controls.Add(this.rbQuestion3No);
            this.panel3.Location = new System.Drawing.Point(16, 168);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(600, 24);
            this.panel3.TabIndex = 4;
            this.panel3.TabStop = true;
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
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(416, 23);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3. Are the services to be paid by a government program such as a research grant?";
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
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.White;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(16, 16);
            this.lblTitle.Multiline = true;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.ReadOnly = true;
            this.lblTitle.Size = new System.Drawing.Size(144, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Special Programs";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbQuestion1No);
            this.panel1.Controls.Add(this.rbQuestion1Yes);
            this.panel1.Controls.Add(this.lblQuestion1);
            this.panel1.Location = new System.Drawing.Point(16, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
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
            // rbQuestion1Yes
            // 
            this.rbQuestion1Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion1Yes.Name = "rbQuestion1Yes";
            this.rbQuestion1Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion1Yes.TabIndex = 1;
            this.rbQuestion1Yes.TabStop = true;
            this.rbQuestion1Yes.Text = "Yes";
            this.rbQuestion1Yes.CheckedChanged += new System.EventHandler(this.rbQuestion1Yes_CheckedChanged);
            // 
            // lblQuestion1
            // 
            this.lblQuestion1.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion1.Name = "lblQuestion1";
            this.lblQuestion1.Size = new System.Drawing.Size(248, 23);
            this.lblQuestion1.TabIndex = 0;
            this.lblQuestion1.Text = "1. Are you receiving Black Lung (BL) benefits?";
            // 
            // grpWorkersCompensation
            // 
            this.grpWorkersCompensation.Controls.Add(this.lblGuarantorInfo);
            this.grpWorkersCompensation.Controls.Add(this.lblInsuranceInfo);
            this.grpWorkersCompensation.Controls.Add(this.lblDate);
            this.grpWorkersCompensation.Controls.Add(this.lblPolicyID);
            this.grpWorkersCompensation.Controls.Add(this.lblStaticGuarantor);
            this.grpWorkersCompensation.Controls.Add(this.lblStaticPolicyNumber);
            this.grpWorkersCompensation.Controls.Add(this.label3);
            this.grpWorkersCompensation.Controls.Add(this.label2);
            this.grpWorkersCompensation.Controls.Add(this.label1);
            this.grpWorkersCompensation.Controls.Add(this.lblStaticInjuryDate);
            this.grpWorkersCompensation.Controls.Add(this.btnEditGuarantor);
            this.grpWorkersCompensation.Controls.Add(this.btnEditPayor);
            this.grpWorkersCompensation.Controls.Add(this.btnEditDiagnosis);
            this.grpWorkersCompensation.Controls.Add(this.btnEditInsurance);
            this.grpWorkersCompensation.Enabled = false;
            this.grpWorkersCompensation.Location = new System.Drawing.Point(27, 280);
            this.grpWorkersCompensation.Name = "grpWorkersCompensation";
            this.grpWorkersCompensation.Size = new System.Drawing.Size(450, 224);
            this.grpWorkersCompensation.TabIndex = 7;
            this.grpWorkersCompensation.TabStop = false;
            this.grpWorkersCompensation.Text = "Worker\'s compensation";
            // 
            // lblGuarantorInfo
            // 
            this.lblGuarantorInfo.Location = new System.Drawing.Point(65, 176);
            this.lblGuarantorInfo.Name = "lblGuarantorInfo";
            this.lblGuarantorInfo.Size = new System.Drawing.Size(174, 41);
            this.lblGuarantorInfo.TabIndex = 0;
            // 
            // lblInsuranceInfo
            // 
            this.lblInsuranceInfo.Location = new System.Drawing.Point(12, 21);
            this.lblInsuranceInfo.Name = "lblInsuranceInfo";
            this.lblInsuranceInfo.Size = new System.Drawing.Size(225, 40);
            this.lblInsuranceInfo.TabIndex = 0;
            // 
            // lblDate
            // 
            this.lblDate.Location = new System.Drawing.Point(111, 84);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(120, 23);
            this.lblDate.TabIndex = 0;
            // 
            // lblPolicyID
            // 
            this.lblPolicyID.Location = new System.Drawing.Point(111, 130);
            this.lblPolicyID.Name = "lblPolicyID";
            this.lblPolicyID.Size = new System.Drawing.Size(120, 23);
            this.lblPolicyID.TabIndex = 0;
            // 
            // lblStaticGuarantor
            // 
            this.lblStaticGuarantor.Location = new System.Drawing.Point(8, 174);
            this.lblStaticGuarantor.Name = "lblStaticGuarantor";
            this.lblStaticGuarantor.Size = new System.Drawing.Size(58, 23);
            this.lblStaticGuarantor.TabIndex = 0;
            this.lblStaticGuarantor.Text = "Guarantor:";
            // 
            // lblStaticPolicyNumber
            // 
            this.lblStaticPolicyNumber.Location = new System.Drawing.Point(8, 130);
            this.lblStaticPolicyNumber.Name = "lblStaticPolicyNumber";
            this.lblStaticPolicyNumber.Size = new System.Drawing.Size(110, 23);
            this.lblStaticPolicyNumber.TabIndex = 0;
            this.lblStaticPolicyNumber.Text = "Policy or ID number:";
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(0, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(448, 3);
            this.label3.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(0, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(448, 3);
            this.label2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(0, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(448, 3);
            this.label1.TabIndex = 0;
            // 
            // lblStaticInjuryDate
            // 
            this.lblStaticInjuryDate.Location = new System.Drawing.Point(8, 84);
            this.lblStaticInjuryDate.Name = "lblStaticInjuryDate";
            this.lblStaticInjuryDate.Size = new System.Drawing.Size(112, 23);
            this.lblStaticInjuryDate.TabIndex = 0;
            this.lblStaticInjuryDate.Text = "Date of illness/injury:";
            // 
            // btnEditGuarantor
            // 
            this.btnEditGuarantor.Location = new System.Drawing.Point(256, 182);
            this.btnEditGuarantor.Name = "btnEditGuarantor";
            this.btnEditGuarantor.Size = new System.Drawing.Size(180, 23);
            this.btnEditGuarantor.TabIndex = 4;
            this.btnEditGuarantor.Text = "Edit &Guarantor && Cancel MSP";
            this.btnEditGuarantor.Click += new System.EventHandler(this.btnEditGuarantor_Click);
            // 
            // btnEditPayor
            // 
            this.btnEditPayor.Location = new System.Drawing.Point(256, 132);
            this.btnEditPayor.Name = "btnEditPayor";
            this.btnEditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnEditPayor.TabIndex = 3;
            this.btnEditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnEditPayor.Click += new System.EventHandler(this.btnEditPayor_Click);
            // 
            // btnEditDiagnosis
            // 
            this.btnEditDiagnosis.Location = new System.Drawing.Point(256, 84);
            this.btnEditDiagnosis.Name = "btnEditDiagnosis";
            this.btnEditDiagnosis.Size = new System.Drawing.Size(180, 23);
            this.btnEditDiagnosis.TabIndex = 2;
            this.btnEditDiagnosis.Text = "Edit Diagn&osis && Cancel MSP";
            this.btnEditDiagnosis.Click += new System.EventHandler(this.btnEditDiagnosis_Click);
            // 
            // btnEditInsurance
            // 
            this.btnEditInsurance.Location = new System.Drawing.Point(256, 21);
            this.btnEditInsurance.Name = "btnEditInsurance";
            this.btnEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnEditInsurance.TabIndex = 1;
            this.btnEditInsurance.Text = "Edit I&nsurance && Cancel MSP";
            this.btnEditInsurance.Click += new System.EventHandler(this.btnEditInsurance_Click);
            // 
            // lblDateBegan
            // 
            this.lblDateBegan.Enabled = false;
            this.lblDateBegan.Location = new System.Drawing.Point(27, 104);
            this.lblDateBegan.Name = "lblDateBegan";
            this.lblDateBegan.Size = new System.Drawing.Size(112, 23);
            this.lblDateBegan.TabIndex = 0;
            this.lblDateBegan.Text = "Date services began:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblQuestion3a);
            this.panel4.Controls.Add(this.rbQuestion4No);
            this.panel4.Controls.Add(this.rbQuestion4Yes);
            this.panel4.Controls.Add(this.lblQuestion4);
            this.panel4.Location = new System.Drawing.Point(16, 206);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(600, 27);
            this.panel4.TabIndex = 5;
            this.panel4.TabStop = true;
            // 
            // lblQuestion3a
            // 
            this.lblQuestion3a.Location = new System.Drawing.Point(8, 14);
            this.lblQuestion3a.Name = "lblQuestion3a";
            this.lblQuestion3a.TabIndex = 0;
            this.lblQuestion3a.Text = " at this facility?";
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
            // lblQuestion4
            // 
            this.lblQuestion4.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion4.Name = "lblQuestion4";
            this.lblQuestion4.Size = new System.Drawing.Size(440, 27);
            this.lblQuestion4.TabIndex = 0;
            this.lblQuestion4.Text = "4. Has the Department of Veteran Affairs (DVA) authorized and agreed to pay for c" +
                "are";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.rbQuestion5No);
            this.panel5.Controls.Add(this.rbQuestion5Yes);
            this.panel5.Controls.Add(this.lblQuestion5);
            this.panel5.Location = new System.Drawing.Point(16, 246);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(600, 24);
            this.panel5.TabIndex = 6;
            this.panel5.TabStop = true;
            // 
            // rbQuestion5No
            // 
            this.rbQuestion5No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion5No.Name = "rbQuestion5No";
            this.rbQuestion5No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5No.TabIndex = 2;
            this.rbQuestion5No.TabStop = true;
            this.rbQuestion5No.Text = "No";
            this.rbQuestion5No.CheckedChanged += new System.EventHandler(this.rbQuestion5No_CheckedChanged);
            // 
            // rbQuestion5Yes
            // 
            this.rbQuestion5Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion5Yes.Name = "rbQuestion5Yes";
            this.rbQuestion5Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion5Yes.TabIndex = 1;
            this.rbQuestion5Yes.TabStop = true;
            this.rbQuestion5Yes.Text = "Yes";
            this.rbQuestion5Yes.CheckedChanged += new System.EventHandler(this.rbQuestion5Yes_CheckedChanged);
            // 
            // lblQuestion5
            // 
            this.lblQuestion5.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion5.Name = "lblQuestion5";
            this.lblQuestion5.Size = new System.Drawing.Size(336, 23);
            this.lblQuestion5.TabIndex = 0;
            this.lblQuestion5.Text = "5. Was the illness/injury due to a work-related accident/condition?";
            // 
            // mskDateBegan
            // 
            this.mskDateBegan.BackColor = System.Drawing.Color.White;
            this.mskDateBegan.Enabled = false;
            this.mskDateBegan.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mskDateBegan.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mskDateBegan.Location = new System.Drawing.Point(137, 104);
            this.mskDateBegan.Mask = "  /  /";
            this.mskDateBegan.MaxLength = 10;
            this.mskDateBegan.Name = "mskDateBegan";
            this.mskDateBegan.Size = new System.Drawing.Size(70, 20);
            this.mskDateBegan.TabIndex = 2;
            this.mskDateBegan.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mskDateBegan.Leave += new System.EventHandler(this.mskDateBegan_Leave);
            this.mskDateBegan.TextChanged += new System.EventHandler(this.mskDateBegan_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbQuestion2No);
            this.panel2.Controls.Add(this.rbQuestion2Yes);
            this.panel2.Controls.Add(this.lblQuestion2);
            this.panel2.Location = new System.Drawing.Point(16, 130);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 24);
            this.panel2.TabIndex = 3;
            this.panel2.TabStop = true;
            // 
            // rbQuestion2No
            // 
            this.rbQuestion2No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion2No.Name = "rbQuestion2No";
            this.rbQuestion2No.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion2No.TabIndex = 2;
            this.rbQuestion2No.TabStop = true;
            this.rbQuestion2No.Text = "No";
            this.rbQuestion2No.CheckedChanged += new System.EventHandler(this.rbQuestion2No_CheckedChanged);
            // 
            // rbQuestion2Yes
            // 
            this.rbQuestion2Yes.Location = new System.Drawing.Point(480, 0);
            this.rbQuestion2Yes.Name = "rbQuestion2Yes";
            this.rbQuestion2Yes.Size = new System.Drawing.Size(50, 24);
            this.rbQuestion2Yes.TabIndex = 1;
            this.rbQuestion2Yes.TabStop = true;
            this.rbQuestion2Yes.Text = "Yes";
            this.rbQuestion2Yes.CheckedChanged += new System.EventHandler(this.rbQuestion2Yes_CheckedChanged);
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(288, 23);
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2. Is the reason for today\'s visit related to Black Lung?";
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(16, 45);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(648, 23);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "If the patient has Black Lung benefits, obtain the date services began even if th" +
                "e reason for today\'s visit is not due to Black Lung.";
            // 
            // SpecialProgramView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.mskDateBegan);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpWorkersCompensation);
            this.Controls.Add(this.lblDateBegan);
            this.Name = "SpecialProgramView";
            this.Size = new System.Drawing.Size(680, 520);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpWorkersCompensation.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public SpecialProgramView( MSPDialog form  )
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            parentForm = form;
            formActivating = true;
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

        private LoggingButton                 btnEditDiagnosis;
        private LoggingButton                 btnEditInsurance;
        private LoggingButton                 btnEditGuarantor;
        private LoggingButton                 btnEditPayor;

        private GroupBox               grpWorkersCompensation;

        private Label                  lblQuestion1;
        private Label                  lblQuestion2;
        private Label                  lblQuestion3;
        private Label                  lblQuestion3a;
        private Label                  lblQuestion4;
        private Label                  lblQuestion5;
        private Label                  lblDateBegan;
        private Label                  label1;
        private Label                  label2;
        private Label                  label3;
        private Label                  lblStaticInjuryDate;
        private Label                  lblDate;
        private Label                  lblStaticPolicyNumber;
        private Label                  lblPolicyID;
        private Label                  lblStaticGuarantor;
        private Label                  lblInsuranceInfo;
        private Label                  lblGuarantorInfo;
        private Label                  lblInstructions;

        private Panel                  panel1;
        private Panel                  panel2;
        private Panel                  panel3;
        private Panel                  panel4;
        private Panel                  panel5;

        private RadioButton            rbQuestion1No;
        private RadioButton            rbQuestion1Yes;
        private RadioButton            rbQuestion2Yes;
        private RadioButton            rbQuestion2No;
        private RadioButton            rbQuestion3No;
        private RadioButton            rbQuestion3Yes;
        private RadioButton            rbQuestion4No;
        private RadioButton            rbQuestion4Yes;
        private RadioButton            rbQuestion5No;
        private RadioButton            rbQuestion5Yes;

        private MaskedEditTextBox              mskDateBegan;
        private NonSelectableTextBox lblTitle;

        private Account                                     i_account;
        private DateTime                                    benefitsDate;
        private MSPDialog                                   parentForm;
        private bool                                        dateFieldError;
        private bool                                        formActivating;
        private static bool                                  formWasChanged;
        private bool                                        question1Response;
        private bool                                        question2Response;
        private bool                                        question3Response;
        private bool                                        question4Response;
        private bool                                        question5Response;
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
