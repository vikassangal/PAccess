using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Xml.Serialization;
using Extensions.PersistenceCommon;
using PatientAccess.RemotingServices;

namespace PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// I provide construct and provide access to brokers.
    /// </summary>
    [Serializable]
    public class BrokerFactory
    {
		#region Constants 

        /// <summary>
        /// This needs to be defined in the local settings file if the factory needs to
        /// create remote instances of classes. This setting is combinged with the 
        /// endpoint setting in the BrokerInterfaces.Mappings.xml file to form the full
        /// URL to the remote object.
        /// </summary>
        private const string APP_SERVER_KEY = "PatientAccess.AppServer";
        /// <summary>
        /// Used to find the BrokerInterfaces.Mappings.xml file stored in the assembly's resource
        /// file
        /// </summary>
        private const string BROKER_INTERFACE_MAPPING_RESOURCE_NAME = 
            "PatientAccess.Resources.BrokerInterfaces.Mappings.xml";
        /// <summary>
        /// This is the root element name in the BrokerInterfaces.Mappings.xml file. It is used
        /// to allow the XmlSerializer to deserialize the elements in the file as an array
        /// of BrokerInterfaceMap objects.
        /// </summary>
        private const string BROKER_INTERFACE_MAPS_CONFIG_ROOT = "BrokerInterfaceMaps";
        /// <summary>
        /// This is used to determin if we need to build a remote or local instance. If this
        /// setting cannot be found in the local config, then the class will throw a
        /// TypeInitializationException and the program will stop. It is possible to have a 
        /// default value for this setting, but a reasonable default is difficult to guess.
        /// </summary>
        private const string IS_SERVER = "PatientAccess.IsServer";
        /// <summary>
        /// Wait for ten second when trying to grab the reader or writer lock. If the operation
        /// blocks for anything even close to that amount of time, something is deadlocked and
        /// we should let the framework throw to avoid deadlocking the application.
        /// </summary>
        private const int LOCK_WAIT = 10000;

		#endregion Constants 

		#region Fields 

        /// <summary>
        /// Guards access to the assembly cache
        /// </summary>
        private static readonly ReaderWriterLock _assemblyCacheLock = 
            new ReaderWriterLock();
        /// <summary>
        /// Holds any loaded assemblies
        /// </summary>
        private static readonly Dictionary<string, Assembly> _brokerAssemblies =
            new Dictionary<string, Assembly>();
        /// <summary>
        /// This holds our list of mapping objects
        /// </summary>
        private static readonly Dictionary<string,BrokerInterfaceMap> _brokerInterfaceMaps = 
            new Dictionary<string,BrokerInterfaceMap>();
        /// <summary>
        /// Provides context to this class so it knows if it needs to create remoting
        /// or local instances of brokers
        /// </summary>
        private static readonly bool _isServer = 
            Boolean.Parse(ConfigurationManager.AppSettings[IS_SERVER]);
        /// <summary>
        /// URL used to create remote instances of brokers
        /// </summary>
        private static readonly string _serverUrl =
            ConfigurationManager.AppSettings[APP_SERVER_KEY];

		#endregion Fields 

		#region Constructors 

        /// <summary>
        /// Initializes the <see cref="BrokerFactory"/> class.
        /// </summary>
        /// <remarks>
        /// The one really nice thing about this static constructor
        /// is that it is guaranteed to be thread safe as all other
        /// threads will block until the initial thread is done with
        /// this constructor. If there is an error in the mapping file
        /// or in the configuration file, this will throw a
        /// TypeInitializationException.
        /// </remarks>
        static BrokerFactory()
        {

            LoadBrokerInterfaceMaps();

            if( !IsHostedOnServer )
            {
                RemotingConfigurationUtility.InitialzeDefaultRemoting();
            }

        }


        /// <summary>
        /// Instances of types that define only static members do not need to be created.
        /// Many compilers will automatically add a public default constructor if no 
        /// constructor is specified.  To prevent this, adding an empty private 
        /// constructor may be required.
        /// </summary>
        private BrokerFactory()
        {

        }

		#endregion Constructors 

		#region Properties 

        /// <summary>
        /// Gets the server URL.
        /// </summary>
        /// <value>The server URL.</value>
        private static string ServerUrl
        {
            get
            {
                return _serverUrl;
            }
        }
        /// <summary>
        /// Guards access to the assembly cache
        /// </summary>
        private static ReaderWriterLock AssemblyCacheLock
        {
            get
            {
                return _assemblyCacheLock;
            }
        }


        /// <summary>
        /// Holds any loaded assemblies
        /// </summary>
        private static Dictionary<string, Assembly> BrokerAssemblies
        {
            get
            {
                return _brokerAssemblies;
            }
        }


        /// <summary>
        /// Gets the broker interface maps.
        /// </summary>
        /// <value>The broker interface maps.</value>
        private static Dictionary<string, BrokerInterfaceMap> BrokerInterfaceMaps
        {
            get
            {
                return _brokerInterfaceMaps;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance is hosted on server.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hosted on server; otherwise, <c>false</c>.
        /// </value>
        private static bool IsHostedOnServer
        {
            get
            {
                return _isServer;
            }
        }

		#endregion Properties 

		#region Methods 

        /// <summary>
        /// Creates a broker of type T and then sets its connection string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>New broker instance</returns>
        public static T BrokerOfType<T>( ConnectionStringSettings connectionString ) where T : class
        {

             IAbstractBroker newBroker =
                BrokerOfType<T>()
                    as IAbstractBroker;

            newBroker.ConnectionString = connectionString.ConnectionString;
            
            return newBroker as T;
            
        }


        /// <summary>
        /// Create a new broker and pass an existing transaction to it so it becomes
        /// ReusableExcecutionPolicy broker
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aTransaction">A transaction.</param>
        /// <returns>New broker instance</returns>
        public static T BrokerOfType<T>( IDbTransaction aTransaction ) where T : class
        {

            IAbstractBroker newBroker =
                BrokerOfType<T>()
                    as IAbstractBroker;

            newBroker.Transaction = aTransaction;

            return newBroker as T;

        }


        /// <summary>
        /// Create a new broker of the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>New broker instance</returns>
        public static T BrokerOfType<T>() where T : class
        {

            return BrokerOfType<T>(null as string);

        }


        /// <summary>
        /// Create a new broker of the given type at the given URL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serverUri">The server URI. If null, the method will attempt to read
        /// the value from the configuration PatientAccess.AppServer key in the configuration
        /// file</param>
        /// <returns>
        /// A proxy to the remote object. You must cast the returned proxy to your type
        /// to begin using it.
        /// </returns>
        public static T BrokerOfType<T>(string serverUri) where T : class
        {

            T newBroker;
            BrokerInterfaceMap interfaceMap;

            // Proceed only of we have a mapping for the interface name
            if( BrokerInterfaceMaps.ContainsKey( typeof(T).FullName ) )
            {

                // The mapping has all our info, so get it
                interfaceMap =
                    BrokerInterfaceMaps[ typeof(T).FullName ];

            }
            else
            {

                throw new RemotingException(
                    string.Format("Cannot find broker interface map for '{0}'",
                    typeof(T).FullName ) );

            }

            if(IsHostedOnServer)
			{

			    // We can only create the type if we load the assembly that contains it
			    Assembly sourceAssembly = GetNamedAssembly( interfaceMap.AssemblyName );

			    // Create the type
			    Type newType = sourceAssembly.GetType( interfaceMap.Implementation );

			    // Create the broker
			    newBroker = Activator.CreateInstance( newType , null ) as T;

			} 
			else
			{

                // Create the local stub for the remote broker. The ?? operator
                // will look at the serverUri and ServerUrl variables and choose
                // the first one that is not null.
                newBroker =
                    Activator.GetObject( typeof(T), 
                                         string.Format(
                                            "{0}{1}",
                                            serverUri ?? ServerUrl,
                                            interfaceMap.Endpoint)) as T;

			}
            
			return newBroker;

        }
                /// <summary>
        /// Gets the named assembly, possibly from cache.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>Assemby used to create new type instance</returns>
        /// <remarks>This might be overkill and it is easily the most
        /// complex bit of code in the class. Keep an eye on this 
        /// as a refactoring cadidate if a better solution comes along
        /// </remarks>
        private static Assembly GetNamedAssembly( string assemblyName )
        {

            Assembly loadedAssembly = null;
            
            
            if( BrokerAssemblies.ContainsKey( assemblyName ) )
            {

                // Get a reader lock to access the cache
                try
                {

                    AssemblyCacheLock.AcquireReaderLock( LOCK_WAIT );

                    loadedAssembly =
                        BrokerAssemblies[ assemblyName ];

                }
                finally
                {

                    AssemblyCacheLock.ReleaseReaderLock();

                }

            }
            else
            {
                
                // Make sure we have a write lock to add to the cache
                try
                {

                    AssemblyCacheLock.AcquireWriterLock( LOCK_WAIT );

                    // This is a double lock to ensure that another thread
                    // has not added the assembly since the first check
                    if( !BrokerAssemblies.ContainsKey( assemblyName ) )
                    {

                        loadedAssembly =
                            Assembly.Load( assemblyName );

                        BrokerAssemblies.Add( assemblyName, loadedAssembly );

                    }

                }
                finally
                {

                    // Make sure we release our lock
                    AssemblyCacheLock.ReleaseWriterLock();

                }

            }

            return loadedAssembly;

        }


        /// <summary>
        /// Loads the broker interface maps from the assembly resources.
        /// </summary>
        /// <remarks>
        /// When the project is compiled, the BrokerInterfaces.Mapping.xml
        /// file is comiled into the assembly as a resource. This method
        /// loads a collection of BrokerInterfaceMap objects from this
        /// resource and then indexes them by the interface property.
        /// </remarks>
        private static void LoadBrokerInterfaceMaps()
        {

            // Need this so we can deserialize as an array
            XmlRootAttribute mappingFileRootAttribute =
                new XmlRootAttribute( BROKER_INTERFACE_MAPS_CONFIG_ROOT );

            // Tell the serializer that we want to be able to handle
            // an array of our BrokerInterfaceMap objects
            XmlSerializer xmlSerializer =
                new XmlSerializer( typeof( BrokerInterfaceMap[] ),
                                   mappingFileRootAttribute );

            // Get the file stream from the resources and deserialize it
            BrokerInterfaceMap[] brokerInterfaceMaps =
                xmlSerializer.Deserialize(
                    Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream(
                                BROKER_INTERFACE_MAPPING_RESOURCE_NAME ) )
                                    as BrokerInterfaceMap[];

            // Sadly, we can't serialize a Dictionary without a lot of
            // extrace code, so we will settle for converting the array
            // into a dictionary that is keyed by the interface name
            foreach( BrokerInterfaceMap map in brokerInterfaceMaps )
            {
                BrokerInterfaceMaps.Add( map.Interface, map );
            }

        }

		#endregion Methods 
    }

}
