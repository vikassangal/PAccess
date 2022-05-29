using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ConsentedResearchStudyTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AccidentTests
        [TestFixtureSetUp]
        public static void SetUpAccidentTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownAccidentTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestConsentedResearchStudy()
        {
            ResearchStudy study1 = new ResearchStudy("TEST001", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1", "12345678");

            ConsentedResearchStudy research1 = new ConsentedResearchStudy(study1, YesNoFlag.Yes);

            Assert.IsNotNull(research1);
            Assert.AreEqual("TEST001", research1.ResearchStudy.Code, "Consented Research study code does not match.");
            Assert.AreEqual("RESEARCH STUDY 1", research1.ResearchStudy.Description, "Consented Research Description does not match.");
            Assert.AreEqual("RESEARCH SPONSOR 1", research1.ResearchStudy.ResearchSponsor, "Consented Research Sponsor does not match.");
            Assert.AreEqual(YesNoFlag.Yes, research1.ProofOfConsent, "Consented Research Proof of Consent does not match.");
            Assert.AreEqual("12345678", research1.RegistryNumber, "Consented Research Registry number does not match.");
        }

        [Test]
        public void TestConsentedResearchStudyDefaultConstructor()
        {
            ConsentedResearchStudy research = new ConsentedResearchStudy();

            Assert.IsNotNull(research);
            Assert.AreEqual(String.Empty, research.ResearchStudy.Code, "Consented Research study code not empty.");
            Assert.AreEqual(String.Empty, research.ResearchStudy.Description, "Consented Research Description not empty.");
            Assert.AreEqual(String.Empty, research.ResearchStudy.ResearchSponsor, "Consented Research Sponsor not empty.");
            Assert.AreEqual(String.Empty, research.ResearchStudy.RegistryNumber, "Consented Research Registry number not empty.");
            Assert.AreEqual(YesNoFlag.No, research.ProofOfConsent, "Consented Research Proof of Consent does not match.");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}
