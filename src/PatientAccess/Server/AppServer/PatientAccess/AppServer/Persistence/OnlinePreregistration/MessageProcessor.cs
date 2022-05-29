using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Messaging;
using PatientAccess.Persistence.Utilities;
using PatientAccess.Utilities;
using log4net;

namespace PatientAccess.Persistence.OnlinePreregistration
{
    public class MessageProcessor
    {
        private IPreRegistrationSubmissionRepository SubmissionRepository { get; set; }
        private IFacilityBroker FacilityBroker { get; set; }

        private static readonly XmlSchemaSet MessageSchemas = new XmlSchemaSet();

        static MessageProcessor()
        {
            var assembly = Assembly.GetAssembly( typeof( MessageProcessor ) );

            var preRegSchemaStream = assembly.GetManifestResourceStream( "PatientAccess.Resources.PreRegistrationData.xsd" );
            var guidSchemaStream = assembly.GetManifestResourceStream( "PatientAccess.Resources.GuidType.xsd" );


            var preRegistrationSchema = XmlSchema.Read( preRegSchemaStream, null );
            var guidSchema = XmlSchema.Read( guidSchemaStream, null );
            MessageSchemas.Add( guidSchema );
            MessageSchemas.Add( preRegistrationSchema );
        }

        public MessageProcessor( ILog logger, IFacilityBroker facilityBroker, IPreRegistrationSubmissionRepository submissionRepository )
        {
            Logger = logger;
            FacilityBroker = facilityBroker;
            SubmissionRepository = submissionRepository;
        }

        private ILog Logger { get; set; }

        public const string MessageType = "PAS_PRE_REGISTRATION";

        public void Process( string message, string messageType )
        {
            try
            {
                var savedMessageId = TryProcessing( message, messageType );

                Logger.Info( String.Format( "Message saved with id: {0}", savedMessageId ) );
            }

            catch ( FacilityNotFoundException exception )
            {
                Logger.Error( "Unable to process message because facility was not found. Error details: " + exception.Message, exception );
            }
        }

        /// <exception cref="InvalidOperationException">If message type is not "PAS_PRE_REGISTRATION", message xml does not validate against the schema</exception>
        /// <exception cref="ArgumentNullException"><c>message</c> is null or empty.</exception>
        /// <exception cref="ArgumentNullException"><c>messageType</c> is null or empty.</exception>
        /// <exception cref="FacilityNotFoundException">When the facility corresponding to the code is not found</exception>
        public Guid TryProcessing( string message, string messageType )
        {
            Guard.ThrowIfArgumentIsNullOrEmpty( message, "message" );

            Guard.ThrowIfArgumentIsNullOrEmpty( messageType, "messageType" );

            if ( messageType != MessageType )
            {
                const string errorMessage = @"Invalid message type, the message type should be ""PAS_PRE_REGISTRATION"" ";

                throw new InvalidOperationException( errorMessage );
            }

            var savedMessageId = Guid.Empty;

            XmlDocument messageXmlDocument;

            var messageIsValid = TryValidatingAndParsing( message, out messageXmlDocument );

            if ( messageIsValid )
            {
                var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );

                var preRegistrationData = (preRegistration)xmlSerializer.Deserialize( new StringReader( message ) );

                var facilityCode = preRegistrationData.facility.code;

                var facility = GetFacility( facilityCode );

                if ( facility.IsAcceptingPreRegistrationSubmissions() )
                {
                    messageXmlDocument.LoadXml( message );

                    ThrowIfEncodingAttributeValueIsBad( messageXmlDocument );

                    ThrowIfDateOfBirthIsOutOfRange( preRegistrationData );

                    ThrowIfAdmitDateIsOutOfRange( preRegistrationData );

                    savedMessageId = SaveMessage( preRegistrationData, messageXmlDocument, facility.Oid );
                }
            }

            else
            {
                const string errorMessage = "Could not validate the xml message, see Patient Access log for details";

                var invalidOperationException = new InvalidOperationException( errorMessage );

                Logger.Error( errorMessage, invalidOperationException );

                throw invalidOperationException;
            }

            return savedMessageId;
        }

        /// <summary>
        /// Tries to validate and parse the message. It catches (and logs) all exceptions generated during the process as we don't want these exceptions (especially XmlSchemaExceptions) 
        /// exposed to the outside world as they might have sensitive information from the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="document">The document.</param>
        /// <returns>true if the message validates against the message schema; false otherwise </returns>
        private bool TryValidatingAndParsing( string message, out XmlDocument document )
        {
            var readerSettings = new XmlReaderSettings
                                                   {
                                                       ValidationType = ValidationType.Schema,
                                                       ConformanceLevel = ConformanceLevel.Document
                                                   };

            readerSettings.Schemas.Add( MessageSchemas );

            document = new XmlDocument();

            try
            {
                //the XmlReaderSettings will ensure that the message is validated as it is loaded into the XmlDocument
                var reader = XmlReader.Create( new StringReader( message ), readerSettings );
                document.Load( reader );
                return true;
            }

            catch ( Exception e )
            {
                Logger.Error( e.Message, e );
                return false;
            }
        }

