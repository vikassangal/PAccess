using System;
using System.ComponentModel;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.UI.PatientSearch;
using log4net;
using PatientAccess.UI.PreMSEViews;

namespace PatientAccess.UI.UCCPreMSEViews
{
    /// <summary>
    /// Summary description for PreMseSearchView.
    /// </summary>
    public class UCCPreMseSearchView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;
            this.preMseRegistrationView.ProgressPanel.Visible = true;
        }

        private void DoWork( object sender, DoWorkEventArgs e )
        {
            // 7/18/07 - kjs - Unconditionally reload the account. 
            Account newAccount = AccountActivityService.SelectedAccountFor( this.selectedAccount );

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            this.Model = newAccount;

            newAccount.Activity = new UCCPreMSERegistrationActivity();
            if ( !newAccount.IsNew )
            {
                newAccount.Activity.AssociatedActivityType = typeof ( EditUCCPreMSEActivity );
            }

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if (this.IsDisposed || this.Disposing)
                return;

            if (e.Cancelled)
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
                preMseRegistrationView.Model = this.Model as Account;
                preMseRegistrationView.UpdateView();
            }

            this.Cursor = Cursors.Default;
            this.preMseRegistrationView.ProgressPanel.Visible = false;
        }

        private void AccountSelectedEventHandler( object sender, EventArgs e )
        {
            if (e != null)
            {
                if (( (LooseArgs)e ).Context != null)
                {
                    this.selectedAccount = ( (LooseArgs)e ).Context as IAccount;
                }
            }

            ClearControls();

            preMseRegistrationView = new PreMseRegistrationView();
            SuspendLayout();
            preMseRegistrationView.Dock = DockStyle.Fill;
            if (selectedAccount != null)
            {
                preMseRegistrationView.SetContextView(selectedAccount.Activity.ContextDescription);
            }
            Controls.Add( preMseRegistrationView );
            ResumeLayout( false );

            if (this.backgroundWorker == null
                ||
                ( this.backgroundWorker != null
                && !this.backgroundWorker.IsBusy )
                )
            {
                // pass in delegates to do your pre-processing, actual processing, and post-processing.
                // if you have no pre or post processing, simply pass nulls for those parms.


                this.BeforeWork();

                this.backgroundWorker = new BackgroundWorker();
                this.backgroundWorker.WorkerSupportsCancellation = true;

                backgroundWorker.DoWork += DoWork;
                backgroundWorker.RunWorkerCompleted += AfterWork;

                backgroundWorker.RunWorkerAsync();
            }
        }
        private void OnReturnToMainScreen( object sender, EventArgs e )
        {
            if (ReturnToMainScreen != null)
            {
                // cancels background worker
                CancelBackgroundWorker();

                ReturnToMainScreen( sender, e );
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
                if (currentActivity == null)
                {
                    currentActivity = new UCCPreMSERegistrationActivity();
                }
                return currentActivity;
            }
        }
        #endregion

        #region Private Methods

        private void ClearControls()
        {
            foreach (Control control in this.Controls)
            {
                if (control != null)
                {
                    try
                    {
                        control.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            Controls.Clear();
        }
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.masterPatientIndexView = new PatientAccess.UI.PatientSearch.MasterPatientIndexView();
            this.masterPatientIndexView.ReturnToMainScreen += new EventHandler( OnReturnToMainScreen );
            this.masterPatientIndexView.CurrentActivity = this.CurrentActivity;
            this.SuspendLayout();
            // 
            // masterPatientIndexView
            // 
            this.masterPatientIndexView.Location = new System.Drawing.Point( 0, 0 );
            this.masterPatientIndexView.Model = null;
            this.masterPatientIndexView.Name = "masterPatientIndexView";
            this.masterPatientIndexView.Size = new System.Drawing.Size( 1024, 512 );
            this.masterPatientIndexView.Dock = DockStyle.Fill;
            this.masterPatientIndexView.TabIndex = 0;
            // 
            // PreRegistrationView
            // 
            this.Controls.Add( this.masterPatientIndexView );
            this.Name = "PreRegistrationView";
            this.Size = new System.Drawing.Size( 1024, 512 );
            this.ResumeLayout( false );

        }
        #endregion
        #endregion

        #region Construction and Finalization
        public UCCPreMseSearchView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            SearchEventAggregator.GetInstance().AccountSelected += this.AccountSelectedEventHandler;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            SearchEventAggregator.GetInstance().AccountSelected -= this.AccountSelectedEventHandler;

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                // cancels background worker
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }

        // encapsulate cancellation of background worker in to a private helper method
        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }
        #endregion

        #region Data Elements
        private static readonly ILog Log = LogManager.GetLogger( typeof( UCCPreMseSearchView ) );

        private IAccount selectedAccount;
        private Activity currentActivity;

        private BackgroundWorker backgroundWorker;
        private readonly Container components = null;
        private PreMseRegistrationView preMseRegistrationView;
        private MasterPatientIndexView masterPatientIndexView;
        #endregion

        #region Constants
        #endregion
    }
}
