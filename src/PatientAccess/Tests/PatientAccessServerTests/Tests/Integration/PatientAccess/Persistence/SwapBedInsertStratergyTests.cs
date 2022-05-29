using System;
using System.Collections;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for SwapBedInsertStratergyTest.
    /// </summary>
    [TestFixture()]
    public class SwapBedInsertStratergyTests : AbstractBrokerTests
    {
          
        #region SetUp and TearDown SwapBedInsertStratergyTests

        [TestFixtureSetUp()]
        public static void SetUp()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            facility = facilityBroker.FacilityWith( FACILITY_CODE_DEL );

            CreateUser();
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestBuildSqlFrom()
        {
            Account accountOne = new Account();

            accountOne.AccountNumber = 4567899;
            Activity currentActivity = 
                new TransferBedSwapActivity();
            accountOne.Activity = currentActivity;
            Assert.AreEqual( 
                4567899,
                accountOne.AccountNumber,
                "AccountNumber should be 4567890" );

            patient.MedicalRecordNumber = 56712;

            accountOne.Patient.DriversLicense = 
                new DriversLicense( "72368223828",new State( 
                                                      PersistentModel.NEW_OID,
                                                      DateTime.Now,
                                                      "Texas" ) );

            Assert.AreEqual( "72368223828",
                             accountOne.Patient.DriversLicense.Number );

            accountOne.Patient = patient;
            accountOne.Facility = facility;
        
            Account accountTwo = new Account();

            accountOne.AccountNumber = 4567898;
            Activity currentActivity2 = 
                new TransferBedSwapActivity();
            accountTwo.Activity = currentActivity;
            Assert.AreEqual( 
                4567898,
                accountOne.AccountNumber,
                "AccountNumber should be 4567898" );

            patient.MedicalRecordNumber = 56713;
            
            //Location Information  
            Location i_Location1 = new Location();
            NursingStation i_NursingStation1 = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, DESC1, ROOM1, BED1 );
            i_Location1.NursingStation = i_NursingStation1;
            accountOne.LocationTo = i_Location1;
            Room i_room1 = new Room( PersistentModel.NEW_OID, DateTime.Now, DESC1, CODE1 );
            i_Location1.Room = i_room1;
            accountOne.LocationTo = i_Location1;
            Bed i_Bed1 = new Bed( PersistentModel.NEW_OID, DateTime.Now, DESC1, CODE1 );
            Accomodation i_Accomodation1 = new Accomodation( PersistentModel.NEW_OID,DateTime.Now, DESC1, CODE1 );
            i_Location1.Bed = i_Bed1;
            i_Location1.Bed.Accomodation = i_Accomodation1;
            accountOne.LocationTo = i_Location1;
            accountOne.LocationFrom = i_Location1;

            Location i_Location2 = new Location();
            NursingStation i_NursingStation2 = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, DESC2, ROOM2, BED2 );
            i_Location2.NursingStation = i_NursingStation2;
            accountTwo.LocationTo = i_Location2;
            Room i_room2 = new Room( PersistentModel.NEW_OID, DateTime.Now, DESC2,CODE2 );
            i_Location2.Room = i_room2;
            accountTwo.LocationTo = i_Location2;
            Bed i_Bed2 = new Bed( PersistentModel.NEW_OID, DateTime.Now, DESC2, CODE2 );
            Accomodation i_Accomodation2 = new Accomodation( PersistentModel.NEW_OID,DateTime.Now, DESC2, CODE2 );
            i_Location2.Bed = i_Bed2;
            i_Location2.Bed.Accomodation = i_Accomodation2;
            accountTwo.LocationTo = i_Location2;
            accountTwo.LocationFrom = i_Location2;

            //Hospital Service Code Data
            HospitalService hsv01 = new HospitalService(PersistentModel.NEW_OID,DateTime.Now,"Ambulance","AB");
            HospitalService hsv02 = new HospitalService(PersistentModel.NEW_OID,DateTime.Now,"Intensive Care","IC");
            accountOne.TransferredFromHospitalService = hsv01;
            accountOne.HospitalService = hsv02;
            accountTwo.TransferredFromHospitalService = hsv02;
            accountTwo.HospitalService = hsv01;

            accountTwo.Patient = patient;
            accountTwo.Facility = facility;
 
            SwapBedInsertStrategy swapBedInsertStratergy
                = new SwapBedInsertStrategy();

            accountOne.TransferDate = DateTime.Now;
            //            transactionKeys = 
            //                new TransactionKeys( 10, 20, 30, 365 );
            
            ArrayList sqlStrings = 
                swapBedInsertStratergy.BuildSqlFrom( accountOne, accountTwo, transactionKeys );
            foreach( string sqlString in sqlStrings )
            {
                int startPositionOfValues = 
                    sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                int lengthOfValues = 
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray = 
                    sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                Assert.AreEqual(NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray");

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "''", ValueArray[1],"Value of APIDID should be '' " );
                Assert.AreEqual( "'$#P@%&'", ValueArray[2],"Value of APRR# should be '$#P@%&' " );
                Assert.AreEqual( "''", ValueArray[3],"Value of APSEC2 should be '' " );
                Assert.AreEqual( "6", ValueArray[4],"Value of APHSP# should be 6 " );
                Assert.AreEqual( "4567898", ValueArray[5],"Value of APACT1 should be 4567898 " );
                Assert.AreEqual( "''", ValueArray[6],"Value of APNS1 should be '' " );
                Assert.AreEqual( "10", ValueArray[7],"Value of APRM1 should be 10 " );
                Assert.AreEqual( "'10'", ValueArray[8],"Value of APBD1 should be '10' " );
                Assert.AreEqual( "'AB'", ValueArray[9],"Value of APMSO1 should be 'AB' " );
                Assert.AreEqual( "'IC'", ValueArray[10],"Value of APMSN1 should be 'IC' " );
                Assert.AreEqual( "0", ValueArray[11],"Value of APACT2 should be 0 " );
                Assert.AreEqual( "''", ValueArray[12],"Value of APNS2 should be '' " );
                Assert.AreEqual( "11", ValueArray[13],"Value of APRM2 should be 11 " );
                Assert.AreEqual( "'11'", ValueArray[14],"Value of APBD2 should be '11' " );
                Assert.AreEqual( "'IC'", ValueArray[15],"Value of APMSO2 should be 'IC' " );
                Assert.AreEqual( "'AB'", ValueArray[16],"Value of APMSN2 should be 'AB' " );
                Assert.AreEqual( "0", ValueArray[20],"Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[21],"Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[22],"Value of APLUL# should be 0 " );
                Assert.AreEqual( "''", ValueArray[23],"Value of APACFL should be '' " );
                Assert.AreEqual( "'$#L@%'", ValueArray[25],"Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[26],"Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[27],"Value of APSWPY should be 0 " );
                Assert.AreEqual( "'10'", ValueArray[28],"Value of APACO1 should be '10' " );
                Assert.AreEqual( "'10'", ValueArray[29],"Value of APACN1 should be '10' " );
                Assert.AreEqual( "'11'", ValueArray[30],"Value of APACO2 should be '11' " );
                Assert.AreEqual( "'11'", ValueArray[31],"Value of APACN2 should be '11' " );
                Assert.AreEqual( "''", ValueArray[32],"Value of APXMIT should be '' " );
                Assert.AreEqual( "0", ValueArray[33],"Value of APQNUM should be 0 " );
                Assert.AreEqual( "''", ValueArray[34],"Value of APUPR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[35],"Value of APUPR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[36],"Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[37],"Value of APZTME should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[38],"Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[39],"Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[40],"Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[41],"Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[42],"Value of APORR3 should be '' " );
            }
        }
        [Test()]
        public void TestUpdateColumnValuesUsing()
        {
            try
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

                Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
                DateTime patientDOB = new DateTime( 1965, 01, 13 );
                Patient patient = new Patient();
                patient.Oid = 1723;
                patient.Facility.Oid = 900;
                patient.FirstName = "SONNY";
                patient.LastName = "SADSTORY";
                patient.DateOfBirth = patientDOB;
                patient.Sex = patientSex;
                patient.MedicalRecordNumber = 785138;
                AccountProxy proxy = new AccountProxy( 30015,  
                                                       patient, 
                                                       DateTime.Now, 
                                                       DateTime.Now,
                                                       new VisitType( 0, ReferenceValue.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ),
                                                       facility_ACO,
                                                       new FinancialClass( 299, ReferenceValue.NEW_VERSION, "MEDICARE", "40" ),
                                                       new HospitalService( 0, ReferenceValue.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                       "OL HSV60",
                                                       false );

                IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account accountOne = accountBroker.AccountFor( proxy );
               
                Location location1 = new Location();
                NursingStation nursingStation1 = new NursingStation( PersistentModel.NEW_OID, DateTime.Now, DESC1, ROOM1, BED1 );
                location1.NursingStation = nursingStation1;
                accountOne.LocationTo = location1;
                Room room1 = new Room( PersistentModel.NEW_OID, DateTime.Now, DESC1, CODE1 );
                location1.Room = room1;
                accountOne.LocationTo = location1;
                Bed bed1 = new Bed( PersistentModel.NEW_OID, DateTime.Now, DESC1, CODE1 );
                Accomodation accomodation1 = new Accomodation( PersistentModel.NEW_OID,DateTime.Now, DESC1, CODE1 );
                location1.Bed = bed1;
                location1.Bed.Accomodation = accomodation1;
                accountOne.LocationTo = location1;
                accountOne.LocationFrom = location1;
               
                accountOne.Activity = new TransferBedSwapActivity();
                SwapBedInsertStrategy swapBedInsertStratergy = new SwapBedInsertStrategy();
                swapBedInsertStratergy.UpdateColumnValuesUsing( accountOne, accountOne );

                ArrayList sqlStrings = 
                    swapBedInsertStratergy.BuildSqlFrom( accountOne, accountOne, transactionKeys );
                foreach( string sqlString in sqlStrings )
                {
                    int startPositionOfValues = 
                        sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                    int lengthOfValues = 
                        sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                    string[] ValueArray = 
                        sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                    Assert.AreEqual(NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray");
                }

                Assert.IsTrue( true, "Update of hashtable with values succeeded" );
            }
            catch( Exception ex )
            {
                Assert.Fail( "Update of hashtable with values failed." );
                throw new BrokerException( ex.Message );
            }
        }
        #endregion

        #region Data Elements
        private static  Facility facility = null;

        private static  Patient patient = new Patient();

        private static  TransactionKeys transactionKeys = new TransactionKeys();

        #endregion

        #region Constants
        private const int
            NUMBER_OF_ENTRIES   =   43;
        private const string
            FACILITY_CODE_DEL       = "DEL",
            ROOM1                   = "room1",
            ROOM2                   = "room2",
            BED1                    = "bed1",
            BED2                    = "bed2",
            DESC1                   = "desc1",
            DESC2                   = "desc2",
            CODE1                   = "10",
            CODE2                   = "11";

        private const int FACILITY_ID = 900;

        #endregion
		
    }
}