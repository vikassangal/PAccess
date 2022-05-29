using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NSubstitute;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.OnlinePreregistration;
using NUnit.Framework;
using Tests.Utilities.OnlinePreRegistration;
using log4net;

namespace Tests.Unit.PatientAccess.Services.PreRegistration
{
    [TestFixture]
    public class MessageProcessorTests
    {
        private const string FacilityCodeForFacilityThatDoesNotExist = "XYZ";

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        [Category( "Fast" )]
        public void WhenMessageAndMessageTypeAreNull_ShouldThrowException()
        {
            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( null, null );
        }

        [Test]
        [Category( "Fast" )]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void WhenMessageAndMessageTypeAreEmpty_ShouldThrowException()
        {
            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( string.Empty, string.Empty );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenMessageTypeIsInvalid_ShouldThrowException()
        {
            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( "some message", "invalid message type" );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenMessageIsNotWellFormedXml_ShouldThrowException()
        {
            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( "some message", MessageProcessor.MessageType );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenMessageIsWellFormedXmlButDoesNotValidate_ShouldThrowException()
        {
            const string wellFormedXml = @"<?xml version=""1.0"" standalone=""yes""?><Test>Some text</Test>";

            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( wellFormedXml, MessageProcessor.MessageType );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenMissingFacilitySection_ShouldThrowException()
        {
            var message = DataCreator.GetMessageFromSampleDocument();

            message = RemoveFacilityElementChildren( message );

            var messageText = message.ToString();

            var messageProcessor = GetMessageProcessorWithAllStubDependencies();

            messageProcessor.TryProcessing( messageText, MessageProcessor.MessageType );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenMessageIsValidButXmlHasUtf8ForEncodingAttribute_ShouldThrowException()
        {
            var message = DataCreator.GetMessageXmlDocumentFromSampleDocument();
            var declaration = (XmlDeclaration)message.FirstChild;
            declaration.Encoding = "Utf-8";
            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( message.OuterXml, MessageProcessor.MessageType );
        }

        [Test]
        [ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void WhenDateOfBirthIsOutOfTheSqlServerRange_ShouldThrowException()
        {
            var newMessage = new MessageBuilder()
                                .SetPatientAdmitDate( new DateTime( 1752, 1, 1 ) )
                                .BuildMessage();

            var messageProcessor = GetMessageProcessorWithAllStubDependencies();
            messageProcessor.TryProcessing( newMessage, MessageProcessor.MessageType );
        }

        [Test]
        public void WhenMessageIsValid_ShouldNotThrowAnyException()
        {
            var message = DataCreator.GetMessageFromSampleDocument();
            var messageProcessor = GetMessageProcessorWithRealFacilityBroker();
            messageProcessor.TryProcessing( message.ToString(), MessageProcessor.MessageType );
        }

        [Test]
        [ExpectedException( typeof( FacilityNotFoundException ) )]
        public void TestTryProcessWithInvalidFacilityCode_ShouldThrowException()
        {
            var newMessage = new MessageBuilder()
                                .SetFacilityCode( FacilityCodeForFacilityThatDoesNotExist )
                                .BuildMessage();

            var messageProcessor = GetMessageProcessorWithRealFacilityBroker();
            messageProcessor.TryProcessing( newMessage, MessageProcessor.MessageType );
        }

        [Test]
        public void TestProcessWithInvalidFacilityCode_ShouldNotThrowException()
        {
            var newMessage = new MessageBuilder()
                                .SetFacilityCode( FacilityCodeForFacilityThatDoesNotExist )
                                .BuildMessage();

            var messageProcessor = GetMessageProcessorWithRealFacilityBroker();
            messageProcessor.Process( newMessage, MessageProcessor.MessageType );
        }


        [Test]
        public void TestPatientCreationFromValidInputMessage()
        {
            var inputMessage = DataCreator.GetMessageFromSampleDocument();

            var stubLogger = Substitute.For<ILog>();
            var stubFacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var submissionRepository = Substitute.For<IPreRegistrationSubmissionRepository>();
            var messageProcessor = new MessageProcessor( stubLogger, stubFacilityBroker, submissionRepository );

            messageProcessor.TryProcessing( inputMessage.ToString(), MessageProcessor.MessageType );

            submissionRepository.Received().Save( Arg.Is<PreRegistrationSubmission>( y => !string.IsNullOrEmpty( y.FirstName ) ) );
        }

        [Test]
        public void TestGetSubmittedMessagesList_WhenFacilityAcceptsMessages_MessageShouldBeSaved()
        {

            var newMessage = new MessageBuilder()
                                .SetFacilityCode( "DEL" )
                                .BuildMessage();

            var stubLogger = Substitute.For<ILog>();

            var stubFacilityBroker = Substitute.For<IFacilityBroker>();

            var facilityAcceptingRegistrations = new Facility();

            facilityAcceptingRegistrations["IsAcceptingPreRegistrationSubmissions"] = new object();

            stubFacilityBroker.FacilityWith( string.Empty ).ReturnsForAnyArgs( facilityAcceptingRegistrations );

            var submissionRepository = Substitute.For<IPreRegistrationSubmissionRepository>();

            var messageProcessor = new MessageProcessor( stubLogger, stubFacilityBroker, submissionRepository );

            messageProcessor.TryProcessing( newMessage, MessageProcessor.MessageType );

            Assert.IsTrue( facilityAcceptingRegistrations.IsAcceptingPreRegistrationSubmissions() );

            submissionRepository.Received().Save( Arg.Is<PreRegistrationSubmission>( y => !string.IsNullOrEmpty( y.FirstName ) ) );
        }

        [Test]
        [Category( "Fast" )]
        public void TestGetSubmittedMessagesList_WhenFacilityDoesNotAcceptMessages_MessageShouldNotBeSaved()
        {
            var newMessage = new MessageBuilder()
                                .SetFacilityCode( "ICE" )
                                .BuildMessage();

            var stubLogger = Substitute.For<ILog>();

            var stubFacilityBroker = Substitute.For<IFacilityBroker>();

            var facilityThatDoesNotAcceptRegistrations = new Facility();

            facilityThatDoesNotAcceptRegistrations["IsAcceptingPreRegistrationSubmissions"] = null;
            stubFacilityBroker.FacilityWith( string.Empty ).ReturnsForAnyArgs( facilityThatDoesNotAcceptRegistrations );

            var submissionRepository = Substitute.For<IPreRegistrationSubmissionRepository>();

            var messageProcessor = new MessageProcessor( stubLogger, stubFacilityBroker, submissionRepository );

            messageProcessor.TryProcessing( newMessage, MessageProcessor.MessageType );

            Assert.IsFalse( facilityThatDoesNotAcceptRegistrations.IsAcceptingPreRegistrationSubmissions() );

            submissionRepository.DidNotReceiveWithAnyArgs().Save( null );


        }

        private static MessageProcessor GetMessageProcessorWithRealFacilityBroker()
        {
            var stubLogger = Substitute.For<ILog>();
            var stubFacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            var submissionRepository = Substitute.For<IPreRegistrationSubmissionRepository>();
            return new MessageProcessor( stubLogger, stubFacilityBroker, submissionRepository );
        }


        private static MessageProcessor GetMessageProcessorWithAllStubDependencies()
        {
            var stubLogger = Substitute.For<ILog>();
            var stubFacilityBroker = Substitute.For<IFacilityBroker>();

            var facilityAcceptingRegistrations = new Facility();

            facilityAcceptingRegistrations["IsAcceptingPreRegistrationSubmissions"] = new object();

            stubFacilityBroker.FacilityWith( string.Empty ).ReturnsForAnyArgs( facilityAcceptingRegistrations );
            var submissionRepository = Substitute.For<IPreRegistrationSubmissionRepository>();
            return new MessageProcessor( stubLogger, stubFacilityBroker, submissionRepository );
        }

        private static XDocument RemoveFacilityElementChildren( XDocument message )
        {
            var facilityElementName = message.Root.Name.Namespace + "facility";
            var facilityElement = message.Descendants( facilityElementName ).First();

            var facilityDescendents = facilityElement.Descendants();
            facilityDescendents.Remove();

            return message;
        }
    }
}
