using System;
using System.Drawing;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Extensions.Exceptions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.Parties.Exceptions;
using PatientAccess.Http;
using PatientAccess.UI.CrashReporting;
using PatientAccess.UI.HelperClasses;
using PatientAccess.UI.Logging;
using log4net;

namespace PatientAccess.UI.ExceptionManagement
{
    public class ApplicationExceptionHandler
    {
        #region Methods

        // Handles the exception event.
        /// <summary>
        /// Called when a [thread exception] occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="threadExceptionArgs">The <see cref="System.Threading.ThreadExceptionEventArgs"/> instance containing the event data.</param>
        public void OnThreadException(object sender, ThreadExceptionEventArgs threadExceptionArgs)
        {
            var exceptionToHandle = threadExceptionArgs.Exception;

            this.Log.Error("Global Application Exception: " + exceptionToHandle, exceptionToHandle);
            this.BreadCrumbLogger.Log(String.Format("Global Application Exception: {0}", exceptionToHandle));

            if (exceptionToHandle is LoadAccountTimeoutException)
            {
                this.MessageBoxAdapter.ShowMessageBox(UIErrorMessages.LOAD_ACCOUNT_TIMEOUT_MESSAGE);
                this.EventAggregator.RaiseActivityCancelEvent(this, EventArgs.Empty);
                return;
            }

            if (exceptionToHandle is RemotingTimeoutException ||
                exceptionToHandle.ToString().Contains("The operation has timed out"))
            {
                this.MessageBoxAdapter.ShowMessageBox(UIErrorMessages.TIMEOUT_GENERAL);

                return;
            }

            /* THIS IS A HORRIBLE HACK
             * -----------------------
             * Microsoft recognizes that there is a problem with the TabControl and its associated window handle
             * creation and destruction becoming out of sink to the point where two destructions happen in a row.
             * 
             * We could have subclassed the TabControl and overidden the OnHandleDestroyed method to do a null check
             * and silently absorb the situation when it arises. Unfortunately, that would have made a huge impact
             * for the automated testing strategy because the object repository for the Mercury tool would need to be
             * updated and/or testing code updated across the board where ever a TabControl is accessed.
             * 
             * Microsoft also suggested some fancy footwork with the event handlers that could accomplish the same
             * thing as above but without the automated testing impacthreadExceptionArgs. That is untenable largely because of the
             * complexity involved in getting all of the event handler tracking righthreadExceptionArgs. It would have ultimately 
             * created more problems that it potentially solved.
             * 
             * So what is left is this solution: swallow the exception by running a regex against the stacktrace
             * to test for the RemoveWindowFromIDTable. If there, just return. The above logging statements will
             * make sure that we still notice ithreadExceptionArgs.
             */

            if (exceptionToHandle.StackTrace != null && this._removeWindowFromIDTable.IsMatch(exceptionToHandle.StackTrace))
            {
                this.BreadCrumbLogger.Log("IGNORED THE ABOVE EXCEPTION");
                return;
            }

            var shouldGoToAnnouncementsForNonCatastrophicError = bool.Parse( this.Cache.Get(ANNOUNCEMENTS_AFTER_CRASH).ToString());

            if (shouldGoToAnnouncementsForNonCatastrophicError)
            {
                string errorMessage = this.GetUIErrorMessageUsing(exceptionToHandle);

                this.ProcessCrashReport(exceptionToHandle, errorMessage);

                if (this.IsCatastrophicError(errorMessage))
                {
                    this.ApplicationAdapter.Exit();
                }

                else
                {
                    this.EventAggregator.RaiseActivityCancelEvent(this, EventArgs.Empty);
                }
            }

            else
            {
                this.ProcessCrashReportAndExitApplication(exceptionToHandle);
            }
        }

        private bool IsCatastrophicError(string errorMessage)
        {
            return errorMessage == UIErrorMessages.APPLICATION_EXCEPTION_CATASTROPHIC;
        }

