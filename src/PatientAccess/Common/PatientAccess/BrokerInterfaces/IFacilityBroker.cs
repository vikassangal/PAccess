using System;
using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IFacilityBroker.
    /// </summary>
    public interface IFacilityBroker
    {
        ICollection AllFacilities();
        Facility FacilityWith( long oid );
        Facility FacilityWith( string HSPCode );    
        DateTime GetAppServerDateTime();
        bool IsDatabaseAvailableFor( string facilityServerIP );
    }
}
