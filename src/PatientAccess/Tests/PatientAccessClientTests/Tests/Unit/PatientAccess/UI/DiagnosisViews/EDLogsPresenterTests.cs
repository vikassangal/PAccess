using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.DiagnosisViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class EDLogsPresenterTests
    {
        [Test]
        public void TestUpdateSelectedModeOfArrival()
        {
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            var account = new Account { Activity = new PostMSERegistrationActivity() };
            var edLogsDisplayPresenter = new EDLogsDisplayPresenter( edLogView, account, ruleEngine );
            const string code = "1";
            var modeOfArrival = new ModeOfArrival( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Hospital", code );

            edLogsDisplayPresenter.UpdateSelectedModeOfArrival( modeOfArrival );

            Assert.AreEqual( code, edLogsDisplayPresenter.Model.ModeOfArrival.Code );
            edLogsDisplayPresenter.RuleEngine.AssertWasCalled( x => x.OneShotRuleEvaluation<ModeOfArrivalRequired>( Arg<object>.Is.Anything, Arg<EventHandler>.Is.Anything ) );
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitTypeBlank()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = string.Empty };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());

        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType1()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "1" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType2()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "2" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType2_FieldsHaveData()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(true);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "2" };

            presenter.UpdateEDLogDisplay(visitType, false, true);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType3()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "3" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType3_ViewIsBeingUpdated()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "3" };

            presenter.UpdateEDLogDisplay(visitType, true, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType4()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "4" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsMaintenance_VisitType9()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new MaintenanceActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "9" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsPostMseRegistration_VisitTypeBlank()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new PostMSERegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = string.Empty };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());

        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsPostMseRegistration_VisitType3()
        {
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            edLogView.Stub(view => view.HasData()).Return(false);

            Activity activity = new PostMSERegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "3" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsRegistration_VisitTypeBlank()
        {
            var activity = new RegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = string.Empty };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());

        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsRegistration_VisitType9()
        {
            var activity = new RegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "9" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());
        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsRegistration_VisitType4()
        {
            var activity = new RegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "4" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasCalled(view => view.ClearFields());
            edLogView.AssertWasCalled(view => view.DoNotShow());
            edLogView.AssertWasNotCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());

        }

        [Test]
        public void TestUpdateEDLogDisplay_WhenActivityIsRegistration_VisitType3()
        {
            var activity = new RegistrationActivity();
            var account = new Account() { Activity = activity };
            var ruleEngine = MockRepository.GenerateMock<IRuleEngine>();
            var edLogView = MockRepository.GenerateStub<IEDLogView>();
            var presenter = new EDLogsDisplayPresenter(edLogView, account, ruleEngine);
            var visitType = new VisitType { Code = "3" };

            presenter.UpdateEDLogDisplay(visitType, false, false);

            Assert.IsTrue(IsModelCleared(presenter));

            edLogView.AssertWasNotCalled(view => view.ClearFields());
            edLogView.AssertWasNotCalled(view => view.DoNotShow());
            edLogView.AssertWasCalled(view => view.ShowEnabled());
            edLogView.AssertWasNotCalled(view => view.ShowDisabled());

        }

        private static bool IsModelCleared( EDLogsDisplayPresenter presenter )
        {
            return presenter.Model.ArrivalTime == DateTime.MinValue &&
                   IsCleared(presenter.Model.ModeOfArrival) &&
                   IsCleared(presenter.Model.ReferralType) &&
                   IsCleared(presenter.Model.ReferralFacility) &&
                   IsCleared(presenter.Model.ReAdmitCode); 
        }

        private static bool IsCleared( CodedReferenceValue arrival )
        {
            return arrival.Code == String.Empty && 
                   arrival.Description == String.Empty;
        }

    }
}
