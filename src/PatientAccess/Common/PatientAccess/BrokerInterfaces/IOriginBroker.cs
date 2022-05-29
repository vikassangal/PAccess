using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface lists the Origin Broker related methods.
    /// </summary>
    public interface IOriginBroker
    {
        //Get all the races for a facilityID.
        ICollection AllRaces( long facilityID );
        //Get the race for a specific facilityID and OID
        ICollection LoadRaces(long facilityID );
        ICollection LoadNationalities(long facilityID );

        Race RaceWith( long facilityID, long oid );
        Race RaceWith(long facilityID, string code);
        //Get all the Ethnicities for a facilityID.
        ICollection AllEthnicities( long facilityID );
        //Get the ethinicity for a specific facilityID and OID
        Ethnicity EthnicityWith( long facilityID, long oid );
        Ethnicity EthnicityWith(long facilityID, string code);
        ICollection LoadEthnicities(long facilityID);
        ICollection LoadDescent(long facilityID);

    }
}
