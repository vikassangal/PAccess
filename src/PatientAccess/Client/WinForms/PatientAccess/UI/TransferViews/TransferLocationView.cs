using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.Factories;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PhysicianSearchViews;
using log4net;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferLocationView.
    /// </summary>
    public class TransferLocationView : ControlView
    {
        #region Events

        //public event EventHandler ReturnToMainScreen;

        #endregion

        #region Event Handlers

        private void OutPatToInPatStep3View_TabSelectedEvent( object sender, EventArgs e )
        {
            int index = ( int )( ( LooseArgs )e ).Context;

            Controls[INDEX_OF_OutPatToInPatStep1View].Hide();
            Controls[INDEX_OF_OutPatToInPatStep2View].Hide();
            Controls[INDEX_OF_OutPatToInPatStep3View].Hide();

            OutPatToInPatStep1View.Enabled = false;
            OutPatToInPatStep2View.Enabled = false;
            OutPatToInPatStep3View.Enabled = false;
            PhysicianSearchFormView physicianSearchForm = new PhysicianSearchFormView();

            switch ( index )
            {
                case 0: // Transfer to

                    OutPatToInPatStep1View.Enabled = true;
                    Controls[INDEX_OF_OutPatToInPatStep1View].Show();
                    Controls[INDEX_OF_OutPatToInPatStep1View].Focus();
                    break;

                case 1: // Physicians

                    OutPatToInPatStep2View.Enabled = true;
                    Controls[INDEX_OF_OutPatToInPatStep2View].Show();
                    Controls[INDEX_OF_OutPatToInPatStep2View].Focus();
                    break;

                case 2: // Insurance Details

                    OutPatToInPatStep3View.Enabled = true;
                    Controls[INDEX_OF_OutPatToInPatStep3View].Show();
                    Controls[INDEX_OF_OutPatToInPatStep3View].Focus();
                    break;

                case 7: // Referring NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.REFERRING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage( physicianSearchForm );
                    break;

                case 8: // Attending NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ATTENDING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage( physicianSearchForm );
                    break;

                case 9: // Admitting NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.ADMITTING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage( physicianSearchForm );
                    break;

                case 10: // Operating NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.OPERATING_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage( physicianSearchForm );
                    break;

                case 11: // PrimaryCare NonStaff Physician

                    physicianSearchForm.PhysicianRelationshipToView = PhysicianRelationship.PRIMARYCARE_PHYSICIAN;
                    DisplayNonStaffPhysicianDialogPage( physicianSearchForm );
                    break;
            }
        }

        private void DisplayNonStaffPhysicianDialogPage( PhysicianSearchFormView physicianSearchForm )
        {
            OutPatToInPatStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatToInPatStep2View].Show();

            Cursor = Cursors.WaitCursor;
            physicianSearchForm.CallingObject = "VIEWDETAIL";
            physicianSearchForm.Model = Model as Account;
            physicianSearchForm.physicianSearchTabView1.tcPhysicianSearch.SelectedIndex =
                PhysicianSearchTabView.NON_STAFF_PHYSICIAN_TAB;
            physicianSearchForm.UpdateView();

            physicianSearchForm.ShowDialog( this );
            Cursor = Cursors.Default;

            OutPatToInPatStep2View.Model = Model as Account;
            OutPatToInPatStep2View.UpdateView();
        }

        private void BeforeWork()
        {
            storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            if ( CurrentActivity.GetType().Equals( typeof( TransferActivity ) ) )
            {
                AttachDetailView();
                DetailView.ShowPanel();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( TransferInToOutActivity ) ) )
            {
                AttachInPatToOutPatView();
                InPatToOutPatView.ShowPanel();
            }
            else if ( CurrentActivity.GetType().Equals( typeof( TransferOutToInActivity ) ) )
            {
                AttachOutPatToInPatView();
                OutPatToInPatStep1View.ShowPanel();
            }
        }

        private void DoWork( object sender, DoWorkEventArgs e )
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;

            AccountProxy proxy = SelectedAccount as AccountProxy;
            Account account = null;
            if ( proxy != null )
            {
                // TLG 05/21/2007 replace proxy.AsAccount with streamlined call

                account = AccountActivityService.SelectedAccountFor( proxy );

                account.Activity = CurrentActivity;

                // poll CancellationPending and set e.Cancel to true and return 
                if ( backgroundWorker.CancellationPending )
                {
                    e.Cancel = true;
                    return;
                }
            }

            Model = account;

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                if ( CurrentActivity.GetType().Equals( typeof( TransferActivity ) ) )
                {
                    DetailView.HidePanel();
                    DisplayDetailView( Model as Account );
                }
                else if ( CurrentActivity.GetType().Equals( typeof( TransferInToOutActivity ) ) )
                {
                    InPatToOutPatView.HidePanel();
                    DisplayInPatToOutPatView( Model as Account );
                }
                else if ( CurrentActivity.GetType().Equals( typeof( TransferOutToInActivity ) ) )
                {
                    OutPatToInPatStep1View.HidePanel();
                    DisplayOutPatToInPatView( Model as Account );
                }
            }

            Cursor = storedCursor;
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            SelectedAccount = ( ( LooseArgs )e ).Context as AccountProxy;

            if ( backgroundWorker == null ||
                !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void CloseViewEventHandler( object sender, EventArgs e )
        {
            //Raise ActivityComplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( Model ) );

            CancelBackgroundWorker();

            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            DisableIconsAndMenuOptions();

            Dispose();
            ActivityEventAggregator.GetInstance().RaiseReturnToMainScreen( this, null );
        }

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void RepeatActivityEventHandler( object sender, EventArgs e )
        {
            DisplayMasterPatientIndexView();
        }

        private void SwapRepeatActivityEventHandler( object sender, EventArgs e )
        {
            DisableIconsAndMenuOptions();
            AttachSwapPatLocView();
        }

        private static void EditAccountEventHandler( object sender, EventArgs e )
        {
            Account realAccount = ( Account )( ( LooseArgs )e ).Context;
            ViewFactory.Instance.CreateView<PatientAccessView>().ActivateTab( null, realAccount );
        }

        private void CancelEventHandler( object sender, EventArgs e )
        {
            ClearControls();

            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            DisableIconsAndMenuOptions();

            Dispose();
        }

        private void Step1NextEventHandler( object sender, EventArgs e )
        {
            Controls[INDEX_OF_OutPatToInPatStep1View].Hide();
            OutPatToInPatStep1View.Enabled = false;
            OutPatToInPatStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatToInPatStep2View].Show();
            Controls[INDEX_OF_OutPatToInPatStep2View].Focus();
        }

        private void Step2BackEventHandler( object sender, EventArgs e )
        {
            OutPatToInPatStep1View.Enabled = true;
            Controls[INDEX_OF_OutPatToInPatStep1View].Show();
            Controls[INDEX_OF_OutPatToInPatStep1View].Focus();
            Controls[INDEX_OF_OutPatToInPatStep2View].Hide();
            OutPatToInPatStep2View.Enabled = false;
        }

        private void Step2NextEventHandler( object sender, EventArgs e )
        {
            Controls[INDEX_OF_OutPatToInPatStep2View].Hide();
            OutPatToInPatStep2View.Enabled = false;
            OutPatToInPatStep3View.Enabled = true;
            Controls[INDEX_OF_OutPatToInPatStep3View].Show();
            Controls[INDEX_OF_OutPatToInPatStep3View].Focus();
        }

        private void Step3BackEventHandler( object sender, EventArgs e )
        {
            OutPatToInPatStep2View.Enabled = true;
            Controls[INDEX_OF_OutPatToInPatStep2View].Show();
            Controls[INDEX_OF_OutPatToInPatStep2View].Focus();
            Controls[INDEX_OF_OutPatToInPatStep3View].Hide();
            OutPatToInPatStep3View.Enabled = false;
        }

        private void Step3FinishEventHandler( object sender, EventArgs e )
        {
            // Validate Physicians for Transfer Out to In Activity on 'Finish' click
            bool isPhysicianValid = OutPatToInPatStep3View.PhysiciansValidated();

            if ( isPhysicianValid != true )
            {
                OutPatToInPatStep3View.EnableFinishButton();
                Controls[INDEX_OF_OutPatToInPatStep3View].Hide();

                OutPatToInPatStep2View.Enabled = true;
                Controls[INDEX_OF_OutPatToInPatStep2View].Show();

                return;
            }

            if ( !OutPatToInPatStep1View.HasUpdatedChiefComplaint() )
            {
                DialogResult action = TransferOutPatToInPatStep3View.ShowChiefComplaintWarning();

                if ( action == DialogResult.Yes )
                {
                    OutPatToInPatStep3View.EnableFinishButton();
                    Controls[INDEX_OF_OutPatToInPatStep3View].Hide();

                    OutPatToInPatStep1View.Enabled = true;
                    Controls[INDEX_OF_OutPatToInPatStep1View].Show();

                    OutPatToInPatStep1View.mtbChiefComplaint.Focus();
                }
                else
                {
                    WrapUpTransferOutPatToInPat( OutPatToInPatStep1View.Model );
                }
            }
            else
            {
                WrapUpTransferOutPatToInPat( OutPatToInPatStep1View.Model );
            }

            //Raise Activitycomplete event
            ActivityEventAggregator.GetInstance().RaiseActivityCompleteEvent( this, new LooseArgs( OutPatToInPatStep1View.Model ) );
        }
        #endregion

        #region public Methods

        #endregion

        #region Public Properties

        private Activity CurrentActivity
        {
            get
            {
                return i_CurrentActivity;
            }
            // next lines are temporary; once the dischargeView has been broken into each of its
            // controllers, this set accessor can be removed.
            set
            {
                i_CurrentActivity = value;
            }
        }

        #endregion

        #region Private Methods
        private static bool checkForError()
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

                        NMHDR nm = new NMHDR();
                        nm.hwndFrom = IntPtr.Zero;
                        nm.idFrom = 0;
                        nm.code = 0;

                        int idCtrl = ( int )m.WParam;
                        NMHDR nmh = ( NMHDR )m.GetLParam( typeof( NMHDR ) );

                        if ( nmh.code == TCN_SELCHANGING )
                        {
                            bool rc = checkForError();
                            int irc = 0;
                            if ( rc )
                            {
                                irc = 1;
                            }

                            Convert.ToInt32( rc );
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

        private static void DisableIconsAndMenuOptions()
        {
            // Set model to null and Disable all icons/menu options for FUS notes and Previously scanned documents
            ViewFactory.Instance.CreateView<PatientAccessView>().Model = null;
            ViewFactory.Instance.CreateView<PatientAccessView>().SetFUSNotesOptions( false );
            ViewFactory.Instance.CreateView<PatientAccessView>().DisablePreviousDocumentOptions();
        }

        private void DisplayMasterPatientIndexView()
        {
            ClearControls();
            DisableIconsAndMenuOptions();
            i_masterPatientIndexView = new MasterPatientIndexView();

            if ( !IsInDesignMode )
            {
                i_masterPatientIndexView.CurrentActivity = CurrentActivity;
            }

            i_masterPatientIndexView.Dock = DockStyle.Fill;
            Controls.Add( i_masterPatientIndexView );
            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
        }

        private void AttachMasterPatientIndexView()
        {
            if ( masterPatientIndexView == null )
            {
                i_masterPatientIndexView = new MasterPatientIndexView();
                if ( !IsInDesignMode )
                {
                    i_masterPatientIndexView.CurrentActivity = CurrentActivity;
                }
                i_masterPatientIndexView.Model = null;
                i_masterPatientIndexView.Name = "masterPatientIndexView";
                i_masterPatientIndexView.Dock = DockStyle.Fill;
            }

            SuspendLayout();
            ClearControls();
            Controls.Add( i_masterPatientIndexView );

            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;

            ResumeLayout( false );
        }

        private void ClearControls()
        {
            if ( Controls == null )
            {
                return;
            }

            foreach ( Control control in Controls )
            {
                if ( control != null )
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch ( Exception ex )
                    {
                        c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            Controls.Clear();
        }

        //for Transfering to new location
        private void AttachDetailView()
        {
            DetailView = new TransferLocationDetailView { Name = "detailView", Dock = DockStyle.Fill };

            SuspendLayout();
            ClearControls();
            Controls.Add( DetailView );

            DetailView.RepeatActivity += RepeatActivityEventHandler;
            DetailView.EditAccount += EditAccountEventHandler;
            DetailView.CloseView += CloseViewEventHandler;

            ResumeLayout( false );
        }

        private void DisplayDetailView( Account anAccount )
        {
            DetailView.Model = anAccount;
            DetailView.UpdateView();
            DetailView.Show();
        }

        private void AttachInPatToOutPatView()
        {
            // OTD 36167 - create new instance for InPatToOutPatView anyway since the old one is disposed
            InPatToOutPatView = new TransferInPatToOutPatView { Name = "InPatToOutPatView", Dock = DockStyle.Fill };

            SuspendLayout();
            ClearControls();
            Controls.Add( InPatToOutPatView );

            ResumeLayout( false );
        }

        private void DisplayInPatToOutPatView( Account anAccount )
        {
            InPatToOutPatView.RepeatActivity += RepeatActivityEventHandler;
            InPatToOutPatView.EditAccount += EditAccountEventHandler;
            InPatToOutPatView.CloseView += CloseViewEventHandler;

            InPatToOutPatView.Model = anAccount;
            InPatToOutPatView.UpdateView();
            InPatToOutPatView.Show();
        }

        private void AttachOutPatToInPatView()
        {
            OutPatToInPatStep1View = new TransferOutPatToInPatStep1View { Name = "OutPatToInPatStep1View", Dock = DockStyle.Fill };

            OutPatToInPatStep2View = new TransferOutPatToInPatStep2View { Name = "OutPatToInPatStep2View", Dock = DockStyle.Fill };

            OutPatToInPatStep3View = new TransferOutPatToInPatStep3View { Name = "OutPatToInPatStep3View", Dock = DockStyle.Fill };

            OutPatToInPatConfirmView = new TransferOutPatToInPatConfirmView { Name = "OutPatToInPatConfirmView", Dock = DockStyle.Fill };

            SuspendLayout();
            ClearControls();
            Controls.Add( OutPatToInPatStep1View );
            Controls.Add( OutPatToInPatStep2View );
            Controls.Add( OutPatToInPatStep3View );
            Controls.Add( OutPatToInPatConfirmView );

            OutPatToInPatStep3View.TabSelectedEvent += OutPatToInPatStep3View_TabSelectedEvent;

            ResumeLayout( false );
        }

        private void DisplayOutPatToInPatView( Account anAccount )
        {
            OutPatToInPatStep1View.CancelButtonClicked += CancelEventHandler;
            OutPatToInPatStep1View.NextButtonClicked += Step1NextEventHandler;

            OutPatToInPatStep2View.CancelButtonClicked += CancelEventHandler;
            OutPatToInPatStep2View.BackButtonClicked += Step2BackEventHandler;
            OutPatToInPatStep2View.NextButtonClicked += Step2NextEventHandler;

            OutPatToInPatStep3View.CancelButtonClicked += CancelEventHandler;
            OutPatToInPatStep3View.BackButtonClicked += Step3BackEventHandler;
            OutPatToInPatStep3View.FinishButtonClicked += Step3FinishEventHandler;

            OutPatToInPatConfirmView.CloseView += CloseViewEventHandler;
            OutPatToInPatConfirmView.RepeatActivity += RepeatActivityEventHandler;
            OutPatToInPatConfirmView.EditAccount += EditAccountEventHandler;

            OutPatToInPatStep1View.Model = anAccount;
            OutPatToInPatStep2View.Model = anAccount;
            OutPatToInPatStep3View.Model = anAccount;
            OutPatToInPatConfirmView.Model = anAccount;

            OutPatToInPatStep2View.UpdateView();
            OutPatToInPatStep3View.UpdateView();

            OutPatToInPatStep2View.Enabled = false;
            OutPatToInPatStep3View.Enabled = false;
            OutPatToInPatConfirmView.Enabled = false;

            OutPatToInPatStep1View.UpdateView();
            OutPatToInPatStep1View.Show();
        }

        private void AttachSwapPatLocView()
        {
            SwapPatLocView = new SwapPatientLocationsView { Name = "swapPatLocView", Dock = DockStyle.Fill };

            SuspendLayout();
            ClearControls();
            Controls.Add( SwapPatLocView );

            SwapPatLocView.RepeatActivity += SwapRepeatActivityEventHandler;
            SwapPatLocView.CloseView += CloseViewEventHandler;

            //Raise avtivity start event to PatientAccessView
            //Raise this event only for swap patient
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, EventArgs.Empty );

            SwapPatLocView.UpdateView();
            SwapPatLocView.Show();

            ResumeLayout( false );
        }

        private void BeforeSaveWork()
        {
            Cursor = Cursors.WaitCursor;
            OutPatToInPatStep3View.ShowPanel();

            OutPatToInPatStep2View.UpdateModel();
        }

        private void AfterSaveWork( object sender, RunWorkerCompletedEventArgs e )
        {
            OutPatToInPatStep3View.HidePanel();

            if ( e.Cancelled )
            {
                OutPatToInPatStep3View.ShowPanel();
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
            }
            else
            {
                // success
                OutPatToInPatConfirmView.Enabled = true;
                OutPatToInPatConfirmView.UpdateView();

                OutPatToInPatStep3View.Enabled = false;
                Controls[INDEX_OF_OutPatToInPatStep3View].Hide();

                Controls[INDEX_OF_OutPatToInPatConfirmView].Show();
                Controls[INDEX_OF_OutPatToInPatConfirmView].Focus();
            }

            Cursor = Cursors.Default;
        }

        private void DoWrapUpOutToIn( object sender, DoWorkEventArgs e )
        {
            OutPatToInPatStep1View.UpdateModel();
            OutPatToInPatStep3View.UpdateModel();

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            wrapUpAccount.Clinics.Clear();

            TransferService.QueueTransfer( wrapUpAccount );

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void WrapUpTransferOutPatToInPat( Account anAccount )
        {
            // SR 804 - Auto-generate Condition Code P7 when ER patient is transferred to Inpatient
            EmergencyToInPatientTransferCodeManager.UpdateConditionCodes();

            wrapUpAccount = anAccount;

            if ( backgroundWorker == null
                || ( !backgroundWorker.IsBusy ) )
            {
                BeforeSaveWork();

                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.DoWork += DoWrapUpOutToIn;
                backgroundWorker.RunWorkerCompleted += AfterSaveWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // TransferLocationView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "TransferLocationView";
            this.Size = new System.Drawing.Size( 16, 24 );

        }
        #endregion

        #endregion

        #region Private Properties
        private TransferLocationDetailView DetailView
        {
            get
            {
                return i_DetailView;
            }
            set
            {
                i_DetailView = value;
            }
        }

        private TransferInPatToOutPatView InPatToOutPatView
        {
            get
            {
                return i_InPatToOutPatView;
            }
            set
            {
                i_InPatToOutPatView = value;
            }
        }

        private TransferOutPatToInPatStep1View OutPatToInPatStep1View
        {
            get
            {
                return i_OutPatToInPatStep1View;
            }
            set
            {
                i_OutPatToInPatStep1View = value;
            }
        }

        private TransferOutPatToInPatStep2View OutPatToInPatStep2View
        {
            get
            {
                return i_OutPatToInPatStep2View;
            }
            set
            {
                i_OutPatToInPatStep2View = value;
            }
        }

        private TransferOutPatToInPatStep3View OutPatToInPatStep3View
        {
            get
            {
                return i_OutPatToInPatStep3View;
            }
            set
            {
                i_OutPatToInPatStep3View = value;
            }
        }

        private TransferOutPatToInPatConfirmView OutPatToInPatConfirmView
        {
            get
            {
                return i_OutPatToInPatConfirmView;
            }
            set
            {
                i_OutPatToInPatConfirmView = value;
            }
        }

        private SwapPatientLocationsView SwapPatLocView
        {
            get
            {
                return i_SwapPatLocView;
            }
            set
            {
                i_SwapPatLocView = value;
            }
        }

        private MasterPatientIndexView masterPatientIndexView
        {
            get
            {
                return i_masterPatientIndexView;
            }
        }

        private EmergencyToInPatientTransferCodeManager EmergencyToInPatientTransferCodeManager
        {
            get
            {
                 
                    var accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                    var conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
                    emergencyToInpatientTransferCodeManager = new EmergencyToInPatientTransferCodeManager(
                        DateTime.Parse( ConfigurationManager.AppSettings[ApplicationConfigurationKeys.ER_TO_IP_CONDITION_CODE_START_DATE] ),
                        OutPatToInPatStep1View.Model, 
                        accountBroker,
                        conditionCodeBroker);
                return emergencyToInpatientTransferCodeManager;
            }
        }

        #endregion

        #region Construction and Finalization

        public TransferLocationView( Activity currentActivity )
        {
            CurrentActivity = currentActivity;
            SuspendLayout();

            InitializeComponent();

            if ( CurrentActivity.GetType().Equals( typeof( TransferActivity ) )
            || CurrentActivity.GetType().Equals( typeof( TransferInToOutActivity ) )
            || CurrentActivity.GetType().Equals( typeof( TransferOutToInActivity ) ) )
            {
                AttachMasterPatientIndexView();
            }
            else
            {
                AttachSwapPatLocView();
            }

            ResumeLayout( false );
            EnableThemesOn( this );
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;

            if ( disposing )
            {
                if ( DetailView != null )
                {
                    DetailView.Dispose();
                }

                if ( InPatToOutPatView != null )
                {
                    InPatToOutPatView.Dispose();
                }

                if ( OutPatToInPatStep1View != null )
                {
                    OutPatToInPatStep1View.Dispose();
                }

                if ( SwapPatLocView != null )
                {
                    SwapPatLocView.Dispose();
                }

                if ( components != null )
                {
                    components.Dispose();
                }

                // cancel the background worker here...
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( TransferLocationView ) );

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private TransferLocationDetailView i_DetailView;
        private TransferInPatToOutPatView i_InPatToOutPatView;
        private TransferOutPatToInPatStep1View i_OutPatToInPatStep1View;
        private TransferOutPatToInPatStep2View i_OutPatToInPatStep2View;
        private TransferOutPatToInPatStep3View i_OutPatToInPatStep3View;
        private TransferOutPatToInPatConfirmView i_OutPatToInPatConfirmView;
        private SwapPatientLocationsView i_SwapPatLocView;

        private MasterPatientIndexView i_masterPatientIndexView;
        private Activity i_CurrentActivity;
        private Account wrapUpAccount;
        private IAccount SelectedAccount;
        private BackgroundWorker backgroundWorker;

        private Cursor storedCursor;
        private EmergencyToInPatientTransferCodeManager emergencyToInpatientTransferCodeManager;
        #endregion

        #region Constants

        private const int
            INDEX_OF_OutPatToInPatStep1View = 0,
            INDEX_OF_OutPatToInPatStep2View = 1,
            INDEX_OF_OutPatToInPatStep3View = 2,
            INDEX_OF_OutPatToInPatConfirmView = 3;
        #endregion
    }
}
