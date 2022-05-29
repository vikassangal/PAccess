using NUnit.Framework;
using PatientAccess.Rules;
using System;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class IMFMReceivedRequiredTests
    {
        #region Defaut Scenario

        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new IMFMReceivedRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new IMFMReceivedRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }

        #endregion

        #region PatientType1 and Valid Financial Class

        //it should return false since patient type and financial class is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass4_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "04" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Valid Financial Class

        #region PatientType1 and Valid Financial Class and IMFM Received Selected

        //it should return true since patient type and financial class is valid and IMFM Received seleccted as Yes
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass10_AndIMFMAsYes_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Yes;
            var financialClass = new FinancialClass { Code = "10" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 and Valid Financial Class and IMFM Received Selected

        #region PatientType1 and Invalid Financial Class

        //it should return true since patient type or financial class is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass2_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "02" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Invalid Financial Class

        #region PatientType1 and Valid Financial Class and account created before release date

        //it should return true since patient type and financial class is valid but account is created before release date
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass4_AndCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "04" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Valid Financial Class

        #region PatientType0 and Valid Financial Class 

        //it should return true since patient type, financial class is valid and hospital service is not 35
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndFinancialClass4_AndHSV_IsNot35_AndCreatedAfterReleaseDate_ShouldReturntrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "04" };
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterPreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_REGISTER;
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        //it should return false since patient type, financial class is valid,  hospital service is 35 and account created before release date 
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndFinancialClass4_AndHSV_Is35_AndCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "04" };
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforePreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        //it should return false since patient type, financial class is valid,  hospital service is 35 and account created after release date 
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndFinancialClass10_AndHSV_Is35_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "10" };
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterPreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new IMFMReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType0 and Valid Financial Class 

        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit, FinancialClass financialClass)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF");

            return new Account
            {
                Facility = facility,
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = kindofVisit,
                FinancialClass = financialClass

            };
        }

        #endregion

        #region Constants
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays(-10);
        }

        private static DateTime GetTestDateAfterPreRegistrationIMFMFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().PregRegistration_IMFM_FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforePreRegistrationIMFMFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().PregRegistration_IMFM_FeatureStartDate.AddDays(-10);
        }

        #endregion
    }
}
