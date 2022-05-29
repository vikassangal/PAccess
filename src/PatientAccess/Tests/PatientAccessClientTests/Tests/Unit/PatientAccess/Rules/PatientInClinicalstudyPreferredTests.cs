using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for PatientInClinicalstudyPreferredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class PatientInClinicalstudyPreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            PatientInClinicalstudyPreferred ruleUnderTest = new PatientInClinicalstudyPreferred();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDate_ShouldReturnFalse()
        {
            Account account = GetAccount( new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Blank );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Yes );
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFacilityInvalidForClinicalResearchFields_ShouldReturnTrue()
        {
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            var account = new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "ACCOUNT CYCLE OVERVIEW",
                                           "ACO" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = true;
            account.Facility = facility;

            account.AdmitDate = new DateTime( 2009, 01, 01 );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientIsInClinicalResearchStudy_ShouldReturnTrue()
        {
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            var account = new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;

            account.AdmitDate = new DateTime( 2009, 12, 01 );
            account.IsPatientInClinicalResearchStudy = new YesNoFlag( YesNoFlag.CODE_YES );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndIsPatientInClinicalResearchStudyIsBlank_ShouldReturnFalse()
        {
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            var account = new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;

            account.AdmitDate = new DateTime( 2009, 12, 01 );
            account.IsPatientInClinicalResearchStudy = new YesNoFlag( YesNoFlag.CODE_BLANK );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsFalse( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndAdmitDateIsBeforeReleaseDate_ShouldReturnTrue()
        {
            var ruleUnderTest = new PatientInClinicalstudyPreferred();
            var account = new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;

            account.AdmitDate = new DateTime( 2009, 01, 01 );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }
        #endregion

        #region Support Methods

        private static Account GetAccount( Activity activity, DateTime admitDate, YesNoFlag patientInClinicalResearch )
        {
            return new Account { Activity = activity, AdmitDate = admitDate, IsPatientInClinicalResearchStudy = patientInClinicalResearch };
        }

        #endregion

        #region Constants

        private readonly DateTime TestDateAfterClinicalResearchReleaseDate = DateTime.Parse( "11-30-2009" );
        private readonly DateTime TestDateBeforeClinicalResearchReleaseDate = DateTime.Parse( "11-30-2008" );

        #endregion
    }
}
