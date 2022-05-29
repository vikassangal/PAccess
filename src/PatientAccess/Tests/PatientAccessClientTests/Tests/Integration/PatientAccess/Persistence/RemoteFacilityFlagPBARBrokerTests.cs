using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteFacilityFlagPBARBrokerTests
    {
        [Test()]
        public void TestFacilityFlagBrokerRemote()
        {
            IFacilityFlagBroker ffb = BrokerFactory.BrokerOfType<IFacilityFlagBroker>();
            Assert.IsNotNull( ffb, "Could not create remote FacilityFlagPBARBroker" );
            IList flags = ffb.FacilityFlagsFor( 900 );
            Assert.IsNotNull( ffb, "Did not get a list of FacilityFlags" );
            Assert.IsTrue( flags.Count > 0, "Did not find any facilityFlags" );
        }
    }
}