using System;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Extensions.Exceptions;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Security;
using NUnit.Framework;
using Xstream.Core;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// General Set of Tests that need to be completed for each Broker
    /// </summary>
    public abstract class AbstractBrokerTests
    {
        #region Constants
        // Connection String in App.Config
        private const string CONFIG_CXN_STRING = "ConnectionString";
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
        public const long DHF_FACILITYID = 54;
        public const long INVALID_FACILITY_ID = 901;
        public const int FACILITY_TENET_STANDARD = 999;

        //private const string USE_REMOTING_COMPRESSION = "CCUseRemotingCompression";
        //private const string REMOTING_ERROR_RETRIES = "CCRemotingErrorRetries";
        //private const string REMOTING_TIMEOUT = "CCRemotingTimeout";

        #endregion

        #region SetUp and TearDown GenericBrokerTests
        [TestFixtureSetUp()]
        public static void CreateUser()
        {
            User patientAccessUser = User.GetCurrent();
            if ( patientAccessUser.SecurityUser == null )
            {
                IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
                Facility selectedFacility = facilityBroker.FacilityWith( ACO_FACILITYID );

                IUserBroker br = BrokerFactory.BrokerOfType<IUserBroker>();
                SecurityResponse securityResponse = br.AuthenticateUser( USER_NAME, PASSWORD, selectedFacility );

                User.SetCurrentUserTo( securityResponse.PatientAccessUser );
            }
        }

        #endregion

        #region Test Methods

        #endregion

        #region Support Methods

        /// <summary>
        /// Loads the file from resources.
        /// </summary>
        /// <param name="nonQualifiedFileName">Name of the non qualified file.</param>
        /// <returns></returns>
        protected Stream LoadFileFromResources( string nonQualifiedFileName )
        {

            string fullyQualifiedResourceName =
                String.Format(
                    "Tests.Resources.{0}",
                    nonQualifiedFileName );
            Stream returnStream =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream( fullyQualifiedResourceName );
            return returnStream;

        }

        /// <summary>
        /// Loads a test object that was saved using the XStream library
        /// </summary>
        /// <typeparam name="T">This is the type of object we are typing to create</typeparam>
        /// <param name="shortResourceName">Name of the test fixture. The method will attempt to open a local xml file with this name</param>
        /// <param name="testObjectName">Name of the test object. The method will look for a TestObject nore with this identifier</param>
        /// <returns>An object of type T that was deserialized from an XML file</returns>
        /// <remarks>XStream does not have the restrictions imposed by MS serialization</remarks>
        protected T CreateTestObjectFor<T>( string shortResourceName, string testObjectName ) where T : class
        {
            string objectAsXml = this.GetObjectAsXml( shortResourceName, testObjectName );

            XStream xstreamSerializer = new XStream();

            T returnObject = xstreamSerializer.FromXml( objectAsXml ) as T;

            Debug.Assert( null != returnObject );

            return returnObject;
        }


        private string GetObjectAsXml( string shortResourceName, string testObjectName )
        {
            Stream inputStream = this.LoadFileFromResources( shortResourceName );

            XmlDocument testDocument = new XmlDocument();

            testDocument.Load( inputStream );

            inputStream.Close();
            
            XmlNode testObjectNode = testDocument.SelectSingleNode( string.Format( "/TestObjects/TestObject[@id='{0}']", testObjectName ) );

            return testObjectNode.InnerXml;
        }


        public void Print( Exception e )
        {
            Console.WriteLine( e.Message );
            if ( e.InnerException != null )
            {
                Console.WriteLine( e.InnerException.Message );
            }
            if ( e is EnterpriseException )
            {
                foreach ( DictionaryEntry de in ( (EnterpriseException)e ).Context )
                {
                    Console.WriteLine( de.Key + " = " + de.Value );
                }
            }
        }

        virtual public string ConnectionString
        {
            get
            {
                string connectionString = ConfigurationManager.AppSettings[CONFIG_CXN_STRING];

                return connectionString;
            }
        }
        #endregion

        #region Data Elements

        protected static bool CacheLoaded = false;

        #endregion
    }
}