using NUnit.Framework;
using PatientAccess.Actions;
using PatientAccess.Domain;

namespace Tests.Integration.PatientAccess.UI
{
    [TestFixture]
    public class AdmitNewbornActivityRequiredFieldsTests
    {
        [Test]
        public void MaritalStatusIsRequireForRegistrationActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };

            var maritalStatusIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.MaritalStatusRequired );

            Assert.IsTrue( maritalStatusIsRequiredAndMissing );
        }
        
        [Test]
        public void AppointmentIsRequiredForAdmitNewborActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };

            var appointmentIsRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.AppointmentRequired );

            Assert.IsTrue(appointmentIsRequiredAndMissing);
        }

        [Test]
        public void AppointmentIsOptionalForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };

            var appointmentIsNotRequiredAndMissing = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>(RuleIds.AppointmentRequired);

            Assert.IsFalse(appointmentIsNotRequiredAndMissing);
        }

        [Test]
        public void PreopDateIsRequiredForNewbornRegistrationActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };

            var preopDateIsRequired = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.PreopDateRequired );

            Assert.IsTrue(preopDateIsRequired);
        }

        [Test]
        public void PreopDateIsPreferredForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };

            var preopDateIsPreferred = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>(RuleIds.PreopDatePreferred);

            Assert.IsTrue(preopDateIsPreferred);
        }

        [Test]
        public void PreopDateIsRequiredForInpatientRegistrationActivity()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };

            var preopDateIsRequired = account.IsRuleEnforcedForCompositeAction<OnPatientDemographics>( RuleIds.PreopDateRequired );

            Assert.IsTrue( preopDateIsRequired );
        }

        [Test]
        public void NPPSignedOnDateIsPreferredForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion = new NPPVersion( 0L, ReferenceValue.NEW_VERSION, "20030101", "20" );
            account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus( "S" );
            var nPPSignedOnDateIsPreferred = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.NPPSignedOnDatePreferred );

            Assert.IsTrue( nPPSignedOnDateIsPreferred );
        }

        [Test]
        public void NPPSignedOnDateIsRequiredForAdmitNewbornActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };
            account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion = new NPPVersion(0L, ReferenceValue.NEW_VERSION, "20030101", "20");
            account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus("S");
            var nPPSignedOnDateIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.NPPSignedOnDateRequired);

            Assert.IsTrue(nPPSignedOnDateIsRequired);
        }

        [Test]
        public void NPPSignedOnDateIsRequiredForInpatientRegistrationActivity()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };
            account.Patient.NoticeOfPrivacyPracticeDocument.NPPVersion = new NPPVersion( 0L, ReferenceValue.NEW_VERSION, "20030101", "20" );
            account.Patient.NoticeOfPrivacyPracticeDocument.SignatureStatus = new SignatureStatus( "S" );
            var nPPSignedOnDateIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.NPPSignedOnDateRequired );

            Assert.IsTrue( nPPSignedOnDateIsRequired );
        }

        [Test]
        public void NPPVersionIsPreferredForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };

            var nPPVersionIsPreferred = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.NPPVersionPreferred );

            Assert.IsTrue( nPPVersionIsPreferred );
        }

        [Test]
        public void NPPVersionIsRequiredForAdmitNewbornActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };

            var nPPVersionIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.NPPVersionRequired);

            Assert.IsTrue(nPPVersionIsRequired);
        }

        [Test]
        public void NPPVersionIsRequiredForInpatientRegistrationActivity()
        {
            var account = new Account { Activity = new PreRegistrationActivity() };

            var nPPVersionIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.NPPVersionRequired );

            Assert.IsTrue( nPPVersionIsRequired );
        }

        [Test]
        public void COSSignedIsPreferredForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };

            var cossSignedIsPreferred = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.COSSignedPreferred );

            Assert.IsTrue( cossSignedIsPreferred );
        }

        [Test]
        public void COSSignedIsPreferredForAdmitNewbornActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };

            var cossSignedIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COSSignedRequired);

            Assert.IsTrue(cossSignedIsRequired);
        }

        [Test]
        public void COSSignedIsRequiredForInpatientRegistrationActivity()
        {
            var account = new Account { Activity = new RegistrationActivity() };

            var cossSignedIsRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>( RuleIds.COSSignedRequired );

            Assert.IsTrue( cossSignedIsRequired );
        }
        
        [Test]
        public void BloodlessIsPreferredForPreAdmitNewbornActivity()
        {
            var account = new Account { Activity = new PreAdmitNewbornActivity() };
            account.Bloodless = YesNoFlag.Blank;
            var bloodlessIsPreferred = account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.BloodlessPreferred);

            Assert.IsTrue(bloodlessIsPreferred);
        }
        [Test]
        public void BloodlessIsRequiredForAdmitNewbornActivity()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };
            account.Bloodless = YesNoFlag.Blank;
            var bloodlessIsRequired = account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.BloodlessRequired);

            Assert.IsTrue(bloodlessIsRequired);
        }

        [Test]
        public void BloodlessIsRequiredForInpatientRegistrationActivity()
        {
            var account = new Account { Activity = new RegistrationActivity() };
            account.Bloodless = YesNoFlag.Blank;
            var bloodlessIsRequired = account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.BloodlessRequired);

            Assert.IsTrue(bloodlessIsRequired);
        }

        [Test]
        public void PrimaryCarePhysicianRequiredForAdmitNewbornActivity_ShouldReturnTrue()
        {
            var account = new Account {Activity = new AdmitNewbornActivity()};
            var PrimaryCarePhysicianRequired =
                account.IsRuleEnforcedForCompositeAction<OnClinicalForm>(RuleIds.PrimaryCarePhysicianRequired);
            Assert.IsTrue(PrimaryCarePhysicianRequired);
        }

        

        [Test]
        public void COBReceivedRequiredForAdmitNewbornActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };
            account.KindOfVisit = VisitType.Inpatient;
            var financialClass = new FinancialClass { Code = "02" };
            account.FinancialClass = financialClass;
            var COBReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.COBReceivedRequired);

            Assert.IsTrue(COBReceivedRequired);
        }

        [Test]
        public void IMFMReceivedRequiredForAdmitNewbornActivity_ShouldReturnTrue()
        {
            var account = new Account { Activity = new AdmitNewbornActivity() };
            account.KindOfVisit = VisitType.Inpatient;
            var financialClass = new FinancialClass { Code = "10" };
            account.FinancialClass = financialClass;
            var IMFMReceivedRequired = account.IsRuleEnforcedForCompositeAction<OnRegulatoryForm>(RuleIds.IMFMReceivedRequired);

            Assert.IsTrue(IMFMReceivedRequired);
        }

    }
}
