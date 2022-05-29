using NUnit.Framework;
using PatientAccess.UI.CrashReporting;

namespace Tests.Unit.PatientAccess.UI.CrashReporting
{
    /// <summary>
    /// Summary description for CrashReporterTests.
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class CrashReporterTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CrashReportTests
        [SetUp()]
        public void SetUpCrashReporterTests()
        {
        }

        [TearDown()]
        public void TearDownCrashReporterTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCrashReporterType()
        {
            CrashReporter aCrashReporter = new CrashReporter();
            
            Assert.IsNotNull( aCrashReporter );
            Assert.AreEqual( aCrashReporter.GetType(), typeof( CrashReporter ) );
        }

        [Test()]
        public void TestSave()
        {
            //            CrashReport report = new CrashReport();
            //            CrashReporter aCrashReporter = new CrashReporter();
            //            aCrashReporter.Save( report );            
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}