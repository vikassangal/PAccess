using System;
using System.Collections;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for ReligionReport.
    /// </summary>
    public class ReligionReport : PrintReport
    {
        #region Events
        #endregion

        #region EventHandlers
        #endregion

        #region Methods
        
        public void PrintPreview()
        {
            FillDataSource();
            FillSummaryDataSource();
            base.DataSource = dataSource;
            base.UpdateView();
            CustomizeGridLayout();
            base.HeaderText = HEADER_TEXT;
            base.FooterText = UILabels.CENSUS_OPT_OUT_LEGENDS;     
            base.GeneratePrintPreview();
        }

        #endregion

        #region Properties

        public new ArrayList Model
        {
            private get
            {
                return base.Model as ArrayList;
            }
            set
            {
                base.Model = value;            
            }
        }

        public new ArrayList SearchCriteria
        {
            private get
            {
                return base.SearchCriteria as ArrayList;
            }
            set
            {
                base.SearchCriteria = value;
            }
        }

        public new ArrayList SummaryInformation
        {
            private get
            {
                return base.SummaryInformation as ArrayList;
            }
            set
            {
                base.SummaryInformation = value;
            }
        }
        #endregion
       
        #region Construction and finalization
        public ReligionReport()
        {			
            CreateDataStructure();            
        }
        #endregion

        #region Private Methods
        
        private void CreateDataStructure()
        {           
            dataSource = new UltraDataSource();

            this.dataSource.Band.Key = SEARCH_CRITERIA_BAND;
            this.dataSource.Band.Columns.Add( GRIDCOL_SELECTED_RELIGION );
            this.dataSource.Band.Columns.Add( GRIDCOL_SORTBY );

            this.dataSource.Band.Columns.Add( GRIDCOL_NO_OF_RECORDS );  

            religionDataBand = this.dataSource.Band.ChildBands.Add( RELIGION_BAND );
            religionDataBand.Columns.Add( GRIDCOL_RELIGION );
            religionDataBand.Columns.Add( GRIDCOL_OPTOUT );
            religionDataBand.Columns.Add( GRIDCOL_PATIENT_NAME  );
            religionDataBand.Columns.Add( GRIDCOL_LOCATION  );
                
            accountInfoDataBand= religionDataBand.ChildBands.Add( ACCOUNT_INFORMATION_BAND );
            
            accountInfoDataBand.Columns.Add( GRIDCOL_AGE_INFO );
            accountInfoDataBand.Columns.Add( GRIDCOL_LOCATION_INFO );
            accountInfoDataBand.Columns.Add( GRIDCOL_DATE_LABEL );
            accountInfoDataBand.Columns.Add( GRIDCOL_DATE_VALUE );
            accountInfoDataBand.Columns.Add( GRIDCOL_WORSHIP_LABEL );
            accountInfoDataBand.Columns.Add( GRIDCOL_WORSHIP_VALUE );

            lastDataBand=this.dataSource.Band.ChildBands.Add( LAST_BAND  );
            
            lastDataBand.Columns.Add( GRIDCOL_LAST );

            emptyDataBand=this.dataSource.Band.ChildBands.Add( EMPTY_BAND );
            
            emptyDataBand.Columns.Add( GRIDCOL_EMPTY );

            summaryDataBand= this.dataSource.Band.ChildBands.Add( SUMMARY_BAND );
            
            summaryDataBand.Columns.Add( GRIDCOL_RELIGION );
            summaryDataBand.Columns.Add( GRIDCOL_TOTAL ) ;
            summaryDataBand.Columns.Add(" ");

        }

        private void CustomizeGridLayout()
        {

            UltraGridBand searchCriteriaBand = new UltraGridBand( SEARCH_CRITERIA_BAND, 0);
            UltraGridBand religionBand       = new UltraGridBand( RELIGION_BAND, 1);
            UltraGridBand accountBand        = new UltraGridBand( ACCOUNT_INFORMATION_BAND, 2);
            UltraGridBand lastBand           = new UltraGridBand( LAST_BAND, 3);
            UltraGridBand emptyBand          = new UltraGridBand( EMPTY_BAND, 4);
            UltraGridBand summaryBand        = new UltraGridBand( SUMMARY_BAND, 5);
        
            UltraGridColumn  selectedReligionGridColumn = 
                                                new UltraGridColumn( GRIDCOL_SELECTED_RELIGION );
            UltraGridColumn  sortbyGridColumn = new UltraGridColumn( GRIDCOL_SORTBY );
            UltraGridColumn  noofRecordsGridColumn = new UltraGridColumn( GRIDCOL_NO_OF_RECORDS );
           
            searchCriteriaBand.Columns.AddRange( new object[] {
                                                                selectedReligionGridColumn,
                                                                sortbyGridColumn,
                                                                noofRecordsGridColumn });

            UltraGridColumn  religionGridColumn    = new UltraGridColumn( GRIDCOL_RELIGION );
            UltraGridColumn  optoutGridColumn      = new UltraGridColumn( GRIDCOL_OPTOUT );
            UltraGridColumn  patientNameGridColumn = new UltraGridColumn( GRIDCOL_PATIENT_NAME );
            UltraGridColumn  locationGridColumn    = new UltraGridColumn( GRIDCOL_LOCATION );

            religionBand.Columns.AddRange( new object[] {
                                                          religionGridColumn,
                                                          optoutGridColumn,
                                                          patientNameGridColumn ,
                                                          locationGridColumn });

            UltraGridColumn  ageInfoGridColumn      = new UltraGridColumn( GRIDCOL_AGE_INFO );
            UltraGridColumn  locationInfoGridColumn = new UltraGridColumn( GRIDCOL_LOCATION_INFO );
            UltraGridColumn  dateLabelGridColumn    = new UltraGridColumn( GRIDCOL_DATE_LABEL );
            UltraGridColumn  dateValueGridColumn    = new UltraGridColumn( GRIDCOL_DATE_VALUE );
            UltraGridColumn  worshipLabelGridColumn = new UltraGridColumn( GRIDCOL_WORSHIP_LABEL );
            UltraGridColumn  worshipValueGridColumn = new UltraGridColumn( GRIDCOL_WORSHIP_VALUE );

            accountBand.Columns.AddRange( new object[] {
                                                            ageInfoGridColumn,
                                                            locationInfoGridColumn,
                                                            dateLabelGridColumn,
                                                            dateValueGridColumn,
                                                            worshipLabelGridColumn,
                                                            worshipValueGridColumn } );

            UltraGridColumn lastGridColumn = new UltraGridColumn( GRIDCOL_LAST );
            
            lastBand.Columns.AddRange( new object[] { lastGridColumn } );

            UltraGridColumn emptyGridColumn = new UltraGridColumn( GRIDCOL_EMPTY );
           
            emptyBand.Columns.AddRange( new object[] { emptyGridColumn} );
            
            UltraGridColumn  summaryReligionGridColumn = new UltraGridColumn( GRIDCOL_RELIGION_LABEL );
            UltraGridColumn  summaryTotalGridColumn    = new UltraGridColumn( GRIDCOL_TOTAL );
            UltraGridColumn  summaryEmptyGridColumn    = new UltraGridColumn("");
            
            summaryBand.Columns.AddRange( new object[] {
                                                            summaryReligionGridColumn,
                                                            summaryTotalGridColumn,
                                                            summaryEmptyGridColumn } );
            printLayout = PrintGrid.DisplayLayout;
            
            printLayout.BandsSerializer.Add( religionBand );
            printLayout.BandsSerializer.Add( accountBand );
            printLayout.BandsSerializer.Add( lastBand );
            printLayout.BandsSerializer.Add( emptyBand );
            printLayout.BandsSerializer.Add( summaryBand );

            searchDisplayBand = printLayout.Bands[ SEARCH_CRITERIA_BAND ];
            religionDisplayBand = printLayout.Bands[ RELIGION_BAND ];
            accountDisplayBand  = printLayout.Bands[ ACCOUNT_INFORMATION_BAND ];
            lastDisplayBand = printLayout.Bands[ LAST_BAND ];
            emptyDisplayBand = printLayout.Bands[ EMPTY_BAND ];
            summaryDisplayBand = printLayout.Bands[ SUMMARY_BAND ];

            printLayout.Override.RowSelectors = DefaultableBoolean.False;
            printLayout.Override.ExpansionIndicator = ShowExpansionIndicator.Never;
            printLayout.Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.Disabled;
            printLayout.Override.GroupByRowDescriptionMask = "[value]";

            religionDisplayBand.IndentationGroupByRow = 0;
            PrintGrid.Rows.ExpandAll(true);

            summaryDisplayBand.Header.Caption = SUMMARY_BAND_HEADER_TEXT;
    
            searchDisplayBand.HeaderVisible     = false;
            searchDisplayBand.ColHeadersVisible = false;
            religionDisplayBand.ColHeadersVisible = false;
            religionDisplayBand.HeaderVisible     = false;
            accountDisplayBand.ColHeadersVisible = false;
            accountDisplayBand.HeaderVisible     = false;
            emptyDisplayBand.ColHeadersVisible = false;
            summaryDisplayBand.HeaderVisible     = true;
           
            searchDisplayBand.Override = base.OverrideWithoutBorder;
            religionDisplayBand.Override = base.OverrideWithBorder;
            accountDisplayBand.Override = base.OverrideWithoutBorder;           
            emptyDisplayBand.Override = base.OverrideWithoutBorder;
           
            searchDisplayBand.Indentation = 0;
            religionDisplayBand.Indentation = 0;
            accountDisplayBand.Indentation = 120;
            lastDisplayBand.Indentation = 0;
            emptyDisplayBand.Indentation = 0;
            summaryDisplayBand.Indentation = 14;
           
            SetColumnWidth();
            
            accountDisplayBand.Columns[ GRIDCOL_LOCATION_INFO ].CellMultiLine = DefaultableBoolean.True;
            accountDisplayBand.Columns[ GRIDCOL_AGE_INFO ].CellMultiLine = DefaultableBoolean.True;
            accountDisplayBand.Columns[ GRIDCOL_DATE_LABEL ].CellMultiLine = DefaultableBoolean.True;
            accountDisplayBand.Columns[ GRIDCOL_DATE_VALUE ].CellMultiLine = DefaultableBoolean.True;
             
            religionDisplayBand.Columns[ GRIDCOL_OPTOUT ].CellAppearance.FontData.Bold = DefaultableBoolean.True;
            religionDisplayBand.Columns[ GRIDCOL_PATIENT_NAME ].CellAppearance.FontData.Bold = DefaultableBoolean.True;
            searchDisplayBand.Columns[ GRIDCOL_NO_OF_RECORDS ].CellAppearance.FontData.Bold = DefaultableBoolean.True;
            searchDisplayBand.Columns[ GRIDCOL_NO_OF_RECORDS ].CellAppearance.TextHAlign    = HAlign.Right;             
            summaryDisplayBand.Columns[ GRIDCOL_TOTAL ].CellAppearance.TextHAlign    = HAlign.Left;
               
            
            summaryDisplayBand.Header.Appearance.BackColor   = Color.White;
            summaryDisplayBand.Header.Appearance.BorderColor = Color.White;
            summaryDisplayBand.Header.Appearance.TextHAlign  = HAlign.Left ;
            summaryDisplayBand.Override.HeaderAppearance.TextHAlign = HAlign.Left ;
            summaryDisplayBand.ColHeaderLines = 1; 
            summaryDisplayBand.Override.HeaderAppearance.BackColor  
                = Color.FromArgb( 140,140,140 );
            religionDisplayBand.ClearGroupByColumns(); 
             
            printLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            printLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
            
            if( this.SearchCriteria[1].ToString().Equals( GRIDCOL_RELIGION ) )
            {
                religionDisplayBand.Columns[3].Hidden = true;
                religionDisplayBand.Columns[ GRIDCOL_RELIGION ].GroupByMode = 
                    GroupByMode.Text;
                religionDisplayBand.SortedColumns.Add( GRIDCOL_RELIGION, false, true);
 
            }
            else
            {
                religionDisplayBand.Columns[ GRIDCOL_LOCATION ].GroupByMode = 
                    GroupByMode.Text;
                religionDisplayBand.Columns[ GRIDCOL_RELIGION ].GroupByMode = 
                    GroupByMode.Text;
                religionDisplayBand.SortedColumns.Add( GRIDCOL_LOCATION , false, true);
                religionDisplayBand.SortedColumns.Add( GRIDCOL_RELIGION , false, true); 
            }

            this.PrintGrid.InitializeGroupByRow += 
                new InitializeGroupByRowEventHandler( this.SetGroupAppearence );
            this.PrintGrid.DrawFilter = this;
            PrintGrid.Refresh(); 
        }

        private void SetColumnWidth()
        {
            searchDisplayBand.Columns[ GRIDCOL_SELECTED_RELIGION ].Width = 100 ;
            searchDisplayBand.Columns[ GRIDCOL_SORTBY ].Width            = 125 ;
            searchDisplayBand.Columns[ GRIDCOL_NO_OF_RECORDS ].Width     = 800 ;
            religionDisplayBand.Columns[ GRIDCOL_OPTOUT ].Width          = 60 ;
            religionDisplayBand.Columns[ GRIDCOL_PATIENT_NAME ].Width    = 977 ;
            accountDisplayBand.Columns[ GRIDCOL_AGE_INFO ].Width         = 160 ;
            accountDisplayBand.Columns[ GRIDCOL_LOCATION_INFO ].Width    = 180;
            accountDisplayBand.Columns[ GRIDCOL_DATE_LABEL ].Width       = 80 ;
            accountDisplayBand.Columns[ GRIDCOL_DATE_VALUE ].Width       = 175 ;
            accountDisplayBand.Columns[ GRIDCOL_WORSHIP_LABEL ].Width    = 90 ;
            lastDisplayBand.Columns[ GRIDCOL_LAST ].Width                = 1037 ;
            emptyDisplayBand.Columns[ GRIDCOL_EMPTY ].Width              = 1037 ;
            summaryDisplayBand.Columns[ GRIDCOL_TOTAL ].Width            = 100 ;
            summaryDisplayBand.Columns[ SUMMARYBAND_GRIDCOL_RELIGION ].Width = 150 ;
            summaryDisplayBand.Columns[ SUMMARYBAND_GRIDCOL_EMPTY ].Width = 770 ;

        }
        //TO set Grouped Column Appearance

        private void SetGroupAppearence(object sender, InitializeGroupByRowEventArgs e)
        {
            Appearance groupAppearance = new Appearance();
            
            if(e.Row.Column.Key == GRIDCOL_LOCATION )
            {
                groupAppearance.BackColor = Color.FromArgb(140,140,140);
             
            }
            else if( e.Row.Column.Key == GRIDCOL_RELIGION  )
            {                 
                groupAppearance.BackColor = Color.FromArgb(159,159,159);
            }
                  
               
            e.Row.Appearance = groupAppearance;
        }
        
        // To Populate Summary Information

        private void FillSummaryDataSource()
        {
                      
            int rowNumber = 0;  
                   
            // Populate Results Grid with UNSPECIFIED religion first
            foreach( Religion religion in SummaryInformation )
            {
                if( ( religion.Description == UNSPECIFIED_RELIGION ) )
                {
                   FillReligionProxyList( religion, rowNumber );
                }             
            }
            // Populate all other religions (specified)
            foreach( Religion religion in SummaryInformation )
            {
                if( ( religion.Description != "" ) && ( 
                    religion.Description.Trim().Length > 0 ) 
                    && ( religion.Description != UNSPECIFIED_RELIGION ) )
                {
                    FillReligionProxyList( religion, rowNumber );
                }            
            }                 
          rowNumber++;
        }
     
        // To add Summary row to Summary Band
        private void FillReligionProxyList ( Religion religion, int rowNumber )
        {

            object [] StatisticalSummaryInformation = new object[ SUMMARYBAND_NUMBER_OF_COLUMNS ];             
          
            religionSummaryRows = 
                dataSource.Rows[ dataSource.Rows.Count - 1 ].GetChildRows( summaryDataBand );
            
            StatisticalSummaryInformation[ SUMMARYBAND_GRIDCOL_RELIGION ] 
                =  religion.Description.Trim();
            StatisticalSummaryInformation[ SUMMARYBAND_GRIDCOL_TOTAL ] 
                = Convert.ToString( religion.TotalCount );             
            StatisticalSummaryInformation[ SUMMARYBAND_GRIDCOL_EMPTY ] = "" ;
            
            summaryRow = religionSummaryRows.Add( StatisticalSummaryInformation );
           
        }

        // To populate Patient Account Information

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            object [] religionAccountInformation;  
            string [] patientNameAndLocation;
            string  religion; 
            string placeofWorship;
            string clergyVisit;
            string isolation;
            string gender;
            string dischargeStatus;
            string location;
            string patientType;
            string age;
            string legend;
            string nursingStation;
            string prevReligion = String.Empty;
           
            
            ArrayList accountProxies = FilterAccountProxyList( this.Model );
          
            dataSource.Rows.Add( new string[] { RELIGION_CRITERIA_FORMAT,
                                                this.SearchCriteria[0].ToString(),
                                                String.Empty } );
            
            dataSource.Rows.Add( new string[] { SORTBY_FORMAT ,
                                                this.SearchCriteria[1].ToString(), 
                                                NO_RECORDS_FORMAT + accountProxies.Count } );                                                  
            religionRows = 
                dataSource.Rows[ dataSource.Rows.Count - 1 ].GetChildRows( religionDataBand );
 
            foreach( AccountProxy accountProxy in accountProxies )
            {                
                religionAccountInformation = new object[ ACCTBAND_NUMBER_OF_COLUMNS ];
                patientNameAndLocation = new string[ PAT_NUMBER_OF_COLUMNS ];
                placeofWorship         = String.Empty;
                clergyVisit            = String.Empty;
                age                    = String.Empty;
                gender                 = String.Empty;
                dischargeStatus        = String.Empty;
                location               = String.Empty;
                patientType            = String.Empty;
                isolation              = String.Empty;
                religion               = String.Empty;
                nursingStation         = String.Empty;
                legend                 = accountProxy.AddOnlyLegends();

                patientNameAndLocation[ PAT_GRIDCOL_OPTOUT ] = legend;

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);
                patientNameAndLocation[PAT_GRIDCOL_NAME] = patientNameFormatter.GetFormattedPatientName();

                if( accountProxy.Patient.Religion != null )
                {
                    if( !accountProxy.Patient.Religion.Code.Equals( "" ) )
                    {
                        if(  accountProxy.Patient.Religion.Description == UNSPECIFIED_RELIGION )
                        {
                            religion = " "+ CommonFormatting.ProperCase(
                                accountProxy.Patient.Religion.Description );
                        }
                        else
                        {
                            religion = CommonFormatting.ProperCase( 
                                accountProxy.Patient.Religion.Description );
                        }
                    }
                }
                
                if( !accountProxy.OptOutOnLocation )
                {
                    if( accountProxy.Location.ToString().Length == 0 )
                    {
                        nursingStation = accountProxy.Location.ToString();
                    }
                    else
                    {
                        nursingStation = accountProxy.Location.ToString().Substring( 0, 2 );
                    }
                }  

                religionRow =religionRows.Add(new string [] { religion, 
                                                                patientNameAndLocation[0],
                                                                patientNameAndLocation[1],
                                                                "Nursing Station: "+nursingStation } );
            
                accountRows = religionRow.GetChildRows( ACCOUNT_INFORMATION_BAND );
            
                if( accountProxy.Patient.Sex != null )
                {
                    gender = accountProxy.Patient.Sex.Code;
                }
            
                if( accountProxy.ClergyVisit != null )
                {
                    clergyVisit = accountProxy.ClergyVisit.Code;
                }

                if( accountProxy.DischargeStatus != null && 
                    accountProxy.DischargeStatus.Description.Equals( PENDING_DISCHARGE ) )
                {
                    dischargeStatus = MSG_PENDING_DISCHARGE;
                }
                else
                {
                    if( accountProxy.DischargeDate.Date.Equals( DateTime.MinValue )  )
                    {
                        dischargeStatus = String.Empty;
                    }
                    else
                    {
                        dischargeStatus = accountProxy.DischargeDate.Date.
                            ToString( DATE_FORMAT_MMDDYYYY );
                    }
                }
            
                if( accountProxy.KindOfVisit != null )
                {
                    patientType = accountProxy.KindOfVisit.Code 
                        + " " 
                        + accountProxy.KindOfVisit.Description;
                }

                if( accountProxy.ClergyVisit != null )
                {
                    clergyVisit = accountProxy.ClergyVisit.Code;                       
                }

                if( !accountProxy.OptOutOnLocation )
                {
                    location = accountProxy.Location.ToString();
                }                
            
                if( accountProxy.IsolationCode  != null )
                {
                    isolation=  accountProxy.IsolationCode ;
                }            
            
                religionAccountInformation[ ACCTBAND_GRIDCOL_ACCT_AGE_LABEL ] = 
                    String.Format( AGE_INFO_COL_FORMAT, patientType, gender,
                    accountProxy.Patient.AgeAt( 
                    DateTime.Today ).PadLeft( 4, '0').ToUpper() );

                religionAccountInformation[ ACCTBAND_GRIDCOL_ACCT_AGE ] = 
                    String.Format( LOCATION_INFO_COL_FORMAT, location,
                                    clergyVisit, isolation);
                                
                religionAccountInformation[ ACCTBAND_GRIDCOL_OTHER_LABEL ] = 
                    String.Format( DATE_INFO_COL_FORMAT );

                religionAccountInformation[ ACCTBAND_GRIDCOL_OTHER ] = String.Format( 
                    "{0}\n{1}",
                    accountProxy.AdmitDate.ToString( DATE_FORMAT_MMDDYYYY ),
                    dischargeStatus);

                religionAccountInformation[ ACCTBAND_GRIDCOL_WORSHIP_LABEL ] = 
                    String.Format( PLACE_OF_WORSHIP_LABEL );

                if( accountProxy.Patient.PlaceOfWorship  != null )
                {
                    religionAccountInformation[ ACCTBAND_GRIDCOL_WORSHIP ] = 
                        accountProxy.Patient.PlaceOfWorship.Description.Trim() ;
                }

                accountRow = accountRows.Add( religionAccountInformation );
            }
          
            emptyRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( EMPTY_BAND ) );
                
            for( int i = 0; i < 30; i++ )
            {
                emptyRow = emptyRows.Add( new string[] { String.Empty } );
            }                
      
        }

        private ArrayList FilterAccountProxyList( ArrayList allAccountProxies )
        {
            ArrayList filteredAccountProxies = new ArrayList();
            foreach( AccountProxy aProxy in allAccountProxies )
            {
                if( !( aProxy.OptOutOnName || aProxy.OptOutOnReligion ||
                    aProxy.OptOutOnLocation ||( aProxy.ClergyVisit.Code == "N" ) ) )
                {
                    filteredAccountProxies.Add( aProxy );
                }
            }
            return filteredAccountProxies;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements
     
        private UltraDataSource dataSource; 
        private UltraDataBand religionDataBand;
        private UltraDataBand accountInfoDataBand;
        private UltraDataBand lastDataBand;
        private UltraDataBand emptyDataBand;
        private UltraDataBand summaryDataBand;
        private UltraDataRowsCollection accountRows;
        private UltraDataRow accountRow;
        private UltraDataRow summaryRow;
        private UltraDataRowsCollection religionSummaryRows;
        private UltraDataRowsCollection religionRows;
        private UltraDataRow religionRow;
        private UltraDataRowsCollection emptyRows;
        private UltraDataRow emptyRow;
        private UltraGridBand searchDisplayBand ;
        private UltraGridBand religionDisplayBand ;
        private UltraGridBand accountDisplayBand ;
        private UltraGridBand lastDisplayBand ;
        private UltraGridBand emptyDisplayBand;
        private UltraGridBand summaryDisplayBand ;
        private UltraGridLayout printLayout;
         
        #endregion

        #region Constants
      

        private const string 

            MSG_PENDING_DISCHARGE = "Pending",
            PENDING_DISCHARGE = "PENDINGDISCHARGE",
            HEADER_TEXT = "Census by Religion";

        private const int 

            ACCTBAND_GRIDCOL_ACCT_AGE_LABEL = 0,
            ACCTBAND_GRIDCOL_ACCT_AGE = 1,                
            ACCTBAND_GRIDCOL_OTHER_LABEL = 2,
            ACCTBAND_GRIDCOL_OTHER = 3,
            ACCTBAND_GRIDCOL_WORSHIP_LABEL = 4,
            ACCTBAND_GRIDCOL_WORSHIP = 5,
            ACCTBAND_NUMBER_OF_COLUMNS = 6,

            PAT_GRIDCOL_OPTOUT = 0,
            PAT_GRIDCOL_NAME = 1,
            PAT_NUMBER_OF_COLUMNS = 2,
          
            SUMMARYBAND_GRIDCOL_RELIGION=0,
            SUMMARYBAND_GRIDCOL_TOTAL=1,
            SUMMARYBAND_GRIDCOL_EMPTY=2,
            SUMMARYBAND_NUMBER_OF_COLUMNS = 3;

        private const string 
            
            SEARCH_CRITERIA_BAND = "SearchCriteria_Band",
            RELIGION_BAND = "Religion_Band",
            ACCOUNT_INFORMATION_BAND = "AccountInformation_Band",
            LAST_BAND = "Last_Band",
            EMPTY_BAND = "Empty_Band",
            SUMMARY_BAND = "ReligionSummary_Band";
       
        private const string 
            
            GRIDCOL_SELECTED_RELIGION = "SelectedReligion",
            GRIDCOL_SORTBY = "Sortby",
            GRIDCOL_NO_OF_RECORDS = "No. of Records",
            GRIDCOL_RELIGION = "Religion",
            GRIDCOL_OPTOUT = "OptOutInfo",
            GRIDCOL_PATIENT_NAME = "PatientName",
            GRIDCOL_LOCATION = "Location",
            GRIDCOL_AGE_INFO = "AgeInfo",
            GRIDCOL_LOCATION_INFO = "LocationInfo",
            GRIDCOL_DATE_LABEL = "DateLabel",
            GRIDCOL_DATE_VALUE = "DateValue",
            GRIDCOL_WORSHIP_LABEL = "WorshipLabel",
            GRIDCOL_WORSHIP_VALUE = "WorshipValue",
            GRIDCOL_LAST = "LastBandCol",
            GRIDCOL_EMPTY = "EmptyBandCol",
            GRIDCOL_RELIGION_LABEL = "ReligionLabel",
            GRIDCOL_TOTAL = "Total",

            SUMMARY_BAND_HEADER_TEXT="Statistical Summary by Religion";

          private const string 
              AGE_INFO_COL_FORMAT = "PT: {0} \nGen: {1}   Age: {2}",
              LOCATION_INFO_COL_FORMAT = " Location: {0} \n CLV: {1}     Isol:  {2}",
              DATE_INFO_COL_FORMAT = "Admit date:\nDisch Status:" ,
              PLACE_OF_WORSHIP_LABEL = "Place of worship:",
              UNSPECIFIED_RELIGION = "UNSPECIFIED",           
              RELIGION_CRITERIA_FORMAT = "Religion:  ",
              SORTBY_FORMAT = "Sorted by: " ,
              NO_RECORDS_FORMAT = "No. of records: ",
              DATE_FORMAT_MMDDYYYY = "MM/dd/yyyy",
              CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";
            
        #endregion        
    }
}
