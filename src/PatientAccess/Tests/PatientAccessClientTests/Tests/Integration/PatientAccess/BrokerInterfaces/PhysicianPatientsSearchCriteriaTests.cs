using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for PhysicianPatientsSearchCriteriaTests.
    /// </summary>
    [TestFixture()]
    public class PhysicianPatientsSearchCriteriaTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PhysicianPatientsSearchCriteriaTests
        [TestFixtureSetUp()]
        public static void SetUpPhysicianPatientsSearchCriteriaTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianPatientsSearchCriteriaTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestConstructor()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            this.iFacility = facilityBroker.FacilityWith( "DEL" );

            PhysicianPatientsSearchCriteria criteria = 
                new PhysicianPatientsSearchCriteria( this.iFacility,12345,1,1,1,1,1);

            Assert.AreEqual( this.iFacility, 
                             criteria.Facility, 
                             "The Facilities are matching" );
            Assert.AreEqual( 12345,
                             criteria.PhysicianNumber, 
                             "The Physician Number is  matching" );
            Assert.AreEqual( 1, 
                             criteria.Admitting,  
                             "The Admitting Number is  matching" );
            Assert.AreEqual( 1,
                             criteria.Attending,
                             "The Attending Number is matching" );          
            Assert.AreEqual( 1,
                             criteria.Referring,
                             "The Referring Number is matching" );
            Assert.AreEqual( 1,
                             criteria.Consulting,
                             "The Consulting Number is matching" );
            Assert.AreEqual( 1,
                             criteria.Operating,
                             "The Operating Number is matching" );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Data Elements

        Facility iFacility;

        #endregion
    }
}