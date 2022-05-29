using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
   
    public class RemoteAccountPBARBrokerTests : RemoteAbstractBrokerTests
    {
        #region Constants

        #endregion

        #region SetUp and TearDown AccountBrokerTests

        [TestFixtureSetUp()]
        public static void SetUpAccountBrokerTests()
        {
            facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            facility = facilityBroker.FacilityWith( 900 );
        }

        [TestFixtureTearDown()]
        public static void TearDownAccountBrokerTests()
        {           
        }
        #endregion

        #region Test Methods

        [Test()]
        [Description( "VeryLongExecution" )]
        public void TestAccountForRemote()
        {
            Patient patient = new Patient();
            patient.Oid = -1L;
            patient.Facility = facility;
            patient.MedicalRecordNumber = 785147;
            AccountProxy proxy = new AccountProxy( 52019, patient, DateTime.Now, DateTime.Now,
                                                   new VisitType( 0, ReferenceValue.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), facility,
                                                   new FinancialClass( 299, ReferenceValue.NEW_VERSION, "MEDICADE", "40" ),
                                                   new HospitalService( 0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                   "OL HSV60", false );

            IAccountBroker remoteAB = BrokerFactory.BrokerOfType< IAccountBroker >();
            Account account = remoteAB.AccountFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );
        }

        #endregion   
    
        #region Data elements

        private static IFacilityBroker facilityBroker;
        private static Facility facility;

        #endregion
    }
}