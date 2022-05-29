using System;
using System.Configuration;
using System.Text;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using cwbx;
using log4net;

namespace PatientAccess.Persistence.Utilities
{
    /// <summary>
    /// Summary description for IBMUtilities.
    /// </summary>
    //TODO: Create XML summary comment for IBMUtilities
    [Serializable]
    public class IBMUtilities : object
    {
        #region Event Handlers
        #endregion

        #region Methods

        internal static bool IsDatabaseAvailableFor( string facilityServerIP )
        {
            AS400System as400SystemClass = new AS400System();

            as400SystemClass.Define(facilityServerIP);

            return as400SystemClass.Verify(cwbcoServiceEnum.cwbcoServiceDatabase);
        }


        internal void LockAccount(long accountNumber, string userid,
            string workstationID, Facility facility)
        {
            SetHRAMLockTo(LOCK_OPERATOR_LOCK,
                accountNumber, userid, workstationID, facility);
        }


        internal void UnlockAccount(long accountNumber, string userid,
            string workstationID, Facility facility)
        {
            SetHRAMLockTo(LOCK_OPERATOR_UNLOCK,
                accountNumber, userid, workstationID, facility);
        }


        internal void FinishWithAccountLock(long accountNumber, string userid,
            string workstationID, Facility facility)
        {
            SetHRAMLockTo(LOCK_OPERATOR_FINISHED,
                accountNumber, userid, workstationID, facility);
        }

        internal void LockFlaggingAccount(long accountNumber, string userid,
            string workstationID, Facility facility)
        {
            SetHRAMLockTo(LOCK_OPERATOR_LOCKFLAGGING,
                accountNumber, userid, workstationID, facility);
        }

        /// <summary>
        /// Method gets the next available MRN from PBAR by calling NH0223
        /// the parameters to this utility are as follows:
        /// XXHSP# - Hospital Number - numeric 3.0 - this is a packed decimal number - input
        /// XXMRC# - The returned MRN - numeric 9.0 - this is a packed decimal number - inout but PA
        ///             only uses it as output
        /// CALLING - The type of calling program - 2 char alpha - this is always 'AR' for this class - input
        /// PROCPT - the processing point this module is at - 2 char alpha - this is always 
        ///             'AN' for this class - input
        /// FNSTS - The funtion to be called - 2 char alpha - this is always '  ' for this class - input
        /// PLSTS - Warning return codes related to pool on pbar - 2 char alpha - output
        /// FLSTS - Warning return codes related to the NH0223 function - 2 char alpha - output
        /// </summary>
        /// <param name="facilityID"></param>
        /// <returns></returns>
        public long GetNextUSCMRN(long facilityID)
        {
            long newMRN = INVALIDMRN;
            Program program = null;

            try
            {
                this.as400.Connect(cwbcoServiceEnum.cwbcoServiceRemoteCmd);

                StringConverter stringConverter = new StringConverter();
                LongConverter longConverter = new LongConverter();
                PackedConverter packedConverter = new PackedConverter();

                ProgramParameters pparms = new ProgramParameters();
                pparms.Clear();

                packedConverter.Digits = 3;
                packedConverter.DecimalPosition = 0;
                pparms.Append("XXHSP#", cwbrcParameterTypeEnum.cwbrcInput, 3).Value =
                    packedConverter.ToBytes(facilityID.ToString());

                packedConverter.Digits = 9;
                packedConverter.DecimalPosition = 0;
                ProgramParameter MRNParameter = pparms.Append("XXMRC#", cwbrcParameterTypeEnum.cwbrcInout, 9);
                MRNParameter.Value = packedConverter.ToBytes("0");

                stringConverter.Length = 2;
                pparms.Append("CALLNG", cwbrcParameterTypeEnum.cwbrcInout, 2).Value = stringConverter.ToBytes("AR");

                stringConverter.Length = 2;
                pparms.Append("PROCPT", cwbrcParameterTypeEnum.cwbrcInout, 2).Value = stringConverter.ToBytes("AN");

                stringConverter.Length = 2;
                pparms.Append("FNSTS", cwbrcParameterTypeEnum.cwbrcInout, 2).Value = stringConverter.ToBytes("  ");

                ProgramParameter poolStatusParameter = pparms.Append("PLSTS", cwbrcParameterTypeEnum.cwbrcInout, 2);
                stringConverter.Length = 2;
                poolStatusParameter.Value = stringConverter.ToBytes("  ");

                ProgramParameter fileStatusParameter = pparms.Append("FLSTS", cwbrcParameterTypeEnum.cwbrcInout, 2);
                stringConverter.Length = 2;
                fileStatusParameter.Value = stringConverter.ToBytes("  ");

                program = new Program();
                program.ProgramName = "NH0223";
                program.LibraryName = "NMLIBR";
                program.system = as400;

                program.Call(pparms);
                string mrn = packedConverter.FromBytes(MRNParameter.Value);
                c_log.Debug("New Mrn from GetNextUSCMRN " + mrn);

                stringConverter.Length = 2;
                string PLSTS = stringConverter.FromBytes(poolStatusParameter.Value);
                c_log.Debug("PLSTS return from GetNextUSCMRN " + PLSTS);

                stringConverter.Length = 2;
                string FLSTS = stringConverter.FromBytes(fileStatusParameter.Value);
                c_log.Debug("FLSTS return from GetNextUSCMRN " + FLSTS);

                newMRN = HandleNH0223ReturnCodes(
                            long.Parse(mrn),
                            PLSTS,
                            FLSTS,
                            facilityID);
            }
            catch (Exception ex)
            {
                c_log.ErrorFormat("Call to NH0223 failed {0}", ex.Message);
            }
            finally
            {
                if (program != null && program.system != null)
                {
                    program.system.Disconnect(cwbcoServiceEnum.cwbcoServiceAll);
                }
            }
            return newMRN;
        }

