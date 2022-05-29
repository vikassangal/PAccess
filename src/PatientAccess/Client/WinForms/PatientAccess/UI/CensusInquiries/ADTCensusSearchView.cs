using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
    /// Admissions, Discharges and Transfers Census Search View
    /// </summary>
    [Serializable]
    public class ADTCensusSearchView : ControlView
    {
        #region Events

        public event EventHandler AccountsFound;
        public event EventHandler ResetSearch;
        public event EventHandler SummaryFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler SortChanged;
        public event EventHandler AcceptButtonChanged;
        public event EventHandler PrintReport;
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;
        
        #endregion

        #region Event Handlers

        private void startTimeTextbox_TextChanged(object sender, EventArgs e)
        {
//            UI.HelperClasses.DateValidator.AuditTimeEntry( this.startTimeTextbox );
        }

        private void ADTCensusSearchView_Load(object sender, EventArgs e)
        {
            this.startTimeTextbox.UnMaskedText  = "0000";
            this.i_ADTType                      = ADT_TYPE_ADMISSIONS;
            this.i_SortByColumn                 = SORT_BY_TIME;
        }
        
        private void admissionsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( admissionsRadioButton.Checked )
            {
                i_ADTType = ADT_TYPE_ADMISSIONS;
            }
        }

        private void dischargesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( dischargesRadioButton.Checked )
            {
                i_ADTType = ADT_TYPE_DISCHARGES;
            }
        }

        private void transfersRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( transfersRadioButton.Checked )
            {
                i_ADTType = ADT_TYPE_TRANSFERS;
            }
        }

        private void allADTRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( allADTRadioButton.Checked )
            {
                i_ADTType = ADT_TYPE_ALL;
            }
        }

        private void patientRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( patientRadioButton.Checked )
            {
                i_SortByColumn = SORT_BY_PATIENT;
                sortByColumn();
            }
        }

        private void timeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if( timeRadioButton.Checked )
            {
                i_SortByColumn = SORT_BY_TIME;
                sortByColumn();
            }
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            admissionsRadioButton.Checked = true;
            startTimeTextbox.UnMaskedText = "";
            allNursingStationsCheckBox.Checked = false;
            nursingStationsListBox.ClearSelected();
            timeRadioButton.Checked = true;
            this.printButton.Enabled = false;
            i_ADTType = ADT_TYPE_ADMISSIONS;
            i_SortByColumn = SORT_BY_TIME;
            accountProxiesCollection = null;

            // Cancel worker here when the reset button is pressed
            // this is in effect the cancel button
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync() ;

            if( ResetSearch != null )
            {
                ResetSearch( this, new LooseArgs( null ) );
            }
        }

        private void ShowButtonClick(object sender, EventArgs e)
        {
            if (ResetSearch != null)
            {
                ResetSearch(this, null);
            }
            SearchData();
        }
        
        private void PrintButtonClick(object sender, EventArgs e)
        {
            if( PrintReport != null )
            {
                adtSearchCriteria.ADTSortColumn = SortByColumn;
                adtSearchCriteria.StartTime = startTimeTextbox.UnMaskedText == ""? "00:00": startTimeTextbox.Text;
                PrintReport( this, new ReportArgs( accountProxiesCollection, 
                                                   adtSearchCriteria, 
                                                   selectedNursingStations ) );
            }
        }
        
        private void allNursingStationsCheckBox_CheckedChanged(object sender, EventArgs e)
        {        
            if( allNursingStationsCheckBox.Checked )
            {
                nursingStationsListBox.ClearSelected();
                nursingStationsListBox.Enabled = false;
            }
            else
            {
                nursingStationsListBox.Enabled = true;
            }
            CheckForDataEntered();
        }
        
        private void nursingStationsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForDataEntered();
        }

        private void startTimeTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint.X = e.X;
            mousePoint.Y = e.Y;
        }

        private void timeRadioButton_Enter(object sender, EventArgs e)
        {
            if( AcceptButtonChanged != null )
            {
                AcceptButtonChanged( this, new LooseArgs( this ) );
            }
        }

        private void timeRadioButton_Leave(object sender, EventArgs e)
        {
            if( this.ParentForm != null )
            {
                this.AcceptButton = showButton;
            }
        }

        private void patientRadioButton_Leave(object sender, EventArgs e)
        {
            if( this.ParentForm != null )
            {
                this.AcceptButton = showButton;
            }
        }

        private void patientRadioButton_Enter(object sender, EventArgs e)
        {
            if( AcceptButtonChanged != null )
            {
                AcceptButtonChanged( this, new LooseArgs( this ) );
            }         
        }

        private void AdmitTimeEnter( object sender, EventArgs e )
        {
            //SetAdmitTimeNormalBgColor();
        }

        private void AdmitTimeValidating( object sender, CancelEventArgs e )
        {
            Rectangle clientRect = ClientRectangle;

            if( VerifyAdmitTime() == false )
            {   // Prevent cursor from advancing to the next control
                e.Cancel = true;
            }
        }

        private void censusView_Close(object sender, EventArgs e)
        {
            if ( this.backgroundWorker != null )
                this.backgroundWorker.CancelAsync();
        }
        
        #endregion

        #region Methods
        
        public override void UpdateView()
        {
            base.UpdateView ();
            if( this.Enabled && nursingStationsListBox.Items.Count == 0)
            {
                facility = User.GetCurrent().Facility;
                LocationBrokerProxy locationPBARBroker =  new LocationBrokerProxy( );

                allnursingStations = locationPBARBroker.NursingStationsFor( facility, false );

                allNursingStation = new ArrayList();
                nursingStations = new ArrayList();

                NursingStation nursingStation;

                for( int i = 0; i < allnursingStations.Count; i++ )    
                {
                    nursingStation = allnursingStations[i];

                    if( nursingStation.Code.Trim().Equals( ALL_NURSINGSTATION_CODE ) )
                    {
                        nursingStation.Code = ALL_NURSINGSTATION_LABEL;
                        allNursingStation.Add( nursingStation );
                    }
                    else
                    {
                        nursingStations.Add( nursingStation );
                        nursingStationsListBox.Items.Add( nursingStation.Code );
                    }                    
                }
            }
        }

        #endregion

        #region Properties
        
        public string ADTType
        {
            get
            {
                return i_ADTType;
            }
            set
            {
                i_ADTType = value;
            }
        }
        
        public string SortByColumn
        {
            get
            {
                return i_SortByColumn;
            }
            set
            {
                i_SortByColumn = value;
            }
        }
        
        #endregion

        #region Private Methods


        private bool VerifyAdmitTime()
        {
            bool result = true;

            if( startTimeTextbox.UnMaskedText.Trim().Length == 4 )
            {
                admitHour   = Convert.ToInt32( startTimeTextbox.Text.Substring( 0, 2 ) );
                admitMinute = Convert.ToInt32( startTimeTextbox.Text.Substring( 3, 2 ) );

				if( admitHour > 23 )
				{
					SetAdmitTimeErrBgColor();
					MessageBox.Show( UIErrorMessages.HOUR_INVALID_ERRMSG, "Error",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
						MessageBoxDefaultButton.Button1 );
					startTimeTextbox.Focus();
					result = false;
				}
				
                else if ( admitMinute > 59 )
                {
                    SetAdmitTimeErrBgColor();
                    MessageBox.Show( UIErrorMessages.MINUTE_INVALID_ERRMSG, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1 );
					startTimeTextbox.Focus();
                    result = false;
                }
				else
				{
					SetAdmitTimeNormalBgColor();
				}
            }
            else if (startTimeTextbox.UnMaskedText.Trim().Length == 0)
            {
                SetAdmitTimeNormalBgColor();
                result = true;
            }
            else 
            {
                SetAdmitTimeErrBgColor();
                MessageBox.Show( UIErrorMessages.ADMIT_TIME_INVALID, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1 );
                startTimeTextbox.Focus();
                result = false;
            }
            return result;
        }

        private void SetAdmitTimeErrBgColor()
        {
            mousePoint.X = mousePoint.Y = -1;
            admitHour = admitMinute = -1;
            UIColors.SetErrorBgColor( startTimeTextbox );
        }

        private void SetAdmitTimeNormalBgColor()
        {
            mousePoint.X = mousePoint.Y = -1;
            UIColors.SetNormalBgColor( startTimeTextbox );
            this.Refresh();
        }

        private void PopulateSelectedNursingStationsList()
        {
            selectedNursingStations = new ArrayList();
            selectedNursingStationsList = String.Empty;
            NursingStation ns;

            if( this.allNursingStationsCheckBox.Checked )
            {
                if( ( allNursingStation != null ) && 
                    ( allNursingStation.Count > 0 ) )
                {
                    ns = (NursingStation)allNursingStation[0];
                    selectedNursingStations.Add( ns );
                }  

                for( int i = 0; i < nursingStations.Count; i++ )    
                {
                    ns = (NursingStation)nursingStations[i];
                    selectedNursingStations.Add( ns );
                }

                selectedNursingStationsList = ALL_NURSINGSTATIONS;
            }
            else
            {
                foreach( object listItem in nursingStationsListBox.SelectedItems ) 
                {
                    ns = (NursingStation)nursingStations[ 
                        nursingStationsListBox.Items.IndexOf( listItem.ToString() ) ];
                    selectedNursingStations.Add( ns );
                        
                    if( !selectedNursingStationsList.Equals( String.Empty ) )
                    {
                        selectedNursingStationsList =  
                            selectedNursingStationsList + ",";
                    }

                    selectedNursingStationsList = selectedNursingStationsList 
                        + "'" + listItem + "'";   
                }
            }
        }

        private void sortByColumn()
        {
            if( ( accountProxiesCollection != null ) && 
                ( accountProxiesCollection.Count > 0 ) && 
                ( SortChanged != null ) )
            {
                SortChanged( this, new LooseArgs( this ) );
            }
        }

        private void CheckForDataEntered()
        {
            if( allNursingStationsCheckBox.Checked ||  
                nursingStationsListBox.SelectedIndex >= 0 )
            {
                this.showButton.Enabled = true;

                if( this.ParentForm != null )
                {
                    this.AcceptButton = showButton;
                }
            }
            else
            {
                this.showButton.Enabled = false;
            }
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

        private void BeforeWork()
        {
            this.Cursor = Cursors.WaitCursor;

            if( VerifyAdmitTime() )
            {
                if(startTimeTextbox.UnMaskedText.Trim() == String.Empty   )
                {
                    startTime = DEFAULT_START_TIME;
                }
                else
                {
                    startTime = startTimeTextbox.UnMaskedText;
                }
            }
            else
            {
                MessageBox.Show( UIErrorMessages.PBAR_UNAVAILABLE_MSG, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1 );
            }

            PopulateSelectedNursingStationsList();

            if( SummaryFound != null )
            {
                this.printButton.Enabled = true;
                SummaryFound( this, new LooseArgs( selectedNursingStations ) );
            }

            if( this.BeforeWorkEvent != null )
            {
                this.BeforeWorkEvent( this, null );
            }
        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
        {
            if ( this.IsDisposed || this.Disposing )
                return ;

            // Handle the cancelled case first
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
                        this.printButton.Enabled = true;
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
            }

/*
            if ( e.Error != null )
            {
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
                if (e.Cancelled)
                {
                }
                else
                {
                    if (accountProxiesCollection != null)
                    {
                        if (accountProxiesCollection.Count != 0 && AccountsFound != null)
                        {
                            this.printButton.Enabled = true;
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
                }
            }
*/

            // place post completion operations here...
            if (this.AfterWorkEvent != null)
            {
                this.AfterWorkEvent(this, null);
            }
            this.Cursor = Cursors.Default;
        }
        
        /// <summary>
        /// 1. Validates the input ADT search criteria.
        /// 2. Searches for matching patients (results) data for the 
        ///    input ADT search criteria.
        /// 3. Updates the ADT Census Results View and ADT Census Summary Views
        ///    with results and summary data.
        /// </summary>
        private void DoSearchData( object sender, DoWorkEventArgs e )
        {
            // Sanjeev Kumar: do not add any try/catch statements within the DoWork.
            // Exceptions are caught in backgroundworkers automatically in the RunWorkerComplete
            // through e.Error, which can be checked.
     
            adtSearchCriteria = new CensusADTSearchCriteria(
                    startTime,
                    i_ADTType,
                    selectedNursingStationsList,
                    SortByColumn,
                    User.GetCurrent().Facility);

            IAccountBroker accountBroker =
                BrokerFactory.BrokerOfType<IAccountBroker>();

            accountProxiesCollection =
                accountBroker.AccountsMatching(adtSearchCriteria);

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: permit user cancellation of bgw. 
            if ( this.backgroundWorker.CancellationPending )
            {
                e.Cancel = true ;
                return ;
            }            
                
/*            
            try
            {                              
                adtSearchCriteria = new CensusADTSearchCriteria(
                        startTime,
                        i_ADTType,
                        selectedNursingStationsList,
                        SortByColumn,
                        User.GetCurrent().Facility );

                IAccountBroker accountBroker =
                    BrokerFactory.BrokerOfType<IAccountBroker>();
                    as IAccountBroker;

                accountProxiesCollection =
                    accountBroker.AccountsMatching( adtSearchCriteria );

                if (backgroundWorker.CancellationPending)
                    e.Cancel = true;               
            }
            catch( Exception )
            {
                throw;
            }
*/
        }       

        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        
        private void InitializeComponent()
        {
			this.adtCensusSearchPanel = new System.Windows.Forms.Panel();
			this.adtTypePanel = new System.Windows.Forms.Panel();
			this.adtTypeLabel = new System.Windows.Forms.Label();
			this.dischargesRadioButton = new System.Windows.Forms.RadioButton();
			this.admissionsRadioButton = new System.Windows.Forms.RadioButton();
			this.transfersRadioButton = new System.Windows.Forms.RadioButton();
			this.allADTRadioButton = new System.Windows.Forms.RadioButton();
			this.soryByPanel = new System.Windows.Forms.Panel();
			this.timeRadioButton = new System.Windows.Forms.RadioButton();
			this.sortByLabel = new System.Windows.Forms.Label();
			this.patientRadioButton = new System.Windows.Forms.RadioButton();
			this.allNursingStationsCheckBox = new System.Windows.Forms.CheckBox();
			this.nursingStationsListBox = new System.Windows.Forms.ListBox();
			this.startTimeTextbox = new Extensions.UI.Winforms.MaskedEditTextBox();
			this.printButton = new LoggingButton();
			this.resetButton = new LoggingButton();
			this.showButton = new LoggingButton();
			this.nursingStationsLabel = new System.Windows.Forms.Label();
			this.startTimeLabel = new System.Windows.Forms.Label();
			this.adtCensusSearchPanel.SuspendLayout();
			this.adtTypePanel.SuspendLayout();
			this.soryByPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// adtCensusSearchPanel
			// 
			this.adtCensusSearchPanel.BackColor = System.Drawing.Color.White;
			this.adtCensusSearchPanel.Controls.Add(this.adtTypePanel);
			this.adtCensusSearchPanel.Controls.Add(this.soryByPanel);
			this.adtCensusSearchPanel.Controls.Add(this.allNursingStationsCheckBox);
			this.adtCensusSearchPanel.Controls.Add(this.nursingStationsListBox);
			this.adtCensusSearchPanel.Controls.Add(this.startTimeTextbox);
			this.adtCensusSearchPanel.Controls.Add(this.printButton);
			this.adtCensusSearchPanel.Controls.Add(this.resetButton);
			this.adtCensusSearchPanel.Controls.Add(this.showButton);
			this.adtCensusSearchPanel.Controls.Add(this.nursingStationsLabel);
			this.adtCensusSearchPanel.Controls.Add(this.startTimeLabel);
			this.adtCensusSearchPanel.Location = new System.Drawing.Point(0, 0);
			this.adtCensusSearchPanel.Name = "adtCensusSearchPanel";
			this.adtCensusSearchPanel.Size = new System.Drawing.Size(940, 122);
			this.adtCensusSearchPanel.TabIndex = 0;
			// 
			// adtTypePanel
			// 
			this.adtTypePanel.Controls.Add(this.adtTypeLabel);
			this.adtTypePanel.Controls.Add(this.dischargesRadioButton);
			this.adtTypePanel.Controls.Add(this.admissionsRadioButton);
			this.adtTypePanel.Controls.Add(this.transfersRadioButton);
			this.adtTypePanel.Controls.Add(this.allADTRadioButton);
			this.adtTypePanel.Location = new System.Drawing.Point(10, 19);
			this.adtTypePanel.Name = "adtTypePanel";
			this.adtTypePanel.Size = new System.Drawing.Size(100, 100);
			this.adtTypePanel.TabIndex = 1;
			// 
			// adtTypeLabel
			// 
			this.adtTypeLabel.Location = new System.Drawing.Point(0, 0);
			this.adtTypeLabel.Name = "adtTypeLabel";
			this.adtTypeLabel.Size = new System.Drawing.Size(74, 16);
			this.adtTypeLabel.TabIndex = 9;
			this.adtTypeLabel.Text = "Show today\'s:";
			this.adtTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// dischargesRadioButton
			// 
			this.dischargesRadioButton.Location = new System.Drawing.Point(0, 39);
			this.dischargesRadioButton.Name = "dischargesRadioButton";
			this.dischargesRadioButton.Size = new System.Drawing.Size(80, 16);
			this.dischargesRadioButton.TabIndex = 2;
			this.dischargesRadioButton.Text = "Discharges";
			this.dischargesRadioButton.CheckedChanged += new System.EventHandler(this.dischargesRadioButton_CheckedChanged);
			// 
			// admissionsRadioButton
			// 
			this.admissionsRadioButton.Checked = true;
			this.admissionsRadioButton.Location = new System.Drawing.Point(0, 18);
			this.admissionsRadioButton.Name = "admissionsRadioButton";
			this.admissionsRadioButton.Size = new System.Drawing.Size(80, 16);
			this.admissionsRadioButton.TabIndex = 1;
			this.admissionsRadioButton.TabStop = true;
			this.admissionsRadioButton.Text = "Admissions";
			this.admissionsRadioButton.CheckedChanged += new System.EventHandler(this.admissionsRadioButton_CheckedChanged);
			// 
			// transfersRadioButton
			// 
			this.transfersRadioButton.Location = new System.Drawing.Point(0, 60);
			this.transfersRadioButton.Name = "transfersRadioButton";
			this.transfersRadioButton.Size = new System.Drawing.Size(80, 16);
			this.transfersRadioButton.TabIndex = 3;
			this.transfersRadioButton.Text = "Transfers";
			this.transfersRadioButton.CheckedChanged += new System.EventHandler(this.transfersRadioButton_CheckedChanged);
			// 
			// allADTRadioButton
			// 
			this.allADTRadioButton.Location = new System.Drawing.Point(0, 81);
			this.allADTRadioButton.Name = "allADTRadioButton";
			this.allADTRadioButton.Size = new System.Drawing.Size(80, 16);
			this.allADTRadioButton.TabIndex = 4;
			this.allADTRadioButton.Text = "All A-D-T";
			this.allADTRadioButton.CheckedChanged += new System.EventHandler(this.allADTRadioButton_CheckedChanged);
			// 
			// soryByPanel
			// 
			this.soryByPanel.Controls.Add(this.timeRadioButton);
			this.soryByPanel.Controls.Add(this.sortByLabel);
			this.soryByPanel.Controls.Add(this.patientRadioButton);
			this.soryByPanel.Location = new System.Drawing.Point(592, 19);
			this.soryByPanel.Name = "soryByPanel";
			this.soryByPanel.Size = new System.Drawing.Size(184, 20);
			this.soryByPanel.TabIndex = 7;
			// 
			// timeRadioButton
			// 
			this.timeRadioButton.Checked = true;
			this.timeRadioButton.Location = new System.Drawing.Point(49, 0);
			this.timeRadioButton.Name = "timeRadioButton";
			this.timeRadioButton.Size = new System.Drawing.Size(47, 16);
			this.timeRadioButton.TabIndex = 1;
			this.timeRadioButton.TabStop = true;
			this.timeRadioButton.Text = "Time";
			this.timeRadioButton.Enter += new System.EventHandler(this.timeRadioButton_Enter);
			this.timeRadioButton.Leave += new System.EventHandler(this.timeRadioButton_Leave);
			this.timeRadioButton.CheckedChanged += new System.EventHandler(this.timeRadioButton_CheckedChanged);
			// 
			// sortByLabel
			// 
			this.sortByLabel.Location = new System.Drawing.Point(0, 0);
			this.sortByLabel.Name = "sortByLabel";
			this.sortByLabel.Size = new System.Drawing.Size(43, 16);
			this.sortByLabel.TabIndex = 7;
			this.sortByLabel.Text = "Sort by:";
			// 
			// patientRadioButton
			// 
			this.patientRadioButton.Location = new System.Drawing.Point(101, 0);
			this.patientRadioButton.Name = "patientRadioButton";
			this.patientRadioButton.Size = new System.Drawing.Size(60, 16);
			this.patientRadioButton.TabIndex = 2;
			this.patientRadioButton.Text = "Patient";
			this.patientRadioButton.Enter += new System.EventHandler(this.patientRadioButton_Enter);
			this.patientRadioButton.Leave += new System.EventHandler(this.patientRadioButton_Leave);
			this.patientRadioButton.CheckedChanged += new System.EventHandler(this.patientRadioButton_CheckedChanged);
			// 
			// allNursingStationsCheckBox
			// 
			this.allNursingStationsCheckBox.Location = new System.Drawing.Point(257, 32);
			this.allNursingStationsCheckBox.Name = "allNursingStationsCheckBox";
			this.allNursingStationsCheckBox.Size = new System.Drawing.Size(87, 24);
			this.allNursingStationsCheckBox.TabIndex = 3;
			this.allNursingStationsCheckBox.Text = "All";
			this.allNursingStationsCheckBox.CheckedChanged += new System.EventHandler(this.allNursingStationsCheckBox_CheckedChanged);
			// 
			// nursingStationsListBox
			// 
			this.nursingStationsListBox.Location = new System.Drawing.Point(257, 58);
			this.nursingStationsListBox.Name = "nursingStationsListBox";
			this.nursingStationsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.nursingStationsListBox.Size = new System.Drawing.Size(50, 56);
			this.nursingStationsListBox.TabIndex = 4;
			this.nursingStationsListBox.SelectedIndexChanged += new System.EventHandler(this.nursingStationsListBox_SelectedIndexChanged);
			// 
			// startTimeTextbox
			// 
			this.startTimeTextbox.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.startTimeTextbox.EntrySelectionStyle = Extensions.UI.Winforms.EntrySelectionStyle.SelectionAtEnd;
			this.startTimeTextbox.KeyPressExpression = HelperClasses.DateValidator.TIMEKeyPressExpression;
			this.startTimeTextbox.Location = new System.Drawing.Point(184, 16);
			this.startTimeTextbox.Mask = "  :";
			this.startTimeTextbox.MaxLength = 5;
			this.startTimeTextbox.Name = "startTimeTextbox";
			this.startTimeTextbox.Size = new System.Drawing.Size(35, 20);
			this.startTimeTextbox.TabIndex = 2;
			this.startTimeTextbox.ValidationExpression = HelperClasses.DateValidator.TIMEValidationExpression;
			this.startTimeTextbox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.startTimeTextbox_MouseDown);
			this.startTimeTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.AdmitTimeValidating);
			this.startTimeTextbox.TextChanged += new System.EventHandler(this.startTimeTextbox_TextChanged);
			this.startTimeTextbox.Enter += new System.EventHandler(this.AdmitTimeEnter);
			// 
			// printButton
			// 
			this.printButton.BackColor = System.Drawing.SystemColors.Control;
			this.printButton.Enabled = false;
			this.printButton.Location = new System.Drawing.Point(825, 16);
			this.printButton.Name = "printButton";
			this.printButton.TabIndex = 8;
			this.printButton.Text = "Pri&nt Report";
			this.printButton.Click += new System.EventHandler(this.PrintButtonClick);
			// 
			// resetButton
			// 
			this.resetButton.BackColor = System.Drawing.SystemColors.Control;
			this.resetButton.Location = new System.Drawing.Point(471, 16);
			this.resetButton.Name = "resetButton";
			this.resetButton.TabIndex = 6;
			this.resetButton.Text = "Rese&t";
			this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
			// 
			// showButton
			// 
			this.showButton.BackColor = System.Drawing.SystemColors.Control;
			this.showButton.Enabled = false;
			this.showButton.Location = new System.Drawing.Point(391, 16);
			this.showButton.Name = "showButton";
			this.showButton.TabIndex = 5;
			this.showButton.Text = "Sh&ow";
			this.showButton.Click += new System.EventHandler(this.ShowButtonClick);
			// 
			// nursingStationsLabel
			// 
			this.nursingStationsLabel.AutoSize = true;
			this.nursingStationsLabel.Location = new System.Drawing.Point(257, 19);
			this.nursingStationsLabel.Name = "nursingStationsLabel";
			this.nursingStationsLabel.Size = new System.Drawing.Size(101, 16);
			this.nursingStationsLabel.TabIndex = 3;
			this.nursingStationsLabel.Text = "For nursing station:";
			// 
			// startTimeLabel
			// 
			this.startTimeLabel.AutoSize = true;
			this.startTimeLabel.Location = new System.Drawing.Point(120, 19);
			this.startTimeLabel.Name = "startTimeLabel";
			this.startTimeLabel.Size = new System.Drawing.Size(59, 16);
			this.startTimeLabel.TabIndex = 2;
			this.startTimeLabel.Text = "Starting at:";
			this.startTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ADTCensusSearchView
			// 
			this.Controls.Add(this.adtCensusSearchPanel);
			this.Name = "ADTCensusSearchView";
			this.Size = new System.Drawing.Size(912, 122);
			this.Load += new System.EventHandler(this.ADTCensusSearchView_Load);
			this.adtCensusSearchPanel.ResumeLayout(false);
			this.adtTypePanel.ResumeLayout(false);
			this.soryByPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
        
        #endregion

        #region Construction and Finalization
        
        public ADTCensusSearchView()
        {
            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);

            this.Cursor = Cursors.WaitCursor;
            InitializeComponent();
            base.EnableThemesOn( this );
            base.AcceptButton = this.showButton;


            this.Cursor = Cursors.Default;
        }
        
        #endregion

        #region Data Elements

        private BackgroundWorker              backgroundWorker;
        private CensusADTSearchCriteria                             adtSearchCriteria;
        private Facility                                            facility;

        private LoggingButton                                       printButton;
        private LoggingButton                                       resetButton;
        private LoggingButton                                       showButton;

        //private bool                                                isAvailable = true;

        private ICollection                                         accountProxiesCollection;

        private ArrayList                                           nursingStations;
        private ArrayList                                           allNursingStation;
        private IList<NursingStation>                               allnursingStations;
        private ArrayList                                           selectedNursingStations;

        private Point                                               mousePoint = new Point(-1, -1);

        private int                                                 admitHour;
        private int                                                 admitMinute;

        private string                                              i_SortByColumn;        
        private string                                              startTime;
        private string                                              selectedNursingStationsList;
        private string                                              i_ADTType;
        
        private Panel                          adtCensusSearchPanel;        
        private Panel                          soryByPanel;
        private Panel                          adtTypePanel;
        
        private RadioButton                    transfersRadioButton;
        private RadioButton                    allADTRadioButton;

        private Label                          startTimeLabel;
        private Label                          nursingStationsLabel;
        private Label                          adtTypeLabel;
        private Label                          sortByLabel;

        private MaskedEditTextBox            startTimeTextbox;

        private ListBox                        nursingStationsListBox;

        private CheckBox                       allNursingStationsCheckBox;

        private RadioButton                    dischargesRadioButton;        
        private RadioButton                    admissionsRadioButton;
        private RadioButton                    timeRadioButton;
        private RadioButton                    patientRadioButton;
        
        #endregion

        #region Constants
        
        private const string ALL_NURSINGSTATION_CODE = "$$";
        private const string ALL_NURSINGSTATION_LABEL = "ALL";
        private const string SORT_BY_TIME = "Time";
        private const string SORT_BY_PATIENT = "Patient";  
        private const string DEFAULT_START_TIME = "0000";  
        private const string ALL_NURSINGSTATIONS = "ALL";

        private const string
            ADT_TYPE_ADMISSIONS   = "A",
            ADT_TYPE_DISCHARGES   = "D",
            ADT_TYPE_TRANSFERS    = "T",
            ADT_TYPE_ALL          = "E";

        #endregion

    }
}
