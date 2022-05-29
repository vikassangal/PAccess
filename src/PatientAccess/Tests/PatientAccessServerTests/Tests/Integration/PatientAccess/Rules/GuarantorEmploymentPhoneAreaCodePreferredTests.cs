using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class GuarantorEmploymentPhoneAreaCodePreferredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipsAreZero_ShouldReturnTrue()
        {
            Account account = GetAccountAndBlankGuarantorWith( new RegistrationActivity() );
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsEmployee_ShouldReturnTrue()
        {
            Account account = GetAccountAndGuarantorWith( new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE }, new RegistrationActivity() );
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsSpouse_ShouldReturnTrue()
        {
            var activity = new RegistrationActivity();
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE };
            
            var account = GetAccountAndGuarantorWith(relType, activity);
            
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsEmployeeAndEmploymentStatusIsNotEmployed_ShouldReturnTrue()
        {
            var account = GetAccountAndGuarantorWith( new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE }, new PreRegistrationActivity() );
            account.Guarantor.Employment.Status = new EmploymentStatus( 1L, DateTime.MinValue, EmploymentStatus.NOT_EMPLOYED_DESC, EmploymentStatus.NOT_EMPLOYED_CODE );

            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();

            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsEmployeeAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            var account = GetAccountAndGuarantorWith(new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE }, new PreRegistrationActivity());
            account.Guarantor.Employment.Status = new EmploymentStatus( 1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );

            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();

            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            var account = GetAccountAndGuarantorWith(new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE }, new PreRegistrationActivity());
            account.Guarantor.Employment.Status = new EmploymentStatus( 1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
            
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSERegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            var account = GetAccountAndGuarantorWith(new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE }, new PostMSERegistrationActivity());
            account.Guarantor.Employment.Status = new EmploymentStatus( 1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployedPhoneNumberisGiven_ShouldReturnTrue()
        {
            var account = GetAccountAndGuarantorWith(new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE }, new RegistrationActivity());
            PhoneNumber phone = new PhoneNumber { AreaCode = "972", Number = "2222222" };
            ContactPoint partyContactPoint = new ContactPoint( new Address(), phone, new EmailAddress(), TypeOfContactPoint.NewEmployerContactPointType() );
            account.Guarantor.Employment.Employer.PartyContactPoint = partyContactPoint;
            account.Guarantor.Employment.Status = new EmploymentStatus( 1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE );
            
            var ruleUnderTest = new GuarantorEmploymentPhoneAreaCodePreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion
        #region Support Methods
        private static Account GetAccountAndGuarantorWith( RelationshipType relType, Activity activity )
        {
            var account = GetAccountAndBlankGuarantorWith(activity);
            Relationship newRelationship = new Relationship( relType, new Patient().GetType(), account.Guarantor.GetType() );
            account.Guarantor.AddRelationship( newRelationship );

            return account;
        }

        private static Account GetAccountAndBlankGuarantorWith( Activity activity )
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            return new Account { Activity = activity, Guarantor = guarantor };
        }

        #endregion
    }
}
