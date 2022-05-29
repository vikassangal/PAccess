using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.PAIWalkinOutpatientCreation;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.InsuranceViews.FindInsurancePlan;

namespace PatientAccess.UI.InsuranceViews
{
    [Serializable]
    public class FindAPlanView : ControlView
    {
        #region Events
        public event EventHandler<SelectInsuranceArgs> PlanSelectedEvent;
        #endregion

        #region Event Handlers


        private static void AdmitDateRequiredForInsuranceSelectionEventHandler( object sender, EventArgs e )
        {
            var rule = (AdmitDateRequiredForInsuranceSelection) sender;

            if(rule != null && rule.ContextActivity != null &&
                (rule.ContextActivity is QuickAccountMaintenanceActivity ||
                rule.ContextActivity is QuickAccountCreationActivity))
            {
                MessageBox.Show(UIErrorMessages.QUICK_ADMIT_FOR_INSURANCE, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
            }
            else if (rule != null && rule.ContextActivity != null &&
                     (rule.ContextActivity is PAIWalkinOutpatientCreationActivity ))
            {
                MessageBox.Show(UIErrorMessages.WALKIN_ADMIT_FOR_INSURANCE, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);
            }
            else
            {
                MessageBox.Show(UIErrorMessages.ADMIT_FOR_INSURANCE, "Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
         
        }
        
        private void NoMedicarePrimaryPayorForAutoAccidentEventHandler( object sender, EventArgs e )
        {
            IErrorMessageDisplayHandler messageDisplayHandler = new ErrorMessageDisplayHandler( PatientAccount );
            messageDisplayHandler.DisplayOkWarningMessageFor( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {
            RegisterRulesEvents();

            if( !RunRules() )
            {
                UnRegisterRulesEvents();
                return;
            }

            UnRegisterRulesEvents();

            i_FindInsurancePlanView = new FindInsurancePlanView( PatientAccount );
            i_FindInsurancePlanView.IsPrimary = IsPrimary;

            try
            {
                DialogResult result = i_FindInsurancePlanView.ShowDialog( this );

                if( result == DialogResult.OK )
                {
                    // Display message if user selects a non-Medicare secondary payor where the patient 
                    // is over 65 and the primary payor is not Medicare.
                    if( IsMedicareAdvisedFor( i_FindInsurancePlanView.SelectedInsurancePlan ) )
                    {
                        MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_MSG,
                            UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE, 
                            MessageBoxButtons.OK, MessageBoxIcon.Information );
                    }

                    MtbPlanSearchEntry.UnMaskedText = String.Empty;
                    // If an employer plan was selected, set a flag that's used to determine the
                    // state of the PlanChangeDialog's "Insured" checkbox state.
                    i_insuredCheckboxState = i_FindInsurancePlanView.CurrentView.ToString().Equals(
                        "PatientAccess.UI.InsuranceViews.FindInsurancePlan.SelectByEmployerView" );
                    OnPlanSelected( i_FindInsurancePlanView.SelectedInsurancePlan, i_FindInsurancePlanView.SelectedEmployer );
                }
            }
            finally
            {
                i_FindInsurancePlanView.Dispose();
            }
        }

        private bool IsMedicareAdvisedFor( InsurancePlan plan )
        {
            Patient patient = PatientAccount.Patient;
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            int patientAge = patientBroker.PatientAgeFor( patient.DateOfBirth, patient.Facility.Code );

            Coverage primaryCoverage = PatientAccount.Insurance.PrimaryCoverage;

            // Display message if user selects a non-Medicare secondary payor where the patient 
            // is over 65 and the primary payor is not Medicare.
            if( !IsPrimary
                && ( primaryCoverage != null && primaryCoverage.GetType() != typeof( GovernmentMedicareCoverage ) )
                && ( plan != null && plan.GetType() != typeof( GovernmentMedicareInsurancePlan ) )
                && ( patientAge == AGE_SIXTY_FIVE || patientAge > AGE_SIXTY_FIVE ) )
            {
                return true;
            }
            
            return false;
        }

		private void mtbPlanSearchEntry_Validating(object sender, CancelEventArgs e)
		{
			UIColors.SetNormalBgColor( MtbPlanSearchEntry );
		}

        private void btnVerify_Click( object sender, EventArgs e )
        {
            UIColors.SetNormalBgColor( MtbPlanSearchEntry );

            if( MtbPlanSearchEntry.Text.Length == 0 )
            {
                return;
            }

            RegisterRulesEvents();

            if( !RunRules() )
            {
                UnRegisterRulesEvents();
                return;
            }

            UnRegisterRulesEvents();

            if( MtbPlanSearchEntry.Text.Length != 5 
                && MtbPlanSearchEntry.Text.Length != 0)
            {
                MtbPlanSearchEntry.Focus();
                UIColors.SetErrorBgColor( MtbPlanSearchEntry );
                MessageBox.Show( FINDPLAN_ERRMSG, "Error", MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1  );
                return;
            }

            IInsuranceBroker broker = 
                BrokerFactory.BrokerOfType<IInsuranceBroker>();

            InsurancePlan plan = null;

            if( broker != null && PatientAccount != null 
                && PatientAccount.Facility != null)
            {
                string searchForId = MtbPlanSearchEntry.Text.Trim();
                if( InsurancePlan.VerifyForGenericAndMasterPlans( searchForId ) )
                {
                    plan = broker.PlanWith( searchForId, PatientAccount.Facility.Oid, PatientAccount.AdmitDate );
                }

                if (plan != null && plan.IsValidFor(PatientAccount.AdmitDate))
                {
                    bool planIsValid = plan.IsValidPlanForAdmitDate(PatientAccount.AdmitDate);
                    if (!planIsValid)
                    {
                        ExpiredPlanContractDialog expiredPlanContractDialog;
                        DialogResult result;

                        using (expiredPlanContractDialog = new ExpiredPlanContractDialog())
                        {
                            expiredPlanContractDialog.Model = plan;
                            expiredPlanContractDialog.UpdateView();
                            result = expiredPlanContractDialog.ShowDialog();
                        }
                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }
                    MtbPlanSearchEntry.UnMaskedText = String.Empty;

                    if (IsPrimary)
                    {
                        PatientAccount.EMPIPrimaryInvalidPlanID = String.Empty;
                    }
                    else
                    {
                        PatientAccount.EMPISecondaryInvalidPlanID = String.Empty;
                    }

                    if (PatientAccount.Insurance != null &&
                        PatientAccount.Insurance.PrimaryCoverage != null)
                    {
                        PreviousSelectedInsurancePlan = PatientAccount.Insurance.PrimaryCoverage.InsurancePlan;
                    }

                    bool evaluateNoMedicarePrimaryPayorForAutoAccidentRule = ShouldEvaluateNoMedicarePrimaryPayorForAutoAccidentRule(plan);

                    if (evaluateNoMedicarePrimaryPayorForAutoAccidentRule)
                    {
                        EvaluateNoMedicarePrimaryPayorForAutoAccidentRule(plan, NoMedicarePrimaryPayorForAutoAccidentEventHandler);
                        PreviousSelectedInsurancePlan = plan;
                    }

                    OnPlanSelected(plan, null);
                }
                else
                {
                    Activity ContextActivity = PatientAccount.Activity;
                    if (ContextActivity != null &&
                        (ContextActivity is QuickAccountMaintenanceActivity ||
                         ContextActivity is QuickAccountCreationActivity) ||
                        ContextActivity  is PAIWalkinOutpatientCreationActivity )
                    {
                        MessageBox.Show(UIErrorMessages.ADMIT_DATE_PLAN_INVALID_FOR_QUICK_ACCOUNT, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show(UIErrorMessages.ADMIT_DATE_PLAN_INVALID, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            if( IsMedicareAdvisedFor( plan ) )
            {
                MessageBox.Show( UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_MSG, UIErrorMessages.AGE_ABOVE_SIXTY_FIVE_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
        }

        private bool ShouldEvaluateNoMedicarePrimaryPayorForAutoAccidentRule(InsurancePlan plan)
        {
            return IsPrimary &&
                   ( PreviousSelectedInsurancePlan == null ||
                     ( PreviousSelectedInsurancePlan != null && 
                       PreviousSelectedInsurancePlan.PlanID != plan.PlanID ) );
        }

        internal bool EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( InsurancePlan plan, EventHandler eventHandler )
        {
            RuleEngine.RegisterEvent<NoMedicarePrimaryPayorForAutoAccident>( PatientAccount,
                PreviousSelectedInsurancePlan, eventHandler );
            bool result = RuleEngine.EvaluateRule<NoMedicarePrimaryPayorForAutoAccident>( PatientAccount, plan );
            RuleEngine.UnregisterEvent<NoMedicarePrimaryPayorForAutoAccident>( PatientAccount, eventHandler );

            return result;
        }

        #endregion

        #region Methods

        public void ResetView()
        {
            UIColors.SetNormalBgColor( MtbPlanSearchEntry );
            MtbPlanSearchEntry.Text = String.Empty;
        }

        public void SetDefaultFocus()
        {
            MtbPlanSearchEntry.Focus();
        }
        #endregion

        #region Properties

        public bool IsPrimary
        {
            private get
            {
                return i_IsPrimary;
            }
            set
            {
                i_IsPrimary = value;
            }
        }

        public Account PatientAccount
        {
            private get
            {
                return i_PatientAccount;
            }
            set
            {
                i_PatientAccount = value;
            }
        }

        public bool EmployerPlanSelected
        {
            get
            {
                return i_insuredCheckboxState;
            }
        }
        #endregion

        #region Private Methods
        private void OnPlanSelected( InsurancePlan aPlan, Employer anEmployer )
        {
            if ( PlanSelectedEvent != null )
            {
                PlanSelectedEvent( this, new SelectInsuranceArgs( aPlan, anEmployer ) );
                ValidateInsurancePlanFor( aPlan );
            }
        }

        private void RegisterRulesEvents()
        {            
            RuleEngine.RegisterEvent<AdmitDateRequiredForInsuranceSelection>( 
                PatientAccount, 
                Name, 
                AdmitDateRequiredForInsuranceSelectionEventHandler );
            
            RuleEngine.RegisterEvent<NoMedicarePrimaryPayorForAutoAccident>( 
                PatientAccount, 
                PreviousSelectedInsurancePlan,
                NoMedicarePrimaryPayorForAutoAccidentEventHandler );
        }

        private void UnRegisterRulesEvents()
        {
            RuleEngine.UnregisterEvent<AdmitDateRequiredForInsuranceSelection>( PatientAccount, AdmitDateRequiredForInsuranceSelectionEventHandler );
          
            RuleEngine.UnregisterEvent<NoMedicarePrimaryPayorForAutoAccident>( PatientAccount, NoMedicarePrimaryPayorForAutoAccidentEventHandler );
        }

        private bool RunRules()
        {
            bool passedAllRules = 
                RuleEngine.EvaluateRule<AdmitDateRequiredForInsuranceSelection>( PatientAccount, Name );

            return passedAllRules;
        }
 
        private void ValidateInsurancePlanFor( InsurancePlan plan )
        {
            //if( plan != null && this.PatientAccount != null )
			if( IsPrimary && plan != null && PatientAccount != null )
            {
                Condition cond = PatientAccount.Diagnosis.Condition;
                Accident accident = null;

                if( cond.GetType() == typeof(Accident) )
                {
                    accident = ( Accident )cond;
                }

                if( ( accident != null
                    && accident.Kind != null
                    && accident.Kind.Oid == TypeOfAccident.EMPLOYMENT_RELATED ) 
                    && plan.PlanCategory != null
                    && plan.PlanCategory.Oid != InsurancePlanCategory.PLANCATEGORY_WORKERS_COMPENSATION  )
                {
                    MessageBox.Show( INVALID_PLAN_SELECTED, "Warning", MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1 );
                }
            }
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btnSearch = new LoggingButton();
            this.btnVerify = new LoggingButton();
            this.MtbPlanSearchEntry = new Extensions.UI.Winforms.MaskedEditTextBox();
            this.lblStaticByPayor = new System.Windows.Forms.Label();
            this.lblStaticByPlanId = new System.Windows.Forms.Label();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.btnSearch);
            this.groupBox.Controls.Add(this.btnVerify);
            this.groupBox.Controls.Add(this.MtbPlanSearchEntry);
            this.groupBox.Controls.Add(this.lblStaticByPayor);
            this.groupBox.Controls.Add(this.lblStaticByPlanId);
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(263, 75);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Find a Plan";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(157, 46);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search...";
            this.btnSearch.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // btnVerify
            // 
            this.btnVerify.Location = new System.Drawing.Point(125, 14);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.TabIndex = 1;
            this.btnVerify.Text = "Verify";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // mtbPlanSearchEntry
            // 
            this.MtbPlanSearchEntry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.MtbPlanSearchEntry.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
            this.MtbPlanSearchEntry.KeyPressExpression = "[a-zA-Z0-9]*";
            this.MtbPlanSearchEntry.Location = new System.Drawing.Point(69, 15);
            this.MtbPlanSearchEntry.Mask = "";
            this.MtbPlanSearchEntry.MaxLength = 5;
            this.MtbPlanSearchEntry.Name = "mtbPlanSearchEntry";
            this.MtbPlanSearchEntry.Size = new System.Drawing.Size(48, 20);
            this.MtbPlanSearchEntry.TabIndex = 0;
            this.MtbPlanSearchEntry.ValidationExpression = "[a-zA-Z0-9]*";
			this.MtbPlanSearchEntry.Validating += new CancelEventHandler(mtbPlanSearchEntry_Validating);
            // 
            // lblStaticByPayor
            // 
            this.lblStaticByPayor.Location = new System.Drawing.Point(10, 50);
            this.lblStaticByPayor.Name = "lblStaticByPayor";
            this.lblStaticByPayor.Size = new System.Drawing.Size(151, 14);
            this.lblStaticByPayor.TabIndex = 1;
            this.lblStaticByPayor.Text = "By Payor/Broker or Employer";
            this.lblStaticByPayor.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblStaticByPlanId
            // 
            this.lblStaticByPlanId.Location = new System.Drawing.Point(10, 18);
            this.lblStaticByPlanId.Name = "lblStaticByPlanId";
            this.lblStaticByPlanId.Size = new System.Drawing.Size(61, 20);
            this.lblStaticByPlanId.TabIndex = 0;
            this.lblStaticByPlanId.Text = "By Plan ID:";
            this.lblStaticByPlanId.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // FindAPlanView
            // 
            this.AcceptButton = this.btnVerify;
            this.Controls.Add(this.groupBox);
            this.Name = "FindAPlanView";
            this.Size = new System.Drawing.Size(263, 75);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties

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

        private InsurancePlan PreviousSelectedInsurancePlan
        {
            get
            {
                return i_PreviousSelectedInsurancePlan;
            }
            set
            {
                i_PreviousSelectedInsurancePlan = value;
            }
        }
        #endregion

        #region Construction and Finalization
        public FindAPlanView()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }

        public FindAPlanView( object model ) : this()
        {
            Model = model;
            UpdateView();
        }

        protected override void Dispose( bool disposing )
        {
            if( IsHandleCreated )
            {
                Application.DoEvents();
            }
            
            if( disposing )
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
        private Container               components = null;
        private GroupBox                groupBox;
        private Label                   lblStaticByPlanId;
        private Label                   lblStaticByPayor;
        public MaskedEditTextBox MtbPlanSearchEntry { get; set; }
        private LoggingButton           btnVerify;
        private LoggingButton           btnSearch;
        private RuleEngine              i_RuleEngine;
        private Account                 i_PatientAccount;
        private bool                    i_insuredCheckboxState;
        private FindInsurancePlanView   i_FindInsurancePlanView;
        private bool                    i_IsPrimary = true;
        private InsurancePlan           i_PreviousSelectedInsurancePlan;

        #endregion

        #region Constants
        const int AGE_SIXTY_FIVE            = 65;
        const string FINDPLAN_ERRMSG        = "The Plan ID must contain 5 characters.";
        const string INVALID_PLAN_SELECTED  = "The primary insurance coverage you selected is not a workers compensation plan. " + 
            "But the Diagnosis screen indicates that this account visit is the result of an \"employment-related\" accident. " + 
            "Review the accident type and the insurance coverage to determine whether this inconsistency is appropriate for this account.";
		#endregion
	}
}
