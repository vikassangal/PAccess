using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AccomodationTests 
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AccomodationTests
        [TestFixtureSetUp()]
        public static void SetUpAccomodationTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownAccomodationTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAccomodationConstructorOne()
        {
            Accomodation acc = new Accomodation( 0L, DateTime.Now, "Accomodation" );
            Assert.AreEqual( acc.Description, "Accomodation" );
        }

        [Test()]
        public void TestAccomodationConstructorTwo()
        {
            Accomodation acc = new Accomodation( 0L, DateTime.Now, "Accomodation", "ACC" );
            Assert.AreEqual( acc.Code, "ACC" );
            Assert.AreEqual( acc.Description, "Accomodation" );
            Assert.AreEqual( acc.ToString(), "ACC Accomodation" );
        }

        [Test()]
        public void TestAccomodationConstructorThree()
        {
            Accomodation acc = new Accomodation();
            Assert.AreEqual( acc.Code, string.Empty );
            Assert.AreEqual( acc.Description, string.Empty );
        }

        public void TestIsReasonRequiredForSelectedAccommodation()
        {
            Accomodation accommodation = new Accomodation(0L, DateTime.Now, "Accomodation", "76");
            bool IsReasonRequired = accommodation.IsReasonRequiredForSelectedAccommodation();
            Assert.IsTrue(IsReasonRequired,"Reason Required");
        }

        public void TestIsPrivateRoomMedicallyNecessary()
        {
            Accomodation accommodation = new Accomodation(0L, DateTime.Now, "Accomodation", "05");
            bool isPrivateRoomRequired = accommodation.IsPrivateRoomMedicallyNecessary();
            Assert.IsTrue(isPrivateRoomRequired, "Private Room Medically Necessary");
        }
        #endregion
    }
}