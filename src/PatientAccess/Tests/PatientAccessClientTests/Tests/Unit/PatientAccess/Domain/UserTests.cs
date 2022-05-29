using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class UserTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown UserTests
        [TestFixtureSetUp()]
        public static void SetUpUserTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownUserTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestExecutionOnServer()
        {
            Assert.AreEqual( false, User.IsExecutingOnServer() );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}