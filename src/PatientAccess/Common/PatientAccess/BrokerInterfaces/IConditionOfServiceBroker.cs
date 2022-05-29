using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
	/// Summary description for IConditionOfServiceBroker.
	/// </summary>
	
    public interface IConditionOfServiceBroker
    {
        ICollection AllConditionsOfService();                    
        ConditionOfService ConditionOfServiceWith( string code );
        ConditionOfService ConditionOfServiceWith( long id );
    } 
}
