using System;
using NUnit.Framework;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class PersonTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PersonTests
        [TestFixtureSetUp()]
        public static void SetUpPersonTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownPersonTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestAgeAt()
        {
            TestPerson fiveDayOld = new TestPerson();
            fiveDayOld.DateOfBirth = DateTime.Now.AddDays( -5 );
            
            Assert.AreEqual( "5d", fiveDayOld.AgeAt( DateTime.Now ) );

            TestPerson fiveMonthOld = new TestPerson();
            fiveMonthOld.DateOfBirth = DateTime.Now.AddMonths( -5 );

            Assert.AreEqual( "5m", fiveMonthOld.AgeAt( DateTime.Now ) );

            TestPerson elevenMonthOld = new TestPerson();
            elevenMonthOld.DateOfBirth = DateTime.Now.AddMonths( -11 );

            Assert.AreEqual( "11m", elevenMonthOld.AgeAt( DateTime.Now ) );

            TestPerson oneYearOld = new TestPerson();
            oneYearOld.DateOfBirth = DateTime.Now.AddYears( -1 );

            Assert.AreEqual( "1y", oneYearOld.AgeAt( DateTime.Now ) );

            TestPerson adult = new TestPerson();
            adult.DateOfBirth = DateTime.Now.AddYears(-25);
            Assert.AreEqual("25y",adult.AgeAt(DateTime.Now) );

            TestPerson adultm = new TestPerson();
            adultm.DateOfBirth = new DateTime(2004,6,1);//DateTime.Now.AddMonths(-1);
            Assert.AreEqual("30d",adultm.AgeAt(new DateTime(2004,7,1)) );

            TestPerson adultm2 = new TestPerson();
            adultm2.DateOfBirth = new DateTime(2004,7,1);//DateTime.Now.AddMonths(-1);
            Assert.AreEqual("1m",adultm2.AgeAt(new DateTime(2004,8,1)) );
        }

        [Test()]
        public void TestIsLeapYear()
        {
            Assert.IsTrue( DateTime.IsLeapYear( 2000 ) );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }


    /// <summary>
    /// TestPerson is a class soley for testing the abstract class Person.
    /// </summary>
    [Serializable]
    internal class TestPerson : Person
    {
        #region Event Handlers
        #endregion

        #region Methods
        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public TestPerson()
        {
        }
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion
    }
}