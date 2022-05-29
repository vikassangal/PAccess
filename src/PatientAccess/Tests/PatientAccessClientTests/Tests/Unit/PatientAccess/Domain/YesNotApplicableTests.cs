using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class YesNotApplicableFlagTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown YesNotApplicableFlagTestsTests
        [TestFixtureSetUp()]
        public static void SetUpYesNotApplicableFlagTests()
        {
           
        }

        [TestFixtureTearDown()]
        public static void TearDownYesNotApplicableFlagTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestYesNotApplicableFlag()
        {
            YesNotApplicableFlag blankFlag = new YesNotApplicableFlag();
            blankFlag.SetBlank();

            YesNotApplicableFlag trueFlag = new YesNotApplicableFlag();
            trueFlag.SetYes();
            YesNotApplicableFlag naFlag = new YesNotApplicableFlag();
            naFlag.SetNotApplicable();
            
            blankFlag.SetYes();
            Assert.AreEqual(
                blankFlag.Code,
                trueFlag.Code) ;
            blankFlag.SetNotApplicable();
            Assert.AreEqual(
                blankFlag.Code,
                naFlag.Code) ;

        }
        [Test()]
        //        [ExpectedException( typeof( ArgumentException ) )]
        public void TestSetCode()
        {
            YesNotApplicableFlag blankFlag = new YesNotApplicableFlag();
            blankFlag.SetBlank();
            blankFlag.Code = "T";
            
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}