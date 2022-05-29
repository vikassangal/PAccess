using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.DemographicsViews;
using PatientAccess.UI.HelperClasses;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DemographicsViews
{
    [TestFixture]
    [Category("Fast")]
    public class BirthTimePresenterTests
    {
        [Test]
        public void TestValidate_WhenInvalidTime1IsEntered_ShowErrorMessage()
        {
            var facilityDateTime = DateTime.Now;

            const string timeEntered = "10";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG),
                            Arg<string>.Is.Anything,
                            Arg<MessageBoxButtons>.Is.Anything,
                            Arg<MessageBoxIcon>.Is.Anything,
                            Arg<MessageBoxDefaultButton>.Is.Anything))
                            .Return(DialogResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now, DateTime.Now, messageBoxAdapter);

            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenInvalidTime2IsEntered_ShowErrorMessage()
        {
            var facilityDateTime = DateTime.Now;

            const string timeEntered = "11:60";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG),
                            Arg<string>.Is.Anything,
                            Arg<MessageBoxButtons>.Is.Anything,
                            Arg<MessageBoxIcon>.Is.Anything,
                            Arg<MessageBoxDefaultButton>.Is.Anything))
                            .Return(DialogResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now, DateTime.Now, messageBoxAdapter);

            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenInvalidTime3IsEntered_ShowErrorMessage()
        {
            var facilityDateTime = DateTime.Now;

            const string timeEntered = "24:00";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG),
                            Arg<string>.Is.Anything,
                            Arg<MessageBoxButtons>.Is.Anything,
                            Arg<MessageBoxIcon>.Is.Anything,
                            Arg<MessageBoxDefaultButton>.Is.Anything))
                            .Return(DialogResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now, DateTime.Now, messageBoxAdapter);

            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenMidnightIsEntered_ShowErrorMessage()
        {
            var facilityDateTime = DateTime.Now;

            const string timeEntered = "00:00";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);


            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.ShowMessageBox(Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_INVALID_ERRMSG),
                                    Arg<string>.Is.Anything,
                                    Arg<MessageBoxAdapterButtons>.Is.Anything,
                                    Arg<MessageBoxAdapterIcon>.Is.Anything,
                                    Arg<MessageBoxAdapterDefaultButton>.Is.Anything))
                                    .Return(MessageBoxAdapterResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now, DateTime.Now, messageBoxAdapter);

            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenValidTime1IsEnteredEqualAdmitTime_DoNotShowErrorMessage()
        {
            var facilityDateTime = new DateTime(2011, 02, 02, 11, 00, 00);

            const string timeEntered = "10:30";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            //using a strick mock with no expectations as we want to make sure that no methods are called
            var messageBoxAdapter = MockRepository.GenerateStrictMock<IMessageBoxAdapter>();

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now,
                                                                                            DateTime.Now.Date.AddHours(10).AddMinutes(30), messageBoxAdapter);
            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();

        }

        [Test]
        public void TestValidate_WhenValidTime2IsEnteredEqualAdmitTime_DoNotShowErrorMessage()
        {
            var facilityDateTime = new DateTime(2011, 02, 02, 11, 00, 00);

            const string timeEntered = "00:01";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            //using a strick mock with no expectations as we want to make sure that no methods are called
            var messageBoxAdapter = MockRepository.GenerateStrictMock<IMessageBoxAdapter>();

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now,
                                                                                            DateTime.Now.Date.AddHours(10).AddMinutes(30), messageBoxAdapter);
            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenValidTime1IsEnteredBeforeAdmitTime_ShowWarningMessage()
        {
            var facilityDateTime = new DateTime(2011, 02, 02, 11, 00, 00);

            const string timeEntered = "10:30";
            var mtbBirthTime = GetMtbBirthTime(timeEntered);
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.ShowMessageBox(
                    Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_BEFORE_ADMIT_TIME),
                    Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME),
                    Arg<MessageBoxAdapterButtons>.Is.Equal(MessageBoxAdapterButtons.YesNo),
                    Arg<MessageBoxAdapterIcon>.Is.Equal(MessageBoxAdapterIcon.Warning),
                    Arg<MessageBoxAdapterDefaultButton>.Is.Equal(MessageBoxAdapterDefaultButton.Button2)))
                    .Return(MessageBoxAdapterResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new AdmitNewbornActivity(), DateTime.Now,
                                                                                            DateTime.Now.Date.AddHours(10).AddMinutes(40), messageBoxAdapter);
            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestValidate_WhenValidTime1IsEnteredAfterAdmitTime_ShowErrorMessage()
        {
            var facilityDateTime = new DateTime(2011, 02, 02, 11, 00, 00);

            const string timeEntered = "10:30";

            var mtbBirthTime = GetMtbBirthTime(timeEntered);
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.ShowMessageBox(Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME_CANNOT_BE_AFTER_ADMIT_TIME),
                                    Arg<string>.Is.Equal(UIErrorMessages.BIRTH_TIME),
                                    Arg<MessageBoxAdapterButtons>.Is.Equal(MessageBoxAdapterButtons.OK),
                                    Arg<MessageBoxAdapterIcon>.Is.Equal(MessageBoxAdapterIcon.Exclamation),
                                    Arg<MessageBoxAdapterDefaultButton>.Is.Equal(MessageBoxAdapterDefaultButton.Button1)))
                                    .Return(MessageBoxAdapterResult.OK);

            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(facilityDateTime, new RegistrationActivity(), DateTime.Now,
                                                                                            DateTime.Now.Date.AddHours(10).AddMinutes(10), messageBoxAdapter);
            presenterUnderTest.Validate(mtbBirthTime);

        }

        [Test]
        [Ignore]
        public void TestValidate_WhenValidTimeAcitivtyEditMaintain_ShowWarningMessage()
        {
            var facilityDateTime = DateTime.Now;

            var timeEntered = (facilityDateTime - TimeSpan.FromHours(1)).ToString("HH:mm");

            var mtbBirthTime = GetMtbBirthTime(timeEntered);

            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.ShowMessageBox(Arg<string>.Is.Equal(UIErrorMessages.EDIT_BIRTHTIME_MESSAGE),
                    Arg<string>.Is.Equal("Patient's Birth Time"),
                    Arg<MessageBoxAdapterButtons>.Is.Equal(MessageBoxAdapterButtons.YesNo),
                    Arg<MessageBoxAdapterIcon>.Is.Equal(MessageBoxAdapterIcon.Warning),
                    Arg<MessageBoxAdapterDefaultButton>.Is.Equal(MessageBoxAdapterDefaultButton.Button2)))
                    .Return(MessageBoxAdapterResult.OK);


            var presenterUnderTest = SetupFixtureAndGetPresenterWithStubs(
                                                                        facilityDateTime,
                                                                        new MaintenanceActivity(),
                                                                        DateTime.Now,
                                                                        DateTime.Now.Date.AddHours(10).AddMinutes(40),
                                                                        messageBoxAdapter);
            presenterUnderTest.Validate(mtbBirthTime);

            messageBoxAdapter.VerifyAllExpectations();
        }

        [Test]
        public void TestUpdateField_WhenPreRegistrationActivityAndAdmitTimeMin_ShouldNotThrowException()
        {
            var patient = new Patient { DateOfBirth = DateTime.Now - TimeSpan.FromDays(1000) };
            var account = new Account
            {
                Patient = patient,
                Activity = new PreRegistrationActivity(),
                AdmitDate = DateTime.MinValue
            };

            var fakeView = new FakeBirthTimeView { ModelAccount = account };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var stubFeatureManager = MockRepository.GenerateStub<IBirthTimeFeatureManager>();

            User user = User.GetCurrent();
            user.Facility = new Facility();
            var timeBroker = MockRepository.GenerateStub<ITimeBroker>();
            var presenterUnderTest = new BirthTimePresenter(fakeView, stubFeatureManager, new MessageBoxAdapter(), ruleEngine, timeBroker, user);

            presenterUnderTest.UpdateField();
        }

        [Test]
        public void UpdateField_RegPreMSE_DOBWithin10days_BirthTimeNotYetEntered_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new PreMSERegisterActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1).Date,
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_RegPreMSE_DOBWithin10days_BirthTimeAlreadyEntered_ShowBirthTimeDisabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new PreMSERegisterActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Disabled);
        }
        [Test]
        public void UpdateField_UCPreMSE_DOBWithin10days_BirthTimeAlreadyEntered_ShowBirthTimeDisabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new UCCPreMSERegistrationActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.OUTPATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Disabled);
        }
        [Test]
        public void UpdateField_RegPreMSE_DOBBeyond10days_BirthTimeNotYetEntered_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new PreMSERegisterActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11).Date,
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }

        [Test]
        public void UpdateField_RegPreMSE_DOBBeyond10days_BirthTimeAlreadyEntered_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new PreMSERegisterActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBWithin10days_BirthTimeNotYetEntered_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1).Date,
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBWithin10days_BirthTimeEnteredWithThisAccount_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1),
                                                                            true,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBWithin10days_BirthTimeEnteredWithOtherAccount_ShowBirthTimeDisabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Disabled);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBBeyond10days_BirthTimeNotYetEntered_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11).Date,
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBBeyond10days_BirthTimeEnteredWithThisAccount_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            true,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }

        [Test]
        public void UpdateField_EditPreMSE_DOBBeyond10days_BirthTimeEnteredWithOtherAccount_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditPreMseActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.EMERGENCY_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }
        [Test]
        public void UpdateField_EditUCPreMSE_DOBBeyond10days_BirthTimeEnteredWithOtherAccount_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new EditUCCPreMSEActivity(), 
                                                                            new VisitType(0, DateTime.Now, VisitType.EMERGENCY_PATIENT_DESC,
                                                                                    VisitType.OUTPATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }
        [Test]
        public void UpdateField_RegNewborn_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new AdmitNewbornActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.INPATIENT_DESC,
                                                                                    VisitType.INPATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.Date,
                                                                            false,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditNewborn_DOBWithin10days_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new MaintenanceActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.INPATIENT_DESC,
                                                                                    VisitType.INPATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.Date,
                                                                            true,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditNewborn_DOBBeyond10days_HideBirthTime()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new MaintenanceActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.INPATIENT_DESC,
                                                                                    VisitType.INPATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            true,
                                                                            ref fakeView);
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Hide);
        }

        [Test]
        public void UpdateField_RegPreNewborn_ShowBirthTimeEnabled()
        {
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(new PreAdmitNewbornActivity(),
                                                                            new VisitType(0, DateTime.Now, VisitType.PREREG_PATIENT_DESC,
                                                                                    VisitType.PREREG_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(1).Date,
                                                                            false,
                                                                            ref fakeView);
            fakeView.ModelAccount.IsNewBorn = true;
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreNewborn_DOBWithin10days_BirthTimeAlreadyEntered_ShowBirthTimeEnabled()
        {
            var activity = new MaintenanceActivity { AssociatedActivityType = typeof(PreAdmitNewbornActivity) };
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(activity,
                                                                            new VisitType(0, DateTime.Now, VisitType.PREREG_PATIENT_DESC,
                                                                                    VisitType.PREREG_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1),
                                                                            true,
                                                                            ref fakeView);
            fakeView.ModelAccount.IsNewBorn = true;
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreNewborn_DOBWithin10days_BirthTimeNotYetEntered_ShowBirthTimeEnabled()
        {
            var activity = new MaintenanceActivity { AssociatedActivityType = typeof(PreAdmitNewbornActivity) };
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(activity,
                                                                            new VisitType(0, DateTime.Now, VisitType.PREREG_PATIENT_DESC,
                                                                                    VisitType.PREREG_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-1).Date,
                                                                            false,
                                                                            ref fakeView);
            fakeView.ModelAccount.IsNewBorn = true;
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreNewborn_DOBBeyond10days_BirthTimeAlreadyEntered_ShowBirthTimeEnabled()
        {
            var activity = new MaintenanceActivity { AssociatedActivityType = typeof(PreAdmitNewbornActivity) };
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(activity,
                                                                            new VisitType(0, DateTime.Now, VisitType.PREREG_PATIENT_DESC,
                                                                                    VisitType.PREREG_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(-11),
                                                                            true,
                                                                            ref fakeView);
            fakeView.ModelAccount.IsNewBorn = true;
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        [Test]
        public void UpdateField_EditPreNewborn_DOBBeyond10days_BirthTimeNotYetEntered_ShowBirthTimeEnabled()
        {
            var activity = new MaintenanceActivity { AssociatedActivityType = typeof(PreAdmitNewbornActivity) };
            FakeBirthTimeView fakeView = null;
            var presenterUnderTest = SetupPresenterWithFakeBirthTimeView(activity,
                                                                            new VisitType(0, DateTime.Now, VisitType.PREREG_PATIENT_DESC,
                                                                                    VisitType.PREREG_PATIENT),
                                                                            DateTime.Now,
                                                                            DateTime.Now.AddDays(1).Date,
                                                                            false,
                                                                            ref fakeView);
            fakeView.ModelAccount.IsNewBorn = true;
            presenterUnderTest.UpdateField();
            Assert.IsTrue(fakeView.BirthTimeStatus == BirthTimeStatus.Enabled);
        }

        private static MaskedEditTextBox GetMtbBirthTime(string timeEntered)
        {
            return new MaskedEditTextBox
                       {
                           EntrySelectionStyle = EntrySelectionStyle.SelectionAtEnd,
                           KeyPressExpression = "^\\d*$",
                           Mask = "  :",
                           MaxLength = 5,
                           ValidationExpression = "^([0-1][0-9]|2[0-3])([0-5][0-9])$",
                           Text = timeEntered
                       };
        }

        private static BirthTimePresenter SetupFixtureAndGetPresenterWithStubs(DateTime facilityDateTime, Activity activity,
                DateTime patientBirthDay, DateTime admitDate, IMessageBoxAdapter messageBoxAdapter)
        {
            var stubView = MockRepository.GenerateStub<IBirthTimeView>();
            var stubFeatureManager = MockRepository.GenerateStub<IBirthTimeFeatureManager>();
            var account = new Account
                              {
                                  AdmitDate = admitDate,
                                  Patient = new Patient { DateOfBirth = patientBirthDay },
                                  Activity = activity
                              };

            stubView.ModelAccount = account;


            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();

            var timeBroker = MockRepository.GenerateStub<ITimeBroker>();
            timeBroker.Stub(x => x.TimeAt(Arg<int>.Is.Anything, Arg<int>.Is.Anything)).Return(facilityDateTime);

            User user = User.GetCurrent();
            user.Facility = new Facility();

            var presenterUnderTest = new BirthTimePresenter(stubView, stubFeatureManager, messageBoxAdapter, ruleEngine, timeBroker, user);

            return presenterUnderTest;
        }

        private static BirthTimePresenter SetupPresenterWithFakeBirthTimeView(Activity activity, VisitType visitType,
                                        DateTime admitDate, DateTime patientBirthDay, bool birthTimeEnteredWithAccount,
                                        ref FakeBirthTimeView fakeView)
        {
            fakeView = new FakeBirthTimeView();

            var stubFeatureManager = MockRepository.GenerateStub<IBirthTimeFeatureManager>();
            stubFeatureManager.Stub(x => x.IsBirthTimeEnabledForDate(Arg<Activity>.Is.Anything, Arg<DateTime>.Is.Anything)).Return(true);
            stubFeatureManager.Stub(x => x.IsBirthTimeEnabledForDate(Arg<DateTime>.Is.Anything)).Return(true);

            var account = new Account
            {
                AdmitDate = admitDate,
                Patient = new Patient { DateOfBirth = patientBirthDay },
                Activity = activity,
                AccountCreatedDate = DateTime.Now,
                KindOfVisit = visitType,
                BirthTimeIsEntered = birthTimeEnteredWithAccount
            };

            fakeView.ModelAccount = account;

            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();

            var timeBroker = MockRepository.GenerateStub<ITimeBroker>();
            
            User user = User.GetCurrent();
            user.Facility = new Facility();
            
            var presenterUnderTest = new BirthTimePresenter(fakeView, stubFeatureManager, new MessageBoxAdapter(), ruleEngine, timeBroker, user);

            return presenterUnderTest;
        }

    }

    internal class FakeBirthTimeView : IBirthTimeView
    {
        public Account ModelAccount
        { get; set; }

        public BirthTimeStatus BirthTimeStatus;
        public void ShowBirthTimeEnabled()
        {
            BirthTimeStatus = BirthTimeStatus.Enabled;
        }

        public void ShowBirthTimeDisabled()
        {
            BirthTimeStatus = BirthTimeStatus.Disabled;
        }

        public void DisableAndHideBirthTime()
        {
            BirthTimeStatus = BirthTimeStatus.Hide;
        }

        public void PopulateBirthTime(DateTime birthDate)
        {
        }

        public void MakeBirthTimeRequired()
        {
        }

        public void MakeBirthTimePreferred()
        {
        }

    }

    internal enum BirthTimeStatus
    {
        Hide = 0,
        Enabled = 1,
        Disabled = 2
    }
}
