using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI;
//using PatientAccess.UI;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews.PatientPortal
{
    [TestFixture]
    [Category( "Fast" )]
    public class PatientPortalOptInPresenterTests
    {
        #region Registration Activity
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType1_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.ShowMe() );
        }
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType1_InvalidHSV_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
        }
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType3AndHSV58_AccountCreatedAfterFeatureStart_AccountCreatedAfterFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndHSV58_AccountCreatedAfterFeatureStart_InValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndHSV58_AccountCreatedBeforeFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType1_InValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType1_AccountCreatedBeforeFeatureStart_ValidFacility()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndnonHSV58DuringRegistrationActivity()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.HideMe() );
        }
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType3AndnonHSV58DuringPostMSEActivity()
        {
            var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndnonHSV58DuringPostMSEActivityBeforeFeatureStartDateForPostMSE()
        {
            var presenter = GetPresenterWithMockView(new PostMSERegistrationActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType2AndHSV58()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Outpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.ShowMe() );
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptInSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.Yes;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.OptIn() );
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptOutSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.No;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.OptOut() );
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptUnSelected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.Blank;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled( view => view.UnSelected() );
        }
     
        [Test]
        public void TestPatientPortalOptInView_OptIn_Selected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.Patient = new Patient();
            presenter.Account.Patient.AddContactPoint( contactPoint );
            presenter.OptIn();
            Assert.AreEqual( YesNoFlag.CODE_YES, presenter.Account.PatientPortalOptIn.Code, "Patient Portal OptIn should be YES." );
        }

        [Test]
        public void TestPatientPortalOptInView_OptOut_Selected()
        {
            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.OptOut();
            Assert.AreEqual( YesNoFlag.CODE_NO, presenter.Account.PatientPortalOptIn.Code, "Patient Portal OptIn should be NO." );
        }

        #endregion Registration Activity

        #region Transfer Activity
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType1_AccountCreatedAfterFeatureStart_ForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
         }
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType1_InvalidHSV_AccountCreatedAfterFeatureStart_ForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
        }
        
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndHSV58_AccountCreatedAfterFeatureStart_ForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndHSV58_AccountCreatedBeforeFeatureStart_ForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Emergency, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType1_InForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType1_AccountCreatedBeforeFeatureStart_ForTransfer()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateBeforeFeatureStart(),
                                                     FacilityNotenabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
        [Test]
        public void TestPatientPortalOptInView_NotVisibleForPatientType3AndnonHSV58DuringTransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Emergency, non58Hsv,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.HideMe());
        }
       
        [Test]
        public void TestPatientPortalOptInView_VisibleForPatientType2()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Outpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.ShowMe());
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptInSelected_TransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.Yes;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.OptIn());
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptOutSelected_TransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.No;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.OptOut());
        }

        [Test]
        public void TestPatientPortalOptInView_UpdateViewForPatientType1_OptUnSelected_TransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.PatientPortalOptIn = YesNoFlag.Blank;
            presenter.UpdateView();
            presenter.PatientPortalOptInView.AssertWasCalled(view => view.UnSelected());
        }

        [Test]
        public void TestPatientPortalOptInView_OptIn_Selected_TransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.Account.Patient = new Patient();
            presenter.Account.Patient.AddContactPoint(contactPoint);
            presenter.OptIn();
            Assert.AreEqual(YesNoFlag.CODE_YES, presenter.Account.PatientPortalOptIn.Code, "Patient Portal OptIn should be YES.");
        }

        [Test]
        public void TestPatientPortalOptInView_OptOut_Selected_TransferActivity()
        {
            var presenter = GetPresenterWithMockView(new TransferOutToInActivity(), VisitType.Inpatient, hsv58,
                                                     GetAccountCreatedDateAfterFeatureStart(),
                                                     FacilityEnabledForPatientPortal);
            presenter.OptOut();
            Assert.AreEqual(YesNoFlag.CODE_NO, presenter.Account.PatientPortalOptIn.Code, "Patient Portal OptIn should be NO.");
        }

        #endregion Transfer Activity

        #region Support Methods
        private static Account GetAccount(Activity activity, VisitType visitType, HospitalService hospitalService, DateTime accountCreatedDate, bool featureEnabledForFacility)
        {
            var facility = new Facility( PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF" );
            if (featureEnabledForFacility )
            {
            facility["IsPatientPortalOptInEnabled"] = true;
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
        private PatientPortalOptInPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, HospitalService hsv, DateTime accountCreatedDate, bool featureEnabledForFacility)
        {
            var view = MockRepository.GenerateMock<IPatientPortalOptInView>();
            var account = GetAccount(activity, patientType, hsv, accountCreatedDate, featureEnabledForFacility);

            return new PatientPortalOptInPresenter(view, new MessageBoxAdapter(),
                                account, new PatientPortalOptInFeatureManager(),
                                                   RuleEngine.GetInstance());
        }
        #endregion

        #region Constants
        private const String EmailAddress = "JOHN@GMAIL.COM";
        private const bool FacilityEnabledForPatientPortal = true;
        private const bool FacilityNotenabledForPatientPortal = false;
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
            return new PatientPortalOptInFeatureManager().FeatureStartDate.AddDays(10);
        }
       
        private static DateTime GetAccountCreatedDateBeforeFeatureStart()
        {
            return new PatientPortalOptInFeatureManager().FeatureStartDate.AddDays(-10);
        }
       
        #endregion
    }

}
