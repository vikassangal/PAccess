using Extensions.Persistence;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using System.Data.SqlClient;
using log4net;
using System;
using System.Collections;
using System.Data;

namespace PatientAccess.Persistence
{
    public class ServiceCategoryBroker : AbstractBroker, IServiceCategoryBroker
    {
        private const string DBPROCEDURE_GETSERVICECATEGORYFORCLINICCODE = "DOFR.GetServiceCategoryForClinicCode";
        private const string DBCOLUMN_SERVICECATEGORY = "ServiceCategory";
        private const string DBCOLUMN_SERVICECATEGORYCODE = "ServiceCategoryCode";
        private const string DBPARAM_FACILITYID = "@FacilityId";
        private const string DBPARAM_CLINICCODE = "@ClinicCode";
        private static readonly ILog c_log = LogManager.GetLogger(typeof(ServiceCategoryBroker));

        public ServiceCategoryBroker() : base()
        {
        }

        public ServiceCategoryBroker(string cxnString) : base(cxnString)
        {
        }

        public ServiceCategoryBroker(SqlTransaction txn) : base(txn)
        {
        }
        public ArrayList GetServiceCategoryForClinicCode(long FacilityId, string ClinicCode)
        {
            ArrayList servicecategories = new ArrayList();

            SafeReader reader = null;
            SqlCommand cmd = null;

            try
            {
                cmd = this.CommandFor(DBPROCEDURE_GETSERVICECATEGORYFORCLINICCODE);
               
                SqlParameter facilityIdParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_FACILITYID,SqlDbType.Int));
                facilityIdParam.Value = FacilityId;
                
                SqlParameter clinicCodeParam = cmd.Parameters.Add(new SqlParameter(DBPARAM_CLINICCODE, SqlDbType.VarChar));
                clinicCodeParam.Value = ClinicCode;

                reader = this.ExecuteReader(cmd);

                while (reader.Read())
                {
                    string name = reader.GetString(DBCOLUMN_SERVICECATEGORY);
                    string code = reader.GetString(DBCOLUMN_SERVICECATEGORYCODE);
                    ClinicServiceCategory sc = new ClinicServiceCategory(name, code);
                    servicecategories.Add(sc);
                }
            }
            catch (Exception ex)
            {
                throw BrokerExceptionFactory.BrokerExceptionFrom("Unexpected Exception reading servicecategory", ex, c_log);
            }
            finally
            {
                base.Close(reader);
                base.Close(cmd);
            }

            return servicecategories;
        }

        public ClinicServiceCategory GetServiceCategoryForClinicCodeWith(long FacilityId, string ClinicCode,string serviceCategoryCode)
        {
            var serviceCategories = GetServiceCategoryForClinicCode(FacilityId, ClinicCode);
            foreach(var serviceCategory in serviceCategories)
            {
                if(((ClinicServiceCategory)serviceCategory).Code.Equals(serviceCategoryCode))
                {
                    return (ClinicServiceCategory)serviceCategory;
                }
            }
            return null;
        }
    }
}