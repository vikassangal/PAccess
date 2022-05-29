using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    [TestFixture]
    [Category( "Fast" )]
    public class PatientSearchCriteriaTests
    {
        #region Constants
        private const string
            HSP_DEL             = "DEL",
            TEST_MRN            = "998398",
            TEST_ACCOUNT_NO     = "111111111111",
            TEST_LNAME          = "Severance",
            TEST_FNAME          = "James";
        private readonly SocialSecurityNumber
            TEST_SSN            = new SocialSecurityNumber( "999441111" );
        private readonly  PhoneNumber
            TEST_PHONENUMBER = new PhoneNumber ( "8050388595" );
        private readonly Gender
            TEST_GENDER         = new Gender( 0L, ReferenceValue.NEW_VERSION, "Male", "M" );
        private long
            TEST_MONTH_OF_BIRTH = 12L,
            TEST_YEAR_OF_BIRTH  = 1968L;
        #endregion

        #region SetUp and TearDown PatientSearchCriteriaTests
        [TestFixtureSetUp()]
        public static void SetUpPatientSearchCriteriaTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPatientSearchCriteriaTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestInValidCriteria()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(String.Empty)
                );

        
            ValidationResult result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid == false,
                "Results should be invalid, false"
                );
        }

        [Test()]
        public void TestValidationWithMrnOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN,
                String.Empty
                );
            
            ValidationResult result = criteria.Validate();

            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );           
        }

        [Test()]
        public void TestValidationWithAccountNoOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                TEST_ACCOUNT_NO
                );
            
            ValidationResult result = criteria.Validate();

            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );
        }

        [Test]
        public void TestValidationWithSsnOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                String.Empty,
                this.TEST_SSN,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty
                );
            
            ValidationResult result = criteria.Validate();

            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );
        }

        [Test()]
        public void TestValidationWithLastNameOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );
            
            ValidationResult result = criteria.Validate();

            Assert.IsTrue(
                result.IsValid == false,
                "Results should be invalid, false"
                );
        }

        [Test()]
        public void TestValidationWithLastNamePlus()
        {
            PatientSearchCriteria criteria;
            ValidationResult result;


            // Last Name & MRN
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN,
                String.Empty,
                new PhoneNumber(string.Empty)
                );
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );

            // Last Name & Account No
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                TEST_ACCOUNT_NO,
                new PhoneNumber(string.Empty)
                );
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // Last Name & First Name
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // Last Name & SSN
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                TEST_SSN.UnformattedSocialSecurityNumber,
                null,
                PatientSearchCriteria.NO_MONTH,
                 PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber( String.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );


            // Last Name & Gender
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                this.TEST_GENDER,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );


            // Last Name & Month of Birth
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                null,
                this.TEST_MONTH_OF_BIRTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );


            // Last Name & Year of Birth
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                this.TEST_YEAR_OF_BIRTH,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );
        }

        [Test()]
        public void TestValidationWithFirstNameOnly()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH, 
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty ,
               new PhoneNumber(String.Empty )
                );
            
            ValidationResult result = criteria.Validate();

            Assert.IsTrue(                
                result.IsValid == false,
                "Results should be invalid, false"
                );
        }

        [Test()]
        public void TestValidationWithFirstNamePlus()
        {
            PatientSearchCriteria criteria;
            ValidationResult result;


            // First Name & MRN
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                TEST_MRN,
                String.Empty,
                new PhoneNumber(string.Empty)
                
                );
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & Account Number
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                TEST_ACCOUNT_NO,
                new PhoneNumber(string.Empty)
                );
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & SSN
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                TEST_SSN.UnformattedSocialSecurityNumber,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & Last Name
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                TEST_LNAME,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & Gender & Month of Birth
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                this.TEST_GENDER,
                this.TEST_MONTH_OF_BIRTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & Gender & Month of Birth
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                this.TEST_GENDER,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                this.TEST_YEAR_OF_BIRTH,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );


            // First Name & Gender & Date of Birth
            criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                this.TEST_GENDER,
                this.TEST_MONTH_OF_BIRTH,
                PatientSearchCriteria.NO_DAY,
                this.TEST_YEAR_OF_BIRTH,
                String.Empty,
                String.Empty,
                new PhoneNumber(string.Empty)
                );            
            result = criteria.Validate();
            Assert.IsTrue(                
                result.IsValid,
                "Results should be valid, true"
                );
        }

        [Test()]
        public void TestDateOfBirth()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                this.TEST_GENDER,
                this.TEST_MONTH_OF_BIRTH,
                this.TEST_YEAR_OF_BIRTH,
                String.Empty,
                String.Empty
                ); 
            criteria.DayOfBirth = 8L;

            DateTime validDate = new DateTime( (int)this.TEST_YEAR_OF_BIRTH, (int)this.TEST_MONTH_OF_BIRTH, 8 );
            Assert.AreEqual( validDate, criteria.DateOfBirth );
        }

        [Test()]
        public void TestDateOfBirth_InvalidMonth()
        {
            PatientSearchCriteria criteria = new PatientSearchCriteria(
                HSP_DEL,
                TEST_FNAME,
                String.Empty,
                String.Empty,
                this.TEST_GENDER,
                13,
                this.TEST_YEAR_OF_BIRTH,
                String.Empty,
                String.Empty
                ); 
            criteria.DayOfBirth = 8L;

            DateTime validDate = new DateTime( (int)this.TEST_YEAR_OF_BIRTH, (int)this.TEST_MONTH_OF_BIRTH, 8 );
            Assert.AreEqual( DateTime.MinValue, criteria.DateOfBirth );
        }
        [Test()]
        public void TestValidationWithPhoneNumberOnly()
        {
            var criteria = new PatientSearchCriteria(
                HSP_DEL,
                string.Empty,
                String.Empty,
                String.Empty,
                null,
                PatientSearchCriteria.NO_MONTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                TEST_PHONENUMBER
                );

            var result = criteria.Validate();

            Assert.IsTrue(
                result.IsValid,
                "Results should be invalid, true"
                );
        }

        [Test()]
        public void TestValidationWithPhoneNumberPlus()
        {
            // Phone Number & Gender & Month of Birth & Last name
            var criteria = new PatientSearchCriteria(
                HSP_DEL,
                String.Empty,
                TEST_LNAME,
                String.Empty,
                TEST_GENDER,
                TEST_MONTH_OF_BIRTH,
                PatientSearchCriteria.NO_DAY,
                PatientSearchCriteria.NO_YEAR,
                String.Empty,
                String.Empty,
                TEST_PHONENUMBER
                );
            var result = criteria.Validate();
            Assert.IsTrue(
                result.IsValid,
                "Results should be valid, true"
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}