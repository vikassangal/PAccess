using System.Collections.Generic;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class DialysisCenterNamePBARBrokerTests : AbstractBrokerTests
    { 
        [Test]
        public void TestAllDialysisCenterNames()
        {
            IEnumerable<string> allDialysisCenterNames = BrokerFactory.BrokerOfType<IDialysisCenterBroker>().AllDialysisCenterNames(ACO_FACILITYID);
            Assert.IsTrue(allDialysisCenterNames.Any(), "No Dialysis center names found");
        }
    }
}