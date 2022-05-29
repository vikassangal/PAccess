using System;
using System.Data;
using IBM.Data.DB2.iSeries;
using PatientAccess.Domain;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence
{
    public class PbarUserBroker : AbstractPBARBroker
    {
        private static readonly ILog Log = LogManager.GetLogger( typeof( PbarUserBroker ) );

        public void UpdateLastLoginDateForPbarUser( string pbarEmployeeId, Facility facility )
        {
            Guard.ThrowIfArgumentIsNull( facility, "facility" );

            iDB2Command cmd = null;
            
            var facilityDateTime = facility.GetCurrentDateTime().ToString( "yyyyMMdd" );

            try
            {
                var parameters = new[] { PARAM_PBARID, PARAM_CURRENTDATE };

                var cmdTextForUpdatingLastLogin = new Db2StoredProcedureCallBuilder( parameters, SP_UPDATELASTLOGINDATEFORUSER ).Build();

                cmd = CommandFor( cmdTextForUpdatingLastLogin, CommandType.Text, facility );

                cmd.Parameters[PARAM_PBARID].Value = pbarEmployeeId;

                cmd.Parameters[PARAM_CURRENTDATE].Value = facilityDateTime;

                cmd.ExecuteScalar();

                Log.Info( string.Format( "Updated last login date for PBAR user: '{0}' for facility: '{1}' and facility time: '{2}'", pbarEmployeeId, facility.Code, facilityDateTime ) );
            }

            catch ( Exception exception )
            {
                string message = string.Format( "An error occured while updating the last login date for PBAR user: '{0}' for facility '{1}' and facility time: '{2}'", pbarEmployeeId, facility.Code, facilityDateTime ) + exception.Message;

                Log.Error( message );
            }

            finally
            {
                Close( cmd );
            }
        }

        private const string
            SP_UPDATELASTLOGINDATEFORUSER = "UPDATELASTLOGINDATEFORUSER",
            PARAM_PBARID = "@P_PBARID",
            PARAM_CURRENTDATE = "@P_LOGINDATE";
    }
}