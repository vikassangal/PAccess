using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.QuickAccountCreation;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.PatientSearch;
using PatientAccess.UI.WorklistViews;
using log4net;
using PatientAccess.Domain.UCCRegistration;

namespace PatientAccess.UI
{
	/// <summary>
	/// Summary description for MaintenanceCmdView.
	/// </summary>
	public class MaintenanceCmdView : UserControl
	{
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void MaintenanceCmdView_Load(object sender, EventArgs e)
        {
            button1.Visible = false;
        }

	    private void BeforeWork()
        {
            Cursor = Cursors.WaitCursor;

            if( SelectedAccount != null )
            {
                if( SelectedAccount.Activity != null )
                {
                    UI.AccountView.GetInstance().ActiveContext = SelectedAccount.Activity.ContextDescription;
                }
            }

            UI.AccountView.GetInstance().ShowPanel();
        }

	    private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDisposed || Disposing)
                return;

            // Handle the cancelled case first
            if (e.Cancelled)
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(UIErrorMessages.LOAD_ACCOUNT_TIMEOUT_MESSAGE);
                    ActivityEventAggregator.GetInstance().RaiseActivityCancelEvent(
                                this, EventArgs.Empty);
                    return;
                }
                
                UI.AccountView.GetInstance().HidePanel();
                throw e.Error;
            }
            else
            {
                // success
                DisplayAccountViewWith( newAccount );
            }

            // place post completion operations here...
            UI.AccountView.GetInstance().HidePanel();
            Cursor = Cursors.Default;
        }

	    private void DoWork( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.
          
            newAccount = null;
            newAccount = AccountActivityService.SelectedAccountFor(SelectedAccount);
            newAccount.Activity = SelectedAccount.Activity;
            newAccount.IsShortRegistered = SelectedAccount.IsShortRegistered;
            newAccount.IsQuickRegistered = SelectedAccount.IsQuickRegistered;
	        if (SelectedAccount.IsUrgentCarePreMse)
	        {
	            newAccount.KindOfVisit = VisitType.UCCOutpatient;
                newAccount.FinancialClass = FinancialClass.MedScreenFinancialClass;
	        }


            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: poll just before the time consuming work is done. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            if (newAccount.Activity.GetType().Equals(typeof(UCCPostMseRegistrationActivity)))
            {
                newAccount.Activity.AssociatedActivityType = typeof(UCCPostMseRegistrationActivity);
                AccountActivityService.CheckPostMSEAccount(newAccount);
            }

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }      

