using System;
using System.Collections;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.PreRegistrationViews
{
    /// <summary>
    /// Summary description for OnlinePreRegistrationSubmissionsReport.
    /// </summary>
    public class OnlinePreRegistrationSubmissionsReport : PrintReport
    {
        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public   void PrintPreview()
        {
            FillDataSource();
            base.DataSource = dataSource;            
            UpdateView();            
            CustomizeGridLayout();
            base.GeneratePrintPreview();
        }

        #endregion

        #region Properties
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

     
        #endregion

        #region Construction and Finalization
        public OnlinePreRegistrationSubmissionsReport()
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

            UltraDataBand accountBand = patientBand.ChildBands.Add( PATIENT_INFORMATION_BAND );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_VISITED_GENDER_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_VISITED_GENDER_VALUE );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_DOB_SSN_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_DOB_SSN_VALUE );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ADDRESS_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ADDRESS_VALUE );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ADMITDATE_LABEL );
            accountBand.Columns.Add(ACCTBAND_GRIDCOL_ADMITDATE_VALUE);

            UltraDataBand lastBand = searchBand.ChildBands.Add( LAST_BAND );
            lastBand.Columns.Add( LASTBAND_COL );
        }

        private void CustomizeGridLayout()
        {
            searchCriteriaBand      = PrintGrid.DisplayLayout.Bands[SEARCH_CRITERIA_BAND];
            patientBand             = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            accountInformationBand  = PrintGrid.DisplayLayout.Bands[PATIENT_INFORMATION_BAND];
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
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_VISITED_GENDER_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_VISITED_GENDER_VALUE].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DOB_SSN_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DOB_SSN_VALUE].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADDRESS_LABEL].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADDRESS_VALUE].CellMultiLine = 
                DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADMITDATE_LABEL].CellMultiLine = 
                DefaultableBoolean.False;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADMITDATE_VALUE].CellMultiLine = 
                DefaultableBoolean.False;

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
            string VisitedBefore;
            string Gender;
            string DOB;
            string SSN;
            string Address;
            String ExpectedAdmitDate;

            UltraDataRowsCollection accountRows;
            UltraDataRow accountRow;
            UltraDataRowsCollection patientRows;
            UltraDataRow patientRow;

            UltraDataRowsCollection summaryRows;
            UltraDataRow summaryRow;

            ArrayList allPreRegistrationSubmissions = this.Model;
            dataSource.Rows.Add(new string[] {
                "Last name range:  ", 
                SearchCriteria.BeginningWithLetter + "-" +
                SearchCriteria.EndingWithLetter, String.Empty });

            if (!((SearchCriteria.FromDate.Equals(DateTime.MinValue)) ||
                (SearchCriteria.ToDate.Equals(DateTime.MinValue))))
            {
                fromDate = (SearchCriteria.FromDate).ToString("MM/dd/yyyy - ");
                toDate = (SearchCriteria.ToDate).ToString("MM/dd/yyyy");
            }

            dataSource.Rows.Add(new string[] {
                "Admit date: ", 
                SearchCriteria.WorklistSelectionRange + "  " 
                + fromDate + toDate, 
               
                "          No. of submissions: " + allPreRegistrationSubmissions.Count } );

            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows(
                dataSource.GetBandByKey(PATIENT_BAND));

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows(
                dataSource.GetBandByKey(LAST_BAND));


            summaryRow = summaryRows.Add(new string[] { " " });
            foreach (OnlinePreRegistrationItem preRegistrationItem in allPreRegistrationSubmissions)
            {
                patientRow = patientRows.Add(new string[] { preRegistrationItem.PatientName });

                accountRows = patientRow.GetChildRows(PATIENT_INFORMATION_BAND);
                accountRow = accountRows.Add();

                VisitedBefore = preRegistrationItem.VisitedBefore.HasValue
                                          ? ( ( bool )preRegistrationItem.VisitedBefore
                                                 ? YesNoFlag.DESCRIPTION_YES
                                                 : YesNoFlag.DESCRIPTION_NO )
                                          : string.Empty;
                Gender = preRegistrationItem.Gender;
                DOB = preRegistrationItem.DateOfBirth.ToString(("MM/dd/yyyy"));
                SSN = preRegistrationItem.Ssn;
                Address = preRegistrationItem.Address;
                ExpectedAdmitDate = preRegistrationItem.AdmitDate.ToString( ( "MM/dd/yyyy HH:mm" ) );
               

                accountRow[ACCTBAND_GRIDCOL_VISITED_GENDER_LABEL] =
                    String.Format("Visited Before:   \nGender:   ");
               
                accountRow[ACCTBAND_GRIDCOL_VISITED_GENDER_VALUE] = String.Format(
                    "{0}\n{1}",
                    VisitedBefore,  Gender);

                accountRow[ACCTBAND_GRIDCOL_DOB_SSN_LABEL] =
                    String.Format(
                    "DOB:  \nSSN:  ");

                accountRow[ACCTBAND_GRIDCOL_DOB_SSN_VALUE] = String.Format(
                    "{0}\n{1}",
                    DOB, SSN);

                accountRow[ACCTBAND_GRIDCOL_ADDRESS_LABEL] = String.Format(
                    "Address:");

                accountRow[ACCTBAND_GRIDCOL_ADDRESS_VALUE] = String.Format(
                    "{0}",
                   Address );

                accountRow[ACCTBAND_GRIDCOL_ADMITDATE_LABEL] = String.Format(
                 "Expected Admit Date/Time:");

                accountRow[ACCTBAND_GRIDCOL_ADMITDATE_VALUE] = String.Format(
                    "{0}",
                   ExpectedAdmitDate);
            }
        }
       
        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT_LABEL].Width = 90;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].Width = 745;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].Width = 200;

            patientBand.Columns[PATBAND_GRIDCOL_NAME].Width = 1037;

            accountInformationBand.Columns[ACCTBAND_GRIDCOL_VISITED_GENDER_LABEL].Width = 110;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_VISITED_GENDER_VALUE].Width = 150;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DOB_SSN_LABEL].Width = 100;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_DOB_SSN_VALUE].Width = 150;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADDRESS_LABEL].Width = 90;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADDRESS_VALUE].Width = 140;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADMITDATE_LABEL].Width = 130;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ADMITDATE_VALUE].Width = 115;

            lastBand.Columns[LASTBAND_COL].Width = 1037;
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

         
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
            PATIENT_INFORMATION_BAND = "PatientInformation_Band";

        private const string 
            ACCTBAND_GRIDCOL_VISITED_GENDER_LABEL = "Account-Visited-Label",
            ACCTBAND_GRIDCOL_VISITED_GENDER_VALUE = "Account-Visited",
            ACCTBAND_GRIDCOL_DOB_SSN_LABEL = "DOB-Label",
            ACCTBAND_GRIDCOL_DOB_SSN_VALUE = "DOB_SSN",
            ACCTBAND_GRIDCOL_ADDRESS_LABEL = "Address-Label",
            ACCTBAND_GRIDCOL_ADDRESS_VALUE = "Address",
            ACCTBAND_GRIDCOL_ADMITDATE_LABEL = "AdmitDate-Label",
            ACCTBAND_GRIDCOL_ADMITDATE_VALUE = "AdmitDate",

            PATBAND_GRIDCOL_NAME = "PatientName",

            SEARCHBAND_SRCHCRIT_LABEL = "SearchCriteria-Label",
            SEARCHBAND_SRCHCRIT = "SearchCriteria",
            SEARCHBAND_RECORDS = "NoOfRecords",
                                
            LASTBAND_COL = "BlankColumn";

        #endregion
    }
}
