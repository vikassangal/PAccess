using System;
using System.Collections;
using System.Drawing;
using System.Text;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.HelperClasses;

namespace PatientAccess.UI.CensusInquiries
{
    public class CensusByPayorReports :  PrintReport
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

        #region Construction and Finalization

        public CensusByPayorReports()
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
            searchBand.Columns.Add( SEARCHBAND_GRIDCOL_LABEL );
            searchBand.Columns.Add( SEARCHBAND_GRIDCOL_SRCHCRITERIA );
            searchBand.Columns.Add( SEARCHBAND_GRIDCOL_RECORDS );

            UltraDataBand planPayorPatBand = searchBand.ChildBands.Add(PAYOR_PLAN_BAND );
            planPayorPatBand.Columns.Add( PAYORBAND_GRIDCOL_NS );
            planPayorPatBand.Columns.Add( PAYORBAND_GRIDCOL_PAYOR_PLAN );
            planPayorPatBand.Columns.Add( PAYORBAND_GRIDCOL_OPTOUT );
            planPayorPatBand.Columns.Add( PAYORBAND_GRIDCOL_NAME );

            UltraDataBand accountBand = planPayorPatBand.ChildBands.Add( ACCOUNT_INFORMATION_BAND );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_MRN_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCT_MRN );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_MISC_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_MISC );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_ACCO_HSV  );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL );
            accountBand.Columns.Add( ACCTBAND_GRIDCOL_PLAN_DETAILS );

            UltraDataBand attPhyBand = accountBand.ChildBands.Add( ATTENDING_PHYSICIAN_BAND );
            attPhyBand.Columns.Add( ATTPHYBAND_ATT_PHY_LABEL );
            attPhyBand.Columns.Add( ATTPHYBAND_ATT_PHYS );
            attPhyBand.Columns.Add( ATTPHYBAND_TOT_CURR_ACCT_PMT_LABEL );
            attPhyBand.Columns.Add( ATTPHYBAND_TOT_CURR_ACCT_PMT );

            UltraDataBand lastBand = searchBand.ChildBands.Add( LAST_BAND );
            lastBand.Columns.Add( LASTBAND_COL );
        }

        private void CustomizeGridLayout()
        {
            UltraGridBand searchCriteriaGridBand = new UltraGridBand( SEARCH_CRITERIA_BAND, 0);
            UltraGridBand payorPlanNSGridBand = new UltraGridBand( PAYOR_PLAN_BAND, 1 );
            UltraGridBand accountInformationBand = new UltraGridBand( ACCOUNT_INFORMATION_BAND, 2 );
            UltraGridBand attendingPhysicianBand = new UltraGridBand( ATTENDING_PHYSICIAN_BAND, 3 );
            UltraGridBand lastBand = new UltraGridBand( LAST_BAND, 4);

            UltraGridColumn searchLabelGridColumn = new UltraGridColumn( SEARCHBAND_GRIDCOL_LABEL ); 
            UltraGridColumn searchValueGridColumn = new UltraGridColumn( SEARCHBAND_GRIDCOL_SRCHCRITERIA ); 
            UltraGridColumn noOfRecordsGridColumn  = new UltraGridColumn( SEARCHBAND_GRIDCOL_RECORDS ); 

            searchCriteriaGridBand.Columns.AddRange( new object[] {
                                                                      searchLabelGridColumn,
                                                                      searchValueGridColumn,
                                                                      noOfRecordsGridColumn });

            UltraGridColumn nursingStationGridColumn  = new UltraGridColumn( PAYORBAND_GRIDCOL_NS );
            UltraGridColumn payorPlanGridColumn = new UltraGridColumn( PAYORBAND_GRIDCOL_PAYOR_PLAN );
            UltraGridColumn optoutGridColumn = new UltraGridColumn( PAYORBAND_GRIDCOL_OPTOUT ); 
            UltraGridColumn patientNameGridColumn = new UltraGridColumn( PAYORBAND_GRIDCOL_NAME );

            payorPlanNSGridBand.Columns.AddRange( new object[] {
                                                                   nursingStationGridColumn,
                                                                   payorPlanGridColumn,
                                                                   optoutGridColumn,
                                                                   patientNameGridColumn });

            UltraGridColumn labelMRNGridColumn = new UltraGridColumn( ACCTBAND_GRIDCOL_ACCT_MRN_LABEL ); 
            UltraGridColumn acctMRNGridColumn = new UltraGridColumn( ACCTBAND_GRIDCOL_ACCT_MRN ); 
            UltraGridColumn labelMiscGridColumn  = new UltraGridColumn( ACCTBAND_GRIDCOL_MISC_LABEL ); 
            UltraGridColumn acctMiscGridColumn = new UltraGridColumn( ACCTBAND_GRIDCOL_MISC ); 
            UltraGridColumn acctHSVGridColumn = new UltraGridColumn( ACCTBAND_GRIDCOL_ACCO_HSV ); 
            UltraGridColumn labelPlanGridColumn  = new UltraGridColumn( ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL ); 
            UltraGridColumn planDetailsGridColumn = new UltraGridColumn( ACCTBAND_GRIDCOL_PLAN_DETAILS ); 
        
            accountInformationBand.Columns.AddRange( new object[] {
                                                                      labelMRNGridColumn,
                                                                      acctMRNGridColumn,
                                                                      labelMiscGridColumn,
                                                                      acctMiscGridColumn,
                                                                      acctHSVGridColumn,
                                                                      labelPlanGridColumn,
                                                                      planDetailsGridColumn});

            UltraGridColumn attPhyLabelGridColumn = new UltraGridColumn( ATTPHYBAND_ATT_PHY_LABEL ); 
            UltraGridColumn attPhyValueGridColumn = new UltraGridColumn( ATTPHYBAND_ATT_PHYS ); 
            UltraGridColumn totalAcctLabelGridColumn  = new UltraGridColumn( ATTPHYBAND_TOT_CURR_ACCT_PMT_LABEL ); 
            UltraGridColumn totalAcctValueGridColumn = new UltraGridColumn( ATTPHYBAND_TOT_CURR_ACCT_PMT ); 

            attendingPhysicianBand.Columns.AddRange( new object[] {
                                                                      attPhyLabelGridColumn,
                                                                      attPhyValueGridColumn,
                                                                      totalAcctLabelGridColumn,
                                                                      totalAcctValueGridColumn });

            UltraGridColumn lastBandGridColumn = new UltraGridColumn( LASTBAND_COL ); 

            lastBand.Columns.AddRange( new object[] {
                                                        lastBandGridColumn });

            //set Grid's DisplayLayout properties                   
            printLayout = PrintGrid.DisplayLayout;
        
            printLayout.BandsSerializer.Add( payorPlanNSGridBand );
            printLayout.BandsSerializer.Add( accountInformationBand );
            printLayout.BandsSerializer.Add( attendingPhysicianBand );
            printLayout.BandsSerializer.Add( lastBand );
            
            searchBand = printLayout.Bands[SEARCH_CRITERIA_BAND];
            payorPlanBand = printLayout.Bands[PAYOR_PLAN_BAND];
            accountBand = printLayout.Bands[ACCOUNT_INFORMATION_BAND];
            attPhyBand = printLayout.Bands[ATTENDING_PHYSICIAN_BAND];
            summBand = printLayout.Bands[LAST_BAND];

            SetBandProperties();
            SetColumnWidths(); 

            if( this.SearchCriteria[SEARCHCRITERIA_SORTEDBY].ToString().Equals( SORT_BY_PAYOR ) )
            {
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[PAYORBAND_GRIDCOL_NS].Hidden = true;
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[PAYORBAND_GRIDCOL_PAYOR_PLAN].GroupByMode = 
                    GroupByMode.Text;
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[PAYORBAND_GRIDCOL_NAME].SortIndicator = 
                    SortIndicator.Ascending;                
                printLayout.Bands[PAYOR_PLAN_BAND].SortedColumns.Add( PAYORBAND_GRIDCOL_PAYOR_PLAN, false, true); 
            }
            
            else
            {                
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[ PAYORBAND_GRIDCOL_NS ].GroupByMode = 
                    GroupByMode.Text;
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[ PAYORBAND_GRIDCOL_PAYOR_PLAN ].GroupByMode = 
                    GroupByMode.Text;
                printLayout.Bands[PAYOR_PLAN_BAND].Columns[PAYORBAND_GRIDCOL_NAME].SortIndicator = 
                    SortIndicator.Ascending;
                printLayout.Bands[PAYOR_PLAN_BAND].SortedColumns.Add( PAYORBAND_GRIDCOL_NS , false, true);
                printLayout.Bands[PAYOR_PLAN_BAND].SortedColumns.Add( PAYORBAND_GRIDCOL_PAYOR_PLAN , false, true); 
            }

            this.PrintGrid.InitializeGroupByRow += 
                new InitializeGroupByRowEventHandler( this.SetGroupAppearance );
            PrintGrid.Refresh(); 
        }

        private void FillDataSource()
        {
            ArrayList coverageCategories;
            ArrayList nursingStations;
            StringBuilder allCoverageCategories = new StringBuilder();
            StringBuilder allNursingStations = new StringBuilder();
            UltraDataRowsCollection planRows;
            UltraDataRow planRow;
            UltraDataRowsCollection accountRows;
            UltraDataRow accountRow;
            UltraDataRowsCollection physicianRows;
            UltraDataRow physicianRow;
            UltraDataRowsCollection summaryRows;
            UltraDataRow summaryRow;
            string nsCode;
            string accomodation;
            string gender;
            string dischargeStatus;
            string location;
            string patientType;
            string hsv;
            string financialClass;
            string secondaryPlan;
            string amtDue;
            string payment;
            string legend;
            string attendingPhysicianName;
            string patientName;
            string sortBy = String.Empty;
          
            coverageCategories = ( ArrayList )SearchCriteria[SEARCHCRITERIA_COVERAGE_CATEGORIES];
            nursingStations = ( ArrayList )SearchCriteria[SEARCHCRITERIA_NURSINGSTATIONS];

            foreach( string s in coverageCategories )
            {
                allCoverageCategories.Append( s + ", " );
            }
            foreach( string s in nursingStations )
            {
                allNursingStations.Append( s + ", " );
            }

            allCoverageCategories.Remove( allCoverageCategories.Length - 2, 2 );
            allNursingStations.Remove( allNursingStations.Length - 2, 2 );
            
            dataSource.Rows.Add( 
                new string[] {"Coverage category: ", allCoverageCategories.ToString(),
                                 String.Empty } );
            dataSource.Rows.Add( 
                new string[] {"Nursing station: ", allNursingStations.ToString(), 
                                 String.Empty } );

            if( SearchCriteria[SEARCHCRITERIA_SORTEDBY].Equals( SORT_BY_PAYOR ) )
            {
                sortBy = PRIMARY_PAYOR;
            }
            
            else
            {
                sortBy = NURSING_STATION;
            }
            dataSource.Rows.Add( 
                new string[] {"Sorted by: ", sortBy, 
                                 "No. of records: " + this.Model.Count } );

            planRows = dataSource.Rows[dataSource.Rows.Count - 1].
                GetChildRows( PAYOR_PLAN_BAND );

            summaryRows = dataSource.Rows[dataSource.Rows.Count - 1].GetChildRows( 
                dataSource.GetBandByKey( LAST_BAND ) );

            summaryRow = summaryRows.Add( new string[]{ String.Empty } );

            foreach( AccountProxy accountProxy in this.Model )
            {                
                accomodation = String.Empty;       
                gender = String.Empty;
                dischargeStatus = String.Empty;
                location = String.Empty;
                patientType = String.Empty;
                hsv = String.Empty;
                financialClass = String.Empty;
                secondaryPlan = String.Empty;
                amtDue = String.Empty;
                payment = String.Empty;
                attendingPhysicianName = String.Empty;
                patientName = String.Empty;
                legend = String.Empty;
                nsCode = String.Empty;

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);
                patientName = patientNameFormatter.GetFormattedPatientName();

                planRow = planRows.Add();

                planRow[PAYORBAND_GRIDCOL_NS] = "Nursing Station: " + 
                    accountProxy.Location.NursingStation.Code;
                planRow[PAYORBAND_GRIDCOL_PAYOR_PLAN] = "Primary Payor: " + 
                    accountProxy.PayorName.Trim() + 
                    "                Plan: " + accountProxy.PrimaryPlanName.Trim();
                planRow[PAYORBAND_GRIDCOL_OPTOUT] = accountProxy.AddOnlyLegends();
                planRow[PAYORBAND_GRIDCOL_NAME] = patientName;                
        
                if( accountProxy.Patient.Sex != null )
                {
                    gender = accountProxy.Patient.Sex.Code;
                }

                if( accountProxy.Location != null )
                {
                    location = accountProxy.Location.ToString();
                }

                if( accountProxy.AttendingPhysician != null )
                {
                    attendingPhysicianName = accountProxy.AttendingPhysician.ToString();
                }

                if( accountProxy.Location.Bed != null
                    && accountProxy.Location.Bed.Accomodation != null )
                {
                    accomodation = String.Format( "{0} {1}",
                        accountProxy.Location.Bed.Accomodation.Code,
                        accountProxy.Location.Bed.Accomodation.Description );
                }

                if( accountProxy.HospitalService != null )
                {
                    hsv = String.Format( "{0}  {1}",
                        accountProxy.HospitalService.Code, 
                        accountProxy.HospitalService.Description );
                }
                        
                if( accountProxy.FinancialClass != null )
                {
                    financialClass = String.Format( "{0}  {1}",
                        accountProxy.FinancialClass.Code,
                        accountProxy.FinancialClass.Description );
                }

                if( accountProxy.SecondaryPlan.Trim().Length > 0 )
                {
                    secondaryPlan = String.Format( "{0}  {1}",
                        accountProxy.SecondaryPlan.Trim(), 
                        accountProxy.SecondaryPlanName );
                }
                
                else
                {
                    secondaryPlan = String.Format( accountProxy.SecondaryPlanName );
                }

                amtDue = accountProxy.AmountDue.ToString("C");
                payment = accountProxy.Payments.ToString("C");

                accountRows = planRow.GetChildRows( ACCOUNT_INFORMATION_BAND );
                accountRow = accountRows.Add();

                accountRow[ACCTBAND_GRIDCOL_ACCT_MRN_LABEL] = String.Format( 
                    "Account: \nMRN: \nGender: {0}", gender );
                accountRow[ACCTBAND_GRIDCOL_ACCT_MRN] = String.Format
                    ( "{0}\n{1}\nAge: {2}", 
                    accountProxy.AccountNumber,
                    accountProxy.Patient.MedicalRecordNumber, 
                    accountProxy.Patient.AgeAt( DateTime.Today ).PadLeft( 4, '0').ToUpper() );
                
                accountRow[ACCTBAND_GRIDCOL_MISC_LABEL] = 
                    "Location: \nAdmit date/time: \nLength of stay: ";

                accountRow[ACCTBAND_GRIDCOL_MISC] = String.Format( "{0}\n{1}\n{2}", 
                    location,
                    accountProxy.AdmitDate.ToString( "MM/dd/yyyy HH:mm" ),
                    accountProxy.LengthOfStay );

                accountRow[ACCTBAND_GRIDCOL_ACCO_HSV] = String.Format( 
                    "Accommodation: {0}\nHSV: {1}", accomodation, hsv );

                accountRow[ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL] = 
                    "Financial class: \nSecondary plan: \nTotal curr acct due: ";

                accountRow[ACCTBAND_GRIDCOL_PLAN_DETAILS] = String.Format( "{0}\n{1}\n{2}", 
                    financialClass,
                    secondaryPlan, 
                    amtDue );

                physicianRows = accountRow.GetChildRows( ATTENDING_PHYSICIAN_BAND );
                physicianRow = physicianRows.Add();

                physicianRow[ATTPHYBAND_ATT_PHY_LABEL] = "Attending physician: ";
                physicianRow[ATTPHYBAND_ATT_PHYS] = attendingPhysicianName;
                physicianRow[ATTPHYBAND_TOT_CURR_ACCT_PMT_LABEL] = "Total curr acct pmt: ";
                physicianRow[ATTPHYBAND_TOT_CURR_ACCT_PMT] = payment;
            }
        }
        private void SetGroupAppearance(object sender, InitializeGroupByRowEventArgs e)
        {
            Appearance groupAppearance = new Appearance();

            if( e.Row.Column.Key.Equals( PAYORBAND_GRIDCOL_NS ) )
            {
                groupAppearance.BackColor = Color.FromArgb( 140,140,140 );
                
            }
            
            else if( e.Row.Column.Key.Equals( PAYORBAND_GRIDCOL_PAYOR_PLAN ) )
            {                 
                groupAppearance.BackColor = Border_Color;
            }                 
               
            groupAppearance.ForeColor = Color.White;
            e.Row.Appearance = groupAppearance;
        }

        private void SetColumnWidths()
        {            
            searchBand.Columns[SEARCHBAND_GRIDCOL_LABEL].Width = 125;
            searchBand.Columns[SEARCHBAND_GRIDCOL_SRCHCRITERIA].Width = 712;
            searchBand.Columns[SEARCHBAND_GRIDCOL_RECORDS].Width = 200;
            
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_NS].Width = 100;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_PAYOR_PLAN].Width = 265;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_OPTOUT].Width = 62;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_NAME].Width = 975;
            
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCT_MRN_LABEL].Width = 75;
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCT_MRN].Width = 100;
            accountBand.Columns[ACCTBAND_GRIDCOL_MISC_LABEL].Width = 125;
            accountBand.Columns[ACCTBAND_GRIDCOL_MISC].Width = 135;
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCO_HSV].Width = 230;
            accountBand.Columns[ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL].Width = 130;
            accountBand.Columns[ACCTBAND_GRIDCOL_PLAN_DETAILS].Width = 227;
                        
            attPhyBand.Columns[ATTPHYBAND_ATT_PHY_LABEL].Width = 125;
            attPhyBand.Columns[ATTPHYBAND_ATT_PHYS].Width = 365;
            attPhyBand.Columns[ATTPHYBAND_TOT_CURR_ACCT_PMT_LABEL].Width = 129;
            attPhyBand.Columns[ATTPHYBAND_TOT_CURR_ACCT_PMT].Width = 228;
            
            summBand.Columns[LASTBAND_COL].Width = 1037;
        }

        private void SetBandProperties()
        {
            // searchCriteriaBand Properties
            searchBand.ColHeadersVisible = false;
            searchBand.Indentation = 0;
            searchBand.Override = base.OverrideWithoutBorder;
            searchBand.Columns[SEARCHBAND_GRIDCOL_SRCHCRITERIA].CellMultiLine = DefaultableBoolean.True;            
            searchBand.Columns[SEARCHBAND_GRIDCOL_RECORDS].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;

            // payorPlanBand Properties
            payorPlanBand.ColHeadersVisible = false;
            payorPlanBand.Indentation = 0;
            payorPlanBand.IndentationGroupByRow = 0;
            payorPlanBand.Override = base.OverrideWithBorder;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_OPTOUT].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_OPTOUT].CellAppearance.TextVAlign = VAlign.Bottom;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_NAME].CellAppearance.FontData.Bold = 
                DefaultableBoolean.True;
            payorPlanBand.Columns[PAYORBAND_GRIDCOL_NAME].CellAppearance.TextVAlign = VAlign.Bottom;

            // accountInformationBand Properties
            accountBand.ColHeadersVisible = false;
            accountBand.Indentation = 14;
            accountBand.Override = base.OverrideWithoutBorder;
            accountBand.Override.CellAppearance.TextVAlign = VAlign.Bottom;
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCT_MRN_LABEL].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCT_MRN].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_MISC_LABEL].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_MISC].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_ACCO_HSV].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL].CellMultiLine = DefaultableBoolean.True;
            accountBand.Columns[ACCTBAND_GRIDCOL_PLAN_DETAILS].CellMultiLine = DefaultableBoolean.True;

            // attendingPhysicianBand Properties
            attPhyBand.ColHeadersVisible = false;
            attPhyBand.Indentation = 175;
            attPhyBand.Override = base.OverrideWithoutBorder;
            attPhyBand.Override.CellAppearance.TextVAlign = VAlign.Top;

            // summBand Properties
            summBand.ColHeadersVisible = false;
            summBand.Indentation = 0;             
            summBand.Override = base.OverrideWithBorder;            
            
            printLayout.Override.GroupByRowExpansionStyle = GroupByRowExpansionStyle.Disabled;
            printLayout.Override.GroupByRowDescriptionMask = "[value]";            
            printLayout.AutoFitStyle = AutoFitStyle.ExtendLastColumn;
            printLayout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;  
        }
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements

        private UltraGridBand searchBand;
        private UltraGridBand payorPlanBand;
        private UltraGridBand accountBand;
        private UltraGridBand attPhyBand;
        private UltraGridBand summBand;
        private UltraGridLayout printLayout;
        private UltraDataSource dataSource;
        #endregion

        #region Constants

        private const string HEADER_TEXT = "Census by Payor",
            SORT_BY_PAYOR = "C",
            PRIMARY_PAYOR = "Primary payor",
            NURSING_STATION = "Nursing station",
            CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";

        private const string            
            ATTPHYBAND_ATT_PHY_LABEL = "AttendingPhysician_Label",
            ATTPHYBAND_ATT_PHYS = "AttendingPhysician_Data",
            ATTPHYBAND_TOT_CURR_ACCT_PMT_LABEL = "TotalCurrAcctPmt_Label",
            ATTPHYBAND_TOT_CURR_ACCT_PMT = "TotalCurrAcctPmt_Data",

            ACCTBAND_GRIDCOL_ACCT_MRN_LABEL = "Account-MRN-Label",
            ACCTBAND_GRIDCOL_ACCT_MRN = "Account-MRN-Data",
            ACCTBAND_GRIDCOL_MISC_LABEL = "Other_Information_Label",
            ACCTBAND_GRIDCOL_MISC = "Other_Information_Data",
            ACCTBAND_GRIDCOL_ACCO_HSV = "Accomodation-HSV_Label_Data",
            ACCTBAND_GRIDCOL_PLAN_DETAILS_LABEL = "Financial_Info_Label",
            ACCTBAND_GRIDCOL_PLAN_DETAILS = "Financial_Info_Data",

            PAYORBAND_GRIDCOL_OPTOUT = "OptOutInformation",
            PAYORBAND_GRIDCOL_NAME = "PatientName",
            PAYORBAND_GRIDCOL_NS = "Nursing Station",
            PAYORBAND_GRIDCOL_PAYOR_PLAN = "PrimaryPayor - Plan",

            SEARCHBAND_GRIDCOL_LABEL = "Coverage-NS-SortedBy-Labels",
            SEARCHBAND_GRIDCOL_SRCHCRITERIA = "Coverage-NS-SortedBy",
            SEARCHBAND_GRIDCOL_RECORDS = "No.OfRecords",

            LASTBAND_COL = "BlankColumn";

        private const string LAST_BAND = "Last_Band",
            SEARCH_CRITERIA_BAND = "SearchCriteria_Band",
            ACCOUNT_INFORMATION_BAND = "AccountInformation_Band",
            PAYOR_PLAN_BAND = "Payor-Plan_Band",
            ATTENDING_PHYSICIAN_BAND = "AttendingPhysician_Band";

        private const int SEARCHCRITERIA_COVERAGE_CATEGORIES = 0,
            SEARCHCRITERIA_NURSINGSTATIONS = 1,
            SEARCHCRITERIA_SORTEDBY = 2;

        #endregion        
    }
}
