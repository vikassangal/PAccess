using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    public class AuthorizeAdditionalPortalUsersRequiredTests
    {
        #region Defualt Scenarios
        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );

            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );

            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var diagnosis = new Diagnosis();
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( diagnosis ) );
        }
        #endregion Defualt Scenarios

        #region PatientType1
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, HSVBLANK, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion PatientType1

        #region PatientType2
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType2_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, HSVBLANK, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsTransferERPatienttoBeddedOutpatient_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new TransferERToOutpatientActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, HSVBLANK, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion PatientType2

        #region PatientType0
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndCreatedDateAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, HSVBLANK, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion PatientType0
        #region PatientType3
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType3_HSV58_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV58, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType3_HSV59_AndCreatedDateAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV59, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType3_HSV58_AndCreatedDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Emergency, HSV58, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType3_HSV01_AndCreatedDateBeforeReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Emergency, HSV01, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSE_AndPatientType3_HSV01_AccountCreatedDateAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV01, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
    
        #endregion PatientType3

        #region PatientType4
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType4_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, HSVBLANK, true);
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion PatientType4

        #region InvalidPatientType
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndCreatedAfterReleaseDate_InvalidKindofVisit_ShouldReturnTrue()
        {
            var account = GetAccount( new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.NonPatient, HSVBLANK, true  );
            var ruleUnderTest = new AuthorizeAdditionalPortalUserRequired();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        #endregion InvalidPatientType

        #region Support Methods

        private static Account GetAccount( Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit, HospitalService hsv, bool featureEnabledForFacility )
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");

            facility["IsAuthorizePortalUserEnabled"] = featureEnabledForFacility;

            return new Account
            {
                Facility = facility,
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = kindofVisit,
                HospitalService = hsv
            };
        }

        #endregion

        #region Constants
        private readonly HospitalService HSV01 = new HospitalService
        {
            Code = HospitalService.ACUTE_CARE_CLINIC_1,
            Description = "Acute Care "
        };
        private readonly HospitalService HSV58 = new HospitalService
        {
            Code = HospitalService.HSV58,
            Description = "HSV 58"
        };
        private readonly HospitalService HSV59 = new HospitalService
        {
            Code = HospitalService.HSV59,
            Description = "HSV 59"
        };

        private readonly HospitalService HSVBLANK = new HospitalService
        {
            Code = HospitalService.BLANK_CODE,
            Description = "HSVBLANK"
        };
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new AuthorizePortalUserFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new AuthorizePortalUserFeatureManager().FeatureStartDate.AddDays(-10);
        }

        #endregion
    }
}
