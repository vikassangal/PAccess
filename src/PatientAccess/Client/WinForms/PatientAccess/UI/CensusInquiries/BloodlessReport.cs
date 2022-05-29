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
    /// Summary description for BloodlessReport.
    /// </summary>
    public class BloodlessReport : PrintReport
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
        #endregion
       
        #region Construction and finalization
        public BloodlessReport()
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
            searchBand.Columns.Add( SEARCHBAND_SRCHCRIT );
            searchBand.Columns.Add( SEARCHBAND_RECORDS );             

            UltraDataBand patientBand = searchBand.ChildBands.Add( PATIENT_BAND );
            patientBand.Columns.Add( PATBAND_GRIDCOL_OPTOUT );
            patientBand.Columns.Add( PATBAND_GRIDCOL_NAME );

            UltraDataBand accountBand = patientBand.ChildBands.Add( ACCOUNT_INFORMATION_BAND );            
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_BLANK );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_AGE_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_AGE );            
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_OTHER_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_OTHER );            
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_PHY_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_PHYS );

            UltraDataBand lastBand = searchBand.ChildBands.Add( LAST_BAND );
            lastBand.Columns.Add( LASTBAND_COL );
        }

        private void CustomizeGridLayout()
        {
            searchCriteriaBand      = PrintGrid.DisplayLayout.Bands[SEARCH_CRITERIA_BAND];
            patientBand             = PrintGrid.DisplayLayout.Bands[PATIENT_BAND];
            accountInformationBand  = PrintGrid.DisplayLayout.Bands[ACCOUNT_INFORMATION_BAND];
            lastBand                = PrintGrid.DisplayLayout.Bands[LAST_BAND];

            // searchCriteriaBand Properties
            searchCriteriaBand.ColHeadersVisible = false;
            searchCriteriaBand.Indentation = 0;
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].CellMultiLine = 
                DefaultableBoolean.True;
            searchCriteriaBand.Override = base.OverrideWithoutBorder;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].
                CellAppearance.FontData.Bold = DefaultableBoolean.True;

            // patientBand Properties
            patientBand.ColHeadersVisible = false;
            patientBand.Indentation = 0;
            patientBand.Override = base.OverrideWithBorder;
            patientBand.Override.CellAppearance.FontData.Bold = DefaultableBoolean.True;
            patientBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;

            // accountInformationBand Properties
            accountInformationBand.ColHeadersVisible = false;
            accountInformationBand.Indentation = 14;
            accountInformationBand.Override = base.OverrideWithoutBorder;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_BLANK].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_AGE_LABEL].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_AGE].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_OTHER_LABEL].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_OTHER].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_PHY_LABEL].
                CellMultiLine = DefaultableBoolean.True;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_PHYS].
                CellMultiLine = DefaultableBoolean.True; 

            // lastBand Properties
            lastBand.ColHeadersVisible = false;
            lastBand.Indentation = 0;
            lastBand.Override = base.OverrideWithBorder;

            SetColumnWidths();

            PrintGrid.Refresh();
        }

        private void FillDataSource()
        {
            dataSource.Rows.Clear();
            string [] patientNameAndLocation;
            string [] consultingPhysiciansList;
            string attendingPhysicianName;
            string consultingPhysicianNames;
            string physicianNames;
            string gender;
            string dischargeStatus;
            string location;
            string patientType;
            string hsv;
            string empty = new string( ' ',80);

            UltraDataRowsCollection accountRows;
            UltraDataRow accountRow;
            UltraDataRowsCollection patientRows;
            UltraDataRow patientRow;
            UltraDataRowsCollection summaryRows;
            UltraDataRow summaryRow;
            
            dataSource.Rows.Add( new string[] {"Patients:  " 
                                                  + this.SearchCriteria[0], String.Empty } );
            dataSource.Rows.Add( new string[] {"Admit date: " + this.SearchCriteria[1], 
                                                  "No. of records: " + this.Model.Count } );                                                  

            patientRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey(PATIENT_BAND) );

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey(LAST_BAND) );

            summaryRow = summaryRows.Add( new string[]{ String.Empty } );

            foreach( AccountProxy accountProxy in this.Model )
            {                
                patientNameAndLocation = new string[2];
                attendingPhysicianName = String.Empty;
                consultingPhysicianNames = String.Empty;
                physicianNames = String.Empty;
                gender = String.Empty;
                dischargeStatus = String.Empty;
                location = String.Empty;
                patientType = String.Empty;
                hsv = String.Empty;

                patientNameAndLocation[0] = accountProxy.AddOnlyLegends();

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);

                if (accountProxy.Patient != null)
                {
                    patientNameAndLocation[1] = patientNameFormatter.GetFormattedPatientName();
                }

                patientRow = patientRows.Add( new string[]
                    {patientNameAndLocation[0], patientNameAndLocation[1] } );

                accountRows = patientRow.GetChildRows( ACCOUNT_INFORMATION_BAND );
                accountRow = accountRows.Add();
   
                if( accountProxy.Patient.Sex != null )
                {
                    gender = accountProxy.Patient.Sex.Code;
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
                        dischargeStatus = accountProxy.DischargeDate.ToString( "MM/dd/yyyy" );
                    }
                }
                
                if( accountProxy.KindOfVisit != null )
                {
                    patientType = String.Format( "{0} {1}",
                        accountProxy.KindOfVisit.Code,
                        accountProxy.KindOfVisit.Description );
                }

                if( accountProxy.HospitalService != null )
                {
                    hsv =  String.Format( "{0} {1}",
                        accountProxy.HospitalService.Code,
                        accountProxy.HospitalService.Description );
                }

                if( !accountProxy.OptOutOnLocation )
                {
                    location = accountProxy.Location.ToString();
                }                
                
                if( accountProxy.AttendingPhysician != null )
                {
                    attendingPhysicianName = accountProxy.AttendingPhysician.ToString();
                }                
               
                if( accountProxy.ConsultingPhysicians != null )
                {
                    consultingPhysiciansList = 
                        new string[accountProxy.ConsultingPhysicians.Count];
                    for( int index = 0; index < accountProxy.ConsultingPhysicians.Count; index++ )
                    {
                        if( accountProxy.ConsultingPhysicians[index] != null )
                        {
                            Physician consultingPhysician = 
                                ( Physician )accountProxy.ConsultingPhysicians[index];
                            consultingPhysiciansList[index] = consultingPhysician.ToString();
                        }
                    }
                    
                    consultingPhysicianNames = string.Join( "\n", consultingPhysiciansList );
                }
                
                if( consultingPhysicianNames.Trim().Equals( string.Empty ) )
                {
                    physicianNames = attendingPhysicianName;
                }
                
                else
                {
                    physicianNames = String.Format( "{0}\n{1}",
                        attendingPhysicianName ,consultingPhysicianNames );
                }
                accountRow[ACCTBAND_GRIDCOL_ACCT_BLANK] = empty;
                    
                accountRow[ACCTBAND_GRIDCOL_ACCT_AGE_LABEL] = 
                    String.Format( "Account:   \nGender: {0}", gender );

                accountRow[ACCTBAND_GRIDCOL_ACCT_AGE] = String.Format( 
                    "{0}\nAge: {1}",
                    accountProxy.AccountNumber, accountProxy.Patient.AgeAt( 
                    DateTime.Today ).PadLeft( 4, '0').ToUpper() );
                                   
                accountRow[ACCTBAND_GRIDCOL_OTHER_LABEL] = 
                    String.Format( 
                    "Location:  \nAdmit date:  \nDisch status:  \nPatient type:  \nHSV:  " );

                accountRow[ACCTBAND_GRIDCOL_OTHER] = String.Format( 
                    "{0}\n{1}\n{2}\n{3}\n{4}",
                    location,
                    accountProxy.AdmitDate.ToString( "MM/dd/yyyy" ),
                    dischargeStatus,
                    patientType,
                    hsv );

                accountRow[ACCTBAND_GRIDCOL_PHY_LABEL] = "Att phys:\nCon phys:";

                accountRow[ACCTBAND_GRIDCOL_PHYS] = physicianNames;
            }
        }

        private void SetColumnWidths()
        {
            searchCriteriaBand.Columns[SEARCHBAND_SRCHCRIT].Width = 806;
            searchCriteriaBand.Columns[SEARCHBAND_RECORDS].Width = 232;

            patientBand.Columns[PATBAND_GRIDCOL_OPTOUT].Width = 67;
            patientBand.Columns[PATBAND_GRIDCOL_NAME].Width = 970;

            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_BLANK].Width = 80;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_AGE_LABEL].Width = 81;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_ACCT_AGE].Width = 210;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_OTHER_LABEL].Width = 82;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_OTHER].Width = 215;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_PHY_LABEL].Width = 60;
            accountInformationBand.Columns[ACCTBAND_GRIDCOL_PHYS].Width = 290;

            lastBand.Columns[LASTBAND_COL].Width = 1037;
        }

        #endregion

        #region Private Properties
        #endregion

        #region Data Elements
        private UltraDataSource dataSource;
        private UltraGridBand searchCriteriaBand;
        private UltraGridBand patientBand;
        private UltraGridBand accountInformationBand;
        private UltraGridBand lastBand;

        #endregion

        #region Constants
        private const string MSG_PENDING_DISCHARGE = "Pending",
            PENDING_DISCHARGE = "PENDINGDISCHARGE",
            HEADER_TEXT = "Census by Bloodless",
            CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";

        private const string 
            ACCTBAND_GRIDCOL_ACCT_BLANK = "Account-Blank",
            ACCTBAND_GRIDCOL_ACCT_AGE_LABEL = "Account-Gender-Age-Label",
            ACCTBAND_GRIDCOL_ACCT_AGE = "Account-Gender-Age",                
            ACCTBAND_GRIDCOL_OTHER_LABEL = "Other Information label",
            ACCTBAND_GRIDCOL_OTHER = "Other Information",
            ACCTBAND_GRIDCOL_PHY_LABEL = "Physician Information label",
            ACCTBAND_GRIDCOL_PHYS = "Physician Information",

            PATBAND_GRIDCOL_OPTOUT = "OptOutInformation",
            PATBAND_GRIDCOL_NAME = "PatientName",

            SEARCHBAND_SRCHCRIT = "SearchCriteria",
            SEARCHBAND_RECORDS = "NoOfRecords",
                                
            LASTBAND_COL = "BlankColumn";

        private const string LAST_BAND = "Last_Band",
            SEARCH_CRITERIA_BAND = "SearchCriteria_Band",
            PATIENT_BAND = "Patient_Band",
            ACCOUNT_INFORMATION_BAND = "AccountInformation_Band";            

        #endregion        
    }
}
