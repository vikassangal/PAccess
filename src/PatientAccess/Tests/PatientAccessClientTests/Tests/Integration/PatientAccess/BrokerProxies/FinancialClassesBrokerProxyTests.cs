using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    [TestFixture()]
    public class FinancialClassesBrokerProxyTests
    {
        #region Constants
        private const string
            TESTFINANCIALCLASSCODE          = "03",
            TEST_FINANCIALCLASS_CODE_14     = "14",
            TESTFINANCIALCLASSCODEBAD       = "RR",
            TESTFINANCIALCLASSCODEAN        = "ZZ",
            TEST_FINAINCIALCLASS_CODE_UNINSURED = "70",
            TESTFINANCIALCLASSDESCRIPTION   = "CHARITY CARE";

        private const long
            STANDARD_FINANCIAL_CLASS_TYPE    = 1L,
            UNINSURED_FINANCIAL_CLASS_TYPE   = 2L,
            NONSENSICAL_FINANCIAL_CLASS_TYPE = 300L;

        private const long ACO_FACILITYID = 900;
        #endregion

        #region SetUp and TearDown FinancialClassesBrokerProxyTests
        [TestFixtureSetUp()]
        public static void SetUpFinancialClassesBrokerProxyTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownFinancialClassesBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestFinancialClassesAll()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();

            ArrayList table = (ArrayList)fcb.AllTypesOfFinancialClasses( ACO_FACILITYID );
            Assert.IsTrue( table != null, "There are no financial Classes" );
        }

        [Test()]
        public void TestSingleFinancialClass()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc = fcb.FinancialClassWith( ACO_FACILITYID,TESTFINANCIALCLASSCODE );

            Assert.IsTrue( fc != null,
                           "Did not find financial Code: " + TESTFINANCIALCLASSCODE );
            Assert.IsTrue( fc.Description.Trim().Equals(TESTFINANCIALCLASSDESCRIPTION),
                           "Did not find the proper financial Class description" );
        }

        [Test()]
        [Ignore]
        public void TestSingleFinancialClassBadFC()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc = fcb.FinancialClassWith( ACO_FACILITYID,TESTFINANCIALCLASSCODEBAD );

            Assert.IsFalse( fc.IsValid ,
                            "Should not have found financial Code: " + TESTFINANCIALCLASSCODEBAD );
        }
        [Test()]
        [Ignore]
        public void TestSingleFinancialClassANFC()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc = fcb.FinancialClassWith(ACO_FACILITYID, TESTFINANCIALCLASSCODEAN);

            Assert.IsTrue(fc.IsValid,
                "Should not have found financial Code: " + TESTFINANCIALCLASSCODEAN);
        }
        [Test()]
        public void TestFinancialClassForBlank()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            string blank = String.Empty;
            FinancialClass es = fcb.FinancialClassWith( ACO_FACILITYID,blank );

            Assert.AreEqual(
                blank,
                es.Code,
                "Code  should be blank");

            Assert.AreEqual
                (blank,
                 es.Description,
                 "Description should be blank"
                );

            Assert.IsTrue(
                es.IsValid            
                );
        }

        [Test()]
        public void TestFinancialClassesForFinancialClassTypeId()
        {
            FinancialClassesBrokerProxy broker = new FinancialClassesBrokerProxy();

            Hashtable standardFinancialClasses = broker.FinancialClassesFor(ACO_FACILITYID, STANDARD_FINANCIAL_CLASS_TYPE );
            Assert.IsNotNull( standardFinancialClasses );
            Assert.IsTrue( standardFinancialClasses.Count > 1 );

            Hashtable uninsuredFinancialClasses = broker.FinancialClassesFor( ACO_FACILITYID,UNINSURED_FINANCIAL_CLASS_TYPE );
            Assert.IsNotNull( uninsuredFinancialClasses );
            Assert.IsTrue( uninsuredFinancialClasses.Count > 1 );

            Assert.IsFalse( standardFinancialClasses.Equals( uninsuredFinancialClasses ) );
            Assert.IsFalse( standardFinancialClasses == uninsuredFinancialClasses );

            Hashtable nonsensicalFinancialClasses = broker.FinancialClassesFor(ACO_FACILITYID, NONSENSICAL_FINANCIAL_CLASS_TYPE );
            Assert.IsNotNull( nonsensicalFinancialClasses );
            Assert.IsTrue( nonsensicalFinancialClasses.Count == 0 );
        }

        [Test()]
        public void TestIsUninsured()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc =
                fcb.FinancialClassWith( ACO_FACILITYID,TEST_FINAINCIALCLASS_CODE_UNINSURED );
            Assert.IsTrue( fcb.IsUninsured( ACO_FACILITYID,fc ) );
            fc =
                fcb.FinancialClassWith( ACO_FACILITYID,TESTFINANCIALCLASSCODE );
            Assert.IsTrue( !fcb.IsUninsured(ACO_FACILITYID, fc ) );
        }

        [Test()]
        public void TestIsinsured()
        {
            var financialClassesBrokerProxy = new FinancialClassesBrokerProxy();
            var financialClass = financialClassesBrokerProxy.FinancialClassWith(ACO_FACILITYID, TEST_FINANCIALCLASS_CODE_14);
            Assert.IsTrue(financialClassesBrokerProxy.IsPatientInsured(financialClass));
        } 

        [Test()]
        [Ignore]
        public void TestIsValidFinancialClass()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc =
                fcb.FinancialClassWith( ACO_FACILITYID,TEST_FINAINCIALCLASS_CODE_UNINSURED );
            Assert.IsTrue( fcb.IsValidFinancialClass( fc ) );
            fc =
                fcb.FinancialClassWith( ACO_FACILITYID, TESTFINANCIALCLASSCODEBAD );
            Assert.IsTrue( !fcb.IsValidFinancialClass( fc ) );
        }
        [Test()]
        [Ignore]
        public void TestIsValid_AN_FinancialClass()
        {
            FinancialClassesBrokerProxy fcb = new FinancialClassesBrokerProxy();
            FinancialClass fc =
                fcb.FinancialClassWith(ACO_FACILITYID, TEST_FINAINCIALCLASS_CODE_UNINSURED);
            Assert.IsTrue(fcb.IsValidFinancialClass(fc));
            fc =
                fcb.FinancialClassWith(ACO_FACILITYID, TESTFINANCIALCLASSCODEAN);
            Assert.IsTrue(fcb.IsValidFinancialClass(fc));
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}