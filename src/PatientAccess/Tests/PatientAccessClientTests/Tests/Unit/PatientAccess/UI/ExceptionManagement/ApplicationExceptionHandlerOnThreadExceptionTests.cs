using System;
using System.Drawing;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using Extensions.Exceptions;
using NUnit.Framework;
using PatientAccess;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.UI;
using PatientAccess.UI.CrashReporting;
using PatientAccess.UI.ExceptionManagement;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using Rhino.Mocks;
using log4net;

namespace Tests.Unit.PatientAccess.UI.ExceptionManagement
{
    /// <summary>
    /// These tests are organized as test fixture per method. The
    /// ApplicationExceptionHandler.OnThreadException method is complex enough to
    /// require a test fixture of its own.
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ApplicationExceptionHandlerOnThreadException_WhenConfigSaysDoNotGoToAnnouncementsAfterACrashTests
    {
        [Test]
        public void OnThreadExceptionForAllScenarios()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");
            exceptionHandler.OnThreadException(new object(), new ThreadExceptionEventArgs(new EnterpriseException()));
            exceptionHandler.Log.AssertWasCalled(x => x.Error(Arg<string>.Is.Anything, Arg<Exception>.Is.Anything));
            exceptionHandler.BreadCrumbLogger.AssertWasCalled(x => x.Log(Arg<string>.Is.Anything));
        }


        [Test]
        public void WhenLoadAccountTimeoutException_ThenShowMessageBoxAndRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            exceptionHandler.OnThreadException(new object(), new ThreadExceptionEventArgs(new LoadAccountTimeoutException()));

            exceptionHandler.MessageBoxAdapter.AssertWasCalled(x => x.ShowMessageBox(UIErrorMessages.LOAD_ACCOUNT_TIMEOUT_MESSAGE));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
        }

        [Test]
        public void WhenRemotingTimeoutException_ThenShowMessageBoxAndDoNotRaiseActivityCancelled()
        {

            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            exceptionHandler.OnThreadException(new object(), new ThreadExceptionEventArgs(new RemotingTimeoutException()));

            exceptionHandler.MessageBoxAdapter.AssertWasCalled(x => x.ShowMessageBox(UIErrorMessages.TIMEOUT_GENERAL));
            exceptionHandler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything));
        }

        [Test]
        public void WhenSeverityIsCatastrophicAndVersionNotMatchedException_ThenShowMessageBoxAndDoNotRaiseActivityCancelled()
        {
            var handler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            handler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new VersionNotMatchedException(Severity.Catastrophic));
            handler.OnThreadException(new object(), threadExceptionArgs);

            handler.MessageBoxAdapter.AssertWasCalled(
                x => x.ShowMessageBox(
                    UIErrorMessages.VERSION_NOT_MATCHED,
                    "Version is out-of-date",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop));

            handler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything));
        }


        [Test]
        public void WhenSeverityIsCatastrophicAndExceptionIsNotVersionNotMatchedException_ThenShowExceptionDialogAndRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var timeoutException = new Extensions.DB2Persistence.TimeoutException(Guid.NewGuid().ToString(), Severity.Catastrophic);

            exceptionHandler.OnThreadException(new object(), new ThreadExceptionEventArgs(timeoutException));

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, UIErrorMessages.APPLICATION_EXCEPTION_CATASTROPHIC, timeoutException.ToString(), null));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
        }


        [Test]
        public void WhenSeverityIsHighAndExceptionIsEnterpriseException_ThenShowExceptionDialogAndRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new EnterpriseException(Guid.NewGuid().ToString(), Severity.High));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, UIErrorMessages.APPLICATION_EXCEPTION_HIGH, threadExceptionArgs.Exception.ToString(), null));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
        }

        [Test]
        public void WhenSeverityIsNotCatastrophicOrHighAndExceptionIsEnterpriseException_ThenShowExceptionDialogAndDoNotRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new EnterpriseException(Guid.NewGuid().ToString(), Severity.Low));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, UIErrorMessages.APPLICATION_EXCEPTION_LOW, threadExceptionArgs.Exception.ToString(), null));
            exceptionHandler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything));
        }

        [Test]
        public void WhenSerializationExceptionOrNetworkConnectivityError_ThenShowExceptionDialogAndDoNotRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new SerializationException(Guid.NewGuid().ToString()));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, UIErrorMessages.APPLICATION_EXCEPTION_UNDERLYING_CONNECTION_CLOSED_MSG, threadExceptionArgs.Exception.ToString(), null, true));
            exceptionHandler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything));
        }

        [Test]
        public void WhenNotEnterpriseExceptionOrSerializationExceptionOrNoNetworkConnectivityError_ThenShowExceptionDialogAndDoNotRaiseActivityCancelled()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new Exception(Guid.NewGuid().ToString()));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, UIErrorMessages.APPLICATION_EXCEPTION_UNEXPECTED_ERROR, threadExceptionArgs.Exception.ToString(), null));
            exceptionHandler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(Arg<object>.Is.Anything, Arg<EventArgs>.Is.Anything));
        }

        [Test]
        public void WhenConfigSaysDoNotGoToAnnouncementsAfterACrashAndExceptionIsEnterpriseExceptionWithSeverityCatastrophic_ThenShowExceptionStatusDialogAndExitTheApplication()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new EnterpriseException(Guid.NewGuid().ToString(), Severity.Catastrophic));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionStatusDialogAndProcessCrashReport(threadExceptionArgs.Exception, exceptionHandler.PhoneNumber, exceptionHandler.Comments));
            exceptionHandler.ApplicationAdapter.AssertWasCalled(x => x.Exit());

        }

        [Test]
        public void WhenConfigSaysDoNotGoToAnnouncementsAfterACrashAndExceptionIsEnterpriseExceptionWithSeverityNotCatastrophic_ThenShowExceptionStatusDialogAndDoNotExitTheApplication()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();

            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("false");

            var threadExceptionArgs = new ThreadExceptionEventArgs(new EnterpriseException(Guid.NewGuid().ToString(), Severity.Ignore));
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionStatusDialogAndProcessCrashReport(threadExceptionArgs.Exception, exceptionHandler.PhoneNumber, exceptionHandler.Comments));
            exceptionHandler.ApplicationAdapter.AssertWasNotCalled(x => x.Exit());
        }
    }

    [TestFixture]
    [Category( "Fast" )]
    public class ApplicationExceptionHandlerOnThreadException_WhenConfigSaysGoToAnnouncementsAfterACrashTests
    {
        [Test]
        public void WhenVersionNotMatchedExceptionAndSeverityIsCatastrophic_ThenShowMessageBoxProcessCrashReportAndDoNotExit()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();
            var exceptionToHandle = new VersionNotMatchedException(Severity.Catastrophic);
            var threadExceptionArgs = new ThreadExceptionEventArgs(exceptionToHandle);
            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("true");
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            var errorMessage = exceptionHandler.GetUIErrorMessageUsing(exceptionToHandle);

            exceptionHandler.MessageBoxAdapter.AssertWasCalled(x => x.ShowMessageBox(errorMessage, ApplicationExceptionHandler.VERSION_IS_OUT_OF_DATE, MessageBoxButtons.OK, MessageBoxIcon.Stop));
//            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ProcessCrashReport(Arg<Exception>.Is.Equal(exceptionToHandle), Arg<Bitmap>.Is.Anything, Arg<PhoneNumber>.Is.Equal(exceptionHandler.PhoneNumber), Arg<string>.Is.Equal(exceptionHandler.Comments)));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
            exceptionHandler.ApplicationAdapter.AssertWasNotCalled(x => x.Exit());
        }

        [Test]
        public void WhenServiceUnavailableExceptionAndSeverityIsCatastrophic_ThenShowMessageBoxProcessCrashReportAndDoNotExit()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();
            var exceptionToHandle = new ServiceUnavailableException(Guid.NewGuid().ToString())
                                        {Severity = Severity.Catastrophic};

            var threadExceptionArgs = new ThreadExceptionEventArgs(exceptionToHandle);
            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("true");
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            var errorMessage = exceptionHandler.GetUIErrorMessageUsing(exceptionToHandle);

            exceptionHandler.MessageBoxAdapter.AssertWasCalled(x => x.ShowMessageBox(errorMessage, ApplicationExceptionHandler.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1));
