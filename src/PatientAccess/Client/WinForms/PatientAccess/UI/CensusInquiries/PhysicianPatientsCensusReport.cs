using System;
using System.Collections;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    /// <summary>
    /// Summary description for PhysicianPatientsCensusReport.
    /// </summary>
    public class PhysicianPatientsCensusReport : PrintReport
    {
        #region Events
        #endregion

        #region EventHandlers
        #endregion

        #region Methods
        
        public void PrintPreview()
        {
            FillDataSource();
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

        public new Physician SearchCriteria
        {
            private get
            {
                return base.SearchCriteria as Physician;
            }
            set
            {
                base.SearchCriteria = value;
            }
        }

        public new Physician SummaryInformation
        {
            private get
            {
                return base.SummaryInformation as Physician;
            }
            set
            {
                base.SummaryInformation = value;
            }
        }

        #endregion
       
        #region Construction and finalization
        public PhysicianPatientsCensusReport()
        {			
            CreateDataStructure();
        }
        #endregion

        #region Private Methods
        
        private void CreateDataStructure()
        {
            dataSource = new UltraDataSource();

            UltraDataBand searchBand = this.dataSource.Band;
            searchBand.Key = SEARCHCRITERIA_BAND;
            searchBand.Columns.Add( GRIDCOL_SEARCHCRITERIA_PHYSICIAN_LABEL );
            searchBand.Columns.Add( GRIDCOL_SEARCHCRITERIA_PHYSICIAN_NAME );
            searchBand.Columns.Add( GRIDCOL_SEARCHCRITERIA_NO_OF_RECORDS ); 

            UltraDataBand patBand = searchBand.ChildBands.Add( PATIENT_BAND );
            patBand.Columns.Add( GRIDCOL_PATIENT_OPTOUT );
            patBand.Columns.Add( GRIDCOL_PATIENT_LOCATION );
            patBand.Columns.Add( GRIDCOL_PATIENT_NAME );    
            patBand.Columns.Add( GRIDCOL_DISCHARGE_STATUS );

            UltraDataBand accountBand = patBand.ChildBands.Add( ACCOUNTINFORMATION_BAND );
            accountBand.Columns.Add( GRIDCOL_ACCOUNT_DUMMY );
            accountBand.Columns.Add( GRIDCOL_ACCOUNT_MRN_LABEL );
            accountBand.Columns.Add( GRIDCOL_ACCOUNT_ISOLATION_PT_DATA );
            accountBand.Columns.Add( GRIDCOL_ACCOUNT_RELATIONSHIP_DATA );

            UltraDataBand chiefComBand = accountBand.ChildBands.Add( CHIEFCOMPLAINT_BAND );
            chiefComBand.Columns.Add( GRIDCOL_CHIEFCOMPLAINT_DUMMY );
            chiefComBand.Columns.Add( GRIDCOL_CHIEFCOMPLAINT_DATA );

            UltraDataBand emptyBand = searchBand.ChildBands.Add( EMPTY_BAND );
            emptyBand.Columns.Add( GRIDCOL_EMPTYBAND_EMPTYCOLUMN );

            UltraDataBand summBand = searchBand.ChildBands.Add( SUMMARY_BAND );
            summBand.Columns.Add( GRIDCOL_SUMMARY_TOTAL_ATTENDING_PATIENTS );
            summBand.Columns.Add( GRIDCOL_SUMMARY_ADMITTING_REFERRING_PATIENTS );
            summBand.Columns.Add( GRIDCOL_SUMMARY_CONSULTING_OPERATING_PATIENTS );
        }

        private void CustomizeGridLayout()
        {
            searchCriteriaBand = PrintGrid.DisplayLayout.Bands[SEARCHCRITERIA_BAND];
            patientBand = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            accountInformationBand = PrintGrid.DisplayLayout.Bands[ACCOUNTINFORMATION_BAND];
            chiefComplaintBand = PrintGrid.DisplayLayout.Bands[CHIEFCOMPLAINT_BAND];
            emptyBand = PrintGrid.DisplayLayout.Bands[EMPTY_BAND];
            summaryBand = PrintGrid.DisplayLayout.Bands[SUMMARY_BAND];

            // searchCriteriaBand Properties
            searchCriteriaBand.ColHeadersVisible = false;
            searchCriteriaBand.Indentation = 0;
            searchCriteriaBand.Override = base.OverrideWithoutBorder;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_NO_OF_RECORDS]
                .CellAppearance.FontData.Bold = DefaultableBoolean.True;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_PHYSICIAN_NAME]
                .CellMultiLine = DefaultableBoolean.True;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_PHYSICIAN_LABEL]
                .CellMultiLine = DefaultableBoolean.True;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_NO_OF_RECORDS]
                .CellMultiLine = DefaultableBoolean.True;
            searchCriteriaBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;

            // patientBand Properties
            patientBand.ColHeadersVisible = false;
            patientBand.Indentation = 0;
            patientBand.Override = base.OverrideWithBorder;
            patientBand.Override.CellAppearance.FontData.Bold = DefaultableBoolean.False;
            patientBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;
         

            accountInformationBand.ColHeadersVisible = false;
            accountInformationBand.Indentation = 34;
            accountInformationBand.Override = base.OverrideWithoutBorder;
            accountInformationBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_DUMMY]
                .CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_MRN_LABEL]
                .CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_ISOLATION_PT_DATA]
                .CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_RELATIONSHIP_DATA]
                .CellMultiLine = DefaultableBoolean.True;

            // chiefComplaintBand Properties
            chiefComplaintBand.ColHeadersVisible = false;
            chiefComplaintBand.Indentation = 0;
            chiefComplaintBand.Override = base.OverrideWithoutBorder;
            chiefComplaintBand.Columns[GRIDCOL_CHIEFCOMPLAINT_DATA].CellMultiLine 
                = DefaultableBoolean.True;
            chiefComplaintBand.Override.CellAppearance.TextVAlign = VAlign.Top;
        

            // emptyBand Properties
            emptyBand.ColHeadersVisible = false;
            emptyBand.Indentation = 0;
            emptyBand.Override = base.OverrideWithoutBorder;

            // to set summaryBand Properties
            SetSummaryBandProperties();
            SetColumnWidths();

            PrintGrid.Refresh();
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            string gender;
            string chiefComplaint;
            string dischargeStatus;
            string patientType;
            string physicianRelationship;
            string blank = new string( ' ', 145 );
            UltraDataRowsCollection patientRows;
            UltraDataRowsCollection summaryRows;
            UltraDataRowsCollection accountRows;
            UltraDataRowsCollection chiefComplaintRows;
            UltraDataRowsCollection emptyRows;
            UltraDataRow patientRow;
            UltraDataRow summaryRow;
            UltraDataRow accountRow;
            UltraDataRow chiefComplaintRow;
            UltraDataRow emptyRow;

            ArrayList allAccountProxies = this.Model;
            
            dataSource.Rows.Add( new string[] { "", "", "" } );
            dataSource.Rows.Add( new string[] {
                                                  "Physician: \n", 
                                                  String.Format( "{0}     {1}\n", 
                                                  SearchCriteria.Name.LastName, 
                                                  SearchCriteria.PhysicianNumber ),
                                                  "\nNo. of records: " + allAccountProxies.Count } );
            
            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( PATIENT_BAND ) );

            foreach( AccountProxy accountProxy in allAccountProxies )
            {      
                gender = String.Empty;
                chiefComplaint = String.Empty;
                dischargeStatus = String.Empty;
                patientType = String.Empty;
                physicianRelationship = String.Empty;

                patientRow = patientRows.Add();

                patientRow[GRIDCOL_PATIENT_OPTOUT] = accountProxy.AddOnlyLegends();

                if( accountProxy.Location != null )
                {
                    patientRow[GRIDCOL_PATIENT_LOCATION] 
                        = accountProxy.Location.ToString();
                }

                patientRow[GRIDCOL_PATIENT_NAME] 
                    = accountProxy.Patient.Name.AsFormattedName();

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);
                patientRow[GRIDCOL_PATIENT_NAME] = patientNameFormatter.GetFormattedPatientName();

                if( accountProxy.DischargeStatus != null && 
                    accountProxy.DischargeStatus.Description.Equals( PENDING_DISCHARGE ) )
                {
                    dischargeStatus = PENDING_DISCHARGE_LABEL;
                }
                else
                {
                    if( accountProxy.DischargeDate.Date.Equals( DateTime.MinValue )  )
                    {
                        dischargeStatus = String.Empty;
                    }
                    else
                    {
                        dischargeStatus = accountProxy.DischargeDate.ToString( "MM/dd/yyyy" );
                    }
                }
   
                if( accountProxy.Patient.Sex != null )
                {
                    gender = accountProxy.Patient.Sex.Code;
                }

                if( accountProxy.Diagnosis != null )
                {
                    chiefComplaint = accountProxy.Diagnosis.ChiefComplaint;
                }

                if( accountProxy.KindOfVisit != null )
                {
                    patientType = String.Format( "{0} {1}", 
                        accountProxy.KindOfVisit.Code, 
                        accountProxy.KindOfVisit.Description );
                }
                
                physicianRelationship = accountProxy.PhysicianRelationship;
                       
                physicianRelationship = 
                    physicianRelationship.Replace( "Adm", "Admitting" );
                physicianRelationship = 
                    physicianRelationship.Replace( "Att", "Attending" );
                physicianRelationship = 
                    physicianRelationship.Replace( "Ref", "Referring" );
                physicianRelationship = 
                    physicianRelationship.Replace( "Opr", "Operating" );
                physicianRelationship = 
                    physicianRelationship.Replace( "Con", "Consulting" );
                
                patientRow[GRIDCOL_DISCHARGE_STATUS] = 
                    String.Format( "Discharge status:  {0}", dischargeStatus );

                accountRows = patientRow.GetChildRows( ACCOUNTINFORMATION_BAND ); 
                accountRow = accountRows.Add();
                accountRow[GRIDCOL_ACCOUNT_DUMMY] = blank;
                accountRow[GRIDCOL_ACCOUNT_MRN_LABEL] = 
                    String.Format( "MRN: {0}\nGender: {1}Age: {2}",
                    accountProxy.Patient.MedicalRecordNumber.ToString().PadLeft( 17, ' ' ), 
                    gender.PadRight( 7, ' ' ),
                    accountProxy.Patient.AgeAt( DateTime.Today ).PadLeft( 4, '0' ).ToUpper() );
                
                accountRow[GRIDCOL_ACCOUNT_ISOLATION_PT_DATA] = 
                    String.Format( "Isolation:  {0}\nPatient type:  {1}", 
                    accountProxy.IsolationCode.PadLeft( 5, ' ' ), 
                    patientType);
                accountRow[GRIDCOL_ACCOUNT_RELATIONSHIP_DATA] = 
                    String.Format( "\nRelationship:  {0}", physicianRelationship );                

                chiefComplaintRows = accountRow.GetChildRows( CHIEFCOMPLAINT_BAND );
                chiefComplaintRow = chiefComplaintRows.Add();

                chiefComplaintRow[GRIDCOL_CHIEFCOMPLAINT_DUMMY] = blank;
                chiefComplaintRow[GRIDCOL_CHIEFCOMPLAINT_DATA] = 
                    String.Format( "Chief complaint:  {0}",chiefComplaint );
            }

            emptyRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( EMPTY_BAND ) );
                
            for( int i = 0; i < 10; i++ )
            {
                emptyRow = emptyRows.Add( new string[] { String.Empty } );
            }                

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( SUMMARY_BAND ) );
            summaryRow = summaryRows.Add();
            
            summaryRow[GRIDCOL_SUMMARY_TOTAL_ATTENDING_PATIENTS] 
                = String.Format( "  Total patients:  {0}\n  Patients as attending physician: {1}", 
                SummaryInformation.TotalPatients.ToString().PadLeft( 29, ' ' ) , 
                SummaryInformation.TotalAttendingPatients.ToString().PadLeft( 7, ' ' ) );
                
            summaryRow[GRIDCOL_SUMMARY_ADMITTING_REFERRING_PATIENTS] 
                = String.Format( "Patients as admitting physician:  {0}\nPatients as referring physician:  {1}", 
                SummaryInformation.TotalAdmittingPatients.ToString().PadLeft( 6, ' ' ), 
                SummaryInformation.TotalReferringPatients.ToString().PadLeft( 7, ' ' ) );

            summaryRow[GRIDCOL_SUMMARY_CONSULTING_OPERATING_PATIENTS] 
                = String.Format( "Patients as consulting physician:  {0}\nPatients as operating physician:  {1}", 
                SummaryInformation.TotalConsultingPatients.ToString().PadLeft( 5, ' ' ), 
                SummaryInformation.TotalOperatingPatients.ToString().PadLeft( 6, ' ' ) );
        }

        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_PHYSICIAN_LABEL].Width = 65;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_PHYSICIAN_NAME].Width = 736;
            searchCriteriaBand.Columns[GRIDCOL_SEARCHCRITERIA_NO_OF_RECORDS].Width = 226;

            patientBand.Columns[GRIDCOL_PATIENT_OPTOUT].Width = 55;
            patientBand.Columns[GRIDCOL_PATIENT_LOCATION].Width = 95;
            patientBand.Columns[GRIDCOL_PATIENT_NAME].Width = 430;
            patientBand.Columns[GRIDCOL_DISCHARGE_STATUS].Width = 458;

            accountInformationBand.Columns[GRIDCOL_ACCOUNT_DUMMY].Width = 145;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_MRN_LABEL].Width = 200;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_ISOLATION_PT_DATA].Width = 200;
            accountInformationBand.Columns[GRIDCOL_ACCOUNT_RELATIONSHIP_DATA].Width = 458;//492

            chiefComplaintBand.Columns[GRIDCOL_CHIEFCOMPLAINT_DUMMY].Width = 145;
            chiefComplaintBand.Columns[GRIDCOL_CHIEFCOMPLAINT_DATA].Width = 858;   //892         

            emptyBand.Columns[GRIDCOL_EMPTYBAND_EMPTYCOLUMN].Width = 1037;

            summaryBand.Columns[GRIDCOL_SUMMARY_TOTAL_ATTENDING_PATIENTS].Width = 251;
            summaryBand.Columns[GRIDCOL_SUMMARY_ADMITTING_REFERRING_PATIENTS].Width = 251;            
            summaryBand.Columns[GRIDCOL_SUMMARY_CONSULTING_OPERATING_PATIENTS].Width = 251;
        }

        // band border should drawn in border_color -- TBD
        private void SetSummaryBandProperties()
        {
            summaryBand.Header.Caption = STATISTICAL_SUMMARY_TABLE_HEADER;
            summaryBand.HeaderVisible = true; 
            summaryBand.Header.Appearance = base.SummaryHeaderAppearance;
            summaryBand.Header.Appearance.BorderColor = Border_Color;
            summaryBand.Header.Appearance.FontData.Bold = DefaultableBoolean.True;

            summaryBand.ColHeadersVisible = false;
            summaryBand.Indentation = 137;            

            summaryBand.Override.BorderStyleRow = UIElementBorderStyle.Solid;
            summaryBand.Override.RowAppearance.BorderColor = Row_Separator_Color;            


            summaryBand.Columns[GRIDCOL_SUMMARY_TOTAL_ATTENDING_PATIENTS].CellMultiLine 
                = DefaultableBoolean.True;
            summaryBand.Columns[GRIDCOL_SUMMARY_ADMITTING_REFERRING_PATIENTS].CellMultiLine 
                = DefaultableBoolean.True;
            summaryBand.Columns[GRIDCOL_SUMMARY_CONSULTING_OPERATING_PATIENTS].CellMultiLine 
                = DefaultableBoolean.True;            
        }

        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private UltraDataSource dataSource;
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        private UltraGridBand accountInformationBand;
        private UltraGridBand chiefComplaintBand;
        private UltraGridBand emptyBand;
        private UltraGridBand summaryBand;
        #endregion

        #region Constants

        private const string HEADER_TEXT = "Patient Census by Physician";

        private const string PENDING_DISCHARGE_LABEL = "Pending",
            PENDING_DISCHARGE = "PENDINGDISCHARGE",
            CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";

        private const string STATISTICAL_SUMMARY_TABLE_HEADER = 
            "Statistical summary for selected physician (Inpatients Only)";

        private const string
            SEARCHCRITERIA_BAND = "SearchCriteria_Band",
            PATIENT_BAND = "Patient_Band",
            ACCOUNTINFORMATION_BAND = "AccountInformation_Band",
            CHIEFCOMPLAINT_BAND = "ChiefComplaint_Band",
            EMPTY_BAND = "Empty_Band",
            SUMMARY_BAND = "Summary_Band";

        private const string 
            GRIDCOL_SEARCHCRITERIA_PHYSICIAN_LABEL = "Physician_Label",
            GRIDCOL_SEARCHCRITERIA_PHYSICIAN_NAME = "Physician_Name",                
            GRIDCOL_SEARCHCRITERIA_NO_OF_RECORDS = "NoOfRecords";

        private const string 
            GRIDCOL_PATIENT_OPTOUT = "OptOutInformation",
            GRIDCOL_PATIENT_LOCATION = "Location",                
            GRIDCOL_PATIENT_NAME = "PatientName",
            GRIDCOL_DISCHARGE_STATUS = "DischargeStatus";

        private const string 
            GRIDCOL_ACCOUNT_DUMMY = "Blank-Data",
            GRIDCOL_ACCOUNT_MRN_LABEL = "MRN-Label",
            GRIDCOL_ACCOUNT_ISOLATION_PT_DATA = "Isolation-PT-Data",
            GRIDCOL_ACCOUNT_RELATIONSHIP_DATA = "RelationShip-Data";

        private const string 
            GRIDCOL_CHIEFCOMPLAINT_DUMMY = "ChiefComplaint-Blank",
            GRIDCOL_CHIEFCOMPLAINT_DATA = "ChiefComplaint-Data";

        private const string 
            GRIDCOL_EMPTYBAND_EMPTYCOLUMN = "EmptyColumn";

        private const string 
            GRIDCOL_SUMMARY_TOTAL_ATTENDING_PATIENTS = "Total/Attending Patients",
            GRIDCOL_SUMMARY_ADMITTING_REFERRING_PATIENTS = "Admitting/Referring Patients",
            GRIDCOL_SUMMARY_CONSULTING_OPERATING_PATIENTS = "Consulting/Operating patients";
            
        #endregion        
    }
}