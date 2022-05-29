using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ITimeoutBroker.
	/// </summary>
	public interface ITimeoutBroker
	{
	    ActivityTimeout TimeoutFor();
	}
}

