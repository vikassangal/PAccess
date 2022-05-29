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
   [Serializable]
    public class InsurancePlanCensusSearchView : ControlView
    {
        #region Events

        public event EventHandler AccountsFound;
        public event EventHandler NoAccountsFound;
        public event EventHandler SortOnRadioButton;       
        public event EventHandler ResetView;
        public event EventHandler AcceptButtonChanged;
        public event EventHandler PrintReport;     
        public event EventHandler BeforeWorkEvent;
        public event EventHandler AfterWorkEvent;
  
        #endregion
        
        #region Event Handlers

        /// <summary>
        /// This method will clear the search result.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButtonClick( object sender, EventArgs e )
        {
            this.coverageCategoryListBox.ClearSelected(); 
            this.nursingStationListBox.ClearSelected();
            this.coverageCategoryCheckBox.Checked = false;
            this.nursingStationCheckBox.Checked = false;
            this.reportPrintButton.Enabled = false;
            this.searchButton.Enabled = false;
            this.ResetView(this, new LooseArgs( null ) );
            accountProxiesCollection = null;

        }
        
       private void InsurancePlanCensusSearchView_Layout(
           object sender, LayoutEventArgs e )
       {   // This gets called after Load event handler
           this.coverageCategoryListBox.SelectedIndex = -1;
       }

       private void BeforeWork()
       {
           if( this.BeforeWorkEvent != null )
           {
               this.BeforeWorkEvent( this, null );
           }

           this.Cursor = Cursors.WaitCursor;

           selectedNursing = new ArrayList();
           SetSortFlag();
           SetSortByColumn();

           if( coverageCategoryCheckBox.Checked )
           {
               selectedCoverageCategories = allcoverageCategories;
               selectedCoverageCategoryList = "ALL";
           }
           else
           {
               PopulateSelectedCoverageCategoryList();
           }
           if( nursingStationCheckBox.Checked )
           {
               selectedNursingStations = nursingStations;
               selectedNursingStationsList = "ALL";
           }
           else
           {
               PopulateSelectedNursingStationsList();
           }           
       }

       private void AfterWork( object sender, RunWorkerCompletedEventArgs e )
       {
           if (this.IsDisposed || this.Disposing)
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
               // Raise AccountsFound Event
               if (AccountsFound != null &&
                   accountProxiesCollection.Count > 0)
               {
                   AccountsFound(this,
                       new LooseArgs(accountProxiesCollection));
                   this.reportPrintButton.Enabled = true;
               }
               else
               {
                   if (NoAccountsFound != null)
                   {
                       NoAccountsFound(this, null);
                       this.reportPrintButton.Enabled = false;
                   }
               }

/*
               if (e.Cancelled)
               {
               }
               else
               {
                   // Raise AccountsFound Event
                   if (AccountsFound != null &&
                       accountProxiesCollection.Count > 0)
                   {
                       AccountsFound(this,
                           new LooseArgs(accountProxiesCollection));
                       this.reportPrintButton.Enabled = true;
                   }
                   else
                   {
                       if (NoAccountsFound != null)
                       {
                           NoAccountsFound(this, null);
                           this.reportPrintButton.Enabled = false;
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
               IAccountBroker accountBroker =
                   BrokerFactory.BrokerOfType<IAccountBroker>();

               accountProxiesCollection
                   = accountBroker.AccountsMatching(
                   selectedCoverageCategoryList,
                   selectedNursingStationsList,
                   User.GetCurrent().Facility.Code);

           // Poll the cancellationPending property, if true set e.Cancel to true and return.
           // Rationale: permit user cancellation of bgw. 
           if (this.backgroundWorker.CancellationPending)
           {
               e.Cancel = true;
               return;
           }     
       }

        /// <summary>
        /// this will fire based on coverage and nursing code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButtonClick( object sender, EventArgs e )
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
                    new DoWorkEventHandler(DoSearchData);
                backgroundWorker.RunWorkerCompleted +=
                    new RunWorkerCompletedEventHandler(AfterWork);

                backgroundWorker.RunWorkerAsync();
            }            
        }


        private void nursingRadio_CheckedChanged(object sender, EventArgs e)
       {
           sortByColumn();
       }

        
        private void reportPrintButtonClick( object sender, EventArgs e )
        {
            ArrayList allCovCategories = new ArrayList();
            ArrayList allNursingStations = new ArrayList();
            searchCriteria = new ArrayList();

            foreach( InsurancePlanCategory ipc in selectedCoverageCategories )
            {
                allCovCategories.Add( ipc.Description );
            }
            foreach( NursingStation ns in selectedNursingStations )
            {
                allNursingStations.Add( ns.Code );
            }
            
            searchCriteria.Add( allCovCategories );
            searchCriteria.Add( allNursingStations );
            searchCriteria.Add( i_SortByColumn );

            if( PrintReport != null )
            {
                PrintReport( this, new ReportArgs( 
                    accountProxiesCollection, searchCriteria, null ) );
            }
        }

        private void coverageCategoryCheckBox_CheckedChanged(object sender, EventArgs e)
       {
           this.coverageCategoryListBox.ClearSelected();
           if( this.coverageCategoryCheckBox.Checked )
           {
               this.coverageCategoryListBox.ClearSelected();
               this.coverageCategoryListBox.Enabled = false;
           }
           else
           {
               this.coverageCategoryListBox.Enabled = true;
           }
           CheckForDataEntered();
       }


        private void nursingStationCheckBox_CheckedChanged(object sender, EventArgs e)
       {
           this.nursingStationListBox.ClearSelected();
           if( this.nursingStationCheckBox.Checked )
           {
               this.nursingStationListBox.ClearSelected();
               this.nursingStationListBox.Enabled = false;
           }
           else
           {
               this.nursingStationListBox.Enabled = true;
           }
           CheckForDataEntered();
       }
      

        private void coverageCategoryListBox_SelectedIndexChanged(object sender, EventArgs e)
       {
           CheckForDataEntered();
       }
       

        private void nursingStationListBox_SelectedIndexChanged(object sender, EventArgs e)
       {
           CheckForDataEntered();
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
            if( this.Enabled && this.nursingStationListBox.Items.Count == 0 )
            {
                PopulateNursingStationListBox();
            }
            if( this.Enabled && this.coverageCategoryListBox.Items.Count == 0 )
            {
                PopulateCoverageCategoryListBox();
            }

			this.CheckForDataEntered();
        }

        public string PayorType
        {
            get
            {
                return i_PayorType;
            }
            set
            {
                i_PayorType = value;
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
        /// Method will populate all CoverageCategory
        /// </summary>
        private void PopulateCoverageCategoryListBox()
        {
            try
            {               
                user = User.GetCurrent();
                facility = User.GetCurrent().Facility;
                IInsuranceBroker broker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
                allcoverageCategories = ( ArrayList )broker.AllTypesOfCategories(facility.Oid);

                coverageCategoryListBox.ValueMember   = "Oid";
                coverageCategoryListBox.DisplayMember = "Description";
                coverageCategoryListBox.DataSource    = allcoverageCategories;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                coverageCategoryListBox.ClearSelected();
            }
        }
        
       /// <summary>
       /// Method will populate all NursingStation
       /// </summary>
       
        private void PopulateNursingStationListBox()
        {
            try
            {
                user = User.GetCurrent();
                facility = User.GetCurrent().Facility;
                LocationBrokerProxy locationBroker = new LocationBrokerProxy( );
                    
                allnursingStations = locationBroker.NursingStationsFor(facility);

                allNursingStation = new ArrayList();
                nursingStations = new ArrayList();

                NursingStation ns;
                for( int i = 0;i < allnursingStations.Count; i++ )    
                {
                    ns = ( NursingStation )allnursingStations[i];

                    if( ns.Code.Trim().Equals( ALL_NURSINGSTATION_CODE ) )
                    {
                        ns.Code = ALL_NURSINGSTATION_LABEL;
                        allNursingStation.Add( ns );
                    }
                    else
                    {
                        nursingStations.Add( ns );
                        nursingStationListBox.Items.Add( ns.Code );
                    }                    
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }        

        private void CheckForDataEntered()
        {
            if(  ( this.coverageCategoryListBox.SelectedIndex != -1 && 
                this.nursingStationListBox.SelectedIndex != -1 ) ||
                ( this.coverageCategoryCheckBox.Checked && 
                this.nursingStationCheckBox.Checked ) )
            {
                this.searchButton.Enabled = true;
                if( this.ParentForm != null )
                {
                    this.AcceptButton = searchButton;
                }
            }
            else if( this.coverageCategoryListBox.SelectedIndex != -1 && 
                this.nursingStationCheckBox.Checked)
            {
                this.searchButton.Enabled = true;
                if( this.ParentForm != null )
                {
                    this.AcceptButton = searchButton;
                }
            }
            else if( this.nursingStationListBox.SelectedIndex != -1 && 
                this.coverageCategoryCheckBox.Checked  )
            {
                this.searchButton.Enabled = true;
                if( this.ParentForm != null )
                {
                    this.AcceptButton = searchButton;
                }
            }
            else if(  this.coverageCategoryCheckBox.Checked && 
                this.nursingStationCheckBox.Checked )
            {
                this.searchButton.Enabled = true;
                if( this.ParentForm != null )
                {
                    this.AcceptButton = searchButton;
                }
            }
            else if( ( this.coverageCategoryListBox.SelectedIndex == -1 && 
                this.nursingStationListBox.SelectedIndex == -1 ) ||
                ( this.coverageCategoryCheckBox.Checked == false && 
                this.nursingStationCheckBox.Checked == false ) )
            {
                    this.searchButton.Enabled = false;   
            }
            else
            {
                this.searchButton.Enabled = false;               
            }
        }


        private void SetSortByColumn()
        {
            if( payorRadio.Checked )
            {
                i_SortByColumn = SORT_BY_PAYOR;
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
                    ( SortOnRadioButton != null ) )
                {
                    SortOnRadioButton( this, new LooseArgs( this ) );
                }          
        }    

        private void PopulateSelectedNursingStationsList()
        {
            selectedNursingStations = new ArrayList();

            NursingStation ns;

            selectedNursingStationsList = String.Empty;

            if( this.nursingStationCheckBox.Checked )
            {
                for( int i = 0;i < nursingStations.Count; i++ )    
                {
                    ns = ( NursingStation )nursingStations[i];
                    selectedNursingStations.Add( ns );
                }
            }
            else
            {
                foreach( object listItem in nursingStationListBox.SelectedItems ) 
                {
                    ns = ( NursingStation )nursingStations[ 
                        nursingStationListBox.Items.IndexOf( listItem.ToString() ) ];
                    selectedNursingStations.Add( ns );                  
                            
                    if( !selectedNursingStationsList.Equals( String.Empty ) )
                    {
                        selectedNursingStationsList =  
                            selectedNursingStationsList + ", ";
                    }

                    selectedNursingStationsList = selectedNursingStationsList 
                        + "'" + listItem + "'";   
                }
            }
        }

        private void PopulateSelectedCoverageCategoryList()
        {
           
           selectedCoverageCategoryList = String.Empty;
            selectedCoverageCategories = new ArrayList();
            InsurancePlanCategory ipc;

            if( this.coverageCategoryCheckBox.Checked )
            {
                for( int i = 0;i < allcoverageCategories.Count; i++ )    
                {
                    ipc = ( InsurancePlanCategory )allcoverageCategories[i];
                    selectedCoverageCategories.Add( ipc );
                }
            }

            else
            {
                foreach( object listItem in coverageCategoryListBox.SelectedItems ) 
                {                   
                    ipc = ( InsurancePlanCategory )listItem;
                    selectedCoverageCategories.Add( ipc );                  
                        
                    if( !selectedCoverageCategoryList.Equals( String.Empty ) )
                    {
                        selectedCoverageCategoryList =  
                            selectedCoverageCategoryList + ", ";
                    }   
                    selectedCoverageCategoryList = selectedCoverageCategoryList 
                        + "'" + ipc.Oid + "'";
                }
            }
        }

       private void coverageCategoryListBox_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void coverageCategoryListBox_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }
       
       private void nursingStationListBox_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void nursingStationListBox_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }

       private void coverageCategoryCheckBox_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void coverageCategoryCheckBox_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }

       private void nursingStationCheckBox_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void nursingStationCheckBox_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }
       
       private void payorRadio_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void payorRadio_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }

       private void nursingRadio_Enter(object sender, EventArgs e)
       {
           if( AcceptButtonChanged != null )
           {
               AcceptButtonChanged( this, new LooseArgs( this ) );
           }
       }

       private void nursingRadio_Leave(object sender, EventArgs e)
       {
           if( this.ParentForm != null )
           {
               this.AcceptButton = searchButton;
           }
       }

       #endregion       

        #region Private Properties
     
        private void SetSortFlag()
        {
            if( payorRadio.Checked )
            {
                i_PayorType = "C";
            }           
            else
            {
                i_PayorType = "N";
            }
        }


       #endregion

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
			this.searchButton = new LoggingButton();
			this.resetButton = new LoggingButton();
			this.showByPayorLabel = new System.Windows.Forms.Label();
			this.sortByLabel = new System.Windows.Forms.Label();
			this.payorRadio = new System.Windows.Forms.RadioButton();
			this.nursingRadio = new System.Windows.Forms.RadioButton();
			this.reportPrintButton = new LoggingButton();
			this.PayorCensusSearchPanel = new System.Windows.Forms.Panel();
			this.pnlRadioBtns = new System.Windows.Forms.Panel();
			this.coverageCategoryListBox = new System.Windows.Forms.ListBox();
			this.nursingStationListBox = new System.Windows.Forms.ListBox();
			this.nursingStationCheckBox = new System.Windows.Forms.CheckBox();
			this.coverageCategoryCheckBox = new System.Windows.Forms.CheckBox();
			this.nursingStationLabel = new System.Windows.Forms.Label();
			this.PayorCensusSearchPanel.SuspendLayout();
			this.pnlRadioBtns.SuspendLayout();
			this.SuspendLayout();
			// 
			// searchButton
			// 
			this.searchButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.searchButton.BackColor = System.Drawing.SystemColors.Control;
			this.searchButton.Enabled = false;
			this.searchButton.Location = new System.Drawing.Point(254, 56);
			this.searchButton.Name = "searchButton";
			this.searchButton.Size = new System.Drawing.Size(75, 24);
			this.searchButton.TabIndex = 5;
			this.searchButton.Text = "Sh&ow";
			this.searchButton.Click += new System.EventHandler(this.searchButtonClick);
			// 
			// resetButton
			// 
			this.resetButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.resetButton.BackColor = System.Drawing.SystemColors.Control;
			this.resetButton.Location = new System.Drawing.Point(334, 56);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(75, 24);
			this.resetButton.TabIndex = 6;
			this.resetButton.Text = "Rese&t";
			this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
			// 
			// showByPayorLabel
			// 
			this.showByPayorLabel.Location = new System.Drawing.Point(0, 7);
			this.showByPayorLabel.Name = "showByPayorLabel";
			this.showByPayorLabel.Size = new System.Drawing.Size(208, 24);
			this.showByPayorLabel.TabIndex = 0;
			this.showByPayorLabel.Text = "Show patients for coverage category:";
			this.showByPayorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// sortByLabel
			// 
			this.sortByLabel.Location = new System.Drawing.Point(16, 8);
			this.sortByLabel.Name = "sortByLabel";
			this.sortByLabel.Size = new System.Drawing.Size(56, 16);
			this.sortByLabel.TabIndex = 15;
			this.sortByLabel.Text = "Sort by:";
			this.sortByLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// payorRadio
			// 
			this.payorRadio.Checked = true;
			this.payorRadio.Location = new System.Drawing.Point(88, 8);
			this.payorRadio.Name = "payorRadio";
			this.payorRadio.Size = new System.Drawing.Size(104, 16);
			this.payorRadio.TabIndex = 7;
			this.payorRadio.TabStop = true;
			this.payorRadio.Text = "Primary payor";
			this.payorRadio.Enter += new System.EventHandler(this.payorRadio_Enter);
			this.payorRadio.Leave += new System.EventHandler(this.payorRadio_Leave);
			// 
			// nursingRadio
			// 
			this.nursingRadio.Location = new System.Drawing.Point(192, 8);
			this.nursingRadio.Name = "nursingRadio";
			this.nursingRadio.Size = new System.Drawing.Size(104, 16);
			this.nursingRadio.TabIndex = 8;
			this.nursingRadio.Text = "Nursing station";
			this.nursingRadio.Enter += new System.EventHandler(this.nursingRadio_Enter);
			this.nursingRadio.Leave += new System.EventHandler(this.nursingRadio_Leave);
			this.nursingRadio.CheckedChanged += new System.EventHandler(this.nursingRadio_CheckedChanged);
			// 
			// reportPrintButton
			// 
			this.reportPrintButton.BackColor = System.Drawing.SystemColors.Control;
			this.reportPrintButton.Enabled = false;
			this.reportPrintButton.Location = new System.Drawing.Point(816, 7);
			this.reportPrintButton.Name = "reportPrintButton";
			this.reportPrintButton.Size = new System.Drawing.Size(80, 24);
			this.reportPrintButton.TabIndex = 9;
			this.reportPrintButton.Text = "Pri&nt Report";
			this.reportPrintButton.Click += new System.EventHandler(this.reportPrintButtonClick);
			// 
			// PayorCensusSearchPanel
			// 
			this.PayorCensusSearchPanel.BackColor = System.Drawing.Color.White;
			this.PayorCensusSearchPanel.Controls.Add(this.pnlRadioBtns);
			this.PayorCensusSearchPanel.Controls.Add(this.coverageCategoryListBox);
			this.PayorCensusSearchPanel.Controls.Add(this.nursingStationListBox);
			this.PayorCensusSearchPanel.Controls.Add(this.nursingStationCheckBox);
			this.PayorCensusSearchPanel.Controls.Add(this.coverageCategoryCheckBox);
			this.PayorCensusSearchPanel.Controls.Add(this.nursingStationLabel);
			this.PayorCensusSearchPanel.Controls.Add(this.searchButton);
			this.PayorCensusSearchPanel.Controls.Add(this.resetButton);
			this.PayorCensusSearchPanel.Controls.Add(this.showByPayorLabel);
			this.PayorCensusSearchPanel.Controls.Add(this.reportPrintButton);
			this.PayorCensusSearchPanel.Location = new System.Drawing.Point(0, 0);
			this.PayorCensusSearchPanel.Name = "PayorCensusSearchPanel";
			this.PayorCensusSearchPanel.Size = new System.Drawing.Size(912, 152);
			this.PayorCensusSearchPanel.TabIndex = 16;
			// 
			// pnlRadioBtns
			// 
			this.pnlRadioBtns.Controls.Add(this.sortByLabel);
			this.pnlRadioBtns.Controls.Add(this.payorRadio);
			this.pnlRadioBtns.Controls.Add(this.nursingRadio);
			this.pnlRadioBtns.Location = new System.Drawing.Point(464, 0);
			this.pnlRadioBtns.Name = "pnlRadioBtns";
			this.pnlRadioBtns.Size = new System.Drawing.Size(328, 176);
			this.pnlRadioBtns.TabIndex = 17;
			// 
			// coverageCategoryListBox
			// 
			this.coverageCategoryListBox.Location = new System.Drawing.Point(0, 56);
			this.coverageCategoryListBox.Name = "coverageCategoryListBox";
			this.coverageCategoryListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.coverageCategoryListBox.Size = new System.Drawing.Size(176, 69);
			this.coverageCategoryListBox.TabIndex = 2;
			this.coverageCategoryListBox.Leave += new System.EventHandler(this.coverageCategoryListBox_Leave);
			this.coverageCategoryListBox.Enter += new System.EventHandler(this.coverageCategoryListBox_Enter);
			this.coverageCategoryListBox.SelectedIndexChanged += new System.EventHandler(this.coverageCategoryListBox_SelectedIndexChanged);
			// 
			// nursingStationListBox
			// 
			this.nursingStationListBox.Location = new System.Drawing.Point(200, 56);
			this.nursingStationListBox.Name = "nursingStationListBox";
			this.nursingStationListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.nursingStationListBox.Size = new System.Drawing.Size(48, 69);
			this.nursingStationListBox.TabIndex = 4;
			this.nursingStationListBox.Leave += new System.EventHandler(this.nursingStationListBox_Leave);
			this.nursingStationListBox.Enter += new System.EventHandler(this.nursingStationListBox_Enter);
			this.nursingStationListBox.SelectedIndexChanged += new System.EventHandler(this.nursingStationListBox_SelectedIndexChanged);
			// 
			// nursingStationCheckBox
			// 
			this.nursingStationCheckBox.Location = new System.Drawing.Point(200, 32);
			this.nursingStationCheckBox.Name = "nursingStationCheckBox";
			this.nursingStationCheckBox.Size = new System.Drawing.Size(104, 16);
			this.nursingStationCheckBox.TabIndex = 3;
			this.nursingStationCheckBox.Text = "All";
			this.nursingStationCheckBox.Enter += new System.EventHandler(this.nursingStationCheckBox_Enter);
			this.nursingStationCheckBox.Leave += new System.EventHandler(this.nursingStationCheckBox_Leave);
			this.nursingStationCheckBox.CheckedChanged += new System.EventHandler(this.nursingStationCheckBox_CheckedChanged);
			// 
			// coverageCategoryCheckBox
			// 
			this.coverageCategoryCheckBox.Location = new System.Drawing.Point(0, 32);
			this.coverageCategoryCheckBox.Name = "coverageCategoryCheckBox";
			this.coverageCategoryCheckBox.TabIndex = 1;
			this.coverageCategoryCheckBox.Text = "All";
			this.coverageCategoryCheckBox.Enter += new System.EventHandler(this.coverageCategoryCheckBox_Enter);
			this.coverageCategoryCheckBox.Leave += new System.EventHandler(this.coverageCategoryCheckBox_Leave);
			this.coverageCategoryCheckBox.CheckedChanged += new System.EventHandler(this.coverageCategoryCheckBox_CheckedChanged);
			// 
			// nursingStationLabel
			// 
			this.nursingStationLabel.Location = new System.Drawing.Point(200, 7);
			this.nursingStationLabel.Name = "nursingStationLabel";
			this.nursingStationLabel.Size = new System.Drawing.Size(100, 24);
			this.nursingStationLabel.TabIndex = 16;
			this.nursingStationLabel.Text = "In nursing station:";
			this.nursingStationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// InsurancePlanCensusSearchView
			// 
			this.AcceptButton = this.searchButton;
			this.Controls.Add(this.PayorCensusSearchPanel);
			this.Name = "InsurancePlanCensusSearchView";
			this.Size = new System.Drawing.Size(912, 152);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.InsurancePlanCensusSearchView_Layout);
			this.PayorCensusSearchPanel.ResumeLayout(false);
			this.pnlRadioBtns.ResumeLayout(false);
			this.ResumeLayout(false);

		}


        #endregion

        #region Construction and Finalization

        public InsurancePlanCensusSearchView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
            CensusEventAggregator.GetInstance().CloseEventHandler += new EventHandler(this.censusView_Close);
        }        

        #endregion               
       
       #region Data Elements

       private BackgroundWorker               backgroundWorker;
       private User                                                 user;
       private Facility                                             facility;

       private LoggingButton                                        searchButton;
       private LoggingButton                                        resetButton;
       private LoggingButton                                        reportPrintButton;

       private Panel                           PayorCensusSearchPanel;
       private Panel                           pnlRadioBtns;

       private RadioButton                     payorRadio;
       private RadioButton                     nursingRadio;
      
       private Label                           showByPayorLabel;
       private Label                           sortByLabel;
       private Label                           nursingStationLabel;

       private CheckBox                        coverageCategoryCheckBox;
       private CheckBox                        nursingStationCheckBox;
       
       private ListBox                         nursingStationListBox;
       private ListBox                         coverageCategoryListBox;

       private ICollection                                          accountProxiesCollection;

       private ArrayList                                            selectedCoverageCategories;
       private ArrayList                                            searchCriteria;
       private ArrayList                                            nursingStations;      
       private ArrayList                                            selectedNursing;
       private ArrayList                                            allNursingStation;
       private IList<NursingStation>                                allnursingStations;       
       private ArrayList                                            allcoverageCategories;
       private ArrayList                                            selectedNursingStations; 
     
       private string                                               selectedNursingStationsList;
       private string                                               selectedCoverageCategoryList;
       private string                                               i_PayorType; 
       private string                                               i_SortByColumn; 
      
       #endregion           
       
        #region Constants

        private const string SORT_BY_PAYOR = "C";
        private const string SORT_BY_NURSING = "N"; 
        private const string ALL_NURSINGSTATION_CODE = "$$";
        private const string ALL_NURSINGSTATION_LABEL = "ALL";

        #endregion
      
    }
}
