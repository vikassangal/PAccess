using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmploymentStatusTests
    {
        #region Constants
        private const string
            TESTDESCRIPTION         = "TESTDESCRIPTION",
            TESTCODE                = "TESTCODE";
        private const long
            TESTOID                 = 3245;
        #endregion

        #region SetUp and TearDown EmploymentStatusTests
        [TestFixtureSetUp()]
        public static void SetUpEmploymentStatusTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownEmploymentStatusTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestNewRetured()
        {
            EmploymentStatus status = EmploymentStatus.NewRetired();
            Assert.IsTrue(status.Description == "RETIRED","Desc Returned is incorrect");
            Assert.IsTrue(status.Code == EmploymentStatus.RETIRED_CODE,"CODE for Retired EmpStatus is incorrect");

            status = EmploymentStatus.NewNotEmployed();
            Assert.IsTrue(status.Description == "NOT EMPLOYED","Desc for Unemployed is incorrect");
            Assert.IsTrue(status.Code == EmploymentStatus.NOT_EMPLOYED_CODE,"CODE for Not Employed EmpStatus is incorrect");

            status = new EmploymentStatus(TESTOID,ReferenceValue.NEW_VERSION,
                                          TESTDESCRIPTION,TESTCODE);
            Assert.IsTrue(status.Description == TESTDESCRIPTION,"Description is incorrect");
            Assert.IsTrue(status.Oid == TESTOID,"OID is incorrect");
            Assert.IsTrue(status.Code == TESTCODE,"Code is incorrect");

            //Employed Test
            status = EmploymentStatus.NewFullTimeEmployed();
            Assert.IsTrue(status.Description == "EMPLOYED FULL TIME","Description Returned is incorrect");
            Assert.IsTrue(status.Code == EmploymentStatus.EMPLOYED_FULL_TIME_CODE,
                          "CODE for Employed EmpStatus is incorrect");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}