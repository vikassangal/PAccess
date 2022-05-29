using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for PatientSearchBrokerTests.
    /// </summary>
    [TestFixture()]
    public class PatientSearchPBARBrokerTests : AbstractBrokerTests
    {

        #region SetUp and TearDown PatientSearchBrokerTests
        [SetUp]
        public void SetUpPatientSearchBrokerTests()
        {
            i_PatientSearchBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            i_DemographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            
            femaleGender = i_DemographicsBroker.GenderWith(ACO_FACILITYID, "F");
            blankGender = i_DemographicsBroker.GenderWith(ACO_FACILITYID, " ");
            
        }

        #endregion

        #region Test Methods


        [Test()]
        public void TestPatientsBroker()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                TESTACOHSPCODE,
                TESTALIAS2FIRSTNAME,
                TESTALIAS2LASTNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patients = this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsNotNull(patients, "Patient list is empty");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "There are no results from the Patients search");
            bool found = false;
            TypeOfName aliasType = TypeOfName.Normal;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.Name.LastName.Equals(TESTALIAS2LASTNAME) &&
                    patientSearchResult.Name.FirstName.Equals(TESTALIAS2FIRSTNAME))
                {
                    aliasType = patientSearchResult.Name.TypeOfName;
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                Assert.Fail("Did not find patient expected by Name search");
            }
            else
            {
                Assert.IsTrue(aliasType == TypeOfName.Alias, "Wrong TypeOfName");
            }

        }

        [Test()]
        public void TestAlias()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE_ICE,
                "MIA",
                "SOLA",
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patients = this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsNotNull(patients.PatientSearchResults, "Patient list is empty");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "There are no results from the Patients search");
            bool found = false;

            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.Name.LastName.Equals(EXPECTED_LASTNAME2) &&
                    patientSearchResult.Name.FirstName.Equals(EXPECTED_FIRSTNAME2) &&
                    patientSearchResult.Name.TypeOfName.Equals(TypeOfName.Normal))
                {
                    foreach (Name n in patientSearchResult.AkaNames)
                    {
                        if (n.FirstName.Equals(EXPECTED_ALIAS_FIRST_NAME) &&
                            n.LastName.Equals(EXPECTED_ALIAS_LAST_NAME) &&
                            n.TypeOfName.Equals(TypeOfName.Alias))
                        {
                            found = true;
                            break; // break aliases

                        }
                    }
                }
            }
            if (found == false)
            {
                Assert.Fail("Did not find expected Alias.");
            }
        }

        [Test()]
        public void TestSpecificSSN()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                TEST_SSNSTR2,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Did not find expected patient");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found to many patients");

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];
            Assert.AreEqual(
                EXPECTED_MEDICAL_RECORD_NUMBER_2,
                patientSearchResult.MedicalRecordNumber,
                "MedicalRecordNumber should be " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
            Assert.AreEqual(
                EXPECTED_LASTNAME,
                patientSearchResult.Name.LastName,
                "Patient Last Name should be " + EXPECTED_LASTNAME);
            Assert.AreEqual(
                TEST_SSNSTR2,
                patientSearchResult.SocialSecurityNumber,
                "Patient SSN Name should be " + TEST_SSNSTR2);
        }

        [Test()]
        //[Ignore("Kevin - Ignore to get Build going by COB")]
        public void TestPatientWithPhysicalAddress()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                TEST_SSNWITHPHYADDR,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Did not find expected patient");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found to many patients");

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel().ToUpper(),
                patientSearchResult.Address.OneLineAddressLabel().ToUpper(),
                "Physical Addresses should match");
        }
        [Test()]
        //[Ignore("DB team is researching on this data issue - SKIP")]
        public void TestPatientWithMailingAddress()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                TEST_SSNWITHMAILADDR,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Did not find expected patient");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found to many patients");

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel(),
                patientSearchResult.Address.OneLineAddressLabel(),
                "Mailing Addresses should match");
        }

        [Test()]
        public void TestPatientWithNoAddress()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                TEST_SSNWITHNOADDR,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];
            Address address = patientSearchResult.Address;
            Assert.IsTrue(address.Address1.Trim() == String.Empty, "Address should be blank");
            Assert.IsTrue(address.City.Trim() == String.Empty, "City should be blank");
            Assert.IsTrue(address.State.Code.Trim() == String.Empty, "State code should be blank");
        }

        [Test()]
        //[Ignore("Mailing Address has changed in Database")]
        public void TestGuarantorWithMailingAddress()
        {

            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                null,
                TEST_GRNTRWITHMAILADDR);
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel(),
                patientSearchResult.Address.OneLineAddressLabel(),
                "Mailing Addresses should match");
        }


        [Test()]
        public void TestSpecificSSNFail()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                "987654320",
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );
            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count == 0, "There should not be any patients for this test");
        }
        [Test()]
        public void TestNameWithGender()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FIRSTNAME,
                LASTNAME,
                String.Empty,
                femaleGender,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found too many patients");

            bool found = false;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found,
                           "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
        }

        [Test()]
        public void TestNameWithBlankGender()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FULLFIRSTNAME,
                FULLLASTNAME,
                String.Empty,
                blankGender,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "Found to many patient");

            bool found = false;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found,
                           "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
        }

        [Test()]
        public void TestPatientWithDate()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FIRSTNAME,
                LASTNAME,
                String.Empty,
                femaleGender,
                TEST_EXPECTED_MONTH,
                TEST_EXPECTED_YEAR_CLOSE,
                null,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsNotNull(patients.PatientSearchResults, "Patient list should not be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "Did not find any patents with specific date.");

            bool found = false;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found,
                           "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
        }

        [Test()]
        public void TestPatientWithDateYearOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FIRSTNAME,
                LASTNAME,
                String.Empty,
                femaleGender,
                PatientSearchCriteria.NO_MONTH,
                TEST_EXPECTED_YEAR_CLOSE,
                null,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Did not find expected patient");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "Did not find any patents with specific date.");

            bool found = false;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found,
                           "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
        }

        [Test()]
        public void TestPatientWithDateMonthOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FIRSTNAME,
                LASTNAME,
                String.Empty,
                femaleGender,
                TEST_EXPECTED_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patients = this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count >= 1, "Did not find any patents with specific date.");

            bool found = false;
            foreach (PatientSearchResult patientSearchResult in patients.PatientSearchResults)
            {
                if (patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2);
        }

        [Test()]
        public void TestSpecificMRN()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found to many patient");

            PatientSearchResult patientSearchResult = (PatientSearchResult)patients.PatientSearchResults[0];

            Assert.IsTrue(patientSearchResult.Name.FirstName.Equals(EXPECTED_FIRSTNAME),
                           "FirstName is not correct");
            Assert.IsTrue(patientSearchResult.Name.LastName.Equals(EXPECTED_LASTNAME),
                           "LastName is not correct");
        }

        [Test()]
        public void TestSpecificMRNFail()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                TEST_BADMRN,
                null
                );

            PatientSearchResponse patients = (PatientSearchResponse)this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should not be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count == 0, "Found patient but should not have.");
        }

        [Test()]
        public void TestSpecificAcctNbr()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                TEST_ACCT_NBR
                );

            PatientSearchResponse patients = this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patients.PatientSearchResults != null, "Patient list should no be null.");
            Assert.IsTrue(patients.PatientSearchResults.Count == 1, "Found too many patients");

            PatientSearchResult patientSearchResult = patients.PatientSearchResults[0];


            Assert.AreEqual(EXPECTED_FIRSTNAME, patientSearchResult.Name.FirstName,
                             "FirstName should be " + EXPECTED_FIRSTNAME);
            Assert.AreEqual(EXPECTED_LASTNAME, patientSearchResult.Name.LastName,
                             "LastName should be " + EXPECTED_LASTNAME);
            Assert.AreEqual(this.EXPECTED_SSN.AreaNumber, new SocialSecurityNumber(patientSearchResult.SocialSecurityNumber).AreaNumber,
                             "SSN should be " + this.EXPECTED_SSN.AreaNumber);
            Assert.AreEqual(femaleGender, patientSearchResult.Gender,
                             "Patient Sex should be " + femaleGender.Description);
            Assert.AreEqual(this.EXPECTED_DOB, patientSearchResult.DateOfBirth,
                             "Patient DOB should be " + this.EXPECTED_DOB.ToShortDateString());
        }



        ////
        //// Begin Guarentor Tests
        ////
        [Test()]
        public void TestGuarantorSearch()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                GUARANTORFIRSTNAME,
                GUARANTORLASTNAME,
                null,
                String.Empty);

            PatientSearchResponse patientSearchResponse = this.PatientSearchBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patientSearchResponse.PatientSearchResults.Count >= 1, "There are no patients from the patients guarantor search");
        }



        #endregion


        #region Support Methods

        private IPatientBroker PatientSearchBroker
        {
            get
            {
                return i_PatientSearchBroker;
            }
            set
            {
                i_PatientSearchBroker = value;
            }
        }


        #endregion

        #region Data Elements

        private static IPatientBroker i_PatientSearchBroker = null;
        private static IDemographicsBroker i_DemographicsBroker = null;
        
        private static Gender femaleGender = null;
        
        private static Gender blankGender = null;

        #endregion

        #region Constants

        private const string
            HSPCODE_ACO = "ACO",
            FC_37 = "37",
            FINANCIAL_CLASS_CODE_02 = "02",
            LOCATION_BED_CODE_B4 = "B4";

        private static readonly string LOCATION_BED_CODE_EMPTY = string.Empty;

        private const long
            TEST_ACCT_NUMBER = 52514;

        private const string
            EXPECTED_MEDICAL_RECORD_NUMBER = "785138";

        private const string
            LASTNAME = "MINTZ",
            FIRSTNAME = "SYLVIA",

            FULLLASTNAME = "MINTZ",
            FULLFIRSTNAME = "SYLVIA",

            GUARANTORLASTNAMEFAIL = "XX",

            GUARANTORLASTNAME = "HALBROOKS",
            GUARANTORFIRSTNAME = "MOTHER",

            HSPCODE = "DEL",
            HSPCODE_ICE = "ICE",

            TEST_MRN = "299",
            TEST_MRN_WITH_NO_ACCT = "786484",
            TEST_BADMRN = "0",

            TEST_ACCT_NBR = "5345740",

            TEST_SSNSTR = "182736473",
            TEST_SSNSTR2 = "075146408",
            TEST_BADSSNSTR = "111111111",
            TEST_SSNWITHMAILADDR = "879879679", //"057107072",
            TEST_SSNWITHPHYADDR = "879879679", //"057107072",
            TEST_SSNWITHNOADDR = "896757967", //"776877768",
            TEST_GRNTRWITHMAILADDR = "879879679"; //"057107072";

        private const string
            OLD_EMERGENCY_PATIENT_DESC = "OP-ER PAT";

        private const string
            EXPECTED_LASTNAME = "MINTZ",
            EXPECTED_FIRSTNAME = "SYLVIA",
            EXPECTED_LASTNAME2 = "SOLA",
            EXPECTED_FIRSTNAME2 = "MIA",
            EXPECTED_ALIAS_FIRST_NAME = "SIMONETTA",
            EXPECTED_ALIAS_LAST_NAME = "MARTINEZ",
            EXPECTED_COUNTRY = "United States Of America",
            EXPECTED_COUNTRY_CODE = "USA",
            EXPECTED_NO_RESULT_FOUND = "-1";

        private const int
            EXPECTED_NUM_ENTRIES_1 = 1,
            EXPECTED_NUM_ENTRIES_3 = 3;

        private const string
            MESSAGE_ERROR_CANNOT_FIND_MRN = "Did not find expected Medical Record Number for given account",
            MESSAGE_SHOULD_NOT_BE_NULL = "Should not be null.",
            MESSAGE_INCORRECT_NUM_ENTRIES_1 = "Incorrect number of entries in returned ArrayList: should be 1 entries.",
            MESSAGE_INCORRECT_NUM_ENTRIES_3 = "Incorrect number of entries in returned ArrayList: should be 3 entries.";

        private const long
            TEST_EXPECTED_ACCOUNTS_COUNT = 5,
            TEST_EXPECTED_YEAR_CLOSE = 1919,
            TEST_EXPECTED_MONTH = 1,
            TEST_INCORRECT_ACCT_NUMBER = 512514,
            TEST_NEGATIVE_MRN = -17823,
            EXPECTED_OID = 0,
            EXPECTED_MEDICAL_RECORD_NUMBER_1 = 785933,
            EXPECTED_MEDICAL_RECORD_NUMBER_2 = 299,
            HSPCODE_ACO_OID_EQUIVALENT = 900;

        private SocialSecurityNumber EXPECTED_SSN = new SocialSecurityNumber("075146408");

        private DateTime EXPECTED_DOB = new DateTime(1919, 1, 9);
        private DateTime EXPECTED_GUARANTOR_DOB = new DateTime(1980, 10, 10);


        private const string
            EXPECTED_STREET_ADDRESS = "2700 W PLANO PKWY",
            EXPECTED_NEW_CITY = "PLANO",
            EXPECTED_NEW_STATE = "TEXAS",
            EXPECTED_NEW_STATE_CODE = "TX",
            EXPECTED_NEW_ZIP_CODE = "75075";

        private Address EXPECTED_ADDRESS = new Address(EXPECTED_STREET_ADDRESS,
                                                        string.Empty,
                                                        EXPECTED_NEW_CITY,
                                                        new ZipCode(EXPECTED_NEW_ZIP_CODE),
                                                        new State(EXPECTED_OID,
                                                                   EXPECTED_NEW_STATE,
                                                                   EXPECTED_NEW_STATE_CODE),
                                                        new Country(EXPECTED_OID,
                                                                     EXPECTED_COUNTRY,
                                                                     EXPECTED_COUNTRY_CODE)
            );
        private const string
            TESTACOHSPCODE = "ACO",
            TESTFIRSTNAME = "SR52223",
            TESTALIAS1FIRSTNAME = "BOB",
            TESTALIAS2FIRSTNAME = "RICK",
            TESTLASTNAME = "Test",
            TESTALIAS1LASTNAME = "NORMAN",
            TESTALIAS2LASTNAME = "WAUGH";


        #endregion

    }
}