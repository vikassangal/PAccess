using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class OtherInsurancePlanTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown OtherInsurancePlanTests
        [TestFixtureSetUp()]
        public static void SetUpOtherInsurancePlanTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownOtherInsurancePlanTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConstructor()
        {
            Payor p = new Payor();
            p.Code = "AA";
                  
            InsurancePlan plan  = new  OtherInsurancePlan();
            plan.Oid = 1;
            Assert.AreEqual(
                1,
                plan.Oid
                );
            plan.PlanName = "other";
            Assert.AreEqual(
                "other",
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
                typeof( OtherInsurancePlan),
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