/*            
            try
            {
                newAccount = AccountActivityService.SelectedAccountFor( SelectedAccount );     

                newAccount.Activity = SelectedAccount.Activity;

                if (newAccount.Activity.GetType().Equals(typeof(PostMSERegistrationActivity)))
                {
                    newAccount.Activity.AssociatedActivityType = typeof(PostMSERegistrationActivity);
                    AccountActivityService.CheckPostMSEAccount(newAccount);
                }
            }
            catch
            {            
                throw;
            }
*/
        }

	    private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            EnableAccountView();

            if ( ((LooseArgs)e).Context != null )
            {
                SelectedAccount = ((LooseArgs)e).Context as IAccount;
            }

            if( backgroundWorker == null || !backgroundWorker.IsBusy )
            {
                // pass in delegates to do your pre-processing, actual processing, and post-processing.
                // if you have no pre or post processing, simply pass nulls for those parms.

                //backgroundWorker = new CommonControls.BackgroundWorker(new CommonControls.BackgroundWorker.BeforeWorkDelegate(BeforeWork),
                //    new CommonControls.BackgroundWorker.DoWorkDelegate(DoWork),
                //    new CommonControls.BackgroundWorker.AfterWorkDelegate(AfterWork));

                //// and kick it off

                //backgroundWorker.Start();

                BeforeWork();

                backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }

        private void DisplayAccountViewWith( Account anAccount )
        {            
            if( ReturnToMainScreen != null )
            {
                accountView.OnCloseActivity += ReturnToMainScreen;
                accountView.OnEditAccount += OnEditAccount;
                accountView.OnRepeatActivity += OnRepeatActivity;
            }
    
            accountView.Model = anAccount;
            accountView.UpdateView();
            accountView.Show();
        }

	    private void AccountViewEventHandler( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;

            if( args.Context != null )
            {
                // Signal the PatientAccessView to destroy the worklist screen
                PatientAccessViewPopulationAggregator aggregator = 
                    PatientAccessViewPopulationAggregator.GetInstance();

                aggregator.RaiseActionSelectedEvent( null, new LooseArgs( null ) );
            }
            Dispose();
        }
        #endregion

        #region Methods


        #endregion

        #region Properties
    
        public MasterPatientIndexView MasterPatientIndexView
        {
            get
            {
                return masterPatientIndexView;
            }
        }

        public IAccountView AccountView
        {
            get
            {
                return accountView;
            }
            set
            {
                accountView = value;
            }
        }

        public string ActivatingTab
        {
            private get
            {
                return i_ActivatingTab;
            }
            set
            {
                i_ActivatingTab = value;
            }
        }
        #endregion

        #region Private Methods
		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

            this.button1 = new LoggingButton();
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
            this.masterPatientIndexView.CurrentActivity = new MaintenanceActivity();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(472, 304);
            this.button1.Name = "button1";
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.Visible = true;
            // 
            // MaintenanceCmdView
            // 
            this.Load +=new EventHandler(MaintenanceCmdView_Load);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.masterPatientIndexView);
            this.Name = "MaintenanceCmdView";
            this.Size = new System.Drawing.Size(1024, 619);
            this.ResumeLayout(false);
        }
		#endregion
        #endregion        

        #region Construction and Finalization

        public MaintenanceCmdView()
        {
            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

		public void RemoveAccountSelectedHandler()
		{
			SearchEventAggregator.GetInstance().AccountSelected -= AccountSelectedEventHandler;
		}

        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if( ReturnToMainScreen != null )
            {
                // before returning to the main screen cancel any background worker activity that was 
                // launched 
                CancelBackgroundWorker();

                ReturnToMainScreen(sender, e);
            }
        }

        // Cancel Background workers
        private void CancelBackgroundWorker()
        {
            if (backgroundWorker != null)
                backgroundWorker.CancelAsync();
        }

        private void OnRepeatActivity(object sender, EventArgs e)
        {
            DisplayMasterPatientIndexView();
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

        private void DisplayMasterPatientIndexView()
        {
            ClearControls();
            
//            masterPatientIndexView = new MasterPatientIndexView();            
            
            masterPatientIndexView.Dock = DockStyle.Fill;
            Controls.Add( masterPatientIndexView );
        }
        private void EnableAccountView()
        {
            try
            {
                ClearControls();

                accountView = UI.AccountView.NewInstance();
                accountView.SetActivatingTab(ActivatingTab);

                SuspendLayout();
                accountView.Dock = DockStyle.Fill;
                ResumeLayout( false );
                Controls.Add( ( Control )accountView );
            }
            catch
            {
                throw;
            }
        }

        private void OnEditAccount(object sender, EventArgs e)
        {
            Cursor storedCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            try
            {
                if( e != null )
                {
                    EnableAccountView();

                    Account realAccount = (Account)((LooseArgs)e).Context;

                    if ( realAccount.IsShortRegistered )
                    {
                        realAccount.Activity = new ShortMaintenanceActivity();
                    }
                    else if ( realAccount.IsQuickRegistered )
                    {
                        realAccount.Activity = new QuickAccountMaintenanceActivity();
                    }
                    else
                    {
                        //Defect 18238, do not overwrite AssociateActivityType if it gets set earlier
                        if(realAccount.Activity ==null || !realAccount.Activity.IsMaintenanceActivity())
                            realAccount.Activity = new MaintenanceActivity();
                    }

                    if( ReturnToMainScreen != null )
                    {
                        accountView.OnCloseActivity += ReturnToMainScreen;
                        accountView.OnEditAccount += OnEditAccount;
                        accountView.OnRepeatActivity += OnRepeatActivity;
                    }

                    ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, e);
                    this.AccountSelectedEventHandler(this, e);

                    accountView.Model = realAccount;
                    accountView.UpdateView();
                    accountView.Show();
                }
            }
            finally
            {
                Cursor = storedCursor;
            }
        }

        public static MaintenanceCmdView GetInstance()
        {
            if( c_singletonInstance == null )
            {
                lock( c_syncRoot )
                {
                    if( c_singletonInstance == null )
                    {
                        c_singletonInstance = new MaintenanceCmdView();
                    }
                }
            }

            return c_singletonInstance;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                WorklistCmdAggregator.GetInstance().ActionSelected -= AccountViewEventHandler;

                RemoveAccountSelectedHandler();

                c_singletonInstance = null;

                if( accountView != null )
                {
                    accountView.Dispose();
                }
                if(components != null)
                {
                    components.Dispose();
                }
                // cancel any background worker activity that was launched 
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        private static readonly ILog c_log = LogManager.GetLogger( typeof( MaintenanceCmdView ) );
        
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container                   components = null;

        private LoggingButton               button1;

        private static object               c_syncRoot = new Object();

        private static MaintenanceCmdView   c_singletonInstance;

        private IAccountView                accountView;
        private MasterPatientIndexView      masterPatientIndexView;

        private string                      i_ActivatingTab = string.Empty;

        private Account                     newAccount;
        private IAccount                    SelectedAccount;
        private BackgroundWorker            backgroundWorker;

        #endregion
    }
}
