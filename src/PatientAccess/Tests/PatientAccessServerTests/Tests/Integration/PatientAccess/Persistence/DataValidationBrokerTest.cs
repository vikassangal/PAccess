using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Xml;
using Extensions.PersistenceCommon;
using PatientAccess.BenefitsValidation5010Proxy;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using PatientAccess.Services;
using Rhino.Mocks;
using NUnit.Framework;
using Patient = PatientAccess.Domain.Parties.Patient;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class DataValidationBrokerTest : AbstractBrokerTests
    {
        #region Constants

        private const string CONNECTION_STRING_NAME = "ConnectionString";

        #endregion

        #region SetUp and TearDown DataValidationBrokerTest

        [SetUp]
        public void SetUpDataValidationBrokerTest()
        {
            broker = BrokerFactory.BrokerOfType<IDataValidationBroker>();
            dbConnection = new SqlConnection();
            dbConnection.ConnectionString =
                ConfigurationManager.ConnectionStrings[CONNECTION_STRING_NAME]
                    .ConnectionString;
            dbConnection.Open();

            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDownDataValidationBrokerTest()
        {
            if ( dbConnection != null )
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        #endregion

        #region Test Methods

        private delegate initiateResponse InitiateBenefitsValidationDelegate(
            initiateRequest request );

        /// <summary>
        /// Ensure that all the fields in an AccountDetailRequest are mapped
        /// to an eDV request object correctly
        /// </summary>
        /// <remarks>OTD36706</remarks>
        [Test]
        [Ignore] //Ignoring because reference.cs file provided by EDV interface has changed, this test is not valid anymore
        public void TestInitiateBenefitsValidationRequestMapping()
        {
            new initiateResponse();
            var mockedBenefitsService = MockRepository.Stub<IBenefitsValidation5010ServiceSoapClient>();

            var mockableDataValidationBroker = new MockableDataValidationBroker();

            // Prepare our mock broker, substituting our mock web service and
            //  turning off the persistence
            mockableDataValidationBroker.BenefitsValidationServiceMock = mockedBenefitsService;
            mockableDataValidationBroker.ShouldAllowDataValidationTicketSave = false;
            mockableDataValidationBroker.ShouldAllowNewTicketSave = false;

            // Need to create a sample request to send to the broker
            var accountDetailsRequest =
                CreateTestObjectFor<AccountDetailsRequest>(
                    "DataValidationBrokerTest.xml",
                    "TestInitiateBenefitsValidationRequestMappingRequest" );

            // Need our mocked webservice to save the request object after it is mapped from the
            //  AccountDetailsRequest. This code uses Rhino.Mocks to wire the call to a method
            //  on the mockableDataValidationBroker that will save this for us.
            SetupResult.For( new BenefitsValidation5010ServiceSoapClient().initiate(null))
                .IgnoreArguments()
                .Do( new InitiateBenefitsValidationDelegate( mockableDataValidationBroker.CaptureBenefitsValidationRequest ) );

            // This is Rhino.Mocks odd syntax, but it basically tells the mock object that you are
            //  done with setup and ready to use it.
            MockRepository.ReplayAll();

            mockableDataValidationBroker.InitiateBenefitsValidation( accountDetailsRequest );

            var theSubscriber = mockableDataValidationBroker.InitiateValidateBenefitsRequest.BenefitsValidation5010Request_1.subscriber;

            // This will have to get more comprehensive and have more stable test data.
            //  At a minimum, we need to take a look at the subscriber since we have 
            //  a defect for it.
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.state ), "Subscriber state is null or empty" );
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.sex ), "Subscriber sex is null or empty" );
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.planId ), "Subscriber planId is null or empty" );
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.firstName ), "Subscriber firstName is null or empty" );
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.lastName ), "Subscriber lastName is null or empty" );
            Assert.IsTrue( !string.IsNullOrEmpty( theSubscriber.memberId ), "Subscriber memberId is null or empty" );
        }

        [Test]
        public void TestGetDataValidationTicketById()
        {
            SqlConnection sqlConnection = null;
            SqlTransaction sqlTransaction = null;
            SqlCommand sqlCommand = null;

            try
            {
                sqlConnection =
                    new SqlConnection( ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString );
                sqlConnection.Open();

                sqlTransaction = sqlConnection.BeginTransaction( IsolationLevel.ReadCommitted );

                sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.Transaction = sqlTransaction;
                sqlCommand.CommandText =
                    @"INSERT INTO DataValidation.Tickets VALUES('e299ec25-cdf2-11dc-90ef-e92ea04838fe',0,999,'Y','1/28/2008 5:26:31 PM',345622,150,'N','Y')";

                sqlCommand.ExecuteNonQuery();

                IDataValidationBroker unitUnderTest = new DataValidationBroker( sqlTransaction );
                DataValidationTicket newticket =
                    unitUnderTest.GetDataValidationTicketFor( "e299ec25-cdf2-11dc-90ef-e92ea04838fe" );

                Assert.AreEqual( 345622, newticket.AccountNumber );
                Assert.AreEqual( 999, newticket.Facility.Oid );
                Assert.AreEqual( true, newticket.FUSNoteSent );
                Assert.AreEqual( true, newticket.ResultsAvailable );
                Assert.AreEqual( false, newticket.ResultsReviewed );
                Assert.AreEqual( 150, newticket.MedicalRecordNumber );
                Assert.AreEqual( DateTime.Parse( "1/28/2008 5:26:31 PM" ), newticket.InitiatedOn );
            }
            finally
            {
                if ( sqlTransaction != null )
                {
                    sqlTransaction.Rollback();
                    sqlTransaction.Dispose();
                }

                if ( sqlConnection != null )
                {
                    sqlConnection.Close();
                    sqlConnection.Dispose();
                }

                if ( sqlCommand != null )
                {
                    sqlCommand.Dispose();
                }
            }
        }

        [Test]
        public void TestUpdateTicketAvailability()
        {
            try
            {
                broker.SaveResponseIndicator( "1891d516-0e04-11db-b8b0-ff2de821f561", false );

                Assert.IsTrue( true, "Saved worked" );
            }
            catch ( Exception ex )
            {
                Assert.Fail( ex.Message + ex );
            }
        }

        [Test]
        public void TestCreditValidationFailure()
        {
            var guarantor = new Guarantor();
            guarantor.Name = new Name( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION,
                                      "Test", "Tester", "T" );
            var state = new State( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Texas", "TX" );
            var country = new Country( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "USA", "USA" );
            var address = new Address( "Street1", "Street2", "Plano", new ZipCode( "12345" ), state, country );
            var phone = new PhoneNumber( "123", "5551234" );
            var email = new EmailAddress( "testerttest@mercury.com" );
            var cp = new ContactPoint( address, phone, phone, email,
                                      TypeOfContactPoint.NewBillingContactPointType() );
            guarantor.AddContactPoint( cp );

            try
            {
                broker.InitiateGuarantorValidation( guarantor, USER_NAME, "DEL", 12345, 13245 );
            }
            catch ( Exception ex )
            {
                Assert.IsTrue( true, ex.Message );
            }
        }

        [Test]
        [Ignore]
        public void TestInitiateDataValidation()
        {
            try
            {
                var account = new Account();
                account.AccountNumber = 345622;
                account.Facility = new Facility( 1L, DateTime.Now, "ACO", "ACO" );

                var p = new Payor();
                p.Code = "AA";

                var plan = new CommercialInsurancePlan();
                plan.Oid = 1;
                plan.PlanName = "PRIVATE PAY";
                plan.Payor = p;
                plan.PlanSuffix = "123";
                var insured = new Insured();
                insured.FirstName = "Sam";
                insured.LastName = "Atkins";
                insured.DateOfBirth = DateTime.Now;

                var fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
                acoFacility = fb.FacilityWith( 900 );

                var patient = new Patient(
                    1,
                    DateTime.Now,
                    new Name( "SomeFirstName", "SomeLastName", "M" ),
                    299,
                    DateTime.Now,
                    new SocialSecurityNumber( "111223333" ),
                    new Gender( 1, DateTime.Now, "Male", "M" ),
                    acoFacility );

                account.Patient = patient;

                Coverage coverage = Coverage.CoverageFor( plan, insured );
                coverage.Account = account;
                var commCoverage = (CommercialCoverage)coverage;
                if ( typeof( CommercialCoverage ) == coverage.GetType() )
                {
                    commCoverage.PreCertNumber = 123;
                    commCoverage.TrackingNumber = "1331332";
                    commCoverage.CertSSNID = "3456";
                }

                CreateUser();
                User patientAccessUser = User.GetCurrent();
                var securityUser = new Extensions.SecurityService.Domain.User();
                securityUser.UPN = USER_NAME;
                patientAccessUser.SecurityUser = securityUser;

                var request = new AccountDetailsRequest();
                request.FacilityOid = 900;

                request.AccountNumber = account.AccountNumber;
                request.AdmitDate = account.AdmitDate;
                request.MedicalRecordNumber = account.Patient.MedicalRecordNumber;
                request.PatientDOB = account.Patient.DateOfBirth;
                request.PatientFirstName = account.Patient.FirstName;
                request.PatientLastName = account.Patient.LastName;
                request.PatientMidInitial = account.Patient.MiddleInitial;
                request.PatientSex = account.Patient.Sex.Code;
                request.PatientSSN = account.Patient.SocialSecurityNumber;
                request.Upn = securityUser.UPN;

                request.InsuredDOB = insured.DateOfBirth;
                request.InsuredFirstName = insured.FirstName;
                request.InsuredLastName = insured.LastName;

                request.CoverageMemberId = commCoverage.CertSSNID;
                request.CoverageInsurancePlanId = coverage.InsurancePlan.PlanID;
                request.CoverageOrderOid = coverage.CoverageOrder.Oid;

                DataValidationTicket ticket = broker.InitiateBenefitsValidation( request );

                Assert.IsTrue( ticket.TicketId != string.Empty, "Ticket did not get generated" );
            }
            catch ( Exception ex )
            {
                Assert.Fail( ex.Message + ex );
            }
        }

        [Test]
        [Ignore]
        public void TestGetBenefitsValidationResponse_WhenBenefitsResponseParseStrategyIsDifferentOnLastTicket_ShouldReturnResponseWithBlankValues ()
        {
            var mocks = new MockRepository();
            var unitUnderTest = (DataValidationBroker)mocks.PartialMock( typeof( DataValidationBroker ) );
            var dataValidationTicketType = new DataValidationTicket();
            dataValidationTicketType.ResultsAvailable = true;
            dataValidationTicketType.Facility = new Facility( 01, DateTime.Now, "SomeFacility", "SomeHospitalCode" );
            dataValidationTicketType.BenefitsResponse.BenefitsResponseParseStrategy =
                typeof( CommercialCoverage ).ToString();
            unitUnderTest.Expect( x => x.GetDataValidationTicketFor( Arg<String>.Is.Anything ) )
                .Return( dataValidationTicketType );
            
            var resultResponse = new obtainResultResponse { result = new benefitsValidationResult() };

            var stubBenefitsValidationService = mocks.Stub<IBenefitsValidation5010ServiceSoapClient>();
            stubBenefitsValidationService.Expect(x => x.obtainResult(Arg<obtainResultRequest>.Is.Anything)).
                Return( resultResponse );
            mocks.ReplayAll();
            
            unitUnderTest.BenefitsValidation5010Service = stubBenefitsValidationService;


            //InvalidCastException in get_Constraints of GovernmentMedicaidCoverage
            // Fix - If the account's current coverage type does not match the coverage type on the latest ticket 
            // in the database do not get the old response. The BenefitsResponseParseStrategy and other parameters 
            // on the response should be made String.Empty so that it will not parse any available constraints 
            // and force the user to re-initiate benefits validation for the new coverage.


            const string ticketId = "SomeTickedId";
            var benefitsValidationResponse = unitUnderTest.GetBenefitsValidationResponse( ticketId, USER_NAME,
                                                                                         typeof(
                                                                                             GovernmentMedicaidCoverage ) );

            Assert.AreEqual( string.Empty, benefitsValidationResponse.MessageUUID, "Response MessageUUID is not empty" );
            Assert.AreEqual( string.Empty, benefitsValidationResponse.PayorXmlMessage,
                            "Response PayorXmlMessage is not empty" );
            Assert.AreEqual( string.Empty, benefitsValidationResponse.Eligible, "Response Eligible is not empty" );
            Assert.AreEqual( string.Empty,
                            benefitsValidationResponse.ReturnedDataValidationTicket.BenefitsResponse.
                                BenefitsResponseParseStrategy,
                            "Benefits Response Parse Strategy is not empty" );
            Assert.IsFalse( benefitsValidationResponse.IsSuccess, "Response IsSuccess is not false" );
            Assert.AreEqual( BenefitsValidationResponse.RESPONSE_TEXT_ON_MISMATCH,
                            benefitsValidationResponse.PayorMessage, "Response text do not match" );

            //mocks.VerifyAll();
        }

        [Test]
        [Ignore]
        public void TestIntegratedGetBenefitsValidationResponse()
        {
            var accountDetailsRequest =
                CreateTestObjectFor<AccountDetailsRequest>(
                    "DataValidationBrokerTest.xml",
                    "TestInitiateBenefitsValidationRequestMappingRequest");
            var dataValidationTicket = new DataValidationBroker().InitiateBenefitsValidation(accountDetailsRequest);
            Assert.IsNotNull(dataValidationTicket.TicketId, "Null ticket is returned.");
            Assert.AreEqual(accountDetailsRequest.AccountNumber, dataValidationTicket.AccountNumber, "Invalid Account number");
            Assert.AreEqual(accountDetailsRequest.MedicalRecordNumber, dataValidationTicket.MedicalRecordNumber, "Invalid Medical Record number");
            Assert.AreEqual(accountDetailsRequest.FacilityOid, dataValidationTicket.Facility.Oid, "Invalid Account number");
        }

        [Test]
        [Ignore]
        public void TestIntegratedEDVGetBenefitsValidationResponse()
        {
            benefitsValidation5010Request benefitsValidationRequest =
                CreateTestObjectFor<benefitsValidation5010Request>(
                    "BenefitsValidationRequestTest.xml",
                    "TestIntegratedEDVGetBenefitsValidationResponse");
            Assert.AreEqual("20060611", benefitsValidationRequest.beginServiceDate, "Begin of service Date does not match");
            Assert.AreEqual("20060611", benefitsValidationRequest.endServiceDate, "End ofservice Date does not match");
            Assert.AreEqual("T38", benefitsValidationRequest.hspcd, "Facility ID does not match");
            Assert.AreEqual("KENNETH MCKENZIE", benefitsValidationRequest.userId, "User ID does not match");
            Assert.AreEqual("19300704", benefitsValidationRequest.patient.dateOfBirth, "Date of birht does not match");
            Assert.AreEqual("JASMINE", benefitsValidationRequest.patient.firstName, "Patient first name does not match");
            Assert.AreEqual("TEA", benefitsValidationRequest.patient.lastName, "Patient Last Name does not match");
            Assert.AreEqual("G", benefitsValidationRequest.patient.middleName, "Patient Middle Name does not match");
            Assert.AreEqual("34", benefitsValidationRequest.patient.relationshipToSubscriber, "Patient releationship to subscriber does not match");
            Assert.AreEqual("F", benefitsValidationRequest.patient.sex, "Patient Sex does not match");
            Assert.AreEqual("999999999", benefitsValidationRequest.patient.ssn, "Patient SSN does not match");
            Assert.AreEqual("", benefitsValidationRequest.subscriber.cardIssueDate, "Subscriber Card Issue Date does not match");
            Assert.AreEqual("19700704", benefitsValidationRequest.subscriber.dateOfBirth, "Subscriber date of birth does not match");
            Assert.AreEqual("JASMINE", benefitsValidationRequest.subscriber.firstName, "Subscriber first name does not match");
            Assert.AreEqual("", benefitsValidationRequest.subscriber.groupId, "Subscriber groupid does not match");
            Assert.AreEqual("TEA", benefitsValidationRequest.subscriber.lastName, "Subscriber Last Name does not match");
            Assert.AreEqual("123456", benefitsValidationRequest.subscriber.memberId, "Subscriber Member Id does not match");
            Assert.AreEqual("", benefitsValidationRequest.subscriber.middleName, "Subscriber Middle Name does not match");
            Assert.AreEqual("070Q2", benefitsValidationRequest.subscriber.planId, "Subscriber PlanId does not match");
            Assert.AreEqual("F", benefitsValidationRequest.subscriber.sex, "Subscriber Sex does not match");
            Assert.AreEqual("TX", benefitsValidationRequest.subscriber.state, "Subscriber State does not match");
            initiateRequest initiateRequest = new initiateRequest();
            initiateRequest.BenefitsValidation5010Request_1 = benefitsValidationRequest;
            var response = new BenefitsValidation5010ServiceSoapClient().initiate(initiateRequest.BenefitsValidation5010Request_1);
            Assert.IsNotNull(response);
        }

        [Test]
        [Ignore]
        public void TestIntegratedEDVObtainBenefitsResult()
        {
            obtainResultRequest benefitResult = new obtainResultRequest();
            
            benefitResult.ServiceKey_1 = new serviceKey();
            benefitResult.ServiceKey_1.customerId = 1;
            benefitResult.ServiceKey_1.hspcd = "DEF";
            benefitResult.ServiceKey_1.userId = "KENNETH MCKENZIE";
            benefitResult.ServiceKey_1.uuid = "3bb9314b-e5f7-11e0-a586-9ba5d0628b07";
            var result = new BenefitsValidation5010ServiceSoapClient().obtainResult(benefitResult.ServiceKey_1);

            Assert.IsNotNull(result);
        }

        [Test]
        public void TestInitiateDataValidationException()
        {
            try
            {
                var request = new AccountDetailsRequest();

                request.IsAccountNull = true;
                request.IsCoverageNull = true;
                request.IsUserNull = true;
                broker.InitiateBenefitsValidation( request );
            }
            catch ( Exception ex )
            {
                Assert.IsTrue( ex.Message == "Invalid Benefits Validation Request;  Coverage, Account, or User not specified.", ex.ToString() );
            }
        }

        [Test]
        //TODO-AC fix the catch clause related issue.
        public void TestValidAddressesMatchingAnAddress()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            User u = User.GetCurrent();
            u.UserID = "patientaccess.user01";
            u.PBAREmployeeID = "NUNITTST";
            u.Facility = facilityBroker.FacilityWith( "ACO" );

            User.SetCurrentUserTo( u );

            var testAddress = new Address( "2300 PLANO PKWY", String.Empty, "PLANO", new ZipCode( "75075" ),
                                          new State( 12L, "Texas", "TX" ), new Country( 12L, "United States", "US" ) );

            try
            {
                AddressValidationResult res = broker.ValidAddressesMatching( testAddress, u.SecurityUser.UPN,
                                                                              u.Facility.Code );
                Assert.IsTrue( res.Addresses.Count == 1 );
            }
            catch ( Exception )
            {
            }
        }
        [Test]
        public void TestValidAddressesMatchingAnAddressWithStreet2()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            User u = User.GetCurrent();
            u.UserID = "patientaccess.user01";
            u.PBAREmployeeID = "NUNITTST";
            u.Facility = facilityBroker.FacilityWith("ACO");

            User.SetCurrentUserTo(u);

            var testAddress = new Address("2300 PLANO PKWY", "STE 310", "PLANO", new ZipCode("75075"),
                new State(12L, "Texas", "TX"), new Country(12L, "United States", "US"));

            try
            {
                AddressValidationResult res = broker.ValidAddressesMatching(testAddress, u.SecurityUser.UPN,
                    u.Facility.Code);
                Assert.IsTrue(res.Addresses.Count == 1);
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void TestAddressesWith_Street1LessThanEqualTo45CharAndStreet2LessThenEqualTo30char()
        {
          var dataValidationBroker = new DataValidationBroker();

          var testAddress = new Address("1700 W INTERNATIONAL SPEEDWAY BLVD", "STE 500 STE 500 STE 500", "DAYTONA BEACH", new ZipCode("32114"),
                new State(12L, "Florida", "FL"), new Country(12L, "United States", "US"));

            try
            {
                Address PASAddress = dataValidationBroker.ExtractAddress(testAddress);

                Assert.IsTrue(PASAddress.Address1.Length <= 45 && PASAddress.Address2.Length <= 30);
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void TestAddressesWith_Street1GreaterThan45AndLessthenEqualTo75CharAndStreet2GreaterThen30char()
        {
            var dataValidationBroker = new DataValidationBroker();

            var testAddress = new Address("1700 W INTERNATIONAL SPEEDWAY BLVD STE 500 AA STE 500",
                "STE 500 STE 500 STE 500 STE 500 STE 500 STE 500", "DAYTONA BEACH", new ZipCode("32114"),
                new State(12L, "Florida", "FL"), new Country(12L, "United States", "US"));

            try
            {
                Address PASAddress = dataValidationBroker.ExtractAddress(testAddress);

                Assert.IsTrue(PASAddress.Address1.Length <= 45 && PASAddress.Address2.Length <= 30);
            }
            catch (Exception)
            {
            }
        }
        [Test]
        public void TestAddressesWith_Street1Greaterthen75CharAndStreet2LessThenEqualTo30char()
        {
            var dataValidationBroker = new DataValidationBroker();

            var testAddress = new Address("1700 W INTERNATIONAL SPEEDWAY BLVD STE 500 STE 500 1700 W INTERNATIONAL SPEEDWAY BLVD STE 500 AA STE 500",
                "STE 500 STE 500 STE 500", "DAYTONA BEACH", new ZipCode("32114"),
                new State(12L, "Florida", "FL"), new Country(12L, "United States", "US"));

            try
            {
                Address PASAddress = dataValidationBroker.ExtractAddress(testAddress);

                Assert.IsTrue(PASAddress.Address1.Length <= 45 && PASAddress.Address2.Length <= 30);
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void TestGetFormatedHawkAlertToSeeIfPathFortheXsltFileIsCorrect()
        {
            var input = new XmlDocument();
            DataValidationBroker.GetFormatedHawkAlert(input);
        }

        [Test]
        public void TestGetFormatedCreditReportSeeIfPathFortheXsltFileIsCorrect()
        {
            var input = new XmlDocument();
            DataValidationBroker.GetFormatedCreditReport( input );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Properties

        private MockRepository MockRepository
        {
            get { return mockRepository; }
            set { mockRepository = value; }
        }

        private static string ConnectionStringName
        {
            get { return CONNECTION_STRING_NAME; }
        }

        #endregion

        #region Data Elements

        private static IDataValidationBroker broker = null;
        private static SqlConnection dbConnection;
        private static Facility acoFacility;
        private static MockRepository mockRepository;

        #endregion

        #region Constants

        #endregion
    }

    public class MockableDataValidationBroker : DataValidationBroker
    {
        #region Constants & Enumerations

        #endregion Constants & Enumerations

        #region Events and Delegates

        #endregion Events and Delegates

        #region Fields

        private initiateRequest i_InitiateValidateBenefitsRequest = new initiateRequest();

        private bool i_ShouldAllowNewTicketSave = true;

        private bool i_ShouldAllowDataValidationTicketSave = true;

        #endregion Fields

        #region Properties

        public initiateRequest InitiateValidateBenefitsRequest
        {
            get { return i_InitiateValidateBenefitsRequest; }
            private set { i_InitiateValidateBenefitsRequest = value; }
        }

        public IBenefitsValidation5010ServiceSoapClient BenefitsValidationServiceMock
        {
            get { return base.BenefitsValidation5010Service; }
            set
            {
                Debug.Assert( null != value );
                base.BenefitsValidation5010Service = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [should allow ticket save].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [should allow ticket save]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldAllowNewTicketSave
        {
            private get { return i_ShouldAllowNewTicketSave; }
            set { i_ShouldAllowNewTicketSave = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [should allow response save].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [should allow response save]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldAllowDataValidationTicketSave
        {
            private get { return i_ShouldAllowDataValidationTicketSave; }
            set { i_ShouldAllowDataValidationTicketSave = value; }
        }

        #endregion Properties

        #region Construction & Finalization

        #endregion Construction & Finalization

        #region Public Methods

        public initiateResponse CaptureBenefitsValidationRequest(initiateRequest theRequest)
        { 
            Debug.Assert( null != theRequest );
            InitiateValidateBenefitsRequest = theRequest;
            return new initiateResponse();
        }

        public override void SaveDataValidationTicket( DataValidationTicket aTicket )
        {
            if ( ShouldAllowDataValidationTicketSave )
            {
                base.SaveDataValidationTicket( aTicket );
            }
        }

        #endregion Public Methods

        #region Non-Public Methods

        /// <summary>
        /// Saves the new ticket.
        /// </summary>
        /// <param name="aTicket">A ticket.</param>
        protected override void SaveNewTicket( DataValidationTicket aTicket )
        {
            if ( ShouldAllowNewTicketSave )
            {
                base.SaveNewTicket( aTicket );
            }
        }

        #endregion Non-Public Methods

        #region Event Handlers

        #endregion Event Handlers
    } 
}