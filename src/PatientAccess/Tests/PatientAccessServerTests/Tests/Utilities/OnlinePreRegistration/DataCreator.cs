using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Persistence.OnlinePreregistration;
using Tests.Unit.PatientAccess.Services.PreRegistration;

namespace Tests.Utilities.OnlinePreRegistration
{
    static class DataCreator
    {
        private const string PatientAccessResourcesSamplePreRegistrationXml = "Tests.Resources.OnlinePreRegistration.SamplePreRegistration.xml";

        public static XmlDocument GetMessageXmlDocumentFromSampleDocument()
        {
            string fileContents = GetXmlFromSampleDocument();

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml( fileContents );

            return xmlDocument;
        }

        public static string GetXmlFromSampleDocument()
        {
            return GetFileContentsFromAssemblyResource( PatientAccessResourcesSamplePreRegistrationXml );
        }

        public static XDocument GetMessageFromSampleDocument()
        {
            var document = GetMessageXmlDocumentFromSampleDocument();

            return XDocument.Load( new XmlNodeReader( document ) );
        }

        public static long GetFacilityIdFromCode( string facilityCode )
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var facility = facilityBroker.FacilityWith( facilityCode );
            return facility.Oid;
        }

        private static string GetFileContentsFromAssemblyResource( string patientAccessResourcesSamplePreRegistrationXml )
        {
            var assembly = Assembly.GetAssembly( typeof( MessageProcessorTests ) );
            var resourceStream = assembly.GetManifestResourceStream( patientAccessResourcesSamplePreRegistrationXml );
            var streamReader = new StreamReader( resourceStream );
            return streamReader.ReadToEnd();
        }

        public static string GetFacilityCode( PreRegistrationSubmission submissionToSave )
        {
            var message = XDocument.Parse( submissionToSave.Message.OuterXml );
            var facilityElementName = message.Root.Name.Namespace + "facility";
            var facilityCodeElementName = message.Root.Name.Namespace + "code";
            var facilityElement = message.Descendants( facilityElementName ).First();
            var facilityCode = facilityElement.Descendants( facilityCodeElementName ).First().Value;
            return facilityCode;
        }
    }
}
