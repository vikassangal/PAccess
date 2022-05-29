using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class WorklistTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown WorklistTestsTests
        [TestFixtureSetUp()]
        public static void SetUpWorklistTests()
        {
           
        }

        [TestFixtureTearDown()]
        public static void TearDownWorklistTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestWorkLists()
        {
            Worklist worklist = new Worklist();
            WorklistSelectionRange range = new WorklistSelectionRange(1L,"10 days",10L);
            WorklistSettings ws = new WorklistSettings("F","M",
                                                       DateTime.Now,DateTime.Now,
                                                       range,1,1);
            worklist.AddWorkListSetting(ws);
            Assert.AreEqual( 1, worklist.WorklistSettings.Count) ;
            foreach ( WorklistSettings w in worklist.WorklistSettings)
            {
                Assert.AreEqual( 10, w.WorklistSelectionRange.RangeInDays );
                Assert.AreEqual( "F", w.BeginningWithLetter );
                Assert.AreEqual( "M", w.EndingWithLetter );
            }
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}