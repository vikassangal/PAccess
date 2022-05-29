using System;
using System.Collections;
using System.Data;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;
using PatientAccess.Domain.InterFacilityTransfer;
using IBM.Data.DB2.iSeries;
using Extensions.DB2Persistence;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Domain.Parties;
using System.Collections.Generic;
using PatientAccess.BrokerProxies;

namespace PatientAccess.Persistence
{
    [Serializable]
    public class InterFacilityTransferPBARBroker : AbstractPBARBroker, IInterfacilityTransferBroker
    {

        #region Methods
        public InterFacilityTransferAccount Accountsfromtransferlogfordischarge(long FromMRN, Facility FromFacility, long FromAccount)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;
           
            InterFacilityTransferAccount interFacilityTransferAccount = null;
            try
            {
                long MRN = FromMRN;
                long facilityId = FromFacility.Oid;
                long accountNumber = FromAccount;

                cmd = CommandFor("CALL " + SP_SELECTACCOUNTSFROMTRANSFERLOGFORDISCHARGE +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                    FromFacility);

                cmd.Parameters[PARAM_MRN].Value = MRN;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;

                reader = ExecuteReader(cmd);
                
                while (reader.Read())
                {
                    interFacilityTransferAccount = new InterFacilityTransferAccount
                    {
                        ToMedicalRecordNumber = reader.GetInt64(COL_TOMRC),
                        ToFacilityOid = reader.GetInt64(COL_TOHOSPITAL),
                        ToAccountNumber = reader.GetInt64(COL_TOACCOUNT),
                        ToAdmitDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat(reader.GetString(COL_TOADMITDATE)),
                        ToAdmitTime = reader.GetInt32(COL_TOADMITTIME),
                        FromDischargeDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat(reader.GetString(COL_FROMDISCHARGEDATE)),
                        FromDischargeTime = reader.GetInt32(COL_FROMDISCHARGETIME),
                        FromFacilityOid = reader.GetInt32(COL_FROMHOSPITAL),
                        FromMedicalRecordNumber = reader.GetInt32(COL_FROMMRC),
                        FromAccountNumber = reader.GetInt32(COL_FROMACCOUNT),
                        FromFacility = i_FacilityBroker.FacilityWith(reader.GetInt32(COL_FROMHOSPITAL)),
                        ToFacility = i_FacilityBroker.FacilityWith(reader.GetInt64(COL_TOHOSPITAL))
                    };
                }
            }
            catch (Exception ex)
            {
                const string message = "InterFacilityTransferPBARBroker for the given " +
                                       "facility, mrn, account  method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(message, ex, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
            ConnectionString = "SDFDSF";
            return interFacilityTransferAccount;
        }
        public InterFacilityTransferAccount AccountsfromtransferlogforRegistration(long FromMRN, Facility FromFacility, long FromAccount)
        {
            iDB2Command cmd = null;
            SafeReader reader = null;

            InterFacilityTransferAccount interFacilityTransferAccount = null;
            try
            {
                long MRN = FromMRN;
                long facilityId = FromFacility.Oid;
                long accountNumber = FromAccount;

                cmd = CommandFor("CALL " + SP_SELECTACCOUNTSFROMTRANSFERLOGFORREGISTRATION +
                    "(" + PARAM_FACILITYID +
                    "," + PARAM_MRN +
                    "," + PARAM_ACCOUNTNUMBER + ")",
                    CommandType.Text,
                   FromFacility);

                cmd.Parameters[PARAM_MRN].Value = MRN;
                cmd.Parameters[PARAM_ACCOUNTNUMBER].Value = accountNumber;
                cmd.Parameters[PARAM_FACILITYID].Value = facilityId;

                reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    interFacilityTransferAccount = new InterFacilityTransferAccount
                    {
                        FromMedicalRecordNumber = reader.GetInt64(COL_FROMMRC),
                        FromFacilityOid = reader.GetInt64(COL_FROMHOSPITAL),
                        FromAccountNumber = reader.GetInt64(COL_FROMACCOUNT),

                        ToAdmitDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat(reader.GetString(COL_TOADMITDATE)),
                        ToAdmitTime = reader.GetInt32(COL_TOADMITTIME),

                        FromDischargeDate =
                            DateTimeUtilities.DateTimeForYYYYMMDDFormat(reader.GetString(COL_FROMDISCHARGEDATE)),
                        FromDischargeTime = reader.GetInt32(COL_FROMDISCHARGETIME),

                        ToFacilityOid = reader.GetInt64(COL_TOHOSPITAL),
                        ToMedicalRecordNumber = reader.GetInt64(COL_TOMRC),
                        ToAccountNumber = reader.GetInt64(COL_TOACCOUNT),
                        FromFacility = i_FacilityBroker.FacilityWith(reader.GetInt32(COL_FROMHOSPITAL)),
                        ToFacility = i_FacilityBroker.FacilityWith(reader.GetInt64(COL_TOHOSPITAL))
                    };
                }
            }
            catch (Exception ex)
            {
                const string message = "InterFacilityTransferPBARBroker for the given " +
                                       "facility, mrn, account  method failed with an unknown error.";
                throw BrokerExceptionFactory.BrokerExceptionFrom(message, ex, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }
            ConnectionString = "SDFDSF";
            return interFacilityTransferAccount;
        }

