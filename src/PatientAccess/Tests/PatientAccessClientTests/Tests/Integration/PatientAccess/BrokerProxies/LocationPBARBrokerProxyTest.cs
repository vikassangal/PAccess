using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for LocationBrokerProxyTest.
    /// </summary>

    /// <summary>
    /// Summary description for LocationBrokerTests.
    /// </summary>
    [TestFixture()]
    public class LocationBrokerProxyTests
    {

        #region SetUp and TearDown LocationPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpLocationPBARBrokerTests()
        {
            IFacilityBroker facilityBroker =  BrokerFactory.BrokerOfType<IFacilityBroker >();
            i_ACOFacility = facilityBroker.FacilityWith( 900 );
        }

        [TestFixtureTearDown()]
        public static void TearDownLocationPBARBrokerTests()
        {
   
        }
        #endregion

        #region Test Methods
        
        [Test()]
        [Ignore("temporarily ignore while PBAR fixes the entries")]
        public void TestAccomodationCodesFor()
        {
            LocationBrokerProxy broker = new LocationBrokerProxy();
            NursingStation ns = new NursingStation( ReferenceValue.NEW_OID, 
                                                    ReferenceValue.NEW_VERSION,string.Empty,"N" );

            ArrayList accomodationsCodes = ( ArrayList )broker.AccomodationCodesFor( ns.Code, i_ACOFacility );
            Assert.IsNotNull( 
                accomodationsCodes,
                "Did not find any accomodation codes for the location selected"
                );

            ns = new NursingStation( ReferenceValue.NEW_OID, 
                                     ReferenceValue.NEW_VERSION,string.Empty,"B4" );  //has 4 rtn
            
            accomodationsCodes = ( ArrayList )broker.AccomodationCodesFor( ns.Code, i_ACOFacility );
            Assert.AreEqual( 7, accomodationsCodes.Count, "has codes:" );
                
        }
        #endregion


        #region Support Methods
        #endregion

        #region Data Elements
        private static Facility i_ACOFacility;
        #endregion

    }
}