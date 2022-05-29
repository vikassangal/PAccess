using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class PreAdmitNewbornActivityTests : ActivityTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitNewbornActivityTests
        [SetUp()]
        public void SetUpAdmitNewbornActivityTests()
        {
            this.UnitUnderTest = new PreAdmitNewbornActivity();
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
            string lDescription = "Pre-Admit Newborn";
            string lContextDescription = "Pre-Admit Newborn";
            // check reference value 
            Assert.AreEqual( lDescription, resultDesc );
            // check activity-based value
            Assert.AreEqual( lContextDescription, resultCntxDesc );
        }

        
        [Test()]
        public void TestReadOnly_ShouldBeFalse()
        {
            Assert.IsFalse( UnitUnderTest.ReadOnlyAccount() );
        }

        [Test()]
        public void TestCanCreateNewPatient_ShouldBeFalse()
        {
            Assert.IsFalse( UnitUnderTest.CanCreateNewPatient() );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}
