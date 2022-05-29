using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class AdmitSourcePBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitSourceBrokerTests

        [TestFixtureSetUp()]
        public static void SetUpAdmitSourceBrokerTests()
        {
            i_Broker = BrokerFactory.BrokerOfType<IAdmitSourceBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownAdmitSourceBrokerTests()
        {
        }

        #endregion

        #region Test Methods
    
        [Test()]
        public void TestAllTypesOfAdmitSources()
        {            
            ArrayList admitSources = (ArrayList)i_Broker.AllTypesOfAdmitSources(ACO_FACILITYID);

            AdmitSource selectedSource = null;

            foreach( AdmitSource admitSource in admitSources )
            {
                if(admitSource.Code == "A")
                    selectedSource = admitSource;
            }

            Assert.IsNotNull(selectedSource,"Did not find Physician AdmitSource");
            
        }

        [Test()]
        public void TestAdmitSourcesForNewBorn()
        {            
            AdmitSource  admitSource = i_Broker.AdmitSourceForNewBorn(ACO_FACILITYID);

            Assert.AreEqual(
                "L",
                admitSource.Code,
                "Code should be L" );
        }

        [Test()]
        public void TestAdmitSourcesForNotNewBorn()
        {            
            ArrayList admitSources = (ArrayList)i_Broker.AdmitSourcesForNotNewBorn(ACO_FACILITYID);

            foreach( AdmitSource admitSource in admitSources)
            {
                Assert.IsTrue( admitSource.Code != "L", "Code should not be L");
            }           
        }
      
        [Test()]
        public void TestAdmitSourceWithCode()
        {            
            string  code = "A";
            AdmitSource admitSource  = i_Broker.AdmitSourceWith(ACO_FACILITYID,code);

            Assert.AreEqual( "A", admitSource.Code, "Code  should be A" );

            Assert.IsTrue( admitSource.IsValid, "Invalid Admitsource Code" );
        }

        [Test(), ExpectedException( typeof( ArgumentNullException ) ) ]
        public void TestAdmitSourcesForNull()
        {
            AdmitSource admitSource = i_Broker.AdmitSourceWith( ACO_FACILITYID, null ) ;
        }
        
        [Test()]
        public void TestAdmitSourcesForBlank()
        {            
            string blank = String.Empty;
            AdmitSource admitSource  = i_Broker.AdmitSourceWith(ACO_FACILITYID,blank);

            Assert.AreEqual(
                blank,
                admitSource.Code,
                "Code  should be blank");
            Assert.AreEqual
                (blank,
                 admitSource.Description,
                 "Description should be blank"
                );
            Assert.IsTrue(
                admitSource.IsValid, "Invalid Admitsource Code"            
                );
        }

        [Test()]
        public void TestAdmitSourcesForInvalid()
        {            
            string InvalidCode = "Q";
            AdmitSource admitSource  = i_Broker.AdmitSourceWith(ACO_FACILITYID,InvalidCode);

            Assert.AreEqual(
                InvalidCode,
                admitSource.Code,
                "Code should be Q");
            Assert.AreEqual
                ("Q",
                 admitSource.Description,
                 "Description should be 'Q'"
                );
            Assert.IsFalse(
                admitSource.IsValid, "Should not be a valid Admit Source code"            
                );
        }
        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IAdmitSourceBroker i_Broker = null;
        #endregion

        #region Constants

        #endregion
    }
}