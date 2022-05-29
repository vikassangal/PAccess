using System;
using System.Collections;
using System.Configuration;
using PatientAccess.BrokerInterfaces;

namespace PatientAccess.Persistence
{

    /// <summary>
    /// Summary description for ClientConfigurationBroker.
    /// </summary>
    [Serializable]
    public class ClientConfigurationBroker : MarshalByRefObject, IClientConfigurationBroker
    {

        #region Events
        #endregion

        #region Event Handlers
        #endregion

        #region Methods

        public Hashtable ConfigurationValues()
        {
            Hashtable configValues = new Hashtable();
            
            foreach (object o in ConfigurationManager.AppSettings )
            {
                // Send down only keys that start with 'CC' (ClientConfiguration)

                if( o.ToString().Substring( 0, 2 ) == "CC" )
                {
                    configValues.Add(o.ToString(), ConfigurationManager.AppSettings[o.ToString()]);
                    //configValues.Add( o.ToString(), Peradigm.Framework.Configuration.ApplicationConfiguration.Settings[o.ToString()] as string );
                }
            }

            return configValues;
        }

        public object ConfigurationValueFor( string key )
        {
            string val = ConfigurationManager.AppSettings[key];

            return val;
        }

        #endregion

        #region Properties
        #endregion

        #region Private Methods
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization

        public ClientConfigurationBroker()
        {
        }

        #endregion

        #region Component Designer generated code
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        #endregion

        
    }
}