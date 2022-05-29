using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class MBINumberPreferredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsOtherCoverage_ShouldReturnTrue()
        {
            var otherCoverage = new OtherCoverage{ CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsGovernmentMedicareCoverage_PT0_HICNumberIsNullOrEmpty_MBINumberIsNullOrEmpty_ShouldReturnFalse()
        {
            var activity = new PreRegistrationActivity();
            var account = new Account() {Activity = activity};
            var govtMedicareCoverage = new GovernmentMedicareCoverage
            {
                Account = account,
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
               
                MBINumber = string.Empty
            };
            govtMedicareCoverage.Account.KindOfVisit = VisitType.PreRegistration;
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( govtMedicareCoverage ) );
        }
     
        
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverage_FCIsEmpty_ShouldReturnTrue()
        {
            var activity = new PreRegistrationActivity();
            var financialClass = new FinancialClass();
            var account = new Account()
            {
                Activity = activity,
                KindOfVisit = VisitType.PreRegistration,
                FinancialClass = financialClass
            };
            var hicNumber = string.Empty;
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(account, hicNumber, mbiNumber);
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( commericialCoverage ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverage_PT0_FC84_HICNumberIsEmpty_MBINumberIsEmpty_ShouldReturnFalse()
        {
            var activity = new PreRegistrationActivity();
            var financialClass = new FinancialClass { Code = "84" };
            var account = new Account()
            {
                Activity = activity,
                KindOfVisit = VisitType.PreRegistration,
                FinancialClass = financialClass
            };
            var hicNumber = string.Empty;
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(account, hicNumber, mbiNumber);
            commericialCoverage.Account = account; 
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( commericialCoverage ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverage_PT4_FC84_HICNumberIsEmpty_MBINumberIsEmpty_ShouldReturnTrue()
        {
            var activity = new RegistrationActivity();
            var financialClass = new FinancialClass { Code = "84" };
            var account = new Account()
            {
                Activity = activity,
                KindOfVisit = VisitType.Recurring,
                FinancialClass = financialClass
            };
            var hicNumber = string.Empty;
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(account, hicNumber, mbiNumber);
            commericialCoverage.Account.KindOfVisit = VisitType.Outpatient;
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(commericialCoverage));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverage_PTisPreReg_FinacialClassIs87_HicNumberIsNotEmpty_MBINUmberIsEmpty_ShouldReturnFalse()
        {
            var activity = new PreRegistrationActivity();
            var financialClass = new FinancialClass { Code = "87" };
            var account = new Account()
            {
                Activity = activity,
                KindOfVisit = VisitType.PreRegistration,
                FinancialClass = financialClass
            };
           
            const string hicNumber = "12345";
            var mbiNumber = string.Empty;
            var commericialCoverage = GetCommercialCoverage(account, hicNumber, mbiNumber);
            commericialCoverage.Account = account; 
            var ruleUnderTest = new MBINumberPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( commericialCoverage ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenContextIsCommercialCoverage_FC84_HICIsNotEmpty_MBINUmberIsNotEmpty_ShouldReturnTrue()
        {
            var activity = new PreRegistrationActivity();
            var financialClass = new FinancialClass { Code = "84" };
            var account = new Account()
            {
                Activity = activity,
                KindOfVisit = VisitType.PreRegistration,
                FinancialClass = financialClass
            };
            const string hicNumber = "12345";
            var mbiNumber = "1EGKTE5MK45";
            var commericialCoverage = GetCommercialCoverage(account, hicNumber, mbiNumber);
            var ruleUnderTest = new MBINumberPreferred();
            commericialCoverage.Account = account; 
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(commericialCoverage));
        }
         
        #endregion

        #region Support Methods

        private static Coverage GetCommercialCoverage(Account account, string HICNumber, string MBINumber)
        {
            var commericalCoverage = new CommercialCoverage
                {
                    CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                    
                    MBINumber =  MBINumber ,
                    Account = account
                };

            return commericalCoverage;
        }

        #endregion

    }
}
