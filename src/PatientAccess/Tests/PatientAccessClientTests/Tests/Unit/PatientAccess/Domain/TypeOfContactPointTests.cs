using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TypeOfContactPointTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown TypeOfContactPointTests
        [TestFixtureSetUp()]
        public static void SetUpTypeOfContactPointTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownTypeOfContactPointTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestFactoryMethods()
        {
            TypeOfContactPoint home     = new TypeOfContactPoint( TypeOfContactPoint.MAILING_OID, "MAILING" );
            TypeOfContactPoint work     = new TypeOfContactPoint( TypeOfContactPoint.EMPLOYER_OID, "EMPLOYER" );
            TypeOfContactPoint cell     = new TypeOfContactPoint( TypeOfContactPoint.MOBILE_OID, "CELL" );
            TypeOfContactPoint billing  = new TypeOfContactPoint( TypeOfContactPoint.BILLING_OID, "BILLING" );

            Assert.AreEqual(
                home,
                TypeOfContactPoint.NewMailingContactPointType()
                );

            Assert.AreEqual(
                work,
                TypeOfContactPoint.NewEmployerContactPointType()
                );

            Assert.AreEqual(
                cell,
                TypeOfContactPoint.NewMobileContactPointType()
                );

            Assert.AreEqual(
                billing,
                TypeOfContactPoint.NewBillingContactPointType()
                );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}