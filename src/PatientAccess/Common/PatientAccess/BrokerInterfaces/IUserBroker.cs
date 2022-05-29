using PatientAccess.Domain;
using PatientAccess.Domain.Security;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IUserBroker.
	/// </summary>
	public interface IUserBroker
	{
		SecurityResponse AuthenticateUser ( string userName, string password, Facility selectedFacility );
        void Logout ( User patientAccessUser );
        void Credentials ( out string userName, out string password, out string workstationID );
        bool HasPermissionToOverrideMonthlyPayment ( string userName, string password, Facility facility );
	}
}