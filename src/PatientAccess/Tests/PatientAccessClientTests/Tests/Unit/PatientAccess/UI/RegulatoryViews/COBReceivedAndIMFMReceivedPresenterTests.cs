using System;
using PatientAccess.Domain;
using NUnit.Framework;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.RegulatoryViews
{
    [TestFixture]
    [Category("Fast")]
    public class COBReceivedAndIMFMReceivedPresenterTests
    {

        #region Registration Activity

        [Test]
        public void TestCOBReceivedView_VisibleForPatientType0_AndFinancialClass2_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "02" };
            var presenter = GetPresenterWithMockView(new PreRegistrationActivity(), VisitType.PreRegistration, GetAccountCreatedDateAfterFeatureStart(),financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_VisibleForPatientType1_AndFinancialClass14_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "14" };
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_VisibleForPatientType3_AndFinancialClass80_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "80" };
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_VisibleForPatientType2_AndFinancialClass20_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "20" };
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Outpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_VisibleForPatientType4_AndFinancialClass81_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "81" };
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Recurring, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_NotVisibleForPatientType4_AndFinancialClass81_AccountCreatedBeforeFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "81" };
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Recurring, GetAccountCreatedDateBeforeFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestCOBReceivedView_NotVisibleForPatientType1_AndFinancialClass26_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "26" };
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowCOBReceived());
        }

        [Test]
        public void TestIMFMReceivedView_NotVisibleForPatientType0_AndFinancialClass2_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "02" };
            var presenter = GetPresenterWithMockView(new PreRegistrationActivity(), VisitType.PreRegistration, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowIMFMReceived());
        }

        [Test]
        public void TestIMFMReceivedView_VisibleForPatientType1_AndFinancialClass4_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "04" };
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowIMFMReceived());
        }

        [Test]
        public void TestIMFMReceivedView_NotVisibleForPatientType2_AndFinancialClass80_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "80" };
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Outpatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowIMFMReceived());
        }

        [Test]
        public void TestIMFMReceivedView_NotVisibleForPatientType3_AndFinancialClass80_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "80" };
            var presenter = GetPresenterWithMockView(new PreMSERegisterActivity(), VisitType.Emergency, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowIMFMReceived());
        }

        [Test]
        public void TestIMFMReceivedView_NotVisibleForPatientType4_AndFinancialClass81_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "81" };
            var presenter = GetPresenterWithMockView(new ShortRegistrationActivity(), VisitType.Recurring, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowIMFMReceived());
        }

        [Test]
        public void TestIMFMReceivedView_NotVisibleForPatientType9_AndFinancialClass44_AccountCreatedAfterFeatureStart()
        {
            var financialClass = new FinancialClass { Code = "44" };
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.NonPatient, GetAccountCreatedDateAfterFeatureStart(), financialClass);
            presenter.UpdateView();
            presenter.View.AssertWasNotCalled(view => view.ShowIMFMReceived());
        }

        #endregion

        #region Support Methods

        private static Account GetAccount(Activity activity, VisitType visitType, DateTime accountCreatedDate,FinancialClass financialClass)
        {
            return new Account
            {
                Activity = activity,
                AdmitDate = new DateTime(2017, 03, 28),
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType,
                FinancialClass = financialClass,

            };
        }
        private COBReceivedAndIMFMReceivedPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate,FinancialClass financialClass)
        {
            var view = MockRepository.GenerateMock<ICOBReceivedAndIMFMReceivedView>();
            var account = GetAccount(activity, patientType, accountCreatedDate,financialClass);
            var Regulatoryview = MockRepository.GenerateMock<IRegulatoryView>();

            return new COBReceivedAndIMFMReceivedPresenter(view, Regulatoryview, new COBReceivedAndIMFMReceivedFeatureManager(), account);
        }

        #endregion

        #region Constants
      
        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion

    }
}
