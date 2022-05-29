using System;
using System.Web;
using Extensions.Persistence;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.DiagnosisViews;
using PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferEmergencyToOutpatientPresenter
    /// </summary>
    [TestFixture]
    public class TransferEmergencyToOutpatientPresenterTests
    {
        private Account account;
        private IMessageBoxAdapter messageBoxAdapter;
        private IAlternateCareFacilityView mockAlternateCareFacilityRToOutPatTransferView;
        private IEmergencyPatientToOutPatientStep1View mockERToOutPatTransferView;

        [TestFixtureSetUp]
        public void SetUpOutpatientToERpatientPresenterTests()
        {
            User.GetCurrent().Facility = GetStubFacility();
            HttpRuntime.Cache[TimeOutFormView.APPLICATION_INACTIVITY_TIMEOUT] = "10";
            mockAlternateCareFacilityRToOutPatTransferView = MockRepository.GenerateStub<IAlternateCareFacilityView>();
            mockERToOutPatTransferView = MockRepository.GenerateStub<IEmergencyPatientToOutPatientStep1View>();
            messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
        }

        #region Tests

        [Test]
        [Category( "Fast" )]
        public void TestConstructor()
        {
            account = GetNewAccount();
            var presenter = new EmergencyPatientToOutPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, new AlternateCareFacilityPresenter
                                                                                        (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                         new AlternateCareFacilityFeatureManager
                                                                                             ()),
                                                                           RuleEngine.GetInstance());
            Assert.IsNotNull(presenter.Step1View);
            Assert.IsNotNull(presenter.Account);
            Assert.IsNotNull(presenter.RuleEngine);
            Assert.IsNotNull(presenter.AlternateCareFacilityPresenter);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullAccount_ShouldThrowException()
        {
            new EmergencyPatientToOutPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                           null, new AlternateCareFacilityPresenter
                                                                     (mockAlternateCareFacilityRToOutPatTransferView,
                                                                      new AlternateCareFacilityFeatureManager
                                                                          ()),
                                                           RuleEngine.GetInstance());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullRulesEngine_ShouldThrowException()
        {
            account = new Account {Activity = new TransferERToOutpatientActivity()};
            new EmergencyPatientToOutPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                           account, new AlternateCareFacilityPresenter
                                                                        (mockAlternateCareFacilityRToOutPatTransferView,
                                                                         new AlternateCareFacilityFeatureManager
                                                                             ()),
                                                           null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullAlternateCareFacilityPresenter_ShouldThrowException()
        {
            account = new Account {Activity = new TransferERToOutpatientActivity()};
            new EmergencyPatientToOutPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                           account, null,
                                                           RuleEngine.GetInstance());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullView_ShouldThrowException()
        {
            account = new Account {Activity = new TransferERToOutpatientActivity()};
            new EmergencyPatientToOutPatientStep1Presenter(null, messageBoxAdapter,
                                                           account, new AlternateCareFacilityPresenter
                                                                        (mockAlternateCareFacilityRToOutPatTransferView,
                                                                         new AlternateCareFacilityFeatureManager
                                                                             ()),
                                                           RuleEngine.GetInstance());
        }

        [Test]
        [Category( "Fast" )]
        public void TestUpdateView_ShouldDisplayContextDescrition()
        {
            PrepareMockERToOutPatTransferView();
            var presenter = new EmergencyPatientToOutPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, new AlternateCareFacilityPresenter
                                                                                        (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                         new AlternateCareFacilityFeatureManager
                                                                                             ()),
                                                                           RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.AreEqual(mockERToOutPatTransferView.UserContextView, "Transfer ER Patient to Bedded Outpatient",
                            "Context description not displayed");
        }

        [Test]
        public void
            Test_PrevalidationSucess_When_PT3FCNot37_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired()
        {
            PrepareMockERToOutPatTransferView();
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsTrue(presenter.PreValidationSuccess);
        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUpdateView_With_PT2FCNot37_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.KindOfVisit = VisitType.Outpatient;
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUpdateView_PT3FCIs37_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.FinancialClass = ERfinacialClass;
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUpdateView_PT3FCIsNot37_AbstractExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail()
        {
            PrepareMockERToOutPatTransferView();
            account.AbstractExists = true;
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT3FCIsNot37_AbstractDoesNotExist_DischargeDateMoreThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.DischargeDate = DateTime.Now.AddDays(-4);
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUpdateView_PT3FCIsNot37_AbstractDoesNotExist_DischargeDateLessThan3Nights_NotLocked_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.AccountLock.IsLocked = false;
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        [Category( "Fast" )]
        public void
            TestUpdateView_PT3FCIsNot37_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockNotAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.AccountLock.AcquiredLock = false;
            var presenter =
                MockRepository.GenerateStub<EmergencyPatientToOutPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        new AlternateCareFacilityPresenter
                                                                                            (mockAlternateCareFacilityRToOutPatTransferView,
                                                                                             new AlternateCareFacilityFeatureManager
                                                                                                 ()),
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        #endregion Tests

        #region Helper Methods

        private static readonly VisitType visitTypeER = VisitType.Emergency;

        private static readonly AdmitSource admitSource =
            new AdmitSource(PersistentModel.NEW_OID, DateTime.Now, "Admit Source", "02");

        private static readonly FinancialClass finacialClass = new FinancialClass(4,
                                                                                  Extensions.PersistenceCommon.
                                                                                      PersistentModel.NEW_VERSION,
                                                                                  "BLOOD TRANSFUSION",
                                                                                  "63");

        private static readonly FinancialClass ERfinacialClass = new FinancialClass(37,
                                                                                    Extensions.PersistenceCommon.
                                                                                        PersistentModel.NEW_VERSION,
                                                                                    "Emergency",
                                                                                    "37");

        private static readonly Bed bed = new Bed(1L, DateTime.Now, "Bed001", "B001",
                                                  new Accomodation(1L, DateTime.Now, "SPL", "001"),
                                                  true);

        private static readonly Location location = new Location(1L, DateTime.Now, "CA", "CA",
                                                                 new NursingStation(1L, DateTime.Now, "NursingStation",
                                                                                    "NS"),
                                                                 new Room(1L, DateTime.Now, "Room", "RM"), bed);

        private static readonly AccountLock accountLock = new AccountLock {AcquiredLock = true, IsLocked = true};

        private static Account GetNewAccount()
        {
            var account = new Account
                              {
                                  Facility = GetStubFacility(),
                                  Activity = new TransferERToOutpatientActivity(),
                                  KindOfVisit = visitTypeER,
                                  FinancialClass = finacialClass,
                                  AdmitSource = admitSource,
                                  Location = location,
                                  AbstractExists = false,
                                  DischargeDate = DateTime.MinValue,
                                  AccountLock = accountLock
                              };
            return account;
        }

        private void PrepareMockERToOutPatTransferView()
        {
            mockERToOutPatTransferView.PatientContextView1 = new PatientContextView();
            mockERToOutPatTransferView.LocationView1 = new LocationView();
            account = GetNewAccount();
        }

        private static Facility GetStubFacility()
        {
            return new Facility(54, DateTime.Now, "SomeFacility", "hospital code")
                       {
                           FollowupUnit = new FollowupUnit(02, DateTime.Now, "description")
                       };
        }

        #endregion Helper Methods
    }
}