using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.FinancialCounselingViews;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.MSP2;
using PatientAccess.UI.InsuranceViews.MedicareSecondaryPayorViews;
using PatientAccess.UI.Logging;
using PatientAccess.UI.InsuranceViews.DOFR.Presenter;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class FinancialClassesView : ControlView
    {
        #region Events
        public event EventHandler FinancialClassSelected;
        public event EventHandler TabSelectedEvent;
        public event CancelEventHandler FinancialClassValidating;
        #endregion

        #region Event Handlers
        private void cmboFinancialClass_Validating( object sender, CancelEventArgs e )
        {
            FinancialClassesSelectedIndexChanged(sender, e);
            if (this.FinancialClassValidating != null)
            {
                this.FinancialClassValidating( this, null );
            }
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassRequired ), this.Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassPreferred ), this.Model_Account );
        }

        private void FinancialClassesView_Load( object sender, EventArgs e )
        {
            cmboFinancialClass.Enabled = false;
        }

        private void FinancialClassesSelectedIndexChanged( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( this.cmboFinancialClass );
            ComboBox cb = sender as ComboBox;

            if (cb.SelectedIndex > 0)
            {
                selectedFinancialClass = cb.SelectedItem as FinancialClass;

                if (( Model_Account.FinancialClass == null ||
                    FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, Model_Account.FinancialClass ) ) &&
                    !FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, selectedFinancialClass ))
                {
                    //based on UC071 12a.
                    Model_Account.TotalCurrentAmtDue = 0m; //is estimated account due
                    //Model_Account.TotalPaid has no change
                    Model_Account.NumberOfMonthlyPayments = 0;
                    Model_Account.MonthlyPayment = 0; // auto set by NumberOfMonthlyPayments = 0;
                    //Model_Account.TotalCurrentAmtDue // same.
                    Model_Account.Insurance.HasNoLiability = false;
                    //monthly due date has no change.
                    //community resource list has no change.
                }
                else if (( Model_Account.FinancialClass == null ||
                    !FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, Model_Account.FinancialClass ) ) &&
                    FinancialCouncelingService.IsUninsured( Model_Account.Facility.Oid, selectedFinancialClass ))
                {   //based on UC071 12b.
                    Model_Coverage.Deductible = 0m;
                    Model_Coverage.CoPay = 0m;
                    Model_Account.MonthlyPayment = 0m;
                    Model_Account.MonthlyPayment = 0; // auto set by NumberOfMonthlyPayments = 0;
                    Model_Account.TotalCurrentAmtDue = 0m;
                    Model_Coverage.NoLiability = false;
                    //monthly due date has no change.                    
                }
                if (selectedFinancialClass != null)
                {
                    
                     UpdatePatientLiabilityForFinancialClass17();
                    
                    if (selectedFinancialClass.Code == FinancialClass.MCARE_MCD_CROSSOVER_CODE)
                    {
                        Model_Coverage.NoLiability = true;
                        ResetPaymentValues();
                    }
                    else if (Model_Account.FinancialClass != null &&
                             (selectedFinancialClass.Code != FinancialClass.MCARE_MCD_CROSSOVER_CODE &&
                              Model_Account.FinancialClass.Code ==
                              FinancialClass.MCARE_MCD_CROSSOVER_CODE))
                        Model_Coverage.NoLiability = false;
                }
            }


            Model_Account.FinancialClass = (FinancialClass)this.cmboFinancialClass.SelectedItem;
            RunRules();

            if (FinancialClassSelected != null)
            {
                FinancialClassSelected( this, new LooseArgs( Model_Account.FinancialClass ) );
            }

            MSPQuestionaireRequired mspQuestionaireRequired = new MSPQuestionaireRequired();
            mspQuestionaireRequired.Model_Account = this.Model_Account;
            bool isMedicareInsurance = mspQuestionaireRequired.InsuranceIsMedicare();
            btnMspForm.Enabled = isMedicareInsurance;

            btnMspSummary.Enabled = (btnMspForm.Enabled
               && mspQuestionaireRequired.CanBeAppliedTo(this.Model_Account));
            DOFRInitiatePresenter.UpdateView();
        }

        public void UpdatePatientLiabilityForFinancialClass17()
        {
            // Auto Complete No Liability Due flag rule for phoenix facility
            if (selectedFinancialClass != null)
            {
                if (AutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedAfterImplementationDate(Model_Account))
                {
                    if (selectedFinancialClass.IsMedicaidManagedFC17)
                    {
                        AutoCompleteNoLiabilityDue();
                    }
                    else if (Model_Account.FinancialClass != null &&
                             (selectedFinancialClass.Code != FinancialClass.MCAID_MGD_CONTR_CODE &&
                              Model_Account.FinancialClass.Code ==
                              FinancialClass.MCAID_MGD_CONTR_CODE))
                    {
                        Model_Coverage.NoLiability = false;
                    }

                }
                if (AutoCompleteNoLiabilityDueFeatureManager.IsAccountCreatedBeforeImplementationDate(Model_Account))
                {
                    if (Model_Account.HasFinancialClassChanged &&
                        selectedFinancialClass.IsMedicaidManagedFC17 &&
                        AutoCompleteNoLiabilityDueFeatureManager.IsFeatureEnabledForToday)
                    {
                        AutoCompleteNoLiabilityDue();
                    }
                    else if (Model_Account.FinancialClass != null &&
                             (selectedFinancialClass.Code != FinancialClass.MCAID_MGD_CONTR_CODE &&
                              Model_Account.FinancialClass.Code ==
                              FinancialClass.MCAID_MGD_CONTR_CODE))
                    {
                        Model_Coverage.NoLiability = false;
                    }
                }
            }
        }

        private void MspButtonClickEventHandler( object sender, EventArgs e )
        {
            if (this.msp != null)
            {
                msp.Dispose();
            }

            if (this.msp2 != null)
            {
                msp2.Dispose();
            }


            if (TabSelectedEvent != null)
            {
                LooseArgs args = e as LooseArgs;
                int index = (int)args.Context;

                TabSelectedEvent( this, new LooseArgs( index ) );
            }
        }

        private void btnMspForm_Click( object sender, EventArgs e )
        {
            BreadCrumbLogger.GetInstance.Log( "Click MSP form", Model_Account );
            medPayor = null;
            medPayor = new MedicareSecondaryPayor();
            medPayor = (MedicareSecondaryPayor)Model_Account.MedicareSecondaryPayor.Clone();
            medPayor.MSPVersion = Model_Account.MedicareSecondaryPayor.MSPVersion;

            if (this.Model_Account.AdmitDate >= this.MSP2StartDate
                && Model_Account.MedicareSecondaryPayor.MSPVersion != 1)
            {
                this.Cursor = Cursors.WaitCursor;

                msp2 = new MSP2Dialog( Model_Account );

                this.Cursor = Cursors.Default;

                msp2.MspButtonClickEvent += MspButtonClickEventHandler;

                if (msp2.ShowDialog( this ) == DialogResult.OK)
                {
                    GetMSPRelatedOCCCondCodes();
                    DeleteMSPRelatedOCCCondCodes();
                    CheckMSPRelatedOccAndCondCodes(Model_Account.MedicareSecondaryPayor);

                    btnMspSummary.Enabled = true;

                    msp2.MspButtonClickEvent -= MspButtonClickEventHandler;
                }

                msp2.Dispose();
            }
            else
            {
                msp = new MSPDialog( Model_Account, medPayor, false );

                try
                {
                    msp.MspButtonClickEvent += MspButtonClickEventHandler;

                    if (msp.ShowDialog( this ) == DialogResult.OK)
                    {
                        // Look at the fields that can be changed by the wizard to see if a copy is required
                        if (Model_Account.MedicareSecondaryPayor.Equals( medPayor ) == false)
                        {   // Copy MSP date to the account
                            Model_Account.MedicareSecondaryPayor = (MedicareSecondaryPayor)medPayor.Clone();
                            GetMSPRelatedOCCCondCodes();
                            DeleteMSPRelatedOCCCondCodes();
                            CheckMSPRelatedOccAndCondCodes(Model_Account.MedicareSecondaryPayor);

                            btnMspSummary.Enabled = true;
                            Model_Account.MedicareSecondaryPayor.MSPVersion = 1;
                        }

                        msp.MspButtonClickEvent -= MspButtonClickEventHandler;
                    }
                }
                finally
                {
                    msp.Dispose();
                }
            }
        }
        private void CheckMSPRelatedOccAndCondCodes(MedicareSecondaryPayor msp )
        {
          
            i_PatientIsNotEmployed = false;
            i_SpouseIsNotEmployed = false;
            i_FamilyMemberNotEmployed = false;
            i_AgeGHPCoverage = false;
            i_DisabilityGHPCoverage = false;

            CheckPatientRetirement(msp );
            CheckSpouseRetirement(msp);
            CheckFamilyMemberRetirement(msp);
            CheckAgeGHP(msp);
            CheckDisabilityGHP(msp);
            CheckAgeEmployeeNumForGHP(msp);
            CheckDisabilityEmployeeNumForGHP(msp);
        }
        private void btnMspSummary_Click( object sender, EventArgs e )
        {
            medPayor = null;
            medPayor = new MedicareSecondaryPayor();

            if (this.Model_Account.AdmitDate >= this.MSP2StartDate
                && Model_Account.MedicareSecondaryPayor.MSPVersion != 1)
            {
                msp2 = new MSP2Dialog( Model_Account );

                try
                {
                    msp2.SummaryOnly = true;
                    msp2.ShowDialog( this );
                }
                finally
                {
                    msp2.Dispose();
                }
            }
            else
            {
                msp = new MSPDialog( Model_Account, medPayor, true );

                try
                {
                    msp.ShowDialog( this );
                }
                finally
                {
                    msp.Dispose();
                }
            }
        }

        private void FinancialClassPreferredEventHandler( object sender, EventArgs e )
        {
            if (cmboFinancialClass.Enabled)
            {
                UIColors.SetPreferredBgColor( cmboFinancialClass );
            }
        }

        private void FinancialClassRequiredEventHandler( object sender, EventArgs e )
        {
            if (cmboFinancialClass.Enabled)
            {
                UIColors.SetRequiredBgColor( cmboFinancialClass );
            }
        }

        private void mtbMotherDOB_Validated( object sender, EventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if (mtb.UnMaskedText == String.Empty)
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if (mtb.Text.Length != 10)
            {
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtb.Focus();
            }
            else
            {
                try
                {
                    birthMonth = Convert.ToInt32( mtb.Text.Substring( 0, 2 ) );
                    birthDay = Convert.ToInt32( mtb.Text.Substring( 3, 2 ) );
                    birthYear = Convert.ToInt32( mtb.Text.Substring( 6, 4 ) );

                    DateTime dateOfBirth = new DateTime( birthYear, birthMonth, birthDay );
                    // Get today's date to check the date entered is not in the future
                    ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                    DateTime todaysDate = timeBroker.TimeAt( GetUser().Facility.GMTOffset,
                                                             GetUser().Facility.DSTOffset );

                    if (dateOfBirth > todaysDate)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else if (DateValidator.IsValidDate( dateOfBirth ) == false)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else if (dateOfBirth < earliestDate)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mtb );
                        Model_Account.Patient.MothersDateOfBirth = dateOfBirth;
                        Refresh();
                    }
                }
                catch (ArgumentOutOfRangeException)
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    UIColors.SetErrorBgColor( mtb );
                    MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtb.Focus();
                }
            }
        }

        private void mtbFatherDOB_Validating( object sender, CancelEventArgs e )
        {
            MaskedEditTextBox mtb = sender as MaskedEditTextBox;

            if (mtb.UnMaskedText == String.Empty)
            {
                UIColors.SetNormalBgColor( mtb );
                Refresh();
            }
            else if (mtb.Text.Length != 10)
            {
                UIColors.SetErrorBgColor( mtb );
                MessageBox.Show( UIErrorMessages.DATE_INVALID_ERRMSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                mtb.Focus();
            }
            else
            {
                try
                {
                    birthMonth = Convert.ToInt32( mtb.Text.Substring( 0, 2 ) );
                    birthDay = Convert.ToInt32( mtb.Text.Substring( 3, 2 ) );
                    birthYear = Convert.ToInt32( mtb.Text.Substring( 6, 4 ) );

                    DateTime dateOfBirth = new DateTime( birthYear, birthMonth, birthDay );
                    // Get today's date to check the date entered is not in the future
                    ITimeBroker timeBroker = ProxyFactory.GetTimeBroker();
                    DateTime todaysDate = timeBroker.TimeAt( GetUser().Facility.GMTOffset,
                                                             GetUser().Facility.DSTOffset );

                    if (dateOfBirth > todaysDate)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_FUTURE_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else if (DateValidator.IsValidDate( dateOfBirth ) == false)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_NOTVALID_ERRMSG, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else if (dateOfBirth < earliestDate)
                    {
                        UIColors.SetErrorBgColor( mtb );
                        MessageBox.Show( UIErrorMessages.DOB_OUT_OF_RANGE, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                            MessageBoxDefaultButton.Button1 );
                        mtb.Focus();
                    }
                    else
                    {
                        UIColors.SetNormalBgColor( mtb );
                        Model_Account.Patient.FathersDateOfBirth = dateOfBirth;
                        Refresh();
                    }
                }
                catch (ArgumentOutOfRangeException)
                {   // DateTime ctor throws ArgumentOutOfRange exception when there's
                    // an invalid year, month, or day.  Simply set field to error color.
                    UIColors.SetErrorBgColor( mtb );
                    MessageBox.Show( UIErrorMessages.DATE_NOT_EXIST_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
                    mtb.Focus();
                }
            }
        }

        #endregion

        #region Methods
        public void ResetFinancialClass()
        {
            cmboFinancialClass.SelectedIndex = -1;
            cmboFinancialClass.Enabled = false;
           
            ResetMBIForSecondaryIns(Model_Account);
        }

        public void ResetMSPGroupView()
        {
            btnMspForm.Enabled = false;
            btnMspSummary.Enabled = false;
        }

        public override void UpdateView()
        {
            this.RegisterRulesEvents();

            if (Model_Account != null)
            {
                if (Model_Account.Patient.FathersDateOfBirth != DateTime.MinValue)
                {
                    mtbFatherDOB.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                        Model_Account.Patient.FathersDateOfBirth.Month,
                        Model_Account.Patient.FathersDateOfBirth.Day,
                        Model_Account.Patient.FathersDateOfBirth.Year );
                }

                if (Model_Account.Patient.MothersDateOfBirth != DateTime.MinValue)
                {
                    mtbMotherDOB.Text = String.Format( "{0:D2}{1:D2}{2:D4}",
                        Model_Account.Patient.MothersDateOfBirth.Month,
                        Model_Account.Patient.MothersDateOfBirth.Day,
                        Model_Account.Patient.MothersDateOfBirth.Year );
                }

                // if there is a secondary coverage wipe out any HIC number
               
                ResetMBIForSecondaryIns(Model_Account);
            }

            // If there is a Medicare coverage, enable the MSP Summary button.  If the MedicareSecondaryPayor
            // data on the accout is incomplete, the MSP rule event handler in InsuranceView will disable the button.

            MSPQuestionaireRequired mspQuestionaireRequired = new MSPQuestionaireRequired();
            mspQuestionaireRequired.Model_Account = this.Model_Account;
            bool isMedicareInsurance = mspQuestionaireRequired.InsuranceIsMedicare();
            btnMspForm.Enabled = isMedicareInsurance;

            GetMSPRelatedOCCCondCodes();
            DeleteMSPRelatedOCCCondCodes();
            if (isMedicareInsurance)
            {
                CheckMSPRelatedOccAndCondCodes(Model_Account.MedicareSecondaryPayor);
            }

            btnMspSummary.Enabled = ( btnMspForm.Enabled
                && mspQuestionaireRequired.CanBeAppliedTo( this.Model_Account ) );

            if (Model_Coverage == null)
            {
                return;
            }

            // ComboBox should only be populated if the Coverage is primary coverage.
            if (Model_Coverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID)
            {
                this.PopulateFinancialClass();
            }

            RunRules();

            // Dofr button Enabled Logic - Enable when financial class is enabled 
            DOFRInitiatePresenter.UpdateView();

        }
        #endregion

        #region Properties

        public Coverage Model_Coverage
        {
           get
            {
                return (Coverage)this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        public Account Model_Account
        {
           get
            {
                return i_account;
            }
            set
            {
                i_account = value;
            }
        }

        public PatientAccessComboBox ComboBoxFinClass
        {
            get
            {
                return this.cmboFinancialClass;
            }
        }

        public DOFRInitiatePresenter DOFRInitiatePresenter
        {
            get
            {
                DOFRInitiatePresenter dOFRInitiatePresenter = this.dofrInitiateView1.DOFRInitiatePresenter;
                if (dOFRInitiatePresenter == null)
                {
                    var dOFRInitiateBroker = BrokerFactory.BrokerOfType<IDOFRInitiateBroker>();

                    dOFRInitiatePresenter = new DOFRInitiatePresenter(this.dofrInitiateView1, Model_Account, dOFRInitiateBroker, DOFRFeatureManager.GetInstance());
                }
                return dOFRInitiatePresenter;
            }
        }

        public bool MspSummaryEnabled
        {
            set
            {
                btnMspSummary.Enabled = value;
            }
        }

        private DateTime MSP2StartDate
        {
            get
            {
                if (i_MSP2StartDate == DateTime.MinValue)
                {
                    // retrieve the date from the broker

                    IMSPBroker broker = BrokerFactory.BrokerOfType<IMSPBroker>();

                    i_MSP2StartDate = broker.GetMSP2StartDate();
                }

                return i_MSP2StartDate;
            }
        }

        #endregion

        #region Private Methods

        private void AutoCompleteNoLiabilityDue()
        {
            Model_Coverage.NoLiability = true;
            ResetPaymentValues();
        }
         
        private static void ResetMBIForSecondaryIns(Account anAccount)
        {
            // if the finClass is blank or anything other than SIGNED_OVER_MEDICARE_FINANCIAL_CLASS_CODE (84 or 87)
            // blank out the HIC on the secondary insurance if present
            if (anAccount.Insurance.SecondaryCoverage != null &&  // there is a seconary coverage
                    (anAccount.FinancialClass != null &&      // there is a finclass other than 84 or 87
                     !string.IsNullOrEmpty(anAccount.FinancialClass.Code) &&
                     !anAccount.FinancialClass.IsSignedOverMedicare())
                    ||
                    (anAccount.FinancialClass == null ||      // there is no finclass
                     anAccount.FinancialClass.Code == null ||
                     (anAccount.FinancialClass.Code != null &&
                      anAccount.FinancialClass.Code.Equals(string.Empty))
                    )

                )
            {
                Coverage coverage = anAccount.Insurance.SecondaryCoverage;
                if (coverage is CoverageForCommercialOther)
                {
                    CoverageForCommercialOther comCoverage = coverage as CoverageForCommercialOther;
                    comCoverage.MBINumber = string.Empty;
                }
            }
        }
        private void PopulateFinancialClass()
        {
            IInsuranceBroker broker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            classCollection = broker.PlanFinClassesFor( User.GetCurrent().Facility.Oid, Model_Coverage.InsurancePlan.PlanSuffix );

            if (classCollection == null)
            {
                return;
            }
            cmboFinancialClass.Items.Clear();

            foreach (FinancialClass fc in classCollection)
            {
                if (!IsFincialAgreeMentOrSelfPay( fc )  &&  !IsMseScreenExam( fc ))
                {
                    cmboFinancialClass.Items.Add( fc );
                }
            }

            cmboFinancialClass.Enabled = cmboFinancialClass.Items.Count > 0;

            if (this.Model_Account.FinancialClass != null)
            {
                cmboFinancialClass.SelectedItem = this.Model_Account.FinancialClass;
            }
            else
            {
                cmboFinancialClass.SelectedIndex = 0;
            }
        }
        private static bool IsFincialAgreeMentOrSelfPay( CodedReferenceValue fc )
        {
            return ( fc.Code == FINANCIAL_AGREEMENT || fc.Code == SELF_PAY );


        }
        private static bool IsMseScreenExam( CodedReferenceValue fc )
        {
            return ( fc.Code == MED_MSE_SCREEN_EXM );
        }

        private void RegisterRulesEvents()
        {
            if (!i_Registered)
            {
                RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassPreferred ), this.Model_Account, new EventHandler( FinancialClassPreferredEventHandler ) );
                RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassRequired ), this.Model_Account, new EventHandler( FinancialClassRequiredEventHandler ) );
            }
        }

        private void UnRegisterRulesEvents()
        {
            i_Registered = false;

            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassPreferred ), this.Model_Account, FinancialClassPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassRequired ), this.Model_Account, FinancialClassRequiredEventHandler );
        }

        public void RunRules()
        {
            UIColors.SetNormalBgColor( cmboFinancialClass );

            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassRequired ), this.Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassPreferred ), this.Model_Account );
        }

        private void CheckPatientRetirement( MedicareSecondaryPayor mp )
        {
            // If patient chose 'retired' update the account with the retirement information

            if (mp != null
                && mp.MedicareEntitlement != null
                && mp.MedicareEntitlement.PatientEmployment != null &&
                ( mp.MedicareEntitlement.PatientEmployment.Status.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) == false &&
                mp.MedicareEntitlement.PatientEmployment.Status.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) == false ))
            {
                this.i_PatientIsNotEmployed = true;
                if (mp.MedicareEntitlement.PatientEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ))
                {
                    i_OCCCode18.OccurrenceDate = mp.MedicareEntitlement.PatientEmployment.RetiredDate;
                    Model_Account.AddOccurrenceCode( i_OCCCode18 );
                }
            }
        }

        private void CheckSpouseRetirement( MedicareSecondaryPayor mp )
        {
            if (mp != null
                && mp.MedicareEntitlement != null
                && mp.MedicareEntitlement.SpouseEmployment != null &&
                ( mp.MedicareEntitlement.SpouseEmployment.Status.Code.Equals( EmploymentStatus.EMPLOYED_FULL_TIME_CODE ) == false &&
                  mp.MedicareEntitlement.SpouseEmployment.Status.Code.Equals( EmploymentStatus.EMPLOYED_PART_TIME_CODE ) == false ))
            {
                this.i_SpouseIsNotEmployed = true;
                if (mp.MedicareEntitlement.SpouseEmployment.Status.Code.Equals( EmploymentStatus.RETIRED_CODE ))
                {
                    i_OCCCode19.OccurrenceDate = mp.MedicareEntitlement.SpouseEmployment.RetiredDate;
                    Model_Account.AddOccurrenceCode( i_OCCCode19 );
                }
            }
        }

        private void CheckFamilyMemberRetirement( MedicareSecondaryPayor mp )
        {
            if (mp != null
                && mp.MedicareEntitlement != null
                && !( mp.MedicareEntitlement is ESRDEntitlement ))
            {
                if (i_PatientIsNotEmployed && ( i_SpouseIsNotEmployed || i_FamilyMemberNotEmployed ))
                {
                    Model_Account.AddConditionCode( i_CondCode9 );
                }
            }
        }

        private void CheckAgeGHP( MedicareSecondaryPayor mp )
        {
            if (mp.MedicareEntitlement is AgeEntitlement)
            {
                if (( mp.MedicareEntitlement as AgeEntitlement ).GroupHealthPlanCoverage.Code == "Y")
                {
                    i_AgeGHPCoverage = true;
                }

                if (( i_PatientIsNotEmployed == false || i_SpouseIsNotEmployed == false ) && i_AgeGHPCoverage == false)
                {
                    Model_Account.AddConditionCode( i_CondCode10 );
                }
            }
        }

        private void CheckDisabilityGHP( MedicareSecondaryPayor mp )
        {
            if (mp.MedicareEntitlement is DisabilityEntitlement)
            {
                if (( mp.MedicareEntitlement as DisabilityEntitlement ).GroupHealthPlanCoverage.Code == YesNoFlag.CODE_YES)
                {
                    i_DisabilityGHPCoverage = true;
                }

                if (( mp.MedicareEntitlement as DisabilityEntitlement ).FamilyMemberGHPFlag.Code == YesNoFlag.CODE_YES)
                {
                    i_DisabilityGHPCoverage = true;
                }

                if ((i_PatientIsNotEmployed == false || i_SpouseIsNotEmployed == false) && i_DisabilityGHPCoverage == false)
                {
                    Model_Account.AddConditionCode( i_CondCode11 );
                }
            }
        }

        private void CheckAgeEmployeeNumForGHP( MedicareSecondaryPayor mp )
        {
            bool ghpLimitExceeded = false;

            if (mp.MedicareEntitlement is AgeEntitlement)
            {
                if (mp.MSPVersion == VERSION_1)
                {
                    if (( mp.MedicareEntitlement as AgeEntitlement ).GHPLimitExceeded.Code == "Y")
                    {
                        ghpLimitExceeded = true;
                    }
                }
                else
                {
                    if (( mp.MedicareEntitlement as AgeEntitlement ).GHPEmploysX.Code == YesNoFlag.CODE_YES
                        || ( mp.MedicareEntitlement as AgeEntitlement ).GHPSpouseEmploysX.Code == YesNoFlag.CODE_YES)
                    {
                        ghpLimitExceeded = true;
                    }
                }

                if (ghpLimitExceeded == false && i_AgeGHPCoverage)
                {
                    Model_Account.AddConditionCode( i_CondCode28 );
                }
            }
        }

        private void CheckDisabilityEmployeeNumForGHP( MedicareSecondaryPayor mp )
        {
            bool ghpLimitExceeded = false;

            if (mp.MedicareEntitlement is DisabilityEntitlement)
            {
                if (mp.MSPVersion == VERSION_1)
                {
                    if (( mp.MedicareEntitlement as DisabilityEntitlement ).GHPLimitExceeded.Code == YesNoFlag.CODE_YES)
                    {
                        ghpLimitExceeded = true;
                    }
                }
                else
                {
                    if (( mp.MedicareEntitlement as DisabilityEntitlement ).GHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES
                        || ( mp.MedicareEntitlement as DisabilityEntitlement ).SpouseGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES
                        || ( mp.MedicareEntitlement as DisabilityEntitlement ).FamilyMemberGHPEmploysMoreThanXFlag.Code == YesNoFlag.CODE_YES)
                    {
                        ghpLimitExceeded = true;
                    }
                }


                if (ghpLimitExceeded == false && i_DisabilityGHPCoverage)
                {
                    Model_Account.AddConditionCode( i_CondCode29 );
                }
            }
        }

        private void GetMSPRelatedOCCCondCodes()
        {
            OccuranceCodeBrokerProxy brokerProxy = new OccuranceCodeBrokerProxy();
            long facilityID = User.GetCurrent().Facility.Oid;
            i_OCCCode18 = brokerProxy.OccurrenceCodeWith( facilityID, OccurrenceCode.OCCURRENCECODE_RETIREDATE );
            i_OCCCode19 = brokerProxy.OccurrenceCodeWith( facilityID, OccurrenceCode.OCCURRENCECODE_SPOUSERETIRED );

            IConditionCodeBroker condBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();

            i_CondCode9 = condBroker.ConditionCodeWith( facilityID, ConditionCode.CONDITIONCODE_ALLMEMBERS_RETIRED );
            i_CondCode10 = condBroker.ConditionCodeWith( facilityID, ConditionCode.CONDITIONCODE_AGE_NO_GHP );
            i_CondCode11 = condBroker.ConditionCodeWith( facilityID, ConditionCode.CONDITIONCODE_DISABILITY_NO_GHP );
            i_CondCode28 = condBroker.ConditionCodeWith( facilityID, ConditionCode.CONDITIONCODE_AGE_GHP_LIMIT_NOT_EXCEED );
            i_CondCode29 = condBroker.ConditionCodeWith( facilityID, ConditionCode.CONDITIONCODE_DISABILITY_GHP_LIMIT_NOT_EXCEED );
        }

        private void DeleteMSPRelatedOCCCondCodes()
        {
            Model_Account.RemoveOccurrenceCode( i_OCCCode18 );
            Model_Account.RemoveOccurrenceCode( i_OCCCode19 );

            Model_Account.RemoveConditionCode( i_CondCode9 );
            Model_Account.RemoveConditionCode( i_CondCode10 );
            Model_Account.RemoveConditionCode( i_CondCode11 );
            Model_Account.RemoveConditionCode( i_CondCode28 );
            Model_Account.RemoveConditionCode( i_CondCode29 );
        }

        private static User GetUser()
        {
            User user = User.GetCurrent();
            user.Oid = 5;
            return user;
        }

        private void ResetPaymentValues()
        {
            Model_Account.TotalCurrentAmtDue = 0m;
            Model_Account.TotalPaid = 0m;
            Model_Account.NumberOfMonthlyPayments = 0;
            Model_Coverage.Deductible = 0;
            Model_Coverage.CoPay = 0;
            Model_Account.Payment = new Payment();
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.cmboFinancialClass = new PatientAccess.UI.CommonControls.PatientAccessComboBox();
            this.lblFinancialClass = new System.Windows.Forms.Label();
            this.grpMSP = new System.Windows.Forms.GroupBox();
            this.btnMspSummary = new LoggingButton();
            this.btnMspForm = new LoggingButton();
            this.grpDOB = new System.Windows.Forms.GroupBox();
            this.mtbFatherDOB = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblFatherDOB = new System.Windows.Forms.Label();
            this.mtbMotherDOB = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblMotherDOB = new System.Windows.Forms.Label();
            this.labelDOB = new System.Windows.Forms.Label();
            this.dofrInitiateView1 = new PatientAccess.UI.InsuranceViews.DOFR.ViewImpl.DOFRInitiateView();
            this.grpMSP.SuspendLayout();
            this.grpDOB.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmboFinancialClass
            // 
            this.cmboFinancialClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboFinancialClass.Enabled = false;
            this.cmboFinancialClass.Location = new System.Drawing.Point( 36, 32 );
            this.cmboFinancialClass.Name = "cmboFinancialClass";
            this.cmboFinancialClass.Size = new System.Drawing.Size( 218, 21 );
            this.cmboFinancialClass.TabIndex = 0;
            this.cmboFinancialClass.Validating += new System.ComponentModel.CancelEventHandler( this.cmboFinancialClass_Validating );
            this.cmboFinancialClass.SelectedIndexChanged += new System.EventHandler( this.FinancialClassesSelectedIndexChanged );
            // 
            // lblFinancialClass
            // 
            this.lblFinancialClass.Location = new System.Drawing.Point( 36, 16 );
            this.lblFinancialClass.Name = "lblFinancialClass";
            this.lblFinancialClass.Size = new System.Drawing.Size( 81, 15 );
            this.lblFinancialClass.TabIndex = 0;
            this.lblFinancialClass.Text = "Financial Class:";
            // 
            // grpMSP
            // 
            this.grpMSP.Controls.Add( this.btnMspSummary );
            this.grpMSP.Controls.Add( this.btnMspForm );
            this.grpMSP.Location = new System.Drawing.Point( 36, 277);
            this.grpMSP.Name = "grpMSP";
            this.grpMSP.Size = new System.Drawing.Size( 218, 80 );
            this.grpMSP.TabIndex = 2;
            this.grpMSP.TabStop = false;
            this.grpMSP.Text = "Medicare Secondary Payor";
            // 
            // btnMspSummary
            // 
            this.btnMspSummary.Enabled = false;
            this.btnMspSummary.Location = new System.Drawing.Point( 89, 32 );
            this.btnMspSummary.Name = "btnMspSummary";
            this.btnMspSummary.Size = new System.Drawing.Size( 120, 23 );
            this.btnMspSummary.TabIndex = 1;
            this.btnMspSummary.Text = "Vie&w MSP Summary";
            this.btnMspSummary.Click += new System.EventHandler( this.btnMspSummary_Click );
            // 
            // btnMspForm
            // 
            this.btnMspForm.Enabled = false;
            this.btnMspForm.Location = new System.Drawing.Point( 9, 32 );
            this.btnMspForm.Name = "btnMspForm";
            this.btnMspForm.TabIndex = 0;
            this.btnMspForm.Text = "M&SP Form";
            this.btnMspForm.Click += new System.EventHandler( this.btnMspForm_Click );
            // 
            // grpDOB
            // 
            this.grpDOB.Controls.Add( this.mtbFatherDOB );
            this.grpDOB.Controls.Add( this.lblFatherDOB );
            this.grpDOB.Controls.Add( this.mtbMotherDOB );
            this.grpDOB.Controls.Add( this.lblMotherDOB );
            this.grpDOB.Controls.Add( this.labelDOB );
            this.grpDOB.Location = new System.Drawing.Point( 36, 86 );
            this.grpDOB.Name = "grpDOB";
            this.grpDOB.Size = new System.Drawing.Size( 218, 184 );
            this.grpDOB.TabIndex = 1;
            this.grpDOB.TabStop = false;
            this.grpDOB.Text = "Dual coverage of a minor";
            // 
            // mtbFatherDOB
            // 
            this.mtbFatherDOB.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbFatherDOB.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbFatherDOB.Location = new System.Drawing.Point( 86, 146 );
            this.mtbFatherDOB.Mask = "  /  /";
            this.mtbFatherDOB.MaxLength = 10;
            this.mtbFatherDOB.Name = "mtbFatherDOB";
            this.mtbFatherDOB.Size = new System.Drawing.Size( 70, 20 );
            this.mtbFatherDOB.TabIndex = 1;
            this.mtbFatherDOB.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbFatherDOB.Validating += new System.ComponentModel.CancelEventHandler( this.mtbFatherDOB_Validating );
            // 
            // lblFatherDOB
            // 
            this.lblFatherDOB.Location = new System.Drawing.Point( 8, 149 );
            this.lblFatherDOB.Name = "lblFatherDOB";
            this.lblFatherDOB.Size = new System.Drawing.Size( 78, 16 );
            this.lblFatherDOB.TabIndex = 1;
            this.lblFatherDOB.Text = "Father\'s DOB:";
            // 
            // mtbMotherDOB
            // 
            this.mtbMotherDOB.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.mtbMotherDOB.KeyPressExpression = HelperClasses.DateValidator.DATEKeyPressExpression;
            this.mtbMotherDOB.Location = new System.Drawing.Point( 86, 120 );
            this.mtbMotherDOB.Mask = "  /  /";
            this.mtbMotherDOB.MaxLength = 10;
            this.mtbMotherDOB.Name = "mtbMotherDOB";
            this.mtbMotherDOB.Size = new System.Drawing.Size( 70, 20 );
            this.mtbMotherDOB.TabIndex = 0;
            this.mtbMotherDOB.ValidationExpression = HelperClasses.DateValidator.DATEValidationExpression;
            this.mtbMotherDOB.Validated += new System.EventHandler( this.mtbMotherDOB_Validated );
            // 
            // lblMotherDOB
            // 
            this.lblMotherDOB.Location = new System.Drawing.Point( 8, 123 );
            this.lblMotherDOB.Name = "lblMotherDOB";
            this.lblMotherDOB.Size = new System.Drawing.Size( 78, 16 );
            this.lblMotherDOB.TabIndex = 0;
            this.lblMotherDOB.Text = "Mother\'s DOB:";
            // 
            // labelDOB
            // 
            this.labelDOB.Location = new System.Drawing.Point( 8, 16 );
            this.labelDOB.Name = "labelDOB";
            this.labelDOB.Size = new System.Drawing.Size( 200, 88 );
            this.labelDOB.TabIndex = 0;
            this.labelDOB.Text = "For a minor who is covered under both parents\' insurance plans that both follow t" +
     "he birthday rule, the primary coverage is that of the parent whose birthday fall" +
     "s earliest in the year regardless of the birth year.";
            // 
            // dofrInitiateView1
            // 
            this.dofrInitiateView1.DOFRInitiatePresenter = null;
            this.dofrInitiateView1.Location = new System.Drawing.Point(39, 57);
            this.dofrInitiateView1.Margin = new System.Windows.Forms.Padding(0);
            this.dofrInitiateView1.Model = null;
            this.dofrInitiateView1.Name = "dofrInitiateView1";
            this.dofrInitiateView1.Size = new System.Drawing.Size(210, 28);
            this.dofrInitiateView1.TabIndex = 3;
            // 
            // FinancialClassesView
            // 
            this.Controls.Add(this.grpDOB);
            this.Controls.Add(this.grpMSP);
            this.Controls.Add(this.cmboFinancialClass);
            this.Controls.Add(this.lblFinancialClass);
            this.Controls.Add(this.dofrInitiateView1);
            this.Name = "FinancialClassesView";
            this.Size = new System.Drawing.Size( 296, 380 );
            this.Load += new System.EventHandler( this.FinancialClassesView_Load );
            this.grpMSP.ResumeLayout( false );
            this.grpDOB.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion

        #region Private Properties

        private IAutoCompleteNoLiabilityDueFeatureManager AutoCompleteNoLiabilityDueFeatureManager
        {
            get
            {
                return autoCompleteNoLiabilityDueFeatureManager ?? (autoCompleteNoLiabilityDueFeatureManager =
                           new AutoCompleteNoLiabilityDueFeatureManager());
            }
        }
        
        #endregion

        #region Construction and Finalization
        public FinancialClassesView()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }

        protected override void Dispose( bool disposing )
        {
            Application.DoEvents();

            this.UnRegisterRulesEvents();

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

        private Container components = null;

        private LoggingButton btnMspSummary;
        private LoggingButton btnMspForm;

        private PatientAccessComboBox cmboFinancialClass;

        private Label lblFinancialClass;
        private GroupBox grpMSP;

        private ICollection classCollection;
        private Account i_account;
        private MSPDialog msp;
        private MSP2Dialog msp2;
        private MedicareSecondaryPayor medPayor;

        private bool i_PatientIsNotEmployed = false;
        private bool i_SpouseIsNotEmployed = false;
        private bool i_FamilyMemberNotEmployed = false;
        private bool i_AgeGHPCoverage = false;
        private bool i_DisabilityGHPCoverage = false;
        private bool i_Registered = false;

        private OccurrenceCode i_OCCCode18 = null;
        private OccurrenceCode i_OCCCode19 = null;

        private ConditionCode i_CondCode9 = null;
        private ConditionCode i_CondCode10 = null;
        private ConditionCode i_CondCode11 = null;
        private ConditionCode i_CondCode28 = null;
        private ConditionCode i_CondCode29 = null;

        private GroupBox grpDOB;

        private Label labelDOB;
        private Label lblMotherDOB;
        private Label lblFatherDOB;

        private MaskedEditTextBox mtbFatherDOB;
        private MaskedEditTextBox mtbMotherDOB;

        private int birthMonth;
        private int birthDay;
        private int birthYear;

        private DateTime earliestDate = new DateTime( 1800, 01, 01 );
        private DateTime i_MSP2StartDate;
        private IAutoCompleteNoLiabilityDueFeatureManager autoCompleteNoLiabilityDueFeatureManager;
        public FinancialClass selectedFinancialClass;

        #endregion

        #region Constants

        private const string FINANCIAL_AGREEMENT = "72";
        private const string SELF_PAY = "73";
        private const string MED_MSE_SCREEN_EXM = "37";
        private DOFR.ViewImpl.DOFRInitiateView dofrInitiateView1;
        private const int VERSION_1 = 1;
        #endregion

        private void btnDOFRInitiate_Click(object sender, EventArgs e)
        {

        }
    }
}
