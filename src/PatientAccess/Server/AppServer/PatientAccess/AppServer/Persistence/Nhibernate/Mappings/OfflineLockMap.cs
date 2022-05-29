using FluentNHibernate.Mapping;
using PatientAccess.Domain.Locking;

namespace PatientAccess.AppServer.Persistence.Locking
{
    public class OfflineLockMap : ClassMap<OfflineLock>
    {
        public OfflineLockMap()
        {
            Table( "OfflineLocks" );
            Schema( "Locking" );
            Id( x => x.Handle )
                .Column( "Handle" )
                .GeneratedBy.Assigned();

            Map( x => x.Owner );
            Map( x => x.TimePrint );
            Map( x => x.ResourceType );
        }
    }
}
