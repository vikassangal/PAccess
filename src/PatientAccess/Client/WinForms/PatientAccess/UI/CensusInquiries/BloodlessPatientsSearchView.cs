using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for BloodlessPatientsSearchView.
    /// </summary>
    public class BloodlessPatientsSearchView : ControlView
    {

        #region Events

        public event EventHandler AccountsFound;
        public event EventHandler ResetView;
        public event EventHandler NoAccountsFound;
        public event EventHandler PrintReport;
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;

        #endregion

        #region Event Handlers

        private void BloodlessPatientsSearchView_Load(object sender, EventArgs e)
        {
            this.printReportButton.Enabled = false;
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if( ResetView != null )
            {
                ResetView( this, new LooseArgs( this ) );
            }            
            SetSearchCriteria();            
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            // cancel the background worker here since a reset refers to the search too  
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync() ; 
            
            ResetSearchView();
            if( ResetView != null )
            {
                ResetView( this, new LooseArgs( this ) );
            }            
        }

        private void printReportButton_Click(object sender, EventArgs e)
        {
            if( PrintReport != null )
            {
                PrintReport( this, new ReportArgs( 
                    accountProxiesCollection, SearchCriteria, null ) );
            }
        }

        private void regRadioButton_Enter(object sender, EventArgs e)
        {
            allDatesRadioButton.Enabled = true;
            TodayRadioButton.Enabled = true;
        }

        private void preRegRadioButton_Enter(object sender, EventArgs e)
        {
            allDatesRadioButton.Checked = true;
            allDatesRadioButton.Enabled = false;
            TodayRadioButton.Enabled = false;
        }

        private void allPatientsRadioButton_Enter(object sender, EventArgs e)
        {
            allDatesRadioButton.Checked = true;
            allDatesRadioButton.Enabled = false;
            TodayRadioButton.Enabled = false;
        }

        private void censusView_Close(object sender, EventArgs e)
        {
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync() ;
        }

        #endregion

        #region Construction and Initialization
        public BloodlessPatientsSearchView()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            base.EnableThemesOn( this );
            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
                // This is a harmless catch all that reveals the need for an urgent redesign. (Working on it.)
                if (this.backgroundWorker != null)
                    this.backgroundWorker.CancelAsync();
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.searchPanel = new System.Windows.Forms.Panel();
			this.printReportButton = new LoggingButton();
			this.resetButton = new LoggingButton();
			this.showButton = new LoggingButton();
			this.admitDatePanel = new System.Windows.Forms.Panel();
			this.TodayRadioButton = new System.Windows.Forms.RadioButton();
			this.allDatesRadioButton = new System.Windows.Forms.RadioButton();
			this.admitDateLabel = new System.Windows.Forms.Label();
			this.showPatientsPanel = new System.Windows.Forms.Panel();
			this.allPatientsRadioButton = new System.Windows.Forms.RadioButton();
			this.preRegRadioButton = new System.Windows.Forms.RadioButton();
			this.regRadioButton = new System.Windows.Forms.RadioButton();
			this.showPatientsLabel = new System.Windows.Forms.Label();
			this.searchPanel.SuspendLayout();
			this.admitDatePanel.SuspendLayout();
			this.showPatientsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// searchPanel
			// 
			this.searchPanel.BackColor = System.Drawing.Color.White;
			this.searchPanel.Controls.Add(this.printReportButton);
			this.searchPanel.Controls.Add(this.resetButton);
			this.searchPanel.Controls.Add(this.showButton);
			this.searchPanel.Controls.Add(this.admitDatePanel);
			this.searchPanel.Controls.Add(this.showPatientsPanel);
			this.searchPanel.Location = new System.Drawing.Point(0, 0);
			this.searchPanel.Name = "searchPanel";
			this.searchPanel.Size = new System.Drawing.Size(897, 64);
			this.searchPanel.TabIndex = 0;
			// 
			// printReportButton
			// 
			this.printReportButton.Location = new System.Drawing.Point(820, 19);
			this.printReportButton.Name = "printReportButton";
			this.printReportButton.TabIndex = 5;
			this.printReportButton.Text = "Pri&nt Report";
			this.printReportButton.Click += new System.EventHandler(this.printReportButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(665, 19);
			this.resetButton.Name = "resetButton";
			this.resetButton.TabIndex = 3;
			this.resetButton.Text = "Rese&t";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// showButton
			// 
			this.showButton.Location = new System.Drawing.Point(585, 19);
			this.showButton.Name = "showButton";
			this.showButton.TabIndex = 2;
			this.showButton.Text = "Sh&ow";
			this.showButton.Click += new System.EventHandler(this.showButton_Click);
			// 
			// admitDatePanel
			// 
			this.admitDatePanel.Controls.Add(this.TodayRadioButton);
			this.admitDatePanel.Controls.Add(this.allDatesRadioButton);
			this.admitDatePanel.Controls.Add(this.admitDateLabel);
			this.admitDatePanel.Location = new System.Drawing.Point(364, 0);
			this.admitDatePanel.Name = "admitDatePanel";
			this.admitDatePanel.Size = new System.Drawing.Size(218, 50);
			this.admitDatePanel.TabIndex = 1;
			this.admitDatePanel.TabStop = true;
			// 
			// TodayRadioButton
			// 
			this.TodayRadioButton.Enabled = false;
			this.TodayRadioButton.Location = new System.Drawing.Point(152, 19);
			this.TodayRadioButton.Name = "TodayRadioButton";
			this.TodayRadioButton.Size = new System.Drawing.Size(58, 24);
			this.TodayRadioButton.TabIndex = 0;
			this.TodayRadioButton.Text = "Today";
			// 
			// allDatesRadioButton
			// 
			this.allDatesRadioButton.Checked = true;
			this.allDatesRadioButton.Enabled = false;
			this.allDatesRadioButton.Location = new System.Drawing.Point(72, 19);
			this.allDatesRadioButton.Name = "allDatesRadioButton";
			this.allDatesRadioButton.Size = new System.Drawing.Size(66, 24);
			this.allDatesRadioButton.TabIndex = 0;
			this.allDatesRadioButton.TabStop = true;
			this.allDatesRadioButton.Text = "All dates";
			// 
			// admitDateLabel
			// 
			this.admitDateLabel.Location = new System.Drawing.Point(0, 24);
			this.admitDateLabel.Name = "admitDateLabel";
			this.admitDateLabel.Size = new System.Drawing.Size(64, 24);
			this.admitDateLabel.TabIndex = 0;
			this.admitDateLabel.Text = "Admit date:";
			this.admitDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// showPatientsPanel
			// 
			this.showPatientsPanel.Controls.Add(this.allPatientsRadioButton);
			this.showPatientsPanel.Controls.Add(this.preRegRadioButton);
			this.showPatientsPanel.Controls.Add(this.regRadioButton);
			this.showPatientsPanel.Controls.Add(this.showPatientsLabel);
			this.showPatientsPanel.Location = new System.Drawing.Point(0, 0);
			this.showPatientsPanel.Name = "showPatientsPanel";
			this.showPatientsPanel.Size = new System.Drawing.Size(319, 50);
			this.showPatientsPanel.TabIndex = 0;
			this.showPatientsPanel.TabStop = true;
			// 
			// allPatientsRadioButton
			// 
			this.allPatientsRadioButton.Checked = true;
			this.allPatientsRadioButton.Location = new System.Drawing.Point(88, 19);
			this.allPatientsRadioButton.Name = "allPatientsRadioButton";
			this.allPatientsRadioButton.Size = new System.Drawing.Size(40, 24);
			this.allPatientsRadioButton.TabIndex = 0;
			this.allPatientsRadioButton.TabStop = true;
			this.allPatientsRadioButton.Text = "All";
			this.allPatientsRadioButton.Enter += new System.EventHandler(this.allPatientsRadioButton_Enter);
			// 
			// preRegRadioButton
			// 
			this.preRegRadioButton.Location = new System.Drawing.Point(142, 19);
			this.preRegRadioButton.Name = "preRegRadioButton";
			this.preRegRadioButton.Size = new System.Drawing.Size(91, 24);
			this.preRegRadioButton.TabIndex = 0;
			this.preRegRadioButton.Text = "Preregistered";
			this.preRegRadioButton.Enter += new System.EventHandler(this.preRegRadioButton_Enter);
			// 
			// regRadioButton
			// 
			this.regRadioButton.Location = new System.Drawing.Point(247, 19);
			this.regRadioButton.Name = "regRadioButton";
			this.regRadioButton.Size = new System.Drawing.Size(77, 24);
			this.regRadioButton.TabIndex = 0;
			this.regRadioButton.Text = "Registered";
			this.regRadioButton.Enter += new System.EventHandler(this.regRadioButton_Enter);
			// 
			// showPatientsLabel
			// 
			this.showPatientsLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.showPatientsLabel.Location = new System.Drawing.Point(0, 24);
			this.showPatientsLabel.Name = "showPatientsLabel";
			this.showPatientsLabel.Size = new System.Drawing.Size(80, 24);
			this.showPatientsLabel.TabIndex = 0;
			this.showPatientsLabel.Text = "Show patients:";
			this.showPatientsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// BloodlessPatientsSearchView
			// 
			this.AcceptButton = this.showButton;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.searchPanel);
			this.Name = "BloodlessPatientsSearchView";
			this.Size = new System.Drawing.Size(897, 64);
			this.Load += new System.EventHandler(this.BloodlessPatientsSearchView_Load);
			this.searchPanel.ResumeLayout(false);
			this.admitDatePanel.ResumeLayout(false);
			this.showPatientsPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        #endregion

        #region Methods
        #endregion

        #region Private Methods

        private void SearchData()
        {
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
                    new DoWorkEventHandler( DoSearchData );
                backgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler( AfterWork );

                backgroundWorker.RunWorkerAsync();

            }
        }

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            if( this.BeforeWorkEvent != null )
            {
                this.BeforeWorkEvent( this, null );
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
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
                // Handle the cancelled case first
                if (e.Error.GetType() == typeof(RemotingTimeoutException))
                {
                    MessageBox.Show(UIErrorMessages.TIMEOUT_CENSUS_REPORT_DISPLAY);
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success
                if (accountProxiesCollection != null)
                {
                    if (accountProxiesCollection.Count != 0 && AccountsFound != null)
                    {
                        AccountsFound(this, new LooseArgs(accountProxiesCollection));
                        printReportButton.Enabled = true;
                    }
                    else
                    {
                        if (NoAccountsFound != null)
                        {
                            NoAccountsFound(this, new LooseArgs(this));
                            printReportButton.Enabled = false;
                        }
                    }
                }
 
/*
                else
                {
                    if( accountProxiesCollection != null )
                    {
                        if( accountProxiesCollection.Count != 0 && AccountsFound != null )
                        {
                            AccountsFound( this, new LooseArgs( accountProxiesCollection ) );
                            printReportButton.Enabled = true;
                        }
                        else
                        {
                            if( NoAccountsFound != null )
                            {
                                NoAccountsFound( this, new LooseArgs( this ) );
                                printReportButton.Enabled = false;
                            }
                        }
                    }
                }
 */
            }

            if( this.AfterWorkEvent != null )
            {
                this.AfterWorkEvent( this, null );
            }
            
            this.Cursor = Cursors.Default;
        }

        private void DoSearchData( object sender, DoWorkEventArgs e )
        {
                if (this.backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                } 
                IAccountBroker accountBroker =
                    BrokerFactory.BrokerOfType<IAccountBroker>();
                accountProxiesCollection = accountBroker.BloodlessTreatmentAccountsFor(
                    User.GetCurrent().Facility.Code, patientType, admitDate);

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }       
        }

        private void ResetSearchView()
        {
            printReportButton.Enabled = false;
            allPatientsRadioButton.Checked = true;
            allDatesRadioButton.Checked = true;
            allDatesRadioButton.Enabled = false;
            TodayRadioButton.Enabled = false;            
        }

        private void SetSearchCriteria()
        {
            if( preRegRadioButton.Checked.Equals( true ) )
            {
                patientType = PREREGISTEREDPATIENTS;
                patientTypeSelected = "Preregistered";
            }
            else if( regRadioButton.Checked.Equals( true ) )
            {
                patientType = REGISTEREDPATIENTS;
                patientTypeSelected = "Registered";
            }
            else
            {
                patientType = ALL;
                patientTypeSelected = "All";
            }

            if( TodayRadioButton.Checked.Equals( true ) )
            {
                admitDate = TODAY;
                admitDateSelected = "Today";
            }
            else
            {
                admitDate = ALL;
                admitDateSelected = "All dates";
            }

            SearchCriteria.Clear();
            SearchCriteria.Add( patientTypeSelected );
            SearchCriteria.Add( admitDateSelected );

            this.SearchData();
        }
        
        #endregion

        #region Properties
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private Container                         components = null;

        private BackgroundWorker                  backgroundWorker;

        private LoggingButton                                           showButton;
        private LoggingButton                                           resetButton;
        private LoggingButton                                           printReportButton;
                            
        private Panel                              searchPanel;
        private Panel                              admitDatePanel;
        private Panel                              showPatientsPanel;

        private Label                              showPatientsLabel;
        private Label                              admitDateLabel;

        private RadioButton                        allPatientsRadioButton;
        private RadioButton                        preRegRadioButton;
        private RadioButton                        regRadioButton;        
        private RadioButton                        allDatesRadioButton;
        private RadioButton                        TodayRadioButton;

        private ArrayList                                               SearchCriteria = new ArrayList();

        private ICollection                                             accountProxiesCollection;

        private string                                                  patientType;
        private string                                                  admitDate;
        private string                                                  patientTypeSelected;
        private string                                                  admitDateSelected;

        #endregion

        #region Constants

        private const string 
            PREREGISTEREDPATIENTS   = "P",
            REGISTEREDPATIENTS = "R",
            TODAY = "T",
            ALL = "A";
        #endregion        
    }
}
