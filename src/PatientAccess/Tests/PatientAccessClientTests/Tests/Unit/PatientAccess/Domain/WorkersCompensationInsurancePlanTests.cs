using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class WorkersCompensationInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown WorkersCompensationInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpWorkersCompensationInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownWorkersCompensationInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new  WorkersCompensationInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "WorkersComp";
            Assert.AreEqual(
                "WorkersComp",
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
                typeof( WorkersCompensationInsurancePlan),
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