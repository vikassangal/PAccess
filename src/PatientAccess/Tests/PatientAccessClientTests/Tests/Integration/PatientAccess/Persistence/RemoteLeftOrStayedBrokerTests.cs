using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for RemoteLeftOrStayedBrokerTests.
    /// </summary>
    [TestFixture]
    public class RemoteLeftOrStayedBrokerTests : RemoteAbstractBrokerTests
    {
        #region Constants
        private const int TOTAL_NUMBER_OF_LEFTORSTAYED = 3;
        #endregion

        #region SetUp and TearDown LeftOrStayedBrokerTests
        [TestFixtureSetUp]
        public static void SetUpLeftOrStayedBrokerTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownLeftOrStayedBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestAllLeftOrStayedToCheckItReturnsCorrectNumberOfLeftOrStayed()
        {
            IList<LeftOrStayed> leftOrStayedCollection = GetAllLeftOrStayed();

            Assert.IsNotNull(leftOrStayedCollection, "Null list returned from LeftOrStayedBroker");
            Assert.AreEqual(TOTAL_NUMBER_OF_LEFTORSTAYED, leftOrStayedCollection.Count, "Incorrect number of LeftOrStayed Values returned");
        }

        [Test]
        public void TestAllLeftOrStayedToCheckItContainsLeftOrStayedCodeBlank()
        {
            IList<LeftOrStayed> allLeftOrStayed = GetAllLeftOrStayed();
            string leftOrStayedCode = LeftOrStayed.BLANK;
            bool leftOrStayedBlank = ContainsLeftOrStayedWith(allLeftOrStayed, leftOrStayedCode);

            Assert.IsTrue(leftOrStayedBlank, string.Format("Left or Stayed with code: {0} was found", leftOrStayedCode));

        }
        [Test]
        public void TestAllLeftOrStayedToCheckItContainsLeftOrStayedCodeLeft()
        {
            IList<LeftOrStayed> allLeftOrStayed = GetAllLeftOrStayed();
            const string leftOrStayedCode = LeftOrStayed.LEFT;
            bool containsLeftOrStayedLeft = ContainsLeftOrStayedWith(allLeftOrStayed, leftOrStayedCode);

            Assert.IsTrue(containsLeftOrStayedLeft, string.Format("Left or Stayed with code: {0} was found", leftOrStayedCode));

        }

        [Test]
        public void TestAllLeftOrStayedToCheckItContainsLeftOrStayedCodeStayed()
        {
            IList<LeftOrStayed> allLeftOrStayed = GetAllLeftOrStayed();
            const string leftOrStayedCode = LeftOrStayed.STAYED;
            bool containsLeftOrStayedStayed = ContainsLeftOrStayedWith(allLeftOrStayed, leftOrStayedCode);

            Assert.IsTrue(containsLeftOrStayedStayed, string.Format("Left or stayed with code: {0} was found", leftOrStayedCode));

        }
         
        private static IList<LeftOrStayed> GetAllLeftOrStayed()
        {
            broker = BrokerFactory.BrokerOfType<ILeftOrStayedBroker>();
            ICollection<LeftOrStayed> leftOrStayedCollection = broker.AllLeftOrStayed();
            return leftOrStayedCollection.ToList();
        }


        private static bool ContainsLeftOrStayedWith(IEnumerable<LeftOrStayed> leftOrStayedCollection, string code)
        {
            return leftOrStayedCollection.Where(leftOrStayed => leftOrStayed.Code == code).Count() > 0;
        }



        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ILeftOrStayedBroker broker;
        #endregion
    }
}