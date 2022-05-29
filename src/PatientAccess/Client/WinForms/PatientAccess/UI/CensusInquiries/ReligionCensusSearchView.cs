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
    /// Census by Religion Search View
    /// </summary>
    [Serializable]
    public class ReligionCensusSearchView : ControlView
    {
        #region Events

        public event EventHandler AccountsFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler NoSummaryAccountsFound;
        public event EventHandler AccountsSummaryFound;
        public event EventHandler ResetView;
        public event EventHandler SortChange;       
        public event EventHandler AcceptButtonChanged;
        public event EventHandler PrintReport;
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;
       
        #endregion
        
        #region Event Handlers

        private void showButtonClick(object sender, EventArgs e)
        {
            this.ResetView(this, null);

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
                    new DoWorkEventHandler(doShowButtonClick);
                backgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(AfterWork);

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
         
            selectedReligion = "";

            selectedReligion = this.cmbReligion.SelectedValue.ToString();

            SetSortFlag();
            SetSortByColumn(); 

        }

        private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
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
                // success!
                // call to a private method created as a result of refactoring the original code
                // that was here. This helps with clarity in regards to what the logic of the previous code 
                // did and the resulting AfterWork method.
                //
                RaiseAccountsAndSummaryFoundEvents();
            }

            // place post completion operations here...
            if( this.AfterWorkEvent != null )
            {
                this.AfterWorkEvent( this, null );
            }

            this.Cursor = Cursors.Default;
        }
        
        // Sanjeev Kumar: new method created from a refactoring of code in the AfterWork
        //
        private void RaiseAccountsAndSummaryFoundEvents()
        {
            // Raise AccountsFound Event
            if (AccountsFound != null && accountProxiesCollection.Count > 0)
            {
                AccountsFound(this,
                    new LooseArgs(accountProxiesCollection));
                this.btnReportPrint.Enabled = true;
            }
            else
            {
                if (NoAccountsFound != null)
                {
                    NoAccountsFound(this, null);
                    this.btnReportPrint.Enabled = false;
                }
            }

            // Raise AccountsSummaryFound Event
            if (AccountsSummaryFound != null && collectionOfReligions.Count > 0)
            {
                AccountsSummaryFound(this, new LooseArgs(collectionOfReligions));
            }
            else
            {
                if (NoSummaryAccountsFound != null)
                {
                    NoSummaryAccountsFound(this, null);
                }
            }
        }

        /// <summary>
        /// This will fire based on religion code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doShowButtonClick(object sender, DoWorkEventArgs e)
        {

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: poll cancellationPending before doing time consuming work. 
            // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();

            accountProxiesCollection = (ICollection)
                accountBroker.AccountsFor(User.GetCurrent().Facility.Code,
                selectedReligion
                );


            IReligionBroker religionBroker = BrokerFactory.BrokerOfType<IReligionBroker>();

            collectionOfReligions = (ICollection)
                religionBroker.ReligionSummaryFor(User.GetCurrent().Facility,
                selectedReligion
                );

            // Poll the cancellationPending property, if true set e.Cancel to true and return.
            // Rationale: poll cancellationPending before doing time consuming work. 
            // this is not the best way to be polling, but the whole thing needs a bit of a rethink!
            if ( backgroundWorker.CancellationPending )
            {
                e.Cancel = true;
                return;
            }
        }

        private void ReligionCensusSearchView_Load( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// This will clear the search result, statistical summary 
        /// and will set the filter as default "ALL".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetButtonClick( object sender, EventArgs e )
        {
            this.cmbReligion.SelectedIndex = 0;        
            this.btnReportPrint.Enabled = false;
            this.ResetView(this, null );
        }
        
        private void cmbReligion_Enter(object sender, EventArgs e)
        {
            if( AcceptButtonChanged != null )
            {
                AcceptButtonChanged( this, new LooseArgs( this ) );
            }
        }

        private void cmbReligion_Leave(object sender, EventArgs e)
        {
            if( this.ParentForm != null )
            {
                this.AcceptButton = showButton;
            }
        }
        
        private void cmbReligion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( cmbReligion.SelectedItem.ToString().Trim().Length == 0 )
            {
                showButton.Enabled = false;
            }
            else
            {
                showButton.Enabled = true;
            }
        }

        private void rdoReligion_CheckedChanged(object sender, EventArgs e)
        {
            sortByColumn();
        }
       
        private void rdoReligion_Enter(object sender, EventArgs e)
        {
            if( AcceptButtonChanged != null )
            {
                AcceptButtonChanged( this, new LooseArgs( this ) );
            }
        }

        private void rdoReligion_Leave(object sender, EventArgs e)
        {
            if( this.ParentForm != null )
            {
                this.AcceptButton = showButton;
            }
        }


        private void rdoNursing_Enter(object sender, EventArgs e)
        {
            if( AcceptButtonChanged != null )
            {
                AcceptButtonChanged( this, new LooseArgs( this ) );
            }
        }


        private void rdoNursing_Leave(object sender, EventArgs e)
        {
            if( this.ParentForm != null )
            {
                this.AcceptButton = showButton;
            }
        }

        private void btnReportPrint_Click( object sender, EventArgs e )
        {
            SetSearchCriteria();
            if( PrintReport != null )
            {
                PrintReport( this, new ReportArgs( 
                    accountProxiesCollection, SearchCriteria, collectionOfReligions  ) );
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
            if( this.Enabled && this.cmbReligion.Items.Count == 0)
            {
                PopulateReligions();
            }
            this.showButton.Enabled = true;
        }


        #endregion

        #region Properties

        public string ReligionType
        {
            get
            {
                return i_ReligionType;
            }
            set
            {
                i_ReligionType = value;
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

        /// <summary>
        /// Method will populate all religions
        /// </summary>
        private void PopulateReligions()
        {
            if( base.IsInRuntimeMode )
            {
                if( cmbReligion.Items.Count == 0 )
                {
                    IReligionBroker broker = BrokerFactory.BrokerOfType<IReligionBroker>();
                    ArrayList religions = ( ArrayList )( 
                                ICollection )broker.AllReligions(User.GetCurrent().Facility.Oid);

                    religions.Insert(0,
                        new Religion( ReferenceValue.NEW_OID,
                        ReferenceValue.NEW_VERSION,
                        "ALL",
                        "ALL" )                                    
                        );   
    
                    religions.Insert(1,
                        new Religion( ReferenceValue.NEW_OID,
                        ReferenceValue.NEW_VERSION,
                        UNSPECIFIED_RELIGION,
                        UNSPECIFIED_RELIGION )                                    
                        ); 

					foreach( Religion religion in religions )
					{
						if( religion.Code.Trim().Length == 0  &&
							religion.Description.Trim().Length == 0 )
						{
							religions.Remove( religion );
							break;
						}
					}

                    cmbReligion.DataSource = religions;
                    cmbReligion.ValueMember   = "Code";
                    cmbReligion.DisplayMember = "Description";
                }              
            }            
        }

        private void SetSortByColumn()
        {
            if( rdoReligion.Checked )
            {
                i_SortByColumn = SORT_BY_RELIGION;
            }
            else
            {
                i_SortByColumn = SORT_BY_NURSING;
            }
        }


        private void sortByColumn()
        {
            SetSortByColumn();
           
            if( ( accountProxiesCollection != null ) && 
                ( accountProxiesCollection.Count > 0 ) && 
                ( SortChange != null ) )
            {
                SortChange( this, new LooseArgs( this ) );
            }
        }
    
        //To specify search critera for the Religion Report
        private void SetSearchCriteria()
        {
            
            selectedReligionDesc = 
                CommonFormatting.ProperCase( this.cmbReligion.SelectedItem.ToString() ); 
            

            if( rdoReligion.Checked.Equals( true ) )
            {
               selectedSortBy =  "Religion";
            }
            else
            {
                selectedSortBy =  "Nursing station";
            }

            SearchCriteria.Clear();
            SearchCriteria.Add( selectedReligionDesc );
            SearchCriteria.Add( selectedSortBy );
            
        }

       
        #endregion

        #region Private Properties

        private void SetSortFlag()
        {
            if( rdoReligion.Checked )
            {
                i_ReligionType = SORT_BY_RELIGION;
            }           
            else
            {
                i_ReligionType = SORT_BY_NURSING;
            }
        }

        #endregion

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
			this.cmbReligion = new System.Windows.Forms.ComboBox();
			this.showButton = new LoggingButton();
			this.resetButton = new LoggingButton();
			this.lblShowByReligion = new System.Windows.Forms.Label();
			this.lblSortBy = new System.Windows.Forms.Label();
			this.rdoReligion = new System.Windows.Forms.RadioButton();
			this.rdoNursing = new System.Windows.Forms.RadioButton();
			this.btnReportPrint = new LoggingButton();
			this.ReligionCensusSearchPanel = new System.Windows.Forms.Panel();
			this.pnlRadioBtns = new System.Windows.Forms.Panel();
			this.ReligionCensusSearchPanel.SuspendLayout();
			this.pnlRadioBtns.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbReligion
			// 
			this.cmbReligion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbReligion.Location = new System.Drawing.Point(152, 17);
			this.cmbReligion.Name = "cmbReligion";
			this.cmbReligion.Size = new System.Drawing.Size(152, 21);
			this.cmbReligion.TabIndex = 1;
			this.cmbReligion.SelectedIndexChanged += new System.EventHandler(this.cmbReligion_SelectedIndexChanged);
			this.cmbReligion.Leave += new System.EventHandler(this.cmbReligion_Leave);
			this.cmbReligion.Enter += new System.EventHandler(this.cmbReligion_Enter);
			// 
			// showButton
			// 
			this.showButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.showButton.BackColor = System.Drawing.SystemColors.Control;
			this.showButton.Location = new System.Drawing.Point(330, 15);
			this.showButton.Name = "showButton";
			this.showButton.Size = new System.Drawing.Size(75, 24);
			this.showButton.TabIndex = 2;
			this.showButton.Text = "Sh&ow";
			this.showButton.Click += new System.EventHandler(this.showButtonClick);
			// 
			// resetButton
			// 
			this.resetButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.resetButton.BackColor = System.Drawing.SystemColors.Control;
			this.resetButton.Location = new System.Drawing.Point(415, 15);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(75, 24);
			this.resetButton.TabIndex = 3;
			this.resetButton.Text = "Rese&t";
			this.resetButton.Click += new System.EventHandler(this.resetButtonClick);
			// 
			// lblShowByReligion
			// 
			this.lblShowByReligion.AutoSize = true;
			this.lblShowByReligion.Location = new System.Drawing.Point(10, 19);
			this.lblShowByReligion.Name = "lblShowByReligion";
			this.lblShowByReligion.Size = new System.Drawing.Size(134, 16);
			this.lblShowByReligion.TabIndex = 0;
			this.lblShowByReligion.Text = "Show patients for religion:";
			this.lblShowByReligion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblSortBy
			// 
			this.lblSortBy.AutoSize = true;
			this.lblSortBy.Location = new System.Drawing.Point(24, 16);
			this.lblSortBy.Name = "lblSortBy";
			this.lblSortBy.Size = new System.Drawing.Size(43, 16);
			this.lblSortBy.TabIndex = 15;
			this.lblSortBy.Text = "Sort by:";
			this.lblSortBy.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// rdoReligion
			// 
			this.rdoReligion.Checked = true;
			this.rdoReligion.Location = new System.Drawing.Point(80, 16);
			this.rdoReligion.Name = "rdoReligion";
			this.rdoReligion.Size = new System.Drawing.Size(64, 16);
			this.rdoReligion.TabIndex = 2;
			this.rdoReligion.TabStop = true;
			this.rdoReligion.Text = "Religion";
			this.rdoReligion.Enter += new System.EventHandler(this.rdoReligion_Enter);
			this.rdoReligion.Leave += new System.EventHandler(this.rdoReligion_Leave);
			this.rdoReligion.CheckedChanged += new System.EventHandler(this.rdoReligion_CheckedChanged);
			// 
			// rdoNursing
			// 
			this.rdoNursing.Location = new System.Drawing.Point(152, 16);
			this.rdoNursing.Name = "rdoNursing";
			this.rdoNursing.Size = new System.Drawing.Size(104, 16);
			this.rdoNursing.TabIndex = 2;
			this.rdoNursing.Text = "Nursing station";
			this.rdoNursing.Enter += new System.EventHandler(this.rdoNursing_Enter);
			this.rdoNursing.Leave += new System.EventHandler(this.rdoNursing_Leave);
			// 
			// btnReportPrint
			// 
			this.btnReportPrint.BackColor = System.Drawing.SystemColors.Control;
			this.btnReportPrint.Enabled = false;
			this.btnReportPrint.Location = new System.Drawing.Point(792, 15);
			this.btnReportPrint.Name = "btnReportPrint";
			this.btnReportPrint.Size = new System.Drawing.Size(80, 24);
			this.btnReportPrint.TabIndex = 5;
			this.btnReportPrint.Text = "Pri&nt Report";
			this.btnReportPrint.Click += new System.EventHandler(this.btnReportPrint_Click);
			// 
			// ReligionCensusSearchPanel
			// 
			this.ReligionCensusSearchPanel.BackColor = System.Drawing.Color.White;
			this.ReligionCensusSearchPanel.Controls.Add(this.pnlRadioBtns);
			this.ReligionCensusSearchPanel.Controls.Add(this.cmbReligion);
			this.ReligionCensusSearchPanel.Controls.Add(this.showButton);
			this.ReligionCensusSearchPanel.Controls.Add(this.resetButton);
			this.ReligionCensusSearchPanel.Controls.Add(this.lblShowByReligion);
			this.ReligionCensusSearchPanel.Controls.Add(this.btnReportPrint);
			this.ReligionCensusSearchPanel.Location = new System.Drawing.Point(0, 0);
			this.ReligionCensusSearchPanel.Name = "ReligionCensusSearchPanel";
			this.ReligionCensusSearchPanel.Size = new System.Drawing.Size(912, 70);
			this.ReligionCensusSearchPanel.TabIndex = 16;
			// 
			// pnlRadioBtns
			// 
			this.pnlRadioBtns.Controls.Add(this.rdoReligion);
			this.pnlRadioBtns.Controls.Add(this.rdoNursing);
			this.pnlRadioBtns.Controls.Add(this.lblSortBy);
			this.pnlRadioBtns.Location = new System.Drawing.Point(512, 0);
			this.pnlRadioBtns.Name = "pnlRadioBtns";
			this.pnlRadioBtns.Size = new System.Drawing.Size(264, 128);
			this.pnlRadioBtns.TabIndex = 4;
			// 
			// ReligionCensusSearchView
			// 
			this.AcceptButton = this.showButton;
			this.Controls.Add(this.ReligionCensusSearchPanel);
			this.Name = "ReligionCensusSearchView";
			this.Size = new System.Drawing.Size(880, 64);
			this.Load += new System.EventHandler(this.ReligionCensusSearchView_Load);
			this.ReligionCensusSearchPanel.ResumeLayout(false);
			this.pnlRadioBtns.ResumeLayout(false);
			this.ResumeLayout(false);

		}

        #endregion

        #region Construction and Finalization

        public ReligionCensusSearchView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);
        }

        #endregion               
       
        #region Data Elements

        private BackgroundWorker                  backgroundWorker;
        private LoggingButton                                           showButton;
        private LoggingButton                                           resetButton;
        private LoggingButton                                           btnReportPrint;
        
        private Panel                              ReligionCensusSearchPanel;

        private ComboBox                           cmbReligion;
        
        private RadioButton                        rdoReligion;
        private RadioButton                        rdoNursing;      
        
        private Label                              lblShowByReligion;        
        private Label                              lblSortBy;

        private ICollection                                             accountProxiesCollection;
        private ICollection                                             collectionOfReligions;

        private string                                                  i_ReligionType;     
        private string                                                  i_SortByColumn; 
        private string                                                  selectedReligionDesc;
        private string                                                  selectedSortBy;
        private string                                                  selectedReligion;
                    
        private ArrayList                                               SearchCriteria = new ArrayList();

        #endregion    
       
        #region Constants

        private const string SORT_BY_RELIGION = "R";
		private Panel pnlRadioBtns;
        private const string SORT_BY_NURSING = "N";
        private const string UNSPECIFIED_RELIGION = "UNSPECIFIED";
       

        #endregion

    }
}
