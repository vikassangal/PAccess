using System;
using System.Configuration;
using PatientAccess.Annotations;
using PatientAccess.Persistence.OnlinePreregistration;
using Quartz;
using log4net;

namespace PatientAccess.Jobs
{
    [UsedImplicitly]
    public class PurgeOldOnlinePreregistrationSubmissions : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger( typeof( PurgeOldOnlinePreregistrationSubmissions ) );
        private const string TEXT_JOB_FAILED = "Job Failed: {0} ({1})";
        private const string TEXT_JOB_COMPLETED = "Job Completed: {0}";

        public void Execute( JobExecutionContext context )
        {
            var repository = new PreRegistrationSubmissionRepository();

            try
            {
                int numberOfDaysAfterAdmitDateToKeepOnlineReregistrationSubmissions = int.Parse( ConfigurationManager.AppSettings[ServerConfigurationConstants.NumberOfDaysAfterAdmitDateToKeepOnlineReregistrationSubmissionsKey] );
                repository.DeleteSubmissionsWithAdmitTimeOlderThan( numberOfDaysAfterAdmitDateToKeepOnlineReregistrationSubmissions );
                Logger.InfoFormat( TEXT_JOB_COMPLETED, context.JobDetail.Name );
            }
            catch ( Exception e )
            {
                Logger.ErrorFormat( TEXT_JOB_FAILED, context.JobDetail.Name, e.Message );
            }

        }
    }
}
