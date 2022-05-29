using System.Collections;
using Extensions.SecurityService.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IRoleBroker.
	/// </summary>
	public interface IRoleBroker
	{
		Hashtable AllRoles();
        Role RoleWith( long oid );
	}
}
