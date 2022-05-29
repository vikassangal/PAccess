using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class HIEShareDataPresenterTests
    {
        #region Registration Activity

        [Test]
        public void TestUpdateView_Registration_AccountCreatedAfterFeatureStartDate_ShouldCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        
        [Test]
        public void TestUpdateView_PreRegistration_AccountCreatedAfterFeatureStartDate_ShouldCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new PreRegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
        public void TestUpdateView_Registration_AccountCreatedBeforeFeatureStartDate_ShouldNotCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateBeforeFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasNotCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
        public void TestUpdateView_ActivatePreRegistration_AccountCreatedAfterFeatureStartDate_ShouldCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new ActivatePreRegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
        public void TestUpdateView_ActivatePreAdmitNewBorn_AccountCreatedAfterFeatureStart_ShouldCall_ShowShareDataWithPublicHIE()
        {
            var activity = new AdmitNewbornActivity { AssociatedActivityType = typeof(ActivatePreRegistrationActivity) };
            if (activity.IsActivatePreAdmitNewbornActivity())
            {
                var presenter = GetPresenterWithMockView(activity, VisitType.Inpatient,
                    GetAccountCreatedDateAfterFeatureStart());
                if (presenter != null)
                {
                    presenter.UpdateView();
                    presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
                }
            }
        }
      [Test]
        public void TestUpdateView_Post_MSE_AccountCreatedAfterFeatureStartDate_ShouldCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Inpatient,
            GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
      public void TestUpdateView_PREMSE_AccountCreateAfterFeatureStartDate__ShouldNotCall_ShowShareDataWithPublicHIEe()
        {
            var presenter = GetPresenterWithMockView(new PreMSERegisterActivity(),VisitType.Inpatient,
            GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
        public void TestUpdateView_UCPREMSE_AccountCreateAfterFeatureStartDate__ShouldCall_ShowShareDataWithPublicHIEe()
        {
            var presenter = GetPresenterWithMockView(new UCCPreMSERegistrationActivity(), VisitType.Inpatient,
                GetAccountCreatedDateAfterFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }
        }
        [Test]
        public void TestUpdateView_UCPostMSE_AccountActivatedBeforeFeatureStart__ShouldCall_ShowShareDataWithPublicHIE()
        {
            var presenter = GetPresenterWithMockView(new UCCPostMseRegistrationActivity(), VisitType.Inpatient,
            GetAccountCreatedDateBeforeFeatureStart());
            if (presenter != null)
            {
                presenter.UpdateView();
                presenter.View.AssertWasCalled(view => view.ShowShareDataWithPublicHIE());
            }

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
                AdmitDate = new DateTime(2016, 07, 26),
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = visitType
            };
        }

        private HIEShareDataPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, DateTime accountCreatedDate)
        {
            var view = MockRepository.GenerateMock<IHIEShareDataFlagView>();
            var account = GetAccount(activity, patientType, accountCreatedDate);
            ShareHIEDataFeatureManager shareHieDataFeatureManager = new ShareHIEDataFeatureManager();
            NotifyPCPDataFeatureManager notifyPCPFeatureManager = new NotifyPCPDataFeatureManager();
            if (shareHieDataFeatureManager.IsShareHieDataEnabledforaccount(account))
            {
                return new HIEShareDataPresenter(view, new ShareHIEDataFeatureManager(), new MessageBoxAdapter(), account,notifyPCPFeatureManager);
            }
            return null;
        }

        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(10);
        }

        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new ShareHIEDataFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion
    }
}
