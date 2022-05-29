using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.HelperClasses;
using log4net;

namespace PatientAccess.UI.PatientSearch
{
    [Serializable]
    public class MasterPatientIndexView : ControlView
    {
        #region Events
        public event EventHandler ReturnToMainScreen;
        #endregion

        #region Event Handlers
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
                        _Logger.Error( "Failed to dispose of a control; " + ex.Message, ex );
                    }
                }
            }
            Controls.Clear();
        }

        private void MasterPatientIndexView_Load( object sender, EventArgs e )
        {
            this.ClearControls();

            this.EnablePatientsAccountsView();

            this._TheSearchView = new SearchView( this );
            //searchView.ReturnToMainScreen += new EventHandler( OnReturnToMainScreen );


            if( !this.IsInDesignMode )
            {
                this._TheSearchView.CurrentActivity = this.CurrentActivity;
            }
            
            this.Controls.Add( this._TheSearchView );
            this.Controls[INDEX_OF_SEARCH_VIEW].Dock = DockStyle.Fill;            
        }

        private void OnReturnToMainScreen( object sender , EventArgs e )
        {
            if( ReturnToMainScreen != null )
            {
                CancelBackgroundWorker();
                ReturnToMainScreen(sender, e);
            }

        }

        private void CancelBackgroundWorker()
        {
            // cancel the background worker here...
            if (this._BackgroundWorker != null)
            {
                this._BackgroundWorker.CancelAsync();
            }
        }

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            this._ThePatientsAccountsView.BeforeWork();
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if ( this.IsDisposed || this.Disposing )   // user may start another activity already
            {
                return ;
            }

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
                    MessageBox.Show(UIErrorMessages.TIMEOUT_GENERAL);
                    this._TheSearchView.Show();
                    this._ThePatientsAccountsView.Hide();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                this._ThePatientsAccountsView.Model = this._ThePatient;
                ( (SearchView)this.Controls[INDEX_OF_SEARCH_VIEW] ).Hide( );
                    this._ThePatientsAccountsView.UpdateView( );
            }

            this.Cursor = Cursors.Default;
        }

        private void DoOnPatientSelectedFromSearchResult(object sender, DoWorkEventArgs e)
        { 
            // poll CancellationPending and set e.Cancel to true and return 
            if ( this._BackgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            } 
        } 

        public void OnPatientSelectedFromSearchResult( object sender, PatientSelectedEventArgs patientSelectedEventArgs )
        {                        

            this._ThePatient = patientSelectedEventArgs.Patient as Patient;
            this._ThePatientsAccountsView.SetSearchedAccountNumber( patientSelectedEventArgs.AccountNumber );
            this._ThePatientsAccountsView.Show( );
            this._TheSearchView.Hide( );
                       
            if( this._BackgroundWorker == null ||
                ( this._BackgroundWorker != null && !this._BackgroundWorker.IsBusy ) )
            {

                this.BeforeWork();

                this._BackgroundWorker = new BackgroundWorker();
                this._BackgroundWorker.WorkerSupportsCancellation = true;

                this._BackgroundWorker.DoWork +=
                    new DoWorkEventHandler( DoOnPatientSelectedFromSearchResult );
                this._BackgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler( AfterWork );

                this._BackgroundWorker.RunWorkerAsync();
            }
        }

        public void OnReturnToSearch( object sender, EventArgs e )
        {
            this.Controls[INDEX_OF_SEARCH_VIEW].Show();
            this.Controls[INDEX_OF_PATIENTS_ACCOUNTS_VIEWS].Hide();
            ((SearchView)this.Controls[INDEX_OF_SEARCH_VIEW]).PatientSearchResultsView.FocusOnGrid();
        }

        #endregion

        #region Methods

        public void RemoveSearch()
        {
            this.Controls.Remove(this._TheSearchView);
            this.Controls.Remove(this._ThePatientsAccountsView);
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (_Components != null) 
                {
                    _Components.Dispose();
                }
                // cancel the background worker here...
                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }

        public override void UpdateView()
        {
        }

        /// <summary>
        /// UpdateModel method.
        /// </summary>
        public override void UpdateModel()
        {
        }

        #endregion

        #region Properties

        public Activity CurrentActivity
        {
            get
            {
                return this._CurrentActivity;
            }
            set
            {
                this._CurrentActivity = value;
            }
        }

        public SearchView TheSearchView
        {
            get
            {
                return this._TheSearchView;
            }
        }

        public PatientsAccountsView ThePatientsAccountsView
        {
            get
            {
                return this._ThePatientsAccountsView;
            }
            set
            {
                this._ThePatientsAccountsView = value;
            }
        }

        #endregion

        #region Private Methods

        private void EnablePatientsAccountsView()
        {
            this._ThePatientsAccountsView = new PatientsAccountsView(this);

            if( this.PatientsAccountsViewIsLoaded() )
            {
                this.Controls.RemoveAt( INDEX_OF_PATIENTS_ACCOUNTS_VIEWS );
            }

            this._ThePatientsAccountsView.Visible = false;
            this.Controls.Add( this._ThePatientsAccountsView );

            this._ThePatientsAccountsView.Dock = DockStyle.Fill;                        
        }

        private bool PatientsAccountsViewIsLoaded()
        {
            return this.Controls.Count > 1;
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MasterPatientIndexView
            // 
            this.Name = "MasterPatientIndexView";
            this.Size = new System.Drawing.Size(200, 141);
            this.Load += new System.EventHandler(this.MasterPatientIndexView_Load);
            this.ResumeLayout(false);

        }
        #endregion

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public MasterPatientIndexView()
        {
            InitializeComponent();
        }
        #endregion

        #region Data Elements

        private static readonly ILog _Logger = 
            LogManager.GetLogger( typeof( MasterPatientIndexView ) );
        private Container _Components = null;
        private BackgroundWorker _BackgroundWorker;
        private Patient _ThePatient;
        private Activity _CurrentActivity;
        private SearchView _TheSearchView;
        private PatientsAccountsView _ThePatientsAccountsView;

        #endregion

        #region Constants
        private const int
            INDEX_OF_SEARCH_VIEW                = 1,
            INDEX_OF_PATIENTS_ACCOUNTS_VIEWS    = 0;
        #endregion
    }
}
