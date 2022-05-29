using System.Collections.Generic;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture]
    public class RemoteResearchStudyBrokerTests : RemoteAbstractBrokerTests
    {
        #region Constants
        const string NO_RESEARCH_STUDY_RETURNED_MESSAGE = "No Research Studies Returned";
        const string CANNOT_CREATE_RESEARCH_STUDY_BROKER = "Could not create remote ResearchStudyBroker";
        #endregion
        [Test]
        [Category( "Fast" )]
        public void TestResearchStudyBrokerTestsRemote()
        {
            IResearchStudyBroker researchStudyBroker = BrokerFactory.BrokerOfType<IResearchStudyBroker>();
            Assert.IsNotNull( researchStudyBroker, CANNOT_CREATE_RESEARCH_STUDY_BROKER );
        }

        [Test]
        public void TestResearchStudyBrokerRemote_AllResearchStudiesTests()
        {
            IResearchStudyBroker researchStudyBroker = BrokerFactory.BrokerOfType<IResearchStudyBroker>();
            IEnumerable<ResearchStudy> researchStudies = researchStudyBroker.AllResearchStudies( 98 );
            Assert.IsNotNull( researchStudies, NO_RESEARCH_STUDY_RETURNED_MESSAGE );
        }
    }
}
