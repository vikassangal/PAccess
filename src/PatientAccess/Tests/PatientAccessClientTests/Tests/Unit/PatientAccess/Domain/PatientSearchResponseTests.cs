using System;
using System.Collections.Generic;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientSearchResponseTests.
    /// </summary>

    [TestFixture]
    [Category( "Fast" )]
    public class PatientSearchResponseTests
    {
        #region Constants
        
        private const string
            TEST_F_NAME     = "John",
            TEST_L_NAME     = "Doe",
            TEST_MI         = "T",
            TEST_SUFFIX     = "SR",

            TEST_AKA1_FNAME = "JohnAKA1",
            TEST_AKA1_LNAME = "Doe",

            TEST_AKA2_FNAME = "JohnAKA2",
            TEST_AKA2_LNAME = "Doe";

        private const string    
            ADDRESS1        = "335 Nicholson Dr.",
            ADDRESS2        = "32321",
            CITY            = "Austin",    
            POSTAL_CODE     = "60505",
            COUNTRY_USA     = "USA" ;

        private const string    
            EXCEPTION_MSG   = "An existing connection was forcibly closed by the remote host";

        #endregion

        #region SetUp and TearDown PatientSearchResponseTests
        [TestFixtureSetUp]
        public static void SetUpPatientSearchResponseTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownPatientSearchResponseTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestDefaultConstructor()
        {
            // Test Default constructor

            PatientSearchResponse defaultSearchResponse = new PatientSearchResponse();

            Assert.IsTrue( 
                defaultSearchResponse.PatientSearchResults.Count == 0, 
                "PatientSearchResults count for default constructor not equal to 0." );

            Assert.IsTrue( 
                defaultSearchResponse.PatientSearchResultStatus == PatientSearchResultStatus.Unknown,
                "PatientSearchResultStatus for default constructor not UNKNOWN." );

            Assert.IsTrue(
                defaultSearchResponse.Message == String.Empty,
                "Patient Search Response Message for default constructor not empty." );
        }

        [Test]
        public void TestSearchResponseWithResults()
        {
            List<Name> akaNames = new List<Name> {aliasName1, aliasName2};

            PatientSearchResult searchResult = new PatientSearchResult( realName, akaNames, gender, patient_dob, ssn, mrn, address, hspCode );

            List<PatientSearchResult> searchResults = new List<PatientSearchResult> {searchResult};

            PatientSearchResultStatus resultStatus = PatientSearchResultStatus.Success;

            PatientSearchResponse searchResponse = new PatientSearchResponse( searchResults, resultStatus );

            Assert.AreEqual(
                resultStatus,
                searchResponse.PatientSearchResultStatus,
                "Patient Search Result Status do not match." );

            Assert.AreEqual( 
                String.Empty,
                searchResponse.Message,
                "Response Messages do not match." );

            Assert.AreEqual( 
                searchResults.Count,
                searchResponse.PatientSearchResults.Count,
                "Patient Search Results count do not match." );

            Assert.AreEqual( 
                akaNames[0].AsFormattedName(),
                searchResponse.PatientSearchResults[0].AkaNames[0].AsFormattedName(),
                "First Alias Names do not match." );

            Assert.AreEqual( 
                akaNames[1].AsFormattedName(),
                searchResponse.PatientSearchResults[0].AkaNames[1].AsFormattedName(),
                "Second Alias Names do not match." );

            Assert.AreEqual( 
                gender,
                searchResponse.PatientSearchResults[0].Gender,
                "Genders do not match." );

            Assert.AreEqual(
                patient_dob.ToShortDateString(),
                searchResponse.PatientSearchResults[0].DateOfBirth.ToShortDateString(),
                "Date of Births do not match." );

            Assert.AreEqual( 
                ssn,
                searchResponse.PatientSearchResults[0].SocialSecurityNumber,
                "Social Security Numbers do not match." );

            Assert.AreEqual( 
                mrn,
                searchResponse.PatientSearchResults[0].MedicalRecordNumber,
                "Medical Record Numbers do not match." );

            Assert.AreEqual( 
                address.AsMailingLabel(),
                searchResponse.PatientSearchResults[0].Address.AsMailingLabel(),
                "Addresses do not match." );

            Assert.AreEqual( 
                hspCode,
                searchResponse.PatientSearchResults[0].HspCode,
                "HspCodes do not match." );
        }

        [Test]
        public void TestSearchResponseWithNoResults()
        {
            List<PatientSearchResult> searchResults = new List<PatientSearchResult>();
            PatientSearchResultStatus resultStatus = PatientSearchResultStatus.Success;

            PatientSearchResponse searchResponse = new PatientSearchResponse( searchResults, resultStatus );

            Assert.IsTrue( 
                searchResponse.PatientSearchResults.Count == 0, 
                "PatientSearchResults count for default constructor not equal to 0." );

            Assert.IsTrue( 
                searchResponse.PatientSearchResultStatus == PatientSearchResultStatus.Success,
                "PatientSearchResultStatus for default constructor not UNKNOWN." );

            Assert.IsTrue(
                searchResponse.Message == String.Empty,
                "Patient Search Response Message for default constructor not empty." );
        }

        [Test]
        public void TestSearchResponseWithException()
        {
            List<PatientSearchResult> searchResults = new List<PatientSearchResult>();
            PatientSearchResultStatus resultStatus = PatientSearchResultStatus.Exception;

            PatientSearchResponse searchResponse = new PatientSearchResponse( searchResults, resultStatus, EXCEPTION_MSG );

            Assert.IsTrue( 
                searchResponse.PatientSearchResults.Count == 0, 
                "PatientSearchResults count is not equal to 0." );

            Assert.IsTrue( 
                searchResponse.PatientSearchResultStatus == PatientSearchResultStatus.Exception,
                "PatientSearchResultStatus is not EXCEPTION." );

            Assert.AreEqual(
                EXCEPTION_MSG,
                searchResponse.Message,
                "Patient Search Response Messages do not match." );
        }

        [Test]
        public void TestGetResultsOfTypeNormal()
        {
            List<Name> akaNames = new List<Name> {aliasName1, aliasName2};

            PatientSearchResult searchResult = new PatientSearchResult( realName, akaNames, gender, patient_dob, ssn, mrn,
                                                                       address, hspCode );

            List<PatientSearchResult> searchResults = new List<PatientSearchResult> {searchResult};

            PatientSearchResultStatus resultStatus = PatientSearchResultStatus.Success;

            PatientSearchResponse searchResponse = new PatientSearchResponse( searchResults, resultStatus );

            List<PatientSearchResult> patientSearchResults = searchResponse.GetResultsOfType( TypeOfName.Normal );

            Assert.IsNotNull( patientSearchResults );
            Assert.IsTrue( patientSearchResults.Count == 1 );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static readonly Name realName        = new Name( TEST_F_NAME, TEST_L_NAME, TEST_MI, TEST_SUFFIX, TypeOfName.Normal );
        private static readonly Name aliasName1      = new Name( TEST_AKA1_FNAME, TEST_AKA1_LNAME, String.Empty, String.Empty, TypeOfName.Alias );
        private static readonly Name aliasName2      = new Name( TEST_AKA2_FNAME, TEST_AKA2_LNAME, String.Empty, String.Empty, TypeOfName.Alias );
 
        private static readonly Gender gender        = new Gender(0,DateTime.Now,"MALE","M");
        private const string ssn            = "123456789";
        private const long mrn              = 11111;
        private const string hspCode        = "ACO";

        private static DateTime patient_dob = new DateTime( 1990, 1, 1 );
        
        private static readonly Address address  = 
            new Address( ADDRESS1,
                         ADDRESS2,
                         CITY,
                         new ZipCode( POSTAL_CODE ),
                         new State( 0L,
                                    PersistentModel.NEW_VERSION,
                                    "TEXAS",
                                    "TX"),
                         new Country( 0L,
                                      PersistentModel.NEW_VERSION,
                                      "United States",
                                      COUNTRY_USA ),
                         new County( 0L,
                                     PersistentModel.NEW_VERSION,
                                     "ORANGE",
                                     "122")
                );  

        #endregion
    }
}