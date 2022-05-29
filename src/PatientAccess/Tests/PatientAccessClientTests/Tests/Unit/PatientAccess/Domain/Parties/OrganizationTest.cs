using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class OrganizationTests
    {

        #region SetUp and TearDown OrganizaitonTests
        [TestFixtureSetUp()]
        public static void SetUpOrganizationTests()
        {
            anOrganization = new Organization();
        }

        [TestFixtureTearDown()]
        public static void TearDownOrganizationTests()
        {
            anOrganization = null;
            otherOrganization = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestConstructor()
        {
            anOrganization = new Organization( 1L, ReferenceValue.NEW_VERSION, NAME );
            Assert.AreEqual(
                NAME,
                anOrganization.Name
                );
            Assert.IsFalse(anOrganization.Equals(null));

            ContactPoint cp = new ContactPoint();
            anOrganization.AddContactPoint(cp);

            Relationship r = new Relationship();
            anOrganization.AddRelationship(r); 
            
            otherOrganization = (Organization)anOrganization.Clone();
            Assert.IsFalse(anOrganization.Equals(otherOrganization));
            Assert.AreNotEqual(anOrganization.GetHashCode(), otherOrganization.GetHashCode());           
        }

   
        #endregion

        #region Data Elements
        private static Organization anOrganization = null;
        private static Organization otherOrganization = null;
        const string NAME = "JP Morgan & Chase";
        #endregion
    }
}