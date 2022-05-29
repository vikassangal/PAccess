using System;
using System.Collections;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class DischargeInsertStrategy : SqlBuilderStrategy
    {
        #region Construction and Finalization
        public DischargeInsertStrategy()
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
               if( account.Activity.GetType() == typeof( DischargeActivity ) )
               {
                   AddChangeFlag = ADD_FLAG;
               }
               else if( account.Activity.GetType() == typeof( EditDischargeDataActivity ) )
               {
                   AddChangeFlag = CHANGE_FLAG;
               }

               //update the local properties with values in domain objects (Account)
               if( account.Facility != null )
               {
                   HospitalNumber = ( int )account.Facility.Oid;
                   AccountNumber = ( int )account.AccountNumber;
               }

                TransferToHospitalNumber = account.Patient.InterFacilityTransferAccount.ToFacilityOid;
                TransferToAccountNumber = account.Patient.InterFacilityTransferAccount.ToAccountNumber;
                DischargeDate = account.DischargeDate;
                DischargeTime =  account.DischargeDate;
                
                if (account.Facility != null)
                {
                    DateTime facilityDateTime = account.Facility.GetCurrentDateTime();
                    TransactionDate = facilityDateTime;
                    TransactionTime = facilityDateTime;
                }

               if( account.DischargeDisposition != null )
               {
                   DischargeCode = account.DischargeDisposition.Code;
               }

               if( account.Activity != null )
               {
                   User appUser = account.Activity.AppUser;
                   if( appUser != null )
                   {
                       if( appUser.WorkstationID != null )
                       {
                           WorkStationId = appUser.WorkstationID;
                       }
                   }
               }
               ReadCodedDiagnosis( account );
               ReadOperatingPhysicianInfo( account );
            }
        }


        public override void InitializeColumnValues()
        {    
            dischargePatientOrderedList.Add( APIDWS, string.Empty );
            dischargePatientOrderedList.Add( APIDID, string.Empty );
            dischargePatientOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER );
            dischargePatientOrderedList.Add( APSEC2, string.Empty );
            dischargePatientOrderedList.Add( APHSP_, 0 );
            dischargePatientOrderedList.Add( APACCT, 0 );
            dischargePatientOrderedList.Add( APLDD, 0 );
            dischargePatientOrderedList.Add( APLDT, 0 );
            dischargePatientOrderedList.Add( APTDAT, 0 );
            dischargePatientOrderedList.Add( APDCOD, string.Empty );
            dischargePatientOrderedList.Add( APCD01, string.Empty );
            dischargePatientOrderedList.Add( APCD02, string.Empty );
            dischargePatientOrderedList.Add( APCD03, string.Empty );
            dischargePatientOrderedList.Add( APCD04, string.Empty );
            dischargePatientOrderedList.Add( APCD05, string.Empty );
            dischargePatientOrderedList.Add( APCD06, string.Empty );
            dischargePatientOrderedList.Add( APCD07, string.Empty );
            dischargePatientOrderedList.Add( APCD08, string.Empty );
            dischargePatientOrderedList.Add( APODR_, 0 );
            dischargePatientOrderedList.Add( APOP01, string.Empty );
            dischargePatientOrderedList.Add( APOP02, string.Empty );
            dischargePatientOrderedList.Add( APOP03, string.Empty );
            dischargePatientOrderedList.Add( APOP04, string.Empty );
            dischargePatientOrderedList.Add( APOD01, 0 );
            dischargePatientOrderedList.Add( APOD02, 0 );
            dischargePatientOrderedList.Add( APOD03, 0 );
            dischargePatientOrderedList.Add( APOD04, 0 );
            dischargePatientOrderedList.Add( APLML, 0 );
            dischargePatientOrderedList.Add( APLMD, 0 );
            dischargePatientOrderedList.Add( APLUL_, 0 );
            dischargePatientOrderedList.Add( APACFL, string.Empty );
            dischargePatientOrderedList.Add( APTTME, 0 );
            dischargePatientOrderedList.Add( APINLG, LOG_NUMBER );
            dischargePatientOrderedList.Add( APBYPS, string.Empty );
            dischargePatientOrderedList.Add( APSWPY, 0 );
            dischargePatientOrderedList.Add( APRA01, string.Empty );
            dischargePatientOrderedList.Add( APRA02, string.Empty );
            dischargePatientOrderedList.Add( APRA03, string.Empty );
            dischargePatientOrderedList.Add( APRA04, string.Empty );
            dischargePatientOrderedList.Add( APRA05, string.Empty );
            dischargePatientOrderedList.Add( APCC01, string.Empty );
            dischargePatientOrderedList.Add( APCC02, string.Empty );
            dischargePatientOrderedList.Add( APCC03, string.Empty );
            dischargePatientOrderedList.Add( APABOR, 0 );
            dischargePatientOrderedList.Add( APABC1, string.Empty );
            dischargePatientOrderedList.Add( APXMIT, string.Empty );
            dischargePatientOrderedList.Add( APQNUM, 0 );
            dischargePatientOrderedList.Add( APUPRV, string.Empty );
            dischargePatientOrderedList.Add( APZDTE, string.Empty );
            dischargePatientOrderedList.Add( APZTME, string.Empty );
            dischargePatientOrderedList.Add( APWSIR, WORKSTATION_ID );
            dischargePatientOrderedList.Add( APSECR, string.Empty );
            dischargePatientOrderedList.Add( APORR1, string.Empty );
            dischargePatientOrderedList.Add( APORR2, string.Empty );
            dischargePatientOrderedList.Add( APORR3, string.Empty );
            dischargePatientOrderedList.Add(APTOHSP_, string.Empty);
            dischargePatientOrderedList.Add(APTOACCT, string.Empty);
        }

        public override ArrayList BuildSqlFrom( Account account, 
            TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( dischargePatientOrderedList, 
                    "HPADAPDI" );   // "HPDATA2.HPADAPDI" );
            SqlStatements.Add(SqlStatement);
            return SqlStatements;
        }  

        private void ReadCodedDiagnosis(Account account)
        {
            string colName = string.Empty;
            for (int x = 0; x < account.CodedDiagnoses.CodedDiagnosises.Count; x++ )
            {
                colName = "APCD0" + (x + 1);
                dischargePatientOrderedList[colName] = account.CodedDiagnoses.CodedDiagnosises[x];
            }
            for (int x = 0; x < account.CodedDiagnoses.AdmittingCodedDiagnosises.Count; x++ )
            {
                colName = "APRA0" + (x + 1);
                dischargePatientOrderedList[colName] = account.CodedDiagnoses.AdmittingCodedDiagnosises[x];
            }
        }

        private void ReadOperatingPhysicianInfo( Account account )
        {
            PhysicianRelationship physicianRelationship =
                account.PhysicianRelationshipWithRole( PhysicianRole.Operating().Role() );
            if ( physicianRelationship != null && physicianRelationship.Physician != null )
            {
                OperatingPhysicianNumber = physicianRelationship.Physician.PhysicianNumber;
            }
        }

        #endregion

        #region private Properties
        private int HospitalNumber
        {
            set
            {
                dischargePatientOrderedList[APHSP_] = value;
            }
        }

        private int AccountNumber
        {
            set
            {
                dischargePatientOrderedList[APACCT] = value;
            }
        }

        private long TransferToHospitalNumber
        {
            set
            {
                dischargePatientOrderedList[APTOHSP_] = value;
            }
        }



        private long TransferToAccountNumber
        {
            set
            {
                dischargePatientOrderedList[APTOACCT] = value;
            }
        }

        private DateTime DischargeDate
        {
            set
            {
                dischargePatientOrderedList[APLDD] = ConvertDateToIntInMddyyFormat( value );
            }
        }
        private DateTime DischargeTime
        {
            set
            {
                dischargePatientOrderedList[APLDT] = ConvertTimeToIntInHHmmFormat( value );
            }
        }
        private DateTime TransactionDate
        {
            set
            {
                dischargePatientOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }
        private DateTime TransactionTime
        {
            set
            {
                dischargePatientOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }
        private string DischargeCode
        {
            set
            {
                dischargePatientOrderedList[APDCOD] = value;
            }
        }
        private string WorkStationId
        {
            set
            {
                dischargePatientOrderedList[APIDWS] = value;
            }
        }

        private long OperatingPhysicianNumber
        {
            set
            {
                dischargePatientOrderedList[APODR_] = value;
            }
        }        

      #endregion

        #region public properties
        public string TransactionFileId
        {
            set
            {
                dischargePatientOrderedList[APIDID] = value;
            }
        }

        public string UserSecurityCode
        {
            set
            {
                dischargePatientOrderedList[APSEC2] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                dischargePatientOrderedList[APACFL] = value;
            }
        } 
        #endregion

        #region Data Elements
        private readonly OrderedList dischargePatientOrderedList = new OrderedList();
        #endregion

        #region Constants

        private const string
            APIDWS = "APIDWS",
            APIDID = "APIDID",
            APRR_ = "APRR#",
            APSEC2 = "APSEC2",
            APHSP_ = "APHSP#",
            APACCT = "APACCT",
            APLDD = "APLDD",
            APLDT = "APLDT",
            APTDAT = "APTDAT",
            APDCOD = "APDCOD",
            APCD01 = "APCD01",
            APCD02 = "APCD02",
            APCD03 = "APCD03",
            APCD04 = "APCD04",
            APCD05 = "APCD05",
            APCD06 = "APCD06",
            APCD07 = "APCD07",
            APCD08 = "APCD08",
            APODR_ = "APODR#",
            APOP01 = "APOP01",
            APOP02 = "APOP02",
            APOP03 = "APOP03",
            APOP04 = "APOP04",
            APOD01 = "APOD01",
            APOD02 = "APOD02",
            APOD03 = "APOD03",
            APOD04 = "APOD04",
            APLML = "APLML",
            APLMD = "APLMD",
            APLUL_ = "APLUL#",
            APACFL = "APACFL",
            APTTME = "APTTME",
            APINLG = "APINLG",
            APBYPS = "APBYPS",
            APSWPY = "APSWPY",
            APRA01 = "APRA01",
            APRA02 = "APRA02",
            APRA03 = "APRA03",
            APRA04 = "APRA04",
            APRA05 = "APRA05",
            APCC01 = "APCC01",
            APCC02 = "APCC02",
            APCC03 = "APCC03",
            APABOR = "APABOR",
            APABC1 = "APABC1",
            APXMIT = "APXMIT",
            APQNUM = "APQNUM",
            APUPRV = "APUPRV",
            APZDTE = "APZDTE",
            APZTME = "APZTME",
            APWSIR = "APWSIR",
            APSECR = "APSECR",
            APORR1 = "APORR1",
            APORR2 = "APORR2",
            APORR3 = "APORR3",
            APTOHSP_ = "APTOHSP_",
            APTOACCT = "APTOACCT";

        #endregion
    }
}
