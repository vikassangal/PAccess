using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Extensions.Exceptions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using Rhino.Mocks;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class PatientPBARBrokerTests : AbstractBrokerTests
    {
        #region SetUp and TearDown PatientBrokerTests
        [SetUp]
        public void SetUpPatientBrokerTests()
        {
            PatientBroker = BrokerFactory.BrokerOfType<IPatientBroker>();
            DemographicsBroker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
            FacilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            femaleGender = DemographicsBroker.GenderWith( ACO_FACILITYID, "F" );
            blankGender = DemographicsBroker.GenderWith( ACO_FACILITYID, " " );
            maleGender = DemographicsBroker.GenderWith( ACO_FACILITYID, "M" );
        }

        #endregion

        #region Test Methods
        [Test]
        public void TestGetIpaForPatient()
        {
            IAccount anAccount = new Account { AccountNumber = 4477677 };
            anAccount.Patient.MedicalRecordNumber = 24004;
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            anAccount.Facility = facilityBroker.FacilityWith( DEL_FACILITY_CODE );
            var ipa = PatientBroker.GetIpaForPatient( anAccount.Patient );

            Assert.IsNotNullOrEmpty( ipa.Code, "Ipa code should not be null or empty" );
        }

        [Test]
        public void TestMrnForAccount()
        {
            // success
            long medicalRecordNumber = this.PatientBroker.MRNForAccount( TEST_ACCT_NUMBER, HSPCODE_ACO );
            Assert.AreEqual( EXPECTED_MEDICAL_RECORD_NUMBER, medicalRecordNumber.ToString(), MESSAGE_ERROR_CANNOT_FIND_MRN );

            // Boundary conditions:
            // negative number
            medicalRecordNumber = this.PatientBroker.MRNForAccount( TEST_NEGATIVE_MRN, HSPCODE_ACO );
            Assert.AreEqual( EXPECTED_NO_RESULT_FOUND, medicalRecordNumber.ToString(), MESSAGE_ERROR_CANNOT_FIND_MRN );

            // invalid account number
            medicalRecordNumber = this.PatientBroker.MRNForAccount( TEST_INCORRECT_ACCT_NUMBER, HSPCODE_ACO );
            Assert.AreEqual( EXPECTED_NO_RESULT_FOUND, medicalRecordNumber.ToString(), MESSAGE_ERROR_CANNOT_FIND_MRN );
        }

        [Test]
        public virtual void TestSparsePatient()
        {
            Patient patient = this.TestSparsePatientHelper();
            Assert.IsNotNull( patient, "Patient Not found using SparsePatientWith" );
        }

        private Patient TestSparsePatientHelper()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            return this.PatientBroker.SparsePatientWith( 785138, facility.Code );
        }

        [Test]
        public void TestPatientsBroker()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                FIRSTNAME,
                LASTNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                null,
                null
                );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );
            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {

                if ( patientSearchResult.Name.LastName.Equals( EXPECTED_LASTNAME ) &&
                    patientSearchResult.Name.FirstName.Equals( EXPECTED_FIRSTNAME ) )
                {
                    found = true;
                    break;
                }
            }
            if ( found == false )
            {
                Assert.Fail( "Did not find patient expected by Name search" );
            }
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );
            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.Name.LastName.Equals( EXPECTED_LASTNAME2 ) &&
                    patientSearchResult.Name.FirstName.Equals( EXPECTED_FIRSTNAME2 ) )
                {
                    foreach ( Name n in patientSearchResult.AkaNames )
                    {
                        if ( n.FirstName.Equals( EXPECTED_ALIAS_FIRST_NAME ) &&
                            n.LastName.Equals( EXPECTED_ALIAS_LAST_NAME ) )
                        {
                            found = true;
                            break; // break aliases

                        }
                    }
                }
            }
            if ( found == false )
            {
                Assert.Fail( "Did not find expected Alias." );
            }
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];
            Assert.AreEqual(
                EXPECTED_MEDICAL_RECORD_NUMBER_2,
                patientSearchResult.MedicalRecordNumber,
                "MedicalRecordNumber should be " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
            Assert.AreEqual(
                EXPECTED_LASTNAME,
                patientSearchResult.Name.LastName,
                "Patient Last Name should be " + EXPECTED_LASTNAME );
        }

        [Test]
        public void TestSearchByPhoneNumber()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                null,
                null,
                TEST_PHONENUMBER
                );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor(criteria);

            Assert.IsTrue(patientSearchResponse.PatientSearchResults.Count >= 0,
                "There are no results from the Patients search with Phone Number");
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel().ToUpper(),
                patientSearchResult.Address.OneLineAddressLabel().ToUpper(),
                "Physical Addresses should match" );
        }

        [Test]
        public void TestGuarantorWithMailingAddress()
        {

            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                null,
                TEST_GRNTRWITHMAILADDR );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel(),
                patientSearchResult.Address.OneLineAddressLabel(),
                "Mailing Addresses should match" );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.AreEqual(
                this.EXPECTED_ADDRESS.OneLineAddressLabel(),
                patientSearchResult.Address.OneLineAddressLabel(),
                "Mailing Addresses should match" );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 0, "There are no results from the Patients search" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.IsTrue( string.IsNullOrEmpty( patientSearchResult.Address.OneLineAddressLabel() ), "Address should be blank" );
        }

        [Test]
        public void TestSpecificSsnFail()
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 0, "There should not be any patients for this test" );

        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.IsTrue( patientSearchResult.Name.FirstName.Equals( EXPECTED_FIRSTNAME ),
                           "FirstName is not correct" );
            Assert.IsTrue( patientSearchResult.Name.LastName.Equals( EXPECTED_LASTNAME ),
                           "LastName is not correct" );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 0, "Found patient but should not have." );

        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Assert.AreEqual( EXPECTED_FIRSTNAME, patientSearchResult.Name.FirstName,
                             "FirstName should be " + EXPECTED_FIRSTNAME );
            Assert.AreEqual( EXPECTED_LASTNAME, patientSearchResult.Name.LastName,
                             "LastName should be " + EXPECTED_LASTNAME );
            Assert.AreEqual( this.EXPECTED_SSN.UnformattedSocialSecurityNumber, patientSearchResult.SocialSecurityNumber );
            Assert.AreEqual( femaleGender, patientSearchResult.Gender,
                             "Patient Sex should be " + femaleGender.Description );
            Assert.AreEqual( this.EXPECTED_DOB, patientSearchResult.DateOfBirth,
                             "Patient DOB should be " + this.EXPECTED_DOB.ToShortDateString() );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2 )
                {
                    found = true;
                }
            }
            Assert.IsTrue( found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2 )
                {
                    found = true;
                }
            }
            Assert.IsTrue( found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2 )
                {
                    found = true;
                }
            }
            Assert.IsTrue( found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2 )
                {
                    found = true;
                }
            }
            Assert.IsTrue( found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
        }

        [Test]
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

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_2 )
                {
                    found = true;
                }
            }
            Assert.IsTrue( found, "Did not find expected patient with MRN " + EXPECTED_MEDICAL_RECORD_NUMBER_2 );
        }

        //
        // Begin Guarentor Tests
        //
        [Test]
        public void TestGuarantorSearch()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                GUARANTORFIRSTNAME,
                GUARANTORLASTNAME,
                null,
                String.Empty );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 1, "There are no patients from the patients guarantor search" );
        }

        [Test]
        public void TestGuarantorSearchWithGender()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                GUARANTORFIRSTNAME,
                GUARANTORLASTNAME,
                femaleGender,
                String.Empty );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 1, "There are no patients from the patients guarantor search" );

        }

        [Test]
        public void TestGuarantorSearchWithGenderFail()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                GUARANTORFIRSTNAME,
                GUARANTORLASTNAME,
                maleGender,
                String.Empty );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 0, "There are no patients from the patients guarantor search" );

        }

        [Test]
        public void TestGuarantorSearchSSN()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria( HSPCODE_ACO, null, null, null, TEST_SSNSTR );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 1,
                           "There are no patients from the patients guarantor search" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == EXPECTED_MEDICAL_RECORD_NUMBER_1 )
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue( found, "Did not find expected patients" );

        }


        [Test]
        public void TestGuarantorSearchSsnFail()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE,
                null,
                null,
                null,
                TEST_BADSSNSTR );
            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 0,
                           "There are patients from the patients guarantor search but there should not be any" );
        }

        [Test]
        public void TestGuarantorSearchALL()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE_ACO,
                "CHRIST",
                "BERNARD",
                maleGender,
                "999999999" );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count >= 1,
                           "There are no patients from the patients guarantor search" );

            bool found = false;
            foreach ( PatientSearchResult patientSearchResult in patientSearchResponse.PatientSearchResults )
            {
                if ( patientSearchResult.MedicalRecordNumber == 786005 )
                {
                    Assert.AreEqual( "CHRIST", patientSearchResult.Name.FirstName,
                                     "FirstName should be " + "CHRIST D" );
                    Assert.AreEqual( "BERNARD", patientSearchResult.Name.LastName,
                                     "LastName should be " + "BERNARD" );
                    Assert.AreEqual( maleGender, patientSearchResult.Gender,
                                     "Patient Sex should be " + maleGender.Description );
                    Assert.AreEqual( this.EXPECTED_GUARANTOR_DOB, patientSearchResult.DateOfBirth,
                                     "Patient DOB should be " + this.EXPECTED_GUARANTOR_DOB.ToShortDateString() );
                    found = true;
                    break;
                }
            }

            Assert.IsTrue( found, "Did not find expected patients" );

        }

        [Test]
        public void TestGuarantorSearchFail()
        {
            GuarantorSearchCriteria criteria = new GuarantorSearchCriteria(
                HSPCODE,
                null,
                GUARANTORLASTNAMEFAIL,
                null,
                String.Empty );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 0, "There should be no patients from the patients guarantor search" );

        }

        [Test]
        public void TestPatientFrom()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria( HSPCODE, null, null, String.Empty, null,
                                                                        PatientSearchCriteria.NO_MONTH,
                                                                        PatientSearchCriteria.NO_YEAR, TEST_MRN, null );
            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Patient patient = this.PatientBroker.PatientFrom( patientSearchResult );

            Assert.IsTrue( patient != null, "Could not convert patient from proxy" );
            Assert.IsTrue( patient.Accounts != null, "Did not find expected accounts for patient" );
            Assert.AreEqual( TEST_EXPECTED_ACCOUNTS_COUNT,
                             patient.Accounts.Count,
                             "Did not find the proper number of accouts" );
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestPatientFromProxyWithNullParameterToForceException()
        {
            this.PatientBroker.PatientFrom( null );
        }

        [Test]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void TestPatientFromPatientSearchResultWithNullParameterToForceException()
        {
            this.PatientBroker.PatientFrom( null );
        }

        [Test]
        public void TestPatientEmployers()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                "27540",
                null
                );

            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Patient patient = this.PatientBroker.PatientFrom( patientSearchResult );
            Assert.IsNotNull( patient.Employment, "Did not find expected patients Employment" );
            Assert.IsNotNull( patient.Employment.Employer, "Did not find the Employer for patient" );
        }

        [Test]
        public void TestPatientFromWithNoAccts()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSPCODE_ACO,
                null,
                null,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN_WITH_NO_ACCT,
                null
                );
            PatientSearchResponse patientSearchResponse = this.PatientBroker.GetPatientSearchResponseFor( criteria );

            Assert.IsTrue( patientSearchResponse.PatientSearchResults.Count == 1, "Found too many patients" );

            PatientSearchResult patientSearchResult = patientSearchResponse.PatientSearchResults[0];

            Patient patient = this.PatientBroker.PatientFrom( patientSearchResult );
            Assert.IsNotNull( patient, "Could not get patient" );
            Assert.IsNotNull( patient.Accounts, "Did not find expected accounts for patient" );
            Assert.AreEqual( 0, patient.Accounts.Count, "Should not have found any accounts for this patient" );
        }

        [Test]
        public void TestPatientTypes()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );

            VisitType preadmit = null;
            VisitType inpatient = null;
            ArrayList allTypes = (ArrayList)this.PatientBroker.AllPatientTypes( facility.Oid );
            foreach ( VisitType type in allTypes )
            {
                switch ( type.Code )
                {
                    case "0":
                        preadmit = type;
                        break;
                    case "1":
                        inpatient = type;
                        break;
                    case "2":
                        break;
                    default:
                        break;
                }
            }
            Assert.AreEqual( VisitType.PREREG_PATIENT_DESC, preadmit.Description, "PatientType 0 should be PREADMIT " );

            Assert.AreEqual( VisitType.INPATIENT_DESC, inpatient.Description, "PatientType 0 should be INPATIENT  " );

            Assert.AreEqual( VisitType.OUTPATIENT_DESC, this.PatientBroker.PatientTypeWith( facility.Oid, "2" ).Description,
                "PatientType 2 should be OUTPATIENT" );
        }

        [Test]
        public void TestPatientTypeForBlank()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );

            string blank = string.Empty;

            VisitType vt = this.PatientBroker.PatientTypeWith( facility.Oid, blank );

            Assert.AreEqual( blank, vt.Code, "Code should be blank" );
            Assert.AreEqual( blank, vt.Description, "Description should be blank" );
            Assert.IsTrue( vt.IsValid );
        }

        [Test]
        public void TestPatientTypeForInvalid()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            const string inValid = "5";

            VisitType vt = this.PatientBroker.PatientTypeWith( facility.Oid, inValid );

            Assert.IsFalse( vt.IsValid );
        }

        [Test]
        public void TestGetPatientTypesList()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            string kindOfVisitCode = VisitType.PREREG_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                kindOfVisitCode,
                                                                financialClassCode,
                                                                locationBedCode,
                                                                facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.PREREG_PATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'PREADMIT' has not been found for MaintenanceActivity" );

            activity = new RegistrationActivity();
            kindOfVisitCode = VisitType.INPATIENT;
            found = false;
            patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                kindOfVisitCode,
                                                                financialClassCode,
                                                                locationBedCode,
                                                                facility.Oid );
            Assert.AreEqual( 6, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );
            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.INPATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'INPATIENT' has not been found for RegistrationActivity" );
        }

        [Test]
        public void TestGetPatientTypesListWithNullParameters()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( null, null, null, null, null, facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
        }

        [Test]
        public void TestPatientTypesForMaintenanceActivityCardinalityVisitTypePreReg()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.PREREG_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            // Maintenance Activities only, see below for Registration 
            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );

            // ---  Test PREADMIT activity. --- 
            // PREREG and PREADMIT essentially  convey the same concept and are thus used interchangeably 
            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.PREREG_PATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'PREADMIT' has not been found for MaintenanceActivity" );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeInPatient()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.INPATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.INPATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'INPATIENT' has not been found for Maintenance" );
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeRecurring()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.RECURRING_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.RECURRING_PATIENT_DESC ) )
                    found = true;
            Assert.IsTrue(found, "Visit Type 'RCRNG O/PT' has not been found for Maintenance");
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeNonPatient()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.NON_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.NON_PATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'NONPATIENT' has not been found for Maintenance" );
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeEmergencyPatient()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.EMERGENCY_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_3, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_3 );

            foreach ( VisitType vt in patientTypes )
                if (vt.Description.Equals("ER PATIENT"))
                    found = true;
            Assert.IsTrue(found, "Visit Type 'ER PATIENT' has not been found for Maintenance");
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeEmergencyPatientFC37()
        {
            // test special case of financial class code being set to financial class code 37 
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.EMERGENCY_PATIENT;
            const string financialClassCode = FC_37;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;
            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            foreach ( VisitType vt in patientTypes )
                if (vt.Description.Equals("ER PATIENT"))
                    found = true;
            Assert.IsTrue(found, "Visit Type 'ER PATIENT' has not been found for Maintenance");
        }

        [Test]
        public void TestPatientTypesForCardinalityOfReturnedArrayListVisitTypeOutPatientEmptyLocationBedCase()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new MaintenanceActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.OUTPATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;
            bool found = false;

            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_3, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_3 );

            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.OUTPATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'OUTPATIENT' has not been found for Maintenance" );

            // test locationBedCode with value B4
            locationBedCode = LOCATION_BED_CODE_B4;
            found = false;

            patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                              kindOfVisitCode,
                                                              financialClassCode,
                                                              locationBedCode,
                                                              facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            foreach ( VisitType vt in patientTypes )
                if ( vt.Description.Equals( VisitType.OUTPATIENT_DESC ) )
                    found = true;
            Assert.IsTrue( found, "Visit Type 'OUTPATIENT' has not been found for Maintenance" );
        }

        [Test]
        public void TestPatientTypesForRegistrationCardinalityActivityTypes()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new PreRegistrationActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.PREREG_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;

            // Test PreRegistrationActivity
            ArrayList patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            // Test AdmitNewbornActivity
            activity = new AdmitNewbornActivity();
            patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                              kindOfVisitCode,
                                                              financialClassCode,
                                                              locationBedCode,
                                                              facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            // Test PreMSERegisterActivity
            activity = new PreMSERegisterActivity();
            patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                              kindOfVisitCode,
                                                              financialClassCode,
                                                              locationBedCode,
                                                              facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );

            // Test PostMSERegistrationActivity
            activity = new PostMSERegistrationActivity();
            patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                              kindOfVisitCode,
                                                              financialClassCode,
                                                              locationBedCode,
                                                              facility.Oid );
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );
            Assert.AreEqual( EXPECTED_NUM_ENTRIES_1, patientTypes.Count, MESSAGE_INCORRECT_NUM_ENTRIES_1 );
        }

        [Test]
        public void TestPatientTypesFor_WhenActivityIsRegistrationAndAssociatedActivityTypeIsNotActivatePreRegistrationAndVisitTypeIsER_ShouldReturnERPatientType()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new RegistrationActivity();
            string associatedActivityType = string.Empty;
            const string kindOfVisitCode = VisitType.EMERGENCY_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;

            List<VisitType> patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid ).Cast<VisitType>().ToList();
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );

            bool containsErVisitType = patientTypes.Where( x => x.Code == VisitType.EMERGENCY_PATIENT ).ToList().Count == 1 ? true : false;
            Assert.IsTrue( containsErVisitType );

        }

        [Test]
        public void TestPatientTypesFor_WhenActivityIsRegistrationAndAssociatedActivityTypeIsActivatePreRegistrationAndVisitTypeIsER_ShouldNotReturnERPatientType()
        {
            Facility facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO_OID_EQUIVALENT );
            Activity activity = new RegistrationActivity();
            const string associatedActivityType = "ActivatePreRegistrationActivity";
            const string kindOfVisitCode = VisitType.EMERGENCY_PATIENT;
            const string financialClassCode = FINANCIAL_CLASS_CODE_02;
            string locationBedCode = LOCATION_BED_CODE_EMPTY;

            List<VisitType> patientTypes = this.PatientBroker.PatientTypesFor( activity.GetType().ToString(), associatedActivityType,
                                                                        kindOfVisitCode,
                                                                        financialClassCode,
                                                                        locationBedCode,
                                                                        facility.Oid ).Cast<VisitType>().ToList();
            Assert.IsNotNull( patientTypes, MESSAGE_SHOULD_NOT_BE_NULL );

            bool containsErVisitType = patientTypes.Where( x => x.Code == VisitType.EMERGENCY_PATIENT ).ToList().Count == 1 ? true : false;
            Assert.IsFalse( containsErVisitType );

        }

     
      
        [Test]
        public void TestNormalizeMiddleInitialFor_WithOneCharacterWithSpaceAfterFirstName_ShouldUseTheLastCharacterAsMiddleInitial()
        {
            var name = new Name( "Some A", "One", String.Empty );

            var expectedName = new Name( "Some", "One", "A" );
            var actualName = PatientPBARBroker.NormalizeMiddleInitialFor( name );
            Assert.AreEqual( expectedName, actualName );
        }

        [Test]
        public void TestNormalizeMiddleInitialFor_WithTwoCharacterWithSpaceAfterFirstName_ShouldUseTheLastCharacterAsMiddleInitial()
        {
            var name = new Name( "Some A B", "One", String.Empty );

            var expectedName = new Name( "Some A", "One", "B" );
            var actualName = PatientPBARBroker.NormalizeMiddleInitialFor( name );
            Assert.AreEqual( expectedName, actualName );
        }

        [Test]
        public void TestNormalizeMiddleInitialFor_WithNoSpaceAndCharacterAfterFirstName_ShouldNotChangeTheMiddleInitial()
        {
            var name = new Name( "Some", "One", String.Empty );

            var expectedName = new Name( "Some", "One", string.Empty );
            var actualName = PatientPBARBroker.NormalizeMiddleInitialFor( name );
            Assert.AreEqual( expectedName, actualName );
        }


        [Test]
        public void TestGetNewMrnFor_DirectCallToPbar_ShouldNotThrowException()
        {
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientPbarBroker = new PatientPBARBroker();
            patientPbarBroker.GetNewMrnFor(facility);
        }

        [Test]
        public void TestGetNewMrnFor_PbarReturnsPositiveInteger_ShouldNotThrowException()
        {
            const int mockExpectedNewMrn = 1;
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientBroker = GetMockPatientBrokerWherePbarReturnsNewMrn( mockExpectedNewMrn );

            var actualNewMrn = patientBroker.GetNewMrnFor( facility );

            Assert.AreEqual( mockExpectedNewMrn, actualNewMrn );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewMrnFor_PbarReturnsZero_ShouldThrowException()
        {
            const int pbarNewMrn = 0;
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientBroker = GetMockPatientBrokerWherePbarReturnsNewMrn( pbarNewMrn );

            patientBroker.GetNewMrnFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewMrnFor_PbarReturnsNegativeInteger_ShouldThrowException()
        {
            const int pbarNewMrn = -1;
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientBroker = GetMockPatientBrokerWherePbarReturnsNewMrn( pbarNewMrn );

            patientBroker.GetNewMrnFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewMrnFor_PbarReturnsNull_ShouldThrowException()
        {
            const object pbarNewMrn = null;
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientBroker = GetMockPatientBrokerWherePbarReturnsNewMrn( pbarNewMrn );

            patientBroker.GetNewMrnFor( facility );
        }

        [Test]
        [ExpectedException( typeof( EnterpriseException ) )]
        public void TestGetNewMrnFor_PbarReturnsNonInteger_ShouldThrowException()
        {
            const string pbarNewMrn = "a";
            var facility = this.FacilityBroker.FacilityWith( HSPCODE_ACO );
            var patientBroker = GetMockPatientBrokerWherePbarReturnsNewMrn( pbarNewMrn );

            patientBroker.GetNewMrnFor( facility );
        }

        #endregion

        #region Support Methods
        private static PatientPBARBroker GetMockPatientBrokerWherePbarReturnsNewMrn( object pbarRetrunValue )
        {
            MockRepository mockRepository = new MockRepository();

            PatientPBARBroker patientPBARBroker = (PatientPBARBroker)mockRepository.PartialMock( typeof( PatientPBARBroker ) );
            patientPBARBroker.Expect( x => x.TryExecutingNewMrnNumberStoredProcedure( Arg<IDbCommand>.Is.Anything ) ).Return( pbarRetrunValue );
            mockRepository.ReplayAll();
            return patientPBARBroker;
        }

        private IPatientBroker PatientBroker { get; set; }
        
        private IDemographicsBroker DemographicsBroker { get; set; }

        internal IFacilityBroker FacilityBroker { get; private set; }

        #endregion

        #region Data Elements

        private  Gender femaleGender = null;
        private  Gender maleGender = null;
        private  Gender blankGender = null;

        #endregion

        #region Constants
        private const string DEL_FACILITY_CODE = "DEL";

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
            TEST_SSNWITHMAILADDR = "879879679",
            TEST_SSNWITHPHYADDR = "879879679",
            TEST_SSNWITHNOADDR = "896757967",
            TEST_GRNTRWITHMAILADDR = "879879679";

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
            HSPCODE_ACO_OID_EQUIVALENT = 900,
            HSPCODE_DHF_OID = 54;

        private readonly SocialSecurityNumber EXPECTED_SSN = new SocialSecurityNumber( "075146408" );

        private DateTime EXPECTED_DOB = new DateTime( 1919, 1, 9 );
        private DateTime EXPECTED_GUARANTOR_DOB = new DateTime( 1980, 10, 10 );
        private readonly PhoneNumber TEST_PHONENUMBER = new PhoneNumber("1234567890");
        private const string
            EXPECTED_STREET_ADDRESS = "2700 W PLANO PKWY",
            EXPECTED_NEW_CITY = "PLANO",
            EXPECTED_NEW_STATE = "TEXAS",
            EXPECTED_NEW_STATE_CODE = "TX",
            EXPECTED_NEW_ZIP_CODE = "75075";

        private readonly Address EXPECTED_ADDRESS = new Address( EXPECTED_STREET_ADDRESS,
                                                        string.Empty,
                                                        EXPECTED_NEW_CITY,
                                                        new ZipCode( EXPECTED_NEW_ZIP_CODE ),
                                                        new State( EXPECTED_OID,
                                                                   EXPECTED_NEW_STATE,
                                                                   EXPECTED_NEW_STATE_CODE ),
                                                        new Country( EXPECTED_OID,
                                                                     EXPECTED_COUNTRY,
                                                                     EXPECTED_COUNTRY_CODE )
            );

        #endregion
    }
}