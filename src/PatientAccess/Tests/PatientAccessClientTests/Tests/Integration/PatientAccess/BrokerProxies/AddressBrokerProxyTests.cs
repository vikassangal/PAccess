using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class AddressBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AddressBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpAddressBrokerProxyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAddressBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void AllCountries()
        {
            long facilityNumber = 98;
            Country afghanistan = null,
                    albania = null,
                    china = null;

            IList countries = new AddressBrokerProxy().AllCountries( facilityNumber );

            // this test is OK. US must be 1st in this list.
            Assert.AreEqual(
                "USA",
                ((Country)countries[1]).Code,
                "First entry should be USA" );

            foreach( Country country in countries )
            {
                switch( country.Code )
                {
                    case "AFG":
                        afghanistan = country;
                        break;
                    case "ALB":
                        albania = country;
                        break;
                    case "CHN":
                        china = country;
                        break;
                    default:
                        break;
                }
            }

            Assert.IsNotNull(afghanistan,"Afghanistan not found");
            Assert.AreEqual(
                "AFG",
                afghanistan.Code,
                "Afghanistan code incorrect" );

            Assert.IsNotNull(albania,"Albania not found");
            Assert.AreEqual(
                "ALB",
                albania.Code,
                "Albania code incorrect" );

            Assert.IsNotNull(china, "China not found");
            Assert.AreEqual(
                "CHN",
                china.Code.Trim(),
                "China code incorrect" ); 
        }

        [Test()]
        public void TestAllCountiesFor()
        {
            long facilityNumber = 98;

            ICollection counties = (new AddressBrokerProxy()).AllCountiesFor(facilityNumber);

            Assert.IsTrue(counties.Count > 0);
        }

        [Test()]
        public void TestCountyWith()
        {
            const long facilityNumber = 98;
            const string expectedCountyCode = "122";
            const string expectedStateCode = "AK";
            const string expectedCountyDescription = "KENAI PENINSULA";

            var actualCounty = (new AddressBrokerProxy()).CountyWith(facilityNumber, expectedStateCode,expectedCountyCode);

            Assert.AreEqual( expectedCountyCode, actualCounty.Code );
            
            Assert.AreEqual( expectedCountyDescription, actualCounty.Description );
        }


        [Test()]
        public void TestCountryByCode()
        {
            long facilityNumber = 98;
            Country canada = new AddressBrokerProxy().CountryWith( facilityNumber, "CAN" );
            Assert.IsNotNull(canada,"Country object for Canada not found");
            Assert.AreEqual("CANADA",canada.Description.ToUpper(), "Country Description not correct");

            Country badCountry = new AddressBrokerProxy().CountryWith(facilityNumber, "CCP");
            Assert.IsNotNull(badCountry, "No unknown country returned for 'CCP'");
            Assert.AreEqual(badCountry.Code, string.Empty);
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}