using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AgeEntitlementTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AgeTests
        [TestFixtureSetUp()]
        public static void SetUpAgeEntitlementTests()
        {
            a = new AgeEntitlement() ;
        }

        [TestFixtureTearDown()]
        public static void TearDownAgeEntitlementTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestSetup()
        {
            // test the created object
            Assert.IsNotNull( a ) ;
        }

        [Test()]
        public void TestEmploymentCloneAndDeepCopy()
        {
            // test clone 
            Employment g = (Employment)e.Clone();
            Assert.IsNotNull(g);
            Assert.AreNotSame(e, g);
            Assert.AreEqual(e.Employee, g.Employee);
            Assert.AreEqual(e.EmployeeID, g.EmployeeID);
            Assert.AreEqual(e.Occupation, g.Occupation);
            Assert.AreEqual(e.RetiredDate, g.RetiredDate);
            Assert.IsNotNull(g.Employer);

            // test deep copy
            Employment f = (Employment)e.DeepCopy();
            Assert.IsNotNull(f);
            Assert.AreNotSame(e, f);
            Assert.AreEqual(e.Employee, f.Employee);
            Assert.AreEqual(e.EmployeeID, f.EmployeeID);
            Assert.AreEqual(e.Occupation, f.Occupation);
            Assert.AreEqual(e.RetiredDate, f.RetiredDate);
            Assert.IsNotNull(f.Employer);
            Assert.AreEqual(e.Employer.EmployerCode, f.Employer.EmployerCode);
            Assert.AreEqual(e.Employer.Timestamp, f.Employer.Timestamp);
        }

        [Test()]
        public void TestAgeEntitlementClone()
        {
            AgeEntitlement aa = new AgeEntitlement();
            AgeEntitlement ab = (AgeEntitlement)aa.Clone();
            Assert.IsNotNull(ab);
            Assert.AreNotSame(aa, ab);
            Assert.AreEqual(aa.EntitlementType, ab.EntitlementType);
        }

        [Test()]
        public void TestAgeEntitlementEquals()
        {
            AgeEntitlement aa = new AgeEntitlement();
            AgeEntitlement ab = null;
            Assert.IsFalse( aa.Equals( ab ) ) ;
        }

        [Test()]
        public void TestCollectRecommendation()
        {
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static  AgeEntitlement a = null ;
        private static readonly Employment e = new Employment();

        #endregion
    }
}