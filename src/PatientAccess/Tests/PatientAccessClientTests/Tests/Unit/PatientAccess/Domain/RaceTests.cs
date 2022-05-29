using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class RaceTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown RaceTests
        [TestFixtureSetUp()]
        public static void SetUpRaceTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownRaceTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testRace()
        {
           
            Race race1 = new Race(3,ReferenceValue.NEW_VERSION,"Asian","Asian");
           
            Assert.AreEqual(
                typeof(Race),
                race1.GetType()
                );
                 
            Assert.AreEqual(
                "Asian",
                race1.Description
                );
            Assert.AreEqual(
                "Asian",
                race1.Code
                );
            Assert.AreEqual(
                3,
                race1.Oid
                );
            Race race2 = new Race(3,ReferenceValue.NEW_VERSION,"White","White");
            Race race3 = new Race(3,ReferenceValue.NEW_VERSION,"Hispanic","Hispanic");
            ArrayList races = new ArrayList();
            races.Add(race1);
            races.Add(race2);
            races.Add(race3);
            Assert.AreEqual(3,
                            races.Count                         
                );
            Assert.IsTrue(races.Contains(race2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}