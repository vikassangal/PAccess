using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.RegulatoryViews
{
    [TestFixture]
    [Category("Fast")]
    public class HospitalCommunicationOptInPresenterTests
    {
        #region Registration Activity
        [Test]
        public void TestHospitalCommunicationOptInView_VisibleForPatientType1_AccountCreatedAfterFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, GetAccountCreatedDateAfterFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.ShowMe());
        }
        [Test]
        public void TestHospitalCommunicationOptInView_NotVisibleForPatientType0_AccountCreatedAfterFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new PreRegistrationActivity(), VisitType.PreRegistration, GetAccountCreatedDateAfterFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.HideMe());
        }
        public void TestPatientHospitalCommunicationOptInView_NotVisibleForPatientType1_AccountCreatedBeforeFeatureStart()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateBeforeFeatureStart());
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.HideMe());
        }

        [Test]
        public void TestHospitalCommunicationOptInView_UpdateViewForPatientType1_OptInSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateAfterFeatureStart());
            presenter.Account.Patient.HospitalCommunicationOptIn = YesNoFlag.Yes;
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.OptIn());
        }

        [Test]
        public void TestPatientHospitalCommunicationOptInView_UpdateViewForPatientType1_OptOutSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateAfterFeatureStart());
            presenter.Account.Patient.HospitalCommunicationOptIn = YesNoFlag.No;
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.OptOut());
        }

        [Test]
        public void TestPatientHospitalCommunicationOptInView_UpdateViewForPatientType1_OptUnSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateAfterFeatureStart());
            presenter.Account.Patient.HospitalCommunicationOptIn = YesNoFlag.Blank;
            presenter.UpdateView();
            presenter.View.AssertWasCalled(view => view.UnSelected());
        }
        public void TestPatientHospitalCommunicationOptInView_OptIn_Selected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateAfterFeatureStart());
            presenter.Account.Patient = new Patient();
            presenter.Account.Patient.AddContactPoint(contactPoint);
            presenter.OptIn();
            Assert.AreEqual(YesNoFlag.CODE_YES, presenter.Account.Patient.HospitalCommunicationOptIn.Code, "patient Marketing Email OptIn should be YES.");
        }

        [Test]
        public void TestPatientHospitalCommunicationInView_OptOut_Selected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                                                     GetAccountCreatedDateAfterFeatureStart());
            presenter.OptOut();
            Assert.AreEqual(YesNoFlag.CODE_NO, presenter.Account.Patient.HospitalCommunicationOptIn.Code, "Patient Marketing Email OptIn should be NO.");
        }


        #endregion
        #region Support Methods
        private static Account GetAccount(Activity activity, VisitType visitType, DateTime accountCreatedDate)
        { 
            return new Account
            {
                Activity = activity,
                AdmitDate = new DateTime(2013, 07, 20),
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType,

            };
        }
        private HospitalCommunicationOptInPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate)
        {
            var view = MockRepository.GenerateMock<IHospitalCommunicationOptInView>();
            var account = GetAccount(activity, patientType, accountCreatedDate);

            return new HospitalCommunicationOptInPresenter(view, new MessageBoxAdapter(),
                                                   account, new HospitalCommunicationOptInFeatureManager(),
                                                   RuleEngine.GetInstance());
        }
        #endregion

        #region Constants
        private const String EmailAddress = "JOHN@GMAIL.COM";
        private readonly ContactPoint contactPoint = new ContactPoint
        {
            TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType(),
            EmailAddress = new EmailAddress(EmailAddress)
        };
        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new HospitalCommunicationOptInFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new HospitalCommunicationOptInFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion
    }
}
