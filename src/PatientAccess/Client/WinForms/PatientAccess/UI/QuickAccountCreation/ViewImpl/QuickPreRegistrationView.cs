using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.Registration;
using log4net;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    [Serializable]
    public partial class QuickPreRegistrationView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            if ( selectedAccount != null )
            {
                if ( selectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = selectedAccount.Activity.ContextDescription;
                }
            }

            Cursor = Cursors.WaitCursor;
        }

        private void DoWork( object sender, DoWorkEventArgs e )
        {
            var newAccount = AccountActivityService.SelectedAccountFor( selectedAccount );
            newAccount.IsShortRegistered = selectedAccount.IsShortRegistered;
            newAccount.IsQuickRegistered = selectedAccount.IsQuickRegistered;
            if ( newAccount.Activity == null )
            {
                newAccount.Activity = CurrentActivity;
            }

            newAccount.Activity.AssociatedActivityType = typeof( PreRegistrationActivity );

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }

            // Sanjeev Kumar - defect 34899 - this appears to be related to the threading issue
            // Dean discovered. Place a lock on this object to prevent deadlock
            lock ( selectedAccount )
            {
                selectedAccount = newAccount;
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( IsDisposed || Disposing )
                return;

            if ( e.Cancelled )
            {
                // cancelled work here
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }

            else
            {
                // success
                Cursor = Cursors.Default;
                DisplayAccountViewWith( selectedAccount as Account );

                AccountView.GetInstance().HidePanel();
            }
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            if ( ( (LooseArgs)e ).Context != null )
            {
                selectedAccount = ( (LooseArgs)e ).Context as IAccount;
            }


            {
                EnableAccountView();
            }

            AccountView.GetInstance().ShowPanel();

            // make sure we are not editing a PreMSE account in the 12-tab view
            //if( SelectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE
            if ( selectedAccount != null &&
                 selectedAccount.FinancialClass != null &&
                 selectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE &&
                 selectedAccount.KindOfVisit != null &&
                 ( selectedAccount.KindOfVisit.IsEmergencyPatient || selectedAccount.KindOfVisit.IsOutpatient )
                )
            {
                return;
            }

            if ( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void ActivatePreregisteredAccountEventHandler( object sender, EventArgs e )
        {
            var storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                var accountProxy = ( (LooseArgs)e ).Context as AccountProxy;
                if ( accountProxy != null )
                {
                    accountProxy.Activity = new QuickAccountCreationActivity { AssociatedActivityType = typeof( ActivatePreRegistrationActivity ) };

                    var realAccount = accountProxy.AsAccount();

                    EnableAccountView();

                    if ( accountView != null )
                    {
                        accountView.Model = realAccount;
                        accountView.UpdateView();
                        accountView.Show();
                    }
                }
            }

            finally
            {
                Cursor = storedCursor;
            }
        }

        private void OnEditAccount( object sender, EventArgs e )
        {
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, e );
            AccountSelectedEventHandler( this, e );
        }

        private void OnRepeatActivity( object sender, EventArgs e )
        {
            DisplayMasterPatientIndexView();
        }

        private void DisplayAccountViewWith( Account anAccount )
        {
            if ( anAccount.Activity == null )
            {
                anAccount.Activity = CurrentActivity;
            }

            if ( !IsInDesignMode )
            {
                masterPatientIndexView.CurrentActivity = CurrentActivity;
            }

            if (anAccount.IsEDorUrgentCarePremseAccount)
            {
                // do nothing...
            }

            else
            {
                accountView.OnCloseActivity += ReturnToMainScreen;
                accountView.OnEditAccount += OnEditAccount;
                accountView.OnRepeatActivity += OnRepeatActivity;

                accountView.Model = anAccount;
                accountView.UpdateView();
                accountView.Show();
            }
        }

        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if ( ReturnToMainScreen != null )
            {
                CancelBackgroundWorker();
                ReturnToMainScreen( sender, e );
            }
        }

        // cancels the background worker 
        private void CancelBackgroundWorker()
        {
            if ( backgroundWorker != null )
            {
                backgroundWorker.CancelAsync();
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties

        public Activity CurrentActivity
        {
            get { return currentActivity ?? ( currentActivity = new PreRegistrationActivity() ); }
            set
            {
                currentActivity = value;
            }
        }

        public MasterPatientIndexView MPIV
        {
            get
            {
                return masterPatientIndexView;
            }
        }

        #endregion

        #region Private Methods

        private void EnableAccountView()
        {
            SuspendLayout();

            ClearControls();

            accountView = AccountView.NewInstance();
            accountView.Dock = DockStyle.Fill;

            Controls.Add( (Control)accountView );

            ResumeLayout( false );
        }

        private void ClearControls()
        {
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
                        Logger.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }

            Controls.Clear();
        }


        private void DisplayMasterPatientIndexView()
        {
            ClearControls();
            masterPatientIndexView = new MasterPatientIndexView
                {
                    CurrentActivity = CurrentActivity,
                    Dock = DockStyle.Fill
                };

            Controls.Add( masterPatientIndexView );
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public QuickPreRegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount += ActivatePreregisteredAccountEventHandler;
        }
        #endregion

        #region Data Elements

        private static readonly ILog Logger = LogManager.GetLogger( typeof( PreRegistrationView ) );
        private IAccount selectedAccount;
        private Activity currentActivity;
        private BackgroundWorker backgroundWorker;

        #endregion

        #region Constants 
        #endregion
    }
}
