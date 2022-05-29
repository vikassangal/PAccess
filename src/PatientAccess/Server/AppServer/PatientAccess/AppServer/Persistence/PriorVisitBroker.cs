using System;
using System.Data;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using log4net;

namespace PatientAccess.Persistence
{
    /// <summary>
    /// Implements Demographics related methods.
    /// </summary>
    [Serializable]
    public class PriorVisitBroker : AbstractPBARBroker, IPriorVisitBroker
    {
       
        #region Methods
        public PriorVisitInformationResponse GetPriorVisitResponse(PriorVisitInformationRequest request)
        {
            var response = new PriorVisitInformationResponse();
            iDB2Command cmd = null;

            try
            {
                cmd = CommandFor("CALL " + SP_PRIOR_VIST_INFO +
                                 "(" + PARAM_FACILITY_CODE +
                                 "," + PARAM_ACCOUNT_NUMBER +
                                 "," + PARAM_MEDICAREHIC +
                                 "," + PARAM_CERTSSNHIC +
                                 "," + PARAM_MBINUMBER +
                                 "," + PARAM_FIRST4OfLASTNAME +
                                 "," + PARAM_FIRST1OFLASTNAME +
                                 "," + PARAM_GENDER +
                                 "," + PARAM_DATEOFBIRTH +
                                 "," + OUTPUT_PRIORHOSPITALCODE +
                                 "," + OUTPUT_PRIOREDICALRECORDNUMBER +
                                 "," + OUTPUT_PRIORACCOUNTNUMBER +
                                 "," + OUTPUT_PRIORADMITDATE +
                                 "," + OUTPUT_PRIORDISCHARGEDATE +
                                 "," + OUTPUT_PRIORMRDRG +
                                 ")",
                                 CommandType.Text,
                                 request.Facility);

                cmd.Parameters[PARAM_FACILITY_CODE].Value = request.Facility.Code;
                cmd.Parameters[PARAM_ACCOUNT_NUMBER].Value = request.AccountNumber.PadLeft(9, '0');
                cmd.Parameters[PARAM_MEDICAREHIC].Value = request.MedicareHic;
                cmd.Parameters[PARAM_CERTSSNHIC].Value = request.Certificate;
                cmd.Parameters[PARAM_FIRST4OfLASTNAME].Value = request.First4OfLastName;
                cmd.Parameters[PARAM_MBINUMBER].Value = request.MBINumber;
                cmd.Parameters[PARAM_FIRST1OFLASTNAME].Value = request.First1OfFirstName;
                cmd.Parameters[PARAM_GENDER].Value = request.Gender;
                cmd.Parameters[PARAM_DATEOFBIRTH].Value = request.DateOfBirth;

                cmd.Parameters[OUTPUT_PRIORHOSPITALCODE].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PRIORACCOUNTNUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PRIOREDICALRECORDNUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PRIORADMITDATE].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PRIOREDICALRECORDNUMBER].Direction = ParameterDirection.Output;
                cmd.Parameters[OUTPUT_PRIORMRDRG].Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                response.PriorAccountNumber = Convert.ToString(cmd.Parameters[OUTPUT_PRIORACCOUNTNUMBER].Value);
                response.PriorHospitalCode = Convert.ToString(cmd.Parameters[OUTPUT_PRIORHOSPITALCODE].Value);
                response.PriorAdmitDate = Convert.ToString(cmd.Parameters[OUTPUT_PRIORADMITDATE].Value);
                response.PriorDischargeDate = Convert.ToString(cmd.Parameters[OUTPUT_PRIORDISCHARGEDATE].Value);
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom(
                    "Unexpected Exception in Prior Visit Broker; Facility : " +
                    request.Facility.Oid + " and Account: " + request.AccountNumber, ex, c_log);
            }
            finally
            {
                base.Close(cmd);
            }
            return response;
        }

        #endregion
        
        #region Data Elements

        private static readonly ILog c_log =
            LogManager.GetLogger(typeof( PriorVisitBroker ));
        #endregion

        #region Constants

        private const string
            SP_PRIOR_VIST_INFO = "PriorVisitInfo";


        private const string
            PARAM_ACCOUNT_NUMBER = "@IACCOUNT",
            PARAM_FACILITY_CODE = "@IHOSPCODE",
            PARAM_MEDICAREHIC = "@IMEDICAREHIC#",
            PARAM_CERTSSNHIC = "@ICERTIFICATE#",
            PARAM_FIRST4OfLASTNAME = "@IFIRST4OFLASTNAME",
            PARAM_FIRST1OFLASTNAME = "@IFIRST1OFFIRSTNAME",
            PARAM_GENDER = "@IGENDER",
            PARAM_DATEOFBIRTH = "@IDOB8",
            PARAM_MBINUMBER = "@IMBI#",

            OUTPUT_PRIORHOSPITALCODE = "@OPRIORHOSPCD",
            OUTPUT_PRIOREDICALRECORDNUMBER = "@OPRIORMR#",
            OUTPUT_PRIORACCOUNTNUMBER = "@OPRIORACCT#",
            OUTPUT_PRIORADMITDATE = "@OPRIORADMITDT",
            OUTPUT_PRIORDISCHARGEDATE = "@OPRIORDISDT",
            OUTPUT_PRIORMRDRG = "@OPRIORMSDRG";

        #endregion
    }
}
