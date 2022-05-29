using NUnit.Framework;
using PatientAccess.Rules;
using System;
using Extensions.PersistenceCommon;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class COBReceivedAndIMFMReceivedFeatureManagerTests
    {
        #region Defaut Scenario

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ForCOBReceivedFeatureEnabled_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(null);

            Assert.IsFalse(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ForIMFMReceivedFeatureEnabled_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(null);

            Assert.IsFalse(actualResult);
        }

        #endregion

        
        //it should return false since patient type or financial class is invalid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndInvalidFinancialClass10_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "10" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
         
        
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass2_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "02" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass21_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "21" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
       
        //it should return false since patient type and financial class is valid but account is created before release date 
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass2_AndCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "02" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.PreRegistration, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
 
      
        //it should return false because patient type or financial class is invalid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndInvalidFinancialClass13_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {

            var financialClass = new FinancialClass { Code = "13" };
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
  
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndValidFinancialClass14_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "14" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndValidFinancialClass21_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "21" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
 
     
        //it should return false since patient type or financial class is invalid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType2_AndInvalidFinancialClass23_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "23" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
  
     
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType2_AndValidFinancialClass20_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "20" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Outpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
          

        //it should return false since patient type and financial class in invalid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsShortRegistration_AndPatientType3_AndInvalidFinancialClass26_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "26" };

            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, financialClass);

            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
 
        
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsShortRegistration_AndPatientType3_AndValidFinancialClass80_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "80" };

            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, financialClass);
            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
         
        //it should return false since patient type or financial class is invalid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsShortRegistration_AndPatientType4_AndInvalidFinancialClass40_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "40" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, financialClass);

            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult); ;
        }
 
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestCOBReceivedFeatureVisible_WhenActivityIsShortRegistration_AndPatientType4_AndValidFinancialClass81_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "81" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, financialClass);

            var actualResult = COBIMFMFeatureManager.IsCOBReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult); ;
        }
         
        //it should return true since patient type or financial class is invalid
        [Test]
        public void TestIMFMReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndInvalidFinancialClass2_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "02" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult); ;
        }
    
        //it should return true since patient type and financial class is valid
        [Test]
        public void TestIMFMReceivedFeatureVisible_WhenActivityIsRegistration_AndPatientType1_AndValidFinancialClass4_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "04" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
        //it should return true since patient type, financial class, hospital service is valid and account created after release date
        [Test]
        public void TestIMFMReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass4_AndHSV_Is35_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "04" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterPreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(account);

            Assert.IsTrue(actualResult);
        }
        //it should return false since patient type, financial class, hospital service is valid and account created before release date
        [Test]
        public void TestIMFMReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass4_AndHSV_Is35_AndCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "04" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforePreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_ADMIT;
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
        //it should return false since patient type, financial class, hospital service is invalid and account created after release  date
        [Test]
        public void TestIMFMReceivedFeatureVisible_WhenActivityIsPreRegistration_AndPatientType0_AndValidFinancialClass23_AndHSV_IsNot35_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            var COBIMFMFeatureManager = new COBReceivedAndIMFMReceivedFeatureManager();
            var financialClass = new FinancialClass { Code = "23" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateBeforePreRegistrationIMFMFeatureStart(), VisitType.PreRegistration, financialClass);
            account.HospitalService.Code = HospitalService.PRE_REGISTER;
            var actualResult = COBIMFMFeatureManager.IsIMFMReceivedEnabledForAccount(account);

            Assert.IsFalse(actualResult);
        }
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
