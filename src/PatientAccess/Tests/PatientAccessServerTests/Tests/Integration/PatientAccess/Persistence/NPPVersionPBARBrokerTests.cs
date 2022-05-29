using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for NPPVersionBrokerTest.
    /// </summary>

    //TODO: Create XML summary comment for NPPVersionBrokerTests
    [TestFixture()]
    public class NPPVersionPBARBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown NPPVersionBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpNPPVersionBrokerTest()
        {
            i_NPPVersionBroker = BrokerFactory.BrokerOfType<INPPVersionBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownNPPVersionBrokerTest()
        {
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestNPPVersionsFor()
        {
            ICollection versions = i_NPPVersionBroker.NPPVersionsFor( ACO_FACILITYID );
            Assert.IsNotNull( versions, "No NPP Versions returned for hospital # 900" );
        }

        [Test()]
        public void TestNPPVersionWithCode()
        {
            NPPVersion version = i_NPPVersionBroker.NPPVersionWith( ACO_FACILITYID, "10" );
            Assert.IsNotNull( version, "No NPP Version returned for hospital # 900 and key='10' " );
            Assert.IsTrue( version.Description == "20030414", "Wrong NPP Version Description returned for hospital # 900 and key='10' " );
            Assert.IsTrue( version.ToString() == "1.0     04/14/2003", "Wrong NPP Version Formatting returned for hospital # 900 and oid=3" );
        }

        [Test()]
        [Ignore()]
        public void TestNPPVersionWithInValidCode()
        {
            NPPVersion version = i_NPPVersionBroker.NPPVersionWith( ACO_FACILITYID, "50" );
            Assert.IsFalse( version.IsValid, "Should be a invalid NPP Version." );
        }

        [Test()]
        public void TestNPPVersionForBlank()
        {
            string blank = String.Empty;
            NPPVersion nv = i_NPPVersionBroker.NPPVersionWith( ACO_FACILITYID, blank );

            Assert.AreEqual( blank, nv.Code, "Code  should be blank" );

            Assert.AreEqual( blank, nv.Description, "Description should be blank" );

            Assert.IsTrue( nv.IsValid );

            string versionCodeZero = "0";
            nv = i_NPPVersionBroker.NPPVersionWith( ACO_FACILITYID, versionCodeZero );

            Assert.AreEqual( blank, nv.Code, "Code  should be blank" );

            Assert.AreEqual( blank, nv.Description, "Description should be blank" );

            Assert.IsTrue( nv.IsValid );
        }
       
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static  INPPVersionBroker i_NPPVersionBroker;
        #endregion
    }
}