using PatientAccess.Annotations;
using log4net.Appender;

namespace PatientAccess.Logging
{
    /// <summary>
    /// The current released version of log4net does not support using encrypted connection strings from the application configuration file.
    /// This class was created as a workaround that issue. The trunk version of log4net has this feature but it is not compatible with 
    /// other 3rd party libraries (Quartz, NHibernate) being used by the application.
    /// Reference: http://stackoverflow.com/questions/827945/encrypt-the-connectionstring-used-by-the-log4ent-adonetappender 
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ConfigAdoNetAppender : AdoNetAppender
    {
        public string ConnectionStringName
        {
            set
            {
                this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[value].ToString();
            }
        }
    }
}
