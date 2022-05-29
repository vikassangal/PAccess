using System;
using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.Persistence
{
    [Serializable]
	public class EditRecurringDischargeInsertStrategy : SqlBuilderStrategy
	{
        #region Construction and Finalization
        public EditRecurringDischargeInsertStrategy()
        {
            InitializeColumnValues();
        }
        #endregion

        #region Methods
        public override void UpdateColumnValuesUsing( Account account )
        {
            if( account != null )
            {
                if( account.Activity.GetType() == typeof( EditRecurringDischargeActivity ) )
                {
                    this.AddChangeFlag = CHANGE_FLAG;
                }

                //update the local properties with values in domain objects (Account)
                if( account.Facility != null )
                {
                    this.HospitalNumber = ( int )account.Facility.Oid;
                    this.AccountNumber = ( int )account.AccountNumber;
                }

                this.DischargeDate = account.DischargeDate;
                this.DischargeTime =  account.DischargeDate;
                if (account.Facility != null)
                {
                    DateTime facilityDateTime = account.Facility.GetCurrentDateTime();
                    TransactionDate = facilityDateTime;
                    TransactionTime = facilityDateTime;
                }

                if( account.DischargeDisposition != null )
                {
                    this.DischargeCode = account.DischargeDisposition.Code;
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
            this.ReadCodedDiagnosis( account );
        }


        public override void InitializeColumnValues()
        {    
            editRecurringDischargeOrderedList.Add( APIDWS, string.Empty );
            editRecurringDischargeOrderedList.Add( APIDID, string.Empty );
            editRecurringDischargeOrderedList.Add( APRR_, OTHER_RECORD_NUMBER );
            editRecurringDischargeOrderedList.Add( APSEC2, string.Empty );
            editRecurringDischargeOrderedList.Add( APHSP_, 0 );
            editRecurringDischargeOrderedList.Add( APACCT, 0 );
            editRecurringDischargeOrderedList.Add( APDIAG, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD01, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD02, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD03, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD04, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD05, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD06, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD07, string.Empty );
            editRecurringDischargeOrderedList.Add( APCD08, string.Empty );
            editRecurringDischargeOrderedList.Add( APTDAT, 0 );
            editRecurringDischargeOrderedList.Add( APLML, 0 );
            editRecurringDischargeOrderedList.Add( APLMD, 0 );
            editRecurringDischargeOrderedList.Add( APLUL_, 0 );
            editRecurringDischargeOrderedList.Add( APACFL, string.Empty );
            editRecurringDischargeOrderedList.Add( APTTME, 0 );
            editRecurringDischargeOrderedList.Add( APINLG, LOG_NUMBER );
            editRecurringDischargeOrderedList.Add( APBYPS, string.Empty );
            editRecurringDischargeOrderedList.Add( APSWPY, 0 );
            editRecurringDischargeOrderedList.Add( APRA01, string.Empty );
            editRecurringDischargeOrderedList.Add( APRA02, string.Empty );
            editRecurringDischargeOrderedList.Add( APRA03, string.Empty );
            editRecurringDischargeOrderedList.Add( APRA04, string.Empty );
            editRecurringDischargeOrderedList.Add( APRA05, string.Empty );
            editRecurringDischargeOrderedList.Add( APCC01, string.Empty );
            editRecurringDischargeOrderedList.Add( APCC02, string.Empty );
            editRecurringDischargeOrderedList.Add( APCC03, string.Empty );
            editRecurringDischargeOrderedList.Add( APOP01, string.Empty );
            editRecurringDischargeOrderedList.Add( APOP02, string.Empty );
            editRecurringDischargeOrderedList.Add( APOP03, string.Empty );
            editRecurringDischargeOrderedList.Add( APOP04, string.Empty );
            editRecurringDischargeOrderedList.Add( APOD01, 0 );
            editRecurringDischargeOrderedList.Add( APOD02, 0 );
            editRecurringDischargeOrderedList.Add( APOD03, 0 );
            editRecurringDischargeOrderedList.Add( APOD04, 0 );
            editRecurringDischargeOrderedList.Add( APDCOD, string.Empty );
            editRecurringDischargeOrderedList.Add( APLDD, 0 );
            editRecurringDischargeOrderedList.Add( APLDT, 0 );
            editRecurringDischargeOrderedList.Add( APUPRV, string.Empty );
            editRecurringDischargeOrderedList.Add( APZDTE, string.Empty );
            editRecurringDischargeOrderedList.Add( APZTME, string.Empty );
            editRecurringDischargeOrderedList.Add( APWSIR, WORKSTATION_ID );
            editRecurringDischargeOrderedList.Add( APSECR, string.Empty );
            editRecurringDischargeOrderedList.Add( APORR1, string.Empty );
            editRecurringDischargeOrderedList.Add( APORR2, string.Empty );
            editRecurringDischargeOrderedList.Add( APORR3, string.Empty );
        }


        public override ArrayList BuildSqlFrom( Account account, 
            TransactionKeys transactionKeys )
        {
            UpdateColumnValuesUsing( account );
            AddColumnsAndValuesToSqlStatement( this.editRecurringDischargeOrderedList, 
                "HPADAPEO" );
            this.SqlStatements.Add(SqlStatement);
            return SqlStatements;
        }  

        private void ReadCodedDiagnosis(Account account)
        {
            for (int x = 0; x < account.CodedDiagnoses.CodedDiagnosises.Count; x++ )
            {
                string colName = "APCD0"  + (x + 1);
                this.editRecurringDischargeOrderedList[colName] = account.CodedDiagnoses.CodedDiagnosises[x];
            }
            for (int x = 0; x < account.CodedDiagnoses.AdmittingCodedDiagnosises.Count; x++ )
            {
                string colName = "APRA0"  + (x + 1);
                this.editRecurringDischargeOrderedList[colName] = account.CodedDiagnoses.AdmittingCodedDiagnosises[x];
            }
        }

        #endregion

        #region private Properties
        private int HospitalNumber
        {
            set
            {
                this.editRecurringDischargeOrderedList[APHSP_] = value;
            }
        }

        private int AccountNumber
        {
            set
            {
                this.editRecurringDischargeOrderedList[APACCT] = value;
            }
        }

        private DateTime DischargeDate
        {
            set
            {
                this.editRecurringDischargeOrderedList[APLDD] = ConvertDateToIntInMddyyFormat( value );
            }
        }
        private DateTime DischargeTime
        {
            set
            {
                this.editRecurringDischargeOrderedList[APLDT] = ConvertTimeToIntInHHmmFormat( value );
            }
        }
        private DateTime TransactionDate
        {
            set
            {
                this.editRecurringDischargeOrderedList[APTDAT] = ConvertDateToIntInMddyyFormat( value );
            }
        }
        private DateTime TransactionTime
        {
            set
            {
                this.editRecurringDischargeOrderedList[APTTME] = ConvertTimeToIntInHHmmSSFormat( value );
            }
        }
        private string DischargeCode
        {
            set
            {
                this.editRecurringDischargeOrderedList[APDCOD] = value;
            }
        }
        private string WorkStationId
        {
            set
            {
                this.editRecurringDischargeOrderedList[APIDWS] = value;
            }
        }
        
        #endregion

        #region public properties
        public string TransactionFileId
        {
            set
            {
                this.editRecurringDischargeOrderedList[APIDID] = value;
            }
        }

        public string UserSecurityCode
        {
            set
            {
                this.editRecurringDischargeOrderedList[APSEC2] = value;
            }
        }
        private string AddChangeFlag
        {
            set
            {
                this.editRecurringDischargeOrderedList[APACFL] = value;
            }
        } 
        #endregion

        #region Data Elements
        private OrderedList editRecurringDischargeOrderedList = new OrderedList();
        #endregion

        #region Constants
        private const string
            APIDWS = "APIDWS",
            APIDID = "APIDID",
            APRR_ = "APRR#",
            APSEC2 = "APSEC2",
            APHSP_ = "APHSP#",
            APACCT = "APACCT",
            APDIAG = "APDIAG",
            APCD01 = "APCD01",
            APCD02 = "APCD02",
            APCD03 = "APCD03",
            APCD04 = "APCD04",
            APCD05 = "APCD05",
            APCD06 = "APCD06",
            APCD07 = "APCD07",
            APCD08 = "APCD08",
            APTDAT = "APTDAT",
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
            APOP01 = "APOP01",
            APOP02 = "APOP02",
            APOP03 = "APOP03",
            APOP04 = "APOP04",
            APOD01 = "APOD01",
            APOD02 = "APOD02",
            APOD03 = "APOD03",
            APOD04 = "APOD04",
            APDCOD = "APDCOD",
            APLDD = "APLDD",
            APLDT = "APLDT",
            APUPRV = "APUPRV",
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
