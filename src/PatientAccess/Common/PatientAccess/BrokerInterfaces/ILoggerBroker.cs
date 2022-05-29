using log4net.Core;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ILoggerBroker.
	/// </summary>
    public interface ILoggerBroker
    {
        void LogEvent( LoggingEvent[] events );        
    }
}
