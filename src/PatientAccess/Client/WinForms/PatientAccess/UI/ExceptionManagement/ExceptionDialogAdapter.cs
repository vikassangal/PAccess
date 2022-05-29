using System;
using System.Drawing;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.ApplicationErrorViews;

namespace PatientAccess.UI.ExceptionManagement
{
    public class ExceptionDialogAdapter : IExceptionDialogAdapter
    {
        public void ShowExceptionDialog(ApplicationExceptionHandler exceptionHandler, string briefErrMsg, string detailErrMsg, string newText)
        {
            this.ShowExceptionDialog(exceptionHandler, briefErrMsg, detailErrMsg, newText, false);
        }

        /// <summary>
        /// Display the exception dialog.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        /// <param name="briefErrMsg">The brief err MSG.</param>
        /// <param name="detailErrMsg">The detail err MSG.</param>
        /// <param name="newText"></param>
        /// <param name="showExitNow">if set to <c>true</c> [show exit now].</param>
        public void ShowExceptionDialog(ApplicationExceptionHandler exceptionHandler, string briefErrMsg, string detailErrMsg, string newText, bool showExitNow)
        {
            ApplicationErrorView applicationErrorView = null;

            try
            {
                applicationErrorView = new ApplicationErrorView();
                if (newText != null)
                    applicationErrorView.SetButtonText(newText);
                applicationErrorView.ErrorText = briefErrMsg;
                applicationErrorView.ErrorDetails = detailErrMsg;
                applicationErrorView.UpdateView();
                applicationErrorView.ShowDialog();

                exceptionHandler.Comments = applicationErrorView.Comments.Trim();
                exceptionHandler.PhoneNumber = applicationErrorView.PhoneNumber;
            }
            finally
            {
                applicationErrorView.Dispose();
            }
        }

        /// <summary>
        /// Display the exception status dialog.
        /// </summary>
        public void ShowExceptionStatusDialogAndProcessCrashReport( Exception exception, PhoneNumber phoneNumber, string comments )
        {
            ApplicationErrorStatusView applicationErrorStatusView = null;

            try
            {
                applicationErrorStatusView = new ApplicationErrorStatusView
                                                 {
                                                     Exception = exception, 
                                                     PhoneNumber = phoneNumber, 
                                                     Comments = comments
                                                 };

                applicationErrorStatusView.ShowDialog();
            }
            finally
            {
                applicationErrorStatusView.Dispose();
            }
        }

        public void ProcessCrashReport( Exception exception, Bitmap screenCapture, PhoneNumber phoneNumber, string comments )
        {
            ApplicationErrorStatusView applicationErrorStatusView = null;

            try
            {
                applicationErrorStatusView = new ApplicationErrorStatusView
                                                 {
                                                     Exception = exception,
                                                     ScreenCapture = screenCapture,
                                                     PhoneNumber = phoneNumber,
                                                     Comments = comments
                                                 };

                applicationErrorStatusView.ProcessCrashReport();
            }
            finally
            {
                applicationErrorStatusView.Dispose();
            }
        }
    }
}