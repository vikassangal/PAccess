using System;
using System.Collections;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.Annotations;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.UI.CensusInquiries
{
    [UsedImplicitly]
    public class NursingStationByInfoDeskReport : CensusByNursingStationReport
    {
        #region Events
        #endregion

        #region EventHandlers
        #endregion

        #region Methods
        public override void CustomizeGridLayout()
        {
            base.CustomizeGridLayout();

            UltraGridBand searchBand = base.PrintGrid.DisplayLayout.Bands[SEARCHCRITERIA_BAND];
            searchBand.Override.RowAppearance.BorderColor = Border_Color;
            searchBand.Override = base.OverrideWithBorder;

            UltraGridBand patBand = base.PrintGrid.DisplayLayout.Bands[PATIENT_BAND];            
            patBand.Columns[GRIDCOL_ACCT_PRVCY].Hidden = true;
            patBand.Columns[GRIDCOL_PATIENT_OVERFLOW].Hidden = true;
            patBand.Columns[GRIDCOL_PATIENT_ROOMCONDITION].Hidden = true;
            patBand.Columns[GRIDCOL_ISOLATION].Hidden = true;
            patBand.Columns[GRIDCOL_PATIENT_ACCOMMODATION].Hidden = true;
//            patBand.Columns[GRIDCOL_PATIENT_GEN_AGE_LABEL].Hidden = true;
//            patBand.Columns[GRIDCOL_PATIENT_GEN_AGE].Hidden = true;
            patBand.Columns[GRIDCOL_HSV_CODE].Hidden = true;

            patBand.ColHeadersVisible = true;
            patBand.Override.HeaderAppearance.TextHAlign = HAlign.Left;

            patBand.Columns[GRIDCOL_PATIENT_LOCATION].Width = 103;            
            patBand.Columns[GRIDCOL_PATIENT_PATIENT_NAME].Width = 390;          
            patBand.Columns[GRIDCOL_UNOC_PATIENT_TYPE].Width = 134;
            patBand.Columns[GRIDCOL_PORTAL_OPTIN].Width = 70;
            patBand.Columns[GRIDCOL_PATIENT_ATTNPHYSICIAN].Width = 340;

            patBand.Columns[GRIDCOL_PATIENT_PATIENT_NAME].SortIndicator = SortIndicator.Ascending;
        }

        public override void FillDataSource()
        {
            dataSource.Rows.Clear();
            string attendingPhysicianName;
            string patientType;
            
            ArrayList accountProxies = FilterAccountProxiesList( this.Model );

            dataSource.Rows.Add( new string[] { " \n              Information Desk\n " } );
            dataSource.Rows.Add( new string[] {"Bed type:  " 
                                                  + base.SearchCriteria[0] + "\nNursing station: " +
                                                  base.SearchCriteria[1], 
                                                  String.Empty + "\nNo. of records: " + 
                                                  accountProxies.Count } );
            
            UltraDataRowsCollection patientRows = dataSource.Rows[dataSource.Rows.Count - 1].
                GetChildRows( dataSource.GetBandByKey( PATIENT_BAND ) );

            foreach( AccountProxy accountProxy in accountProxies )
            {
                
                if( accountProxy.AddOnlyLegends().Equals( String.Empty ) )
                {
                    UltraDataRow patientRow = patientRows.Add();
                    attendingPhysicianName = String.Empty;
                    patientType = String.Empty;

                    if( accountProxy.Location != null )
                    {
                        patientRow[ GRIDCOL_PATIENT_LOCATION ] 
                            = accountProxy.Location.ToString();                            
                    }
                        
                    if( accountProxy.PendingAdmission.Equals( "A" ) )
                    {
                        patientRow[ GRIDCOL_PATIENT_PATIENT_NAME ] 
                            = PENDING_ADMISSION;
                    }
                    
                    else
                    {
                        
                        if( accountProxy.KindOfVisit != null )
                        {
                            patientType = String.Format( "{0} {1}",
                                accountProxy.KindOfVisit.Code , 
                                accountProxy.KindOfVisit.Description );
                        }                            
                    
                        if( accountProxy.AttendingPhysician != null )
                        {
                            patientRow[ GRIDCOL_PATIENT_ATTNPHYSICIAN] = 
                                accountProxy.AttendingPhysician.ToString();
                        }

                        patientRow[ GRIDCOL_UNOC_PATIENT_TYPE ] = patientType;

                        var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,true);

                        patientRow[GRIDCOL_PATIENT_PATIENT_NAME] = patientNameFormatter.GetFormattedPatientName();

                        if (accountProxy.KindOfVisit != null &&
                            (accountProxy.KindOfVisit.Equals(VisitType.Inpatient) ||
                            accountProxy.KindOfVisit.Equals(VisitType.Outpatient) ||
                            accountProxy.KindOfVisit.Equals(VisitType.Recurring)) &&
                            accountProxy.PatientPortalOptIn != null &&
                            accountProxy.PatientPortalOptIn.Code != string.Empty)
                        {
                            patientRow[ GRIDCOL_PORTAL_OPTIN ] = accountProxy.PatientPortalOptIn.Code;
                        }
                    }
                }
            }

        }
        #endregion

        #region Properties
        #endregion

        #region Construction and Finalization
        public NursingStationByInfoDeskReport()
        {
        }
        #endregion

        #region Private Methods
        private ArrayList FilterAccountProxiesList( ArrayList allAccountProxies )
        {
            ArrayList filteredAccountProxies = new ArrayList();
            foreach( AccountProxy aProxy in allAccountProxies )
            {
                
                if( aProxy.AddOnlyLegends().Equals( String.Empty ) )
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
        #endregion

        #region Constants
        #endregion
    }
}
