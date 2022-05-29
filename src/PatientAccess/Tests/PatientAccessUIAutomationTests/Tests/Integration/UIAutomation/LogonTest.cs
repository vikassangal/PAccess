using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientAccess.Tests.Integration.UIAutomation.Util;

namespace PatientAccess.Tests.Integration.UIAutomation
{
    /// <summary>
    /// Summary description for LogonTest
    /// </summary>
    [CodedUITest]
    public class LogonTest
    {

        [ClassInitialize]
        public static void ClassInit( TestContext context )
        {
            PatientAccessProcess.KillAllPatientAccessInstances();

            PatientAccessProcess.StartNewPatientAccessProcess();
        }

        [ClassCleanup]
        static public void ClassCleanup()
        {
            PatientAccessProcess.KillAllPatientAccessInstances();
        }

        [TestMethod]
        public void TestLogon()
        {
            UIMap.VerifyLogonScreenIsVisible();
            UIMap.Logon();
            UIMap.AssertThatLogonWasSuccessful();
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ( ( map == null ) )
                {
                    map = new UIMap();
                }

                return map;
            }
        }

        private UIMap map;
    }
}
