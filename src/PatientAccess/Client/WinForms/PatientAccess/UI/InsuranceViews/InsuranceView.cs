using System;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.InsuranceViews
{
    public class InsuranceView : ControlView
    {
        #region Events
        public event EventHandler SetTabPageEvent;
        #endregion

        #region Event Handlers

        private void InsuranceView_Leave( object sender, EventArgs e )
        {
            blnLeaveRun = true;
            AccountView.GetInstance().EnableInsuranceTab = false;
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCode ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCodeChange ), Model_Account );
            blnLeaveRun = false;
        }

        private void InsuranceView_Enter( object sender, EventArgs e )
        {
            AccountView.GetInstance().EnableInsuranceTab = false;
        }

        /// <summary>
        /// When the user updates the coverage
        /// </summary>
        private void CatchCoverageEvent( ICollection coverages )
        {
            primaryCoverageView.ResetView();
            secondaryCoverageView.ResetView();
            financialClassesView.ResetMSPGroupView();
            financialClassesView.DOFRInitiatePresenter.UpdateView();

            foreach ( Coverage coverage in coverages )
            {
                if ( coverage == null )
                {
                    continue;
                }

                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    savedPrimaryModelCoverage = coverage.DeepCopy();
                    primaryCoverageView.Model = coverage;
                    primaryCoverageView.UpdateView();

                    financialClassesView.Model = coverage;
                    financialClassesView.UpdateView();
                }
                else if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    savedSecondaryModelCoverage = coverage.DeepCopy();

                    secondaryCoverageView.Model = coverage;
                    secondaryCoverageView.UpdateView();
                }
            }

            RunRules();
            financialClassesView.RunRules();
        }

        private void CatchCoverageResetClickedEvent( Coverage aCoverage )
        {
            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID )
            {
                Model_Account.DeletedSecondaryCoverage = aCoverage;
            }

            Model_Account.Insurance.RemoveCoverage( aCoverage );

            if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
            {
                // remove the coverage from the FC view
                financialClassesView.Model_Coverage = null;
                financialClassesView.ResetFinancialClass();
                // reset HIC on 2ndary 
            }

            MSPQuestionaireRequired rule = new MSPQuestionaireRequired();

            // find the remaining coverage (if any) and determine if MSP should be cleared

            bool isMedicare = false;

            if ( Model_Account.Insurance.Coverages.Count > 0 )
            {
                long covOrder;
                if ( aCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID )
                {
                    covOrder = CoverageOrder.SECONDARY_OID;
                }
                else
                {
                    covOrder = CoverageOrder.PRIMARY_OID;
                }

                Coverage remainingCoverage = Model_Account.Insurance.CoverageFor( covOrder );

                isMedicare = rule.InsuranceIsMedicare( Model_Account.FinancialClass,
                    Model_Account.KindOfVisit,
                    Model_Account.HospitalService,
                    remainingCoverage );
            }

            if ( !isMedicare )
            {
                Model_Account.MedicareSecondaryPayor = new MedicareSecondaryPayor();

                financialClassesView.ResetMSPGroupView();
            }

            RunRules();

            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions();
        }

        private void CatchPlanSelectedEvent( Coverage newCoverage )
        {
            newCoverage.IsNew = true;
            newCoverage.Account.Facility = User.GetCurrent().Facility;

            // The user has chosen a new InsurancePlan... this means that the existing
            // coverage instance is no longer valid.  We need to capture the CoverageOrder
            // (so we know if it's Primary or Secondary), capture the chosen Plan,
            // remove the old coverage and add a new one (based on the Category associated
            // with the newly chosen plan)!

            bool showCoverageDialog = false;

            // default just to intantiate

            Coverage oldCoverage = new OtherCoverage();

            // See if patient already had a plan and it is different from the selected plan

            if ( newCoverage.CoverageOrder.Oid == CoverageOrder.PRIMARY_OID
                && savedPrimaryModelCoverage != null
                && newCoverage.InsurancePlan.PlanID != savedPrimaryModelCoverage.InsurancePlan.PlanID )
            {
                oldCoverage = savedPrimaryModelCoverage;
                showCoverageDialog = true;
            }
            else if ( newCoverage.CoverageOrder.Oid == CoverageOrder.SECONDARY_OID
                && savedSecondaryModelCoverage != null
                && newCoverage.InsurancePlan.PlanID != savedSecondaryModelCoverage.InsurancePlan.PlanID )
            {
                oldCoverage = savedSecondaryModelCoverage;
                showCoverageDialog = true;
            }

            if ( showCoverageDialog )
            {
                // PlanChangeDialog will give the user the opportunity to import   
                // some of the information on the old coverage to the new coverage,
                // or cancel the change completely.
                PlanChangeDialog dialog = new PlanChangeDialog( oldCoverage, newCoverage, Model_Account );

                try
                {
                    dialog.ShowDialog( this );

                    if ( dialog.DialogResult == DialogResult.OK )
                    {
                        // Update the saved plan ID
                        if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                        {
                            Model_Account.Insurance.RemovePrimaryCoverage();
                            Model_Account.Insurance.AddCoverage( newCoverage );
                        }
                        else if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                        {
                            Model_Account.Insurance.RemoveSecondaryCoverage();
                            Model_Account.Insurance.AddCoverage( newCoverage );
                        }
                    }
                    else // Cancel change
                    {
                        if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                        {
                            Model_Account.Insurance.RemovePrimaryCoverage();
                            Model_Account.Insurance.AddCoverage( savedPrimaryModelCoverage );
                        }
                        else if ( newCoverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                        {
                            Model_Account.Insurance.RemoveSecondaryCoverage();
                            Model_Account.Insurance.AddCoverage( savedSecondaryModelCoverage );
                        }
                    }
                }
                finally
                {
                    dialog.Dispose();
                }
            }
            else
            {
                Model_Account.Insurance.AddCoverage( newCoverage );
            }


            UpdateViewDetail();
            financialClassesView.RunRules();

            RuleEngine.RegisterEvent<MedicarePatientHasHMO>(
                HandleRuleHmoIsSecondary );
            RuleEngine.EvaluateRule<MedicarePatientHasHMO>(
                Model_Account.Insurance );
            RuleEngine.UnregisterEvent<MedicarePatientHasHMO>(
                HandleRuleHmoIsSecondary );

            // Re-set the previously scanned documents icons/menu options as the plan category may have changed

            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions();

        }

        /// <summary>
        /// private handler for rule: MedicarePatientHasHMOEventHandler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void HandleRuleHmoIsSecondary( object sender, EventArgs eventArgs )
        {

            MessageBox.Show( UIErrorMessages.MEDICARE_WITH_MEDICARE_HMO,
                             "Warning",
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Exclamation,
                             MessageBoxDefaultButton.Button1 );

        }
        private void FinancialClassesViewFinancialClassSelectedEvent( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Fired from FinancialClassesView when the MSP Wizard does a button click
        /// </summary>
        private void TabSelectedEventHandler( object sender, EventArgs e )
        {
            if ( SetTabPageEvent != null )
            {
                LooseArgs args = ( LooseArgs )e;
                int index = ( int )args.Context;
                SetTabPageEvent( this, new LooseArgs( index ) );
            }
        }

        //---------------------Evaluate ComboBoxes -------------------------------------------------------------
        private void financialClassesView_FinancialClassValidating( object sender, CancelEventArgs e )
        {
            if ( !blnLeaveRun )
            {
                UIColors.SetNormalBgColor( financialClassesView.ComboBoxFinClass );
                Refresh();
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCode ), Model );
                RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCodeChange ), Model );
            }
        }

        //--------------------- InvalidValues in Comboboxes Event Handlers: ------------------------------------

        private static void ProcessInvalidCodeEvent( PatientAccessComboBox comboBox )
        {
            UIColors.SetDeactivatedBgColor( comboBox );

            MessageBox.Show( UIErrorMessages.INVALID_VALUE_ERRMSG, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button1 );

            if ( !comboBox.Focused )
            {
                comboBox.Focus();
            }
        }
        #endregion

        #region Rule Event Handlers
        private void PlanIDInPrimaryDisplayPreferredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetPreferredBgColor( primaryCoverageView.lblPlanID );
            UIColors.SetPreferredBgColor( primaryCoverageView.lblPlanName );
        }

        private void PlanIDInPrimaryDisplayRequiredEventHandler( object sender, EventArgs e )
        {
            UIColors.SetRequiredBgColor( primaryCoverageView.lblPlanID );
            UIColors.SetRequiredBgColor( primaryCoverageView.lblPlanName );
        }

        private void InvalidFinancialClassCodeChangeEventHandler( object sender, EventArgs e )
        {
            ProcessInvalidCodeEvent( financialClassesView.ComboBoxFinClass );
        }

        private void InvalidFinancialClassCodeEventHandler( object sender, EventArgs e )
        {
            UIColors.SetDeactivatedBgColor( financialClassesView.ComboBoxFinClass );
        }

        private void MSPQuestionaireRequiredEventHandler( object sender, EventArgs e )
        {
            financialClassesView.MspSummaryEnabled = false;
        }
        private void FinancialClassPreferredEventHandler( object sender, EventArgs e )
        {
            if ( financialClassesView.ComboBoxFinClass.Enabled )
            {
                UIColors.SetPreferredBgColor( financialClassesView.ComboBoxFinClass );
            }
        }
        private void FinancialClassRequiredEventHandler( object sender, EventArgs e )
        {
            if ( financialClassesView.ComboBoxFinClass.Enabled )
            {
                UIColors.SetRequiredBgColor( financialClassesView.ComboBoxFinClass );
            }
        }
        #endregion

        #region Methods
        public override void UpdateView()
        {
            UpdateViewDetail();
            Visible = true;
        }

        public override void UpdateModel()
        {
        }

        public void SetDefaultFocus()
        {
            primaryCoverageView.SetDefaultFocus();
        }

        #endregion

        public void RegisterRequiredFieldSummaryTabSelectedEvent()
        {
            EventHandler eventHandler = TabSelectedEventHandler;
            IRequiredFieldsSummaryPresenter requiredFieldsSummaryPresenter = AccountView.GetInstance().RequiredFieldsSummaryPresenter;

            if ( !requiredFieldsSummaryPresenter.IsTabEventRegistered( eventHandler ) )
            {
                requiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
            }
        }

        #region Properties

        public Account Model_Account
        {
            get
            {
                return ( Account )Model;
            }
            set
            {
                Model = value;
            }
        }
        #endregion

        #region Private Methods

        private void UpdateViewDetail()
        {
            Insurance insurance = Model_Account.Insurance;

            primaryCoverageView.Account = Model_Account;
            secondaryCoverageView.Account = Model_Account;
            primaryCoverageView.UpdateView();
            secondaryCoverageView.UpdateView();

            // The account is needed in order to save the Mother's and Father's DOB 
            // entered in the FinancialClassesView, to the Account.Patient object 
            financialClassesView.Model_Account = Model_Account;
            financialClassesView.DOFRInitiatePresenter.UpdateView();
            if ( insurance == null )
            {   // TODO: Add error log message here
                return;
            }
            // Iterate over the coverage collection and populate
            // the primary and secondary insurance screens.

            ICollection coverageCollection = insurance.Coverages;

            if ( coverageCollection == null )
            {
                return;
            }

            foreach ( Coverage coverage in coverageCollection )
            {
                if ( coverage == null )
                {
                    continue;
                }
                if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.PRIMARY_OID ) )
                {
                    savedPrimaryModelCoverage = coverage.DeepCopy();

                    // The account is needed in order to populate the 'CopyPartyView' 
                    // in the InsuranceDetails dialog box which is invoked from the
                    // InsuredSummaryView's Edit button.

                    primaryCoveragePlanID.Append( coverage.InsurancePlan.PlanID );
                    primaryCoverageView.Model = coverage;
                    primaryCoverageView.UpdateView();

                    financialClassesView.Model = coverage;
                    financialClassesView.UpdateView();
                }
                else if ( coverage.CoverageOrder.Oid.Equals( CoverageOrder.SECONDARY_OID ) )
                {
                    savedSecondaryModelCoverage = coverage.DeepCopy();

                    secondaryCoveragePlanID.Append( coverage.InsurancePlan.PlanID );
                    secondaryCoverageView.Model = coverage;
                    secondaryCoverageView.UpdateView();

                    financialClassesView.UpdateView();
                }
            }

           
            RunRules();
        }

        private void RegisterRulesEvents()
        {
            if ( rulesRegistered )
            {
                return;
            }

            rulesRegistered = true;

            RuleEngine.LoadRules( Model_Account );

            RuleEngine.GetInstance().RegisterEvent( typeof( PlanIDInPrimaryDisplayPreferred ), Model_Account, new EventHandler( PlanIDInPrimaryDisplayPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( PlanIDInPrimaryDisplayRequired ), Model_Account, new EventHandler( PlanIDInPrimaryDisplayRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassPreferred ), Model_Account, new EventHandler( FinancialClassPreferredEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( FinancialClassRequired ), Model_Account, new EventHandler( FinancialClassRequiredEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidFinancialClassCode ), Model_Account, new EventHandler( InvalidFinancialClassCodeEventHandler ) );
            RuleEngine.GetInstance().RegisterEvent( typeof( InvalidFinancialClassCodeChange ), Model_Account, new EventHandler( InvalidFinancialClassCodeChangeEventHandler ) );

            RuleEngine.GetInstance().RegisterEvent( typeof( MSPQuestionaireRequired ), Model_Account, new EventHandler( MSPQuestionaireRequiredEventHandler ) );
        }

        private void UnRegisterRulesEvents()
        {

            rulesRegistered = false;
            // UNREGISTER EVENTS             
            RuleEngine.GetInstance().UnregisterEvent( typeof( PlanIDInPrimaryDisplayPreferred ), Model_Account, PlanIDInPrimaryDisplayPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( PlanIDInPrimaryDisplayRequired ), Model_Account, PlanIDInPrimaryDisplayRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassPreferred ), Model_Account, FinancialClassPreferredEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( FinancialClassRequired ), Model_Account, FinancialClassRequiredEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidFinancialClassCode ), Model_Account, InvalidFinancialClassCodeEventHandler );
            RuleEngine.GetInstance().UnregisterEvent( typeof( InvalidFinancialClassCodeChange ), Model_Account, InvalidFinancialClassCodeChangeEventHandler );

            RuleEngine.GetInstance().UnregisterEvent( typeof( MSPQuestionaireRequired ), Model_Account, MSPQuestionaireRequiredEventHandler );
        }

        private void RunRules()
        {
            // reset all fields that might have error, preferred, or required backgrounds
            UnRegisterRulesEvents();
            RegisterRulesEvents();
            UIColors.SetNormalBgColor( primaryCoverageView.lblPlanID );
            UIColors.SetNormalBgColor( primaryCoverageView.lblPlanName );

            UIColors.SetNormalBgColor( secondaryCoverageView.lblPlanID );
            UIColors.SetNormalBgColor( secondaryCoverageView.lblPlanName );

            UIColors.SetNormalBgColor( financialClassesView.ComboBoxFinClass );

            RuleEngine.GetInstance().EvaluateRule( typeof( PlanIDInPrimaryDisplayPreferred ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( PlanIDInPrimaryDisplayRequired ), Model_Account );

            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassRequired ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( FinancialClassPreferred ), Model_Account );

            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCode ), Model_Account );
            RuleEngine.GetInstance().EvaluateRule( typeof( InvalidFinancialClassCodeChange ), Model_Account );


            RuleEngine.GetInstance().EvaluateRule( typeof( MSPQuestionaireRequired ), Model_Account );
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelFinancial = new System.Windows.Forms.Panel();
            this.financialClassesView = new PatientAccess.UI.InsuranceViews.FinancialClassesView();
            this.panelCoverage = new System.Windows.Forms.Panel();
            this.panelSecondary = new System.Windows.Forms.Panel();
            this.panelHorizontalLine = new System.Windows.Forms.Panel();
            this.secondaryCoverageView = new PatientAccess.UI.InsuranceViews.CoverageView( false );
            this.panelPrimary = new System.Windows.Forms.Panel();
            this.primaryCoverageView = new PatientAccess.UI.InsuranceViews.CoverageView( true );
            this.panelMain.SuspendLayout();
            this.panelFinancial.SuspendLayout();
            this.panelCoverage.SuspendLayout();
            this.panelSecondary.SuspendLayout();
            this.panelPrimary.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add( this.panelFinancial );
            this.panelMain.Controls.Add( this.panelCoverage );
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point( 0, 0 );
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size( 1024, 380 );
            this.panelMain.TabIndex = 0;
            // 
            // panelFinancial
            // 
            this.panelFinancial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelFinancial.Controls.Add( this.financialClassesView );
            this.panelFinancial.Location = new System.Drawing.Point( 720, 1 );
            this.panelFinancial.Name = "panelFinancial";
            this.panelFinancial.Size = new System.Drawing.Size( 302, 377 );
            this.panelFinancial.TabIndex = 3;
            // 
            // financialClassesView
            // 
            this.financialClassesView.Location = new System.Drawing.Point( 0, 0 );
            this.financialClassesView.Model = null;
            this.financialClassesView.Model_Account = null;
            this.financialClassesView.Model_Coverage = null;
            this.financialClassesView.Name = "financialClassesView";
            this.financialClassesView.Size = new System.Drawing.Size( 296, 380 );
            this.financialClassesView.TabIndex = 1;
            this.financialClassesView.FinancialClassSelected += new System.EventHandler( this.FinancialClassesViewFinancialClassSelectedEvent );
            this.financialClassesView.TabSelectedEvent += new System.EventHandler( this.TabSelectedEventHandler );
            this.financialClassesView.FinancialClassValidating += new System.ComponentModel.CancelEventHandler( this.financialClassesView_FinancialClassValidating );
            // 
            // panelCoverage
            // 
            this.panelCoverage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCoverage.Controls.Add( this.panelSecondary );
            this.panelCoverage.Controls.Add( this.panelPrimary );
            this.panelCoverage.Location = new System.Drawing.Point( 0, 1 );
            this.panelCoverage.Name = "panelCoverage";
            this.panelCoverage.Size = new System.Drawing.Size( 720, 377 );
            this.panelCoverage.TabIndex = 0;
            // 
            // panelSecondary
            // 
            this.panelSecondary.Controls.Add( this.panelHorizontalLine );
            this.panelSecondary.Controls.Add( this.secondaryCoverageView );
            this.panelSecondary.Location = new System.Drawing.Point( 0, 185 );
            this.panelSecondary.Name = "panelSecondary";
            this.panelSecondary.Size = new System.Drawing.Size( 720, 193 );
            this.panelSecondary.TabIndex = 2;
            // 
            // panelHorizontalLine
            // 
            this.panelHorizontalLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelHorizontalLine.Location = new System.Drawing.Point( 0, 0 );
            this.panelHorizontalLine.Name = "panelHorizontalLine";
            this.panelHorizontalLine.Size = new System.Drawing.Size( 720, 1 );
            this.panelHorizontalLine.TabIndex = 35;
            // 
            // secondaryCoverageView
            // 
            this.secondaryCoverageView.Account = null;
            this.secondaryCoverageView.BackColor = System.Drawing.Color.White;
            this.secondaryCoverageView.Location = new System.Drawing.Point( 0, 1 );
            this.secondaryCoverageView.Model = null;
            this.secondaryCoverageView.Model_Coverage = null;
            this.secondaryCoverageView.Name = "secondaryCoverageView";
            this.secondaryCoverageView.Size = new System.Drawing.Size( 760, 185 );
            this.secondaryCoverageView.TabIndex = 1;
            this.secondaryCoverageView.PlanSelectedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoverageDelegate( this.CatchPlanSelectedEvent );
            this.secondaryCoverageView.CoverageUpdatedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoveragesDelegate( this.CatchCoverageEvent );
            this.secondaryCoverageView.CoverageResetClickedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoverageDelegate( this.CatchCoverageResetClickedEvent );
            // 
            // panelPrimary
            // 
            this.panelPrimary.Controls.Add( this.primaryCoverageView );
            this.panelPrimary.Location = new System.Drawing.Point( 0, 0 );
            this.panelPrimary.Name = "panelPrimary";
            this.panelPrimary.Size = new System.Drawing.Size( 720, 185 );
            this.panelPrimary.TabIndex = 1;
            // 
            // primaryCoverageView
            // 
            this.primaryCoverageView.Account = null;
            this.primaryCoverageView.BackColor = System.Drawing.Color.White;
            this.primaryCoverageView.Location = new System.Drawing.Point( 0, 0 );
            this.primaryCoverageView.Model = null;
            this.primaryCoverageView.Model_Coverage = null;
            this.primaryCoverageView.Name = "primaryCoverageView";
            this.primaryCoverageView.Size = new System.Drawing.Size( 760, 185 );
            this.primaryCoverageView.TabIndex = 1;
            this.primaryCoverageView.PlanSelectedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoverageDelegate( this.CatchPlanSelectedEvent );
            this.primaryCoverageView.CoverageUpdatedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoveragesDelegate( this.CatchCoverageEvent );
            this.primaryCoverageView.CoverageResetClickedEvent += new PatientAccess.UI.InsuranceViews.CoverageView.CoverageDelegate( this.CatchCoverageResetClickedEvent );
            // 
            // InsuranceView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add( this.panelMain );
            this.Name = "InsuranceView";
            this.Size = new System.Drawing.Size( 1024, 380 );
            this.Leave += new System.EventHandler( this.InsuranceView_Leave );
            this.Enter += new System.EventHandler( this.InsuranceView_Enter );
            this.panelMain.ResumeLayout( false );
            this.panelFinancial.ResumeLayout( false );
            this.panelCoverage.ResumeLayout( false );
            this.panelSecondary.ResumeLayout( false );
            this.panelPrimary.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        #endregion

        #region Private Properties
        private RuleEngine RuleEngine
        {
            get
            {
                if ( i_RuleEngine == null )
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion

        #region Construction and Finalization
        public InsuranceView()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }

        protected override void Dispose( bool disposing )
        {
            if ( IsHandleCreated )
            {
                Application.DoEvents();
            }

            UnRegisterRulesEvents();

            if ( disposing )
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
        private Container components = null;

        private Panel panelMain;
        private Panel panelPrimary;
        private Panel panelSecondary;
        private Panel panelFinancial;
        private Panel panelCoverage;
        private Panel panelHorizontalLine;
        // Nested child view objects
        private CoverageView primaryCoverageView;
        private CoverageView secondaryCoverageView;

        private FinancialClassesView financialClassesView;

        private readonly StringBuilder primaryCoveragePlanID = new StringBuilder();
        private readonly StringBuilder secondaryCoveragePlanID = new StringBuilder();

        private RuleEngine i_RuleEngine = null;

        private Coverage savedPrimaryModelCoverage;
        private Coverage savedSecondaryModelCoverage;

        private bool rulesRegistered;
        private bool blnLeaveRun;

        #endregion

        #region Constants
        #endregion
    }
}
