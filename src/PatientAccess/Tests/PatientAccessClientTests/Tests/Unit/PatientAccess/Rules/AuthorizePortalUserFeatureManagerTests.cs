using System; 
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class AuthorizePortalUserFeatureManagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            authorizePortalUserFeatureManager = new AuthorizePortalUserFeatureManager();
        }
        #region Defualt Scenarios
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {
            
            var actualResult = authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(null);
            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestAuthorizePortalUserFeatureVisible_CreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient, HSVBLANK, true);
            Assert.IsFalse(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }

        [Test]
        public void TestAuthorizePortalUserFeatureVisible_CreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(),
                VisitType.Inpatient, HSVBLANK, true );
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion Defualt Scenarios

        #region PreRegistration
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsPreRegistration_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, HSVBLANK, true);
            Assert.IsFalse(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion PreRegistration

        #region ActivatePreRegistrationAfterReleaseDate
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenCreatedBeforeReleaseDateAndActivatePreRegistrationAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient, HSVBLANK, true);
            account.Activity.AssociatedActivityType = typeof (ActivatePreRegistrationActivity);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion ActivatePreRegistrationAfterReleaseDate

        
        #region Registration
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, HSVBLANK, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType2_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, HSVBLANK, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType4_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, HSVBLANK, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }

       [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType3_HSV58_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, HSV58, true);
            Assert.IsTrue( authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
       public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType3_HSV59_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV59, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        
        #endregion Registration
        #region ShortRegistration
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsShortRegistration_AndPatientType2_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, HSVBLANK, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsShortRegistration_AndPatientType4_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, HSVBLANK, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion ShortRegistration
       
        #region InvalidCase
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_AndPatientType3_HSV01_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV01, true);
            Assert.IsFalse(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsRegistration_NONPatient_HSVBLANK_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            Account account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.NonPatient, HSV58, true);
            Assert.IsFalse(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion InvalidCase

        #region  PostMSERegistration
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsPOSTMSE_AndPatientType3_HSV59_AndCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new PostMSERegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Emergency, HSV59, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        [Test]
        public void TestAuthorizePortalUserFeatureVisible_WhenActivityIsPOSTMSE_AndPatientType3_HSV01_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            Account account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, HSV01, true);
            Assert.IsTrue(authorizePortalUserFeatureManager.ShouldFeatureBeEnabled(account));
        }
        #endregion PostMSERegistration
   

        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
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
        private DateTime GetTestDateAfterFeatureStart()
        {
            return authorizePortalUserFeatureManager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return authorizePortalUserFeatureManager.FeatureStartDate.AddDays(-10);
        }
        #endregion

        #region Constants

        private AuthorizePortalUserFeatureManager authorizePortalUserFeatureManager;
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
        private readonly HospitalService HSV01 = new HospitalService
        {
            Code = HospitalService.ACUTE_CARE_CLINIC_1,
            Description = "HSV 01"
        };
        private readonly HospitalService HSVBLANK = new HospitalService
        {
            Code = HospitalService.BLANK_CODE,
            Description = "HSVBLANK"
        };
       
        #endregion
    }
}
