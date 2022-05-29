using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class CommercialInsurancePlanTests
    {
        #region Constants
   
        #endregion

        #region SetUp and TearDown CommercialInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpCommercialInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownCommercialInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new CommercialInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "Commercial";
            Assert.AreEqual(
                "Commercial",
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
                typeof(CommercialInsurancePlan),
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