using System;
using PatientAccess.Domain;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class ProcedureRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION) };
            var ruleUnderTest = new ProcedureRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(otherCoverage));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndProcedureIsEmptyVisitTypeIsInpatient_ShouldReturnFalse()
        {
            Account account = GetAccountAndDiagnosisWith(new RegistrationActivity(),new VisitType(1L,DateTime.Now,"Inpatient",VisitType.INPATIENT), string.Empty);
            var ruleUnderTest = new ProcedureRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationProcedureIsNotEmptyVisitTypeIsInpatient_ShouldReturnTrue()
        {
            Account account = GetAccountAndDiagnosisWith(new RegistrationActivity(), new VisitType(1L, DateTime.Now, "Inpatient", VisitType.INPATIENT), "Procedure Info Provided");
            var ruleUnderTest = new ProcedureRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndProcedureIsEmptyVisitTypeIsOutPatient_ShouldReturnFalse()
        {
            Account account = GetAccountAndDiagnosisWith(new RegistrationActivity(), new VisitType(1L, DateTime.Now, "Outpatient", VisitType.OUTPATIENT), string.Empty);
            var ruleUnderTest = new ProcedureRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationProcedureIsNotEmptyVisitTypeIsOutPatient_ShouldReturnTrue()
        {
            Account account = GetAccountAndDiagnosisWith(new RegistrationActivity(), new VisitType(1L, DateTime.Now, "Outpatient", VisitType.OUTPATIENT), "Procedure Info Provided");
            var ruleUnderTest = new ProcedureRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

      

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndProcedureIsEmptyVisitTypeIsOutPatient_ShouldReturnFalse()
        {
            var account = GetAccountAndDiagnosisWith(new PreRegistrationActivity(), new VisitType(1L, DateTime.Now, "Outpatient", VisitType.OUTPATIENT), string.Empty);
          
            var ruleUnderTest = new ProcedureRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndProcedureIsNotEmptyVisitTypeIsOutPatient_ShouldReturnTrue()
        {
            var account = GetAccountAndDiagnosisWith(new PreRegistrationActivity(), new VisitType(1L, DateTime.Now, "Outpatient", VisitType.OUTPATIENT),  "Procedure Info Provided");
            
            var ruleUnderTest = new ProcedureRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

     #endregion
        #region Support Methods
       
        private static Account GetAccountAndDiagnosisWith(Activity activity, VisitType kindOfVisit, string procedure)
        {
            Diagnosis diagnosis = new Diagnosis();
            diagnosis.Procedure = procedure;
            return new Account { Activity = activity, KindOfVisit = kindOfVisit, Diagnosis = diagnosis };
        }

        #endregion
    }
}
