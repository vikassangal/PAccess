using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using Appearance = Infragistics.Win.Appearance;
using PropertyChangedEventArgs = Infragistics.Win.PropertyChangedEventArgs;
using PropertyChangedEventHandler = Infragistics.Win.PropertyChangedEventHandler;
using PropertyIds = Infragistics.Win.UltraWinGrid.PropertyIds;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for InsurancePlanCensusResultView.
    /// </summary>
    public class InsurancePlanCensusResultView : ControlView
    {
        #region Event
        #endregion

        #region Event Handlers

        private void InsurancePlanCensusResultView_Load(object sender, EventArgs e)
        {
            this.progressPanel1.Visible      = false;
        }
         
        private void GridControlClick( UltraGridRow ultraGridRow )
        {
            if( sortingNotAllowed )
            {
                if( !ultraGridRow.GetType().Equals( typeof ( UltraGridGroupByRow ) ) )
                {
                    previousSelectedAccountNumber = 
                        Convert.ToString( ultraGridRow.Cells[ GRIDCOL_ACCOUNTNO ].Value );               
                }
            }
        }

        private void BeforeSortOrderChange( object sender, BeforeSortChangeEventArgs e )
        {
            e.Cancel = sortingNotAllowed;
        }

        private void CensusGrid_AfterSortChange(object sender, BandEventArgs e)
        {
            SetFirstRowSpacing();
        }

        private void CensusGrid_BeforeRowRegionScroll(object sender, BeforeRowRegionScrollEventArgs e)
        {
            SetFirstRowSpacing(e.NewState.FirstRow);
        }

        private void CensusGrid_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.ChangeInfo.FindPropId(PropertyIds.ColumnFilters) != null)
            {
                SetFirstRowSpacing();
            }
			
        }
        #endregion

        #region Methods

        public void BeforeWork(object sender, EventArgs e)
        {
            this.progressPanel1.Visible = true;
            this.progressPanel1.BringToFront();
            this.payorCensusResultsViewPanel.Visible = false;
        }

        public void AfterWork(object sender, EventArgs e)
        {
            if (this.IsDisposed || this.Disposing)
                
                return;
            this.progressPanel1.Visible = false;
            this.progressPanel1.SendToBack();
            this.payorCensusResultsViewPanel.Visible = true;
        }

        public override void UpdateView()
        {
            FillDataSource();
            SortOnRadioButton();
            insuranceGridCtrl.Focus(); 
        }

        public void ResetResultView()
        {
            this.payorCensusResultsViewPanel.Visible = true;
            this.payorResultgridPanel.Visible = true;
            this.payorResultgridPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblPayorResult.Visible = false;           
            this.previousSelectedAccountNumber = String.Empty;        
            dataSource.Rows.Clear();
        }    

        public void NoAccountsFound()
        {
            this.payorResultgridPanel.Visible = true;
            this.payorResultgridPanel.BorderStyle = BorderStyle.FixedSingle;
            this.lblPayorResult.Visible = true;
            this.previousSelectedAccountNumber = String.Empty;
            this.insuranceGridCtrl.Visible = false;
        }
        
        public void SortOnRadioButton()
        {
            sortingNotAllowed = false;            
            CustomizeGridLayout(); 
            
            if( this.i_SortByColumn.Equals( SORT_BY_PAYOR ) )
            {
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_NS].Hidden = true;
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_PAYOR_PLAN].GroupByMode = GroupByMode.Text;
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_PATIENT].SortIndicator = SortIndicator.Ascending;
                gridLayout.Bands[ACCOUNT_BAND].SortedColumns.Add( GRIDCOL_PAYOR_PLAN, false, true );
            }
            
            else
            {                
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_NS].GroupByMode = GroupByMode.Text;
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_PAYOR_PLAN].GroupByMode = GroupByMode.Text;
                gridLayout.Bands[ACCOUNT_BAND].Columns[GRIDCOL_PATIENT].SortIndicator = SortIndicator.Ascending;
                gridLayout.Bands[ACCOUNT_BAND].SortedColumns.Add( GRIDCOL_NS, false, true );
                gridLayout.Bands[ACCOUNT_BAND].SortedColumns.Add( GRIDCOL_PAYOR_PLAN, false, true );
            }
            this.insuranceGridCtrl.CensusGrid.DataSource = dataSource;
            SetActiveRow();
            
            this.insuranceGridCtrl.Name = "PatientGrid";
            this.lblPayorResult.Visible = false;           
            this.insuranceGridCtrl.Visible = true;
            sortingNotAllowed = true;
        }
        public void SetRowSelectionActiveAppearance()
        {
            insuranceGridCtrl.SetRowSelectionActiveAppearance();
        }
        public void SetRowSelectionInActiveAppearance()
        {
            insuranceGridCtrl.SetRowSelectionDimAppearance();
        }

        #endregion

        #region Properties
       
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
    
        private void SetActiveRow()
        {
            if( previousSelectedAccountNumber.Trim().Length <= 0 )
            {
                return;
            }
            string accountNumber;
            
            if( i_SortByColumn.Equals( SORT_BY_NURSING ) )
            {
                foreach( UltraGridGroupByRow ultraGridRow in this.insuranceGridCtrl.CensusGrid.Rows )
                {
                    foreach( UltraGridGroupByRow ultraGridGrpByRow in ultraGridRow.Rows )
                    {
                        foreach( UltraGridRow row in ultraGridGrpByRow.ChildBands[ACCOUNT_BAND].Rows )
                        {
                            accountNumber = Convert.ToString( row.Cells[GRIDCOL_ACCOUNTNO].Value );
                            
                            if( accountNumber.Equals( previousSelectedAccountNumber ) )
                            {
                                this.insuranceGridCtrl.CensusGrid.ActiveRow = row;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach( UltraGridGroupByRow ultraGridRow in this.insuranceGridCtrl.CensusGrid.Rows )
                {
                    foreach( UltraGridRow row in ultraGridRow.ChildBands[ACCOUNT_BAND].Rows )
                    {
                        accountNumber = Convert.ToString( row.Cells[GRIDCOL_ACCOUNTNO].Value );
                        
                        if( accountNumber.Equals( previousSelectedAccountNumber ) )
                        {
                            this.insuranceGridCtrl.CensusGrid.ActiveRow = row;
                        }
                    }
                }
            }
        }
        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            UltraDataBand acctBand = this.dataSource.Band;
            acctBand.Key = ACCOUNT_BAND;

            acctBand.Columns.Add( GRIDCOL_NS );
            acctBand.Columns.Add( GRIDCOL_PAYOR_PLAN );
            acctBand.Columns.Add( GRIDCOL_OPT_OUT );
            acctBand.Columns.Add( GRIDCOL_LOCATION );
            acctBand.Columns.Add( GRIDCOL_PATIENT );
            acctBand.Columns.Add( GRIDCOL_ACCOM_HSV );
            acctBand.Columns.Add( GRIDCOL_ADMITDATETIME_ATTPHY );
            acctBand.Columns.Add( GRIDCOL_LOS );
            acctBand.Columns.Add( GRIDCOL_FC );
            acctBand.Columns.Add( GRIDCOL_TOTAL_CURR_ACCT );
            acctBand.Columns.Add( GRIDCOL_SECONDARYPLAN );  
            acctBand.Columns.Add( GRIDCOL_ACCOUNTNO );
        }
        private void CustomizeGridLayout()
        {            
            UltraGridBand accountGridBand = new UltraGridBand( ACCOUNT_BAND, 0 );

            UltraGridColumn NSGridColumn = new UltraGridColumn( GRIDCOL_NS );
            UltraGridColumn payorPlanGridColumn = new UltraGridColumn( GRIDCOL_PAYOR_PLAN );
            UltraGridColumn prvcyOptGridColumn = new UltraGridColumn( GRIDCOL_OPT_OUT );
            UltraGridColumn locationGridColumn = new UltraGridColumn( GRIDCOL_LOCATION );
            UltraGridColumn patientGridColumn = new UltraGridColumn( GRIDCOL_PATIENT );
            UltraGridColumn accomHSVGridColumn = new UltraGridColumn( GRIDCOL_ACCOM_HSV );
            UltraGridColumn admitDateTimeAttPhyGridColumn = new UltraGridColumn( GRIDCOL_ADMITDATETIME_ATTPHY );
            UltraGridColumn LOSGridColumn = new UltraGridColumn( GRIDCOL_LOS );
            UltraGridColumn FCGridColumn = new UltraGridColumn( GRIDCOL_FC );
            UltraGridColumn totalCurrAcctGridColumn = new UltraGridColumn( GRIDCOL_TOTAL_CURR_ACCT );
            UltraGridColumn secondaryPlanGridColumn = new UltraGridColumn( GRIDCOL_SECONDARYPLAN );
            // This is hidden column used for finding active row to be highlighted 
            // upon change in the search or sort criteria
            UltraGridColumn AccountNumberColumn = new UltraGridColumn( GRIDCOL_ACCOUNTNO );

            accountGridBand.Columns.AddRange( 
                new object[] { 
                                 NSGridColumn,
                                 payorPlanGridColumn,
                                 prvcyOptGridColumn,
                                 locationGridColumn,
                                 patientGridColumn,
                                 accomHSVGridColumn,
                                 admitDateTimeAttPhyGridColumn,
                                 LOSGridColumn,
                                 FCGridColumn,
                                 totalCurrAcctGridColumn,
                                 secondaryPlanGridColumn,
                                 AccountNumberColumn });

            gridLayout = insuranceGridCtrl.CensusGrid.DisplayLayout;
            gridLayout.BandsSerializer.Add( accountGridBand );            

            accountBand = gridLayout.Bands[ACCOUNT_BAND];            
            this.insuranceGridCtrl.CensusGrid.DataBind();
            
            SetAccountBandProperties();
            SetColumnWidths();

            gridLayout.InterBandSpacing = 0;
            gridLayout.ClearGroupByColumns();
            gridLayout.Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;
            gridLayout.Override.SelectTypeCol = SelectType.None;
            gridLayout.Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.Disabled;
            gridLayout.Override.GroupByRowDescriptionMask = "[value]";            
            gridLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            gridLayout.GroupByBox.Hidden = true;

            this.insuranceGridCtrl.CensusGrid.InitializeGroupByRow += 
                new InitializeGroupByRowEventHandler( this.SetGroupAppearance );

            this.insuranceGridCtrl.CensusGrid.CreationFilter = gridCreationFilter =  
                new Single_Column_Header_At_Top_Creation_Filter( 
                this.insuranceGridCtrl.CensusGrid );
            this.SetFirstRowSpacing();

            this.insuranceGridCtrl.CensusGrid.AfterSortChange += new BandEventHandler(CensusGrid_AfterSortChange);
            this.insuranceGridCtrl.CensusGrid.BeforeRowRegionScroll += new BeforeRowRegionScrollEventHandler(CensusGrid_BeforeRowRegionScroll);
            this.insuranceGridCtrl.CensusGrid.PropertyChanged += new PropertyChangedEventHandler(CensusGrid_PropertyChanged);
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            string gender = String.Empty;
            string accomodation = String.Empty;
            string previousNursingStation = String.Empty;            
            UltraDataRow accountRow;
            string attendingPhysicianName;
            string consultingPhysicianNames;
            string physicianNames;   
                     
            ArrayList allAccountProxies = ( ArrayList ) this.Model;
           
            foreach( AccountProxy accountProxy in allAccountProxies )
            {
                attendingPhysicianName = String.Empty;
                consultingPhysicianNames = String.Empty;
                physicianNames = String.Empty;   

                accountRow = dataSource.Rows.Add();

                accountRow[GRIDCOL_NS] = "Nursing Station: " + accountProxy.Location.NursingStation.Code;
                accountRow[GRIDCOL_PAYOR_PLAN] = "Primary Payor: " + accountProxy.PayorName.Trim()
                    + "                          Plan: " + 
                    accountProxy.PrimaryInsurancePlan.Trim() + " " +
                    accountProxy.PrimaryPlanName.Trim();

                accountRow[ GRIDCOL_OPT_OUT ] = accountProxy.AddOnlyLegends();
            
                if( accountProxy.Location != null )
                {
                    accountRow[ GRIDCOL_LOCATION ] = accountProxy.Location.ToString();                  
                }
            
                if( accountProxy.Patient.Sex != null )
                {
                    gender = accountProxy.Patient.Sex.Code;
                }

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                String patientName = patientNameFormatter.GetFormattedPatientName();

                accountRow[ GRIDCOL_PATIENT ] = 
                    String.Format( "{0} \n      Account: {1}"+
                    "\n      MRN: {2} "+"\n      Gender: {3}    Age: {4}",
                    patientName,
                    accountProxy.AccountNumber.ToString().PadRight( 10, ' ' ),
                    accountProxy.Patient.MedicalRecordNumber.ToString().PadRight( 10, ' ' ),
                    gender,  
                    accountProxy.Patient.AgeAt
                    ( DateTime.Today ).PadLeft( 4, '0').ToUpper() );
            
                if( accountProxy.AttendingPhysician != null )
                {
                    attendingPhysicianName = "Phys: " 
                        + accountProxy.AttendingPhysician;
                }
                
                else
                {
                    attendingPhysicianName = "Phys: ";
                }
                
                if( accountProxy.Location != null && 
                    accountProxy.Location.Bed != null
                    && accountProxy.Location.Bed.Accomodation != null )
                {
                    accomodation = String.Format("{0} {1}",
                        accountProxy.Location.Bed.Accomodation.Code,
                        accountProxy.Location.Bed.Accomodation.Description );
                }
                
                else
                {
                    accomodation = "";
                }
                            
                if( accountProxy.HospitalService != null )
                {
                    accountRow[ GRIDCOL_ACCOM_HSV ] = accomodation + 
                        "\n" + "HSV: " + accountProxy.HospitalService.Code + 
                        " " + accountProxy.HospitalService.Description;
                }
                
                else
                {
                    accountRow[ GRIDCOL_ACCOM_HSV ] = accomodation;
                }

                //  accountRow[ GRIDCOL_LOS ] = accountProxy.LengthOfStay;

                if( accountProxy.FinancialClass != null )
                {
                    accountRow[ GRIDCOL_FC ] = accountProxy.FinancialClass.Code + 
                        " " + accountProxy.FinancialClass.Description;
                }
            
                accountRow[ GRIDCOL_TOTAL_CURR_ACCT ] = 
                    accountProxy.CalculatedAmountDue() +
                    "\n" +"Pmnts:  " + accountProxy.Payments.ToString("C");
                
                accountRow[ GRIDCOL_SECONDARYPLAN ] = accountProxy.SecondaryPlan + 
                    " " + accountProxy.SecondaryPlanName;     
            
                accountRow[ GRIDCOL_ADMITDATETIME_ATTPHY ] =   
                    "Date/Time:"+ accountProxy.AdmitDate.ToString( 
                    "MM/dd/yyyy HH:mm" ) +
                    "\n" + attendingPhysicianName;

                accountRow[ GRIDCOL_ACCOUNTNO ] = accountProxy.AccountNumber;
            }
        }

        private void SetAccountBandProperties()
        {
            accountBand.ColHeadersVisible = false;
            accountBand.GroupHeadersVisible = false;
            accountBand.Indentation = 0;
            accountBand.IndentationGroupByRow = 0;
            accountBand.Columns[GRIDCOL_NS].Hidden = true;
            accountBand.Columns[GRIDCOL_PAYOR_PLAN].Hidden = true;
            accountBand.Columns[GRIDCOL_ACCOUNTNO].Hidden = true;
            
            accountBand.Columns[GRIDCOL_PATIENT].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[GRIDCOL_ACCOM_HSV ].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[GRIDCOL_ADMITDATETIME_ATTPHY].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[GRIDCOL_TOTAL_CURR_ACCT].CellMultiLine = DefaultableBoolean.True; 
        }

        private void SetColumnWidths()
        {
            accountBand.Columns[ GRIDCOL_OPT_OUT ].Width = 57;
            accountBand.Columns[ GRIDCOL_LOCATION ].Width = 65;
            accountBand.Columns[ GRIDCOL_PATIENT ].Width = 195;
            accountBand.Columns[ GRIDCOL_ACCOM_HSV ].Width = 125;
            accountBand.Columns[ GRIDCOL_ADMITDATETIME_ATTPHY ].Width = 155;
            accountBand.Columns[ GRIDCOL_LOS ].Width = 35;
            accountBand.Columns[ GRIDCOL_FC ].Width = 40;
            accountBand.Columns[ GRIDCOL_TOTAL_CURR_ACCT ].Width = 120;
            accountBand.Columns[ GRIDCOL_SECONDARYPLAN ].Width = 100;
            accountBand.Columns[ GRIDCOL_ACCOUNTNO ].Width = 0;
        }

        private void SetGroupAppearance(object sender, InitializeGroupByRowEventArgs e)
        {
            Appearance groupAppearance = new Appearance();            

            if( e.Row.Column.Key.Equals( GRIDCOL_NS ) )
            {
                groupAppearance.ForeColor = Color.FromArgb( 035, 082, 169 );
                groupAppearance.FontData.Bold = DefaultableBoolean.True;
            }
            
            else if( e.Row.Column.Key.Equals( GRIDCOL_PAYOR_PLAN ) )
            {                 
                groupAppearance.BackColor = Color.FromArgb( 248, 240, 211 );
                groupAppearance.ForeColor = Color.Blue;
                groupAppearance.FontData.Bold = DefaultableBoolean.False;
            }               
                           
            e.Row.Appearance = groupAppearance;
        }

        
        private void SetFirstRowSpacing()
        {
            if (this.insuranceGridCtrl.CensusGrid.ActiveRowScrollRegion.FirstRow != null)
            {
                this.SetFirstRowSpacing( this.insuranceGridCtrl.CensusGrid.ActiveRowScrollRegion.FirstRow );
            }
        }

        private void SetFirstRowSpacing( UltraGridRow newFirstRow )
        {
            if (lastFirstRow == newFirstRow)
            {
                return;
            }

            if ( lastFirstRow != null)
            {
                lastFirstRow.RowSpacingBefore = 0;
            }            
            newFirstRow.RowSpacingBefore = this.gridCreationFilter.GetHeadersHeight( );
            lastFirstRow = newFirstRow;
        }    

        private class Single_Column_Header_At_Top_Creation_Filter : IUIElementCreationFilter
        {
            private UltraGridBase grid = null;

            internal Single_Column_Header_At_Top_Creation_Filter( UltraGridBase grid )
            {
                this.grid = grid;
            }

            private class GridHeaderUIElement : HeaderUIElement
            {
                internal GridHeaderUIElement( UIElement parent, HeaderBase context ) : base( parent )
                {
                    this.PrimaryContext = context;
                }
            }

            #region Implementation of IUIElementCreationFilter

            internal int GetHeadersHeight( )
            {
                return this.grid.DisplayLayout.Bands[ this.grid.DisplayLayout.Bands.Count - 1 ] .
                    GetFirstVisibleCol( 
                    this.grid.ActiveColScrollRegion, true ).Header.Height;
            }
            
            private void AddBandHeaders( BandHeadersUIElement parent )
            {
                UltraGridColumn column = parent.Band.GetFirstVisibleCol( 
                    this.grid.ActiveColScrollRegion, true );

                Rectangle parentRect = parent.Rect;
                int nextHeaderLeft = parentRect.X;

                bool isFirstHeader = true;
                while ( null != column )
                {
                    // ------------------------------------------------------------------------------
                    // Until a new build of v2 with public HeaderUIElement taking a header is dropped,
                    // use a derived GridHeaderUIElement class which takes in a header and sets the 
                    // PrimaryContext of the header ui element.
                    // ------------------------------------------------------------------------------
                    //HeaderUIElement colHeaderElem = new HeaderUIElement( parent, column.Header );
                    HeaderUIElement colHeaderElem = new GridHeaderUIElement( 
                        parent, column.Header );
                    Rectangle rect = new Rectangle(                         
                        nextHeaderLeft, parentRect.Y, column.Extent, 
                        parentRect.Height );

                    int rowSelectorWidth = 0;
                    
                    if ( isFirstHeader )
                    {
                        rect.Width += rowSelectorWidth;
                    }

                    isFirstHeader = false;
                    colHeaderElem.Rect = rect;
                    nextHeaderLeft = rect.Right;
					
                    parent.ChildElements.Add( colHeaderElem );
					
                    column = column.GetRelatedVisibleColumn( 
                        VisibleRelation.Next );
                }
            }


            public void AfterCreateChildElements( UIElement parent )
            {
                if ( parent is RowColRegionIntersectionUIElement )
                {
                    RowColRegionIntersectionUIElement rcrElem = 
                        ( RowColRegionIntersectionUIElement )parent;
                    RowScrollRegion rsr = rcrElem.RowScrollRegion;
                    ColScrollRegion csr = rcrElem.ColScrollRegion;


                    UltraGridBand band = this.grid.DisplayLayout.Bands[
                        this.grid.DisplayLayout.Bands.Count - 1];

                    BandHeadersUIElement bandHeadersElem = 
                        new BandHeadersUIElement(
                        parent, band, false, this.grid.Rows );

                    Rectangle rect = new Rectangle(
                        1 + band.GetOrigin( BandOrigin.RowSelector ) - csr.Position,   // X
                        parent.GetAncestor( typeof( DataAreaUIElement ) ).Rect.Y,  // Y
                        band.GetExtent( BandOrigin.RowSelector ),				   // Width	
                        this.GetHeadersHeight( ) );								   // Height

                    bandHeadersElem.Rect = rect;
                    parent.ChildElements.Add( bandHeadersElem );
                }
                
                else if ( parent is BandHeadersUIElement )
                {
                    this.AddBandHeaders( parent as BandHeadersUIElement );
                }
            }


            public bool BeforeCreateChildElements( UIElement parent )
            {
                return false;
            }
            

            #endregion
        }
        #endregion

        #region Private Properties
        #endregion      
       
        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.insuranceGridCtrl = new GridControl();
            this.lblPayorResult = new System.Windows.Forms.Label();       
            this.payorCensusResultsViewPanel = new System.Windows.Forms.Panel();
            this.payorCensusResultsViewPanel.SuspendLayout();
            this.progressPanel1 = new ProgressPanel();

            this.payorResultgridPanel = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)( 
                this.insuranceGridCtrl.CensusGrid ) ).BeginInit();


            this.SuspendLayout();
            // 
            // lblPayorResult
            // 
            this.lblPayorResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, 
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( ( System.Byte )( 0 ) ) );
            this.lblPayorResult.Location = new System.Drawing.Point(7, 8);
            this.lblPayorResult.Name = "lblPayorResult";
            this.lblPayorResult.Size = new System.Drawing.Size(296, 23);
            this.lblPayorResult.TabIndex = 0;
            this.lblPayorResult.Text = "No patients were found based on the selected criteria.";
            this.lblPayorResult.Visible = false;

            // 
            // payorCensusResultsViewPanel
            // 
            this.payorCensusResultsViewPanel.BackColor = System.Drawing.Color.White;
            this.payorCensusResultsViewPanel.Controls.Add(this.payorResultgridPanel);
            this.payorCensusResultsViewPanel.Location = new System.Drawing.Point(0, 0);
            this.payorCensusResultsViewPanel.Name = "payorCensusResultsViewPanel";
            this.payorCensusResultsViewPanel.Size = new System.Drawing.Size(912, 300);
            this.payorCensusResultsViewPanel.TabIndex = 0;

            // 
            // payorResultgridPanel
            // 
            this.payorResultgridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.payorResultgridPanel.Controls.Add(this.insuranceGridCtrl);
            this.payorResultgridPanel.Controls.Add(this.lblPayorResult);
            this.payorResultgridPanel.Location = new System.Drawing.Point(0, 0);
            this.payorResultgridPanel.Name = "payorResultgridPanel";
            this.payorResultgridPanel.Size = new System.Drawing.Size(899, 290);
            this.payorResultgridPanel.TabIndex = 1;
            // 
            // progressPanel1
            // 
            this.progressPanel1.BackColor = System.Drawing.Color.White;
            this.progressPanel1.Location = new System.Drawing.Point(0, 0);
            this.progressPanel1.Name = "progressPanel1";
            this.progressPanel1.Size = new System.Drawing.Size(904, 296);
            this.progressPanel1.TabIndex = 3;
            // 
            // insuranceGridCtrl
            // 
            appearance1.BackColor = System.Drawing.Color.White;
            this.insuranceGridCtrl.CensusGrid.DisplayLayout.Appearance = appearance1;
            this.insuranceGridCtrl.Location = new System.Drawing.Point(1,1);
            this.insuranceGridCtrl.Name = "insuranceGridCtrl";
            this.insuranceGridCtrl.Size = new System.Drawing.Size(896, 285);
            this.insuranceGridCtrl.TabIndex = 9; 
            this.insuranceGridCtrl.GridControl_Click += 
                new PatientAccess.UI.CommonControls.GridControl.UltraGridClickEventHandler
                ( this.GridControlClick );
            this.insuranceGridCtrl.GridControl_BeforeSortOrderChange +=
                new PatientAccess.UI.CommonControls.GridControl.BeforeSortChangeEventHandler
                ( this.BeforeSortOrderChange );

            // 
            // InsurancePlanCensusResultView
            // 
            this.Load +=new EventHandler(InsurancePlanCensusResultView_Load);
            this.ClientSize = new System.Drawing.Size(944, 470);
            this.Controls.Add(this.progressPanel1);
            this.Controls.Add(this.insuranceGridCtrl); 
            this.Name = "GridControl";
            ((System.ComponentModel.ISupportInitialize)(this.insuranceGridCtrl.CensusGrid)).EndInit();
            this.ResumeLayout(false);
            this.payorResultgridPanel.ResumeLayout(false);
            this.Controls.Add(this.payorCensusResultsViewPanel);
            this.payorCensusResultsViewPanel.ResumeLayout(false);         

        }
     

        #endregion

        #region Construction and Finalization
     
        public InsurancePlanCensusResultView()
        {
            InitializeComponent();
            base.EnableThemesOn( this );
           
            CreateDataStructure();
            insuranceGridCtrl.CensusGrid.DataSource = dataSource;
            CustomizeGridLayout();
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #endregion

        #region Data Elements

        private Container             components = null;
        
        private ProgressPanel                               progressPanel1;
        private UltraGridBand                               accountBand;
        private UltraGridLayout                             gridLayout;
        private GridControl                                 insuranceGridCtrl;      
        private UltraDataSource                             dataSource;
        Single_Column_Header_At_Top_Creation_Filter         gridCreationFilter  = null;
        UltraGridRow                                        lastFirstRow = null;

        private Label                  lblPayorResult;

        private Panel                  payorResultgridPanel;
        private Panel                  payorCensusResultsViewPanel;     

        private string                                      previousSelectedAccountNumber = String.Empty;     
        private string                                      i_SortByColumn;
        private string                                      i_PayorType = "C";

        private bool                                        sortingNotAllowed = true;
       
        #endregion  
        
        #region Constants

        private string ACCOUNT_BAND = "AccountBand";
        private const string
            GRIDCOL_NS                    = "Nursing station",
            GRIDCOL_PAYOR_PLAN            = "Payor plan",
            GRIDCOL_OPT_OUT               = "Prvcy Opt",
            GRIDCOL_LOCATION              = "Location",
            GRIDCOL_PATIENT               = "Patient",
            GRIDCOL_ACCOM_HSV             = "Accom/HSV",
            GRIDCOL_ADMITDATETIME_ATTPHY  = "Admit Date/Time and Att Phys",
            GRIDCOL_LOS                   = "LOS",
            GRIDCOL_FC                    = "FC",
            GRIDCOL_TOTAL_CURR_ACCT       = "Total Current Account",
            GRIDCOL_SECONDARYPLAN         = "Secondary Plan",
            GRIDCOL_ACCOUNTNO             = "AccountNo";
               
        private const string SORT_BY_PAYOR = "C";
        private const string SORT_BY_NURSING = "N";      
       
        #endregion
    }
}