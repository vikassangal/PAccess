using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture()]
    public class FacilityBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FacilityBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpFacilityBrokerTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownFacilityBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestAllFacilities()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();

            ICollection list = fb.AllFacilities();
            Assert.IsNotNull(
                list, "No Facilities were found" );

            foreach( Facility entry in list )
            {
                Assert.IsTrue( entry.GetType() == typeof( Facility ) );
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}