using System.Collections;
using NUnit.Framework;
using PatientAccess.UI.CrashReporting;

namespace Tests.Unit.PatientAccess.UI.CrashReporting
{
    [TestFixture]
    [Category( "Fast" )]
    public class CrashReportStoreTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown CrashReportStoreTests
        /// <summary>
        /// Sets the up crash report store tests.
        /// </summary>
        [SetUp()]
        public void SetUpCrashReportStoreTests()
        {
        }

        /// <summary>
        /// Tears the down crash report store tests.
        /// </summary>
        [TearDown()]
        public void TearDownCrashReportStoreTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestCrashReporterStoreType()
        {
            CrashReportStore aCrashReportStore = new CrashReportStore();
            
            Assert.IsNotNull( aCrashReportStore );
            Assert.AreEqual( aCrashReportStore.GetType(), typeof( CrashReportStore ) );
        }

        [Test()]
        public void TestGetSavedReports()
        {
            CrashReportStore aCrashReportStore = new CrashReportStore();

            //CrashReport[] crashReports = aCrashReportStore.GetSavedReports();
            IList crashReports = aCrashReportStore.GetSavedReports();
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}