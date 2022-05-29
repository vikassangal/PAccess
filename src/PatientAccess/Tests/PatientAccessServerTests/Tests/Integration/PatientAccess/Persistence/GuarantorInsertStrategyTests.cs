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
    [TestFixture()]
    public class GuarantorInsertStrategyTests : AbstractBrokerTests
    {
        #region SetUp and TearDown GuarantorInsertStrategyTests

        [TestFixtureSetUp()]
        public static void SetUpGuarantorInsertStrategyTests()
        {
            CreateUser();

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            facility = facilityBroker.FacilityWith( FACILITY_CODE );
        }

        [TestFixtureTearDown()]
        public static void TearDownGuarantorInsertStrategyTests()
        {

        }

        #endregion

        #region Test Methods

        [Test()]
        //[Ignore("Check with DBA team on test data issues - Krishna.")]
        public void TestBuildSqlFromForPreRegistration()
        {
            Account anAccount = new Account();
            Name GUARANTOR_NAME =
                new Name( GUARANTOR_F_NAME, GUARANTOR_L_NAME, GUARANTOR_MI );
            Guarantor guarantor =
                new Guarantor(
                    PersistentModel.NEW_OID,
                    PersistentModel.NEW_VERSION,
                    GUARANTOR_NAME );

            anAccount.AccountNumber = 4567899;
            Activity currentActivity =
                new PreRegistrationActivity();
            anAccount.Activity = currentActivity;
            Assert.AreEqual(
                4567899,
                anAccount.AccountNumber, "AccountNumber should be 4567890" );

            this.patient.MedicalRecordNumber = 56712;

            ContactPoint contactPointMOB =
                new ContactPoint( null, this.phoneNumberMOB, null, this.typeOfContactPointMOB );
            ContactPoint contactPointMLG =
                new ContactPoint( this.addressM, this.phoneNumberMLG, this.emailAddress, this.typeOfContactPointMLG );
            ContactPoint contactPointEMP =
                new ContactPoint( this.addressE, this.phoneNumberEMP, null, this.typeOfContactPointEMP );

            anAccount.Patient.DriversLicense =
                new DriversLicense( "72368223828", new State(
                                                       PersistentModel.NEW_OID,
                                                       DateTime.Now,
                                                       "Texas" ) );

            Assert.AreEqual( "72368223828",
                             anAccount.Patient.DriversLicense.Number );

            RelationshipType relType =
                new RelationshipType( PersistentModel.NEW_OID, DateTime.Now, "Spouse", "2" );

            guarantor.AddContactPoint( contactPointMOB );
            guarantor.AddContactPoint( contactPointMLG );
            //guarantor.AddContactPoint( contactPointEMP );

            foreach( ContactPoint cp in anAccount.Guarantor.ContactPoints )
            {
                Assert.AreEqual(
                    cp.TypeOfContactPoint,
                    TypeOfContactPoint.NewMobileContactPointType(),
                    "contact point should be mobile contact point" );
            }

            this.employment.EmployeeID = "123789";
            this.employment.Occupation = "SALES";
            this.employment.Status = this.empSt;

            this.empr.Name = "US POSTAL SERVICE";
            this.employment.Employer = this.empr;
            this.empr.PartyContactPoint = contactPointEMP;

            guarantor.Employment = this.employment;

            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                "Primary",
                primary.Description );

            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                "Secondary",
                secondary.Description );

            CommercialCoverage coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            GovernmentMedicareCoverage coverage2 = new GovernmentMedicareCoverage();
            coverage2.CoverageOrder = secondary;
            coverage2.Oid = 2;

            Insurance insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual(
                1,
                insurance.Coverages.Count );

            insurance.AddCoverage( coverage2 );
            Assert.AreEqual(
                2,
                insurance.Coverages.Count );

            Insured insured1 =
                new Insured(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, this.INSURED_NAME1 );
            Insured insured2 = new Insured(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, this.INSURED_NAME2 );
            insured1.GroupNumber = "1";

            insured2.GroupNumber = "2";
            Assert.AreEqual(
                "2",
                insured2.GroupNumber, "GroupNumber should be 2" );

            coverage1.Insured = insured1;
            coverage2.Insured = insured2;

            anAccount.GuarantorIs( guarantor, relType );
            anAccount.Patient = this.patient;
            anAccount.Facility = facility;
            anAccount.Insurance = insurance;

            anAccount.IsNew = true;
            GuarantorInsertStrategy guarantorInsertStrategy =
                new GuarantorInsertStrategy();
            guarantorInsertStrategy.UserSecurityCode = "KEVN";

            //            transactionKeys = 
            //                new TransactionKeys( 10, 20, 30, 365 );

            ArrayList sqlStrings =
                guarantorInsertStrategy.BuildSqlFrom( anAccount, this.transactionKeys );
            foreach( string sqlString in sqlStrings )
            {

                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'GM'", ValueArray[1], "Value of APIDID should be 'GM'" );
                Assert.AreEqual( "'$#G%&*'", ValueArray[2], "Value of APRR# should be '$#G%&*'" );
                Assert.AreEqual( "'KEVN'", ValueArray[3], "Value of APSEC2 should be 'KEVN'" );
                Assert.AreEqual( "6", ValueArray[4], "Value of APHSP# should be 6" );
                Assert.AreEqual( "'$#P@%&'", ValueArray[5], "Value of APPREC should be '$#P@%&'" );
                Assert.AreEqual( "4567899", ValueArray[6], "Value of APACCT should be 4567899" );
                Assert.AreEqual( "56712", ValueArray[7], "Value of APMRC# should be 56712" );
                Assert.AreEqual( "4567899", ValueArray[8], "Value of APGAR# should be 4567899" );
                Assert.AreEqual( "0", ValueArray[9], "Value of APPGAR should be 0" );
                Assert.AreEqual( "'SADSTORY'", ValueArray[10], "Value of APGLNM should be 'SADSTORY'" );

                // firstname for insurance should now contain the middle initial appended to the end of the firstname 
                // and preceded by a space - See OTD 36226
                string firstNameAndMI = "'SONNY " + ValueArray[100];
                string removeApost = firstNameAndMI.Remove(7, 1);
                Assert.AreEqual(removeApost, ValueArray[11], "Value of APGFNM should be 'SONNY'");
                //Assert.AreEqual("'SONNY'", ValueArray[11], "Value of APGFNM should be 'SONNY'");

                Assert.AreEqual( "''", ValueArray[12], "Value of APGAD1 should be ''" );
                Assert.AreEqual( "''", ValueArray[13], "Value of APGAD2 should be ''" );
                Assert.AreEqual( "'Austin'", ValueArray[14], "Value of APGCIT should be 'Austin'" );
                Assert.AreEqual( "60594", ValueArray[15], "Value of APGZIP should be 60594" );
                Assert.AreEqual( "'123'", ValueArray[16], "Value of APGCNT should be '123'" );
                Assert.AreEqual( "972", ValueArray[17], "Value of APGACD should be 972" );
                Assert.AreEqual( "6903564", ValueArray[18], "Value of APGPH# should be 6903564" );
                Assert.AreEqual( "'US POSTAL SERVICE'", ValueArray[19], "Value of APENM should be 'US POSTAL SERVICE'" );
                Assert.AreEqual( "'335 Nicholson Dr.#303'", ValueArray[20], "Value of APEADR should be '335 Nicholson Dr.#303'" );
                Assert.AreEqual( "'Austin'", ValueArray[21], "Value of APECIT should be 'Austin'" );
                Assert.AreEqual( "'TX'", ValueArray[22], "Value of APESTE should be 'TX'" );
                Assert.AreEqual( "60504", ValueArray[23], "Value of APEZIP should be 60504" );
                Assert.AreEqual( "5000", ValueArray[24], "Value of APEZP4 should be 5000" );
                Assert.AreEqual( "610", ValueArray[25], "Value of APEACD should be 610" );
                Assert.AreEqual( "4545454", ValueArray[26], "Value of APEPH# should be 4545454" );
                Assert.AreEqual( "'SALES'", ValueArray[27], "Value of APGOCC should be 'SALES'" );
                Assert.AreEqual( "0", ValueArray[28], "Value of APFR01 should be 0" );
                Assert.AreEqual( "0", ValueArray[29], "Value of APFR02 should be 0" );
                Assert.AreEqual( "0", ValueArray[30], "Value of APFR03 should be 0" );
                Assert.AreEqual( "'1'", ValueArray[31], "Value of APGP01 should be '1'" );
                Assert.AreEqual( "'2'", ValueArray[32], "Value of APGP02 should be '2'" );
                Assert.AreEqual( "''", ValueArray[33], "Value of APGP03 should be ''" );
                Assert.AreEqual( "''", ValueArray[34], "Value of APPO01 should be ''" );
                Assert.AreEqual( "''", ValueArray[35], "Value of APPO02 should be ''" );
                Assert.AreEqual( "''", ValueArray[36], "Value of APPO03 should be ''" );
                Assert.AreEqual( "'HAPPY'", ValueArray[37], "Value of APSB01 should be 'HAPPY'" );
                Assert.AreEqual( "'SOMETHING'", ValueArray[38], "Value of APSB02 should be 'SOMETHING'" );
                Assert.AreEqual( "''", ValueArray[39], "Value of APSB03 should be ''" );
                Assert.AreEqual( "''", ValueArray[40], "Value of APRL01 should be ''" );
                Assert.AreEqual( "''", ValueArray[41], "Value of APRL02 should be ''" );
                Assert.AreEqual( "''", ValueArray[42], "Value of APRL03 should be ''" );
                Assert.AreEqual( "''", ValueArray[43], "Value of APNST should be ''" );
                Assert.AreEqual( "''", ValueArray[44], "Value of APNOTE should be ''" );
                Assert.AreEqual( "'TX'", ValueArray[45], "Value of APGSTE should be 'TX'" );
                Assert.AreEqual( "5000", ValueArray[46], "Value of APGZP4 should be 5000" );
                Assert.AreEqual( "123", ValueArray[47], "Value of APGCNY should be 123" );
                Assert.AreEqual( "''", ValueArray[48], "Value of APGSSN should be ''" );
                Assert.AreEqual( "'123789'", ValueArray[49], "Value of APGEID should be '123789'" );
                Assert.AreEqual( "'1'", ValueArray[50], "Value of APGESC should be  '1'" );
                Assert.AreEqual( "0", ValueArray[51], "Value of APFR04 should be 0" );
                Assert.AreEqual( "0", ValueArray[52], "Value of APFR05 should be 0" );
                Assert.AreEqual( "0", ValueArray[53], "Value of APFR06 should be 0" );
                Assert.AreEqual( "''", ValueArray[54], "Value of APGP04 should be ''" );
                Assert.AreEqual( "''", ValueArray[55], "Value of APGP05 should be ''" );
                Assert.AreEqual( "''", ValueArray[56], "Value of APGP06 should be ''" );
                Assert.AreEqual( "''", ValueArray[57], "Value of APPO04 should be ''" );
                Assert.AreEqual( "''", ValueArray[58], "Value of APPO05 should be ''" );
                Assert.AreEqual( "''", ValueArray[59], "Value of APPO06 should be ''" );
                Assert.AreEqual( "''", ValueArray[60], "Value of APSB04 should be ''" );
                Assert.AreEqual( "''", ValueArray[61], "Value of APSB05 should be ''" );
                Assert.AreEqual( "''", ValueArray[62], "Value of APSB06 should be ''" );
                Assert.AreEqual( "''", ValueArray[63], "Value of APRL04 should be ''" );
                Assert.AreEqual( "''", ValueArray[64], "Value of APRL05 should be ''" );
                Assert.AreEqual( "''", ValueArray[65], "Value of APRL06 should be ''" );
                Assert.AreEqual( "0", ValueArray[66], "Value of APLML should be 0" );
                Assert.AreEqual( "0", ValueArray[67], "Value of APLMD should be 0" );
                Assert.AreEqual( "0", ValueArray[68], "Value of APLUL# should be 0" );
                Assert.AreEqual( "'A'", ValueArray[69], "Value of APACFL should be 'A'" );
                //Assert.AreEqual( "' '", ValueArray[70],"Value of APTTME should be ''" );
                Assert.AreEqual( "'$#L@%'", ValueArray[71], "Value of APINLG should be '$#L@%'" );
                Assert.AreEqual( "''", ValueArray[72], "Value of APBYPS should be ''" );
                Assert.AreEqual( "0", ValueArray[73], "Value of APSWPY should be 0" );
                Assert.AreEqual( "''", ValueArray[74], "Value of APDRL# should be ''" );
                Assert.AreEqual( "''", ValueArray[75], "Value of APGLOE should be ''" );
                Assert.AreEqual("'00000000'", ValueArray[76], "Value of APUN should be ‘00000000’");
                Assert.AreEqual( "'Y'", ValueArray[77], "Value of APGPSM should be 'Y'" );
                Assert.AreEqual( "'FC@YAHOO.COM'", ValueArray[78], "Value of APGEML should be ''" );
                Assert.AreEqual( "''", ValueArray[79], "Value of APGLR should be ''" );
                Assert.AreEqual( "''", ValueArray[80], "Value of APGLRO should be ''" );
                Assert.AreEqual( "''", ValueArray[81], "Value of APIN01 should be ''" );
                Assert.AreEqual( "''", ValueArray[82], "Value of APIN02 should be ''" );
                Assert.AreEqual( "''", ValueArray[83], "Value of APIN03 should be ''" );
                Assert.AreEqual( "''", ValueArray[84], "Value of APIN04 should be ''" );
                Assert.AreEqual( "''", ValueArray[85], "Value of APIN05 should be ''" );
                Assert.AreEqual( "''", ValueArray[86], "Value of APIN06 should be ''" );
                //Assert.AreEqual( "''", ValueArray[86],"Value of APTDAT should be ''" );
                Assert.AreEqual( "''", ValueArray[88], "Value of APCLRK should be ''" );
                Assert.AreEqual( "''", ValueArray[89], "Value of APZDTE should be ''" );
                Assert.AreEqual( "''", ValueArray[90], "Value of APZTME should be ''" );
                Assert.AreEqual( "'60594'", ValueArray[91], "Value of APGZPA should be '60594'" );
                Assert.AreEqual( "'5000'", ValueArray[92], "Value of APGZ4A should be '5000'" );
                Assert.AreEqual( "'USA'", ValueArray[93], "Value of APGCUN should be 'USA'" );
                Assert.AreEqual( "'60504'", ValueArray[94], "Value of APEZPA should be '60504'" );
                Assert.AreEqual( "'5000'", ValueArray[95], "Value of APEZ4A should be '5000'" );
                Assert.AreEqual( "'USA'", ValueArray[96], "Value of APECUN should be 'USA'" );
                Assert.AreEqual( "''", ValueArray[97], "Value of APLAST should be ''" );
                Assert.AreEqual( "'56712'", ValueArray[98], "Value of APOMR# should be '56712'" );
                Assert.AreEqual( "''", ValueArray[99], "Value of APAPP# should be ''" );
                Assert.AreEqual( "'A'", ValueArray[100], "Value of APGMI should be 'A'" );
                Assert.AreEqual( "''", ValueArray[101], "Value of APGSEX should be ''" );
                Assert.AreEqual( "'6106903412'", ValueArray[102], "Value of APGCPH should be '6106903412'" );
                Assert.AreEqual( "'PACCESS'", ValueArray[103], "Value of APWSIR should be 'PACCESS' " );
                Assert.AreEqual( "''", ValueArray[104], "Value of APSECR should be '' " );
                Assert.AreEqual( "''", ValueArray[105], "Value of APORR1 should be '' " );
                Assert.AreEqual( "''", ValueArray[106], "Value of APORR2 should be '' " );
                Assert.AreEqual( "''", ValueArray[107], "Value of APORR3 should be '' " );
                Assert.AreEqual("'234 MulHolland Drive'", ValueArray[109], "Value of APGAD1E1 should be '234 MulHolland Drive' ");
                Assert.AreEqual("'#1'", ValueArray[110], "Value of APGAD1E2 should be '#1' ");
            }
        }

        [Test()]
        public void TestInitializeColumnValues()
        {
            try
            {
                GuarantorInsertStrategy guarantorInsertStrategy = new GuarantorInsertStrategy();
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
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility facility_ACO = facilityBroker.FacilityWith( FACILITY_ID );

                Gender patientSex = new Gender( 2, DateTime.Now, "Male", "M" );
                DateTime patientDOB = new DateTime( 1965, 01, 13 );
                Patient patient = new Patient();
                patient.Oid = 1723;
                patient.Facility.Oid = 900;
                patient.FirstName = "SONNY";
                patient.LastName = "SADSTORY";
                patient.MiddleInitial = "A";
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
                Account account = accountBroker.AccountFor( proxy );

                account.Activity = new PreRegistrationActivity();
                GuarantorInsertStrategy guarantorInsertStrategy = new GuarantorInsertStrategy();
                guarantorInsertStrategy.UpdateColumnValuesUsing( account );

                Assert.IsTrue( true, "Update of hashtable with values succeeded" );

                ArrayList sqlStrings =
                    guarantorInsertStrategy.BuildSqlFrom( account, this.transactionKeys );
                foreach( string sqlString in sqlStrings )
                {
                    string l_sqlString = sqlString.Replace( "INSERT INTO HPADAPGM( ", string.Empty );
                    int startPositionOfValues =
                        l_sqlString.IndexOf( "(", l_sqlString.IndexOf( ")" ) ) + 1;
                    string[] ValueArray =
                        l_sqlString.Substring( 0, startPositionOfValues ).Split( ',' );

                    Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );
                }
            }
            catch( Exception ex )
            {
                Assert.Fail( "Updation of hashtable with values failed." );
                throw new BrokerException( ex.Message );
            }
        }
        [Test()]
        //[Ignore("Check with DBA team on test data issues - Krishna.")]
        public void TestBuildSqlFromForPreDischarge()
        {
            Account anAccount = new Account();
            Name GUARANTOR_NAME =
                new Name( GUARANTOR_F_NAME, GUARANTOR_L_NAME, GUARANTOR_MI );
            Guarantor guarantor =
                new Guarantor(
                    PersistentModel.NEW_OID,
                    PersistentModel.NEW_VERSION,
                    GUARANTOR_NAME );

            anAccount.AccountNumber = 4567899;
            Activity currentActivity =
                new PreDischargeActivity();
            anAccount.Activity = currentActivity;
            Assert.AreEqual(
                4567899,
                anAccount.AccountNumber,
                "AccountNumber should be 4567890" );

            this.patient.MedicalRecordNumber = 56712;

            ContactPoint contactPointMOB =
                new ContactPoint( null, this.phoneNumberMOB, null, this.typeOfContactPointMOB );
            ContactPoint contactPointMLG =
                new ContactPoint( this.addressM, this.phoneNumberMLG, this.emailAddress, this.typeOfContactPointMLG );
            ContactPoint contactPointEMP =
                new ContactPoint( this.addressE, this.phoneNumberEMP, null, this.typeOfContactPointEMP );

            anAccount.Patient.DriversLicense =
                new DriversLicense( "72368223828", new State(
                                                       PersistentModel.NEW_OID,
                                                       DateTime.Now,
                                                       "Texas" ) );

            Assert.AreEqual( "72368223828",
                             anAccount.Patient.DriversLicense.Number );

            RelationshipType relType =
                new RelationshipType( PersistentModel.NEW_OID, DateTime.Now, "Spouse", "2" );

            guarantor.AddContactPoint( contactPointMOB );
            guarantor.AddContactPoint( contactPointMLG );
            //guarantor.AddContactPoint( contactPointEMP );

            foreach( ContactPoint cp in anAccount.Guarantor.ContactPoints )
            {
                Assert.AreEqual(
                    cp.TypeOfContactPoint,
                    TypeOfContactPoint.NewMobileContactPointType(),
                    "contact point should be mobile contact point" );
            }

            this.employment.EmployeeID = "123789";
            this.employment.Occupation = "SALES";
            this.employment.Status = this.empSt;

            this.empr.Name = "US POSTAL SERVICE";
            this.employment.Employer = this.empr;
            this.empr.PartyContactPoint = contactPointEMP;

            guarantor.Employment = this.employment;

            guarantor.DriversLicense = new DriversLicense( "12345678901234567", new State( "TX" ) );

            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual(
                "Primary",
                primary.Description );

            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual(
                "Secondary",
                secondary.Description );

            CommercialCoverage coverage1 = new CommercialCoverage();
            coverage1.CoverageOrder = primary;
            coverage1.Oid = 1;

            GovernmentMedicareCoverage coverage2 = new GovernmentMedicareCoverage();
            coverage2.CoverageOrder = secondary;
            coverage2.Oid = 2;

            Insurance insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual(
                1,
                insurance.Coverages.Count );

            insurance.AddCoverage( coverage2 );
            Assert.AreEqual(
                2,
                insurance.Coverages.Count );

            Insured insured1 =
                new Insured(
                    PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, this.INSURED_NAME1 );
            Insured insured2 = new Insured(
                PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, this.INSURED_NAME2 );
            insured1.GroupNumber = "1";

            insured2.GroupNumber = "2";
            Assert.AreEqual( "2",
                             insured2.GroupNumber,
                             "GroupNumber should be 2" );

            coverage1.Insured = insured1;
            coverage2.Insured = insured2;

            anAccount.GuarantorIs( guarantor, relType );
            anAccount.Patient = this.patient;
            anAccount.Facility = facility;
            anAccount.Insurance = insurance;

            GuarantorInsertStrategy guarantorInsertStrategy =
                new GuarantorInsertStrategy();
            guarantorInsertStrategy.UserSecurityCode = "KEVN";
            guarantorInsertStrategy.LastTransactionInGroup = "Y";

            this.transactionKeys =
                new TransactionKeys( 10, 20, 30, 0, 365 );

            ArrayList sqlStrings =
                guarantorInsertStrategy.BuildSqlFrom( anAccount, this.transactionKeys );
            foreach( string sqlString in sqlStrings )
            {
                int startPositionOfValues =
                    sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) + 1;
                int lengthOfValues =
                    sqlString.Length - sqlString.IndexOf( "(", sqlString.IndexOf( ")" ) ) - 2;
                string[] ValueArray =
                    sqlString.Substring( startPositionOfValues, lengthOfValues ).Split( ',' );

                Assert.AreEqual( NUMBER_OF_ENTRIES, ValueArray.Length, "Wrong number of entries in ValueArray" );

                Assert.AreEqual( " ''", ValueArray[0], "Value of APIDWS should be ''" );
                Assert.AreEqual( "'GM'", ValueArray[1], "Value of APIDID should be GM" );
                Assert.AreEqual( "'$#G%&*'", ValueArray[2], "Value of APRR# should be '$#G%&*'" );
                Assert.AreEqual( "'KEVN'", ValueArray[3], "Value of APSEC2 should be 'KEVN'" );
                Assert.AreEqual( "6", ValueArray[4], "Value of APHSP# should be 6" );
                Assert.AreEqual( "'$#P@%&'", ValueArray[5], "Value of APPREC should be '$#P@%&'" );
                Assert.AreEqual( "4567899", ValueArray[6], "Value of APACCT should be 4567899" );
                Assert.AreEqual( "56712", ValueArray[7], "Value of APMRC# should be 56712" );
                Assert.AreEqual( "4567899", ValueArray[8], "Value of APGAR# should be 4567899" );
                Assert.AreEqual( "0", ValueArray[9], "Value of APPGAR should be 0" );
                Assert.AreEqual( "'SADSTORY'", ValueArray[10], "Value of APGLNM should be 'SADSTORY'" );

                // firstname for insurance should now contain the middle initial appended to the end of the firstname 
                // and preceded by a space - See OTD 36226
                string firstNameAndMI = "'SONNY " + ValueArray[100] ;
                string removeApost = firstNameAndMI.Remove(7, 1);
                Assert.AreEqual(removeApost, ValueArray[11], "Value of APGFNM should be 'SONNY'");
                //Assert.AreEqual( "'SONNY'", ValueArray[11], "Value of APGFNM should be 'SONNY'" );
                
                Assert.AreEqual( "''", ValueArray[12], "Value of APGAD1 should be ''" );
                Assert.AreEqual( "''", ValueArray[13], "Value of APGAD2 should be ''" );
                Assert.AreEqual( "'Austin'", ValueArray[14], "Value of APGCIT should be 'Austin'" );
                Assert.AreEqual( "60594", ValueArray[15], "Value of APGZIP should be 60594" );
                Assert.AreEqual( "'123'", ValueArray[16], "Value of APGCNT should be '123'" );
                Assert.AreEqual( "972", ValueArray[17], "Value of APGACD should be 972" );
                Assert.AreEqual( "6903564", ValueArray[18], "Value of APGPH# should be 6903564" );
                Assert.AreEqual( "'US POSTAL SERVICE'", ValueArray[19], "Value of APENM should be 'US POSTAL SERVICE'" );
                Assert.AreEqual( "'335 Nicholson Dr.#303'", ValueArray[20], "Value of APEADR should be '335 Nicholson Dr.#303'" );
                Assert.AreEqual( "'Austin'", ValueArray[21], "Value of APECIT should be 'Austin'" );
                Assert.AreEqual( "'TX'", ValueArray[22], "Value of APESTE should be 'TX'" );
                Assert.AreEqual( "60504", ValueArray[23], "Value of APEZIP should be 60504" );
                Assert.AreEqual( "5000", ValueArray[24], "Value of APEZP4 should be 5000" );
                Assert.AreEqual( "610", ValueArray[25], "Value of APEACD should be 610" );
                Assert.AreEqual( "4545454", ValueArray[26], "Value of APEPH# should be 4545454" );
                Assert.AreEqual( "'SALES'", ValueArray[27], "Value of APGOCC should be 'SALES'" );
                Assert.AreEqual( "0", ValueArray[28], "Value of APFR01 should be 0" );
                Assert.AreEqual( "0", ValueArray[29], "Value of APFR02 should be 0" );
                Assert.AreEqual( "0", ValueArray[30], "Value of APFR03 should be 0" );
                Assert.AreEqual( "'1'", ValueArray[31], "Value of APGP01 should be '1'" );
                Assert.AreEqual( "'2'", ValueArray[32], "Value of APGP02 should be '2'" );
                Assert.AreEqual( "''", ValueArray[33], "Value of APGP03 should be ''" );
                Assert.AreEqual( "''", ValueArray[34], "Value of APPO01 should be ''" );
                Assert.AreEqual( "''", ValueArray[35], "Value of APPO02 should be ''" );
                Assert.AreEqual( "''", ValueArray[36], "Value of APPO03 should be ''" );
                Assert.AreEqual( "'HAPPY'", ValueArray[37], "Value of APSB01 should be 'HAPPY'" );
                Assert.AreEqual( "'SOMETHING'", ValueArray[38], "Value of APSB02 should be 'SOMETHING'" );
                Assert.AreEqual( "''", ValueArray[39], "Value of APSB03 should be ''" );
                Assert.AreEqual( "''", ValueArray[40], "Value of APRL01 should be ''" );
                Assert.AreEqual( "''", ValueArray[41], "Value of APRL02 should be ''" );
                Assert.AreEqual( "''", ValueArray[42], "Value of APRL03 should be ''" );
                Assert.AreEqual( "''", ValueArray[43], "Value of APNST should be ''" );
                Assert.AreEqual( "''", ValueArray[44], "Value of APNOTE should be ''" );
                Assert.AreEqual( "'TX'", ValueArray[45], "Value of APGSTE should be 'TX'" );
                Assert.AreEqual( "5000", ValueArray[46], "Value of APGZP4 should be 5000" );
                Assert.AreEqual( "123", ValueArray[47], "Value of APGCNY should be 123" );
                Assert.AreEqual( "''", ValueArray[48], "Value of APGSSN should be ''" );
                Assert.AreEqual( "'123789'", ValueArray[49], "Value of APGEID should be '123789' " );
                Assert.AreEqual( "'1'", ValueArray[50], "Value of APGESC should be '1' " );
                Assert.AreEqual( "0", ValueArray[51], "Value of APFR04 should be 0" );
                Assert.AreEqual( "0", ValueArray[52], "Value of APFR05 should be 0" );
                Assert.AreEqual( "0", ValueArray[53], "Value of APFR06 should be 0" );
                Assert.AreEqual( "''", ValueArray[54], "Value of APGP04 should be ''" );
                Assert.AreEqual( "''", ValueArray[55], "Value of APGP05 should be ''" );
                Assert.AreEqual( "''", ValueArray[56], "Value of APGP06 should be ''" );
                Assert.AreEqual( "''", ValueArray[57], "Value of APPO04 should be ''" );
                Assert.AreEqual( "''", ValueArray[58], "Value of APPO05 should be '' " );
                Assert.AreEqual( "''", ValueArray[59], "Value of APPO06 should be ''" );
                Assert.AreEqual( "''", ValueArray[60], "Value of APSB04 should be '' " );
                Assert.AreEqual( "''", ValueArray[61], "Value of APSB05 should be ''" );
                Assert.AreEqual( "''", ValueArray[62], "Value of APSB06 should be ''" );
                Assert.AreEqual( "''", ValueArray[63], "Value of APRL04 should be ''" );
                Assert.AreEqual( "''", ValueArray[64], "Value of APRL05 should be ''" );
                Assert.AreEqual( "''", ValueArray[65], "Value of APRL06 should be ''" );
                Assert.AreEqual( "0", ValueArray[66], "Value of APLML should be 0" );
                Assert.AreEqual( "0", ValueArray[67], "Value of APLMD should be 0" );
                Assert.AreEqual( "0", ValueArray[68], "Value of APLUL# should be 0" );
                Assert.AreEqual( "'C'", ValueArray[69], "Value of APACFL should be 'C'" );
                //Assert.AreEqual( "''", ValueArray[70],"Value of APTTME should be ''" );
                Assert.AreEqual( "'$#L@%'", ValueArray[71], "Value of APINLG should be '$#L@%'" );
                Assert.AreEqual( "''", ValueArray[72], "Value of APBYPS should be ''" );
                Assert.AreEqual( "0", ValueArray[73], "Value of APSWPY should be 0" );
                Assert.AreEqual( "'123456789012345TX'", ValueArray[74], "Value of APDRL# should be '123456789012345TX'" );
                Assert.AreEqual( "''", ValueArray[75], "Value of APGLOE should be ''" );
                Assert.AreEqual("'00000000'", ValueArray[76], "Value of APUN should be ‘00000000’");
                Assert.AreEqual( "'Y'", ValueArray[77], "Value of APGPSM should be 'Y'" );
                Assert.AreEqual( "'FC@YAHOO.COM'", ValueArray[78], "Value of APGEML should be ''" );
                Assert.AreEqual( "''", ValueArray[79], "Value of APGLR should be ''" );
                Assert.AreEqual( "''", ValueArray[80], "Value of APGLRO should be ''" );
                Assert.AreEqual( "''", ValueArray[81], "Value of APIN01 should be '' " );
                Assert.AreEqual( "''", ValueArray[82], "Value of APIN02 should be ''" );
                Assert.AreEqual( "''", ValueArray[83], "Value of APIN03 should be ''" );
                Assert.AreEqual( "''", ValueArray[84], "Value of APIN04 should be ''" );
                Assert.AreEqual( "''", ValueArray[85], "Value of APIN05 should be ''" );
                Assert.AreEqual( "''", ValueArray[86], "Value of APIN06 should be ''" );
                //Assert.AreEqual( "''", ValueArray[87],"Value of APTDAT should be ''" );
                Assert.AreEqual( "''", ValueArray[88], "Value of APCLRK should be ''" );
                Assert.AreEqual( "''", ValueArray[89], "Value of APZDTE should be ''" );
                Assert.AreEqual( "''", ValueArray[90], "Value of APZTME should be ''" );
                Assert.AreEqual( "'60594'", ValueArray[91], "Value of APGZPA should be '60594'" );
                Assert.AreEqual( "'5000'", ValueArray[92], "Value of APGZ4A should be '5000'" );
                Assert.AreEqual( "'USA'", ValueArray[93], "Value of APGCUN should be 'USA'" );
                Assert.AreEqual( "'60504'", ValueArray[94], "Value of APEZPA should be '60504'" );
                Assert.AreEqual( "'5000'", ValueArray[95], "Value of APEZ4A should be '5000'" );
                Assert.AreEqual( "'USA'", ValueArray[96], "Value of APECUN should be 'USA'" );
                Assert.AreEqual( "'Y'", ValueArray[97], "Value of APLAST should be 'Y'" );
                Assert.AreEqual( "'56712'", ValueArray[98], "Value of APOMR# should be '56712'" );
                Assert.AreEqual( "''", ValueArray[99], "Value of APAPP# should be ''" );
                Assert.AreEqual( "'A'", ValueArray[100], "Value of APGMI should be 'A'" );
                Assert.AreEqual( "''", ValueArray[101], "Value of APGSEX should be ''" );
                Assert.AreEqual( "'6106903412'", ValueArray[102], "Value of APGCPH should be '6106903412'" );
                Assert.AreEqual( "'PACCESS'", ValueArray[103], "Value of APWSIR should be 'PACCESS'" );
                Assert.AreEqual( "''", ValueArray[104], "Value of APSECR should be ''" );
                Assert.AreEqual( "''", ValueArray[105], "Value of APORR1 should be ''" );
                Assert.AreEqual( "''", ValueArray[106], "Value of APORR2 should be ''" );
                Assert.AreEqual( "''", ValueArray[107], "Value of APORR3 should be ''" );
                Assert.AreEqual("'234 MulHolland Drive'", ValueArray[109], "Value of APGAD1E1 should be '234 MulHolland Drive'");
                Assert.AreEqual("'#1'", ValueArray[110], "Value of APGAD1E2 should be '#1'");
            }
        }

        #endregion

        #region Data Elements

        private Name
            INSURED_NAME1 = new Name( INSURED_F_NAME1, INSURED_L_NAME1, INSURED_MI1 );

        private Name
            INSURED_NAME2 = new Name( INSURED_F_NAME2, INSURED_L_NAME2, INSURED_MI2 );

        private static  Facility facility = null;
        private Address addressE = new Address( ADDRESS1E,
                                                ADDRESS2E,
                                                CITYE,
                                                new ZipCode( POSTALCODEE ),
                                                new State( PersistentModel.NEW_OID,
                                                           ReferenceValue.NEW_VERSION,
                                                           "TEXAS",
                                                           "TX" ),
                                                new Country( PersistentModel.NEW_OID,
                                                             ReferenceValue.NEW_VERSION,
                                                             "United States",
                                                             "USA" ),
                                                new County( PersistentModel.NEW_OID,
                                                            ReferenceValue.NEW_VERSION,
                                                            "ORANGE",
                                                            "122" )
            );

        private Address addressM = new Address( ADDRESS1M,
                                                ADDRESS2M,
                                                CITYM,
                                                new ZipCode(  POSTALCODEM ),
                                                new State( PersistentModel.NEW_OID,
                                                           ReferenceValue.NEW_VERSION,
                                                           "TEXAS",
                                                           "TX" ),
                                                new Country( PersistentModel.NEW_OID,
                                                             ReferenceValue.NEW_VERSION,
                                                             "United States",
                                                             "USA" ),
                                                new County( PersistentModel.NEW_OID,
                                                            ReferenceValue.NEW_VERSION,
                                                            "ORANGE",
                                                            "123" )
            );

        private Patient patient = new Patient();

        private TypeOfContactPoint typeOfContactPointMOB =
            new TypeOfContactPoint( 2, "CELL" );
        private TypeOfContactPoint typeOfContactPointMLG =
            new TypeOfContactPoint( 0, "MAILING" );
        private TypeOfContactPoint typeOfContactPointEMP =
            new TypeOfContactPoint( 1, "EMPLOYER" );

        private PhoneNumber phoneNumberMOB = new PhoneNumber( "610", "6903412" );
        private PhoneNumber phoneNumberMLG = new PhoneNumber( "972", "6903564" );
        private PhoneNumber phoneNumberEMP = new PhoneNumber( "610", "4545454" );

        private EmailAddress emailAddress = new EmailAddress( "FC@YAHOO.COM" );

        private Employment employment = new Employment();
        private Employer empr = new Employer();
        private EmploymentStatus empSt = new EmploymentStatus(
            PersistentModel.NEW_OID, DateTime.Now, "EMPLOYED FULL TIME", "1" );

        private TransactionKeys transactionKeys;

        #endregion

        #region Constants

        private const int
            NUMBER_OF_ENTRIES = 111;

        private const string
            GUARANTOR_F_NAME = "SONNY",
            GUARANTOR_L_NAME = "SADSTORY",
            GUARANTOR_MI = "A",
            INSURED_F_NAME1 = "HAPPY",
            INSURED_L_NAME1 = "SADSTORY",
            INSURED_F_NAME2 = "SOMETHING",
            INSURED_L_NAME2 = "SADSTORY",
            INSURED_MI2 = "N";

        private static readonly string INSURED_MI1 = string.Empty;
        
        private new const string FACILITY_CODE = "DEL";

        private const string ADDRESS1E = "335 Nicholson Dr.",
                             ADDRESS2E = "#303",
                             CITYE = "Austin",
                             POSTALCODEE = "605045000";

        private const string ADDRESS1M = "234 MulHolland Drive",
                             ADDRESS2M = "#1",
                             CITYM = "Austin",
                             POSTALCODEM = "605945000";

        private const int FACILITY_ID = 900;
      
        #endregion
    }
}