using NUnit.Framework;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class PhoneNumberTests
    {
        #region Constants
        const string
            VALID_AREA_CODE     = "972",
            VALID_NUMBER        = "5777254",
            VALID_PHONE_NO_1    = VALID_AREA_CODE + VALID_NUMBER,
            INVALID_PHONE_NO    = "1" + VALID_PHONE_NO_1,
            NONNUMERIC_PHONE_NO = "NA",
            ZERO				= "0";
        #endregion

        #region SetUp and TearDown PhoneNumberTests
        [TestFixtureSetUp()]
        public static void SetUpPhoneNumberTests()
        {
            //this.WorkPhoneType = new PhoneNumberType( 0L, DateTime.Now, "Work" );
        }

        [TestFixtureTearDown()]
        public static void TearDownPhoneNumberTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestSingleNumberConstructor()
        {
            PhoneNumber phoneNumber = new PhoneNumber( "9725777254");

            Assert.AreEqual(
                VALID_PHONE_NO_1,
                phoneNumber.ToString()
                );

            Assert.AreEqual(
                VALID_PHONE_NO_1,
                phoneNumber.AsUnformattedString()
                );

            Assert.AreEqual(
                VALID_AREA_CODE,
                phoneNumber.AreaCode
                );

            Assert.AreEqual(
                VALID_NUMBER,
                phoneNumber.Number
                );
        }

        [Test()]
        public void TestMultiNumberConstructor()
        {
            PhoneNumber phoneNumber = new PhoneNumber( VALID_AREA_CODE, VALID_NUMBER);

            Assert.AreEqual(
                VALID_PHONE_NO_1,
                phoneNumber.ToString()
                );

            Assert.AreEqual(
                VALID_PHONE_NO_1,
                phoneNumber.AsUnformattedString()
                );

            Assert.AreEqual(
                VALID_AREA_CODE,
                phoneNumber.AreaCode
                );

            Assert.AreEqual(
                VALID_NUMBER,
                phoneNumber.Number
                );
        }

        [Test()]
        public void TestAsFormattedString()
        {
            PhoneNumber phoneNumber = new PhoneNumber( VALID_AREA_CODE, VALID_NUMBER );

            Assert.AreEqual(
                VALID_PHONE_NO_1,
                phoneNumber.ToString()
                );

            Assert.AreEqual(
                VALID_AREA_CODE,
                phoneNumber.AreaCode
                );

            Assert.AreEqual(
                VALID_NUMBER,
                phoneNumber.Number
                );

            Assert.AreEqual(
                "(972) 577-7254",
                phoneNumber.AsFormattedString()
                );
        }

        [Test()]       
        public void TestArgumentOutOfRangeException()
        {
            PhoneNumber phoneNumber = new PhoneNumber( INVALID_PHONE_NO);
            Assert.AreEqual(false, phoneNumber.IsValid);
        }

        [Test()]
        public void TestBlankAndZeroPhoneNumber()
        {
            PhoneNumber blankPhoneNumber = new PhoneNumber( string.Empty );
            Assert.AreEqual( string.Empty, blankPhoneNumber.AsFormattedString() );

            PhoneNumber zeroPhoneNumber = new PhoneNumber( ZERO, ZERO );
            Assert.AreEqual( string.Empty, zeroPhoneNumber.AsFormattedString() );
        }
        [Test()]
        public void TestNonNumericPhoneNumber()
        {
            PhoneNumber nnPhoneNumber = new PhoneNumber(NONNUMERIC_PHONE_NO);
            Assert.AreEqual(string.Empty, nnPhoneNumber.AsFormattedString());
        }
        #endregion

        #region Support Methods
        /*   private PhoneNumberType WorkPhoneType
        {
            get
            {
                return i_WorkPhoneType;
            }
            set
            {
                i_WorkPhoneType = value;
            }
        }*/
        #endregion

        #region Data Elements
        //private PhoneNumberType i_WorkPhoneType;
        #endregion
    }
}