//            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ProcessCrashReport(Arg<Exception>.Is.Equal(exceptionToHandle), Arg<Bitmap>.Is.Anything, Arg<PhoneNumber>.Is.Equal(exceptionHandler.PhoneNumber), Arg<string>.Is.Equal(exceptionHandler.Comments)));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
            exceptionHandler.ApplicationAdapter.AssertWasNotCalled(x => x.Exit());
        }

        [Test]
        public void WhenEnterpriseExceptionAndSeverityIsCatastrophic_ThenShowExceptionDialogProcessCrashReportAndExit()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();
            var exceptionToHandle = new EnterpriseException(Guid.NewGuid().ToString(),Severity.Catastrophic);
            var threadExceptionArgs = new ThreadExceptionEventArgs(exceptionToHandle);
            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("true");
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            var errorMessage = exceptionHandler.GetUIErrorMessageUsing(exceptionToHandle);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, errorMessage, exceptionToHandle.ToString(), "OK"));
            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ProcessCrashReport(Arg<Exception>.Is.Equal(exceptionToHandle), Arg<Bitmap>.Is.Anything, Arg<PhoneNumber>.Is.Equal(exceptionHandler.PhoneNumber), Arg<string>.Is.Equal(exceptionHandler.Comments)));
            exceptionHandler.EventAggregator.AssertWasNotCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
            exceptionHandler.ApplicationAdapter.AssertWasCalled(x => x.Exit());
        }


        [Test]
        public void WhenNotEnterpriseException_ThenShowExceptionDialogProcessCrashReportAndDoNotExit()
        {
            var exceptionHandler = ApplicationExceptionHandlerTests.GetApplicationExceptionHandlerWithMockDependencies();
            var exceptionToHandle = new TimeoutException(Guid.NewGuid().ToString());
            var threadExceptionArgs = new ThreadExceptionEventArgs(exceptionToHandle);
            exceptionHandler.Cache.Stub(x => x.Get(ApplicationExceptionHandler.ANNOUNCEMENTS_AFTER_CRASH)).Return("true");
            exceptionHandler.OnThreadException(new object(), threadExceptionArgs);

            var errorMessage = exceptionHandler.GetUIErrorMessageUsing(exceptionToHandle);

            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ShowExceptionDialog(exceptionHandler, errorMessage, exceptionToHandle.ToString(), "OK"));
//            exceptionHandler.ExceptionDialogAdapter.AssertWasCalled(x => x.ProcessCrashReport(Arg<Exception>.Is.Equal(exceptionToHandle), Arg<Bitmap>.Is.Anything, Arg<PhoneNumber>.Is.Equal(exceptionHandler.PhoneNumber), Arg<string>.Is.Equal(exceptionHandler.Comments)));
            exceptionHandler.EventAggregator.AssertWasCalled(x => x.RaiseActivityCancelEvent(exceptionHandler, EventArgs.Empty));
            exceptionHandler.ApplicationAdapter.AssertWasNotCalled(x => x.Exit());
        }


    }

    public class ApplicationExceptionHandlerTests
    {
        public static ApplicationExceptionHandler GetApplicationExceptionHandlerWithMockDependencies()
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
    }
}
