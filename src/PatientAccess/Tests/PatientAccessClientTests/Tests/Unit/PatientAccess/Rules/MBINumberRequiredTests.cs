using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class MBINumberRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsOtherCoverage_ShouldReturnTrue()
        {
            var otherCoverage = new OtherCoverage{ CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new MBINumberRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsGovernmentMedicareCoverageAndHICNumberIsNullOrEmpty_MBINumberIsNullOrEmpty_ShouldReturnFalse()
        {
            var govtMedicareCoverage = new GovernmentMedicareCoverage
                {
                    CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                    
                    MBINumber = string.Empty
                };
            var ruleUnderTest = new MBINumberRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( govtMedicareCoverage ) );
        }
     
        
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverageFinacialClassIsEmpty_ShouldReturnTrue()
        {
            var financialClass = new FinancialClass { Code = string.Empty };
            var hicNumber = string.Empty;
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(financialClass, hicNumber, mbiNumber);
            var ruleUnderTest = new MBINumberRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( commericialCoverage ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverageFinacialClassIs84HicNumberIsEmptyMBINumberIsEmpty_ShouldReturnFalse()
        {
            var financialClass = new FinancialClass { Code = "84" };
            var hicNumber = string.Empty;
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(financialClass, hicNumber, mbiNumber);
            var ruleUnderTest = new MBINumberRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( commericialCoverage ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverageFinacialClassIs84HicNumberIsNotEmpty_MBINUmberIsNotEmpty_ShouldReturnTrue()
        {
            var financialClass = new FinancialClass { Code = "84" };
            const string hicNumber = "12345";
            var mbiNumber = "1EGKTE5MK45";
            var commericialCoverage = GetCommercialCoverage(financialClass, hicNumber, mbiNumber);
            var ruleUnderTest = new MBINumberRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(commericialCoverage));
        }
         
        #endregion

        #region Support Methods

        private static Coverage GetCommercialCoverage( FinancialClass fc, string HICNumber, string MBINumber )
        {
            var commericalCoverage = new CommercialCoverage
                {
                    CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                   
                    MBINumber =  MBINumber ,
                    Account = new Account {FinancialClass = fc}
                };

            return commericalCoverage;
        }

        #endregion

    }
}
