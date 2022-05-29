using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class LocationTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ModeOfArrivalTests
        [TestFixtureSetUp()]
        public static void SetUpLocationTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownLocationTests()
        {
        }
        #endregion

        #region Methods
        [Test()]
        public void ToStringTest()
        {
            Location location = new Location( "NS", "180", "A" );
            Assert.AreEqual( location.FormattedLocation, "NS-0180-A" );

            NursingStation ns = new NursingStation( 0L, DateTime.Now, "NS", "NS" );
            location = new Location( 0L, DateTime.Now, string.Empty, string.Empty, ns, null, null );
            Assert.AreEqual( location.FormattedLocation, "NS-0000-" );

            ns = new NursingStation();
            Bed bed = new Bed();
            Room room = new Room();

            location = new Location( 0L, DateTime.Now, string.Empty, string.Empty, ns, room, bed );
            Assert.AreEqual( location.FormattedLocation, string.Empty );
        }
        #endregion
    }
}