using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Extensions.Exceptions;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Persistence;
using Rhino.Mocks;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]

    public class AccountPbarBrokerTests : AbstractBrokerTests
    {
        #region Constants

        private const string DEL_FAC_CODE = "DEL";

        private const long
            HSP_CODE = 6L,
            TEST_MEDICAL_RECORD_NUMBER = 299,
            TEST_MRN_NUMBER = 785138,
            TEST_MEDICAL_RECORD_NUMBER_BAD = 0,
            TEST_EXPECTED_ACCOUNT_COUNT = 5,
            TEST_EXPECTED_ACCOUNT_NUMBER = 3515311;

        private const string
            TEST_EXPECTED_FINANCIALCLASSCODE = "41",
            TEST_EXPECTED_MEDICAL_SERVICE_CODE = "56",
            TEST_EXPECTED_PATIEHT_TYPE = "2";

        private readonly DateTime TEST_ADMIT_DATE = new DateTime( 2002, 2, 4, 11, 6, 0 );
        private const string PAYOR_COVERAGE_CATEGORY = "'1'";
        private const string PAYOR_COVERAGE_CATEGORY_NULL = "ALL";
        private const string PAYOR_NURSING_STATION_CODE = "'3N'";
        private const string PAYOR_NURSING_STATION_CODE_NULL = "ALL";

        private const string REL_REL_CODE_ONE = "BP";
        private const string REL_REL_CODE_ALL = "ALL";
        private const string REL_REL_CODE_UNSPECIFIED = "UNSPECIFIED";

        private const string PATIENT_LAST_NAME = "FRANCISCO";
        private const string PATIENT_FIRST_NAME = "SAM";
        private const long PATIENT_ACCOUNT_NUMBER = 31120;

        private const int
            PHYSICIAN_CENSUS_ADMITTING = 1,
            PHYSICIAN_CENSUS_ATTENDING = 1,
            PHYSICIAN_CENSUS_REFERRING = 1,
            PHYSICIAN_CENSUS_CONSULTING = 1,
            PHYSICIAN_CENSUS_OPERATING = 0;

        private const long PHYSICIAN_CENSUS_PHYSICIAN_NUMBER = 10;

        private const string START_TIME = "0000";
        private const string ADT_TYPE = "E";
        private const string NURSINGSTATIONS_LIST = "ALL";
        private const string ADT_SORT_COLUMN = "Patient";

        private const int PRIMARY_COVERAGE = 1;

        private const string
            INPATIENT_ACCOUNT_ACTIVITY = "033199-FC 41  PT 1 BY NMAP         ",
            OUTPATIENT_ACCOUNT_ACTIVITY = "040699-FC 37  PT 2 BY JMAC      /AC",
            EMERGENCY_ACCOUNT_ACTIVITY = "042299-FC 70  PT 3 BY HCAT      /AC",
            DUMMY_ACCOUNT_ACTIVITY = "Dummy Activity";

        #endregion

        #region SetUp and TearDown AccountBrokerTests
        [SetUp]
        public void SetUpAccountBrokerTests()
        {
            i_FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_AccountBroker = BrokerFactory.BrokerOfType<IAccountBroker>();
            AccountPbarBroker = new AccountPBARBroker();
            i_FinancialClassesBroker = BrokerFactory.BrokerOfType<IFinancialClassesBroker>();

            i_Facility = i_FacilityBroker.FacilityWith( 6 );
            i_ACOFacility = i_FacilityBroker.FacilityWith( ACO_FACILITYID );
            CreateUser();
        }

        [TearDown]
        public void TearDownAccountBrokerTests()
        {
            i_FacilityBroker = null;
            i_AccountBroker = null;
            i_FinancialClassesBroker = null;
            i_Facility = null;
            i_ACOFacility = null;
        }

        #endregion

        #region Test Methods

        /// <summary>
        /// Tests extracting the patient drivers license info
        /// </summary>
        /// <remarks>
        /// This is the happy path test
        /// Defect: OTD37278
        /// </remarks>
        [Test]
        public void TestGetPatientDrivingLicenseInfo_ExpectedFormat()
        {
            TestGetPatientDrivingLicenseInfo( "11221122       TX", "11221122", "TX" );
        }


        /// <summary>
        /// Tests extracting the patient drivers license info
        /// </summary>
        /// <remarks>
        /// This is the data that triggered the defect. The original
        /// code was not built to more than two words in the string
        /// Defect: OTD37278
        /// </remarks>
        [Test]
        public void TestGetPatientDrivingLicenseInfo_OTD37278Format()
        {
            TestGetPatientDrivingLicenseInfo( "NONE AVAIL     TX", "NONE AVAIL", "TX" );
        }


        /// <summary>
        /// Tests extracting the patient drivers license info
        /// </summary>
        /// <remarks>
        /// There is a lot of data with no state and some kind of remark.
        /// This should result in a message in the number an any empty
        /// string for the state value
        /// Defect: OTD37278
        /// </remarks>
        [Test]
        public void TestGetPatientDrivingLicenseInfo_NoState()
        {
            TestGetPatientDrivingLicenseInfo( "UNAVAILABLE      ", "UNAVAILABLE", String.Empty );
        }


        /// <summary>
        /// Tests extracting the patient drivers license info
        /// </summary>
        /// <remarks>
        /// Since we are assuming that the first 15 are for the number and
        /// the final two are for the state, let's make sure that the
        /// parsing still holds if we max out the number
        /// Defect: OTD37278
        /// </remarks>
        [Test]
        public void TestGetPatientDrivingLicenseInfo_MaxNumber()
        {
            TestGetPatientDrivingLicenseInfo( "123456789012345OH", "123456789012345", "OH" );
        }//method


        /// <summary>
        /// Tests extracting the patient drivers license info
        /// </summary>
        /// <remarks>
        /// Should still get a state out of this
        /// the final two are for the state, let's make sure that the
        /// parsing still holds if we max out the number
        /// Defect: OTD37278
        /// </remarks>
        [Test]
        public void TestGetPatientDrivingLicenseInfo_NoNumber()
        {
            TestGetPatientDrivingLicenseInfo( "               OH", String.Empty, "OH" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void AddDemographicsTo()
        {
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            IAccount anAccount = new Account {AccountNumber = 4477677};
            Facility facility = facilityBroker.FacilityWith( DEL_FAC_CODE );
            anAccount.Facility = facility;

            Patient patient = new Patient(
                46L,
                PersistentModel.NEW_VERSION,
                new Name( "Claire", "Fried", "D", "Mrs." ),
                132288,
                new DateTime( 1955, 3, 5 ),
                new SocialSecurityNumber( "123121234" ),
                new Gender( 0, DateTime.Now, "Female", "F" ),
                facility
                );

            anAccount.Patient = patient;

            anAccount = AccountBroker.AddDemographicDataTo( anAccount );

            Assert.AreEqual(
                "UNKNOWN",
                anAccount.Patient.MaritalStatus.Description,
                "Marital Status should be UNKNOWN" );

            Assert.AreEqual(
                "WHITE",
                anAccount.Patient.Race.Description,
                "Race should be WHITE" );

          Assert.AreEqual(
                "NON-HISPANIC",
                anAccount.Patient.Ethnicity.Description,
                "Ethnicity should be NON-HISPANIC" );

            Assert.AreEqual(
                string.Empty,
                anAccount.Patient.NationalID.Trim(),
                "National ID should be blank" );

            Assert.AreEqual(
                string.Empty,
                anAccount.Patient.PlaceOfBirth.Trim(),
                "PlaceOfBirth should be blank" );

            Assert.AreEqual(
                "ENGLISH",
                anAccount.Patient.Language.Description,
                "Language should be ENGLISH" );

            Assert.AreEqual(
                "NO PREFERENCE",
                anAccount.Patient.Religion.Description,
                "Religion should be NO PREFERENCE" );

            Assert.AreEqual(
                string.Empty,
                anAccount.Patient.PlaceOfWorship.Description,
                "PlaceOfWorship should be blank" );
        }

        [Test]
        [Description("VeryLongExecution")]
        public void TestRace2ToAddDemographicsTo()
        {
            const int mrn = 832239;
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            IAccount anAccount = new Account { AccountNumber = 582809 };
            Facility facility = facilityBroker.FacilityWith(ACO_FACILITYID);
            anAccount.Facility = facility;

            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name("FOR-NUNIT", "PRE-REG", string.Empty),
                mrn,
                DateTime.Now,
                new SocialSecurityNumber("230230230"),
                new Gender(1, DateTime.Now, "Male", "M"),
                FacilityBroker.FacilityWith(ACO_FACILITYID));

            anAccount.Patient = patient;

            anAccount = AccountBroker.AddDemographicDataTo(anAccount);


            Assert.AreEqual(
                "BLACK",
                anAccount.Patient.Race2.Description,
                "Race2 should be BLACK");

        }
        [Test]
        [Description("VeryLongExecution")]
        public void TestEthnicity2ToAddDemographicsTo()
        {
            const int mrn = 832239;
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            IAccount anAccount = new Account { AccountNumber = 582809 };
            Facility facility = facilityBroker.FacilityWith(ACO_FACILITYID);
            anAccount.Facility = facility;

            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name("FOR-NUNIT", "PRE-REG", string.Empty),
                mrn,
                DateTime.Now,
                new SocialSecurityNumber("230230230"),
                new Gender(1, DateTime.Now, "Male", "M"),
                FacilityBroker.FacilityWith(ACO_FACILITYID));

            anAccount.Patient = patient;

            anAccount = AccountBroker.AddDemographicDataTo(anAccount);


            Assert.AreEqual(
                "NON-HISPANIC",
                anAccount.Patient.Ethnicity2.Description,
                "Ethnicity2 should be NON-HISPANIC");

        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountProxyFor()
        {
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                TEST_MEDICAL_RECORD_NUMBER,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                i_ACOFacility );
            AccountProxy anAccountProxy = AccountBroker.AccountProxyFor( i_ACOFacility.Code, patient,
                                                                             785138, 30015 );
            Assert.IsNotNull( anAccountProxy, "Account Proxy not found" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void AccountsForAPatient()
        {

            // Create a valid patient object
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                TEST_MEDICAL_RECORD_NUMBER,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                FacilityBroker.FacilityWith( HSP_CODE ) );

            // now look for accounts for that patient.
            ArrayList accounts = AccountBroker.AccountsFor( patient );

            Assert.IsTrue( accounts != null,
                           "No Accounts found for medical record number: " + TEST_MEDICAL_RECORD_NUMBER );
            Assert.IsTrue( accounts.Count >= 1,
                           "No Accounts found for medical record number: " + TEST_MEDICAL_RECORD_NUMBER );

            Assert.AreEqual( TEST_EXPECTED_ACCOUNT_COUNT, accounts.Count,
                             " Expected number of accounts not found" );

            // find the specific proxy
            AccountProxy accountProxy = null;
            foreach ( AccountProxy proxy in accounts )
            {
                if ( proxy.AccountNumber == TEST_EXPECTED_ACCOUNT_NUMBER )
                {
                    accountProxy = proxy;
                    break;
                }
            }

            Assert.IsNotNull( accountProxy, "Could not find expected account" );
            Assert.AreEqual( TEST_EXPECTED_ACCOUNT_NUMBER,
                             accountProxy.AccountNumber,
                             "Account Number should be " + TEST_EXPECTED_ACCOUNT_NUMBER );
            Assert.AreEqual( TEST_ADMIT_DATE,
                             accountProxy.AdmitDate,
                             "Admit Date should be " + TEST_ADMIT_DATE );
            Assert.AreEqual( "4/9/2007 11:59:00 PM",
                             accountProxy.DischargeDate.ToString(),
                             "Discharge should be 4/9/2007 11:59:00 PM" );

            FinancialClass finClass = FinancialClassesBroker.FinancialClassWith( ACO_FACILITYID, TEST_EXPECTED_FINANCIALCLASSCODE );
            Assert.AreEqual( accountProxy.FinancialClass,
                             finClass,
                             "Financial Code is not as expected" );
            Assert.AreEqual( TEST_EXPECTED_MEDICAL_SERVICE_CODE,
                             accountProxy.HospitalService.Code,
                             "Service code should be " + TEST_EXPECTED_MEDICAL_SERVICE_CODE );
            Assert.AreEqual( TEST_EXPECTED_PATIEHT_TYPE,
                             accountProxy.KindOfVisit.Code,
                             "Patient Type should be " + TEST_EXPECTED_PATIEHT_TYPE );

            // finally check to see that there is one record that is locked
            // Problem: The lock record keeps disappearing from pBAR.
            //          bool accountWithLockfound = false;
            //          foreach( AccountProxy ap in accounts )
            //          {
            //              if(ap.AccountNumber == TEST_EXPECTED_ACCOUNT_NUMBER &&
            //                  ap.IsLocked == true)
            //              {
            //                  accountWithLockfound = true;
            //              }
            //          }
            //          Assert.AreEqual( true, accountWithLockfound, "Could not find locked record" );

            Account acct = AccountBroker.AccountFor( accountProxy );
            ArrayList ocs = ( ArrayList )acct.OccurrenceCodes;
            Assert.AreEqual( 4, ocs.Count, "Wrong number of occurrence Codes" );

            Assert.IsTrue( acct.IsCanceled == false,
                           "Account is not canceled but proxy is returning true for canceled" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void AccountsForAPreRegPatient()
        {
            // Create a valid patient object
            const int mrn = 832239;
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "FOR-NUNIT", "PRE-REG", string.Empty ),
                mrn,
                DateTime.Now,
                new SocialSecurityNumber( "230230230" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                FacilityBroker.FacilityWith( ACO_FACILITYID ) );

            // now look for accounts for that patient.
            ArrayList accounts = AccountBroker.PreRegAccountsFor( patient );

            Assert.IsTrue( accounts != null,
                          "No Accounts found for medical record number: " + mrn );
            Assert.IsTrue( accounts.Count >= 1,
                          "No Accounts found for medical record number: " + mrn );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void MPIAccountsForAPatient()
        {

            // Create a valid patient object
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                TEST_MRN_NUMBER,
                DateTime.Now,
                null,
                new Gender( 1, DateTime.Now, "Male", "M" ),
                FacilityBroker.FacilityWith( ACO_FACILITYID ) );

            // now look for accounts for that patient.
            ArrayList accounts = AccountBroker.MPIAccountsFor( patient );

            Assert.IsTrue( accounts != null,
                           "No Accounts found for medical record number: " + TEST_MRN_NUMBER );
            Assert.IsTrue( accounts.Count >= 1,
                           "No Accounts found for medical record number: " + TEST_MRN_NUMBER );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void MPIDetailsForAnAccount()
        {
            Patient patient = new Patient();
            patient.Oid = 46;
            patient.Facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            patient.MedicalRecordNumber = 24004;

            AccountProxy proxy = new AccountProxy( 32110, patient, DateTime.Now, DateTime.Now,
                                                  new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), FacilityBroker.FacilityWith( ACO_FACILITYID ),
                                                  new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                  new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                  "OL HSV60", false );

            AccountProxy aProxy = AccountBroker.MPIAccountDetailsFor( proxy );

            Assert.IsTrue( aProxy != null,
                           "No Accounts found for medical record number: " );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        //[Ignore] //"Data for this test keeps going away"
        public void TestCanceledAccount()
        {
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                786415,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                i_ACOFacility );

            ArrayList accounts = AccountBroker.AccountsFor( patient );
            AccountProxy accountProxy = null;
            foreach ( AccountProxy proxy in accounts )
            {
                if ( proxy.AccountNumber == 92346 )
                {
                    accountProxy = proxy;
                    break;
                }
            }
            Assert.IsNotNull( accountProxy, "Canceled account not found" );
            Assert.IsTrue( accountProxy.IsCanceled,
                           "Account 92346 Not canceled but should be" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAutoAccident()
        {
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                274445,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                i_Facility );
            ArrayList accounts = AccountBroker.AccountsFor( patient );
            AccountProxy accountProxy = null;
            foreach ( AccountProxy proxy in accounts )
            {
                if ( proxy.AccountNumber == 8895583 )
                {
                    accountProxy = proxy;
                    break;
                }
            }
            Account acct = i_AccountBroker.AccountFor( accountProxy );
            ArrayList occurrenceCodes = ( ArrayList )acct.OccurrenceCodes;
            Assert.AreEqual( 2, occurrenceCodes.Count,
                             "Wrong number of occurrence Codes" );

            Condition condition = acct.Diagnosis.Condition;
            Assert.AreEqual( typeof( Accident ),
                             condition.GetType(),
                             "Wrong type of condition found for account" );
            Accident accident = ( Accident )condition;
            Assert.AreEqual( new DateTime( 2001, 3, 11 ),
                             accident.OccurredOn,
                             "Incorrect Accident date found" );
            Assert.AreEqual( 5,
                             accident.Kind.Oid,
                             "Incorrect Accident type found" );
            Console.WriteLine( accident.OccurredOn );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForAllCoverageCategoryAndAllNursing()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching( 
                PAYOR_COVERAGE_CATEGORY_NULL, PAYOR_NURSING_STATION_CODE_NULL, i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForOneCoverageCategoryAndAllNursing()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching( 
                PAYOR_COVERAGE_CATEGORY, PAYOR_NURSING_STATION_CODE_NULL, i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsForReligionResultsForAll()
        {
            ArrayList accountProxies = 
                ( ArrayList )AccountBroker.AccountsFor( i_ACOFacility.Code, REL_REL_CODE_ALL );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsForReligionResultsForUnspc()
        {
            ArrayList accountProxies = 
                ( ArrayList )AccountBroker.AccountsFor( i_ACOFacility.Code, REL_REL_CODE_UNSPECIFIED );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsForReligionResultsForOne()
        {
            ArrayList accountProxies = 
                ( ArrayList )AccountBroker.AccountsFor( i_ACOFacility.Code, REL_REL_CODE_ONE );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForAllCoverageAndOneNursingStation()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching( 
                PAYOR_COVERAGE_CATEGORY_NULL, PAYOR_NURSING_STATION_CODE, i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForOneCoverageAndOneNursingStation()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching( 
                PAYOR_COVERAGE_CATEGORY, PAYOR_NURSING_STATION_CODE, i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void AccountsForAPregnantPatient()
        {

            // Create a valid patient object
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                785216,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Female", "F" ),
                FacilityBroker.FacilityWith( ACO_FACILITYID ) );

            // now look for accounts for that patient.
            ArrayList accounts = AccountBroker.AccountsFor( patient );

            Assert.IsTrue( accounts != null,
                           "No Accounts found for medical record number: " + 785216 );
            Assert.IsTrue( accounts.Count >= 1,
                           "No Accounts found for medical record number: " + 785216 );


            AccountProxy accountProxy = null;
            foreach ( AccountProxy ap in accounts )
            {
                if ( ap.AccountNumber == 31112 )
                {
                    accountProxy = ap;
                }
            }
            Assert.IsNotNull( accountProxy, "Did not find account 31112 (Pregnant)" );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void AccountBrokerFailure()
        {
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "SomeFirstName", "SomeLastName", "M" ),
                TEST_MEDICAL_RECORD_NUMBER_BAD,
                DateTime.Now,
                new SocialSecurityNumber( "111223333" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                FacilityBroker.FacilityWith( HSP_CODE ) );

            // now look for accounts for that patient.
            ArrayList accounts = AccountBroker.AccountsFor( patient );

            Assert.IsTrue( accounts.Count == 0,
                           "Found accounts when there should be none" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountFor()
        {
            Patient patient = new Patient();
            patient.Oid = 46;
            patient.Facility = i_Facility;
            patient.MedicalRecordNumber = 24004;
            AccountProxy proxy = new AccountProxy( 4477677, patient, DateTime.Now, DateTime.Now,
                                                  new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), i_Facility,
                                                  new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                  new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                  "OL HSV60", false );

            Account account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );

            // account with PBAR Value Codes
            patient = new Patient();
            patient.Facility = i_ACOFacility;
            patient.MedicalRecordNumber = 834614;
            proxy = new AccountProxy( 587733, patient, DateTime.Now, DateTime.Now,
                                     new VisitType(), i_ACOFacility, new FinancialClass(),
                                     new HospitalService(), string.Empty, false );

            account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Cannot read account" );
            //Assert.AreEqual( "A3", account.ValueCode1, "PBAR Value Codes 1 do not match." );

            // account with Medicaid as primary insurance
            patient = new Patient();
            patient.Facility = i_ACOFacility;
            patient.MedicalRecordNumber = 834614;
            proxy = new AccountProxy( 587733, patient, DateTime.Now, DateTime.Now,
                                     new VisitType(), i_ACOFacility, new FinancialClass(),
                                     new HospitalService(), string.Empty, false );

            account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Cannot read account" );

            // account with Medicare as primary insurance
            patient = new Patient();
            patient.Facility = i_ACOFacility;
            patient.MedicalRecordNumber = 834614;
            proxy = new AccountProxy( 587733, patient, DateTime.Now, DateTime.Now,
                                     new VisitType(), i_ACOFacility, new FinancialClass(),
                                     new HospitalService(), string.Empty, false );

            account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Cannot read account" );

            // NOTE: Test needs to be modified since this is not checking to see if the account
            // has SelfPay or Worker's Comp as primary insurance although the comment says so.

            // account with SelfPay as primary insurance
            //patient = new Patient();
            //patient.Facility = i_ACOFacility;
            //patient.MedicalRecordNumber = 834614;
            //proxy = new AccountProxy( 428029, patient, DateTime.Now, DateTime.Now,
            //                         new VisitType(), i_ACOFacility, new FinancialClass(),
            //                         new HospitalService(), string.Empty, false );

            //account = AccountBroker.AccountFor( proxy );
            //Assert.IsNotNull( account, "Cannot read account" );

            //// account with WorkerCompensation as primary insurance
            //patient = new Patient();
            //patient.Facility = i_ACOFacility;
            //patient.MedicalRecordNumber = 834614;
            //proxy = new AccountProxy( 428086, patient, DateTime.Now, DateTime.Now,
            //                         new VisitType(), i_ACOFacility, new FinancialClass(),
            //                         new HospitalService(), string.Empty, false );

            //account = AccountBroker.AccountFor( proxy );
            //Assert.IsNotNull( account, "Cannot read account" );

        }

        [Test]
        [Ignore ]
        [Description( "Ignore this test, wating for the account to be discharged.")]
        public void TestPreDischargeLocation()
        {
            Patient patient = new Patient();
            patient.Oid = -1;
            patient.Facility = i_ACOFacility;
            patient.MedicalRecordNumber = 830563;
            AccountProxy proxy = new AccountProxy( 581603, patient, DateTime.Now, DateTime.Now,
                                                  new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), i_ACOFacility,
                                                  new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                  new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                  "OL HSV60", false );

            proxy.Activity = new CancelOutpatientDischargeActivity();
            proxy.Activity.AppUser.Facility = i_ACOFacility;

            Account account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );

            Assert.IsNotNull( account.PreDischargeLocation, "Pre-discharge location should not be null" );
            Assert.AreEqual( account.PreDischargeLocation.Bed.Code, "6", "Bed not Correct" );
            Assert.AreEqual( account.PreDischargeLocation.Room.Code, "0652", "Room not Correct" );
            Assert.AreEqual( account.PreDischargeLocation.NursingStation.Code, "6H", "Nursing Station not Correct" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountDetailsFor()
        {
            Patient patient = new Patient();
            patient.Oid = 46;
            patient.Facility = i_Facility;
            patient.MedicalRecordNumber = 24004;
            AccountProxy proxy = new AccountProxy( 4477677, patient, DateTime.Now, DateTime.Now,
                                                  new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), i_Facility,
                                                  new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                  new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                  "OL HSV60", false );

            Account account = AccountBroker.AccountDetailsFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountForWithPatientAndAccountNumber()
        {
            Patient patient = new Patient();
            patient.Oid = 46;
            patient.Facility = i_Facility;
            patient.MedicalRecordNumber = 24004;

            const long accountNumber = 4477677;

            Account account = AccountBroker.AccountFor( patient, accountNumber );
            Assert.IsNotNull( account, "AccountFor with patient and accountNumber parameters failed" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountForACOPatient()
        {
            Patient patient = new Patient();
            patient.Oid = 46;
            Facility facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            patient.Facility = facility;
            patient.MedicalRecordNumber = 785138;
            AccountProxy proxy = new AccountProxy( 30015, patient, DateTime.Now, DateTime.Now,
                                                  new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), facility,
                                                  new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                  new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                  "OL HSV60", false );

            Account account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );

        }
        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountForWithThinPatient()
        {
            Patient patient = new Patient();
            patient.Oid = 299;
            patient.MedicalRecordNumber = 299;
            patient.Facility = i_Facility;
            AccountProxy proxy = new AccountProxy( 5345740,
                                                   patient, DateTime.Now, DateTime.Now,
                                                   new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC, VisitType.OUTPATIENT ), i_Facility,
                                                   new FinancialClass( 299, PersistentModel.NEW_VERSION, "MEDICADE", "40" ),
                                                   new HospitalService( 0, PersistentModel.NEW_VERSION, "DIAGNOSTIC OUTOPT", "60" ),
                                                   "OL HSV60", false );
            Account account = AccountBroker.AccountFor( proxy );
            Assert.IsNotNull( account, "Can not read account" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestBloodlessTreatmentAccountsFor()
        {
            ICollection accountProxies = AccountBroker.BloodlessTreatmentAccountsFor( i_ACOFacility.Code, "R", "T" );
            Assert.IsNotNull( accountProxies, "Did not find any accounts for registered patients and today as admit date" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountWithPreRegWorklists()
        {
            Facility facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            WorklistSelectionRange r = new WorklistSelectionRange( 9, "Date Range", 0 );
            WorklistSettings worklistSettings = new WorklistSettings(
                "R", "T", new DateTime( 0 ), new DateTime( 0 ), r, 1, 1 );
            worklistSettings.WorkListID = 1;
            IList list = AccountBroker.AccountsWithWorklistsWith( facility.Code, worklistSettings );

            Assert.IsNotNull( list, "Did not find any accounts" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountWithPostRegWorklists()
        {
            i_Facility = FacilityBroker.FacilityWith( 6 );
            Facility facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            WorklistSelectionRange r = new WorklistSelectionRange( 9, "Date Range", 0 );
            WorklistSettings worklistSettings = new WorklistSettings(
                "A", "Z", new DateTime( 0 ), new DateTime( 0 ), r, 1, 1 );
            worklistSettings.WorkListID = 2;
            IList list = AccountBroker.AccountsWithWorklistsWith( facility.Code, worklistSettings );

            Assert.IsNotNull( list, "Did not find any accounts" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForPatientCensus()
        {
            PatientCensusSearchCriteria patientCriteria =
                new PatientCensusSearchCriteria( PATIENT_LAST_NAME,
                                                 PATIENT_FIRST_NAME,
                                                 PATIENT_ACCOUNT_NUMBER,
                                                 i_ACOFacility );

            ICollection accounts =
                i_AccountBroker.AccountsMatching( patientCriteria );


            Assert.IsNotNull( accounts, "Accounts list is empty" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        //[Ignore] //"Reproduced the problem with DB2 SP ACCOUNTSMATCHINGNURSINGSTATIONS"
        public void TestAccountsMatchingForAllNursingStation()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching(
                                                        false, "All", i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForAllNursingStationWithOccupiedBeds()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching(
                                                        true, "All", i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForOneNursingStation()
        {
            ArrayList accountProxies = ( ArrayList )AccountBroker.AccountsMatching(
                                                        false, "3N", i_ACOFacility.Code );

            Assert.IsNotNull( accountProxies, "Did not find any Accounts" );
        }


        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountMatchingForPhysician()
        {
            try
            {
                PhysicianPatientsSearchCriteria physicianPatientsSearchCriteria =
                    new PhysicianPatientsSearchCriteria(
                        i_ACOFacility,
                        PHYSICIAN_CENSUS_PHYSICIAN_NUMBER,
                        PHYSICIAN_CENSUS_ADMITTING,
                        PHYSICIAN_CENSUS_ATTENDING,
                        PHYSICIAN_CENSUS_REFERRING,
                        PHYSICIAN_CENSUS_CONSULTING,
                        PHYSICIAN_CENSUS_OPERATING );

                ICollection accounts =
                    i_AccountBroker.AccountsMatching( physicianPatientsSearchCriteria );

                Assert.IsNotNull( accounts, "Accounts list is empty" );
                Assert.IsTrue( accounts.Count >= 1,
                               "There are no results from the Physicians Search" );
            }
            catch ( Exception ee )
            {
                Assert.Fail( ee.Message );
            }
        }

        [Test]
        [Description( "VeryLongExecution" )]
        [ExpectedException( typeof( BrokerException ) )]
        public void TestSaveAccountFailFinClass()
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            PatientSearchCriteria searchCriteria = new PatientSearchCriteria(
                FACILITY_CODE,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                "785138",
                String.Empty );

            searchCriteria.HSPNumber = ACO_FACILITYID;

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( searchCriteria );
            Patient patient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            ArrayList accounts = AccountBroker.AccountsFor( patient );
            Account account = null;
            foreach ( AccountProxy ap in accounts )
            {
                if ( ap.AccountNumber == 30049 )
                {
                    account = AccountBroker.AccountFor( ap );
                    break;
                }
            }
            if ( account == null )
            {
                Assert.Fail( "Account 64238 not found" );
            }

            account.Activity = new MaintenanceActivity();

            account.FinancialClass.Code = string.Empty;
            AccountBroker.Save( account, account.Activity );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        [ExpectedException( typeof( BrokerException ) )]
        public void TestSaveAccountFailName()
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            PatientSearchCriteria searchCriteria = new PatientSearchCriteria(
                FACILITY_CODE,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                "785138",
                String.Empty );

            searchCriteria.HSPNumber = ACO_FACILITYID;

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( searchCriteria );
            Patient patient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            ArrayList accounts = AccountBroker.AccountsFor( patient );
            Account account = null;
            foreach ( AccountProxy ap in accounts )
            {
                if ( ap.AccountNumber == 30049 )
                {
                    account = AccountBroker.AccountFor( ap );
                    break;
                }
            }
            if ( account == null )
            {
                Assert.Fail( "Account 64238 not found" );
            }

            account.Activity = new MaintenanceActivity();

            account.Patient.LastName = string.Empty;
            AccountBroker.Save( account, account.Activity );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        [ExpectedException( typeof( BrokerException ) )]
        public void TestSaveAccountFailAdmitDate()
        {
            IPatientBroker patientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            PatientSearchCriteria searchCriteria = new PatientSearchCriteria(
                FACILITY_CODE,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                "785138",
                String.Empty );

            searchCriteria.HSPNumber = ACO_FACILITYID;

            PatientSearchResponse patientSearchResponse = patientBroker.GetPatientSearchResponseFor( searchCriteria );
            Patient patient = patientBroker.PatientFrom( patientSearchResponse.PatientSearchResults[0] );

            ArrayList accounts = AccountBroker.AccountsFor( patient );
            Account account = null;
            foreach ( AccountProxy ap in accounts )
            {
                if ( ap.AccountNumber == 30049 )
                {
                    account = AccountBroker.AccountFor( ap );
                    break;
                }
            }
            if ( account == null )
            {
                Assert.Fail( "Account 64238 not found" );
            }

            account.Activity = new MaintenanceActivity();

            account.AdmitDate = new DateTime();
            AccountBroker.Save( account, account.Activity );

        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestAccountsMatchingForADTCensus()
        {
            try
            {
                CensusADTSearchCriteria adtSearchCriteria =
                    new CensusADTSearchCriteria( START_TIME,
                                                 ADT_TYPE,
                                                 NURSINGSTATIONS_LIST,
                                                 ADT_SORT_COLUMN,
                                                 i_ACOFacility );

                ICollection accounts =
                    i_AccountBroker.AccountsMatching( adtSearchCriteria );

                if ( accounts != null )
                {
                    Assert.IsTrue( true );
                }
            }
            catch ( Exception ee )
            {
                Assert.Fail( ee.Message );
            }
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestVerifyOfflineMRN()
        {
            Facility facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            try
            {
                //valid and not used
                AccountBroker.VerifyOfflineMRN( 11, facility.Oid );

            }
            catch ( OfflineMRNAlreadyUsedException )
            {
                Assert.AreEqual( false, true, "should not be in use" );
            }
            catch ( OfflineMRNInvalidException )
            {
                Assert.AreEqual( true, true, "should be invalid" );
            }

            try
            {
                //in use
                i_AccountBroker.VerifyOfflineMRN( 785138, facility.Oid );

            }
            catch ( OfflineMRNAlreadyUsedException )
            {
                Assert.AreEqual( true, true, "should be in use" );
            }
            catch ( OfflineMRNInvalidException )
            {
                Assert.AreEqual( true, true, "should be invalid" );
            }
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestVerifyOfflineAccountNumber()
        {
            Facility facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            try
            {
                //valid and not used
                AccountBroker.VerifyOfflineAccountNumber( 3112, facility.Oid );

            }
            catch ( OfflineAccountAlreadyUsedException )
            {
                Assert.AreEqual( false, true, "should not be in use" );
            }
            catch ( OfflineAccountInvalidException )
            {
                Assert.AreEqual( true, true, "should be invalid" );
            }

            try
            {
                //in use
                i_AccountBroker.VerifyOfflineAccountNumber( 31005, facility.Oid );

            }
            catch ( OfflineAccountAlreadyUsedException )
            {
                Assert.AreEqual( true, true, "should be in use" );
            }
            catch ( OfflineAccountInvalidException )
            {
                Assert.AreEqual( true, true, "should be invalid" );
            }
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestBuildPreMSECoverage()
        {
            Patient patient = new Patient(
                1,
                DateTime.Now,
                new Name( "MSEFirstName", "MSELastName", "M" ),
                111,
                DateTime.Now,
                new SocialSecurityNumber( "222333444" ),
                new Gender( 1, DateTime.Now, "Male", "M" ),
                i_ACOFacility );

            Account anAccount = new Account();
            anAccount.AccountNumber = 100;
            anAccount.Patient = patient;

            anAccount.Facility = FacilityBroker.FacilityWith( ACO_FACILITYID );

            anAccount.AdmitDate = new DateTime( 2006, 4, 4, 4, 4, 4 );
            ConditionOfService cos = new ConditionOfService();
            cos.Code = "Y";
            anAccount.COSSigned = cos;

            anAccount = i_AccountBroker.BuildCoverageForPreMSEActivity( anAccount );
            Coverage coverage = anAccount.Insurance.CoverageFor( PRIMARY_COVERAGE );

            Assert.AreEqual( "Y", anAccount.COSSigned.Code, "Incorrect APRICF - Release of Info Cert Flag." );
            Assert.AreEqual( "MSELastName", coverage.Insured.LastName, "Incorrect APSLNM - Subscriber Last Name." );

            Assert.AreEqual( "MSEFirstName", coverage.Insured.FirstName, "Incorrect APSFNM - Subscriber First name." );
            Assert.AreEqual( "M", coverage.Insured.Sex.Code, "Incorrect APSSEX - Subscriber Sex Code." );

            RelationshipType relationshipType = anAccount.Patient.RelationshipWith( coverage.Insured );
            Assert.AreEqual( "01", relationshipType.Code, "Incorrect APSRCD - Subscriber Relationship Code." );
            Assert.AreEqual( "SELF", relationshipType.Description.ToUpper(), "Incorrect RelationshipType Description." );

            Assert.AreEqual( String.Empty, coverage.Insured.GroupNumber, "Incorrect APINS# - Insurance Company Number." );
            Assert.AreEqual( "EDL81", coverage.InsurancePlan.PlanID, "Incorrect APPNID - UBF1 PLAN#. " );

            Assert.AreEqual( "1", coverage.CoverageOrder.Oid.ToString(), "Incorrect APSEQ# - Sequence Number." );
            Assert.AreEqual( "100", anAccount.AccountNumber.ToString(), "Incorrect APGAR# - Guarantor Number." );
            Assert.AreEqual( "1", coverage.CoverageOrder.Oid.ToString(), "Incorrect APPTY - Priority Code." );
            Assert.AreEqual( "ER MED SCREENING", coverage.InsurancePlan.Payor.Name,
                             "Incorrect APINM - Insurance Company Name." );

            if ( coverage.BillingInformation != null )
            {
                BillingInformation billingInformation = coverage.BillingInformation;
                if ( billingInformation.Address != null )
                {
                    Assert.AreEqual( String.Empty, billingInformation.Address.Address1,
                                     "Incorrect APIAD1 - Mailing Address Line 1." );
                    Assert.AreEqual( String.Empty, billingInformation.Address.Address2,
                                     "Incorrect APIAD2 - Mailing Address Line 2." );
                }
            }
            Assert.AreEqual( "ER MED SCREENING", coverage.InsurancePlan.PlanName,
                             "Incorrect APP#NM - Plan Name." );
            Assert.AreEqual( "1/1/1901", coverage.InsurancePlan.EffectiveOn.ToShortDateString(),
                             "Incorrect APCBGD - Effective Date." );
            Assert.AreEqual( "5/3/2006", coverage.InsurancePlan.ApprovedOn.ToShortDateString(),
                             "Incorrect APPLAD - Approval Date." );
        }

        [Test]
        [Ignore] //"This test is not valid; default PreMSE plan exists for Delray."
        [Description( "VeryLongExecution" )]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestBuildPreMSECoverageException()
        {
            Account anAccount = new Account();
            anAccount.Facility = FacilityBroker.FacilityWith( 6 );
            anAccount.AdmitDate = new DateTime( 2006, 4, 4, 4, 4, 4 );

            i_AccountBroker.BuildCoverageForPreMSEActivity( anAccount );
        }

        [Test]
        public void TestSelectDuplicatePreRegAccountsForMRN()
        {
            const long mrn = 832239;
            DuplicatePreRegAccountsSearchCriteria criteria =
                new DuplicatePreRegAccountsSearchCriteria(
                    ACO_FACILITYID, 0, new Name( "FOR-NUNIT", "PRE-REG", " " ), new SocialSecurityNumber( "230230230" ),
                    new DateTime( 1980, 10, 10 ), mrn, new DateTime( 2022, 07, 05  ) );
            ICollection duplicateAccountsWithMRN = AccountBroker.SelectDuplicatePreRegAccounts( criteria );
            Console.Write( duplicateAccountsWithMRN.Count + "Duplicate accounts found" );
            Assert.IsTrue( duplicateAccountsWithMRN.Count > 0,
                           "Did not find any accounts for all patients types and all admit dates" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        public void TestSelectDuplicatePreRegAccountsForSSN()
        {
            const long mrn = 0;
            DuplicatePreRegAccountsSearchCriteria criteria =
                new DuplicatePreRegAccountsSearchCriteria(
                    ACO_FACILITYID, 0, new Name( "FOR-NUNIT", "PRE-REG", " " ), new SocialSecurityNumber( "230230230" ),
                    new DateTime(1980, 10, 10), mrn, new DateTime(2022, 07, 05));
            criteria.MedicalRecordNumber = 0;
            ICollection duplicateAccountsWithSsn = AccountBroker.SelectDuplicatePreRegAccounts( criteria );
            Console.Write( duplicateAccountsWithSsn.Count + "Duplicate accounts found" );
            Assert.IsTrue( duplicateAccountsWithSsn.Count > 0,
                           "Did not find any accounts for all patients types and all admit dates" );
        }

        [Test]
        [Description( "VeryLongExecution" )]
        [Ignore] //"Need good data!"
        public void TestSelectDuplicatePreRegAccountsForNames()
        {
            const long mrn = 0;
            DuplicatePreRegAccountsSearchCriteria criteria =
                new DuplicatePreRegAccountsSearchCriteria(
                    ACO_FACILITYID, 0, new Name( "PREREG", "TEST", string.Empty ), new SocialSecurityNumber( "254354354" ),
                    new DateTime( 1990, 01, 01 ), mrn, new DateTime( 2007, 12, 10 ) );
            criteria.MedicalRecordNumber = mrn;
            criteria.SocialSecurityNumber.SSNStatus.Description = "KNOWN";

            ICollection duplicateAccountsWithNames = AccountBroker.SelectDuplicatePreRegAccounts( criteria );
            Console.Write( duplicateAccountsWithNames.Count + "Duplicate accounts found" );
            Assert.IsTrue( duplicateAccountsWithNames.Count > 0,
                           "Did not find any accounts for all patients types and all admit dates" );
        }

        [Test]
        public void TestVerifyPatientWasEverAnERTypeInAccountActivityHistory_WhenCreatedAsER_AndCurrentlyOP_ShouldReturnTrue()
        {
            Account account = new Account( 579342 );
            account.Facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            bool isEmergencyPatient = AccountBroker.WasAccountEverAnERType( account );
            Assert.IsTrue( isEmergencyPatient, "Should be True since Patient was ER Patient initially." );
        }

        [Test]
        public void TestVerifyPatientWasEverAnERTypeInAccountActivityHistory_WhenCreatedAsOP_AndCurrentlyOP_ShouldReturnFalse()
        {
            Account account = new Account( 579367 );
            account.Facility = FacilityBroker.FacilityWith( ACO_FACILITYID );
            bool isEmergencyPatient = AccountBroker.WasAccountEverAnERType( account );
            Assert.IsFalse( isEmergencyPatient, "Should be False since Patient was never in ER." );
        }

        [Test]
        public void TestWasEmergencyAccount_WhenCreatedAsOP_AndCurrentlyOP_ShouldReturnFalse()
        {
            IList<string> accountHistory = new List<string> { OUTPATIENT_ACCOUNT_ACTIVITY };

            bool isEmergencyPatient = AccountPbarBroker.WasEmergencyAccount( accountHistory );
            Assert.IsFalse( isEmergencyPatient, "Should be False since Patient was never in ER." );
        }

        [Test]
        public void TestWasEmergencyAccount_WhenCreatedAsIP_AndCurrentlyOP_ShouldReturnFalse()
        {
            IList<string> accountHistory = new List<string> 
                { INPATIENT_ACCOUNT_ACTIVITY,
                  OUTPATIENT_ACCOUNT_ACTIVITY 
                };

            bool isEmergencyPatient = AccountPbarBroker.WasEmergencyAccount( accountHistory );
            Assert.IsFalse( isEmergencyPatient, "Should be False since Patient was never in ER." );
        }

        [Test]
        public void TestWasEmergencyAccount_ForDummyActivity_WithLengthLessThan18_ShouldReturnFalse()
        {
            IList<string> accountHistory = new List<string> { DUMMY_ACCOUNT_ACTIVITY };

            bool isEmergencyPatient = AccountPbarBroker.WasEmergencyAccount( accountHistory );
            Assert.IsFalse( isEmergencyPatient, "Should be False since Patient was never in ER." );
        }

        [Test]
        public void TestWasEmergencyAccount_WhenActivityIsIPToERToOP_ShouldReturnTrue()
        {
            IList<string> accountHistory = new List<string> 
                { INPATIENT_ACCOUNT_ACTIVITY,
                  EMERGENCY_ACCOUNT_ACTIVITY, 
                  OUTPATIENT_ACCOUNT_ACTIVITY 
                };

            bool isEmergencyPatient = AccountPbarBroker.WasEmergencyAccount( accountHistory );
            Assert.IsTrue( isEmergencyPatient, "Should be True since Patient was in ER before." );
        }

        [Test]
        public void TestWasEmergencyAccount_WhenActivityIsERToIPToOP_ShouldReturnTrue()
        {
            IList<string> accountHistory = new List<string> 
                { EMERGENCY_ACCOUNT_ACTIVITY,
                  INPATIENT_ACCOUNT_ACTIVITY,
                  OUTPATIENT_ACCOUNT_ACTIVITY 
                };

            bool isEmergencyPatient = AccountPbarBroker.WasEmergencyAccount( accountHistory );
            Assert.IsTrue( isEmergencyPatient, "Should be True since Patient was in ER before." );
        }

        [Test]
        public void TestGetNewAccountNumberFor_DirectCallToPbar_ShouldNotThrowException()
        {
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            AccountBroker.GetNewAccountNumberFor( facility );
        }

        [Test]
        public void TestGetNewAccountNumberFor_PbarReturnsPositiveInteger_ShouldNotThrowException()
        {
            const int mockExpectedNewAccountNumber = 1;
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            var accountBroker = GetMockAccountBrokerWherePbarReturnsNewAccountNumber( mockExpectedNewAccountNumber );

            var expectedNewAccountNumber = accountBroker.GetNewAccountNumberFor( facility );

            Assert.AreEqual( mockExpectedNewAccountNumber, expectedNewAccountNumber );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewAccountNumberFor_PbarReturnsZero_ShouldThrowException()
        {
            const int pbarNewAccountNumber = 0;
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            var accountBroker = GetMockAccountBrokerWherePbarReturnsNewAccountNumber( pbarNewAccountNumber );

            accountBroker.GetNewAccountNumberFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewAccountNumberFor_PbarReturnsNegativeInteger_ShouldThrowException()
        {
            const int pbarNewAccountNumber = -1;
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            var accountBroker = GetMockAccountBrokerWherePbarReturnsNewAccountNumber( pbarNewAccountNumber );

            accountBroker.GetNewAccountNumberFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewAccountNumberFor_PbarReturnsNull_ShouldThrowException()
        {
            const object pbarNewAccountNumber = null;
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            var accountBroker = GetMockAccountBrokerWherePbarReturnsNewAccountNumber( pbarNewAccountNumber );

            accountBroker.GetNewAccountNumberFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewAccountNumberFor_PbarReturnsNonInteger_ShouldThrowException()
        {
            const string pbarNewAccountNumber = "a";
            var facility = FacilityBroker.FacilityWith( HSP_CODE );
            var accountBroker = GetMockAccountBrokerWherePbarReturnsNewAccountNumber( pbarNewAccountNumber );

            accountBroker.GetNewAccountNumberFor( facility );
        }

        [Test, Sequential]
        public void TestSetportalOptInAndRightToRestrict(
            [Values("Y", " ")] string patientPortalOptIn,
            [Values("Y", " ")] string rightToRestrict,
            [Values("N", "Y")] string ShareDataWithPublicHIE,
            [Values("Y", "N")] string ShareDataWithPCP)
        {
            Account account = new Account(579342);
            account.Facility = FacilityBroker.FacilityWith(ACO_FACILITYID);
            string LPFILL = patientPortalOptIn + rightToRestrict + ShareDataWithPublicHIE + ShareDataWithPCP;
            AccountPbarBroker.SetPortalOptInAndRightToRestrictValue(account, LPFILL);

            Assert.AreEqual(account.PatientPortalOptIn.Code, patientPortalOptIn,
                            string.Format("patientPortalOptIn should be set to {0}", patientPortalOptIn));

            Assert.AreEqual(account.RightToRestrict.Code, rightToRestrict,
                            string.Format("RightToRestrict Should be set to {0}", rightToRestrict));

            Assert.AreEqual(account.ShareDataWithPublicHieFlag.Code, ShareDataWithPublicHIE,
                            string.Format("ShareDataWithPublicHIE should be set to {0}", ShareDataWithPublicHIE));

            Assert.AreEqual(account.ShareDataWithPCPFlag.Code, ShareDataWithPCP,
                            string.Format("ShareDataWithPCP Should be set to {0}", ShareDataWithPCP));
        }

        [Test, Sequential]
        public void TestSetPortalOptIn(
            [Values("Y", " ")] string patientPortalOptIn,
            [Values("N", "N")] string rightToRestrict)
        {
            var ap = new AccountProxy();
            string LPFILL = patientPortalOptIn + rightToRestrict;
            AccountPbarBroker.SetPortalOptIn(ap,LPFILL);
            Assert.AreEqual(ap.PatientPortalOptIn.Code, patientPortalOptIn,
                string.Format("PatientPortalOptIn should be set to {0}",patientPortalOptIn));
        }
        [Test, Sequential]
        public void TestSetUnablePortalOptIn(
            [Values("U", " ")] string patientPortalOptIn,
            [Values("N", "N")] string rightToRestrict)
        {
            var ap = new AccountProxy();
            string LPFILL = patientPortalOptIn + rightToRestrict;
            AccountPbarBroker.SetPortalOptIn(ap, LPFILL);
            Assert.AreEqual(ap.PatientPortalOptIn.Code, patientPortalOptIn,
                string.Format("PatientPortalOptIn should be set to {0}", patientPortalOptIn));
        }
        #endregion


        #region Support Methods

        private static AccountPBARBroker GetMockAccountBrokerWherePbarReturnsNewAccountNumber( object pbarRetrunValue )
        {
            MockRepository mockRepository = new MockRepository();

            AccountPBARBroker accountBroker = ( AccountPBARBroker )mockRepository.PartialMock( typeof( AccountPBARBroker ) );
            accountBroker.Expect( x => x.TryExecutingNewAccountNumberStoredProcedure( Arg<IDbCommand>.Is.Anything ) ).Return( pbarRetrunValue );
            mockRepository.ReplayAll();


            return accountBroker;
        }

        /// <summary>
        /// Tests the get patient driving license info.
        /// </summary>
        /// <param name="driversLicenseInfo">The drivers license info.</param>
        /// <param name="expectedNumber">The expected number.</param>
        /// <param name="expectedStateCode">The expected state code.</param>
        private void TestGetPatientDrivingLicenseInfo( string driversLicenseInfo, string expectedNumber, string expectedStateCode )
        {

            MethodInfo getPatientDrivingLicenseInfoMethod =
                typeof( AccountPBARBroker )
                    .GetMethod( "GetPatientDrivingLicenseInfo",
                               BindingFlags.NonPublic | BindingFlags.Instance );

            DriversLicense testResult =
                getPatientDrivingLicenseInfoMethod.Invoke( AccountBroker,
                                                          new object[] { driversLicenseInfo,98 } )
                as DriversLicense;

            Assert.IsNotNull( testResult );
            Assert.AreEqual( expectedNumber,
                            testResult.Number,
                            "License number did not match" );

            Assert.AreEqual( expectedStateCode,
                            testResult.State.Code,
                            "State code did not match" );

        }//method

        private IAccountBroker AccountBroker
        {
            get
            {
                return i_AccountBroker;
            }
            set
            {
                i_AccountBroker = value;
            }
        }

        private AccountPBARBroker AccountPbarBroker { get; set; }

        private IFacilityBroker FacilityBroker
        {
            get
            {
                return i_FacilityBroker;
            }
            set
            {
                i_FacilityBroker = value;
            }
        }

        private IFinancialClassesBroker FinancialClassesBroker
        {
            get
            {
                return i_FinancialClassesBroker;
            }
            set
            {
                i_FinancialClassesBroker = value;
            }
        }
        #endregion

        #region Data Elements
        private IFacilityBroker i_FacilityBroker;
        private IFinancialClassesBroker i_FinancialClassesBroker;
        private IAccountBroker i_AccountBroker;
        private Facility i_Facility;
        private Facility i_ACOFacility;

        #endregion
    }
}