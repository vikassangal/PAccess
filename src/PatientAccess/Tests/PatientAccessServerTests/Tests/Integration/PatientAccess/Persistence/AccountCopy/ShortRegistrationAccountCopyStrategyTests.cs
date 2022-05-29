using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Persistence.AccountCopy;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.AccountCopy
{
    [TestFixture]
    [Category( "Fast" )]
    public class ShortRegistrationAccountCopyStrategyTests
    {
        #region Constants

        private const string DHF_FACILITY_CODE = "DHF";

        #endregion

        #region SetUp and TearDown ShortRegistrationAccountCopyStrategyTests
        [TestFixtureSetUp]
        public static void SetUpShortRegistrationAccountCopyStrategyTests()
        {
            DHF_FACILITY = facilityBroker.FacilityWith(DHF_FACILITY_CODE);
        }

        [TestFixtureTearDown]
        public static void TearDownShortRegistrationAccountCopyStrategyTests()
        {
        }
        #endregion

        #region Test Methods
        [Test]
        public void TestEmailAddress_ForShortPreRegistration_ShouldCopyForwardToNewAccount()
        {
            var oldAccount = GetAccountWithEmailAddress(new ShortPreRegistrationActivity());
            var oldAccountEmailAddress =
                oldAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).EmailAddress.
                    ToString();
            var newAccountEmailAddress = GetNewAccountEmailAddress(oldAccount);
            Assert.IsTrue(newAccountEmailAddress.Equals(oldAccountEmailAddress),"Email address failed to copy forward");
        }

        [Test]
        public void TestEmailAddress_ForShortRegistration_ShouldCopyForwardToNewAccount()
        {
            var oldAccount = GetAccountWithEmailAddress(new ShortRegistrationActivity());
            var oldAccountEmailAddress =
                oldAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).EmailAddress.
                    ToString();
            var newAccountEmailAddress = GetNewAccountEmailAddress(oldAccount);
            Assert.IsTrue(newAccountEmailAddress.Equals(oldAccountEmailAddress), "Email address failed to copy forward");
        }


        #endregion

        #region Support Methods
        private Account GetAccountWithEmailAddress(Activity activity)
        {
            var accountWithEmail = new Account
            {
                AccountNumber = 111,
                Patient = new Patient(),
                Activity = activity,
                Facility = DHF_FACILITY
            };
            accountWithEmail.Patient.AddContactPoint(patientMailingContactPointWithEmail);
            return accountWithEmail;
        }

        private string GetNewAccountEmailAddress(Account oldAccount)
        {
            var newAccount = copyStrategy.CopyAccount(oldAccount);
            return newAccount.Patient.ContactPointWith(TypeOfContactPoint.NewMailingContactPointType()).EmailAddress.
                    ToString();
        }

        #endregion

        #region Properties

        #endregion

        #region Data Elements

        private readonly ShortRegistrationAccountCopyStrategy copyStrategy = new ShortRegistrationAccountCopyStrategy();
        private static Facility DHF_FACILITY = new Facility();
        private static readonly IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
        private readonly ContactPoint patientMailingContactPointWithEmail = new ContactPoint
        {
            Address = new Address("2300 W PLANO PKWY", "", "PLANO", new ZipCode("75075"), new State("TX"), new Country("USA")),
            PhoneNumber = new PhoneNumber("6786786789"),
            EmailAddress = new EmailAddress("ABC@ABC.COM"),
            TypeOfContactPoint = TypeOfContactPoint.NewMailingContactPointType()
        };

        #endregion
    }
}
