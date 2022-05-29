using System;
using Extensions.ClassLibrary.PersistenceCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PatientAccess.Domain;
using PatientAccess.Domain.Specialized;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for ResearchSponsorRequiredTests
    /// </summary>
    [TestClass]
    public class ResearchSponsorRequiredTests
    {
        #region Test Methods
        [TestMethod]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            ResearchSponsorRequired ruleUnderTest = new ResearchSponsorRequired();
            object inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new ResearchSponsorRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new ResearchSponsorRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDate_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank, researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return true as PatientInClinicalStudy is blank
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDate_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount( new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Blank, researchSponsor );
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return true as PatientInClinicalStudy is blank
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDate_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount( new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Blank, researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            Account account = GetAccount( new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes , null);
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return true as ResearchSponsor is null
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            var account = GetAccount( new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, null );
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return true as ResearchSponsor is null
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndAdmitAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnFalse()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            Account account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return false as ResearchSponsor is not null and blank
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateAfterReleaseDatePatientInClinicalStudyIsY_ShouldReturnFalse()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount(new PreRegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes, researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();

            //Should return false as ResearchSponsor is not null and blank (not Y or N)
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndAdmitDateBeforeReleaseDatePatientInClinicalStudyIsY_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount(new PreRegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Yes, researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityInvalidForClinicalResearchFields_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var ruleUnderTest = new ResearchSponsorRequired();
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate,YesNoFlag.Yes,
                                     researchSponsor);
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
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientIsInClinicalResearchStudyAndHasResearchSponsor_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor("RSRCH");
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes,
                                    researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
           
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
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientIsInClinicalResearchStudyAndNoResearchSponsor_ShouldReturnFalse()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.Yes,
                                    researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
            Facility facility = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           "DOCTORS HOSPITAL DALLAS",
                                           "DHF" );
            facility[ClinicalTrialsConstants.KEY_IS_FACILITY_CLINICAL_TRIAL_ENABLED] = null;
            account.Facility = facility;
            
            var actualResult = ruleUnderTest.CanBeAppliedTo( account );
            Assert.IsFalse( actualResult );
        }

        [TestMethod]
        public void TestCanBeAppliedTo_WithFacilityValidForClinicalResearchFieldsAndPatientNotInClinicalResearchStudy_ShouldReturnTrue()
        {
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount(new RegistrationActivity(), TestDateAfterClinicalResearchReleaseDate, YesNoFlag.No,
                                    researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
          
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
            ResearchSponsor researchSponsor = GetResearchSponsor(" ");
            var account = GetAccount(new RegistrationActivity(), TestDateBeforeClinicalResearchReleaseDate, YesNoFlag.Yes,
                                    researchSponsor);
            var ruleUnderTest = new ResearchSponsorRequired();
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
        private static ResearchSponsor GetResearchSponsor(string researchSponsorCode)
        {
            return new ResearchSponsor { Code = researchSponsorCode };
        }

        private static Account GetAccount(Activity activity, DateTime admitDate, YesNoFlag patientInClinicalResearch, ResearchSponsor researchSponsor)
        {
            return new Account { Activity = activity, AdmitDate = admitDate, IsPatientInClinicalResearchStudy = patientInClinicalResearch, ResearchSponsor = researchSponsor  };
        }
        #endregion

        #region Constants

        private readonly DateTime TestDateAfterClinicalResearchReleaseDate = DateTime.Parse( "11-30-2009" );
        private readonly DateTime TestDateBeforeClinicalResearchReleaseDate = DateTime.Parse( "11-30-2008" );

        #endregion
    }
}
