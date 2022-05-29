using System.Collections.Generic;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface for retrieving <see cref="AlternateCareFacility"/> objects
    /// </summary>
    public interface IAlternateCareFacilityBroker
    {
        ICollection<string> AllAlternateCareFacilities( long facilityID );
    }
}
