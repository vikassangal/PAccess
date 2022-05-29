using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for NursingStationReport.
    /// </summary>
    public abstract class CensusByNursingStationReport : PrintReport
    {
        #region Events
        #endregion

        #region EventHandlers
        #endregion

        #region Methods
        
        public void PrintPreview()
        {
            FillDataSource();            
            FillNursingStationSummary();
            base.DataSource = dataSource;
            base.UpdateView();
            base.HeaderText = HEADER_TEXT;
            base.FooterText = UILabels.CENSUS_OPT_OUT_LEGENDS;
        }

        public virtual void CustomizeGridLayout()
        {
            searchCriteriaBand  = PrintGrid.DisplayLayout.Bands[SEARCHCRITERIA_BAND];
            patientBand         = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            emptyBand           = PrintGrid.DisplayLayout.Bands[EMPTY_BAND];
            summaryBand         = PrintGrid.DisplayLayout.Bands[SUMMARY_BAND];
            lastBand            = PrintGrid.DisplayLayout.Bands[LAST_BAND];
            
            PrintGrid.DisplayLayout.Override.HeaderStyle = HeaderStyle.XPThemed;
            //searchCriteriaBand properties
            searchCriteriaBand.ColHeadersVisible = false;
            searchCriteriaBand.Indentation = 0;
            searchCriteriaBand.Override = base.OverrideWithoutBorder;

            searchCriteriaBand.Columns[SEARCHBAND_GRIDCOL_SRCHCRITERIA].CellMultiLine = 
                DefaultableBoolean.True;
            searchCriteriaBand.Columns[SEARCHBAND_GRIDCOL_RECORDS].CellMultiLine = 
                DefaultableBoolean.True;            
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;

            // to set patientBand properties
            SetPatientBandProperties();

            // lastBand properties
            lastBand.ColHeadersVisible = false;          
            lastBand.Indentation = 0;  
            lastBand.Override = base.OverrideWithBorder;

            // to set summaryBand properties
            SetSummaryBandProperties();

            //emptyBand properties
            emptyBand.ColHeadersVisible = false;
            emptyBand.Override = base.OverrideWithoutBorder;

            // to set column widths
            SetColumnWidths();
            PrintGrid.Refresh();
        }

        public abstract void FillDataSource();
        #endregion

        #region Properties
        public new ArrayList Model
        {
            get
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
            get
            {
                return base.SearchCriteria as ArrayList;
            }
            set
            {
                base.SearchCriteria = value;
            }
        }

        public new IList<NursingStation> SummaryInformation
        {
            private get
            {
                return base.SummaryInformation as IList<NursingStation>;
            }
            set
            {
                base.SummaryInformation = value;
            }
        }

        #endregion
       
        #region Construction and finalization
        public CensusByNursingStationReport()
        {			
            CreateDataStructure();           
        }
        #endregion

        #region Private Methods
        
        private void SetPatientBandProperties()
        {
            patientBand.ColHeadersVisible = false;
            patientBand.GroupHeadersVisible = true;
            patientBand.Indentation = 0;
            
            patientBand.Columns[GRIDCOL_PATIENT_PATIENT_NAME].CellMultiLine = DefaultableBoolean.True;
            patientBand.Columns[GRIDCOL_PORTAL_OPTIN].Width = 70;
//            patientBand.Columns[GRIDCOL_PATIENT_GEN_AGE_LABEL].CellMultiLine = DefaultableBoolean.True;
//            patientBand.Columns[GRIDCOL_PATIENT_GEN_AGE].CellMultiLine = DefaultableBoolean.True;

            patientBand.Override.BorderStyleCell = UIElementBorderStyle.Solid;
            patientBand.Override.BorderStyleRow = UIElementBorderStyle.Solid;
            patientBand.Override.HeaderAppearance = base.BandHeaderRowAppearance;            
        }

        // grid lines( vertical band lines) not visible -- TBD
        private void SetSummaryBandProperties()
        {
            summaryBand.Header.Caption = STATISTICS_HEADER;
            summaryBand.HeaderVisible = true;            
            summaryBand.Header.Appearance = base.SummaryHeaderAppearance;
            
            summaryBand.ColHeadersVisible = true; 
            summaryBand.Indentation = 14;
            
            summaryBand.Override.BorderStyleRow = UIElementBorderStyle.None;
            summaryBand.Override.RowAppearance.BorderColor = Row_Separator_Color;
            summaryBand.Override.BorderStyleCell = UIElementBorderStyle.None;
            summaryBand.Override.CellAppearance.BorderColor = Border_Color;
            summaryBand.Override.HeaderAppearance = base.BandHeaderRowAppearance;
            summaryBand.Override.HeaderAppearance.TextHAlign = HAlign.Center;

            summaryBand.Override.CellAppearance.TextHAlign = HAlign.Left;
            summaryBand.Columns[GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS].CellAppearance.TextHAlign = HAlign.Right;
             summaryBand.Columns[GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH].CellAppearance.TextHAlign = HAlign.Right;
        }
        
        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[SEARCHBAND_GRIDCOL_SRCHCRITERIA].Width = 812;
            searchCriteriaBand.Columns[SEARCHBAND_GRIDCOL_RECORDS].Width = 225;

            emptyBand.Columns[GRIDCOL_EMPTYBAND_EMPTYCOLUMN].Width = 1000;

            summaryBand.Columns[ GRIDCOL_NURSING_STATION ].Width = 30;
            summaryBand.Columns[ GRIDCOL_PREVIOUS_CENSUS ].Width = 80;
            summaryBand.Columns[ GRIDCOL_ADMIT_TODAY ].Width = 80;
            summaryBand.Columns[ GRIDCOL_DISCHARGE_TODAY ].Width = 80;
            summaryBand.Columns[ GRIDCOL_DEATHS_TODAY ].Width = 90;
            summaryBand.Columns[ GRIDCOL_TRANSFER_FROM ].Width = 70;
            summaryBand.Columns[ GRIDCOL_TRANSFER_TO ].Width = 55;
            summaryBand.Columns[ GRIDCOL_AVAILABLE_BEDS ].Width = 72;
            summaryBand.Columns[ GRIDCOL_CURRENT_CENSUS ].Width = 80;
            summaryBand.Columns[ GRIDCOL_TOTAL_BEDS ].Width = 75;
            summaryBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ].Width = 60;
            summaryBand.Columns[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ].Width = 75;
            summaryBand.Columns[ GRIDCOL_TOTAL_BEDS_MONTH ].Width = 75;
            summaryBand.Columns[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ].Width = 75;

            lastBand.Columns[LASTBAND_COL].Width = 1037;
        }

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            UltraDataBand searchBand    = this.dataSource.Band;
            UltraDataBand patBand       = searchBand.ChildBands.Add( PATIENT_BAND );
            UltraDataBand emptyBand     = searchBand.ChildBands.Add( EMPTY_BAND );
            UltraDataBand summBand      = searchBand.ChildBands.Add( SUMMARY_BAND );
            UltraDataBand lastBand      = searchBand.ChildBands.Add( LAST_BAND );

            searchBand.Key = SEARCHCRITERIA_BAND;          
            searchBand.Columns.Add( SEARCHBAND_GRIDCOL_SRCHCRITERIA );
            searchBand.Columns.Add( SEARCHBAND_GRIDCOL_RECORDS );

            patBand.Columns.Add( GRIDCOL_ACCT_PRVCY );
            patBand.Columns.Add( GRIDCOL_PATIENT_LOCATION );
            patBand.Columns.Add( GRIDCOL_PATIENT_PATIENT_NAME );
            patBand.Columns.Add( GRIDCOL_PORTAL_OPTIN );
