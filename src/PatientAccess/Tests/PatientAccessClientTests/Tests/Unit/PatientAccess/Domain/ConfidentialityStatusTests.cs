using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ConfidentialityStatusTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown ConfidentialityStatusTests
        [TestFixtureSetUp()]
        public static void SetUpConfidentialityStatusTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownConfidentialityStatusTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testConfidentialityStatus()
        {
           
            ConfidentialityStatus cs = new ConfidentialityStatus(3,ReferenceValue.NEW_VERSION,"Incomplete","I");
            
            Assert.AreEqual(
                typeof(ConfidentialityStatus),
                cs.GetType()
                );
                 
            Assert.AreEqual(
                "Incomplete",
                cs.Description
                );
            Assert.AreEqual(
                "I",
                cs.Code
                );
            Assert.AreEqual(
                3,
                cs.Oid
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}