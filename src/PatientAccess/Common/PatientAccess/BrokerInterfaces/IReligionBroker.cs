using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IReligionBroker.
    /// </summary>
    public interface IReligionBroker
    {

        IList AllPlacesOfWorshipFor( long facilityNumber );
        PlaceOfWorship PlaceOfWorshipWith( long facilityNumber, long oid );
        PlaceOfWorship PlaceOfWorshipWith( long facilityNumber, string code );

        ICollection AllReligions(long facilityID);
        Religion ReligionWith ( long facilityID, long oid );
        Religion ReligionWith(long facilityID, string code);

        ICollection ReligionSummaryFor(  Facility facility, string religionCode );
        //void SetCurrentFacility( Facility aFacility );
       // ICollection ReligionSummaryFor( string religionCode, Facility facility);
    }
}
