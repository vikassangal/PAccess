using System.Collections.Generic;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface for retrieving <see cref="ISuffixBroker"/> objects
    /// </summary>
    public interface ISuffixBroker
    {
        ICollection<string> AllSuffixCodes( long facilityID );
    }
}
