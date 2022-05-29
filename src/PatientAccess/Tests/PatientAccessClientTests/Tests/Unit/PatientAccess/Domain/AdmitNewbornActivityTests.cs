using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class AdmitNewbornActivityTests : ActivityTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown AdmitNewbornActivityTests
        [SetUp()]
        public void SetUpAdmitNewbornActivityTests()
        {
            this.UnitUnderTest = new AdmitNewbornActivity();
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestAdmitNewbornActivity()
        {
            AdmitNewbornActivity a = new AdmitNewbornActivity() ;
            // actual
            string resultDesc = UnitUnderTest.Description ;
            string resultCntxDesc = UnitUnderTest.ContextDescription ;
            // expected
            string lDescription = "Register &Newborn";
            string lContextDescription = "Register Newborn";
            // check reference value 
            Assert.AreEqual( lDescription, resultDesc ) ;
            // check activity-based value
            Assert.AreEqual( lContextDescription, resultCntxDesc ) ;
        }

        [Test()]
        public void TestAdmitNewbornWithOfflineActivity()
        {
            AdmitNewbornWithOfflineActivity a = new AdmitNewbornWithOfflineActivity();
            // actual
            string resultDesc = a.Description;
            string resultCntxDesc = a.ContextDescription;
            // expected
            string lDescription = "Enter Ne&wborn Registration with Offline Account Number...";
            string lContextDescription = "Enter Newborn Registration with Offline Account Number...";
            // check reference value 
            Assert.AreEqual( lDescription, resultDesc ) ;
            // check activity-based value
            Assert.AreEqual( lContextDescription, resultCntxDesc ) ;
        }

        [Test()]
        public void TestReadOnly()
        {
            // almost redundant testing since the returned value is hardcoded, but increase code coverage
            AdmitNewbornActivity a = new AdmitNewbornActivity() ;
            Assert.IsFalse( a.ReadOnlyAccount() ) ;

            AdmitNewbornWithOfflineActivity aoff = new AdmitNewbornWithOfflineActivity();
            Assert.IsTrue( aoff.ReadOnlyAccount() ) ; 
        }

        [Test()]
        public void TestCanCreateNewPatient()
        {
            // almost redundant testing since the returned value is hardcoded, but increase code coverage
            AdmitNewbornActivity a = new AdmitNewbornActivity();
            Assert.IsFalse( a.CanCreateNewPatient() ) ;

            AdmitNewbornWithOfflineActivity aoff = new AdmitNewbornWithOfflineActivity();
            Assert.IsTrue(aoff.CanCreateNewPatient());
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion

    }
}