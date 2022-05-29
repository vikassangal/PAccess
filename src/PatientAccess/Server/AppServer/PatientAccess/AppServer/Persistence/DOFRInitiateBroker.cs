using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.AppServer.Services;
using System.Data.SqlClient;
using log4net;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.Data;

namespace PatientAccess.Persistence
{
    public class DOFRInitiateBroker : AbstractBroker, IDOFRInitiateBroker
    {
        public DOFRInitiateBroker() : base()
        {
        }

        public DOFRInitiateBroker(string cxnString) : base(cxnString)
        {
        }

        public DOFRInitiateBroker(SqlTransaction txn) : base(txn)
        {
        }
        public Tuple<DOFRAPIResponse, HttpStatusCode> DOFRInitiate(DOFRAPIRequest dOFRAPIRequest, Account account)
        {
            
            try
            {
                DOFRInitiateService dOFRInitiateService = new DOFRInitiateService();
                var response = dOFRInitiateService.Predict(dOFRAPIRequest);
                JavaScriptSerializer js = new JavaScriptSerializer();
                string httpResponse = response.Content.ReadAsStringAsync().Result;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        var dOFRAPIResponses = js.Deserialize<List<DOFRAPIResponse>>(httpResponse);
                        var dOFRAPIResponse = dOFRAPIResponses.SingleOrDefault();
                        return new Tuple<DOFRAPIResponse, HttpStatusCode>(dOFRAPIResponse, response.StatusCode);
                    default:
                        string msg = string.Format("DOFR API Failed.\nPASUser:{0}\nStatusCode:{1}\nDOFRAPIRequest:[{2}]\nDOFRAPIResponse:[{3}]",
                            account.Activity.AppUser.UserID,
                            response.StatusCode.ToString(),
                            dOFRAPIRequest.ToString(),
                            httpResponse);
                        c_log.Error(msg, BrokerExceptionFactory.BrokerExceptionFrom(msg, new Exception("DOFR API Failed")));
                        return new Tuple<DOFRAPIResponse, HttpStatusCode>(null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("DOFR API Failed.\nPASUser:{0}\nDOFRAPIRequest:[{1}]\nError:[{2}]",
                          account.Activity.AppUser.UserID,
                          dOFRAPIRequest.ToString(),
                          ex.Message);
                
                c_log.Error(msg, BrokerExceptionFactory.BrokerExceptionFrom(msg, ex));
                var statuscode = HttpStatusCode.InternalServerError;
               if ( ex.GetType() == typeof(System.Threading.Tasks.TaskCanceledException) || ex.InnerException.GetType() == typeof(System.Threading.Tasks.TaskCanceledException))
                    statuscode = HttpStatusCode.RequestTimeout;

                return new Tuple<DOFRAPIResponse, HttpStatusCode>(null, statuscode);
            }
        }

        public string GetInsurancePlanPartOfIPA(Account account)
        {
            string returnInsurancePlanPartOfIPA = string.Empty;

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(DBPROCEDURE_GetInsurancePlanPartOfIPA);

                SqlParameter facilityIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_FacilityId, SqlDbType.Int));
                facilityIdParam.Value = (int)account.Facility.Oid;

                SqlParameter accountNumberParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_AccountNumber, SqlDbType.VarChar));
                accountNumberParam.Value = account.AccountNumber;

                SqlParameter planIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_PlanId, SqlDbType.VarChar));
                planIdParam.Value =account.Insurance.PrimaryCoverage.InsurancePlan.PlanID;


                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    returnInsurancePlanPartOfIPA = reader.GetString(DBCOLUMN_InsurancePlanPartOfIPA);
                }
               
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception reading GetInsurancePlanPartOfIPA", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return returnInsurancePlanPartOfIPA;
        }

        public void SetInsurancePlanPartOfIPA(Account account)
        {
            string returnInsurancePlanPartOfIPA = string.Empty;

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(DBPROCEDURE_SetInsurancePlanPartOfIPA);

                SqlParameter facilityIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_FacilityId, SqlDbType.Int));
                facilityIdParam.Value = (int)account.Facility.Oid;

                SqlParameter accountNumberParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_AccountNumber, SqlDbType.VarChar));
                accountNumberParam.Value = account.AccountNumber;

                SqlParameter planIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_PlanId, SqlDbType.VarChar));
                planIdParam.Value = account.Insurance.PrimaryCoverage.InsurancePlan.PlanID;

                SqlParameter insurancePlanPartOfIPAParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_InsurancePlanPartOfIPA, SqlDbType.VarChar));
                insurancePlanPartOfIPAParam.Value = ((CommercialCoverage)account.Insurance.PrimaryCoverage).IsInsurancePlanPartOfIPA.ToString();

                this.ExecuteNonQuery(cmd);

            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception saving SetInsurancePlanPartOfIPA", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }
        }

        private const string DBPROCEDURE_GetInsurancePlanPartOfIPA = "DOFR.GetInsurancePlanPartOfIPA";
        private const string DBPROCEDURE_SetInsurancePlanPartOfIPA = "DOFR.SetInsurancePlanPartOfIPA";
        private const string DBCOLUMN_InsurancePlanPartOfIPA = "InsurancePlanPartOfIPA";
        private const string DBPARAM_FacilityId = "@FacilityId";
        private const string DBPARAM_AccountNumber = "@AccountNumber";
        private const string DBPARAM_PlanId = "@PlanId";
        private const string DBPARAM_InsurancePlanPartOfIPA = "@InsurancePlanPartOfIPA";

        private static readonly ILog c_log =
           LogManager.GetLogger(typeof(DOFRInitiateBroker));
    }
}