using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentMedicaidInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentMedicaidInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpGovernmentMedicaidInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentMedicaidInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new GovernmentMedicaidInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "Medicaid";
            Assert.AreEqual(
                "Medicaid",
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
                typeof(GovernmentMedicaidInsurancePlan),
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