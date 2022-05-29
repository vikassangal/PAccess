using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	public interface IHDIService
	{
        ICollection GetFUSNotesFor( Account anAccount );
	}
}
