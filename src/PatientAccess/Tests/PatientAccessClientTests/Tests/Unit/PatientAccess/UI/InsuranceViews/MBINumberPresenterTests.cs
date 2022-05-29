using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.InsuranceViews.Presenters;
using PatientAccess.UI.InsuranceViews.Views;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for MBINumberPresenterTests
    /// </summary>
    [TestFixture]
    public class MBINumberPresenterTests
    {
        [Test]
        public void TestCommercialCoverageMBINumber_SavedToAccount()
        {
            var mockery = new MockRepository();
            var mbiView = mockery.Stub<IMBINumberView>();

            var financialClass = new FinancialClass { Code = "84" };
            var coverage = GetCommercialCoverage(financialClass);
            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            mbiView.MBINumber = VALID_MBINUMBER;
            presenter.UpdateModel();
            var coverageMBINumber = ((CoverageForCommercialOther)coverage).MBINumber;
            Assert.IsTrue(coverageMBINumber == VALID_MBINUMBER,
                "valid MBI number should be saved in the commercial coverage");
        }
        [Test]
        public void TestGovernmentMedicareCoverageMBINumber_SavedToAccount()
        {
            var mockery = new MockRepository(); 
            var mbiView = mockery.Stub<IMBINumberView>();

            var coverage = GetGovernmentMedicareCoverage();

            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            mbiView.MBINumber = VALID_MBINUMBER;
            presenter.UpdateModel();
            var coverageMBINumber = ((GovernmentMedicareCoverage)coverage).MBINumber;
            Assert.IsTrue(coverageMBINumber == VALID_MBINUMBER,
                "valid MBI number should be saved in the commercial coverage");
        }
        [Test]
        public void TestCommercialCoverage_ValidateNumber()
        {
            var mockery = new MockRepository(); 
            var mbiView = mockery.Stub<IMBINumberView>();

            var financialClass = new FinancialClass { Code = "84" };
            var coverage = GetCommercialCoverage(financialClass);

            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            mbiView.MBINumber = VALID_MBINUMBER;
            var validMBI = presenter.ValidateMBINUmber();
            Assert.IsTrue(validMBI,
                   "MBI number should be valid");
        }
        [Test]
        public void TestCommercialCoverage_InValidateNumber()
        {
            var mockery = new MockRepository(); 
            var mbiView = mockery.Stub<IMBINumberView>();

            var financialClass = new FinancialClass { Code = "84" };
            var coverage = GetCommercialCoverage(financialClass);

            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            mbiView.MBINumber = INVALID_MBINUMBER;
            var validMBI = presenter.ValidateMBINUmber();
            Assert.IsFalse(validMBI,
                   "vMBI number should be valid");
        }
        [Test]
        public void TestCommercialCoverage_SetMBINumberStatusForFinancialClass_ValidFC_MBINumberShouldBeEnabled()
        {
            var mockery = new MockRepository();
            var mbiView = mockery.Stub<IMBINumberView>();
            var financialClass = new FinancialClass { Code = "84" };
            mbiView.Account = new Account() { FinancialClass = financialClass };
            var coverage = GetCommercialCoverage(financialClass);
            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
            coverage, RuleEngine);
            mockery.ReplayAll();
            presenter.SetMBINumberStateForFinancialClass();
            mbiView.AssertWasCalled(x => x.EnbleMBINumber());
        }
        [Test]
        public void TestCommercialCoverage_SetMBINumberStatusForFinancialClass_InValidFC_MBINumberShouldBeEnabled()
        {
            var mockery = new MockRepository(); 
            var mbiView = mockery.Stub<IMBINumberView>();

            var financialClass = new FinancialClass {Code = "14"};
            mbiView.Account = new Account() {FinancialClass = financialClass};
            var coverage = GetCommercialCoverage(financialClass);

            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
            coverage, RuleEngine);
            mockery.ReplayAll();
            presenter.SetMBINumberStateForFinancialClass();
            mbiView.AssertWasCalled(x => x.DisableMBINumber());
        }
        [Test]
        public void TestCommercialCoverage_UpdateView()
        {
            var mockery = new MockRepository();
            var mbiView = mockery.Stub<IMBINumberView>();

            var financialClass = new FinancialClass { Code = "14" };
            var coverage = GetCommercialCoverage(financialClass);
            ((CoverageForCommercialOther)coverage).MBINumber = VALID_MBINUMBER;
            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            presenter.UpdateView();
            Assert.IsTrue(mbiView.MBINumber == VALID_MBINUMBER, " View should be populated with MBI number knk coverage");
        }
        [Test]
        public void TestGovernmentMedicareCoverage_UpdateView()
        {
            var mockery = new MockRepository();
            var mbiView = mockery.Stub<IMBINumberView>();
            var financialClass = new FinancialClass { Code = "14" };
            var coverage = GetGovernmentMedicareCoverage();
            ((GovernmentMedicareCoverage)coverage).MBINumber = VALID_MBINUMBER;
            var presenter = new MBINumberPresenter(mbiView, new MessageBoxAdapter(),
                coverage, RuleEngine);
            presenter.UpdateView();
            Assert.IsTrue(mbiView.MBINumber == VALID_MBINUMBER, " View should be populated with MBI number knk coverage");
        }
        private static Coverage GetCommercialCoverage(FinancialClass fc)
        {
            var commericalCoverage = new CommercialCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Account = new Account { FinancialClass = fc }
            };

            return commericalCoverage;
        }
        private static Coverage GetGovernmentMedicareCoverage()
        {
            var governmentMedicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION)
            };
            return governmentMedicareCoverage;
        }

        private const string VALID_MBINUMBER = "1EG4TE5MK73";
        private const string INVALID_MBINUMBER = "ABCDEFGHIJKLM";
        # region Private Properties
        private RuleEngine i_RuleEngine;

        private RuleEngine RuleEngine
        {
            get
            {
                if (i_RuleEngine == null)
                {
                    i_RuleEngine = RuleEngine.GetInstance();
                }
                return i_RuleEngine;
            }
        }
        #endregion
    }
}
