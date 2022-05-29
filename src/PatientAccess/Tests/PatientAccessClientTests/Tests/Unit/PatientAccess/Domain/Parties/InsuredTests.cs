using System;
using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class InsuredTests
    {
        #region Constants
        const string ADDRESS1 = "335 Nicholson Dr.",
                     ADDRESS2 = "32321",
                     CITY = "Austin",    
                     POSTALCODE = "60505";   
        #endregion

        #region SetUp and TearDown InsuredTests
        [TestFixtureSetUp()]
        public static void SetUpInsuredTests()
        {
            address = new Address( ADDRESS1,
                                   ADDRESS2,
                                   CITY,
                                   new ZipCode( POSTALCODE ),
                                   new State( 0L,
                                              ReferenceValue.NEW_VERSION,
                                              "TEXAS",
                                              "TX"),
                                   new Country( 0L,
                                                ReferenceValue.NEW_VERSION,
                                                "United States",
                                                "USA"),
                                   new County( 0L,
                                               ReferenceValue.NEW_VERSION,
                                               "ORANGE",
                                               "122")
                ); 
          
            aPhoneNumber = new PhoneNumber("1234567890");
        }

        [TestFixtureTearDown()]
        public static void TearDownInsuredTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestDeepCopy()
        {
            Name insuredsName = new Name( "Joe", "Guarantor", "D." );
            Insured  insured  = new Insured( Insured.NEW_OID, Insured.NEW_VERSION, insuredsName );
            RelationshipType RType = new RelationshipType( 0, DateTime.MinValue, "Parent" );
            Assert.AreEqual(
                "Parent",
                RType.Description
                );
       
            ContactPoint cp = new ContactPoint();
            cp.PhoneNumber = aPhoneNumber ;
            cp.Address = address;
            insured.AddContactPoint(cp);
            insured.AddRelationship( new Relationship( RType, insured.GetType(), insured.GetType() ) );
       
            Insured copy = insured.DeepCopy() as Insured;
            ArrayList    relationships = copy.FindRelationships(RType);
            
            Assert.IsNotNull( relationships );
          
            Assert.IsNotNull(
                copy,
                "If copy is null then DeepCopy failed or casting the copy to the correct type failed"
                );
            Assert.AreEqual(
                insured.ContactPoints.Count,
                copy.ContactPoints.Count
                );

           
            Assert.AreEqual(
                insured.Name.FirstName,
                copy.Name.FirstName
                );
           

        }

        [Test()]
        public void TestPatientCopyAsInsured_WhenPatientHasEmailAddress_TheEmailAddressShouldBeReset()
        {
            Patient patient = new Patient
            {
                Name = new Name(0L, DateTime.Now, "Perot Systems", "Test", "J"),
                SocialSecurityNumber = new SocialSecurityNumber("123456789")
            };

            Address address = new Address(ADDRESS1,
                                          ADDRESS2,
                                          CITY,
                                          new ZipCode(POSTALCODE),
                                          new State(0L,
                                                    ReferenceValue.NEW_VERSION,
                                                    "TEXAS",
                                                    "TX"),
                                          new Country(0L,
                                                      ReferenceValue.NEW_VERSION,
                                                      "United States",
                                                      "USA"),
                                          new County(0L,
                                                     ReferenceValue.NEW_VERSION,
                                                     "ORANGE",
                                                     "122")
                );
            patient.AddContactPoint(new ContactPoint(TypeOfContactPoint.NewMailingContactPointType())
                {
                    Address = address,
                    PhoneNumber = new PhoneNumber {AreaCode = "123", Number = "4567890"},
                    EmailAddress = new EmailAddress("AA@ABC.COM")
                }
                );

            var insured = patient.CopyAsInsured();
            var cp = insured.ContactPointWith(TypeOfContactPoint.NewPhysicalContactPointType());
            Assert.IsEmpty(cp.EmailAddress.ToString().Trim());
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static Address address;
        private static PhoneNumber aPhoneNumber;
        #endregion
    }
}