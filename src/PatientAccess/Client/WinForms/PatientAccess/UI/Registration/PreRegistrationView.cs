using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.PreMSEViews;
using log4net;

namespace PatientAccess.UI.Registration
{
    //TODO: Create XML summary comment for PreRegistrationView
    [Serializable]
    public class PreRegistrationView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            if( this.SelectedAccount != null )
            {
                if( this.SelectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = this.SelectedAccount.Activity.ContextDescription;
                }
            }            

            this.Cursor = Cursors.WaitCursor;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Account newAccount = null;
            newAccount = AccountActivityService.SelectedAccountFor(this.SelectedAccount);
            newAccount.IsShortRegistered = this.SelectedAccount.IsShortRegistered;
            newAccount.IsQuickRegistered = this.SelectedAccount.IsQuickRegistered;
            if (newAccount.Activity == null)
            {
                newAccount.Activity = this.CurrentActivity;
            }
            if(newAccount.Activity.AssociatedActivityType == null )
                newAccount.Activity.AssociatedActivityType = typeof( PreRegistrationActivity );

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // Sanjeev Kumar - defect 34899 - this appears to be related to the threading issue
            // Dean discovered. Place a lock on this object to prevent deadlock
            lock (this.SelectedAccount)
            {
                this.SelectedAccount = newAccount;
            }

 
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( this.IsDisposed || this.Disposing )
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
                this.Cursor = Cursors.Default;
                this.DisplayAccountViewWith( this.SelectedAccount as Account );

