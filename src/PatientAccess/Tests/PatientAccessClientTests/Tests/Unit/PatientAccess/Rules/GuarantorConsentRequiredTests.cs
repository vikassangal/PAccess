using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for GuarantorConsentRequiredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class GuarantorConsentRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_ForRegistrationActivity_WithCellPhone_BlankConsent_ShouldReturnFalse()
        {
            var account = new Account();
            var guarantor = GetGuarantor(new RegistrationActivity(), String.Empty);
            account.Guarantor = guarantor;
            account.AdmitDate = DateTime.MinValue;
            var cp = guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            cp.PhoneNumber = cellPhoneNumber;
            var ruleUnderTest = new GuarantorConsentRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsFalse( result );
        }

        [Test]
        public void TestCanBeAppliedTo_ForRegistrationActivity_WithoutCellPhone_BlankConsent_ShouldReturnFalse()
        {
            var account = new Account();
            var guarantor = GetGuarantor(new RegistrationActivity(), String.Empty);
            account.Guarantor = guarantor;
            account.AdmitDate = DateTime.MinValue;
            var cp = guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            cp.PhoneNumber = new PhoneNumber();
            var ruleUnderTest = new GuarantorConsentRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestCanBeAppliedTo_ForPreRegistrationActivity_WithoutCellPhone_BlankConsent_ShouldReturnFalse()
        {
            var account = new Account();
            var guarantor = GetGuarantor(new PreRegistrationActivity(), String.Empty);
            account.Guarantor = guarantor;
            account.AdmitDate = DateTime.MinValue;
            var cp = guarantor.ContactPointWith(TypeOfContactPoint.NewMobileContactPointType());
            cp.PhoneNumber = new PhoneNumber();
            var ruleUnderTest = new GuarantorConsentRequired();
            var result = ruleUnderTest.CanBeAppliedTo(account);
            Assert.IsFalse(result);
        }

       #endregion

        #region Support Methods

        private static Guarantor GetGuarantor(Activity activity, String consentCode)
        {
            var account = new Account
            {
                Activity = activity,
                KindOfVisit = VisitType.Inpatient
            };

            var guarantor = new Guarantor();
            account.Guarantor = guarantor;
            return guarantor;

        }

        #endregion

        #region Data Elements

        private  readonly  PhoneNumber cellPhoneNumber = new PhoneNumber(AREA_CODE,NUMBER);

        #endregion

        #region Constants

        private const string AREA_CODE = "123";
        private const string NUMBER = "4567891";

        #endregion
    }
}
