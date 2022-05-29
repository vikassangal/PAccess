using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture()]
    public class PlansLoaderTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PlansLoaderTests
        [TestFixtureSetUp()]
        public static void SetUpPlansLoaderTests()
        {   
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();

            i_Account           = new Account();
            i_Account.AdmitDate = DateTime.Parse("01/01/2005");
            Facility aFacility  = fb.FacilityWith(6);
            i_Account.Facility  = aFacility;
        }

        [TestFixtureTearDown()]
        public static void TearDownPlansLoaderTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        [Ignore()]
        public void TestPlanLoadForPayor()
        {

            Payor p = new Payor();
            p.Oid = 10L;
            p.PlansLoader = new PlansLoader( p, i_Account.Facility.Oid, i_Account.AdmitDate, i_PlanCategory);

            Assert.IsNotNull( p.InsurancePlans, "Did not find any plans for plan: 10" );
            Assert.AreEqual( 
                2,
                p.InsurancePlans.Count, 
                "Expected there to be 2 plans for the Payor" );
        }

        [Test()]
        [Ignore()]
        public void TestPlanLoadForPayorCat1()
        {
            InsurancePlanCategory pc = new InsurancePlanCategory(1,ReferenceValue.NEW_VERSION,"Cat1","1");
            Payor p = new Payor();
            p.Oid = 365L;
            p.PlansLoader = new PlansLoader( p, i_Account.Facility.Oid, i_Account.AdmitDate ,pc);

            Assert.IsNotNull( p.InsurancePlans, "Did not find any plans for plan: 365" );
            Assert.AreEqual( 
                3,
                p.InsurancePlans.Count, 
                "Expected there to be 3 plans for the Payor" );            
        }
        [Test()]
        [Ignore()]
        public void TestPlanLoadForBroker()
        {
            Broker b = new Broker();
            
            b.Oid = 172L;
            b.PlansLoader = new PlansLoader(b, i_Account.Facility.Oid, i_Account.AdmitDate, i_PlanCategory);

            Assert.IsNotNull( b.InsurancePlans, "Did not find any plans for broker: 172" );
            Assert.AreEqual( 
                3,
                b.InsurancePlans.Count,
                "Expected there to be 3 plans for the Broker" );
        }

        [Test()] 
        public void TestFailPlanLoader()
        {
            IInsuranceBroker iInsuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
            InsurancePlanCategory pc = iInsuranceBroker.InsurancePlanCategoryWith( 1, i_Account.Facility.Oid);
            Payor p = new Payor();
            p.Oid = -1L;
            p.PlansLoader = new PlansLoader( p, i_Account.Facility.Oid, i_Account.AdmitDate ,pc);
            if(p.InsurancePlans.Count > 0)
            {
                Assert.Fail( "Should not have found any plans with ID of -1" );
            }
            //Assert.IsNotNull( p.InsurancePlans, "Did not find any plans for plan: 365" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private static Account                     i_Account;
        private static InsurancePlanCategory       i_PlanCategory = null;

        #endregion
    }
}