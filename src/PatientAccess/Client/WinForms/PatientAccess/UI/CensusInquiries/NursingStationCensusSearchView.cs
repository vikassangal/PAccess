using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Nursing Station Search Panel View
    /// </summary>
    [Serializable]
    public class NursingStationCensusSearchView : ControlView
    {

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.selection = new PatientAccess.UI.CensusInquiries.ReportTypeSelection();
            this.NSCensusSearchpanel = new System.Windows.Forms.Panel();
            this.pnlRadioBtns = new System.Windows.Forms.Panel();
            this.bedTypeLabel = new System.Windows.Forms.Label();
            this.allRadioButton = new System.Windows.Forms.RadioButton();
            this.unOccupiedRadioButton = new System.Windows.Forms.RadioButton();
            this.printButton = new LoggingButton();
            this.resetButton = new LoggingButton();
            this.showButton = new LoggingButton();
            this.nursingStationComboBox = new System.Windows.Forms.ComboBox();
            this.nursingStationLabel = new System.Windows.Forms.Label();
            this.NSCensusSearchpanel.SuspendLayout();
            this.pnlRadioBtns.SuspendLayout();
            this.SuspendLayout();
            // 
            // selection
            // 
            this.selection.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.selection.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(209)), ((System.Byte)(228)), ((System.Byte)(243)));
            this.selection.ClientSize = new System.Drawing.Size(504, 237);
            this.selection.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.selection.Location = new System.Drawing.Point(22, 29);
            this.selection.MaximizeBox = false;
            this.selection.MinimizeBox = false;
            this.selection.Model = null;
            this.selection.Name = "selection";
            this.selection.ShowInTaskbar = false;
            this.selection.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.selection.Text = "Select Report Type";
            this.selection.Visible = false;
            this.selection.ReportTypeSelected += new System.EventHandler(this.PrintSelectedReport);
            // 
            // NSCensusSearchpanel
            // 
            this.NSCensusSearchpanel.BackColor = System.Drawing.Color.White;
            this.NSCensusSearchpanel.Controls.Add(this.pnlRadioBtns);
            this.NSCensusSearchpanel.Controls.Add(this.printButton);
            this.NSCensusSearchpanel.Controls.Add(this.resetButton);
            this.NSCensusSearchpanel.Controls.Add(this.showButton);
            this.NSCensusSearchpanel.Controls.Add(this.nursingStationComboBox);
            this.NSCensusSearchpanel.Controls.Add(this.nursingStationLabel);
            this.NSCensusSearchpanel.Location = new System.Drawing.Point(0, 0);
            this.NSCensusSearchpanel.Name = "NSCensusSearchpanel";
            this.NSCensusSearchpanel.Size = new System.Drawing.Size(896, 48);
            this.NSCensusSearchpanel.TabIndex = 0;
            // 
            // pnlRadioBtns
            // 
            this.pnlRadioBtns.Controls.Add(this.bedTypeLabel);
            this.pnlRadioBtns.Controls.Add(this.allRadioButton);
            this.pnlRadioBtns.Controls.Add(this.unOccupiedRadioButton);
            this.pnlRadioBtns.Location = new System.Drawing.Point(8, 8);
            this.pnlRadioBtns.Name = "pnlRadioBtns";
            this.pnlRadioBtns.Size = new System.Drawing.Size(368, 112);
            this.pnlRadioBtns.TabIndex = 0;
            // 
            // bedTypeLabel
            // 
            this.bedTypeLabel.AutoSize = true;
            this.bedTypeLabel.Location = new System.Drawing.Point(8, 8);
            this.bedTypeLabel.Name = "bedTypeLabel";
            this.bedTypeLabel.Size = new System.Drawing.Size(82, 16);
            this.bedTypeLabel.TabIndex = 1;
            this.bedTypeLabel.Text = "Show bed type:";
            this.bedTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // allRadioButton
            // 
            this.allRadioButton.Checked = true;
            this.allRadioButton.Location = new System.Drawing.Point(96, 8);
            this.allRadioButton.Name = "allRadioButton";
            this.allRadioButton.Size = new System.Drawing.Size(40, 16);
            this.allRadioButton.TabIndex = 1;
            this.allRadioButton.TabStop = true;
            this.allRadioButton.Text = "All";
            // 
            // unOccupiedRadioButton
            // 
            this.unOccupiedRadioButton.Location = new System.Drawing.Point(136, 8);
            this.unOccupiedRadioButton.Name = "unOccupiedRadioButton";
            this.unOccupiedRadioButton.Size = new System.Drawing.Size(240, 16);
            this.unOccupiedRadioButton.TabIndex = 2;
            this.unOccupiedRadioButton.TabStop = true;
            this.unOccupiedRadioButton.Text = "Unoccupied (includes pending admission)";
            // 
            // printButton
            // 
            this.printButton.BackColor = System.Drawing.SystemColors.Control;
            this.printButton.Enabled = false;
            this.printButton.Location = new System.Drawing.Point(816, 16);
            this.printButton.Name = "printButton";
            this.printButton.TabIndex = 6;
            this.printButton.Text = "Pri&nt Report";
            this.printButton.Click += new System.EventHandler(this.PrintButtonClick);
            // 
            // resetButton
            // 
            this.resetButton.BackColor = System.Drawing.SystemColors.Control;
            this.resetButton.Location = new System.Drawing.Point(614, 16);
            this.resetButton.Name = "resetButton";
            this.resetButton.TabIndex = 5;
            this.resetButton.Text = "Rese&t";
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // showButton
            // 
            this.showButton.BackColor = System.Drawing.SystemColors.Control;
            this.showButton.Location = new System.Drawing.Point(529, 16);
            this.showButton.Name = "showButton";
            this.showButton.TabIndex = 4;
            this.showButton.Text = "Sh&ow";
            this.showButton.Click += new System.EventHandler(this.ShowButtonClick);
            // 
            // nursingStationComboBox
            // 
            this.nursingStationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nursingStationComboBox.Location = new System.Drawing.Point(474, 16);
            this.nursingStationComboBox.Name = "nursingStationComboBox";
            this.nursingStationComboBox.Size = new System.Drawing.Size(40, 21);
            this.nursingStationComboBox.TabIndex = 3;
            // 
            // nursingStationLabel
            // 
            this.nursingStationLabel.AutoSize = true;
            this.nursingStationLabel.Location = new System.Drawing.Point(375, 19);
            this.nursingStationLabel.Name = "nursingStationLabel";
            this.nursingStationLabel.Size = new System.Drawing.Size(93, 16);
            this.nursingStationLabel.TabIndex = 3;
            this.nursingStationLabel.Text = "In nursing station:";
            this.nursingStationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NursingStationCensusSearchView
            // 
            this.Controls.Add(this.NSCensusSearchpanel);
            this.Name = "NursingStationCensusSearchView";
            this.Size = new System.Drawing.Size(896, 208);
            this.NSCensusSearchpanel.ResumeLayout(false);
            this.pnlRadioBtns.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Events
        
        public event EventHandler AccountsFound;
        public event EventHandler ResetSearch;
        public event EventHandler SummaryFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler PrintReport;
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;

        #endregion

        #region Event Handlers
        
        private void PrintSelectedReport(object sender, EventArgs e)
        {
            string reportType = ( ( LooseArgs )e ).Context.ToString();
            SearchCriteria.Clear();
            SearchCriteria.Add( selectedBedType );
            SearchCriteria.Add( nursingStationCode );          
            SearchCriteria.Add( reportType );
            if( PrintReport != null )
            {
                if( nursingStationCode == "All" )
                {
                    PrintReport( this, new ReportArgs( 
                        accountProxiesCollection, SearchCriteria, 
                        nursingStations ) );
                }
                else
                {
                    PrintReport( this, new ReportArgs( 
                        accountProxiesCollection, SearchCriteria, 
                        selectedNursingStation ) );
                }   
            }
        }
        private void ResetButtonClick(object sender, EventArgs e)
        {
            allRadioButton.Checked = true;
            nursingStationComboBox.SelectedIndex = 0;
            this.printButton.Enabled = false;
            ResetSearch( this, new LooseArgs( null ) );
        }

        private void ShowButtonClick(object sender, EventArgs e)
        {
            this.ResetSearch(this, null);
            SearchData();
            SetSearchCriteria();
        }
       
        private void PrintButtonClick( object sender, EventArgs e )
        {            
            if( PrintReport != null )
            {
				
					selection.ShowDialog( this );
				
            }
        }

        private void censusView_Close(object sender, EventArgs e)
        {
            CancelBackgroundWorker();
        }

        // encapsulate cancellation functionality so it can be reused
        private void CancelBackgroundWorker()
        {
            if (this.backgroundWorker != null)
                this.backgroundWorker.CancelAsync();
        }

        #endregion

        #region Methods
        public override void UpdateView()
        {
            try
            {
                base.UpdateView ();
                if( this.Enabled && nursingStationComboBox.Items.Count == 0)
                {
                    user = User.GetCurrent();
                    facility = User.GetCurrent().Facility;

                    LocationBrokerProxy locationPBARBroker =  new LocationBrokerProxy( );

                    nursingStations = locationPBARBroker.NursingStationsFor(facility,false);

                    nursingStationComboBox.Items.Add( "All" );
                    nursingStationComboBox.SelectedItem = "All";  
                    NursingStation nursingStation;

                    for( int i = 0;i < nursingStations.Count; i++ )    
                    {
                        nursingStation = nursingStations[i];

                        if( !nursingStation.Code.Equals( ALL_NURSINGSTATION_CODE ) )
                        {                        
                            nursingStationComboBox.Items.Add( nursingStation.Code );
                        }
                    }
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }   
        }

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            user = User.GetCurrent();
            nursingStationCode = nursingStationComboBox.SelectedItem.ToString();
            selectedNursingStation.Clear();

            if( this.BeforeWorkEvent != null )
            {
                this.BeforeWorkEvent(this,null);
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

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
                this.printButton.Enabled = true;

                if (accountProxiesCollection != null)
                {
                    if (accountProxiesCollection.Count != 0 && AccountsFound != null)
                    {
                        AccountsFound(this, new LooseArgs(accountProxiesCollection));
                    }
                    else
                    {
                        if (NoAccountsFound != null)
                        {
                            NoAccountsFound(this, new LooseArgs(this));
                        }
                    }
                }

                if (nursingStationCode == "All")
                {
                    SummaryFound(this, new LooseArgs(nursingStations));
                }
                else
                {
                    foreach (NursingStation nursingStation in nursingStations)
                    {
                        if (nursingStation.Code == nursingStationCode)
                        {
                            selectedNursingStation.Add(nursingStation);
                            SummaryFound(this, new LooseArgs(selectedNursingStation));
                            break;
                        }
                    }
                }

/*
                if( e.Cancelled )
                {
                }
                else
                {
                    this.printButton.Enabled = true;

                    if( accountProxiesCollection != null )
                    {
                        if( accountProxiesCollection.Count != 0 && AccountsFound != null )
                        {
                            AccountsFound( this, new LooseArgs( accountProxiesCollection ) );
                        }
                        else
                        {
                            if( NoAccountsFound != null )
                            {
                                NoAccountsFound( this, new LooseArgs( this ) );
                            }
                        }
                    }

                    if( nursingStationCode == "All" )
                    {
                        SummaryFound( this, new LooseArgs( nursingStations ) );
                    }
                    else
                    {
                        foreach( NursingStation nursingStation in nursingStations )
                        {
                            if( nursingStation.Code == nursingStationCode )
                            {
                                selectedNursingStation.Add( nursingStation );
                                SummaryFound( this, new LooseArgs( selectedNursingStation ) );
                                break;
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

        private void DoSearchData( object sender, DoWorkEventArgs e )
        {
            isOccupiedBeds = unOccupiedRadioButton.Checked;

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            } 

            IAccountBroker accountBroker =
                BrokerFactory.BrokerOfType<IAccountBroker>();

            accountProxiesCollection = accountBroker.AccountsMatching(
                isOccupiedBeds,
                nursingStationCode,
                facility.Code);

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }     
        }
        
        private void SetSearchCriteria()
        {
            try
            {
                ArrayList allNursingStations = new ArrayList();

                if( unOccupiedRadioButton.Checked.Equals( true ) )
                {
                    selectedBedType = "Unoccupied";
                }          
                else
                {
                   selectedBedType = "All";
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public NursingStationCensusSearchView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);

            base.AcceptButton = this.showButton;
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                selection.Dispose();

                CancelBackgroundWorker();
            }
            base.Dispose( disposing );
        }
        #endregion

        #region Data Elements
        
        private BackgroundWorker      backgroundWorker;

        private ReportTypeSelection                          selection;
        private Facility                                     facility;
        private User                                         user;

        private LoggingButton                                printButton;
        private LoggingButton                                resetButton;
        private LoggingButton                                showButton;

        private Panel                   pnlRadioBtns;
        private Panel                   NSCensusSearchpanel;   

        private ComboBox                nursingStationComboBox;

        private Label                   nursingStationLabel;
        private Label                   bedTypeLabel;

        private RadioButton             unOccupiedRadioButton;
        private RadioButton             allRadioButton;

        private ICollection                                  accountProxiesCollection;

        private bool                                         isOccupiedBeds;           

        private string                                       nursingStationCode;
        private string                                       selectedBedType;

        private IList<NursingStation>                        nursingStations; 
        private IList<NursingStation>                        selectedNursingStation = new List<NursingStation>();  
        private ArrayList                                    SearchCriteria = new ArrayList();
        
        #endregion		

        #region Constants
        private const string ALL_NURSINGSTATION_CODE = "$$";
        #endregion

    }
}
