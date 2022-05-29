using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class WorkListPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown WorkListBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpWorkListBrokerTests()
        {
            i_WorklistBroker = BrokerFactory.BrokerOfType<IWorklistBroker>();
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_AccountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownWorkListBrokerTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        [Ignore()] //"transaction Boundries no longer support this"
        public void TestSaveRemainingActions()
        {
            Facility facility = i_FacilityBroker.FacilityWith(900);
            Patient patient = new Patient();
            patient.Oid = 0;
            patient.Facility = facility;
            //patient.MedicalRecordNumber = 785424 ; 
            patient.MedicalRecordNumber = 790880;
            IList list = i_AccountBroker.AccountsFor(patient);

            foreach( AccountProxy ap in list )
            {
                Account anAccount = ap.AsAccount();
                anAccount.Activity = new MaintenanceActivity();

                i_WorklistBroker.SaveRemainingActions( anAccount );         
            }
        }

        [Test()]
        [Ignore]
        //very long execution
        public void TestProcessAllWorklistsForValidFacility()
        {
            Facility facility = i_FacilityBroker.FacilityWith(900);

            i_WorklistBroker.ProcessAllWorklistsFor( facility );
        }

        [Test()]
        //      [Ignore("Commented - breaking build")]
        public void TestgetRemainingActionsFor()
        {
            Facility facility = i_FacilityBroker.FacilityWith(900);
            Patient patient = new Patient();
            patient.Oid = 0;
            patient.Facility = facility;
            patient.MedicalRecordNumber = 787797 ; 
            IList list = i_AccountBroker.AccountsFor(patient);
                  
            foreach( AccountProxy ap in list )
            {
                Hashtable actions = i_WorklistBroker.RemainingActionsFor( 
                    ap.AccountNumber, patient.MedicalRecordNumber, ap.Patient.Facility.Oid );
                Account anAccount = ap.AsAccount();
                anAccount.Activity = new  MaintenanceActivity();     
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IWorklistBroker i_WorklistBroker = null;
        private static IFacilityBroker i_FacilityBroker = null;
        private static IAccountBroker  i_AccountBroker = null;
        #endregion
    }
}