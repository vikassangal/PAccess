using System;
using System.Web;
using Extensions.Exceptions;
using Extensions.SecurityService.Domain;
using NUnit.Framework;
using PatientAccess;
using PatientAccess.Domain;
using PatientAccess.Persistence.Security;
using PatientAccess.UI;
using PatientAccess.UI.CrashReporting;
using PatientAccess.UI.ExceptionManagement;
using PatientAccess.UI.Logging;
using Rhino.Mocks;
using log4net;
using Facility = Peradigm.Framework.Domain.Parties.Facility;
using User = Extensions.SecurityService.Domain.User;
using Role = Peradigm.Framework.Domain.Security.Role;

namespace Tests.Unit.PatientAccess.UI
{
    /// <summary>
    /// Summary description for PatientAccessViewTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class PatientAccessViewTests
    {
        [Test]
        public void TestOnActivityCancelled_WhenSenderIsNotApplicationExceptionHandler_ShouldThrowEnterpriseExceptionWithCatastrophicSeverity()
        {
            HttpRuntime.Cache[PatientAccessView.APPLICATION_INACTIVITY_TIMEOUT] = "10";
            bool exceptionWasThrown = false;
            var patientAccessView = new MockPatientAccessView();
            var severity = Severity.Ignore;

            try
            {
                patientAccessView.OnActivityCancelled(new object(), EventArgs.Empty);
            }
            catch (EnterpriseException e)
            {
                exceptionWasThrown = true;
                severity = e.Severity;
            }
            finally
            {
                Assert.IsTrue(exceptionWasThrown);
                Assert.IsTrue(severity == Severity.Catastrophic);
            }
        }

        [Test]
        public void TestOnActivityCancelled_WhenSenderIsNotApplicationExceptionHandler_ShouldSwallowExceptionAndCallGoHome()
        {
            HttpRuntime.Cache[PatientAccessView.APPLICATION_INACTIVITY_TIMEOUT] = "10";

            var patientAccessView = new MockPatientAccessView();
            var exceptionHandler = this.GetApplicationExceptionHandlerWithMockDependencies();

            patientAccessView.OnActivityCancelled(exceptionHandler, EventArgs.Empty);

            Assert.IsTrue(patientAccessView.GoHomeWasCalled);
        }

        [Test]
        public void TestRegisterMenuPrivileges_For_DischargeTransferRole_ShouldReturnFalse()
        {
            var securityUser = GetUserWithRoleDischargeTransferUser();
            var RegisterMenuItem = new RegistrationActivity().Description;
            var hasPermission = securityUser.HasPermissionTo(Privilege.Actions.View, RegisterMenuItem, Facility);
            Assert.IsFalse(hasPermission, "DischargeTransferRole does not have permission to the Registration Activity");
        }

        [Test]
        public void TestDischargeActivityPrivileges_For_DischargeTransferRole_ShouldReturnTrue()
        {
            var securityUser = GetUserWithRoleDischargeTransferUser();
            var DischargeActivity = new DischargeActivity().Description;
            var hasPermission = securityUser.HasPermissionTo(Privilege.Actions.View, DischargeActivity, Facility);
            Assert.IsTrue(hasPermission, "DischargeTransferRole has permission to the Discharge Activity");
        }

        [Test]
        public void TestMaintenanceActivityPrivileges_For_DischargeTransferRole_ShouldReturnFalse()
        {
            var securityUser = GetUserWithRoleDischargeTransferUser();
            var MaintenanceActivity = new MaintenanceActivity().Description;
            var hasPermission = securityUser.HasPermissionTo(Privilege.Actions.View, MaintenanceActivity, Facility);
            Assert.IsFalse(hasPermission, "DischargeTransferRole does not have permission to the Maintenence Activity");
        }

        [Test]
        public void TestPrintFaceSheetPrivileges_For_DischargeTransferRole_ShouldReturnTrue()
        {
            var securityUser = GetUserWithRoleDischargeTransferUser();
            var PrintFaceSheetActivity = new PrintFaceSheetActivity().Description;
            var hasPermission = securityUser.HasPermissionTo(Privilege.Actions.View, PrintFaceSheetActivity, Facility);
            Assert.IsTrue(hasPermission, "DischargeTransferRole has permission to PrintFaceSheet");
        }

        private ApplicationExceptionHandler GetApplicationExceptionHandlerWithMockDependencies()
        {
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var applicationAdapter = MockRepository.GenerateMock<IWindowsFormsApplicationAdapter>();
            var logger = MockRepository.GenerateMock<ILog>();
            var exceptionDialogAdapter = MockRepository.GenerateMock<IExceptionDialogAdapter>();
            var activityEventAggregator = MockRepository.GenerateMock<IActivityEventAggregator>();
            var screenCaptureTool = MockRepository.GenerateMock<IScreenCaptureTool>();
            var breadCrumbLogger = MockRepository.GenerateMock<IBreadCrumbLogger>();

            ICache cache = MockRepository.GenerateMock<ICache>();

            return new ApplicationExceptionHandler(
                messageBoxAdapter,
                applicationAdapter,
                logger,
                exceptionDialogAdapter,
                activityEventAggregator,
                screenCaptureTool,
                breadCrumbLogger, cache);
        }

        #region Support Methods
        private User GetUserWithRoleDischargeTransferUser()
        {
            var patientAccessApplication = new PatientAccessApplication();
            var securityUser = new User();

            var dischargeAndTransferRole = patientAccessApplication.RoleWith(Role.DISCHARGE_TRANSFER_USER);
            var roleRelationship = new RoleRelationship(securityUser, dischargeAndTransferRole, Facility);
            securityUser.AddRelationship(roleRelationship);
            return securityUser;
        }

        #endregion

        #region Data Elements
        private Facility Facility = new Facility("DEL", "DELRAY TEST HOSPITAL");
        #endregion
    }
    
    /// <summary>
    /// This class was create because MSTest was not playing nice with Rhino mocks
    /// partial mocks and we did not have time to refactor the entire 
    /// <see cref="PatientAccessView"/> class to make it testable
    /// </summary>
    public class MockPatientAccessView : PatientAccessView
    {
        protected override void GoHome()
        {
            this.GoHomeWasCalled = true;
        }

        public bool GoHomeWasCalled { get; private set; }

        protected override void HandleActivityCancelled()
        {
            throw new Exception();
        }
    }
}
