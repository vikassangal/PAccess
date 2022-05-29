using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.Logging;
using PatientAccess.UI.PatientSearch;
using log4net;

namespace PatientAccess.UI.ShortRegistration
{
    [Serializable]
    public class ShortRegistrationView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            Cursor = Cursors.WaitCursor;

            if( SelectedAccount != null )
            {
                if( SelectedAccount.Activity != null )
                {
                    AccountView.GetInstance().ActiveContext = SelectedAccount.Activity.ContextDescription;
                }
            }

            AccountView.GetInstance().ShowPanel();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Account newAccount = null;

            newAccount = AccountActivityService.SelectedAccountFor(SelectedAccount);
            newAccount.IsShortRegistered = SelectedAccount.IsShortRegistered;
            newAccount.IsQuickRegistered = SelectedAccount.IsQuickRegistered;
            // poll CancellationPending and set e.Cancel to true and return 
            // might be a good idea to refactor this code into its own method
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            } 
            
            if (newAccount.Activity != null)
            {
                newAccount.Activity.AssociatedActivityType = typeof(ShortRegistrationActivity);
            }
            else
            {
                newAccount.Activity = new ShortRegistrationActivity
                    { AssociatedActivityType = typeof (ShortRegistrationActivity) };
            }

            // Place a lock on this object to prevent deadlock
            lock ( SelectedAccount )
            {
                SelectedAccount = newAccount;
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            } 
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( IsDisposed || Disposing )
                return ;

            if ( e.Cancelled )
            {
                // user cancelled
                // Due to a race condition in the DoWork event handler, the Cancelled
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
                DisplayAccountViewWith( SelectedAccount as Account );
            }

            AccountView.GetInstance().HidePanel();
            Cursor = Cursors.Default;
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            EnableAccountView();

            if( e != null )                
            {
                if( ((LooseArgs)e).Context != null )
                {
                    SelectedAccount = ((LooseArgs)e).Context as IAccount;
                }
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
            if ( backgroundWorker != null )
                backgroundWorker.CancelAsync();

            if ( backgroundWorkerActivate != null )
                backgroundWorkerActivate.CancelAsync() ;
        }
        /// <summary>
        /// EditRegistrationEventHandler - handles the event to edit an existing account in Registration activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditRegistrationEventHandler( object sender, EventArgs e)
        {
             AccountSelectedEventHandler( this, e );
        }


        private void ActivatePreregisteredAccountEventHandler( object sender, EventArgs e )
        {
            if( e != null )
            {
                Model = ( (LooseArgs)e ).Context as IAccount;
            }

            if( backgroundWorkerActivate == null || !backgroundWorkerActivate.IsBusy )
            {
                BeforeActivateWork();

                backgroundWorkerActivate = new BackgroundWorker {WorkerSupportsCancellation = true};

                backgroundWorkerActivate.DoWork += DoActivate;
                backgroundWorkerActivate.RunWorkerCompleted += AfterActivateWork;

                backgroundWorkerActivate.RunWorkerAsync();
            }
        }

        private void BeforeActivateWork()
        {            
            Cursor = Cursors.WaitCursor;
            Visible = true;
            progressPanel1.Visible = true;
            progressPanel1.BringToFront();
        }

        private void AfterActivateWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // user cancelled
                // Due to a race condition in the DoWork event handler, the Cancelled
                // flag may not have been set, even though CancelAsync was called.
            }
            else if (e.Error != null)
            {
                // handle errors/exceptions thrown from within the DoWork method here
                Cursor = Cursors.Default;
                progressPanel1.Visible = false;
                progressPanel1.SendToBack();
                throw e.Error;
            }
            else
            {
                // create a temporary account so as to be able to use this later
                Account localAccount = new Account();

                // log typeof information on Model to see what's being passed in for casting to Account
                // this is being logged using the BreadCrumbLogger in namespace UI.Logging
                BreadCrumbLogger.GetInstance.Log(String.Format(
                    "*** RegistrationView, AfterActivateWork(), testing typeof Model {0} ***", Model.GetType()));

                // call private method to get an Account object from Model
                localAccount = GetAccountFromModel(localAccount);

                ActivityEventAggregator.GetInstance().RaiseActivityStartEvent(this, new LooseArgs( localAccount ));

                ActivateWithEdit(localAccount);

                Cursor = Cursors.Default;
                progressPanel1.Visible = false;
                progressPanel1.SendToBack();
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
            if (Model is Account)
            {
                localAccount = Model as Account;
                if (IsEMPIFeatureEnabled(localAccount))
                {
                    localAccount.OverLayEMPIData();
                    localAccount.Activity.EmpiPatient = null;
                }
            }
            else if (Model is AccountProxy)
            {
                localAccount = ((AccountProxy) Model).AsAccount();
                if (IsEMPIFeatureEnabled(localAccount))
                {
                    localAccount.OverLayEMPIData();
                    localAccount.Activity.EmpiPatient = null;
                }
                Model = localAccount;
            }
            return localAccount;
        }

        private bool IsEMPIFeatureEnabled(IAccount localAccount)
        {
            EMPIFeatureManager = new EMPIFeatureManager(localAccount.Facility);
            return (EMPIFeatureManager.IsEMPIFeatureEnabled(localAccount.Activity));
        }

        private void DoActivate( object sender, DoWorkEventArgs e )
        {
            // if we came here via the Worklists (No-Show), make the RegistrationView visible            
          
            IAccount anAccount = Model as IAccount;

            if (anAccount != null)
            {
                Account realAccount = AccountActivityService.SelectedAccountFor(anAccount);
                realAccount.IsShortRegistered = anAccount.IsShortRegistered;
                // poll CancellationPending and set e.Cancel to true and return 
                if (backgroundWorkerActivate.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                realAccount.Activity = new ShortRegistrationActivity
                    { AssociatedActivityType = typeof (ActivatePreRegistrationActivity) };
                realAccount.Activity.EmpiPatient = anAccount.Activity.EmpiPatient;
                CurrentActivity.EmpiPatient = anAccount.Activity.EmpiPatient;

                // check for any remaining actions, deactivated codes, remaining errors,
                // or required fields

                RuleEngine.GetInstance().LoadRules(realAccount);
                RuleEngine.GetInstance().EvaluateAllRules(realAccount);

                //fix Defect 15461: use the same implementation in RegistrationView here, except removing streamlined view part
                string deactivatedCodes = RuleEngine.GetInstance().GetInvalidCodeFieldSummary();
                string remainingErrors = RuleEngine.GetInstance().GetRemainingErrorsSummary();
                string requiredFields = RuleEngine.GetInstance().GetRequiredFieldSummary();

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
                }
                realAccount.RemovePreMseFinancialClass();
                // poll CancellationPending and set e.Cancel to true and return 
                if (backgroundWorkerActivate.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Model = realAccount;
            }
        }

        private void ActivateWithEdit( Account realAccount )
        {
            EnableAccountView();

            if( accountView != null )
            {
                accountView.OnCloseActivity    += ReturnToMainScreen;
                accountView.OnEditAccount      += OnEditAccount;
                accountView.OnRepeatActivity   += OnRepeatActivity;

                accountView.Model = realAccount;
                accountView.UpdateView();
                accountView.Show();
            }
        }

        private void OnEditAccount( object sender, EventArgs e )
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
        #endregion

        #region Methods
        #endregion

        #region Properties

        public Activity CurrentActivity
        {
            get { return i_CurrentActivity ?? (i_CurrentActivity = new ShortRegistrationActivity()); }
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

            if (masterPatientIndexView != null
                && masterPatientIndexView.TheSearchView != null
                && masterPatientIndexView.ThePatientsAccountsView != null )
            {

                masterPatientIndexView.TheSearchView.Visible = false;
                masterPatientIndexView.ThePatientsAccountsView.Visible = false;
            }

            ClearControls();

            accountView = AccountView.NewInstance();
            accountView.Model = Model as IAccount;

            accountView.Dock = DockStyle.Fill;
            
            Controls.Add( ( Control )accountView );

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
            SearchEventAggregator.GetInstance().EditRegistrationEvent -= EditRegistrationEventHandler;

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
            this.Name = "ShortRegistrationView";
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

        public ShortRegistrationView()
        {
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected += AccountSelectedEventHandler;
            SearchEventAggregator.GetInstance().ActivatePreregisteredAccount += ActivatePreregisteredAccountEventHandler;
            SearchEventAggregator.GetInstance().EditRegistrationEvent += EditRegistrationEventHandler;
        }
        #endregion

        #region Data Elements

        private static readonly ILog c_log = LogManager.GetLogger( typeof( ShortRegistrationView ) );

        private IAccount                                                SelectedAccount;
        IAccountView                                    accountView;
        private Container                         components = null;
        private MasterPatientIndexView   masterPatientIndexView;
        private Activity                                                i_CurrentActivity;
        private BackgroundWorker                  backgroundWorker;
        private BackgroundWorker                  backgroundWorkerActivate;
        private ProgressPanel                                           progressPanel1;

        #endregion

        #region Constants
        #endregion
    }
}
 