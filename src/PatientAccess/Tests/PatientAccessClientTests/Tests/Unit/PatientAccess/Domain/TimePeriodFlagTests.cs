using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class TimePeriodFlagTests
    {
        #region Constants
        #endregion
        #region SetUp and TearDown ModeOfArrivalTests
        [TestFixtureSetUp()]
        public static void SetUpTimePeriodFlagTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownTimePeriodFlagTests()
        {
        }
        #endregion

        #region Methods
        [Test()]
        public void SetPeriodTest()
        {
            TimePeriodFlag timePeriodFlag = new TimePeriodFlag( "Y" );
            Assert.AreEqual( timePeriodFlag.Code, "Y" );
            Assert.AreEqual( timePeriodFlag.Description, "Year" );

            timePeriodFlag = new TimePeriodFlag( "V" );
            Assert.AreEqual( timePeriodFlag.Code, "V" );
            Assert.AreEqual( timePeriodFlag.Description, "Visit" );

            timePeriodFlag = new TimePeriodFlag( "B" );
            Assert.AreEqual( timePeriodFlag.Code, "B" );
            Assert.AreEqual( timePeriodFlag.Description, " " );

            timePeriodFlag = new TimePeriodFlag( string.Empty );
            Assert.AreEqual( timePeriodFlag.Code, "B" );
            Assert.AreEqual( timePeriodFlag.Description, " " );
        }

        [Test(), ExpectedException( typeof(ApplicationException) )]
        public void SetInvalidPeriodTest()
        {
            TimePeriodFlag timePeriodFlag = new TimePeriodFlag( "A" );
        }
        #endregion
    }
}