//            patBand.Columns.Add( GRIDCOL_PATIENT_GEN_AGE_LABEL );
//            patBand.Columns.Add( GRIDCOL_PATIENT_GEN_AGE );
            patBand.Columns.Add( GRIDCOL_PATIENT_OVERFLOW );
            patBand.Columns.Add( GRIDCOL_PATIENT_ROOMCONDITION );
            patBand.Columns.Add( GRIDCOL_ISOLATION );
            patBand.Columns.Add( GRIDCOL_PATIENT_ACCOMMODATION );
            patBand.Columns.Add(GRIDCOL_HSV_CODE);
            patBand.Columns.Add( GRIDCOL_UNOC_PATIENT_TYPE );
            patBand.Columns.Add( GRIDCOL_PATIENT_ATTNPHYSICIAN );            
            
            emptyBand.Columns.Add( GRIDCOL_EMPTYBAND_EMPTYCOLUMN );

            summBand.Columns.Add( GRIDCOL_NURSING_STATION );
            summBand.Columns.Add( GRIDCOL_PREVIOUS_CENSUS );
            summBand.Columns.Add( GRIDCOL_ADMIT_TODAY );
            summBand.Columns.Add( GRIDCOL_DISCHARGE_TODAY );
            summBand.Columns.Add( GRIDCOL_DEATHS_TODAY );
            summBand.Columns.Add( GRIDCOL_TRANSFER_FROM );
            summBand.Columns.Add( GRIDCOL_TRANSFER_TO );
            summBand.Columns.Add( GRIDCOL_AVAILABLE_BEDS );
            summBand.Columns.Add( GRIDCOL_CURRENT_CENSUS );
            summBand.Columns.Add( GRIDCOL_TOTAL_BEDS );
            summBand.Columns.Add( GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS );
            summBand.Columns.Add( GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH );
            summBand.Columns.Add( GRIDCOL_TOTAL_BEDS_MONTH );
            summBand.Columns.Add( GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH );            

            lastBand.Columns.Add( LASTBAND_COL );
        }
        
        private void FillNursingStationSummary()
        {
            string attendingPhysicianName = String.Empty;           
            int percentOccupiedBeds;
            int percentOccupiedBedsForMonth;
            UltraDataRow summaryRow;
            UltraDataRowsCollection summaryRows;
            UltraDataRowsCollection emptyRows;
            UltraDataRow emptyRow;

            emptyRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( EMPTY_BAND ) );
            
            for( int i = 0; i < 10; i++ )
            {
                emptyRow = emptyRows.Add( new string[] { String.Empty } );
            }

            if( SearchCriteria[NS_REPORT_TYPE].Equals( NS_INFO_DESK_KEY )  )
            {
                dataSource.Rows.Add( new string[] { SUMMARY_HEADER } );
            }

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey(SUMMARY_BAND ) );                

            foreach( NursingStation nursingStation in this.SummaryInformation )
            {
                if( !nursingStation.Code.Trim().
                    Equals( ALL_NURSINGSTATION_CODE ) )
                { 
                    summaryRow = summaryRows.Add();

                    percentOccupiedBeds = 0;
                    percentOccupiedBedsForMonth = 0;               
                    summaryRow[ GRIDCOL_NURSING_STATION ] = nursingStation.Code;
                    summaryRow[ GRIDCOL_PREVIOUS_CENSUS ] = nursingStation.PreviousCensus;
                    summaryRow[ GRIDCOL_ADMIT_TODAY ] = nursingStation.AdmitToday;
                    summaryRow[ GRIDCOL_DISCHARGE_TODAY ] = nursingStation.DischargeToday;
                    summaryRow[ GRIDCOL_DEATHS_TODAY ] = nursingStation.DeathsToday;
                    summaryRow[ GRIDCOL_TRANSFER_FROM ] = 
                        nursingStation.TransferredFromToday;
                    summaryRow[ GRIDCOL_TRANSFER_TO ] = nursingStation.TransferredToToday;
                    summaryRow[ GRIDCOL_AVAILABLE_BEDS ] = nursingStation.AvailableBeds;
                    summaryRow[ GRIDCOL_CURRENT_CENSUS ] = nursingStation.CurrentCensus;
                    summaryRow[ GRIDCOL_TOTAL_BEDS ] = nursingStation.TotalBeds;

                    if( nursingStation.TotalBeds > 0 )
                    {
                        percentOccupiedBeds = (int) Math.Round( ( (decimal) 
                            nursingStation.CurrentCensus * 100 / 
                            (decimal) nursingStation.TotalBeds ) );
                        summaryRow[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] 
                            = percentOccupiedBeds + "%";
                    }
                    else
                    {
                        summaryRow[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS ] = "0%";
                    }
            
                    if( nursingStation.TotalBedsForMonth > 0 )
                    {
                        percentOccupiedBedsForMonth = (int)Math.Round( ( (decimal) 
                            nursingStation.TotalOccupiedBedsForMonth * 100
                            / (decimal) nursingStation.TotalBedsForMonth ) );
                        summaryRow[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] 
                            = percentOccupiedBedsForMonth + "%";
                    }
                    else
                    {
                        summaryRow[ GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH ] 
                            = "0%";
                    }
                    summaryRow[ GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH ] 
                        = nursingStation.TotalOccupiedBedsForMonth;
                    summaryRow[ GRIDCOL_TOTAL_BEDS_MONTH ] 
                        = nursingStation.TotalBedsForMonth;              
                }
            }
        }
        
        
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        private UltraGridBand emptyBand;
        private UltraGridBand summaryBand;
        private UltraGridBand lastBand;        
        protected UltraDataSource dataSource;
 
        private Color cellBorderColor = Color.FromArgb( 140, 140, 140 );
       
        #endregion

        #region Constants
        protected const string PENDING_ADMISSION = "PENDING ADMISSION";        

        protected const string 
            GRIDCOL_ACCT_PRVCY = "Prvcy Opt",
            GRIDCOL_PATIENT_LOCATION = "Location",
            GRIDCOL_PATIENT_PATIENT_NAME = "Patient",
            GRIDCOL_PORTAL_OPTIN = "Portal Opt In",
