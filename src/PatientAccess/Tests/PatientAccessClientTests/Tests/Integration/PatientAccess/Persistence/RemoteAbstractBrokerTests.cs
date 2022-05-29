using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Xml;
using Extensions.Exceptions;
using Xstream.Core;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// General Set of Tests that need to be completed for each Broker
    /// </summary>
    public abstract class RemoteAbstractBrokerTests
    {
        #region Constants

        private const string CONNECTION_STRING = "User ID=PA_DEV_USER;Password=PA_DEV_USER;Data Source=PAD;Connection Timeout=60;";
        // Connection String in App.Config
        private const string CONFIG_CXN_STRING = "OracleConnectionString";
        // Failure Messages
        public const string MSG_CONNECTION_FAILED = "Database Connection Failed";
        public const string MSG_WRONG_ATTRIBUTE_VALUE = "Wrong value for <{0}>";
        public const string MSG_WRONG_NUMBER_OF_RECORDS = "Expecting Different Number of records for <{0}>";
        public const string INSERT_SUCCESS_MESSAGE = "Row Inserted Successfully";
        public const string UPDATE_SUCCESS_MESSAGE = "Row Update Successfully";
        public const string DELETE_SUCCESS_MESSAGE = "Row Deleted Successfully";

        public const string USER_NAME = "patientaccess.user02";
        public const string PASSWORD = "Password1";
        public const string FACILITY_CODE = "ACO";
        public const long ACO_FACILITYID = 900;

        public const int FACILITY_TENET_STANDARD = 999; 

        #endregion

        #region SetUp and TearDown GenericBrokerTests

        #endregion

        #region Test Methods

        #endregion

        #region Support Methods

        /// <summary>
        /// Loads a test object that was saved using the XStream library
        /// </summary>
        /// <typeparam name="T">This is the type of object we are typing to create</typeparam>
        /// <param name="testFixtureName">Name of the test fixture. The method will attempt to open a local xml file with this name</param>
        /// <param name="testObjectName">Name of the test object. The method will look for a TestObject nore with this identifier</param>
        /// <returns>An object of type T that was deserialized from an XML file</returns>
        /// <remarks>XStream does not have the restrictions imposed by MS serialization</remarks>
        protected T CreateTestObjectFor<T>( string testFixtureName, string testObjectName ) where T : class
        {

            Debug.Assert( !string.IsNullOrEmpty( testFixtureName ) );
            Debug.Assert( !string.IsNullOrEmpty( testObjectName ) );

            // Our test information is stored in the XML doc
            XmlDocument testDocument = 
                new XmlDocument();
            testDocument.Load( string.Format( "{0}.xml", testFixtureName ) );

            // We need the XML text for the object so we can feed XStream
            XStream xstreamSerializer = 
                new XStream();
            XmlNode testObjectNode =
                testDocument.SelectSingleNode( string.Format( "/TestObjects/TestObject[@id='{0}']", testObjectName ) );

            T returnObject =
                xstreamSerializer.FromXml( testObjectNode.InnerXml ) as T; 
            
            Debug.Assert( null != returnObject );

            return returnObject;

        }

        public void Print( Exception e )
        {
            Console.WriteLine( e.Message );
            if( e.InnerException != null )
            {
                Console.WriteLine( e.InnerException.Message );
            }
            if( e is EnterpriseException )
            {
                foreach( DictionaryEntry de in ((EnterpriseException)e).Context )
                {
                    Console.WriteLine( de.Key + " = " + de.Value ); 
                }
            }
        }

        virtual public string ConnectionString
        {
            get
            {
                string cxn = ConfigurationManager.ConnectionStrings[CONFIG_CXN_STRING].ConnectionString;

                if( cxn == null )
                {
                    cxn = CONNECTION_STRING;
                }

                return cxn;
            }
        }
        #endregion

        #region Data Elements

        protected static bool CacheLoaded = false;

        #endregion
    }
}