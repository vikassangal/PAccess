using FluentNHibernate.Mapping;
using PatientAccess.Persistence.Nhibernate;
using PatientAccess.Persistence.OnlinePreregistration;

namespace PatientAccess.AppServer.Persistence.Nhibernate.Mappings
{
    public class PreRegistrationSubmissionMap : ClassMap<PreRegistrationSubmission>
    {
        public PreRegistrationSubmissionMap()
        {
            Table( "PreRegistrationSubmissions" );
            Schema( "Messaging" );
            Id( x => x.Id ).GeneratedBy.GuidComb();
            Map( x => x.FirstName );
            Map( x => x.LastName );
            Map(x => x.MiddleInitial);
            Map( x => x.ReturningPatient );
            Map(x => x.Gender);
            Map(x => x.DateOfBirth);
            Map( x => x.SSN ).CustomType(typeof(EncryptedString));
            Map(x => x.Address);
            Map( x => x.AdmitDate );
            Map( x => x.DateTimeReceived );
            Map( x => x.Message ).CustomType( typeof( EncryptedXmlType ) );
            Map( x => x.FacilityId ).Column( "FacilityId" );
        }
    }
}