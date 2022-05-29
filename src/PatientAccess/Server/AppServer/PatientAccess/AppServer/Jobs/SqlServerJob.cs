using System;
using System.Configuration;
using System.Data.Common;
using PatientAccess.Annotations;
using Quartz;
using log4net;

namespace PatientAccess.Jobs
{

    /// <summary>
    /// Purge out offline locks which may have been orphaned
    /// </summary>
    [UsedImplicitly]
    public class SqlServerJob : IJob
    {
		#region Constants 

        private const string KEY_COMMAND_TEXT = "CommandText";
        private const string KEY_CONNECTION_NAME = "ConnectionName";
        private const string PROVIDER_SQL_SERVER = "System.Data.SqlClient";
        private const string TEXT_JOB_COMPLETED = "Job Completed: {0}";
        private const string TEXT_JOB_FAILED = "Job Failed: {0} ({1})";

		#endregion Constants 

		#region Fields 

        /// <summary>
        /// Private Logger for job class
        /// </summary>
        private static readonly ILog _logger = 
            LogManager.GetLogger( typeof( SqlServerJob ) );

		#endregion Fields 

		#region Properties 

        /// <summary>
        /// Gets or sets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionName { get; set; }
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        private string CommandText { get; set; }


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

            DbProviderFactory dbProviderFactory = 
                DbProviderFactories.GetFactory(PROVIDER_SQL_SERVER);
            this.ConnectionName = 
                context.JobDetail.JobDataMap[KEY_CONNECTION_NAME] as string;
            this.CommandText = 
                context.JobDetail.JobDataMap[KEY_COMMAND_TEXT] as string;
            DbConnection dbConnection = 
                dbProviderFactory.CreateConnection();
            dbConnection.ConnectionString =
                ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;

            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = CommandText;

            try
            {
                dbConnection.Open();
                dbCommand.ExecuteNonQuery();
                Logger.InfoFormat( TEXT_JOB_COMPLETED, context.JobDetail.Name );
            }
            catch( Exception anyException )
            {
                Logger.ErrorFormat( TEXT_JOB_FAILED, 
                                    context.JobDetail.Name, 
                                    anyException.Message );
            }
            finally
            {
                dbCommand.Dispose();
                dbConnection.Close();
                dbConnection.Dispose();
            }

        }

		#endregion Methods 
    }

}
