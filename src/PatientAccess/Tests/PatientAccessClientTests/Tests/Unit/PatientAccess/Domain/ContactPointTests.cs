using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ContactPointTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ContactPointTests
        [TestFixtureSetUp()]
        public static void SetUpContactPointTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownContactPointTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestSimpleConstructor()
        {
            ContactPoint cp = new ContactPoint( TypeOfContactPoint.NewMailingContactPointType() );

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}