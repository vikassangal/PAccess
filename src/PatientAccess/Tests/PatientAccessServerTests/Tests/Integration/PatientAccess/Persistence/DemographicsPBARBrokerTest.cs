using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class DemographicsPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GenderBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpDemographicsBrokerTests()
        {
            i_Broker = BrokerFactory.BrokerOfType<IDemographicsBroker>();
        
        }

        [TestFixtureTearDown()]
        public static void TearDownDemographicsBrokerTests()
        {
            i_Broker = null;       
        }
        #endregion

        #region Test Methods
        [Test()]
        public void AllTypesOfGenders()
        {
            //This is a case where the order of the return is specified.
            ArrayList genders = (ArrayList)i_Broker.AllTypesOfGenders( ACO_FACILITYID );

            Gender male = null;
            Gender female = null;
            Gender unknown = null;

            foreach( Gender gender in genders )
            {
                switch( gender.Code )
                {
                    case "M": 
                        male = gender;
                        break;
                    case "F": 
                        female = gender;
                        break;
                    case "U": 
                        unknown = gender;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(female,"Did not find female gender object");
            Assert.AreEqual(
                "FEMALE",
                female.Description,
                "FEMALE description is incorrect"
                );

            Assert.IsNotNull(male,"Did not find male gender object");
            Assert.AreEqual(
                "MALE",
                male.Description,
                "MALE description is incorrect"
                );
            Assert.IsNotNull(unknown,"Did not find unknown gender object");
            Assert.AreEqual(
                "UNKNOWN", 
                unknown.Description,
                "UNKNOWN description is incorrect"
                ); 
        }

        [Test()]
        public void TestGenderWithSuccess()
        {
            Gender gender = i_Broker.GenderWith(ACO_FACILITYID,"M");
            Assert.IsNotNull(gender,"Did not find Male Gender");

            gender = i_Broker.GenderWith(ACO_FACILITYID, "F");
            Assert.IsNotNull(gender, "Did not find Female Gender");
        }

        [Test()]
        public void TestGenderWithInvalidCode()
        {
            Gender gender = i_Broker.GenderWith(ACO_FACILITYID, "XX");
            Assert.IsFalse(gender.IsValid,"Should not have found gender with code of 'XX'");
        }

        [Test()]
        public void TestGenderForBlank()
        {
            string blank = String.Empty;
            Gender gender = i_Broker.GenderWith(ACO_FACILITYID, blank);

            Assert.AreEqual(
                blank,
                gender.Code,
                "Code  should be blank");

            Assert.IsTrue(
                gender.IsValid, "Blank Row should be valid"
                );
        }
        
        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestGenderWithNullCode()
        {
            Gender gender = i_Broker.GenderWith( ACO_FACILITYID, null ) ;
        }

        [Test()]
        public void AllMaritalStatuses()
        {
            /*
             * This method tests all valid codes ensuring a complete happy path test.
             * The following codes are valid codes:
                 * D=Divorced
                 * F=Widowed
                 * M=Married
                 * S=Single
                 * U=Unknown
                 * X=LG Separat
                 * L=Life Partn 
             */            
            MaritalStatus maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "M");
            Assert.IsNotNull( maritalStatus, "Marital status, Married, not found." ) ;
            Assert.AreEqual( "M", maritalStatus.Code.Trim(), "Incorrect code for Married Marital Status" ) ;

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "S");
            Assert.IsNotNull( maritalStatus, "Marital status, Single, not found." ) ;
            Assert.AreEqual( "S", maritalStatus.Code.Trim(), "Incorrect code for Single Marital Status" ) ;

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "D");
            Assert.IsNotNull( maritalStatus, "Marital status, Divorced, not found." ) ;
            Assert.AreEqual( "D", maritalStatus.Code.Trim(), "Incorrect code for Divorced Marital Status" ) ;

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "F");
            Assert.IsNotNull( maritalStatus, "Marital status, Widowed, not found." ) ;
            Assert.AreEqual( "F", maritalStatus.Code.Trim(), "Incorrect code for Widowed Marital Status" ) ;

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "U");
            Assert.IsNotNull( maritalStatus, "Marital status, Unknown, not found." ) ;
            Assert.AreEqual( "U", maritalStatus.Code.Trim(), "Incorrect code for Unknown Marital Status" ) ;

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "X");
            Assert.IsNotNull(maritalStatus, "Marital status, LG Separated, not found." ) ;
            Assert.AreEqual("X", maritalStatus.Code.Trim(), "Incorrect code for LG Separated Marital Status");

            maritalStatus = i_Broker.MaritalStatusWith(ACO_FACILITYID, "L");
            Assert.IsNotNull( maritalStatus, "Marital status, Life Partner, not found." ) ;
            Assert.AreEqual("L", maritalStatus.Code.Trim(), "Incorrect code for Life Partner Marital Status");
        }

        [Test()]
        public void TestInvalidMaritalStatus()
        {
            MaritalStatus maritalStatus = i_Broker.MaritalStatusWith( ACO_FACILITYID, "Z" ) ;
            Assert.IsFalse(maritalStatus.IsValid, "Should not have found marital status of code = 'Z'");
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestMaritalStatusWithNullCode()
        {
            MaritalStatus maritalStatus = i_Broker.MaritalStatusWith( ACO_FACILITYID, null ) ;
        }

        [Test()]
        [Ignore()]
        public void AllLanguages()
        {
            ArrayList languages = (ArrayList)i_Broker.AllLanguages( ACO_FACILITYID );
            Language arabic = null;
            Language armenian = null;
            
            foreach( Language lang in languages )
            {
                switch( lang.Description )
                {
                    case "ARABIC":
                        arabic = lang;
                        break;
                    case "ARMENIAN":
                        armenian = lang;
                        break;
                    default:
                        break;
                }
            }
            Assert.IsNotNull(arabic,"Did not find arabic language");
            Assert.AreEqual("AB",arabic.Code,"Aribic Code is not correct");
            Assert.IsNotNull(armenian,"Did not find armenian language");
            Assert.AreEqual("AR",armenian.Code,"armenian Code is not correct");
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ]
        public void AllLanguagesWithiInvalidFacilityID()
        {
            ArrayList languages = (ArrayList)i_Broker.AllLanguages( INVALID_FACILITY_ID ) ;
        }

        [Test()]
        public void TestTwoLanguages()
        {
            Language language = i_Broker.LanguageWith( ACO_FACILITYID, "PO" ) ;
            Assert.IsNotNull( language, "Did not find Polish Language" ) ;

            language = i_Broker.LanguageWith( ACO_FACILITYID, "EN" ) ;
            Assert.IsNotNull( language, "Did not find English Language");
        }

        [Test()]
        public void TestInvalidLanguage()
        {
            Language language = i_Broker.LanguageWith( ACO_FACILITYID, "PP" ) ;
            Assert.IsFalse( language.IsValid, "Should not have found language with code of PP" ) ;
        }

        [Test()]
        public void TestLanguageForBlank()
        {            
            string blank = String.Empty ;
            Language language = i_Broker.LanguageWith( ACO_FACILITYID,blank );

            Assert.AreEqual(
                blank,
                language.Code,
                "Code should be blank");
            Assert.AreEqual
                (blank,
                 language.Description,
                 "Description should be blank"
                );
         
            Assert.IsTrue(
                language.IsValid, "Blank Row should be valid"            
                );
        }

        [Test()]
        [ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestLanguageForNull()
        {
            Language language = i_Broker.LanguageWith( ACO_FACILITYID, null ) ; 
        }

        [Test()]
        [ExpectedException( typeof( BrokerException ) ) ]
        public void TestLanguageWithInvalidFacilityID()
        {
            Language language = i_Broker.LanguageWith( INVALID_FACILITY_ID, "EN" ) ;
        }
        
        [Test()]
        public void TestMaritalStatusForBlank()
        {            
            string blank =String.Empty ;
            MaritalStatus maritalStatus = i_Broker.MaritalStatusWith( ACO_FACILITYID,blank );

            Assert.AreEqual(
                blank,
                maritalStatus.Code,
                "Code  should be blank");
        
            Assert.IsTrue(
                maritalStatus.IsValid, "Blank Row should be valid"            
                );
        }
       
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IDemographicsBroker i_Broker = null;
        #endregion

    }
}