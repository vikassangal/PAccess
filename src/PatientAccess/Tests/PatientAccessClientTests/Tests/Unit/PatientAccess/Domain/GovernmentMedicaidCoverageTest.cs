using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentMedicaidCoverageTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentMedicaidCoverageTests
        [TestFixtureSetUp()]
        public static void SetUpGovernmentMedicaidCoverageTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentMedicaidCoverageTests()
        {
        }

        [SetUp()]
        public void SetUpGovernmentCoverage()
        {
            BuildCoverage( );
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testGovernmentMedicaidCoverage()
        {

            Assert.AreEqual(
                i_Coverage.PolicyCINNumber,
                "8347384"
                );
                 
            Assert.AreEqual(
                i_Coverage.IssueDate,
                DateTime.Parse("Aug 21, 2005")
                );
        }

        [Test()]
        public void TestNullCoverage( )
        {
            i_Coverage.SetCoverageConstraints( null );
            Assert.AreEqual( i_Coverage.MedicaidCopay, 100 );
        }

        [Test()]
        public void TestContraintsExpectedValue( )
        {
            MedicaidConstraints constraint = new MedicaidConstraints( );
            constraint.MedicaidCopay = 25;
            constraint.EVCNumber = "1010";
            constraint.EligibilityDate = DateTime.MinValue;
            i_Coverage.SetCoverageConstraints( constraint );
            Assert.AreEqual( i_Coverage.MedicaidCopay, 25 );
            Assert.AreEqual( i_Coverage.EVCNumber, "1010" );
            Assert.AreEqual( i_Coverage.EligibilityDate, DateTime.MinValue );
        }

        #endregion

        #region Support Methods
        private static void BuildCoverage( )
        {
            YesNoFlag flag = new YesNoFlag( );
            flag.SetNo( );
            i_Coverage = new GovernmentMedicaidCoverage( );

            i_Coverage.IssueDate = DateTime.Parse( "Aug 21, 2005" );
            i_Coverage.EVCNumber = "8373";
            i_Coverage.PolicyCINNumber = "8347384";
            i_Coverage.EligibilityDate = DateTime.Parse( "Aug 21, 2005" );
            i_Coverage.PatienthasMedicare = flag;
            i_Coverage.PatienthasOtherInsuranceCoverage = flag;
            i_Coverage.MedicaidCopay = 100.00f;
            
        }
        #endregion

        #region Data Elements
        private static GovernmentMedicaidCoverage i_Coverage = null;
        #endregion
    }
}