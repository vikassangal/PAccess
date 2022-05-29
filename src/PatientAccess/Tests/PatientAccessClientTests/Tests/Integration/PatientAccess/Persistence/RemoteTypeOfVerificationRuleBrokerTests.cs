using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class RemoteTypeOfVerificationRuleBrokerTests
    {
        [Test()]
        public void TestTypeOfVerificationRuleBrokerRemote()
        {
            ITypeOfVerificationRuleBroker br = BrokerFactory.BrokerOfType<ITypeOfVerificationRuleBroker>();
            Assert.IsNotNull( br, "Could not create remote TypeOfVerificationRuleBroker" );
            IList allTypeOfVerificationRules = br.AllTypeOfVerificationRules();
            Assert.IsNotNull( allTypeOfVerificationRules, "No TypeOfVerificationRules returned" );
            Assert.IsTrue( allTypeOfVerificationRules.Count == 4, "Incorrect number of Type Of VerificationRules returned" );

        }
    }
}