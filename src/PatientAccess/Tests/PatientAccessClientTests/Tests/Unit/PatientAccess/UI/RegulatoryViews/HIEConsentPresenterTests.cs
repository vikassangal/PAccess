using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules; 
using PatientAccess.UI.RegulatoryViews.Presenters; 
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.RegulatoryViews
{
    [TestFixture]
    [Category("Fast")]
    public class HIEConsentPresenterTests
    {
        #region Registration Activity
        [Test]
        public void TestUpdateView_HIEConsent_VisibleForPatientType1_AccountCreatedAfterFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
        }

        [Test]
        public void TestUpdateView_HIEConsent_NotVisibleForPatientType1_AccountCreatedBeforeFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowShareDataWithPublicHIE());
        }
        [Test]
        public void TestUpdateView_HIEConsent_NotVisibleForPatientType0_AccountCreatedAfterFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new PreMSERegisterActivity(), VisitType.PreRegistration,
                GetAccountCreatedDateAfterFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowShareDataWithPublicHIE());
        }
        #endregion Registration Activity

        #region Support Methods

        private static Account GetAccount(Activity activity, VisitType visitType, DateTime accountCreatedDate)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF");

            return new Account
                       {
                           Activity = activity,
                           Facility = facility,
                           AdmitDate = new DateTime(2013, 07, 20),
                           AccountCreatedDate = accountCreatedDate,
                           KindOfVisit = visitType
                       };
        }

        private HIEConsentPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate)
        {
            var view = MockRepository.GenerateMock<IHIEShareDataFlagView>();
            var account = GetAccount(activity, patientType, accountCreatedDate);
            return new HIEConsentPresenter(view, new HIEConsentFeatureManager(), account);
        }

        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new HIEConsentFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new HIEConsentFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion
    }
}