        private void ProcessCrashReportAndExitApplication(Exception exceptionToHandle)
        {
            if (exceptionToHandle is EnterpriseException)
            {
                this.HandleEnterpriseExceptionAndExitIfCatastrophicError(exceptionToHandle);
            }

            else if (exceptionToHandle.Message ==
                     "The underlying connection was closed: Unable to connect to the remote server." ||
                     exceptionToHandle.Message ==
                     "The underlying connection was closed: An unexpected error occurred on a receive." ||
                     exceptionToHandle.Message == "The remote server returned an error: (404) Not Found." ||
                     exceptionToHandle.GetType() == typeof(SerializationException))
            {
                this.ExceptionDialogAdapter.ShowExceptionDialog(
                            this,
                            UIErrorMessages.APPLICATION_EXCEPTION_UNDERLYING_CONNECTION_CLOSED_MSG,
                            exceptionToHandle.ToString(),
                            null,
                            true);

                this.ExceptionDialogAdapter.ShowExceptionStatusDialogAndProcessCrashReport(
                            exceptionToHandle,
                            this.PhoneNumber,
                            this.Comments);
            }

            else
            {
                this.ExceptionDialogAdapter.ShowExceptionDialog(
                            this,
                            UIErrorMessages.APPLICATION_EXCEPTION_UNEXPECTED_ERROR,
                            exceptionToHandle.ToString(),
                            null);

                this.ExceptionDialogAdapter.ShowExceptionStatusDialogAndProcessCrashReport(
                            exceptionToHandle,
                            this.PhoneNumber,
                            this.Comments);
            }
        }

        private void ProcessCrashReport(Exception exceptionToHandle, string errorMessage)
        {
            // Grab a screenshot of the user's desktop.
            Bitmap screenCapture = this.ScreenCaptureTool.GetScreenCapture();

            this.ShowErrorDialog(exceptionToHandle, errorMessage);

            if (errorMessage == UIErrorMessages.APPLICATION_EXCEPTION_CATASTROPHIC)
            {
                this.ExceptionDialogAdapter.ProcessCrashReport(exceptionToHandle, screenCapture, this.PhoneNumber, this.Comments);
            }
            else
            {
                processCrashReport del = this.ExceptionDialogAdapter.ProcessCrashReport;
                del.BeginInvoke(exceptionToHandle, screenCapture, this.PhoneNumber, this.Comments, null, null);
            }
        }

