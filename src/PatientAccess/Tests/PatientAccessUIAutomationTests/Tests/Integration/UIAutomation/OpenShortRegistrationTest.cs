using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientAccess.Tests.Integration.UIAutomation.Util;

namespace PatientAccess.Tests.Integration.UIAutomation
{
    /// <summary>
    /// Summary description for OpenShortRegistrationTest
    /// </summary>
    [CodedUITest]
    public class OpenShortRegistrationTest
    {
        [ClassInitialize]
        public static void ClassInit( TestContext context )
        {
            PatientAccessProcess.KillAllPatientAccessInstances();

            PatientAccessProcess.StartNewPatientAccessProcess();

            Playback.Initialize();

            try
            {
                SharedUIMap.VerifyLogonScreenIsVisible();
                SharedUIMap.Logon();
                SharedUIMap.AssertThatLogonWasSuccessful();
            }

            finally
            {
                Playback.Cleanup();
            }
        }

        [TestCleanup()]
        public void TearDown()
        {
            UIMap.CancelActivity();
        }

        [ClassCleanup]
        static public void ClassCleanup()
        {
            PatientAccessProcess.KillAllPatientAccessInstances();
        }

        [TestMethod]
        public void ShortRegOpensIn8Tabs()
        {
            UIMap.ClickShortRegistration();
            UIMap.SearchToCreateNewPatient();

            UIMap.AssertThatTheCreateNewPatientButtonIsVisible();
            WaitForSearchToFinish();
            UIMap.ClickCreateNewPatient();

            UIMap.AssertThatTheShortAccountViewWasLoaded();
        }

        [TestMethod]
        public void ShorRegOpensIn8TabsIfHelpMenuIsClickedBeforeCreatingNewPatient()
        {
            UIMap.ClickShortRegistration();
            UIMap.SearchToCreateNewPatient();
            WaitForSearchToFinish();
            UIMap.AssertThatTheCreateNewPatientButtonIsVisible();
            UIMap.ClickOnHelpAboutAndDismissTheDialog();

            UIMap.ClickCreateNewPatient();

            UIMap.AssertThatTheShortAccountViewWasLoaded();
        }


        [TestMethod]
        public void ShorRegOpensIn8TabsIfReportsPhysiciansMenuIsClickedBeforeCreatingNewPatient()
        {
            UIMap.ClickShortRegistration();
            UIMap.SearchToCreateNewPatient();
            WaitForSearchToFinish();
            UIMap.AssertThatTheCreateNewPatientButtonIsVisible();
            UIMap.ClickOnReportsPhysiciansAndCloseTheDialog();

            UIMap.ClickCreateNewPatient();

            UIMap.AssertThatTheShortAccountViewWasLoaded();
        }


        [TestMethod]
        public void ShorRegOpensIn8TabsIfRegisterPrintFaceSheetMenuIsClickedBeforeCreatingNewPatient()
        {
            UIMap.ClickShortRegistration();
            UIMap.SearchToCreateNewPatient();
            WaitForSearchToFinish();
            UIMap.AssertThatTheCreateNewPatientButtonIsVisible();
            UIMap.ClickOnRegisterPrintFaceSheetAndCloseTheDialog();

            UIMap.ClickCreateNewPatient();

            UIMap.AssertThatTheShortAccountViewWasLoaded();
        }


        private static readonly UIMap SharedUIMap = new UIMap();

        private void WaitForSearchToFinish()
        {
            UIMap.UIPatientAccessWindow.UICreateNewPatientWindow.UICreateNewPatientButton.WaitForControlReady();
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
                if ( ( this.map == null ) )
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