        public int GetNextEmployerNumber(string areaName, string libName)
        {
            int returnVal = DEFAULTRETURNVAL;
            try
            {
                this.as400.Connect(cwbcoServiceEnum.cwbcoServiceRemoteCmd);
                this.DataAreaName = areaName;
                this.DataAreaLib = libName;
                returnVal = this.GetNextValue(as400);
                if (returnVal != DEFAULTRETURNVAL)
                {
                    this.BumpValue(this.as400, returnVal);
                    returnVal++;
                }

                this.as400.Disconnect(cwbcoServiceEnum.cwbcoServiceAll);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return returnVal;
        }
        #endregion

        #region Properties

        private string ServerIP
        {
            get
            {
                return i_ServerIP;
            }
            set
            {
                i_ServerIP = value;
            }
        }

        private string UserID
        {
            get
            {
                return i_UserID;
            }
            set
            {
                i_UserID = value;
            }
        }

        private string Password
        {
            get
            {
                return i_Password;
            }
            set
            {
                i_Password = value;
            }
        }

        private AS400System as400
        {
            get
            {
                return i_as400;
            }
        }
        private string DataAreaName
        {
            get
            {
                return i_DataAreaName;
            }
            set
            {
                i_DataAreaName = value;
            }
        }
        private string DataAreaLib
        {
            get
            {
                return i_DataAreaLib;
            }
            set
            {
                i_DataAreaLib = value;
            }
        }
        #endregion

        #region Private Methods
        private long HandleNH0223ReturnCodes(long TestMRN, string PLSTS, string FLSTS, long facilityID)
        {
            long newMRN = INVALIDMRN;

            if (PLSTS.Equals("CR"))
            {
                c_log.ErrorFormat("NH0223 - USC MRN Pool running below minimum for facility {0}. Contact Clinical Application Services.",
                    facilityID);
            }
            if (PLSTS.Equals("EM"))
            {
                c_log.ErrorFormat("NH0223 - USC MRN Pool EXHAUSTED for faciltiy {0}. ContactClinical Application Services Immediately.",
                    facilityID);
            }
            if (FLSTS.Equals("XX"))
            {
                c_log.ErrorFormat("NH0223 - Attempted to run NH0223 against facility {0} which is not configured for this function",
                    facilityID);
            }
            if (FLSTS.Equals("AP"))
            {
                newMRN = TestMRN;
            }

            return newMRN;
        }
        /// <summary>
        /// This routine gets the next value from a data area which is used
        /// as a sequence.
        /// </summary>
        /// <param name="as400"></param>
        /// <returns></returns>
        private int GetNextValue(AS400System as400)
        {
            int ipStart = 1;
            const int ipDataLen = 50;
            const int errorc = 0;
            const int data_len = 4;
            string da = this.DataAreaName + this.DataAreaLib;//DATAAREANAME + DATAAREALIB; //"EM#       PALIBR    ";

            Program program = new Program();
            program.ProgramName = "QWCRDTAA";
            program.LibraryName = "QSYS";
            program.system = as400;

            StringConverter stringConverter = new StringConverter();
            LongConverter longConverter = new LongConverter();

            ProgramParameters pparms = new ProgramParameters();

            pparms.Clear();
            stringConverter.Length = 20;

            ProgramParameter ppout = pparms.Append("return_value", cwbrcParameterTypeEnum.cwbrcOutput, ipDataLen);

            pparms.Append("length", cwbrcParameterTypeEnum.cwbrcInput, 4).Value = longConverter.ToBytes(ipDataLen);
            pparms.Append("data_area_name", cwbrcParameterTypeEnum.cwbrcInput, 20).Value = stringConverter.ToBytes(da);
            pparms.Append("start_pos", cwbrcParameterTypeEnum.cwbrcInput, 4).Value = longConverter.ToBytes(ipStart);
            pparms.Append("data_len", cwbrcParameterTypeEnum.cwbrcInput, 4).Value = longConverter.ToBytes(data_len);
            pparms.Append("errorc", cwbrcParameterTypeEnum.cwbrcInput, 4).Value = longConverter.ToBytes(errorc);

            program.Call(pparms);

            //stringConverter.Length = ipDataLen;
            //string spath = stringConverter.FromBytes(ppout.Value);

            PackedConverter packedConverter = new PackedConverter();
            packedConverter.Digits = 7;
            packedConverter.DecimalPosition = 0;

            byte[] ppbytes = new byte[4];
            Array.Copy((Byte[])ppout.Value, 36, ppbytes, 0, 4);
            string number = packedConverter.FromBytes(ppbytes);

            return (Convert.ToInt32(number));
        }

        private void BumpValue(AS400System as400, int curVal)
        {
            Command cmd = new Command();

            curVal++;
            string snumber = Convert.ToString(curVal);

            cmd.system = as400;

            cmd.Run("chgdtaara dtaara(" + this.DataAreaLib.Trim() + "/" + this.DataAreaName.Trim() + ") value(" + snumber.Trim() + ")");
        }

        private string CreateCMD(long accountNumber, string lockCmd,
            string userid, string workstationID,
            Facility facility)
        {
            // the account number must be a 9 digit string padded on the left with zeros
            string an = MakePaddedNumber(accountNumber, 9, '0');
            string wsID = workstationID.PadRight(10, ' ').Substring(0, 10);

            ITimeBroker timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
            DateTime d = timeBroker.TimeAt(facility.GMTOffset,
                                            facility.DSTOffset);

            string hspCode = facility.Oid.ToString().PadLeft(3, '0');
            string userProfile = userid.PadRight(10, ' ');

            StringBuilder sb = new StringBuilder();
            sb.Append("SNDMSG MSG('");
            sb.Append(lockCmd);
            sb.Append(facility.Code);
            sb.Append(MakePaddedNumber(accountNumber, 9, '0'));
            sb.Append("                          "); //HRKEY                 18     52      acct # (left-justified; include leading zeros; pad w/ blanks)
            sb.Append("01");    //HRRCID                53     54  0    01
            sb.Append(wsID); //"SEDOTAKJ4 "; //HRJOBN                55     64      job name (workstation ID)
            sb.Append(userProfile); //HRUSER                65     74      user profile (e.g., NMxxxUSER)
            sb.Append("123456");     //HRJOB#                75     80  0   job number of workstation job
            sb.Append("PACCESS   "); //HRPGM                 81     90      locking/unlocking program name
            sb.Append(hspCode);        //HRHSP#                91     93  0   3-digit HPMS Hospital # (e.g., 098)
            sb.Append(d.ToString("yyMMdd")); //HRCDAT                94     99  0   date (YYMMDD)
            sb.Append(d.ToString("HHmmss")); //HRCTIM               100    105  0   time (HHMMSS)
            sb.Append("') TOMSGQ(HRAMMQ)");

            return sb.ToString();
        }

        public static string MakePaddedNumber(long num, int len, char padChar)
        {
            string returnStr = null;

            string s = num.ToString();

            if (s.Length > len)
            {
                returnStr = s.Substring(0, len);
            }
            else
            {
                while (s.Length < len)
                {
                    s = padChar + s;
                }
                returnStr = s;
            }

            return returnStr;
        }
        private void SetHRAMLockTo(string lockMode, long accountNumber,
            string userid, string workstationID,
            Facility facility)
        {
            try
            {
                Command cmd = new Command();

                cmd.system = as400;

                string cmdstr = this.CreateCMD(accountNumber, lockMode, userid, workstationID, facility);
                c_log.Info(cmdstr);
                this.as400.Connect(cwbcoServiceEnum.cwbcoServiceRemoteCmd);
                cmd.Run(cmdstr);
                this.as400.Disconnect(cwbcoServiceEnum.cwbcoServiceAll);
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(ex, c_log);
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public IBMUtilities()
            :
            this(
            ConfigurationManager.AppSettings[DB2UTIL_SYSTEM_NAME],
            ConfigurationManager.AppSettings[DB2UTIL_USER_NAME],
            ConfigurationManager.AppSettings[DB2UTIL_PASSWORD])
        {
        }

        public IBMUtilities(Facility aFacility)
        {
            string userID = ConfigurationManager.AppSettings[DB2UTIL_USER_NAME];
            string password = ConfigurationManager.AppSettings[DB2UTIL_PASSWORD];
            UserID = userID;
            Password = password;
            ConnectionSpec spec = aFacility.ConnectionSpec;
            i_as400 = new AS400System();
            this.i_as400.Define(spec.ServerIP);
            this.i_as400.UserID = UserID;
            this.i_as400.Password = Password;
        }

        private IBMUtilities(string serverIP, string userID, string password)
        {
            ServerIP = serverIP;
            UserID = userID;
            Password = password;
            i_as400 = new AS400System();
            i_as400.Define(ServerIP);
            i_as400.UserID = UserID;
            i_as400.Password = password;
        }
        #endregion

        #region Data Elements
        private AS400System i_as400;
        private string i_ServerIP;
        private string i_UserID;
        private string i_Password;
        private string i_DataAreaLib;
        private string i_DataAreaName;

        private string
            LOCK_OPERATOR_LOCK = "LC",
            LOCK_OPERATOR_UNLOCK = "UC",
            LOCK_OPERATOR_FINISHED = "FG",
            LOCK_OPERATOR_LOCKFLAGGING = "LF";


        #endregion

        #region Constants
        public const long INVALIDMRN = -1;

        private const int DEFAULTRETURNVAL = -1;
        private const string //DATAAREANAME = "EM#       ",
            //DATAAREALIB  = "PALIBR    ",

            DB2UTIL_SYSTEM_NAME = "DB2UTIL_SYSTEM_NAME",
            DB2UTIL_USER_NAME = "DB2UTIL_USER_NAME",
            DB2UTIL_PASSWORD = "DB2UTIL_PASSWORD";

        private static readonly ILog c_log =
            LogManager.GetLogger(typeof(IBMUtilities));

        #endregion
    }

}