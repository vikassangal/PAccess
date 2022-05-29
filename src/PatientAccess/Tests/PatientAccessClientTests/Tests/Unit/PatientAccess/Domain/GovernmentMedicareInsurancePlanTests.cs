using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentMedicareInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentMedicareInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpGovernmentMedicareInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentMedicareInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new GovernmentMedicareInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "Medicare";
            Assert.AreEqual(
                "Medicare",
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
                typeof(GovernmentMedicareInsurancePlan),
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