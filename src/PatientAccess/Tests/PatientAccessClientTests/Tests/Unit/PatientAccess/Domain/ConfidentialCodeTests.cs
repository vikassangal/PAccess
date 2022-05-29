using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ConfidentialCodeTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ConfidentialCodeTests
        [TestFixtureSetUp()]
        public static void SetUpConfidentialCodeTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownConfidentialCodeTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConfidentialCode()
        {
           
            ConfidentialCode cCode = new ConfidentialCode(6,ReferenceValue.NEW_VERSION,"Confidential","C");
            
            Assert.AreEqual(
                typeof(ConfidentialCode),
                cCode.GetType()
                );
                 
            Assert.AreEqual(
                "Confidential",
                cCode.Description
                );
            Assert.AreEqual(
                "C",
                cCode.Code
                );
            Assert.AreEqual(
                6,
                cCode.Oid
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}