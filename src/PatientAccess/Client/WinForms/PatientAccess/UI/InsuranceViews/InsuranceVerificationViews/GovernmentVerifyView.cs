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
    /// Summary description for GovernmentVerifyView.
    /// </summary>
    public class GovernmentVerifyView : ControlView
    {
        #region Event Handlers

		private void mtbDeductible_Enter(object sender, EventArgs e)
		{						
            if( this.mtbDeductible.UnMaskedText != string.Empty )
			{
                decimal deductible = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbDeductible );
				this.mtbDeductible.UnMaskedText = deductible.ToString();
			}        
		}

		private void mtbCoPayAmount_Enter(object sender, EventArgs e)
		{			
			if( this.mtbCoPayAmount.UnMaskedText != string.Empty )
			{
				decimal coPayAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCoPayAmount );
                this.mtbCoPayAmount.UnMaskedText = coPayAmount.ToString();
			}
		}

		private void  mtbDeductibleDollarsMet_Enter(object sender, EventArgs e)
		{			
			if( this.mtbDeductibleDollarsMet.UnMaskedText != string.Empty )
			{
                decimal deductibleDollarsMet = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbDeductibleDollarsMet );	
				this.mtbDeductibleDollarsMet.UnMaskedText = deductibleDollarsMet.ToString();
			}
		}

		private void  mtbOutOfPocketAmount_Enter(object sender, EventArgs e)
		{			
			if( this.mtbOutOfPocketAmount.UnMaskedText != string.Empty )
			{
                decimal outOfPocketAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbOutOfPocketAmount );	
				this.mtbOutOfPocketAmount.UnMaskedText = outOfPocketAmount.ToString();
			}     
		}

		private void  mtbOutOfPocketDollarsMet_Enter(object sender, EventArgs e)
		{			
			if( this.mtbOutOfPocketDollarsMet.UnMaskedText != string.Empty )
			{
                decimal outOfPocketDollarsMet = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbOutOfPocketDollarsMet );	
				this.mtbOutOfPocketDollarsMet.UnMaskedText = outOfPocketDollarsMet.ToString();
			}
		}

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ResetView();
        }

        private void GovtInfoRecvdFromPreferredEvent(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( cmbInfoRecvFrom );
        }

        private void GovtEffectiveDatePreferredEvent(object sender, EventArgs e)
        {
            UIColors.SetPreferredBgColor( mtbEffectiveDate );
        }

        private void dtpEffectiveDate_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbEffectiveDate );
            DateTime dt = dtp.Value;
            mtbEffectiveDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbEffectiveDate.Focus();
        }

        private void mtbEffectiveDate_Enter(object sender, EventArgs e)
        {
            Refresh();
        }

        private void mtbEffectiveDate_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor( this.mtbEffectiveDate );

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( dtpEffectiveDate.Focused )
            {
                return;
            }
            if( mtb.UnMaskedText != String.Empty )
            {
                if( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
                {
                    UIColors.SetNormalBgColor( mtb );
                    effectiveInsuredDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                effectiveInsuredDate = DateTime.MinValue;
            }

            Model_Coverage.EffectiveDateForInsured = effectiveInsuredDate;

            RuleEngine.GetInstance().EvaluateRule( typeof( GovtEffectiveDatePreferred ), Model_Coverage );
        }

        private void dtpTerminationDate_CloseUp(object sender, EventArgs e)
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbTerminationDate );
            DateTime dt = dtp.Value;
            mtbTerminationDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbTerminationDate.Focus();
        }

        private void mskTerminationDate_Enter(object sender, EventArgs e)
        {
            Refresh();
        }

        private void mtbTerminationDate_Validating(object sender, CancelEventArgs e)
        {
            UIColors.SetNormalBgColor( this.mtbTerminationDate );

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( dtpTerminationDate.Focused )
            {
                return;
            }
            if( mtb.UnMaskedText != String.Empty )
            {
                if( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
                {
                    UIColors.SetNormalBgColor( mtb );
                    terminationInsuredDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                terminationInsuredDate = DateTime.MinValue;
            }
        }

        private void mtbEligibilityPhone_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                eligibilityPhone = mtb.UnMaskedText;
            }
            else
            {
                eligibilityPhone = String.Empty;
            }
        }

        private void mtbInsuranceRepName_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                insuranceRepName = mtb.UnMaskedText.Trim();
            }
            else
            {
                insuranceRepName = String.Empty;
            }	
        }

        private void mtbTypeOfCoverage_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                typeOfCoverage = mtb.UnMaskedText.Trim();
            }
            else
            {
                typeOfCoverage = String.Empty;
            }
        }

		private void mtbDeductible_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;
			
            if( mtb.UnMaskedText.Trim() != String.Empty )
			{
				deductible = Convert.ToDecimal( mtb.UnMaskedText );
                CommonFormatting.FormatTextBoxCurrency( mtbDeductible, "###,###,###.##" );
			}		
		}

        private void cmbDeductibleMet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                deductibleMet = cb.SelectedItem as YesNoFlag;

                if( deductibleMet.Code.Equals( "N" ) )
                {
                    mtbDeductibleDollarsMet.Enabled = true;
                    mtbDeductibleDollarsMet.UnMaskedText = String.Empty;
                }
                else if( deductibleMet.Code.Equals( "Y" ) )
                {
                    mtbDeductibleDollarsMet.UnMaskedText = mtbDeductible.Text;
                    mtbDeductibleDollarsMet.Enabled = false;
                }
                else
                {
                    mtbDeductibleDollarsMet.Enabled = true;
                    mtbDeductibleDollarsMet.UnMaskedText = String.Empty;
                }
            }

        }

        private void mtbDeductibleDollarsMet_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText.Trim() != String.Empty )
            {
                deductibleDollarsMet = Convert.ToDouble( mtb.UnMaskedText.Trim() );
                CommonFormatting.FormatTextBoxCurrency( mtbDeductibleDollarsMet, "###,###,###.##" );
            }          			
        }

        private void mtbCoInsurancePercent_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                coInsurance = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                coInsurance = -1;
            }
        }

		private void mtbOutOfPocketAmount_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText.Trim() != String.Empty )
			{
				outOfPocket = Convert.ToDouble( mtb.UnMaskedText );
                CommonFormatting.FormatTextBoxCurrency( mtbOutOfPocketAmount, "###,###,###.##" );
			}
		}

        private void cmbOutOfPocketMet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                outOfPocketMet = cb.SelectedItem as YesNoFlag;

                if( outOfPocketMet.Code.Equals( "N" ) )
                {
                    mtbOutOfPocketDollarsMet.Enabled = true;
                    mtbOutOfPocketDollarsMet.UnMaskedText = String.Empty;
                }
                else if( outOfPocketMet.Code.Equals( "Y" ) )
                {
                    mtbOutOfPocketDollarsMet.UnMaskedText = mtbOutOfPocketAmount.Text;
                    mtbOutOfPocketDollarsMet.Enabled = false;
                }
                else
                {
                    mtbOutOfPocketDollarsMet.Enabled = true;
                    mtbOutOfPocketDollarsMet.UnMaskedText = String.Empty;
                }
            }

        }

        private void mtbOutOfPocketDollarsMet_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if( mtb.UnMaskedText.Trim() != String.Empty )
            {
                outOfPocketDollarsMet = Convert.ToDouble( mtb.UnMaskedText.Trim() );
                CommonFormatting.FormatTextBoxCurrency( mtbOutOfPocketDollarsMet, "###,###,###.##" );
            }
        }
		
        private void mtbPercentAfterOutOfPocket_Validating(object sender, CancelEventArgs e)
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                afterOutOfPocketPercent = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                afterOutOfPocketPercent = -1;
            }
        }

		private void mtbCoPayAmount_Validating(object sender, CancelEventArgs e)
		{
			MaskedEditTextBox mtb = sender as MaskedEditTextBox;

			if( mtb.UnMaskedText.Trim() != String.Empty )
			{
				coPayAmount = Convert.ToDecimal(  mtb.UnMaskedText );
                CommonFormatting.FormatTextBoxCurrency( mtbCoPayAmount, "###,###,###.##" );
			}
		}

        private void txtRemarks_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if( tb.Text.Trim() != String.Empty )
            {
                remarks = tb.Text.Trim();
            }
            else
            {
                remarks = String.Empty;
            }
        }

        private void cmbInfoRecvFrom_DropDown(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetPreferredBgColor( cb );
        }

        private void cmbInfoRecvFrom_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbInfoRecvFrom );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 && cb.Text.Equals( " " ) == false )
            {
                informationReceivedSource = cb.SelectedItem as InformationReceivedSource;
                Model_Coverage.InformationReceivedSource = informationReceivedSource;
            }
            
            RuleEngine.GetInstance().EvaluateRule( typeof( GovtInfoRecvdFromPreferred ), Model_Coverage );
        }

        #endregion

        #region Methods

        public override void UpdateView()
        {
           
            if( loadingModelData )
            {
                loadingModelData = false;
                
                PopulateInfoRecvComboBox();
                PopulateDeductibleMetComboBox();
                PopulateOutOfPocketMetComboBox();
            }

            this.cmbInfoRecvFrom.SelectedItem               = this.Model_Coverage.InformationReceivedSource;
            this.informationReceivedSource                  = this.Model_Coverage.InformationReceivedSource;

            this.mtbEligibilityPhone.UnMaskedText           = this.Model_Coverage.EligibilityPhone;
            this.eligibilityPhone                           = this.Model_Coverage.EligibilityPhone;
            
            this.mtbInsuranceRepName.UnMaskedText           = this.Model_Coverage.InsuranceCompanyRepName;
            this.insuranceRepName                           = this.Model_Coverage.InsuranceCompanyRepName;

            if( this.Model_Coverage.EffectiveDateForInsured == DateTime.MinValue )
            {
                this.mtbEffectiveDate.UnMaskedText = string.Empty;
            }
            else
            {
                this.mtbEffectiveDate.UnMaskedText          = this.Model_Coverage.EffectiveDateForInsured.ToString("MMddyyyy");
            }
            this.effectiveInsuredDate                       = this.Model_Coverage.EffectiveDateForInsured;

            if( this.Model_Coverage.TerminationDateForInsured == DateTime.MinValue )
            {
                this.mtbTerminationDate.UnMaskedText = string.Empty;
            }
            else
            {
                this.mtbTerminationDate.UnMaskedText        = this.Model_Coverage.TerminationDateForInsured.ToString("MMddyyyy");
            }
            this.terminationInsuredDate                     = this.Model_Coverage.TerminationDateForInsured;

            this.mtbTypeOfCoverage.UnMaskedText             = this.Model_Coverage.TypeOfCoverage;
            this.typeOfCoverage                             = this.Model_Coverage.TypeOfCoverage;
            
			BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetails;
            
            if( bcd == null )
            {
                bcd = new BenefitsCategoryDetails();
            }
			
			if( bcd != null )
			{
                this.outOfPocket                        = bcd.OutOfPocket;
                this.outOfPocketMet                     = bcd.OutOfPocketMet;
                this.outOfPocketDollarsMet              = bcd.OutOfPocketDollarsMet;
                this.deductible                         = (decimal)bcd.Deductible;
                this.deductibleDollarsMet               = bcd.DeductibleDollarsMet;
                this.cmbDeductibleMet.SelectedItem      = bcd.DeductibleMet;
                this.coPayAmount                        = (decimal)bcd.CoPay;
                this.cmbOutOfPocketMet.SelectedItem     = bcd.OutOfPocketMet;
                this.afterOutOfPocketPercent            = bcd.AfterOutOfPocketPercent;
                this.deductibleMet                      = bcd.DeductibleMet;
                this.coInsurance                        = bcd.CoInsurance;
                
                if( this.coInsurance != -1 )
                {
                    this.mtbCoInsurancePercent.Text = this.coInsurance.ToString();
                }
                else
                {
                    this.mtbCoInsurancePercent.Text = string.Empty;
                }

                if( this.outOfPocket != -1 )
                {
                    this.mtbOutOfPocketAmount.Text = this.outOfPocket.ToString( "###,###,###.##" );
                }
                else
                {
                    this.mtbOutOfPocketAmount.Text = string.Empty;
                }

                if( this.outOfPocketDollarsMet != -1 )
                {
                    this.mtbOutOfPocketDollarsMet.Text = outOfPocketDollarsMet.ToString( "###,###,###.##" );
                }
                else
                {
                    this.mtbOutOfPocketDollarsMet.Text = string.Empty;
                }
                if( this.afterOutOfPocketPercent != -1 )
                {
                    this.mtbPercentAfterOutOfPocket.Text = this.afterOutOfPocketPercent.ToString();
                }
                else
                {
                    this.mtbPercentAfterOutOfPocket.Text = string.Empty;
                }

                if( this.Model_Coverage.BenefitsCategoryDetails.Deductible != -1 )
                {
                    this.mtbDeductible.Text = deductible.ToString( "###,###,###.##" );
                }
                else
                {
                    this.mtbDeductible.Clear();
                }                    

                if( this.Model_Coverage.BenefitsCategoryDetails.DeductibleDollarsMet != -1 )
                {
                    mtbDeductibleDollarsMet.Text = this.deductibleDollarsMet.ToString( "###,###,###.##" );
                }
                else
                {
                    mtbDeductibleDollarsMet.Clear();
                }

                if( this.coPayAmount != -1 )
                {
                    this.mtbCoPayAmount.Text = this.coPayAmount.ToString( "###,###,###.##" );
                }
                else
                {
                    this.mtbCoPayAmount.Text = string.Empty;
                }
			}
			

            this.remarks                                        = this.Model_Coverage.Remarks;
            this.mtbRemarks.UnMaskedText                        = this.Model_Coverage.Remarks;

            //RuleEngine.LoadRules( Account );

            RuleEngine.GetInstance().RegisterEvent( typeof( GovtInfoRecvdFromPreferred ), Model_Coverage, new EventHandler( GovtInfoRecvdFromPreferredEvent ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( GovtEffectiveDatePreferred ), Model_Coverage, new EventHandler( GovtEffectiveDatePreferredEvent ) );
            
            CheckForRequiredFields();
        

            bool result = true;
            int year    = 0;
            int month   = 0;
            int day     = 0;

            if( mtbEffectiveDate.UnMaskedText != String.Empty )
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbEffectiveDate, ref year, ref month, ref day );
            }
            if( result && mtbTerminationDate.UnMaskedText != String.Empty )
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbTerminationDate, ref year, ref month, ref day );
            }
        }

        public override void UpdateModel()
        {
            if( this.cmbInfoRecvFrom.SelectedItem != null )
            {
                Model_Coverage.InformationReceivedSource = this.cmbInfoRecvFrom.SelectedItem as InformationReceivedSource;
            }

            if( !string.IsNullOrEmpty( this.mtbEligibilityPhone.Text.Trim() ) )
            {
                this.mtbEligibilityPhone_Validating( this.mtbEligibilityPhone, null );
                Model_Coverage.EligibilityPhone = this.mtbEligibilityPhone.UnMaskedText.Trim();
            }

            if( !string.IsNullOrEmpty( this.mtbInsuranceRepName.Text.Trim() ) )
            {
                this.mtbInsuranceRepName_Validating( this.mtbInsuranceRepName, null );
                Model_Coverage.InsuranceCompanyRepName = this.mtbInsuranceRepName.UnMaskedText.Trim();
            }

            if( !string.IsNullOrEmpty( this.mtbTypeOfCoverage.Text.Trim() ) )
            {
                this.mtbTypeOfCoverage_Validating( this.mtbTypeOfCoverage, null );
                Model_Coverage.TypeOfCoverage = this.mtbTypeOfCoverage.UnMaskedText.Trim();
            }

            //this.mtbEffectiveDate_Validating(this.mtbEffectiveDate, null);
            Model_Coverage.EffectiveDateForInsured = effectiveInsuredDate;

            //this.mtbTerminationDate_Validating(this.mtbTerminationDate, null);
            Model_Coverage.TerminationDateForInsured = terminationInsuredDate;

            if( !string.IsNullOrEmpty( this.mtbDeductible.Text ) )
            {
                this.mtbDeductible_Validating( this.mtbDeductible, null );
                Model_Coverage.BenefitsCategoryDetails.Deductible = float.Parse( this.mtbDeductible.UnMaskedText.Trim() );
            }

            if( !string.IsNullOrEmpty( this.mtbDeductibleDollarsMet.Text ) )
            {
                this.mtbDeductibleDollarsMet_Validating( this.mtbDeductibleDollarsMet, null );
                Model_Coverage.BenefitsCategoryDetails.DeductibleDollarsMet = float.Parse(this.mtbDeductibleDollarsMet.UnMaskedText.Trim());
            }
       
            if( this.cmbDeductibleMet.SelectedItem != null )
            {
                Model_Coverage.BenefitsCategoryDetails.DeductibleMet = this.cmbDeductibleMet.SelectedItem as YesNoFlag;
            }

            if( !string.IsNullOrEmpty( this.mtbCoInsurancePercent.Text ) )
            {
                this.mtbCoInsurancePercent_Validating( this.mtbCoInsurancePercent, null );
                Model_Coverage.BenefitsCategoryDetails.CoInsurance = int.Parse( this.mtbCoInsurancePercent.UnMaskedText.Trim() );
            }

            if( !string.IsNullOrEmpty( this.mtbOutOfPocketAmount.Text ) )
            {
                this.mtbOutOfPocketAmount_Validating( this.mtbOutOfPocketAmount, null );
                Model_Coverage.BenefitsCategoryDetails.OutOfPocket = float.Parse( this.mtbOutOfPocketAmount.UnMaskedText.Trim());
            }

            if( this.cmbOutOfPocketMet.SelectedItem != null )
            {
                Model_Coverage.BenefitsCategoryDetails.OutOfPocketMet = this.cmbOutOfPocketMet.SelectedItem as YesNoFlag;
            }

            if( !string.IsNullOrEmpty( this.mtbOutOfPocketDollarsMet.Text ) )
            {
                this.mtbOutOfPocketDollarsMet_Validating( this.mtbOutOfPocketDollarsMet, null );
                Model_Coverage.BenefitsCategoryDetails.OutOfPocketDollarsMet = float.Parse( this.mtbOutOfPocketDollarsMet.UnMaskedText.Trim() );
            }

            if( !string.IsNullOrEmpty( this.mtbPercentAfterOutOfPocket.Text ) )
            {
                this.mtbPercentAfterOutOfPocket_Validating( this.mtbPercentAfterOutOfPocket, null );
                Model_Coverage.BenefitsCategoryDetails.AfterOutOfPocketPercent = int.Parse( this.mtbPercentAfterOutOfPocket.UnMaskedText.Trim() );
            }

            if( !string.IsNullOrEmpty( this.mtbCoPayAmount.Text ) )
            {
                this.mtbCoPayAmount_Validating( this.mtbCoPayAmount, null );
                Model_Coverage.BenefitsCategoryDetails.CoPay = float.Parse( this.mtbCoPayAmount.UnMaskedText.Trim());
            }

            if( !string.IsNullOrEmpty( this.mtbRemarks.Text.Trim() ) )
            {
                Model_Coverage.Remarks = this.mtbRemarks.UnMaskedText;
            }

        }
        #endregion

        #region Properties
        [Browsable(false)]
        public GovernmentOtherCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentOtherCoverage)this.Model;
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
            RuleEngine.GetInstance().EvaluateRule( typeof( GovtInfoRecvdFromPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( GovtEffectiveDatePreferred ), Model_Coverage  );
        }

        private void PopulateDeductibleMetComboBox()
        {
            cmbDeductibleMet.Items.Add( blankYesNoFlag );
            cmbDeductibleMet.Items.Add( yesYesNoFlag );
            cmbDeductibleMet.Items.Add( noYesNoFlag );
        }

        private void PopulateOutOfPocketMetComboBox()
        {
            cmbOutOfPocketMet.Items.Add( blankYesNoFlag );
            cmbOutOfPocketMet.Items.Add( yesYesNoFlag );
            cmbOutOfPocketMet.Items.Add( noYesNoFlag );
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
            Model_Coverage.RemoveCoverageVerificationData();
            this.UpdateView();
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
            this.btnClearAll = new LoggingButton();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.mtbCoPayAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticCoPay = new System.Windows.Forms.Label();
            this.mtbPercentAfterOutOfPocket = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblPercent2 = new System.Windows.Forms.Label();
            this.lblStaticOutOfPocketPercent = new System.Windows.Forms.Label();
            this.mtbOutOfPocketDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticOutOfPocketDollarsMet = new System.Windows.Forms.Label();
            this.cmbOutOfPocketMet = new System.Windows.Forms.ComboBox();
            this.lblStaticOutOfPocketMet = new System.Windows.Forms.Label();
            this.mtbOutOfPocketAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticOutOfPocket = new System.Windows.Forms.Label();
            this.lblPercent1 = new System.Windows.Forms.Label();
            this.mtbCoInsurancePercent = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDeductibleDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticCoInsurance = new System.Windows.Forms.Label();
            this.lblStaticDollarsMet = new System.Windows.Forms.Label();
            this.cmbDeductibleMet = new System.Windows.Forms.ComboBox();
            this.lblStaticDeductibleMet = new System.Windows.Forms.Label();
            this.mtbDeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDeductible = new System.Windows.Forms.Label();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.dtpTerminationDate = new System.Windows.Forms.DateTimePicker();
            this.mtbTerminationDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticTermDate = new System.Windows.Forms.Label();
            this.dtpEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.mtbEffectiveDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEffectiveDate = new System.Windows.Forms.Label();
            this.mtbTypeOfCoverage = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticTypeOfCoverage = new System.Windows.Forms.Label();
            this.mtbInsuranceRepName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticInsuranceRep = new System.Windows.Forms.Label();
            this.mtbEligibilityPhone = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEligibilityPhone = new System.Windows.Forms.Label();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
            this.lblStaticInfoRecvFrom = new System.Windows.Forms.Label();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.Controls.Add(this.btnClearAll);
            this.panel.Controls.Add(this.mtbRemarks);
            this.panel.Controls.Add(this.lblRemarks);
            this.panel.Controls.Add(this.mtbCoPayAmount);
            this.panel.Controls.Add(this.lblStaticCoPay);
            this.panel.Controls.Add(this.mtbPercentAfterOutOfPocket);
            this.panel.Controls.Add(this.lblPercent2);
            this.panel.Controls.Add(this.lblStaticOutOfPocketPercent);
            this.panel.Controls.Add(this.mtbOutOfPocketDollarsMet);
            this.panel.Controls.Add(this.lblStaticOutOfPocketDollarsMet);
            this.panel.Controls.Add(this.cmbOutOfPocketMet);
            this.panel.Controls.Add(this.lblStaticOutOfPocketMet);
            this.panel.Controls.Add(this.mtbOutOfPocketAmount);
            this.panel.Controls.Add(this.lblStaticOutOfPocket);
            this.panel.Controls.Add(this.lblPercent1);
            this.panel.Controls.Add(this.mtbCoInsurancePercent);
            this.panel.Controls.Add(this.mtbDeductibleDollarsMet);
            this.panel.Controls.Add(this.lblStaticCoInsurance);
            this.panel.Controls.Add(this.lblStaticDollarsMet);
            this.panel.Controls.Add(this.cmbDeductibleMet);
            this.panel.Controls.Add(this.lblStaticDeductibleMet);
            this.panel.Controls.Add(this.mtbDeductible);
            this.panel.Controls.Add(this.lblStaticDeductible);
            this.panel.Controls.Add(this.lineLabel);
            this.panel.Controls.Add(this.dtpTerminationDate);
            this.panel.Controls.Add(this.mtbTerminationDate);
            this.panel.Controls.Add(this.lblStaticTermDate);
            this.panel.Controls.Add(this.dtpEffectiveDate);
            this.panel.Controls.Add(this.mtbEffectiveDate);
            this.panel.Controls.Add(this.lblStaticEffectiveDate);
            this.panel.Controls.Add(this.mtbTypeOfCoverage);
            this.panel.Controls.Add(this.lblStaticTypeOfCoverage);
            this.panel.Controls.Add(this.mtbInsuranceRepName);
            this.panel.Controls.Add(this.lblStaticInsuranceRep);
            this.panel.Controls.Add(this.mtbEligibilityPhone);
            this.panel.Controls.Add(this.lblStaticEligibilityPhone);
            this.panel.Controls.Add(this.cmbInfoRecvFrom);
            this.panel.Controls.Add(this.lblStaticInfoRecvFrom);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(847, 300);
            this.panel.TabIndex = 0;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(740, 263);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.TabIndex = 17;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point(570, 199);
            this.mtbRemarks.Mask = string.Empty;
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size(230, 48);
            this.mtbRemarks.TabIndex = 16;
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler(this.txtRemarks_Validating);
            // 
            // lblRemarks
            // 
            this.lblRemarks.Location = new System.Drawing.Point(512, 202);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(56, 23);
            this.lblRemarks.TabIndex = 0;
            this.lblRemarks.Text = "Remarks:";
            // 
            // mtbCoPayAmount
            // 
            this.mtbCoPayAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoPayAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbCoPayAmount.Location = new System.Drawing.Point(570, 165);
            this.mtbCoPayAmount.Mask = string.Empty;
            this.mtbCoPayAmount.MaxLength = 10;
            this.mtbCoPayAmount.Name = "mtbCoPayAmount";
            this.mtbCoPayAmount.Size = new System.Drawing.Size(60, 20);
            this.mtbCoPayAmount.TabIndex = 15;
            this.mtbCoPayAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCoPayAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbCoPayAmount.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCoPayAmount_Validating);
            this.mtbCoPayAmount.Enter += new System.EventHandler(this.mtbCoPayAmount_Enter);
            // 
            // lblStaticCoPay
            // 
            this.lblStaticCoPay.Location = new System.Drawing.Point(512, 168);
            this.lblStaticCoPay.Name = "lblStaticCoPay";
            this.lblStaticCoPay.Size = new System.Drawing.Size(59, 23);
            this.lblStaticCoPay.TabIndex = 0;
            this.lblStaticCoPay.Text = "Co-pay:   $";
            // 
            // mtbPercentAfterOutOfPocket
            // 
            this.mtbPercentAfterOutOfPocket.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbPercentAfterOutOfPocket.KeyPressExpression = "^\\d*";
            this.mtbPercentAfterOutOfPocket.Location = new System.Drawing.Point(360, 267);
            this.mtbPercentAfterOutOfPocket.Mask = string.Empty;
            this.mtbPercentAfterOutOfPocket.MaxLength = 3;
            this.mtbPercentAfterOutOfPocket.Name = "mtbPercentAfterOutOfPocket";
            this.mtbPercentAfterOutOfPocket.Size = new System.Drawing.Size(30, 20);
            this.mtbPercentAfterOutOfPocket.TabIndex = 14;
            this.mtbPercentAfterOutOfPocket.ValidationExpression = "^\\d*";
            this.mtbPercentAfterOutOfPocket.Validating += new System.ComponentModel.CancelEventHandler(this.mtbPercentAfterOutOfPocket_Validating);
            // 
            // lblPercent2
            // 
            this.lblPercent2.Location = new System.Drawing.Point(394, 270);
            this.lblPercent2.Name = "lblPercent2";
            this.lblPercent2.Size = new System.Drawing.Size(24, 23);
            this.lblPercent2.TabIndex = 0;
            this.lblPercent2.Text = "%";
            // 
            // lblStaticOutOfPocketPercent
            // 
            this.lblStaticOutOfPocketPercent.Location = new System.Drawing.Point(250, 270);
            this.lblStaticOutOfPocketPercent.Name = "lblStaticOutOfPocketPercent";
            this.lblStaticOutOfPocketPercent.Size = new System.Drawing.Size(115, 23);
            this.lblStaticOutOfPocketPercent.TabIndex = 0;
            this.lblStaticOutOfPocketPercent.Text = "% after out-of-pocket:";
            // 
            // mtbOutOfPocketDollarsMet
            // 
            this.mtbOutOfPocketDollarsMet.Enabled = false;
            this.mtbOutOfPocketDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbOutOfPocketDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketDollarsMet.Location = new System.Drawing.Point(416, 233);
            this.mtbOutOfPocketDollarsMet.Mask = string.Empty;
            this.mtbOutOfPocketDollarsMet.MaxLength = 10;
            this.mtbOutOfPocketDollarsMet.Name = "mtbOutOfPocketDollarsMet";
            this.mtbOutOfPocketDollarsMet.Size = new System.Drawing.Size(60, 20);
            this.mtbOutOfPocketDollarsMet.TabIndex = 13;
            this.mtbOutOfPocketDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketDollarsMet.Validating += new System.ComponentModel.CancelEventHandler(this.mtbOutOfPocketDollarsMet_Validating);
            this.mtbOutOfPocketDollarsMet.Enter += new System.EventHandler(this.mtbOutOfPocketDollarsMet_Enter);
            // 
            // lblStaticOutOfPocketDollarsMet
            // 
            this.lblStaticOutOfPocketDollarsMet.Location = new System.Drawing.Point(340, 236);
            this.lblStaticOutOfPocketDollarsMet.Name = "lblStaticOutOfPocketDollarsMet";
            this.lblStaticOutOfPocketDollarsMet.Size = new System.Drawing.Size(78, 23);
            this.lblStaticOutOfPocketDollarsMet.TabIndex = 0;
            this.lblStaticOutOfPocketDollarsMet.Text = "Dollars met:  $";
            // 
            // cmbOutOfPocketMet
            // 
            this.cmbOutOfPocketMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutOfPocketMet.Location = new System.Drawing.Point(371, 198);
            this.cmbOutOfPocketMet.Name = "cmbOutOfPocketMet";
            this.cmbOutOfPocketMet.Size = new System.Drawing.Size(50, 21);
            this.cmbOutOfPocketMet.TabIndex = 12;
            this.cmbOutOfPocketMet.SelectedIndexChanged += new System.EventHandler(this.cmbOutOfPocketMet_SelectedIndexChanged);
            // 
            // lblStaticOutOfPocketMet
            // 
            this.lblStaticOutOfPocketMet.Location = new System.Drawing.Point(340, 202);
            this.lblStaticOutOfPocketMet.Name = "lblStaticOutOfPocketMet";
            this.lblStaticOutOfPocketMet.Size = new System.Drawing.Size(35, 23);
            this.lblStaticOutOfPocketMet.TabIndex = 0;
            this.lblStaticOutOfPocketMet.Text = "Met:";
            // 
            // mtbOutOfPocketAmount
            // 
            this.mtbOutOfPocketAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbOutOfPocketAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketAmount.Location = new System.Drawing.Point(340, 165);
            this.mtbOutOfPocketAmount.Mask = string.Empty;
            this.mtbOutOfPocketAmount.MaxLength = 10;
            this.mtbOutOfPocketAmount.Name = "mtbOutOfPocketAmount";
            this.mtbOutOfPocketAmount.Size = new System.Drawing.Size(60, 20);
            this.mtbOutOfPocketAmount.TabIndex = 11;
            this.mtbOutOfPocketAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketAmount.Validating += new System.ComponentModel.CancelEventHandler(this.mtbOutOfPocketAmount_Validating);
            this.mtbOutOfPocketAmount.Enter += new System.EventHandler(this.mtbOutOfPocketAmount_Enter);
            // 
            // lblStaticOutOfPocket
            // 
            this.lblStaticOutOfPocket.Location = new System.Drawing.Point(250, 168);
            this.lblStaticOutOfPocket.Name = "lblStaticOutOfPocket";
            this.lblStaticOutOfPocket.Size = new System.Drawing.Size(88, 23);
            this.lblStaticOutOfPocket.TabIndex = 0;
            this.lblStaticOutOfPocket.Text = "Out-of-pocket:  $";
            // 
            // lblPercent1
            // 
            this.lblPercent1.Location = new System.Drawing.Point(117, 270);
            this.lblPercent1.Name = "lblPercent1";
            this.lblPercent1.Size = new System.Drawing.Size(24, 23);
            this.lblPercent1.TabIndex = 0;
            this.lblPercent1.Text = "%";
            // 
            // mtbCoInsurancePercent
            // 
            this.mtbCoInsurancePercent.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoInsurancePercent.KeyPressExpression = "^\\d*";
            this.mtbCoInsurancePercent.Location = new System.Drawing.Point(83, 267);
            this.mtbCoInsurancePercent.Mask = string.Empty;
            this.mtbCoInsurancePercent.MaxLength = 3;
            this.mtbCoInsurancePercent.Name = "mtbCoInsurancePercent";
            this.mtbCoInsurancePercent.Size = new System.Drawing.Size(30, 20);
            this.mtbCoInsurancePercent.TabIndex = 10;
            this.mtbCoInsurancePercent.ValidationExpression = "^\\d*";
            this.mtbCoInsurancePercent.Validating += new System.ComponentModel.CancelEventHandler(this.mtbCoInsurancePercent_Validating);
            // 
            // mtbDeductibleDollarsMet
            // 
            this.mtbDeductibleDollarsMet.Enabled = false;
            this.mtbDeductibleDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductibleDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductibleDollarsMet.Location = new System.Drawing.Point(161, 233);
            this.mtbDeductibleDollarsMet.Mask = string.Empty;
            this.mtbDeductibleDollarsMet.MaxLength = 10;
            this.mtbDeductibleDollarsMet.Name = "mtbDeductibleDollarsMet";
            this.mtbDeductibleDollarsMet.Size = new System.Drawing.Size(60, 20);
            this.mtbDeductibleDollarsMet.TabIndex = 9;
            this.mtbDeductibleDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductibleDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductibleDollarsMet.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDeductibleDollarsMet_Validating);
            this.mtbDeductibleDollarsMet.Enter += new System.EventHandler(this.mtbDeductibleDollarsMet_Enter);
            // 
            // lblStaticCoInsurance
            // 
            this.lblStaticCoInsurance.Location = new System.Drawing.Point(8, 270);
            this.lblStaticCoInsurance.Name = "lblStaticCoInsurance";
            this.lblStaticCoInsurance.Size = new System.Drawing.Size(76, 23);
            this.lblStaticCoInsurance.TabIndex = 0;
            this.lblStaticCoInsurance.Text = "Co-insurance:";
            // 
            // lblStaticDollarsMet
            // 
            this.lblStaticDollarsMet.Location = new System.Drawing.Point(84, 236);
            this.lblStaticDollarsMet.Name = "lblStaticDollarsMet";
            this.lblStaticDollarsMet.Size = new System.Drawing.Size(78, 23);
            this.lblStaticDollarsMet.TabIndex = 0;
            this.lblStaticDollarsMet.Text = "Dollars met:  $";
            // 
            // cmbDeductibleMet
            // 
            this.cmbDeductibleMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDeductibleMet.Location = new System.Drawing.Point(120, 198);
            this.cmbDeductibleMet.Name = "cmbDeductibleMet";
            this.cmbDeductibleMet.Size = new System.Drawing.Size(50, 21);
            this.cmbDeductibleMet.TabIndex = 8;
            this.cmbDeductibleMet.SelectedIndexChanged += new System.EventHandler(this.cmbDeductibleMet_SelectedIndexChanged);
            // 
            // lblStaticDeductibleMet
            // 
            this.lblStaticDeductibleMet.Location = new System.Drawing.Point(84, 202);
            this.lblStaticDeductibleMet.Name = "lblStaticDeductibleMet";
            this.lblStaticDeductibleMet.Size = new System.Drawing.Size(35, 23);
            this.lblStaticDeductibleMet.TabIndex = 0;
            this.lblStaticDeductibleMet.Text = "Met:";
            // 
            // mtbDeductible
            // 
            this.mtbDeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductible.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductible.Location = new System.Drawing.Point(84, 165);
            this.mtbDeductible.Mask = string.Empty;
            this.mtbDeductible.MaxLength = 10;
            this.mtbDeductible.Name = "mtbDeductible";
            this.mtbDeductible.Size = new System.Drawing.Size(60, 20);
            this.mtbDeductible.TabIndex = 7;
            this.mtbDeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductible.ValidationExpression = "^([0-9]+(,)?)+(\\.[0-9]{0,2})?$";
            this.mtbDeductible.Validating += new System.ComponentModel.CancelEventHandler(this.mtbDeductible_Validating);
            this.mtbDeductible.Enter += new System.EventHandler(this.mtbDeductible_Enter);
            // 
            // lblStaticDeductible
            // 
            this.lblStaticDeductible.Location = new System.Drawing.Point(8, 168);
            this.lblStaticDeductible.Name = "lblStaticDeductible";
            this.lblStaticDeductible.Size = new System.Drawing.Size(78, 23);
            this.lblStaticDeductible.TabIndex = 0;
            this.lblStaticDeductible.Text = "Deductible:  $";
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = string.Empty;
            this.lineLabel.Location = new System.Drawing.Point(8, 138);
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size(831, 18);
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // dtpTerminationDate
            // 
            this.dtpTerminationDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpTerminationDate.Checked = false;
            this.dtpTerminationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTerminationDate.Location = new System.Drawing.Point(616, 48);
            this.dtpTerminationDate.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dtpTerminationDate.Name = "dtpTerminationDate";
            this.dtpTerminationDate.Size = new System.Drawing.Size(21, 20);
            this.dtpTerminationDate.TabIndex = 0;
            this.dtpTerminationDate.CloseUp += new System.EventHandler(this.dtpTerminationDate_CloseUp);
            // 
            // mtbTerminationDate
            // 
            this.mtbTerminationDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTerminationDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbTerminationDate.Location = new System.Drawing.Point(552, 48);
            this.mtbTerminationDate.Mask = "  /  /";
            this.mtbTerminationDate.MaxLength = 10;
            this.mtbTerminationDate.Name = "mtbTerminationDate";
            this.mtbTerminationDate.Size = new System.Drawing.Size(70, 20);
            this.mtbTerminationDate.TabIndex = 6;
            this.mtbTerminationDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbTerminationDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTerminationDate_Validating);
            this.mtbTerminationDate.Enter += new System.EventHandler(this.mskTerminationDate_Enter);
            // 
            // lblStaticTermDate
            // 
            this.lblStaticTermDate.Location = new System.Drawing.Point(408, 51);
            this.lblStaticTermDate.Name = "lblStaticTermDate";
            this.lblStaticTermDate.Size = new System.Drawing.Size(152, 23);
            this.lblStaticTermDate.TabIndex = 0;
            this.lblStaticTermDate.Text = "Termination date for Insured:";
            // 
            // dtpEffectiveDate
            // 
            this.dtpEffectiveDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpEffectiveDate.Checked = false;
            this.dtpEffectiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEffectiveDate.Location = new System.Drawing.Point(616, 14);
            this.dtpEffectiveDate.MinDate = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            this.dtpEffectiveDate.Name = "dtpEffectiveDate";
            this.dtpEffectiveDate.Size = new System.Drawing.Size(21, 20);
            this.dtpEffectiveDate.TabIndex = 0;
            this.dtpEffectiveDate.CloseUp += new System.EventHandler(this.dtpEffectiveDate_CloseUp);
            // 
            // mtbEffectiveDate
            // 
            this.mtbEffectiveDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEffectiveDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbEffectiveDate.Location = new System.Drawing.Point(552, 14);
            this.mtbEffectiveDate.Mask = "  /  /";
            this.mtbEffectiveDate.MaxLength = 10;
            this.mtbEffectiveDate.Name = "mtbEffectiveDate";
            this.mtbEffectiveDate.Size = new System.Drawing.Size(70, 20);
            this.mtbEffectiveDate.TabIndex = 5;
            this.mtbEffectiveDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbEffectiveDate.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEffectiveDate_Validating);
            this.mtbEffectiveDate.Enter += new System.EventHandler(this.mtbEffectiveDate_Enter);
            // 
            // lblStaticEffectiveDate
            // 
            this.lblStaticEffectiveDate.Location = new System.Drawing.Point(408, 17);
            this.lblStaticEffectiveDate.Name = "lblStaticEffectiveDate";
            this.lblStaticEffectiveDate.Size = new System.Drawing.Size(136, 23);
            this.lblStaticEffectiveDate.TabIndex = 0;
            this.lblStaticEffectiveDate.Text = "Effective date for Insured:";
            // 
            // mtbTypeOfCoverage
            // 
            this.mtbTypeOfCoverage.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbTypeOfCoverage.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTypeOfCoverage.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbTypeOfCoverage.Location = new System.Drawing.Point(160, 116);
            this.mtbTypeOfCoverage.Mask = string.Empty;
            this.mtbTypeOfCoverage.MaxLength = 25;
            this.mtbTypeOfCoverage.Name = "mtbTypeOfCoverage";
            this.mtbTypeOfCoverage.Size = new System.Drawing.Size(185, 20);
            this.mtbTypeOfCoverage.TabIndex = 4;
            this.mtbTypeOfCoverage.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbTypeOfCoverage.Validating += new System.ComponentModel.CancelEventHandler(this.mtbTypeOfCoverage_Validating);
            // 
            // lblStaticTypeOfCoverage
            // 
            this.lblStaticTypeOfCoverage.Location = new System.Drawing.Point(8, 119);
            this.lblStaticTypeOfCoverage.Name = "lblStaticTypeOfCoverage";
            this.lblStaticTypeOfCoverage.TabIndex = 0;
            this.lblStaticTypeOfCoverage.Text = "Type of coverage:";
            // 
            // mtbInsuranceRepName
            // 
            this.mtbInsuranceRepName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbInsuranceRepName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbInsuranceRepName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsuranceRepName.Location = new System.Drawing.Point(160, 82);
            this.mtbInsuranceRepName.Mask = string.Empty;
            this.mtbInsuranceRepName.MaxLength = 25;
            this.mtbInsuranceRepName.Name = "mtbInsuranceRepName";
            this.mtbInsuranceRepName.Size = new System.Drawing.Size(185, 20);
            this.mtbInsuranceRepName.TabIndex = 3;
            this.mtbInsuranceRepName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsuranceRepName.Validating += new System.ComponentModel.CancelEventHandler(this.mtbInsuranceRepName_Validating);
            // 
            // lblStaticInsuranceRep
            // 
            this.lblStaticInsuranceRep.Location = new System.Drawing.Point(8, 85);
            this.lblStaticInsuranceRep.Name = "lblStaticInsuranceRep";
            this.lblStaticInsuranceRep.Size = new System.Drawing.Size(160, 23);
            this.lblStaticInsuranceRep.TabIndex = 0;
            this.lblStaticInsuranceRep.Text = "Insurance company rep name:";
            // 
            // mtbEligibilityPhone
            // 
            this.mtbEligibilityPhone.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEligibilityPhone.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEligibilityPhone.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEligibilityPhone.Location = new System.Drawing.Point(160, 48);
            this.mtbEligibilityPhone.Mask = string.Empty;
            this.mtbEligibilityPhone.MaxLength = 15;
            this.mtbEligibilityPhone.Name = "mtbEligibilityPhone";
            this.mtbEligibilityPhone.TabIndex = 2;
            this.mtbEligibilityPhone.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEligibilityPhone.Validating += new System.ComponentModel.CancelEventHandler(this.mtbEligibilityPhone_Validating);
            // 
            // lblStaticEligibilityPhone
            // 
            this.lblStaticEligibilityPhone.Location = new System.Drawing.Point(8, 51);
            this.lblStaticEligibilityPhone.Name = "lblStaticEligibilityPhone";
            this.lblStaticEligibilityPhone.TabIndex = 0;
            this.lblStaticEligibilityPhone.Text = "Eligibility phone:";
            // 
            // cmbInfoRecvFrom
            // 
            this.cmbInfoRecvFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point(160, 14);
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size(185, 21);
            this.cmbInfoRecvFrom.TabIndex = 1;
            this.cmbInfoRecvFrom.DropDown += new System.EventHandler(this.cmbInfoRecvFrom_DropDown);
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler(this.cmbInfoRecvFrom_SelectedIndexChanged);
            // 
            // lblStaticInfoRecvFrom
            // 
            this.lblStaticInfoRecvFrom.Location = new System.Drawing.Point(8, 17);
            this.lblStaticInfoRecvFrom.Name = "lblStaticInfoRecvFrom";
            this.lblStaticInfoRecvFrom.Size = new System.Drawing.Size(144, 23);
            this.lblStaticInfoRecvFrom.TabIndex = 0;
            this.lblStaticInfoRecvFrom.Text = "Information received from:";
            // 
            // GovernmentVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel);
            this.Name = "GovernmentVerifyView";
            this.Size = new System.Drawing.Size(847, 300);
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public GovernmentVerifyView()
        {
            InitializeComponent();

            ConfigureControls();

            loadingModelData = true;
            benefitsCategoryDetails = new BenefitsCategoryDetails();
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

        private ComboBox               cmbInfoRecvFrom;
        private ComboBox               cmbDeductibleMet;
        private ComboBox               cmbOutOfPocketMet;

        private DateTimePicker         dtpEffectiveDate;
        private DateTimePicker         dtpTerminationDate;
        
        private Label                  lblStaticInfoRecvFrom;
        private Label                  lblStaticEligibilityPhone;
        private Label                  lblStaticInsuranceRep;
        private Label                  lblStaticTypeOfCoverage;
        private Label                  lblStaticEffectiveDate;
        private Label                  lblStaticTermDate;
        private Label                  lblStaticDeductible;
        private Label                  lblStaticDeductibleMet;
        private Label                  lblStaticDollarsMet;
        private Label                  lblStaticCoInsurance;
        private Label                  lblStaticOutOfPocket;
        private Label                  lblStaticOutOfPocketMet;
        private Label                  lblStaticOutOfPocketDollarsMet;
        private Label                  lblStaticOutOfPocketPercent;
        private Label                  lblPercent1;
        private Label                  lblPercent2;
        private Label                  lblStaticCoPay;
        private Label                  lblRemarks;

        private Panel                  panel;
        
        private MaskedEditTextBox    mtbRemarks;

        private LineLabel   lineLabel;

        private MaskedEditTextBox    mtbEligibilityPhone;
        private MaskedEditTextBox    mtbOutOfPocketAmount;
        private MaskedEditTextBox    mtbOutOfPocketDollarsMet;
        private MaskedEditTextBox    mtbPercentAfterOutOfPocket;
        private MaskedEditTextBox    mtbCoPayAmount;
        private MaskedEditTextBox    mtbInsuranceRepName;
        private MaskedEditTextBox    mtbTypeOfCoverage;
        private MaskedEditTextBox    mtbEffectiveDate;
        private MaskedEditTextBox    mtbDeductible;
        private MaskedEditTextBox    mtbDeductibleDollarsMet;
        private MaskedEditTextBox    mtbCoInsurancePercent;
        private MaskedEditTextBox    mtbTerminationDate;

        private bool                                        loadingModelData;
        private decimal                                     deductible;
        private decimal                                     coPayAmount;
        private double                                      outOfPocket;
        private double                                      outOfPocketDollarsMet;
        private double                                      deductibleDollarsMet;
        private int                                         insuranceMonth;
        private int                                         insuranceDay;
        private int                                         insuranceYear;
        private int                                         coInsurance;
        private int                                         afterOutOfPocketPercent;
        private string                                      insuranceRepName;
        private string                                      eligibilityPhone;
        private string                                      remarks;
        private string                                      typeOfCoverage;
        private Account                                     i_Account;
       // private BenefitsCategory                            benefitsCategory;
        private BenefitsCategoryDetails                     benefitsCategoryDetails;
        private DateTime                                    effectiveInsuredDate;
        private DateTime                                    terminationInsuredDate;
        private InformationReceivedSource                   informationReceivedSource;
        private YesNoFlag                                   deductibleMet;
        private YesNoFlag                                   outOfPocketMet;
        private YesNoFlag                                   blankYesNoFlag;
        private YesNoFlag                                   noYesNoFlag;
        private YesNoFlag                                   yesYesNoFlag;
        private RuleEngine                                  i_RuleEngine;
        #endregion

        #region Constants
        #endregion

   }
}
