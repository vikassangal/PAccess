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
    /// Summary description for MedicareVerifyView.
    /// </summary>
    public class MedicareVerifyView : ControlView
    {
        #region Event Handlers
        private void MedicareVerifyView_Disposed( object sender, EventArgs e )
        {
            UnRegisterEvents();
        }

        private void MedicarePartACoveragePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbPartACoverage );
        }


        private void MedicarePartBCoveragePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbPartBCoverage );
        }


        private void MedicareHasHMOCoveragePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbHMOCoverage );
        }

        private void MedicareIsSecondaryPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbMedicareIsSecondary );
        }

        private void MedicareDaysRemainingBenefitPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbDaysRemainBenefitPeriod );
        }


        private void MedicareDaysRemainingCoInsPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbDaysRemainCoInsurance );
        }


        private void MedicareDaysRemainLifeServePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( mtbDaysRemainLifetimeReserve );
        }

        private void MedicareHospiceProgramPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbPatientIsHospiceProgram );
        }

        private void MedicareVerifiedBeneficiaryNamePreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbVerifiedBeneficiaryName );
        }

        private void MedicareInfoRecvdFromPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( cmbInfoRecvFrom );
        }
        /// <summary>
        /// private handler for rule: MedicarePatientHasHMOEventHandler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MedicarePatientHasHMOEventHandler( object sender, EventArgs e )
        {

            MessageBox.Show( UIErrorMessages.MEDICARE_WITH_MEDICARE_HMO,
                            "Warning",
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                             MessageBoxDefaultButton.Button1 );

        }

        private void btnClearAll_Click( object sender, EventArgs e )
        {
            ResetView();
        }

        private void cmbPartACoverage_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbPartACoverage_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
            if (cb.SelectedIndex != -1)
            {
                Model_Coverage.PartACoverage = cb.SelectedItem as YesNoFlag;
            }

            if (cb.SelectedItem == null ||
                Model_Coverage.PartACoverage.Code.Equals( " " ) ||
                Model_Coverage.PartACoverage.Code.Equals( "N" ))
            {
                mtbPartAEffectiveDate.UnMaskedText = string.Empty;
            }

            if (!loadingModelData)
            {
                RuleEngine.OneShotRuleEvaluation<MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage>(
                    this.Model_Coverage.Account, MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler );
        
            }

            CheckForRequiredFields();
        }

        private void MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverageHandler( object sender, EventArgs e )
        {
            var MessageDisplayHandler = new ErrorMessageDisplayHandler( this.Model_Coverage.Account );
            MessageDisplayHandler.DisplayOkWarningMessageFor( typeof( MedicareCannotBePrimaryPayorForInPatientWithoutPartACoverage ) );

        }

        private void dtpPartAEffectiveDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbPartAEffectiveDate );
            DateTime dt = dtp.Value;
            mtbPartAEffectiveDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbPartAEffectiveDate.Focus();
        }

        private void mtbPartAEffectiveDate_Enter( object sender, EventArgs e )
        {
            Refresh();
        }

        private void mtbPartAEffectiveDate_Validating( object sender, CancelEventArgs e )
        {
            if (this.dtpPartAEffectiveDate.Focused)
            {
                mtbPartAEffectiveDate.Focus();
                return;
            }

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            if (mtb.UnMaskedText != String.Empty)
            {
                if (InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ))
                {
                    UIColors.SetNormalBgColor( mtb );
                    part_A_EffectiveDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                part_A_EffectiveDate = DateTime.MinValue;
            }
            Model_Coverage.PartACoverageEffectiveDate = part_A_EffectiveDate;
        }

        private void cmbPartBCoverage_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbPartBCoverage_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
            if (cb.SelectedIndex != -1)
            {
                Model_Coverage.PartBCoverage = cb.SelectedItem as YesNoFlag;
            }
            else
            {
                UIColors.SetPreferredBgColor( cb );
            }

            if (cb.SelectedItem == null ||
                Model_Coverage.PartBCoverage.Code == " " ||
                Model_Coverage.PartBCoverage.Code == "N")
            {
                mtbPartBEffectiveDate.UnMaskedText = string.Empty;
            }


            CheckForRequiredFields();
        }

        private void dtpPartBEffectiveDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbPartBEffectiveDate );
            DateTime dt = dtp.Value;
            mtbPartBEffectiveDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbPartBEffectiveDate.Focus();
        }

        private void mtbPartBEffectiveDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            //UIColors.SetNormalBgColor( mtb );
            Refresh();
        }


        private void mtbPartBEffectiveDate_Validating( object sender, CancelEventArgs e )
        {
            if (this.dtpPartBEffectiveDate.Focused)
            {
                mtbPartBEffectiveDate.Focus();
                return;
            }

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            if (mtb.UnMaskedText != String.Empty)
            {
                if (InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ))
                {
                    UIColors.SetNormalBgColor( mtb );
                    part_B_EffectiveDate = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                part_B_EffectiveDate = DateTime.MinValue;
            }
            Model_Coverage.PartBCoverageEffectiveDate = part_B_EffectiveDate;
        }

        private void cmbHMOCoverage_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbHMOCoverage_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );

            if (cb.SelectedIndex != -1 && !cb.SelectedItem.Equals( Model_Coverage.PatientHasMedicareHMOCoverage ))
            {
                patientHasMedicareHMOCoverage = cb.SelectedItem as YesNoFlag;
                Model_Coverage.PatientHasMedicareHMOCoverage = cb.SelectedItem as YesNoFlag;
                this.ApplyMedicarePatientHasHMORule();
            }

            CheckForRequiredFields();
        }

        private void ApplyMedicarePatientHasHMORule()
        {


            try
            {

                this.RuleEngine.RegisterEvent<MedicarePatientHasHMO>(
                    this.MedicarePatientHasHMOEventHandler );

                this.RuleEngine.EvaluateRule<MedicarePatientHasHMO>( this.Account.Insurance );

            }
            finally
            {

                this.RuleEngine.UnregisterEvent<MedicarePatientHasHMO>(
                    this.MedicarePatientHasHMOEventHandler );

            }

        }


        private void cmbMedicareIsSecondary_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbMedicareIsSecondary_SelectedIndexChanged( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
            if (cb.SelectedIndex != -1)
            {
                medicareIsSecondary = cb.SelectedItem as YesNoFlag;

                if (medicareIsSecondary.Code.Equals( "Y" ))
                {
                    if (Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID &&
                        Model_Coverage.InsurancePlan.GetType() == typeof( GovernmentMedicareInsurancePlan ))
                    {
                        MessageBox.Show( UIErrorMessages.MEDICARE_SECONDARY_BUT_PRIMARY, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                    }
                }
                Model_Coverage.MedicareIsSecondary = medicareIsSecondary;
            }

            CheckForRequiredFields();
        }

        private void dtpBillingActivityDate_CloseUp( object sender, EventArgs e )
        {
            DateTimePicker dtp = sender as DateTimePicker;
            UIColors.SetNormalBgColor( mtbBillingActivityDate );
            DateTime dt = dtp.Value;
            mtbBillingActivityDate.UnMaskedText = String.Format( "{0:D2}{1:D2}{2:D4}", dt.Month, dt.Day, dt.Year );
            this.mtbBillingActivityDate.Focus();
        }

        private void mtbBillingActivityDate_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            Refresh();
        }

        private void mtbBillingActivityDate_Validating( object sender, CancelEventArgs e )
        {
            if (this.dtpBillingActivityDate.Focused)
            {
                this.mtbBillingActivityDate.Focus();
                return;
            }

            UIColors.SetNormalBgColor( this.mtbBillingActivityDate );

            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText != String.Empty)
            {
                if (InsuranceDateVerify.VerifyInsuranceDate( ref mtb, ref insuranceYear, ref insuranceMonth, ref insuranceDay ))
                {
                    dateOfLastBillingActivity = new DateTime( insuranceYear, insuranceMonth, insuranceDay );
                }
            }
            else
            {
                dateOfLastBillingActivity = DateTime.MinValue;
            }
        }

        private void mtbDaysRemainBenefitPeriod_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }

        private void mtbDaysRemainBenefitPeriod_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if (mtb.UnMaskedText != String.Empty)
            {
                remainingBenefitPeriod = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                remainingBenefitPeriod = -1;
            }
            Model_Coverage.RemainingBenefitPeriod = remainingBenefitPeriod;
            CheckForRequiredFields();
        }

        private void mtbDaysRemainCoInsurance_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }

        private void mtbDaysRemainCoInsurance_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );

            if (mtb.UnMaskedText != String.Empty)
            {
                remainingCoInsurance = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                remainingCoInsurance = -1;
            }
            Model_Coverage.RemainingCoInsurance = remainingCoInsurance;
            CheckForRequiredFields();
        }

        private void mtbDaysRemainLifetimeReserve_Enter( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
        }


        private void mtbDaysRemainLifetimeReserve_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            UIColors.SetNormalBgColor( mtb );
            if (mtb.UnMaskedText != String.Empty)
            {
                remainingLifeTimeReserve = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                remainingLifeTimeReserve = -1;
            }
            Model_Coverage.RemainingLifeTimeReserve = remainingLifeTimeReserve;
            CheckForRequiredFields();
        }

        private void mtbDaysRemainSNF_Leave( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText != String.Empty)
            {
                remainingSNF = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                remainingSNF = -1;
            }
        }

        private void mtbDaysRemainSnfCoInsurance_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText != String.Empty)
            {
                remainingSNFCoInsurance = Convert.ToInt32( mtb.UnMaskedText );
            }
            else
            {
                remainingSNFCoInsurance = -1;
            }
        }

        private void cmbPatientIsHospiceProgram_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbPatientIsHospiceProgram_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbPatientIsHospiceProgram );

            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex != -1)
            {
                Model_Coverage.PatientIsPartOfHospiceProgram = cb.SelectedItem as YesNoFlag;
            }

            CheckForRequiredFields();
        }

        private void cmbVerifiedBeneficiaryName_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbVerifiedBeneficiaryName_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbVerifiedBeneficiaryName );
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex != -1)
            {
                Model_Coverage.VerifiedBeneficiaryName = cb.SelectedItem as YesNoFlag;
            }

            CheckForRequiredFields();
        }

        private void cmbInfoRecvFrom_DropDown( object sender, EventArgs e )
        {
            ComboBox cb = sender as ComboBox;
            UIColors.SetNormalBgColor( cb );
        }

        private void cmbInfoReceivedFrom_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbInfoRecvFrom );
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex != -1 && cb.Text.Equals( " " ) == false)
            {
                informationReceivedSource = cb.SelectedItem as InformationReceivedSource;
                Model_Coverage.InformationReceivedSource = informationReceivedSource;
            }

            CheckForRequiredFields();
        }

        private void mtbRemarks_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb.UnMaskedText != String.Empty)
            {
                remarks = mtb.UnMaskedText;
            }
            else
            {
                remarks = String.Empty;
            }
        }

        private void mtbRemainingPartADeductible_Enter( object sender, EventArgs e )
        {
            if (this.mtbRemainingPartADeductible.UnMaskedText != string.Empty)
            {
                decimal remPartADeductible = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbRemainingPartADeductible );
                this.mtbRemainingPartADeductible.UnMaskedText = remPartADeductible.ToString();
            }
        }

        private void mtbRemainingPartADeductible_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb != null && mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != ".")
            {
                remainingPartADeductible = Convert.ToDecimal( mtb.UnMaskedText );
                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
            else
            {
                remainingPartADeductible = -1;
            }

        }

        private void mtbRemainingPartBDeductible_Enter( object sender, EventArgs e )
        {
            if (this.mtbRemainingPartBDeductible.UnMaskedText != string.Empty)
            {
                decimal remPartBDeductible = CommonFormatting.ConvertTextToCurrencyDecimal( this.mtbRemainingPartBDeductible );
                this.mtbRemainingPartBDeductible.UnMaskedText = remPartBDeductible.ToString();
            }
        }

        private void mtbRemainingPartBDeductible_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            if (mtb != null && mtb.UnMaskedText.Trim() != String.Empty && mtb.UnMaskedText.Trim() != ".")
            {
                remainingPartBDeductible = Convert.ToDecimal( mtb.UnMaskedText );
                CommonFormatting.FormatTextBoxCurrency( mtb, "###,###,##0.00" );
            }
            else
            {
                remainingPartBDeductible = -1;
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if (loadingModelData)
            {
                PopulatePartACoverageComboBox();
                PopulatePartBCoverageComboBox();
                PopulateInfoRecvComboBox();
                PopulatePatientHasMedicareComboBox();
                PopulateMedicareIsSecondaryComboBox();
                PopulateHospiceProgramComboBox();
                PopulateBeneficiaryNameComboBox();
            }

            // populate the view!

            this.cmbPartACoverage.SelectedItem = Model_Coverage.PartACoverage;
            this.partACoverage = Model_Coverage.PartACoverage;

            this.cmbPartBCoverage.SelectedItem = Model_Coverage.PartBCoverage;
            this.partBCoverage = Model_Coverage.PartBCoverage;

            this.mtbPartAEffectiveDate.UnMaskedText = string.Empty;
            this.part_A_EffectiveDate = Model_Coverage.PartACoverageEffectiveDate;
            if (Model_Coverage.PartACoverageEffectiveDate != DateTime.MinValue)
            {
                this.mtbPartAEffectiveDate.Text = String.Format( "{0:D2}/{1:D2}/{2:D4}",
                    Model_Coverage.PartACoverageEffectiveDate.Month,
                    Model_Coverage.PartACoverageEffectiveDate.Day,
                    Model_Coverage.PartACoverageEffectiveDate.Year );
            }

            this.mtbPartBEffectiveDate.UnMaskedText = string.Empty;
            this.part_B_EffectiveDate = Model_Coverage.PartBCoverageEffectiveDate;
            if (Model_Coverage.PartBCoverageEffectiveDate != DateTime.MinValue)
            {
                this.mtbPartBEffectiveDate.Text = String.Format( "{0:D2}/{1:D2}/{2:D4}",
                    Model_Coverage.PartBCoverageEffectiveDate.Month,
                    Model_Coverage.PartBCoverageEffectiveDate.Day,
                    Model_Coverage.PartBCoverageEffectiveDate.Year );
            }

            this.cmbHMOCoverage.SelectedItem = Model_Coverage.PatientHasMedicareHMOCoverage;
            this.patientHasMedicareHMOCoverage = Model_Coverage.PatientHasMedicareHMOCoverage;

            this.cmbMedicareIsSecondary.SelectedItem = Model_Coverage.MedicareIsSecondary;
            this.medicareIsSecondary = Model_Coverage.MedicareIsSecondary;

            this.cmbPatientIsHospiceProgram.SelectedItem = Model_Coverage.PatientIsPartOfHospiceProgram;
            this.patientIsPartOfHospiceProgram = Model_Coverage.PatientIsPartOfHospiceProgram;

            this.mtbBillingActivityDate.UnMaskedText = string.Empty;

            this.dateOfLastBillingActivity = Model_Coverage.DateOfLastBillingActivity;

            if (Model_Coverage.DateOfLastBillingActivity != DateTime.MinValue)
            {
                this.mtbBillingActivityDate.UnMaskedText = Model_Coverage.DateOfLastBillingActivity.ToString( "MMddyyyy" );
            }

            if (this.Model_Coverage.RemainingBenefitPeriod >= 0)
            {
                this.remainingBenefitPeriod = this.Model_Coverage.RemainingBenefitPeriod;
                this.mtbDaysRemainBenefitPeriod.UnMaskedText = this.remainingBenefitPeriod.ToString();
            }
            else
            {
                this.mtbDaysRemainBenefitPeriod.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingCoInsurance >= 0)
            {
                this.remainingCoInsurance = this.Model_Coverage.RemainingCoInsurance;
                this.mtbDaysRemainCoInsurance.UnMaskedText = this.remainingCoInsurance.ToString();
            }
            else
            {
                this.mtbDaysRemainCoInsurance.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingLifeTimeReserve >= 0)
            {
                this.remainingLifeTimeReserve = this.Model_Coverage.RemainingLifeTimeReserve;
                this.mtbDaysRemainLifetimeReserve.UnMaskedText = this.remainingLifeTimeReserve.ToString();
            }
            else
            {
                this.mtbDaysRemainLifetimeReserve.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingSNF >= 0)
            {
                this.remainingSNF = this.Model_Coverage.RemainingSNF;
                this.mtbDaysRemainSNF.UnMaskedText = this.remainingSNF.ToString();
            }
            else
            {
                this.mtbDaysRemainSNF.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingSNFCoInsurance >= 0)
            {
                this.remainingSNFCoInsurance = this.Model_Coverage.RemainingSNFCoInsurance;
                this.mtbDaysRemainSnfCoInsurance.UnMaskedText = this.remainingSNFCoInsurance.ToString();
            }
            else
            {
                this.mtbDaysRemainSnfCoInsurance.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingPartADeductible >= 0)
            {
                mtbRemainingPartADeductible.Text = this.Model_Coverage.RemainingPartADeductible.ToString( "###,###,##0.00" );
            }
            else
            {
                mtbRemainingPartADeductible.UnMaskedText = string.Empty;
            }

            if (this.Model_Coverage.RemainingPartBDeductible >= 0)
            {
                mtbRemainingPartBDeductible.Text = this.Model_Coverage.RemainingPartBDeductible.ToString( "###,###,##0.00" );
            }
            else
            {
                mtbRemainingPartBDeductible.UnMaskedText = string.Empty;
            }

            this.verifiedBeneficiaryName = Model_Coverage.VerifiedBeneficiaryName;
            this.cmbVerifiedBeneficiaryName.SelectedItem = Model_Coverage.VerifiedBeneficiaryName;

            this.cmbInfoRecvFrom.SelectedItem = Model_Coverage.InformationReceivedSource;
            this.informationReceivedSource = Model_Coverage.InformationReceivedSource;

            this.mtbRemarks.UnMaskedText
                = ( Model_Coverage.Remarks == null ) ? string.Empty : Model_Coverage.Remarks.TrimEnd();
            this.remarks = Model_Coverage.Remarks;

            //RuleEngine.LoadRules( Account );

            RuleEngine.GetInstance().RegisterEvent( typeof( MedicarePartACoveragePreferred ), Model_Coverage, new EventHandler( MedicarePartACoveragePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicarePartBCoveragePreferred ), Model_Coverage, new EventHandler( MedicarePartBCoveragePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareHasHMOCoveragePreferred ), Model_Coverage, new EventHandler( MedicareHasHMOCoveragePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareIsSecondaryPreferred ), Model_Coverage, new EventHandler( MedicareIsSecondaryPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareDaysRemainingBenefitPreferred ), Model_Coverage, new EventHandler( MedicareDaysRemainingBenefitPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareDaysRemainingCoInsPreferred ), Model_Coverage, new EventHandler( MedicareDaysRemainingCoInsPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareDaysRemainLifeServePreferred ), Model_Coverage, new EventHandler( MedicareDaysRemainLifeServePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareHospiceProgramPreferred ), Model_Coverage, new EventHandler( MedicareHospiceProgramPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareVerifiedBeneficiaryNamePreferred ), Model_Coverage, new EventHandler( MedicareVerifiedBeneficiaryNamePreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( MedicareInfoRecvdFromPreferred ), Model_Coverage, new EventHandler( MedicareInfoRecvdFromPreferredEventHandler ) );

            CheckForRequiredFields();

            bool result = true;
            int year = 0;
            int month = 0;
            int day = 0;

            if (mtbPartAEffectiveDate.UnMaskedText != String.Empty)
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbPartAEffectiveDate, ref year, ref month, ref day );
            }
            if (result && mtbPartBEffectiveDate.UnMaskedText != String.Empty)
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbPartBEffectiveDate, ref year, ref month, ref day );
            }
            if (result && mtbBillingActivityDate.UnMaskedText != String.Empty)
            {
                result = InsuranceDateVerify.VerifyInsuranceDate( ref mtbBillingActivityDate, ref year, ref month, ref day );
            }

            loadingModelData = false;
        }

        public override void UpdateModel()
        {
            if (this.cmbPartACoverage.SelectedItem != null)
                Model_Coverage.PartACoverage = this.cmbPartACoverage.SelectedItem as YesNoFlag;

            Model_Coverage.PartACoverageEffectiveDate = part_A_EffectiveDate;

            if (this.cmbPartBCoverage.SelectedItem != null)
                Model_Coverage.PartBCoverage = this.cmbPartACoverage.SelectedItem as YesNoFlag;

            Model_Coverage.PartBCoverageEffectiveDate = part_B_EffectiveDate;

            if (this.cmbHMOCoverage.SelectedItem != null)
                Model_Coverage.PatientHasMedicareHMOCoverage = this.cmbHMOCoverage.SelectedItem as YesNoFlag;

            if (this.cmbMedicareIsSecondary.SelectedItem != null)
                Model_Coverage.MedicareIsSecondary = this.cmbMedicareIsSecondary.SelectedItem as YesNoFlag;

            Model_Coverage.DateOfLastBillingActivity = dateOfLastBillingActivity;

            this.mtbDaysRemainBenefitPeriod_Validating( this.mtbDaysRemainBenefitPeriod, null );
            Model_Coverage.RemainingBenefitPeriod = remainingBenefitPeriod;

            this.mtbDaysRemainCoInsurance_Validating( this.mtbDaysRemainCoInsurance, null );
            Model_Coverage.RemainingCoInsurance = remainingCoInsurance;

            this.mtbDaysRemainLifetimeReserve_Validating( this.mtbDaysRemainLifetimeReserve, null );
            Model_Coverage.RemainingLifeTimeReserve = remainingLifeTimeReserve;

            if (!string.IsNullOrEmpty( this.mtbDaysRemainSNF.UnMaskedText ))
                Model_Coverage.RemainingSNF = Convert.ToInt32( this.mtbDaysRemainSNF.UnMaskedText );

            this.mtbDaysRemainSnfCoInsurance_Validating( this.mtbDaysRemainSnfCoInsurance, null );
            Model_Coverage.RemainingSNFCoInsurance = remainingSNFCoInsurance;

            if (this.cmbPatientIsHospiceProgram.SelectedItem != null)
                Model_Coverage.PatientIsPartOfHospiceProgram = this.cmbPatientIsHospiceProgram.SelectedItem as YesNoFlag;

            if (this.cmbVerifiedBeneficiaryName.SelectedItem != null)
                Model_Coverage.VerifiedBeneficiaryName = this.cmbVerifiedBeneficiaryName.SelectedItem as YesNoFlag;

            if (this.cmbInfoRecvFrom.SelectedItem != null)
                Model_Coverage.InformationReceivedSource = this.cmbInfoRecvFrom.SelectedItem as InformationReceivedSource;

            this.mtbRemainingPartADeductible_Validating( this.mtbRemainingPartADeductible, null );
            Model_Coverage.RemainingPartADeductible = (float)remainingPartADeductible;

            this.mtbRemainingPartBDeductible_Validating( this.mtbRemainingPartBDeductible, null );
            Model_Coverage.RemainingPartBDeductible = (float)remainingPartBDeductible;

            this.mtbRemarks_Validating( this.mtbRemarks, null );
            Model_Coverage.Remarks = remarks;

        }
        #endregion

        #region Properties
        [Browsable( false )]
        public GovernmentMedicareCoverage Model_Coverage
        {
            set
            {
                this.Model = value;
            }
            private get
            {
                return (GovernmentMedicareCoverage)this.Model;
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
                if (i_RuleEngine == null)
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
        /// <returns></returns>
        private void CheckForRequiredFields()
        {
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicarePartACoveragePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicarePartBCoveragePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareHasHMOCoveragePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareIsSecondaryPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareDaysRemainingBenefitPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareDaysRemainingCoInsPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareDaysRemainLifeServePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareHospiceProgramPreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareVerifiedBeneficiaryNamePreferred ), Model_Coverage );
            RuleEngine.GetInstance().EvaluateRule( typeof( MedicareInfoRecvdFromPreferred ), Model_Coverage );
        }

        private void PopulatePartACoverageComboBox()
        {
            cmbPartACoverage.Items.Add( blankYesNoFlag );
            cmbPartACoverage.Items.Add( yesYesNoFlag );
            cmbPartACoverage.Items.Add( noYesNoFlag );
        }

        private void PopulatePartBCoverageComboBox()
        {
            cmbPartBCoverage.Items.Add( blankYesNoFlag );
            cmbPartBCoverage.Items.Add( yesYesNoFlag );
            cmbPartBCoverage.Items.Add( noYesNoFlag );
        }

        private void PopulateMedicareIsSecondaryComboBox()
        {
            cmbMedicareIsSecondary.Items.Add( blankYesNoFlag );
            cmbMedicareIsSecondary.Items.Add( yesYesNoFlag );
            cmbMedicareIsSecondary.Items.Add( noYesNoFlag );
        }

        private void PopulatePatientHasMedicareComboBox()
        {
            cmbHMOCoverage.Items.Add( blankYesNoFlag );
            cmbHMOCoverage.Items.Add( yesYesNoFlag );
            cmbHMOCoverage.Items.Add( noYesNoFlag );
        }

        private void PopulateHospiceProgramComboBox()
        {
            cmbPatientIsHospiceProgram.Items.Add( blankYesNoFlag );
            cmbPatientIsHospiceProgram.Items.Add( yesYesNoFlag );
            cmbPatientIsHospiceProgram.Items.Add( noYesNoFlag );
        }

        private void PopulateBeneficiaryNameComboBox()
        {
            cmbVerifiedBeneficiaryName.Items.Add( blankYesNoFlag );
            cmbVerifiedBeneficiaryName.Items.Add( yesYesNoFlag );
            cmbVerifiedBeneficiaryName.Items.Add( noYesNoFlag );
        }

        private void PopulateInfoRecvComboBox()
        {
            IInfoReceivedSourceBroker broker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();
            ICollection alist = broker.AllInfoReceivedSources();

            cmbInfoRecvFrom.Items.Clear();

            foreach (InformationReceivedSource o in alist)
            {
                cmbInfoRecvFrom.Items.Add( o );
            }
        }

        private void ResetView()
        {
            Model_Coverage.RemoveCoverageVerificationData();
            UpdateView();
        }

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicarePartACoveragePreferred ), Model_Coverage, MedicarePartACoveragePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicarePartBCoveragePreferred ), Model_Coverage, MedicarePartBCoveragePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareHasHMOCoveragePreferred ), Model_Coverage, MedicareHasHMOCoveragePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareIsSecondaryPreferred ), Model_Coverage, MedicareIsSecondaryPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareDaysRemainingBenefitPreferred ), Model_Coverage, MedicareDaysRemainingBenefitPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareDaysRemainingCoInsPreferred ), Model_Coverage, MedicareDaysRemainingCoInsPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareDaysRemainLifeServePreferred ), Model_Coverage, MedicareDaysRemainLifeServePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareHospiceProgramPreferred ), Model_Coverage, MedicareHospiceProgramPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareVerifiedBeneficiaryNamePreferred ), Model_Coverage, MedicareVerifiedBeneficiaryNamePreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( MedicareInfoRecvdFromPreferred ), Model_Coverage, MedicareInfoRecvdFromPreferredEventHandler );
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( MedicareVerifyView ) );
            this.panel = new System.Windows.Forms.Panel();
            this.mtbRemainingPartBDeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblRemainingPartBDeductible = new System.Windows.Forms.Label();
            this.lblRemainingPartADeductible = new System.Windows.Forms.Label();
            this.mtbRemainingPartADeductible = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDaysRemainSnfCoInsurance = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbHMOCoverage = new System.Windows.Forms.ComboBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticRemarks = new System.Windows.Forms.Label();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
            this.lblStaticInfoRecvFrom = new System.Windows.Forms.Label();
            this.cmbVerifiedBeneficiaryName = new System.Windows.Forms.ComboBox();
            this.lblStaticBeneficiaryName = new System.Windows.Forms.Label();
            this.cmbPatientIsHospiceProgram = new System.Windows.Forms.ComboBox();
            this.lblStaticHospiceProgram = new System.Windows.Forms.Label();
            this.lineLabel = new PatientAccess.UI.CommonControls.LineLabel();
            this.lblStaticDaysRemainSnfCoinsurance = new System.Windows.Forms.Label();
            this.mtbDaysRemainSNF = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDaysRemainSNF = new System.Windows.Forms.Label();
            this.mtbDaysRemainLifetimeReserve = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDaysRemainLifetime = new System.Windows.Forms.Label();
            this.mtbDaysRemainCoInsurance = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbDaysRemainBenefitPeriod = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDaysRemainingCoInsurance = new System.Windows.Forms.Label();
            this.lblStaticDaysBenefitePeriod = new System.Windows.Forms.Label();
            this.dtpBillingActivityDate = new System.Windows.Forms.DateTimePicker();
            this.mtbBillingActivityDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticDateBilling = new System.Windows.Forms.Label();
            this.cmbMedicareIsSecondary = new System.Windows.Forms.ComboBox();
            this.lblStaticMedicareSecondary = new System.Windows.Forms.Label();
            this.lblStaticHMOCoverage = new System.Windows.Forms.Label();
            this.dtpPartBEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.mtbPartBEffectiveDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticPartBEffectDate = new System.Windows.Forms.Label();
            this.cmbPartBCoverage = new System.Windows.Forms.ComboBox();
            this.lblStaticPartBEffectCoverage = new System.Windows.Forms.Label();
            this.dtpPartAEffectiveDate = new System.Windows.Forms.DateTimePicker();
            this.mtbPartAEffectiveDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticPartAEffectCoverage = new System.Windows.Forms.Label();
            this.cmbPartACoverage = new System.Windows.Forms.ComboBox();
            this.lblStaticPartACoverage = new System.Windows.Forms.Label();
            this.btnClearAll = new PatientAccess.UI.CommonControls.LoggingButton();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.BackColor = System.Drawing.Color.White;
            this.panel.Controls.Add( this.mtbRemainingPartBDeductible );
            this.panel.Controls.Add( this.lblRemainingPartBDeductible );
            this.panel.Controls.Add( this.lblRemainingPartADeductible );
            this.panel.Controls.Add( this.mtbRemainingPartADeductible );
            this.panel.Controls.Add( this.mtbDaysRemainSnfCoInsurance );
            this.panel.Controls.Add( this.cmbHMOCoverage );
            this.panel.Controls.Add( this.mtbRemarks );
            this.panel.Controls.Add( this.lblStaticRemarks );
            this.panel.Controls.Add( this.cmbInfoRecvFrom );
            this.panel.Controls.Add( this.lblStaticInfoRecvFrom );
            this.panel.Controls.Add( this.cmbVerifiedBeneficiaryName );
            this.panel.Controls.Add( this.lblStaticBeneficiaryName );
            this.panel.Controls.Add( this.cmbPatientIsHospiceProgram );
            this.panel.Controls.Add( this.lblStaticHospiceProgram );
            this.panel.Controls.Add( this.lineLabel );
            this.panel.Controls.Add( this.lblStaticDaysRemainSnfCoinsurance );
            this.panel.Controls.Add( this.mtbDaysRemainSNF );
            this.panel.Controls.Add( this.lblStaticDaysRemainSNF );
            this.panel.Controls.Add( this.mtbDaysRemainLifetimeReserve );
            this.panel.Controls.Add( this.lblStaticDaysRemainLifetime );
            this.panel.Controls.Add( this.mtbDaysRemainCoInsurance );
            this.panel.Controls.Add( this.mtbDaysRemainBenefitPeriod );
            this.panel.Controls.Add( this.lblStaticDaysRemainingCoInsurance );
            this.panel.Controls.Add( this.lblStaticDaysBenefitePeriod );
            this.panel.Controls.Add( this.dtpBillingActivityDate );
            this.panel.Controls.Add( this.mtbBillingActivityDate );
            this.panel.Controls.Add( this.lblStaticDateBilling );
            this.panel.Controls.Add( this.cmbMedicareIsSecondary );
            this.panel.Controls.Add( this.lblStaticMedicareSecondary );
            this.panel.Controls.Add( this.lblStaticHMOCoverage );
            this.panel.Controls.Add( this.dtpPartBEffectiveDate );
            this.panel.Controls.Add( this.mtbPartBEffectiveDate );
            this.panel.Controls.Add( this.lblStaticPartBEffectDate );
            this.panel.Controls.Add( this.cmbPartBCoverage );
            this.panel.Controls.Add( this.lblStaticPartBEffectCoverage );
            this.panel.Controls.Add( this.dtpPartAEffectiveDate );
            this.panel.Controls.Add( this.mtbPartAEffectiveDate );
            this.panel.Controls.Add( this.lblStaticPartAEffectCoverage );
            this.panel.Controls.Add( this.cmbPartACoverage );
            this.panel.Controls.Add( this.lblStaticPartACoverage );
            this.panel.Controls.Add( this.btnClearAll );
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point( 0, 0 );
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size( 847, 285 );
            this.panel.TabIndex = 0;
            // 
            // mtbRemainingPartBDeductible
            // 
            this.mtbRemainingPartBDeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbRemainingPartBDeductible.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbRemainingPartBDeductible.Location = new System.Drawing.Point( 448, 150 );
            this.mtbRemainingPartBDeductible.Mask = string.Empty;
            this.mtbRemainingPartBDeductible.MaxLength = 10;
            this.mtbRemainingPartBDeductible.Name = "mtbRemainingPartBDeductible";
            this.mtbRemainingPartBDeductible.Size = new System.Drawing.Size( 60, 20 );
            this.mtbRemainingPartBDeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingPartBDeductible.TabIndex = 9;
            this.mtbRemainingPartBDeductible.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbRemainingPartBDeductible.Enter += new System.EventHandler( this.mtbRemainingPartBDeductible_Enter );
            this.mtbRemainingPartBDeductible.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemainingPartBDeductible_Validating );
            // 
            // lblRemainingPartBDeductible
            // 
            this.lblRemainingPartBDeductible.Location = new System.Drawing.Point( 259, 150 );
            this.lblRemainingPartBDeductible.Name = "lblRemainingPartBDeductible";
            this.lblRemainingPartBDeductible.Size = new System.Drawing.Size( 185, 23 );
            this.lblRemainingPartBDeductible.TabIndex = 0;
            this.lblRemainingPartBDeductible.Text = "Remaining Part-B Deductible:           $";
            // 
            // lblRemainingPartADeductible
            // 
            this.lblRemainingPartADeductible.Location = new System.Drawing.Point( 259, 120 );
            this.lblRemainingPartADeductible.Name = "lblRemainingPartADeductible";
            this.lblRemainingPartADeductible.Size = new System.Drawing.Size( 185, 23 );
            this.lblRemainingPartADeductible.TabIndex = 0;
            this.lblRemainingPartADeductible.Text = "Remaining Part-A Deductible:           $";
            // 
            // mtbRemainingPartADeductible
            // 
            this.mtbRemainingPartADeductible.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbRemainingPartADeductible.KeyPressExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbRemainingPartADeductible.Location = new System.Drawing.Point( 448, 117 );
            this.mtbRemainingPartADeductible.Mask = string.Empty;
            this.mtbRemainingPartADeductible.MaxLength = 10;
            this.mtbRemainingPartADeductible.Name = "mtbRemainingPartADeductible";
            this.mtbRemainingPartADeductible.Size = new System.Drawing.Size( 60, 20 );
            this.mtbRemainingPartADeductible.TabIndex = 8;
            this.mtbRemainingPartADeductible.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.mtbRemainingPartADeductible.ValidationExpression = "^[0-9]{0,6}(\\.[0-9]{0,2})?$";
            this.mtbRemainingPartADeductible.Enter += new System.EventHandler( this.mtbRemainingPartADeductible_Enter );
            this.mtbRemainingPartADeductible.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemainingPartADeductible_Validating );
            // 
            // mtbDaysRemainSnfCoInsurance
            // 
            this.mtbDaysRemainSnfCoInsurance.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDaysRemainSnfCoInsurance.KeyPressExpression = "^\\d*";
            this.mtbDaysRemainSnfCoInsurance.Location = new System.Drawing.Point( 752, 150 );
            this.mtbDaysRemainSnfCoInsurance.Mask = string.Empty;
            this.mtbDaysRemainSnfCoInsurance.MaxLength = 3;
            this.mtbDaysRemainSnfCoInsurance.Name = "mtbDaysRemainSnfCoInsurance";
            this.mtbDaysRemainSnfCoInsurance.Size = new System.Drawing.Size( 40, 20 );
            this.mtbDaysRemainSnfCoInsurance.TabIndex = 14;
            this.mtbDaysRemainSnfCoInsurance.ValidationExpression = "^\\d*";
            this.mtbDaysRemainSnfCoInsurance.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDaysRemainSnfCoInsurance_Validating );
            // 
            // cmbHMOCoverage
            // 
            this.cmbHMOCoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHMOCoverage.Location = new System.Drawing.Point( 448, 14 );
            this.cmbHMOCoverage.Name = "cmbHMOCoverage";
            this.cmbHMOCoverage.Size = new System.Drawing.Size( 50, 21 );
            this.cmbHMOCoverage.TabIndex = 5;
            this.cmbHMOCoverage.SelectedIndexChanged += new System.EventHandler( this.cmbHMOCoverage_SelectedIndexChanged );
            this.cmbHMOCoverage.DropDown += new System.EventHandler( this.cmbHMOCoverage_DropDown );
            // 
            // mtbRemarks
            // 
            this.mtbRemarks.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbRemarks.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbRemarks.Location = new System.Drawing.Point( 320, 238 );
            this.mtbRemarks.Mask = string.Empty;
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 345, 32 );
            this.mtbRemarks.TabIndex = 18;
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemarks_Validating );
            // 
            // lblStaticRemarks
            // 
            this.lblStaticRemarks.Location = new System.Drawing.Point( 264, 241 );
            this.lblStaticRemarks.Name = "lblStaticRemarks";
            this.lblStaticRemarks.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticRemarks.TabIndex = 0;
            this.lblStaticRemarks.Text = "Remarks:";
            // 
            // cmbInfoRecvFrom
            // 
            this.cmbInfoRecvFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point( 395, 204 );
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size( 185, 21 );
            this.cmbInfoRecvFrom.TabIndex = 17;
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler( this.cmbInfoReceivedFrom_SelectedIndexChanged );
            this.cmbInfoRecvFrom.DropDown += new System.EventHandler( this.cmbInfoRecvFrom_DropDown );
            // 
            // lblStaticInfoRecvFrom
            // 
            this.lblStaticInfoRecvFrom.Location = new System.Drawing.Point( 264, 207 );
            this.lblStaticInfoRecvFrom.Name = "lblStaticInfoRecvFrom";
            this.lblStaticInfoRecvFrom.Size = new System.Drawing.Size( 143, 23 );
            this.lblStaticInfoRecvFrom.TabIndex = 0;
            this.lblStaticInfoRecvFrom.Text = "Information received from:";
            // 
            // cmbVerifiedBeneficiaryName
            // 
            this.cmbVerifiedBeneficiaryName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVerifiedBeneficiaryName.Location = new System.Drawing.Point( 188, 238 );
            this.cmbVerifiedBeneficiaryName.Name = "cmbVerifiedBeneficiaryName";
            this.cmbVerifiedBeneficiaryName.Size = new System.Drawing.Size( 48, 21 );
            this.cmbVerifiedBeneficiaryName.TabIndex = 16;
            this.cmbVerifiedBeneficiaryName.SelectedIndexChanged += new System.EventHandler( this.cmbVerifiedBeneficiaryName_SelectedIndexChanged );
            this.cmbVerifiedBeneficiaryName.DropDown += new System.EventHandler( this.cmbVerifiedBeneficiaryName_DropDown );
            // 
            // lblStaticBeneficiaryName
            // 
            this.lblStaticBeneficiaryName.Location = new System.Drawing.Point( 8, 241 );
            this.lblStaticBeneficiaryName.Name = "lblStaticBeneficiaryName";
            this.lblStaticBeneficiaryName.Size = new System.Drawing.Size( 160, 23 );
            this.lblStaticBeneficiaryName.TabIndex = 0;
            this.lblStaticBeneficiaryName.Text = "Verified beneficiary name:";
            // 
            // cmbPatientIsHospiceProgram
            // 
            this.cmbPatientIsHospiceProgram.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPatientIsHospiceProgram.Location = new System.Drawing.Point( 188, 204 );
            this.cmbPatientIsHospiceProgram.Name = "cmbPatientIsHospiceProgram";
            this.cmbPatientIsHospiceProgram.Size = new System.Drawing.Size( 48, 21 );
            this.cmbPatientIsHospiceProgram.TabIndex = 15;
            this.cmbPatientIsHospiceProgram.SelectedIndexChanged += new System.EventHandler( this.cmbPatientIsHospiceProgram_SelectedIndexChanged );
            this.cmbPatientIsHospiceProgram.DropDown += new System.EventHandler( this.cmbPatientIsHospiceProgram_DropDown );
            // 
            // lblStaticHospiceProgram
            // 
            this.lblStaticHospiceProgram.Location = new System.Drawing.Point( 8, 208 );
            this.lblStaticHospiceProgram.Name = "lblStaticHospiceProgram";
            this.lblStaticHospiceProgram.Size = new System.Drawing.Size( 189, 23 );
            this.lblStaticHospiceProgram.TabIndex = 0;
            this.lblStaticHospiceProgram.Text = "Patient is part of a hospice program:";
            // 
            // lineLabel
            // 
            this.lineLabel.Caption = string.Empty;
            this.lineLabel.Location = new System.Drawing.Point( 8, 177 );
            this.lineLabel.Name = "lineLabel";
            this.lineLabel.Size = new System.Drawing.Size( 831, 18 );
            this.lineLabel.TabIndex = 0;
            this.lineLabel.TabStop = false;
            // 
            // lblStaticDaysRemainSnfCoinsurance
            // 
            this.lblStaticDaysRemainSnfCoinsurance.Location = new System.Drawing.Point( 566, 153 );
            this.lblStaticDaysRemainSnfCoinsurance.Name = "lblStaticDaysRemainSnfCoinsurance";
            this.lblStaticDaysRemainSnfCoinsurance.Size = new System.Drawing.Size( 192, 23 );
            this.lblStaticDaysRemainSnfCoinsurance.TabIndex = 0;
            this.lblStaticDaysRemainSnfCoinsurance.Text = "Days of remaining SNF co-insurance:";
            // 
            // mtbDaysRemainSNF
            // 
            this.mtbDaysRemainSNF.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDaysRemainSNF.KeyPressExpression = "^\\d*";
            this.mtbDaysRemainSNF.Location = new System.Drawing.Point( 752, 116 );
            this.mtbDaysRemainSNF.Mask = string.Empty;
            this.mtbDaysRemainSNF.MaxLength = 3;
            this.mtbDaysRemainSNF.Name = "mtbDaysRemainSNF";
            this.mtbDaysRemainSNF.Size = new System.Drawing.Size( 40, 20 );
            this.mtbDaysRemainSNF.TabIndex = 13;
            this.mtbDaysRemainSNF.ValidationExpression = "^\\d*";
            this.mtbDaysRemainSNF.Leave += new System.EventHandler( this.mtbDaysRemainSNF_Leave );
            // 
            // lblStaticDaysRemainSNF
            // 
            this.lblStaticDaysRemainSNF.Location = new System.Drawing.Point( 566, 119 );
            this.lblStaticDaysRemainSNF.Name = "lblStaticDaysRemainSNF";
            this.lblStaticDaysRemainSNF.Size = new System.Drawing.Size( 146, 23 );
            this.lblStaticDaysRemainSNF.TabIndex = 0;
            this.lblStaticDaysRemainSNF.Text = "Days of remaining SNF:";
            // 
            // mtbDaysRemainLifetimeReserve
            // 
            this.mtbDaysRemainLifetimeReserve.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDaysRemainLifetimeReserve.KeyPressExpression = "^\\d*";
            this.mtbDaysRemainLifetimeReserve.Location = new System.Drawing.Point( 752, 82 );
            this.mtbDaysRemainLifetimeReserve.Mask = string.Empty;
            this.mtbDaysRemainLifetimeReserve.MaxLength = 3;
            this.mtbDaysRemainLifetimeReserve.Name = "mtbDaysRemainLifetimeReserve";
            this.mtbDaysRemainLifetimeReserve.Size = new System.Drawing.Size( 40, 20 );
            this.mtbDaysRemainLifetimeReserve.TabIndex = 12;
            this.mtbDaysRemainLifetimeReserve.ValidationExpression = "^\\d*";
            this.mtbDaysRemainLifetimeReserve.Enter += new System.EventHandler( this.mtbDaysRemainLifetimeReserve_Enter );
            this.mtbDaysRemainLifetimeReserve.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDaysRemainLifetimeReserve_Validating );
            // 
            // lblStaticDaysRemainLifetime
            // 
            this.lblStaticDaysRemainLifetime.Location = new System.Drawing.Point( 566, 85 );
            this.lblStaticDaysRemainLifetime.Name = "lblStaticDaysRemainLifetime";
            this.lblStaticDaysRemainLifetime.Size = new System.Drawing.Size( 178, 23 );
            this.lblStaticDaysRemainLifetime.TabIndex = 0;
            this.lblStaticDaysRemainLifetime.Text = "Days of remaining lifetime reserve:";
            // 
            // mtbDaysRemainCoInsurance
            // 
            this.mtbDaysRemainCoInsurance.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDaysRemainCoInsurance.KeyPressExpression = "^\\d*";
            this.mtbDaysRemainCoInsurance.Location = new System.Drawing.Point( 752, 48 );
            this.mtbDaysRemainCoInsurance.Mask = string.Empty;
            this.mtbDaysRemainCoInsurance.MaxLength = 3;
            this.mtbDaysRemainCoInsurance.Name = "mtbDaysRemainCoInsurance";
            this.mtbDaysRemainCoInsurance.Size = new System.Drawing.Size( 40, 20 );
            this.mtbDaysRemainCoInsurance.TabIndex = 11;
            this.mtbDaysRemainCoInsurance.ValidationExpression = "^\\d*";
            this.mtbDaysRemainCoInsurance.Enter += new System.EventHandler( this.mtbDaysRemainCoInsurance_Enter );
            this.mtbDaysRemainCoInsurance.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDaysRemainCoInsurance_Validating );
            // 
            // mtbDaysRemainBenefitPeriod
            // 
            this.mtbDaysRemainBenefitPeriod.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectAllText;
            this.mtbDaysRemainBenefitPeriod.KeyPressExpression = "^\\d*";
            this.mtbDaysRemainBenefitPeriod.Location = new System.Drawing.Point( 752, 14 );
            this.mtbDaysRemainBenefitPeriod.Mask = string.Empty;
            this.mtbDaysRemainBenefitPeriod.MaxLength = 3;
            this.mtbDaysRemainBenefitPeriod.Name = "mtbDaysRemainBenefitPeriod";
            this.mtbDaysRemainBenefitPeriod.Size = new System.Drawing.Size( 40, 20 );
            this.mtbDaysRemainBenefitPeriod.TabIndex = 10;
            this.mtbDaysRemainBenefitPeriod.ValidationExpression = "^\\d*";
            this.mtbDaysRemainBenefitPeriod.Enter += new System.EventHandler( this.mtbDaysRemainBenefitPeriod_Enter );
            this.mtbDaysRemainBenefitPeriod.Validating += new System.ComponentModel.CancelEventHandler( this.mtbDaysRemainBenefitPeriod_Validating );
            // 
            // lblStaticDaysRemainingCoInsurance
            // 
            this.lblStaticDaysRemainingCoInsurance.Location = new System.Drawing.Point( 566, 51 );
            this.lblStaticDaysRemainingCoInsurance.Name = "lblStaticDaysRemainingCoInsurance";
            this.lblStaticDaysRemainingCoInsurance.Size = new System.Drawing.Size( 168, 23 );
            this.lblStaticDaysRemainingCoInsurance.TabIndex = 0;
            this.lblStaticDaysRemainingCoInsurance.Text = "Days of remaining co-insurance:";
            // 
            // lblStaticDaysBenefitePeriod
            // 
            this.lblStaticDaysBenefitePeriod.Location = new System.Drawing.Point( 566, 17 );
            this.lblStaticDaysBenefitePeriod.Name = "lblStaticDaysBenefitePeriod";
            this.lblStaticDaysBenefitePeriod.Size = new System.Drawing.Size( 170, 23 );
            this.lblStaticDaysBenefitePeriod.TabIndex = 0;
            this.lblStaticDaysBenefitePeriod.Text = "Days of remaining benefit period:";
            // 
            // dtpBillingActivityDate
            // 
            this.dtpBillingActivityDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpBillingActivityDate.Checked = false;
            this.dtpBillingActivityDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpBillingActivityDate.Location = new System.Drawing.Point( 512, 82 );
            this.dtpBillingActivityDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpBillingActivityDate.Name = "dtpBillingActivityDate";
            this.dtpBillingActivityDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpBillingActivityDate.TabIndex = 0;
            this.dtpBillingActivityDate.TabStop = false;
            this.dtpBillingActivityDate.CloseUp += new System.EventHandler( this.dtpBillingActivityDate_CloseUp );
            // 
            // mtbBillingActivityDate
            // 
            this.mtbBillingActivityDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbBillingActivityDate.KeyPressExpression = "^\\d*$";
            this.mtbBillingActivityDate.Location = new System.Drawing.Point( 448, 82 );
            this.mtbBillingActivityDate.Mask = "  /  /";
            this.mtbBillingActivityDate.MaxLength = 10;
            this.mtbBillingActivityDate.Name = "mtbBillingActivityDate";
            this.mtbBillingActivityDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbBillingActivityDate.TabIndex = 7;
            this.mtbBillingActivityDate.ValidationExpression = resources.GetString( "mtbBillingActivityDate.ValidationExpression" );
            this.mtbBillingActivityDate.Enter += new System.EventHandler( this.mtbBillingActivityDate_Enter );
            this.mtbBillingActivityDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbBillingActivityDate_Validating );
            // 
            // lblStaticDateBilling
            // 
            this.lblStaticDateBilling.Location = new System.Drawing.Point( 259, 85 );
            this.lblStaticDateBilling.Name = "lblStaticDateBilling";
            this.lblStaticDateBilling.Size = new System.Drawing.Size( 152, 23 );
            this.lblStaticDateBilling.TabIndex = 0;
            this.lblStaticDateBilling.Text = "Date of last billing activity:";
            // 
            // cmbMedicareIsSecondary
            // 
            this.cmbMedicareIsSecondary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMedicareIsSecondary.Location = new System.Drawing.Point( 448, 48 );
            this.cmbMedicareIsSecondary.Name = "cmbMedicareIsSecondary";
            this.cmbMedicareIsSecondary.Size = new System.Drawing.Size( 50, 21 );
            this.cmbMedicareIsSecondary.TabIndex = 6;
            this.cmbMedicareIsSecondary.SelectedIndexChanged += new System.EventHandler( this.cmbMedicareIsSecondary_SelectedIndexChanged );
            this.cmbMedicareIsSecondary.DropDown += new System.EventHandler( this.cmbMedicareIsSecondary_DropDown );
            // 
            // lblStaticMedicareSecondary
            // 
            this.lblStaticMedicareSecondary.Location = new System.Drawing.Point( 259, 51 );
            this.lblStaticMedicareSecondary.Name = "lblStaticMedicareSecondary";
            this.lblStaticMedicareSecondary.Size = new System.Drawing.Size( 128, 23 );
            this.lblStaticMedicareSecondary.TabIndex = 0;
            this.lblStaticMedicareSecondary.Text = "Medicare is secondary:";
            // 
            // lblStaticHMOCoverage
            // 
            this.lblStaticHMOCoverage.Location = new System.Drawing.Point( 259, 17 );
            this.lblStaticHMOCoverage.Name = "lblStaticHMOCoverage";
            this.lblStaticHMOCoverage.Size = new System.Drawing.Size( 194, 23 );
            this.lblStaticHMOCoverage.TabIndex = 0;
            this.lblStaticHMOCoverage.Text = "Patient has Medicare HMO coverage:";
            // 
            // dtpPartBEffectiveDate
            // 
            this.dtpPartBEffectiveDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpPartBEffectiveDate.Checked = false;
            this.dtpPartBEffectiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPartBEffectiveDate.Location = new System.Drawing.Point( 182, 116 );
            this.dtpPartBEffectiveDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpPartBEffectiveDate.Name = "dtpPartBEffectiveDate";
            this.dtpPartBEffectiveDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpPartBEffectiveDate.TabIndex = 0;
            this.dtpPartBEffectiveDate.TabStop = false;
            this.dtpPartBEffectiveDate.CloseUp += new System.EventHandler( this.dtpPartBEffectiveDate_CloseUp );
            // 
            // mtbPartBEffectiveDate
            // 
            this.mtbPartBEffectiveDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPartBEffectiveDate.KeyPressExpression = "^\\d*$";
            this.mtbPartBEffectiveDate.Location = new System.Drawing.Point( 118, 116 );
            this.mtbPartBEffectiveDate.Mask = "  /  /";
            this.mtbPartBEffectiveDate.MaxLength = 10;
            this.mtbPartBEffectiveDate.Name = "mtbPartBEffectiveDate";
            this.mtbPartBEffectiveDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbPartBEffectiveDate.TabIndex = 4;
            this.mtbPartBEffectiveDate.ValidationExpression = resources.GetString( "mtbPartBEffectiveDate.ValidationExpression" );
            this.mtbPartBEffectiveDate.Enter += new System.EventHandler( this.mtbPartBEffectiveDate_Enter );
            this.mtbPartBEffectiveDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPartBEffectiveDate_Validating );
            // 
            // lblStaticPartBEffectDate
            // 
            this.lblStaticPartBEffectDate.Location = new System.Drawing.Point( 8, 119 );
            this.lblStaticPartBEffectDate.Name = "lblStaticPartBEffectDate";
            this.lblStaticPartBEffectDate.Size = new System.Drawing.Size( 112, 23 );
            this.lblStaticPartBEffectDate.TabIndex = 0;
            this.lblStaticPartBEffectDate.Text = "Part B effective date:";
            // 
            // cmbPartBCoverage
            // 
            this.cmbPartBCoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPartBCoverage.Location = new System.Drawing.Point( 118, 82 );
            this.cmbPartBCoverage.Name = "cmbPartBCoverage";
            this.cmbPartBCoverage.Size = new System.Drawing.Size( 50, 21 );
            this.cmbPartBCoverage.TabIndex = 3;
            this.cmbPartBCoverage.SelectedIndexChanged += new System.EventHandler( this.cmbPartBCoverage_SelectedIndexChanged );
            this.cmbPartBCoverage.DropDown += new System.EventHandler( this.cmbPartBCoverage_DropDown );
            // 
            // lblStaticPartBEffectCoverage
            // 
            this.lblStaticPartBEffectCoverage.Location = new System.Drawing.Point( 8, 85 );
            this.lblStaticPartBEffectCoverage.Name = "lblStaticPartBEffectCoverage";
            this.lblStaticPartBEffectCoverage.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticPartBEffectCoverage.TabIndex = 0;
            this.lblStaticPartBEffectCoverage.Text = "Part B coverage:";
            // 
            // dtpPartAEffectiveDate
            // 
            this.dtpPartAEffectiveDate.CalendarTitleForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.dtpPartAEffectiveDate.Checked = false;
            this.dtpPartAEffectiveDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPartAEffectiveDate.Location = new System.Drawing.Point( 182, 48 );
            this.dtpPartAEffectiveDate.MinDate = new System.DateTime( 1800, 1, 1, 0, 0, 0, 0 );
            this.dtpPartAEffectiveDate.Name = "dtpPartAEffectiveDate";
            this.dtpPartAEffectiveDate.Size = new System.Drawing.Size( 21, 20 );
            this.dtpPartAEffectiveDate.TabIndex = 0;
            this.dtpPartAEffectiveDate.TabStop = false;
            this.dtpPartAEffectiveDate.CloseUp += new System.EventHandler( this.dtpPartAEffectiveDate_CloseUp );
            // 
            // mtbPartAEffectiveDate
            // 
            this.mtbPartAEffectiveDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPartAEffectiveDate.KeyPressExpression = "^\\d*$";
            this.mtbPartAEffectiveDate.Location = new System.Drawing.Point( 118, 48 );
            this.mtbPartAEffectiveDate.Mask = "  /  /";
            this.mtbPartAEffectiveDate.MaxLength = 10;
            this.mtbPartAEffectiveDate.Name = "mtbPartAEffectiveDate";
            this.mtbPartAEffectiveDate.Size = new System.Drawing.Size( 70, 20 );
            this.mtbPartAEffectiveDate.TabIndex = 2;
            this.mtbPartAEffectiveDate.ValidationExpression = resources.GetString( "mtbPartAEffectiveDate.ValidationExpression" );
            this.mtbPartAEffectiveDate.Enter += new System.EventHandler( this.mtbPartAEffectiveDate_Enter );
            this.mtbPartAEffectiveDate.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPartAEffectiveDate_Validating );
            // 
            // lblStaticPartAEffectCoverage
            // 
            this.lblStaticPartAEffectCoverage.Location = new System.Drawing.Point( 8, 51 );
            this.lblStaticPartAEffectCoverage.Name = "lblStaticPartAEffectCoverage";
            this.lblStaticPartAEffectCoverage.Size = new System.Drawing.Size( 112, 23 );
            this.lblStaticPartAEffectCoverage.TabIndex = 0;
            this.lblStaticPartAEffectCoverage.Text = "Part A effective date:";
            // 
            // cmbPartACoverage
            // 
            this.cmbPartACoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPartACoverage.Location = new System.Drawing.Point( 118, 14 );
            this.cmbPartACoverage.Name = "cmbPartACoverage";
            this.cmbPartACoverage.Size = new System.Drawing.Size( 50, 21 );
            this.cmbPartACoverage.TabIndex = 1;
            this.cmbPartACoverage.SelectedIndexChanged += new System.EventHandler( this.cmbPartACoverage_SelectedIndexChanged );
            this.cmbPartACoverage.DropDown += new System.EventHandler( this.cmbPartACoverage_DropDown );
            // 
            // lblStaticPartACoverage
            // 
            this.lblStaticPartACoverage.Location = new System.Drawing.Point( 8, 17 );
            this.lblStaticPartACoverage.Name = "lblStaticPartACoverage";
            this.lblStaticPartACoverage.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticPartACoverage.TabIndex = 0;
            this.lblStaticPartACoverage.Text = "Part A coverage:";
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point( 740, 234 );
            this.btnClearAll.Message = null;
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size( 75, 23 );
            this.btnClearAll.TabIndex = 19;
            this.btnClearAll.Text = "&Clear All";
            this.btnClearAll.Click += new System.EventHandler( this.btnClearAll_Click );
            // 
            // MedicareVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panel );
            this.Name = "MedicareVerifyView";
            this.Size = new System.Drawing.Size( 847, 285 );
            this.Disposed += new System.EventHandler( this.MedicareVerifyView_Disposed );
            this.panel.ResumeLayout( false );
            this.panel.PerformLayout();
            this.ResumeLayout( false );

        }
        #endregion

        #endregion

        #region Construction and Finalization
        public MedicareVerifyView()
        {
            InitializeComponent();

            ConfigureControls();

            loadingModelData = true;
            EnableThemesOn( this );

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
            if (disposing)
            {
                if (components != null)
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

        private ComboBox cmbPartACoverage;
        private ComboBox cmbPartBCoverage;
        private ComboBox cmbHMOCoverage;
        private ComboBox cmbMedicareIsSecondary;
        private ComboBox cmbPatientIsHospiceProgram;
        private ComboBox cmbVerifiedBeneficiaryName;
        private ComboBox cmbInfoRecvFrom;

        private DateTimePicker dtpPartAEffectiveDate;
        private DateTimePicker dtpPartBEffectiveDate;
        private DateTimePicker dtpBillingActivityDate;

        private Label lblStaticPartACoverage;
        private Label lblStaticPartAEffectCoverage;
        private Label lblStaticPartBEffectCoverage;
        private Label lblStaticPartBEffectDate;
        private Label lblStaticHMOCoverage;
        private Label lblStaticDaysRemainLifetime;
        private Label lblStaticDaysRemainSNF;
        private Label lblStaticDaysRemainSnfCoinsurance;
        private Label lblStaticHospiceProgram;
        private Label lblStaticBeneficiaryName;
        private Label lblStaticInfoRecvFrom;
        private Label lblStaticMedicareSecondary;
        private Label lblStaticDateBilling;
        private Label lblStaticDaysBenefitePeriod;
        private Label lblStaticDaysRemainingCoInsurance;
        private Label lblStaticRemarks;
        private Label lblRemainingPartADeductible;
        private Label lblRemainingPartBDeductible;

        private Panel panel;

        private MaskedEditTextBox mtbPartAEffectiveDate;
        private MaskedEditTextBox mtbPartBEffectiveDate;
        private MaskedEditTextBox mtbBillingActivityDate;
        private MaskedEditTextBox mtbDaysRemainBenefitPeriod;
        private MaskedEditTextBox mtbDaysRemainCoInsurance;
        private MaskedEditTextBox mtbDaysRemainLifetimeReserve;
        private MaskedEditTextBox mtbDaysRemainSNF;
        private MaskedEditTextBox mtbDaysRemainSnfCoInsurance;
        private MaskedEditTextBox mtbRemarks;
        private MaskedEditTextBox mtbRemainingPartADeductible;
        private MaskedEditTextBox mtbRemainingPartBDeductible;

        private LineLabel lineLabel;

        private bool loadingModelData;
        private int insuranceMonth;
        private int insuranceDay = -1;
        private int insuranceYear;
        private int remainingBenefitPeriod = -1;
        private int remainingCoInsurance = -1;
        private int remainingLifeTimeReserve = -1;
        private int remainingSNF = -1;
        private int remainingSNFCoInsurance = -1;
        private decimal remainingPartADeductible = -1;
        private decimal remainingPartBDeductible = -1;
        private string remarks;
        private Account i_Account;
        private DateTime dateOfLastBillingActivity;
        private DateTime part_A_EffectiveDate;
        private DateTime part_B_EffectiveDate;
        private InformationReceivedSource informationReceivedSource;
        private YesNoFlag patientHasMedicareHMOCoverage;
        private YesNoFlag medicareIsSecondary;
        private YesNoFlag partACoverage;
        private YesNoFlag partBCoverage;
        private YesNoFlag patientIsPartOfHospiceProgram;
        private YesNoFlag verifiedBeneficiaryName;
        private YesNoFlag blankYesNoFlag;
        private YesNoFlag yesYesNoFlag;
        private YesNoFlag noYesNoFlag;
        private RuleEngine i_RuleEngine;
        #endregion

        #region Constants
        #endregion
    }
}
