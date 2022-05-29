using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IModeOfArrivalBroker.
	/// </summary>
	public interface IModeOfArrivalBroker
	{
		ArrayList ModesOfArrivalFor( long facilityID );
	    ModeOfArrival ModeOfArrivalWith(long facilityID, string code);
    }
}