//            GRIDCOL_PATIENT_GEN_AGE_LABEL = "gender_age_label",
//            GRIDCOL_PATIENT_GEN_AGE = "gender_age",
            GRIDCOL_PATIENT_OVERFLOW = "Ovfl",
            GRIDCOL_PATIENT_ROOMCONDITION = "RC",           
            GRIDCOL_ISOLATION             = "Isol",
            GRIDCOL_PATIENT_ACCOMMODATION = "Accom",
            GRIDCOL_HSV_CODE = "HSV Code",
            GRIDCOL_PATIENT_ATTNPHYSICIAN = "Attending Physician",
            GRIDCOL_UNOC_PATIENT_TYPE = "Patient Type",
            SEARCHCRITERIA_BAND = "SearchCriteria_Band",
            PATIENT_BAND = "Patient_Band",
            ACCOUNT_INFORMATION_BAND = "AccountInformation_Band";

        private const string 
            LAST_BAND = "Last_Band",
            EMPTY_BAND = "Empty_Band",
            SUMMARY_BAND = "Summary_Band";

        private const string 
            SUMMARY_HEADER      = "               Information Desk ",
            STATISTICS_HEADER   = "Statistical Summary by Nursing Station (Inpatients only) ___";
        private const string HEADER_TEXT = "Census by Nursing Station";
        private const int
            SEARCHBAND_RECORDS = 1;
        private const string
            LASTBAND_COL = "TestInfo",
            GRIDCOL_EMPTYBAND_EMPTYCOLUMN = "EmptyColumn";
           
        private const string GRIDCOL_NURSING_STATION = "NS",
            GRIDCOL_PREVIOUS_CENSUS = "Prev Census",
            GRIDCOL_ADMIT_TODAY = "Admit Today",
            GRIDCOL_DISCHARGE_TODAY = "Disch Today",
            GRIDCOL_DEATHS_TODAY = "Deaths Today",
            GRIDCOL_TRANSFER_FROM = "Trfr From",
            GRIDCOL_TRANSFER_TO = "Trfr To",
            GRIDCOL_AVAILABLE_BEDS = "Avl Beds",
            GRIDCOL_CURRENT_CENSUS = "Curr Census",
            GRIDCOL_TOTAL_BEDS = "Total Beds",
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS = "% Occ",
            GRIDCOL_TOTAL_OCCUPIED_BEDS_MONTH = "Mo Pt Days",
            GRIDCOL_TOTAL_BEDS_MONTH = "Mo Beds Avl",
            GRIDCOL_PERCENTAGE_OF_OCCUPIED_BEDS_MONTH = "Mo % Occ";

        private const string SEARCHBAND_GRIDCOL_SRCHCRITERIA = "Nursing-NS-SortedBy",
            SEARCHBAND_GRIDCOL_RECORDS = "No.OfRecords";

        private const string 
            ALL_NURSINGSTATION_CODE = "$$",
            NS_INFO_DESK_KEY        = "Information Desk";

        private const int NS_REPORT_TYPE = 2;
       
        #endregion        
    }
}
