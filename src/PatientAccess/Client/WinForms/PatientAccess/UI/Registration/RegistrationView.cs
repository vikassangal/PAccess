using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using log4net;

namespace PatientAccess.UI.Registration
{
    //TODO: Create XML summary comment for RegistrationView
    [Serializable]
    public class RegistrationView : ControlView
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
            Account newAccount = null;

            newAccount = AccountActivityService.SelectedAccountFor(this.SelectedAccount);

            // poll CancellationPending and set e.Cancel to true and return 
            // might be a good idea to refactor this code into its own method
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            } 
            
            if (newAccount.Activity != null)
            {
                newAccount.Activity.AssociatedActivityType = typeof(RegistrationActivity);
            }
            else
            {
                newAccount.Activity = new RegistrationActivity();
                newAccount.Activity.AssociatedActivityType = typeof(RegistrationActivity);
            }

            // Place a lock on this object to prevent deadlock
            lock ( this.SelectedAccount )
            {
                this.SelectedAccount = newAccount;
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( this.backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            } 
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
                throw e.Error;
            }
            else
            {
                // success
                this.DisplayAccountViewWith( this.SelectedAccount as Account );
            }

            AccountView.GetInstance().HidePanel();
            this.Cursor = Cursors.Default;
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
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

        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if( ReturnToMainScreen != null )
            {
                CancelBackgroundWorkers();
                ReturnToMainScreen(sender, e);
            }
        }

        private void CancelBackgroundWorkers()
        {
            // cancel both background workers here...
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync();

            if ( this.backgroundWorkerActivate != null )
                this.backgroundWorkerActivate.CancelAsync() ;
        }
        /// <summary>
        /// EditRegistrationEventHandler - handles the event to edit an existing account in Registration activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRegistrationEventHandler( object sender, EventArgs e)
        {
             this.AccountSelectedEventHandler( this, e );
        }


        private void ActivatePreregisteredAccountEventHandler( object sender, EventArgs e )
        {
            if( e != null )
            {
                this.Model = ( (LooseArgs)e ).Context as IAccount;
            }

            if( this.backgroundWorkerActivate == null
                || 
                ( this.backgroundWorkerActivate != null
                && !this.backgroundWorkerActivate.IsBusy )
                )
            {
                this.BeforeActivateWork();

                this.backgroundWorkerActivate = new BackgroundWorker();
                this.backgroundWorkerActivate.WorkerSupportsCancellation = true;

                backgroundWorkerActivate.DoWork +=
                    new DoWorkEventHandler(DoActivate);
                backgroundWorkerActivate.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(AfterActivateWork);

                backgroundWorkerActivate.RunWorkerAsync();
            }
        }

        private void BeforeActivateWork()
        {            
            this.Cursor = Cursors.WaitCursor;
            this.Visible = true;
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
        }

        private void AfterActivateWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // user cancelled
                // Note that due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                this.Cursor = Cursors.Default;
                this.progressPanel1.Visible = false;
                this.progressPanel1.SendToBack();
                throw e.Error;
            }
            else
            {
                // create a temporary account so as to be able to use this later
                Account localAccount = new Account();

                // log typeof information on this.Model to see what's being passed in for casting to Account
                // this is being logged using the BreadCrumbLogger in namespace UI.Logging
                BreadCrumbLogger.GetInstance.Log(String.Format("*** RegistrationView, AfterActivateWork(), testing typeof this.Model {0} ***", this.Model.GetType()));

                // call private method to get an Account object from this.Model
                localAccount = GetAccountFromModel(localAccount);

                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this,
                    new LooseArgs(localAccount));

                if (this.blnStreamlinedActivation)
                {
                    this.EnableActivatePreRegistrationView();

                    if (activatePreRegistrationView != null)
                    {
                        this.activatePreRegistrationView.OnCloseActivity += new EventHandler(ReturnToMainScreen);
                        this.activatePreRegistrationView.OnEditAccount += new EventHandler(OnEditAccount);
                        this.activatePreRegistrationView.OnRepeatActivity += new EventHandler(OnRepeatActivity);

                        activatePreRegistrationView.Model = localAccount;
                        activatePreRegistrationView.UpdateView();
                        activatePreRegistrationView.Show();
                    }
                }
                else
                {
                    this.activateWithEdit(localAccount);
                }

                this.Cursor = Cursors.Default;
                this.progressPanel1.Visible = false;
                this.progressPanel1.SendToBack();
            }
        }

        /// <summary>
        /// GetAccountFromModel. Returns an Account object after casting this.Model 
        /// to the appropriate type from either an Account or an AccountProxy.
        /// 
        /// This addresses defect ID: 35635
        /// </summary>
        /// <param name="localAccount"></param>
        /// <returns></returns>
        private Account GetAccountFromModel(Account localAccount)
        {
            // success
            if (this.Model is Account)
            {
                localAccount = this.Model as Account;
                if (IsEMPIFeatureEnabled(localAccount))
                {
                    localAccount.OverLayEMPIData();
                    localAccount.Activity.EmpiPatient = null;
                }
            }
            else if (this.Model is AccountProxy)
            {
                localAccount = ((AccountProxy)this.Model).AsAccount();
                if (IsEMPIFeatureEnabled(localAccount))
                {
                    localAccount.OverLayEMPIData();
                    localAccount.Activity.EmpiPatient = null;
                }
                this.Model = localAccount;
            }
            return localAccount;
        }

        private void DoActivate( object sender, DoWorkEventArgs e )
        {
            // if we came here via the Worklists (No-Show), make the RegistrationView visible            
          
            this.blnStreamlinedActivation = false;
            IAccount anAccount = this.Model as IAccount;

            if (anAccount != null)
            {
                Account realAccount = AccountActivityService.SelectedAccountFor(anAccount);
               
                // poll CancellationPending and set e.Cancel to true and return 
                if (this.backgroundWorkerActivate.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                realAccount.Activity = CurrentActivity;
                realAccount.Activity.EmpiPatient = anAccount.Activity.EmpiPatient;
                CurrentActivity.EmpiPatient = anAccount.Activity.EmpiPatient;
                realAccount.Activity.AssociatedActivityType = typeof(ActivatePreRegistrationActivity);

                // check for any remaining actions, deactivated codes, remaining errors,
                // or required fields

                RuleEngine.GetInstance().LoadRules(realAccount);
                RuleEngine.GetInstance().EvaluateAllRules(realAccount);

                string deactivatedCodes = RuleEngine.GetInstance().GetInvalidCodeFieldSummary();
                string remainingErrors = RuleEngine.GetInstance().GetRemainingErrorsSummary();
                string requiredFields = RuleEngine.GetInstance().GetRequiredFieldSummary();

                //Activate a Pre-Admit Newborn should never reach streamlined view. But need to reset HospitalService
                if ( CurrentActivity!=null && CurrentActivity.IsAdmitNewbornActivity()
                        && CurrentActivity.AssociatedActivityType != null && CurrentActivity.AssociatedActivityType == typeof( ActivatePreRegistrationActivity ) )
                { 
		        	realAccount.HospitalService = new HospitalService();	
		        }
		        else
		        {
                    if ( deactivatedCodes.Length > 0
                    || remainingErrors.Length > 0
                    || requiredFields.Length > 0 )
                    {
                        realAccount.HospitalService = new HospitalService();
                    }
                    else
                    {
                        RuleEngine.GetInstance().GetWorklistActionItems( realAccount );
                        realAccount.HospitalService = new HospitalService();
                        if ( realAccount.GetAllRemainingActions().Count <= 0 )
                        {
                            this.blnStreamlinedActivation = true;
                        }
                    }
                }
                
                realAccount.RemovePreMseFinancialClass();
                // poll CancellationPending and set e.Cancel to true and return 
                if (this.backgroundWorkerActivate.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                this.Model = realAccount;
            }
        }
      

        private void activateWithEdit( Account realAccount )
        {
            this.EnableAccountView();

            if( accountView != null )
            {
                this.accountView.OnCloseActivity    += new EventHandler( ReturnToMainScreen );
                this.accountView.OnEditAccount      += new EventHandler( OnEditAccount );
                this.accountView.OnRepeatActivity   += new EventHandler( OnRepeatActivity );

                accountView.Model = realAccount;
                accountView.UpdateView();
                accountView.Show();
            }
        }

        private void OnEditAccount( object sender, EventArgs e )
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

            if( ReturnToMainScreen != null )
            {
                this.accountView.OnCloseActivity += new EventHandler( ReturnToMainScreen );
                this.accountView.OnEditAccount += new EventHandler( OnEditAccount );
                this.accountView.OnRepeatActivity += new EventHandler( OnRepeatActivity );
            }

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

        private bool IsEMPIFeatureEnabled(IAccount localAccount)
        {
            EMPIFeatureManager = new EMPIFeatureManager(localAccount.Facility);
            return (EMPIFeatureManager.IsEMPIFeatureEnabled(localAccount.Activity));
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

            if (this.masterPatientIndexView != null
                && this.masterPatientIndexView.TheSearchView != null
                && this.masterPatientIndexView.ThePatientsAccountsView != null )
            {

                this.masterPatientIndexView.TheSearchView.Visible = false;
                this.masterPatientIndexView.ThePatientsAccountsView.Visible = false;
            }

            this.ClearControls();

            accountView = AccountView.NewInstance();
            accountView.Model = this.Model as IAccount;

            accountView.Dock = DockStyle.Fill;
            
            Controls.Add( ( Control )accountView );

            ResumeLayout( false );
        }

        private void EnableActivatePreRegistrationView()
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
            SearchEventAggregator.GetInstance().EditRegistrationEvent -= new EventHandler( this.EditRegistrationEventHandler );

            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
                
                CancelBackgroundWorkers() ;
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
            this.progressPanel1 = new PatientAccess.UI.CommonControls.ProgressPanel();
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.SuspendLayout();
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point( 3, 3 );
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size( 1017, 584 );
            this.progressPanel1.TabIndex = 1;
            this.progressPanel1.Visible = false;
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.CurrentActivity = null;
            this.masterPatientIndexView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterPatientIndexView.Location = new System.Drawing.Point( 0, 0 );
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size( 1024, 622 );
            this.masterPatientIndexView.TabIndex = 0;
            this.masterPatientIndexView.ReturnToMainScreen += new System.EventHandler( this.OnReturnToMainScreen );
            // 
            // RegistrationView
            // 
            this.Controls.Add( this.masterPatientIndexView );
            this.Controls.Add( this.progressPanel1 );
            this.Name = "RegistrationView";
            this.Size = new System.Drawing.Size( 1024, 622 );
            this.Load += new System.EventHandler( this.RegistrationView_Load );
            this.ResumeLayout( false );

        }

        void RegistrationView_Load(object sender, EventArgs e)
        {
            if (!this.IsInDesignMode)
            {
                this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            }
        }
        #endregion

        #endregion

        #region Private Properties
        private IEMPIFeatureManager EMPIFeatureManager { get; set; }
        #endregion

        #region Construction and Finalization

        public RegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected += new EventHandler( this.AccountSelectedEventHandler );
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount += new EventHandler( this.ActivatePreregisteredAccountEventHandler );
            SearchEventAggregator.GetInstance().EditRegistrationEvent += new EventHandler( this.EditRegistrationEventHandler );
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log = LogManager.GetLogger( typeof( RegistrationView ) );

        private IAccount                                                SelectedAccount;
        IAccountView                                    accountView;
        ActivatePreRegistrationView		activatePreRegistrationView;
        private Container                         components = null;
        private MasterPatientIndexView   masterPatientIndexView;
        private Activity                                                i_CurrentActivity;
        private BackgroundWorker                  backgroundWorker;
        private BackgroundWorker                  backgroundWorkerActivate;
        private ProgressPanel                                           progressPanel1;
        private bool                                                    blnStreamlinedActivation;

        #endregion

        #region Constants
        #endregion
    }
}
