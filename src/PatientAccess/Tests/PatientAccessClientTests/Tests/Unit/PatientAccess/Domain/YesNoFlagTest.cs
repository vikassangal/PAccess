using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class YesNoFlagTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown YesNoFlagTestsTests
        [TestFixtureSetUp()]
        public static void SetUpYesNoFlagTests()
        {
           
        }

        [TestFixtureTearDown()]
        public static void TearDownYesNoFlagTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestYesNoFlagCreate()
        {
            YesNoFlag blankFlag = new YesNoFlag(CODE_BLANK);
            YesNoFlag trueFlag = new YesNoFlag(CODE_YES);
            YesNoFlag falseFlag = new YesNoFlag(CODE_NO);

            blankFlag.SetYes("Yes");
            Assert.AreEqual(true, trueFlag.Equals(blankFlag));

            blankFlag.SetNo("No");
            Assert.AreEqual(true, falseFlag.Equals(blankFlag));

            blankFlag.SetBlank(" ");
            blankFlag = null;
            Assert.AreEqual(false, trueFlag.Equals(blankFlag));
        }

        [Test()]
        public void TestYesNoFlagClone()
        {
            YesNoFlag blankFlag = new YesNoFlag();
            YesNoFlag dupFlag = (YesNoFlag)blankFlag.Clone();
            Assert.AreEqual(blankFlag.Code, dupFlag.Code);
        }

        [Test()]
        public void TestYesNoFlagHashCode()
        {
            YesNoFlag trueFlag = new YesNoFlag(CODE_YES);
            YesNoFlag falseFlag = new YesNoFlag(CODE_NO);
            Assert.AreNotEqual(trueFlag.GetHashCode(), falseFlag.GetHashCode());
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements

        private const string CODE_YES = "Y";
        private const string CODE_NO = "N";
        private const string CODE_BLANK = " ";
        #endregion
    }
}