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
    /// Summary description for EditRecurringDischargeInsertStrategyTests.
    /// </summary>
    [TestFixture()]
    public class EditRecurringDischargeInsertStrategyTests
    {
        #region SetUp and TearDown EditRecurringDischargeInsertStrategyTests
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
                new EditRecurringDischargeActivity();
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

            EditRecurringDischargeInsertStrategy strategy
                = new EditRecurringDischargeInsertStrategy();

            //            transactionKeys = 
            //                new TransactionKeys( 10, 20, 30, 365 );

            strategy.TransactionFileId = TRANSACTION_FILE_ID;
            strategy.UserSecurityCode = SECURITY_CODE;
            
            ArrayList sqlStrings = 
                strategy.BuildSqlFrom( anAccount, transactionKeys );
            foreach( string  sqlString in sqlStrings )
            {
            
                int startPositionOfValues = 
                    sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                int lengthOfValues = 
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray = 
                    sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                Assert.AreEqual(51, ValueArray.Length,"Wrong number of entries in ValueArray");
                
                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'EO'", ValueArray[1],"Value of APIDID should be 'EO' " );
                Assert.AreEqual( "'&*@&Q%'", ValueArray[2],"Value of APRR#  should be '&*@&Q%' " );
                Assert.AreEqual( "'KEVN'", ValueArray[3],"Value of APSEC2 should be 'KEVN' " );
                Assert.AreEqual( "6", ValueArray[4],"Value of APHSP#  should be 6 " );
                Assert.AreEqual( "4567899", ValueArray[5],"Value of APACCT should be 4567899 " );
                Assert.AreEqual( "''", ValueArray[6],"Value of APDIAG should be '' " );
                Assert.AreEqual( "'123.1'", ValueArray[7],"Value of APCD01 should be '123.1' " );
                Assert.AreEqual( "''", ValueArray[8],"Value of APCD02 should be '' " );
                Assert.AreEqual( "''", ValueArray[9],"Value of APCD03 should be '' " );
                Assert.AreEqual( "''", ValueArray[10],"Value of APCD04 should be '' " );
                Assert.AreEqual( "''", ValueArray[11],"Value of APCD05 should be '' " );
                Assert.AreEqual( "''", ValueArray[12],"Value of APCD06 should be '' " );
                Assert.AreEqual( "''", ValueArray[13],"Value of APCD07 should be '' " );
                Assert.AreEqual( "''", ValueArray[14],"Value of APCD08 should be '' " );
                //Assert.AreEqual( "113006", ValueArray[15],"Value of APTDAT should be 113006 " );
                Assert.AreEqual( "0", ValueArray[16],"Value of APLML should be 0 " );
                Assert.AreEqual( "0", ValueArray[17],"Value of APLMD should be 0 " );
                Assert.AreEqual( "0", ValueArray[18],"Value of APLUL#  should be 0 " );
                Assert.AreEqual( "'C'", ValueArray[19],"Value of APACFL should be 'C' " );
                // Assert.AreEqual( "111541", ValueArray[20],"Value of APTTME  should be 111541 " );
                Assert.AreEqual( "'$#L@%'", ValueArray[21],"Value of APINLG  should be '$#L@%' " );
                Assert.AreEqual( "''", ValueArray[22],"Value of APBYPS should be '' " );
                Assert.AreEqual( "0", ValueArray[23],"Value of APSWPY  should be 0 " );
                Assert.AreEqual( "'321.1'", ValueArray[24],"Value of APRA01 should be '321.1' " );
                Assert.AreEqual( "''", ValueArray[25],"Value of APRA02 should be '' " );
                Assert.AreEqual( "''", ValueArray[26],"Value of APRA03 should be '' " );
                Assert.AreEqual( "''", ValueArray[27],"Value of APRA04 should be '' " );
                Assert.AreEqual( "''", ValueArray[28],"Value of APRA05 should be '' " );
                Assert.AreEqual( "''", ValueArray[29],"Value of APCC01 should be '' " );
                Assert.AreEqual( "''", ValueArray[30],"Value of APCC02 should be '' " );
                Assert.AreEqual( "''", ValueArray[31],"Value of APCC03 should be '' " );
                Assert.AreEqual( "''", ValueArray[32],"Value of APOP01 should be '' " );
                Assert.AreEqual( "''", ValueArray[33],"Value of APOP02 should be '' " );
                Assert.AreEqual( "''", ValueArray[34],"Value of APOP03 should be '' " );
                Assert.AreEqual( "''", ValueArray[35],"Value of APOP04 should be '' " );
                Assert.AreEqual( "0", ValueArray[36],"Value of APOD01  should be 0 " );
                Assert.AreEqual( "0", ValueArray[37],"Value of APOD02  should be 0 " );
                Assert.AreEqual( "0", ValueArray[38],"Value of APOD03  should be 0 " );
                Assert.AreEqual( "0", ValueArray[39],"Value of APOD04  should be 0 " );
                Assert.AreEqual( "''", ValueArray[40],"Value of APDCOD should be '' " );
                //Assert.AreEqual( "113006", ValueArray[41],"Value of APLDD  should be 113006 " );
                //Assert.AreEqual( "1115", ValueArray[42],"Value of APLDT  should be 1115 " );
                Assert.AreEqual( "''", ValueArray[43],"Value of APUPRV should be '' " );
                Assert.AreEqual( "''", ValueArray[44],"Value of APZDTE should be '' " );
                Assert.AreEqual( "''", ValueArray[45],"Value of APZTME should be '' " );
                Assert.AreEqual( "'PACCESS'", ValueArray[46],"Value of APWSIR  should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[47],"Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[48],"Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[49],"Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[50],"Value of APORR3 should be '' " );

            }
        }
        [Test()]
        public void TestInitializeColumnValues()
        {
            try
            {
                EditRecurringDischargeInsertStrategy strategy
                    = new EditRecurringDischargeInsertStrategy();

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
               
                anAccount.Activity = new EditRecurringDischargeActivity();

                EditRecurringDischargeInsertStrategy strategy
                    = new EditRecurringDischargeInsertStrategy();

                strategy.UpdateColumnValuesUsing( anAccount );

                Assert.IsTrue( true, "Update of hashtable with values succeeded" );

                ArrayList sqlStrings = 
                    strategy.BuildSqlFrom( anAccount, transactionKeys );
                foreach( string  sqlString in sqlStrings )
                {
                    int startPositionOfValues = 
                        sqlString.IndexOf( "(", sqlString.IndexOf(")" ) ) + 1;
                    int lengthOfValues = 
                        sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                    string[] ValueArray = 
                        sqlString.Substring( startPositionOfValues , lengthOfValues ).Split( ',' );

                    Assert.AreEqual(51, ValueArray.Length,"Wrong number of entries in ValueArray");
                }
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
        private static  TransactionKeys transactionKeys = new TransactionKeys();

        #endregion

        #region Constants

        private const string
            FACILITY_CODE = "DEL";
        private const string TRANSACTION_FILE_ID = "EO";
        private const string SECURITY_CODE = "KEVN";
        #endregion
    }
}