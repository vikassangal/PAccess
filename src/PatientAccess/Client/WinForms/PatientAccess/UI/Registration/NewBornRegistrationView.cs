using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.PatientSearch;
using log4net;

namespace PatientAccess.UI.Registration
{
    //TODO: Create XML summary comment for NewBornRegistrationView
    [Serializable]
    public class NewBornRegistrationView : ControlView
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
                else
                {
                    if( this.CurrentActivity != null )
                    {
                        AccountView.GetInstance().ActiveContext = this.CurrentActivity.ContextDescription;
                    }
                }
            }

            AccountView.GetInstance().ShowPanel();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.
            
            Account newBornAccount = null;

            if ( SelectedAccount.GetType() == typeof( Account ) && SelectedAccount.IsNew )
            {
                newBornAccount = (Account)SelectedAccount;
            }
            else
            {
                newBornAccount = AccountActivityService.SelectedAccountFor(SelectedAccount);
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: poll cancellationPending before doing time consuming work. 
            // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (newBornAccount != null && newBornAccount.Activity.GetType() == typeof(AdmitNewbornActivity))
            {
                //setting mother's account activity
                this.CurrentActivity.AppUser = User.GetCurrent();

                //setting newborn's account activity
                //newBornAccount.Activity = this.CurrentActivity;
                newBornAccount.Patient.IsNew = true;
                newBornAccount.IsNew = true;

                newBornAccount.ActionsLoader = new ActionLoader(newBornAccount);
                //newBornAccount.Activity.AssociatedActivityType = typeof( AdmitNewbornActivity );                    
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return ;
            }

            this.Model = newBornAccount;
            
/*
            try
            {

                if (SelectedAccount.GetType() == typeof(Account))
                {
                    newBornAccount = (Account)SelectedAccount;
                }
                else
                {
                    newBornAccount = AccountActivityService.SelectedAccountFor(SelectedAccount);
                }

                if ( newBornAccount != null && newBornAccount.Activity.GetType() == typeof( AdmitNewbornActivity ) )
                {                  
                    //setting mother's account activity
                    this.CurrentActivity.AppUser = User.GetCurrent(); 
                   
                    //setting newborn's account activity
                    //newBornAccount.Activity = this.CurrentActivity;
                    newBornAccount.Patient.IsNew = true;
                    newBornAccount.IsNew = true;

                    newBornAccount.ActionsLoader = new ActionLoader(newBornAccount);
                    //newBornAccount.Activity.AssociatedActivityType = typeof( AdmitNewbornActivity );                    
                }

                this.Model = newBornAccount;
            }
            catch
            {
                throw;
            }
*/
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            if( e.Cancelled )
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                throw e.Error;
            }
            else
            {
                // success
                this.DisplayAccountViewWith( this.Model as Account );
            }

            // place post completion operations here...
            this.Cursor = Cursors.Default;
            AccountView.GetInstance().HidePanel();
        }

        private void CreateNewBornAccountEventHandler( object sender, EventArgs e )
        {
            EnableAccountView();  

            if( e != null )                
            {
                if( ((LooseArgs)e).Context != null )
                {
                    this.SelectedAccount = ((LooseArgs)e).Context as IAccount;
                }
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

        private void OnEditAccount( object sender, EventArgs e )
        {

            ActivityEventAggregator.GetInstance().RaiseActivityStartEvent( this, e );
            this.CreateNewBornAccountEventHandler( this, e );
        }

        #region Confirmation View event handlers


        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if( ReturnToMainScreen != null )
            {
                // Cancel any active background workers
                CancelBackgroundWorker();

                ReturnToMainScreen( sender, e );
            }
        }

        private void OnRepeatActivity(object sender, EventArgs e)
        {
            this.DisplayMasterPatientIndexView();
        }
        #endregion

        private void DisplayAccountViewWith( Account anAccount )
        {          
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
                    i_CurrentActivity = new AdmitNewbornActivity();
                }
                return i_CurrentActivity;
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
            SearchEventAggregator.GetInstance().CreateNewBornAccountEvent -= new EventHandler(this.CreateNewBornAccountEventHandler);
            SearchEventAggregator.GetInstance().AccountSelected -= new EventHandler( this.OnEditAccount );
            
            if( disposing )
            {                
                if (components != null) 
                {
                    components.Dispose();
                }

                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }

        private void CancelBackgroundWorker()
        {
            // cancel the backgroundWorker  
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync();
        }

        private void DisplayMasterPatientIndexView()
        {
            this.ClearControls();
            this.masterPatientIndexView = new MasterPatientIndexView();

            if( !this.IsInDesignMode )
            {
                this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            }
            
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
            // NewBornRegistrationView
            // 
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "newbornRegistrationView";
            this.Size = new System.Drawing.Size(1024, 512);
            this.ResumeLayout(false);

        }
        #endregion
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public NewBornRegistrationView()
        {
            InitializeComponent();

			if( !this.IsInDesignMode )
			{
				this.masterPatientIndexView.CurrentActivity     = this.CurrentActivity;
			}

            SearchEventAggregator.GetInstance().CreateNewBornAccountEvent += new EventHandler( this.CreateNewBornAccountEventHandler );
            SearchEventAggregator.GetInstance().AccountSelected += new EventHandler( this.OnEditAccount );
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( NewBornRegistrationView ) );

        private IAccount                                                SelectedAccount;
        private IAccountView                            accountView;
        private Container                         components = null;
        private MasterPatientIndexView   masterPatientIndexView;
        private Activity                                                i_CurrentActivity;
        private BackgroundWorker                  backgroundWorker;

        #endregion

        #region Constants
        #endregion
    }
}

