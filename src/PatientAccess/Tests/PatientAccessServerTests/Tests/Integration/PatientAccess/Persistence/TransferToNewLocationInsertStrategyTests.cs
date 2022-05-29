using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for TransferPatientToNewLocationInsertStrategyTest.
    /// </summary>
    [TestFixture()]
    public class TransferToNewLocationInsertStrategyTest
    {
        #region SetUp and TearDown TransferPatientToNewLocationInsertStrategyTest
        [TestFixtureSetUp()]
        public static void SetUp()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();

            facility = facilityBroker.FacilityWith( FACILITY_CODE );
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestBuildSqlFrom()
        {
            Account anAccount = new Account();
            anAccount.AccountNumber = 4567899;
            Activity currentActivity = new TransferActivity();
            anAccount.Activity = currentActivity;
            patient.MedicalRecordNumber = 56712;
            anAccount.Patient = patient;
            anAccount.Facility = facility;
            anAccount.TransferDate = DateTime.Now;

            //Location Info.
            NursingStation nursingStationFrom = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
            Room roomFrom = new Room( ReferenceValue.NEW_OID, DateTime.Now, "610", "610" );
            Bed bedFrom = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
            Location location = new Location();
            location.NursingStation = nursingStationFrom;
            location.Room = roomFrom;
            location.Bed = bedFrom;
            anAccount.LocationFrom = location;
            anAccount.HospitalService = new HospitalService(ReferenceValue.NEW_OID, DateTime.Now, "Emergency", HospitalService.EMERGENCY_ROOM);
            anAccount.HospitalService = new HospitalService(ReferenceValue.NEW_OID, DateTime.Now, "surgical", HospitalService.SURG_DIAG_OBS_PT_BED);


            NursingStation nursingStationTo = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
            Room roomTo = new Room( ReferenceValue.NEW_OID, DateTime.Now, "611", "611" );
            Bed bedTo = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
            Location newLocation = new Location();
            newLocation.NursingStation = nursingStationTo;
            newLocation.Room = roomTo;
            newLocation.Bed = bedTo;
            anAccount.LocationTo = newLocation;

            //Accomodation Info.
            Accomodation accomodation = new Accomodation( ReferenceValue.NEW_OID, DateTime.Now, "01", "01" );
            location.Bed.Accomodation = accomodation;
            newLocation.Bed.Accomodation = accomodation;

            Physician physician = new Physician(123,ReferenceValue.NEW_VERSION,1,1,1,1,1,1);
            physician.FirstName = "Doctor";
            physician.LastName = "Bob";
            physician.UPIN = "UP234";
            physician.NationalID = "NI234";
            Address addr = new Address("123 Dr Place",string.Empty,"Healthland",new ZipCode( "11223" ),
                                       new State(1,ReferenceValue.NEW_VERSION,"Texas","TX"),
                                       new Country(1,ReferenceValue.NEW_VERSION,"United States","USA"));
            PhoneNumber pn = new PhoneNumber ("111","2222222");
            physician.AddContactPoint(new ContactPoint(addr,pn,
                                                       new PhoneNumber(),new EmailAddress(),TypeOfContactPoint.NewBillingContactPointType()));
            anAccount.AddPhysicianWithRole(PhysicianRole.Attending(),physician);
             
            // initialize user with values explicitly
            User appUser = User.GetCurrent();
            appUser.PBAREmployeeID = "PAUSRT03";
            appUser.WorkstationID = string.Empty;
            appUser.UserID = "PAUSRT03";
            IConditionCodeBroker conditionCodeBroker = BrokerFactory.BrokerOfType<IConditionCodeBroker>();
            ConditionCode conditionCode = conditionCodeBroker.ConditionCodeWith( 
                FACILITY_ID, ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED );
            ConditionCode conditionCode1 = conditionCodeBroker.ConditionCodeWith( FACILITY_ID, ConditionCode.CONDITIONCODE_DOB_OVER_100Y );

            anAccount.ConditionCodes.Add( conditionCode );
            anAccount.ConditionCodes.Add( conditionCode1 );

            transactionKeys = new TransactionKeys( 12, 24, 36, 0, 365 );
           
            TransferToNewLocationInsertStrategy transferPatientToNewLocationInsertStrategy 
                = new TransferToNewLocationInsertStrategy();
            //            transactionKeys = 
            //                new TransactionKeys( 10, 20, 30, 365 );
            ArrayList sqlStrings = 
                transferPatientToNewLocationInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach( string sqlString in  sqlStrings )
            {
                int startPositionOfValues = 
                    sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                int lengthOfValues = 
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray = 
                    sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of parameters in Array");

                Assert.AreEqual( " ''", ValueArray[0],"Value of APIDWS should be  '' " );
                Assert.AreEqual( "''", ValueArray[1],"Value of APIDID should be '' " );
                Assert.AreEqual( "'$#P@%&'", ValueArray[2],"Value of APRR# should be '$#P@%&' " );
                Assert.AreEqual( "''", ValueArray[3],"Value of APSEC2 should be '' " );
                Assert.AreEqual( "900", ValueArray[4],"Value of APHSP# should be 900 " );
                Assert.AreEqual( "4567899", ValueArray[5],"Value of APACCT should be 4567899 " );
                //  Assert.AreEqual( "112806", ValueArray[6],"Value of APTRDT should be 112806 " );
                //Assert.AreEqual( "1648", ValueArray[7],"Value of APTRTM should be 1648 " );
                //  Assert.AreEqual( "112806", ValueArray[8],"Value of APTDAT should be 112806 " );
                Assert.AreEqual( "'6N'", ValueArray[9],"Value of APFNS should be '6N' " );
                Assert.AreEqual( "610", ValueArray[10],"Value of APFRM should be 610 " );
                Assert.AreEqual( "'A'", ValueArray[11],"Value of APFBD should be 'A' " );
                Assert.AreEqual( "'65'", ValueArray[12],"Value of APFMSV should be '65' " );
                Assert.AreEqual( "'6N'", ValueArray[13],"Value of APTNS should be '6N' " );
                Assert.AreEqual( "611", ValueArray[14],"Value of APTRM should be 611 " );
                Assert.AreEqual( "'A'", ValueArray[15],"Value of APTBD should be 'A' " );
                Assert.AreEqual( "'59'", ValueArray[16],"Value of APTMSV should be '59' " );
                Assert.AreEqual( "0", ValueArray[17],"Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[18],"Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[19],"Value of APLUL# should be 0 " );
                Assert.AreEqual( "'A'", ValueArray[20],"Value of APACFL should be 'A' " );
                //  Assert.AreEqual( "111835", ValueArray[21],"Value of APTTME should be 111835 " );
                Assert.AreEqual( "'$#L@%'", ValueArray[22],"Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[23],"Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[24],"Value of APSWPY should be 0 " );
                Assert.AreEqual( "123", ValueArray[25],"Value of APADR# should be 123 " );
                Assert.AreEqual( "''", ValueArray[26],"Value of APPTYP should be '' " );
                Assert.AreEqual( "'01'", ValueArray[27],"Value of APFACC should be '01' " );
                Assert.AreEqual( "'01'", ValueArray[28],"Value of APTACM should be '01' " );
                Assert.AreEqual( "''", ValueArray[29],"Value of APXMIT should be '' " );
                Assert.AreEqual( "0", ValueArray[30],"Value of APQNUM should be 0 " );
                Assert.AreEqual( "''", ValueArray[31],"Value of APUPRV should be '' " );
                Assert.AreEqual( "''", ValueArray[32],"Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[33],"Value of APZTME should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[34],"Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[35],"Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[36],"Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[37],"Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[38],"Value of APORR3 should be '' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "'", ValueArray[39], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_PRIVATE_ROOM_MEDICALLY_REQUIRED + "' " );
                Assert.AreEqual( "'" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "'", ValueArray[40], "Value of APCI02 should be '" + ConditionCode.CONDITIONCODE_DOB_OVER_100Y + "' " );
                Assert.AreEqual( "''", ValueArray[41], "Value of APCI04 should be '' " );
                Assert.AreEqual( "''", ValueArray[42], "Value of APCI05 should be '' " );
                Assert.AreEqual( "''", ValueArray[43], "Value of APCI06 should be '' " );
                Assert.AreEqual( "''", ValueArray[44], "Value of APCI07 should be '' " );
                
            }
        }
        [Test()]
        public void TestInitializeColumnValues()
        {
            try
            {
                TransferToNewLocationInsertStrategy transferPatientToNewLocationInsertStrategy = new TransferToNewLocationInsertStrategy();
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
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();
                Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

                Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
                DateTime patientDOB = new DateTime( 1965, 01, 13 );
                Patient patient = new Patient();
                patient.Oid = 1723;
                patient.Facility = facility_ACO;
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

                IAccountBroker accountBroker = BrokerFactory.BrokerOfType<IAccountBroker >();
                Account account = accountBroker.AccountFor( proxy );
              
                NursingStation nursingStationFrom = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
                Room roomFrom = new Room( ReferenceValue.NEW_OID, DateTime.Now, "610", "610" );
                Bed bedFrom = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
                Location location = new Location();
                location.NursingStation = nursingStationFrom;
                location.Room = roomFrom;
                location.Bed = bedFrom;
                account.LocationFrom = location;

                NursingStation nursingStationTo = new NursingStation( ReferenceValue.NEW_OID, DateTime.Now, "6N", "6N" );
                Room roomTo = new Room( ReferenceValue.NEW_OID, DateTime.Now, "611", "611" );
                Bed bedTo = new Bed( ReferenceValue.NEW_OID, DateTime.Now, "A","A" );
                Location newLocation = new Location();
                newLocation.NursingStation = nursingStationTo;
                newLocation.Room = roomTo;
                newLocation.Bed = bedTo;
                account.LocationTo = newLocation;

                Accomodation accomodation = new Accomodation( ReferenceValue.NEW_OID, DateTime.Now, "01", "01" );
                location.Bed.Accomodation = accomodation;
                newLocation.Bed.Accomodation = accomodation;

                account.Activity = new TransferActivity();
                account.TransferDate = DateTime.Now;
                TransferToNewLocationInsertStrategy transferToNewLocationInsertStrategy = 
                    new TransferToNewLocationInsertStrategy();
                transferToNewLocationInsertStrategy.UpdateColumnValuesUsing( account );

                Assert.IsTrue( true, "Update of hashtable with values succeeded" );
                ArrayList sqlStrings = 
                    transferToNewLocationInsertStrategy.BuildSqlFrom( account, transactionKeys );
                foreach( string sqlString in  sqlStrings )
                {
                    int startPositionOfValues = 
                        sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                    int lengthOfValues = 
                        sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                    string[] ValueArray = 
                        sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                    Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of parameters in Array");
                }
            }
            catch( Exception ex )
            {
                Assert.Fail( "Update of hashtable with values failed." );
                throw new BrokerException( ex.Message );
            }
        }
        #endregion

        #region Data Elements

        private static Facility facility = null;
        private static Patient patient = new Patient();
        private static TransactionKeys transactionKeys;

        #endregion

        #region Constants
        
        private const int
            NUMBER_OF_ENTRIES   = 46;

        private const string
            FACILITY_CODE           = "ACO";

        private const int FACILITY_ID = 900;

        #endregion
    }
}