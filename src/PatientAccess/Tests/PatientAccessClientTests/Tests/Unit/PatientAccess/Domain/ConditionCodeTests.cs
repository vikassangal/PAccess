using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ConditionCodeTests
    {
        #region Constants
        private const string ADMITTED_AS_IP_FROM_ER = "ADMITTED AS I/P FROM E/R";
        #endregion

        #region SetUp and TearDown ConditionCodeTests
        [TestFixtureSetUp]
        public static void SetUpConditionCodeTests()
        {
        }

        [TestFixtureTearDown]
        public static void TearDownConditionCodeTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestToString()
        {
            var p7Code = new ConditionCode
            {
                Code = ConditionCode.ADMITTED_AS_IP_FROM_ER,
                Description = ADMITTED_AS_IP_FROM_ER
            };
            Assert.IsTrue( "P7 ADMITTED AS I/P FROM E/R" == p7Code.ToString(), "Incorrect ToString value." );
        }

        [Test]
        public void TestCompareTo()
        {
            var p7Code = new ConditionCode
            {
                Code = ConditionCode.ADMITTED_AS_IP_FROM_ER,
                Description = ADMITTED_AS_IP_FROM_ER
            };
            var conditionCode = new ConditionCode
            {
                Code = ConditionCode.ADMITTED_AS_IP_FROM_ER,
                Description = ADMITTED_AS_IP_FROM_ER
            };

            int compare = p7Code.CompareTo( conditionCode );

            Assert.IsTrue( compare == 0, "Condition codes are not the same." );
        }

        [Test]
        public void TestP7IsSystemGeneratedCode()
        {
            var p7Code = new ConditionCode
            {
                Code = ConditionCode.ADMITTED_AS_IP_FROM_ER
            };
            bool isSystemCode = p7Code.IsSystemConditionCode();
            Assert.IsTrue( isSystemCode );
        }

        [Test]
        public void TestIsEmergencyToInpatientTransferConditionCode_WhenCodeIsP7_ShouldReturnTrue()
        {
            var conditionCode = new ConditionCode
            {
                Code = ConditionCode.ADMITTED_AS_IP_FROM_ER,
            };

            Assert.IsTrue( conditionCode.IsEmergencyToInpatientTransferConditionCode() );
        }

        [Test]
        public void TestIsEmergencyToInpatientTransferConditionCode_WhenCodeIsNotP7_ShouldReturnFalse()
        {
            var conditionCode = new ConditionCode
            {
                Code = ConditionCode.CONDITIONCODE_AGE_NO_GHP,
                Description = ADMITTED_AS_IP_FROM_ER
            };

            Assert.IsFalse( conditionCode.IsEmergencyToInpatientTransferConditionCode() );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}