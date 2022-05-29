using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class CoveredGroupTests

    {
        #region Constants
        #endregion

        #region SetUp and TearDown CoveredGroupTests
        [TestFixtureSetUp()]
        public static void SetUpCoveredGroupTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCoveredGroupTests()
        {
        }
        #endregion

        #region Test Methods
        public void TestAddAddress()
        {
            CoveredGroup group = new CoveredGroup();
            ContactPoint cp = new ContactPoint();
            cp.Address = this.address;
            //            group.AddAddress(address);
            group.AddContactPoint(cp);
            group.Name = "Cigna";
            Assert.AreEqual( 
                "Cigna",
                group.Name,
                "Name of the CoveredGroup should be " );

            Assert.AreEqual(
                1,
                group.ContactPoints.Count
                );
            
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        const string ADDRESS1 = "335 Nicholson Dr.",
                     ADDRESS2 = "#303",
                     CITY = "Austin",    
                     POSTALCODE = "60505";

        readonly Address address = new Address( ADDRESS1,
                                       ADDRESS2,
                                       CITY,
                                       new ZipCode( POSTALCODE ),
                                       new State( 0L,
                                                  ReferenceValue.NEW_VERSION,
                                                  "TEXAS",
                                                  "TX"),
                                       new Country( 0L,
                                                    ReferenceValue.NEW_VERSION,
                                                    "United States",
                                                    "USA"),
                                       new County( 0L,
                                                   ReferenceValue.NEW_VERSION,
                                                   "ORANGE",
                                                   "122")
            );
        #endregion

    }
}