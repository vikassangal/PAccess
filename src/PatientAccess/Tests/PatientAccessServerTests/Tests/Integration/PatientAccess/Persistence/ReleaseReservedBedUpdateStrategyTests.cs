using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class ReleaseReservedBedUpdateStrategyTests
    {
        #region SetUp and TearDown ReleaseReservedBedUpdateStrategyTests
        [TestFixtureSetUp()]
        public static void SetUp()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            facility = facilityBroker.FacilityWith( FACILITY_CODE );
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestBuildSqlFrom()
        {
            Account anAccount = new Account();
            anAccount.AccountNumber = 4567899;
            Activity currentActivity = new ReleaseReservedBedActivity();
            anAccount.Activity = currentActivity;
            patient.MedicalRecordNumber = 56712;
            anAccount.Patient = patient;
            anAccount.Facility = facility;

            //Location Info.
            NursingStation nursingStation = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
            Room room = new Room( ReferenceValue.NEW_OID, DateTime.Now, "610", "610" );
            Bed bed = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
            Location location = new Location();
            location.NursingStation = nursingStation;
            location.Room = room;
            location.Bed = bed;
            anAccount.Location = location;

            ReleaseReservedBedUpdateStrategy strategy = 
                new ReleaseReservedBedUpdateStrategy();

            ArrayList sqlStrings = 
                strategy.BuildSqlFrom( anAccount, null );
            string expectedSqlStatement = 
                "UPDATE HPADLRP SET LRFLG = ''  WHERE LRHSP# = 900 AND LRNS = '6N' AND LRROOM = 610 AND LRBED = 'A'";

            string sqlString = sqlStrings[0].ToString();

            Assert.AreEqual( expectedSqlStatement, sqlString );
        }
        [Test()]
        public void TestInitializeColumnValues()
        {
            try
            {
                
                ReleaseReservedBedUpdateStrategy strategy = 
                    new ReleaseReservedBedUpdateStrategy();
                Assert.IsTrue( true, "Initialization of hashtable with default values succeeded" );
            }
            catch( Exception ex )
            {
                Assert.Fail( "Initialization of hashtable with default values failed." );
                throw new BrokerException( ex.Message );
            }
        }

        [Test()]
        public void TestUpdateColumnValuesUsing()
        {
            try
            {
                Account anAccount = new Account();
                anAccount.AccountNumber = 4567899;
                Activity currentActivity = new ReleaseReservedBedActivity();
                anAccount.Activity = currentActivity;
                patient.MedicalRecordNumber = 56712;
                anAccount.Patient = patient;
                anAccount.Facility = facility;
              
                NursingStation nursingStation = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
                Room room = new Room( ReferenceValue.NEW_OID, DateTime.Now, "610", "610" );
                Bed bed = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
                Location location = new Location();
                location.NursingStation = nursingStation;
                location.Room = room;
                location.Bed = bed;
                anAccount.Location = location;

                anAccount.Activity = new ReleaseReservedBedActivity();

                ReleaseReservedBedUpdateStrategy strategy = 
                    new ReleaseReservedBedUpdateStrategy();
                strategy.UpdateColumnValuesUsing( anAccount );

                Assert.IsTrue( true, "Updation of hashtable with values succeeded" );
            }
            catch( Exception ex )
            {
                Assert.Fail( "Updation of hashtable with values failed." );
                throw new BrokerException( ex.Message );
            }
        }
        #endregion

        #region Data Elements

        private static  Facility facility = null;

        private static  Patient patient = new Patient();
        #endregion

        #region Constants

        private const string
            FACILITY_CODE = "ACO";
        #endregion

    }
}