using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;
using Timer = System.Threading.Timer;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    ///  Base class for Worklist View classes
    /// </summary>
    public class WorklistView : ControlView
    {
        #region Event Handlers

        public event EventHandler WorklistSelectedIndexEvent;
        public event EventHandler WorklistEvent;
        public event EventHandler AccountNameEvent;
        public event EventHandler ResetActionsButtonClick;

        #endregion

        #region Event Handlers

        private void dgWorklistAccounts_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Return )
            {
                this.WorklistsView.GetSelectedAccount();
            }
        }

        private void WorklistView_Disposed(object sender, EventArgs e)
        {
            CancelBackgroundWorker();

            this.cancelTimer();
        }

        // Cancels background worker
        private void CancelBackgroundWorker()
        {
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }

        private void WorklistView_Leave(object sender, EventArgs e)
        {
            this.FilterWorklistView.WorkListView_Leave();
        }

        private void PeriodFilterSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args     = (LooseArgs) e;
            selectionRange     = args.Context as WorklistSelectionRange;

            DateTime temp = DateTime.Now;
            if( temp.Day != todaysDate.Day || temp.Month != todaysDate.Month || temp.Year != todaysDate.Year )
            {
                todaysDate = new DateTime( temp.Year, temp.Month, temp.Day );
            }

            switch( selectionRange.Oid )
            {
                case WorklistSelectionRange.ALL:
                    toDate   = new DateTime( 0 );
                    fromDate = new DateTime( 0 );
                    break;

                case WorklistSelectionRange.TODAY:
                    fromDate = todaysDate;
                    toDate   = todaysDate;
                    break;

                case WorklistSelectionRange.YESTERDAY:
                {
                    TimeSpan ts = new TimeSpan( -1, 0, 0, 0 );
                    DateTime yesterday = todaysDate + ts;

                    fromDate = yesterday;
                    toDate   = yesterday;
                }
                    break;

                case WorklistSelectionRange.LAST_3_DAYS:
                {
                    TimeSpan ts = new TimeSpan( -3, 0, 0, 0 );
                    DateTime threeDaysAgo = todaysDate + ts;

                    fromDate = threeDaysAgo;
                    toDate   = todaysDate;
                }
                    break;

                case WorklistSelectionRange.LAST_10_DAYS:
                {
                    TimeSpan ts = new TimeSpan( -10, 0, 0, 0 );
                    DateTime tenDaysAgo = todaysDate + ts;

                    fromDate = tenDaysAgo;
                    toDate   = todaysDate;
                }
                    break;

                case WorklistSelectionRange.DATE_RANGE:
                    // Date validation is done in FilterWorklistView which 
                    // controls date field's background color if in error.
                    break;
            }
        }

        private void ResetButtonClickEvent( object sender, EventArgs e )
        {
            this.cancelTimer();
            this.ResetControls();
            this.SetDefaultWorkllistSettings();

            this.labelNoPatientsFound.Text = string.Empty;
            this.labelNoPatientsFound.Visible = true;

            i_FilterWorklistView.Model_WorklistSetting.SortedColumnDirection = 1;
            i_FilterWorklistView.UpdateView();

            if( ResetActionsButtonClick != null )
            {
                ResetActionsButtonClick( this, null );
            }            
        }

        public void RefreshWorklistResults()
        {
            this.ShowWorklistRequestEvent(this, null);
        }

        protected virtual void ShowWorklistRequestEvent( object sender, EventArgs e )
        {
            if( this.WorklistsView != null )
            {
                this.WorklistsView.ListView.Items.Clear();
            }
            
            Cursor = Cursors.WaitCursor;            

            PopulateWorklist();
            Cursor = Cursors.Default;
        }

        protected virtual void WorklistViewsPrintReportEvent()
        {
            WorklistReports worklistReport = new WorklistReports();
            worklistReport.Sender = this;
            worklistReport.Model = Worklists;
            worklistReport.SearchCriteria = WorklistSearchCriteria;
            worklistReport.HeaderText = this.HeaderText;
            worklistReport.PrintPreview();
        }

        private void AdmitFromFilterSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            admitDateFrom = args.Context as String;

            if( admitDateFrom != String.Empty )
            {   // No need for exception handling here.  The data was validated in FilterWorklistView
                int month = Convert.ToInt32( admitDateFrom.Substring( 0, 2 ) );
                int day   = Convert.ToInt32( admitDateFrom.Substring( 3, 2 ) );
                int year  = Convert.ToInt32( admitDateFrom.Substring( 6, 4 ) );
                fromDate  = new DateTime( year, month, day );
            }
        }

        private void AdmitToFilterSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            admitDateTo = args.Context as String;

            if( admitDateTo != String.Empty )
            {   // No need for exception handling here.  The data was validated in FilterWorklistView
                int month = Convert.ToInt32( admitDateTo.Substring( 0, 2 ) );
                int day   = Convert.ToInt32( admitDateTo.Substring( 3, 2 ) );
                int year  = Convert.ToInt32( admitDateTo.Substring( 6, 4 ) );
                toDate  = new DateTime( year, month, day );
            }
        }

        private void FirstFilterLetterSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            nameFilterFirstLetter = args.Context as String;
        }

        private void LastFilterLetterSelectedEvent( object sender, EventArgs e )
        {
            LooseArgs args = (LooseArgs) e;
            nameFilterLastLetter = args.Context as String;
        }

        #endregion

        #region Methods
        public User GetUser()
        {
            return User.GetCurrent();
        }

        public virtual void PopulateWorklist()
        {   
            this.dgWorklistAccounts.DataSource = worklists;

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
                    new DoWorkEventHandler(DoPopulateWorklist);
                backgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(AfterWork);

                backgroundWorker.RunWorkerAsync();
            }
            
        }

        private void BeforeWork()
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();

            if( this.WorklistsView != null )
            {
                this.WorklistsView.lblItems.Text = string.Empty;
            }

            this.FilterWorklistView.SetPrintButtonState( false );

            // Cancel timer in case it's running

            if( refreshTimer == null )
            {
                timerDelegate = new TimerCallback( RefreshWorklist );
                refreshTimer = new Timer( timerDelegate, this, Timeout.Infinite, Timeout.Infinite );
            }

            refreshTimer.Change( Timeout.Infinite, Timeout.Infinite );

            this.Cursor = Cursors.AppStarting;
        }

        private void AfterWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                return;

            Cursor = Cursors.Default;

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
                    MessageBox.Show( UIErrorMessages.TIMEOUT_WORKLIST_DISPLAY );
                    this.labelNoPatientsFound.Visible = true;
                    this.ResetWorklistsView();
                }
                else
                {
                    throw e.Error;
                }
            }
            else
            {
                // success 
                // refactored the existing logic into a private method called FindWorkListSearchCriteria()
                // so that the AfterWork is kept as simple as possible
                FindWorkListSearchCriteria();
            }

            // post-completion operations...
            this.dgWorklistAccounts.DataSource = worklists;

            bindingManager = this.BindingContext[this.dgWorklistAccounts.DataSource]; 
            bindingManager.PositionChanged -= new EventHandler(RowChanged);    
            bindingManager.PositionChanged += new EventHandler(RowChanged);

            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
        }

        // This method encapsulates existing logic refactored from the success portion of the 
        // AfterWork method -  so that the AfterWork is kept as simple as possible
        private void FindWorkListSearchCriteria()
        {
            if ( worklistItems == null
                || worklistItems.Count <= 0)
            {
                this.labelNoPatientsFound.Text = i_noMatchMessage;
                this.labelNoPatientsFound.Visible = true;

                this.ResetWorklistsView();

                return;
            }

            this.labelNoPatientsFound.Visible = false;
            this.FilterWorklistView.SetPrintButtonState(true);

            if (this.WorklistsView != null)
            {
                this.WorklistsView.lblItems.Text = worklistItems.Count.ToString();
            }

            this.blnPrevSortWasAsc = !this.blnPrevSortWasAsc;

            switch (this.WorklistSearchCriteria.SortedColumn)
            {
                case 1:

                    this.SortWLDisplay(SORT_BY_NAME, this.WorklistSearchCriteria.SortedColumnDirection, new SortByNameAsc(), new SortByNameDesc());
                    break;
                case 2:
                    this.SortWLDisplay(SORT_BY_ACCOUNT, this.WorklistSearchCriteria.SortedColumnDirection, new SortByAccountAsc(), new SortByAccountDesc());
                    break;
                case 3:
                    this.SortWLDisplay(SORT_BY_ADMIT, this.WorklistSearchCriteria.SortedColumnDirection, new SortByAdmitDateAsc(), new SortByAdmitDateDesc());
                    break;
                case 4:
                    this.SortWLDisplay(SORT_BY_HSV, this.WorklistSearchCriteria.SortedColumnDirection, new SortByHSVAsc(), new SortByHSVDesc());
                    break;
                case 5:
                    this.SortWLDisplay(SORT_BY_CLINIC, this.WorklistSearchCriteria.SortedColumnDirection, new SortByClinicAsc(), new SortByClinicDesc());
                    break;
                case 6:
                    this.SortWLDisplay(SORT_BY_FC, this.WorklistSearchCriteria.SortedColumnDirection, new SortByFCAsc(), new SortByFCDesc());
                    break;
                case 7:
                    this.SortWLDisplay(SORT_BY_PAYOR, this.WorklistSearchCriteria.SortedColumnDirection, new SortByPayorAsc(), new SortByPayorDesc());
                    break;
                case 8:
                    this.SortWLDisplay(SORT_BY_DISCH_STATUS, this.WorklistSearchCriteria.SortedColumnDirection, new SortByDischargeStatusAsc(), new SortByDischargeStatusDesc());
                    break;
                case 9:
                    this.SortWLDisplay(SORT_BY_TO_DO_COUNT, this.WorklistSearchCriteria.SortedColumnDirection, new SortByToDoCountAsc(), new SortByToDoCountDesc());
                    break;
                default:
                    IComparer defaultSortByNameAsc = new SortByNameAsc();
                    IComparer defaultSortByNameDesc = new SortByNameDesc();

                    this.worklistItems.Sort(defaultSortByNameAsc);
                    this.prevSort = SORT_BY_NAME;
                    break;
            }

            this.blnPrevSortWasAsc = !this.blnPrevSortWasAsc;

            WorklistItem awi = worklistItems[0] as WorklistItem;

            GetSelectedAction(awi);

            // Start the timer to refresh the worklist automatically
            if (refreshTimer != null)
            {
                refreshTimer.Change(timerIntervalInMilliseconds, timerIntervalInMilliseconds);
            }
        }

        public virtual void DoPopulateWorklist(object sender, DoWorkEventArgs e)
        {
            i_worklistOid = worklist.Oid;

            if (Facility == null)
            {
                MessageBox.Show("FacilityBroker returned null Facility object", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
                return;
            }

            WorklistSettings ws = this.i_FilterWorklistView.Model_WorklistSetting;

            ws.WorkListID = i_worklistOid;

            worklists = AccountPBarBroker.AccountsWithWorklistsWith(Facility.Code, ws);
            this.Worklists = worklists;
            this.WorklistSearchCriteria = ws;

            this.worklistItems = worklists;

            // poll CancellationPending and set e.Cancel to true and return 
            if (this.backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return ;
            }
        }

        private void SortWLDisplay(string sortColumn , long  sortedColumnDirection ,IComparer sortByNameAsc,IComparer sortByNameDesc )                      
        {            
            if( sortedColumnDirection == 0 )
            {
                this.worklistItems.Sort( sortByNameDesc );
            }
            else
            {
                this.worklistItems.Sort( sortByNameAsc );
            }
            this.prevSort = sortColumn;
        }

        private void WorklistView_Load(object sender, EventArgs e)
        {                    
            this.progressPanel1.Visible     = false;
            labelNoPatientsFound.Visible    = false;
        }

        #endregion
        
        #region Properties

        private Facility Facility
        {
            get
            {
                return User.GetCurrent().Facility;
            }
        }

        public PatientAccessDataGrid DataGridWorklistAccounts
        {
            get
            {
                return this.dgWorklistAccounts;
            }
        }

        public BindingManagerBase DataGridBindingManager
        {
            get
            {
                return this.bindingManager;
            }
        }

        public bool AccountIsLocked
        {
            get
            {

                return i_accountIsLocked;
            }
            set
            {
                i_accountIsLocked = value;
            }
        }

        public WorklistsView WorklistsView
        {
            private get
            {
                return i_WorklistsView;
            }
            set
            {
                i_WorklistsView = value;
            }
        }

        public FilterWorklistView FilterWorklistView
        {
            get
            {
                if( i_FilterWorklistView == null )
                {
                    i_FilterWorklistView = new FilterWorklistView();
                }

                return this.i_FilterWorklistView;
            }
            set
            {
                this.i_FilterWorklistView = value;
            }
        }

        private IAccountBroker AccountPBarBroker
        {
            get
            {
                if( i_accountBroker == null )
                {
                    i_accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                }
                return i_accountBroker;
            }
        }

        protected IWorklistSettingsBroker WorklistBroker
        {
            get
            {
                if( i_broker == null )
                {
                    i_broker = BrokerFactory.BrokerOfType<IWorklistSettingsBroker>();
                }
                return i_broker;
            }
            set
            {
                i_broker = value;
            }
        }

        protected IHDIService CIEServiceBroker
        {
            get
            {
                if( i_cieBroker == null )
                {
                    i_cieBroker = BrokerFactory.BrokerOfType<IHDIService>();
                }
                return i_cieBroker;
            }
            set
            {
                i_cieBroker = value;
            }
        }

        protected string NoMatchMessage
        {
            get
            {
                return i_noMatchMessage;
            }
        }

        protected string NoActionsMessage
        {
            get
            {
                return i_noActionsMessage;
            }
        }

        protected string PBARErrorMessage
        {
            get
            {
                return i_pbarErrorMessage;
            }
        }

        public Worklist Worklist
        {
            set
            {
                this.worklist = value;
                this.WorklistType = Convert.ToInt32(worklist.Oid);
            }
        }

        public int WorklistType
        {
            get
            {
                return this.FilterWorklistView.WorklistType;
            }
            set
            {
                this.FilterWorklistView.WorklistType = value;
            }
        }

        public string HeaderText
        {
            private get
            {
                return i_HeaderText;
            }
            set
            {
                i_HeaderText = value;
            }
        }

        #endregion

        #region Private Methods
        
        protected virtual void RefreshWorklist( Object state )
        {
            WorklistView wv = (WorklistView) state;
            if( wv != null && wv.Visible )
            {
                wv.Invoke(new PopulateWorklistDelegate(PopulateWorklist));
            }
        }

        /// <summary>
        /// cancelTimer gets invoked by parent form--WorklistsView--when Open Account button is clicked
        /// or and action item is double-clicked.
        /// </summary>
        private void cancelTimer()
        {
            if( refreshTimer != null )
            {
                refreshTimer.Change( Timeout.Infinite, Timeout.Infinite );
                refreshTimer.Dispose();  
                refreshTimer = null;
            }
        }

        private void ResetWorklistsView()
        {
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            if( this.WorklistsView != null )
            {
                this.WorklistsView.ResetControls();
            }
        }
        protected virtual void ResetControls()
        {
            selectedAccountNumber = -1;
        }

        protected virtual void SetDefaultWorkllistSettings()
        {
            ITimeBroker tb = ProxyFactory.GetTimeBroker();
        }

        private void dgWorklistAccounts_RowChanged()
        {            
            // this is the 'selected' event for a row
            
            int rowNum = this.bindingManager.Position;
            
            if( rowNum < 0 )
            {
                return;
            }                

            ArrayList items = (ArrayList) this.dgWorklistAccounts.DataSource;
            WorklistItem wi     = items[rowNum] as WorklistItem;

            this.GetSelectedAction( wi );                    
  
            this.dgWorklistAccounts.CurrentCell = new DataGridCell( this.dgWorklistAccounts.CurrentCell.RowNumber, 0 ); 
            this.dgWorklistAccounts.CurrentRowIndex = rowNum;             
            this.dgWorklistAccounts.Select( rowNum ); 
        }    

        private void RowChanged(object sender, EventArgs e) 
        { 
            this.dgWorklistAccounts_RowChanged();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ResourceManager resources = new ResourceManager(typeof(WorklistView));
            this.columnHdrBmp = new ColumnHeader();
            this.columnHdrName = new ColumnHeader();
            this.columnHdrAccount = new ColumnHeader();
            this.columnHdrAdmitDate = new ColumnHeader();
            this.columnHdrService = new ColumnHeader();
            this.columnHdrClinic = new ColumnHeader();
            this.columnHdrFC = new ColumnHeader();
            this.columnHdrPayor = new ColumnHeader();
            this.columnHdrDiscStatus = new ColumnHeader();
            this.columnToDo = new ColumnHeader();
            this.dataGridTableStyle1 = new DataGridTableStyle();
            this.dgWorklistAccounts = new PatientAccessDataGrid();
            this.dataGridColumn0 = new DataGridImageColumn();
            this.dataGridColumn1 = new DataGridTextBoxColumn();
            this.dataGridColumn2 = new DataGridTextBoxColumn();
            this.dataGridColumn3 = new DataGridTextBoxColumn();
            this.dataGridColumn4 = new DataGridTextBoxColumn();
            this.dataGridColumn5 = new DataGridTextBoxColumn();
            this.dataGridColumn6 = new DataGridTextBoxColumn();
            this.dataGridColumn7 = new DataGridTextBoxColumn();
            this.dataGridColumn8 = new DataGridTextBoxColumn();
            this.dataGridColumn9 = new DataGridTextBoxColumn();
            this.dataGridColumn10 = new DataGridTextBoxColumn();
            this.i_FilterWorklistView = new FilterWorklistView();
            this.imlLock = new ImageList(this.components);
            this.imlArrows = new ImageList(this.components);
            this.labelNoPatientsFound = new Label();
            this.panelGrid = new Panel();
            this.progressPanel1 = new ProgressPanel();
            ((ISupportInitialize)(this.dgWorklistAccounts)).BeginInit();
            this.panelGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.DataGrid = this.dgWorklistAccounts;
            this.dataGridTableStyle1.GridColumnStyles.AddRange(new DataGridColumnStyle[] {
                                                                                                                  this.dataGridColumn0,
                                                                                                                  this.dataGridColumn1,
                                                                                                                  this.dataGridColumn2,
                                                                                                                  this.dataGridColumn3,
                                                                                                                  this.dataGridColumn4,
                                                                                                                  this.dataGridColumn5,
                                                                                                                  this.dataGridColumn6,
                                                                                                                  this.dataGridColumn7,
                                                                                                                  this.dataGridColumn8,
                                                                                                                  this.dataGridColumn9,
                                                                                                                  this.dataGridColumn10});
            this.dataGridTableStyle1.HeaderForeColor = SystemColors.ControlText;
            this.dataGridTableStyle1.MappingName = "ArrayList";
            this.dataGridTableStyle1.ReadOnly = true;
            // 
            // dgWorklistAccounts
            // 
            this.dgWorklistAccounts.BackgroundColor = SystemColors.Window;
            this.dgWorklistAccounts.CaptionVisible = false;
            this.dgWorklistAccounts.DataMember = "";
            this.dgWorklistAccounts.Dock = DockStyle.Fill;
            this.dgWorklistAccounts.HeaderForeColor = SystemColors.ControlText;
            this.dgWorklistAccounts.Location = new Point(2, 2);
            this.dgWorklistAccounts.Name = "dgWorklistAccounts";
            this.dgWorklistAccounts.ReadOnly = true;
            this.dgWorklistAccounts.RowHeadersVisible = false;
            this.dgWorklistAccounts.RowHeaderWidth = 0;
            this.dgWorklistAccounts.Size = new Size(992, 192);
            this.dgWorklistAccounts.TabIndex = 0;
            this.dgWorklistAccounts.TableStyles.AddRange(new DataGridTableStyle[] {
                                                                                                           this.dataGridTableStyle1});
            this.dgWorklistAccounts.MouseDown += new MouseEventHandler(this.dgWorklistAccounts_MouseDown);
            this.dgWorklistAccounts.KeyUp += new KeyEventHandler(this.dgWorklistAccounts_KeyUp);
            // 
            // dataGridColumn0
            // 
            this.dataGridColumn0.MappingName = "IsLocked";
            this.dataGridColumn0.ReadOnly = true;
            this.dataGridColumn0.Width = 22;
            // 
            // dataGridColumn1
            // 
            this.dataGridColumn1.Format = "";
            this.dataGridColumn1.FormatInfo = null;
            this.dataGridColumn1.HeaderText = "Patient Name";
            this.dataGridColumn1.MappingName = "Name";
            this.dataGridColumn1.ReadOnly = true;
            this.dataGridColumn1.Width = 188;
            // 
            // dataGridColumn2
            // 
            this.dataGridColumn2.Format = "############";
            this.dataGridColumn2.FormatInfo = null;
            this.dataGridColumn2.HeaderText = "Account";
            this.dataGridColumn2.MappingName = "AccountNumber";
            this.dataGridColumn2.ReadOnly = true;
            this.dataGridColumn2.Width = 63;
            // 
            // dataGridColumn3
            // 
            this.dataGridColumn3.Format = "";
            this.dataGridColumn3.FormatInfo = null;
            this.dataGridColumn3.HeaderText = "Admit Date/Time";
            this.dataGridColumn3.MappingName = "AdmitDate";
            this.dataGridColumn3.ReadOnly = true;
            this.dataGridColumn3.Width = 95;
            // 
            // dataGridColumn4
            // 
            this.dataGridColumn4.Format = "";
            this.dataGridColumn4.FormatInfo = null;
            this.dataGridColumn4.HeaderText = "Hospital Service";
            this.dataGridColumn4.MappingName = "HospitalService";
            this.dataGridColumn4.ReadOnly = true;
            this.dataGridColumn4.Width = 118;
            // 
            // dataGridColumn5
            // 
            this.dataGridColumn5.Format = "";
            this.dataGridColumn5.FormatInfo = null;
            this.dataGridColumn5.HeaderText = "Clinic";
            this.dataGridColumn5.MappingName = "Clinic";
            this.dataGridColumn5.ReadOnly = true;
            this.dataGridColumn5.Width = 118;
            // 
            // dataGridColumn6
            // 
            this.dataGridColumn6.Format = "";
            this.dataGridColumn6.FormatInfo = null;
            this.dataGridColumn6.HeaderText = "FC";
            this.dataGridColumn6.MappingName = "FinancialClass";
            this.dataGridColumn6.ReadOnly = true;
            this.dataGridColumn6.Width = 77;
            // 
            // dataGridColumn7
            // 
            this.dataGridColumn7.Format = "";
            this.dataGridColumn7.FormatInfo = null;
            this.dataGridColumn7.HeaderText = "Primary Payor";
            this.dataGridColumn7.MappingName = "PrimaryPayor";
            this.dataGridColumn7.ReadOnly = true;
            this.dataGridColumn7.Width = 140;
            // 
            // dataGridColumn8
            // 
            this.dataGridColumn8.Format = "";
            this.dataGridColumn8.FormatInfo = null;
            this.dataGridColumn8.HeaderText = "Discharge Status";
            this.dataGridColumn8.MappingName = "DischargeStatus";
            this.dataGridColumn8.ReadOnly = true;
            this.dataGridColumn8.Width = 90;
            // 
            // dataGridColumn9
            // 
            this.dataGridColumn9.Format = "#####";
            this.dataGridColumn9.FormatInfo = null;
            this.dataGridColumn9.HeaderText = "To Do";
            this.dataGridColumn9.MappingName = "ToDoCount";
            this.dataGridColumn9.ReadOnly = true;
            this.dataGridColumn9.Width = 45;
            // 
            // dataGridColumn10
            // 
            this.dataGridColumn10.Format = "";
            this.dataGridColumn10.FormatInfo = null;
            this.dataGridColumn10.MappingName = "Filler";
            this.dataGridColumn10.ReadOnly = true;
            this.dataGridColumn10.Width = 0;
            // 
            // i_FilterWorklistView
            // 
            this.i_FilterWorklistView.BackColor = Color.White;
            this.i_FilterWorklistView.Items = 0;
            this.i_FilterWorklistView.Location = new Point(0, 0);
            this.i_FilterWorklistView.Model = null;
            this.i_FilterWorklistView.Model_WorklistSetting = null;
            this.i_FilterWorklistView.Name = "i_FilterWorklistView";
            this.i_FilterWorklistView.Size = new Size(1010, 75);
            this.i_FilterWorklistView.TabIndex = 0;
            this.i_FilterWorklistView.WorklistType = 0;
            this.i_FilterWorklistView.AdmitFromFilterSelected += new EventHandler(this.AdmitFromFilterSelectedEvent);
            this.i_FilterWorklistView.LastFilterLetterSelected += new EventHandler(this.LastFilterLetterSelectedEvent);
            this.i_FilterWorklistView.AdmitToFilterSelected += new EventHandler(this.AdmitToFilterSelectedEvent);
            this.i_FilterWorklistView.ResetButtonClick += new EventHandler(this.ResetButtonClickEvent);
            this.i_FilterWorklistView.ShowWorklistRequest += new EventHandler(this.ShowWorklistRequestEvent);
            this.i_FilterWorklistView.PeriodFilterSelected += new EventHandler(this.PeriodFilterSelectedEvent);
            this.i_FilterWorklistView.FirstFilterLetterSelected += new EventHandler(this.FirstFilterLetterSelectedEvent);
            this.i_FilterWorklistView.PrintReport += new FilterWorklistView.Print(this.WorklistViewsPrintReportEvent);
            // 
            // imlLock
            // 
            this.imlLock.ImageSize = new Size(16, 16);
            this.imlLock.ImageStream = ((ImageListStreamer)(resources.GetObject("imlLock.ImageStream")));
            this.imlLock.TransparentColor = Color.Transparent;
            // 
            // imlArrows
            // 
            this.imlArrows.ImageSize = new Size(10, 10);
            this.imlArrows.ImageStream = ((ImageListStreamer)(resources.GetObject("imlArrows.ImageStream")));
            this.imlArrows.TransparentColor = Color.Transparent;
            // 
            // labelNoPatientsFound
            // 
            this.labelNoPatientsFound.BorderStyle = BorderStyle.FixedSingle;
            this.labelNoPatientsFound.Dock = DockStyle.Fill;
            this.labelNoPatientsFound.Location = new Point(2, 2);
            this.labelNoPatientsFound.Name = "labelNoPatientsFound";
            this.labelNoPatientsFound.Size = new Size(992, 192);
            this.labelNoPatientsFound.TabIndex = 1;
            // 
            // panelGrid
            // 
            this.panelGrid.Controls.Add(this.labelNoPatientsFound);
            this.panelGrid.Controls.Add(this.dgWorklistAccounts);
            this.panelGrid.DockPadding.All = 2;
            this.panelGrid.Location = new Point(0, 80);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Size = new Size(996, 196);
            this.panelGrid.TabIndex = 2;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = Color.White;
            this.progressPanel1.Location = new Point(0, 8);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new Size(1000, 280);
            this.progressPanel1.TabIndex = 3;
            // 
            // WorklistView
            // 
            this.Disposed +=new EventHandler(WorklistView_Disposed);
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.i_FilterWorklistView);
            this.Controls.Add(this.panelGrid);
            this.Name = "WorklistView";
            this.Size = new Size(1000, 300);
            this.Load += new EventHandler(this.WorklistView_Load);
            this.Leave += new EventHandler(this.WorklistView_Leave);
            ((ISupportInitialize)(this.dgWorklistAccounts)).EndInit();
            this.panelGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        private void GetSelectedAction( WorklistItem wi )
        {
            IAccountBroker broker = BrokerFactory.BrokerOfType<IAccountBroker>();

            long accountNumber  = wi.AccountNumber;
            long medRecNo       = wi.MedicalRecordNumber;

            AccountProxy aProxy = broker.AccountProxyFor( User.GetCurrent().Facility.Code, medRecNo, accountNumber);

            if( aProxy != null )
            {
                aProxy.Patient.MedicalRecordNumber = wi.MedicalRecordNumber;

                AccountIsLocked = aProxy.IsLocked;
                
                selectedAccountNumber = aProxy.AccountNumber;
                
                AccountNameEvent( this, new LooseArgs( wi.Name ) );
                WorklistEvent( this, new LooseArgs( worklist ) );
                WorklistSelectedIndexEvent( this, new LooseArgs( aProxy ) );
            }            
        }

        private void dgWorklistAccounts_MouseDown(object sender, MouseEventArgs e)
        {
            DataGrid.HitTestInfo hti = null;

            try
            {
                hti = this.dgWorklistAccounts.HitTest(e.X, e.Y);
            }
            catch
            {
            }            

            if ( ( hti.Type == DataGrid.HitTestType.ColumnHeader )
                && ( (e.Button & MouseButtons.Left) == MouseButtons.Left ) )
            {                       
                if( this.worklistItems != null )
                {
                    if( this.worklistItems.Count == 0 )
                    {
                        return;
                    }
                    else if( this.worklistItems.Count == 1 )                    
                    {
                        WorklistItem w = this.worklistItems[0] as WorklistItem;
                    
                        if( w != null
                            && w.Name == this.i_noMatchMessage )
                        {
                            return;
                        }
                    }  
                }
                else
                {
                    return;
                }

                this.sortColHeader = this.dgWorklistAccounts.GetCellBounds(0, hti.Column);
                this.sortColHeader.X += 1;
                this.sortColHeader.Y -= 18;
                
                bindingManager = this.BindingContext[this.dgWorklistAccounts.DataSource]; 
 
                switch( hti.Column )
                {
                    case 1:  
                        this.DisplaySortedWLView(  SORT_BY_NAME , new SortByNameAsc(),  new SortByNameDesc() ) ;
                        break;
                    case 2:           
                        this.DisplaySortedWLView(  SORT_BY_ACCOUNT , new SortByAccountAsc(),  new SortByAccountDesc() ) ;     
                        break;
                    case 3:                 
                        this.DisplaySortedWLView(  SORT_BY_ADMIT , new SortByAdmitDateAsc(),  new SortByAdmitDateDesc() ) ;     
                        break;
                    case 4:                    
                        this.DisplaySortedWLView(  SORT_BY_HSV , new SortByHSVAsc(),  new SortByHSVDesc() ) ;            
                        break;
                    case 5:
                        this.DisplaySortedWLView(  SORT_BY_CLINIC , new SortByClinicAsc() ,   new SortByClinicDesc() ) ;            
                        break;
                    case 6:                   
                        this.DisplaySortedWLView(  SORT_BY_FC , new SortByFCAsc() ,  new SortByFCDesc() ) ;            
                        break;
                       
                    case 7:
                        this.DisplaySortedWLView(  SORT_BY_PAYOR ,  new SortByPayorAsc(),  new SortByPayorDesc() ) ;            
                        break;
                    case 8:
                        this.DisplaySortedWLView(  SORT_BY_DISCH_STATUS , new SortByDischargeStatusAsc(),  new SortByDischargeStatusDesc() ) ;                                
                        break;

                    case 9:
                        this.DisplaySortedWLView(  SORT_BY_TO_DO_COUNT , new SortByToDoCountAsc(),  new SortByToDoCountDesc() ) ;                                
                        break;

                    default:
                        return;
                }                     
              
               
                FilterWorklistView.Model_WorklistSetting.SortedColumn = hti.Column ;
                if(blnPrevSortWasAsc)
                {
                    FilterWorklistView.Model_WorklistSetting.SortedColumnDirection  = 0 ;
                }
                else
                {
                    FilterWorklistView.Model_WorklistSetting.SortedColumnDirection = 1;
                }
                this.WorklistBroker.SaveWorklistSettings( GetUser().SecurityUser.TenetID, FilterWorklistView.Model_WorklistSetting , this.worklist );

                WorklistItem wi     = this.worklistItems[0] as WorklistItem;
                this.GetSelectedAction( wi );

                if (  hti.Type == DataGrid.HitTestType.ColumnHeader
                    && ( e.Button & MouseButtons.Left) == MouseButtons.Left )
                {
                    this.dgWorklistAccounts.UnSelect( this.dgWorklistAccounts.CurrentCell.RowNumber );
                    this.dgWorklistAccounts.CurrentRowIndex = 0;
                }
            }     
        }

        private class SortByNameAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.Name, wi2.Name ) );
            }
        }

        private class SortByNameDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.Name, wi1.Name ) );
            }
        }

        public class SortByAccountAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.AccountNumber, wi2.AccountNumber ) );
            }
        }

        private class SortByAccountDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.AccountNumber, wi1.AccountNumber ) );
            }
        }

        private class SortByAdmitDateAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.AdmitDate, wi2.AdmitDate ) );
            }
        }

        private class SortByAdmitDateDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.AdmitDate, wi1.AdmitDate ) );
            }
        }

        private class SortByHSVAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.HospitalService, wi2.HospitalService ) );
            }
        }

        private class SortByHSVDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.HospitalService, wi1.HospitalService ) );
            }
        }

        private class SortByClinicAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.Clinic, wi2.Clinic ) );
            }
        }

        private class SortByClinicDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.Clinic, wi1.Clinic ) );
            }
        }

        private class SortByFCAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.FinancialClass, wi2.FinancialClass ) );
            }
        }

        private class SortByFCDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.FinancialClass, wi1.FinancialClass ) );
            }
        }

        private class SortByPayorAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.PrimaryPayor, wi2.PrimaryPayor ) );
            }
        }

        private class SortByPayorDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.PrimaryPayor, wi1.PrimaryPayor ) );
            }
        }

        private class SortByDischargeStatusAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.DischargeStatus, wi2.DischargeStatus ) );
            }
        }

        private class SortByDischargeStatusDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.DischargeStatus, wi1.DischargeStatus ) );
            }
        }

        private class SortByToDoCountAsc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi1.ToDoCount, wi2.ToDoCount ) );
            }
        }

        private class SortByToDoCountDesc : IComparer  
        {
            // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
            int IComparer.Compare( Object x, Object y )  
            {
                WorklistItem wi1 = x as WorklistItem;
                WorklistItem wi2 = y as WorklistItem;

                return( (new CaseInsensitiveComparer()).Compare( wi2.ToDoCount, wi1.ToDoCount ) );
            }
        }

        #region Data Grid Utilities

        /// <summary>
        /// Paint the Text and the Image passed
        /// </summary>
        /// <param name="g">Graphics device where you can render your image and text</param>
        /// <param name="p_displayRectangle">Relative rectangle based on the display area</param>
        /// <param name="p_Asc"></param>
        /// <param name="p_ImageAlignment">Alignment of the image</param>
        /// <param name="p_ImageStretch">True to make the draw the image with the same size of the cell</param>
        /// <param name="p_Text">Text to draw (can be null)</param>
        /// <param name="p_StringFormat">String format (can be null)</param>
        /// <param name="p_AlignTextToImage">True to align the text with the image</param>
        /// <param name="p_TextColor">Text Color</param>
        /// <param name="p_TextFont">Text Font(can be null)</param>
        private void PaintImageAndText(Graphics g, 
            Rectangle p_displayRectangle,
            bool p_Asc,
            ContentAlignment p_ImageAlignment,
            bool p_ImageStretch,
            string p_Text,
            StringFormat p_StringFormat,
            bool p_AlignTextToImage,            
            Color p_TextColor,
            Font p_TextFont)
        {
            // this is not working correctly, so bypass

            return; 
 
            #region Image
            #endregion

            #region Text
            #endregion
        }
        
        private void DisplaySortedWLView( string sortColumn , IComparer sortColumnComparerAsc, IComparer sortColumnComparerDsc )
        {
            if( this.blnPrevSortWasAsc && this.prevSort == sortColumn )
            {
                this.blnPrevSortWasAsc = false;
                this.worklistItems.Sort( sortColumnComparerAsc );
            }
            else
            {
                this.blnPrevSortWasAsc = true;
                this.worklistItems.Sort( sortColumnComparerDsc );
            }

            this.PaintImageAndText( this.dgWorklistAccounts.CreateGraphics(), 
                this.sortColHeader,
                blnPrevSortWasAsc,
                ContentAlignment.MiddleRight, false, sortColumn,
                StringFormat.GenericDefault, true, Color.FromKnownColor( KnownColor.Black ),
                new Font("Microsoft Sans Serif",8));
                          
            this.prevSort = sortColumn;
        }

        #endregion

        #endregion

        #region Private Properties
        protected ArrayList Worklists
        {
            get
            {
                return i_Worklists;
            }
            private set
            {
                i_Worklists = value;
            }
        }

        protected WorklistSettings WorklistSearchCriteria
        {
            get
            {
                return i_WorklistSearchCriteria;
            }
            private set
            {
                i_WorklistSearchCriteria = value;
            }
        }
        #endregion

        #region Construction and Finalization

        public WorklistView()
        {
            InitializeComponent();

            timerDelegate = new TimerCallback( RefreshWorklist );
            refreshTimer  = new Timer( timerDelegate, this, Timeout.Infinite, Timeout.Infinite );
        }

        #endregion

        #region Data Elements
        
        private delegate void                               PopulateWorklistDelegate();

        private string                                      i_HeaderText = string.Empty;
        private long                                        i_worklistOid;
        private string                                      nameFilterLastLetter;
        private string                                      nameFilterFirstLetter;
        private string                                      admitDateFrom;
        private string                                      admitDateTo;
        private DateTime                                    fromDate;
        private DateTime                                    toDate;
        private DateTime                                    todaysDate;        
        private long                                        selectedAccountNumber;

        private Timer                      refreshTimer;
        private TimerCallback              timerDelegate;
        private int                                         timerIntervalInMilliseconds = 600000; // 10 minutes
        //private int                                         timerIntervalInMilliseconds = 30000; // 30 seconds for testing

        private ProgressPanel                               progressPanel1;
        private BackgroundWorker      backgroundWorker; 
        private Worklist                                    worklist;
        private WorklistSelectionRange                      selectionRange;
        private WorklistsView                               i_WorklistsView;
        private BindingManagerBase                          bindingManager; 
        private WorklistSettings                            i_WorklistSearchCriteria;
        private ArrayList                                   i_Worklists;
        private IAccountBroker                          i_accountBroker;
        private IWorklistSettingsBroker                             i_broker;
        private IHDIService                                 i_cieBroker;
        private FilterWorklistView                          i_FilterWorklistView;

        private PatientAccessDataGrid                       dgWorklistAccounts;

        private Panel                  panelGrid;

        private Label                  labelNoPatientsFound;

        private IContainer            components;
        
        private ImageList              imlLock;
        private ImageList              imlArrows;

        private ColumnHeader           columnHdrBmp;
        private ColumnHeader           columnHdrAccount;
        private ColumnHeader           columnHdrAdmitDate;
        private ColumnHeader           columnHdrDiscStatus;
        private ColumnHeader           columnHdrClinic;
        private ColumnHeader           columnHdrFC;
        private ColumnHeader           columnHdrName;
        private ColumnHeader           columnHdrPayor;
        private ColumnHeader           columnHdrService;
        private ColumnHeader           columnToDo;

        private DataGridTableStyle     dataGridTableStyle1;

        private DataGridImageColumn          dataGridColumn0;
        private DataGridTextBoxColumn  dataGridColumn1;
        private DataGridTextBoxColumn  dataGridColumn2;
        private DataGridTextBoxColumn  dataGridColumn3;
        private DataGridTextBoxColumn  dataGridColumn4;
        private DataGridTextBoxColumn  dataGridColumn5;
        private DataGridTextBoxColumn  dataGridColumn6;
        private DataGridTextBoxColumn  dataGridColumn7;
        private DataGridTextBoxColumn  dataGridColumn8;
        private DataGridTextBoxColumn  dataGridColumn9;
        private DataGridTextBoxColumn  dataGridColumn10;

        private bool                                        blnPrevSortWasAsc = true;
        private string                                      prevSort = string.Empty;
        private Rectangle                                   sortColHeader;
        //private Rectangle                                   prevSortColHeader;
        private ArrayList                                   worklistItems;
        private ArrayList                                   worklists = new ArrayList();
        private bool                                        i_accountIsLocked;

        #endregion

        #region Constants

        private string i_pbarErrorMessage = "This activity cannot proceed because the system is unavailable.  Please try again later.";
        private string i_noMatchMessage = "No patients were found based on the applied filter settings.";
        private string i_noActionsMessage = "No action items to display.";

        private const string                                
            SORT_BY_NAME            = "Patient Name",
            SORT_BY_ACCOUNT         = "Account Number",
            SORT_BY_ADMIT           = "Admit Date",
            SORT_BY_HSV             = "Hospital Service",
            SORT_BY_CLINIC          = "Clinic",
            SORT_BY_FC              = "FC",
            SORT_BY_PAYOR           = "Primary Payor",
            SORT_BY_DISCH_STATUS    = "Discharge Status",
            SORT_BY_TO_DO_COUNT     = "To Do Count";

        #endregion

    }
}
