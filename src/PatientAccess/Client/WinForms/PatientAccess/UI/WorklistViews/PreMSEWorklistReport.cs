using System;
using System.Collections;
using System.Drawing;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for PreMSEWorklistReport.
    /// </summary>
    public class PreMSEWorklistReport : PrintReport
    {
        #region Events
        #endregion

        #region Event Handler
        #endregion

        #region Construction And Finalization
        public PreMSEWorklistReport()
        {
            CreateDataStructure();
        }
        #endregion

        #region Methods

        public void PrintPreview()
        {
            FillDataSource();
            base.DataSource = dataSource;
            base.UpdateView();
            CustomizeGridLayout();
            base.HeaderText = HEADER_TEXT;
            base.GeneratePrintPreview();
        }

        #endregion

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            UltraDataBand searchBand = this.dataSource.Band;
            searchBand.Key = SEARCH_CRITERIA_BAND;
            searchBand.Columns.Add( SEARCHBAND_SRCHCRIT_LABEL );
            searchBand.Columns.Add( SEARCHBAND_SRCHCRIT );
            searchBand.Columns.Add( SEARCHBAND_RECORDS );

            UltraDataBand patBand = searchBand.ChildBands.Add( PATIENT_BAND );
            patBand.Columns.Add( PATBAND_GRIDCOL_PATNAME );
            patBand.Columns.Add( PATBAND_GRIDCOL_ACCOUNTNUMBER, typeof( long ) );
            patBand.Columns.Add( PATBAND_GRIDCOL_ADMITDATETIME );
            patBand.Columns.Add( PATBAND_GRIDCOL_TODO, typeof( int ) );
            patBand.Columns.Add( PATBAND_GRIDCOL_EMPTYCOL );
          }

        private void CustomizeGridLayout()
        {
            searchCriteriaBand = PrintGrid.DisplayLayout.Bands[SEARCH_CRITERIA_BAND];
            patientBand = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];

            searchCriteriaBand.ColHeadersVisible = false;
            searchCriteriaBand.Indentation = 0;
            searchCriteriaBand.Override = base.OverrideWithoutBorder;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT_LABEL].CellAppearance.TextHAlign = HAlign.Left;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].CellAppearance.TextHAlign = HAlign.Left;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].CellAppearance.TextHAlign = HAlign.Right;

            patientBand.ColHeadersVisible = true;
            patientBand.Indentation = 0;            
            patientBand.Override = base.OverrideWithoutBorder;
            patientBand.Override.HeaderAppearance = base.BandHeaderRowAppearance;
            patientBand.Override.HeaderAppearance.ForeColor = Color.Black;
            patientBand.Override.CellAppearance.TextHAlign = HAlign.Right;
            patientBand.Columns[PATBAND_GRIDCOL_PATNAME].CellAppearance.TextHAlign = HAlign.Left;

            PrintGrid.DisplayLayout.InterBandSpacing = 0;

            SetColumnWidths();
            PrintGrid.Refresh();
        }

        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT_LABEL].Width = 90;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].Width = 720;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].Width = 225;

            patientBand.Columns[PATBAND_GRIDCOL_PATNAME].Width = 400;
            patientBand.Columns[PATBAND_GRIDCOL_ACCOUNTNUMBER].Width = 100;
            patientBand.Columns[PATBAND_GRIDCOL_ADMITDATETIME].Width = 100;
            patientBand.Columns[PATBAND_GRIDCOL_TODO].Width = 47;
            patientBand.Columns[PATBAND_GRIDCOL_EMPTYCOL].Width = 440;
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            string patientName = String.Empty;
            long accountNumber;
            string admitDateTime = String.Empty;
            long toDo;
            string fromDate = String.Empty;
            string toDate = String.Empty;
            UltraDataRowsCollection patientRows;
            UltraDataRow patientRow;

            if( !( ( SearchCriteria.FromDate.Equals( DateTime.MinValue ) ) ||
                ( SearchCriteria.ToDate.Equals( DateTime.MinValue ) ) ) )
            {
                fromDate = ( SearchCriteria.FromDate ).ToString( "MM/dd/yyyy - " );
                toDate = ( SearchCriteria.ToDate ).ToString( "MM/dd/yyyy" );
            }

            dataSource.Rows.Add( new string[] { "Last name range:", 
                this.SearchCriteria.BeginningWithLetter
                + "-" + this.SearchCriteria.EndingWithLetter, 
                String.Empty } );

            dataSource.Rows.Add( new string[] { "Admit date:", 
                this.SearchCriteria.WorklistSelectionRange 
                + " " + fromDate + toDate,
                "No. of records: " + this.Model.Count } );                                                  

            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( PATIENT_BAND );

            foreach( WorklistItem wi in this.Model )
            {
                patientName         = wi.Name;
                accountNumber       = wi.AccountNumber;
                toDo                = wi.ToDoCount;

                if( wi.AdmitDate != DateTime.MinValue )
                {
                    admitDateTime = wi.AdmitDate.ToString( "MM/dd/yyyy HH:mm" );
                }

                patientRow = patientRows.Add();

                patientRow[ PATBAND_GRIDCOL_PATNAME ] = patientName;
                patientRow[ PATBAND_GRIDCOL_ACCOUNTNUMBER ] = accountNumber;
                patientRow[ PATBAND_GRIDCOL_ADMITDATETIME ] = admitDateTime;
                patientRow[ PATBAND_GRIDCOL_TODO ] = toDo;
            }
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

        public new WorklistSettings SearchCriteria
        {
            private get
            {
                return base.SearchCriteria as WorklistSettings;
            }
            set
            {
                base.SearchCriteria = value;
            }
        }
        #endregion

        #region Data Elements

        private UltraDataSource dataSource;
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        #endregion

        #region Constants
        private const string HEADER_TEXT = "Pre-MSE Worklist";

        private const string 
            PATBAND_GRIDCOL_PATNAME = "Patient Name",
            PATBAND_GRIDCOL_ACCOUNTNUMBER = "Account",
            PATBAND_GRIDCOL_ADMITDATETIME = "Admit Date/Time",
            PATBAND_GRIDCOL_TODO = "To Do",
            PATBAND_GRIDCOL_EMPTYCOL = " ",

            SEARCHBAND_SRCHCRIT_LABEL = "SearchCriteria Label",
            SEARCHBAND_SRCHCRIT = "SearchCriteria",
            SEARCHBAND_RECORDS = "No Of Records";
                                
        private const string SEARCH_CRITERIA_BAND = "SearchCriteria_Band",
            PATIENT_BAND = "Patient_Band";
            #endregion

    }
}
