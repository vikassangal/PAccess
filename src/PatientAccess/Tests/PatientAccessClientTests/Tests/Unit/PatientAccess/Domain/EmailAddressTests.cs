using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmailAddressTests
    {
        #region Constants
        const string
            VALID_EMAIL_ADDRESS1    = "james.severance@ps.net",
            VALID_EMAIL_ADDRESS2    = "severajp@ps.net",
            INVALID_EMAIL_ADDRESS1  = "james.severance@ps",
            INVALID_EMAIL_ADDRESS2  = "james",
            INVALID_EMAIL_ADDRESS3  = "@PS.NET";
        #endregion

        #region SetUp and TearDown EmailAddressTests
        [TestFixtureSetUp()]
        public static void SetUpEmailAddressTests()
        {
            //this.WorkEmailType = new ReferenceValue( 0L, DateTime.Now, "Work" );
        }

        [TestFixtureTearDown()]
        public static void TearDownEmailAddressTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestValidEmailAddresses()
        {
            EmailAddress email1 = new EmailAddress( VALID_EMAIL_ADDRESS1 );
            EmailAddress email2 = new EmailAddress( VALID_EMAIL_ADDRESS2 );

            Assert.AreEqual(
                VALID_EMAIL_ADDRESS1,
                email1.ToString()
                );

            Assert.AreEqual(
                VALID_EMAIL_ADDRESS1,
                email1.Uri
                );

            Assert.AreEqual(
                VALID_EMAIL_ADDRESS2,
                email2.ToString()
                );

            Assert.AreEqual(
                VALID_EMAIL_ADDRESS2,
                email2.Uri
                );
        }
        
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}