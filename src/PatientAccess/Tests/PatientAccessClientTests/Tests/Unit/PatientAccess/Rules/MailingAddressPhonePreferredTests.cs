using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{

    [TestFixture]
    [Category("Fast")]
    public class MailingAddressPhonePreferredTests
    {
        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new MailingAddressPhonePreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCanBeAppliedTo_PhoneNumberIsRequiredWhenAreaCodeIsNotNullOrEmpty_ShouldReturnTrue()
        {
            var account = GetAccount(new ShortRegistrationActivity());
            account.KindOfVisit = VisitType.Outpatient;
            var ruleUnderTest = new MailingAddressPhonePreferred();
            TypeOfContactPoint mailingType = new TypeOfContactPoint(TypeOfContactPoint.MAILING_OID, "Mailing");
            ContactPoint mailingContactPoint = account.Patient.ContactPointWith(mailingType);
            mailingContactPoint.PhoneNumber.AreaCode = "011";
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_PhoneNumberIsNotRequiredWhenAreaCodeIsNullOrEmpty_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity());
            account.KindOfVisit = VisitType.Outpatient;
            var ruleUnderTest = new MailingAddressPhonePreferred();
            TypeOfContactPoint mailingType = new TypeOfContactPoint(TypeOfContactPoint.MAILING_OID, "Mailing");
            ContactPoint mailingContactPoint = account.Patient.ContactPointWith(mailingType);
            mailingContactPoint.PhoneNumber.AreaCode = string.Empty;
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #region Support Methods
        private static Account GetAccount(Activity activity)
        {
            return new Account
            {
                Activity = activity,
               
            };
        }
        #endregion

    }
}
