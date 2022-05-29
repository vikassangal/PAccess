using System;
using System.Collections;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;

namespace PatientAccess.UI.CensusInquiries
{
	/// <summary>
	/// Summary description for ADTCensusResults.
	/// </summary>
	public abstract class ADTCensusResults
	{

        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public void UpdateGrid( GridControl ADTGrid )
        {
            i_ADTGrid = ADTGrid;
        }

        public void UpdateView()
        {
            this.dataSource = null;
            CreateDataStructure();
            i_ADTGrid.CensusGrid.DataSource = dataSource;
            FillDataSource();
            CustomizeGridLayout();
            i_ADTGrid.CensusGrid.Focus(); 
        }

        #endregion

        #region Properties

        public UltraGrid ADTGrid
        {
            get
            {
                return i_ADTGrid.CensusGrid;
            }
        }

        public ArrayList AccountProxies
        {
            private get
            {
                return i_AccountProxy;
            }
            set
            {
                i_AccountProxy = value;
            }
        }

        public string ADTType
        {
            get
            {
                return i_ADTType;
            }
            set
            {
                i_ADTType = value;
            }
        }

        public string PreviousSelectedAccount
        {
            set
            {
                i_previousSelectedAccount = value;
            }
        }

        #endregion

        #region Private Methods

        private void CreateDataStructure()
        {
            dataSource              = new UltraDataSource();
            UltraDataBand ADTBand   = this.dataSource.Band;
            ADTBand.Key             = ADT_BAND;
            ADTBand.Columns.Add( COL_CONFIDENTIAL );
            ADTBand.Columns.Add( COL_ADT_TYPE );
            ADTBand.Columns.Add( COL_TRANSACTION_TIME );
            ADTBand.Columns.Add( COL_PATIENT_NAME_ACCOUNT );
			ADTBand.Columns.Add( COL_TRANSACTION_TIME_Hidden );
			ADTBand.Columns.Add( COL_PATIENT_NAME_ACCOUNT_Hidden );
            ADTBand.Columns.Add( COL_PATIENT_ACCOUNT );
            ADTBand.Columns.Add( COL_PATIENT_TYPE );
            ADTBand.Columns.Add( COL_LOCATION );
            ADTBand.Columns.Add( COL_LOCATION_FROM );
            ADTBand.Columns.Add( COL_LOCATION_TO );
            ADTBand.Columns.Add( COL_PHYSICIANS );
            ADTBand.Columns.Add( COL_CHIEF_COMPLAINT );
            ADTBand.Columns.Add( COL_DISCH_DISPOSITION );
        }

        public virtual void CustomizeGridLayout()
        {
            UltraGridBand ADT_Band;
            ADT_Band = i_ADTGrid.CensusGrid.DisplayLayout.Bands[ADT_BAND];

            ADT_Band.Columns[COL_CONFIDENTIAL].Width          = 25;
            ADT_Band.Columns[COL_ADT_TYPE].Width              = 20;
            ADT_Band.Columns[COL_TRANSACTION_TIME].Width      = 20;
            ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT].Width  = 100;
			ADT_Band.Columns[COL_TRANSACTION_TIME_Hidden].Width      = 20;
			ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT_Hidden].Width  = 100;
            ADT_Band.Columns[COL_PATIENT_ACCOUNT].Width       = 0;
			ADT_Band.Columns[COL_PATIENT_TYPE].Width          = 45;
            ADT_Band.Columns[COL_LOCATION].Width              = 30;
            ADT_Band.Columns[COL_LOCATION_FROM].Width         = 35;
            ADT_Band.Columns[COL_LOCATION_TO].Width           = 35;
            ADT_Band.Columns[COL_PHYSICIANS].Width            = 110;
            ADT_Band.Columns[COL_CHIEF_COMPLAINT].Width       = 85;
            ADT_Band.Columns[COL_DISCH_DISPOSITION].Width     = 70;

