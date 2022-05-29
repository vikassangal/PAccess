using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IResistantOrganismBroker
    /// </summary>
    public interface IResistantOrganismBroker
    {
        ICollection AllResistantOrganisms( long facilityID );
        ResistantOrganism ResistantOrganismWith( long facilityID, string code );
    }
}
