using System;
using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for InfoReceivedSourceBrokerTest.
    /// </summary>

    [TestFixture()]
    public class InfoReceivedSourceBrokerTest : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown InfoReceivedSourceBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpInfoReceivedSourceBrokerTest()
        {
            infoReceivedSourceBroker = BrokerFactory.BrokerOfType<IInfoReceivedSourceBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownInfoReceivedSourceBrokerTest()
        {
        }
        #endregion

        #region Test Methods         
        
        [Test()]
        public void TestAllInfoReceivedSource()
        {
            ArrayList list = (ArrayList)infoReceivedSourceBroker.AllInfoReceivedSources();
            Assert.IsTrue( list.Count > 0, "List not null" );
        }

        [Test()]
        public void TestInfoReceivedSourceForValid()
        {
            string blank = String.Empty;
            InformationReceivedSource informationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith( "2" );

            Assert.AreEqual
                ( "PHONE",
                  informationReceivedSource.Description.ToUpper(),
                  "Description should be PHONE"
                );
        }
        [Test()]
        public void TestInfoReceivedSourceForBlank()
        {            
            string blank = String.Empty;
            InformationReceivedSource informationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith( "1" );

            Assert.AreEqual
                (blank,
                 informationReceivedSource.Description,
                 "Description should be blank"
                );
        }
      
        [Test()]
        public void TestInfoReceivedSourceForInValid()
        {
            InformationReceivedSource informationReceivedSource = infoReceivedSourceBroker.InfoReceivedSourceWith( "5" );
            Assert.IsFalse( informationReceivedSource.IsValid, "Invalid Code Expected" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IInfoReceivedSourceBroker infoReceivedSourceBroker;
        #endregion
    }
}