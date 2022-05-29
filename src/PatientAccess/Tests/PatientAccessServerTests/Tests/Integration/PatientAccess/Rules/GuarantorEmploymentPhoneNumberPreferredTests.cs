using System;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Rules
{
    [TestFixture]
    [Category( "Fast" )]
    public class GuarantorEmploymentPhoneNumberPreferredTests
    {
        #region Test Methods

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }
        
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipsAreZero_ShouldReturnTrue()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            var account = new Account { Activity = new RegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsEmployee_ShouldReturnTrue()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            RelationshipType relType = new RelationshipType {Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE};
              Relationship newRelationship = new Relationship( relType,
                    new Patient().GetType(),guarantor.GetType() );
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new RegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsSpouse_ShouldReturnTrue()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new RegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsEmployeeAndEmploymentStatusIsNotEmployed_ShouldReturnTrue()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
             guarantor.Employment.Status = new EmploymentStatus(1L,DateTime.MinValue, EmploymentStatus.NOT_EMPLOYED_DESC,EmploymentStatus.NOT_EMPLOYED_CODE);
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new PreRegistrationActivity() , Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsEmployeeAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            guarantor.Employment.Status = new EmploymentStatus(1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE);
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_EMPLOYEE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new PreRegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPreRegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            guarantor.Employment.Status = new EmploymentStatus(1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE);
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new PreRegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsPostMSERegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployed_ShouldReturnFalse()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            guarantor.Employment.Status = new EmploymentStatus(1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE);
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new PostMSERegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }
        [Test]
        public void TestCanBeAppliedTo_WhenActivityIsRegistrationAndRelationshipIsSpouseAndEmploymentStatusIsEmployedPhoneNumberisGiven_ShouldReturnTrue()
        {
            Guarantor guarantor = new Guarantor();
            guarantor.Employment = new Employment();
            PhoneNumber phone = new PhoneNumber { AreaCode = "972", Number = "2222222" };
            ContactPoint partyContactPoint = new ContactPoint(new Address(), phone,new EmailAddress(),TypeOfContactPoint.NewEmployerContactPointType() );
            guarantor.Employment.Employer.PartyContactPoint = partyContactPoint;
            guarantor.Employment.Status = new EmploymentStatus(1L, DateTime.MinValue, EmploymentStatus.EMPLOYED_FULL_TIME_DESC, EmploymentStatus.EMPLOYED_FULL_TIME_CODE);
            RelationshipType relType = new RelationshipType { Code = RelationshipType.RELATIONSHIPTYPE_SPOUSE };
            Relationship newRelationship = new Relationship(relType,
                  new Patient().GetType(), guarantor.GetType());
            guarantor.AddRelationship(newRelationship);
            var account = new Account { Activity = new RegistrationActivity(), Guarantor = guarantor };
            var ruleUnderTest = new GuarantorEmploymentPhoneNumberPreferred();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }
        #endregion

        #region Support Methods
        #endregion
    }
}
