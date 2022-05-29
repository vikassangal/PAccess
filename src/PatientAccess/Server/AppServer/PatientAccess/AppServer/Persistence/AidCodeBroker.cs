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
using PatientAccess.Persistence;
using System.Collections;
using System.Data;

namespace PatientAccess.Persistence
{
    public class AidCodeBroker : AbstractBroker, IAidCodeBroker
    {
        private const string DBPROCEDURE_GETAIDCODEMAPPING = "DOFR.GetAidCodeMapping";
        private const string DBCOLUMN_AIDCODE = "AidCode";
        private const string DBCOLUMN_TYPE = "Type";
        private const string DBPARAM_FACILITYID = "@FacilityId";

        private const string DBPROCEDURE_GETCALOPTIMAPLANIDS = "DOFR.GetCalOptimaPlanIds";
        private const string DBCOLUMN_PLANID = "PlanId";

        private static readonly ILog c_log = LogManager.GetLogger(typeof(AidCodeBroker));

        public AidCodeBroker() : base()
        {
        }

        public AidCodeBroker(string cxnString) : base(cxnString)
        {
        }

        public AidCodeBroker(SqlTransaction txn) : base(txn)
        {
        }
        public ArrayList GetCalOptimaPlanIds(int FacilityId)
        {
            ArrayList calOptimaPlanIds = new ArrayList();

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(DBPROCEDURE_GETCALOPTIMAPLANIDS);
               
                SqlParameter facilityIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_FACILITYID,SqlDbType.Int));
                facilityIdParam.Value = FacilityId;
                
                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string planidname = reader.GetString(DBCOLUMN_PLANID);
                    string type = reader.GetString(DBCOLUMN_TYPE);
                    CalOptimaPlanId planid = new CalOptimaPlanId(planidname, type);
                    calOptimaPlanIds.Add(planid);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception reading GetCalOptimaPlanIds", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return calOptimaPlanIds;
        }

        public ArrayList GetAidCode(int FacilityId)
        {
            ArrayList aidCodes = new ArrayList();

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(DBPROCEDURE_GETAIDCODEMAPPING);

                SqlParameter facilityIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_FACILITYID, SqlDbType.Int));
                facilityIdParam.Value = FacilityId;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string aidcodename = reader.GetString(DBCOLUMN_AIDCODE);
                    string type = reader.GetString(DBCOLUMN_TYPE);
                    AidCode aidcode = new AidCode(aidcodename, type);
                    aidCodes.Add(aidcode);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception reading GetAidCode", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return aidCodes;
        }
    }
}