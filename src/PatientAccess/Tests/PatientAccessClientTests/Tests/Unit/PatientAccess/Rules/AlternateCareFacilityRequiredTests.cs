using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AlternateCareFacilityRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class AlternateCareFacilityRequiredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            AlternateCareFacilityRequired ruleUnderTest = new AlternateCareFacilityRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AlternateCareFacilityRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndCreatedDateAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateAfterAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndCreatedDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateBeforeAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedAfterReleaseDateAdmitSourceValid_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndCreatedDateAfterReleaseDateAdmitSourceValid_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateAfterAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndCreatedDateBeforeReleaseDateAdmitSourceValid_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateBeforeAlternateCareFacilityReleaseDate, ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndCreatedDateAfterReleaseDateAdmitSourceInValid_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateBeforeAlternateCareFacilityReleaseDate, ADMIT_SOURCE_NOT_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndCreatedAfterReleaseDateAdmitSourceInValid_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterAlternateCareFacilityReleaseDate, ADMIT_SOURCE_NOT_VALID_FOR_ALTERNATE_CARE_FACILITY);
            var ruleUnderTest = new AlternateCareFacilityRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion

        #region Support Methods

        private static Account GetAccount( Activity activity, DateTime accountCreatedDate, string AdmitSourceCode  )
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                AdmitSource = new AdmitSource(PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION, "AdmitSource", AdmitSourceCode), 
                                           AlternateCareFacility = String.Empty };
        }

        #endregion

        #region Constants

        private readonly DateTime TestDateAfterAlternateCareFacilityReleaseDate = DateTime.Parse( "01-05-2010" );
        private readonly DateTime TestDateBeforeAlternateCareFacilityReleaseDate = DateTime.Parse( "01-01-2009" );
        private const string ADMIT_SOURCE_VALID_FOR_ALTERNATE_CARE_FACILITY = "H";
        private const string ADMIT_SOURCE_NOT_VALID_FOR_ALTERNATE_CARE_FACILITY = "Q";

        #endregion
    }
}
