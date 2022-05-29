using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews
{
    /// <summary>
    /// Summary description for LiabilityInsurerView.
    /// </summary>
    public class LiabilityInsurerView : ControlView
    {
        [DllImport("User32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        #region Event Handlers
        private void btnEditDiagnosis_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.DIAGNOSIS );
        }

        private void btnLiabilityEditPayor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.PAYORDETAILS );
        }

        private void btnLiabilityEditInsurance_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURANCE );
        }

        private void btnNoFaultEditInsurance_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.INSURANCE );
        }

        private void btnNoFaultEditPayor_Click(object sender, EventArgs e)
        {
            parentForm.RaiseTabSelectedEvent( (int)AccountView.ScreenIndexes.PAYORDETAILS );
        }

        private void rbQuestion1Yes_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {   // Enable date of accident controls
                btnEditDiagnosis.Enabled = true;
                question1Response = true;
                SetAccidentControlState( true );
                bool state = rbQuestion1Yes.Checked && rbQuestion3Yes.Checked;
                DisplayLiabilityInsurer( state );
                SetQuestion2State( true );
                SetQuestion3State( false );

                Model_MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.SetYes();

                // Redisplay date if RadioButton was toggled
                DateTime date = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentDate;
                if( Model_Account.Diagnosis.Condition.GetType() == typeof( Accident ) )
                {
                    date = ( Model_Account.Diagnosis.Condition as Accident ).OccurredOn;
                }
                Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentDate = date;
                lblAccidentDate.Text  = date.ToString( "d", DateTimeFormatInfo.InvariantInfo );

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
                btnEditDiagnosis.Enabled = false;
                question1Response = true;
                SetAccidentControlState( false );
                SetQuestion2State( false );
                SetQuestion3State( false );
                bool state = rbQuestion1Yes.Checked && rbQuestion3Yes.Checked;
                DisplayLiabilityInsurer( state );

                Model_MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.SetNo();

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Auto_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question2Response = true;
                SetQuestion3State( false );
                DisplayNoFaultInsurer( true );

                Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentType =
                    new TypeOfAccident( TypeOfAccident.AUTO, "Auto" );

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2NonAuto_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question2Response = true;
                SetQuestion3State( false );
                DisplayNoFaultInsurer( true );

                Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentType = 
                    new TypeOfAccident( TypeOfAccident.NON_AUTO, "Non-automobile" );

                FormCanTransition();

                if( (bool) Tag == true )
                {
                    formWasChanged = true;
                    Tag = false;
                    parentForm.ClearLinkLabels();
                }
            }
        }

        private void rbQuestion2Other_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if( rb.Checked )
            {
                question2Response = true;
                SetQuestion3State( true );
                DisplayNoFaultInsurer( false );

                Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentType =
                    new TypeOfAccident( TypeOfAccident.OTHER, "Other" );

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

                Model_MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.SetYes();

                bool state = rbQuestion1Yes.Checked && rbQuestion3Yes.Checked;
                DisplayLiabilityInsurer( state );

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
                bool state = rbQuestion1Yes.Checked && rbQuestion3Yes.Checked;
                DisplayLiabilityInsurer( state );

                Model_MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.SetNo();

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
            }
            else if( formActivating )
            {
                ResetView();

                // Set the UI RadioButtons to reflect the data in the Account
                TypeOfAccident accidentType = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentType;

                if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.Code.Equals( "Y" ) )
                {   // Other buttons are disabled if first one is "No"
                    rbQuestion1Yes.Checked = true;

                    DateTime date = Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AccidentDate;
                    lblAccidentDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", date.Month, date.Day, date.Year );

                    if( accidentType != null )
                    {
                        switch( Convert.ToInt64( accidentType.Oid ) )
                        {
                            case TypeOfAccident.AUTO:
                                rbQuestion2Auto.Checked = true;
                                return; // Question 3 only enabled if accidentType is 'Other'

                            case TypeOfAccident.NON_AUTO:
                                rbQuestion2NonAuto.Checked = true;
                                return;

                            case TypeOfAccident.OTHER:
                                rbQuestion2Other.Checked = true;
                                break;
                        }
                    }

                    if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.Code.Equals( "Y" ) )
                    {
                        rbQuestion3Yes.Checked = true;
                    }
                    else if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.Code.Equals( "N" ) )
                    {
                        rbQuestion3No.Checked = true;
                    }
                }
                else if( Model_Account.MedicareSecondaryPayor.LiabilityInsurer.NonWorkRelated.Code.Equals( "N" ) )
                {
                    rbQuestion1No.Checked = true;
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
        private void DisplayLiabilityInsurer( bool state )
        {
            SetLiabilityGroupBoxState( state );

            if( state )
            {
                Coverage coverage = GetPrimaryCoverage();
                
                if( coverage == null )
                {
                    lblLiabilityInfo.Text = "Not available";
                    lblClaimNumber.Text = "Not available";
                    return;
                }
                if( coverage.BillingInformation.Address != null )
                {
                    string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                    if( mailingLabel != String.Empty )
                    {
                        lblLiabilityInfo.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                            Environment.NewLine, mailingLabel );
                    }
                    else
                    {
                        lblLiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                    }
                }
                else
                {
                    lblLiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                }

                if( coverage.GetType().Equals( typeof( CommercialCoverage ) ) )
                {
                    lblClaimNumber.Text = (coverage as CommercialCoverage).AutoInsuranceClaimNumber;
                }
                else if( coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                {
                    lblClaimNumber.Text = (coverage as OtherCoverage).AutoInsuranceClaimNumber;
                }
                else if( coverage.GetType().Equals( typeof( WorkersCompensationCoverage )  ) )
                {
                    lblClaimNumber.Text = (coverage as WorkersCompensationCoverage).ClaimNumberForIncident;
                }
                else
                {
                    lblClaimNumber.Text = "Not available";
                }
            }
        }

        private void DisplayNoFaultInsurer( bool state )
        {
            SetNoFaultGroupBoxState( state );
            
            if( state )
            {
                Coverage coverage = GetPrimaryCoverage();

                if( coverage == null )
                {
                    lblNoFaultLiabilityInfo.Text = "Not available";
                    lblNoFaultClaimNumber.Text = "Not available";
                    return;
                }
                if( coverage.BillingInformation.Address != null )
                {
                    string mailingLabel = coverage.BillingInformation.Address.AsMailingLabel();
                    if( mailingLabel != String.Empty )
                    {
                        lblNoFaultLiabilityInfo.Text = String.Format( "{0}{1}{2}", coverage.InsurancePlan.PlanName,
                            Environment.NewLine, mailingLabel );
                    }
                    else
                    {
                        lblNoFaultLiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                    }
                }
                else
                {
                    lblNoFaultLiabilityInfo.Text = coverage.InsurancePlan.PlanName;
                }

                if( coverage.GetType().Equals( typeof( CommercialCoverage ) ) ||
                    coverage.GetType().Equals( typeof( OtherCoverage ) ) )
                {
                    string claimNumber = (coverage as CommercialCoverage).AutoInsuranceClaimNumber;
                    if( claimNumber != String.Empty )
                    {
                        lblNoFaultClaimNumber.Text = claimNumber;
                    }
                    else
                    {
                        lblNoFaultClaimNumber.Text = "Claim number missing";
                    }
                }
                else if( coverage.GetType().Equals( typeof( WorkersCompensationCoverage )  ) )
                {
                    string claimNumber = (coverage as WorkersCompensationCoverage).ClaimNumberForIncident;
                    if( claimNumber != String.Empty )
                    {
                        lblNoFaultClaimNumber.Text = claimNumber;
                    }
                    else
                    {
                        lblNoFaultClaimNumber.Text = "Claim number missing";
                    }
                }
                else
                {
                    lblNoFaultClaimNumber.Text = "Not available";
                }
            }
        }

        /// <summary>
        /// Determines if the form responses are complete enough to allow the form to transition.
        /// </summary>
        private void FormCanTransition()
        {
            bool result = false;
            if( question1Response && question2Response && question3Response )
            {
                if( rbQuestion3Yes.Checked )
                {   // Liability insurer is primary
                    response = MSPEventCode.YesStimulus();
                    result = true;
                }
                else
                {   // Medicare is primary
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
            }
            else if( question1Response && question2Response )
            {
                if( rbQuestion2Auto.Checked || rbQuestion2NonAuto.Checked )
                {
                    response = MSPEventCode.YesStimulus();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else if( question1Response )
            {
                if( rbQuestion1No.Checked )
                {
                    response = MSPEventCode.NoStimulus();
                    result = true;
                }
                else
                {
                    result = false;
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

        /// <summary>
        /// Get the primary insurance coverage that is not Medicare
        /// </summary>
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
            rbQuestion1Yes.Checked = false;
            rbQuestion1No.Checked = false;
            rbQuestion2Auto.Checked = false;
            rbQuestion2NonAuto.Checked = false;
            rbQuestion2Other.Checked = false;
            rbQuestion3No.Checked = false;
            rbQuestion3Yes.Checked = false;
            formActivating = false;
            dateFieldError = false;
            question1Response = false;
            question2Response = false;
            question3Response = false;
        }

        private void SetNoFaultGroupBoxState( bool state )
        {
            if( state == false )
            {
                lblNoFaultClaimNumber.Text   = String.Empty;
                lblNoFaultLiabilityInfo.Text = String.Empty;
            }
            grpNoFaultLiabilityInsurer.Enabled = state;
        }

        private void SetLiabilityGroupBoxState( bool state )
        {
            if( state == false )
            {
                lblClaimNumber.Text   = String.Empty;
                lblLiabilityInfo.Text = String.Empty;
            }
            grpLiabilityInsurer.Enabled = state;
        }

        private void SetAccidentControlState( bool state )
        {
            if( state == false )
            {
                lblAccidentDate.Text = String.Empty;
            }
            lblDate.Enabled = state;
            dateFieldError  = false;
        }

        /// <summary>
        /// Enable or disable the controls for question 2
        /// </summary>
        private void SetQuestion2State( bool state )
        {
            if( state == false )
            {
                rbQuestion2Auto.Checked    = state;
                rbQuestion2NonAuto.Checked = state;
                rbQuestion2Other.Checked   = state;
                SetNoFaultGroupBoxState( state );
                Model_MedicareSecondaryPayor.LiabilityInsurer.AccidentType = new TypeOfAccident();
            }

            panel2.Enabled             = state;
            rbQuestion2Auto.Checked    = false;
            rbQuestion2NonAuto.Checked = false;
            rbQuestion2Other.Checked   = false;
            question2Response          = false;
        }

        /// <summary>
        /// Enable or disable the controls for question 3
        /// </summary>
        private void SetQuestion3State( bool state )
        {
            if( state == false )
            {
                rbQuestion3Yes.Checked = state;
                rbQuestion3No.Checked  = state;
                SetLiabilityGroupBoxState( state );
                Model_MedicareSecondaryPayor.LiabilityInsurer.AnotherPartyResponsibility.SetBlank();
            }

            panel3.Enabled         = state;
            rbQuestion3Yes.Checked = false;
            rbQuestion3No.Checked  = false;
            question3Response      = false;
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDate = new System.Windows.Forms.Label();
            this.lblTitle = new PatientAccess.UI.CommonControls.NonSelectableTextBox();
            this.grpLiabilityInsurer = new System.Windows.Forms.GroupBox();
            this.lblLiabilityInfo = new System.Windows.Forms.Label();
            this.lblClaimNumber = new System.Windows.Forms.Label();
            this.btnLiabilityEditPayor = new LoggingButton();
            this.lblStaticClaimNumber = new System.Windows.Forms.Label();
            this.lblLine2 = new System.Windows.Forms.Label();
            this.btnLiabilityEditInsurance = new LoggingButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblQuestion3 = new System.Windows.Forms.Label();
            this.rbQuestion3No = new System.Windows.Forms.RadioButton();
            this.rbQuestion3Yes = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbQuestion2Auto = new System.Windows.Forms.RadioButton();
            this.rbQuestion2NonAuto = new System.Windows.Forms.RadioButton();
            this.rbQuestion2Other = new System.Windows.Forms.RadioButton();
            this.lblQuestion2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbQuestion1Yes = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rbQuestion1No = new System.Windows.Forms.RadioButton();
            this.grpNoFaultLiabilityInsurer = new System.Windows.Forms.GroupBox();
            this.lblNoFaultLiabilityInfo = new System.Windows.Forms.Label();
            this.lblNoFaultClaimNumber = new System.Windows.Forms.Label();
            this.btnNoFaultEditPayor = new LoggingButton();
            this.lblStaticNoFaultClaimNumber = new System.Windows.Forms.Label();
            this.lblLine1 = new System.Windows.Forms.Label();
            this.btnNoFaultEditInsurance = new LoggingButton();
            this.btnEditDiagnosis = new LoggingButton();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.lblAccidentDate = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.grpLiabilityInsurer.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grpNoFaultLiabilityInsurer.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDate
            // 
            this.lblDate.Enabled = false;
            this.lblDate.Location = new System.Drawing.Point(0, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(89, 23);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Date of accident:";
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
            this.lblTitle.Size = new System.Drawing.Size(120, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Liability Insurer";
            // 
            // grpLiabilityInsurer
            // 
            this.grpLiabilityInsurer.Controls.Add(this.lblLiabilityInfo);
            this.grpLiabilityInsurer.Controls.Add(this.lblClaimNumber);
            this.grpLiabilityInsurer.Controls.Add(this.btnLiabilityEditPayor);
            this.grpLiabilityInsurer.Controls.Add(this.lblStaticClaimNumber);
            this.grpLiabilityInsurer.Controls.Add(this.lblLine2);
            this.grpLiabilityInsurer.Controls.Add(this.btnLiabilityEditInsurance);
            this.grpLiabilityInsurer.Enabled = false;
            this.grpLiabilityInsurer.Location = new System.Drawing.Point(27, 338);
            this.grpLiabilityInsurer.Name = "grpLiabilityInsurer";
            this.grpLiabilityInsurer.Size = new System.Drawing.Size(469, 120);
            this.grpLiabilityInsurer.TabIndex = 6;
            this.grpLiabilityInsurer.TabStop = false;
            this.grpLiabilityInsurer.Text = "Liability Insurer";
            // 
            // lblLiabilityInfo
            // 
            this.lblLiabilityInfo.Location = new System.Drawing.Point(12, 21);
            this.lblLiabilityInfo.Name = "lblLiabilityInfo";
            this.lblLiabilityInfo.Size = new System.Drawing.Size(250, 40);
            this.lblLiabilityInfo.TabIndex = 0;
            // 
            // lblClaimNumber
            // 
            this.lblClaimNumber.Location = new System.Drawing.Point(131, 84);
            this.lblClaimNumber.Name = "lblClaimNumber";
            this.lblClaimNumber.Size = new System.Drawing.Size(135, 23);
            this.lblClaimNumber.TabIndex = 0;
            // 
            // btnLiabilityEditPayor
            // 
            this.btnLiabilityEditPayor.Location = new System.Drawing.Point(273, 84);
            this.btnLiabilityEditPayor.Name = "btnLiabilityEditPayor";
            this.btnLiabilityEditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnLiabilityEditPayor.TabIndex = 2;
            this.btnLiabilityEditPayor.Text = "Edit Pa&yor Details && Cancel MSP";
            this.btnLiabilityEditPayor.Click += new System.EventHandler(this.btnLiabilityEditPayor_Click);
            // 
            // lblStaticClaimNumber
            // 
            this.lblStaticClaimNumber.Location = new System.Drawing.Point(8, 84);
            this.lblStaticClaimNumber.Name = "lblStaticClaimNumber";
            this.lblStaticClaimNumber.Size = new System.Drawing.Size(128, 23);
            this.lblStaticClaimNumber.TabIndex = 0;
            this.lblStaticClaimNumber.Text = "Insurance claim number:";
            // 
            // lblLine2
            // 
            this.lblLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine2.Location = new System.Drawing.Point(0, 72);
            this.lblLine2.Name = "lblLine2";
            this.lblLine2.Size = new System.Drawing.Size(467, 3);
            this.lblLine2.TabIndex = 0;
            // 
            // btnLiabilityEditInsurance
            // 
            this.btnLiabilityEditInsurance.Location = new System.Drawing.Point(273, 21);
            this.btnLiabilityEditInsurance.Name = "btnLiabilityEditInsurance";
            this.btnLiabilityEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnLiabilityEditInsurance.TabIndex = 1;
            this.btnLiabilityEditInsurance.Text = "&Edit Insurance && Cancel MSP";
            this.btnLiabilityEditInsurance.Click += new System.EventHandler(this.btnLiabilityEditInsurance_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblQuestion3);
            this.panel3.Controls.Add(this.rbQuestion3No);
            this.panel3.Controls.Add(this.rbQuestion3Yes);
            this.panel3.Location = new System.Drawing.Point(16, 308);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(600, 24);
            this.panel3.TabIndex = 5;
            this.panel3.TabStop = true;
            // 
            // lblQuestion3
            // 
            this.lblQuestion3.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion3.Name = "lblQuestion3";
            this.lblQuestion3.Size = new System.Drawing.Size(264, 23);
            this.lblQuestion3.TabIndex = 0;
            this.lblQuestion3.Text = "3. Was another party responsible for this accident?";
            // 
            // rbQuestion3No
            // 
            this.rbQuestion3No.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion3No.Name = "rbQuestion3No";
            this.rbQuestion3No.Size = new System.Drawing.Size(48, 24);
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
            this.panel2.Controls.Add(this.rbQuestion2Auto);
            this.panel2.Controls.Add(this.rbQuestion2NonAuto);
            this.panel2.Controls.Add(this.rbQuestion2Other);
            this.panel2.Controls.Add(this.lblQuestion2);
            this.panel2.Location = new System.Drawing.Point(16, 138);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 24);
            this.panel2.TabIndex = 3;
            this.panel2.TabStop = true;
            // 
            // rbQuestion2Auto
            // 
            this.rbQuestion2Auto.Location = new System.Drawing.Point(343, 0);
            this.rbQuestion2Auto.Name = "rbQuestion2Auto";
            this.rbQuestion2Auto.Size = new System.Drawing.Size(80, 24);
            this.rbQuestion2Auto.TabIndex = 1;
            this.rbQuestion2Auto.TabStop = true;
            this.rbQuestion2Auto.Text = "Automobile";
            this.rbQuestion2Auto.CheckedChanged += new System.EventHandler(this.rbQuestion2Auto_CheckedChanged);
            // 
            // rbQuestion2NonAuto
            // 
            this.rbQuestion2NonAuto.Location = new System.Drawing.Point(430, 0);
            this.rbQuestion2NonAuto.Name = "rbQuestion2NonAuto";
            this.rbQuestion2NonAuto.Size = new System.Drawing.Size(103, 24);
            this.rbQuestion2NonAuto.TabIndex = 2;
            this.rbQuestion2NonAuto.TabStop = true;
            this.rbQuestion2NonAuto.Text = "Non-automobile";
            this.rbQuestion2NonAuto.CheckedChanged += new System.EventHandler(this.rbQuestion2NonAuto_CheckedChanged);
            // 
            // rbQuestion2Other
            // 
            this.rbQuestion2Other.Location = new System.Drawing.Point(540, 0);
            this.rbQuestion2Other.Name = "rbQuestion2Other";
            this.rbQuestion2Other.Size = new System.Drawing.Size(55, 24);
            this.rbQuestion2Other.TabIndex = 3;
            this.rbQuestion2Other.TabStop = true;
            this.rbQuestion2Other.Text = "Other";
            this.rbQuestion2Other.CheckedChanged += new System.EventHandler(this.rbQuestion2Other_CheckedChanged);
            // 
            // lblQuestion2
            // 
            this.lblQuestion2.Location = new System.Drawing.Point(0, 0);
            this.lblQuestion2.Name = "lblQuestion2";
            this.lblQuestion2.Size = new System.Drawing.Size(258, 23);
            this.lblQuestion2.TabIndex = 0;
            this.lblQuestion2.Text = "2. What type of accident caused the illness/injury?";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbQuestion1Yes);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.rbQuestion1No);
            this.panel1.Location = new System.Drawing.Point(16, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 24);
            this.panel1.TabIndex = 1;
            this.panel1.TabStop = true;
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
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "1. Was illness/injury due to a non-work related accident?";
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
            // grpNoFaultLiabilityInsurer
            // 
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.lblNoFaultLiabilityInfo);
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.lblNoFaultClaimNumber);
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.btnNoFaultEditPayor);
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.lblStaticNoFaultClaimNumber);
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.lblLine1);
            this.grpNoFaultLiabilityInsurer.Controls.Add(this.btnNoFaultEditInsurance);
            this.grpNoFaultLiabilityInsurer.Enabled = false;
            this.grpNoFaultLiabilityInsurer.Location = new System.Drawing.Point(27, 168);
            this.grpNoFaultLiabilityInsurer.Name = "grpNoFaultLiabilityInsurer";
            this.grpNoFaultLiabilityInsurer.Size = new System.Drawing.Size(469, 120);
            this.grpNoFaultLiabilityInsurer.TabIndex = 4;
            this.grpNoFaultLiabilityInsurer.TabStop = false;
            this.grpNoFaultLiabilityInsurer.Text = "No-fault or liability insurer";
            // 
            // lblNoFaultLiabilityInfo
            // 
            this.lblNoFaultLiabilityInfo.Location = new System.Drawing.Point(12, 21);
            this.lblNoFaultLiabilityInfo.Name = "lblNoFaultLiabilityInfo";
            this.lblNoFaultLiabilityInfo.Size = new System.Drawing.Size(250, 40);
            this.lblNoFaultLiabilityInfo.TabIndex = 0;
            // 
            // lblNoFaultClaimNumber
            // 
            this.lblNoFaultClaimNumber.Location = new System.Drawing.Point(128, 84);
            this.lblNoFaultClaimNumber.Name = "lblNoFaultClaimNumber";
            this.lblNoFaultClaimNumber.Size = new System.Drawing.Size(135, 23);
            this.lblNoFaultClaimNumber.TabIndex = 0;
            // 
            // btnNoFaultEditPayor
            // 
            this.btnNoFaultEditPayor.Location = new System.Drawing.Point(273, 84);
            this.btnNoFaultEditPayor.Name = "btnNoFaultEditPayor";
            this.btnNoFaultEditPayor.Size = new System.Drawing.Size(180, 23);
            this.btnNoFaultEditPayor.TabIndex = 2;
            this.btnNoFaultEditPayor.Text = "Edit &Payor Details && Cancel MSP";
            this.btnNoFaultEditPayor.Click += new System.EventHandler(this.btnNoFaultEditPayor_Click);
            // 
            // lblStaticNoFaultClaimNumber
            // 
            this.lblStaticNoFaultClaimNumber.Location = new System.Drawing.Point(8, 84);
            this.lblStaticNoFaultClaimNumber.Name = "lblStaticNoFaultClaimNumber";
            this.lblStaticNoFaultClaimNumber.Size = new System.Drawing.Size(128, 23);
            this.lblStaticNoFaultClaimNumber.TabIndex = 0;
            this.lblStaticNoFaultClaimNumber.Text = "Insurance claim number:";
            // 
            // lblLine1
            // 
            this.lblLine1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine1.Location = new System.Drawing.Point(0, 72);
            this.lblLine1.Name = "lblLine1";
            this.lblLine1.Size = new System.Drawing.Size(467, 3);
            this.lblLine1.TabIndex = 0;
            // 
            // btnNoFaultEditInsurance
            // 
            this.btnNoFaultEditInsurance.Location = new System.Drawing.Point(273, 21);
            this.btnNoFaultEditInsurance.Name = "btnNoFaultEditInsurance";
            this.btnNoFaultEditInsurance.Size = new System.Drawing.Size(180, 23);
            this.btnNoFaultEditInsurance.TabIndex = 1;
            this.btnNoFaultEditInsurance.Text = "Edit I&nsurance && Cancel MSP";
            this.btnNoFaultEditInsurance.Click += new System.EventHandler(this.btnNoFaultEditInsurance_Click);
            // 
            // btnEditDiagnosis
            // 
            this.btnEditDiagnosis.Enabled = false;
            this.btnEditDiagnosis.Location = new System.Drawing.Point(260, 0);
            this.btnEditDiagnosis.Name = "btnEditDiagnosis";
            this.btnEditDiagnosis.Size = new System.Drawing.Size(180, 23);
            this.btnEditDiagnosis.TabIndex = 1;
            this.btnEditDiagnosis.Text = "Edit Diagn&osis && Cancel MSP";
            this.btnEditDiagnosis.Click += new System.EventHandler(this.btnEditDiagnosis_Click);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(16, 45);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(472, 23);
            this.lblInstructions.TabIndex = 0;
            this.lblInstructions.Text = "Complete each question in this section as it pertains to the patient\'s reason for" +
                " today\'s visit.";
            // 
            // lblAccidentDate
            // 
            this.lblAccidentDate.Location = new System.Drawing.Point(92, 0);
            this.lblAccidentDate.Name = "lblAccidentDate";
            this.lblAccidentDate.Size = new System.Drawing.Size(96, 23);
            this.lblAccidentDate.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblAccidentDate);
            this.panel4.Controls.Add(this.lblDate);
            this.panel4.Controls.Add(this.btnEditDiagnosis);
            this.panel4.Location = new System.Drawing.Point(27, 102);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(445, 24);
            this.panel4.TabIndex = 2;
            this.panel4.TabStop = true;
            // 
            // LiabilityInsurerView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.grpNoFaultLiabilityInsurer);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grpLiabilityInsurer);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "LiabilityInsurerView";
            this.Size = new System.Drawing.Size(680, 520);
            this.grpLiabilityInsurer.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.grpNoFaultLiabilityInsurer.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Construction
        public LiabilityInsurerView( MSPDialog form )
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
        private LoggingButton                 btnLiabilityEditPayor;
        private LoggingButton                 btnLiabilityEditInsurance;
        private LoggingButton                 btnNoFaultEditInsurance;
        private LoggingButton                 btnNoFaultEditPayor;
        private LoggingButton                 btnEditDiagnosis;

        private GroupBox               grpLiabilityInsurer;
        private GroupBox               grpNoFaultLiabilityInsurer;

        private Label                  lblDate;
        private Label                  lblQuestion3;
        private Label                  lblQuestion2;
        private Label                  label1;
        private Label                  lblLine2;
        private Label                  lblStaticClaimNumber;
        private Label                  lblLine1;
        private Label                  lblStaticNoFaultClaimNumber;
        private Label                  lblNoFaultClaimNumber;
        private Label                  lblClaimNumber;
        private Label                  lblNoFaultLiabilityInfo;
        private Label                  lblLiabilityInfo;
        private Label                  lblAccidentDate;
        private Label                  lblInstructions;

        private Panel                  panel1;
        private Panel                  panel2;
        private Panel                  panel3;
        private Panel                  panel4;

        private RadioButton            rbQuestion1No;
        private RadioButton            rbQuestion1Yes;
        private RadioButton            rbQuestion2Auto;
        private RadioButton            rbQuestion2NonAuto;
        private RadioButton            rbQuestion2Other;
        private RadioButton            rbQuestion3No;
        private RadioButton            rbQuestion3Yes;

        private NonSelectableTextBox lblTitle;

        private Account                                     i_account;
        private MSPDialog                                   parentForm;
        public static bool                                  formActivating;
        private static bool                                  formWasChanged;
        private bool                                        dateFieldError;
        private bool                                        question1Response;
        private bool                                        question2Response;
        private bool                                        question3Response;
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
