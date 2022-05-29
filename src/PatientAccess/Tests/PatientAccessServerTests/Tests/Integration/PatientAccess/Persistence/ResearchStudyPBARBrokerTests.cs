using System;
using System.Collections.Generic;
using System.Linq;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class ResearchStudyPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ResearchStudyPBARBrokerTests
        [TestFixtureSetUp]
        public static void SetUpResearchStudyBrokerTests()
        {
            i_ResearchStudyBroker =
                BrokerFactory.BrokerOfType<IResearchStudyBroker>();
        }

        [TestFixtureTearDown]
        public static void TearDownResearchStudyBrokerTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestAllResearchStudies()
        {
            IEnumerable<ResearchStudy> allResearchStudies = i_ResearchStudyBroker.AllResearchStudies( ACO_FACILITYID );
            Assert.IsTrue( allResearchStudies.Count() > 0, "No ResearchStudies found" );
        }

        [Test]
        public void TestResearchStudiesWithCode()
        {
            string code = "ICE09005";
            ResearchStudy researchStudy = i_ResearchStudyBroker.ResearchStudyWith( ACO_FACILITYID, code );

            Assert.AreEqual( "TEST STUDY 2009 005", researchStudy.Description,
                             "description  should be TEST STUDY 2009 005" );
            Assert.IsTrue( researchStudy.IsValid );
        }

        [Test]
        public void TestResearchStudyForBlank()
        {
            string blank = String.Empty;
            ResearchStudy ro = i_ResearchStudyBroker.ResearchStudyWith( ACO_FACILITYID, blank );

            Assert.IsFalse( ro.IsValid, "Code Blank should be valid" );
        }

        [Test]
        public void TestResearchStudyForInvalid()
        {
            string code = "M";
            ResearchStudy ro = i_ResearchStudyBroker.ResearchStudyWith( ACO_FACILITYID, code );

            Assert.IsFalse( ro.IsValid, "Code M should be invalid" );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestResearchStudyForNULL()
        {

            ResearchStudy ro = i_ResearchStudyBroker.ResearchStudyWith( ACO_FACILITYID, null );

            Assert.IsFalse( ro.IsValid, "Code null should be invalid" );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IResearchStudyBroker i_ResearchStudyBroker;
        #endregion

        #region Constants
        #endregion
    }
}