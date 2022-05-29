using System.Collections;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for IClientConfigurationBroker.
    /// </summary>
    public interface IClientConfigurationBroker
    {
        Hashtable ConfigurationValues();

        object ConfigurationValueFor( string key );
    }
}
