using System;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.CensusInquiries
{
    [UsedImplicitly]
	public class NursingStationByDepartmentReport : CensusByNursingStationReport
	{
        #region Events
        #endregion

        #region EventHandlers
        #endregion

        #region Methods
        public override void CustomizeGridLayout()
        {
            base.CustomizeGridLayout();
            UltraGridBand patBand = base.PrintGrid.DisplayLayout.Bands[PATIENT_BAND];

            patBand.Columns[GRIDCOL_UNOC_PATIENT_TYPE].Hidden = true;
            patBand.Override.HeaderAppearance.TextHAlign = HAlign.Left;

            UltraGridGroup groupPrvcyOpt = new UltraGridGroup(GRIDCOL_ACCT_PRVCY, 1 ); 
            UltraGridGroup groupLocation = new UltraGridGroup(GRIDCOL_PATIENT_LOCATION, 2);
            UltraGridGroup groupNameAgeGen = new UltraGridGroup(GRIDCOL_PATIENT_PATIENT_NAME, 3);
            UltraGridGroup groupportalOptin = new UltraGridGroup(GRIDCOL_PORTAL_OPTIN, 4);
            UltraGridGroup groupOverflow = new UltraGridGroup(GRIDCOL_PATIENT_OVERFLOW, 5);
            UltraGridGroup groupRC = new UltraGridGroup(GRIDCOL_PATIENT_ROOMCONDITION, 6);
            UltraGridGroup groupIsol = new UltraGridGroup(GRIDCOL_ISOLATION, 7);
            UltraGridGroup groupAcco = new UltraGridGroup(GRIDCOL_PATIENT_ACCOMMODATION, 8);
            UltraGridGroup groupHSVcode = new UltraGridGroup(GRIDCOL_HSV_CODE, 9);
            UltraGridGroup groupAttPhy = new UltraGridGroup(GRIDCOL_PATIENT_ATTNPHYSICIAN, 10);
            

            patBand.Groups.Add(groupPrvcyOpt);
            patBand.Groups.Add(groupLocation);
            patBand.Groups.Add(groupNameAgeGen);
            patBand.Groups.Add(groupportalOptin);
            patBand.Groups.Add(groupOverflow);
            patBand.Groups.Add(groupRC);
            patBand.Groups.Add(groupIsol);
            patBand.Groups.Add(groupAcco);
            patBand.Groups.Add(groupHSVcode);
            patBand.Groups.Add(groupAttPhy);

            groupPrvcyOpt.Columns.Add(patBand.Columns[GRIDCOL_ACCT_PRVCY], 0 );
            groupLocation.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_LOCATION], 0 );
            groupNameAgeGen.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_PATIENT_NAME], 0 );
            groupportalOptin.Columns.Add(patBand.Columns[GRIDCOL_PORTAL_OPTIN], 0);
            //groupNameAgeGen.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_GEN_AGE_LABEL], 1 );
            //groupNameAgeGen.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_GEN_AGE], 2 );
            groupOverflow.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_OVERFLOW], 0 );
            groupRC.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_ROOMCONDITION], 0 );
            groupIsol.Columns.Add(patBand.Columns[GRIDCOL_ISOLATION], 0 );
            groupAcco.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_ACCOMMODATION], 0 );
            groupHSVcode.Columns.Add(patBand.Columns[GRIDCOL_HSV_CODE], 0);
            groupAttPhy.Columns.Add(patBand.Columns[GRIDCOL_PATIENT_ATTNPHYSICIAN], 0 );

            patBand.Columns[GRIDCOL_ACCT_PRVCY].Width = 70;
            patBand.Columns[GRIDCOL_PATIENT_LOCATION].Width = 90;
            patBand.Columns[GRIDCOL_PATIENT_PATIENT_NAME].Width = 290;
            patBand.Columns[GRIDCOL_PORTAL_OPTIN].Width = 62;
