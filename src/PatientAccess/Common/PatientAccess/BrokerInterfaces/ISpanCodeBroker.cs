using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for ISpanCodeBroker
    /// </summary>
    public interface ISpanCodeBroker
    {
        ICollection AllSpans( long facilityID );
        SpanCode SpanCodeWith( long facilityID, string code );
    }
}
