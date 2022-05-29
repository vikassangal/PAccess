using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IFacilityFlagBroker.
	/// </summary>
	public interface IFacilityFlagBroker
	{
		IList FacilityFlagsFor(long facilityId);
        FacilityDeterminedFlag FacilityFlagWith(long facilityId, string code);
        FacilityDeterminedFlag FacilityFlagWith(long facilityId, long facilityFlagID);
    }
}
