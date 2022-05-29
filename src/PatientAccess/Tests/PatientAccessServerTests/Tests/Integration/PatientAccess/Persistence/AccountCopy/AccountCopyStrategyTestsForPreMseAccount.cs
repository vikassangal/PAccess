using System;
using Extensions.PersistenceCommon;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Persistence.AccountCopy;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.AccountCopy
{
    [TestFixture]
    public class AccountCopyStrategyTestsForPreMseAccount
    {
        #region Constants

        private const long FacilityID = 900;
        private readonly DateTime AdmitDate = DateTime.Parse("01-30-2009");
        private const string PRE_MSE_INSURANCE_PLAN_ID = "EDL81";
        #endregion

        #region SetUp and TearDown AccountCopyStrategyTestsForPreMseAccount
        [TestFixtureSetUp()]
        public static void SetUpAccountCopyStrategyTestsForPreMseAccount()
        {
            insuranceBroker = BrokerFactory.BrokerOfType<IInsuranceBroker>();
        }

        [TestFixtureTearDown()]
        public static void TearDownAccountCopyStrategyTestsForPreMseAccount()
        {
        }
        #endregion

        #region Test Methods

        [Test]
        public void TestEditInsuranceUsing_WhenOldAccountFCis37_NewAccountCoveragesShouldBeReset()
        {
            Account oldAccount = new Account();
            InsurancePlan planSelfPay = insuranceBroker.PlanWith( PRE_MSE_INSURANCE_PLAN_ID, FacilityID, AdmitDate );
            SelfPayCoverage coverage = new SelfPayCoverage();
            coverage.InsurancePlan = planSelfPay;
            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            coverage.CoverageOrder = primary;
            Insurance insurance = new Insurance();
            insurance.AddCoverage(coverage);
            FinancialClass finClass = new FinancialClass( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "SELFPAY",
                                                         FinancialClass.MED_SCREEN_EXM_CODE );
            VisitType kindOfVisit = new VisitType( 0, PersistentModel.NEW_VERSION, VisitType.EMERGENCY_PATIENT_DESC, VisitType.EMERGENCY_PATIENT );
            oldAccount.Insurance = insurance;
            oldAccount.FinancialClass = finClass;
            oldAccount.KindOfVisit = kindOfVisit;
            oldAccount.Activity = new RegistrationActivity();
            AccountCopyStrategy copyStrategy = new BinaryCloneAccountCopyStrategy();
            Account newAccount = copyStrategy.CopyAccount( oldAccount );

            Assert.IsNotNull( newAccount.Insurance );
            Assert.IsTrue(newAccount.Insurance.Coverages.Count == 0);
            Assert.IsTrue(newAccount.FinancialClass.Code == string.Empty);
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static IInsuranceBroker insuranceBroker = null;
        #endregion
    }
}
