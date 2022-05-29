using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SelfPayInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown SelfPayInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpSelfPayInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownSelfPayInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new  SelfPayInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "SelfPay";
            Assert.AreEqual(
                "SelfPay",
                plan.PlanName
                );
            plan.Payor = p;
            plan.PlanSuffix = "123";
            Assert.AreEqual(
                "123",
                plan.PlanSuffix
                );
            
            string planID = p.Code+plan.PlanSuffix;
            Assert.AreEqual(
                planID,
                plan.PlanID
                );
            Assert.AreEqual(
                typeof( SelfPayInsurancePlan),
                plan.GetType()
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
   
        #endregion
    }
}