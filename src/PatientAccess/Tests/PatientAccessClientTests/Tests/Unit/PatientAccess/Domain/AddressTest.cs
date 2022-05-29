using System;
using System.Collections.Specialized;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AddressTests
    {

        #region SetUp and TearDown PhoneNumberTests
        [TestFixtureSetUp()]
        public static void SetUpAddressTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAddressTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructor()
        {
            Address address = new Address( ADDRESS1,
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

            Assert.AreEqual(
                ADDRESS1,
                address.Address1
                );

            Assert.AreEqual(
                ADDRESS2,
                address.Address2
                );
            
            Assert.AreEqual(
                CITY,
                address.City
                );

            Assert.AreEqual(
                POSTAL_CODE,
                address.ZipCode.PostalCode
                );

            Assert.AreEqual(
                "TEXAS",
                address.State.Description
                );

            Assert.AreEqual(
                "TX",
                address.State.Code
                );

            Assert.AreEqual(
                "United States",
                address.Country.Description
                );

            Assert.AreEqual(
                COUNTRY_USA,
                address.Country.Code
                );

            Assert.AreEqual(
                "ORANGE",
                address.County.Description
                );

            Assert.AreEqual(
                "122",
                address.County.Code
                );

        }

        [Test()]
        public void testGetMailingLabelsString()
        {
            Address address = new Address(ADDRESS1,
                                          ADDRESS2,
                                          CITY,
                                          new ZipCode(POSTAL_CODE),
                                          new State(0L,
                                                    ReferenceValue.NEW_VERSION,
                                                    "TEXAS",
                                                    "TX"),
                                          new Country(0L,
                                                      ReferenceValue.NEW_VERSION,
                                                      "United States",
                                                      COUNTRY_USA),
                                          new County(0L,
                                                     ReferenceValue.NEW_VERSION,
                                                     "ORANGE",
                                                     "122")
                );
            StringCollection mailingList = address.GetMailingLabelStrings();
            Assert.AreEqual(
                ADDRESS1,
                mailingList[0]
                );
            if( ADDRESS2.Length > 0 )
            {
                Assert.AreEqual(
                    ADDRESS2,
                    mailingList[1]
                    );
            }
            // Console.WriteLine( mailingList[0].ToString(), mailingList[1].ToString(),mailingList[2].ToString() );
            string label = address.AsMailingLabel();
            Assert.AreEqual(
                "335 Nicholson Dr.\r\n32321\r\nAustin, TEXAS 60505\r\n\r\nCounty: 122 ORANGE", label);
                    
        }
        [Test()]
        public void testOneLineAddressLabelUS()
        {
            Address addUS = new Address( "5352 Linton Blvd",
                                         string.Empty, 
                                         "Delray Beach",
                                         new ZipCode( "33484" ),
                                         new State( 172,
                                                    "FLORIDA",
                                                    "FL" ), 
                                         new Country( 559,
                                                      "United States Of America",
                                                      COUNTRY_USA )
                );
            Assert.AreEqual( 
                EXPECTED_US_ADDRESS,
                addUS.OneLineAddressLabel()
                );
        }

        [Test()]
        public void testOneLineAddressLabelNonUS()
        {
            Address addNonUS = new Address("Plot No. 123",
                                           "SHIVALAYA", 
                                           "Whitefield",
                                           new ZipCode( "560 066" ),
                                           new State( 178,
                                                      "Bangalore ",
                                                      "Bangalore" ), 
                                           new Country( 437,
                                                        "INDIA",
                                                        "IND" )
                );
            Assert.AreEqual( 
                EXPECTED_NON_US_ADDRESS,
                addNonUS.OneLineAddressLabel()
                );
        }

        [Test()]
        public void testOneLineAddressLabelNoAddress()
        {
            Address addNo = new Address(
                String.Empty,
                String.Empty, 
                String.Empty,
                new ZipCode( string.Empty ),
                new State(), 
                new Country()
                );
            Assert.AreEqual( 
                EXPECTED_NO_ADDRESS,
                addNo.OneLineAddressLabel()
                );
        }

        [Test()]
        public void TestDeepCopy()
        {
            Address addy = new Address( "123 Foo St.", 
                                        String.Empty, "Plano", 
                                        new ZipCode("75075"), 
                                        new State(), 
                                        new Country( "US" ) );
            Address copy = (Address)addy.DeepCopy();

            Assert.AreNotSame( addy, copy ) ;
            Assert.AreEqual( addy.Address1, copy.Address1 );
            Assert.AreEqual( addy.Address2, copy.Address2 );
            Assert.AreEqual( addy.City, copy.City );
        }

        [Test()]
        public void TestFormattedPostalCode()
        {
            string unformattedPostalCode = "123456789";
            string formattedPostalCode   = "12345-6789";

            Address address = new Address();
            address.ZipCode = new ZipCode( "123456789    " );

            Assert.AreEqual( 
                unformattedPostalCode,
                address.ZipCode.FormattedPostalCodeFor( false )
                );
            
            address.Country = Country.NewUnitedStatesCountry();

            Assert.AreEqual( 
                formattedPostalCode,
                address.ZipCode.FormattedPostalCodeFor( true )
                );

            address.ZipCode.PostalCode = formattedPostalCode + "    ";

            Assert.AreEqual( 
                formattedPostalCode,
                address.ZipCode.FormattedPostalCodeFor( true )
                );

            address.ZipCode = new ZipCode( EXPECTED_FORMATTED_ZIP_CODE ) ;
            
            Assert.AreEqual( 
                EXPECTED_FORMATTED_ZIP_CODE,
                address.ZipCode.FormattedPostalCodeFor( true )
                );

            address.Country = null;

            Assert.AreEqual( 
                EXPECTED_FORMATTED_ZIP_CODE,
                address.ZipCode.FormattedPostalCodeFor(false )
                );
        }

        #endregion

        #region Data Elements

        const string    ADDRESS1                    = "335 Nicholson Dr.",
                        ADDRESS2                    = "32321",
                        CITY                        = "Austin",    
                        POSTAL_CODE                 = "60505",
                        EXPECTED_US_ADDRESS         = "5352 Linton Blvd, Delray Beach, FL, 33484",
                        EXPECTED_NON_US_ADDRESS     = "Plot No. 123, SHIVALAYA, Whitefield, Bangalore, 560 066, INDIA",
                        EXPECTED_FORMATTED_ZIP_CODE = "T2N 0E6",
                        COUNTRY_USA                 = "USA" ;
        readonly string EXPECTED_NO_ADDRESS         = string.Empty;

        #endregion

    }
}