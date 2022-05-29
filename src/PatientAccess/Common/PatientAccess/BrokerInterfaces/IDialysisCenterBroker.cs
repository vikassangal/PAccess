using System.Collections.Generic;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Interface for retrieving <see cref="DialysisCenterNames"/> objects
    /// </summary>
    public interface IDialysisCenterBroker
    {
        ICollection<string> AllDialysisCenterNames(long facilityID);
    }
}