            ADT_Band.Columns[COL_CONFIDENTIAL].Hidden          = true;
            ADT_Band.Columns[COL_ADT_TYPE].Hidden              = true;
            ADT_Band.Columns[COL_TRANSACTION_TIME].Hidden      = true;
            ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT].Hidden  = true;
			ADT_Band.Columns[COL_TRANSACTION_TIME_Hidden].Hidden      = true;
			ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT_Hidden].Hidden  = true;
            ADT_Band.Columns[COL_PATIENT_ACCOUNT].Hidden       = true;
            ADT_Band.Columns[COL_PATIENT_TYPE].Hidden          = true;
            ADT_Band.Columns[COL_LOCATION].Hidden              = true;
            ADT_Band.Columns[COL_LOCATION_FROM].Hidden         = true;
            ADT_Band.Columns[COL_LOCATION_TO].Hidden           = true;
            ADT_Band.Columns[COL_PHYSICIANS].Hidden            = true;
            ADT_Band.Columns[COL_CHIEF_COMPLAINT].Hidden       = true;
            ADT_Band.Columns[COL_DISCH_DISPOSITION].Hidden     = true;

            ADT_Band.Columns[COL_PATIENT_NAME_ACCOUNT].CellMultiLine = DefaultableBoolean.True;
            ADT_Band.Columns[COL_PHYSICIANS].CellMultiLine           = DefaultableBoolean.True;       
        }

        private void FillDataSource()
        {
            UltraDataRow ADTRow;
            int rowNumber = 0;
            string admittingPhysician   = String.Empty;
            string attendingPhysician   = String.Empty;
            string physicians           = String.Empty; 
            string patientType          = String.Empty;
            string patientName          = String.Empty;

            dataSource.Rows.Clear();
            ArrayList allAccountProxies = this.AccountProxies;

            if( allAccountProxies == null || allAccountProxies.Count <= 0 )
            {
                return;
            }
            foreach( AccountProxy accountProxy in allAccountProxies )
            {
                ADTRow = dataSource.Rows.Add();

                admittingPhysician = "Adm:     ";
                attendingPhysician = "Att:        ";
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

                var patientNameFormatter = new PatientNameFormatterForCensusReports(accountProxy,false);
                patientName = patientNameFormatter.GetFormattedPatientName();

                ADTRow[COL_CONFIDENTIAL] = accountProxy.AddOnlyLegends();
                ADTRow[COL_ADT_TYPE]     = accountProxy.TransactionType;
            
                ADTRow[COL_TRANSACTION_TIME] = 
                    accountProxy.TransactionTime;
            
                ADTRow[COL_PATIENT_NAME_ACCOUNT] = 
                    patientName + 
                    "\n          Account:  " + accountProxy.AccountNumber;
				ADTRow[COL_TRANSACTION_TIME_Hidden] = 
					accountProxy.TransactionTime;
            
				ADTRow[COL_PATIENT_NAME_ACCOUNT_Hidden] = 
					patientName + 
					"\n          Account:  " + accountProxy.AccountNumber;
                
                ADTRow[COL_PATIENT_ACCOUNT] = accountProxy.AccountNumber.ToString();
            
                ADTRow[COL_PATIENT_TYPE]    = patientType;
            
                if( accountProxy.LocationFrom != null )
                {
                    ADTRow[COL_LOCATION] = accountProxy.LocationFrom.ToString();
                }

                if( accountProxy.LocationFrom != null )
                {
                    ADTRow[COL_LOCATION_FROM] = 
                        accountProxy.LocationFrom.ToString();
                }
                
                if( accountProxy.LocationTo != null )
                {
                    ADTRow[COL_LOCATION_TO] = accountProxy.LocationTo.ToString();
                }

                ADTRow[COL_PHYSICIANS] = physicians;

                if( accountProxy.Diagnosis != null )
                {
                    ADTRow[COL_CHIEF_COMPLAINT] = 
                        accountProxy.Diagnosis.ChiefComplaint;
                }
                
                if( accountProxy.DischargeDisposition != null )
                {
                    ADTRow[COL_DISCH_DISPOSITION] =
                        accountProxy.DischargeDisposition.ToCodedString();
                }

                if( i_previousSelectedAccount != String.Empty &&
                    accountProxy.AccountNumber.ToString() == i_previousSelectedAccount )
                {
                    i_ADTGrid.CensusGrid.ActiveRow = 
                        (UltraGridRow)i_ADTGrid.CensusGrid.Rows[rowNumber];
                }
                rowNumber++;
            }
        }

        #endregion

        #region Private Properties
        #endregion

        #region Windows Form Designer generated code
        #endregion

        #region Construction and Finalization

        public ADTCensusResults()
        {
        }
     
        #endregion        

        #region Data Elements

        private UltraDataSource dataSource;
        private GridControl i_ADTGrid;
        private string i_ADTType;
        private string i_previousSelectedAccount;
        private ArrayList i_AccountProxy;

        #endregion

        #region Constants

        protected const string
        ADT_BAND                        = "ADT_Band",
        COL_CONFIDENTIAL                = "Prvcy Opt",
		COL_ADT_TYPE				    = "A-D-T",
		COL_TRANSACTION_TIME            = "Time",
		COL_PATIENT_NAME_ACCOUNT        = "Patient", 
	    COL_PATIENT_TYPE                = "PT",
        COL_LOCATION                    = "Location",
        COL_LOCATION_FROM               = "Location From",
        COL_LOCATION_TO                 = "Location To",
        COL_PHYSICIANS                  = "Physicians",
        COL_CHIEF_COMPLAINT             = "Chief Complaint",
        COL_DISCH_DISPOSITION           = "Discharge Disposition";

        private const string
        COL_TRANSACTION_TIME_Hidden = "Time_Hidden",
        COL_PATIENT_NAME_ACCOUNT_Hidden = "Patient_Hidden",
        COL_PATIENT_ACCOUNT = "Account";

	    #endregion
    }
}
