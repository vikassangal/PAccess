 
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain; 
using PatientAccess.Domain.UCCRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class AdditionalRaceCodesFeatureManagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            additionalRaceCodesFeatureManager = new AdditionalRacesFeatureManager();

        }
        #region Defualt Scenarios
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {

            var actualResult = additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(null);
            Assert.IsFalse(actualResult);
        }
        #endregion Defualt Scenarios

        #region PreRegistration Scenarios
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsPreRegistration_PT0_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(),  VisitType.PreRegistration, true);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsPreRegistration_PT0_InvalidFacilityShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), VisitType.PreRegistration, false);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        #endregion PreRegistration Scenarios

        #region Registration Scenarios
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT1__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Inpatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT2__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Outpatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT3__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Emergency, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT4__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Recurring, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT9__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.NonPatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsRegistration_PT1__InvalidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Inpatient, false);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        #endregion Registration Scenarios

        #region PostMSE Registration Scenarios
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsPostMSE_PT3__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), VisitType.Inpatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        
       
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsPostMSE_PT3__InvalidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), VisitType.Emergency, false);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        #endregion PostMSE Registration Scenarios

        #region UC PostMSE Registration Scenarios
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsUCPostMSE_PT2__ShouldReturnTrue()
        {
            var account = GetAccount(new UCCPostMseRegistrationActivity(), VisitType.Outpatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }


        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsUCPostMSE_PT2__InvalidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), VisitType.Outpatient, false);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        #endregion UC PostMSERegistration Scenarios

        #region Activate PreRegistration Scenarios
        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsActivatePreReg_PT2__ShouldReturnTrue()
        {
            var account = GetAccount(new ActivatePreRegistrationActivity(), VisitType.Outpatient, true);
            Assert.IsTrue(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }


        [Test]
        public void TestAdditionalRaceCodesFeatureVisible_ActivityIsActivatePreReg_PT2__InvalidFacility_ShouldReturnTrue()
        {
            var account = GetAccount(new ActivatePreRegistrationActivity(), VisitType.Outpatient, false);
            Assert.IsFalse(additionalRaceCodesFeatureManager.IsAdditionalRacesFeatureValidForAccount(account));
        }
        #endregion Activate PreRegistration Scenarios

        #region Support Methods

        private static Account GetAccount(Activity activity, VisitType kindofVisit,
            bool featureEnabledForFacility)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");
            var facilityState = new State(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Texas", "TX");
            if (featureEnabledForFacility)
            {
                facilityState = new State(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "California", "CA");
            }

            facility.FacilityState = facilityState;
            return new Account
            {
                Facility = facility,
                Activity = activity,
                KindOfVisit = kindofVisit
            };
        }

        #endregion Support Methods

        #region Constants

        private AdditionalRacesFeatureManager additionalRaceCodesFeatureManager;
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
