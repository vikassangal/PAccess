using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.DiagnosisViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DiagnosisViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class DiagnosisViewPresenterTests
    {

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitTypeBlank()
        {
            var diagnosisView = GetStubDiagnosisView();


            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = string.Empty };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType2()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var kind = new VisitType { Code = "2" };

            diagnosisViewPresenter.HandleProcedureField(kind, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType3()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "3" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType1_ViewIsBeingUpdated()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "1" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType3_ViewIsBeingUpdated()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var kind = new VisitType { Code = "3" };

            diagnosisViewPresenter.HandleProcedureField(kind, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaitenance_VisitType9_ViewIsBeingUpdated()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "9" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaitenance_VisitType4_ViewIsBeingUpdated()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "4" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType2_ViewIsBeingUpdated()
        {
            var diagnosisView = GetStubDiagnosisView();
            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "2" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsPostMseReg_VisitTypeIsBlank()
        {
            var diagnosisView = GetStubDiagnosisView();

            var activity = new PostMSERegistrationActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = string.Empty };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsPostMseReg_VisitType3()
        {
            var diagnosisView = GetStubDiagnosisView();

            var activity = new PostMSERegistrationActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "3" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsMaintenance_VisitType1()
        {
            var diagnosisView = GetStubDiagnosisView();


            var activity = new MaintenanceActivity();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "1" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);

            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());

        }
        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType9()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "9" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());

        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType4()
        {
            Activity activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "4" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());

        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType2()
        {
            Activity activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "2" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());

        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType3_ViewIsBeingUpdated()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "3" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureDisabled());

        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType1()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "1" };

            diagnosisViewPresenter.HandleProcedureField(visitType, false);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType1_ViewIsBeingUpdated()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "1" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType4_ViewIsBeingUpdated()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "4" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType2_ViewIsBeingUpdated()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "2" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        [Test]
        public void TestHandleProcedureField_WhenActivityIsReg_VisitType9_ViewIsBeingUpdated()
        {
            var activity = new RegistrationActivity();
            var diagnosisView = GetStubDiagnosisView();
            var diagnosisViewPresenter = new DiagnosisViewPresenter(diagnosisView, activity);
            var visitType = new VisitType { Code = "9" };

            diagnosisViewPresenter.HandleProcedureField(visitType, true);

            Assert.IsTrue(diagnosisView.Model.Diagnosis.Procedure == String.Empty);
            diagnosisView.AssertWasNotCalled(view => view.ClearProcedureField());
            diagnosisView.AssertWasCalled(view => view.ShowProcedureEnabled());
            diagnosisView.AssertWasNotCalled(view => view.ShowProcedureDisabled());
        }

        private static IDiagnosisView GetStubDiagnosisView()
        {
            var diagnosisView = MockRepository.GenerateStub<IDiagnosisView>();
            diagnosisView.Model = new Account { Diagnosis = new Diagnosis() };
            return diagnosisView;
        }
    }
}
