using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TransferPatientToNewLocationInsertStrategy.
    /// </summary>
    [Serializable]
    public class TransferToNewLocationInsertStrategy : SqlBuilderStrategy
    {
        # region Construction And Finalization
        public TransferToNewLocationInsertStrategy()
        {
            InitializeColumnValues();
        }
        # endregion

        # region Methods

        public override void UpdateColumnValuesUsing( Account account )
        {
            if( account != null )
            {
                ITimeBroker tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                DateTime facilityDateTime = tBroker.TimeAt( account.Facility.GMTOffset, account.Facility.DSTOffset );

                this.TransactionDate = facilityDateTime;// DateTime.Today;
                this.TimeRecordCreation = facilityDateTime; // DateTime.Now;

                //Set Transaction APACFL Add/Change flag
                if( account.Activity.GetType() == typeof( TransferActivity ) )
                {
                    this.AddChangeFlag = ADD_FLAG;
                }

                this.PatientAccountNumber = account.AccountNumber;
                if( account.Facility != null )
                {
                    this.HospitalNumber = account.Facility.Oid;
                }
                if( account.LocationFrom != null )
                {
                    if( account.LocationFrom.NursingStation != null )
                    {
                        this.FromNursingStation = account.LocationFrom.NursingStation.Code;
                    }
                    if( account.LocationFrom.Room != null )
                    {
                        this.FromRoomNumber = Convert.ToInt32( account.LocationFrom.Room.Code );
                    }
                    if( account.LocationFrom.Bed != null )
                    {
                        this.FromBedNumber = account.LocationFrom.Bed.Code;
                        if( account.LocationFrom.Bed.Accomodation != null )
                        {
                            this.FromAccomodationCode = account.LocationFrom.Bed.Accomodation.Code;
                        }
                    }
                }
                if( account.LocationTo != null )
                {
                    if( account.LocationTo.NursingStation != null )
                    {
                        this.ToNursingStation = account.LocationTo.NursingStation.Code;
                    }
                    if( account.LocationTo.Room != null )
                    {
                        this.ToRoomNumber = Convert.ToInt32( account.LocationTo.Room.Code );
                    }
                    if( account.LocationTo.Bed != null )
                    {
                        this.ToBedNumber = account.LocationTo.Bed.Code;
                        if( account.LocationTo.Bed.Accomodation != null )
                        {
                            this.ToAccomodationCode = account.LocationTo.Bed.Accomodation.Code;
                        }
                    }
                }
                if (account.old_HospitalService !=  null)
                {
                   this.FromMedicalServiceCode = account.old_HospitalService.Code;
                }
  
                if( account.HospitalService != null )
                {
                    this.ToMedicalServiceCode = account.HospitalService.Code;
                }
                if( account.KindOfVisit != null )
                {
                    SetPatientType(account);
                }
                if( account.TransferDate != DateTime.MinValue )
                {
                    this.TransferDate  = account.TransferDate;
                    this.TransferTime = account.TransferDate;
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
 
                PhysicianRelationship physicianRelationship = 
                    account.PhysicianRelationshipWithRole( PhysicianRole.Attending().Role() );
                if( physicianRelationship != null && 
                    physicianRelationship.Physician != null )
                {
                    this.AttendingPhysicianNumber = 
                        physicianRelationship.Physician.PhysicianNumber;
                }
                ReadConditionCode( account.ConditionCodes );
            }
        }
		
        private void SetPatientType(Account account)
        {
            switch (account.Activity.GetType().Name)
            {
                case Activity.TransferErPatientToOutPatient:
                    PatientType = VisitType.EMERGENCY_PATIENT;
                    break;

                case Activity.TransferOutPatientToErPatient:
                    PatientType = VisitType.OUTPATIENT;
                    break;

                default:
                    PatientType = account.KindOfVisit.Code;
                    break;
            }

            if (account.Activity.GetType() == typeof(TransferERToOutpatientActivity))
            {
                PatientType = VisitType.EMERGENCY_PATIENT;
            }

            else
            { 
                PatientType = account.KindOfVisit.Code;
            }
        }

        private void ReadConditionCode( IList conditionCodes )
        {
            int index = 1;
            string conditionCodeValue;
            foreach( ConditionCode conditionCode in conditionCodes )
            {
                conditionCodeValue = conditionCode.Code;
                switch( index )
                {
                    case 1:
                        {
                            this.ConditionCode1 = conditionCodeValue;
                            break;
                        }
                    case 2:
                        {
                            this.ConditionCode2 = conditionCodeValue;
                            break;
                        }
                    case 3:
                        {
                            this.ConditionCode3 = conditionCodeValue;
                            break;
                        }
                    case 4:
                        {
                            this.ConditionCode4 = conditionCodeValue;
                            break;
                        }
                    case 5:
                        {
                            this.ConditionCode5 = conditionCodeValue;
                            break;
                        }
                    case 6:
                        {
                            this.ConditionCode6 = conditionCodeValue;
                            break;
                        }
                    case 7:
                        {
                            this.ConditionCode7 = conditionCodeValue;
                            break;
                        }
                }
                index++;
            }
        }
        public override void InitializeColumnValues()
        {    
            transferPatientToNewLocationOrderedList.Add( APIDWS, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APIDID, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER);
            transferPatientToNewLocationOrderedList.Add( APSEC2, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APHSP_, 0 );
            transferPatientToNewLocationOrderedList.Add( APACCT, 0 );
            transferPatientToNewLocationOrderedList.Add( APTRDT, 0 );
            transferPatientToNewLocationOrderedList.Add( APTRTM, 0 );
            transferPatientToNewLocationOrderedList.Add( APTDAT, 0 );
            transferPatientToNewLocationOrderedList.Add( APFNS, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APFRM, 0 );
            transferPatientToNewLocationOrderedList.Add( APFBD, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APFMSV, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APTNS, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APTRM, 0 );
            transferPatientToNewLocationOrderedList.Add( APTBD, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APTMSV, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APLML, 0 );
            transferPatientToNewLocationOrderedList.Add( APLMD, 0 );
            transferPatientToNewLocationOrderedList.Add( APLUL_, 0 );
            transferPatientToNewLocationOrderedList.Add( APACFL, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APTTME, 0 );
            transferPatientToNewLocationOrderedList.Add( APINLG, LOG_NUMBER );
            transferPatientToNewLocationOrderedList.Add( APBYPS, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APSWPY, 0 );
            transferPatientToNewLocationOrderedList.Add( APADR_, 0 );
            transferPatientToNewLocationOrderedList.Add( APPTYP, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APFACC, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APTACM, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APXMIT, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APQNUM, 0 );
            transferPatientToNewLocationOrderedList.Add( APUPRV, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APZDTE, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APZTME, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APWSIR, WORKSTATION_ID );
            transferPatientToNewLocationOrderedList.Add( APSECR, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APORR1, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APORR2, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APORR3, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI01, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI02, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI03, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI04, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI05, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI06, string.Empty );
            transferPatientToNewLocationOrderedList.Add( APCI07, string.Empty );

        }

        public override ArrayList BuildSqlFrom( Account account, 
            TransactionKeys transactionKeys )
        {
//            this.RelativeRecordNumber = transactionKeys.PatientRecordNumber;
//            this.InputLogNumber = transactionKeys.LogNumber;
            
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( this.transferPatientToNewLocationOrderedList, 
                "HPADAPIT" );
//                "HPDATA2.HPADAPIT" );
            this.SqlStatements.Add(SqlStatement);
            return SqlStatements;
        }   
     
        #endregion
       
        # region Private Methods
        # endregion

        #region Public Property

        public string TransactionFileId
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APIDID] = value;
            }
        }
                
        public string SecurityCode
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APSEC2] = value;
            }
        }

        #endregion 

        #region Private Property
      
       
        private long HospitalNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APHSP_] = value;
            }
        }
           
        private string FromNursingStation
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APFNS] = value;
            }
        }
        
        private int FromRoomNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APFRM] = value;
            }
        }
        private long AttendingPhysicianNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APADR_] = value;
            }
        }
        
        private string FromBedNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APFBD] = value;
            }
        }

        private string ToNursingStation
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTNS] = value;
            }
        }
        
        private int ToRoomNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTRM] = value;
            }
        }
        
        private string ToBedNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTBD] = value;
            }
        }
        private string FromMedicalServiceCode
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APFMSV] = value;
                
           }
        }

                        
        private string ToMedicalServiceCode
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTMSV] = value;
            }
        }
        
        private long PatientAccountNumber
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APACCT] = value;
            }
        }
                
        private string PatientType
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APPTYP] = value;
            }
        }

        
        private string FromAccomodationCode
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APFACC] = value;
            }
        }

        private string ToAccomodationCode
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTACM] = value;
            }
        }
        private DateTime TransferDate
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTRDT] = ConvertDateToIntInMddyyFormat( value );
            }
        }  
        private DateTime TransferTime
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTRTM] = ConvertTimeToIntInHHmmFormat( value );
            }
        }  
        private string WorkStationId
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APIDWS] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APACFL] = value;
            }
        } 
        private DateTime TransactionDate
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }
        private DateTime TimeRecordCreation
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }

        #endregion 
        # region Condition Codes
        private string ConditionCode1
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI01] = value;
            }
        }
        private string ConditionCode2
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI02] = value;
            }
        }
        private string ConditionCode3
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI03] = value;
            }
        }
        private string ConditionCode4
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI04] = value;
            }
        }
        private string ConditionCode5
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI05] = value;
            }
        }
        private string ConditionCode6
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI06] = value;
            }
        }
        private string ConditionCode7
        {
            set
            {
                this.transferPatientToNewLocationOrderedList[APCI07] = value;
            }
        }
         #endregion

        # region Data Elements
        private OrderedList transferPatientToNewLocationOrderedList = new OrderedList();
        # endregion 

        # region Constants 
        private const string
                APIDWS = "APIDWS",
                APIDID = "APIDID",
                APRR_ = "APRR#",
                APSEC2 = "APSEC2",
                APHSP_ = "APHSP#",
                APACCT = "APACCT",
                APTRDT = "APTRDT",
                APTRTM = "APTRTM",
                APTDAT = "APTDAT",
                APFNS = "APFNS",
                APFRM = "APFRM",
                APFBD = "APFBD",
                APFMSV = "APFMSV",
                APTNS = "APTNS",
                APTRM = "APTRM",
                APTBD = "APTBD",
                APTMSV = "APTMSV",
                APLML = "APLML",
                APLMD = "APLMD",
                APLUL_ = "APLUL#",
                APACFL = "APACFL",
                APTTME = "APTTME",
                APINLG = "APINLG",
                APBYPS = "APBYPS",
                APSWPY = "APSWPY",
                APADR_ = "APADR#",
                APPTYP = "APPTYP",
                APFACC = "APFACC",
                APTACM = "APTACM",
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
                APCI01 = "APCI01",
                APCI02 = "APCI02",
                APCI03 = "APCI03",
                APCI04 = "APCI04",
                APCI05 = "APCI05",
                APCI06 = "APCI06",
                APCI07 = "APCI07";
        # endregion
    }
}
