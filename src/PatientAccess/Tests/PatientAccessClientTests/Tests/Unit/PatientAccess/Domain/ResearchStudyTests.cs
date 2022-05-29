using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ResearchStudyTests
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
        public void TestResearchStudy()
        {
            ResearchStudy study1 = new ResearchStudy("RESEARCH_1", "RESEARCH_DESCRIPTION_1");
            ResearchStudy study2 = new ResearchStudy("RESEARCH_2", "RESEARCH_DESCRIPTION_2", "SPONSOR_2");
            ResearchStudy study3 = new ResearchStudy("RESEARCH_3", "RESEARCH_DESCRIPTION_3", "SPONSOR_3", "33333333");

            Assert.IsNotNull(study1);
            Assert.AreEqual("RESEARCH_1", study1.Code, "Research Study1 code does not match.");
            Assert.AreEqual("RESEARCH_DESCRIPTION_1", study1.Description, "Research Study1 Description does not match.");
            Assert.AreEqual(String.Empty, study1.ResearchSponsor, "Research Sponsor is not empty");

            Assert.IsNotNull(study2);
            Assert.AreEqual("RESEARCH_2", study2.Code, "Research Study1 code does not match.");
            Assert.AreEqual("RESEARCH_DESCRIPTION_2", study2.Description, "Research Study1 Description does not match.");
            Assert.AreEqual("SPONSOR_2", study2.ResearchSponsor, "Research Sponsor value does not match.");

            Assert.IsNotNull(study3);
            Assert.AreEqual("RESEARCH_3", study3.Code, "Research Study3 code does not match.");
            Assert.AreEqual("RESEARCH_DESCRIPTION_3", study3.Description, "Research Study3 Description does not match.");
            Assert.AreEqual("SPONSOR_3", study3.ResearchSponsor, "Research Sponsor 3 value does not match.");
            Assert.AreEqual("33333333", study3.RegistryNumber, "Registry number 3 value does not match.");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}
