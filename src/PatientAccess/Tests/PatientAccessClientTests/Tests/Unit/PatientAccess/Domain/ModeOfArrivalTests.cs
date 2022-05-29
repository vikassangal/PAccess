using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ModeOfArrivalTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ModeOfArrivalTests
        [TestFixtureSetUp()]
        public static void SetUpModeOfArrivalTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownModeOfArrivalTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestModeOfArrival()
        {
            ArrayList  allModeOfArrivals = new ArrayList() ;
            ModeOfArrival modeOfArrival1 = new ModeOfArrival(0L,
                                                             DateTime.Now,
                                                             "modeOfArrival1",
                                                             "01");
            ModeOfArrival modeOfArrival2 = new ModeOfArrival(1L,
                                                             DateTime.Now,
                                                             "modeOfArrival2",
                                                             "02");
            ModeOfArrival modeOfArrival3 = new ModeOfArrival(1L,
                                                             DateTime.Now,
                                                             "modeOfArrival3",
                                                             "02");

            allModeOfArrivals.Add(modeOfArrival1);
            allModeOfArrivals.Add(modeOfArrival2);
            allModeOfArrivals.Add(modeOfArrival3);
            Assert.AreEqual(3,
                            allModeOfArrivals.Count                         
                );
            Assert.IsTrue(allModeOfArrivals.Contains(modeOfArrival1) );

            Assert.AreEqual( modeOfArrival1.ToString(), "01 modeOfArrival1" );

        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}