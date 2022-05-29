using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.UI.DiagnosisViews.Specialized;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews.Specialized
{
    /// <summary>
    ///This is a test class for ClinicalTrialsBoardDiagnosisViewTests and is intended
    ///to contain all ClinicalTrialsBoardDiagnosisViewTests Unit Tests
    ///</summary>
    [TestFixture]
    public class ClinicalTrialsBoardDiagnosisViewTests
    {

        #region Constants 

        private const string KEY_IS_CLINICAL_TRIAL_BOARD_PARTICIPANT = 
            "IsClinicalTrialBoardParticipant";

        #endregion Constants 

        #region Properties 

        private ClinicalTrialsBoardDiagnosisView UnitUnderTest { get; set; }

        #endregion Properties 

        #region Methods 

        [SetUp]
        public void Initialize()
        {
        
            this.UnitUnderTest = new ClinicalTrialsBoardDiagnosisView();
            
        }

        /// <summary>
        ///A test for UpdateView
        ///</summary>
        [Test]
        public void UpdateViewClinicalTrialNoValueShouldShowPanel()
        {

            this.TestClinicalTrialControlGroupFor( "N", true, "No", new RegistrationActivity());

        }

        /// <summary>
        ///A test for UpdateView
        ///</summary>
        [Test]
        public void UpdateViewClinicalTrialNullValueShouldShowPanel()
        {

            this.TestClinicalTrialControlGroupFor( null, true, "No", new RegistrationActivity());

        }

        /// <summary>
        ///A test for UpdateView
        ///</summary>
        [Test]
        public void UpdateViewClinicalTrialYesValueShouldShowPanel()
        {

            this.TestClinicalTrialControlGroupFor( "Y", true, "Yes", new RegistrationActivity() );

        }

        /// <summary>
        /// Updates the view clinical trial pre reg should hide panel.
        /// </summary>
        [Test]
        public void UpdateViewClinicalTrialPreRegShouldHidePanel()
        {        
            this.TestClinicalTrialControlGroupFor( "Y", false, "Yes/No", new PreRegistrationActivity() );
        }

        /// <summary>
        /// Updates the view clinical trial pre reg should hide panel.
        /// </summary>
        [Test]
        public void UpdateViewClinicalTrialPreRegOfflineShouldHidePanel()
        {
            this.TestClinicalTrialControlGroupFor("Y", false, "Yes/No", new PreRegistrationWithOfflineActivity());
        }

        /// <summary>
        /// Updates the view clinical trial pre reg should hide panel.
        /// </summary>
        [Test]
        public void UpdateViewClinicalTrialPreRegWorklistShouldHidePanel()
        {
            this.TestClinicalTrialControlGroupFor("Y", false, "Yes/No", new PreRegistrationWorklistActivity());
        }

        /// <summary>
        /// Tests the clinical trial control group for.
        /// </summary>
        /// <param name="clinicalTrialFlagValue">The clinical trial flag value.</param>
        /// <param name="isPanelVisible">if set to <c>true</c> [is panel visible].</param>
        /// <param name="expectedLabelText">The expected label text.</param>
        /// <param name="anActivity">the activity</param>
        private void TestClinicalTrialControlGroupFor( string clinicalTrialFlagValue, bool isPanelVisible, string expectedLabelText, Activity anActivity )
        {

            Account testAccount = new Account();
            testAccount.Activity = anActivity;
            if( clinicalTrialFlagValue != null )
            {
                testAccount[ClinicalTrialsConstants.KEY_IS_ACCOUNT_ELIGIBLE_FOR_CLINICAL_TRIALS] = clinicalTrialFlagValue;
            }
            testAccount.Facility = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( "ACO" );

            User.SetCurrentUserTo( User.NewInstance() );
            User.GetCurrent().Facility = testAccount.Facility;

            this.UnitUnderTest.Model = testAccount;
            this.UnitUnderTest.UpdateView();

            Assert.AreEqual( isPanelVisible,
                             this.UnitUnderTest.ClinicalTrialsBoardPanel.Visible );
            Assert.AreEqual( expectedLabelText,
                             this.UnitUnderTest.ClinicalTrialsBoardFlagValueLabel.Text );
            
        }

        #endregion Methods 

    }
}