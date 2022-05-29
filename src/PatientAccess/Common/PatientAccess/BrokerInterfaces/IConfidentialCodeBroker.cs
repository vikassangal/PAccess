using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IConfidentialCodeBroker.
    /// </summary>
    public interface IConfidentialCodeBroker
    {
        IList ConfidentialCodesFor( long facilityNumber );
        ConfidentialCode ConfidentialCodeWith( long facilityNumber, long oid );
        ConfidentialCode ConfidentialCodeWith( long facilityNumber, string code );
    }
}


