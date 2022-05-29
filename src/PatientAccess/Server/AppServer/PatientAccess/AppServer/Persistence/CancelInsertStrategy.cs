using System;
using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class CancelInsertStrategy : SqlBuilderStrategy
    {
        #region Construction and Finalization
        public CancelInsertStrategy()
        {
            InitializeColumnValues();
        }   
        #endregion

        #region Methods
        public override void UpdateColumnValuesUsing( Account account )
        {
            
            if( account != null )
            {
                //Set Transaction APACFL Add/Change flag
                if( account.Activity.GetType() == typeof( CancelPreRegActivity ) )
                {
                    this.AddChangeFlag = BLANK_FLAG;
                }
                else if( account.Activity.GetType() == typeof( CancelInpatientDischargeActivity ) )
                {
                    this.AddChangeFlag = BLANK_FLAG;
                }
                else if( account.Activity.GetType() == typeof( CancelOutpatientDischargeActivity ) )
                {
                    this.AddChangeFlag = BLANK_FLAG;
                }

                //update the local properties with values in domain objects (Account)
                if( account.Facility != null )
                {
                    this.HospitalNumber = ( int )account.Facility.Oid;
                    this.AccountNumber = ( int )account.AccountNumber;
                }
                if( account.KindOfVisit != null )
                {
                    this.PatientType = account.KindOfVisit.Code;
                }
                //Required Medical Service Code.
                if( account.HospitalService != null )
                {
                    this.MedicalServiceCode = account.HospitalService.Code;
                }

                if( account.Location != null && account.Location.Bed != null &&
                    account.Location.Bed.Accomodation != null )
                {
                    this.AfterAccomodationCode = account.Location.Bed.Accomodation.Code;
                }
                if (account.Facility != null)
                {
                    DateTime facilityDateTime = account.Facility.GetCurrentDateTime();
                    TransactionDate = facilityDateTime;
                    TimeRecordCreation = facilityDateTime;
                }
                
                if( account.Activity != null )
                {
                    User appUser = account.Activity.AppUser;
                    if( appUser != null )
                    {
                        if( appUser.WorkstationID != null )
                        {
                            this.WorkStationId = appUser.WorkstationID;
                        }
                    }
                }
              }
        }


        public override void InitializeColumnValues()
        {    
            cancelDetailsOrderedList.Add( APIDWS, string.Empty );
            cancelDetailsOrderedList.Add( APIDID, "  " );
            cancelDetailsOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER );
            cancelDetailsOrderedList.Add( APSEC2, string.Empty );
            cancelDetailsOrderedList.Add( APHSP_, 0 );
            cancelDetailsOrderedList.Add( APACCT, 0 );
            cancelDetailsOrderedList.Add( APLML, 0 );
            cancelDetailsOrderedList.Add( APLMD, 0 );
            cancelDetailsOrderedList.Add( APLUL_, 0 );
            cancelDetailsOrderedList.Add( APACFL, string.Empty );
            cancelDetailsOrderedList.Add( APTTME, 0 );
            cancelDetailsOrderedList.Add( APINLG, LOG_NUMBER);
            cancelDetailsOrderedList.Add( APBYPS, string.Empty );
            cancelDetailsOrderedList.Add( APSWPY, 0 );
            cancelDetailsOrderedList.Add( APTDAT, 0 );
            cancelDetailsOrderedList.Add( APPTYP, string.Empty );
            cancelDetailsOrderedList.Add( APMSV, string.Empty );
            cancelDetailsOrderedList.Add( APXMIT, string.Empty );
            cancelDetailsOrderedList.Add( APQNUM, 0 );
            cancelDetailsOrderedList.Add( APZDTE, string.Empty );
            cancelDetailsOrderedList.Add( APZTME, string.Empty );
            cancelDetailsOrderedList.Add( APACC, string.Empty );
            cancelDetailsOrderedList.Add( APWSIR, WORKSTATION_ID );
            cancelDetailsOrderedList.Add( APSECR, string.Empty );
            cancelDetailsOrderedList.Add( APORR1, string.Empty );
            cancelDetailsOrderedList.Add( APORR2, string.Empty );
            cancelDetailsOrderedList.Add( APORR3, string.Empty );
        }


        public override ArrayList BuildSqlFrom( Account account, TransactionKeys transactionKeys )
        {
      
//            this.RelativeRecordNumber = transactionKeys.PatientRecordNumber;
//            this.InputLogNumber = transactionKeys.LogNumber;
            
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( this.cancelDetailsOrderedList, 
                  "HPADAPCP" );
//                "HPDATA2.HPADAPCP" );
            this.SqlStatements.Add(SqlStatement);
            return SqlStatements;
        }  

        #endregion

        #region private Properties
        private int HospitalNumber
        {
            set
            {
                this.cancelDetailsOrderedList[APHSP_] = value;
            }
        }

        private int AccountNumber
        {
            set
            {
                this.cancelDetailsOrderedList[APACCT] = value;
            }
        }
        private string WorkStationId
        {
            set
            {
                this.cancelDetailsOrderedList[APIDWS] = value;
            }
        }
        private DateTime TimeRecordCreation
        {
            set
            {
                this.cancelDetailsOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }
        private DateTime TransactionDate
        {
            set
            {
                this.cancelDetailsOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string PatientType
        {
            set
            {
                this.cancelDetailsOrderedList[APPTYP] = value;
            }
        }
        
        private string MedicalServiceCode
        {
            set
            {
                this.cancelDetailsOrderedList[APMSV] = value;
            }
        }
        
        private string AfterAccomodationCode
        {
            set
            {
                this.cancelDetailsOrderedList[APACC] = value;
            }
        }

        #endregion

        #region public properties
        public string TransactionFileId
        {
            set
            {
                this.cancelDetailsOrderedList[APIDID] = value;
                
            }
        }

        public string UserSecurityCode
        {
            set
            {
                this.cancelDetailsOrderedList[APSEC2] = value;
                
            }
        }
        private string AddChangeFlag
        {
            set
            {
                this.cancelDetailsOrderedList[APACFL] = value;
               
            }
        } 
        #endregion

        #region Data Elements
        private OrderedList cancelDetailsOrderedList = new OrderedList();
        #endregion

        #region Constants
        private const string
              APIDWS = "APIDWS",
              APIDID = "APIDID",
              APRR_ = "APRR#",
              APSEC2 = "APSEC2",
              APHSP_  = "APHSP#",
              APACCT  = "APACCT",
              APLML = "APLML",
              APLMD = "APLMD",
              APLUL_ = "APLUL#",
              APACFL = "APACFL",
              APTTME = "APTTME",
              APINLG = "APINLG", 
              APBYPS = "APBYPS",
              APSWPY = "APSWPY",
              APTDAT = "APTDAT",
              APPTYP = "APPTYP",
              APMSV = "APMSV",
              APXMIT = "APXMIT",
              APQNUM = "APQNUM",
              APZDTE = "APZDTE",
              APZTME = "APZTME",
              APACC = "APACC",
              APWSIR = "APWSIR",
              APSECR = "APSECR",
              APORR1 = "APORR1",
              APORR2 = "APORR2",
              APORR3 = "APORR3";
        #endregion

    }
}
