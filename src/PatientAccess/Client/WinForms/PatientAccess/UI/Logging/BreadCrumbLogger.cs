using System;
using System.Diagnostics;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using log4net;

namespace PatientAccess.UI.Logging
{
    //TODO: Create XML summary comment for BreadCrumbLogger

    /// <summary>
    /// User activity logger
    /// </summary>
    [Serializable]
    public sealed class BreadCrumbLogger : IBreadCrumbLogger
    {
        #region Event Handlers
        #endregion

        #region Methods

        public void Log(string message)
        {
            this.logIt(message);
        }
        public void Log(string message, Account account)
        {
            if (account == null ||
              (account.Activity == null || account.Patient == null))
            {
                this.logIt(message);
            }
            else
            {
                this.logIt(string.Format("{0},{1},{2},{3}",
                     account.Activity.ContextDescription,
                     account.AccountNumber,
                     account.Patient.MedicalRecordNumber,
                     message));
            }
        }
        public void Log(string message, AccountProxy ap)
        {

            if (ap == null ||
              (ap.Activity == null || ap.Patient == null))
            {
                this.logIt(message);
            }
            else
            {
                this.logIt(string.Format("{0},{1},{2},{3}",
                    ap.Activity.ContextDescription,
                    ap.AccountNumber,
                    ap.Patient.MedicalRecordNumber,
                    message));
            }
        }
        #endregion

        #region Properties
        public static BreadCrumbLogger GetInstance
        {
            get
            {
                return i_instance;
            }
        }

        #endregion

        #region Private Methods

        private void logIt(string message)
        {
            Process p = Process.GetCurrentProcess();
            // push the PID of the current app on the stack. 
            // this will be read from "%property{NDC}" in the configfile
            using (ThreadContext.Stacks["NDC"].Push(p.Id.ToString()))
            {
                c_log.Info(message);
            } 
        }

        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        BreadCrumbLogger()
        {
        }

        static BreadCrumbLogger()
        {
        }
        #endregion

        #region Data Elements

        static readonly BreadCrumbLogger i_instance = new BreadCrumbLogger();

        private static readonly ILog c_log =
        LogManager.GetLogger(typeof(BreadCrumbLogger));

        #endregion

        #region Constants
        #endregion
    }
}
