using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class ShortPreRegistrationWithOfflineActivityTests : ActivityTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitNewbornActivityTests
        [SetUp()]
        public void SetUpAdmitNewbornActivityTests()
        {
            this.UnitUnderTest = new ShortPreRegistrationWithOfflineActivity();
        }

        #endregion

        #region Test Methods

        [Test()]
        public void TestDescriptions()
        {
            // actual
            string resultDesc = UnitUnderTest.Description;
            string resultCntxDesc = UnitUnderTest.ContextDescription;
            // expected
            string lDescription = "Enter Diagnostic Preregistration with Offline Account Number...";
            string lContextDescription = "Enter Diagnostic Preregistration with Offline Account Number...";
            // check reference value 
            Assert.AreEqual( lDescription, resultDesc );
            // check activity-based value
            Assert.AreEqual( lContextDescription, resultCntxDesc );
        }

        [Test()]
        public void TestReadOnly_ShouldBeTrue()
        {
            Assert.IsTrue( UnitUnderTest.ReadOnlyAccount() );
        }

        [Test()]
        public void TestCanCreateNewPatient_ShouldBeTrue()
        {
            Assert.IsTrue( UnitUnderTest.CanCreateNewPatient() );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}
