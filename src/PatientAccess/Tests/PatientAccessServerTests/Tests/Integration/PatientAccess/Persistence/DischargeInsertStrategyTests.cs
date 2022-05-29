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
    /// Summary description for DischargeInsertStrategyTests.
    /// </summary>
    [TestFixture()]
    public class DischargeInsertStrategyTests
    {
        #region SetUp and TearDown DischargeInsertStrategyTests
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
            Activity currentActivity = 
                new DischargeActivity();
            anAccount.Activity = currentActivity;
            Assert.AreEqual( 4567899,
                             anAccount.AccountNumber,
                             "AccountNumber should be 4567890" );

            patient.MedicalRecordNumber = 56712;
 
            anAccount.Patient = patient;
            anAccount.Facility = facility;
            anAccount.DischargeDate = DateTime.Now;

            anAccount.CodedDiagnoses = new CodedDiagnoses();
            anAccount.CodedDiagnoses.CodedDiagnosises.Add("123.1");
            anAccount.CodedDiagnoses.AdmittingCodedDiagnosises.Add("321.1");

            DischargeInsertStrategy dischargeInsertStrategy 
                = new DischargeInsertStrategy();

            //            transactionKeys = 
            //                new TransactionKeys( 10, 20, 30, 365 );

            dischargeInsertStrategy.TransactionFileId = TRANSACTION_FILE_ID;
            dischargeInsertStrategy.UserSecurityCode = SECURITY_CODE;
            
            ArrayList sqlStrings = 
                dischargeInsertStrategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach( string  sqlString in sqlStrings )
            {
            
                int startPositionOfValues = 
                    sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                int lengthOfValues = 
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray = 
                    sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                Assert.AreEqual(NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray");

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'DI'", ValueArray[1],"Value of APIDID should be 'DI' " );
                Assert.AreEqual( "'$#P@%&'", ValueArray[2],"Value of APRR# should be '$#P@%&' " );
                Assert.AreEqual( "'KEVN'", ValueArray[3],"Value of APSEC2 should be 'KEVN' " );
                Assert.AreEqual( "6", ValueArray[4],"Value of APHSP# should be 6 " );
                Assert.AreEqual( "4567899", ValueArray[5],"Value of APACCT should be 4567899 " );
                //Assert.AreEqual( "112806", ValueArray[6],"Value of APLDD should be 112806 " );
                //  Assert.AreEqual( "1608", ValueArray[7],"Value of APLDT should be 1608 " );
                // Assert.AreEqual( "112806", ValueArray[8],"Value of APTDAT should be 112806 " );
                Assert.AreEqual( "''", ValueArray[9],"Value of APDCOD should be '' " );
                Assert.AreEqual( "'123.1'", ValueArray[10],"Value of APCD01 should be '123.1' " );
                Assert.AreEqual( "''", ValueArray[11],"Value of APCD02 should be '' " );
                Assert.AreEqual( "''", ValueArray[12],"Value of APCD03 should be '' " );
                Assert.AreEqual( "''", ValueArray[13],"Value of APCD04 should be '' " );
                Assert.AreEqual( "''", ValueArray[14],"Value of APCD05 should be '' " );
                Assert.AreEqual( "''", ValueArray[15],"Value of APCD06 should be '' " );
                Assert.AreEqual( "''", ValueArray[16],"Value of APCD07 should be '' " );
                Assert.AreEqual( "''", ValueArray[17],"Value of APCD08 should be '' " );
                Assert.AreEqual( "0", ValueArray[18],"Value of APODR# should be 0 " );
                Assert.AreEqual( "''", ValueArray[19],"Value of APOP01 should be '' " );
                Assert.AreEqual( "''", ValueArray[20],"Value of APOP02 should be '' " );
                Assert.AreEqual( "''", ValueArray[21],"Value of APOP03 should be '' " );
                Assert.AreEqual( "''", ValueArray[22],"Value of APOP04 should be '' " );
                Assert.AreEqual( "0", ValueArray[23],"Value of APOD01 should be 0 " );
                Assert.AreEqual( "0", ValueArray[24],"Value of APOD02 should be 0 " );
                Assert.AreEqual( "0", ValueArray[25],"Value of APOD03 should be 0 " );
                Assert.AreEqual( "0", ValueArray[26],"Value of APOD04 should be 0 " );
                Assert.AreEqual( "0", ValueArray[27],"Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[28],"Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[29],"Value of APLUL# should be 0 " );
                Assert.AreEqual( "'A'", ValueArray[30],"Value of APACFL should be 'A' " );
                //Assert.AreEqual( "160811", ValueArray[31],"Value of APTTME should be 160811 " );
                Assert.AreEqual( "'$#L@%'", ValueArray[32],"Value of APINLG should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[33],"Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[34],"Value of APSWPY should be 0 " );
                Assert.AreEqual( "'321.1'", ValueArray[35],"Value of APRA01 should be '321.1' " );
                Assert.AreEqual( "''", ValueArray[36],"Value of APRA02 should be '' " );
                Assert.AreEqual( "''", ValueArray[37],"Value of APRA03 should be '' " );
                Assert.AreEqual( "''", ValueArray[38],"Value of APRA04 should be '' " );
                Assert.AreEqual( "''", ValueArray[39],"Value of APRA05 should be '' " );
                Assert.AreEqual( "''", ValueArray[40],"Value of APCC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[41],"Value of APCC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[42],"Value of APCC03 should be '' " );
                Assert.AreEqual( "0", ValueArray[43],"Value of APABOR should be 0 " );
                Assert.AreEqual( "''", ValueArray[44],"Value of APABC1 should be '' " );
                Assert.AreEqual( "''", ValueArray[45],"Value of APXMIT should be '' " );
                Assert.AreEqual( "0", ValueArray[46],"Value of APQNUM should be 0 " );
                Assert.AreEqual( "''", ValueArray[47],"Value of APUPRV should be '' " );
                Assert.AreEqual( "''", ValueArray[48],"Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[49],"Value of APZTME should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[50],"Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[51],"Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[52],"Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[53],"Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[54],"Value of APORR3 should be '' " );

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

                IAccountBroker  accountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
                Account account = accountBroker.AccountFor( proxy );
               
                account.Activity = new DischargeActivity();
                DischargeInsertStrategy dischargeInsertStrategy = new DischargeInsertStrategy();
                dischargeInsertStrategy.UpdateColumnValuesUsing( account );

                Assert.IsTrue( true, "Updation of hashtable with values succeeded" );
                ArrayList sqlStrings = 
                    dischargeInsertStrategy.BuildSqlFrom( account, transactionKeys );
                foreach( string  sqlString in sqlStrings )
                {
            
                    int startPositionOfValues = 
                        sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                    int lengthOfValues = 
                        sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                    string[] ValueArray = 
                        sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                    Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray");
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

        private static  Facility facility = null;
        private static  Patient patient = new Patient();
        private static  TransactionKeys transactionKeys = new TransactionKeys();

        #endregion

        #region Constants

        private const string
            FACILITY_CODE = "DEL";

        private const int 
            FACILITY_ID = 900,
            NUMBER_OF_ENTRIES = 57;
        private const string TRANSACTION_FILE_ID = "DI";
        private const string SECURITY_CODE = "KEVN";
        #endregion
    }
}