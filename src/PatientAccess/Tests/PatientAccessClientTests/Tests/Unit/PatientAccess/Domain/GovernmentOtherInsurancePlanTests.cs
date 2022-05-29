using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentOtherInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentOtherInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpGovernmentOtherInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentOtherInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new GovernmentOtherInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "GovtOther";
            Assert.AreEqual(
                "GovtOther",
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
                typeof(GovernmentOtherInsurancePlan),
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