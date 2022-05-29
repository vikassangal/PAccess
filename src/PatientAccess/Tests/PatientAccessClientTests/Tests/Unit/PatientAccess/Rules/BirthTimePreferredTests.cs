using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BirthTimeRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class BirthTimePreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new BirthTimePreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new BirthTimePreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithBirthTime_PreAdmitNewborn_ShouldReturnTrue()
        {

            var patient = new Patient
                              {
                                  DateOfBirth =
                                      new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 30, 5)
                              };
            var visitType = new VisitType {Code = VisitType.PREREG_PATIENT};
            var account = new Account { AccountCreatedDate = DateTime.Now, Patient =patient, Activity=new PreAdmitNewbornActivity(),
                                        KindOfVisit = visitType};
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithBirthTimeWithin10DaysFromAdmitDate_Reg_ShouldReturnTrue()
        {

            var patient = new Patient
            {
                DateOfBirth =
                    new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 30, 5 )
            };
            var visitType = new VisitType { Code = VisitType.INPATIENT };
            var account = new Account
            {
                AccountCreatedDate = DateTime.Now,
                AdmitDate = DateTime.Now,
                Patient = patient,
                Activity = new RegistrationActivity(),
                KindOfVisit = visitType
            };
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutBirthTimeWithin10DaysFromAdmitDate_Reg_ShouldReturnFalse()
        {

            var patient = new Patient
            {
                DateOfBirth =
                    new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };
            var visitType = new VisitType { Code = VisitType.INPATIENT };
            var account = new Account
            {
                AccountCreatedDate = DateTime.Now,
                AdmitDate = DateTime.Now,
                Patient = patient,
                Activity = new RegistrationActivity(),
                KindOfVisit = visitType
            };
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithBirthTimeBeyond10DaysFromAdmitDate_Reg_ShouldReturnTrue()
        {

            var patient = new Patient
            {
                DateOfBirth = DateTime.Now.Date.AddDays( -11 )
            };
            var visitType = new VisitType { Code = VisitType.INPATIENT };
            var account = new Account
            {
                AccountCreatedDate = DateTime.Now,
                AdmitDate = DateTime.Now,
                Patient = patient,
                Activity = new RegistrationActivity(),
                KindOfVisit = visitType
            };
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutBirthTimeBeyond10DaysFromAdmitDate_Reg_ShouldReturnTrue()
        {

            var patient = new Patient
            {
                DateOfBirth = DateTime.Now.Date.AddDays(-11)
            };
            var visitType = new VisitType { Code = VisitType.INPATIENT };
            var account = new Account
            {
                AccountCreatedDate = DateTime.Now,
                AdmitDate = DateTime.Now,
                Patient = patient,
                Activity = new RegistrationActivity(),
                KindOfVisit = visitType
            };
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutBirthTime_PreReg_ShouldReturnFalse()
        {

            var patient = new Patient
                              {
                                  DateOfBirth =
                                      new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
                              };
            var visitType = new VisitType {Code = VisitType.PREREG_PATIENT};
            var account = GetAccount(visitType, patient);
            var ruleUnderTest = new BirthTimePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        

        #endregion

        #region Support Methods
        private static Account GetAccount( VisitType visitType, Patient patient )
        {
            return new Account
            {
                AccountCreatedDate = DateTime.Now.AddDays( -1 ),
                AdmitDate = DateTime.Now,
                Patient = patient,
                Activity = new PreAdmitNewbornActivity(),
                KindOfVisit = visitType
            };
        }
        #endregion

        #region Constants

        #endregion
    }
}
