using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class EmailAddressRequiredTests
    {
        
        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new EmailAddressRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new EmailAddressRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var diagnosis = new Diagnosis();
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(diagnosis));
        }

        #region Registration Activity
        #region PatientPortalOptIn_Yes
        [Test]
        public void TestCanBeAppliedTo_WhenAccountCreatedAfterReleaseDate_NonPatientType_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.NonPatient, hsvblank, true);
            var ruleUnderTest = new EmailAddressRequired();
            
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_AccountCreatedAfterReleaseDate_AndPT2_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Outpatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInYes_AndPT1_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Inpatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInYes_AndPT3AndHSV58_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Emergency, hsv58, true);
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInYes_AndPT3AndHSV59_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Emergency, hsv59, true);
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInYes_AndPT3AndHSV59_DuringPOSTMSEActivityAfterFeatureStartDate_ShouldReturnFalse()
        {
            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Emergency, hsv59, true);
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion PatientPortalOptIn_Yes

        #region PatientPortalOptIn_No
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInNo_AndCreatedDateAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinno, VisitType.NonPatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
       
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInNo_PT3ANDHSV58_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart() ,
                patientportaloptinno, VisitType.Emergency, hsv59, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion PatientPortalOptIn_No

        #region PatientPortalOptIn_Blank
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInBlank_PT3ANDHSV58_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart() ,
                patientportaloptinblank, VisitType.Emergency, hsv58, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        
        #endregion PatientPortalOptIn_Blank

        #region ConditionOfService

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsYes_HospitalCommunicationOptInIsYes_PatientPortalOptInIsYes_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(),
                 patientportaloptinyes, VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.Yes;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsRefused__ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.REFUSED,
                Description = ConditionOfService.REFUSED_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.True(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsRefused_EmailReasonIsProvided_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var emailReason = new EmailReason
            {
                Code = EmailReason.PROVIDED,
                Description = EmailReason.PROVIDED_DESCRIPTION
            };
            account.Patient.EmailReason = emailReason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.REFUSED,
                Description = ConditionOfService.REFUSED_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsRefused_EmailReasonIsProvided_ForPreRegistrationActivity_ShouldReturnFalse()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.PreRegistration, hsvblank, true);
            var emailReason = new EmailReason
            {
                Code = EmailReason.PROVIDED,
                Description = EmailReason.PROVIDED_DESCRIPTION
            };
            account.Patient.EmailReason = emailReason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.REFUSED,
                Description = ConditionOfService.REFUSED_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsRefused_EmailReasonIsNotProvided_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var emailReason = new EmailReason
            {
                Code = EmailReason.DECLINED,
                Description = EmailReason.DECLINED_DESCRIPTION
            };
            account.Patient.EmailReason = emailReason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.REFUSED,
                Description = ConditionOfService.REFUSED_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsNotAvailable_EmailReasonIsProvided_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var emailreason = new EmailReason()
            {
                Code = EmailReason.PROVIDED,
                Description = EmailReason.PROVIDED_DESCRIPTION
            };
            account.Patient.EmailReason = emailreason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.NOT_AVAILABLE,
                Description = ConditionOfService.NOT_AVAILABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsNotAvailable_EmailReasonIsNotProvided_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var emailreason = new EmailReason()
            {
                Code = EmailReason.REMOVE,
                Description = EmailReason.REMOVE_DESCRIPTION
            };
            account.Patient.EmailReason = emailreason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.NOT_AVAILABLE,
                Description = ConditionOfService.NOT_AVAILABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsNotAvailable_HospitalCommunicationOptInIsYes_PatientPortalOptInIsNo_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                patientportaloptinno, VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.Yes;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.NOT_AVAILABLE,
                Description = ConditionOfService.NOT_AVAILABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsNotAvailable_HospitalCommunicationOptInIsNo_PatientPortalOptInIsNo_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                patientportaloptinno, VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.No;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.NOT_AVAILABLE,
                Description = ConditionOfService.NOT_AVAILABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsMedicallyUnableToSign_EmailReasonIsProvided_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                 patientportaloptinno, VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.No;
            var emailreason = new EmailReason()
            {
                Code = EmailReason.PROVIDED,
                Description = EmailReason.PROVIDED_DESCRIPTION
            };
            account.Patient.EmailReason = emailreason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.UNABLE,
                Description = ConditionOfService.UNABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsMedicallyUnableToSign_EmailReasonIsNotProvided_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                VisitType.Inpatient, hsvblank, true);
            var emailreason = new EmailReason()
            {
                Code = EmailReason.REMOVE,
                Description = EmailReason.REMOVE_DESCRIPTION
            };
            account.Patient.EmailReason = emailreason;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.UNABLE,
                Description = ConditionOfService.UNABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsMedicallyUnableToSign_HospitalCommunicationOptInIsYes_PatientPortalOptInIsNo_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                patientportaloptinno,VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.Yes;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.UNABLE,
                Description = ConditionOfService.UNABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenConditionOfServiceIsMedicallyUnableToSign_HospitalCommunicationOptInIsNo_PatientPortalOptInIsNo_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStartForEmailReason(),
                patientportaloptinno, VisitType.Inpatient, hsvblank, true);
            account.Patient.HospitalCommunicationOptIn = YesNoFlag.No;
            var condtionOfService = new ConditionOfService()
            {
                Code = ConditionOfService.UNABLE,
                Description = ConditionOfService.UNABLE_DESCRIPTION
            };
            account.COSSigned = condtionOfService;
            var ruleUnderTest = new EmailAddressRequired();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion

        #endregion Registration Activity

        #region Transfer Activity

        #region PatientPortalOptIn_Yes
        [Test]
        public void TestCanBeAppliedTo_WhenAccountCreatedAfterReleaseDate_NonPatientType_ShouldReturnTrue_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.NonPatient, hsvblank, true);
            var ruleUnderTest = new EmailAddressRequired();
            
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_AccountCreatedAfterReleaseDate_AndPT2_ShouldReturnFalse_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Outpatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInYes_AndPT1_ShouldReturnFalse_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinyes, VisitType.Inpatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();
            account.IsShortRegistered = false;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

       #endregion PatientPortalOptIn_Yes

        #region PatientPortalOptIn_No
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInNo_AndCreatedDateAfterReleaseDate_ShouldReturnTrue_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinno, VisitType.NonPatient, hsvblank, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
       
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInNo_PT3ANDHSV58_ShouldReturnTrue_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinno, VisitType.Emergency, hsv59, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion PatientPortalOptIn_No

        #region PatientPortalOptIn_Blank
        [Test]
        public void TestCanBeAppliedTo_WhenPatientPortalOptInBlank_PT3ANDHSV58_ShouldReturnTrue_TransferActivity()
        {
            var account = GetAccount(new TransferOutToInActivity(), GetTestDateAfterFeatureStart(),
                patientportaloptinblank, VisitType.Emergency, hsv58, true );
            var ruleUnderTest = new EmailAddressRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        
        #endregion PatientPortalOptIn_Blank

        #endregion Transfer Activity

        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, 
            YesNoFlag patientPortalOptIn, VisitType kindofVisit, HospitalService hsv, 
            bool featureEnabledForFacility )
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF");
            if (featureEnabledForFacility)
            {
                facility["IsPatientPortalOptInEnabled"] = true;
            }
            return new Account
            {
                Facility = facility,
                Activity = activity, 
                AccountCreatedDate = accountCreatedDate,
                PatientPortalOptIn = patientPortalOptIn,
                KindOfVisit = kindofVisit,
                HospitalService =  hsv
            };
           
        }

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit, HospitalService hsv,
            bool featureEnabledForFacility)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");
            if (featureEnabledForFacility)
            {
                facility["IsPatientPortalOptInEnabled"] = true;
            }
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

        private readonly YesNoFlag patientportaloptinyes = YesNoFlag.Yes;
        private readonly YesNoFlag patientportaloptinno = YesNoFlag.No;
        private readonly YesNoFlag patientportaloptinblank = YesNoFlag.Blank;
 
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new PatientPortalOptInFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateAfterFeatureStartForEmailReason()
        {
            return new EmailReasonFeatureManager().FeatureStartDate.AddDays(10);
        }
        private readonly HospitalService hsv58 = new HospitalService
        {
            Code = HospitalService.HSV58,
            Description = "HSV 58"
        };
        private readonly HospitalService hsv59 = new HospitalService
        {
            Code = HospitalService.HSV59,
            Description = "HSV 59"
        };

        private readonly HospitalService hsvblank = new HospitalService
        {
            Code = HospitalService.BLANK_CODE,
            Description = "HSVBLANK"
        };
    }
}
