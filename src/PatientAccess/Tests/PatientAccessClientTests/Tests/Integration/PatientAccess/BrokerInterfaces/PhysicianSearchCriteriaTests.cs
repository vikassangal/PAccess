using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for PhysicianSearchCriteriaTests.
    /// </summary>
    [TestFixture()]
    public class PhysicianSearchCriteriaTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PhysicianSearchCriteriaTests
        [TestFixtureSetUp()]
        public static void SetUpPhysicianSearchCriteriaTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianSearchCriteriaTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestConstructor()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            this.iFacility = facilityBroker.FacilityWith( "DEL" );
            PhysicianSearchCriteria criteria = 
                new PhysicianSearchCriteria( this.iFacility, "FNameTest", 
                                             "LNameTest", 12345 );

            Assert.AreEqual( this.iFacility, criteria.Facility,
                             "The Facilities are matching" );
            Assert.AreEqual( "FNameTest", criteria.FirstName, 
                             "The First Names are matching" );
            Assert.AreEqual( "LNameTest", criteria.LastName,
                             "The Last Names are matching" );
            Assert.AreEqual( 12345, criteria.PhysicianNumber,
                             "The Physician Numbers are matching" );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Data Elements

        Facility iFacility;

        #endregion
    }
}