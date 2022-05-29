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
    public class COBReceivedRequiredTests
    {
        #region Default Scenario

        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new COBReceivedRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);

            Assert.IsTrue(actualResult);
        }
        
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new COBReceivedRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);

            Assert.IsTrue(actualResult);
        }

        #endregion

        #region PatientType0 and Invalid Financial Class

        //it should return true since patient type or financial class or activity is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistration_AndPatientType0_AndFinancialClass10_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "10" };

            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.PreRegistration, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType0 Invalid Financial Class

        #region PatientType1 and Valid Financial Class

        //it should return false since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass14_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "14" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired(); 

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Valid Financial Class

        #region PatientType1 and Valid Financial Class and Valid Activity

        //it should return false since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsAdmitNewborn_AndPatientType1_AndFinancialClass14_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "14" };

            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Valid Financial Class and Valid Activity

        #region PatientType1 and Valid Financial Class

        //it should return false since since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass14_AndCreatedBeforeReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "14" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Valid Financial Class

        #region PatientType1 and Valid Financial Class and COB Received Selected

        //it should return true because Patient Type , Financial class is valid and COB Received seleccted as Yes
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass14_AndCOBAsYes_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Yes;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "14" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 and Valid Financial Class

        #region PatientType1 and Invalid Financial Class

        //it should return true since patient type and financial class is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType1_AndFinancialClass13_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "13" };

            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType1 Invalid Financial Class

        #region PatientType2 and Valid Financial Class

        //it should return false since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType2_AndFinancialClass20_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "20" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType2 and Valid Financial Class

        #region PatientType2 and Invalid Financial Class

        //it should return true since COB Received is required for Registration activity but financial class is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistration_AndPatientType2_AndFinancialClass20_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "23" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType2 and Invalid Financial Class

        #region PatientType3 and Valid Financial Class

        //it should return false since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsShortRegistration_AndPatientType3_AndFinancialClass80_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "80" };

            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType3 and Valid Financial Class

        #region PatientType3 and Invalid Financial Class

        //it should return true since patient type , financial class and activity is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsShortRegistration_AndPatientType3_AndFinancialClass26_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "26" };

            var account = GetAccount(new PostMSERegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Emergency, financialClass);  
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType3 and Invalid Financial Class

        #region PatientType4 and Valid Financial Class

        //it should return false since since patient type , financial class and activity is valid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsShortRegistration_AndPatientType4_AndFinancialClass81_AndCreatedAfterReleaseDate_ShouldReturnFalse()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "81" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType4 and Valid Financial Class

        #region PatientType4 and Invalid Financial Class

        //it should return true since patient type , financial class and activity is invalid
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsShortRegistration_AndPatientType4_AndFinancialClass26_AndCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            YesNoFlag COBReceived = YesNoFlag.Blank;
            YesNoFlag IMFMReceived = YesNoFlag.Blank;
            var financialClass = new FinancialClass { Code = "40" };

            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Recurring, financialClass);
            account.COBReceived = COBReceived;
            account.IMFMReceived = IMFMReceived;
            var ruleUnderTest = new COBReceivedRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion PatientType4 and Invalid Financial Class

        #region Support Methods

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit , FinancialClass financialClass)
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
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays( 10 );
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new COBReceivedAndIMFMReceivedFeatureManager().FeatureStartDate.AddDays(-10);
        }

       #endregion
        

    }
}
