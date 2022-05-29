using System;
using IBM.Data.DB2.iSeries;
using PatientAccess.Annotations;
using Quartz;

namespace PatientAccess.Jobs
{
    /// <summary>
    /// Purge out old entries for new employer addresses
    /// </summary>
    [UsedImplicitly]
    class PurgeNewEmployersJob : DB2Job
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>The command text.</value>
        protected override string CommandText
        {
            get
            {
                return "DELETE FROM EMPLOYERSFORAPPROVAL WHERE DAYS(DATE( CURRENT TIMESTAMP )) - DAYS(DATE( DATETIMEENTERED )) >= @MaxDays;";
            }
        }

        /// <summary>
        /// Called when [before excecute].
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="context"></param>
        protected override void OnBeforeExcecute(iDB2Command command, JobExecutionContext context)
        {
            iDB2Parameter maxDaysParameter = 
                command.CreateParameter();
            maxDaysParameter.ParameterName = 
                "@MaxDays";
            maxDaysParameter.Value = 
                Int32.Parse( context.JobDetail.JobDataMap["MaxDays"].ToString() );
            command.Parameters.Add( maxDaysParameter );
        }
    }
}
