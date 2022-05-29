using System; 
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Domain.ShortRegistration;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    [TestFixture]
    [Category("Fast")]
    public class GuarantorDateOfBirthRequiredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WithInvalidContextType_ShouldReturnTrue()
        {
            var inValidObjectType = new object();
            var ruleUnderTest = new GuarantorDateOfBirthRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(inValidObjectType);
            Assert.IsTrue(actualResult);
        }

        [Test]
        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new GuarantorDateOfBirthRequired();
            var actualResult = ruleUnderTest.CanBeAppliedTo(null);
            Assert.IsTrue(actualResult);
        }
        [Test]
        public void TestCanBeAppliedTo_RegistrationActivity_InsuranceIsNullShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), null, new Guarantor());
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_RegistrationActivity_GuaranroIsNullShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), new Insurance(), null );
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
       
        [Test]
        public void TestCanBeAppliedTo_RegistrationActivity_ShouldReturnFalse()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), new Insurance(), new Guarantor());
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }


        [Test]
        public void TestCanBeAppliedTo_DuringShortRegistrationActivityAfterFeatureStartDate_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), new Insurance(),
                new Guarantor());
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringShortRegistrationActivityAfterFeatureStartDate_GuarantorDateIsNotEmpty_ShouldReturnTrue
            ()
        {
            var guarantor = GetGuarantor(DateTime.Today, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateAfterFeatureStart(), new Insurance(),
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringRegistrationActivityAfterFeatureStartDate_GuarantorDateIsNotEmpty_ShouldReturnTrue
            ()
        {
            var guarantor = GetGuarantor(DateTime.Today, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), new Insurance(),
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringPreAdmitNewBornActivityAfterFeatureStartDate_GuarantorDateIsNotEmpty_ShouldReturnTrue
            ()
        {
            var guarantor = GetGuarantor(DateTime.Today, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateAfterFeatureStart(), new Insurance(),
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringAdmitNewBornActivityAfterFeatureStartDate_GuarantorDateIsEmpty_ShouldReturnFalse
            ()
        {
            var guarantor = GetGuarantor(DateTime.MinValue, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var account = GetAccount(new AdmitNewbornActivity(), GetTestDateAfterFeatureStart(), new Insurance(),
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringRegistrationActivityAfterFeatureStartDate_GuarantorDateIsEmpty_InsuranceIsWorkersCompAndGuarantorIsEmployeeShouldReturnTrue
            ()
        {
            var guarantor = GetGuarantor(DateTime.MinValue, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance,
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringRegistrationActivityAfterFeatureStartDate_GuarantorDateIsNotEmpty_InsuranceIsWorkersCompAndGuarantorIsEmployeeShouldReturnTrue
            ()
        {
            var guarantor = GetGuarantor(DateTime.Today, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance,
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringRegistrationActivityAfterFeatureStartDate_GuarantorDateIsEmpty_InsuranceIsNotWorkersCompAndGuarantorIsEmployeeShouldReturnFalse
            ()
        {
            var guarantor = GetGuarantor(DateTime.MinValue, RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetCommercialCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance,
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void
            TestCanBeAppliedTo_DuringRegistrationActivityAfterFeatureStartDate_GuarantorDateIsEmpty_InsuranceIsWorkersCompAndGuarantorIsNotEmployeeShouldReturnFalse
            ()
        {
            var guarantor = GetGuarantor(DateTime.MinValue, RelationshipType.RELATIONSHIPTYPE_MOTHER);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance,
                guarantor);
            var ruleUnderTest = new GuarantorDateOfBirthRequired();

            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        #endregion

        #region Support Methods

        private static Guarantor GetGuarantor(DateTime DOB, string relType)
        {
            var guarantor = new Guarantor()
            {
                DateOfBirth = DOB
            };

            var relationShip =
                new Relationship(
                    new RelationshipType(0, DateTime.MinValue, "Relationship", relType),
                    guarantor.GetType(), new Employer().GetType());

            guarantor.AddRelationship(relationShip);
            return guarantor;
        }

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, Insurance insurance,
            Guarantor guarantor)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                Insurance = insurance,
                Guarantor = guarantor
            };
        }

        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new GuarantorDateOfBirthFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new GuarantorDateOfBirthFeatureManager().FeatureStartDate.AddDays(-10);
        }

        private Coverage GetWorkersCompensationCoverage()
        {
            var workersCompCoverage = new WorkersCompensationCoverage()
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION)
            };
         
            return workersCompCoverage;
        }

        private static Coverage GetCommercialCoverage()
        {
            var commericalCoverage = new CommercialCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
            
            };

            return commericalCoverage;
        }

        #endregion

        #region Data Elements

        private readonly PhoneNumber cellPhoneNumber = new PhoneNumber(AREA_CODE, NUMBER);

        #endregion

        #region Constants

        private const string AREA_CODE = "123";
        private const string NUMBER = "4567891";

        #endregion
    }
}
