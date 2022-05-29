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
    public class GuarantorDateOfBirthFeaturemanagerTests
    {
        [SetUp]
        public void SetUpFeatureManager()
        {
            guarantorDateOfBirthFeaturemanager = new GuarantorDateOfBirthFeatureManager();
        }

        public void TestCanBeAppliedTo_WithNullContext_ShouldReturnFalse()
        {

            var actualResult = guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(null);
            Assert.IsFalse(actualResult);
        }
      
        [Test]
        public void TestRegistrationGuarantorDOBVisible_AccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), null, null);
            Assert.IsTrue(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }
        
        [Test]
        public void TestPreRegistrationGuarantorDOBVisible_AccountCreatedAfterReleaseDate_ShouldReturnTrue()
        {
            var account = GetAccount(new PreRegistrationActivity(), GetTestDateAfterFeatureStart(), null, null);
            Assert.IsTrue(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }

        [Test]
        public void TestShortRegistrationGuarantorDOBVisible_AccountCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortRegistrationActivity(), GetTestDateBeforeFeatureStart(), null, null);
            Assert.IsFalse(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }
        
        [Test]
        public void TestShortPreRegistrationGuarantorDOBVisible_AccountCreatedBeforeReleaseDate_ShouldReturnFalse()
        {
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateBeforeFeatureStart(), null, null );
            Assert.IsFalse(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }
        [Test]
        public void TestShortPreRegistrationGuarantorDOBVisible_AccountCreatedAfterReleaseDate_InsuranceIsNull_GuarantorIsEmployee_ShouldReturnTrue()
        {
         var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
             
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart(), null, guarantor);
            Assert.IsTrue(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }
        [Test]
        public void TestShortPreRegistrationGuarantorDOBVisible_AccountCreatedAfterReleaseDate_InsuranceIsWorkersCompensation_GuarantorNull_ShouldReturnTrue()
        {
            
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, null);
            Assert.IsTrue(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }

        [Test]
        public void
            TestShortPreRegistrationGuarantorDOBVisible_AccountCreatedAfterReleaseDate_InsuranceIsWorkersCompensation_GuarantorIsEmployee_ShouldReturnFalse
            ()
        {
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());

            var account = GetAccount(new ShortPreRegistrationActivity(), GetTestDateAfterFeatureStart(), insurance,
                guarantor);
            Assert.IsFalse(guarantorDateOfBirthFeaturemanager.ShouldWeEnableGuarantorDateOfBirthFeature(account));
        }

        private static Guarantor GetGuarantor(string relType )
        {
            var guarantor = new Guarantor();
           
                var relationShip =
                    new Relationship(
                        new RelationshipType(0, DateTime.MinValue, "Employee", relType),
                        guarantor.GetType(), new Employer().GetType());

                guarantor.AddRelationship(relationShip);
                return guarantor;
            
        }

        #region Support Methods
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
        private DateTime GetTestDateAfterFeatureStart()
        {
            return guarantorDateOfBirthFeaturemanager.FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return guarantorDateOfBirthFeaturemanager.FeatureStartDate.AddDays(-10);
        }
        #endregion

        #region constants

        private GuarantorDateOfBirthFeatureManager guarantorDateOfBirthFeaturemanager;

        #endregion

    }

}
