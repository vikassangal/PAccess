using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IPBARSecurityCodeBroker.
	/// </summary>
    public interface ISecurityCodeBroker
    {
        string GetPrintedSecurityCode( string PBAREmployeeId, Facility facility );
    }
}