        /// <exception cref="FacilityNotFoundException">When the facility corresponding to the code is not found</exception>
        private Facility GetFacility( string facilityCode )
        {
            try
            {
                var facility = FacilityBroker.FacilityWith( facilityCode );
                return facility;
            }

            catch ( ArgumentException e )
            {
                string errorMessage = String.Format( @"No facility with facility code =""{0}"" found", facilityCode );
                throw new FacilityNotFoundException( errorMessage, e );
            }

        }

        /// <summary>
        /// //the sql server 2005 datetime data type has a narrower range for dates than the .Net framework DateTime type
        /// </summary>
        /// <param name="preRegistrationData">The pre registration data.</param>
        /// <exception cref="InvalidOperationException">The admit date must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.</exception>
        private static void ThrowIfAdmitDateIsOutOfRange( preRegistration preRegistrationData )
        {
            var isAdmitDateValid = DateTimeUtilities.IsValidSqlDateTime( preRegistrationData.visit.expectedAdmissionDateTime );

            if ( !isAdmitDateValid )
            {
                throw new InvalidOperationException( "The admit date must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM." );
            }
        }

        /// <summary>
        /// //the sql server 2005 datetime data type has a narrower range for dates than the .Net framework DateTime type
        /// </summary>
        /// <param name="preRegistrationData">The pre registration data.</param>
        /// <exception cref="InvalidOperationException">The admit date must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.</exception>
        private static void ThrowIfDateOfBirthIsOutOfRange( preRegistration preRegistrationData )
        {
            var isDateOfBirthValid = DateTimeUtilities.IsValidSqlDateTime( preRegistrationData.patient.dateOfBirth );

            if ( !isDateOfBirthValid )
            {
                const string errorMessage = "The date of birth must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM.";
                throw new InvalidOperationException( errorMessage );
            }
        }

        /// <summary>
        ///sql server's xml column does not store xml documents with encoding set to Utf-8, the actual encoding of the data does not matter
        ///Reference: http://www.mikewilson.cc/2007/09/23/xml-in-sql-server-2005/
        #pragma warning disable 1570
        ///http://www.google.com/search?q=sql+server+xml+type+utf-16&ie=utf-8&oe=utf-8&aq=t&rls=org.mozilla:en-US:official&client=firefox-a
        #pragma warning restore 1570
        /// </summary>
        /// <param name="messageXmlDocument">The message XML document.</param>
        /// <exception cref="InvalidOperationException">The message processor does not accept xml documents with the encoding attribute set to UTF-8, either omit the encoding attribute or use UTF-16, the actual encoding of the data does not matter</exception>
        private static void ThrowIfEncodingAttributeValueIsBad( XmlDocument messageXmlDocument )
        {
            bool isEncodingSetToUtf8 = IsEncodingSetToUtf8( messageXmlDocument );

            if ( isEncodingSetToUtf8 )
            {
                const string errorMessage = "The message processor does not accept xml documents with the encoding attribute set to UTF-8, either omit the encoding attribute or use UTF-16, the actual encoding of the data does not matter";
                throw new InvalidOperationException( errorMessage );
            }
        }

        private Guid SaveMessage( preRegistration preRegistrationData, XmlDocument messageXmlDocument, long facilityId )
        {
            var patientFactory = new PatientFactory( preRegistrationData );
            var patient = patientFactory.BuildNewPatient();

            var registrationSubmission = new PreRegistrationSubmission
                                             {
                                                 DateTimeReceived = preRegistrationData.dateTimeOfSubmission,
                                                 AdmitDate = patient.SelectedAccount.AdmitDate,
                                                 FirstName = patient.FirstName,
                                                 LastName = patient.LastName,
                                                 MiddleInitial = patient.MiddleInitial,
                                                 Gender = patient.Sex.Description,
                                                 DateOfBirth = patient.DateOfBirth,
                                                 ReturningPatient = preRegistrationData.visit.returningPatientSpecified
                                                                        ? (bool?)preRegistrationData.visit.returningPatient
                                                                        : null,
                                                 SSN = preRegistrationData.patient.socialSecurityNumber ?? String.Empty,
                                                 Address = patient.ContactPointWith( TypeOfContactPoint.NewMailingContactPointType() ).Address.OneLineAddressLabel(),
                                                 Message = messageXmlDocument,
                                                 FacilityId = facilityId
                                             };

            var savedMessageId = SubmissionRepository.Save( registrationSubmission );

            return savedMessageId;
        }

        private static bool IsEncodingSetToUtf8( XmlDocument xmlDocument )
        {
            if ( xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration )
            {
                var declaration = (XmlDeclaration)xmlDocument.FirstChild;
                const string utf8 = "UTF-8";
                int result = String.Compare( declaration.Encoding, utf8, true );

                if ( result == 0 )
                {
                    return true;
                }
            }

            return false;
        }
    }
}