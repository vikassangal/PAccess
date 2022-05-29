using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PreMSEViews;
using PatientAccess.UI.Registration;
using log4net;

namespace PatientAccess.UI.ShortRegistration
{
    [Serializable]
    public class ShortPreRegistrationView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            if( SelectedAccount != null )
            {
                if( SelectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = SelectedAccount.Activity.ContextDescription;
                }
            }            

            Cursor = Cursors.WaitCursor;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Account newAccount = null;
            newAccount = AccountActivityService.SelectedAccountFor(SelectedAccount);
            newAccount.IsShortRegistered = SelectedAccount.IsShortRegistered; 
            newAccount.IsQuickRegistered = SelectedAccount.IsQuickRegistered;
            if (newAccount.Activity == null)
            {
                newAccount.Activity = CurrentActivity;
            }

            newAccount.Activity.AssociatedActivityType = typeof(PreRegistrationActivity);
            
            // poll CancellationPending and set e.Cancel to true and return 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // Sanjeev Kumar - defect 34899 - this appears to be related to the threading issue
            // Dean discovered. Place a lock on this object to prevent deadlock
            lock (SelectedAccount)
            {
                SelectedAccount = newAccount;
            }
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( IsDisposed || Disposing )
                return ;

            if ( e.Cancelled )
            {
                // cancelled work here
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                Cursor = Cursors.Default;
                DisplayAccountViewWith( SelectedAccount as Account );

                AccountView.GetInstance().HidePanel();
            }
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {            
            if ( ((LooseArgs)e).Context != null )
            {
                SelectedAccount = ((LooseArgs)e).Context as IAccount;
            }

            if ( SelectedAccount != null &&
                SelectedAccount.FinancialClass != null &&
                SelectedAccount.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) &&
                SelectedAccount.KindOfVisit != null && 
                SelectedAccount.KindOfVisit.IsEmergencyPatient )
            {
                SelectedAccount.Activity = new EditPreMseActivity();
                EnablePreMseRegistrationView( SelectedAccount );
            }
            else
            {
                EnableAccountView();                
            }

            AccountView.GetInstance().ShowPanel();

            // make sure we are not editing a PreMSE account in the 12-tab view
            //if( SelectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE
            if ( SelectedAccount != null && 
                 SelectedAccount.FinancialClass != null &&
                 SelectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE &&
                 SelectedAccount.KindOfVisit != null &&
                 SelectedAccount.KindOfVisit.IsEmergencyPatient )
            {
                return;
            }

            if( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void ActivatePreregisteredAccountEventHandler( object sender, EventArgs e )
        {
            Cursor storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                AccountProxy proxy = ((LooseArgs)e).Context as AccountProxy;
                if( proxy != null )
                {
                    proxy.Activity = new ShortRegistrationActivity
                        { AssociatedActivityType = typeof (ActivatePreRegistrationActivity) };

                    Account realAccount = proxy.AsAccount();

                    EnableAccountView();

                    if( accountView != null )
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

        private void OnEditAccount(object sender, EventArgs e)
        {
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, e );
            AccountSelectedEventHandler( this, e );
        }

        private void OnRepeatActivity(object sender, EventArgs e)
        {
            DisplayMasterPatientIndexView();
        }

        private void DisplayAccountViewWith( Account anAccount )
        {
            if( anAccount.Activity == null )
            {
                anAccount.Activity = CurrentActivity;
            }

            if( !IsInDesignMode )
            {
                masterPatientIndexView.CurrentActivity = CurrentActivity;
            }

            if( anAccount.FinancialClass != null &&
                anAccount.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) &&
                anAccount.KindOfVisit != null && 
                anAccount.KindOfVisit.IsEmergencyPatient )
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
            if( ReturnToMainScreen != null )
            {
                CancelBackgroundWorker(); 
                ReturnToMainScreen( sender, e );
            }
        }

        // cancels the background worker 
        private void CancelBackgroundWorker()
        {
            if (backgroundWorker != null)
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
            get { return i_CurrentActivity ?? (i_CurrentActivity = new PreRegistrationActivity()); }
            set
            {
                i_CurrentActivity = value;
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
            
            Controls.Add( ( Control )accountView );

            ResumeLayout( false );
        }

        private void EnablePreMseRegistrationView( IAccount anAccount )
        {
            ClearControls();

            preMseRegistrationView = new PreMseRegistrationView();

            SuspendLayout();
            preMseRegistrationView.Dock = DockStyle.Fill;
            preMseRegistrationView.Model = anAccount as Account;
            Controls.Add( preMseRegistrationView );
            ResumeLayout( false );
        }

        private void ClearControls()
        {
            foreach( Control control in Controls )
            {
                if( control != null )
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch( Exception ex )
                    {
                        c_log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            Controls.Clear();
        }

        protected override void Dispose( bool disposing )
        {
            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount -= ActivatePreregisteredAccountEventHandler;
            
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
                // cancel the background worker here...
                CancelBackgroundWorker(); 
            }
            base.Dispose( disposing );
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

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.masterPatientIndexView.ReturnToMainScreen += new EventHandler( OnReturnToMainScreen );

            this.SuspendLayout();
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.Location = new System.Drawing.Point(0, 0);
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size(1024, 512);
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // PreRegistrationView
            // 
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "PreRegistrationView";
            this.Size = new System.Drawing.Size(1024, 512);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public ShortPreRegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount += ActivatePreregisteredAccountEventHandler;            
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( PreRegistrationView ) );

        private IAccount                                                        SelectedAccount;
        private IAccountView                                    accountView;
        private PreMseRegistrationView             preMseRegistrationView;
        private Container                                 components = null;
        private MasterPatientIndexView           masterPatientIndexView;
        private Activity                                                        i_CurrentActivity;
        private BackgroundWorker                          backgroundWorker;
        #endregion

        #region Constants
        public const string MED_SCREEN_EXM_CODE = "37";
        #endregion
    }
}
