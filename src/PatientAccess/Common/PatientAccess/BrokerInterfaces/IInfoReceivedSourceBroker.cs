using System.Collections;
using PatientAccess.Domain;

namespace PatientAccess.BrokerInterfaces
{
	/// <summary>
    ///interface for InfoReceivedSourceBroker and related methods
	/// </summary>
	public interface IInfoReceivedSourceBroker
	{
        //Get All Information received sources
        ICollection AllInfoReceivedSources();
        //Get the Information received sources for a code
        InformationReceivedSource InfoReceivedSourceWith( string code );
	}
}
