using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;
using log4net;

namespace PatientAccess.UI.Registration
{
    //TODO: Create XML summary comment for PostMSERegistrationView
    [Serializable]
    public class PostMSERegistrationView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            if( this.SelectedAccount != null )
            {
                if( this.SelectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = this.SelectedAccount.Activity.ContextDescription;
                }
            }

            AccountView.GetInstance().ShowPanel();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.
            Account newAccount = null;

            // 7/24/07 - bjw - Unconditionally reload the account. Defect 34552
            newAccount = AccountActivityService.SelectedAccountFor(this.SelectedAccount);

            if (newAccount.Activity != null)
            {
                newAccount.Activity.AssociatedActivityType = typeof(PostMSERegistrationActivity);
            }
            else
            {
                newAccount.Activity = new PostMSERegistrationActivity();
                newAccount.Activity.AssociatedActivityType = typeof(PostMSERegistrationActivity);
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            // whack the ins and FC if this is a post mse
            this.Model = AccountActivityService.CheckPostMSEAccount(newAccount);
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            if ( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if ( e.Error != null )
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    throw new LoadAccountTimeoutException();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success!
                this.DisplayAccountViewWith( this.Model as Account );
            }

            this.Cursor = Cursors.Default;
            AccountView.GetInstance().HidePanel();
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {

            if( e != null )                
            {
                if( ((LooseArgs)e).Context != null )
                {
                    this.SelectedAccount = ((LooseArgs)e).Context as IAccount;
                }
            }

            EnableAccountView();

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

        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if( ReturnToMainScreen != null )
            {
                CancelBackgroundWorker();

                ReturnToMainScreen( sender, e );
            }
        }

        private void CancelBackgroundWorker()
        {
            // cancel any background worker activity that was launched 
            if (this.backgroundWorker != null)
                this.backgroundWorker.CancelAsync();
        }
        /// <summary>
        /// EditRegistrationEventHandler - handles the event to edit an existing account in Registration activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRegistrationEventHandler( object sender, EventArgs e)
        {
            Cursor storedCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                IAccount proxy = ((LooseArgs)e).Context as IAccount;
                if( proxy != null )
                {
                    EnableAccountView();

                    if( accountView != null )
                    {
                        proxy.Activity = this.CurrentActivity;
                        Account realAccount = proxy.AsAccount();

                        accountView.Model = realAccount;
                        accountView.UpdateView();
                        accountView.Show();
                    }
                }
            }
            catch (RemotingTimeoutException)
            {
                throw new LoadAccountTimeoutException();
            }
            finally
            {
                this.Cursor = storedCursor;
            }
        }

        private void OnEditAccount( object sender, EventArgs e )
        {
            Account anAccount = null;

            if( e != null )
            {
                if( ( (LooseArgs)e ).Context != null )
                {
                    anAccount = ( (LooseArgs)e ).Context as Account;
                }
            }

            if( anAccount != null )
            {
                anAccount.Activity = new MaintenanceActivity();
            }

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

            this.accountView.OnCloseActivity += new EventHandler( ReturnToMainScreen );
            this.accountView.OnEditAccount += new EventHandler( OnEditAccount );
            this.accountView.OnRepeatActivity += new EventHandler( OnRepeatActivity );

            accountView.Model = anAccount;
            accountView.UpdateView();
            accountView.Show();
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
                    i_CurrentActivity = new RegistrationActivity();
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
            this.ClearControls();

            accountView = AccountView.NewInstance();

            SuspendLayout();
            accountView.Dock = DockStyle.Fill;
            ResumeLayout( false );
            Controls.Add( ( Control )accountView );
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
            SearchEventAggregator.GetInstance().EditRegistrationEvent -= new EventHandler( this.EditRegistrationEventHandler );

            if( disposing )
            {
                
                if (components != null) 
                {
                    components.Dispose();
                }

                // cancel any background worker activity that was launched 
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
            this.masterPatientIndexView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterPatientIndexView.Location = new System.Drawing.Point(0, 0);
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size(1024, 512);
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // PostMSERegistrationView
            // 
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "PostMSERegistrationView";
            this.Size = new System.Drawing.Size(1024, 512);
            this.Load += new EventHandler( PostMSERegistrationView_Load );
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        private void PostMSERegistrationView_Load( object sender, EventArgs e )
        {
            this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
        }
        public PostMSERegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected += new EventHandler( this.AccountSelectedEventHandler );
            SearchEventAggregator.GetInstance().EditRegistrationEvent += new EventHandler( this.EditRegistrationEventHandler );
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( PostMSERegistrationView ) );

        private IAccount                                                SelectedAccount;
        IAccountView                                    accountView;
        private Container                         components = null;
        private MasterPatientIndexView   masterPatientIndexView;
        private Activity                                                i_CurrentActivity;
        private BackgroundWorker                  backgroundWorker;
        #endregion

        #region Constants
        #endregion
    }
}

