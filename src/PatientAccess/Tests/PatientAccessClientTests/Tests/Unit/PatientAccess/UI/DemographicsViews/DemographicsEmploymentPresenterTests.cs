using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.DemographicsViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DemographicsViews
{
    /// <summary>
    /// Summary description for DemographicsEmploymentPresenterTests
    /// </summary>
    [TestFixture]
    public class DemographicsEmploymentPresenterTests
    {
        #region Tests

        [Test]
        public void TestHandleOtherLanguageDisplay_WhenFacilityIsCa_LanguageIsEnglish_ShouldCallAnyOtherViewMethods()
        {
            var presenter = GetPresenterWithMockView( CALIFORNIA_FACILITY, ENGLISH_LANGUAGE );

            presenter.SelectedLanguageChanged( ENGLISH_LANGUAGE );

            presenter.View.AssertWasNotCalled( x => x.PopulateOtherLanguage() );
            presenter.View.AssertWasCalled( x => x.ClearOtherLanguage() );
        }

        [Test]
        [Ignore()]
        public void TestHandleOtherLanguageDisplay_WhenFacilityIsCa_LanguageIsOther_ShouldCallAnyOtherViewMethods()
        {
            var presenter = GetPresenterWithMockView( CALIFORNIA_FACILITY, OTHER_LANGUAGE );

            presenter.SelectedLanguageChanged( OTHER_LANGUAGE );

            presenter.View.AssertWasCalled( x => x.PopulateOtherLanguage() );
            presenter.View.AssertWasNotCalled( x => x.ClearOtherLanguage() );
        }
        [Test]
        public void TestHandleOtherLanguageDisplay_WhenFacilityIsTx_LanguageIsOther_ShouldCallAnyOtherViewMethods()
        {
            var presenter = GetPresenterWithMockView( TEXAS_FACILITY, OTHER_LANGUAGE );

            presenter.SelectedLanguageChanged( OTHER_LANGUAGE );
            presenter.View.AssertWasNotCalled( x => x.PopulateOtherLanguage() );
            presenter.View.AssertWasCalled( x => x.ClearOtherLanguage() );
        }
        [Test]
        public void TestHandleOtherLanguageDisplay_WhenFacilityIsTx_LanguageIsEnglish_ShouldCallAnyOtherViewMethods()
        {
            var presenter = GetPresenterWithMockView( TEXAS_FACILITY, ENGLISH_LANGUAGE );

            presenter.SelectedLanguageChanged( ENGLISH_LANGUAGE );
            presenter.View.AssertWasNotCalled( x => x.PopulateOtherLanguage() );
            presenter.View.AssertWasCalled( x => x.ClearOtherLanguage() );
        }

        #endregion Tests

        #region Helper Methods

        private static DemographicsEmploymentPresenter GetPresenterWithMockView( Facility facility, Language language )
        {
            var view = MockRepository.GenerateMock<IDemographicsEmploymentView>();
            var account = GetAccountWith( facility, language );
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            return new DemographicsEmploymentPresenter( view, account, ruleEngine );
        }

        private static Account GetAccountWith( Facility facility, Language language )
        {
            var patient = new Patient {Language = language ,
                                           OtherLanguage = language.IsOtherLanguage() ? OTHER_LANGUAGE_TEXT : ENGLISH_LANGUAGE_TEXT
            };
            return new Account
                {
                    Patient = patient ,
                    Facility = facility
                };
        }


        private readonly Facility CALIFORNIA_FACILITY  = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( "ACO" );
        private readonly Facility TEXAS_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( "DHF" );
        private readonly Language OTHER_LANGUAGE = new Language( 1L, DateTime.Now, Language.OTHER_LANGUAGE , "OT");
        private readonly Language ENGLISH_LANGUAGE = new Language( 1L, DateTime.Now, "English" ,"EN");
        private static readonly string OTHER_LANGUAGE_TEXT = "SOME LANGUAGE";
        private static readonly string ENGLISH_LANGUAGE_TEXT = "ENGLISH";

        #endregion Helper Methods

    }
}
