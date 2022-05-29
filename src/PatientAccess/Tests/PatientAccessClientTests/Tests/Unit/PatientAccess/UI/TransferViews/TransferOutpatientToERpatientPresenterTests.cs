using System;
using System.Web;
using Extensions.Persistence;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.TransferViews.EmergencyPatientAndOutPatient;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.TransferViews
{
    /// <summary>
    /// Summary description for TransferOutpatientToERpatientPresenterTest
    /// </summary>
    [TestFixture]
    public class TransferOutpatientToERpatientPresenterTest
    {
        private Account account;
        private IMessageBoxAdapter messageBoxAdapter;
        private IOutPatientToEmergencyPatientStep1View mockERToOutPatTransferView;

        [TestFixtureSetUp]
        public void SetUpOutpatientToERpatientPresenterTests()
        {
            User.GetCurrent().Facility = GetStubFacility();
            HttpRuntime.Cache[TimeOutFormView.APPLICATION_INACTIVITY_TIMEOUT] = "10";
            mockERToOutPatTransferView = MockRepository.GenerateStub<IOutPatientToEmergencyPatientStep1View>();
            messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
        }

        #region Tests

        [Test]
        [Category( "Fast" )]
        public void TestConstructor()
        {
            account = GetNewAccount();
            var presenter = new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, RuleEngine.GetInstance());
            Assert.IsNotNull(presenter.Step1View);
            Assert.IsNotNull(presenter.Account);
            Assert.IsNotNull(presenter.RuleEngine);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullAccount_ShouldThrowException()
        {
            new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                           null, RuleEngine.GetInstance());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullRulesEngine_ShouldThrowException()
        {
            account = new Account {Activity = new TransferERToOutpatientActivity()};
            new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                           account, null);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        [Category( "Fast" )]
        public void TestConstructor_WithNullView_ShouldThrowException()
        {
            account = new Account {Activity = new TransferERToOutpatientActivity()};
            new OutPatientToEmergencyPatientStep1Presenter(null, messageBoxAdapter,
                                                           account, RuleEngine.GetInstance());
        }

        [Test]
        public void TestUpdateView_ShouldDisplayContextDescrition()
        {
            PrepareMockERToOutPatTransferView();
            var presenter = new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.AreEqual(mockERToOutPatTransferView.UserContextView, "Transfer Bedded Outpatient to ER Patient",
                            "Context description not displayed");
        }

        [Test]
        public void
            Test_PrevalidationSucess_When_PT2_HSVBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired()
        {
            User.GetCurrent().Facility = GetStubFacility();
            HttpRuntime.Cache[TimeOutFormView.APPLICATION_INACTIVITY_TIMEOUT] = "10";
            mockERToOutPatTransferView = MockRepository.GenerateStub<IOutPatientToEmergencyPatientStep1View>();
            messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            PrepareMockERToOutPatTransferView();
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsTrue(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_With_PT1_HSVBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.KindOfVisit = VisitType.Inpatient;
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT2_HSVNotBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.HospitalService = hospitalService;
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT2_HSVBedded_AbstractExist_DischargeDateLessThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.DischargeDate = DateTime.Today;
            account.AbstractExists = true;
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT2_HSVBedded_AbstractDoesNotExist_DischargeDateMoreThan3Nights_LockAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.DischargeDate = DateTime.Now.AddDays(-4);
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT2_HSVBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_NotLocked_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.AccountLock.IsLocked = false;
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void
            TestUpdateView_PT2_HSVBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockNotAcquired_PrevalidationShouldFail
            ()
        {
            PrepareMockERToOutPatTransferView();
            account.AccountLock.AcquiredLock = false;
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.IsFalse(presenter.PreValidationSuccess);
        }

        [Test]
        public void TestUpdateView_ShouldDefaultTransferDateToBlank()
        {
            PrepareMockERToOutPatTransferView();
            var presenter = new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.AreEqual(mockERToOutPatTransferView.TransferDate,String.Empty,
                            "Transfer date does not default to blank");
        }
        [Test]
        public void TestUpdateView_ShouldDefaultAdmitSourceToPreviousValue()
        {
            PrepareMockERToOutPatTransferView();
            var presenter = new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.AreEqual(mockERToOutPatTransferView.AdmitSource, account.AdmitSource.ToCodedString(),
                            "Admit Source Does not default to Previous value");
        }
        [Test]
        public void TestUpdateView_ShouldDefaultPatientNameShouldBeDisplayedCompletely()
        {
            Patient patient = new Patient();
            patient.Name = new Name("1234567890123","123456789012345678901234567","M","SUF");
            PrepareMockERToOutPatTransferView();
            account.Patient = patient;
            var presenter = new OutPatientToEmergencyPatientStep1Presenter(mockERToOutPatTransferView, messageBoxAdapter,
                                                                           account, RuleEngine.GetInstance());
            presenter.UpdateView();
            Assert.AreEqual(mockERToOutPatTransferView.PatientName, account.Patient.Name.AsFormattedName(),
                            "Patient name is not completely displayed");
        }
        [Test]
        public void
            Test_ShouldSortHSVCodes_When_PT2_HSVBedded_AbstractDoesNotExist_DischargeDateLessThan3Nights_LockAcquired()
        {
            User.GetCurrent().Facility = GetStubFacility();
            HttpRuntime.Cache[TimeOutFormView.APPLICATION_INACTIVITY_TIMEOUT] = "10";
            mockERToOutPatTransferView = MockRepository.GenerateStub<IOutPatientToEmergencyPatientStep1View>();
            messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            PrepareMockERToOutPatTransferView();
            var presenter =
                MockRepository.GenerateStub<OutPatientToEmergencyPatientStep1Presenter>(mockERToOutPatTransferView,
                                                                                        messageBoxAdapter,
                                                                                        account,
                                                                                        RuleEngine.GetInstance());
            presenter.PopulateHsvCodes();
            mockERToOutPatTransferView.AssertWasCalled(x => x.SetHospitalServiceListSorted());
        }

        #endregion Tests

        #region Helper Methods

        private static readonly VisitType OutpatientVisitType = VisitType.Outpatient;

        private static readonly AdmitSource admitSource =
            new AdmitSource(PersistentModel.NEW_OID, DateTime.Now, "Admit Source", "02");

        private static readonly Bed bed = new Bed(1L, DateTime.Now, "Bed001", "B001",
                                                  new Accomodation(1L, DateTime.Now, "SPL", "001"),
                                                  true);

        private static readonly Location location = new Location(1L, DateTime.Now, "CA", "CA",
                                                                 new NursingStation(1L, DateTime.Now, "NursingStation",
                                                                                    "NS"),
                                                                 new Room(1L, DateTime.Now, "Room", "RM"), bed);

        private static readonly HospitalService hospitalService = new HospitalService(4,
                                                                                      Extensions.PersistenceCommon.
                                                                                          PersistentModel.NEW_VERSION,
                                                                                      "AMBULANCE",
                                                                                      HospitalService.AMBULANCE);

        private static readonly HospitalService dayCareHospitalService = new HospitalService(37,
                                                                                             Extensions.
                                                                                                 PersistenceCommon.
                                                                                                 PersistentModel.
                                                                                                 NEW_VERSION,
                                                                                             "OUTPT_IN_BED_NON_OBS",
                                                                                             HospitalService.HSV57);
        
       

        private static Account GetNewAccount()
        {
            
            var account = new Account
                              {
                                  
                                  Facility = GetStubFacility(),
                                  Activity = new TransferOutpatientToERActivity(),
                                  KindOfVisit = OutpatientVisitType,
                                  HospitalService = dayCareHospitalService,
                                  AdmitSource = admitSource,
                                  Location = location,
                                  AbstractExists = false,
                                  DischargeDate = DateTime.MinValue,
                                  AccountLock = new AccountLock { AcquiredLock = true, IsLocked = true }
                              };
            account.HospitalService.DayCare = "Y";
            return account;
        }

        private void PrepareMockERToOutPatTransferView()
        {
            mockERToOutPatTransferView.PatientContextView1 = new PatientContextView();
            account = GetNewAccount();
        }

        private static Facility GetStubFacility()
        {
            return new Facility(54, DateTime.Now, "DHF", "DHF")
                       {
                           FollowupUnit = new FollowupUnit(02, DateTime.Now, "description")
                       };
        }

        #endregion Helper Methods
    }
}