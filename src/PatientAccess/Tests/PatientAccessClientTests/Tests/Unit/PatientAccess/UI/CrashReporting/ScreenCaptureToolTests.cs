using System.Drawing;
using NUnit.Framework;
using PatientAccess.UI.CrashReporting;

namespace Tests.Unit.PatientAccess.UI.CrashReporting
{
    [TestFixture]
    [Category( "Fast" )]
    public class ScreenCaptureToolTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ScreenCaptureToolTests
        /// <summary>
        /// Sets the up crash report store tests.
        /// </summary>
        [SetUp()]
        public void SetUpScreenCaptureToolTests()
        {
        }

        /// <summary>
        /// Tears the down crash report store tests.
        /// </summary>
        [TearDown()]
        public void TearDownScreenCaptureToolTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestScreenCapture()
        {
            ScreenCaptureTool screenCaptureTool = new ScreenCaptureTool();
            Bitmap aScreenCapture = screenCaptureTool.GetScreenCapture();

            Assert.IsNotNull(aScreenCapture);
            Assert.IsFalse(aScreenCapture.Size.IsEmpty);
            Assert.AreEqual(aScreenCapture.GetType(), typeof(Bitmap));
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}