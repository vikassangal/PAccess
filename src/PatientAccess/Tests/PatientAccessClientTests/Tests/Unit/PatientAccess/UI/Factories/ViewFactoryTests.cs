using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.DiagnosisViews.Specialized;
using PatientAccess.UI.Factories;

namespace Tests.Unit.PatientAccess.UI.Factories
{
    /// <summary>
    ///This is a test class for ViewFactoryTests and is intended
    ///to contain all ViewFactoryTests Unit Tests
    ///</summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ViewFactoryTests
    {

        #region Properties 

        private ViewFactory UnitUnderTest { get; set; }
        private User TestUser { get; set; }

        #endregion Properties 

        #region Methods 

        [Test]
        public void CreateViewNoRuleMatchShouldCreateDefaultView()
        {
            object actual =
                this.UnitUnderTest.CreateView<DiagnosisView>();

            // Downcast to check the actual
            Assert.IsInstanceOf<DiagnosisView>( actual );
        }

        /// <summary>
        ///A test for CreateView
        ///</summary>
        [Test]
        public void CreateViewRuleCreatesViewShouldReturnNonDefaultView()
        {
            this.TestUser.Facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = true;
            object actual = 
                this.UnitUnderTest.CreateView<DiagnosisView>();
            
            // Downcast to check the actual
            Assert.IsInstanceOf<ClinicalTrialsBoardDiagnosisView>( actual );
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [SetUp]
        public void Initialize()
        {            
            this.UnitUnderTest = new ViewFactory();            
            this.TestUser = User.NewInstance();
            this.TestUser.Facility = new Facility();
            User.SetCurrentUserTo( this.TestUser );
        }

        #endregion Methods 

    }
}