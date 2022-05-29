using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class SocialSecurityNumberStatusTests
    {
        #region Constants
        #endregion

        #region Test Methods
        [Test]
        public void TestToString()
        {
            SocialSecurityNumberStatus ssnStatus = SocialSecurityNumberStatus.NewbornSSNStatus;

            string ssnStatusString = ssnStatus.ToString();
            Assert.AreEqual( ssnStatus.Description.ToUpper(), ssnStatusString );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}
