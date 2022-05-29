using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PassportTests
    {
        #region SetUp and TearDown PassportTests
        [TestFixtureSetUp()]
        public static void SetUpPassportTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPassportTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestPassportCreate()
        {
            Passport pt0 = new Passport();
            Passport pt1 = new Passport(PASSPORTNUM1);
            Passport pt2 = new Passport(PASSPORTNUM2, new Country(COUNTRYCODE2));
            Country cnt = new Country();

            Assert.AreEqual(string.Empty, pt0.Number);
            Assert.AreEqual(cnt.Code, pt0.Country.Code);
            Assert.AreEqual(PASSPORTNUM1, pt1.Number);
            Assert.AreEqual(COUNTRYCODE2, pt2.Country.Code);
        }

        [Test()]
        public void TestPassportCompare()
        {
            Passport ptNull = null;           
            Passport pt1 = new Passport(PASSPORTNUM1, new Country(COUNTRYCODE1));
            Passport pt2 = new Passport(PASSPORTNUM2, new Country(COUNTRYCODE2));

            Assert.AreEqual(false, pt1.Equals(ptNull));
            Assert.AreEqual(false, pt1.Equals(pt2));
        }
        #endregion

        [Test()]
        public void TestGetHashCode()
        {
            Passport pt1 = new Passport(PASSPORTNUM1, new Country(COUNTRYCODE1));
            Passport pt2 = new Passport(PASSPORTNUM2, new Country(COUNTRYCODE2));

            Assert.AreNotEqual(pt1.GetHashCode(), pt2.GetHashCode());
        }

        #region Data Elements
        const string
            PASSPORTNUM1 = "12345678",
            COUNTRYCODE1 = "CAN",
            PASSPORTNUM2 = "23456789",
            COUNTRYCODE2 = "CHN";
        #endregion
    }
}