                AccountView.GetInstance().HidePanel();
            }
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {            
            if ( ((LooseArgs)e).Context != null )
            {
                this.SelectedAccount = ((LooseArgs)e).Context as IAccount;
            }

            if( SelectedAccount.FinancialClass != null &&
                SelectedAccount.FinancialClass.Code.Equals( FinancialClass.MED_SCREEN_EXM_CODE ) &&
                SelectedAccount.KindOfVisit != null && 
                this.SelectedAccount.KindOfVisit.IsEmergencyPatient )
            {
                SelectedAccount.Activity = new EditPreMseActivity();
                EnablePreMseRegistrationView( SelectedAccount );
            }
            if (SelectedAccount.FinancialClass != null &&
                SelectedAccount.FinancialClass.Code.Equals(FinancialClass.MED_SCREEN_EXM_CODE) &&
                SelectedAccount.KindOfVisit != null &&
                this.SelectedAccount.KindOfVisit.IsOutpatient)
            {
                SelectedAccount.Activity = new EditUCCPreMSEActivity();
                EnablePreMseRegistrationView(SelectedAccount);
            }
            else
            {
                EnableAccountView();
            }

            AccountView.GetInstance().ShowPanel();

            if( ( SelectedAccount != null ) && ( SelectedAccount.FinancialClass != null ) &&
              ( SelectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE ) &&
              ( SelectedAccount != null ) && ( SelectedAccount.KindOfVisit != null ) &&
              ( SelectedAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT )
              )
            // make sure we are not editing a PreMSE account in the 12-tab view
            //if( SelectedAccount.FinancialClass.Code == FinancialClass.MED_SCREEN_EXM_CODE
            //    && SelectedAccount.KindOfVisit.Code == VisitType.EMERGENCY_PATIENT)
            {
                return;
            }

            if( this.backgroundWorker == null
                || 
                ( this.backgroundWorker != null
                && !this.backgroundWorker.IsBusy )
                )
            {
                this.BeforeWork();

                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.DoWork +=
                    new DoWorkEventHandler(DoWork);
                backgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(AfterWork);

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void ActivatePreregisteredAccountEventHandler( object sender, EventArgs e )
        {
            Cursor storedCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                AccountProxy proxy = ((LooseArgs)e).Context as AccountProxy;
                if( proxy != null )
                {
                    proxy.Activity = new RegistrationActivity();
                    proxy.Activity.AssociatedActivityType = typeof(ActivatePreRegistrationActivity);

                    Account realAccount = proxy.AsAccount();

                    if( realAccount.GetAllRemainingActions().Count  > 0 )
                    {
                        this.EnableAccountView();

                        if( accountView != null )
                        {
                            accountView.Model = realAccount;
                            accountView.UpdateView();
                            accountView.Show();
                        }
                    }
                    else
                    {
                        this.EnableActivatePreregistrationView();

                        if( activatePreRegistrationView != null )
                        {
                            this.activatePreRegistrationView.OnCloseActivity += new EventHandler( ReturnToMainScreen );
                            this.activatePreRegistrationView.OnEditAccount += new EventHandler( OnEditAccount );
                            this.activatePreRegistrationView.OnRepeatActivity += new EventHandler( OnRepeatActivity );
                        }
                        activatePreRegistrationView.Model = realAccount;
                        activatePreRegistrationView.UpdateView();
                        activatePreRegistrationView.Show();
                    }
                }
            }
            finally
            {
                this.Cursor = storedCursor;
            }
        }

        private void OnEditAccount(object sender, EventArgs e)
        {
            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, e );
            this.AccountSelectedEventHandler( this, e );
        }

        private void OnRepeatActivity(object sender, EventArgs e)
        {
            this.DisplayMasterPatientIndexView();
        }

        private void DisplayAccountViewWith( Account anAccount )
        {
            if( anAccount.Activity == null )
            {
                anAccount.Activity = this.CurrentActivity;
            }

            if( !this.IsInDesignMode )
            {
                this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
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
                this.accountView.OnCloseActivity += new EventHandler( ReturnToMainScreen );
                this.accountView.OnEditAccount += new EventHandler( OnEditAccount );
                this.accountView.OnRepeatActivity += new EventHandler( OnRepeatActivity );

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
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Properties
        public Activity CurrentActivity
        {
            get
            {
                if( i_CurrentActivity == null )
                {
                    i_CurrentActivity = new PreRegistrationActivity();
                }
                return i_CurrentActivity;
            }
            set
            {
                i_CurrentActivity = value;
            }
        }

        public MasterPatientIndexView MPIV
        {
            get
            {
                return this.masterPatientIndexView;
            }
        }

        #endregion

        #region Private Methods
        private void EnableAccountView()
        {
            SuspendLayout();

            this.ClearControls();

            accountView = AccountView.NewInstance();
            accountView.Dock = DockStyle.Fill;
            
            Controls.Add( ( Control )accountView );

            ResumeLayout( false );
        }

        private void EnablePreMseRegistrationView( IAccount anAccount )
        {
            this.ClearControls();

            preMseRegistrationView = new PreMseRegistrationView();

            SuspendLayout();
            preMseRegistrationView.Dock = DockStyle.Fill;
            preMseRegistrationView.Model = anAccount as Account;
            Controls.Add( preMseRegistrationView );
            ResumeLayout( false );
        }

        private void EnableActivatePreregistrationView()
        {
            this.ClearControls();

            activatePreRegistrationView = new ActivatePreRegistrationView();

            SuspendLayout();
            activatePreRegistrationView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( activatePreRegistrationView );
        }

        private void ClearControls()
        {
            foreach( Control control in this.Controls )
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
            SearchEventAggregator.GetInstance().AccountSelected -= new EventHandler( this.AccountSelectedEventHandler );
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount -= new EventHandler( this.ActivatePreregisteredAccountEventHandler );
            
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
            this.ClearControls();
            this.masterPatientIndexView = new MasterPatientIndexView();
            this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.Controls.Add( this.masterPatientIndexView );
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
        public PreRegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected -= new EventHandler( this.AccountSelectedEventHandler );
            SearchEventAggregator.GetInstance().AccountSelected += new EventHandler( this.AccountSelectedEventHandler );
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount += new EventHandler( this.ActivatePreregisteredAccountEventHandler );            
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( PreRegistrationView ) );

        private IAccount                                                        SelectedAccount;
        private IAccountView                                    accountView;
        private ActivatePreRegistrationView       activatePreRegistrationView;
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
