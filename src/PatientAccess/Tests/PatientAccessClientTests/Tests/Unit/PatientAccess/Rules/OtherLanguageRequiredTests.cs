using System;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    public class OtherLanguageRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_LanguageIsEnglishAndOtherLanguageIsEmpty_ShouldReturnTrue()
        {
            Account account = GetAccountWithLanguage( CALIFORNIA_FACILITY, ENGLISH_LANGUAGE );
            account.Patient.OtherLanguage = String.Empty;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_LanguageIsEnglishAndOtherLanguageIsNotEmpty_ShouldReturnTrue()
        {
            Account account = GetAccountWithLanguage( CALIFORNIA_FACILITY, ENGLISH_LANGUAGE );
            account.Patient.OtherLanguage = Something;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_LanguageIsOtherAndOtherLanguageIsEmptyInCAFacility_ShouldReturnFalse()
        {
            Account account = GetAccountWithLanguage( CALIFORNIA_FACILITY, OTHER_LANGUAGE );
            account.Patient.OtherLanguage = String.Empty;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_LanguageIsOtherAndOtherLanguageIsEmptyInNONCAFacility_ShouldReturnTrue()
        {
            Account account = GetAccountWithLanguage( TEXAS_FACILITY, OTHER_LANGUAGE );
            account.Patient.OtherLanguage = String.Empty;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_LanguageIsOtherAndOtherLanguageIsNotEmptyInCAFacility_ShouldReturnTrue()
        {
            Account account = GetAccountWithLanguage( CALIFORNIA_FACILITY, OTHER_LANGUAGE );
            account.Patient.OtherLanguage = Something;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_LanguageIsOtherAndOtherLanguageIsNotEmptyInNONCAFacility_ShouldReturnTrue()
        {
            Account account = GetAccountWithLanguage( TEXAS_FACILITY, OTHER_LANGUAGE );
            account.Patient.OtherLanguage = Something;
            var ruleUnderTest = new OtherLanguageRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion

        #region Support Methods

        private static Account GetAccountWithLanguage( Facility facility, Language language )
        {
            Patient patient = new Patient { Language = language };
            Account anAccount = new Account { Patient = patient, Facility = facility };
            return anAccount;
        }

        #endregion

        private const string Something = "Something";
        private Language OTHER_LANGUAGE = new Language( 1L, DateTime.Now, "OTHER", Language.OTHER_LANGUAGE );
        private Language ENGLISH_LANGUAGE = new Language( 1L, DateTime.Now, "EN", "English" );

        private Facility CALIFORNIA_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( "ACO" );
        private Facility TEXAS_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith( "DHF" );
    }
}
