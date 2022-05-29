using System;
using System.Collections.Generic;
using NUnit.Framework;
using PatientAccess;
using PatientAccess.BrokerInterfaces;
using PatientAccess.BrokerProxies;
using PatientAccess.Domain;
using Rhino.Mocks;

namespace Tests.Integration.PatientAccess.BrokerProxies
{
    /// <summary>
    /// Summary description for ResearchStudyBrokerProxyTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class ResearchStudyBrokerProxyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitSourceBrokerProxyTests
        [TestFixtureSetUp]
        public static void SetUpAdmitSourceBrokerProxyTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownAdmitSourceBrokerProxyTests()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestAllResearchStudies_ShouldUseUnderlyingBroker()
        {
            ICache cache = MockRepository.GenerateStub<ICache>();
            cache.Expect( x => x.Get( Arg<string>.Is.Anything ) ).Return( null );

            IResearchStudyBroker researchStudyBroker = MockRepository.GenerateMock<IResearchStudyBroker>();

            var researchStudyBrokerProxy = new ResearchStudyBrokerProxy( researchStudyBroker, cache );
            researchStudyBroker.Expect( ( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ) ).Return( new List<ResearchStudy>() );
            researchStudyBrokerProxy.AllResearchStudies( 54 );

            researchStudyBroker.VerifyAllExpectations();
        }

        [Test]
        public void TestTimeAtSecondCall_ShouldNotCallTheUnderlyingBrokerAndShouldUseTheCache()
        {
            ICache cache = MockRepository.GenerateStub<ICache>();
            cache.Expect( x => x.Get( Arg<string>.Is.Anything ) ).Return( new List<ResearchStudy>() );

            IResearchStudyBroker researchStudyBroker = MockRepository.GenerateMock<IResearchStudyBroker>();
            var researchStudyBrokerProxy = new ResearchStudyBrokerProxy( researchStudyBroker, cache );

            researchStudyBroker.Expect( ( x => x.AllResearchStudies( Arg<long>.Is.Anything ) ) ).Repeat.Never();
            researchStudyBrokerProxy.AllResearchStudies( 54 );

            researchStudyBroker.VerifyAllExpectations();
        }

        [Test]
        public void TestResearchStudyWith_ShouldNotCallTheUnderlyingBrokerAndShouldUseTheCache()
        {
            List<ResearchStudy> researchStudiesList = new List<ResearchStudy>();
            researchStudiesList.Add( new ResearchStudy( TEST_RESEARCH_STUDY_CODE, TEST_RESEARCH_STUDY_DESCRIPTION, TEST_RESEARCH_STUDY_SPONSOR ) );

            ICache cache = MockRepository.GenerateStub<ICache>();
            cache.Expect( x => x.Get( Arg<string>.Is.Anything ) ).Return( researchStudiesList );

            IResearchStudyBroker researchStudyBroker = MockRepository.GenerateMock<IResearchStudyBroker>();
            var researchStudyBrokerProxy = new ResearchStudyBrokerProxy( researchStudyBroker, cache );

            researchStudyBroker.Expect( ( x => x.ResearchStudyWith( Arg<long>.Is.Anything, Arg<string>.Is.Anything ) ) ).Repeat.Never();
            ResearchStudy study = researchStudyBrokerProxy.ResearchStudyWith( 54, TEST_RESEARCH_STUDY_CODE );

            Assert.IsNotNull( study, "Research Study should not be null." );
            Assert.IsTrue( study.Code == TEST_RESEARCH_STUDY_CODE, "Research Codes do not match." );
            Assert.IsTrue( study.Description == TEST_RESEARCH_STUDY_DESCRIPTION, "Research Descriptions do not match." );
            Assert.IsTrue( study.ResearchSponsor == TEST_RESEARCH_STUDY_SPONSOR, "Research Sponsors do not match." );
        }

        [Test]
        public void TestResearchStudyWithNonExistentCode_ShouldCallTheUnderlyingBroker()
        {
            List<ResearchStudy> researchStudiesList = new List<ResearchStudy>();
            researchStudiesList.Add( new ResearchStudy( DUMMY_CODE, DUMMY_DESCRIPTION, DUMMY_SPONSOR ) );

            ICache cache = MockRepository.GenerateStub<ICache>();
            cache.Expect( x => x.Get( Arg<string>.Is.Anything ) ).Return( researchStudiesList );

            IResearchStudyBroker researchStudyBroker = MockRepository.GenerateMock<IResearchStudyBroker>();
            var researchStudyBrokerProxy = new ResearchStudyBrokerProxy( researchStudyBroker, cache );

            researchStudyBroker.Expect( ( x => x.ResearchStudyWith( Arg<long>.Is.Anything, Arg<string>.Is.Anything ) ) ).Return( new ResearchStudy() ).Repeat.Any();
            researchStudyBrokerProxy.ResearchStudyWith( 54, TEST_RESEARCH_STUDY_CODE );

            researchStudyBroker.VerifyAllExpectations();
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        public void TestResearchStudyWithNullCode()
        {
            ICache cache = MockRepository.GenerateStub<ICache>();

            IResearchStudyBroker researchStudyBroker = MockRepository.GenerateMock<IResearchStudyBroker>();
            var researchStudyBrokerProxy = new ResearchStudyBrokerProxy( researchStudyBroker, cache );

            researchStudyBrokerProxy.ResearchStudyWith( 54, null );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion

        #region Constants
        private const string TEST_RESEARCH_STUDY_CODE = "123456789";
        private const string TEST_RESEARCH_STUDY_DESCRIPTION = "TEST RESEARCH STUDY CODE HOSPITAL";
        private const string TEST_RESEARCH_STUDY_SPONSOR = "TESTER HOSPITAL/CLINIC";

        private const string DUMMY_CODE = "DUMMY121";
        private const string DUMMY_DESCRIPTION = "DUMMY HOSPITAL";
        private const string DUMMY_SPONSOR = "DUMMY SPONSOR";
        #endregion
    }
}
