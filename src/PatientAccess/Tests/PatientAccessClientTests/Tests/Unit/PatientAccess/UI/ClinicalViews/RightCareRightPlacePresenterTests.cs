using System;
using System.Configuration;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI.ClinicalViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.ClinicalViews
{
    [TestFixture]
    public class RightCareRightPlacePresenterTests
    {
        [Test]
        public void UpdateView_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterRelease_RCRPShouldBeVisibleAndEnabled()
        {
            RightCareRightPlacePresenter presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, DateTime.Today, true );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasCalled( x => x.PopulateRCRP() );
            Assert.IsTrue( view.RCRPVisible );
            Assert.IsTrue( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsRegistrationInValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterRelease_RCRPShouldBeVisibleAndEnabled()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, DateTime.Now, false );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasNotCalled( x => x.PopulateRCRP() );
            Assert.IsFalse( view.RCRPVisible );
            Assert.IsFalse( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsRegistrationValidFacility_PTisOP_AccountCreatedfterRelease_AdmitDateAfterRelease_RCRPShouldNotBeVisible()
        {
            var presenter = GetPresenterWith( OPVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, DateTime.Now, true );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasNotCalled( x => x.PopulateRCRP() );
            Assert.IsFalse( view.RCRPVisible );
            Assert.IsFalse( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedBeforeRelease_AdmitDateAfterRelease_RCRPShouldBeCleared()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), DateBeforeRCRPReleaseDate, DateTime.Now, true );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasCalled( x => x.PopulateRCRP() );
            view.AssertWasCalled( x => x.ClearRCRP() );
            Assert.IsTrue( view.RCRPVisible );
            Assert.IsFalse( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateBeforeRelease_ShouldNotClearAndNotCallEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasCalled( x => x.PopulateRCRP() );
            view.AssertWasNotCalled( x => x.ClearRCRP() );
            Assert.IsTrue( view.RCRPVisible );
            Assert.IsFalse( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedBeforeRelease_AdmitDateBeforeRelease_ShouldNotEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), DateBeforeRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasCalled( x => x.PopulateRCRP() );
            Assert.IsTrue( view.RCRPVisible );
            Assert.IsFalse( view.RCRPEnabled );
        }

        [Test]
        public void UpdateView_ActivityIsUCCPOstMseRegistrationValidFacility_AccountCreatedAfterRelease_AdmitDateAfterRelease_ShouldEnable()
        {
            var presenter = GetPresenterWith(VisitType.UCCOutpatient, new UCCPostMseRegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true);
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasCalled(x => x.PopulateRCRP());
            Assert.IsTrue(view.RCRPVisible);
            Assert.IsTrue(view.RCRPEnabled);
        }
        [Test]
        public void UpdateView_ActivityIsUCCPOstMseRegistration_InValidFacility_AccountCreatedAfterRelease_AdmitDateAfterRelease_ShouldNotEnable()
        {
            var presenter = GetPresenterWith(VisitType.UCCOutpatient, new UCCPostMseRegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, false);
            var view = presenter.View;

            presenter.UpdateView();

            view.AssertWasNotCalled(x => x.PopulateRCRP());
            Assert.IsFalse(view.RCRPVisible);
            Assert.IsFalse(view.RCRPEnabled);
        }
        [Test]
        public void TestUpdateLeftOrStayed_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterReleaseRCRPisYes_ShouldEnablePopulteAndEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), DateBeforeRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.Yes;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasCalled( x => x.PopulateLeftOrStayed() );
            view.AssertWasNotCalled( x => x.ClearLeftOrStayed() );
            Assert.IsTrue( view.LeftOrStayedEnabled );

        }

        [Test]
        public void TestUpdateLeftOrStayed_ActivityIsRegistrationValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterReleaseRCRPisNo_ShouldCallVisibleAndEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), DateBeforeRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.No;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasNotCalled( x => x.PopulateLeftOrStayed() );
            view.AssertWasCalled( x => x.ClearLeftOrStayed() );
            Assert.IsFalse( view.LeftOrStayedEnabled );
        }

        [Test]
        public void TestUpdateLeftOrStayed_ActivityIsMaintenanceValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterReleaseRCRPisYes_ShouldCallVisibleAndEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new MaintenanceActivity(), DateBeforeRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.Yes;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasCalled( x => x.PopulateLeftOrStayed() );
            view.AssertWasNotCalled( x => x.ClearLeftOrStayed() );
            Assert.IsTrue( view.LeftOrStayedEnabled );
        }

        [Test]
        public void TestUpdateLeftOrStayed_ActivityIsMaintenanceValidFacility_PTisER_AccountCreatedAfterRelease_AdmitDateAfterReleaseRCRPisNo_ShouldCallVisibleAndEnable()
        {
            var presenter = GetPresenterWith( ERVisitType, new MaintenanceActivity(), DateBeforeRCRPReleaseDate, DateBeforeRCRPReleaseDate, true );
            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.No;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasCalled( x => x.PopulateLeftOrStayed() );
            view.AssertWasNotCalled( x => x.ClearLeftOrStayed() );
            Assert.IsFalse( view.LeftOrStayedEnabled );
        }

        [Test]
        public void TestUpdateRightCareRightPlace_ActivityIsRegistration()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true );

            presenter.UpdateRightCareRightPlace( YesNoFlag.Yes );

            Assert.AreEqual( YesNoFlag.Yes.Code, presenter.Account.RightCareRightPlace.RCRP.Code );
        }

        [Test]
        [Category( "Fast" )]
        public void TestUpdateLeftWithFinancialClearance_WhenActivityIsRegistration()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true );
            presenter.UpdateLeftWithoutFinancialClearance( YesNoFlag.Yes );

            Assert.AreEqual( YesNoFlag.Yes.Code, presenter.Account.LeftWithoutFinancialClearance.Code );
        }

        [Test]
        public void TestUpdateLeftOrStayed_WhenActivityIsRegistration()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true );

            presenter.UpdateLeftOrStayed( LeftOrStayedSelected );

            Assert.AreEqual( LeftOrStayedSelected.Code, presenter.Account.RightCareRightPlace.LeftOrStayed.Code );
        }
        [Test]
        public void TestUpdateLeftOrStayed_WhenActivityIsUCCPostMSERegistrationn_RCRP_isNo()
        {
            var presenter = GetPresenterWith(VisitType.UCCOutpatient, new UCCPostMseRegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true);
          
            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.No;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasNotCalled(x => x.PopulateLeftOrStayed());
            view.AssertWasCalled(x => x.ClearLeftOrStayed());
            Assert.IsFalse(view.LeftOrStayedEnabled);
        }

        [Test]
        public void TestUpdateLeftOrStayed_WhenActivityIsUCCPostMSERegistration_RCRP_isYes()
        {
            var presenter = GetPresenterWith(VisitType.UCCOutpatient, new UCCPostMseRegistrationActivity(),
                TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true);

            presenter.Account.RightCareRightPlace.RCRP = YesNoFlag.Yes;
            var view = presenter.View;

            presenter.UpdateLeftOrStayed();

            view.AssertWasCalled(x => x.PopulateLeftOrStayed());
            view.AssertWasNotCalled(x => x.ClearLeftOrStayed());
            Assert.IsTrue(view.LeftOrStayedEnabled);
        }

        [Test]
        [Category( "Fast" )]
        public void TestUpdateLeftWithOutBeingSeen_WhenActivityIsRegistration()
        {
            var presenter = GetPresenterWith( ERVisitType, new RegistrationActivity(), TestDateAfterRCRPReleaseDate, TestDateAfterRCRPReleaseDate, true );
            presenter.UpdateLeftWithoutBeingSeen( YesNoFlag.Yes );

            Assert.AreEqual( YesNoFlag.Yes.Code, presenter.Account.LeftWithOutBeingSeen.Code );
        }

        #region Constants

        private readonly DateTime TestDateAfterRCRPReleaseDate = DateTime.Parse( "01-05-2011" );
        private readonly DateTime DateBeforeRCRPReleaseDate = DateTime.Parse( "01-01-2009" );

        #endregion

        private readonly LeftOrStayed LeftOrStayedSelected = new LeftOrStayed( PersistentModel.NEW_OID,
                                                                              PersistentModel.NEW_VERSION,
                                                                              LeftOrStayed.LEFT_DESCRIPTION,
                                                                              LeftOrStayed.LEFT );

        private readonly VisitType ERVisitType = new VisitType( PersistentModel.NEW_OID,
                                                               PersistentModel.NEW_VERSION,
                                                               VisitType.EMERGENCY_PATIENT_DESC,
                                                               VisitType.EMERGENCY_PATIENT );

        private readonly VisitType OPVisitType = new VisitType( PersistentModel.NEW_OID,
                                                               PersistentModel.NEW_VERSION,
                                                               VisitType.OUTPATIENT_DESC,
                                                               VisitType.OUTPATIENT );

        private static RightCareRightPlacePresenter GetPresenterWith( VisitType visitType, Activity activity, DateTime accountCreatedDate, DateTime admitDate, bool facilityIsRCRPEnabled )
        {
            var account = GetAccount( visitType, activity, accountCreatedDate, admitDate, facilityIsRCRPEnabled );
            var view = GetStubClinicalView( account );
            var featureManager = new RightCareRightPlaceFeatureManager( ConfigurationManager.AppSettings );
            return new RightCareRightPlacePresenter( view, featureManager, account );
        }

        private static IRightCareRigtPlaceView GetStubClinicalView( Account account )
        {
            var clinicalView = MockRepository.GenerateStub<IRightCareRigtPlaceView>();
            clinicalView.Model_Account = account;
            return clinicalView;
        }

        private static Account GetAccount( VisitType visitType, Activity activity, DateTime accountCreatedDate, DateTime admitDate, bool facilityIsRCRPEnabled )
        {
            var facility = new Facility( PersistentModel.NEW_OID,
                                        PersistentModel.NEW_VERSION,
                                        "DOCTORS HOSPITAL DALLAS",
                                        "DHF" );
            if ( facilityIsRCRPEnabled )
            {
                facility["IsFacilityRCRPEnabled"] = facilityIsRCRPEnabled;
            }
            var account = new Account
                              {
                                  Facility = facility,
                                  Activity = activity,
                                  AccountCreatedDate = accountCreatedDate,
                                  AdmitDate = admitDate,
                                  KindOfVisit = visitType
                              };
            return account;
        }
    }
}