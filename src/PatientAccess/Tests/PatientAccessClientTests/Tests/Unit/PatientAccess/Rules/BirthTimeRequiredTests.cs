using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for BirthTimeRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class BirthTimeRequiredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            BirthTimeRequired ruleUnderTest = new BirthTimeRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new BirthTimeRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_ValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date,0,0);
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_ValidPT_NonEmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount( new RegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date,12,34 );
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsNewBornRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_ValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount( new AdmitNewbornActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date,0,0 );
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsNewBornRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_ValidPT_NonEmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount( new AdmitNewbornActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date ,12,34);
            DateTime patientDateOfBirth = account.Patient.DateOfBirth;
            account.Patient.DateOfBirth = new DateTime( patientDateOfBirth.Year, patientDateOfBirth.Month, patientDateOfBirth.Day, 12, 34, 0 );
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_INValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            var account = GetAccount( new PreRegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE, 
                INVALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date.AddDays(-20) ,0,0);

            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedBeforeReleaseDate_DOBLessThan10Days_ValidPT_EmptyBirthTime_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), ACCOUNTCREATED_BEFORE_BIRTHTIME_RELEASEDATE,
                                        VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date,0,0 );
           
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AccountCreatedAfterReleaseDate_DOBLessThan10Days_ValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount( new PreRegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                        VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date,0,0 );
           
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_EmptyBirthTime_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date.AddDays(-15), 0, 0 );
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_NonEmptyBirthTime_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today.Date.AddDays(-15), 12, 34 );
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsUCPreMse_AccountCreatedAfterReleaseDate_DOBThan10Days_ValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount(new UCCPreMSERegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                                         VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today, 0, 0);
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_WhenActivityIsUCPostMse_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_EmptyBirthTime_ShouldReturnFalse()
        {
            Account account = GetAccount(new UCCPreMSERegistrationActivity(), ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE,
                VALID_PATIENTTYPE_FOR_BIRTHTIME, DateTime.Today, 0, 0);
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #region TestsforAdmitDate
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIsBirthTimeVisibleForDOB_ExpectedExceptionOnAdmitDate_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.Today.Date.AddDays(-15), DateTime.MinValue.AddDays(-10).Date));
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIsBirthTimeVisibleForDOB_ExpectedExceptionOnDOB_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.MinValue.AddDays(-10).Date, DateTime.Today.Date));
        }
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIsBirthTimeVisibleForDOB_ExpectedException_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.MinValue.AddDays(-10).Date, DateTime.MinValue.AddDays(-10).Date));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_AdmitTimeisToday_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();

            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.Today.Date.AddDays(-15), DateTime.Today));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBisMin_ValidPT_AdmitTimeisMin_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.MinValue, DateTime.MinValue));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBisMin_ValidPT_AdmitTimeisToday_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.MinValue, DateTime.Today));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_AdmitTimeisMin_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.Today.Date.AddDays(-15), DateTime.MinValue));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBMoreThan10Days_ValidPT_AdmitTimeisMinPlus8Days_ShouldReturnFalse()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsFalse(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.Today.Date.AddDays(-15), DateTime.Now));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AccountCreatedAfterReleaseDate_DOBisMax_ValidPT_AdmitTimeisMax_ShouldReturnTrue()
        {
            var ruleUnderTest = new BirthTimeRequired();
            Assert.IsTrue(ruleUnderTest.IsBirthTimeVisibleForDOB(DateTime.MaxValue, DateTime.MaxValue));
        }

        #endregion TestsforAdmitDate

        #endregion

        #region Support Methods

        private static Account GetAccount( Activity activity, DateTime accountCreatedDate, string patienTType , DateTime DOB ,int hour, int minute)
        {

            var patient = new Patient {DateOfBirth = new DateTime(DOB.Year, DOB.Month, DOB.Day, hour, minute, 0)};
                       
            Account account =  new Account
                       {
                           Activity = activity,
                           AccountCreatedDate = accountCreatedDate,
                           AdmitDate = DateTime.Now,
                           KindOfVisit = new VisitType(PersistentModel.NEW_OID,
                                                       PersistentModel.NEW_VERSION, "PatientType", patienTType),
                           Patient = patient };
            account.Facility = new Facility(PersistentModel.NEW_OID,
                                            PersistentModel.NEW_VERSION, "Doctors of dallas", "DHF", 10, -6, true);

            User.SetCurrentUserTo( User.NewInstance() );
            User.GetCurrent().Facility = account.Facility;
            return account;
        }
        
        #endregion

        #region Constants

        private readonly DateTime ACCOUNTCREATED_AFTER_BIRTHTIME_RELEASEDATE = DateTime.Parse( "01-05-2015" );
        private readonly DateTime ACCOUNTCREATED_BEFORE_BIRTHTIME_RELEASEDATE = DateTime.Parse( "01-01-2009" );
        private const string VALID_PATIENTTYPE_FOR_BIRTHTIME = "2";
        private const string INVALID_PATIENTTYPE_FOR_BIRTHTIME = "0";

        #endregion
    }
}
