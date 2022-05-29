using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for RemoteConditionOfServiceBrokerTests.
    /// </summary>
    [TestFixture]
    public class RemoteConditionOfServiceBrokerTests : RemoteAbstractBrokerTests
    {
        #region Constants
        private const int TOTAL_NUMBER_OF_COS = 5;
        #endregion

        #region SetUp and TearDown ConditionOfServiceBrokerTests
        [TestFixtureSetUp]
        public static void SetUpConditionOfServiceBrokerTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownConditionOfServiceBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestAllConditionsOfServiceToCheckItReturnsCorrectNumberOfConditionsOfService()
        {
            IList<ConditionOfService> services = GetAllConditionsOfService();

            Assert.IsNotNull(services, "Null list returned from ConditionOfServiceBroker");
            Assert.AreEqual(TOTAL_NUMBER_OF_COS, services.Count, "Incorrect number of COS Values returned");
        }

        [Test]
        public void TestAllConditionsOfServiceToCheckItContainsConditionOfServiceCodeBlank()
        {
            IList<ConditionOfService> allConditionsOfService = GetAllConditionsOfService();
            string conditionOfServiceCode = ConditionOfService.BLANK;
            bool containsServicecodeBlank = ContainsConditionServiceWith(allConditionsOfService, conditionOfServiceCode);

            Assert.IsTrue(containsServicecodeBlank, string.Format("Condition of service with code: {0} was found", conditionOfServiceCode));

        }
        [Test]
        public void TestAllConditionsOfServiceToCheckItContainsConditionOfServiceCodeYes()
        {
            IList<ConditionOfService> allConditionsOfService = GetAllConditionsOfService();
            const string conditionOfServiceCode = ConditionOfService.YES;
            bool containsServicecodeBlank = ContainsConditionServiceWith(allConditionsOfService, conditionOfServiceCode);

            Assert.IsTrue(containsServicecodeBlank, string.Format("Condition of service with code: {0} was found", conditionOfServiceCode));

        }

        [Test]
        public void TestAllConditionsOfServiceToCheckItContainsConditionOfServiceCodeUnable()
        {
            IList<ConditionOfService> allConditionsOfService = GetAllConditionsOfService();
            const string conditionOfServiceCode = ConditionOfService.UNABLE;
            bool containsServicecodeBlank = ContainsConditionServiceWith(allConditionsOfService, conditionOfServiceCode);

            Assert.IsTrue(containsServicecodeBlank, string.Format("Condition of service with code: {0} was found", conditionOfServiceCode));

        }
        [Test]
        public void TestAllConditionsOfServiceToCheckItContainsConditionOfServiceCodeRefused()
        {
            IList<ConditionOfService> allConditionsOfService = GetAllConditionsOfService();
            const string conditionOfServiceCode = ConditionOfService.REFUSED;
            bool containsServicecodeBlank = ContainsConditionServiceWith(allConditionsOfService, conditionOfServiceCode);

            Assert.IsTrue(containsServicecodeBlank, string.Format("Condition of service with code: {0} was found", conditionOfServiceCode));

        }
        [Test]
        public void TestAllConditionsOfServiceToCheckItContainsConditionOfServiceCodeNotAvailable()
        {
            IList<ConditionOfService> allConditionsOfService = GetAllConditionsOfService();
            const string conditionOfServiceCode = ConditionOfService.NOT_AVAILABLE;
            bool containsServicecodeBlank = ContainsConditionServiceWith(allConditionsOfService, conditionOfServiceCode);

            Assert.IsTrue(containsServicecodeBlank, string.Format("Condition of service with code: {0} was found", conditionOfServiceCode));
        }

        private static IList<ConditionOfService> GetAllConditionsOfService()
        {
            conditionOfServiceBroker = BrokerFactory.BrokerOfType<IConditionOfServiceBroker>();
            ICollection conditionServices = conditionOfServiceBroker.AllConditionsOfService();
            return conditionServices.Cast<ConditionOfService>().ToList();
        }


        private static bool ContainsConditionServiceWith(IEnumerable<ConditionOfService> services, string code)
        {
            return services.Where(conditionOfService => conditionOfService.Code == code).Count() > 0;
        }



        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IConditionOfServiceBroker conditionOfServiceBroker;
        #endregion
    }
}