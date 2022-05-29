using PatientAccess.Domain;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class NoMedicarePrimaryPayorForAutoAccidentTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsNotMedicareOrMedicaidAndActivityIsRegistration_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            insurance.AddCoverage( otherCoverage );
            var account = new Account { Activity = new RegistrationActivity(), Insurance = insurance };

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsMedicareAndActivityIsRegistrationAndOccurrenceCodeIsIllness_ShouldReturnTrue()
        {
            var medicareCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };

            var insurance = new Insurance();
            insurance.AddCoverage( medicareCoverage );
            var account = new Account { Activity = new RegistrationActivity(), Insurance = insurance };
            var illness = new Illness();
            account.Diagnosis.Condition = illness;

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsMedicareAndActivityIsAdmitNewbornAndOccurrenceCodeIsIllness_ShouldReturnTrue()
        {
            var medicareCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            insurance.AddCoverage( medicareCoverage );

            var account = new Account { Activity = new AdmitNewbornActivity(), Insurance = insurance };

            var illness = new Illness();
            account.Diagnosis.Condition = illness;

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsMedicareAndActivityIsRegistrationAndOccurrenceCodeIsAutoAccident_ShouldReturnFalse()
        {
            var medicareCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            insurance.AddCoverage( medicareCoverage );

            var account = new Account { Activity = new RegistrationActivity(), Insurance = insurance };

            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            account.Diagnosis.Condition = accident;

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsNotMedicareOrMedicaidAndActivityIsPreAdmitNewborn_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            insurance.AddCoverage( otherCoverage );
            var account = new Account { Activity = new PreAdmitNewbornActivity(), Insurance = insurance };

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenCoverageIsMedicareAndActivityIsPreAdmitNewbornAndOccurrenceCodeIsAutoAccident_ShouldReturnFalse()
        {
            var medicareCoverage = new GovernmentMedicareCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var insurance = new Insurance();
            insurance.AddCoverage( medicareCoverage );

            var account = new Account { Activity = new PreAdmitNewbornActivity(), Insurance = insurance };

            var accident = new Accident { Kind = TypeOfAccident.NewAuto() };
            accident.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            account.Diagnosis.Condition = accident;

            var ruleUnderTest = new NoMedicarePrimaryPayorForAutoAccident();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion

        

        #region Support Methods
        #endregion
    }
}
