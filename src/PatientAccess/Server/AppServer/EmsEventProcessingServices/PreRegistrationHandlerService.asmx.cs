using System;
using System.Web.Services;
using System.Web.Services.Protocols;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Persistence.OnlinePreregistration;
using log4net;

namespace PatientAccess.Services.EmsEventProcessing
{
    public class PreRegistrationHandlerService : WebService
    {
        private static readonly ILog Logger = LogManager.GetLogger( typeof( PreRegistrationHandlerService ) );
        private readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly IPreRegistrationSubmissionRepository submissionRepository = new PreRegistrationSubmissionRepository();

        /// <summary>
        /// Processes the message. We cannot use a proper namespace for this web service as the tempuri namespace is hardcoded in EMS
        /// and it expects a service handler with the visual studio generated temporary namespace.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <exception cref="SoapException"><c>SoapException</c>.</exception>
        [WebMethod( Description = "Process pre-registration message", EnableSession = false )]
        public void ProcessMessage( string message, string messageType )
        {
            var messageProcessor = new MessageProcessor( Logger, facilityBroker, submissionRepository );

            try
            {
                messageProcessor.Process( message, messageType );
            }

            catch ( Exception exception )
            {
                Logger.Error( "Unable to process message", exception );

                throw new SoapException( exception.Message, SoapException.ServerFaultCode, exception.StackTrace );
            }
        }
    }
}