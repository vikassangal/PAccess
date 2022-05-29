using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Extensions.PersistenceCommon;
using Extensions.UI.Builder;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Factories;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PhysicianSearchViews;
using PatientAccess.UI.Registration;  
using PatientAccess.UI.RegulatoryViews.ViewImpl;

namespace PatientAccess.UI.PreMSEViews
{
    /// <summary>
    /// Summary description for PreMseRegistrationView.
    /// </summary>
    [Serializable]
    public class PreMseRegistrationView : ControlView
    {
        #region Events
        #endregion

        #region Event Handlers

        void RequiredFieldSummary_TabSelectedEvent( object sender, EventArgs e )
        {
            int index = ( int )( ( LooseArgs )e ).Context;

            // go to a tab...
            if ( index == Convert.ToInt32( AccountView.ScreenIndexes.REGULATORY ) )
                index = PREMSE_REGULATORY; //Regulatory Tab for Pre MSE
            tabControl.SelectedIndex = index;
        }

        private void RegisterNonStaffPhysicianTabSelectedEvent()
        {
            EventHandler eventHandler = SetNonStaffPhysicianTabPageEventHandler;

            if ( !RequiredFieldsSummaryPresenter.IsTabEventRegistered( eventHandler ) )
            {
                RequiredFieldsSummaryPresenter.TabSelectedEvent += eventHandler;
            }
        }

        /// <summary>
        /// Fired from RequiredFieldsSummaryView when NPI details of a NonStaff Physician is not completed.
        /// </summary>
        private void SetNonStaffPhysicianTabPageEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = ( LooseArgs )e;
            int selectedScreenIndex = ( int )args.Context;

            if ( !AccountView.IsNonStaffPhysicianScreenIndex( selectedScreenIndex ) ) return;

            tabControl.SelectedIndex = 1;   // Contact & Diagnosis tab

            Cursor = Cursors.WaitCursor;
            PhysicianSearchFormView physicianSearchForm =
                PhysicianSearchFormView.GetPhysicianSearchForm( ( AccountView.ScreenIndexes )selectedScreenIndex, Model_Account );

            physicianSearchForm.UpdateView();
            physicianSearchForm.ShowDialog( this );
            Cursor = Cursors.Default;

            diagnosisView.Model = Model_Account;
            diagnosisView.UpdateView();
        }

        private void DiagnosisView_PhysicianSelectionLeaveEvent( object sender, EventArgs e )
        {
            AcceptButton = btnFinish;
            Refresh();
        }

        private void PreMseRegistrationView_Load( object sender, EventArgs e )
        {
        }

        private void TabControl_SelectedIndexChanged( object sender, EventArgs e )
        {
            UpdateView();
        }