//            patBand.Columns[GRIDCOL_PATIENT_GEN_AGE_LABEL].Width = 60;
//            patBand.Columns[GRIDCOL_PATIENT_GEN_AGE].Width = 40;
            patBand.Columns[GRIDCOL_PATIENT_OVERFLOW].Width = 25; 
            patBand.Columns[GRIDCOL_PATIENT_ROOMCONDITION].Width = 20;
            patBand.Columns[GRIDCOL_ISOLATION].Width = 50;
            patBand.Columns[GRIDCOL_PATIENT_ACCOMMODATION].Width = 110;
            patBand.Columns[GRIDCOL_HSV_CODE].Width = 125;
            patBand.Columns[GRIDCOL_PATIENT_ATTNPHYSICIAN].Width = 200;
        }

        public override void FillDataSource()
        {
            dataSource.Rows.Clear();
            string attendingPhysicianName;
            string gender;
            string patientType;
            string patientName;
            string accountNumber;
            string age;
            
            dataSource.Rows.Add( new string[] { "Bed type:  " 
                                                  + base.SearchCriteria[0], String.Empty } );
            dataSource.Rows.Add( new string[] { "Nursing station: " + base.SearchCriteria[1], 
                                                  "No. of records: " + base.Model.Count } );                                                 
            UltraDataRowsCollection patientRows = dataSource.Rows[dataSource.Rows.Count - 1].
                GetChildRows( dataSource.GetBandByKey( PATIENT_BAND ) );            

            foreach( AccountProxy accountProxy in base.Model )
            {
                UltraDataRow patientRow = patientRows.Add();
                attendingPhysicianName = String.Empty;
                gender = String.Empty;
                patientType = String.Empty;
                patientName = String.Empty;
                accountNumber = String.Empty;
                age = String.Empty;
                
                patientName = accountProxy.Patient.Name.AsFormattedName();
                
                if( accountProxy.Location != null )
                {
                    patientRow[ GRIDCOL_PATIENT_LOCATION ] = 
                        accountProxy.Location.ToString();

                    if( accountProxy.Location.Room != null 
                        && accountProxy.Location.Room.RoomCondition != null )
                    {
                        patientRow[ GRIDCOL_PATIENT_ROOMCONDITION ] = 
                            accountProxy.Location.Room.RoomCondition.Code;
                    }
                    
                    if( accountProxy.Location.Bed != null
                        && accountProxy.Location.Bed.Accomodation != null )
                    {
                        patientRow[ GRIDCOL_PATIENT_ACCOMMODATION ] = 
                            String.Format( "{0} {1}", 
                            accountProxy.Location.Bed.Accomodation.Code,
                            accountProxy.Location.Bed.Accomodation.Description );
                    }
                }                

                if( accountProxy.PendingAdmission.Equals( "A" ) )
                {
                    patientRow[ GRIDCOL_PATIENT_PATIENT_NAME ] = PENDING_ADMISSION;
                }
                
                else
                {       
                    
                    if( accountProxy.AccountNumber != 0 )
                    {
                        accountNumber = accountProxy.AccountNumber.ToString();
                        
                        if( accountProxy.Patient.Sex != null )
                        {
                            gender = accountProxy.Patient.Sex.Code;
                        }   
                        
                        if( accountProxy.KindOfVisit != null )
                        {
                            patientType = String.Format( "{0} {1}",
                                accountProxy.KindOfVisit.Code ,
                                accountProxy.KindOfVisit.Description );
                        }

                        var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);
                        patientName = patientNameFormatter.GetFormattedPatientName();

                        if( !accountProxy.Patient.DateOfBirth.Equals( DateTime.MinValue ) )
                        {
                            age = accountProxy.Patient.AgeAt
                                ( DateTime.Today ).PadLeft( 4, '0').ToUpper();
                        }

                        if( accountProxy.AttendingPhysician != null )
                        {
                            attendingPhysicianName = 
                                accountProxy.AttendingPhysician.ToString();
                        }
                        patientRow[GRIDCOL_HSV_CODE] = accountProxy.HospitalService.DisplayString;
                        patientRow[ GRIDCOL_PATIENT_ATTNPHYSICIAN ] = attendingPhysicianName;
                        patientRow[GRIDCOL_ACCT_PRVCY] = accountProxy.AddOnlyLegends();
                        patientRow[ GRIDCOL_PATIENT_OVERFLOW ] = accountProxy.Overflow;

                        patientRow[ GRIDCOL_PATIENT_PATIENT_NAME ] = String.Format( 
                            "{0} \n   Account:          {1}  Gen:  {2}  \n   Patient Type:    {3}  Age:  {4}",
                            patientName, accountNumber.PadRight(40,' '), gender, patientType.PadRight(35,' '), age );

                        if (accountProxy.KindOfVisit != null &&
                            (accountProxy.KindOfVisit.Equals(VisitType.Inpatient) ||
                            accountProxy.KindOfVisit.Equals(VisitType.Outpatient) ||
                            accountProxy.KindOfVisit.Equals(VisitType.Recurring)) &&
                            accountProxy.PatientPortalOptIn != null &&
                            accountProxy.PatientPortalOptIn.Code != string.Empty )
                        {
                            patientRow[GRIDCOL_PORTAL_OPTIN] = accountProxy.PatientPortalOptIn.Code;
                        }
                    }
                    
                    else
                    {
                        patientRow[ GRIDCOL_PATIENT_PATIENT_NAME ] = String.Format( 
                            "\n   Account:          {0}  Gen: \n   Patient Type:    {1}  Age: ",
                            new String(' ',44), new String(' ', 43 ) );

                    }
                    patientRow[ GRIDCOL_ISOLATION ] = accountProxy.IsolationCode;

                }
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Construction and Finalization
		public NursingStationByDepartmentReport()
		{
		}
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string CONFIDENTIAL_PATIENT_NAME  = "OCCUPIED";
        #endregion
	}
}
