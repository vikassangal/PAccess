namespace PatientAccess.BrokerInterfaces
{

    /// <summary>
    /// This class is used to hold the settings used by the BrokerFactory class when
    /// mapping an interface to a concrete type.
    /// </summary>
    public class BrokerInterfaceMap
    {

        #region Fields

        private string i_Interface;
        private string i_Implementation;
        private string i_Endpoint;
        private string i_AssemblyName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>The name of the assembly.</value>
        public string AssemblyName
        {
            get
            {
                return i_AssemblyName;
            }
            set
            {
                i_AssemblyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the fully-qualified name of the interface.
        /// </summary>
        /// <value>The interface name.</value>
        /// <example>Foo.Bar.IFooBroker</example>
        public string Interface
        {
            get
            {
                return i_Interface;
            }
            set
            {
                i_Interface = value;
            }
        }

        /// <summary>
        /// Gets or sets the fully-qualified name of the implementation type.
        /// </summary>
        /// <value>The implementation type name.</value>
        /// <example>Foo.Bar.BigFooBroker</example>
        public string Implementation
        {
            get
            {
                return i_Implementation;
            }
            set
            {
                i_Implementation = value;
            }
        }

        /// <summary>
        /// Gets or sets the endpoint (minus the URL) used for remoting.
        /// </summary>
        /// <value>The endpoint name.</value>
        /// <example>TheFooBroker.rem</example>
        public string Endpoint
        {
            get
            {
                return i_Endpoint;
            }
            set
            {
                i_Endpoint = value;
            }
        }

        #endregion

    }//class

}//namespace
