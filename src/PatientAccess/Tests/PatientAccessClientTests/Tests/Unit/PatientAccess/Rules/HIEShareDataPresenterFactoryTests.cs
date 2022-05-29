using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class HIEShareDataPresenterFactoryTests
    {
        [Test]
        public void TestGetPresenter_Registration_AccountCreatedBeforeFeatureStart_ShouldReturnHIEConsentPresenter()
        {
           var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart(), GetAdmitDateBeforeFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEConsentPresenter));
         }
        [Test]
        public void TestGetPresenter_Registration_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
         }
        [Test]
        public void TestGetPresenter_ShortRegistration_AccountCreatedBeforeFeatureStart_ShouldReturnHIEConsentPresenter()
        {
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart(), GetAdmitDateBeforeFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEConsentPresenter));
        }
        [Test]
        public void TestGetPresenter_ShortRegistration_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
        public void TestGetPresenter_ActivatePreRegistration_AccountCreatedBeforeFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var activity = new PreRegistrationActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
            var presenter = GetPresenterWithMockView(activity, VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
        public void TestGetPresenter_ActivatePreRegistration_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var activity = new PreRegistrationActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
            var presenter = GetPresenterWithMockView(activity, VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
       [Test]
        public void TestGetPresenter_ActivatePreAdminNewBorn_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var activity = new AdmitNewbornActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
            var presenter = GetPresenterWithMockView(activity, VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
       public void TestGetPresenter_PRE_MSE_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var presenter = GetPresenterWithMockView(new PreMSERegisterActivity(), VisitType.Emergency,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
        public void TestGetPresenter_UCPRE_MSE_AccountCreatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var presenter = GetPresenterWithMockView(new UCCPreMSERegistrationActivity(), VisitType.Emergency,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
        public void TestGetPresenter_POST_MSE_AccountActivatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
           var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        [Test]
        public void TestGetPresenter_UC_POST_MSE_AccountActivatedAfterFeatureStart_ShouldReturnHIEShareDataPresenter()
        {
            var presenter = GetPresenterWithMockView(new UCCPostMseRegistrationActivity(), VisitType.Inpatient,
                 GetAccountCreatedDateAfterFeatureStart(), GetAdmitDateAfterFeatureStart());
            Assert.IsTrue(presenter.GetType() == typeof(HIEShareDataPresenter));
        }
        #region Support Methods

        private static Account GetAccount(Activity activity, VisitType visitType, DateTime accountCreatedDate, DateTime admitDate)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF");

            return new Account
            {
                Activity = activity,
                Facility = facility,
                AdmitDate = admitDate,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType
            };
        }

        private IHIEShareDataPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate, DateTime admitDate)
        {
            var view = MockRepository.GenerateMock<IHIEShareDataFlagView>();
            var account = GetAccount(activity, patientType, accountCreatedDate, admitDate);
            return HIEShareDataPresenterFactory.GetPresenter(view, new HIEConsentFeatureManager(), account);
        }

        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(-10);
        }
        private static DateTime GetAdmitDateAfterFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAdmitDateBeforeFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(-10);
        }


        #endregion
    }


}
