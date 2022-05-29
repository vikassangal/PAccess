using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Xml.Serialization;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;

namespace Tests.Integration.PatientAccess.BrokerInterfaces
{
    /// <summary>
    /// Summary description for BrokerFactoryTests.
    /// </summary>

    [TestFixture]
    [Category( "Fast" )]
    public class BrokerFactoryTests
    {
        #region SetUp and TearDown CancelPreRegTransactionCoordinatorTests
        [TestFixtureSetUp]
        public static void SetUpCancelPreRegTransactionCoordinatorTests()
        {
            DeserializeXmlMappingFile() ;
        }

        [TestFixtureTearDown]
        public static void TearDownCancelPreRegTransactionCoordinatorTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestBrokerFactoryWithConfig()
        {
            var facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker >();
            Assert.IsNotNull( facilityBroker );
        }

        [Test, ExpectedException( typeof( RemotingException ) )]
        public void TestFailingWhenNoBrokerExists()
        {
            BrokerFactory.BrokerOfType<string>();
        }

        [Test]
        public void TestBrokerFactoryWithSuppliedServerUri()
        {
            var facilityBroker = BrokerFactory.BrokerOfType< IFacilityBroker >( ConfigurationManager.AppSettings[APP_SERVER_KEY] ) ;
            Assert.IsNotNull( facilityBroker );
        }

        [Test, ExpectedException( typeof( WebException) )]
        public void TestFailingWhenSupplyingAnInvalidServerUri()
        {
            var facilityBroker = BrokerFactory.BrokerOfType< IFacilityBroker >("http://someinvaliduri.hdc.net" ) ;
            facilityBroker.AllFacilities();
        }

        /// <summary>
        /// Private static method called in Setup to read in the mappings
        /// from the XML file into an array, brokerInterfaceMaps.
        /// </summary>
        private static void DeserializeXmlMappingFile()
        {
            // Need this so we can deserialize as an array
            var mappingFileRootAttribute = new XmlRootAttribute(BROKER_INTERFACE_MAPS_CONFIG_ROOT);

            // Tell the serializer that we want to be able to handle
            // an array of our BrokerInterfaceMap objects
            var xmlSerializer = new XmlSerializer(typeof(BrokerInterfaceMap[]), mappingFileRootAttribute);

            // Get the file stream from the resources and deserialize it
            brokerInterfaceMaps =
                xmlSerializer.Deserialize(
                    Assembly.GetAssembly(typeof(BrokerFactory))
                        .GetManifestResourceStream(
                        BROKER_INTERFACE_MAPPING_RESOURCE_NAME))
                as BrokerInterfaceMap[];
        }

        /// <summary>
        /// After having read in the contents of the 'BrokerInterfaces.Mappings.xml'
        /// file into the BrokerInterfaceMap array, this private method loops through
        /// the passed in array extracting the interface and concrete implementation types.
        /// It then tests whether the concrete implementation is indeed an instance of
        /// the extracted interface; if not, the assert will fail. Note. this is done
        /// for all type mapping entries that are located in the BrokerInterface Assembly.
        /// Note. this method requires DeserializeXmlMappingFile() to have
        /// already been called, hence it is called in Setup.
        /// </summary>
        [Test]
        public void TestXMLTypeMappingEntriesBrokerInterfaceAssembly()
        {
            var interfaceAssembly = Assembly.Load( BROKER_INTERFACES_ASSEMBLY );
            var implementationAssemblies = new Dictionary<string, Assembly>
                {
                    {PERSISTENCE_ASSEMBLY, Assembly.Load(PERSISTENCE_ASSEMBLY)}
                };

            foreach ( var map in brokerInterfaceMaps )
            {
                var interfaceName = map.Interface;
                var interfaceType = interfaceAssembly.GetType( interfaceName );

                var concreteType = implementationAssemblies[map.AssemblyName].GetType( map.Implementation, false, true );

                Assert.IsTrue( interfaceType.IsAssignableFrom( concreteType ), "Interface type = " + interfaceType + "Concrete type = " + concreteType );
            }
        }

        [Test]
        public void CanCreateInstancesOfAllInterfacesInTheMappingFile()
        {
            var interfaceAssembly = Assembly.Load( BROKER_INTERFACES_ASSEMBLY );

            foreach ( var map in brokerInterfaceMaps )
            {
                var interfaceName = map.Interface;
                var interfaceType = interfaceAssembly.GetType( interfaceName );

                var brokerInstance = typeof( BrokerFactory ).GetMethod( "BrokerOfType", new Type[] { } )
                    .MakeGenericMethod( interfaceType )
                    .Invoke( null, null );

                Assert.IsNotNull( brokerInstance, "Could not create an instace for type: " + interfaceType );
            }
        }

        #endregion

        #region Properties
        #endregion

        		
        #region Support Methods
        #endregion

        #region Data Elements

        private static BrokerInterfaceMap[] brokerInterfaceMaps;	    

        #endregion

        #region Constants
        private const string APP_SERVER_KEY = "PatientAccess.AppServer" ;
        private const string BROKER_INTERFACES_ASSEMBLY = "PatientAccess.Common" ;
        private const string PERSISTENCE_ASSEMBLY = "PatientAccess.AppServer" ;


        /// <summary>
        /// This constant keeps a count of the number of entries in the
        /// XML mapping file that have Interfaces located the persistnce
        /// assembly and not the brokerinterfaces Assembly.
        /// </summary>
        
        /// <summary>
        /// This is the root element name in the BrokerInterfaces.Mappings.xml file. It is used
        /// to allow the XmlSerializer to deserialize the elements in the file as an array
        /// of BrokerInterfaceMap objects.
        /// </summary>
        private const string BROKER_INTERFACE_MAPS_CONFIG_ROOT = "BrokerInterfaceMaps";

        /// <summary>
        /// Used to find the BrokerInterfaces.Mappings.xml file stored in the assembly's resource
        /// file
        /// </summary>
        private const string BROKER_INTERFACE_MAPPING_RESOURCE_NAME = "PatientAccess.Resources.BrokerInterfaces.Mappings.xml";

        #endregion
    }
}