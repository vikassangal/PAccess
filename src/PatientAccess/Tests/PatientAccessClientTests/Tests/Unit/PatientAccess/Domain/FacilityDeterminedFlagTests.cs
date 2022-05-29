using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class FacilityDeterminedFlagTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown FacilityDeterminedFlagTests
        [TestFixtureSetUp()]
        public static void SetUpFacilityDeterminedFlagTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownFacilityDeterminedFlagTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testFacilityDeterminedFlag()
        {
           
            FacilityDeterminedFlag flag = new FacilityDeterminedFlag(3,ReferenceValue.NEW_VERSION,"FacilityDeterminedFlag","flag");
            
            Assert.AreEqual(
                typeof(FacilityDeterminedFlag),
                flag.GetType()
                );
                 
            Assert.AreEqual(
                "FacilityDeterminedFlag",
                flag.Description
                );
            Assert.AreEqual(
                "flag",
                flag.Code
                );
            Assert.AreEqual(
                3,
                flag.Oid
                );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}