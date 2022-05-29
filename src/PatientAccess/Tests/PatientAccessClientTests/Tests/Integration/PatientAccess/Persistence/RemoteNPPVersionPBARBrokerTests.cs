using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteNPPVersionPBARBrokerTests : RemoteAbstractBrokerTests
    {
        [Test()]
        public void TestNPPVersionBrokerRemote()
        {
            INPPVersionBroker nppb = BrokerFactory.BrokerOfType< INPPVersionBroker >();
            Assert.IsNotNull( nppb, "Could not create remote NPPVersionBroker" );
            ICollection versions = nppb.NPPVersionsFor( ACO_FACILITYID );
            Assert.IsNotNull( versions, "Did not get a list of NPPVersions for hospital # 900" );
            Assert.IsTrue( versions.Count > 1, "Did not find enough NPPVersions for hospital # 900" );
        }
    }
}