        public List<AccountProxy> GetAllAccountsForPatient(long PatientMRN, Facility FromFacility)
        {
            DataTable AccountsForPatient = new DataTable();
            AccountsForPatient.Columns.Add("AccountNumber");
            AccountsForPatient.Columns.Add("PatientType");
            AccountsForPatient.Columns.Add("KindOfVisit");
            AccountsForPatient.Columns.Add("HSV");
            AccountsForPatient.Columns.Add("IsPurged");
            AccountsForPatient.Columns.Add("AdmitDate");
            iDB2Command cmd = null;
            SafeReader reader = null;
            Patient ac=new Patient();
            List<AccountProxy> lstap = new List<AccountProxy>();

            try
            {
                cmd = CommandFor("CALL " + SP_MPI_ACCOUNTS +
                                 "(" + PARAM_MRN +
                                 "," + PARAM_FACILITYID + ")",
                    CommandType.Text,
                    FromFacility);

                cmd.Parameters[PARAM_MRN].Value = PatientMRN;
                cmd.Parameters[PARAM_FACILITYID].Value = FromFacility.Oid;

                reader = ExecuteReader(cmd);
                DataRow dr;
                dr = AccountsForPatient.NewRow();
                dr[0] = "";
                AccountsForPatient.Rows.Add(dr);
                
                while (reader.Read())
                {
                    AccountProxy ap = new AccountProxy();
                    dr = AccountsForPatient.NewRow();
                    var patientTypeString = reader.GetString(COL_PATIENTTYPE).ToString();
                    var fromPatientType = patientBroker.PatientTypeWith(FromFacility.Oid,
                        patientTypeString.Trim());

                    ap.HospitalService = hsvBroker.HospitalServiceWith(FromFacility.Oid, reader.GetString(COL_HOSPITALSERVICECODE).ToString());
                    var admitTime = reader.GetString(COL_ADMIT_TIME).ToString();
                    DateTime admitDate = DateTimeUtilities.FullDateTime(
                        reader.GetInt64(COL_ADMIT_DATE),
                        Convert.ToInt32(admitTime.PadLeft(4, '0').Substring(0, 2)),
                        Convert.ToInt32(admitTime.PadLeft(4, '0').Substring(2, 2)),
                        0);
                    long accountNumber = reader.GetInt64(COL_ACCOUNTNUMBER);

                    ap.AccountNumber = accountNumber;
                    ap.AdmitDate = admitDate;
                    ap.KindOfVisit = fromPatientType;

                    long lppAccountNumber = reader.GetInt64(COL_LPP_ACCOUNT_NUMBER);
                    ap.IsPurged = new YesNoFlag();
                    
                    if (lppAccountNumber != 0)
                    {
                        ap.IsPurged.SetNo();
                    }
                    else
                    {
                        ap.IsPurged.SetYes();
                    }
                    lstap.Add(ap);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            finally
            {

                Close(reader);
                Close(cmd);
            }

            return lstap;
        }

        public DataTable GetAccountsForPatient(long PatientMRN, Facility FromFacility)
        {
            var cutOffDate = DateTime.Now.Subtract(TimeSpan.FromDays(30));
            List<AccountProxy> patient =GetAllAccountsForPatient(PatientMRN, FromFacility);
            DataTable dtAccount = new DataTable();
            dtAccount.Columns.Add("AccountNumber");
            DataRow dr;
            dr = dtAccount.NewRow();
            dr[0] = string.Empty;
            dtAccount.Rows.Add(dr);
            if (patient.Count > 0)
            {
                foreach (AccountProxy ap in patient)
                {
                    //PatientAccess.User03   Registration Administrator
                    if (ap.KindOfVisit.IsEmergencyPatient &&
                        ap.AdmitDate > cutOffDate &&
                        (!ap.IsPurged.Code.Equals("Y")))
                    {
                        dr = dtAccount.NewRow();
                        dr[0] = ap.AccountNumber;
                        dtAccount.Rows.Add(dr);
                    }
                }
            }

            return dtAccount;
        }

        public ArrayList SortAccountsByDate(ArrayList accounts)
        {
            ArrayList newList = new ArrayList();

            foreach (AccountProxy proxy in accounts)
            {
                if (proxy.DischargeDate.Equals(EPOCH))
                {
                    newList.Add(proxy);

                }
            }
            foreach (AccountProxy proxy in accounts)
            {
                if (!proxy.DischargeDate.Equals(EPOCH))
                {
                    newList.Add(proxy);

                }
            }
            return (newList);
        }

        public ArrayList AllInterFacilityTransferHospitals(Facility facility)
        {
            ArrayList transferFacilitiesList = new ArrayList();
            iDB2Command databaseCommand = null;
            SafeReader reader = null;

            try
            {
                databaseCommand =
                    CommandFor(SP_GET_TRANSFERFACILITIES,
                        CommandType.Text,
                        facility);

                databaseCommand.Parameters[PARAM_FACILITYID].Value = facility.Code;
                reader = ExecuteReader(databaseCommand);

                while (reader.Read())
                {
                    var transferFacilityCode = reader.GetString(COL_TRANSFERFACILITYCODE);
                    if (transferFacilityCode.Trim().Length > 0)
                    {
                        transferFacilitiesList.Add(transferFacilityCode);
                    }
                }
            }

            finally
            {
                Close(reader);
                Close(databaseCommand);
            }

            return transferFacilitiesList;
        }

        public DataTable GETIFXRFROMHOSPITALACCOUNTSFOR(long PatientMRN, Facility FromFacility)
        {
            DataTable AccountsForPatient = new DataTable();
            AccountsForPatient.Columns.Add("AccountNumber");
            iDB2Command cmd = null;
            SafeReader reader = null;
            ITimeBroker tb = ProxyFactory.GetTimeBroker();
            DateTime facilityTime = tb.TimeAt(FromFacility.GMTOffset,
                FromFacility.DSTOffset);
            try
            {
                cmd = CommandFor("CALL " + SP_GETIFXRFROMHOSPITALACCOUNTSFOR +
                "(" + PARAM_FACILITYID +
                "," + PARAM_MRN + ")",
                CommandType.Text,
                FromFacility);
                cmd.Parameters[PARAM_MRN].Value = PatientMRN;
                cmd.Parameters[PARAM_FACILITYID].Value = FromFacility.Oid;

                reader = ExecuteReader(cmd);
                DataRow dr;
                dr = AccountsForPatient.NewRow();
                dr[0] = "";
                AccountsForPatient.Rows.Add(dr);
                while (reader.Read())
                {
                    AccountProxy ap = new AccountProxy();
                    var admitTime = reader.GetString(COL_FROMADMIT_TIME).ToString();
                    var admitDate = DateTimeUtilities.FullDateTime(
                    reader.GetInt64(COL_FROMADMIT_DATE),
                    Convert.ToInt32(admitTime.PadLeft(4, '0').Substring(0, 2)),
                    Convert.ToInt32(admitTime.PadLeft(4, '0').Substring(2, 2)),
                    0);
                    var dischargeDateFromDB = DateTimeUtilities.DateTimeForYYYYMMDDFormat(
                    reader.GetInt64(COL_DISCHARGEDATE));
                    var dischargeDate = new DateTime();
                    try
                    {
                        string dischargeTime = reader.GetString(COL_DISCHARGETIME).Trim();
                        int dischargeHour = Convert.ToInt32(dischargeTime.PadLeft(4, '0').Substring(0, 2));
                        int dischargeMin = Convert.ToInt32(dischargeTime.PadLeft(4, '0').Substring(2, 2));
                        dischargeDate = new DateTime(
                        dischargeDateFromDB.Year,
                        dischargeDateFromDB.Month,
                        dischargeDateFromDB.Day,
                        dischargeHour,
                        dischargeMin,
                        0);
                    }
                    catch
                    {
                        dischargeDate = new DateTime();
                    }

                    long accountNumber = reader.GetInt64(COL_FROMACCOUNT);

                    ap.AccountNumber = accountNumber;
                    ap.AdmitDate = admitDate;
                    ap.DischargeDate = dischargeDate;
                    TimeSpan timeSpan = facilityTime - dischargeDate;

                    if (timeSpan.TotalHours<=12 || reader.GetInt64(COL_DISCHARGEDATE)==0)
                    {
                        dr = AccountsForPatient.NewRow();
                        dr[0] = ap.AccountNumber;
                        AccountsForPatient.Rows.Add(dr);
                    }
                    AccountsForPatient.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
            finally
            {
                Close(reader);
                Close(cmd);
            }

            return AccountsForPatient;
        }

        #endregion

        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(InterFacilityTransferPBARBroker));
        #endregion

        #region Constants
        private const string
            SP_SELECTACCOUNTSFROMTRANSFERLOGFORDISCHARGE = "SELECTACCOUNTSFROMTRANSFERLOGFORDISCHARGE",
            SP_SELECTACCOUNTSFROMTRANSFERLOGFORREGISTRATION = "SELECTACCOUNTSFROMTRANSFERLOGFORREGISTRATION",
            SP_GETIFXRFROMHOSPITALACCOUNTSFOR = "GETIFXRFROMHOSPITALACCOUNTSFOR",
            SP_GET_TRANSFERFACILITIES = "CALL ALLINTERFACILITYFACILITYTRANSFERHOSPITALS( @P_FACILITYID )";

        private const string PARAM_MRN = "@P_MRN";
        private const string PARAM_ACCOUNTNUMBER = "@P_ACCOUNTNUMBER";
        public const string PARAM_FACILITYID = "@P_FACILITYID";

        private const string COL_FROMHOSPITAL = "FROMHOSPITAL";
        private const string COL_FROMMRC = "FROMMRC";
        private const string COL_FROMACCOUNT = "FROMACCOUNT";

        private const string COL_FROMDISCHARGEDATE = "FROMDISCHARGEDATE";
        private const string COL_FROMDISCHARGETIME = "FROMDISCHARGETIME";

        private const string COL_TOMRC = "TOMRC";
        private const string COL_TOHOSPITAL = "TOHOSPITAL";
        private const string COL_TOACCOUNT = "TOACCOUNT";

        private const string COL_TOADMITDATE = "TOADMITDATE";
        private const string COL_TOADMITTIME = "TOADMITTIME";
        private const string COL_FROMADMIT_DATE = "FROMADMITDATE";
        private const string COL_FROMADMIT_TIME = "FROMADMITTIME";
        private const string COL_ADMIT_DATE = "ADMITDATE";
        private const string COL_ADMIT_TIME = "ADMITTIME";

        private const string COL_ACCOUNTNUMBER = "AccountNumber";
        private const string COL_PATIENTTYPE = "PatientType";
        private const string COL_HOSPITALSERVICECODE = "HospitalServiceCode";
        private const string COL_LPP_ACCOUNT_NUMBER = "LPPACCOUNTNUMBER";
        private const string SP_MPI_ACCOUNTS = "MPIACCOUNTSFOR";
        private const string COL_TRANSFERFACILITYCODE = "TRANSFERFACILITYCODE";
        private const string COL_DISCHARGEDATE = "FROMDISCHARGEDATE";
        private const string COL_DISCHARGETIME = "FROMDISCHARGETIME";
        private readonly IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
        private readonly IHSVBroker hsvBroker = BrokerFactory.BrokerOfType<IHSVBroker>();
        private readonly DateTime EPOCH = new DateTime(1, 1, 1);
        private readonly IFacilityBroker i_FacilityBroker= BrokerFactory.BrokerOfType<IFacilityBroker>();

        #endregion
    }
}
