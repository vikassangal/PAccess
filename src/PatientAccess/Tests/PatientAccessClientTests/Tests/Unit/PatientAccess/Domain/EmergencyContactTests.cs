using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class EmergencyContactTests
    {
        #region Constants
        
        #endregion

        #region SetUp and TearDown EmergencyContactTests
        [TestFixtureSetUp()]
        public static void SetUpEmergencyContactTests()
        {
            
        }

        [TestFixtureTearDown()]
        public static void TearDownEmergencyContactTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestEmergencyContactTests()
        {
            Patient patient =  new Patient(
                PATIENT_OID, 
                Patient.NEW_VERSION, 
                this.PATIENT_NAME, 
                PATIENT_MRN, 
                this.PATIENT_DOB, 
                this.PATIENT_SSN, 
                this.PATIENT_SEX,
                this.FACILITY
                );   

            PhoneNumber pPhoneNumber = new PhoneNumber( "9725778888" );
            ContactPoint cp = new ContactPoint();
            cp.PhoneNumber = pPhoneNumber ;
            patient.AddContactPoint(cp); 
         
            Account anAccount = new Account();
            anAccount.AccountNumber = 5336040;
            anAccount.Patient = patient;            


            EmergencyContact ec = new EmergencyContact() ;
            ec.Name = "Alan Smith" ;           
            ContactPoint  ecp = new ContactPoint() ;
            ecp.Address = this.address ;
            ecp.PhoneNumber = new PhoneNumber("8005235800");
            ecp.TypeOfContactPoint = new TypeOfContactPoint(0L,"work");
            ec.AddContactPoint(ecp);      
            Relationship relationship = new Relationship((new RelationshipType(0L,DateTime.Now,"Spouse")),patient.GetType(), ec.GetType());
            ec.AddRelationship(relationship);
            anAccount.EmergencyContact1 = ec ;
            Assert.AreEqual(
                anAccount.EmergencyContact1.Name,
                "Alan Smith");
            foreach( Relationship r in anAccount.EmergencyContact1.Relationships)
            {
                Assert.AreEqual(
                    r.Type.Description , "Spouse");
               
            }
            foreach( ContactPoint c in anAccount.EmergencyContact1.ContactPoints)
            {
                Assert.AreEqual(
                    c.TypeOfContactPoint.Description , "work");
               
            }
            
            

           
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private const string
            PATIENT_F_NAME  = "Sam",
            PATIENT_L_NAME  = "Spade",
            PATIENT_MI      = "L" ;
        private readonly SocialSecurityNumber
            PATIENT_SSN     = new SocialSecurityNumber( "123121234" );
        private const long
            PATIENT_OID     = 45L,
            PATIENT_MRN     = 123456789;
        private readonly Name
            PATIENT_NAME    = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private readonly DateTime
            PATIENT_DOB     = new DateTime( 1955, 3, 5 );
        private readonly Gender
            PATIENT_SEX     = new Gender( 0, DateTime.Now, "Male", "M" );
        private const string
            FACILILTY_NAME = "DELRAY TEST HOSPITAL",
            FACILITY_CODE = "DEL";
        private readonly Facility
            FACILITY        = new Facility(PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           FACILILTY_NAME,
                                           FACILITY_CODE  );

        private const string ADDRESS1 = "335 Nicholson Dr.",
                             ADDRESS2 = "#303",
                             CITY = "Austin",    
                             POSTALCODE = "60505";    

        private readonly Address address = new Address( ADDRESS1,
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
        #endregion
    }
}