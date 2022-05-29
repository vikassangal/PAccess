using System;
using System.Configuration;
using IBM.Data.DB2.iSeries;
using Quartz;
using log4net;

namespace PatientAccess.Jobs
{

    /// <summary>
    /// Run SQL against a DB2 instance. Template pattern used here to keep the
    /// SQL in the source code since the security is looser in the current
    /// DB2 environment. This avoids allowing the arbitrary execution of 
    /// SQL through the job engine.
    /// </summary>
    public abstract class DB2Job : IJob
    {
		#region Constants 

        private const string KEY_CONNECTION_NAME = "ConnectionName";
        private const string KEY_HUB_ADDRESS = "Address";
        private const string KEY_HUB_NAME = "Hub";
        private const string TEXT_JOB_COMPLETED = "Job Completed: {0}";
        private const string TEXT_JOB_FAILED = "Job Failed: {0} ({1})";

		#endregion Constants 

		#region Fields 

        /// <summary>
        /// Private Logger for job class
        /// </summary>
        private static readonly ILog _logger =
            LogManager.GetLogger(typeof(DB2Job));

		#endregion Fields 

		#region Properties 

        protected abstract string CommandText { get; }
        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> when a <see cref="T:Quartz.Trigger"/>
        /// fires that is associated with the <see cref="T:Quartz.IJob"/>.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <remarks>
        /// The implementation may wish to set a  result object on the
        /// JobExecutionContext before this method exits.  The result itself
        /// is meaningless to Quartz, but may be informative to
        /// <see cref="T:Quartz.IJobListener"/>s or
        /// <see cref="T:Quartz.ITriggerListener"/>s that are watching the job's
        /// execution.
        /// </remarks>
        public void Execute(JobExecutionContext context)
        {
            string connectionName = (string)
                context.JobDetail.JobDataMap[KEY_CONNECTION_NAME];
            string connectionString = 
                ConfigurationManager.ConnectionStrings[connectionName]
                                    .ConnectionString;
            string hub = (string)
                context.Trigger.JobDataMap[KEY_HUB_NAME];
            string address = (string)
                context.Trigger.JobDataMap[KEY_HUB_ADDRESS];

            iDB2Connection dbConnection = new iDB2Connection();

            iDB2Command dbCommand = dbConnection.CreateCommand();

            dbConnection.ConnectionString =
                string.Format( connectionString, address, hub );
                                               
            dbCommand.CommandText = this.CommandText;

            this.OnBeforeExcecute( dbCommand, context );

            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                Logger.InfoFormat(TEXT_JOB_COMPLETED, context.JobDetail.Name);
            }
            catch (Exception anyException)
            {
                Logger.ErrorFormat(TEXT_JOB_FAILED,
                                    context.JobDetail.Name,
                                    anyException.Message);
            }
            finally
            {
                dbCommand.Dispose();
                dbConnection.Close();
                dbConnection.Dispose();
            }

        }

        /// <summary>
        /// Called when [before excecute].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="context"></param>
        protected virtual void OnBeforeExcecute(iDB2Command command, JobExecutionContext context)
        {
            return;
        }

		#endregion Methods 
    }

}
