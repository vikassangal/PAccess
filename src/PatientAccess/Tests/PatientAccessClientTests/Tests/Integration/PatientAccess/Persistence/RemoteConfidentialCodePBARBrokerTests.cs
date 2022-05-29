using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteConfidentialCodePBARBrokerTests
    {
        [Test()]
        public void TestConfidentialCodeBrokerRemote()
        {
            IConfidentialCodeBroker ccb = BrokerFactory.BrokerOfType<IConfidentialCodeBroker>();
            Assert.IsNotNull( ccb, "Could not create remote ConfidentialCodeBroker" );
            IList codes = ccb.ConfidentialCodesFor( 900 );
            Assert.IsNotNull( codes, "Did not get a list of ConfidentialCodes for hospital # 900" );
        }
    }
}