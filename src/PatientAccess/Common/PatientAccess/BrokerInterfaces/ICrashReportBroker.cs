using System.Collections;
using PatientAccess.BrokerInterfaces.CrashReporting;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ICrashReportBroker.
	/// </summary>
	public interface ICrashReportBroker
	{
        void Delete( long crashReportID );
        void Delete( string comments );
        void Save( CrashReport report );
        void Save( ArrayList crashReports );
    }
}
