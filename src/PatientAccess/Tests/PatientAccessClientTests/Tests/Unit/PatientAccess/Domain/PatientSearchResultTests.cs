using System;
using System.Collections.Generic;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for PatientSearchResultTests.
    /// </summary>

    [TestFixture]
    [Category( "Fast" )]
    public class PatientSearchResultTests
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

        #endregion

        #region SetUp and TearDown PatientSearchResultTests
        [TestFixtureSetUp()]
        public static void SetUpPatientSearchResultTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPatientSearchResultTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestConstructor()
        {
            List<Name> akaNames = new List<Name>();
            akaNames.Add( aliasName1 );
            akaNames.Add( aliasName2 );

            PatientSearchResult searchResult = 
                new PatientSearchResult( realName, akaNames, gender, patient_dob, ssn, mrn, address, hspCode );

            Assert.AreEqual( 
                realName.AsFormattedNameWithSuffix(), 
                searchResult.Name.AsFormattedNameWithSuffix(), 
                "Real Names do not match." );

            Assert.AreEqual( 
                akaNames.Count,
                searchResult.AkaNames.Count,
                "AKA Names count do not match." );

            Assert.AreEqual( 
                akaNames[0].AsFormattedName(),
                searchResult.AkaNames[0].AsFormattedName(),
                "First Alias Names do not match." );

            Assert.AreEqual( 
                akaNames[1].AsFormattedName(),
                searchResult.AkaNames[1].AsFormattedName(),
                "Second Alias Names do not match." );

            Assert.AreEqual( 
                gender,
                searchResult.Gender,
                "Genders do not match." );

            Assert.AreEqual(
                patient_dob.ToShortDateString(),
                searchResult.DateOfBirth.ToShortDateString(),
                "Date of Births do not match." );

            Assert.AreEqual( 
                ssn,
                searchResult.SocialSecurityNumber,
                "Social Security Numbers do not match." );

            Assert.AreEqual( 
                mrn,
                searchResult.MedicalRecordNumber,
                "Medical Record Numbers do not match." );

            Assert.AreEqual( 
                address.AsMailingLabel(),
                searchResult.Address.AsMailingLabel(),
                "Addresses do not match." );

            Assert.AreEqual( 
                hspCode,
                searchResult.HspCode,
                "HspCodes do not match." );
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
                                    ReferenceValue.NEW_VERSION,
                                    "TEXAS",
                                    "TX"),
                         new Country( 0L,
                                      ReferenceValue.NEW_VERSION,
                                      "United States",
                                      COUNTRY_USA ),
                         new County( 0L,
                                     ReferenceValue.NEW_VERSION,
                                     "ORANGE",
                                     "122")
                );  

        #endregion
    }
}