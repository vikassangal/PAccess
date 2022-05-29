using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using PatientAccess.Persistence.OnlinePreregistration;

namespace PatientAccess.Persistence.Nhibernate
{
    public static class NHibernateInitializer
    {
        public static readonly ISessionFactory SessionFactory;
        static NHibernateInitializer()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ServerConfigurationConstants.SqlServerConnectionStringName].ConnectionString;
            SessionFactory = Fluently.Configure()
                .Database( MsSqlConfiguration.MsSql2005 .ConnectionString( connectionString ) )
                .Mappings( m => m.FluentMappings.AddFromAssemblyOf<PreRegistrationSubmission>() )
                .BuildSessionFactory(); 
        }
 
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
