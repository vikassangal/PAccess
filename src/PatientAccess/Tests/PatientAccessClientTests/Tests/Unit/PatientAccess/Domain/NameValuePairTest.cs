using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Auditing.FusNotes;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for NameValuePairTest.
    /// </summary>

    //TODO: Create XML summary comment for NameValuePairTest
    [TestFixture]
    [Category( "Fast" )]
    public class NameValuePairTest
    {
        #region Constants
        private const string KEY = "THISISTHEKEY";
        private const string VAL = "THISISTHEVALUE";
        #endregion

        #region SetUp and TearDown NameValuePairTest
        [TestFixtureSetUp()]
        public static void SetUpNameValuePairTest()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownNameValuePairTest()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestFormatString()
        {
            NameValuePair nvp = new NameValuePair(KEY,VAL);
            string s = nvp.ToString();

            Assert.AreEqual(s,KEY + FusLabel.LABEL_COLON_SPACE + VAL + FusLabel.LABEL_SEMI_COLON_SPACE,
                            "Formated strings do not match");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}