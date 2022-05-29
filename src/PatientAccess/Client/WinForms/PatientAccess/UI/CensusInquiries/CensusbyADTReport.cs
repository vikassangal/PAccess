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
	/// Summary description for CensusbyADTReport.
	/// </summary>
	public abstract class CensusbyADTReport : PrintReport
	{

        #region Events
        #endregion

        #region Event Handlers
        #endregion
      
        #region Methods

        public void PrintPreview()
        {
            this.FillDataSource();
            base.DataSource = dataSource;
            base.UpdateView();
            base.HeaderText = HEADER_TEXT;
            base.FooterText = UILabels.CENSUS_OPT_OUT_LEGENDS;
        }

        public virtual void CustomizeGridLayout()
        {
            searchCriteriaBand = PrintGrid.DisplayLayout.Bands[SEARCH_CRITERIA_BAND];
            patientBand        = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            emptyBand          = PrintGrid.DisplayLayout.Bands[EMPTY_BAND];
            summaryHeaderBand  = PrintGrid.DisplayLayout.Bands[SUMMARY_HEADER_BAND];
            summaryBand        = PrintGrid.DisplayLayout.Bands[SUMMARY_BAND];
            
            searchCriteriaBand.Indentation              = 0;
            searchCriteriaBand.ColHeadersVisible        = false;
            searchCriteriaBand.Override.BorderStyleCell = UIElementBorderStyle.None;
            searchCriteriaBand.Override.BorderStyleRow  = UIElementBorderStyle.None;
            searchCriteriaBand.Override.RowAppearance.BorderColor = SystemColors.Window;
            searchCriteriaBand.Override.RowAppearance.BorderAlpha = Alpha.Transparent;
            searchCriteriaBand.Columns[COL_TOTAL_RECORDS].CellAppearance.TextHAlign = HAlign.Right;

            patientBand.Indentation         = 0;
            patientBand.ColHeadersVisible   = true;
            patientBand.HeaderVisible       = true;
            patientBand.Header.Appearance   = BandHeaderRowAppearance;
            patientBand.Header.Appearance.TextHAlign         = HAlign.Left;
            patientBand.Override.HeaderAppearance.TextHAlign = HAlign.Left;
            patientBand.Override.HeaderAppearance.BackColor  = Color.FromArgb( 140, 140, 140 );

            emptyBand.Indentation              = 0;
            emptyBand.ColHeadersVisible        = false;
            emptyBand.Override.BorderStyleCell = UIElementBorderStyle.None;
            emptyBand.Override.BorderStyleRow  = UIElementBorderStyle.None;
            emptyBand.Override.RowAppearance.BorderColor = SystemColors.Window;
            emptyBand.Override.RowAppearance.BorderAlpha = Alpha.Transparent;

            summaryHeaderBand.Indentation       = 0;
            summaryHeaderBand.ColHeadersVisible = false;
            summaryHeaderBand.HeaderVisible     = true;
            summaryHeaderBand.Header.Appearance = BandHeaderRowAppearance;
            summaryHeaderBand.Header.Appearance.TextHAlign = HAlign.Left;
            summaryHeaderBand.Override.BorderStyleCell     = UIElementBorderStyle.None;
            summaryHeaderBand.Override.BorderStyleRow      = UIElementBorderStyle.None;
            summaryHeaderBand.Override.RowAppearance.BorderColor = SystemColors.Window;
            summaryHeaderBand.Override.RowAppearance.BorderAlpha = Alpha.Transparent;
            //summaryHeaderBand.Override.RowAppearance.FontData.SizeInPoints = 0.1F;

            summaryBand.Indentation       = 14;  
            summaryBand.ColHeadersVisible = true;
            summaryBand.HeaderVisible     = true;
            summaryBand.Header.Appearance.ForeColor   = Color.Black;
            summaryBand.Header.Appearance.BackColor   = Color.White;
            summaryBand.Header.Appearance.BorderColor = SystemColors.Window;
            summaryBand.Header.Appearance.TextHAlign  = HAlign.Left;
            summaryBand.Override.HeaderAppearance.BackColor  = Color.FromArgb( 140, 140, 140 );
            summaryBand.Override.HeaderAppearance.TextHAlign = HAlign.Left;
            summaryBand.Override.RowAppearance.TextHAlign    = HAlign.Left;
            summaryBand.Columns[COL_PERCENT_OCCUPANCY].CellAppearance.TextHAlign = HAlign.Right;
            summaryBand.Columns[COL_MO_PERCENT_OCCUPANCY].CellAppearance.TextHAlign = HAlign.Right;
            summaryBand.Header.Caption = SUMMARY_BAND_CAPTION;
            
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].CellMultiLine         = DefaultableBoolean.True;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT_TYPE].CellMultiLine    = DefaultableBoolean.True;
            patientBand.Columns[COL_LOCATION_FROM_TO].CellMultiLine             = DefaultableBoolean.True;
            patientBand.Columns[COL_PHYSICIANS].CellMultiLine                   = DefaultableBoolean.True;
            patientBand.Columns[COL_PHYSICIANS_CHIEF_COMPLAINT].CellMultiLine   = DefaultableBoolean.True;
            patientBand.Columns[COL_PHYSICIANS_DISCH_DISPOSITION].CellMultiLine = DefaultableBoolean.True;

            PrintGrid.Width = 1025;

            SetColumnsWidth();
            SetSortColumns();
            HideAllColumns();
            
            PrintGrid.Refresh();
            PrintGrid.DisplayLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
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

        public new CensusADTSearchCriteria SearchCriteria
        {
            private get
            {
                return base.SearchCriteria as CensusADTSearchCriteria;
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

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            dataSource.Band.Key       = SEARCH_CRITERIA_BAND;
            UltraDataBand patientBand = dataSource.Band.ChildBands.Add( PATIENT_BAND );
            UltraDataBand emptyBand   = dataSource.Band.ChildBands.Add( EMPTY_BAND );
            UltraDataBand summaryHeaderBand = dataSource.Band.ChildBands.Add( SUMMARY_HEADER_BAND );
            UltraDataBand summaryBand = dataSource.Band.ChildBands.Add( SUMMARY_BAND );

            dataSource.Band.Columns.Add( COL_SEARCH_CRITERIA_LABEL );
            dataSource.Band.Columns.Add( COL_SEARCH_CRITERIA_VALUE );
            dataSource.Band.Columns.Add( COL_TOTAL_RECORDS );             

            patientBand.Columns.Add( COL_CONFIDENTIAL );
            patientBand.Columns.Add( COL_ADT_TYPE );
            patientBand.Columns.Add( COL_TRANSACTION_TIME );
            patientBand.Columns.Add( COL_PATIENT_NAME_ACCOUNT );
            patientBand.Columns.Add( COL_PATIENT_NAME_ACCOUNT_TYPE );
            patientBand.Columns.Add( COL_PATIENT_TYPE );
            patientBand.Columns.Add( COL_LOCATION );
            patientBand.Columns.Add( COL_LOCATION_FROM_TO );
            patientBand.Columns.Add( COL_PHYSICIANS_CHIEF_COMPLAINT );
            patientBand.Columns.Add( COL_PHYSICIANS_DISCH_DISPOSITION );
            patientBand.Columns.Add( COL_PHYSICIANS );

            emptyBand.Columns.Add( COL_EMPTY );
            
            summaryHeaderBand.Columns.Add( COL_SUMMARY_HEADER );
                        
            summaryBand.Columns.Add( COL_NURSING_STATION );
            summaryBand.Columns.Add( COL_PREVIOUS_CENSUS );
            summaryBand.Columns.Add( COL_ADMIT_TODAY );
            summaryBand.Columns.Add( COL_DISCH_TODAY );
            summaryBand.Columns.Add( COL_DEATHS_TODAY );
            summaryBand.Columns.Add( COL_TRANSFERS_TODAY );
            summaryBand.Columns.Add( COL_AVAILABLE_BEDS );
            summaryBand.Columns.Add( COL_CURRENT_CENSUS );
            summaryBand.Columns.Add( COL_TOTAL_BEDS );
            summaryBand.Columns.Add( COL_PERCENT_OCCUPANCY );
            summaryBand.Columns.Add( COL_MO_PATIENT_DAYS );
            summaryBand.Columns.Add( COL_MO_BEDS_AVAILABLE );
            summaryBand.Columns.Add( COL_MO_PERCENT_OCCUPANCY );
        }

        private void SetColumnsWidth()
        {
            searchCriteriaBand.Columns[COL_SEARCH_CRITERIA_LABEL].Width = 140;
            searchCriteriaBand.Columns[COL_SEARCH_CRITERIA_VALUE].Width = 760;
            searchCriteriaBand.Columns[COL_TOTAL_RECORDS].Width         = 120;

            patientBand.Columns[COL_CONFIDENTIAL].Width                 = 100;
            patientBand.Columns[COL_ADT_TYPE].Width                     = 95;
            patientBand.Columns[COL_TRANSACTION_TIME].Width             = 60;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].Width         = 250;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT_TYPE].Width    = 250;
            patientBand.Columns[COL_PATIENT_TYPE].Width                 = 95;
            patientBand.Columns[COL_LOCATION].Width                     = 150;
            patientBand.Columns[COL_LOCATION_FROM_TO].Width             = 170;
            patientBand.Columns[COL_PHYSICIANS_CHIEF_COMPLAINT].Width   = 465;
            patientBand.Columns[COL_PHYSICIANS_DISCH_DISPOSITION].Width = 465;
            patientBand.Columns[COL_PHYSICIANS].Width                   = 350;

            emptyBand.Columns[COL_EMPTY].Width                  = 1025;

            summaryHeaderBand.Columns[COL_SUMMARY_HEADER].Width = 1025;

            summaryBand.Columns[COL_NURSING_STATION].Width      = 35;
            summaryBand.Columns[COL_PREVIOUS_CENSUS].Width      = 110;
            summaryBand.Columns[COL_ADMIT_TODAY].Width          = 80;
            summaryBand.Columns[COL_DISCH_TODAY].Width          = 80;
            summaryBand.Columns[COL_DEATHS_TODAY].Width         = 100;
            summaryBand.Columns[COL_TRANSFERS_TODAY].Width      = 80;
            summaryBand.Columns[COL_AVAILABLE_BEDS].Width       = 70;
            summaryBand.Columns[COL_CURRENT_CENSUS].Width       = 80;
            summaryBand.Columns[COL_TOTAL_BEDS].Width           = 70;
            summaryBand.Columns[COL_PERCENT_OCCUPANCY].Width    = 70;
            summaryBand.Columns[COL_MO_PATIENT_DAYS].Width      = 80;
            summaryBand.Columns[COL_MO_BEDS_AVAILABLE].Width    = 80;
            summaryBand.Columns[COL_MO_PERCENT_OCCUPANCY].Width = 75;
        }

        private void SetSortColumns()
        {
            if( SearchCriteria.ADTSortColumn == ADT_SORTBY_TIME )
            {
                patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].SortIndicator = SortIndicator.None;
                patientBand.Columns[COL_TRANSACTION_TIME].SortIndicator     = SortIndicator.Ascending;
            }
            
            else if( SearchCriteria.ADTSortColumn == ADT_SORTBY_PATIENT )
            {
                patientBand.Columns[COL_TRANSACTION_TIME].SortIndicator     = SortIndicator.None;
                patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].SortIndicator = SortIndicator.Ascending;
            }
        }

        private void HideAllColumns()
        {
            patientBand.Columns[COL_CONFIDENTIAL].Hidden                 = true;
            patientBand.Columns[COL_ADT_TYPE].Hidden                     = true;
            patientBand.Columns[COL_TRANSACTION_TIME].Hidden             = true;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT].Hidden         = true;
            patientBand.Columns[COL_PATIENT_NAME_ACCOUNT_TYPE].Hidden    = true;
            patientBand.Columns[COL_PATIENT_TYPE].Hidden                 = true;
            patientBand.Columns[COL_LOCATION].Hidden                     = true;
            patientBand.Columns[COL_LOCATION_FROM_TO].Hidden             = true;
            patientBand.Columns[COL_PHYSICIANS_CHIEF_COMPLAINT].Hidden   = true;
            patientBand.Columns[COL_PHYSICIANS_DISCH_DISPOSITION].Hidden = true;
            patientBand.Columns[COL_PHYSICIANS].Hidden                   = true;
        }

        private void FillDataSource()
        {
            string selectedNursingStations = SearchCriteria.NursingStations;
            allAccountProxies = this.Model;

            dataSource.Rows.Clear();

            selectedNursingStations = selectedNursingStations.Replace( "'", "" );
            selectedNursingStations = selectedNursingStations.Replace( ",", ", " );
            selectedNursingStations = 
                selectedNursingStations.Replace( ALL_NURSINGSTATIONS_UPPER, ALL_NURSINGSTATIONS_PROPER );

            ADTReportRow = dataSource.Rows.Add();
            ADTReportRow[COL_SEARCH_CRITERIA_LABEL] = "Nursing station:";
            ADTReportRow[COL_SEARCH_CRITERIA_VALUE] = selectedNursingStations;
            ADTReportRow[COL_TOTAL_RECORDS]         = String.Empty;
            
            ADTReportRow = dataSource.Rows.Add();
            ADTReportRow[COL_SEARCH_CRITERIA_LABEL] = "Sorted by:";
            ADTReportRow[COL_SEARCH_CRITERIA_VALUE] = SearchCriteria.ADTSortColumn;
            ADTReportRow[COL_TOTAL_RECORDS]         =  String.Empty;
                
            
            ADTReportRow = dataSource.Rows.Add();
            ADTReportRow[COL_SEARCH_CRITERIA_LABEL] = "Show today's starting at:";
            ADTReportRow[COL_SEARCH_CRITERIA_VALUE] = SearchCriteria.StartTime;
            ADTReportRow[COL_TOTAL_RECORDS]         = 
                "No. of records: " + allAccountProxies.Count;

            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                            dataSource.GetBandByKey( PATIENT_BAND ) );

            PopulatePatientData();

            emptyRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                            dataSource.GetBandByKey( EMPTY_BAND ) );
            
            //Infragistics print can't start printing Nursing Station Statistics in a new page.
            //So, adding some space between ADT results and Nursing Station Statistics.
            for( int i = 0; i < 5; i++ )
            {
                ADTReportRow = emptyRows.Add();
                ADTReportRow[COL_EMPTY] = String.Empty;
            } 

            summaryHeaderRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( SUMMARY_HEADER_BAND ) );
            
            ADTReportRow = summaryHeaderRows.Add();
            ADTReportRow[COL_SUMMARY_HEADER] = PAD_TEXT;

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( SUMMARY_BAND ) );

            PopulateNursingStationStatisticsData();
        }

        private void PopulatePatientData()
        {
            string admittingPhysician   = String.Empty;
            string attendingPhysician   = String.Empty;
            string physicians           = String.Empty; 
            string patientType          = String.Empty;
            string patientName          = String.Empty;
           
            foreach( AccountProxy accountProxy in allAccountProxies )
            {        
                ADTReportRow = patientRows.Add();

                admittingPhysician = "Admitting:     ";
                attendingPhysician = "Attending:     ";
                patientType        = String.Empty; 
                patientName        = String.Empty;
               
                if( accountProxy.AdmittingPhysician != null )
                {
                    admittingPhysician += accountProxy.AdmittingPhysician.FormattedName;
                }
                
                if( accountProxy.AttendingPhysician != null )
                {
                    attendingPhysician += accountProxy.AttendingPhysician.FormattedName;
                }
                physicians = admittingPhysician + "\n" + attendingPhysician;

                if( accountProxy.KindOfVisit != null )
                {
                    patientType = accountProxy.KindOfVisit.ToCodedString();
                }

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);
                patientName = patientNameFormatter.GetFormattedPatientName();

                ADTReportRow[COL_CONFIDENTIAL] = accountProxy.AddOnlyLegends();
                ADTReportRow[COL_ADT_TYPE]     = accountProxy.TransactionType;
                
                ADTReportRow[COL_TRANSACTION_TIME] = 
                    accountProxy.TransactionTime;
                
                ADTReportRow[COL_PATIENT_NAME_ACCOUNT] = 
                    patientName + 
                    "\n     Account:  " + accountProxy.AccountNumber;
                
                ADTReportRow[COL_PATIENT_NAME_ACCOUNT_TYPE] = 
                    patientName + 
                    "\n     Account:  " + accountProxy.AccountNumber +
                    "\n     PT:         " + patientType;

                ADTReportRow[COL_PATIENT_TYPE] = patientType;
                
                if( accountProxy.Location != null )
                {
                    ADTReportRow[COL_LOCATION] = accountProxy.Location.ToString();
                }
                
                else
                {
                    ADTReportRow[COL_LOCATION] = String.Empty;
                }
                
                if( accountProxy.LocationFrom != null )
                {
                    ADTReportRow[COL_LOCATION_FROM_TO] = 
                        "From: " + accountProxy.LocationFrom;
                }
                
                else
                {
                    ADTReportRow[COL_LOCATION_FROM_TO] = "From: ";

                }
                
                if( accountProxy.LocationTo != null )
                {
                    ADTReportRow[COL_LOCATION_FROM_TO] =
                        ADTReportRow[COL_LOCATION_FROM_TO]+
                        "\nTo:    " + accountProxy.LocationTo;
                }
                
                else
                {
                    ADTReportRow[COL_LOCATION_FROM_TO] =
                        ADTReportRow[COL_LOCATION_FROM_TO] +
                        "\nTo:   ";
                }

                ADTReportRow[COL_PHYSICIANS_CHIEF_COMPLAINT] = physicians;
                
                if( accountProxy.Diagnosis != null )
                {
                    ADTReportRow[COL_PHYSICIANS_CHIEF_COMPLAINT] = 
                        ADTReportRow[COL_PHYSICIANS_CHIEF_COMPLAINT] +
                        "\nChf complnt:  " + accountProxy.Diagnosis.ChiefComplaint;
                }
                
                else
                {
                    ADTReportRow[COL_PHYSICIANS_CHIEF_COMPLAINT] = 
                        ADTReportRow[COL_PHYSICIANS_CHIEF_COMPLAINT] +
                        "\nChf complnt:  ";
                }
 
                ADTReportRow[COL_PHYSICIANS_DISCH_DISPOSITION] = physicians;
                
                if( accountProxy.DischargeDisposition != null  && ( accountProxy.TransactionType == "E" || accountProxy.TransactionType == "D" ) )
                {
                    ADTReportRow[COL_PHYSICIANS_DISCH_DISPOSITION] =
                        ADTReportRow[COL_PHYSICIANS_DISCH_DISPOSITION] +
                        "\nDisch Disp:    " + accountProxy.DischargeDisposition.ToCodedString();
                }

                ADTReportRow[COL_PHYSICIANS] = physicians;
            }
        }

        private void PopulateNursingStationStatisticsData()
        {
            int percentOccupancy        = 0;
            int percentMonthlyOccupancy = 0;

            foreach( NursingStation nursingStation in SummaryInformation )
            {
                if( nursingStation == null )
                {
                    continue;
                }
                percentOccupancy        = 0;
                percentMonthlyOccupancy = 0;
                
                if( nursingStation.TotalBeds > 0 )
                {
                    percentOccupancy = (int)Math.Round( 
                        ( (decimal)nursingStation.CurrentCensus * 100 / 
                        (decimal)nursingStation.TotalBeds ) );
                }
                
                if( nursingStation.TotalBedsForMonth > 0 )
                {
                    percentMonthlyOccupancy = (int)Math.Round( 
                        ( (decimal)nursingStation.TotalOccupiedBedsForMonth * 100 / 
                        (decimal)nursingStation.TotalBedsForMonth ) );
                }
                ADTReportRow = summaryRows.Add();
                ADTReportRow[COL_NURSING_STATION]       = nursingStation.Code + PAD_TEXT;
                ADTReportRow[COL_PREVIOUS_CENSUS]       = nursingStation.PreviousCensus + PAD_TEXT;
                ADTReportRow[COL_ADMIT_TODAY]           = nursingStation.AdmitToday + PAD_TEXT;
                ADTReportRow[COL_DISCH_TODAY]           = nursingStation.DischargeToday + PAD_TEXT;
                ADTReportRow[COL_DEATHS_TODAY]          = nursingStation.DeathsToday + PAD_TEXT;
                ADTReportRow[COL_TRANSFERS_TODAY]       = nursingStation.TransferredFromToday + PAD_TEXT;
                ADTReportRow[COL_AVAILABLE_BEDS]        = nursingStation.AvailableBeds + PAD_TEXT;
                ADTReportRow[COL_CURRENT_CENSUS]        = nursingStation.CurrentCensus + PAD_TEXT;
                ADTReportRow[COL_TOTAL_BEDS]            = nursingStation.TotalBeds + PAD_TEXT;
                ADTReportRow[COL_PERCENT_OCCUPANCY]     = percentOccupancy + "%" + PAD_TEXT;
                ADTReportRow[COL_MO_PATIENT_DAYS]       = nursingStation.TotalOccupiedBedsForMonth + PAD_TEXT;
                ADTReportRow[COL_MO_BEDS_AVAILABLE]     = nursingStation.TotalBedsForMonth + PAD_TEXT;
                ADTReportRow[COL_MO_PERCENT_OCCUPANCY]  = percentMonthlyOccupancy + "%" + PAD_TEXT;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        
        public CensusbyADTReport()
        {
            CreateDataStructure();
        }

        #endregion

        #region Data Elements

        private UltraDataSource dataSource;
        private UltraDataRow ADTReportRow;
        private UltraDataRowsCollection patientRows;
        private UltraDataRowsCollection emptyRows;
        private UltraDataRowsCollection summaryHeaderRows;
        private UltraDataRowsCollection summaryRows;
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        private UltraGridBand emptyBand;
        private UltraGridBand summaryHeaderBand;
        private UltraGridBand summaryBand;
        private ArrayList allAccountProxies;

        #endregion

        #region Constants

        private const string 
            HEADER_TEXT                = "Census by A-D-T",
            ALL_NURSINGSTATIONS_UPPER  = "ALL",
            ALL_NURSINGSTATIONS_PROPER = "All",
            CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";

        protected const int
        SELECTED_NURSING_STATIONS   = 0,
        ADT_SORT_TYPE               = 1;

        private const string
 
            SEARCH_CRITERIA_BAND    = "SearchCriteria_Band", 
            EMPTY_BAND              = "Empty_Band",
            SUMMARY_BAND            = "Summary_Band", 
            PAD_TEXT                = " ",
            ADT_SORTBY_PATIENT      = "Patient",
            ADT_SORTBY_TIME         = "Time",
            SUMMARY_BAND_CAPTION    = 
                "Statistical Summary by Nursing Station (Inpatients Only)";

        protected const string

            SUMMARY_HEADER_BAND              = "SummaryHeader_Band",
            PATIENT_BAND                     = "Patient_Band",
            COL_CONFIDENTIAL                 = "Prvcy Opt",
            COL_ADT_TYPE                     = "A-D-T",
            COL_TRANSACTION_TIME             = "Time",
            COL_PATIENT_NAME_ACCOUNT         = "Patient",
            COL_PATIENT_NAME_ACCOUNT_TYPE    = "Name_Account_Type",
            COL_PATIENT_TYPE                 = "PT",
            COL_LOCATION                     = "Location",
            COL_LOCATION_FROM_TO             = "Location_From_To",
            COL_PHYSICIANS_CHIEF_COMPLAINT   = "Physicians/Chief Complaint",
            COL_PHYSICIANS_DISCH_DISPOSITION = "Physicians/Discharge Disposition",
            COL_PHYSICIANS                   = "Physicians";


        private const string

            COL_SEARCH_CRITERIA_LABEL        = "Search Label",
            COL_SEARCH_CRITERIA_VALUE        = "Search Value",
            COL_TOTAL_RECORDS                = "Total Records",
            COL_EMPTY                        = "Empty Column",
            COL_SUMMARY_HEADER               = "Summary Header",

            COL_NURSING_STATION              = "NS",
            COL_PREVIOUS_CENSUS              = "Previous Census",
            COL_ADMIT_TODAY                  = "Admit Today",
            COL_DISCH_TODAY                  = "Disch Today",
            COL_DEATHS_TODAY                 = "Deaths Today",
            COL_TRANSFERS_TODAY              = "Trfr Today",
            COL_AVAILABLE_BEDS               = "Avl Beds",
            COL_CURRENT_CENSUS               = "Curr Census",
            COL_TOTAL_BEDS                   = "Total Beds",
            COL_PERCENT_OCCUPANCY            = "% Occ",
            COL_MO_PATIENT_DAYS              = "Mo Pt Days",
            COL_MO_BEDS_AVAILABLE            = "Mo Beds Avl",
            COL_MO_PERCENT_OCCUPANCY         = "Mo % Occ";

        #endregion
	}
}
