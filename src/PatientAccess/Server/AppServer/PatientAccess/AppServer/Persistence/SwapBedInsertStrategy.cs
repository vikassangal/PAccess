using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for CancelDischargeInsertStrategy.
    /// </summary>
    public class SwapBedInsertStrategy : SqlBuilderStrategy
    {
        #region Construction And Finalization

        public SwapBedInsertStrategy()
        {
            InitializeColumnValues();
        }
        #endregion

        #region Methods
  
        public override void UpdateColumnValuesUsing( Account accountOne, Account accountTwo )
        {
            if( accountOne != null && accountTwo != null )
            {
                //Set Transaction APACFL Add/Change flag
                this.AddChangeFlag = BLANK_FLAG;

                if( accountOne.Facility != null )
                {
                    this.HospitalNumber = accountOne.Facility.Oid;
                }
                this.PatientAccountNumberOne = accountOne.AccountNumber;
                if( accountOne.LocationFrom != null )
                {
                    if( accountOne.LocationFrom.NursingStation != null )
                    {
                        this.OldNursingStationOne = accountOne.LocationFrom.NursingStation.Code;
                    }
                    if( accountOne.LocationFrom.Room != null && 
                        accountOne.LocationFrom.Room.Code.Trim().Length > 0 )
                    {
                        this.OldRoomNumberOne = Convert.ToInt32( accountOne.LocationFrom.Room.Code );
                    }
                    if( accountOne.LocationFrom.Bed != null )
                    {
                        this.OldBedNumberOne = accountOne.LocationFrom.Bed.Code;
                        if( accountOne.LocationTo.Bed.Accomodation != null )
                        {
                            this.NewAccomodationCodeOne = accountOne.LocationTo.Bed.Accomodation.Code;
                        }
                    }
                }
                if( accountOne.TransferredFromHospitalService != null )
                {
                    this.OldMedicalServiceCodeOne = accountOne.TransferredFromHospitalService.Code;
                }
                if( accountOne.HospitalService != null )
                {
                    this.NewMedicalServiceCodeOne = accountOne.HospitalService.Code;
                }

                if( accountOne.LocationFrom != null && accountOne.LocationFrom.Bed != null &&
                    accountOne.LocationFrom.Bed.Accomodation != null )
                {
                    this.OldAccomodationCodeOne = accountOne.LocationFrom.Bed.Accomodation.Code;
                }
                if( accountTwo.LocationTo.Bed.Accomodation != null )
                {
                    this.NewAccomodationCodeTwo = accountTwo.LocationTo.Bed.Accomodation.Code;                
                }
                if( accountOne.TransferDate != DateTime.MinValue )
                {
                    this.TransferDate  = accountOne.TransferDate;
                    this.TransferTime = accountOne.TransferDate;
                }
                this.PatientAccountNumberTwo = accountTwo.AccountNumber;
                if( accountTwo.LocationFrom != null )
                {
                    if( accountTwo.LocationFrom.NursingStation != null )
                    {
                        this.OldNursingStationTwo = accountTwo.LocationFrom.NursingStation.Code;
                    }
                    if( accountTwo.LocationFrom.Room != null && 
                        accountTwo.LocationFrom.Room.Code.Trim().Length > 0 )
                    {
                        this.OldRoomNumberTwo = Convert.ToInt32( accountTwo.LocationFrom.Room.Code );
                    }
                    if( accountTwo.LocationFrom.Bed != null )
                    {
                        this.OldBedNumberTwo = accountTwo.LocationFrom.Bed.Code;
                    }
                }
                if( accountTwo.TransferredFromHospitalService != null )
                {
                    this.OldMedicalServiceCodeTwo = accountTwo.TransferredFromHospitalService.Code;
                }
                if( accountTwo.HospitalService != null )
                {
                    this.NewMedicalServiceCodeTwo = accountTwo.HospitalService.Code;
                }

                if( accountTwo.LocationFrom.Bed.Accomodation != null )
                {
                    this.OldAccomodationCodeTwo = accountTwo.LocationFrom.Bed.Accomodation.Code;              
                }

                ITimeBroker tBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
                DateTime facilityDateTime = tBroker.TimeAt(accountTwo.Facility.GMTOffset, accountTwo.Facility.DSTOffset);

                this.TimeRecordCreation = facilityDateTime;  
                this.TransactionDate = facilityDateTime;  

                if( accountTwo.Activity != null )
                {
                    User appUser = accountTwo.Activity.AppUser;
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
            swapBedOrderedList.Add( APIDWS, string.Empty );
            swapBedOrderedList.Add( APIDID, string.Empty );
            swapBedOrderedList.Add( APRR_, PATIENT_RECORD_NUMBER );
            swapBedOrderedList.Add( APSEC2, string.Empty );
            swapBedOrderedList.Add( APHSP_, 0 );
            swapBedOrderedList.Add( APACT1, 0 );
            swapBedOrderedList.Add( APNS1, string.Empty );
            swapBedOrderedList.Add( APRM1, 0 );
            swapBedOrderedList.Add( APBD1, string.Empty );
            swapBedOrderedList.Add( APMSO1, string.Empty );
            swapBedOrderedList.Add( APMSN1, string.Empty );
            swapBedOrderedList.Add( APACT2, 0 );
            swapBedOrderedList.Add( APNS2, string.Empty );
            swapBedOrderedList.Add( APRM2, 0 );
            swapBedOrderedList.Add( APBD2, string.Empty );
            swapBedOrderedList.Add( APMSO2, string.Empty );
            swapBedOrderedList.Add( APMSN2, string.Empty );
            swapBedOrderedList.Add( APTRDT, 0 );
            swapBedOrderedList.Add( APTRTM, 0 );
            swapBedOrderedList.Add( APTDAT, 0 );
            swapBedOrderedList.Add( APLML, 0 );
            swapBedOrderedList.Add( APLMD, 0 );
            swapBedOrderedList.Add( APLUL_, 0 );
            swapBedOrderedList.Add( APACFL, string.Empty );
            swapBedOrderedList.Add( APTTME, 0 );
            swapBedOrderedList.Add( APINLG, LOG_NUMBER );
            swapBedOrderedList.Add( APBYPS, string.Empty );
            swapBedOrderedList.Add( APSWPY, 0 );
            swapBedOrderedList.Add( APACO1, string.Empty );
            swapBedOrderedList.Add( APACN1, string.Empty );
            swapBedOrderedList.Add( APACO2, string.Empty );
            swapBedOrderedList.Add( APACN2, string.Empty );
            swapBedOrderedList.Add( APXMIT, string.Empty );
            swapBedOrderedList.Add( APQNUM, 0 );
            swapBedOrderedList.Add( APUPR1, string.Empty );
            swapBedOrderedList.Add( APUPR2, string.Empty );
            swapBedOrderedList.Add( APZDTE, string.Empty );
            swapBedOrderedList.Add( APZTME, string.Empty );
            swapBedOrderedList.Add( APWSIR, WORKSTATION_ID );
            swapBedOrderedList.Add( APSECR, string.Empty );
            swapBedOrderedList.Add( APORR1, string.Empty );
            swapBedOrderedList.Add( APORR2, string.Empty );
            swapBedOrderedList.Add( APORR3, string.Empty );
        }



        public override ArrayList BuildSqlFrom( Account accountOne, Account accountTwo,
            TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( accountOne, accountTwo );
            AddColumnsAndValuesToSqlStatement( this.swapBedOrderedList , 
                "HPADAPDT" );

            this.SqlStatements.Add(SqlStatement);
            return SqlStatements;
        }   




        #endregion

        #region Private Methods
        #endregion

        #region Private Property

        
        private long HospitalNumber
        {
            set
            {
                this.swapBedOrderedList[APHSP_] = value;
            }
        }
        private long PatientAccountNumberOne
        {
            set
            {
                this.swapBedOrderedList[APACT1] = value;
            }
        }
        private long PatientAccountNumberTwo
        {
            set
            {
                this.swapBedOrderedList[APACT2] = value;
            }
        }
        private string OldNursingStationOne
        {
            set
            {
                this.swapBedOrderedList[APNS1] = value;
            }

        }
        private string OldNursingStationTwo
        {
            set
            {
                this.swapBedOrderedList[APNS2] = value;
            }

        }
        private int OldRoomNumberOne
        {
            set
            {
                this.swapBedOrderedList[APRM1] = value;
            }

        }
        private int OldRoomNumberTwo
        {
            set
            {
                this.swapBedOrderedList[APRM2] = value;
            }

        }
        private string OldBedNumberOne
        {
            set
            {
                this.swapBedOrderedList[APBD1] = value;
            }
        }
        private string OldBedNumberTwo
        {
            set
            {
                this.swapBedOrderedList[APBD2] = value;
            }
        }
        private string OldMedicalServiceCodeOne
        {
            set
            {
                this.swapBedOrderedList[APMSO1] = value;
            }
        }
        private string NewMedicalServiceCodeOne
        {
            set
            {
                this.swapBedOrderedList[APMSN1] = value;
            }
        }
        private string OldMedicalServiceCodeTwo
        {
            set
            {
                this.swapBedOrderedList[APMSO2] = value;
            }
        }
        private string NewMedicalServiceCodeTwo
        {
            set
            {
                this.swapBedOrderedList[APMSN2] = value;
            }
        }
        private DateTime TransferDate
        {
            set
            {
                this.swapBedOrderedList[APTRDT] = ConvertDateToIntInMddyyFormat( value );
            }
        }    
        private DateTime TransferTime
        {
            set
            {
                this.swapBedOrderedList[APTRTM] = ConvertTimeToIntInHHmmFormat( value );
            }
        }  
        private string OldAccomodationCodeOne
        {
            set
            {
                this.swapBedOrderedList[APACO1] = value;
            }
        }  
        private string OldAccomodationCodeTwo
        {
            set
            {
                this.swapBedOrderedList[APACO2] = value;
            }
        }  
        private string NewAccomodationCodeOne
        {
            set
            {
                this.swapBedOrderedList[APACN1] = value;
            }
        }  
        private string NewAccomodationCodeTwo
        {
            set
            {
                this.swapBedOrderedList[APACN2] = value;
            }
        }  
        private DateTime TimeRecordCreation
        {
            set
            {
                this.swapBedOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }
        private DateTime TransactionDate
        {
            set
            {
                this.swapBedOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }

        private string WorkStationId
        {
            set
            {
                this.swapBedOrderedList[APIDWS] = value;
            }
        }

        #endregion

        #region Public Property

        public string TransactionFileId
        {
            set
            {
                this.swapBedOrderedList[APIDID] = value;
            }
        }
                
        public string SecurityCode
        {
            set
            {
                this.swapBedOrderedList[APSEC2] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                this.swapBedOrderedList[APACFL] = value;
            }
        } 
        #endregion 

        #region Data Elements
        
        private OrderedList swapBedOrderedList = new OrderedList();
        #endregion

        #region Constants

        private const string
             APIDWS = "APIDWS", 
             APIDID = "APIDID", 
             APRR_ = "APRR#", 
             APSEC2 = "APSEC2", 
             APHSP_ = "APHSP#", 
             APACT1 = "APACT1", 
             APNS1 = "APNS1", 
             APRM1 = "APRM1", 
             APBD1 = "APBD1", 
             APMSO1 = "APMSO1", 
             APMSN1 = "APMSN1", 
             APACT2 = "APACT2", 
             APNS2 = "APNS2", 
             APRM2 = "APRM2", 
             APBD2 = "APBD2", 
             APMSO2 = "APMSO2", 
             APMSN2 = "APMSN2", 
             APTRDT = "APTRDT", 
             APTRTM = "APTRTM", 
             APTDAT = "APTDAT", 
             APLML = "APLML", 
             APLMD = "APLMD", 
             APLUL_ = "APLUL#", 
             APACFL = "APACFL", 
             APTTME = "APTTME", 
             APINLG = "APINLG",
             APBYPS = "APBYPS", 
             APSWPY = "APSWPY", 
             APACO1 = "APACO1", 
             APACN1 = "APACN1", 
             APACO2 = "APACO2", 
             APACN2 = "APACN2", 
             APXMIT = "APXMIT", 
             APQNUM = "APQNUM", 
             APUPR1 = "APUPR1", 
             APUPR2 = "APUPR2", 
             APZDTE = "APZDTE",
             APZTME = "APZTME", 
             APWSIR = "APWSIR",
             APSECR = "APSECR", 
             APORR1 = "APORR1", 
             APORR2 = "APORR2", 
             APORR3 = "APORR3"; 
        #endregion
    }
}
