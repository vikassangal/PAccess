
using NUnit.Framework; 
using PatientAccess.Domain; 
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class MSPQuestionaireRequiredTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            i_MSPQuestionaireRequired = new MSPQuestionaireRequired();
            
            primary = new CoverageOrder(1, "Primary");
            secondary = new CoverageOrder(2, "Secondary");
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullInsurance_ForRegistrationActivity_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), null );
            var actualResult = i_MSPQuestionaireRequired.CanBeAppliedTo(account);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullInsurance_ForPreRegistrationActivity_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), null);
            var actualResult = i_MSPQuestionaireRequired.CanBeAppliedTo(account);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullInsurance_NewBornRegistrationActivity_ShouldReturnTrue()
        {
            var account = GetAccount(new AdmitNewbornActivity(), null);
            var actualResult = i_MSPQuestionaireRequired.CanBeAppliedTo(account);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestInsuranceIsMedicareToPrimaryInsurance_Medicare_VE304_ShouldReturnTrue()
        {
           var medicareCov = MedicareCoverageWithVE3(primary);
            CommercialCoverage coverage1 = CommercialCoverage(secondary);

            Insurance insurance = new Insurance();

            insurance.AddCoverage(medicareCov);
            insurance.AddCoverage(coverage1);

            Assert.IsTrue(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), medicareCov));
        }
        [Test]
        public void TestInsuranceIsMedicareToSecondaryInsurance_Medicare_VE304_ShouldReturnTrue()
        {
            var medicareCov = MedicareCoverageWithVE3(secondary);
            CommercialCoverage coverage1 = CommercialCoverage(primary);

            Insurance insurance = new Insurance();

            insurance.AddCoverage(medicareCov);
            insurance.AddCoverage(coverage1);

            Assert.IsTrue(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), medicareCov));
        }
        [Test]
        public void TestInsuranceIsMedicareToPrimaryInsurance_NonMedicare_ShouldReturnFalse()
        {
            
            CommercialCoverage coverage1 = CommercialCoverage(primary);
              

            Assert.IsFalse(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), coverage1));
        }
        [Test]
        public void TestInsuranceIsMedicareToPrimaryInsurance_Medicare_WithVE2ShouldReturnFalse()
        {

            GovernmentMedicareCoverage coverage1 = MedicareCoverageWithVEX(primary);


            Assert.IsFalse(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), coverage1));
        }
        [Test]
        public void TestInsuranceIsMedicareToSecondaryInsurance_Medicare_WithVE2ShouldReturnFalse()
        {

            GovernmentMedicareCoverage coverage1 = MedicareCoverageWithVEX(secondary);


            Assert.IsFalse(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), coverage1));
        }
        [Test]
        public void TestInsuranceIsMedicareToPrimaryInsurance_Medicare_WithOutVE_ShouldReturnTrue()
        {

            GovernmentMedicareCoverage coverage1 = MedicareCoverageWithOutVE(primary);


            Assert.IsTrue(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), coverage1));
        }
        [Test]
        public void TestInsuranceIsMedicareToSecondaryInsurance_Medicare_WithOutVE_ShouldReturnTrue()
        {

            GovernmentMedicareCoverage coverage1 = MedicareCoverageWithOutVE(secondary);


            Assert.IsTrue(i_MSPQuestionaireRequired.InsuranceIsMedicare(new FinancialClass(), new VisitType(),
                new HospitalService(), coverage1));
        }

        [Test]
        public void TestCanBeAppliedTo_PrimaryInsurance_IsMedicareVE3_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), null);
            var medicareCov = MedicareCoverageWithVE3(primary);
            CommercialCoverage coverage1 = CommercialCoverage(secondary);

            Insurance insurance = new Insurance();

            insurance.AddCoverage(medicareCov);
            insurance.AddCoverage(coverage1);

            var actualResult = i_MSPQuestionaireRequired.CanBeAppliedTo(account);
            Assert.IsTrue(actualResult);
        }

        #region Support Methods
        private static Account GetAccount(Activity activity, Insurance insurance)
        {
            return new Account
            {
                Activity = activity, 
                Insurance = insurance
            };
        }
        private static GovernmentMedicareCoverage MedicareCoverageWithVE3(CoverageOrder order)
        {
            GovernmentMedicareCoverage medicareCov = new GovernmentMedicareCoverage()
            {
                InsurancePlan =
                    new GovernmentMedicareInsurancePlan() { Payor = new Payor() { Code = "VE" }, PlanSuffix = "304" },
                CoverageOrder = order

            };

            return medicareCov;
        }
        private static GovernmentMedicareCoverage MedicareCoverageWithVEX(CoverageOrder order)
        {
            GovernmentMedicareCoverage medicareCov = new GovernmentMedicareCoverage()
            {
                InsurancePlan =
                    new GovernmentMedicareInsurancePlan() { Payor = new Payor() { Code = "VE" }, PlanSuffix = "204" },
                CoverageOrder = order
            };

            return medicareCov;
        }
        private static GovernmentMedicareCoverage MedicareCoverageWithOutVE(CoverageOrder order)
        {
            GovernmentMedicareCoverage medicareCov = new GovernmentMedicareCoverage()
            {
                InsurancePlan =
                    new GovernmentMedicareInsurancePlan() { Payor = new Payor() { Code = "53" }, PlanSuffix = "544" },
                CoverageOrder = order
            };

            return medicareCov;
        }
        private static CommercialCoverage CommercialCoverage(CoverageOrder order)
        {
            CommercialCoverage commercialCov = new CommercialCoverage()
            {
                InsurancePlan =
                    new CommercialInsurancePlan() { Payor = new Payor() { Code = "EN" }, PlanSuffix = "901" },
                CoverageOrder = order
            };

            return commercialCov;
        }

        #endregion

        #region constants

        private MSPQuestionaireRequired i_MSPQuestionaireRequired;

        #endregion
        #region Data Elements 
        CoverageOrder primary;
        CoverageOrder secondary;
        #endregion
    }

}
