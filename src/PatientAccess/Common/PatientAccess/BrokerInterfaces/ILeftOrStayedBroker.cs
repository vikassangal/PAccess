using System.Collections.Generic;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for ILeftOrStayedBroker.
	/// </summary>
	
    public interface ILeftOrStayedBroker
    {
        ICollection<LeftOrStayed> AllLeftOrStayed();
        LeftOrStayed LeftOrStayedWith( string code );
    } 
}
