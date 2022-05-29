using System;
using Extensions.ClassLibrary.PersistenceCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for DoesPatientShowProofOfConsentRequiredTests
    /// </summary>
    [TestClass]
    public class DoesPatientShowProofOfConsentRequiredTests
    {
        #region Test Methods
        [TestMethod]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            DoesPatientShowProofOfConsentRequired ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank, YesNoFlag.Blank );
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();

            //During Reg Activity, If AdmitDate is After release date and PatientInClinicalStudy is Blank, Should return true
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank,YesNoFlag.Blank );
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();

            //During PreReg Activity, If AdmitDate is After release date and PatientInClinicalStudy is Blank, Should return true
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Blank,YesNoFlag.Blank );
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Blank);
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            
            //This should return false since DoesPatientShowProof is Blank
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Blank);
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();

            //This should return false since DoesPatientShowProof is Blank
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Blank);
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityInvalidForClinicalResearchFields_ShouldReturnTrue()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Yes);//  new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "ACCOUNT CYCLE OVERVIEW",
                                           "ACO" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = true;
            account.Facility = facility;
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientIsInClinicalResearchStudyAndHasProofOfConsent_ShouldReturnTrue()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Yes);// new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientIsInClinicalResearchStudyAndProofOfConsentIsBlank_ShouldReturnFalse()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Yes);// new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;

            account.AdmitDate = new DateTime( 2009, 12, 01 );
            account.IsPatientInClinicalResearchStudy = new YesNoFlag( YesNoFlag.CODE_YES );
            account.DoesPatientShowProofOfConsent = new YesNoFlag( YesNoFlag.CODE_BLANK );
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsFalse( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientNotInClinicalResearchStudy_ShouldReturnTrue()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.No, YesNoFlag.Yes);// new Account();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndAdmitDateIsBeforeReleaseDate_ShouldReturnTrue()
        {
            var ruleUnderTest = new DoesPatientShowProofOfConsentRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Yes, YesNoFlag.Blank);
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility; 
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsTrue( actualResult );
        }
        #endregion

        #region Support Methods

        private static Account GetAccount( Activity activity, DateTime admitDate, YesNoFlag patientInClinicalResearch,YesNoFlag doesPatientShowProof )
        {
            return new Account { Activity = activity, AdmitDate = admitDate, IsPatientInClinicalResearchStudy = patientInClinicalResearch, DoesPatientShowProofOfConsent = doesPatientShowProof};
        }

        #endregion

        #region Constants

        private readonly DateTime TestDateAfterClinicalResearchReleaseDate = DateTime.Parse( "11-30-2009" );
        private readonly DateTime TestDateBeforeClinicalResearchReleaseDate = DateTime.Parse( "11-30-2008" );

        #endregion
    }
}
