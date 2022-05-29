using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture()]
    public class CoveredGroupPlansLoaderTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PlansLoaderTests
        [TestFixtureSetUp()]
        public static void SetUpPlansLoaderTests()
        {
            iFacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            iAccount = new Account();
            iAccount.AdmitDate = DateTime.Parse( "01/01/2005" );
            Facility aFacility = iFacilityBroker.FacilityWith( 6 );
            iAccount.Facility = aFacility;
        }

        [TestFixtureTearDown()]
        public static void TearDownPlansLoaderTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        [Ignore()]
        public void TestLoadCoveredGroup()
        {
            CoveredGroup cg = new CoveredGroup();
            cg.Oid = 506L;
            cg.PlansLoader = new CoveredGroupPlansLoader( cg, iAccount.Facility.Oid, iAccount.AdmitDate );

            ICollection plans = cg.InsurancePlans;
            Assert.IsNotNull( plans, "No plans list returned" );
            Assert.AreEqual( 8, plans.Count, "Did not find expected number of plans" );

            InsurancePlan foundplan = null;
            foreach (InsurancePlan plan in plans)
            {
                if (plan.Oid == 2250)
                {
                    Assert.AreEqual( "MEDICA CHOICE / AHP", plan.PlanName, "Plan Name not correct" );
                    Assert.AreEqual( "R4030", plan.PlanID, "PlanID is not as expected" );
                    foundplan = plan;
                    break;
                }
            }
            Assert.IsNotNull( foundplan, "Plan with OID 2250 not found" );

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IFacilityBroker iFacilityBroker;
        private static Account iAccount;
        #endregion
    }
}