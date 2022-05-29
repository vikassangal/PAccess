using System;
using PatientAccess.BrokerInterfaces;
using log4net;
using log4net.Core;
using log4net.Repository;

namespace PatientAccess.Persistence
{
    public class LoggerBroker : MarshalByRefObject, ILoggerBroker
    {
		#region Fields 

        private static readonly ILoggerRepository _log4NetRepository = LogManager.GetRepository();

		#endregion Fields 

		#region Constructors 

        public LoggerBroker()
        {
        }

		#endregion Constructors 

		#region Properties 

        private static ILoggerRepository Log4netRepository
        {
            get
            {
                return _log4NetRepository;
            }
        }

		#endregion Properties 

		#region Methods 

        public void LogEvent( LoggingEvent[] events )
        {
            if (events != null)
            {
                foreach(LoggingEvent logEvent in events)
                {
                    if (logEvent != null)
                    {
                        Log4netRepository.Log( logEvent );
                    }
                }
            }
        }

		#endregion Methods 
    }
}
