using System;
using System.Drawing;
using PatientAccess.Domain.Parties;

namespace PatientAccess.UI.ExceptionManagement
{
    public interface IExceptionDialogAdapter
    {
        void ShowExceptionDialog(ApplicationExceptionHandler exceptionHandler, string briefErrMsg, string detailErrMsg, string newText);

        /// <summary>
        /// Display the exception dialog.
        /// </summary>
        /// <param name="exceptionHandler"></param>
        /// <param name="briefErrMsg">The brief err MSG.</param>
        /// <param name="detailErrMsg">The detail err MSG.</param>
        /// <param name="newText"></param>
        /// <param name="showExitNow">if set to <c>true</c> [show exit now].</param>
        void ShowExceptionDialog(ApplicationExceptionHandler exceptionHandler, string briefErrMsg, string detailErrMsg, string newText, bool showExitNow);

        /// <summary>
        /// Display the exception status dialog.
        /// </summary>
        void ShowExceptionStatusDialogAndProcessCrashReport( Exception exception, PhoneNumber phoneNumber, string comments );

        /// <summary>
        /// Display the exception status dialog.
        /// </summary>
        void ProcessCrashReport( Exception exception, Bitmap screenCapture, PhoneNumber phoneNumber, string comments );
    }
}