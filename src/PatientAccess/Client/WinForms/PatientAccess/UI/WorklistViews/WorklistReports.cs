using System;
using System.Collections;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.WorklistViews
{
    /// <summary>
    /// Summary description for WorklistReports.
    /// </summary>
    public class WorklistReports : PrintReport
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public void PrintPreview()
        {
            FillDataSource();
            base.DataSource = dataSource;            
            UpdateView();            
            CustomizeGridLayout();
            // TLG 8/14/2006 - Use the sort from the actual worklist
            //patientBand.Columns[PATBAND_GRIDCOL_NAME].SortIndicator = SortIndicator.Ascending;
            base.GeneratePrintPreview();
        }

        #endregion

        #region Properties
        public object Sender
        {
            private get
            {
                return i_Sender;
            }
            set
            {
                i_Sender = value;
            }
        }

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

        #region Construction and Finalization
        public WorklistReports()
        {
            CreateDataStructure();
        }
        #endregion

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            UltraDataBand searchBand = this.dataSource.Band;
            searchBand.Key = SEARCH_CRITERIA_BAND;
            searchBand.Columns.Add( SEARCHBAND_SRCHCRIT_LABEL);
            searchBand.Columns.Add( SEARCHBAND_SRCHCRIT );
            searchBand.Columns.Add( SEARCHBAND_RECORDS );

            UltraDataBand patientBand = searchBand.ChildBands.Add( PATIENT_BAND );            
            patientBand.Columns.Add( PATBAND_GRIDCOL_NAME );

            UltraDataBand accountBand = patientBand.ChildBands.Add( ACCOUNT_INFORMATION_BAND );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_HSV_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_HSV_CLINIC );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_FC_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_FC_PAYOR );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_DISCH_TODO );

            UltraDataBand lastBand = searchBand.ChildBands.Add( LAST_BAND );
            lastBand.Columns.Add( LASTBAND_COL );
        }

        private void CustomizeGridLayout()
        {
            searchCriteriaBand      = PrintGrid.DisplayLayout.Bands[SEARCH_CRITERIA_BAND];
            patientBand             = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            accountInformationBand  = PrintGrid.DisplayLayout.Bands[ACCOUNT_INFORMATION_BAND];
            lastBand                = PrintGrid.DisplayLayout.Bands[LAST_BAND];

            searchCriteriaBand.ColHeadersVisible = false;
            searchCriteriaBand.Indentation = 0;
            searchCriteriaBand.Override = base.OverrideWithoutBorder;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].CellMultiLine = DefaultableBoolean.True;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].CellMultiLine = DefaultableBoolean.True;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;

            patientBand.ColHeadersVisible = false;
            patientBand.Indentation = 0;
            patientBand.Override = base.OverrideWithBorder;
            patientBand.Columns[PATBAND_GRIDCOL_NAME].CellAppearance.FontData.Bold =
                DefaultableBoolean.True;
            patientBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;

            accountInformationBand.ColHeadersVisible = false;
            accountInformationBand.Indentation = 14;
            accountInformationBand.Override = base.OverrideWithoutBorder;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_HSV_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_HSV_CLINIC].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_FC_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_FC_PAYOR].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DISCH_TODO].CellMultiLine = 
                DefaultableBoolean.True;

            lastBand.ColHeadersVisible = false;
            lastBand.Indentation = 0;
            lastBand.Override = base.OverrideWithBorder;

            SetColumnWidths();
            PrintGrid.Refresh();
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
           
            string fromDate = String.Empty;
            string toDate = String.Empty;
            dataSource.Rows.Clear();
            string dischargeStatus;
            string hsv;
            string clinic;
            string financialClass;

            UltraDataRowsCollection accountRows;
            UltraDataRow accountRow;
            UltraDataRowsCollection patientRows;
            UltraDataRow patientRow;

            UltraDataRowsCollection summaryRows;
            UltraDataRow summaryRow;

            ArrayList allWorklistItems = this.Model;

            dataSource.Rows.Add( new string[] {
                "Last name range:  ", 
                SearchCriteria.BeginningWithLetter + "-" +
                SearchCriteria.EndingWithLetter, String.Empty } );

            if( !( ( SearchCriteria.FromDate.Equals( DateTime.MinValue ) ) ||
                ( SearchCriteria.ToDate.Equals( DateTime.MinValue ) ) ) )
            {
                fromDate = ( SearchCriteria.FromDate ).ToString( "MM/dd/yyyy - " );
                toDate = ( SearchCriteria.ToDate ).ToString( "MM/dd/yyyy" );
            }

            dataSource.Rows.Add( new string[] {
                "Admit date: ", 
                SearchCriteria.WorklistSelectionRange + "  " 
                + fromDate + toDate, 
                "No. of records: " + allWorklistItems.Count } );

            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey(PATIENT_BAND) );

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey(LAST_BAND) );

            summaryRow = summaryRows.Add( new string[] {" " } );
            foreach (WorklistItem wi in allWorklistItems)
            {
                patientRow = patientRows.Add(new string[] { wi.Name });

                accountRows = patientRow.GetChildRows(ACCOUNT_INFORMATION_BAND);
                accountRow = accountRows.Add();

                dischargeStatus = wi.DischargeStatus;
                hsv = wi.HospitalService;
                clinic = wi.Clinic;
                financialClass = wi.FinancialClass;

                accountRow[ACCTBAND_GRIDCOL_ACCT_LABEL] =
                    String.Format("Account:   \nAdmit date/time:   ");

                accountRow[ACCTBAND_GRIDCOL_ACCT] = String.Format(
                    "{0}\n{1}",
                    wi.AccountNumber,
                    wi.AdmitDate.ToString("MM/dd/yyyy HH:mm"));

                accountRow[ACCTBAND_GRIDCOL_HSV_LABEL] =
                    String.Format(
                    "Hospital Service:  \nClinic:  ");

                accountRow[ACCTBAND_GRIDCOL_HSV_CLINIC] = String.Format(
                    "{0}\n{1}",
                    hsv, clinic);

                accountRow[ACCTBAND_GRIDCOL_FC_LABEL] = String.Format(
                    "Financial Class:\nPrimary Payor:");

                accountRow[ACCTBAND_GRIDCOL_FC_PAYOR] = String.Format(
                    "{0}\n{1}",
                    financialClass,
                    wi.PrimaryPayor);

                if (Sender.GetType().Equals(typeof(NoShowWorklistView)))
                {
                    accountRow[ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL] = "To Do Count:";

                    accountRow[ACCTBAND_GRIDCOL_DISCH_TODO] =
                        wi.ToDoCount;
                }
                else
                {
                    accountRow[ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL] =
                        "Discharge status:\nTo Do count:";

                    accountRow[ACCTBAND_GRIDCOL_DISCH_TODO] = String.Format(
                        "{0}\n{1}",
                        dischargeStatus,
                        wi.ToDoCount);
                }
            }


        }
      
        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT_LABEL].Width = 90;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].Width = 745;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].Width = 200;

            patientBand.Columns[PATBAND_GRIDCOL_NAME].Width = 1037;

            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_LABEL].Width = 110;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT].Width = 150;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_HSV_LABEL].Width = 110;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_HSV_CLINIC].Width = 175;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_FC_LABEL].Width = 100;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_FC_PAYOR].Width = 150;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL].Width = 112;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DISCH_TODO].Width = 115;

            lastBand.Columns[LASTBAND_COL].Width = 1037;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private object i_Sender;   
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        private UltraGridBand accountInformationBand;
        private UltraGridBand lastBand;
        private UltraDataSource dataSource;
        
        #endregion

        #region Constants

        private const string LAST_BAND = "Last_Band",
            SEARCH_CRITERIA_BAND = "SearchCriteria_Band",
            PATIENT_BAND = "Patient_Band",
            ACCOUNT_INFORMATION_BAND = "AccountInformation_Band";

        private const string 
            ACCTBAND_GRIDCOL_ACCT_LABEL = "Account-AdmitDate-Label",
            ACCTBAND_GRIDCOL_ACCT = "Account-AdmitDate",
            ACCTBAND_GRIDCOL_HSV_LABEL = "HSV-Clinic-Label",
            ACCTBAND_GRIDCOL_HSV_CLINIC = "HSV-Clinic",
            ACCTBAND_GRIDCOL_FC_LABEL = "FC-Payor-Label",
            ACCTBAND_GRIDCOL_FC_PAYOR = "FC-Payor",
            ACCTBAND_GRIDCOL_DISCHSTATUS_LABEL = "DischStatus-ToDo-Label",
            ACCTBAND_GRIDCOL_DISCH_TODO = "DischStatus-ToDo",

            PATBAND_GRIDCOL_NAME = "PatientName",

            SEARCHBAND_SRCHCRIT_LABEL = "SearchCriteria-Label",
            SEARCHBAND_SRCHCRIT = "SearchCriteria",
            SEARCHBAND_RECORDS = "NoOfRecords",
                                
            LASTBAND_COL = "BlankColumn";

        #endregion
    }
}
