using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class GovernmentMedicareCoverageTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown GovernmentMedicareCoverageTests
        [TestFixtureSetUp()]
        public static void SetUpGovernmentMedicareCoverageTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownGovernmentMedicareCoverageTests()
        {
        }

        [SetUp()]
        public void SetUpGovernmentMedicareCoverage()
        {
            BuildCoverage();
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testGovernmentMedicareCoverage()
        {
            Assert.AreEqual(
                i_Coverage.MBINumber,
                "73462"
                );
                 
            Assert.AreEqual(
                i_Coverage.PartBCoverage.Code ,
                "Y"
                );
           
        }

        [Test()]
        public void TestNullConstraints( )
        {
            i_Coverage.SetCoverageConstraints( null );
            Assert.IsTrue( i_Coverage.RemainingCoInsurance == 1000 );
        }

        [Test()]
        public void TestConstraintsExpectedValue( )
        {

            MedicareConstraints constraints = new MedicareConstraints( );
            
            constraints.PartACoverage.SetYes( );
            constraints.PartBCoverage.SetNo( );
            constraints.PartACoverageEffectiveDate = DateTime.MinValue;
            constraints.PatientHasMedicareHMOCoverage.SetYes( );
            constraints.DateOfLastBillingActivity = DateTime.MinValue;
            constraints.RemainingBenefitPeriod = 50;
            constraints.RemainingLifeTimeReserve = 200;
            constraints.RemainingCoInsurance = 30;
            constraints.RemainingSNF = 11;
            constraints.RemainingSNFCoInsurance = 600;
            constraints.PatientIsPartOfHospiceProgram.SetYes( );
            constraints.RemainingPartADeductible = 0;
            constraints.RemainingPartBDeductible = 0;
            
            i_Coverage.SetCoverageConstraints( constraints );
            Assert.AreEqual( i_Coverage.PartACoverage.Code, "Y" );
            Assert.AreEqual( i_Coverage.PartBCoverage.Code, "N" );            
            Assert.AreEqual( i_Coverage.PartACoverageEffectiveDate, DateTime.MinValue );
            Assert.AreEqual( i_Coverage.PartBCoverageEffectiveDate, DateTime.MinValue );
            Assert.AreEqual( i_Coverage.PatientHasMedicareHMOCoverage.Code, "Y" );
            Assert.AreEqual( i_Coverage.DateOfLastBillingActivity, DateTime.MinValue );
            Assert.AreEqual( i_Coverage.RemainingBenefitPeriod, 50 );
            Assert.AreEqual( i_Coverage.RemainingLifeTimeReserve, 200 );
            Assert.AreEqual( i_Coverage.RemainingCoInsurance, 30 );
            Assert.AreEqual( i_Coverage.RemainingSNF, 11 );
            Assert.AreEqual( i_Coverage.RemainingSNFCoInsurance, 600 );
            Assert.AreEqual( i_Coverage.PatientIsPartOfHospiceProgram.Code, "Y" );
            Assert.AreEqual(i_Coverage.RemainingPartADeductible, 0);
            Assert.AreEqual(i_Coverage.RemainingPartBDeductible, 0);
        }

        #endregion

        #region Support Methods

        private static void BuildCoverage( )
        {
            YesNoFlag falseFlag = new YesNoFlag( );
            falseFlag.SetNo( );
            YesNoFlag trueFlag = new YesNoFlag( );
            trueFlag.SetYes( );
            i_Coverage = new GovernmentMedicareCoverage( );
            i_Coverage.MBINumber = "73462";
            i_Coverage.PartACoverage = falseFlag;
            i_Coverage.PartBCoverage = trueFlag;
            i_Coverage.PartACoverageEffectiveDate = DateTime.Parse( "Aug 20, 2005" );
            i_Coverage.PartBCoverageEffectiveDate = DateTime.Parse( "Aug 19, 2005" );
            i_Coverage.PatientHasMedicareHMOCoverage = falseFlag;
            i_Coverage.MedicareIsSecondary = trueFlag;
            i_Coverage.DateOfLastBillingActivity = DateTime.Parse( "Aug 21, 2005" );
            i_Coverage.RemainingBenefitPeriod = 365;
            i_Coverage.RemainingCoInsurance = 1000;
            i_Coverage.RemainingLifeTimeReserve = 1000;
            i_Coverage.RemainingSNF = 100;
            i_Coverage.RemainingSNFCoInsurance = 10;
            i_Coverage.PatientIsPartOfHospiceProgram = falseFlag;
            i_Coverage.VerifiedBeneficiaryName = trueFlag;
            
            
        }

        #endregion

        #region Data Elements
        private static GovernmentMedicareCoverage i_Coverage = null;
        #endregion
    }
}