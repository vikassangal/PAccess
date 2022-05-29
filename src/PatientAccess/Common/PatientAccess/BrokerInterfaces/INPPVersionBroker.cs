using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for INPPVersionBroker.
	/// </summary>
	public interface INPPVersionBroker
    {
        ICollection NPPVersionsFor( long facilityNumber );
        NPPVersion NPPVersionWith( long facilityNumber, string code );
    }
}