        private void HandleEnterpriseExceptionAndExitIfCatastrophicError(Exception exceptionToHandle)
        {
            EnterpriseException ex = (EnterpriseException)exceptionToHandle;

            if (ex.Severity == Severity.Catastrophic)
            {
                if (exceptionToHandle is VersionNotMatchedException)
                {

                    this.MessageBoxAdapter.ShowMessageBox(UIErrorMessages.VERSION_NOT_MATCHED, "Version is out-of-date", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    this.ExceptionDialogAdapter.ShowExceptionDialog(
                        this,
                        UIErrorMessages.APPLICATION_EXCEPTION_CATASTROPHIC,
                        exceptionToHandle.ToString(),
                        null);

                    this.EventAggregator.RaiseActivityCancelEvent(this, EventArgs.Empty);
                }
            }
            else if (ex.Severity == Severity.High)
            {
                this.ExceptionDialogAdapter.ShowExceptionDialog(this, UIErrorMessages.APPLICATION_EXCEPTION_HIGH, exceptionToHandle.ToString(), null);

                this.EventAggregator.RaiseActivityCancelEvent(this, EventArgs.Empty);
            }
            else
            {
                this.ExceptionDialogAdapter.ShowExceptionDialog(this, UIErrorMessages.APPLICATION_EXCEPTION_LOW, exceptionToHandle.ToString(), null);
            }

            this.ExceptionDialogAdapter.ShowExceptionStatusDialogAndProcessCrashReport(exceptionToHandle, this.PhoneNumber, this.Comments);

            if (ex.Severity == Severity.Catastrophic)
            {
                this.ApplicationAdapter.Exit();
            }
        }

        private void ShowErrorDialog(Exception exception, string errorMessage)
        {
            if (exception is VersionNotMatchedException)
            {
                this.MessageBoxAdapter.ShowMessageBox(errorMessage, VERSION_IS_OUT_OF_DATE, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (exception is ServiceUnavailableException)
            {
                this.MessageBoxAdapter.ShowMessageBox(errorMessage, ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else
            {
                this.ExceptionDialogAdapter.ShowExceptionDialog(this, errorMessage, exception.ToString(), "OK");
            }
        }

        internal string GetUIErrorMessageUsing(Exception exception)
        {
            string strException;
            if (exception is EnterpriseException)
            {
                EnterpriseException ex = (EnterpriseException)exception;

                if (ex.Severity == Severity.Catastrophic)
                {
                    strException = UIErrorMessages.APPLICATION_EXCEPTION_CATASTROPHIC;
                }
                else
                {
                    strException = UIErrorMessages.APPLICATION_EXCEPTION_HIGH;
                }

                if (exception is VersionNotMatchedException)
                {
                    strException = UIErrorMessages.VERSION_NOT_MATCHED;
                }
                else if (exception is ServiceUnavailableException)
                {
                    strException = UIErrorMessages.PBAR_SERVICE_IS_UNAVAILABLE;
                }
            }

            else
            {
                strException = UIErrorMessages.APPLICATION_EXCEPTION_HIGH;
            }
            return strException;
        }

        #endregion

        #region Properties

        private delegate void processCrashReport(Exception exception, Bitmap bitmap, PhoneNumber phoneNumber, string comments);

        #endregion

        #region Private Methods

        #endregion

        #region Properties

        public PhoneNumber PhoneNumber
        {
            get
            {
                return this._phoneNumber;
            }
            set
            {
                this._phoneNumber = value;
            }
        }

        public string Comments
        {
            get
            {
                return this._comments ?? string.Empty;
            }
            set
            {
                this._comments = value ?? string.Empty;
            }
        }

        internal IMessageBoxAdapter MessageBoxAdapter { get; private set; }

        internal IWindowsFormsApplicationAdapter ApplicationAdapter { get; private set; }

        internal ILog Log { get; private set; }

        private IScreenCaptureTool ScreenCaptureTool { get; set; }

        internal IActivityEventAggregator EventAggregator { get; private set; }

        internal IExceptionDialogAdapter ExceptionDialogAdapter { get; private set; }

        internal IBreadCrumbLogger BreadCrumbLogger { get; private set; }

        internal ICache Cache { get; private set; }

        #endregion

        #region Construction and Finalization

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationExceptionHandler"/> class.
        /// </summary>
        public ApplicationExceptionHandler()
            : this(
                new MessageBoxAdapter(),
                new WindowsFormsApplicationAdapter(),
                LogManager.GetLogger(typeof(ApplicationExceptionHandler)),
                new ExceptionDialogAdapter(),
                ActivityEventAggregator.GetInstance(),
                new ScreenCaptureTool(),
                Logging.BreadCrumbLogger.GetInstance,
                new HttpRuntimeCache())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationExceptionHandler"/>
        /// class. This constructor was created as part of the effort to make this class
        /// more testable and is meant to be called from tests only
        /// </summary>
        /// <param name="messageBoxAdapter">The message box adapter.</param>
        /// <param name="applicationAdapter">The application adapter.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="exceptionDialogAdapter">The exception dialog adapter.</param>
        /// <param name="activityEventAggregator">The activity event aggregator.</param>
        /// <param name="screenCaptureTool">The screen capture tool.</param>
        /// <param name="breadCrumbLogger">The bread crumb logger.</param>
        /// <param name="cache">The HTTP runtime cache source.</param>
        internal ApplicationExceptionHandler(IMessageBoxAdapter messageBoxAdapter,
                                             IWindowsFormsApplicationAdapter applicationAdapter,
                                             ILog logger, IExceptionDialogAdapter exceptionDialogAdapter,
                                             IActivityEventAggregator activityEventAggregator,
                                             IScreenCaptureTool screenCaptureTool,
                                             IBreadCrumbLogger breadCrumbLogger,
                                             ICache cache)
        {
            this.MessageBoxAdapter = messageBoxAdapter;
            this.ApplicationAdapter = applicationAdapter;
            this.Log = logger;
            this.ScreenCaptureTool = screenCaptureTool;
            this.ExceptionDialogAdapter = exceptionDialogAdapter;
            this.EventAggregator = activityEventAggregator;
            this.BreadCrumbLogger = breadCrumbLogger;
            this.Cache = cache;

            _phoneNumber = new PhoneNumber();
            _comments = string.Empty;
            _removeWindowFromIDTable = new Regex(@".*RemoveWindowFromIDTable.*", RegexOptions.Singleline);
        }

        #endregion

        #region Data Elements

        private PhoneNumber _phoneNumber;
        private string _comments;
        private readonly Regex _removeWindowFromIDTable;

        #endregion

        #region Constants

        internal const string ANNOUNCEMENTS_AFTER_CRASH = "CCAnnouncementsAfterCrash";
        internal const string ERROR = "Error";
        internal const string VERSION_IS_OUT_OF_DATE = "Version is out-of-date";

        #endregion
    }
}
