using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PatientAccess.Messaging;
using PatientAccess.Persistence.OnlinePreregistration;
using Tests.Unit.PatientAccess.Services.PreRegistration;

namespace Tests.Utilities.OnlinePreRegistration
{
    public class MessageBuilder
    {

        private readonly preRegistration preRegistrationData;

        static MessageBuilder()
        {
            var assembly = Assembly.GetAssembly( typeof( MessageProcessor ) );

            var preRegSchemaStream = assembly.GetManifestResourceStream( "PatientAccess.Resources.PreRegistrationData.xsd" );
            var guidSchemaStream = assembly.GetManifestResourceStream( "PatientAccess.Resources.GuidType.xsd" );


            var preRegistrationSchema = XmlSchema.Read( preRegSchemaStream, SchemaValidationEventHandler );
            var guidSchema = XmlSchema.Read( guidSchemaStream, SchemaValidationEventHandler );


            ReaderSettings = new XmlReaderSettings();
            ReaderSettings.ValidationType = ValidationType.Schema;
            ReaderSettings.ConformanceLevel = ConformanceLevel.Document;
            ReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            ReaderSettings.ValidationEventHandler += MessageValidationEventHandler;

            ReaderSettings.Schemas.Add( preRegistrationSchema );
            ReaderSettings.Schemas.Add( guidSchema );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBuilder"/> class. 
        /// This class uses the sample xml document in the server tests. This document contains all the required and optional elements for the preregistration data schema
        /// </summary>
        public MessageBuilder()
        {
            string messageXml = DataCreator.GetXmlFromSampleDocument();

            using ( XmlReader xmlReader = XmlReader.Create( new StringReader( messageXml ) ) )
            {
                var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );

                preRegistrationData = (preRegistration)xmlSerializer.Deserialize( xmlReader );
            }
        }

        public MessageBuilder( string xml )
        {
            Validate(xml);

            using ( XmlReader xmlReader = XmlReader.Create( new StringReader( xml ) ) )
            {
                var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );

                preRegistrationData = (preRegistration)xmlSerializer.Deserialize( xmlReader );
            }
        }

        public MessageBuilder SetFacilityCode( string facilityCode )
        {
            preRegistrationData.facility.code =  facilityCode ;
            return this;
        }

        public MessageBuilder SetPatientName( string firstName, string lastName, string middleInitial )
        {
            SetPatientFirstName( firstName );
            SetPatientLastName( lastName );
            SetPatientMiddleInitial( middleInitial );

            return this;
        }

        public MessageBuilder SetPatientName( string firstName, string lastName )
        {
            SetPatientFirstName( firstName );
            SetPatientLastName( lastName );
            SetPatientMiddleInitial( null );

            return this;
        }

        private MessageBuilder SetPatientFirstName( string firstName )
        {
            preRegistrationData.patient.name.first = firstName;

            return this;
        }

        private MessageBuilder SetPatientLastName( string lastName )
        {
            preRegistrationData.patient.name.last = lastName;

            return this;
        }


        private MessageBuilder SetPatientMiddleInitial( string middleInitial )
        {
            preRegistrationData.patient.name.middleInitial = middleInitial;

            return this;
        }

        public MessageBuilder SetPatientDateOfBirth( DateTime dateOfBirth )
        {
            preRegistrationData.patient.dateOfBirth = dateOfBirth;

            return this;
        }



        public MessageBuilder SetPatientSsn( string ssn )
        {
            preRegistrationData.patient.socialSecurityNumber = ssn;

            return this;
        }


        public MessageBuilder SetMale()
        {
            preRegistrationData.patient.gender = genderType.CODE_M_VALUE_MALE;

            return this;
        }


        public MessageBuilder SetFemale()
        {
            preRegistrationData.patient.gender = genderType.CODE_F_VALUE_FEMALE;

            return this;
        }


        public MessageBuilder SetPatientAdmitDate( DateTime admitDate )
        {
            preRegistrationData.visit.expectedAdmissionDateTime = admitDate;

            return this;
        }


        public MessageBuilder SetVisitedBefore( bool visitedBefore )
        {
            preRegistrationData.visit.returningPatient = visitedBefore;

            return this;
        }

        public MessageBuilder SetNoPrimaryAndSecondaryInsurance()
        {
            preRegistrationData.visit.insuranceInformation = new visitTypeInsuranceInformation
                {
                    primaryInsurance = null,
                    secondaryInsurance = null
                };

            return this;
        }

        public MessageBuilder SetPrimaryInsurancePolicyNumber( string policyNumber )
        {
            preRegistrationData.visit.insuranceInformation.primaryInsurance.policyNumber = policyNumber;
            
            return this;
        }

        public MessageBuilder SetSecondaryInsurancePolicyNumber( string policyNumber )
        {
            preRegistrationData.visit.insuranceInformation.secondaryInsurance.policyNumber = policyNumber;

            return this;
        }

        public MessageBuilder SetInsuranceNotSelected()
        {
            preRegistrationData.visit.insuranceInformation = null;

            return this;
        }

        private static XmlReaderSettings ReaderSettings { get; set; }

        /// <exception cref="XmlSchemaException"><c>XmlSchemaException</c>.</exception>
        public string BuildMessage()
        {
            var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );
            var stringWriter = new StringWriter();

            xmlSerializer.Serialize( stringWriter, preRegistrationData );
            string message = stringWriter.ToString();

            Validate( message );

            return message;
        }

        private static void Validate( string message )
        {
            using ( var xmlReader = XmlReader.Create( new StringReader( message ), ReaderSettings ) )
            {
                while ( xmlReader.Read() )
                {
                }
            }
        }

        /// <exception cref="XmlSchemaException"><c>XmlSchemaException</c>.</exception>
        private static void MessageValidationEventHandler( object sender, ValidationEventArgs e )
        {
            throw e.Exception;
        }


        /// <exception cref="XmlSchemaException"><c>XmlSchemaException</c>.</exception>
        private static void SchemaValidationEventHandler( object sender, ValidationEventArgs e )
        {
            throw e.Exception;
        }

        public MessageBuilder SetPatientAddressUS( string street, string city, string state, string zipcode )
        {
            var usAddressType = new usAddressType();
            usAddressType.addressLine = street;
            usAddressType.city = city;
            usAddressType.stateOrTerritory = usStateOrTerritoryType.CODE_TX_VALUE_TEXAS;
            usAddressType.zipCode = zipcode;
            preRegistrationData.patient.address.Item = usAddressType;

            return this;
        }

        public static preRegistration GetPreRegistrationData( string patientAccessResourcesSamplePreRegistrationXml )
        {
            var assembly = Assembly.GetAssembly( typeof( PatientFactoryTests ) );
            var streamReader = new StreamReader( assembly.GetManifestResourceStream( patientAccessResourcesSamplePreRegistrationXml ) );

            var xmlReader = XmlReader.Create( streamReader );
            var xmlSerializer = new XmlSerializer( typeof( preRegistration ) );
            var registrationData = (preRegistration)xmlSerializer.Deserialize( xmlReader );


            return registrationData;
        }

        public MessageBuilder SetOtherLanguage(string otherLanguage)
        {
            preRegistrationData.patient.preferredLanguage.Item = otherLanguage;
            return this;
        }
    }
}