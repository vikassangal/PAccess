using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for MedicaidVerifyView.
    /// </summary>
    public class MedicaidVerifyView : ControlView
    {
        #region Event Handlers

        private void MedicaidVerifyView_Disposed(object sender, EventArgs e)
        {
            UnRegisterEvents();
        }

        private void InsuranceMedicaidHasOtherCoveragePreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( cmbOtherCoverage );
        }

        private void MedicaidEligibilityDatePreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( mtbEligibilityDate );
        }

        private void MedicaidPatientHasMedicarePreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( cmbPatientHasMedicare );
        }

        private void InsuranceInformationRecvFromPreferredEventHandler(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( this.cmbInfoRecvFrom );
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        private void dtpEligibility_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbEligibilityDate );
            DateTime dt = dtp.Value;
            mtbEligibilityDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbEligibilityDate.Focus();
        }

        private void mtbEligibilityDate_Enter(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            mtb.Refresh();
        }

		private void mtbEligibilityDate_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
			if( dtpEligibility.Focused )
			{
				return;
			}
			if( mtb.UnMaskedText != String.Empty )
			{
				if( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
				{
					eligibilityDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
				}
			}
			else
			{
				eligibilityDate = DateTime.MinValue;
			}

            Model_Coverage.EligibilityDate = eligibilityDate;

			if( !dtpEligibility.Focused )
			{   // Run the rule only focus is not on DateTimePicker
				CheckForRequiredFields();
			}
		}

		private void mtbMedicaidCoPay_Validating(object sender, CancelEventArgs e)
		{
            if( this.mtbMedicaidCoPay.UnMaskedText != string.Empty )
            {
                CommonFormatting.FormatTextBoxCurrency( mtbMedicaidCoPay, "###,###,##0.00" );    
            }			
		}

        private void cmbPatientHasMedicare_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbPatientHasMedicare_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cmbPatientHasMedicare );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                if( (cb.SelectedItem as YesNoFlag).Code.Equals( "Y" ) && medicaidIsPrimary )
                {
                    MessageBox.Show( UIErrorMessages.MEDICARE_WITH_MEDICAID_YES, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
                patientHasMedicare = cb.SelectedItem as YesNoFlag;
            }
            else
            {                
                patientHasMedicare = null;
            }
            Model_Coverage.PatienthasMedicare = patientHasMedicare;

            CheckForRequiredFields();
        }

        private void cmbOtherCoverage_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbOtherCoverage_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cmbOtherCoverage );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                if( (cb.SelectedItem as YesNoFlag).Code.Equals( "Y" ) && medicaidIsPrimary )
                {
                    MessageBox.Show( UIErrorMessages.MEDICAID_WITH_TPL, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                }
                patienthasOtherInsuranceCoverage = cb.SelectedItem as YesNoFlag;
            }
            else
            {
                patienthasOtherInsuranceCoverage = null;
            }
            
            Model_Coverage.PatienthasOtherInsuranceCoverage = patienthasOtherInsuranceCoverage;

            CheckForRequiredFields();
        }

        private void mtbMedicaidCoPay_Leave(object sender, EventArgs e)
        {
            this.mtbMedicaidCoPay_Validating(sender, null);
        }

        private void mtbMedicaidCoPay_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != ".")
            {
                medicaidCopay = Convert.ToDouble( mtb.UnMaskedText );
            }
            else
            {
                medicaidCopay = -1;
            }
        }

        private void mtbEvcNumber_TextChanged(object sender, EventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.Text != String.Empty )
            {
                EVCNumber = mtb.Text.Trim();
            }
            else
            {
                EVCNumber = String.Empty;
            }
        }

        private void cmbInfoRecvFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIColors.SetNormalBgColor( this.cmbInfoRecvFrom );

            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 && !string.IsNullOrEmpty(cb.Text) )
            {
                Model_Coverage.InformationReceivedSource = cb.SelectedItem as InformationReceivedSource;
            }
            RuleEngine.EvaluateRule( typeof(InsuranceInformationRecvFromPreferred), this.Model_Coverage );
        }

		private void mtbRemarks_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;
			if( mtb.UnMaskedText != String.Empty )
			{
				remarks = mtb.Text.TrimEnd();
			}
			else
			{
				remarks = String.Empty;
			}
		}
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;
                PopulateInfoRecvComboBox();
                PopulatePatientHasMedicareComboBox();
                PopulatePatientOtherCoverageComboBox();
            }

            this.cmbPatientHasMedicare.SelectedItem = this.Model_Coverage.PatienthasMedicare;
            this.patientHasMedicare = this.Model_Coverage.PatienthasMedicare;

            this.cmbOtherCoverage.SelectedItem = this.Model_Coverage.PatienthasOtherInsuranceCoverage;
            this.patienthasOtherInsuranceCoverage = this.Model_Coverage.PatienthasOtherInsuranceCoverage;

            this.cmbInfoRecvFrom.SelectedItem = this.Model_Coverage.InformationReceivedSource;
            this.informationReceivedSource = this.Model_Coverage.InformationReceivedSource;

            this.mtbEvcNumber.UnMaskedText = this.Model_Coverage.EVCNumber;
            this.EVCNumber = this.Model_Coverage.EVCNumber;

            this.mtbRemarks.UnMaskedText = this.Model_Coverage.Remarks;
            this.remarks = this.Model_Coverage.Remarks;

            if( Model_Coverage.EligibilityDate == DateTime.MinValue )
            {
                this.mtbEligibilityDate.UnMaskedText = string.Empty;
            }
            else
            {
                this.mtbEligibilityDate.UnMaskedText = this.Model_Coverage.EligibilityDate.ToString( "MMddyyyy" );
            }
            this.eligibilityDate = this.Model_Coverage.EligibilityDate;

            if( this.Model_Coverage.MedicaidCopay != -1 )
            {
                this.mtbMedicaidCoPay.UnMaskedText = this.Model_Coverage.MedicaidCopay.ToString();
            }
            else
            {
                this.mtbMedicaidCoPay.UnMaskedText = string.Empty;
            }
            this.mtbMedicaidCoPay_Validating( this, null );
            this.medicaidCopay = this.Model_Coverage.MedicaidCopay;

            //RuleEngine.LoadRules( Account );

            RuleEngine.RegisterEvent( typeof( InsuranceMedicaidHasOtherCoveragePreferred ), Model_Coverage, new EventHandler( InsuranceMedicaidHasOtherCoveragePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( MedicaidEligibilityDatePreferred ), Model_Coverage, new EventHandler( MedicaidEligibilityDatePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( MedicaidPatientHasMedicarePreferred ), Model_Coverage, new EventHandler( MedicaidPatientHasMedicarePreferredEventHandler ) );
            RuleEngine.RegisterEvent( typeof( InsuranceInformationRecvFromPreferred ), this.Model_Coverage, new EventHandler( InsuranceInformationRecvFromPreferredEventHandler ) );

            CheckForRequiredFields();


            bool result = true;
            int year = 0;
            int month = 0;
            int day = 0;

            if( mtbEligibilityDate.UnMaskedText != String.Empty )
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbEligibilityDate, ref year, ref month, ref day );
            }

            medicaidIsPrimary = Model_Coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID );
        }

        public override void UpdateModel()
        {
            // force validating events to ensure that the working variables are updated.
            // Validating does NOT fire unless you tab out of the field; so, the last
            // mtb changed before clicking OK will not get updated values

            //this.mtbEligibilityDate_Validating(this.mtbEligibilityDate, null);
            Model_Coverage.EligibilityDate = eligibilityDate;
 
            if( this.cmbPatientHasMedicare.SelectedItem != null )
            {
                Model_Coverage.PatienthasMedicare = (YesNoFlag)this.cmbPatientHasMedicare.SelectedItem;
            }

            if( patienthasOtherInsuranceCoverage != null )
            {
                Model_Coverage.PatienthasOtherInsuranceCoverage = (YesNoFlag) patienthasOtherInsuranceCoverage.Clone();
            }

            this.mtbMedicaidCoPay_Validating(this.mtbMedicaidCoPay, null);

            if( this.mtbMedicaidCoPay.UnMaskedText != string.Empty)
            {
                Model_Coverage.MedicaidCopay = (float) medicaidCopay;
            }
            else
            {
                this.Model_Coverage.MedicaidCopay = -1;
            }

            if( !string.IsNullOrEmpty( this.mtbEvcNumber.Text) )
            {
                Model_Coverage.EVCNumber = this.mtbEvcNumber.Text;
            }

            if( this.cmbInfoRecvFrom.SelectedItem != null )
            {
                Model_Coverage.InformationReceivedSource = (InformationReceivedSource) this.cmbInfoRecvFrom.SelectedItem;
            }

            if( !string.IsNullOrEmpty( this.mtbRemarks.Text ) )
            {
                this.mtbRemarks_Validating( this.mtbRemarks, null );
                Model_Coverage.Remarks = (String)remarks.Clone();
            }

        }
        #endregion

        #region Properties
        [Browsable(false)]
        public GovernmentMedicaidCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentMedicaidCoverage)this.Model;
            }
        }

        [Browsable(false)]
        public Account Account
        {
            get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        [Browsable(false)]
        private RuleEngine RuleEngine
        {
            get
            {
                if( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        private void CheckForRequiredFields()
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceMedicaidHasOtherCoveragePreferred ), Model_Coverage  );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidEligibilityDatePreferred ), Model_Coverage  );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicaidPatientHasMedicarePreferred ), Model_Coverage  );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceInformationRecvFromPreferred ), Model_Coverage  );
        }

        private void PopulatePatientHasMedicareComboBox()
        {
            cmbPatientHasMedicare.Items.Add( blankYesNoFlag );
            cmbPatientHasMedicare.Items.Add( yesYesNoFlag );
            cmbPatientHasMedicare.Items.Add( noYesNoFlag );
        }

        private void PopulatePatientOtherCoverageComboBox()
        {
            cmbOtherCoverage.Items.Add( blankYesNoFlag );
            cmbOtherCoverage.Items.Add( yesYesNoFlag );
            cmbOtherCoverage.Items.Add( noYesNoFlag );
        }

        private void PopulateInfoRecvComboBox()
        {
            IInfoReceivedSourceBroker broker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();
            ICollection alist = broker.AllInfoReceivedSources();

            cmbInfoRecvFrom.Items.Clear();

            foreach( InformationReceivedSource o in alist )
            {
                cmbInfoRecvFrom.Items.Add( o );
            }
        }

        private void ResetView()
        {
            this.Model_Coverage.RemoveCoverageVerificationData();
            this.UpdateView();
        }

        private void UnRegisterEvents()
        {
            RuleEngine.UnregisterEvent( typeof( InsuranceMedicaidHasOtherCoveragePreferred ), Model_Coverage, InsuranceMedicaidHasOtherCoveragePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MedicaidEligibilityDatePreferred ), Model_Coverage, MedicaidEligibilityDatePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( MedicaidPatientHasMedicarePreferred ), Model_Coverage, MedicaidPatientHasMedicarePreferredEventHandler );
            RuleEngine.UnregisterEvent( typeof( InsuranceInformationRecvFromPreferred ), this.Model_Coverage, InsuranceInformationRecvFromPreferredEventHandler );
        }

        private void ConfigureControls()
        {
            MaskedEditTextBoxBuilder.ConfigureInsuranceVerificationRemarks( mtbRemarks );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.mtbEvcNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblEvcNumber = new System.Windows.Forms.Label();
            this.mtbMedicaidCoPay = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.btnClearAll = new LoggingButton();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
            this.lblInfoRecvFrom = new System.Windows.Forms.Label();
            this.lblMedicaidCoPay = new System.Windows.Forms.Label();
            this.cmbOtherCoverage = new System.Windows.Forms.ComboBox();
            this.lblOtherCoverage = new System.Windows.Forms.Label();
            this.cmbPatientHasMedicare = new System.Windows.Forms.ComboBox();
            this.dtpEligibility = new System.Windows.Forms.DateTimePicker();
            this.mtbEligibilityDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblPatientHasMedicare = new System.Windows.Forms.Label();
            this.lblEligibilityDate = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.Controls.Add(this.mtbEvcNumber);
            this.panel.Controls.Add(this.lblEvcNumber);
            this.panel.Controls.Add(this.mtbMedicaidCoPay);
            this.panel.Controls.Add(this.mtbRemarks);
            this.panel.Controls.Add(this.btnClearAll);
            this.panel.Controls.Add(this.lblRemarks);
            this.panel.Controls.Add(this.cmbInfoRecvFrom);
            this.panel.Controls.Add(this.lblInfoRecvFrom);
            this.panel.Controls.Add(this.lblMedicaidCoPay);
            this.panel.Controls.Add(this.cmbOtherCoverage);
            this.panel.Controls.Add(this.lblOtherCoverage);
            this.panel.Controls.Add(this.cmbPatientHasMedicare);
            this.panel.Controls.Add(this.dtpEligibility);
            this.panel.Controls.Add(this.mtbEligibilityDate);
            this.panel.Controls.Add(this.lblPatientHasMedicare);
            this.panel.Controls.Add(this.lblEligibilityDate);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(847, 187);
            this.panel.TabIndex = 1;
            // 
            // mtbEvcNumber
            // 
            this.mtbEvcNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEvcNumber.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEvcNumber.Location = new System.Drawing.Point(120, 151);
            this.mtbEvcNumber.Mask = "";
            this.mtbEvcNumber.MaxLength = 12;
            this.mtbEvcNumber.Name = "mtbEvcNumber";
            this.mtbEvcNumber.TabIndex = 5;
            this.mtbEvcNumber.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEvcNumber.TextChanged += new System.EventHandler(this.mtbEvcNumber_TextChanged);
            // 
            // lblEvcNumber
            // 
            this.lblEvcNumber.Location = new System.Drawing.Point(8, 153);
            this.lblEvcNumber.Name = "lblEvcNumber";
            this.lblEvcNumber.Size = new System.Drawing.Size(88, 23);
            this.lblEvcNumber.TabIndex = 0;
            this.lblEvcNumber.Text = " EVC number:";
            // 
            // mtbMedicaidCoPay
            // 
            this.mtbMedicaidCoPay.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbMedicaidCoPay.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbMedicaidCoPay.Location = new System.Drawing.Point(120, 116);
            this.mtbMedicaidCoPay.Mask = "";
            this.mtbMedicaidCoPay.MaxLength = 10;
            this.mtbMedicaidCoPay.Name = "mtbMedicaidCoPay";
            this.mtbMedicaidCoPay.Size = new System.Drawing.Size(73, 20);
            this.mtbMedicaidCoPay.TabIndex = 4;
            this.mtbMedicaidCoPay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMedicaidCoPay.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbMedicaidCoPay.Validating += new System.ComponentModel.CancelEventHandler(this.mtbMedicaidCoPay_Validating);
            this.mtbMedicaidCoPay.TextChanged += new System.EventHandler(this.mtbMedicaidCoPay_TextChanged);
            //this.mtbMedicaidCoPay.Enter += new System.EventHandler(this.mtbMedicaidCoPay_Enter);
            this.mtbMedicaidCoPay.Leave +=new EventHandler(mtbMedicaidCoPay_Leave);
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point(376, 48);
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size(230, 48);
            this.mtbRemarks.TabIndex = 7;
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler(this.mtbRemarks_Validating);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(740, 146);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.TabIndex = 8;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // lblRemarks
            // 
            this.lblRemarks.Location = new System.Drawing.Point(320, 51);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(56, 23);
            this.lblRemarks.TabIndex = 0;
            this.lblRemarks.Text = "Remarks:";
            // 
            // cmbInfoRecvFrom
            // 
            this.cmbInfoRecvFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point(448, 14);
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size(185, 21);
            this.cmbInfoRecvFrom.TabIndex = 6;
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler(this.cmbInfoRecvFrom_SelectedIndexChanged);
            // 
            // lblInfoRecvFrom
            // 
            this.lblInfoRecvFrom.Location = new System.Drawing.Point(320, 17);
            this.lblInfoRecvFrom.Name = "lblInfoRecvFrom";
            this.lblInfoRecvFrom.Size = new System.Drawing.Size(136, 23);
            this.lblInfoRecvFrom.TabIndex = 0;
            this.lblInfoRecvFrom.Text = "Information received from:";
            // 
            // lblMedicaidCoPay
            // 
            this.lblMedicaidCoPay.Location = new System.Drawing.Point(8, 119);
            this.lblMedicaidCoPay.Name = "lblMedicaidCoPay";
            this.lblMedicaidCoPay.Size = new System.Drawing.Size(117, 23);
            this.lblMedicaidCoPay.TabIndex = 0;
            this.lblMedicaidCoPay.Text = "Medicaid co-pay:      $";
            // 
            // cmbOtherCoverage
            // 
            this.cmbOtherCoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOtherCoverage.Location = new System.Drawing.Point(229, 82);
            this.cmbOtherCoverage.Name = "cmbOtherCoverage";
            this.cmbOtherCoverage.Size = new System.Drawing.Size(45, 21);
            this.cmbOtherCoverage.TabIndex = 3;
            this.cmbOtherCoverage.DropDown += new System.EventHandler(this.cmbOtherCoverage_DropDown);
            this.cmbOtherCoverage.SelectedIndexChanged += new System.EventHandler(this.cmbOtherCoverage_SelectedIndexChanged);
            // 
            // lblOtherCoverage
            // 
            this.lblOtherCoverage.Location = new System.Drawing.Point(8, 85);
            this.lblOtherCoverage.Name = "lblOtherCoverage";
            this.lblOtherCoverage.Size = new System.Drawing.Size(232, 23);
            this.lblOtherCoverage.TabIndex = 0;
            this.lblOtherCoverage.Text = "Patient has other insurance coverage (TPL):";
            // 
            // cmbPatientHasMedicare
            // 
            this.cmbPatientHasMedicare.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientHasMedicare.Location = new System.Drawing.Point(120, 48);
            this.cmbPatientHasMedicare.Name = "cmbPatientHasMedicare";
            this.cmbPatientHasMedicare.Size = new System.Drawing.Size(45, 21);
            this.cmbPatientHasMedicare.TabIndex = 2;
            this.cmbPatientHasMedicare.DropDown += new System.EventHandler(this.cmbPatientHasMedicare_DropDown);
            this.cmbPatientHasMedicare.SelectedIndexChanged += new System.EventHandler(this.cmbPatientHasMedicare_SelectedIndexChanged);
            // 
            // dtpEligibility
            // 
            this.dtpEligibility.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpEligibility.Checked = false;
            this.dtpEligibility.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEligibility.Location = new System.Drawing.Point(188, 14);
            this.dtpEligibility.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dtpEligibility.Name = "dtpEligibility";
            this.dtpEligibility.Size = new System.Drawing.Size(21, 20);
            this.dtpEligibility.TabIndex = 0;
            this.dtpEligibility.TabStop = false;
            this.dtpEligibility.CloseUp += new System.EventHandler(this.dtpEligibility_CloseUp);
            // 
            // mtbEligibilityDate
            // 
            this.mtbEligibilityDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEligibilityDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbEligibilityDate.Location = new System.Drawing.Point(120, 14);
            this.mtbEligibilityDate.Mask = "  /  /";
            this.mtbEligibilityDate.MaxLength = 10;
            this.mtbEligibilityDate.Name = "mtbEligibilityDate";
            this.mtbEligibilityDate.Size = new System.Drawing.Size(70, 20);
            this.mtbEligibilityDate.TabIndex = 1;
            this.mtbEligibilityDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbEligibilityDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEligibilityDate_Validating);
            this.mtbEligibilityDate.Enter += new System.EventHandler(this.mtbEligibilityDate_Enter);
            // 
            // lblPatientHasMedicare
            // 
            this.lblPatientHasMedicare.Location = new System.Drawing.Point(8, 51);
            this.lblPatientHasMedicare.Name = "lblPatientHasMedicare";
            this.lblPatientHasMedicare.Size = new System.Drawing.Size(115, 23);
            this.lblPatientHasMedicare.TabIndex = 0;
            this.lblPatientHasMedicare.Text = "Patient has Medicare:";
            // 
            // lblEligibilityDate
            // 
            this.lblEligibilityDate.Location = new System.Drawing.Point(8, 17);
            this.lblEligibilityDate.Name = "lblEligibilityDate";
            this.lblEligibilityDate.Size = new System.Drawing.Size(100, 18);
            this.lblEligibilityDate.TabIndex = 0;
            this.lblEligibilityDate.Text = "Eligibility date:";
            // 
            // MedicaidVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel);
            this.Name = "MedicaidVerifyView";
            this.Size = new System.Drawing.Size(847, 187);
            this.Disposed += new System.EventHandler(this.MedicaidVerifyView_Disposed);
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public MedicaidVerifyView()
        {
            InitializeComponent();

            ConfigureControls();

            loadingModelData = true;
            base.EnableThemesOn( this );

            blankYesNoFlag = new YesNoFlag();
            blankYesNoFlag.SetBlank();
            yesYesNoFlag = new YesNoFlag();
            yesYesNoFlag.SetYes();
            noYesNoFlag = new YesNoFlag();
            noYesNoFlag.SetNo();
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

        private LoggingButton                 btnClearAll;

        private ComboBox               cmbOtherCoverage;
        private ComboBox               cmbPatientHasMedicare;
        private ComboBox               cmbInfoRecvFrom;

        private DateTimePicker         dtpEligibility;

        private Label                  lblRemarks;
        private Label                  lblInfoRecvFrom;
        private Label                  lblMedicaidCoPay;
        private Label                  lblOtherCoverage;
        private Label                  lblPatientHasMedicare;
        private Label                  lblEligibilityDate;
        private Label                  lblEvcNumber;

        private Panel                  panel;

        private MaskedEditTextBox    mtbMedicaidCoPay;
        private MaskedEditTextBox    mtbEligibilityDate;
        private MaskedEditTextBox    mtbRemarks;
        private MaskedEditTextBox    mtbEvcNumber;

        private bool                                        loadingModelData;
        private int                                         insuranceMonth;
        private int                                         insuranceDay;
        private int                                         insuranceYear;
        private double                                      medicaidCopay;
        private string                                      remarks;
        private string                                      EVCNumber;
        private Account                                     i_Account;
        private DateTime                                    eligibilityDate;
        private YesNoFlag                                   patientHasMedicare;
        private YesNoFlag                                   patienthasOtherInsuranceCoverage;
        private YesNoFlag                                   blankYesNoFlag;
        private YesNoFlag                                   yesYesNoFlag;
        private YesNoFlag                                   noYesNoFlag;
        private InformationReceivedSource                   informationReceivedSource;
        private RuleEngine                                  i_RuleEngine;
        private bool                                        medicaidIsPrimary;
        #endregion

        #region Constants
        #endregion


    }
}