        private void ButtonCancel_Click( object sender, EventArgs e )
        {
            if ( AccountActivityService.ConfirmCancelActivity(
                sender, new LooseArgs( Model_Account ) ) )
            {
                Dispose();
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        private void ButtonBack_Click( object sender, EventArgs e )
        {
            if ( !CheckForError() )
            {
                if ( tabControl.SelectedIndex != 0 )
                {
                    tabControl.SelectedIndex = tabControl.SelectedIndex - 1;
                }
            }
        }

        private void ButtonNext_Click( object sender, EventArgs e )
        {
            if ( !CheckForError() )
            {
                if ( tabControl.SelectedIndex != tabControl.TabCount )
                {
                    tabControl.SelectedIndex = tabControl.SelectedIndex + 1;
                }
            }
        }

        private void ButtonFinish_Click( object sender, EventArgs e )
        {
            Cursor = Cursors.WaitCursor;

            if ( !RuleEngine.GetInstance().EvaluateAllRules( Model_Account ) )
            {
                invalidCodeFieldsDialog = new InvalidCodeFieldsDialog();
                invalidCodeOptionalFieldsDialog = new InvalidCodeOptionalFieldsDialog();
                string inValidCodesSummary = RuleEngine.GetInstance().GetInvalidCodeFieldSummary();

                if ( inValidCodesSummary != String.Empty )
                {
                    if ( Model_Account.DischargeDate == DateTime.MinValue )
                    {
                        invalidCodeFieldsDialog.ErrorText = inValidCodesSummary;
                        invalidCodeFieldsDialog.UpdateView();

                        try
                        {
                            invalidCodeFieldsDialog.ShowDialog( this );
                        }
                        finally
                        {
                            Cursor = Cursors.Default;
                            invalidCodeFieldsDialog.Dispose();
                            invalidCodeOptionalFieldsDialog.Dispose();
                        }
                        btnFinish.Enabled = true;
                        return;
                    }

                    invalidCodeOptionalFieldsDialog.ErrorText = inValidCodesSummary;
                    invalidCodeOptionalFieldsDialog.UpdateView();

                    try
                    {
                        invalidCodeOptionalFieldsDialog.ShowDialog( this );
                        btnFinish.Enabled = true;
                    }
                    finally
                    {
                        invalidCodeOptionalFieldsDialog.Dispose();
                        invalidCodeFieldsDialog.Dispose();
                    }
                }
            }

            string summary = RuleEngine.GetInstance().GetRemainingErrorsSummary();

            if ( summary != String.Empty )
            {
                requiredFieldDialog = new RequiredFieldsDialog();
                requiredFieldDialog.Title = "Warning for Remaining Errors";
                requiredFieldDialog.HeaderText = "The fields listed have values that are creating errors. Visit each field\r\n"
                    + "listed and correct the value before completing this activity.";
                requiredFieldDialog.ErrorText = summary;

                try
                {
                    requiredFieldDialog.ShowDialog( this );
                    Cursor = Cursors.Default;
                    RuleEngine.GetInstance().ClearActions();
                    RunRulesForTab();
                    btnFinish.Enabled = true;
                }
                finally
                {
                    requiredFieldDialog.Dispose();
                }
                return;
            }

            ICollection<CompositeAction> collection = RuleEngine.GetInstance().GetCompositeItemsCollection();

            if ( collection.Count > 0
                && RuleEngine.AccountHasRequiredFields( collection ) )
            {
                requiredFieldSummaryView = new RequiredFieldsSummaryView();
                RequiredFieldsSummaryPresenter = new RequiredFieldsSummaryPresenter( requiredFieldSummaryView, collection )
                                                 {
                                                     Model = Model_Account,
                                                     Header = RequiredFieldsSummaryPresenter.REQUIRED_FIELDS_HEADER
                                                 };

                RequiredFieldsSummaryPresenter.TabSelectedEvent += RequiredFieldSummary_TabSelectedEvent;

                RegisterNonStaffPhysicianTabSelectedEvent();

                try
                {
                    RequiredFieldsSummaryPresenter.ShowViewAsDialog( this );
                    RuleEngine.GetInstance().ClearActions();
                    RunRulesForTab();
                    Cursor = Cursors.Default;
                    btnFinish.Enabled = true;
                }
                finally
                {
                    requiredFieldSummaryView.Dispose();
                }
                return;
            }

            if ( !RuleEngine.GetInstance().AccountHasFailedError() )
            {
                try
                {
                    if ( SaveAccount() )
                    {
                        Model_Account.Patient.SetPatientContextHeaderData();
                        ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model_Account ) );

                        DisplayConfirmationScreen();
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void ConfirmationView_EditAccount( object sender, EventArgs e )
        {
            Cursor storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                EnableAccountView();

                if ( e != null )
                {

                    Account realAccount = ( Account )( ( LooseArgs )e ).Context;

                    if ( realAccount.IsShortRegistered )
                    {
                        realAccount.Activity = new ShortMaintenanceActivity();
                    }
                    else
                    {
                        realAccount.Activity = new MaintenanceActivity();
                    }

                    LooseArgs args = new LooseArgs( realAccount );
                    SearchEventAggregator.GetInstance().RaiseAccountSelectedEvent( this, args );
                    ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, args );
                }
            }
            finally
            {
                Cursor = storedCursor;
                Dispose();
            }
        }

        /// <summary>
        /// fire the close activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmationView_CloseActivity( object sender, EventArgs e )
        {
            if ( ParentForm != null && ParentForm.GetType() == typeof( PatientAccessView ) )
            {
                ( ( PatientAccessView )ParentForm ).ReLoad();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// UpdateView method.
        /// </summary>
        public override void UpdateView()
        {
            contextLabel.Description = GetPreMSETitle();

            if ( Model_Account != null )
            {
                if ( loadingModelData )
                {
                    if (Model_Account.IsUrgentCarePreMse )
                    {
                        Model_Account.KindOfVisit = VisitType.UCCOutpatient;
                    }
                    else
                    {
                        Model_Account.KindOfVisit = new VisitType(4L, PersistentModel.NEW_VERSION,
                            VisitType.EMERGENCY_PATIENT_DESC, VisitType.EMERGENCY_PATIENT);
                    }
                    Model_Account.FinancialClass = new FinancialClass( 296L, PersistentModel.NEW_VERSION, "MED (MSE) SCREEN EXM", FC_37 );
                    loadingModelData = false;
                }
                demographicsView.Enabled = true;
                diagnosisView.Enabled = true;
                regulatoryView.Enabled = true;
                patientContextView.Model = Model_Account.Patient;
                Model_Account.Patient.SetPatientContextHeaderData();
                patientContextView.Account = Model_Account;
                patientContextView.UpdateView();
            }
            else
            {
                Cursor = Cursors.Default;
                return;
            }

            if ( tabControl.SelectedTab == tabDemographics )
            {
                demographicsView.Model = Model_Account;
                demographicsView.UpdateView();
                btnBack.Enabled = false;
                btnNext.Enabled = true;
                btnFinish.Enabled = true;
                AcceptButton = btnNext;
                demographicsView.Focus();

                BreadCrumbLogger.GetInstance.Log( "select demographics tab ", Model_Account );
            }
            else if ( tabControl.SelectedTab == tabDiagnosis )
            {
                diagnosisView.Model = Model_Account;
                diagnosisView.UpdateView();
                btnBack.Enabled = true;
                btnNext.Enabled = true;
                btnFinish.Enabled = true;
                AcceptButton = btnFinish;

                BreadCrumbLogger.GetInstance.Log( "select diagnosis tab ", Model_Account );
            }
            else if ( tabControl.SelectedTab == tabRegulatory )
            {
                regulatoryView.Model = Model_Account;
                regulatoryView.IsCosRequired = false;
                regulatoryView.UpdateView();
                regulatoryView.Focus();
                btnBack.Enabled = true;
                btnNext.Enabled = false;
                btnFinish.Enabled = true;
            }

            // SR 41094 - January 2008 Release
            // Initialize icons/menu options for previosly scanned documents

            ViewFactory.Instance.CreateView<PatientAccessView>().Model = Model_Account;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetPreviousDocumentOptions( Model_Account );

            Cursor = Cursors.Default;
        }

        public void SetContextView(string contextDescription)
        {
            contextLabel.Description = contextDescription;
        }

        #endregion

        #region Private And Protected Methods
        
        private string GetPreMSETitle()
        {

            string result;
          
            if ( Model_Account != null )
            {
                if (Model_Account.IsUrgentCarePreMse )
                {
                    if (Model_Account.IsNew)
                    {
                        result = Model_Account.Activity.ContextDescription;
                    }
                    else
                    {
                        result = "Edit Urgent Care Pre-MSE Information";
                    }
                }
                else
                {
                    if (Model_Account.IsNew)
                    {
                        result = "Register ED Patient Pre-MSE";
                    }
                    else
                    {
                        result = "Edit Pre-MSE Demographic Information";
                    }
                }
            }
            else
            {
                result = "Register ED Patient Pre-MSE";
            }

            return result;
        }

        private static bool CheckForError()
        {
            return RuleEngine.GetInstance().AccountHasFailedError();
        }

        protected override void WndProc( ref Message m )
        {

            const uint WM_NOTIFY = 0x004E;
            const uint TCN_FIRST = 0xFFFFFDDA;
            const uint TCN_SELCHANGING = TCN_FIRST - 2;

            base.WndProc( ref m );

            switch ( ( uint )m.Msg )
            {
                case WM_NOTIFY:
                    {
                        if ( progressPanel1.Visible )
                        {
                            const int loading = 1;
                            m.Result = ( IntPtr )loading;
                            return;
                        }



                        NMHDR nm = new NMHDR();
                        nm.hwndFrom = IntPtr.Zero;
                        nm.idFrom = 0;
                        nm.code = 0;

                        int idCtrl = ( int )m.WParam;
                        NMHDR nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            bool thereWasAnError = CheckForError();
                            int irc = 0;
                            if ( thereWasAnError )
                            {
                                irc = 1;
                            }

                            Convert.ToInt32( thereWasAnError );
                            m.Result = ( IntPtr )irc;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        protected override bool ProcessCmdKey( ref Message msg, Keys keyData )
        {
            if ( ( keyData == Keys.Right ) && ( tabControl.Focused ) )
            {
                TabControl_SelectedIndexChanged( this, null );
            }
            else if ( ( keyData == Keys.Left ) && ( tabControl.Focused ) )
            {
                if ( tabControl.Focused )
                {
                    TabControl_SelectedIndexChanged( this, null );
                }
            }

            return base.ProcessCmdKey( ref msg, keyData );
        }

        private void HideControls()
        {
            foreach ( Control control in Controls )
            {
                if ( control != null )
                {
                    control.Hide();
                }
            }
        }

        /// <summary>
        /// fire the repeat activity event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmationView_RepeatActivity( object sender, EventArgs e )
        {
            if (this.Model_Account.Activity.IsCreateUCCPreMSEActivity() )
            {
                SearchEventAggregator.GetInstance().RaiseUCCPreMseRegistrationEvent(this, null);
            }
            else if (this.Model_Account.Activity.IsEditUCCPreMSEActivity())
            {
                SearchEventAggregator.GetInstance().RaiseEditUCPreMseRegistrationEvent(this, null);
            }
             else if (this.Model_Account.Activity.IsEditPreMseActivity())
            {
                SearchEventAggregator.GetInstance().RaiseEditEDPreMseRegistrationEvent(this, null);
            }
            else
            {
                SearchEventAggregator.GetInstance().RaisePreMseRegistrationEvent(this, null);
            }
            Dispose();
        }


        private void RunRulesForTab()
        {
            if ( tabControl.SelectedTab == tabDemographics )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( OnPatientDemographicsForm ), Model_Account );
            }
            else if ( tabControl.SelectedTab == tabDiagnosis )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( OnContactAndDiagnosisForm ), Model_Account );
            }
            else if ( tabControl.SelectedTab == tabRegulatory )
            {
                RuleEngine.GetInstance().EvaluateRule( typeof( OnRegulatoryForm ), Model_Account );
            }
        }

        /// <summary>
        /// display a confirmation screen if the activate was successful
        /// </summary>

        private void DisplayConfirmationScreen()
        {
            HideControls();

            Controls.Add( confirmationView );
            confirmationView.Dock = DockStyle.Fill;
            confirmationView.Model = Model as Account;
            confirmationView.UpdateView();
            confirmationView.RepeatActivity += ConfirmationView_RepeatActivity;
            confirmationView.CloseActivity += ConfirmationView_CloseActivity;
            confirmationView.EditAccount += ConfirmationView_EditAccount;

            confirmationView.Show();
        }

        private void EnableAccountView()
        {
            accountView = AccountView.NewInstance();

            SuspendLayout();
            accountView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( ( Control )accountView );
        }

        /// <summary>
        /// Save Account
        /// </summary>
        private bool SaveAccount()
        {
            bool rc = true;

            Model_Account.Facility = User.GetCurrent().Facility;
            Account anAccount = new Account();

            Activity currentActivity = Model_Account.Activity;
            currentActivity.AppUser = User.GetCurrent();
            CoverageDefaults coverageDefaults = new CoverageDefaults();
            coverageDefaults.SetCoverageDefaultsForActivity( anAccount );

            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            if ( broker != null )
            {
                AccountSaveResults results = broker.Save( Model_Account, currentActivity );
                results.SetResultsTo( Model_Account );
            }
            else
            {
                rc = false;
            }

            return rc;
        }

        #region Properties

        public ProgressPanel ProgressPanel
        {
            get { return progressPanel1; }
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contextLabel = new UserContextView();
            this.panelPatientContext = new Panel();
            this.patientContextView = new PatientContextView();
            this.tabControl = new TabControl();
            this.tabDemographics = new TabPage();
            this.progressPanel1 = new ProgressPanel();
            this.demographicsView = new PreMseDemographicsView();
            this.tabDiagnosis = new TabPage();
            this.diagnosisView = new PreMseDiagnosisView();
            this.tabRegulatory = new TabPage();
            this.regulatoryView = new RegulatoryView();
            this.panelButtons = new Panel();
            this.btnCancel = new ClickOnceLoggingButton();
            this.btnBack = new LoggingButton();
            this.btnNext = new LoggingButton();
            this.btnFinish = new ClickOnceLoggingButton();
            this.panelPatientContext.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabDemographics.SuspendLayout();
            this.tabDiagnosis.SuspendLayout();
            this.tabRegulatory.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextLabel
            // 
            this.contextLabel.BackColor = Color.FromArgb( ( ( int )( ( ( byte )( 128 ) ) ) ), ( ( int )( ( ( byte )( 162 ) ) ) ), ( ( int )( ( ( byte )( 200 ) ) ) ) );
            this.contextLabel.Description = " Register ED Patient Pre-MSE";
            this.contextLabel.Dock = DockStyle.Top;
            this.contextLabel.ForeColor = Color.White;
            this.contextLabel.Location = new Point( 0, 0 );
            this.contextLabel.Model = null;
            this.contextLabel.Name = "contextLabel";
            this.contextLabel.Size = new Size( 1024, 23 );
            this.contextLabel.TabIndex = 0;
            this.contextLabel.TabStop = false;
            // 
            // panelPatientContext
            // 
            this.panelPatientContext.BorderStyle = BorderStyle.FixedSingle;
            this.panelPatientContext.Controls.Add( this.patientContextView );
            this.panelPatientContext.Location = new Point( 8, 24 );
            this.panelPatientContext.Name = "panelPatientContext";
            this.panelPatientContext.Size = new Size( 1008, 26 );
            this.panelPatientContext.TabIndex = 0;
            // 
            // patientContextView
            // 
            this.patientContextView.Account = null;
            this.patientContextView.BackColor = Color.White;
            this.patientContextView.DateOfBirthText = "";
            this.patientContextView.Dock = DockStyle.Fill;
            this.patientContextView.GenderLabelText = "";
            this.patientContextView.Location = new Point( 0, 0 );
            this.patientContextView.Model = null;
            this.patientContextView.Name = "patientContextView";
            this.patientContextView.PatientNameText = "";
            this.patientContextView.Size = new Size( 1006, 24 );
            this.patientContextView.SocialSecurityNumber = "";
            this.patientContextView.TabIndex = 0;
            this.patientContextView.TabStop = false;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add( this.tabDemographics );
            this.tabControl.Controls.Add( this.tabDiagnosis );
            this.tabControl.Controls.Add( this.tabRegulatory );
            this.tabControl.Location = new Point( 8, 55 );
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size( 1008, 524 );
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new EventHandler( this.TabControl_SelectedIndexChanged );
            // 
            // tabDemographics
            // 
            this.tabDemographics.Controls.Add( this.progressPanel1 );
            this.tabDemographics.Controls.Add( this.demographicsView );
            this.tabDemographics.Location = new Point( 4, 22 );
            this.tabDemographics.Name = "tabDemographics";
            this.tabDemographics.Size = new Size( 1000, 498 );
            this.tabDemographics.TabIndex = 0;
            this.tabDemographics.Text = "Demographics";
            this.tabDemographics.UseVisualStyleBackColor = true;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = Color.White;
            this.progressPanel1.Location = new Point( 0, 0 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new Size( 1008, 504 );
            this.progressPanel1.TabIndex = 1;
            this.progressPanel1.Visible = false;
            // 
            // demographicsView
            // 
            this.demographicsView.BackColor = Color.White;
            this.demographicsView.Enabled = false;
            this.demographicsView.Location = new Point( 0, 0 );
            this.demographicsView.Model = null;
            this.demographicsView.Name = "demographicsView";
            this.demographicsView.Size = new Size( 1000, 498 );
            this.demographicsView.TabIndex = 0;
            this.demographicsView.RefreshTopPanel += new System.EventHandler(this.demographicsView_RefreshTopPanel);
            // 
            // tabDiagnosis
            // 
            this.tabDiagnosis.Controls.Add( this.diagnosisView );
            this.tabDiagnosis.Location = new Point( 4, 22 );
            this.tabDiagnosis.Name = "tabDiagnosis";
            this.tabDiagnosis.Size = new Size( 1000, 498 );
            this.tabDiagnosis.TabIndex = 1;
            this.tabDiagnosis.Text = "Contact & Diagnosis";
            this.tabDiagnosis.UseVisualStyleBackColor = true;
            // 
            // diagnosisView
            // 
            this.diagnosisView.BackColor = Color.White;
            this.diagnosisView.Enabled = false;
            this.diagnosisView.Location = new Point( 0, 0 );
            this.diagnosisView.Model = null;
            this.diagnosisView.Name = "diagnosisView";
            this.diagnosisView.Size = new Size( 1000, 498 );
            this.diagnosisView.TabIndex = 0;
            this.diagnosisView.PhysicianSelectionLeaveEvent += new EventHandler( this.DiagnosisView_PhysicianSelectionLeaveEvent );
            // 
            // tabRegulatory
            // 
            this.tabRegulatory.BackColor = Color.White;
            this.tabRegulatory.Controls.Add( this.regulatoryView );
            this.tabRegulatory.Location = new Point( 4, 22 );
            this.tabRegulatory.Name = "tabRegulatory";
            this.tabRegulatory.Padding = new Padding( 3 );
            this.tabRegulatory.Size = new Size( 1000, 498 );
            this.tabRegulatory.TabIndex = 2;
            this.tabRegulatory.Text = "Regulatory";
            // 
            // regulatoryView
            // 
            this.regulatoryView.BackColor = Color.White;
            this.regulatoryView.Enabled = false;
            this.regulatoryView.Location = new Point( 8, 9 );
            this.regulatoryView.Model = null;
            this.regulatoryView.Name = "regulatoryView";
            this.regulatoryView.Size = new Size( 1024, 370 );
            this.regulatoryView.TabIndex = 0;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add( this.btnCancel );
            this.panelButtons.Controls.Add( this.btnBack );
            this.panelButtons.Controls.Add( this.btnNext );
            this.panelButtons.Controls.Add( this.btnFinish );
            this.panelButtons.Location = new Point( 0, 585 );
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new Size( 1024, 35 );
            this.panelButtons.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new Point( 686, 6 );
            this.btnCancel.Message = null;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size( 75, 23 );
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new EventHandler( this.ButtonCancel_Click );
            // 
            // btnBack
            // 
            this.btnBack.Location = new Point( 771, 6 );
            this.btnBack.Message = null;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new Size( 75, 23 );
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<   &Back";
            this.btnBack.Click += new EventHandler( this.ButtonBack_Click );
            // 
            // btnNext
            // 
            this.btnNext.Location = new Point( 851, 6 );
            this.btnNext.Message = null;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new Size( 75, 23 );
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "&Next   >";
            this.btnNext.Click += new EventHandler( this.ButtonNext_Click );
            // 
            // btnFinish
            // 
            this.btnFinish.Location = new Point( 937, 6 );
            this.btnFinish.Message = null;
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Size = new Size( 75, 23 );
            this.btnFinish.TabIndex = 5;
            this.btnFinish.Text = "Fini&sh";
            this.btnFinish.Click += new EventHandler( this.ButtonFinish_Click );
            // 
            // PreMseRegistrationView
            // 
            this.BackColor = Color.FromArgb( ( ( int )( ( ( byte )( 209 ) ) ) ), ( ( int )( ( ( byte )( 228 ) ) ) ), ( ( int )( ( ( byte )( 243 ) ) ) ) );
            this.Controls.Add( this.panelButtons );
            this.Controls.Add( this.tabControl );
            this.Controls.Add( this.panelPatientContext );
            this.Controls.Add( this.contextLabel );
            this.Name = "PreMseRegistrationView";
            this.Size = new Size( 1024, 620 );
            this.Load += new EventHandler( this.PreMseRegistrationView_Load );
            this.panelPatientContext.ResumeLayout( false );
            this.tabControl.ResumeLayout( false );
            this.tabDemographics.ResumeLayout( false );
            this.tabDiagnosis.ResumeLayout( false );
            this.tabRegulatory.ResumeLayout( false );
            this.panelButtons.ResumeLayout( false );
            this.ResumeLayout( false );
        }

        #endregion

        #endregion
        private void demographicsView_RefreshTopPanel(object sender, EventArgs e)
        {
            patientContextView.GenderLabelText = String.Empty;
            patientContextView.PatientNameText = String.Empty;

            if ((((LooseArgs)e).Context) != null)
            {
                if (((Patient)
                     (((LooseArgs)e).Context)).Sex.Description != String.Empty &&
                    ((Patient)
                     (((LooseArgs)e).Context)).Sex.Description != String.Empty)
                {
                    patientContextView.GenderLabelText = ((Patient)
                                                               (((LooseArgs)e).Context)).Sex.Description;
                }

                patientContextView.PatientNameText = ((Patient)
                                                           (((LooseArgs)e).Context)).Name.AsFormattedName();

                patientContextView.DateOfBirthText = ((Patient)
                                                           (((LooseArgs)e).Context)).DateOfBirth.Date.ToString
                    ("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo);

                patientContextView.SocialSecurityNumber = ((Patient)
                                                                (((LooseArgs)e).Context)).SocialSecurityNumber.
                    AsFormattedString();

                if (patientContextView.DateOfBirthText == DateTime.MinValue.ToString("MM/dd/yyyy"))
                {
                    patientContextView.DateOfBirthText = String.Empty;
                }
            }
        }

        #region Private Properties

        private Account Model_Account
        {
            get
            {
                return ( Account )Model;
            }
        }

        private RequiredFieldsSummaryPresenter RequiredFieldsSummaryPresenter { get; set; }

        #endregion

        #region Construction and Finalization
        public PreMseRegistrationView()
        {
            loadingModelData = true;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            EnableThemesOn( this );
            ResizeRedraw = true;

            btnCancel.Message = "Click cancel activity";
            btnFinish.Message = "Click finish activity";

            btnBack.Enabled = btnNext.Enabled = false;
            btnFinish.Enabled = false;
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
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
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private ClickOnceLoggingButton btnFinish;
        private LoggingButton btnNext;
        private LoggingButton btnBack;
        private ClickOnceLoggingButton btnCancel;

        private Panel panelPatientContext;
        private Panel panelButtons;

        private TabControl tabControl;

        private TabPage tabDemographics;
        private TabPage tabDiagnosis;
        private TabPage tabRegulatory;

        private IAccountView accountView = null;
        private RequiredFieldsDialog requiredFieldDialog;
        private RequiredFieldsSummaryView requiredFieldSummaryView;
        private InvalidCodeFieldsDialog invalidCodeFieldsDialog;
        private InvalidCodeOptionalFieldsDialog invalidCodeOptionalFieldsDialog;

        private PreMseDemographicsView demographicsView;
        private PreMseDiagnosisView diagnosisView;
        private UserContextView contextLabel;
        private PatientContextView patientContextView;
        private readonly RegisterConfirmationView confirmationView = new RegisterConfirmationView();
        private RegulatoryView regulatoryView;

        private ProgressPanel progressPanel1;
        private bool loadingModelData;

        #endregion

        #region Constants

        private const int PREMSE_REGULATORY = 2;
        private const string FC_37 = "37";

        #endregion
    }
}
