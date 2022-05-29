using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for MSPPBARBrokerTests.
    /// </summary>

    //TODO: Create XML summary comment for MSPPBARBrokerTests
    [TestFixture()]
    public class MSPPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown MSPPBARBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpMSPPBARBrokerTests()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();
            i_ACOFacility = facilityBroker.FacilityWith(900);

            i_mspBroker = BrokerFactory.BrokerOfType<IMSPBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownMSPPBARBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestReadMSP()
        {
            Account anAccount = new Account();
            anAccount.AccountNumber = 53314;
            Patient patient = new Patient();
            patient.MedicalRecordNumber = 785425;
            anAccount.Patient = patient;
            anAccount.Facility = i_ACOFacility;
            MedicareSecondaryPayor msp = i_mspBroker.MSPFor(anAccount);

            Assert.IsNotNull( msp, "No MSP read" );
            Assert.AreEqual(
                typeof(AgeEntitlement), 
                msp.MedicareEntitlement.GetType(),
                "Wrong Type read"
                );

            AgeEntitlement ageEnt = (AgeEntitlement)msp.MedicareEntitlement;
            Assert.AreEqual( "Y", ageEnt.GroupHealthPlanCoverage.Code,
                             "Wrong Group health Plan indicator" );
            Assert.AreEqual( "N", ageEnt.GHPLimitExceeded.Code,
                             "Wrong value for more that 20 employees flag" );

            Employment employment = ageEnt.SpouseEmployment;
            Assert.IsNotNull( employment,"No Spouse Employment Found" );
            Assert.AreEqual( "TACO BUENO",employment.Employer.Name,
                             "Wrong Employer Found" );
            Assert.AreEqual( new DateTime(0), employment.RetiredDate,
                             "Wrong Spouses retirement date" );
        }

        [Test()]
        public void TestESRDProgram()
        {
            i_mspBroker = BrokerFactory.BrokerOfType<IMSPBroker >();
            Account anAccount = new Account();
            anAccount.AccountNumber = 53231;
            Patient patient = new Patient();
            patient.MedicalRecordNumber = 785785;
            anAccount.Patient = patient;
            anAccount.Facility = i_ACOFacility;
            MedicareSecondaryPayor msp = i_mspBroker.MSPFor(anAccount);

            Assert.IsNotNull( msp, "No MSP read" );                

            Assert.AreEqual( typeof(ESRDEntitlement), msp.MedicareEntitlement.GetType(),
                             "Wrong Type read" );
            ESRDEntitlement esrdEnt = (ESRDEntitlement)msp.MedicareEntitlement;
            Assert.AreEqual( new DateTime(1990,1,1),esrdEnt.TransplantDate, 
                             "Wrong Transplant Date" );
            Assert.AreEqual( new DateTime(1992,1,1), esrdEnt.DialysisDate,
                             "Wrong Dialysis Date" );
            Assert.AreEqual( new DateTime(1995,1,1), esrdEnt.DialysisTrainingStartDate, 
                             "Wrong Training start date" );
            Assert.AreEqual( "Y",esrdEnt.GroupHealthPlanCoverage.Code, 
                             "Wrong Group Health plan flag" );
            Assert.AreEqual( "Y",esrdEnt.KidneyTransplant.Code, 
                             "Wrong Group Health plan flag" );
            Assert.AreEqual( "Y",esrdEnt.DialysisTreatment.Code, 
                             "Wrong Group Health plan flag" );
        }

        [Test()]
        public void TestDisabilityProgram()
        {
            i_mspBroker = BrokerFactory.BrokerOfType<IMSPBroker >();
            Account anAccount = new Account();
            anAccount.AccountNumber = 52704;
            Patient patient = new Patient();
            patient.MedicalRecordNumber = 785749;
            anAccount.Patient = patient;
            anAccount.Facility = i_ACOFacility;
            MedicareSecondaryPayor msp = i_mspBroker.MSPFor(anAccount);

            Assert.IsNotNull( msp,"No MSP read" );                

            Assert.AreEqual( typeof(DisabilityEntitlement), msp.MedicareEntitlement.GetType(),
                             "Wrong Type read" );
            DisabilityEntitlement disEnt = (DisabilityEntitlement)msp.MedicareEntitlement;
            Assert.AreEqual( "Y",disEnt.GroupHealthPlanCoverage.Code,
                             "Wrong Group Health Flag" );
            Assert.AreEqual( "Y",disEnt.GHPLimitExceeded.Code, 
                             "Wrong Group Health Max employee number Flag" );
            Assert.IsNull( disEnt.FamilyMemberEmployment, 
                           "Should not have found employer for this Questionare" );
        }

        [Test()]
        public void TestGetMSP2StartDate()
        {
            i_mspBroker = BrokerFactory.BrokerOfType<IMSPBroker >();
            DateTime startDate = i_mspBroker.GetMSP2StartDate();
            Assert.AreEqual( startDate, new DateTime( 2006, 11, 01 ), "MSP Start date is incorrect" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static Facility i_ACOFacility = null;
        private static IMSPBroker i_mspBroker = null;
        #endregion
    }
}