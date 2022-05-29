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
    [Category( "Fast" )]
    public class AuthorizeAdditionalPortalUsersPresenterTests
    {
        #region Registration Activity
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType1_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType1_InvalidHSV_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType3AndHSV58_AccountCreatedAfterFeatureStart_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_NotVisibleForPatientType3AndHSV58_AccountCreatedAfterFeatureStart_InValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.HideAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_NotVisibleForPatientType3AndHSV58_AccountCreatedBeforeFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.HideAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_NotVisibleForPatientType1_InValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.HideAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_NotVisibleForPatientType1_AccountCreatedBeforeFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.HideAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_NotVisibleForPatientType3AndnonHSV58DuringRegistrationActivity()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.HideAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType3AndnonHSV58DuringPostMSEActivity()
        {
            var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType3AndnonHSV58DuringPostMSEActivityBeforeFeatureStartDateForPostMSE()
        {
            var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType2AndHSV58()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Outpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }

      

        #endregion Registration Activity

        
        [Test]
        public void TestAuthorizeAdditionalPortalUsersView_VisibleForPatientType2()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Outpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForAuthorizeAdditionalPortalUsers);
            presenter.UpdateView();
            presenter.AuthorizeAdditionalPortalUsersView.AssertWasCalled(view => view.ShowAuthorizeAdditionalPortalUser());
        }
    

        #region Support Methods
        private static Account GetAccount(Activity activity, VisitType visitType, HospitalService hospitalService, DateTime accountCreatedDate, bool featureEnabledForFacility)
        {
            var facility = new Facility( PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF" );
            if (featureEnabledForFacility )
            {
                facility["IsAuthorizePortalUserEnabled"] = true;
                }

            return new Account
                       {
                           Activity = activity ,
                           Facility = facility,
                           AdmitDate = new DateTime(2013, 07, 20),
                           AccountCreatedDate = accountCreatedDate,
                           KindOfVisit = visitType,
                           HospitalService = hospitalService
                       };
        }
        private AuthorizeAdditionalPortalUsersPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, HospitalService hsv, DateTime accountCreatedDate, bool featureEnabledForFacility)
        {
            var view = MockRepository.GenerateMock<IAuthorizeAdditionalPortalUsersView>();
            var account = GetAccount(activity, patientType, hsv, accountCreatedDate, featureEnabledForFacility);

            return new AuthorizeAdditionalPortalUsersPresenter(view,
                                account, new AuthorizePortalUserFeatureManager());
        }
        #endregion

        #region Constants
        private const String EmailAddress = "JOHN@GMAIL.COM";
        private const bool FacilityEnabledForAuthorizeAdditionalPortalUsers = true;
        private const bool FacilityNotenabledForAuthorizeAdditionalPortalUsers = false;
        private readonly ContactPoint contactPoint = new ContactPoint
        {
            TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType(),
            EmailAddress = new EmailAddress( EmailAddress )
        };
        private readonly HospitalService non58Hsv = new HospitalService
        {
            Code = HospitalService.PSYCH_NON_LOCKED,
            Description = "PSYCH_NON_LOCKED"
        };

        private readonly HospitalService hsv58 = new HospitalService
        {
            Code = HospitalService.HSV58,
            Description = "HSV 58"
        };
        private static DateTime GetAccountCreatedDateAfterFeatureStart()
        {
            return new AuthorizePortalUserFeatureManager().FeatureStartDate.AddDays(10);
        }
       
        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new AuthorizePortalUserFeatureManager().FeatureStartDate.AddDays(-10);
        }
       
        #endregion
    }

}
