using System;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IMSPBroker.
	/// </summary>
	
    public interface IMSPBroker
    {
        MedicareSecondaryPayor MSPFor(IAccount anAccount);
        DateTime GetMSP2StartDate();
    }
}
