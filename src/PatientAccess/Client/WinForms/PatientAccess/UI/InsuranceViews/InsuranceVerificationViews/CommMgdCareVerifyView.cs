using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.AddressViews;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;
using log4net;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for CommMgdCareVerifyView.
    /// </summary>
    public class CommMgdCareVerifyView : ControlView
    {
        #region Event Handlers

        private void mtbRemainingLifetimeValue_Enter( object sender, EventArgs e )
        {
            if( this.mtbRemainingLifetimeValue.UnMaskedText != string.Empty )
            {
                decimal remainingLifetimeValue = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbRemainingLifetimeValue );
                this.mtbRemainingLifetimeValue.UnMaskedText = remainingLifetimeValue.ToString();
            }
        }

        private void mtbRemainingBenefitPerVisit_Enter( object sender, EventArgs e )
        {
            if( this.mtbRemainingBenefitPerVisit.UnMaskedText != string.Empty )
            {
                decimal remainingBenefitsPerVisit = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbRemainingBenefitPerVisit );
                this.mtbRemainingBenefitPerVisit.UnMaskedText = remainingBenefitsPerVisit.ToString();
            }
        }

        private void mtbDeductible_Enter( object sender, EventArgs e )
        {
            if( this.mtbDeductible.UnMaskedText != string.Empty )
            {
                decimal deductible = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbDeductible );
                this.mtbDeductible.UnMaskedText = deductible.ToString();
            }
        }

        private void mtbCoPayAmount_Enter( object sender, EventArgs e )
        {
            if( this.mtbCoPayAmount.UnMaskedText != string.Empty )
            {
                decimal coPayAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbCoPayAmount );
                this.mtbCoPayAmount.UnMaskedText = coPayAmount.ToString();
            }
        }

        private void mtbDeductibleDollarsMet_Enter( object sender, EventArgs e )
        {
            if( this.mtbDeductibleDollarsMet.UnMaskedText != string.Empty )
            {
                decimal deductibleDollarsMet = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbDeductibleDollarsMet );
                this.mtbDeductibleDollarsMet.UnMaskedText = deductibleDollarsMet.ToString();
            }
        }

        private void mtbOutOfPocketAmount_Enter( object sender, EventArgs e )
        {
            if( this.mtbOutOfPocketAmount.UnMaskedText != string.Empty )
            {
                decimal outOfPocketAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbOutOfPocketAmount );
                this.mtbOutOfPocketAmount.UnMaskedText = outOfPocketAmount.ToString();
            }
        }

        private void mtbMaxBenefitAmount_Enter( object sender, EventArgs e )
        {
            if( this.mtbMaxBenefitAmount.UnMaskedText != string.Empty )
            {
                decimal maxBenefitAmount = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbMaxBenefitAmount );
                this.mtbMaxBenefitAmount.UnMaskedText = maxBenefitAmount.ToString();
            }
        }

        private void mtbMaxBenefitPerVisit_Enter( object sender, EventArgs e )
        {
            if( this.mtbMaxBenefitPerVisit.UnMaskedText != string.Empty )
            {
                decimal maxBenefitPerVisit = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbMaxBenefitPerVisit );
                this.mtbMaxBenefitPerVisit.UnMaskedText = maxBenefitPerVisit.ToString();
            }
        }

        private void mtbOutOfPocketDollarsMet_Enter( object sender, EventArgs e )
        {
            if( this.mtbOutOfPocketDollarsMet.UnMaskedText != string.Empty )
            {
                decimal outOfPocketDollarsMet = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbOutOfPocketDollarsMet );
                this.mtbOutOfPocketDollarsMet.UnMaskedText = outOfPocketDollarsMet.ToString();
            }
        }

        private void CommMgdCareVerifyView_Disposed( object sender, EventArgs e )
        {
            UnRegisterEvents();
        }


        private void InsuranceInformationRecvFromPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbInfoRecvFrom );
        }


        private void InsuranceEffectiveDatePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbEffectiveDate );
        }


        private void InsuranceClaimsVerifiedPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbAddressVerified );
        }

        private void btnClearAll_Click( object sender, EventArgs e )
        {
            ResetView();
        }

        private void cmbAddressVerified_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbAddressVerified );

            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                Model_Coverage.ClaimsAddressVerified = cb.SelectedItem as YesNoFlag ?? new YesNoFlag();
                CheckForRequiredFields();
            }
        }

        private void cmbInfoRecvFrom_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetPreferredBgColor( cb );
        }

        private void cmbInfoRecvFrom_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbInfoRecvFrom );
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                Model_Coverage.InformationReceivedSource = cb.SelectedItem as InformationReceivedSource ?? new InformationReceivedSource();
            }

            CheckForRequiredFields();
        }

        private void dtpEffectiveDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbEffectiveDate );
            DateTime dt = dtp.Value;
            mtbEffectiveDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbEffectiveDate.Focus();
        }

        private void mtbEffectiveDate_Enter( object sender, EventArgs e )
        {
            Refresh();
        }

        private void mtbEffectiveDate_Validating( object sender, CancelEventArgs e )
        {
            UIColors.SetNormalBgColor( this.mtbEffectiveDate );

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( dtpEffectiveDate.Focused )
            {
                return;
            }
            if( mtb.UnMaskedText.Trim() != String.Empty )
            {
                if( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
                {
                    UIColors.SetNormalBgColor( mtb );
                    Refresh();
                    Model_Coverage.EffectiveDateForInsured = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                Model_Coverage.EffectiveDateForInsured = DateTime.MinValue;
            }

            if( dtpEffectiveDate.Focused == false )
            {   // Run the rules only if the DateTimePicker was not cicked
                CheckForRequiredFields();
            }
        }

        private void dtpTerminationDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbTerminationDate );
            DateTime dt = dtp.Value;
            mtbTerminationDate.Text = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbTerminationDate.Focus();
        }

        private void mtbTerminationDate_Enter( object sender, EventArgs e )
        {
            Refresh();
        }

        private void mtbTerminationDate_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( dtpTerminationDate.Focused )
            {
                return;
            }
            if( mtb.UnMaskedText != String.Empty )
            {
                if( InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ) )
                {
                    Model_Coverage.TerminationDateForInsured = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                    UIColors.SetNormalBgColor( mtb );
                }
            }
        }

        private void mskEligibilityPhone_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mtbEligibilityPhone_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                Model_Coverage.EligibilityPhone = mtb.UnMaskedText;
            }
        }

        private void mtbDeductible_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                if( Convert.ToDecimal( mtb.UnMaskedText ) != deductible )
                {
                    deductible = Convert.ToDecimal( mtb.UnMaskedText );

                    BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                    bcd.Deductible = (float)deductible;
                }

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbRemainingLifetimeValue_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                if( Convert.ToDouble( mtb.UnMaskedText ) != remainingLifetimeValue )
                {
                    remainingLifetimeValue = Convert.ToDouble( mtb.UnMaskedText );

                    BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                    bcd.RemainingLifetimeValue = remainingLifetimeValue;
                }

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbRemainingBenefitPerVisit_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                if( Convert.ToDouble( mtb.UnMaskedText ) != remainingBenefitsPerVisit )
                {
                    remainingBenefitsPerVisit = Convert.ToDouble( mtb.UnMaskedText );

                    BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                    bcd.RemainingBenefitPerVisits = remainingBenefitsPerVisit;
                }

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbInsRepName_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                Model_Coverage.InsuranceCompanyRepName = mtb.Text;
            }
        }

        private void cmbTypeOfProduct_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 && cb.Text.Equals( String.Empty ) == false )
            {
                TypeOfProduct typeOfProduct = cb.SelectedItem as TypeOfProduct;
                Model_Coverage.TypeOfProduct = (TypeOfProduct)typeOfProduct.Clone();
            }
        }

        private void cmbRule_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 && cb.Text.Equals( String.Empty ) == false )
            {
                TypeOfVerificationRule typeOfVerificationRule = cb.SelectedItem as TypeOfVerificationRule;
                Model_Coverage.TypeOfVerificationRule = (TypeOfVerificationRule)typeOfVerificationRule.Clone();
            }
        }

        private void cmbTimePeriod_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                TimePeriodFlag timePeriod = cb.SelectedItem as TimePeriodFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.TimePeriod = timePeriod;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.TimePeriod = timePeriod;
            }
        }

        private void cmbMaxBenefitPerVisitMet_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag remMaxBenefitPerVisitMet = cb.SelectedItem as YesNoFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.RemainingBenefitPerVisitsMet = remMaxBenefitPerVisitMet;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.RemainingBenefitPerVisitsMet = remMaxBenefitPerVisitMet;
            }
        }

        private void cmbDeductibleMet_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag deductibleMet = cb.SelectedItem as YesNoFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.DeductibleMet = deductibleMet;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.DeductibleMet = deductibleMet;
            }
        }

        private void cmbOutOfPocketMet_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag outOfPocketMet = cb.SelectedItem as YesNoFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.OutOfPocketMet = outOfPocketMet;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.OutOfPocketMet = outOfPocketMet;
            }
        }

        private void cmbCoPayWaiveIfAdmitted_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag waiveIfAdmitted = cb.SelectedItem as YesNoFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.WaiveCopayIfAdmitted = waiveIfAdmitted;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.WaiveCopayIfAdmitted = waiveIfAdmitted;
            }
        }

        private void cmbLifetimeMaxBenefitMet_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag remLifeTimeMaxBenefitMet = cb.SelectedItem as YesNoFlag;

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.RemainingLifetimeValueMet = remLifeTimeMaxBenefitMet;

                BenefitsCategoryData bc = (BenefitsCategoryData)categoryTable[currentBenefitsIndex];
                bc.bcd.RemainingLifetimeValueMet = remLifeTimeMaxBenefitMet;
            }
        }

        private void cmbPreexistingCondition_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag serviceForPreExistingCondition = cb.SelectedItem as YesNoFlag;
                Model_Coverage.ServiceForPreExistingCondition = (YesNoFlag)serviceForPreExistingCondition.Clone();
            }
        }

        private void cmbCoveredBenefit_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag serviceIsCoveredBenefit = cb.SelectedItem as YesNoFlag;
                Model_Coverage.ServiceIsCoveredBenefit = (YesNoFlag)serviceIsCoveredBenefit.Clone();
            }
        }

        private void cmbCoordinationOfBenefits_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag coordinationOfBenefits = cb.SelectedItem as YesNoFlag;
                Model_Coverage.CoordinationOfbenefits = (YesNoFlag)coordinationOfBenefits.Clone();

                if( coordinationOfBenefits.Code == "Y" )
                {
                    cmbRule.Enabled = true;
                    PopulateRuleComboBox();
                }
                else
                {
                    cmbRule.Items.Clear();
                    cmbRule.Enabled = false;
                }
            }
        }

        private void mtbPPOProvider_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                string PPOPricingOrBroker = mtb.UnMaskedText;
                Model_Coverage.PPOPricingOrBroker = (String)PPOPricingOrBroker.Clone();
            }
        }

        private void mtbAutoInsuranceClaimNumber_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                string autoInsuranceClaimNumber = mtb.UnMaskedText;
                Model_Coverage.AutoInsuranceClaimNumber = (String)autoInsuranceClaimNumber.Clone();
            }
        }

        private void cmbFacilityIsProvider_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag facilityContractedProvider = cb.SelectedItem as YesNoFlag;
                Model_Coverage.FacilityContractedProvider = (YesNoFlag)facilityContractedProvider.Clone();
            }
        }

        private void cmbAutoMedpayCoverage_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            if( cb.SelectedIndex != -1 )
            {
                YesNoFlag autoMedpayCoverage = cb.SelectedItem as YesNoFlag;
                Model_Coverage.AutoMedPayCoverage = (YesNoFlag)autoMedpayCoverage.Clone();
            }
        }

        private void mtbRemarks_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                string remarks = mtb.Text.TrimEnd();
                Model_Coverage.Remarks = (String)remarks.Clone();
            }
        }

        private void mtbDeductibleDollarsMet_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double deductableDollarsMet = Convert.ToDouble( mtb.UnMaskedText );
                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.DeductibleDollarsMet = (float)deductableDollarsMet;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }

        }

        private void mtbCoInsuranceAmount_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                int coInsurance = Convert.ToInt32( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.CoInsurance = coInsurance;
            }
        }

        private void mtbOutOfPocketAmount_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double outOfPocket = Convert.ToDouble( mtb.UnMaskedText );
                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.OutOfPocket = (float)outOfPocket;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbOutOfPocketDollarsMet_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double outOfPocketDollarsMet = Convert.ToDouble( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.OutOfPocketDollarsMet = (float)outOfPocketDollarsMet;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbPercentOutOfPocket_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                int afterOutOfPocketPercent = Convert.ToInt32( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.AfterOutOfPocketPercent = afterOutOfPocketPercent;
            }
        }

        private void mtbCoPayAmount_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double coPayAmount = Convert.ToDouble( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.CoPay = (float)coPayAmount;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbNumberVisitsPerYear_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText != String.Empty )
            {
                int visitsPerYear = Convert.ToInt32( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.VisitsPerYear = visitsPerYear;
            }
        }

        private void mtbMaxBenefitAmount_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double lifeTimeMaxBenefit = Convert.ToDouble( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );
                bcd.LifeTimeMaxBenefit = lifeTimeMaxBenefit;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void mtbMaxBenefitPerVisit_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if( mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != "." )
            {
                double maxBenefitPerVisit = Convert.ToDouble( mtb.UnMaskedText );

                BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );

                bcd.MaxBenefitPerVisit = maxBenefitPerVisit;

                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
        }

        private void UpdateBenefitsCategoryControls()
        {
            BenefitsCategoryDetails bcd = Model_Coverage.BenefitsCategoryDetailsFor( benefitsCategory );

            if( bcd != null )
            {
                cmbTimePeriod.SelectedItem = bcd.TimePeriod;
                cmbDeductibleMet.SelectedItem = bcd.DeductibleMet;
                cmbOutOfPocketMet.SelectedItem = bcd.OutOfPocketMet;
                cmbCoPayWaiveIfAdmitted.SelectedItem = bcd.WaiveCopayIfAdmitted;
                cmbLifetimeMaxBenefitMet.SelectedItem = bcd.RemainingLifetimeValueMet;
                cmbMaxBenefitPerVisitMet.SelectedItem = bcd.RemainingBenefitPerVisitsMet;

                if( bcd.Deductible != -1 )
                {
                    mtbDeductible.Text = bcd.Deductible.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbDeductible.UnMaskedText = string.Empty;
                }

                if( bcd.CoPay != -1 )
                {
                    mtbCoPayAmount.Text = bcd.CoPay.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbCoPayAmount.UnMaskedText = string.Empty;
                }

                if( bcd.CoInsurance != -1 )
                {
                    mtbCoInsuranceAmount.Text = bcd.CoInsurance.ToString();
                }
                else
                {
                    mtbCoInsuranceAmount.UnMaskedText = string.Empty;
                }

                if( bcd.AfterOutOfPocketPercent != -1 )
                {
                    mtbPercentOutOfPocket.Text = bcd.AfterOutOfPocketPercent.ToString();
                }
                else
                {
                    mtbPercentOutOfPocket.UnMaskedText = string.Empty;
                }

                if( bcd.DeductibleDollarsMet != -1 )
                {
                    mtbDeductibleDollarsMet.Text = bcd.DeductibleDollarsMet.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbDeductibleDollarsMet.UnMaskedText = string.Empty;
                }

                if( bcd.OutOfPocket != -1 )
                {
                    mtbOutOfPocketAmount.Text = bcd.OutOfPocket.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbOutOfPocketAmount.UnMaskedText = string.Empty;
                }

                if( bcd.LifeTimeMaxBenefit != -1 )
                {
                    mtbMaxBenefitAmount.Text = bcd.LifeTimeMaxBenefit.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbMaxBenefitAmount.UnMaskedText = string.Empty;
                }

                if( bcd.RemainingLifetimeValue != -1 )
                {
                    this.mtbRemainingLifetimeValue.Text = bcd.RemainingLifetimeValue.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbRemainingLifetimeValue.UnMaskedText = string.Empty;
                }

                if( bcd.MaxBenefitPerVisit != -1 )
                {
                    mtbMaxBenefitPerVisit.Text = bcd.MaxBenefitPerVisit.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbMaxBenefitPerVisit.UnMaskedText = string.Empty;
                }

                if( bcd.RemainingBenefitPerVisits != -1 )
                {
                    this.mtbRemainingBenefitPerVisit.Text = bcd.RemainingBenefitPerVisits.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbRemainingBenefitPerVisit.UnMaskedText = string.Empty;
                }

                if( bcd.OutOfPocketDollarsMet != -1 )
                {
                    mtbOutOfPocketDollarsMet.Text = bcd.OutOfPocketDollarsMet.ToString( "###,###,##0.00" );
                }
                else
                {
                    mtbOutOfPocketDollarsMet.UnMaskedText = string.Empty;
                }

                if( mtbNumberVisitsPerYear.Enabled
                    && bcd.VisitsPerYear != -1 )
                {
                    mtbNumberVisitsPerYear.Text = bcd.VisitsPerYear.ToString();
                }
                else
                {
                    mtbNumberVisitsPerYear.UnMaskedText = string.Empty;
                }
            }
        }

        private void lvBenefitsCategories_SelectedIndexChanged( object sender, EventArgs e )
        {
            ListView lv = sender as ListView;
            ListView.SelectedListViewItemCollection collection = lv.SelectedItems;

            if( collection.Count > 0 )
            {
                ListViewItem item = collection[0];
                benefitsCategory = item.Tag as BenefitsCategory;

                if( benefitsCategory != null )
                {   // Set the HashTable index key to store the control data related to the Benefits Category
                    currentBenefitsIndex = benefitsCategory.Description.ToUpper();
                    EnableControls( true );
                    UpdateBenefitsCategoryControls();
                }
            }
            else
            {
                EnableControls( false );
            }
        }
        #endregion

        #region Methods 

        public void InvokeBenefitsCategoriesSelectionChangedEvent()
        {
            this.lvBenefitsCategories_SelectedIndexChanged( this.lvBenefitsCategories, null );
        }

        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;

                PopulateInfoRecvComboBox();
                PopulateDeductibleTimePeriodComboBox();
                PopulateDeductibleMetComboBox();
                PopulateOutOfPocketMetComboBox();
                PopulateWaiveIfAdmittedComboBox();
                PopulateMaxBenefitMetComboBox();
                PopulateRuleComboBox();
                PopulateTypeOfProductComboBox();
                PopulateBenefitsCategoriesListBox();
                PopulateMaxBenefitPerVisitComboBox();
                PopulatePreexistingConditionComboBox();
                PopulateCoveredBenefitComboBox();
                PopulateAddressVerifiedComboBox();
                PopulateCoordinationOfBenefitsComboBox();
                PopulateAutoMedpayCoverageComboBox();
                PopulateFacilityIsContractedProviderComboBox();
            }

            PopulateAddressViews();

            this.cmbInfoRecvFrom.SelectedItem = this.Model_Coverage.InformationReceivedSource;
            this.mtbEligibilityPhone.UnMaskedText 
                = (this.Model_Coverage.EligibilityPhone==null)?string.Empty:this.Model_Coverage.EligibilityPhone.TrimEnd();
            this.mtbInsRepName.UnMaskedText 
                = (this.Model_Coverage.InsuranceCompanyRepName==null)?string.Empty:this.Model_Coverage.InsuranceCompanyRepName.TrimEnd();

            if( this.Model_Coverage.EffectiveDateForInsured == DateTime.MinValue )
            {
                this.mtbEffectiveDate.UnMaskedText = string.Empty;
            }
            else
            {
                this.mtbEffectiveDate.UnMaskedText = this.Model_Coverage.EffectiveDateForInsured.ToString( "MMddyyyy" );
            }

            if( this.Model_Coverage.TerminationDateForInsured == DateTime.MinValue )
            {
                this.mtbTerminationDate.UnMaskedText = string.Empty;
            }
            else
            {
                this.mtbTerminationDate.UnMaskedText = this.Model_Coverage.TerminationDateForInsured.ToString( "MMddyyyy" );
            }

            this.cmbPreexistingCondition.SelectedItem = this.Model_Coverage.ServiceForPreExistingCondition;
            this.cmbCoveredBenefit.SelectedItem = this.Model_Coverage.ServiceIsCoveredBenefit;
            this.cmbAddressVerified.SelectedItem = this.Model_Coverage.ClaimsAddressVerified;
            this.cmbCoordinationOfBenefits.SelectedItem = this.Model_Coverage.CoordinationOfbenefits;
            this.cmbTypeOfProduct.SelectedItem = this.Model_Coverage.TypeOfProduct;
            this.mtbPPOProvider.UnMaskedText = 
                (this.Model_Coverage.PPOPricingOrBroker==null)?string.Empty:this.Model_Coverage.PPOPricingOrBroker.TrimEnd();
            this.cmbFacilityIsProvider.SelectedItem = this.Model_Coverage.FacilityContractedProvider;
            this.mtbAutoInsuranceClaimNumber.UnMaskedText = 
                (this.Model_Coverage.AutoInsuranceClaimNumber==null)?string.Empty:this.Model_Coverage.AutoInsuranceClaimNumber.TrimEnd();
            this.cmbAutoMedpayCoverage.SelectedItem = this.Model_Coverage.AutoMedPayCoverage;

            if( this.Model_Coverage.TypeOfVerificationRule != null )
            {
                this.cmbRule.SelectedItem = this.Model_Coverage.TypeOfVerificationRule;
            }
            else
            {
                this.cmbRule.SelectedItem = new TypeOfVerificationRule();
            }

            this.mtbRemarks.UnMaskedText = (this.Model_Coverage.Remarks==null)?string.Empty:this.Model_Coverage.Remarks.TrimEnd();

            if( lvBenefitsCategories.Items.Count > 0 )
            {
                lvBenefitsCategories.Items[0].Selected = true;
            }

            RuleEngine.GetInstance().RegisterEvent( typeof( InsuranceInformationRecvFromPreferred ), Model_Coverage, new EventHandler( InsuranceInformationRecvFromPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InsuranceEffectiveDatePreferred ), Model_Coverage, new EventHandler( InsuranceEffectiveDatePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InsuranceClaimsVerifiedPreferred ), Model_Coverage, new EventHandler( InsuranceClaimsVerifiedPreferredEventHandler ) );

            CheckForRequiredFields();

            bool result = true;
            int year = 0;
            int month = 0;
            int day = 0;
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
            UpdateAttorneyAndInsuranceAgent();
            if( (InformationReceivedSource)this.cmbInfoRecvFrom.SelectedItem != null )
            {
                this.Model_Coverage.InformationReceivedSource = (InformationReceivedSource)this.cmbInfoRecvFrom.SelectedItem;
            }

            this.mtbEligibilityPhone_Validating( this.mtbEligibilityPhone, null );
            this.Model_Coverage.EligibilityPhone = this.mtbEligibilityPhone.UnMaskedText;

            this.mtbInsRepName_Validating( this.mtbInsRepName, null );
            this.Model_Coverage.InsuranceCompanyRepName = this.mtbInsRepName.UnMaskedText;

            if( InsuranceDateVerify.IsValidDateTime( this.mtbEffectiveDate ) )
            {
                this.Model_Coverage.EffectiveDateForInsured = DateTime.Parse( this.mtbEffectiveDate.Text );
            }
            else
            {
                this.Model_Coverage.EffectiveDateForInsured = DateTime.MinValue;
            }

            if( InsuranceDateVerify.IsValidDateTime( this.mtbTerminationDate ) )
            {
                this.Model_Coverage.TerminationDateForInsured = DateTime.Parse( this.mtbTerminationDate.Text );
            }
            else
            {
                this.Model_Coverage.TerminationDateForInsured = DateTime.MinValue;
            }

            if(this.cmbPreexistingCondition.SelectedItem != null)
                this.Model_Coverage.ServiceForPreExistingCondition = (YesNoFlag)this.cmbPreexistingCondition.SelectedItem;
            if(this.cmbCoveredBenefit.SelectedItem != null)
                this.Model_Coverage.ServiceIsCoveredBenefit = (YesNoFlag)this.cmbCoveredBenefit.SelectedItem;
            if(this.cmbAddressVerified.SelectedItem != null)
                this.Model_Coverage.ClaimsAddressVerified = (YesNoFlag)this.cmbAddressVerified.SelectedItem;
            if(this.cmbCoordinationOfBenefits.SelectedItem != null)
                this.Model_Coverage.CoordinationOfbenefits = (YesNoFlag)this.cmbCoordinationOfBenefits.SelectedItem;

            if(this.cmbTypeOfProduct.SelectedItem != null)
                this.Model_Coverage.TypeOfProduct = (TypeOfProduct)this.cmbTypeOfProduct.SelectedItem;

            this.mtbPPOProvider_Validating( this.mtbPPOProvider, null );
            this.Model_Coverage.PPOPricingOrBroker = this.mtbPPOProvider.UnMaskedText;

            if(this.cmbFacilityIsProvider.SelectedItem != null)
                this.Model_Coverage.FacilityContractedProvider = (YesNoFlag)this.cmbFacilityIsProvider.SelectedItem;

            this.mtbAutoInsuranceClaimNumber_Validating( this.mtbAutoInsuranceClaimNumber, null );
            this.Model_Coverage.AutoInsuranceClaimNumber = this.mtbAutoInsuranceClaimNumber.UnMaskedText;

            if(this.cmbAutoMedpayCoverage.SelectedItem != null)
                this.Model_Coverage.AutoMedPayCoverage = (YesNoFlag)this.cmbAutoMedpayCoverage.SelectedItem;
            if(this.cmbRule.SelectedItem != null)
                this.Model_Coverage.TypeOfVerificationRule = (TypeOfVerificationRule)this.cmbRule.SelectedItem;

            this.mtbRemarks_Validating( this.mtbRemarks, null );
            this.Model_Coverage.Remarks = this.mtbRemarks.UnMaskedText;

        }
        #endregion

        #region Properties
        [Browsable( false )]
        public CommercialCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (CommercialCoverage)this.Model;
            }
        }

        [Browsable( false )]
        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }

        [Browsable( false )]
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
        /// The controls to the right of the Benefits Category ListBox must have their values saved while the form
        /// is showing so that if the user enters data in the controls and then changes to another benefits category
        /// </summary>
        private void AddObjectToCategoryTable( string category )
        {
            if( categoryTable.Contains( category ) == false )
            {
                BenefitsCategoryData benefitsCategoryData = new BenefitsCategoryData();
                categoryTable.Add( category, benefitsCategoryData );
            }
        }

        private void PopulateAddressViews()
        {
            if( this.Model_Coverage != null && Model_Coverage.Attorney != null )
            {
                attorneyAddressView.KindOfTargetParty = Model_Coverage.Attorney.GetType();
                attorneyAddressView.PatientAccount = Model_Coverage.Account;
                ContactPoint businessContactPoint = new ContactPoint();
                businessContactPoint.Address = Model_Coverage.Attorney.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() ).Address;
                attorneyAddressView.Model = businessContactPoint;
                attorneyAddressView.UpdateView();

                nameAndPhoneView1.Model = Model_Coverage.Attorney;
                nameAndPhoneView1.UpdateView();
            }

            if( this.Model_Coverage != null && Model_Coverage.InsuranceAgent != null )
            {
                insuranceAgentAddressView.KindOfTargetParty = Model_Coverage.InsuranceAgent.GetType();
                insuranceAgentAddressView.PatientAccount = Model_Coverage.Account;
                ContactPoint businessContactPoint = new ContactPoint();
                businessContactPoint.Address = Model_Coverage.InsuranceAgent.ContactPointWith(
                  TypeOfContactPoint.NewBusinessContactPointType() ).Address;
                insuranceAgentAddressView.Model = businessContactPoint;
                insuranceAgentAddressView.UpdateView();

                nameAndPhoneView2.Model = Model_Coverage.InsuranceAgent;
                nameAndPhoneView2.UpdateView();
            }
        }
        /// <summary>
        /// Add Attorney and Insurance agent to the coverage object
        /// </summary>
        private void UpdateAttorneyAndInsuranceAgent()
        {
            if( attorneyAddressView.Model_ContactPoint != null &&
                attorneyAddressView.Model_ContactPoint.Address != null )
            {
                ContactPoint businessContactPoint = Model_Coverage.Attorney.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() );
                businessContactPoint.Address = attorneyAddressView.Model_ContactPoint.Address;
            }

            if( nameAndPhoneView1.Model_Person != null )
            {
                Model_Coverage.Attorney.AttorneyName = (string)nameAndPhoneView1.Model_Person.Name.FirstName.Trim().Clone();
            }

            if( insuranceAgentAddressView.Model_ContactPoint != null &&
                insuranceAgentAddressView.Model_ContactPoint.Address != null )
            {
                ContactPoint businessContactPoint = Model_Coverage.InsuranceAgent.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() );
                businessContactPoint.Address = insuranceAgentAddressView.Model_ContactPoint.Address;
            }

            if( nameAndPhoneView2.Model_Person != null )
            {
                Model_Coverage.InsuranceAgent.AgentName = (string)nameAndPhoneView2.Model_Person.Name.FirstName.Trim().Clone();
            }
        }

        /// <summary>
        /// CheckForRequiredFields - determine if the user has entered all required fields
        /// Some are conditional based on other fields.  Returns true or false;
        /// </summary>
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceInformationRecvFromPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceEffectiveDatePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( InsuranceClaimsVerifiedPreferred ), Model_Coverage );
        }

        private void EnableControls( bool state )
        {
            mtbDeductible.Enabled = state;
            mtbDeductible.UnMaskedText = String.Empty;

            mtbDeductibleDollarsMet.Enabled = state;
            mtbDeductibleDollarsMet.UnMaskedText = String.Empty;

            mtbCoPayAmount.Enabled = state;
            mtbCoPayAmount.UnMaskedText = String.Empty;

            mtbCoInsuranceAmount.Enabled = state;
            mtbCoInsuranceAmount.UnMaskedText = String.Empty;

            mtbOutOfPocketAmount.Enabled = state;
            mtbOutOfPocketAmount.UnMaskedText = String.Empty;

            mtbOutOfPocketDollarsMet.Enabled = state;
            mtbOutOfPocketDollarsMet.UnMaskedText = String.Empty;

            mtbPercentOutOfPocket.Enabled = state;
            mtbPercentOutOfPocket.UnMaskedText = String.Empty;

            mtbMaxBenefitAmount.Enabled = state;
            mtbMaxBenefitAmount.UnMaskedText = String.Empty;

            mtbMaxBenefitPerVisit.Enabled = state;
            mtbMaxBenefitPerVisit.UnMaskedText = String.Empty;

            if( state && ( benefitsCategory.isOutPatient() ||
                benefitsCategory.isPSYCHOP() || benefitsCategory.isREHABOP() ) )
            {
                mtbNumberVisitsPerYear.Enabled = true;
            }
            else
            {
                mtbNumberVisitsPerYear.UnMaskedText = String.Empty;
                mtbNumberVisitsPerYear.Enabled = false;
            }
        }

        private void PopulateAutoMedpayCoverageComboBox()
        {
            cmbAutoMedpayCoverage.Items.Add( blankYesNoFlag );
            cmbAutoMedpayCoverage.Items.Add( yesYesNoFlag );
            cmbAutoMedpayCoverage.Items.Add( noYesNoFlag );
        }

        private void PopulateAddressVerifiedComboBox()
        {
            cmbAddressVerified.Items.Add( blankYesNoFlag );
            cmbAddressVerified.Items.Add( yesYesNoFlag );
            cmbAddressVerified.Items.Add( noYesNoFlag );
        }

        private void PopulateBenefitsCategoriesListBox()
        {
            if( Account == null
                || Account.HospitalService == null
                || Account.KindOfVisit == null )
            {
                return;
            }

            IBenefitsCategoryBroker broker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            ArrayList hsvList = (ArrayList)broker.BenefitsCategoriesFor( User.GetCurrent().Facility, Account.HospitalService.Code.Trim() );
            lvBenefitsCategories.Items.Clear();
            ArrayList boldFaceStrings = new ArrayList();

            if( ( Account.KindOfVisit.Code == VisitType.INPATIENT ||
                  Account.KindOfVisit.Code == VisitType.OUTPATIENT ) &&
                  hsvList.Count > 1 )
            {
                BenefitsCategory bc = new BenefitsCategory();

                if( Account.HospitalService.Code != HospitalService.HSV_OB_16 )
                {
                    for( int i = 0; i < hsvList.Count; i++ )
                    {
                        bc = (BenefitsCategory)hsvList[i];
                        if( bc.Description.ToUpper() != Account.KindOfVisit.Description.ToUpper() )
                        {
                            hsvList.RemoveAt( i );
                            break;
                        }
                    }
                }
            }

            foreach( BenefitsCategory o in hsvList )
            {
                ListViewItem lv = new ListViewItem();
                lv.Text = o.ToString();
                lv.Tag = o;
                lv.Font = new Font( lv.Font, lv.Font.Style | FontStyle.Bold );
                lvBenefitsCategories.Items.Add( lv );
                boldFaceStrings.Add( o.ToString() );
                AddObjectToCategoryTable( o.Description.ToUpper() );

            }

            ArrayList alist = (ArrayList)broker.AllBenefitsCategories( User.GetCurrent().Facility.Oid );

            foreach( BenefitsCategory o in alist )
            {
                bool found = false;
                foreach( string s in boldFaceStrings )
                {
                    if( o.ToString().Equals( s ) )
                    {
                        found = true;
                        break;
                    }
                }
                if( !found )
                {
                    ListView.ListViewItemCollection items = lvBenefitsCategories.Items;
                    ListViewItem lv = new ListViewItem();
                    lv.Text = o.ToString();
                    lv.Tag = o;
                    lvBenefitsCategories.Items.Add( lv );
                    AddObjectToCategoryTable( o.Description.ToUpper() );
                }
            }
        }

        private void PopulateCoordinationOfBenefitsComboBox()
        {
            cmbCoordinationOfBenefits.Items.Add( blankYesNoFlag );
            cmbCoordinationOfBenefits.Items.Add( yesYesNoFlag );
            cmbCoordinationOfBenefits.Items.Add( noYesNoFlag );
        }

        private void PopulateCoveredBenefitComboBox()
        {
            cmbCoveredBenefit.Items.Add( blankYesNoFlag );
            cmbCoveredBenefit.Items.Add( yesYesNoFlag );
            cmbCoveredBenefit.Items.Add( noYesNoFlag );
        }

        private void PopulateDeductibleMetComboBox()
        {
            cmbDeductibleMet.Items.Add( blankYesNoFlag );
            cmbDeductibleMet.Items.Add( yesYesNoFlag );
            cmbDeductibleMet.Items.Add( noYesNoFlag );
        }

        private void PopulateDeductibleTimePeriodComboBox()
        {
            cmbTimePeriod.Items.Add( blankTimePeriodFlag );
            cmbTimePeriod.Items.Add( yearTimePeriodFlag );
            cmbTimePeriod.Items.Add( visitTimePeriodFlag );
        }

        private void PopulateFacilityIsContractedProviderComboBox()
        {
            cmbFacilityIsProvider.Items.Add( blankYesNoFlag );
            cmbFacilityIsProvider.Items.Add( yesYesNoFlag );
            cmbFacilityIsProvider.Items.Add( noYesNoFlag );
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

        private void PopulateMaxBenefitMetComboBox()
        {
            cmbLifetimeMaxBenefitMet.Items.Add( blankYesNoFlag );
            cmbLifetimeMaxBenefitMet.Items.Add( yesYesNoFlag );
            cmbLifetimeMaxBenefitMet.Items.Add( noYesNoFlag );
        }

        private void PopulateMaxBenefitPerVisitComboBox()
        {
            cmbMaxBenefitPerVisitMet.Items.Add( blankYesNoFlag );
            cmbMaxBenefitPerVisitMet.Items.Add( yesYesNoFlag );
            cmbMaxBenefitPerVisitMet.Items.Add( noYesNoFlag );
        }

        private void PopulateOutOfPocketMetComboBox()
        {
            cmbOutOfPocketMet.Items.Add( blankYesNoFlag );
            cmbOutOfPocketMet.Items.Add( yesYesNoFlag );
            cmbOutOfPocketMet.Items.Add( noYesNoFlag );
        }

        private void PopulatePreexistingConditionComboBox()
        {
            cmbPreexistingCondition.Items.Add( blankYesNoFlag );
            cmbPreexistingCondition.Items.Add( yesYesNoFlag );
            cmbPreexistingCondition.Items.Add( noYesNoFlag );
        }

        private void PopulateRuleComboBox()
        {
            ITypeOfVerificationRuleBroker broker = BrokerFactory.BrokerOfType<ITypeOfVerificationRuleBroker>();
            ArrayList alist = (ArrayList)broker.AllTypeOfVerificationRules();

            cmbRule.Items.Clear();

            foreach( TypeOfVerificationRule o in alist )
            {
                cmbRule.Items.Add( o );
            }
        }

        private void PopulateTypeOfProductComboBox()
        {
            ITypeOfProductBroker broker = BrokerFactory.BrokerOfType<ITypeOfProductBroker>();
            ArrayList alist = (ArrayList)broker.AllTypeOfProducts();

            cmbTypeOfProduct.Items.Clear();

            foreach( TypeOfProduct o in alist )
            {
                cmbTypeOfProduct.Items.Add( o );
            }
        }

        private void PopulateWaiveIfAdmittedComboBox()
        {
            cmbCoPayWaiveIfAdmitted.Items.Add( blankYesNoFlag );
            cmbCoPayWaiveIfAdmitted.Items.Add( yesYesNoFlag );
            cmbCoPayWaiveIfAdmitted.Items.Add( noYesNoFlag );
        }

        private void ResetView()
        { 
            this.Model_Coverage.RemoveCoverageVerificationData(); 
            this.UpdateView();
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceInformationRecvFromPreferred ), Model_Coverage, InsuranceInformationRecvFromPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceEffectiveDatePreferred ), Model_Coverage, InsuranceEffectiveDatePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceClaimsVerifiedPreferred ), Model_Coverage, InsuranceClaimsVerifiedPreferredEventHandler );
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDollar1 = new System.Windows.Forms.Label();
            this.lblRemainingBenefitPerVisit = new System.Windows.Forms.Label();
            this.lblRemainingLifetimeValue = new System.Windows.Forms.Label();
            this.mtbRemainingBenefitPerVisit = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemainingLifetimeValue = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblVertLineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.mtbDeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDeductible = new System.Windows.Forms.Label();
            this.nameAndPhoneView2 = new PatientAccess.UI.CommonControls.NameAndPhoneView();
            this.nameAndPhoneView1 = new PatientAccess.UI.CommonControls.NameAndPhoneView();
            this.insuranceAgentAddressView = new PatientAccess.UI.AddressViews.AddressView();
            this.lineLabel4 = new PatientAccess.UI.CommonControls.LineLabel();
            this.attorneyAddressView = new PatientAccess.UI.AddressViews.AddressView();
            this.lineLabel5 = new PatientAccess.UI.CommonControls.LineLabel();
            this.btnClearAll = new PatientAccess.UI.CommonControls.LoggingButton();
            this.mtbPPOProvider = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbInsRepName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDeductibleDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbMaxBenefitPerVisitMet = new System.Windows.Forms.ComboBox();
            this.lblStaticMaxBenefitVisitMet = new System.Windows.Forms.Label();
            this.mtbMaxBenefitPerVisit = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticMaxBenefitVisit = new System.Windows.Forms.Label();
            this.cmbCoPayWaiveIfAdmitted = new System.Windows.Forms.ComboBox();
            this.cmbOutOfPocketMet = new System.Windows.Forms.ComboBox();
            this.lblStaticRemarks = new System.Windows.Forms.Label();
            this.cmbAutoMedpayCoverage = new System.Windows.Forms.ComboBox();
            this.cmbRule = new System.Windows.Forms.ComboBox();
            this.lineLabel3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.cmbPreexistingCondition = new System.Windows.Forms.ComboBox();
            this.cmbLifetimeMaxBenefitMet = new System.Windows.Forms.ComboBox();
            this.mtbMaxBenefitAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbNumberVisitsPerYear = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbCoPayAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbPercentOutOfPocket = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOutOfPocketDollarsMet = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbOutOfPocketAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticOutOfPocket = new System.Windows.Forms.Label();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.mtbCoInsuranceAmount = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticPercent1 = new System.Windows.Forms.Label();
            this.cmbDeductibleMet = new System.Windows.Forms.ComboBox();
            this.cmbTimePeriod = new System.Windows.Forms.ComboBox();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabelBenefits1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.mtbTerminationDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
            this.lvBenefitsCategories = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.lblStaticListLabel = new System.Windows.Forms.Label();
            this.lblStaticMaxBenefit = new System.Windows.Forms.Label();
            this.lblStaticMaxBenefitMet = new System.Windows.Forms.Label();
            this.lineLabelBenefits3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblStaticCoPay = new System.Windows.Forms.Label();
            this.lblStaticWaive = new System.Windows.Forms.Label();
            this.lblStaticVisitsPerYear = new System.Windows.Forms.Label();
            this.lblStaticPaymentMet = new System.Windows.Forms.Label();
            this.lblStaticOutOfPocketDollarsMet = new System.Windows.Forms.Label();
            this.lblStaticAfterOutOfPocket = new System.Windows.Forms.Label();
            this.lblStaticPercent2 = new System.Windows.Forms.Label();
            this.lblStaticTime = new System.Windows.Forms.Label();
            this.lblStaticMet = new System.Windows.Forms.Label();
            this.lblStaticDollarsMet = new System.Windows.Forms.Label();
            this.lblStaticCoInsurance = new System.Windows.Forms.Label();
            this.lblStaticInsRepName = new System.Windows.Forms.Label();
            this.lblStaticTermDate = new System.Windows.Forms.Label();
            this.dtpTerminationDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.mtbEffectiveDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEffectiveDate = new System.Windows.Forms.Label();
            this.mtbEligibilityPhone = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticEligibilityPhone = new System.Windows.Forms.Label();
            this.lblStaticInfoRecvFrom = new System.Windows.Forms.Label();
            this.lblStaticPreCondition = new System.Windows.Forms.Label();
            this.cmbCoveredBenefit = new System.Windows.Forms.ComboBox();
            this.lblStaticCoveredBenefit = new System.Windows.Forms.Label();
            this.lblStaticClaimsVerified = new System.Windows.Forms.Label();
            this.cmbAddressVerified = new System.Windows.Forms.ComboBox();
            this.lblStaticProduct = new System.Windows.Forms.Label();
            this.cmbTypeOfProduct = new System.Windows.Forms.ComboBox();
            this.lblStaticPPO = new System.Windows.Forms.Label();
            this.lblStaticFacilityProvider = new System.Windows.Forms.Label();
            this.cmbFacilityIsProvider = new System.Windows.Forms.ComboBox();
            this.lblStaticMedpayCoverage = new System.Windows.Forms.Label();
            this.mtbAutoInsuranceClaimNumber = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticClaimNumber = new System.Windows.Forms.Label();
            this.lblStaticRule = new System.Windows.Forms.Label();
            this.cmbCoordinationOfBenefits = new System.Windows.Forms.ComboBox();
            this.lblStaticCoordBenefits = new System.Windows.Forms.Label();
            this.lineLabelBenefits2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.btnDumpResponse = new System.Windows.Forms.Button();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.btnDumpResponse );
            this.panelMain.Controls.Add( this.label1 );
            this.panelMain.Controls.Add( this.lblDollar1 );
            this.panelMain.Controls.Add( this.lblRemainingBenefitPerVisit );
            this.panelMain.Controls.Add( this.lblRemainingLifetimeValue );
            this.panelMain.Controls.Add( this.mtbRemainingBenefitPerVisit );
            this.panelMain.Controls.Add( this.mtbRemainingLifetimeValue );
            this.panelMain.Controls.Add( this.lblVertLineLabel );
            this.panelMain.Controls.Add( this.mtbDeductible );
            this.panelMain.Controls.Add( this.lblStaticDeductible );
            this.panelMain.Controls.Add( this.nameAndPhoneView2 );
            this.panelMain.Controls.Add( this.nameAndPhoneView1 );
            this.panelMain.Controls.Add( this.insuranceAgentAddressView );
            this.panelMain.Controls.Add( this.lineLabel4 );
            this.panelMain.Controls.Add( this.attorneyAddressView );
            this.panelMain.Controls.Add( this.lineLabel5 );
            this.panelMain.Controls.Add( this.btnClearAll );
            this.panelMain.Controls.Add( this.mtbPPOProvider );
            this.panelMain.Controls.Add( this.mtbInsRepName );
            this.panelMain.Controls.Add( this.mtbRemarks );
            this.panelMain.Controls.Add( this.mtbDeductibleDollarsMet );
            this.panelMain.Controls.Add( this.cmbMaxBenefitPerVisitMet );
            this.panelMain.Controls.Add( this.lblStaticMaxBenefitVisitMet );
            this.panelMain.Controls.Add( this.mtbMaxBenefitPerVisit );
            this.panelMain.Controls.Add( this.lblStaticMaxBenefitVisit );
            this.panelMain.Controls.Add( this.cmbCoPayWaiveIfAdmitted );
            this.panelMain.Controls.Add( this.cmbOutOfPocketMet );
            this.panelMain.Controls.Add( this.lblStaticRemarks );
            this.panelMain.Controls.Add( this.cmbAutoMedpayCoverage );
            this.panelMain.Controls.Add( this.cmbRule );
            this.panelMain.Controls.Add( this.lineLabel3 );
            this.panelMain.Controls.Add( this.cmbPreexistingCondition );
            this.panelMain.Controls.Add( this.cmbLifetimeMaxBenefitMet );
            this.panelMain.Controls.Add( this.mtbMaxBenefitAmount );
            this.panelMain.Controls.Add( this.mtbNumberVisitsPerYear );
            this.panelMain.Controls.Add( this.mtbCoPayAmount );
            this.panelMain.Controls.Add( this.mtbPercentOutOfPocket );
            this.panelMain.Controls.Add( this.mtbOutOfPocketDollarsMet );
            this.panelMain.Controls.Add( this.mtbOutOfPocketAmount );
            this.panelMain.Controls.Add( this.lblStaticOutOfPocket );
            this.panelMain.Controls.Add( this.lineLabel2 );
            this.panelMain.Controls.Add( this.mtbCoInsuranceAmount );
            this.panelMain.Controls.Add( this.lblStaticPercent1 );
            this.panelMain.Controls.Add( this.cmbDeductibleMet );
            this.panelMain.Controls.Add( this.cmbTimePeriod );
            this.panelMain.Controls.Add( this.lineLabel1 );
            this.panelMain.Controls.Add( this.lineLabelBenefits1 );
            this.panelMain.Controls.Add( this.mtbTerminationDate );
            this.panelMain.Controls.Add( this.cmbInfoRecvFrom );
            this.panelMain.Controls.Add( this.lvBenefitsCategories );
            this.panelMain.Controls.Add( this.lblStaticListLabel );
            this.panelMain.Controls.Add( this.lblStaticMaxBenefit );
            this.panelMain.Controls.Add( this.lblStaticMaxBenefitMet );
            this.panelMain.Controls.Add( this.lineLabelBenefits3 );
            this.panelMain.Controls.Add( this.lblStaticCoPay );
            this.panelMain.Controls.Add( this.lblStaticWaive );
            this.panelMain.Controls.Add( this.lblStaticVisitsPerYear );
            this.panelMain.Controls.Add( this.lblStaticPaymentMet );
            this.panelMain.Controls.Add( this.lblStaticOutOfPocketDollarsMet );
            this.panelMain.Controls.Add( this.lblStaticAfterOutOfPocket );
            this.panelMain.Controls.Add( this.lblStaticPercent2 );
            this.panelMain.Controls.Add( this.lblStaticTime );
            this.panelMain.Controls.Add( this.lblStaticMet );
            this.panelMain.Controls.Add( this.lblStaticDollarsMet );
            this.panelMain.Controls.Add( this.lblStaticCoInsurance );
            this.panelMain.Controls.Add( this.lblStaticInsRepName );
            this.panelMain.Controls.Add( this.lblStaticTermDate );
            this.panelMain.Controls.Add( this.dtpTerminationDate );
            this.panelMain.Controls.Add( this.dtpEffectiveDate );
            this.panelMain.Controls.Add( this.mtbEffectiveDate );
            this.panelMain.Controls.Add( this.lblStaticEffectiveDate );
            this.panelMain.Controls.Add( this.mtbEligibilityPhone );
            this.panelMain.Controls.Add( this.lblStaticEligibilityPhone );
            this.panelMain.Controls.Add( this.lblStaticInfoRecvFrom );
            this.panelMain.Controls.Add( this.lblStaticPreCondition );
            this.panelMain.Controls.Add( this.cmbCoveredBenefit );
            this.panelMain.Controls.Add( this.lblStaticCoveredBenefit );
            this.panelMain.Controls.Add( this.lblStaticClaimsVerified );
            this.panelMain.Controls.Add( this.cmbAddressVerified );
            this.panelMain.Controls.Add( this.lblStaticProduct );
            this.panelMain.Controls.Add( this.cmbTypeOfProduct );
            this.panelMain.Controls.Add( this.lblStaticPPO );
            this.panelMain.Controls.Add( this.lblStaticFacilityProvider );
            this.panelMain.Controls.Add( this.cmbFacilityIsProvider );
            this.panelMain.Controls.Add( this.lblStaticMedpayCoverage );
            this.panelMain.Controls.Add( this.mtbAutoInsuranceClaimNumber );
            this.panelMain.Controls.Add( this.lblStaticClaimNumber );
            this.panelMain.Controls.Add( this.lblStaticRule );
            this.panelMain.Controls.Add( this.cmbCoordinationOfBenefits );
            this.panelMain.Controls.Add( this.lblStaticCoordBenefits );
            this.panelMain.Controls.Add( this.lineLabelBenefits2 );
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point( 0, 0 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 850, 912 );
            this.panelMain.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 555, 267 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 13, 13 );
            this.label1.TabIndex = 0;
            this.label1.Text = "$";
            // 
            // lblDollar1
            // 
            this.lblDollar1.AutoSize = true;
            this.lblDollar1.Location = new System.Drawing.Point( 555, 240 );
            this.lblDollar1.Name = "lblDollar1";
            this.lblDollar1.Size = new System.Drawing.Size( 13, 13 );
            this.lblDollar1.TabIndex = 0;
            this.lblDollar1.Text = "$";
            // 
            // lblRemainingBenefitPerVisit
            // 
            this.lblRemainingBenefitPerVisit.AutoSize = true;
            this.lblRemainingBenefitPerVisit.Location = new System.Drawing.Point( 424, 267 );
            this.lblRemainingBenefitPerVisit.Name = "lblRemainingBenefitPerVisit";
            this.lblRemainingBenefitPerVisit.Size = new System.Drawing.Size( 118, 13 );
            this.lblRemainingBenefitPerVisit.TabIndex = 0;
            this.lblRemainingBenefitPerVisit.Text = "Remaining benefit/visit:";
            // 
            // lblRemainingLifetimeValue
            // 
            this.lblRemainingLifetimeValue.AutoSize = true;
            this.lblRemainingLifetimeValue.Location = new System.Drawing.Point( 424, 240 );
            this.lblRemainingLifetimeValue.Name = "lblRemainingLifetimeValue";
            this.lblRemainingLifetimeValue.Size = new System.Drawing.Size( 124, 13 );
            this.lblRemainingLifetimeValue.TabIndex = 0;
            this.lblRemainingLifetimeValue.Text = "Remaining lifetime value:";
            // 
            // mtbRemainingBenefitPerVisit
            // 
            this.mtbRemainingBenefitPerVisit.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemainingBenefitPerVisit.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingBenefitPerVisit.Location = new System.Drawing.Point( 570, 264 );
            this.mtbRemainingBenefitPerVisit.Mask = "";
            this.mtbRemainingBenefitPerVisit.MaxLength = 14;
            this.mtbRemainingBenefitPerVisit.Name = "mtbRemainingBenefitPerVisit";
            this.mtbRemainingBenefitPerVisit.Size = new System.Drawing.Size( 101, 20 );
            this.mtbRemainingBenefitPerVisit.TabIndex = 23;
            this.mtbRemainingBenefitPerVisit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingBenefitPerVisit.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingBenefitPerVisit.Enter += new System.EventHandler( this.mtbRemainingBenefitPerVisit_Enter );
            this.mtbRemainingBenefitPerVisit.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemainingBenefitPerVisit_Validating );
            // 
            // mtbRemainingLifetimeValue
            // 
            this.mtbRemainingLifetimeValue.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemainingLifetimeValue.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingLifetimeValue.Location = new System.Drawing.Point( 570, 237 );
            this.mtbRemainingLifetimeValue.Mask = "";
            this.mtbRemainingLifetimeValue.MaxLength = 14;
            this.mtbRemainingLifetimeValue.Name = "mtbRemainingLifetimeValue";
            this.mtbRemainingLifetimeValue.Size = new System.Drawing.Size( 101, 20 );
            this.mtbRemainingLifetimeValue.TabIndex = 20;
            this.mtbRemainingLifetimeValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingLifetimeValue.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbRemainingLifetimeValue.Enter += new System.EventHandler( this.mtbRemainingLifetimeValue_Enter );
            this.mtbRemainingLifetimeValue.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemainingLifetimeValue_Validating );
            // 
            // lblVertLineLabel
            // 
            this.lblVertLineLabel.BackColor = System.Drawing.Color.Black;
            this.lblVertLineLabel.Caption = "label1";
            this.lblVertLineLabel.Location = new System.Drawing.Point( 166, 109 );
            this.lblVertLineLabel.Name = "lblVertLineLabel";
            this.lblVertLineLabel.Size = new System.Drawing.Size( 1, 186 );
            this.lblVertLineLabel.TabIndex = 0;
            this.lblVertLineLabel.TabStop = false;
            // 
            // mtbDeductible
            // 
            this.mtbDeductible.Enabled = false;
            this.mtbDeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductible.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductible.Location = new System.Drawing.Point( 256, 111 );
            this.mtbDeductible.Mask = "";
            this.mtbDeductible.MaxLength = 10;
            this.mtbDeductible.Name = "mtbDeductible";
            this.mtbDeductible.Size = new System.Drawing.Size( 60, 20 );
            this.mtbDeductible.TabIndex = 7;
            this.mtbDeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductible.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductible.Enter += new System.EventHandler( this.mtbDeductible_Enter );
            this.mtbDeductible.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDeductible_Validating );
            // 
            // lblStaticDeductible
            // 
            this.lblStaticDeductible.Location = new System.Drawing.Point( 184, 114 );
            this.lblStaticDeductible.Name = "lblStaticDeductible";
            this.lblStaticDeductible.Size = new System.Drawing.Size( 77, 23 );
            this.lblStaticDeductible.TabIndex = 0;
            this.lblStaticDeductible.Text = "Deductible:  $";
            // 
            // nameAndPhoneView2
            // 
            this.nameAndPhoneView2.Location = new System.Drawing.Point( 8, 680 );
            this.nameAndPhoneView2.Model = null;
            this.nameAndPhoneView2.Model_Person = null;
            this.nameAndPhoneView2.Name = "nameAndPhoneView2";
            this.nameAndPhoneView2.NameLabel = "Auto/Home insurance agent name:";
            this.nameAndPhoneView2.PhoneLabel = "Phone:";
            this.nameAndPhoneView2.Size = new System.Drawing.Size( 464, 56 );
            this.nameAndPhoneView2.TabIndex = 46;
            // 
            // nameAndPhoneView1
            // 
            this.nameAndPhoneView1.Location = new System.Drawing.Point( 8, 496 );
            this.nameAndPhoneView1.Model = null;
            this.nameAndPhoneView1.Model_Person = null;
            this.nameAndPhoneView1.Name = "nameAndPhoneView1";
            this.nameAndPhoneView1.NameLabel = "Attorney name:";
            this.nameAndPhoneView1.PhoneLabel = "Phone:";
            this.nameAndPhoneView1.Size = new System.Drawing.Size( 464, 56 );
            this.nameAndPhoneView1.TabIndex = 40;
            // 
            // insuranceAgentAddressView
            // 
            this.insuranceAgentAddressView.Context = null;
            this.insuranceAgentAddressView.KindOfTargetParty = null;
            this.insuranceAgentAddressView.Location = new System.Drawing.Point( 568, 680 );
            this.insuranceAgentAddressView.Model = null;
            this.insuranceAgentAddressView.Name = "insuranceAgentAddressView";
            this.insuranceAgentAddressView.PatientAccount = null;
            this.insuranceAgentAddressView.Size = new System.Drawing.Size( 265, 144 );
            this.insuranceAgentAddressView.TabIndex = 50;
            this.insuranceAgentAddressView.ShowStatus = false;
            // 
            // lineLabel4
            // 
            this.lineLabel4.Caption = "";
            this.lineLabel4.Location = new System.Drawing.Point( 8, 832 );
            this.lineLabel4.Name = "lineLabel4";
            this.lineLabel4.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel4.TabIndex = 0;
            this.lineLabel4.TabStop = false;
            // 
            // attorneyAddressView
            // 
            this.attorneyAddressView.Context = null;
            this.attorneyAddressView.KindOfTargetParty = null;
            this.attorneyAddressView.Location = new System.Drawing.Point( 568, 496 );
            this.attorneyAddressView.Model = null;
            this.attorneyAddressView.Name = "attorneyAddressView";
            this.attorneyAddressView.PatientAccount = null;
            this.attorneyAddressView.Size = new System.Drawing.Size( 265, 144 );
            this.attorneyAddressView.TabIndex = 45;
            this.attorneyAddressView.ShowStatus = false;
            // 
            // lineLabel5
            // 
            this.lineLabel5.Caption = "";
            this.lineLabel5.Location = new System.Drawing.Point( 8, 648 );
            this.lineLabel5.Name = "lineLabel5";
            this.lineLabel5.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel5.TabIndex = 0;
            this.lineLabel5.TabStop = false;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point( 760, 864 );
            this.btnClearAll.Message = null;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size( 75, 23 );
            this.btnClearAll.TabIndex = 99;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler( this.btnClearAll_Click );
            // 
            // mtbPPOProvider
            // 
            this.mtbPPOProvider.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPPOProvider.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPPOProvider.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOProvider.Location = new System.Drawing.Point( 552, 351 );
            this.mtbPPOProvider.Mask = "";
            this.mtbPPOProvider.MaxLength = 25;
            this.mtbPPOProvider.Name = "mtbPPOProvider";
            this.mtbPPOProvider.Size = new System.Drawing.Size( 160, 20 );
            this.mtbPPOProvider.TabIndex = 29;
            this.mtbPPOProvider.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOProvider.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPPOProvider_Validating );
            // 
            // mtbInsRepName
            // 
            this.mtbInsRepName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbInsRepName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbInsRepName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsRepName.Location = new System.Drawing.Point( 159, 74 );
            this.mtbInsRepName.Mask = "";
            this.mtbInsRepName.MaxLength = 25;
            this.mtbInsRepName.Name = "mtbInsRepName";
            this.mtbInsRepName.Size = new System.Drawing.Size( 185, 20 );
            this.mtbInsRepName.TabIndex = 3;
            this.mtbInsRepName.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsRepName.Validating += new System.ComponentModel.CancelEventHandler( this.mtbInsRepName_Validating );
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point( 64, 864 );
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 345, 32 );
            this.mtbRemarks.TabIndex = 60;
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemarks_Validating );
            // 
            // mtbDeductibleDollarsMet
            // 
            this.mtbDeductibleDollarsMet.Enabled = false;
            this.mtbDeductibleDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDeductibleDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductibleDollarsMet.Location = new System.Drawing.Point( 620, 111 );
            this.mtbDeductibleDollarsMet.Mask = "";
            this.mtbDeductibleDollarsMet.MaxLength = 10;
            this.mtbDeductibleDollarsMet.Name = "mtbDeductibleDollarsMet";
            this.mtbDeductibleDollarsMet.Size = new System.Drawing.Size( 60, 20 );
            this.mtbDeductibleDollarsMet.TabIndex = 10;
            this.mtbDeductibleDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbDeductibleDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbDeductibleDollarsMet.Enter += new System.EventHandler( this.mtbDeductibleDollarsMet_Enter );
            this.mtbDeductibleDollarsMet.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDeductibleDollarsMet_Validating );
            // 
            // cmbMaxBenefitPerVisitMet
            // 
            this.cmbMaxBenefitPerVisitMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMaxBenefitPerVisitMet.Location = new System.Drawing.Point( 739, 268 );
            this.cmbMaxBenefitPerVisitMet.Name = "cmbMaxBenefitPerVisitMet";
            this.cmbMaxBenefitPerVisitMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbMaxBenefitPerVisitMet.TabIndex = 24;
            this.cmbMaxBenefitPerVisitMet.SelectedIndexChanged += new System.EventHandler( this.cmbMaxBenefitPerVisitMet_SelectedIndexChanged );
            // 
            // lblStaticMaxBenefitVisitMet
            // 
            this.lblStaticMaxBenefitVisitMet.Location = new System.Drawing.Point( 707, 271 );
            this.lblStaticMaxBenefitVisitMet.Name = "lblStaticMaxBenefitVisitMet";
            this.lblStaticMaxBenefitVisitMet.Size = new System.Drawing.Size( 35, 23 );
            this.lblStaticMaxBenefitVisitMet.TabIndex = 0;
            this.lblStaticMaxBenefitVisitMet.Text = "Met:";
            // 
            // mtbMaxBenefitPerVisit
            // 
            this.mtbMaxBenefitPerVisit.Enabled = false;
            this.mtbMaxBenefitPerVisit.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbMaxBenefitPerVisit.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitPerVisit.Location = new System.Drawing.Point( 299, 268 );
            this.mtbMaxBenefitPerVisit.Mask = "";
            this.mtbMaxBenefitPerVisit.MaxLength = 14;
            this.mtbMaxBenefitPerVisit.Name = "mtbMaxBenefitPerVisit";
            this.mtbMaxBenefitPerVisit.Size = new System.Drawing.Size( 88, 20 );
            this.mtbMaxBenefitPerVisit.TabIndex = 22;
            this.mtbMaxBenefitPerVisit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMaxBenefitPerVisit.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitPerVisit.Enter += new System.EventHandler( this.mtbMaxBenefitPerVisit_Enter );
            this.mtbMaxBenefitPerVisit.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMaxBenefitPerVisit_Validating );
            // 
            // lblStaticMaxBenefitVisit
            // 
            this.lblStaticMaxBenefitVisit.Location = new System.Drawing.Point( 184, 271 );
            this.lblStaticMaxBenefitVisit.Name = "lblStaticMaxBenefitVisit";
            this.lblStaticMaxBenefitVisit.Size = new System.Drawing.Size( 121, 23 );
            this.lblStaticMaxBenefitVisit.TabIndex = 0;
            this.lblStaticMaxBenefitVisit.Text = "Max benefit/visit:       $";
            // 
            // cmbCoPayWaiveIfAdmitted
            // 
            this.cmbCoPayWaiveIfAdmitted.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoPayWaiveIfAdmitted.Location = new System.Drawing.Point( 408, 193 );
            this.cmbCoPayWaiveIfAdmitted.Name = "cmbCoPayWaiveIfAdmitted";
            this.cmbCoPayWaiveIfAdmitted.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoPayWaiveIfAdmitted.TabIndex = 17;
            this.cmbCoPayWaiveIfAdmitted.SelectedIndexChanged += new System.EventHandler( this.cmbCoPayWaiveIfAdmitted_SelectedIndexChanged );
            // 
            // cmbOutOfPocketMet
            // 
            this.cmbOutOfPocketMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutOfPocketMet.Location = new System.Drawing.Point( 376, 153 );
            this.cmbOutOfPocketMet.Name = "cmbOutOfPocketMet";
            this.cmbOutOfPocketMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbOutOfPocketMet.TabIndex = 13;
            this.cmbOutOfPocketMet.SelectedIndexChanged += new System.EventHandler( this.cmbOutOfPocketMet_SelectedIndexChanged );
            // 
            // lblStaticRemarks
            // 
            this.lblStaticRemarks.Location = new System.Drawing.Point( 8, 864 );
            this.lblStaticRemarks.Name = "lblStaticRemarks";
            this.lblStaticRemarks.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticRemarks.TabIndex = 0;
            this.lblStaticRemarks.Text = "Remarks:";
            // 
            // cmbAutoMedpayCoverage
            // 
            this.cmbAutoMedpayCoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoMedpayCoverage.Location = new System.Drawing.Point( 552, 453 );
            this.cmbAutoMedpayCoverage.Name = "cmbAutoMedpayCoverage";
            this.cmbAutoMedpayCoverage.Size = new System.Drawing.Size( 50, 21 );
            this.cmbAutoMedpayCoverage.TabIndex = 32;
            this.cmbAutoMedpayCoverage.SelectedIndexChanged += new System.EventHandler( this.cmbAutoMedpayCoverage_SelectedIndexChanged );
            // 
            // cmbRule
            // 
            this.cmbRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRule.Location = new System.Drawing.Point( 215, 453 );
            this.cmbRule.Name = "cmbRule";
            this.cmbRule.Size = new System.Drawing.Size( 80, 21 );
            this.cmbRule.TabIndex = 27;
            this.cmbRule.SelectedIndexChanged += new System.EventHandler( this.cmbRule_SelectedIndexChanged );
            // 
            // lineLabel3
            // 
            this.lineLabel3.Caption = "";
            this.lineLabel3.Location = new System.Drawing.Point( 8, 476 );
            this.lineLabel3.Name = "lineLabel3";
            this.lineLabel3.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel3.TabIndex = 0;
            this.lineLabel3.TabStop = false;
            // 
            // cmbPreexistingCondition
            // 
            this.cmbPreexistingCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreexistingCondition.Location = new System.Drawing.Point( 181, 317 );
            this.cmbPreexistingCondition.Name = "cmbPreexistingCondition";
            this.cmbPreexistingCondition.Size = new System.Drawing.Size( 45, 21 );
            this.cmbPreexistingCondition.TabIndex = 23;
            this.cmbPreexistingCondition.SelectedIndexChanged += new System.EventHandler( this.cmbPreexistingCondition_SelectedIndexChanged );
            // 
            // cmbLifetimeMaxBenefitMet
            // 
            this.cmbLifetimeMaxBenefitMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLifetimeMaxBenefitMet.Location = new System.Drawing.Point( 739, 237 );
            this.cmbLifetimeMaxBenefitMet.Name = "cmbLifetimeMaxBenefitMet";
            this.cmbLifetimeMaxBenefitMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbLifetimeMaxBenefitMet.TabIndex = 21;
            this.cmbLifetimeMaxBenefitMet.SelectedIndexChanged += new System.EventHandler( this.cmbLifetimeMaxBenefitMet_SelectedIndexChanged );
            // 
            // mtbMaxBenefitAmount
            // 
            this.mtbMaxBenefitAmount.Enabled = false;
            this.mtbMaxBenefitAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.Default;
            this.mtbMaxBenefitAmount.KeyPressExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitAmount.Location = new System.Drawing.Point( 299, 238 );
            this.mtbMaxBenefitAmount.Mask = "";
            this.mtbMaxBenefitAmount.MaxLength = 14;
            this.mtbMaxBenefitAmount.Name = "mtbMaxBenefitAmount";
            this.mtbMaxBenefitAmount.Size = new System.Drawing.Size( 88, 20 );
            this.mtbMaxBenefitAmount.TabIndex = 19;
            this.mtbMaxBenefitAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbMaxBenefitAmount.ValidationExpression = "^[0-9]{0,9}(\\.[0-9]{0,2})?$";
            this.mtbMaxBenefitAmount.Enter += new System.EventHandler( this.mtbMaxBenefitAmount_Enter );
            this.mtbMaxBenefitAmount.Validating += new System.ComponentModel.CancelEventHandler( this.mtbMaxBenefitAmount_Validating );
            // 
            // mtbNumberVisitsPerYear
            // 
            this.mtbNumberVisitsPerYear.Enabled = false;
            this.mtbNumberVisitsPerYear.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbNumberVisitsPerYear.KeyPressExpression = "^\\d*$";
            this.mtbNumberVisitsPerYear.Location = new System.Drawing.Point( 528, 194 );
            this.mtbNumberVisitsPerYear.Mask = "";
            this.mtbNumberVisitsPerYear.MaxLength = 3;
            this.mtbNumberVisitsPerYear.Name = "mtbNumberVisitsPerYear";
            this.mtbNumberVisitsPerYear.Size = new System.Drawing.Size( 30, 20 );
            this.mtbNumberVisitsPerYear.TabIndex = 18;
            this.mtbNumberVisitsPerYear.ValidationExpression = "^\\d*$";
            this.mtbNumberVisitsPerYear.Validating += new System.ComponentModel.CancelEventHandler( this.mtbNumberVisitsPerYear_Validating );
            // 
            // mtbCoPayAmount
            // 
            this.mtbCoPayAmount.Enabled = false;
            this.mtbCoPayAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoPayAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbCoPayAmount.Location = new System.Drawing.Point( 240, 193 );
            this.mtbCoPayAmount.Mask = "";
            this.mtbCoPayAmount.MaxLength = 10;
            this.mtbCoPayAmount.Name = "mtbCoPayAmount";
            this.mtbCoPayAmount.Size = new System.Drawing.Size( 60, 20 );
            this.mtbCoPayAmount.TabIndex = 16;
            this.mtbCoPayAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbCoPayAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbCoPayAmount.Enter += new System.EventHandler( this.mtbCoPayAmount_Enter );
            this.mtbCoPayAmount.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCoPayAmount_Validating );
            // 
            // mtbPercentOutOfPocket
            // 
            this.mtbPercentOutOfPocket.Enabled = false;
            this.mtbPercentOutOfPocket.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbPercentOutOfPocket.KeyPressExpression = "^\\d*$";
            this.mtbPercentOutOfPocket.Location = new System.Drawing.Point( 688, 153 );
            this.mtbPercentOutOfPocket.Mask = "";
            this.mtbPercentOutOfPocket.MaxLength = 3;
            this.mtbPercentOutOfPocket.Name = "mtbPercentOutOfPocket";
            this.mtbPercentOutOfPocket.Size = new System.Drawing.Size( 30, 20 );
            this.mtbPercentOutOfPocket.TabIndex = 15;
            this.mtbPercentOutOfPocket.ValidationExpression = "^\\d*$";
            this.mtbPercentOutOfPocket.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPercentOutOfPocket_Validating );
            // 
            // mtbOutOfPocketDollarsMet
            // 
            this.mtbOutOfPocketDollarsMet.Enabled = false;
            this.mtbOutOfPocketDollarsMet.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbOutOfPocketDollarsMet.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketDollarsMet.Location = new System.Drawing.Point( 504, 153 );
            this.mtbOutOfPocketDollarsMet.Mask = "";
            this.mtbOutOfPocketDollarsMet.MaxLength = 10;
            this.mtbOutOfPocketDollarsMet.Name = "mtbOutOfPocketDollarsMet";
            this.mtbOutOfPocketDollarsMet.Size = new System.Drawing.Size( 60, 20 );
            this.mtbOutOfPocketDollarsMet.TabIndex = 14;
            this.mtbOutOfPocketDollarsMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketDollarsMet.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketDollarsMet.Enter += new System.EventHandler( this.mtbOutOfPocketDollarsMet_Enter );
            this.mtbOutOfPocketDollarsMet.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOutOfPocketDollarsMet_Validating );
            // 
            // mtbOutOfPocketAmount
            // 
            this.mtbOutOfPocketAmount.Enabled = false;
            this.mtbOutOfPocketAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbOutOfPocketAmount.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketAmount.Location = new System.Drawing.Point( 272, 153 );
            this.mtbOutOfPocketAmount.Mask = "";
            this.mtbOutOfPocketAmount.MaxLength = 10;
            this.mtbOutOfPocketAmount.Name = "mtbOutOfPocketAmount";
            this.mtbOutOfPocketAmount.Size = new System.Drawing.Size( 60, 20 );
            this.mtbOutOfPocketAmount.TabIndex = 12;
            this.mtbOutOfPocketAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbOutOfPocketAmount.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbOutOfPocketAmount.Enter += new System.EventHandler( this.mtbOutOfPocketAmount_Enter );
            this.mtbOutOfPocketAmount.Validating += new System.ComponentModel.CancelEventHandler( this.mtbOutOfPocketAmount_Validating );
            // 
            // lblStaticOutOfPocket
            // 
            this.lblStaticOutOfPocket.Location = new System.Drawing.Point( 184, 156 );
            this.lblStaticOutOfPocket.Name = "lblStaticOutOfPocket";
            this.lblStaticOutOfPocket.Size = new System.Drawing.Size( 90, 23 );
            this.lblStaticOutOfPocket.TabIndex = 0;
            this.lblStaticOutOfPocket.Text = "Out-of-pocket:  $";
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "";
            this.lineLabel2.Location = new System.Drawing.Point( 8, 290 );
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            // 
            // mtbCoInsuranceAmount
            // 
            this.mtbCoInsuranceAmount.Enabled = false;
            this.mtbCoInsuranceAmount.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbCoInsuranceAmount.KeyPressExpression = "^\\d*$";
            this.mtbCoInsuranceAmount.Location = new System.Drawing.Point( 764, 111 );
            this.mtbCoInsuranceAmount.Mask = "";
            this.mtbCoInsuranceAmount.MaxLength = 3;
            this.mtbCoInsuranceAmount.Name = "mtbCoInsuranceAmount";
            this.mtbCoInsuranceAmount.Size = new System.Drawing.Size( 30, 20 );
            this.mtbCoInsuranceAmount.TabIndex = 11;
            this.mtbCoInsuranceAmount.ValidationExpression = "^\\d*$";
            this.mtbCoInsuranceAmount.Validating += new System.ComponentModel.CancelEventHandler( this.mtbCoInsuranceAmount_Validating );
            // 
            // lblStaticPercent1
            // 
            this.lblStaticPercent1.Location = new System.Drawing.Point( 796, 114 );
            this.lblStaticPercent1.Name = "lblStaticPercent1";
            this.lblStaticPercent1.Size = new System.Drawing.Size( 16, 23 );
            this.lblStaticPercent1.TabIndex = 0;
            this.lblStaticPercent1.Text = "%";
            // 
            // cmbDeductibleMet
            // 
            this.cmbDeductibleMet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDeductibleMet.Location = new System.Drawing.Point( 492, 111 );
            this.cmbDeductibleMet.Name = "cmbDeductibleMet";
            this.cmbDeductibleMet.Size = new System.Drawing.Size( 45, 21 );
            this.cmbDeductibleMet.TabIndex = 9;
            this.cmbDeductibleMet.SelectedIndexChanged += new System.EventHandler( this.cmbDeductibleMet_SelectedIndexChanged );
            // 
            // cmbTimePeriod
            // 
            this.cmbTimePeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTimePeriod.Location = new System.Drawing.Point( 400, 111 );
            this.cmbTimePeriod.Name = "cmbTimePeriod";
            this.cmbTimePeriod.Size = new System.Drawing.Size( 50, 21 );
            this.cmbTimePeriod.TabIndex = 8;
            this.cmbTimePeriod.SelectedIndexChanged += new System.EventHandler( this.cmbTimePeriod_SelectedIndexChanged );
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point( 9, 88 );
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel1.TabIndex = 0;
            this.lineLabel1.TabStop = false;
            // 
            // lineLabelBenefits1
            // 
            this.lineLabelBenefits1.Caption = "";
            this.lineLabelBenefits1.Location = new System.Drawing.Point( 184, 128 );
            this.lineLabelBenefits1.Name = "lineLabelBenefits1";
            this.lineLabelBenefits1.Size = new System.Drawing.Size( 664, 18 );
            this.lineLabelBenefits1.TabIndex = 0;
            this.lineLabelBenefits1.TabStop = false;
            // 
            // mtbTerminationDate
            // 
            this.mtbTerminationDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTerminationDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbTerminationDate.Location = new System.Drawing.Point( 579, 45 );
            this.mtbTerminationDate.Mask = "  /  /";
            this.mtbTerminationDate.MaxLength = 10;
            this.mtbTerminationDate.Name = "mtbTerminationDate";
            this.mtbTerminationDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbTerminationDate.TabIndex = 5;
            this.mtbTerminationDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbTerminationDate.Enter += new System.EventHandler( this.mtbTerminationDate_Enter );
            this.mtbTerminationDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbTerminationDate_Validating );
            // 
            // cmbInfoRecvFrom
            // 
            this.cmbInfoRecvFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point( 159, 14 );
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size( 185, 21 );
            this.cmbInfoRecvFrom.TabIndex = 1;
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler( this.cmbInfoRecvFrom_SelectedIndexChanged );
            this.cmbInfoRecvFrom.DropDown += new System.EventHandler( this.cmbInfoRecvFrom_DropDown );
            // 
            // lvBenefitsCategories
            // 
            this.lvBenefitsCategories.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvBenefitsCategories.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1} );
            this.lvBenefitsCategories.FullRowSelect = true;
            this.lvBenefitsCategories.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvBenefitsCategories.HideSelection = false;
            this.lvBenefitsCategories.Location = new System.Drawing.Point( 8, 128 );
            this.lvBenefitsCategories.MultiSelect = false;
            this.lvBenefitsCategories.Name = "lvBenefitsCategories";
            this.lvBenefitsCategories.Size = new System.Drawing.Size( 145, 140 );
            this.lvBenefitsCategories.TabIndex = 6;
            this.lvBenefitsCategories.UseCompatibleStateImageBehavior = false;
            this.lvBenefitsCategories.View = System.Windows.Forms.View.Details;
            this.lvBenefitsCategories.SelectedIndexChanged += new System.EventHandler( this.lvBenefitsCategories_SelectedIndexChanged );
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Benefits Category";
            this.columnHeader1.Width = 128;
            // 
            // lblStaticListLabel
            // 
            this.lblStaticListLabel.Location = new System.Drawing.Point( 8, 275 );
            this.lblStaticListLabel.Name = "lblStaticListLabel";
            this.lblStaticListLabel.Size = new System.Drawing.Size( 104, 23 );
            this.lblStaticListLabel.TabIndex = 0;
            this.lblStaticListLabel.Text = "Bold = HSV default";
            // 
            // lblStaticMaxBenefit
            // 
            this.lblStaticMaxBenefit.Location = new System.Drawing.Point( 184, 241 );
            this.lblStaticMaxBenefit.Name = "lblStaticMaxBenefit";
            this.lblStaticMaxBenefit.Size = new System.Drawing.Size( 122, 23 );
            this.lblStaticMaxBenefit.TabIndex = 0;
            this.lblStaticMaxBenefit.Text = "Lifetime max benefit:  $";
            // 
            // lblStaticMaxBenefitMet
            // 
            this.lblStaticMaxBenefitMet.Location = new System.Drawing.Point( 707, 238 );
            this.lblStaticMaxBenefitMet.Name = "lblStaticMaxBenefitMet";
            this.lblStaticMaxBenefitMet.Size = new System.Drawing.Size( 35, 23 );
            this.lblStaticMaxBenefitMet.TabIndex = 0;
            this.lblStaticMaxBenefitMet.Text = "Met:";
            // 
            // lineLabelBenefits3
            // 
            this.lineLabelBenefits3.Caption = "";
            this.lineLabelBenefits3.Location = new System.Drawing.Point( 184, 211 );
            this.lineLabelBenefits3.Name = "lineLabelBenefits3";
            this.lineLabelBenefits3.Size = new System.Drawing.Size( 664, 18 );
            this.lineLabelBenefits3.TabIndex = 0;
            this.lineLabelBenefits3.TabStop = false;
            // 
            // lblStaticCoPay
            // 
            this.lblStaticCoPay.Location = new System.Drawing.Point( 184, 196 );
            this.lblStaticCoPay.Name = "lblStaticCoPay";
            this.lblStaticCoPay.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticCoPay.TabIndex = 0;
            this.lblStaticCoPay.Text = "Co-pay:  $";
            // 
            // lblStaticWaive
            // 
            this.lblStaticWaive.Location = new System.Drawing.Point( 312, 196 );
            this.lblStaticWaive.Name = "lblStaticWaive";
            this.lblStaticWaive.Size = new System.Drawing.Size( 95, 23 );
            this.lblStaticWaive.TabIndex = 0;
            this.lblStaticWaive.Text = "Waive if admitted:";
            // 
            // lblStaticVisitsPerYear
            // 
            this.lblStaticVisitsPerYear.Location = new System.Drawing.Point( 464, 197 );
            this.lblStaticVisitsPerYear.Name = "lblStaticVisitsPerYear";
            this.lblStaticVisitsPerYear.Size = new System.Drawing.Size( 60, 23 );
            this.lblStaticVisitsPerYear.TabIndex = 0;
            this.lblStaticVisitsPerYear.Text = "Visits/year:";
            // 
            // lblStaticPaymentMet
            // 
            this.lblStaticPaymentMet.Location = new System.Drawing.Point( 344, 156 );
            this.lblStaticPaymentMet.Name = "lblStaticPaymentMet";
            this.lblStaticPaymentMet.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticPaymentMet.TabIndex = 0;
            this.lblStaticPaymentMet.Text = "Met:";
            // 
            // lblStaticOutOfPocketDollarsMet
            // 
            this.lblStaticOutOfPocketDollarsMet.Location = new System.Drawing.Point( 432, 156 );
            this.lblStaticOutOfPocketDollarsMet.Name = "lblStaticOutOfPocketDollarsMet";
            this.lblStaticOutOfPocketDollarsMet.Size = new System.Drawing.Size( 78, 23 );
            this.lblStaticOutOfPocketDollarsMet.TabIndex = 0;
            this.lblStaticOutOfPocketDollarsMet.Text = "Dollars met:  $";
            // 
            // lblStaticAfterOutOfPocket
            // 
            this.lblStaticAfterOutOfPocket.Location = new System.Drawing.Point( 576, 156 );
            this.lblStaticAfterOutOfPocket.Name = "lblStaticAfterOutOfPocket";
            this.lblStaticAfterOutOfPocket.Size = new System.Drawing.Size( 112, 23 );
            this.lblStaticAfterOutOfPocket.TabIndex = 0;
            this.lblStaticAfterOutOfPocket.Text = "% after out-of-pocket:";
            // 
            // lblStaticPercent2
            // 
            this.lblStaticPercent2.Location = new System.Drawing.Point( 720, 156 );
            this.lblStaticPercent2.Name = "lblStaticPercent2";
            this.lblStaticPercent2.Size = new System.Drawing.Size( 16, 23 );
            this.lblStaticPercent2.TabIndex = 0;
            this.lblStaticPercent2.Text = "%";
            // 
            // lblStaticTime
            // 
            this.lblStaticTime.Location = new System.Drawing.Point( 328, 114 );
            this.lblStaticTime.Name = "lblStaticTime";
            this.lblStaticTime.Size = new System.Drawing.Size( 71, 23 );
            this.lblStaticTime.TabIndex = 0;
            this.lblStaticTime.Text = "Time period:";
            // 
            // lblStaticMet
            // 
            this.lblStaticMet.Location = new System.Drawing.Point( 461, 114 );
            this.lblStaticMet.Name = "lblStaticMet";
            this.lblStaticMet.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticMet.TabIndex = 0;
            this.lblStaticMet.Text = "Met:";
            // 
            // lblStaticDollarsMet
            // 
            this.lblStaticDollarsMet.Location = new System.Drawing.Point( 548, 114 );
            this.lblStaticDollarsMet.Name = "lblStaticDollarsMet";
            this.lblStaticDollarsMet.Size = new System.Drawing.Size( 77, 23 );
            this.lblStaticDollarsMet.TabIndex = 0;
            this.lblStaticDollarsMet.Text = "Dollars met:  $";
            // 
            // lblStaticCoInsurance
            // 
            this.lblStaticCoInsurance.Location = new System.Drawing.Point( 692, 114 );
            this.lblStaticCoInsurance.Name = "lblStaticCoInsurance";
            this.lblStaticCoInsurance.Size = new System.Drawing.Size( 76, 23 );
            this.lblStaticCoInsurance.TabIndex = 0;
            this.lblStaticCoInsurance.Text = "Co-insurance:";
            // 
            // lblStaticInsRepName
            // 
            this.lblStaticInsRepName.Location = new System.Drawing.Point( 8, 77 );
            this.lblStaticInsRepName.Name = "lblStaticInsRepName";
            this.lblStaticInsRepName.Size = new System.Drawing.Size( 157, 23 );
            this.lblStaticInsRepName.TabIndex = 0;
            this.lblStaticInsRepName.Text = "Insurance company rep name:";
            // 
            // lblStaticTermDate
            // 
            this.lblStaticTermDate.Location = new System.Drawing.Point( 416, 48 );
            this.lblStaticTermDate.Name = "lblStaticTermDate";
            this.lblStaticTermDate.Size = new System.Drawing.Size( 168, 23 );
            this.lblStaticTermDate.TabIndex = 0;
            this.lblStaticTermDate.Text = "Termination date for the Insured:";
            // 
            // dtpTerminationDate
            // 
            this.dtpTerminationDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpTerminationDate.Checked = false;
            this.dtpTerminationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTerminationDate.Location = new System.Drawing.Point( 649, 45 );
            this.dtpTerminationDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpTerminationDate.Name = "dtpTerminationDate";
            this.dtpTerminationDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpTerminationDate.TabIndex = 0;
            this.dtpTerminationDate.TabStop = false;
            this.dtpTerminationDate.CloseUp += new System.EventHandler( this.dtpTerminationDate_CloseUp );
            // 
            // dtpEffectiveDate
            // 
            this.dtpEffectiveDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpEffectiveDate.Checked = false;
            this.dtpEffectiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEffectiveDate.Location = new System.Drawing.Point( 649, 14 );
            this.dtpEffectiveDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpEffectiveDate.Name = "dtpEffectiveDate";
            this.dtpEffectiveDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpEffectiveDate.TabIndex = 0;
            this.dtpEffectiveDate.TabStop = false;
            this.dtpEffectiveDate.CloseUp += new System.EventHandler( this.dtpEffectiveDate_CloseUp );
            // 
            // mtbEffectiveDate
            // 
            this.mtbEffectiveDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEffectiveDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbEffectiveDate.Location = new System.Drawing.Point( 579, 14 );
            this.mtbEffectiveDate.Mask = "  /  /";
            this.mtbEffectiveDate.MaxLength = 10;
            this.mtbEffectiveDate.Name = "mtbEffectiveDate";
            this.mtbEffectiveDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbEffectiveDate.TabIndex = 4;
            this.mtbEffectiveDate.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbEffectiveDate.Enter += new System.EventHandler( this.mtbEffectiveDate_Enter );
            this.mtbEffectiveDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbEffectiveDate_Validating );
            // 
            // lblStaticEffectiveDate
            // 
            this.lblStaticEffectiveDate.Location = new System.Drawing.Point( 416, 17 );
            this.lblStaticEffectiveDate.Name = "lblStaticEffectiveDate";
            this.lblStaticEffectiveDate.Size = new System.Drawing.Size( 136, 23 );
            this.lblStaticEffectiveDate.TabIndex = 0;
            this.lblStaticEffectiveDate.Text = "Effective date for Insured:";
            // 
            // mtbEligibilityPhone
            // 
            this.mtbEligibilityPhone.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbEligibilityPhone.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbEligibilityPhone.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEligibilityPhone.Location = new System.Drawing.Point( 159, 45 );
            this.mtbEligibilityPhone.Mask = "";
            this.mtbEligibilityPhone.MaxLength = 15;
            this.mtbEligibilityPhone.Name = "mtbEligibilityPhone";
            this.mtbEligibilityPhone.Size = new System.Drawing.Size( 100, 20 );
            this.mtbEligibilityPhone.TabIndex = 2;
            this.mtbEligibilityPhone.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbEligibilityPhone.Enter += new System.EventHandler( this.mskEligibilityPhone_Enter );
            this.mtbEligibilityPhone.Validating += new System.ComponentModel.CancelEventHandler( this.mtbEligibilityPhone_Validating );
            // 
            // lblStaticEligibilityPhone
            // 
            this.lblStaticEligibilityPhone.Location = new System.Drawing.Point( 8, 48 );
            this.lblStaticEligibilityPhone.Name = "lblStaticEligibilityPhone";
            this.lblStaticEligibilityPhone.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticEligibilityPhone.TabIndex = 0;
            this.lblStaticEligibilityPhone.Text = "Eligibility phone:";
            // 
            // lblStaticInfoRecvFrom
            // 
            this.lblStaticInfoRecvFrom.Location = new System.Drawing.Point( 8, 17 );
            this.lblStaticInfoRecvFrom.Name = "lblStaticInfoRecvFrom";
            this.lblStaticInfoRecvFrom.Size = new System.Drawing.Size( 136, 23 );
            this.lblStaticInfoRecvFrom.TabIndex = 0;
            this.lblStaticInfoRecvFrom.Text = "Information received from:";
            // 
            // lblStaticPreCondition
            // 
            this.lblStaticPreCondition.Location = new System.Drawing.Point( 8, 320 );
            this.lblStaticPreCondition.Name = "lblStaticPreCondition";
            this.lblStaticPreCondition.Size = new System.Drawing.Size( 180, 23 );
            this.lblStaticPreCondition.TabIndex = 0;
            this.lblStaticPreCondition.Text = "Service is for preexisting condition:";
            // 
            // cmbCoveredBenefit
            // 
            this.cmbCoveredBenefit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoveredBenefit.Location = new System.Drawing.Point( 181, 351 );
            this.cmbCoveredBenefit.Name = "cmbCoveredBenefit";
            this.cmbCoveredBenefit.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoveredBenefit.TabIndex = 24;
            this.cmbCoveredBenefit.SelectedIndexChanged += new System.EventHandler( this.cmbCoveredBenefit_SelectedIndexChanged );
            // 
            // lblStaticCoveredBenefit
            // 
            this.lblStaticCoveredBenefit.Location = new System.Drawing.Point( 8, 354 );
            this.lblStaticCoveredBenefit.Name = "lblStaticCoveredBenefit";
            this.lblStaticCoveredBenefit.Size = new System.Drawing.Size( 150, 23 );
            this.lblStaticCoveredBenefit.TabIndex = 0;
            this.lblStaticCoveredBenefit.Text = "Service is a covered benefit:";
            // 
            // lblStaticClaimsVerified
            // 
            this.lblStaticClaimsVerified.Location = new System.Drawing.Point( 8, 388 );
            this.lblStaticClaimsVerified.Name = "lblStaticClaimsVerified";
            this.lblStaticClaimsVerified.Size = new System.Drawing.Size( 136, 23 );
            this.lblStaticClaimsVerified.TabIndex = 0;
            this.lblStaticClaimsVerified.Text = "Claims address verified:";
            // 
            // cmbAddressVerified
            // 
            this.cmbAddressVerified.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddressVerified.Location = new System.Drawing.Point( 181, 385 );
            this.cmbAddressVerified.Name = "cmbAddressVerified";
            this.cmbAddressVerified.Size = new System.Drawing.Size( 45, 21 );
            this.cmbAddressVerified.TabIndex = 25;
            this.cmbAddressVerified.SelectedIndexChanged += new System.EventHandler( this.cmbAddressVerified_SelectedIndexChanged );
            // 
            // lblStaticProduct
            // 
            this.lblStaticProduct.Location = new System.Drawing.Point( 350, 320 );
            this.lblStaticProduct.Name = "lblStaticProduct";
            this.lblStaticProduct.Size = new System.Drawing.Size( 96, 23 );
            this.lblStaticProduct.TabIndex = 0;
            this.lblStaticProduct.Text = "Type of product:";
            // 
            // cmbTypeOfProduct
            // 
            this.cmbTypeOfProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeOfProduct.Location = new System.Drawing.Point( 552, 317 );
            this.cmbTypeOfProduct.Name = "cmbTypeOfProduct";
            this.cmbTypeOfProduct.Size = new System.Drawing.Size( 150, 21 );
            this.cmbTypeOfProduct.TabIndex = 28;
            this.cmbTypeOfProduct.SelectedIndexChanged += new System.EventHandler( this.cmbTypeOfProduct_SelectedIndexChanged );
            // 
            // lblStaticPPO
            // 
            this.lblStaticPPO.Location = new System.Drawing.Point( 350, 354 );
            this.lblStaticPPO.Name = "lblStaticPPO";
            this.lblStaticPPO.Size = new System.Drawing.Size( 208, 23 );
            this.lblStaticPPO.TabIndex = 0;
            this.lblStaticPPO.Text = "PPO network, pricing network, or broker:";
            // 
            // lblStaticFacilityProvider
            // 
            this.lblStaticFacilityProvider.Location = new System.Drawing.Point( 350, 388 );
            this.lblStaticFacilityProvider.Name = "lblStaticFacilityProvider";
            this.lblStaticFacilityProvider.Size = new System.Drawing.Size( 165, 23 );
            this.lblStaticFacilityProvider.TabIndex = 0;
            this.lblStaticFacilityProvider.Text = "Facility is a contracted provider:";
            // 
            // cmbFacilityIsProvider
            // 
            this.cmbFacilityIsProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFacilityIsProvider.Location = new System.Drawing.Point( 552, 385 );
            this.cmbFacilityIsProvider.Name = "cmbFacilityIsProvider";
            this.cmbFacilityIsProvider.Size = new System.Drawing.Size( 50, 21 );
            this.cmbFacilityIsProvider.TabIndex = 30;
            this.cmbFacilityIsProvider.SelectedIndexChanged += new System.EventHandler( this.cmbFacilityIsProvider_SelectedIndexChanged );
            // 
            // lblStaticMedpayCoverage
            // 
            this.lblStaticMedpayCoverage.Location = new System.Drawing.Point( 350, 456 );
            this.lblStaticMedpayCoverage.Name = "lblStaticMedpayCoverage";
            this.lblStaticMedpayCoverage.Size = new System.Drawing.Size( 128, 23 );
            this.lblStaticMedpayCoverage.TabIndex = 0;
            this.lblStaticMedpayCoverage.Text = "Auto medpay coverage:";
            // 
            // mtbAutoInsuranceClaimNumber
            // 
            this.mtbAutoInsuranceClaimNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbAutoInsuranceClaimNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAutoInsuranceClaimNumber.KeyPressExpression = "^[a-zA-Z0-9]*$";
            this.mtbAutoInsuranceClaimNumber.Location = new System.Drawing.Point( 552, 419 );
            this.mtbAutoInsuranceClaimNumber.Mask = "";
            this.mtbAutoInsuranceClaimNumber.MaxLength = 15;
            this.mtbAutoInsuranceClaimNumber.Name = "mtbAutoInsuranceClaimNumber";
            this.mtbAutoInsuranceClaimNumber.Size = new System.Drawing.Size( 100, 20 );
            this.mtbAutoInsuranceClaimNumber.TabIndex = 31;
            this.mtbAutoInsuranceClaimNumber.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbAutoInsuranceClaimNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAutoInsuranceClaimNumber_Validating );
            // 
            // lblStaticClaimNumber
            // 
            this.lblStaticClaimNumber.Location = new System.Drawing.Point( 350, 422 );
            this.lblStaticClaimNumber.Name = "lblStaticClaimNumber";
            this.lblStaticClaimNumber.Size = new System.Drawing.Size( 157, 23 );
            this.lblStaticClaimNumber.TabIndex = 0;
            this.lblStaticClaimNumber.Text = "Auto insurance claim number:";
            // 
            // lblStaticRule
            // 
            this.lblStaticRule.Location = new System.Drawing.Point( 181, 456 );
            this.lblStaticRule.Name = "lblStaticRule";
            this.lblStaticRule.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticRule.TabIndex = 0;
            this.lblStaticRule.Text = "Rule:";
            // 
            // cmbCoordinationOfBenefits
            // 
            this.cmbCoordinationOfBenefits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoordinationOfBenefits.Location = new System.Drawing.Point( 181, 419 );
            this.cmbCoordinationOfBenefits.Name = "cmbCoordinationOfBenefits";
            this.cmbCoordinationOfBenefits.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoordinationOfBenefits.TabIndex = 26;
            this.cmbCoordinationOfBenefits.SelectedIndexChanged += new System.EventHandler( this.cmbCoordinationOfBenefits_SelectedIndexChanged );
            // 
            // lblStaticCoordBenefits
            // 
            this.lblStaticCoordBenefits.Location = new System.Drawing.Point( 8, 422 );
            this.lblStaticCoordBenefits.Name = "lblStaticCoordBenefits";
            this.lblStaticCoordBenefits.Size = new System.Drawing.Size( 144, 23 );
            this.lblStaticCoordBenefits.TabIndex = 0;
            this.lblStaticCoordBenefits.Text = "Coordination of benefits:";
            // 
            // lineLabelBenefits2
            // 
            this.lineLabelBenefits2.Caption = "";
            this.lineLabelBenefits2.Location = new System.Drawing.Point( 184, 172 );
            this.lineLabelBenefits2.Name = "lineLabelBenefits2";
            this.lineLabelBenefits2.Size = new System.Drawing.Size( 664, 13 );
            this.lineLabelBenefits2.TabIndex = 0;
            this.lineLabelBenefits2.TabStop = false;
            // 
            // btnDumpResponse
            // 
            this.btnDumpResponse.Location = new System.Drawing.Point( 11, 103 );
            this.btnDumpResponse.Name = "btnDumpResponse";
            this.btnDumpResponse.Size = new System.Drawing.Size( 141, 23 );
            this.btnDumpResponse.TabIndex = 100;
            this.btnDumpResponse.Text = "DumpResponse";
            this.btnDumpResponse.UseVisualStyleBackColor = true;
            this.btnDumpResponse.Click += new System.EventHandler( this.btnDumpResponse_Click );
            // 
            // CommMgdCareVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelMain );
            this.Name = "CommMgdCareVerifyView";
            this.Size = new System.Drawing.Size( 850, 912 );
            this.Disposed += new System.EventHandler( this.CommMgdCareVerifyView_Disposed );
            this.panelMain.ResumeLayout( false );
            this.panelMain.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Construction and Finalization
        public CommMgdCareVerifyView()
        {
            InitializeComponent();

            ConfigureControls();

            loadingModelData = true;
            base.EnableThemesOn( this );

            blankTimePeriodFlag = new TimePeriodFlag();
            blankTimePeriodFlag.SetBlank();
            yearTimePeriodFlag = new TimePeriodFlag();
            yearTimePeriodFlag.SetYear();
            visitTimePeriodFlag = new TimePeriodFlag();
            visitTimePeriodFlag.SetVisit();

            blankYesNoFlag = new YesNoFlag();
            blankYesNoFlag.SetBlank();
            yesYesNoFlag = new YesNoFlag();
            yesYesNoFlag.SetYes();
            noYesNoFlag = new YesNoFlag();
            noYesNoFlag.SetNo();

            User aUser = User.GetCurrent();

            if( ( aUser.LastName.ToUpper() == "PEREZ" && aUser.FirstName.ToUpper() == "SUSAN" )
                || ( aUser.LastName.ToUpper() == "GAUTHREAUX" && aUser.FirstName.ToUpper() == "TOM" )
                || ( aUser.FirstName.ToUpper() == "PATIENTACCESS" ) )
            {
                this.btnDumpResponse.Visible = true;
            }
            else
            {
                this.btnDumpResponse.Visible = false;
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
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

        private LoggingButton btnClearAll;

        private ColumnHeader columnHeader1;

        private ComboBox cmbAddressVerified;
        private ComboBox cmbAutoMedpayCoverage;
        private ComboBox cmbCoordinationOfBenefits;
        private ComboBox cmbCoPayWaiveIfAdmitted;
        private ComboBox cmbDeductibleMet;
        private ComboBox cmbFacilityIsProvider;
        private ComboBox cmbInfoRecvFrom;
        private ComboBox cmbMaxBenefitPerVisitMet;
        private ComboBox cmbLifetimeMaxBenefitMet;
        private ComboBox cmbTimePeriod;
        private ComboBox cmbTypeOfProduct;
        private ComboBox cmbRule;
        private ComboBox cmbPreexistingCondition;
        private ComboBox cmbCoveredBenefit;
        private ComboBox cmbOutOfPocketMet;

        private DateTimePicker dtpTerminationDate;
        private DateTimePicker dtpEffectiveDate;

        private Label lblStaticInfoRecvFrom;
        private Label lblStaticEligibilityPhone;
        private Label lblStaticInsRepName;
        private Label lblStaticEffectiveDate;
        private Label lblStaticTermDate;
        private Label lblStaticMaxBenefitMet;
        private Label lblStaticMaxBenefit;
        private Label lblStaticVisitsPerYear;
        private Label lblStaticWaive;
        private Label lblStaticCoPay;
        private Label lblStaticPercent1;
        private Label lblStaticPercent2;
        private Label lblStaticAfterOutOfPocket;
        private Label lblStaticOutOfPocketDollarsMet;
        private Label lblStaticPaymentMet;
        private Label lblStaticMet;
        private Label lblStaticOutOfPocket;
        private Label lblStaticCoInsurance;
        private Label lblStaticDollarsMet;
        private Label lblStaticTime;
        private Label lblStaticDeductible;
        private Label lblStaticListLabel;
        private Label lblStaticMedpayCoverage;
        private Label lblStaticFacilityProvider;
        private Label lblStaticPPO;
        private Label lblStaticProduct;
        private Label lblStaticClaimNumber;
        private Label lblStaticRule;
        private Label lblStaticCoordBenefits;
        private Label lblStaticCoveredBenefit;
        private Label lblStaticPreCondition;
        private Label lblStaticRemarks;
        private Label lblStaticClaimsVerified;
        private Label lblStaticMaxBenefitVisit;
        private Label lblStaticMaxBenefitVisitMet;
        private Label label1;
        private Label lblDollar1;
        private Label lblRemainingBenefitPerVisit;
        private Label lblRemainingLifetimeValue;

        private ListView lvBenefitsCategories;

        private Panel panelMain;

        private MaskedEditTextBox mtbRemarks;

        private MaskedEditTextBox mtbEligibilityPhone;
        private MaskedEditTextBox mtbInsRepName;
        private MaskedEditTextBox mtbEffectiveDate;
        private MaskedEditTextBox mtbTerminationDate;
        private MaskedEditTextBox mtbMaxBenefitAmount;
        private MaskedEditTextBox mtbNumberVisitsPerYear;
        private MaskedEditTextBox mtbCoPayAmount;
        private MaskedEditTextBox mtbDeductible;
        private MaskedEditTextBox mtbDeductibleDollarsMet;
        private MaskedEditTextBox mtbPercentOutOfPocket;
        private MaskedEditTextBox mtbOutOfPocketDollarsMet;
        private MaskedEditTextBox mtbOutOfPocketAmount;
        private MaskedEditTextBox mtbCoInsuranceAmount;
        private MaskedEditTextBox mtbPPOProvider;
        private MaskedEditTextBox mtbMaxBenefitPerVisit;
        private MaskedEditTextBox mtbAutoInsuranceClaimNumber;
        private MaskedEditTextBox mtbRemainingLifetimeValue;
        private MaskedEditTextBox mtbRemainingBenefitPerVisit;

        private LineLabel lineLabelBenefits1;
        private LineLabel lineLabelBenefits2;
        private LineLabel lineLabelBenefits3;
        private LineLabel lineLabel1;
        private LineLabel lineLabel2;
        private LineLabel lineLabel3;
        private LineLabel lineLabel4;
        private LineLabel lineLabel5;
        private LineLabel lblVertLineLabel;

        private NameAndPhoneView nameAndPhoneView1;
        private NameAndPhoneView nameAndPhoneView2;

        private AddressView insuranceAgentAddressView;
        private AddressView attorneyAddressView;

        private Account i_Account;
        private BenefitsCategory benefitsCategory;
        private bool loadingModelData;
        private decimal deductible;
        private double remainingLifetimeValue;
        private double remainingBenefitsPerVisit;
        private int insuranceMonth;
        private int insuranceDay;
        private int insuranceYear;
        private string currentBenefitsIndex;
        private TimePeriodFlag blankTimePeriodFlag;
        private TimePeriodFlag yearTimePeriodFlag;
        private TimePeriodFlag visitTimePeriodFlag;
        private YesNoFlag blankYesNoFlag;
        private YesNoFlag yesYesNoFlag;
        private YesNoFlag noYesNoFlag;
        private RuleEngine i_RuleEngine;

        private Hashtable categoryTable = new Hashtable( 11 );

        #endregion
        private Button btnDumpResponse;

        #region Constants

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( CommMgdCareVerifyView ) );

        #endregion

        private void btnDumpResponse_Click( object sender, EventArgs e )
        {
            DebugBenefitsResponse win = new DebugBenefitsResponse();
            win.Model_Coverage = this.Model_Coverage;
            win.UpdateView();
            win.ShowDialog();
        }
    }
}
