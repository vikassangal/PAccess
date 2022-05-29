using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class FinancialClassesPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        private const string
            TESTFINANCIALCLASSCODE          = "03",
            TESTFINANCIALCLASSCODEBAD       = "RR",
            TESTFINANCIALCLASSCODEAN        = "ZZ",
            TEST_FINAINCIALCLASS_CODE_UNINSURED = "70",
            TESTFINANCIALCLASSDESCRIPTION   = "CHARITY CARE";

        private const long
            STANDARD_FINANCIAL_CLASS_TYPE = 1L,
            UNINSURED_FINANCIAL_CLASS_TYPE = 2L;
        #endregion

        #region SetUp and TearDown FinancialClassesBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpFinancialClassesBrokerTests()
        {
            i_Broker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownFinancialClassesBrokerTests()
        {
            
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestFinancialClassesAll()
        {
            ArrayList table = (ArrayList)i_Broker.AllTypesOfFinancialClasses( ACO_FACILITYID );
            Assert.IsTrue( table != null, "There are no financial Classes" );

        }

        [Test()]
        public void TestSingleFinancialClass()
        {
            FinancialClass fc = i_Broker.FinancialClassWith( ACO_FACILITYID, TESTFINANCIALCLASSCODE );

            Assert.IsTrue( fc != null,
                           "Did not find financial Code: " + TESTFINANCIALCLASSCODE );
            Assert.IsTrue( fc.Description.Trim().Equals(TESTFINANCIALCLASSDESCRIPTION),
                           "Did not find the proper financial Class description" );
        }

        [Test()]
        [Ignore]
        public void TestSingleFinancialClassBadFC()
        {
            FinancialClass fc = i_Broker.FinancialClassWith( ACO_FACILITYID, TESTFINANCIALCLASSCODEBAD );

            Assert.IsFalse( fc.IsValid ,
                            "Should not have found financial Code: " + TESTFINANCIALCLASSCODEBAD );

        }

        [Test()]
        [Ignore]
        public void TestSingleFinancialClassANFC()
        {
            FinancialClass fc = i_Broker.FinancialClassWith(ACO_FACILITYID, TESTFINANCIALCLASSCODEAN);

            Assert.IsTrue(fc.IsValid,
                "Should not have found financial Code: " + TESTFINANCIALCLASSCODEAN);

        }
        [Test()]
        public void TestFinancialClassForBlank()
        {
            string blank = String.Empty;
            FinancialClass es = i_Broker.FinancialClassWith( ACO_FACILITYID, blank );

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
        public void FinancialClassesForFinancialClassTypeId()
        {
            Hashtable standardFinancialClasses = i_Broker.FinancialClassesFor( ACO_FACILITYID, STANDARD_FINANCIAL_CLASS_TYPE );
            Assert.IsNotNull( standardFinancialClasses );
            Assert.IsTrue( standardFinancialClasses.Count > 1 );

            Hashtable uninsuredFinancialClasses = i_Broker.FinancialClassesFor( ACO_FACILITYID, UNINSURED_FINANCIAL_CLASS_TYPE );
            Assert.IsNotNull( uninsuredFinancialClasses );
            Assert.IsTrue( uninsuredFinancialClasses.Count > 1 );

            Assert.IsFalse( standardFinancialClasses.Equals( uninsuredFinancialClasses ) );
            Assert.IsFalse( standardFinancialClasses == uninsuredFinancialClasses );
        }

        [Test()]
        public void TestIsUninsured()
        {
            FinancialClass fc =
                i_Broker.FinancialClassWith( ACO_FACILITYID, TEST_FINAINCIALCLASS_CODE_UNINSURED );
            Assert.IsTrue( i_Broker.IsUninsured( ACO_FACILITYID, fc ) );
        }
        [Test()]
        public void TestIsStandard()
        {
            FinancialClass fc = i_Broker.FinancialClassWith( ACO_FACILITYID, TESTFINANCIALCLASSCODE );
            Assert.IsTrue( !i_Broker.IsUninsured( ACO_FACILITYID, fc ) );
        }
        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestIsNull()
        {
            FinancialClass fc = i_Broker.FinancialClassWith( ACO_FACILITYID, null );
        }
       
        [Test()]
        [Ignore]
        public void TestIsValidFinancialClass()
        {
            FinancialClass fc =
                i_Broker.FinancialClassWith( ACO_FACILITYID, TEST_FINAINCIALCLASS_CODE_UNINSURED );
            Assert.IsTrue( i_Broker.IsValidFinancialClass( fc ) );
            fc =
                i_Broker.FinancialClassWith( ACO_FACILITYID, TESTFINANCIALCLASSCODEBAD );
            Assert.IsTrue( !i_Broker.IsValidFinancialClass( fc ) );
        }
        [Test()]
        [Ignore]
        public void TestIsValid_AN_FinancialClass()
        {
            FinancialClass fc =
                i_Broker.FinancialClassWith(ACO_FACILITYID, TEST_FINAINCIALCLASS_CODE_UNINSURED);
            Assert.IsTrue(i_Broker.IsValidFinancialClass(fc));
            fc =
                i_Broker.FinancialClassWith(ACO_FACILITYID, TESTFINANCIALCLASSCODEAN);
            Assert.IsTrue(i_Broker.IsValidFinancialClass(fc));
        }
        [Test()]
        public void TestIsInValidFinancialClass()
        {
            Assert.IsFalse( i_Broker.IsValidFinancialClass( null ), "Should be invalid financial class." );
        }

        [Test()]
        public void TestIsPatientInsured_For_FC02_Should_Be_True()
        {
            var financialClass = new FinancialClass {Code = "02"};
            Assert.IsTrue( i_Broker.IsPatientInsured( financialClass ), "Should be valid financial class for insured patient" ); 
        }

        [Test()]
        public void TestIsPatientInsured_For_FC03_Should_Be_False()
        {
            var financialClass = new FinancialClass {Code = "03"};
            Assert.IsFalse( i_Broker.IsPatientInsured( financialClass ), "Should be invalid financial class for insured patient" );
        }

        [Test()]
        public void TestIsPatientInsured_For_FC45_Should_Be_True()
        {
            var financialClass = new FinancialClass {Code = "45"};
            Assert.IsTrue(i_Broker.IsPatientInsured(financialClass), "Should be valid financial class for insured patient");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IFinancialClassesBroker i_Broker = null;
        #endregion
    }
}