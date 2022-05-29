using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Persistence;
using Peradigm.Framework.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for OrganizationalHierarchyProviderTest.
    /// </summary>

    //TODO: Create XML summary comment for OrganizationalHierarchyProviderTests
    [TestFixture()]
    public class OrganizationalHierarchyProviderTest : AbstractBrokerTests
    {
        #region Constants

        private new const string 
            FACILITY_CODE = "DEL";

        #endregion

        #region SetUp and TearDown OrganizationalHierarchyProviderTests
        [TestFixtureSetUp()]
        public static void SetUpOrganizationalHierarchyProviderTest()
        {
            i_OrganizationalHierarchyProvider =  new OrganizationalHierarchyProvider();
        }

        [TestFixtureTearDown()]
        public static void TearDownOrganizationalHierarchyProviderTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestGetOrganizationalHierarchy()
        {
            bool facilityFoundFlag = false;

            OrganizationalUnit hierarchyRoot = i_OrganizationalHierarchyProvider.GetOrganizationalHierarchy();
            ArrayList allHierarchyFacilities = hierarchyRoot.AllFacilities();

            FacilityBroker facilityBroker = (FacilityBroker)BrokerFactory.BrokerOfType<IFacilityBroker>();
            ArrayList allFacilities = facilityBroker.AllFacilities() as ArrayList;

            Assert.AreEqual( allHierarchyFacilities.Count, allFacilities.Count, "Different number of facilities returned by OrganizationalHierarchyProvider and FacilityBroker" );

            foreach( AbstractOrganizationalUnit securityFrameworkFacility in allHierarchyFacilities )
            {
                if( securityFrameworkFacility.Code.Equals( FACILITY_CODE ) )
                {
                    facilityFoundFlag = true;
                    break;
                }
            }
            Assert.IsTrue( facilityFoundFlag, "Facility with the code = DEL was not found in built OrganizationalHierarchy");
        }
         
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static OrganizationalHierarchyProvider i_OrganizationalHierarchyProvider;
        #endregion
    }
}