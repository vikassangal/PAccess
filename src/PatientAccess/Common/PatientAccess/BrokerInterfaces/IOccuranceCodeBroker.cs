using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IOccurrenceCodeBroker.
    /// </summary>
    public interface IOccuranceCodeBroker
    {
        ICollection AllOccurrenceCodes( long facilityID );
        ICollection AllSelectableOccurrenceCodes( long facilityID );
        OccurrenceCode OccurrenceCodeWith( long facilityID, long occurrenceCodeID );
        OccurrenceCode OccurrenceCodeWith( long facilityID, string occurrenceCode );
        TypeOfAccident AccidentTypeFor(long facilityID, OccurrenceCode occurrenceCode);
        ICollection GetAccidentTypes( long facilityID );
        OccurrenceCode CreateOccurrenceCode(long facilityId, string occurrenceCode, long occurrenceDate);
    }
}
