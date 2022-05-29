namespace PatientAccess.AppStart.HelperClasses
{
    /// <summary>
    /// Error message strings used by the AppStart GUI.
    /// </summary>
    public class ErrorMessages
    {
        public static string APPSTART_DUPLICATE_PROCESS_MSG = "Patient Access is checking for updates.  Please wait for the updates to complete before starting another instance of Patient Access.";
        public static string APPSTART_ERROR_MSG = "An error occurred when Patient Access was trying to start.  Please contact the Tenet Help Desk for assistance.";
        public static string APPSTART_DUPLICATE_PROCESSES_DURING_UPDATE_MSG = "Updates have been detected, however, another instance of Patient Access is currently running.\n\nPlease exit all instances of Patient Access and relaunch the application to apply the updates.";
    }
}
