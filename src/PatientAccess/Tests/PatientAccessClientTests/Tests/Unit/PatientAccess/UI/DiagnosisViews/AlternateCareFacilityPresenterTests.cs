using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.DiagnosisViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews
{
    [TestFixture]
    public class AlternateCareFacilityPresenterTests
    {
        [Test]
        public void TestHandleAlternateCareFacility_WhenActivityIsRegAdmitDateIsBeforeReleaseDateAndAdmitSourceH()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new RegistrationActivity(), BeforeReleaseDate, AdmitSourceH);
            var alternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(alternateCareFacilityView, alternateCareFacilityFeatureManager);

            alternateCareFacilityPresenter.HandleAlternateCareFacility();

            alternateCareFacilityView.AssertWasCalled(view => view.PopulateAlternateCareFacility());
            alternateCareFacilityView.AssertWasCalled(view => view.ShowAlternateCareFacilityDisabled());
            alternateCareFacilityView.AssertWasNotCalled(view => view.ShowAlternateCareFacilityEnabled());
        }

        [Test]
        public void TestHandleAlternateCareFacility_WhenActivityIsRegAdmitDateAfterReleaseAdmitSourceH()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new RegistrationActivity(), AfterReleaseDate, AdmitSourceH);
            var mockAlternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(mockAlternateCareFacilityView, alternateCareFacilityFeatureManager);

            alternateCareFacilityPresenter.HandleAlternateCareFacility();

            mockAlternateCareFacilityView.AssertWasCalled(view => view.PopulateAlternateCareFacility());
            mockAlternateCareFacilityView.AssertWasNotCalled(view => view.ShowAlternateCareFacilityDisabled());
            mockAlternateCareFacilityView.AssertWasCalled(view => view.ShowAlternateCareFacilityEnabled());

        }

        [Test]
        public void TestHandleAlternateCareFacility_WhenActivityIsRegAdmitDateIsAfterReleaseAdmitSourceN()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new RegistrationActivity(), AfterReleaseDate, AdmitSourceN);

            var mockAlternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(mockAlternateCareFacilityView, alternateCareFacilityFeatureManager);

            alternateCareFacilityPresenter.HandleAlternateCareFacility();

            mockAlternateCareFacilityView.AssertWasCalled(view => view.PopulateAlternateCareFacility());
            mockAlternateCareFacilityView.AssertWasNotCalled(view => view.ShowAlternateCareFacilityDisabled());
            mockAlternateCareFacilityView.AssertWasCalled(view => view.ShowAlternateCareFacilityEnabled());

        }
        [Test]
        public void TestHandleAlternateCareFacility_WhenActivityIsAdmitNewBornAdmitDateIsAfterReleaseAdmitSourceH()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new AdmitNewbornActivity(), AfterReleaseDate, AdmitSourceH);
            var mockAlternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(mockAlternateCareFacilityView, alternateCareFacilityFeatureManager);

            alternateCareFacilityPresenter.HandleAlternateCareFacility();

            mockAlternateCareFacilityView.AssertWasNotCalled(view => view.PopulateAlternateCareFacility());
            mockAlternateCareFacilityView.AssertWasNotCalled(view => view.ShowAlternateCareFacilityDisabled());
            mockAlternateCareFacilityView.AssertWasNotCalled(view => view.ShowAlternateCareFacilityEnabled());

        }

        [Test]
        public void TestUpdateAlternateCareFacility_WhenActivityIsRegAccountCreatedAfterReleaseAdmitSourceH()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new RegistrationActivity(), AfterReleaseDate, AdmitSourceH);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var mockAlternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(mockAlternateCareFacilityView, alternateCareFacilityFeatureManager);

            const string alternateCareFacility = "0123";
            alternateCareFacilityPresenter.UpdateAlternateCareFacility(alternateCareFacility);

            Assert.AreEqual(alternateCareFacility, account.AlternateCareFacility);
        }

        [Test]
        public void TestUpdateAlternateCareFacility_WhenActivityIsRegAccountCreatedAfterReleaseAdmitSourceNOTH()
        {
            var account = GetAccountWithAccountCreatedDateAndAdmitSource(new RegistrationActivity(), AfterReleaseDate, AdmitSourceN);
            var alternateCareFacilityFeatureManager = new AlternateCareFacilityFeatureManager();
            var mockAlternateCareFacilityView = GetMockAlternateCareFacilityView(account);
            var alternateCareFacilityPresenter = new AlternateCareFacilityPresenter(mockAlternateCareFacilityView, alternateCareFacilityFeatureManager);

            const string alternateCareFacility = "0123";

            alternateCareFacilityPresenter.UpdateAlternateCareFacility(alternateCareFacility);

            Assert.AreEqual(alternateCareFacility, account.AlternateCareFacility);
        }
        private static Account GetAccountWithAccountCreatedDateAndAdmitSource(Activity activity, DateTime accountCreatedDate, string admitSourceCode)
        {

            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                AdmitSource =
                    new AdmitSource(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Admit Source",
                                    admitSourceCode)
            };
        }

        #region Constants

        private readonly DateTime AfterReleaseDate = DateTime.Parse("01-30-2010");
        private readonly DateTime BeforeReleaseDate = DateTime.Parse("11-30-2008");
        private const string AdmitSourceH = "H";
        private const string AdmitSourceN = "N";

        #endregion

        private static IAlternateCareFacilityView GetMockAlternateCareFacilityView(Account account)
        {
            var mockAlternateCareFacilityView = MockRepository.GenerateStub<IAlternateCareFacilityView>();
            mockAlternateCareFacilityView.Model = account;
            return mockAlternateCareFacilityView;
        }
    }
}
