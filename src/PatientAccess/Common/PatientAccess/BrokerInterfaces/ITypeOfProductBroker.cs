using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ITypeOfProductBroker.
	/// </summary>
	public interface ITypeOfProductBroker
	{
        IList AllTypeOfProducts();
        TypeOfProduct TypeOfProductWith( long oid );
	}
}
