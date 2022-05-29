using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using log4net;

namespace PatientAccess.UI.InsuranceViews.InsuranceVerificationViews
{
    /// <summary>
    /// Summary description for CommMgdCareVerifyView.
    /// </summary>
    public class DebugCommMgdCareVerifyView : ControlView
    {
        private TimePeriodFlag blankTimePeriodFlag;
        private TimePeriodFlag yearTimePeriodFlag;
        private TimePeriodFlag visitTimePeriodFlag;
        private YesNoFlag blankYesNoFlag;
        private YesNoFlag yesYesNoFlag;
        private YesNoFlag noYesNoFlag;

        #region Event Handlers

        void DebugCommMgdCareVerifyView_Load( object sender, EventArgs e )
        {
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

            PopulateInfoRecvComboBox();
            PopulateRuleComboBox();
            PopulateTypeOfProductComboBox();
            PopulatePreexistingConditionComboBox();
            PopulateCoveredBenefitComboBox();
            PopulateAddressVerifiedComboBox();
            PopulateCoordinationOfBenefitsComboBox();
            PopulateAutoMedpayCoverageComboBox();
            PopulateFacilityIsContractedProviderComboBox();

            this.cmbInfoRecvFrom.SelectedItem = this.Model_Coverage.InformationReceivedSource;
            this.mtbEligibilityPhone.UnMaskedText = this.Model_Coverage.EligibilityPhone;
            this.mtbInsRepName.UnMaskedText = this.Model_Coverage.InsuranceCompanyRepName;

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
            this.mtbPPOProvider.UnMaskedText = this.Model_Coverage.PPOPricingOrBroker;
            this.cmbFacilityIsProvider.SelectedItem = this.Model_Coverage.FacilityContractedProvider;
            this.mtbAutoInsuranceClaimNumber.UnMaskedText = this.Model_Coverage.AutoInsuranceClaimNumber;
            this.cmbAutoMedpayCoverage.SelectedItem = this.Model_Coverage.AutoMedPayCoverage;

            if( this.Model_Coverage.TypeOfVerificationRule != null )
            {
                this.cmbRule.SelectedItem = this.Model_Coverage.TypeOfVerificationRule;
            }
            else
            {
                this.cmbRule.SelectedItem = new TypeOfVerificationRule();
            }

            this.mtbRemarks.UnMaskedText = this.Model_Coverage.Remarks;

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

        private void PopulateFacilityIsContractedProviderComboBox()
        {
            cmbFacilityIsProvider.Items.Add( blankYesNoFlag );
            cmbFacilityIsProvider.Items.Add( yesYesNoFlag );
            cmbFacilityIsProvider.Items.Add( noYesNoFlag );
        }

        private void PopulateAutoMedpayCoverageComboBox()
        {
            cmbAutoMedpayCoverage.Items.Add( blankYesNoFlag );
            cmbAutoMedpayCoverage.Items.Add( yesYesNoFlag );
            cmbAutoMedpayCoverage.Items.Add( noYesNoFlag );
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

        private void PopulatePreexistingConditionComboBox()
        {
            cmbPreexistingCondition.Items.Add( blankYesNoFlag );
            cmbPreexistingCondition.Items.Add( yesYesNoFlag );
            cmbPreexistingCondition.Items.Add( noYesNoFlag );
        }

        private void PopulateCoveredBenefitComboBox()
        {
            cmbCoveredBenefit.Items.Add( blankYesNoFlag );
            cmbCoveredBenefit.Items.Add( yesYesNoFlag );
            cmbCoveredBenefit.Items.Add( noYesNoFlag );
        }

        private void PopulateAddressVerifiedComboBox()
        {
            cmbAddressVerified.Items.Add( blankYesNoFlag );
            cmbAddressVerified.Items.Add( yesYesNoFlag );
            cmbAddressVerified.Items.Add( noYesNoFlag );
        }



        private void PopulateCoordinationOfBenefitsComboBox()
        {
            cmbCoordinationOfBenefits.Items.Add( blankYesNoFlag );
            cmbCoordinationOfBenefits.Items.Add( yesYesNoFlag );
            cmbCoordinationOfBenefits.Items.Add( noYesNoFlag );
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
        
        
        private void DemographicsView_Disposed( object sender, EventArgs e )
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

        private void cmbAddressVerified_SelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmbAddressVerified );

            ComboBox cb = sender as ComboBox;

            Model_Coverage.ClaimsAddressVerified = cb.SelectedItem as YesNoFlag;
            CheckForRequiredFields();
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
            Model_Coverage.InformationReceivedSource = cb.SelectedItem as InformationReceivedSource;

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
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            //mtb.BackColor = UIColors.TextFieldBackgroundColor( true );
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
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;
            //mtb.BackColor = UIColors.TextFieldBackgroundColor( true );
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
        
        #endregion

        #region Methods

        public override void UpdateView()
        {
            if( loadingModelData )
            {
                loadingModelData = false;

                PopulateInfoRecvComboBox();
                
                PopulateRuleComboBox();
                PopulateTypeOfProductComboBox();
                PopulateBenefitsCategoriesListBox();
                
                PopulatePreexistingConditionComboBox();
                PopulateCoveredBenefitComboBox();
                PopulateAddressVerifiedComboBox();
                PopulateCoordinationOfBenefitsComboBox();
                PopulateAutoMedpayCoverageComboBox();
                PopulateFacilityIsContractedProviderComboBox();                
            }

            

            this.cmbInfoRecvFrom.SelectedItem = this.Model_Coverage.InformationReceivedSource;
            this.mtbEligibilityPhone.UnMaskedText = this.Model_Coverage.EligibilityPhone;
            this.mtbInsRepName.UnMaskedText = this.Model_Coverage.InsuranceCompanyRepName;

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
            this.mtbPPOProvider.UnMaskedText = this.Model_Coverage.PPOPricingOrBroker;
            this.cmbFacilityIsProvider.SelectedItem = this.Model_Coverage.FacilityContractedProvider;
            this.mtbAutoInsuranceClaimNumber.UnMaskedText = this.Model_Coverage.AutoInsuranceClaimNumber;
            this.cmbAutoMedpayCoverage.SelectedItem = this.Model_Coverage.AutoMedPayCoverage;

            if( this.Model_Coverage.TypeOfVerificationRule != null )
            {
                this.cmbRule.SelectedItem = this.Model_Coverage.TypeOfVerificationRule;
            }
            else
            {
                this.cmbRule.SelectedItem = new TypeOfVerificationRule();
            }

            this.mtbRemarks.UnMaskedText = this.Model_Coverage.Remarks;


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

            int i = 0;

            ArrayList bcds = new ArrayList( this.Model_Coverage.BenefitsCategories.Values );

            foreach( BenefitsCategoryDetails bcd in bcds )
            {
                i++;

                switch( i )
                {
                    case 1:
                        {
                            this.debugBenResponseCatDetails1.Model = bcd;
                            this.debugBenResponseCatDetails1.UpdateView();
                            break;
                        }
                    case 2:
                        {
                            this.debugBenResponseCatDetails2.Model = bcd;
                            this.debugBenResponseCatDetails2.UpdateView();
                            break;
                        }
                    case 3:
                        {
                            this.debugBenResponseCatDetails3.Model = bcd;
                            this.debugBenResponseCatDetails3.UpdateView();
                            break;
                        }
                    case 4:
                        {
                            this.debugBenResponseCatDetails4.Model = bcd;
                            this.debugBenResponseCatDetails4.UpdateView();
                            break;
                        }
                    case 5:
                        {
                            this.debugBenResponseCatDetails5.Model = bcd;
                            this.debugBenResponseCatDetails5.UpdateView();
                            break;
                        }
                    case 6:
                        {
                            this.debugBenResponseCatDetails6.Model = bcd;
                            this.debugBenResponseCatDetails6.UpdateView();
                            break;
                        }
                    case 7:
                        {
                            this.debugBenResponseCatDetails7.Model = bcd;
                            this.debugBenResponseCatDetails7.UpdateView();
                            break;
                        }
                    case 8:
                        {
                            this.debugBenResponseCatDetails8.Model = bcd;
                            this.debugBenResponseCatDetails8.UpdateView();
                            break;
                        }
                    case 9:
                        {
                            this.debugBenResponseCatDetails9.Model = bcd;
                            this.debugBenResponseCatDetails9.UpdateView();
                            break;
                        }
                    case 10:
                        {
                            this.debugBenResponseCatDetails10.Model = bcd;
                            this.debugBenResponseCatDetails10.UpdateView();
                            break;
                        }
                    case 11:
                        {
                            this.debugBenResponseCatDetails11.Model = bcd;
                            this.debugBenResponseCatDetails11.UpdateView();
                            break;
                        }
                    case 12:
                        {
                            this.debugBenResponseCatDetails12.Model = bcd;
                            this.debugBenResponseCatDetails12.UpdateView();
                            break;
                        }
                }
            }
        }

        public override void UpdateModel()
        {
            UpdateAttorneyAndInsuranceAgent();

            this.Model_Coverage.InformationReceivedSource = (InformationReceivedSource)this.cmbInfoRecvFrom.SelectedItem;

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

            this.Model_Coverage.ServiceForPreExistingCondition = (YesNoFlag)this.cmbPreexistingCondition.SelectedItem;
            this.Model_Coverage.ServiceIsCoveredBenefit = (YesNoFlag)this.cmbCoveredBenefit.SelectedItem;
            this.Model_Coverage.ClaimsAddressVerified = (YesNoFlag)this.cmbAddressVerified.SelectedItem;
            this.Model_Coverage.CoordinationOfbenefits = (YesNoFlag)this.cmbCoordinationOfBenefits.SelectedItem;

            this.Model_Coverage.TypeOfProduct = (TypeOfProduct)this.cmbTypeOfProduct.SelectedItem;
            if( this.Model_Coverage.TypeOfProduct == null )
            {
                this.Model_Coverage.TypeOfProduct = new TypeOfProduct();
            }

            this.mtbPPOProvider_Validating( this.mtbPPOProvider, null );
            this.Model_Coverage.PPOPricingOrBroker = this.mtbPPOProvider.UnMaskedText;

            this.Model_Coverage.FacilityContractedProvider = (YesNoFlag)this.cmbFacilityIsProvider.SelectedItem;

            this.mtbAutoInsuranceClaimNumber_Validating( this.mtbAutoInsuranceClaimNumber, null );
            this.Model_Coverage.AutoInsuranceClaimNumber = this.mtbAutoInsuranceClaimNumber.UnMaskedText;

            this.Model_Coverage.AutoMedPayCoverage = (YesNoFlag)this.cmbAutoMedpayCoverage.SelectedItem;
            this.Model_Coverage.TypeOfVerificationRule = (TypeOfVerificationRule)this.cmbRule.SelectedItem;
            if( this.Model_Coverage.TypeOfVerificationRule == null )
            {
                this.Model_Coverage.TypeOfVerificationRule = new TypeOfVerificationRule();
            }

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

        private void UpdateAttorneyAndInsuranceAgent()
        {
            

            if( nameAndPhoneView1.Model_Person != null )
            {
                Model_Coverage.Attorney.AttorneyName = nameAndPhoneView1.Model_Person.Name.FirstName;

                if( nameAndPhoneView1.Model_Person.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() ) != null )
                {
                    ContactPoint cp = nameAndPhoneView1.Model_Person.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() );

                    Model_Coverage.Attorney.RemoveContactPoint( cp.TypeOfContactPoint );
                    Model_Coverage.Attorney.AddContactPoint( cp );
                }
            }
            else
            {
                Model_Coverage.Attorney.AttorneyName = string.Empty;
                Model_Coverage.Attorney.RemoveContactPoint( TypeOfContactPoint.NewBusinessContactPointType() );
            }

            

            if( nameAndPhoneView2.Model_Person != null )
            {
                Model_Coverage.InsuranceAgent.AgentName = nameAndPhoneView2.Model_Person.Name.FirstName;

                if( nameAndPhoneView2.Model_Person.ContactPointWith(
                    TypeOfContactPoint.NewBusinessContactPointType() ) != null )
                {
                    ContactPoint cp = nameAndPhoneView2.Model_Person.ContactPointWith(
                        TypeOfContactPoint.NewBusinessContactPointType() );
                    Model_Coverage.InsuranceAgent.RemoveContactPoint( cp.TypeOfContactPoint );
                    Model_Coverage.InsuranceAgent.AddContactPoint( cp );
                }
            }
            else
            {
                Model_Coverage.InsuranceAgent.AgentName = string.Empty;
                Model_Coverage.InsuranceAgent.RemoveContactPoint( TypeOfContactPoint.NewBusinessContactPointType() );
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
        
        private void PopulateBenefitsCategoriesListBox()
        {
            if( Account == null
                || Account.HospitalService == null 
                || Account.KindOfVisit == null)
            {
                return;
            }

            IBenefitsCategoryBroker broker = BrokerFactory.BrokerOfType<IBenefitsCategoryBroker>();
            ArrayList hsvList = (ArrayList)broker.BenefitsCategoriesFor( User.GetCurrent().Facility, Account.HospitalService.Code.Trim() );

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

                    ListViewItem lv = new ListViewItem();
                    lv.Text = o.ToString();
                    lv.Tag = o;

                    AddObjectToCategoryTable( o.Description.ToUpper() );
                }
            }
        }
      

        private void UnRegisterEvents()
        {
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceInformationRecvFromPreferred ), Model_Coverage, new EventHandler( InsuranceInformationRecvFromPreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceEffectiveDatePreferred ), Model_Coverage, new EventHandler( InsuranceEffectiveDatePreferredEventHandler ) );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InsuranceClaimsVerifiedPreferred ), Model_Coverage, new EventHandler( InsuranceClaimsVerifiedPreferredEventHandler ) );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.mtbPPOProvider = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbInsRepName = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.mtbRemarks = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticRemarks = new System.Windows.Forms.Label();
            this.cmbAutoMedpayCoverage = new System.Windows.Forms.ComboBox();
            this.cmbRule = new System.Windows.Forms.ComboBox();
            this.cmbPreexistingCondition = new System.Windows.Forms.ComboBox();
            this.mtbTerminationDate = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.cmbInfoRecvFrom = new System.Windows.Forms.ComboBox();
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
            this.debugBenResponseCatDetails12 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails11 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails10 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails9 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails8 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails7 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails6 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails5 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails4 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails3 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails2 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.debugBenResponseCatDetails1 = new PatientAccess.UI.InsuranceViews.InsuranceVerificationViews.DebugBenResponseCatDetails();
            this.nameAndPhoneView2 = new PatientAccess.UI.CommonControls.NameAndPhoneView();
            this.nameAndPhoneView1 = new PatientAccess.UI.CommonControls.NameAndPhoneView();
            this.lineLabel4 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel5 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel3 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel2 = new PatientAccess.UI.CommonControls.LineLabel();
            this.lineLabel1 = new PatientAccess.UI.CommonControls.LineLabel();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails12 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails11 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails10 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails9 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails8 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails7 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails6 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails5 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails4 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails3 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails2 );
            this.panelMain.Controls.Add( this.debugBenResponseCatDetails1 );
            this.panelMain.Controls.Add( this.nameAndPhoneView2 );
            this.panelMain.Controls.Add( this.nameAndPhoneView1 );
            this.panelMain.Controls.Add( this.lineLabel4 );
            this.panelMain.Controls.Add( this.lineLabel5 );
            this.panelMain.Controls.Add( this.mtbPPOProvider );
            this.panelMain.Controls.Add( this.mtbInsRepName );
            this.panelMain.Controls.Add( this.mtbRemarks );
            this.panelMain.Controls.Add( this.lblStaticRemarks );
            this.panelMain.Controls.Add( this.cmbAutoMedpayCoverage );
            this.panelMain.Controls.Add( this.cmbRule );
            this.panelMain.Controls.Add( this.lineLabel3 );
            this.panelMain.Controls.Add( this.cmbPreexistingCondition );
            this.panelMain.Controls.Add( this.lineLabel2 );
            this.panelMain.Controls.Add( this.lineLabel1 );
            this.panelMain.Controls.Add( this.mtbTerminationDate );
            this.panelMain.Controls.Add( this.cmbInfoRecvFrom );
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
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point( 0, 0 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 1029, 3500 );
            this.panelMain.TabIndex = 0;
            this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler( this.panelMain_Paint );
            // 
            // mtbPPOProvider
            // 
            this.mtbPPOProvider.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbPPOProvider.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbPPOProvider.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOProvider.Location = new System.Drawing.Point( 644, 170 );
            this.mtbPPOProvider.Mask = "";
            this.mtbPPOProvider.MaxLength = 25;
            this.mtbPPOProvider.Name = "mtbPPOProvider";
            this.mtbPPOProvider.Size = new System.Drawing.Size( 160, 20 );
            this.mtbPPOProvider.TabIndex = 29;
            this.mtbPPOProvider.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbPPOProvider.Validating += new System.ComponentModel.CancelEventHandler( this.mtbPPOProvider_Validating );
            this.mtbPPOProvider.TextChanged += new System.EventHandler( this.mtbPPOProvider_TextChanged );
            // 
            // mtbInsRepName
            // 
            this.mtbInsRepName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbInsRepName.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbInsRepName.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbInsRepName.Location = new System.Drawing.Point( 257, 72 );
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
            this.mtbRemarks.KeyPressExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbRemarks.Location = new System.Drawing.Point( 193, 470 );
            this.mtbRemarks.Mask = "";
            this.mtbRemarks.MaxLength = 60;
            this.mtbRemarks.Multiline = true;
            this.mtbRemarks.Name = "mtbRemarks";
            this.mtbRemarks.Size = new System.Drawing.Size( 345, 32 );
            this.mtbRemarks.TabIndex = 60;
            this.mtbRemarks.ValidationExpression = "^[a-zA-Z0-9\\\\ \\[\\^\\$\\.\\|\\?\\*\\+\\(\\)\\-~`!@#%&_={}:;\"\'<,>./\\]]*$";
            this.mtbRemarks.Validating += new System.ComponentModel.CancelEventHandler( this.mtbRemarks_Validating );
            this.mtbRemarks.TextChanged += new System.EventHandler( this.mtbRemarks_TextChanged );
            // 
            // lblStaticRemarks
            // 
            this.lblStaticRemarks.Location = new System.Drawing.Point( 118, 473 );
            this.lblStaticRemarks.Name = "lblStaticRemarks";
            this.lblStaticRemarks.Size = new System.Drawing.Size( 56, 23 );
            this.lblStaticRemarks.TabIndex = 0;
            this.lblStaticRemarks.Text = "Remarks:";
            // 
            // cmbAutoMedpayCoverage
            // 
            this.cmbAutoMedpayCoverage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAutoMedpayCoverage.Location = new System.Drawing.Point( 644, 272 );
            this.cmbAutoMedpayCoverage.Name = "cmbAutoMedpayCoverage";
            this.cmbAutoMedpayCoverage.Size = new System.Drawing.Size( 50, 21 );
            this.cmbAutoMedpayCoverage.TabIndex = 32;
            this.cmbAutoMedpayCoverage.SelectedIndexChanged += new System.EventHandler( this.cmbAutoMedpayCoverage_SelectedIndexChanged );
            // 
            // cmbRule
            // 
            this.cmbRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRule.Location = new System.Drawing.Point( 307, 272 );
            this.cmbRule.Name = "cmbRule";
            this.cmbRule.Size = new System.Drawing.Size( 80, 21 );
            this.cmbRule.TabIndex = 27;
            this.cmbRule.SelectedIndexChanged += new System.EventHandler( this.cmbRule_SelectedIndexChanged );
            // 
            // cmbPreexistingCondition
            // 
            this.cmbPreexistingCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPreexistingCondition.Location = new System.Drawing.Point( 273, 136 );
            this.cmbPreexistingCondition.Name = "cmbPreexistingCondition";
            this.cmbPreexistingCondition.Size = new System.Drawing.Size( 45, 21 );
            this.cmbPreexistingCondition.TabIndex = 23;
            this.cmbPreexistingCondition.SelectedIndexChanged += new System.EventHandler( this.cmbPreexistingCondition_SelectedIndexChanged );
            // 
            // mtbTerminationDate
            // 
            this.mtbTerminationDate.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbTerminationDate.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbTerminationDate.Location = new System.Drawing.Point( 677, 43 );
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
            this.cmbInfoRecvFrom.Location = new System.Drawing.Point( 257, 12 );
            this.cmbInfoRecvFrom.Name = "cmbInfoRecvFrom";
            this.cmbInfoRecvFrom.Size = new System.Drawing.Size( 185, 21 );
            this.cmbInfoRecvFrom.TabIndex = 1;
            this.cmbInfoRecvFrom.SelectedIndexChanged += new System.EventHandler( this.cmbInfoRecvFrom_SelectedIndexChanged );
            this.cmbInfoRecvFrom.DropDown += new System.EventHandler( this.cmbInfoRecvFrom_DropDown );
            // 
            // lblStaticInsRepName
            // 
            this.lblStaticInsRepName.Location = new System.Drawing.Point( 106, 75 );
            this.lblStaticInsRepName.Name = "lblStaticInsRepName";
            this.lblStaticInsRepName.Size = new System.Drawing.Size( 157, 23 );
            this.lblStaticInsRepName.TabIndex = 0;
            this.lblStaticInsRepName.Text = "Insurance company rep name:";
            // 
            // lblStaticTermDate
            // 
            this.lblStaticTermDate.Location = new System.Drawing.Point( 514, 46 );
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
            this.dtpTerminationDate.Location = new System.Drawing.Point( 747, 43 );
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
            this.dtpEffectiveDate.Location = new System.Drawing.Point( 747, 12 );
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
            this.mtbEffectiveDate.Location = new System.Drawing.Point( 677, 12 );
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
            this.lblStaticEffectiveDate.Location = new System.Drawing.Point( 514, 15 );
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
            this.mtbEligibilityPhone.Location = new System.Drawing.Point( 257, 43 );
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
            this.lblStaticEligibilityPhone.Location = new System.Drawing.Point( 106, 46 );
            this.lblStaticEligibilityPhone.Name = "lblStaticEligibilityPhone";
            this.lblStaticEligibilityPhone.Size = new System.Drawing.Size( 100, 23 );
            this.lblStaticEligibilityPhone.TabIndex = 0;
            this.lblStaticEligibilityPhone.Text = "Eligibility phone:";
            // 
            // lblStaticInfoRecvFrom
            // 
            this.lblStaticInfoRecvFrom.Location = new System.Drawing.Point( 106, 15 );
            this.lblStaticInfoRecvFrom.Name = "lblStaticInfoRecvFrom";
            this.lblStaticInfoRecvFrom.Size = new System.Drawing.Size( 136, 23 );
            this.lblStaticInfoRecvFrom.TabIndex = 0;
            this.lblStaticInfoRecvFrom.Text = "Information received from:";
            // 
            // lblStaticPreCondition
            // 
            this.lblStaticPreCondition.Location = new System.Drawing.Point( 100, 139 );
            this.lblStaticPreCondition.Name = "lblStaticPreCondition";
            this.lblStaticPreCondition.Size = new System.Drawing.Size( 180, 23 );
            this.lblStaticPreCondition.TabIndex = 0;
            this.lblStaticPreCondition.Text = "Service is for preexisting condition:";
            this.lblStaticPreCondition.Click += new System.EventHandler( this.lblStaticPreCondition_Click );
            // 
            // cmbCoveredBenefit
            // 
            this.cmbCoveredBenefit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoveredBenefit.Location = new System.Drawing.Point( 273, 170 );
            this.cmbCoveredBenefit.Name = "cmbCoveredBenefit";
            this.cmbCoveredBenefit.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoveredBenefit.TabIndex = 24;
            this.cmbCoveredBenefit.SelectedIndexChanged += new System.EventHandler( this.cmbCoveredBenefit_SelectedIndexChanged );
            // 
            // lblStaticCoveredBenefit
            // 
            this.lblStaticCoveredBenefit.Location = new System.Drawing.Point( 100, 173 );
            this.lblStaticCoveredBenefit.Name = "lblStaticCoveredBenefit";
            this.lblStaticCoveredBenefit.Size = new System.Drawing.Size( 150, 23 );
            this.lblStaticCoveredBenefit.TabIndex = 0;
            this.lblStaticCoveredBenefit.Text = "Service is a covered benefit:";
            this.lblStaticCoveredBenefit.Click += new System.EventHandler( this.lblStaticCoveredBenefit_Click );
            // 
            // lblStaticClaimsVerified
            // 
            this.lblStaticClaimsVerified.Location = new System.Drawing.Point( 100, 207 );
            this.lblStaticClaimsVerified.Name = "lblStaticClaimsVerified";
            this.lblStaticClaimsVerified.Size = new System.Drawing.Size( 136, 23 );
            this.lblStaticClaimsVerified.TabIndex = 0;
            this.lblStaticClaimsVerified.Text = "Claims address verified:";
            this.lblStaticClaimsVerified.Click += new System.EventHandler( this.lblStaticClaimsVerified_Click );
            // 
            // cmbAddressVerified
            // 
            this.cmbAddressVerified.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAddressVerified.Location = new System.Drawing.Point( 273, 204 );
            this.cmbAddressVerified.Name = "cmbAddressVerified";
            this.cmbAddressVerified.Size = new System.Drawing.Size( 45, 21 );
            this.cmbAddressVerified.TabIndex = 25;
            this.cmbAddressVerified.SelectedIndexChanged += new System.EventHandler( this.cmbAddressVerified_SelectedIndexChanged );
            // 
            // lblStaticProduct
            // 
            this.lblStaticProduct.Location = new System.Drawing.Point( 442, 139 );
            this.lblStaticProduct.Name = "lblStaticProduct";
            this.lblStaticProduct.Size = new System.Drawing.Size( 96, 23 );
            this.lblStaticProduct.TabIndex = 0;
            this.lblStaticProduct.Text = "Type of product:";
            this.lblStaticProduct.Click += new System.EventHandler( this.lblStaticProduct_Click );
            // 
            // cmbTypeOfProduct
            // 
            this.cmbTypeOfProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeOfProduct.Location = new System.Drawing.Point( 644, 136 );
            this.cmbTypeOfProduct.Name = "cmbTypeOfProduct";
            this.cmbTypeOfProduct.Size = new System.Drawing.Size( 150, 21 );
            this.cmbTypeOfProduct.TabIndex = 28;
            this.cmbTypeOfProduct.SelectedIndexChanged += new System.EventHandler( this.cmbTypeOfProduct_SelectedIndexChanged );
            // 
            // lblStaticPPO
            // 
            this.lblStaticPPO.Location = new System.Drawing.Point( 442, 173 );
            this.lblStaticPPO.Name = "lblStaticPPO";
            this.lblStaticPPO.Size = new System.Drawing.Size( 208, 23 );
            this.lblStaticPPO.TabIndex = 0;
            this.lblStaticPPO.Text = "PPO network, pricing network, or broker:";
            this.lblStaticPPO.Click += new System.EventHandler( this.lblStaticPPO_Click );
            // 
            // lblStaticFacilityProvider
            // 
            this.lblStaticFacilityProvider.Location = new System.Drawing.Point( 442, 207 );
            this.lblStaticFacilityProvider.Name = "lblStaticFacilityProvider";
            this.lblStaticFacilityProvider.Size = new System.Drawing.Size( 165, 23 );
            this.lblStaticFacilityProvider.TabIndex = 0;
            this.lblStaticFacilityProvider.Text = "Facility is a contracted provider:";
            this.lblStaticFacilityProvider.Click += new System.EventHandler( this.lblStaticFacilityProvider_Click );
            // 
            // cmbFacilityIsProvider
            // 
            this.cmbFacilityIsProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFacilityIsProvider.Location = new System.Drawing.Point( 644, 204 );
            this.cmbFacilityIsProvider.Name = "cmbFacilityIsProvider";
            this.cmbFacilityIsProvider.Size = new System.Drawing.Size( 50, 21 );
            this.cmbFacilityIsProvider.TabIndex = 30;
            this.cmbFacilityIsProvider.SelectedIndexChanged += new System.EventHandler( this.cmbFacilityIsProvider_SelectedIndexChanged );
            // 
            // lblStaticMedpayCoverage
            // 
            this.lblStaticMedpayCoverage.Location = new System.Drawing.Point( 442, 275 );
            this.lblStaticMedpayCoverage.Name = "lblStaticMedpayCoverage";
            this.lblStaticMedpayCoverage.Size = new System.Drawing.Size( 128, 23 );
            this.lblStaticMedpayCoverage.TabIndex = 0;
            this.lblStaticMedpayCoverage.Text = "Auto medpay coverage:";
            this.lblStaticMedpayCoverage.Click += new System.EventHandler( this.lblStaticMedpayCoverage_Click );
            // 
            // mtbAutoInsuranceClaimNumber
            // 
            this.mtbAutoInsuranceClaimNumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.mtbAutoInsuranceClaimNumber.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbAutoInsuranceClaimNumber.KeyPressExpression = "^[a-zA-Z0-9]*$";
            this.mtbAutoInsuranceClaimNumber.Location = new System.Drawing.Point( 644, 238 );
            this.mtbAutoInsuranceClaimNumber.Mask = "";
            this.mtbAutoInsuranceClaimNumber.MaxLength = 15;
            this.mtbAutoInsuranceClaimNumber.Name = "mtbAutoInsuranceClaimNumber";
            this.mtbAutoInsuranceClaimNumber.Size = new System.Drawing.Size( 100, 20 );
            this.mtbAutoInsuranceClaimNumber.TabIndex = 31;
            this.mtbAutoInsuranceClaimNumber.ValidationExpression = "^[a-zA-Z0-9]*$";
            this.mtbAutoInsuranceClaimNumber.Validating += new System.ComponentModel.CancelEventHandler( this.mtbAutoInsuranceClaimNumber_Validating );
            this.mtbAutoInsuranceClaimNumber.TextChanged += new System.EventHandler( this.mtbAutoInsuranceClaimNumber_TextChanged );
            // 
            // lblStaticClaimNumber
            // 
            this.lblStaticClaimNumber.Location = new System.Drawing.Point( 442, 241 );
            this.lblStaticClaimNumber.Name = "lblStaticClaimNumber";
            this.lblStaticClaimNumber.Size = new System.Drawing.Size( 157, 23 );
            this.lblStaticClaimNumber.TabIndex = 0;
            this.lblStaticClaimNumber.Text = "Auto insurance claim number:";
            this.lblStaticClaimNumber.Click += new System.EventHandler( this.lblStaticClaimNumber_Click );
            // 
            // lblStaticRule
            // 
            this.lblStaticRule.Location = new System.Drawing.Point( 273, 275 );
            this.lblStaticRule.Name = "lblStaticRule";
            this.lblStaticRule.Size = new System.Drawing.Size( 32, 23 );
            this.lblStaticRule.TabIndex = 0;
            this.lblStaticRule.Text = "Rule:";
            this.lblStaticRule.Click += new System.EventHandler( this.lblStaticRule_Click );
            // 
            // cmbCoordinationOfBenefits
            // 
            this.cmbCoordinationOfBenefits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCoordinationOfBenefits.Location = new System.Drawing.Point( 273, 238 );
            this.cmbCoordinationOfBenefits.Name = "cmbCoordinationOfBenefits";
            this.cmbCoordinationOfBenefits.Size = new System.Drawing.Size( 45, 21 );
            this.cmbCoordinationOfBenefits.TabIndex = 26;
            this.cmbCoordinationOfBenefits.SelectedIndexChanged += new System.EventHandler( this.cmbCoordinationOfBenefits_SelectedIndexChanged );
            // 
            // lblStaticCoordBenefits
            // 
            this.lblStaticCoordBenefits.Location = new System.Drawing.Point( 100, 241 );
            this.lblStaticCoordBenefits.Name = "lblStaticCoordBenefits";
            this.lblStaticCoordBenefits.Size = new System.Drawing.Size( 144, 23 );
            this.lblStaticCoordBenefits.TabIndex = 0;
            this.lblStaticCoordBenefits.Text = "Coordination of benefits:";
            this.lblStaticCoordBenefits.Click += new System.EventHandler( this.lblStaticCoordBenefits_Click );
            // 
            // debugBenResponseCatDetails12
            // 
            this.debugBenResponseCatDetails12.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails12.Location = new System.Drawing.Point( 23, 2820 );
            this.debugBenResponseCatDetails12.Name = "debugBenResponseCatDetails12";
            this.debugBenResponseCatDetails12.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails12.TabIndex = 72;
            // 
            // debugBenResponseCatDetails11
            // 
            this.debugBenResponseCatDetails11.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails11.Location = new System.Drawing.Point( 23, 2611 );
            this.debugBenResponseCatDetails11.Name = "debugBenResponseCatDetails11";
            this.debugBenResponseCatDetails11.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails11.TabIndex = 71;
            // 
            // debugBenResponseCatDetails10
            // 
            this.debugBenResponseCatDetails10.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails10.Location = new System.Drawing.Point( 23, 2392 );
            this.debugBenResponseCatDetails10.Name = "debugBenResponseCatDetails10";
            this.debugBenResponseCatDetails10.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails10.TabIndex = 70;
            // 
            // debugBenResponseCatDetails9
            // 
            this.debugBenResponseCatDetails9.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails9.Location = new System.Drawing.Point( 23, 2183 );
            this.debugBenResponseCatDetails9.Name = "debugBenResponseCatDetails9";
            this.debugBenResponseCatDetails9.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails9.TabIndex = 69;
            // 
            // debugBenResponseCatDetails8
            // 
            this.debugBenResponseCatDetails8.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails8.Location = new System.Drawing.Point( 23, 1974 );
            this.debugBenResponseCatDetails8.Name = "debugBenResponseCatDetails8";
            this.debugBenResponseCatDetails8.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails8.TabIndex = 68;
            // 
            // debugBenResponseCatDetails7
            // 
            this.debugBenResponseCatDetails7.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails7.Location = new System.Drawing.Point( 23, 1777 );
            this.debugBenResponseCatDetails7.Name = "debugBenResponseCatDetails7";
            this.debugBenResponseCatDetails7.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails7.TabIndex = 67;
            // 
            // debugBenResponseCatDetails6
            // 
            this.debugBenResponseCatDetails6.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails6.Location = new System.Drawing.Point( 23, 1568 );
            this.debugBenResponseCatDetails6.Name = "debugBenResponseCatDetails6";
            this.debugBenResponseCatDetails6.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails6.TabIndex = 66;
            // 
            // debugBenResponseCatDetails5
            // 
            this.debugBenResponseCatDetails5.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails5.Location = new System.Drawing.Point( 23, 1359 );
            this.debugBenResponseCatDetails5.Name = "debugBenResponseCatDetails5";
            this.debugBenResponseCatDetails5.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails5.TabIndex = 65;
            // 
            // debugBenResponseCatDetails4
            // 
            this.debugBenResponseCatDetails4.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails4.Location = new System.Drawing.Point( 23, 1150 );
            this.debugBenResponseCatDetails4.Name = "debugBenResponseCatDetails4";
            this.debugBenResponseCatDetails4.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails4.TabIndex = 64;
            // 
            // debugBenResponseCatDetails3
            // 
            this.debugBenResponseCatDetails3.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails3.Location = new System.Drawing.Point( 23, 941 );
            this.debugBenResponseCatDetails3.Name = "debugBenResponseCatDetails3";
            this.debugBenResponseCatDetails3.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails3.TabIndex = 63;
            // 
            // debugBenResponseCatDetails2
            // 
            this.debugBenResponseCatDetails2.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails2.Location = new System.Drawing.Point( 23, 732 );
            this.debugBenResponseCatDetails2.Name = "debugBenResponseCatDetails2";
            this.debugBenResponseCatDetails2.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails2.TabIndex = 62;
            // 
            // debugBenResponseCatDetails1
            // 
            this.debugBenResponseCatDetails1.BackColor = System.Drawing.Color.White;
            this.debugBenResponseCatDetails1.Location = new System.Drawing.Point( 23, 523 );
            this.debugBenResponseCatDetails1.Name = "debugBenResponseCatDetails1";
            this.debugBenResponseCatDetails1.Size = new System.Drawing.Size( 951, 203 );
            this.debugBenResponseCatDetails1.TabIndex = 61;
            // 
            // nameAndPhoneView2
            // 
            this.nameAndPhoneView2.Location = new System.Drawing.Point( 100, 393 );
            this.nameAndPhoneView2.Model = null;
            this.nameAndPhoneView2.Model_Person = null;
            this.nameAndPhoneView2.Name = "nameAndPhoneView2";
            this.nameAndPhoneView2.NameLabel = "Auto/Home insurance agent name:";
            this.nameAndPhoneView2.PhoneLabel = "Phone:";
            this.nameAndPhoneView2.Size = new System.Drawing.Size( 464, 56 );
            this.nameAndPhoneView2.TabIndex = 46;
            this.nameAndPhoneView2.Load += new System.EventHandler( this.nameAndPhoneView2_Load );
            // 
            // nameAndPhoneView1
            // 
            this.nameAndPhoneView1.Location = new System.Drawing.Point( 100, 315 );
            this.nameAndPhoneView1.Model = null;
            this.nameAndPhoneView1.Model_Person = null;
            this.nameAndPhoneView1.Name = "nameAndPhoneView1";
            this.nameAndPhoneView1.NameLabel = "Attorney name:";
            this.nameAndPhoneView1.PhoneLabel = "Phone:";
            this.nameAndPhoneView1.Size = new System.Drawing.Size( 464, 56 );
            this.nameAndPhoneView1.TabIndex = 40;
            this.nameAndPhoneView1.Load += new System.EventHandler( this.nameAndPhoneView1_Load );
            // 
            // lineLabel4
            // 
            this.lineLabel4.Caption = "";
            this.lineLabel4.Location = new System.Drawing.Point( 100, 440 );
            this.lineLabel4.Name = "lineLabel4";
            this.lineLabel4.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel4.TabIndex = 0;
            this.lineLabel4.TabStop = false;
            this.lineLabel4.Load += new System.EventHandler( this.lineLabel4_Load );
            // 
            // lineLabel5
            // 
            this.lineLabel5.Caption = "";
            this.lineLabel5.Location = new System.Drawing.Point( 100, 361 );
            this.lineLabel5.Name = "lineLabel5";
            this.lineLabel5.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel5.TabIndex = 0;
            this.lineLabel5.TabStop = false;
            this.lineLabel5.Load += new System.EventHandler( this.lineLabel5_Load );
            // 
            // lineLabel3
            // 
            this.lineLabel3.Caption = "";
            this.lineLabel3.Location = new System.Drawing.Point( 100, 295 );
            this.lineLabel3.Name = "lineLabel3";
            this.lineLabel3.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel3.TabIndex = 0;
            this.lineLabel3.TabStop = false;
            this.lineLabel3.Load += new System.EventHandler( this.lineLabel3_Load );
            // 
            // lineLabel2
            // 
            this.lineLabel2.Caption = "";
            this.lineLabel2.Location = new System.Drawing.Point( 100, 109 );
            this.lineLabel2.Name = "lineLabel2";
            this.lineLabel2.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel2.TabIndex = 0;
            this.lineLabel2.TabStop = false;
            this.lineLabel2.Load += new System.EventHandler( this.lineLabel2_Load );
            // 
            // lineLabel1
            // 
            this.lineLabel1.Caption = "";
            this.lineLabel1.Location = new System.Drawing.Point( 107, 86 );
            this.lineLabel1.Name = "lineLabel1";
            this.lineLabel1.Size = new System.Drawing.Size( 834, 18 );
            this.lineLabel1.TabIndex = 0;
            this.lineLabel1.TabStop = false;
            // 
            // DebugCommMgdCareVerifyView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelMain );
            this.Name = "DebugCommMgdCareVerifyView";
            this.Size = new System.Drawing.Size( 1029, 3500 );
            this.Disposed += new System.EventHandler( this.DemographicsView_Disposed );
            this.Load += new EventHandler( DebugCommMgdCareVerifyView_Load );
            this.panelMain.ResumeLayout( false );
            this.panelMain.PerformLayout();
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Construction and Finalization
        public DebugCommMgdCareVerifyView()
        {
            InitializeComponent();
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

        private ComboBox cmbAddressVerified;
        private ComboBox cmbAutoMedpayCoverage;
        private ComboBox cmbCoordinationOfBenefits;
        private ComboBox cmbFacilityIsProvider;
        private ComboBox cmbInfoRecvFrom;
        private ComboBox cmbTypeOfProduct;
        private ComboBox cmbRule;
        private ComboBox cmbPreexistingCondition;
        private ComboBox cmbCoveredBenefit;

        private DateTimePicker dtpTerminationDate;
        private DateTimePicker dtpEffectiveDate;

        private Label lblStaticInfoRecvFrom;
        private Label lblStaticEligibilityPhone;
        private Label lblStaticInsRepName;
        private Label lblStaticEffectiveDate;
        private Label lblStaticTermDate;
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

        private Panel panelMain;

        private MaskedEditTextBox mtbRemarks;

        private MaskedEditTextBox mtbEligibilityPhone;
        private MaskedEditTextBox mtbInsRepName;
        private MaskedEditTextBox mtbEffectiveDate;
        private MaskedEditTextBox mtbTerminationDate;
        private MaskedEditTextBox mtbPPOProvider;
        private MaskedEditTextBox mtbAutoInsuranceClaimNumber;
        private LineLabel lineLabel1;
        private LineLabel lineLabel2;
        private LineLabel lineLabel3;
        private LineLabel lineLabel4;
        private LineLabel lineLabel5;

        private NameAndPhoneView nameAndPhoneView1;
        private NameAndPhoneView nameAndPhoneView2;

        private Account                                     i_Account;
        
        private bool                                        loadingModelData;
        private int                                         insuranceMonth;
        private int                                         insuranceDay;
        private int                                         insuranceYear;
        
        private RuleEngine                                  i_RuleEngine;

        private Hashtable                                   categoryTable = new Hashtable(11);

        #endregion
        private DebugBenResponseCatDetails debugBenResponseCatDetails8;
        private DebugBenResponseCatDetails debugBenResponseCatDetails7;
        private DebugBenResponseCatDetails debugBenResponseCatDetails6;
        private DebugBenResponseCatDetails debugBenResponseCatDetails5;
        private DebugBenResponseCatDetails debugBenResponseCatDetails4;
        private DebugBenResponseCatDetails debugBenResponseCatDetails3;
        private DebugBenResponseCatDetails debugBenResponseCatDetails2;
        private DebugBenResponseCatDetails debugBenResponseCatDetails1;
        private DebugBenResponseCatDetails debugBenResponseCatDetails12;
        private DebugBenResponseCatDetails debugBenResponseCatDetails11;
        private DebugBenResponseCatDetails debugBenResponseCatDetails10;
        private DebugBenResponseCatDetails debugBenResponseCatDetails9;



        #region Constants

        private static readonly ILog c_log =
            LogManager.GetLogger( typeof( CommMgdCareVerifyView ) );

        #endregion

        private void panelMain_Paint( object sender, PaintEventArgs e )
        {

        }

        private void nameAndPhoneView2_Load( object sender, EventArgs e )
        {

        }

        private void nameAndPhoneView1_Load( object sender, EventArgs e )
        {

        }

        private void lineLabel4_Load( object sender, EventArgs e )
        {

        }

        private void lineLabel5_Load( object sender, EventArgs e )
        {

        }

        private void mtbRemarks_TextChanged( object sender, EventArgs e )
        {

        }

        private void lineLabel3_Load( object sender, EventArgs e )
        {

        }

        private void lineLabel2_Load( object sender, EventArgs e )
        {

        }

        private void lblStaticPreCondition_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticCoveredBenefit_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticClaimsVerified_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticProduct_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticPPO_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticFacilityProvider_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticMedpayCoverage_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticClaimNumber_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticRule_Click( object sender, EventArgs e )
        {

        }

        private void lblStaticCoordBenefits_Click( object sender, EventArgs e )
        {

        }

        private void mtbPPOProvider_TextChanged( object sender, EventArgs e )
        {

        }

        private void mtbAutoInsuranceClaimNumber_TextChanged( object sender, EventArgs e )
        {

        }
    }
}
