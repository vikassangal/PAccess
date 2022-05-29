using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class SSNPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants

        private const   long    TEST_HOSPITAL_CODE  = 178;
        private const   string  NEWBORN_DESCRIPTION = "NEWBORN";
        private const   string  TEST_STRING_FLORIDA = "FLORIDA";
        private const   string  TEST_CODE_FLORIDA   = "FL";
        #endregion

        #region SetUp and TearDown SSNBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpSSNBrokerTests()
        {
            i_SSNBroker = BrokerFactory.BrokerOfType<ISSNBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownSSNBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestSSNStatuses()
        {
            State st = new State( TEST_HOSPITAL_CODE, TEST_STRING_FLORIDA, TEST_CODE_FLORIDA );
            ArrayList statuses = i_SSNBroker.SSNStatuses( ACO_FACILITYID, st.Code);

            Assert.IsNotNull(statuses, "No SSN Statuses list found");
            Assert.IsTrue(statuses.Count > 0, "No SSN Statuses Found");

            SocialSecurityNumberStatus ssnStat = statuses[0] as SocialSecurityNumberStatus ;
            if (null != ssnStat)
            {
                Assert.AreEqual(ssnStat.Description, "Known");
            }            
        }

        [Test()]
        public void TestSSNStatusesWithNullCode()
        {
            ArrayList statuses = i_SSNBroker.SSNStatuses( ACO_FACILITYID, null) ;
            Assert.IsNotNull(statuses, "No SSN Statuses list found");
        }

        [Test()]
        public void TestSSNStatusesForGuarantor()
        {
            State st = new State( TEST_HOSPITAL_CODE, TEST_STRING_FLORIDA, TEST_CODE_FLORIDA );
            ICollection statuses = i_SSNBroker.SSNStatusesForGuarantor( ACO_FACILITYID, st.Code);

            Assert.IsNotNull(statuses, "No SSN Statuses list found");
            Assert.IsTrue(statuses.Count > 0, "No SSN Statuses Found");

            foreach (SocialSecurityNumberStatus ssnStatus in statuses)
                if (ssnStatus.Description.Trim().ToUpper() == NEWBORN_DESCRIPTION)
                    Assert.Fail("Fetching NewBorn SSN Status for Guarantor");
        }

        [Test()]
        public void TestSSNStatusesForGuarantorWithNull()
        {
            ICollection statuses = i_SSNBroker.SSNStatusesForGuarantor( ACO_FACILITYID, null) ;
            Assert.IsNotNull( statuses, "No SSN Statuses list found" ) ;
        }

        [Test()]
        public void TestSSNStatusWith()
        {
            i_SSNBroker = BrokerFactory.BrokerOfType<ISSNBroker>();
            SocialSecurityNumberStatus ssnStatus = i_SSNBroker.SSNStatusWith( ACO_FACILITYID, "None" );
            Assert.IsNotNull(ssnStatus, "No SSN Status was found for description 'None'.");

            ssnStatus = i_SSNBroker.SSNStatusWith( ACO_FACILITYID, "Newborn" );
            Assert.IsNotNull(ssnStatus, "No SSN Status found for description 'Newborn'.");
        }
      
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static ISSNBroker i_SSNBroker = null;
        #endregion
    }
}