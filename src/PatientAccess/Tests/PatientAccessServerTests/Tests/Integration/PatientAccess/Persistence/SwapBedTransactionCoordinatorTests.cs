using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for SwapBedTransactionCoordinatorTests.
    /// </summary>
    [TestFixture()]
    public class SwapBedTransactionCoordinatorTests
    {
        #region Constants

        private const string
            PATIENT_F_NAME1 = "BANGALORE",
            PATIENT_L_NAME1 = "OFFSHORE";

        private static readonly string PATIENT_MI1 = string.Empty;

        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN1 = 785673;

        private SocialSecurityNumber
            PATIENT_SSN1    = new SocialSecurityNumber("123456789");

        private Name
            PATIENT_NAME1   = new Name(PATIENT_F_NAME1, PATIENT_L_NAME1, PATIENT_MI1);

        private DateTime
            PATIENT_DOB1    = new DateTime(1981, 12, 31);

        private Gender
            PATIENT_SEX1    = new Gender(1, DateTime.Now, "Male", "M");


        #endregion

        #region SetUp and TearDown SwapBedTransactionCoordinatorTests
        [TestFixtureSetUp()]
        public static void SetUpSwapBedTransactionCoordinatorTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownSwapBedTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        [Category( "Fast" )]
        public void TestTransactionCoordinatorFor()
        {
            Activity currentActivity = 
                new TransferBedSwapActivity();
            TransactionCoordinator swapBedTransactionCoordinator =
                TransactionCoordinator.TransactionCoordinatorFor( currentActivity );
            Assert.IsNotNull( swapBedTransactionCoordinator );
            Assert.AreEqual( typeof( SwapBedTransactionCoordinator ), 
                             swapBedTransactionCoordinator.GetType() );

            TransactionCoordinator nullTransactionCoordinator = 
                TransactionCoordinator.TransactionCoordinatorFor( null );
            Assert.IsNull( nullTransactionCoordinator );
        }

        [Test()]
        public void TestGetInsertStrategy()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = facilityBroker.FacilityWith( "ACO" );
                Patient patientOne = new Patient(
                    PATIENT_OID,
                    Patient.NEW_VERSION,
                    this.PATIENT_NAME1,
                    PATIENT_MRN1,
                    this.PATIENT_DOB1,
                    this.PATIENT_SSN1,
                    this.PATIENT_SEX1,
                    facility
                    );
                //account one
                AccountProxy proxyOne = new AccountProxy( 51581,
                                                          patientOne,
                                                          DateTime.Now,
                                                          DateTime.Now,
                                                          new VisitType( 1, ReferenceValue.NEW_VERSION, VisitType.INPATIENT_DESC, VisitType.INPATIENT ),
                                                          facility,
                                                          new FinancialClass( 20, ReferenceValue.NEW_VERSION, "COMMERCIAL INS", "20" ),
                                                          new HospitalService( PersistentModel.NEW_OID, DateTime.Now, "CORRECTIONAL INPT", "CI" ),
                                                          VisitType.INPATIENT_DESC,
                                                          false );
                IAccountBroker accountBrokerOne = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account accountOne = accountBrokerOne.AccountFor( proxyOne );
                accountOne.Activity = new TransferBedSwapActivity();
                accountOne.Facility.Oid = facility.Oid;

                //location one
                Location location = new Location();
                NursingStation nursingStation1 = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, "6N", "6N" );
                location.NursingStation = nursingStation1;
                accountOne.LocationTo = location;
                Room room1 = new Room( PersistentModel.NEW_OID, DateTime.Now, "610", "610" );
                location.Room = room1;
                accountOne.LocationTo = location;
                Bed bed1 = new Bed( PersistentModel.NEW_OID, DateTime.Now, "A", "A" );
                Accomodation accomodation1 = new Accomodation( PersistentModel.NEW_OID, DateTime.Now, "01", "01" );
                location.Bed = bed1;
                location.Bed.Accomodation = accomodation1;
                accountOne.LocationTo = location;
                accountOne.LocationFrom = location;

                accountOne.TransferDate = DateTime.Now;
                HospitalService hsv01 = new HospitalService( PersistentModel.NEW_OID, DateTime.Now, "CORRECTIONAL INPT", "CI" );
                accountOne.TransferredFromHospitalService = hsv01;
                HospitalService hsv02 = new HospitalService( PersistentModel.NEW_OID, DateTime.Now, "INPATIENT TRAUMA", "37" );
                accountOne.HospitalService = hsv02;

                SwapBedTransactionCoordinator swapBedTransactionCoordinator =
                    new SwapBedTransactionCoordinator( accountOne, accountOne );
                swapBedTransactionCoordinator.NumberOfInsurances = 0;
                swapBedTransactionCoordinator.NumberOfNonInsurances = 0;
                swapBedTransactionCoordinator.NumberOfOtherRecs = 0;
                swapBedTransactionCoordinator.Account = accountOne;
                swapBedTransactionCoordinator.AccountTwo = accountOne;
                swapBedTransactionCoordinator.AppUser = accountOne.Activity.AppUser;
                
                SqlBuilderStrategy[] strategy = swapBedTransactionCoordinator.InsertStrategies;

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
	
    }
}