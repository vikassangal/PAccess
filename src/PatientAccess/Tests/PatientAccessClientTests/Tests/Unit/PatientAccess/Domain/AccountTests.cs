using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for AccountTests.
    /// </summary>

    [TestFixture]
    public class AccountTests
    {
        #region Constants
        private const long SAUL_FESTINGER_ACCT_NUM = 5208513L;

        private const int
            MAX_CONSULT_PHYS = 5;
        private readonly static Address someAddress = new Address("123 street", "somestrert",
                                                            "asdfffCity",
                                                            new ZipCode("12332"),
                                                            new State(0L,
                                                                      PersistentModel.NEW_VERSION,
                                                                      "TEXAS",
                                                                      "TX"),
                                                            new Country(0L,
                                                                        PersistentModel.NEW_VERSION,
                                                                        "United States",
                                                                        "USA"),
                                                            new County(0L,
                                                                       PersistentModel.NEW_VERSION,
                                                                       "ORANGE",
                                                                       "122")
             );
        private readonly ContactPoint newMailing = new ContactPoint(TypeOfContactPoint.NewMailingContactPointType())
        {
            Address = someAddress,
            PhoneNumber = new PhoneNumber("1234567890"),
            CellPhoneNumber = new PhoneNumber("9393939393"),
            EmailAddress = new EmailAddress("emailaddress")
        };

        private readonly ContactPoint newPhysical = new ContactPoint(TypeOfContactPoint.NewPhysicalContactPointType())
        {
            Address = someAddress,
            PhoneNumber = new PhoneNumber("1234567890"),
            CellPhoneNumber = new PhoneNumber("9393939393"),
            EmailAddress = new EmailAddress("emailaddress")
        };

        #endregion

        #region SetUp and TearDown AccountTests

        [TestFixtureSetUp]
        public static void SetUpAccountTests()
        {
        }

        [SetUp]
        public void SetUpAccounts()
        {
            DefaultAccount = new Account { AccountNumber = SAUL_FESTINGER_ACCT_NUM };
        }
        #endregion

        #region Test Methods

        [Test]
        [Category( "Fast" )]
        public void TestConstructors()
        {
            Account anAccount = new Account( PersistentModel.NEW_OID );
            Assert.AreEqual( PersistentModel.NEW_OID, anAccount.Oid,
                String.Format( "Account OID should be {0}", PersistentModel.NEW_OID ) );
        }

        [Test]
        [Category( "Fast" )]
        public void testMedicalGroupIPA()
        {
            Account anAccount = new Account( PersistentModel.NEW_OID );

            MedicalGroupIPA aMGIPA = new MedicalGroupIPA { Code = "A1234", Name = "Pain Medical Group" };
            Clinic clinic1 = new Clinic();
            Clinic clinic2 = new Clinic();

            clinic1.Code = "01";
            clinic1.Name = "Alpha Clinic";

            clinic2.Code = "02";
            clinic2.Name = "Body Blows Clinic";

            aMGIPA.AddClinic( clinic1 );
            aMGIPA.AddClinic( clinic2 );

            Assert.AreEqual( 2, aMGIPA.Clinics.Count, "MedicalGroupIPA has  has two Clinics" );

            foreach ( Clinic c in aMGIPA.Clinics )
            {
                string formattedString = String.Format(
                    "{0}{1}{2}{3}", c.Name, "    ", c.Code, Environment.NewLine );

                Console.WriteLine( formattedString );
            }

            anAccount.MedicalGroupIPA = aMGIPA;

            Assert.AreEqual( 2, anAccount.MedicalGroupIPA.Clinics.Count,
                "MedicalGroupIPA for this Account has  has two Clinics" );

            foreach ( Clinic c in anAccount.MedicalGroupIPA.Clinics )
            {
                string formattedString = String.Format(
                    "{0}{1}{2}{3}", c.Name, "    ", c.Code, Environment.NewLine );

                Console.WriteLine( formattedString );
            }

            Assert.AreEqual( "Pain Medical Group", anAccount.MedicalGroupIPA.Name,
                "MedicalGroupIPA.Name for this Account" );
            Assert.AreEqual( aMGIPA.Code = "A1234", anAccount.MedicalGroupIPA.Code,
                "MedicalGroupIPA.Code for this Account" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestGuarantorIsAndRelationshipWith()
        {
            Patient patient = new Patient( PATIENT_OID, PersistentModel.NEW_VERSION, PATIENT_NAME,
                PATIENT_MRN, PATIENT_DOB, PATIENT_SSN, PATIENT_SEX, FACILITY );

            PhoneNumber pPhoneNumber = new PhoneNumber( "9725778888" );
            ContactPoint cp = new ContactPoint { PhoneNumber = pPhoneNumber };
            patient.AddContactPoint( cp );

            Account anAccount = new Account { AccountNumber = 5336040, Patient = patient };

            Name guarantorName = new Name( GUARANTOR_F_NAME, GUARANTOR_L_NAME, GUARANTOR_MI );
            Guarantor guarantor = new Guarantor( 1L, PersistentModel.NEW_VERSION, guarantorName );
            PhoneNumber gPhoneNumber = new PhoneNumber( "9725336666" );
            ContactPoint gp = new ContactPoint { PhoneNumber = gPhoneNumber };
            guarantor.AddContactPoint( gp );

            RelationshipType aRelationshipType = new RelationshipType(
                51L, PersistentModel.NEW_VERSION, "NATURAL CHILD", "03" );

            anAccount.GuarantorIs( guarantor, aRelationshipType );

            foreach ( Relationship r in patient.Relationships )
            {
                Assert.AreEqual( "NATURAL CHILD", r.Type.Description,
                    "Patient is the guarantor's NATURAL CHILD" );
            }

            RelationshipType aRelationshipType2 = null;
            aRelationshipType2 = patient.RelationshipWith( guarantor );

            Assert.AreEqual( "NATURAL CHILD", aRelationshipType2.Description,
                "Patient is the guarantor's NATURAL CHILD" );

            Assert.AreEqual( "03", aRelationshipType2.Code,
                "Patient is the guarantor's relationshiptype code 03" );

            RelationshipType aRelationshipType3 = null;
            aRelationshipType3 = patient.RelationshipWith( patient );

            Assert.AreEqual( null, aRelationshipType3, "no relationship exists" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestParties()
        {
            Account anAccount = new Account( PersistentModel.NEW_OID );
            Console.WriteLine( "DictionaryEntry.key for Parties()" );

            Guarantor guarantor = new Guarantor();

            CoverageOrder primary = new CoverageOrder( 1, "Primary" );
            Assert.AreEqual( primary.Description, "Primary" );
            CoverageOrder secondary = new CoverageOrder( 2, "Secondary" );
            Assert.AreEqual( secondary.Description, "Secondary" );

            CommercialCoverage coverage1 = new CommercialCoverage { CoverageOrder = primary, Oid = 1 };

            Assert.AreEqual( coverage1.CoverageOrder, primary );
            Assert.AreEqual( coverage1.Oid, 1 );

            GovernmentMedicareCoverage coverage2 = new GovernmentMedicareCoverage { CoverageOrder = secondary, Oid = 2 };

            Assert.AreEqual( coverage2.CoverageOrder, secondary );
            Assert.AreEqual( coverage1.Oid, 1 );
            BillingInformation aBillingInformation = new BillingInformation( address, new PhoneNumber( "8005235800" ),
                new EmailAddress( "Bill.Person@claims.net" ), new TypeOfContactPoint( 0L, "work" ) ) 
                { BillingCOName = "Medicare Part A Claims" };

            coverage1.Authorization.AuthorizationNumber = "12122";
            coverage1.AuthorizingPerson = "bill person 1";
            coverage1.BillingInformation = aBillingInformation;

            Insurance insurance = new Insurance();
            insurance.AddCoverage( coverage1 );
            Assert.AreEqual( insurance.Coverages.Count, 1 );
            insurance.AddCoverage( coverage2 );
            Assert.AreEqual( insurance.Coverages.Count, 2 );

            anAccount.Insurance = insurance;
            Patient aPatient = new Patient();
            Insured insured = new Insured();
            Employment emp = new Employment( insured );

            Employer empr = new Employer { Name = "qwert" };
            emp.Employer = empr;
            aPatient.Employment = emp;
            aPatient.AddAccount( anAccount );
            anAccount.Patient = aPatient;

            IDictionary partiesForInsured = anAccount.PartiesForCopyingTo( insured.GetType() );
            IDictionary partiesForGuarantor = anAccount.PartiesForCopyingTo( guarantor.GetType() );
            Assert.IsTrue( partiesForInsured.Contains( "Guarantor" ) );
            Assert.IsTrue( !partiesForGuarantor.Contains( "Guarantor" ) );
            Console.WriteLine( "DictionaryEntry.key for PartiesForCopying(Insured)" );
            foreach ( DictionaryEntry entry in partiesForInsured )
            {
                Console.WriteLine( entry.Key );
                Console.WriteLine( entry.Value );

                Assert.IsNotNull( entry );
            }
            Console.WriteLine( "DictionaryEntry.key for PartiesForCopying(Guarantor)" );
            foreach ( DictionaryEntry entry in partiesForGuarantor )
            {
                Console.WriteLine( entry.Key );
                Console.WriteLine( entry.Value );

                Assert.IsNotNull( entry );
            }
        }

        [Test]
        [Category( "Fast" )]
        public void TestEmergencyContactTests()
        {
            Patient patient = new Patient( PATIENT_OID, PersistentModel.NEW_VERSION, PATIENT_NAME, 
                PATIENT_MRN, PATIENT_DOB, PATIENT_SSN, PATIENT_SEX, FACILITY );

            PhoneNumber pPhoneNumber = new PhoneNumber( "9725778888" );
            ContactPoint cp = new ContactPoint { PhoneNumber = pPhoneNumber };
            patient.AddContactPoint( cp );

            Account anAccount = new Account { AccountNumber = 5336040, Patient = patient };

            EmergencyContact ec = new EmergencyContact { Name = "Alan Smith" };
            ContactPoint ecp = new ContactPoint
                {   Address = address,
                    PhoneNumber = new PhoneNumber( "8005235800" ),
                    TypeOfContactPoint = new TypeOfContactPoint( 0L, "work" )
                };
            ec.AddContactPoint( ecp );
            Relationship relationship = new Relationship( ( new RelationshipType( 0L, DateTime.Now, "Spouse" ) ), patient.GetType(), ec.GetType() );
            ec.AddRelationship( relationship );
            anAccount.EmergencyContact1 = ec;
            Assert.AreEqual( anAccount.EmergencyContact1.Name, "Alan Smith" );
            foreach ( Relationship r in anAccount.EmergencyContact1.Relationships )
            {
                Assert.AreEqual( r.Type.Description, "Spouse" );
            }

            foreach ( ContactPoint c in anAccount.EmergencyContact1.ContactPoints )
            {
                Assert.AreEqual( c.TypeOfContactPoint.Description, "work" );
            }
        }

        [Test]
        [Category( "Fast" )]
        public void TestDemographicsTests()
        {
            Patient patient = new Patient( PATIENT_OID, PersistentModel.NEW_VERSION, PATIENT_NAME,
                PATIENT_MRN, PATIENT_DOB, PATIENT_SSN, PATIENT_SEX, FACILITY );

            PhoneNumber pPhoneNumber = new PhoneNumber( "9725778888" );
            ContactPoint cp = new ContactPoint { PhoneNumber = pPhoneNumber };
            patient.AddContactPoint( cp );

            Account anAccount = new Account { AccountNumber = 5336040, Patient = patient };

            anAccount.Patient.SocialSecurityNumber = new SocialSecurityNumber( "908744833" )
                { SSNStatus = new SocialSecurityNumberStatus( 0L, DateTime.Now, "Known" ) };
            anAccount.Patient.NationalID = "001";

            anAccount.Patient.DriversLicense = new DriversLicense( "72368223828", new State( 0L, DateTime.Now, "Texas" ) );
            anAccount.Patient.Passport = new Passport( "123456789", new Country( "AUS", "Australia" ) );

            anAccount.ClergyVisit.SetYes( "yes" );

            Assert.AreEqual( anAccount.Patient.SocialSecurityNumber.SSNStatus.Description, "Known" );

            Assert.AreEqual( anAccount.Patient.SocialSecurityNumber.AreaNumber, "908" );
            Assert.AreEqual( anAccount.Patient.NationalID, "001" );

            Assert.AreEqual( anAccount.Patient.DriversLicense.Number, "72368223828" );

            Assert.AreEqual( anAccount.Patient.Passport.Number, "123456789" );

            Assert.AreEqual( anAccount.ClergyVisit.Description, "yes" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestDiagnosis()
        {
            Account anAccount = new Account();
            Diagnosis diagnosis = new Diagnosis();
            Illness illness = new Illness { Onset = DateTime.Parse( "Jan 21, 2005" ) };
            diagnosis.ChiefComplaint = "Soar Throat";
            diagnosis.Condition = illness;
            anAccount.Diagnosis = diagnosis;
            Illness patientCond = ( Illness )anAccount.Diagnosis.Condition;
            Assert.AreEqual( patientCond.Onset.Day, 21 );
            Assert.AreEqual( typeof( Illness ), anAccount.Diagnosis.Condition.GetType() );

            Accident accident = new Accident
                {   OccurredOn = DateTime.Parse( "Jan 21, 2005" ),
                    OccurredAtHour = "11:00",
                    Country = new Country( 0L, DateTime.Now, "USA" ),
                    State = new State( 0L, DateTime.Now, "Texas" ),
                    Kind = new TypeOfAccident( 0L, DateTime.Now, "Auto" )
                };
            diagnosis.Condition = accident;
            anAccount.Diagnosis = diagnosis;
            Accident acdt = anAccount.Diagnosis.Condition as Accident;
            Assert.IsNotNull( acdt );
            Assert.AreEqual( acdt.OccurredOn.Day, 21 ); 
            Assert.AreEqual( acdt.OccurredAtHour, "11:00" );
            Assert.AreEqual( acdt.Country.Description, "USA" );
            Assert.AreEqual( acdt.State.Description, "Texas" );
            Assert.AreEqual( acdt.Kind.Description, "Auto" );
            Assert.AreEqual( typeof( Accident ), anAccount.Diagnosis.Condition.GetType() );

            Pregnancy pregnancy = new Pregnancy { LastPeriod = DateTime.Parse( "Jan 21, 2005" ) };
            diagnosis.Condition = pregnancy;
            anAccount.Diagnosis = diagnosis;
            Pregnancy preg = anAccount.Diagnosis.Condition as Pregnancy;
            Assert.IsNotNull( preg );
            Assert.AreEqual( preg.LastPeriod.Day, 21 );
            Assert.AreEqual( typeof( Pregnancy ), anAccount.Diagnosis.Condition.GetType() );

            Crime crime = new Crime();
            diagnosis.Condition = crime;
            anAccount.Diagnosis = diagnosis;
            Crime cr = anAccount.Diagnosis.Condition as Crime;

            Assert.IsNotNull( cr );
            Assert.AreEqual( typeof( Crime ), anAccount.Diagnosis.Condition.GetType() );

            UnknownCondition unknown = new UnknownCondition();
            diagnosis.Condition = unknown;
            anAccount.Diagnosis = diagnosis;
            UnknownCondition unk = anAccount.Diagnosis.Condition as UnknownCondition;

            Assert.IsNotNull( unk );
            Assert.AreEqual( typeof( UnknownCondition ), anAccount.Diagnosis.Condition.GetType() );
        }

        [Test]
        [Category( "Fast" )]
        public void TestMOD10()
        {
            Assert.AreEqual( Account.Mod10( 3112 ), 0 );
            Assert.AreEqual( Account.Mod10( 3116 ), 1 );
            Assert.AreEqual( Account.Mod10( 3234 ), 2 );
            Assert.AreEqual( Account.Mod10( 3233 ), 4 );
            Assert.AreEqual( Account.Mod10( 3039 ), 5 );
            Assert.AreEqual( Account.Mod10( 3010 ), 6 );
            Assert.AreEqual( Account.Mod10( 3038 ), 7 );
        }

        [Test]
        [Category( "Fast" )]
        public void TestMOD11()
        {
            Assert.AreEqual( Account.Mod11( 32342 ), 0 );
            Assert.AreEqual( Account.Mod11( 466484 ), 1 );
            Assert.AreEqual( Account.Mod11( 407460 ), 2 );
            Assert.AreEqual( Account.Mod11( 520851 ), 3 );
            Assert.AreEqual( Account.Mod11( 476244 ), 4 );
            Assert.AreEqual( Account.Mod11( 520555 ), 7 );
            Assert.AreEqual( Account.Mod11( 466484 ), 1 );
        }

        [Test]
        [Category( "Fast" )]
        public void TestIsValidAccountNumber()
        {
            Facility facility = FACILITY;
            int modType = ( int )facility.ModType;

            bool result = Account.IsValidAccountNumber( modType, 4477677 );
            Assert.AreEqual( true, result, "should be valid" );

            bool result1 = Account.IsValidAccountNumber( FACILITY, 4477677 );
            Assert.AreEqual( true, result1, "should be valid" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddAdmittingPhysicianRelationship()
        {
            //Admitting Physician
            Physician admittingPhysician = new Physician
                {   FirstName = "Admitting",
                    LastName = "Physician",
                    PhysicianNumber = 1L
                };
            PhysicianRelationship admRelationship = new PhysicianRelationship(
                PhysicianRole.Admitting(), admittingPhysician );

            DefaultAccount.AddPhysicianRelationship( admRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                "There should be one physician relationship" );

            admRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician, "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, "Admitting",
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( admRelationship.Physician.LastName, "Physician",
                             "Admitting Physician's last name does not match" );
            Assert.AreEqual( admRelationship.Physician.PhysicianNumber, 1L,
                             "Admitting Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Admitting().Role(), admRelationship.PhysicianRole.Role(),
                "Admitting Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddAttendingPhysicianRelationship()
        {
            //Attending Physician
            Physician attendingPhysician = new Physician
                {   FirstName = "Attending",
                    LastName = "Physician",
                    PhysicianNumber = 2L
                };
            PhysicianRelationship attRelationship = new PhysicianRelationship(
                PhysicianRole.Attending(), attendingPhysician );

            DefaultAccount.AddPhysicianRelationship( attRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                "There should be one physician relationship" );

            attRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician,
                              "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, "Attending",
                             "Attending Physician's first name does not match" );
            Assert.AreEqual( attRelationship.Physician.LastName, "Physician",
                             "Attending Physician's last name does not match" );
            Assert.AreEqual( attRelationship.Physician.PhysicianNumber, 2L,
                             "Attending Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                "Attending Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddReferringPhysicianRelationship()
        {
            //Referring Physician
            Physician referringPhysician = new Physician
                {   FirstName = "Referring",
                    LastName = "Physician",
                    PhysicianNumber = 3L
                };
            PhysicianRelationship refRelationship = new PhysicianRelationship(
                PhysicianRole.Referring(), referringPhysician );

            DefaultAccount.AddPhysicianRelationship( refRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be one physician relationship" );

            refRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician,
                              "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist" );
            Assert.AreEqual( refRelationship.Physician.FirstName, "Referring",
                             "Referring Physician's first name does not match" );
            Assert.AreEqual( refRelationship.Physician.LastName, "Physician",
                             "Referring Physician's last name does not match" );
            Assert.AreEqual( refRelationship.Physician.PhysicianNumber, 3L,
                             "Referring Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                             "Referring Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddOperatingPhysicianRelationship()
        {
            //Operating Physician
            Physician operatingPhysician = new Physician
                {   FirstName = "Operating",
                    LastName = "Physician",
                    PhysicianNumber = 4L
                };
            PhysicianRelationship oprRelationship = new PhysicianRelationship(
                PhysicianRole.Operating(), operatingPhysician );

            DefaultAccount.AddPhysicianRelationship( oprRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be one physician relationship" );

            oprRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician,
                              "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, "Operating",
                             "Operating Physician's first name does not match" );
            Assert.AreEqual( oprRelationship.Physician.LastName, "Physician",
                             "Operating Physician's last name does not match" );
            Assert.AreEqual( oprRelationship.Physician.PhysicianNumber, 4L,
                             "Operating Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddPrimaryCarePhysicianRelationship()
        {
            //Primary Care Physician
            Physician primaryPhysician = new Physician
                {   FirstName = "PrimaryCare",
                    LastName = "Physician",
                    PhysicianNumber = 5L
                };
            PhysicianRelationship othRelationship = new PhysicianRelationship(
                PhysicianRole.PrimaryCare(), primaryPhysician );

            DefaultAccount.AddPhysicianRelationship( othRelationship );
            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be one physician relationship" );

            othRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, "PrimaryCare",
                             "PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( othRelationship.Physician.LastName, "Physician",
                             "PrimaryCare Physician's last name does not match" );
            Assert.AreEqual( othRelationship.Physician.PhysicianNumber, 5L,
                             "PrimaryCare Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                             "PrimaryCare Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void AddConsultingPhysicianRelationship()
        {
            //Consulting Physician
            Physician consultingPhysician = new Physician
                {   FirstName = "Consulting",
                    LastName = "Physician",
                    PhysicianNumber = 6L
                };
            PhysicianRelationship conRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consultingPhysician );

            DefaultAccount.AddPhysicianRelationship( conRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be one physician relationship" );

            conRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Consulting().Role() );

            Assert.IsNotNull( conRelationship.Physician,
                              "Consulting Physician does not exist" );
            Assert.IsNotNull( conRelationship.PhysicianRole,
                              "Consulting Physician Role does not exist" );
            Assert.AreEqual( conRelationship.Physician.FirstName, "Consulting",
                             "Consulting Physician's first name does not match" );
            Assert.AreEqual( conRelationship.Physician.LastName, "Physician",
                             "Consulting Physician's last name does not match" );
            Assert.AreEqual( conRelationship.Physician.PhysicianNumber, 6L,
                             "Consulting Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Consulting().Role(),
                conRelationship.PhysicianRole.Role(),
                "Consulting Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void PhysicianAndPhysicianRelationshipTests()
        {
            #region AddAdmittingPhysicianRelationship
            //Admitting Physician
            Physician admittingPhysician = new Physician
                {   FirstName = "Admitting",
                    LastName = "Physician",
                    PhysicianNumber = 1L
                };
            PhysicianRelationship admRelationship = new PhysicianRelationship(
                PhysicianRole.Admitting(), admittingPhysician );

            DefaultAccount.AddPhysicianRelationship( admRelationship );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                "There should be one physician relationship" );

            admRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician,
                              "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, "Admitting",
                             "Admitting Physician's first name does not match" );

            Assert.AreEqual( PhysicianRole.Admitting().Role(),
                admRelationship.PhysicianRole.Role(),
                "Admitting Physician Roles do not match" );

            ArrayList admittingPhysicians = ( ArrayList )DefaultAccount.PhysiciansWith(
                                                           PhysicianRole.Admitting().Role() );
            Assert.AreEqual( 1, admittingPhysicians.Count,
                              "There should be one admitting physician" );

            admittingPhysician = DefaultAccount.AdmittingPhysician;
            Assert.IsNotNull( admittingPhysician, "Admitting Physician does not exist" );
            Assert.AreEqual( admittingPhysician.FirstName, "Admitting",
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( admittingPhysician.LastName, "Physician",
                             "Admitting Physician's last name does not match" );
            #endregion

            #region AddAttendingPhysicianRelationship
            //Attending Physician
            Physician attendingPhysician = new Physician
                {   FirstName = "Attending",
                    LastName = "Physician",
                    PhysicianNumber = 2L
                };
            PhysicianRelationship attRelationship = new PhysicianRelationship(
                PhysicianRole.Attending(), attendingPhysician );

            DefaultAccount.AddPhysicianRelationship( attRelationship );

            Assert.AreEqual( 2, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be two physician relationships" );

            attRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician,
                              "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, "Attending",
                             "Attending Physician's first name does not match" );

            Assert.AreEqual( PhysicianRole.Attending().Role(),
                attRelationship.PhysicianRole.Role(),
                "Attending Physician Roles do not match" );

            ArrayList attendingPhysicians = ( ArrayList )DefaultAccount.PhysiciansWith(
                                                           PhysicianRole.Attending().Role() );
            Assert.AreEqual( 1, attendingPhysicians.Count,
                              "There should be one attending physician" );

            attendingPhysician = DefaultAccount.AttendingPhysician;
            Assert.IsNotNull( attendingPhysician, "Attending Physician does not exist" );
            Assert.AreEqual( attendingPhysician.FirstName, "Attending",
                             "Attending Physician's first name does not match" );
            #endregion

            #region AddReferringPhysicianRelationship
            //Referring Physician
            Physician referringPhysician = new Physician
                {   FirstName = "Referring",
                    LastName = "Physician",
                    PhysicianNumber = 3L
                };
            PhysicianRelationship refRelationship = new PhysicianRelationship(
                PhysicianRole.Referring(), referringPhysician );

            DefaultAccount.AddPhysicianRelationship( refRelationship );

            Assert.AreEqual( 3, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be three physician relationships" );

            refRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician,
                              "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist" );
            Assert.AreEqual( refRelationship.Physician.FirstName, "Referring",
                             "Referring Physician's first name does not match" );

            Assert.AreEqual( PhysicianRole.Referring().Role(),
                refRelationship.PhysicianRole.Role(),
                "Referring Physician Roles do not match" );

            ArrayList referringPhysicians = ( ArrayList )DefaultAccount.PhysiciansWith(
                                                           PhysicianRole.Referring().Role() );
            Assert.AreEqual( 1, referringPhysicians.Count,
                              "There should be one referring physician" );

            referringPhysician = DefaultAccount.ReferringPhysician;
            Assert.IsNotNull( referringPhysician, "Referring Physician does not exist" );
            Assert.AreEqual( referringPhysician.FirstName, "Referring",
                             "Referring Physician's first name does not match" );
            #endregion

            #region AddOperatingPhysicianRelationship
            //Operating Physician
            Physician operatingPhysician = new Physician
                {   FirstName = "Operating",
                    LastName = "Physician",
                    PhysicianNumber = 4L
                };
            PhysicianRelationship oprRelationship = new PhysicianRelationship(
                PhysicianRole.Operating(), operatingPhysician );

            DefaultAccount.AddPhysicianRelationship( oprRelationship );

            Assert.AreEqual( 4, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be fours physician relationship" );

            oprRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician,
                              "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, "Operating",
                             "Operating Physician's first name does not match" );

            Assert.AreEqual( PhysicianRole.Operating().Role(),
                             oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );

            ArrayList operatingPhysicians = ( ArrayList )DefaultAccount.PhysiciansWith(
                                                           PhysicianRole.Operating().Role() );
            Assert.AreEqual( 1, operatingPhysicians.Count,
                              "There should be one operating physician" );

            operatingPhysician = DefaultAccount.OperatingPhysician;
            Assert.IsNotNull( operatingPhysician, "Operating Physician does not exist" );
            Assert.AreEqual( operatingPhysician.FirstName, "Operating",
                             "Operating Physician's first name does not match" );
            #endregion

            #region AddPrimaryCarePhysicianRelationship
            //Primary Care Physician
            Physician primaryCarePhysician = new Physician
                {   FirstName = "PrimaryCare",
                    LastName = "Physician",
                    PhysicianNumber = 5L
                };
            PhysicianRelationship othRelationship = new PhysicianRelationship(
                PhysicianRole.PrimaryCare(), primaryCarePhysician );

            DefaultAccount.AddPhysicianRelationship( othRelationship );

            Assert.AreEqual( MAX_CONSULT_PHYS,
                              DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be five physician relationships" );

            othRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, "PrimaryCare",
                             "PrimaryCare Physician's first name does not match" );

            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(),
                othRelationship.PhysicianRole.Role(),
                "PrimaryCare Physician Roles do not match" );

            ArrayList primaryCarePhysicians = ( ArrayList )DefaultAccount.PhysiciansWith(
                                                       PhysicianRole.PrimaryCare().Role() );
            Assert.AreEqual( 1, primaryCarePhysicians.Count,
                              "There should be one other physician" );

            primaryCarePhysician = DefaultAccount.PrimaryCarePhysician;
            Assert.IsNotNull( primaryCarePhysicians, "PrimaryCare Physician does not exist" );
            Assert.AreEqual( primaryCarePhysician.FirstName, "PrimaryCare",
                             "PrimaryCare Physician's first name does not match" );
            #endregion

            #region AddConsultingPhysicianRelationships
            //Consulting1 Physician
            Physician consulting1Physician = new Physician
                {   FirstName = "Consulting1",
                    LastName = "Physician",
                    PhysicianNumber = 6L
                };
            PhysicianRelationship con1Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting1Physician );

            for ( int i = 1; i < 6; i++ )
            {
                DefaultAccount.AddPhysicianRelationship( con1Relationship );

                Assert.AreEqual( i, DefaultAccount.ConsultingPhysicians.Count,
                                  "There should be " + i + " physician relationship(s)" );
            }

            ArrayList consultingPhysicians = DefaultAccount.ConsultingPhysicians;
            Assert.AreEqual( MAX_CONSULT_PHYS, consultingPhysicians.Count,
                              "There should be five consulting physician relationships" );
            foreach ( Physician consultingPhysician in consultingPhysicians )
            {
                Assert.IsNotNull( consultingPhysician, "Consulting Physician does not exist" );
                Assert.AreEqual( consultingPhysician.FirstName, "Consulting1",
                                 "Consulting Physician's first name does not match" );
            }

            consultingPhysicians.Clear();
            consultingPhysicians = ( ArrayList )DefaultAccount.PhysiciansWith( PhysicianRole.Consulting().Role() );
            Assert.AreEqual( MAX_CONSULT_PHYS, consultingPhysicians.Count,
                              "There should be five consulting physicians" );

            foreach ( Physician consultingPhysician in consultingPhysicians )
            {
                Assert.IsNotNull( consultingPhysician, "Consulting Physician does not exist" );
                Assert.AreEqual( consultingPhysician.FirstName, "Consulting1",
                                 "Consulting Physician's first name does not match" );
            }

            ArrayList consultingPhysRelationships = ( ArrayList )DefaultAccount.PhysicianRelationshipsWith( PhysicianRole.Consulting().Role() );
            Assert.AreEqual( MAX_CONSULT_PHYS, consultingPhysRelationships.Count,
                              "There should be five consulting physician relationships" );

            foreach ( PhysicianRelationship consultingRelationship in consultingPhysRelationships )
            {
                Assert.IsNotNull( consultingRelationship, "Consulting Physician relationship does not exist" );
                Assert.AreEqual( consultingRelationship.Physician.FirstName, "Consulting1",
                                 "Consulting Physician's first name does not match" );
            }
            #endregion

            Assert.AreEqual(
                10, DefaultAccount.AllPhysicianRelationships.Count,
                "There should be ten physician relationships" );
        }

        [Test]
        [Category( "Fast" )]
        public void ReplaceExistingPhysicianRelationship()
        {
            #region InitializePhysicianRelationships
            //Admitting Physician
            Physician admittingPhysician = new Physician
                {   FirstName = "Admitting", LastName = "Physician", PhysicianNumber = 1L };
            
            PhysicianRelationship admRelationship = new PhysicianRelationship(
                PhysicianRole.Admitting(), admittingPhysician );

            //Attending Physician
            Physician attendingPhysician = new Physician
                { FirstName = "Attending", LastName = "Physician", PhysicianNumber = 2L };
            
            PhysicianRelationship attRelationship = new PhysicianRelationship(
                PhysicianRole.Attending(), attendingPhysician );

            //Referring Physician
            Physician referringPhysician = new Physician
                { FirstName = "Referring", LastName = "Physician", PhysicianNumber = 3L };
            
            PhysicianRelationship refRelationship = new PhysicianRelationship(
                PhysicianRole.Referring(), referringPhysician );

            //Operating Physician
            Physician operatingPhysician = new Physician
                { FirstName = "Operating", LastName = "Physician", PhysicianNumber = 4L };

            PhysicianRelationship oprRelationship = new PhysicianRelationship(
                PhysicianRole.Operating(), operatingPhysician );

            //Primary Care Physician
            Physician primaryCarePhysician = new Physician
                { FirstName = "PrimaryCare", LastName = "Physician", PhysicianNumber = 5L };

            PhysicianRelationship othRelationship = new PhysicianRelationship(
                PhysicianRole.PrimaryCare(), primaryCarePhysician );

            //Consulting1 Physician
            Physician consulting1Physician = new Physician
                { FirstName = "Consulting1", LastName = "Physician", PhysicianNumber = 6L };

            PhysicianRelationship con1Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting1Physician );

            //Consulting2 Physician
            Physician consulting2Physician = new Physician
                { FirstName = "Consulting2", LastName = "Physician", PhysicianNumber = 7L };

            PhysicianRelationship con2Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting2Physician );

            //Consulting3 Physician
            Physician consulting3Physician = new Physician
                { FirstName = "Consulting3", LastName = "Physician", PhysicianNumber = 8L };

            PhysicianRelationship con3Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting3Physician );

            //Consulting4 Physician
            Physician consulting4Physician = new Physician
                { FirstName = "Consulting4", LastName = "Physician", PhysicianNumber = 9L };

            PhysicianRelationship con4Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting4Physician );

            //Consulting5 Physician
            Physician consulting5Physician = new Physician
                { FirstName = "Consulting5", LastName = "Physician", PhysicianNumber = 10L };

            PhysicianRelationship con5Relationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consulting5Physician );

            //New Admitting Physician
            Physician admittingPhysicianNew = new Physician
                { FirstName = "NewAdmitting", LastName = "Physician", PhysicianNumber = 11L };

            PhysicianRelationship newAdmRelationship = new PhysicianRelationship(
                PhysicianRole.Admitting(), admittingPhysicianNew );

            //New Attending Physician
            Physician attendingPhysicianNew = new Physician
                { FirstName = "NewAttending", LastName = "Physician", PhysicianNumber = 12L };

            PhysicianRelationship newAttRelationship = new PhysicianRelationship(
                PhysicianRole.Attending(), attendingPhysicianNew );

            //New Referring Physician
            Physician referringPhysicianNew = new Physician
                { FirstName = "NewReferring", LastName = "Physician", PhysicianNumber = 13L };

            PhysicianRelationship newRefRelationship = new PhysicianRelationship(
                PhysicianRole.Referring(), referringPhysicianNew );

            //New Operating Physician
            Physician operatingPhysicianNew = new Physician
                { FirstName = "NewOperating", LastName = "Physician", PhysicianNumber = 14L };

            PhysicianRelationship newOprRelationship = new PhysicianRelationship(
                PhysicianRole.Operating(), operatingPhysicianNew );

            //New Primary Care Physician
            Physician primaryCarePhysicianNew = new Physician
                { FirstName = "NewOther", LastName = "Physician", PhysicianNumber = 15L };

            PhysicianRelationship newOthRelationship = new PhysicianRelationship(
                PhysicianRole.PrimaryCare(), primaryCarePhysicianNew );

            //New Consulting Physician
            Physician consultingPhysicianNew = new Physician
                { FirstName = "NewConsulting", LastName = "Physician", PhysicianNumber = 16L };

            PhysicianRelationship newConRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), consultingPhysicianNew );
            #endregion

            #region AddIntialRelationships
            DefaultAccount.AddPhysicianRelationship( admRelationship );
            DefaultAccount.AddPhysicianRelationship( attRelationship );
            DefaultAccount.AddPhysicianRelationship( refRelationship );
            DefaultAccount.AddPhysicianRelationship( oprRelationship );
            DefaultAccount.AddPhysicianRelationship( othRelationship );
            DefaultAccount.AddPhysicianRelationship( con1Relationship );
            DefaultAccount.AddPhysicianRelationship( con2Relationship );
            DefaultAccount.AddPhysicianRelationship( con3Relationship );
            DefaultAccount.AddPhysicianRelationship( con4Relationship );
            DefaultAccount.AddPhysicianRelationship( con5Relationship );

            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Admitting Relationship
            admRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( admRelationship.Physician,
                              "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole,
                              "Admitting Physician Role does not exist" );
            Assert.AreEqual( admRelationship.Physician.FirstName, "Admitting",
                             "Admitting Physician's first name does not match" );
            Assert.AreEqual( admRelationship.Physician.LastName, "Physician",
                             "Admitting Physician's last name does not match" );
            Assert.AreEqual( admRelationship.Physician.PhysicianNumber, 1L,
                             "Admitting Physician's number does not match" );

            Assert.AreEqual( PhysicianRole.Admitting().Role(), admRelationship.PhysicianRole.Role(),
                             "Admitting Physician Roles do not match" );
            DefaultAccount.AddPhysicianRelationship( newAdmRelationship );

            newAdmRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Admitting().Role() );

            Assert.IsNotNull( newAdmRelationship.Physician,
                              "New Admitting Physician does not exist" );
            Assert.IsNotNull( newAdmRelationship.PhysicianRole,
                              "New Admitting Physician Role does not exist" );
            Assert.AreEqual( newAdmRelationship.Physician.FirstName, "NewAdmitting",
                             "New Admitting Physician's first name does not match" );
            Assert.AreEqual( newAdmRelationship.Physician.LastName, "Physician",
                             "New Admitting Physician's last name does not match" );
            Assert.AreEqual( newAdmRelationship.Physician.PhysicianNumber, 11L,
                             "New Admitting Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Admitting().Role(), newAdmRelationship.PhysicianRole.Role(),
                             "New Admitting Physician Roles do not match" );
            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Attending Relationship
            attRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( attRelationship.Physician,
                              "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole,
                              "Attending Physician Role does not exist" );
            Assert.AreEqual( attRelationship.Physician.FirstName, "Attending",
                             "Attending Physician's first name does not match" );
            Assert.AreEqual( attRelationship.Physician.LastName, "Physician",
                             "Attending Physician's last name does not match" );
            Assert.AreEqual( attRelationship.Physician.PhysicianNumber, 2L,
                             "Attending Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                             "Attending Physician Roles do not match" );

            DefaultAccount.AddPhysicianRelationship( newAttRelationship );

            newAttRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Attending().Role() );

            Assert.IsNotNull( newAttRelationship.Physician,
                              "New Attending Physician does not exist" );
            Assert.IsNotNull( newAttRelationship.PhysicianRole,
                              "New Attending Physician Role does not exist" );
            Assert.AreEqual( newAttRelationship.Physician.FirstName, "NewAttending",
                             "New Attending Physician's first name does not match" );
            Assert.AreEqual( newAttRelationship.Physician.LastName, "Physician",
                             "New Attending Physician's last name does not match" );
            Assert.AreEqual( newAttRelationship.Physician.PhysicianNumber, 12L,
                             "New Attending Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Attending().Role(), newAttRelationship.PhysicianRole.Role(),
                             "New Attending Physician Roles do not match" );
            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Referring Relationship
            refRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( refRelationship.Physician,
                              "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole,
                              "Referring Physician Role does not exist" );
            Assert.AreEqual( refRelationship.Physician.FirstName, "Referring",
                             "Referring Physician's first name does not match" );
            Assert.AreEqual( refRelationship.Physician.LastName, "Physician",
                             "Referring Physician's last name does not match" );
            Assert.AreEqual( refRelationship.Physician.PhysicianNumber, 3L,
                             "Referring Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                             "Referring Physician Roles do not match" );

            DefaultAccount.AddPhysicianRelationship( newRefRelationship );

            newRefRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Referring().Role() );

            Assert.IsNotNull( newRefRelationship.Physician,
                              "New Referring Physician does not exist" );
            Assert.IsNotNull( newRefRelationship.PhysicianRole,
                              "New Referring Physician Role does not exist" );
            Assert.AreEqual( newRefRelationship.Physician.FirstName, "NewReferring",
                             "New Referring Physician's first name does not match" );
            Assert.AreEqual( newRefRelationship.Physician.LastName, "Physician",
                             "New Referring Physician's last name does not match" );
            Assert.AreEqual( newRefRelationship.Physician.PhysicianNumber, 13L,
                             "New Referring Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Referring().Role(), newRefRelationship.PhysicianRole.Role(),
                             "New Referring Physician Roles do not match" );

            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Operating Relationship
            oprRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( oprRelationship.Physician,
                              "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole,
                              "Operating Physician Role does not exist" );
            Assert.AreEqual( oprRelationship.Physician.FirstName, "Operating",
                             "Operating Physician's first name does not match" );
            Assert.AreEqual( oprRelationship.Physician.LastName, "Physician",
                             "Operating Physician's last name does not match" );
            Assert.AreEqual( oprRelationship.Physician.PhysicianNumber, 4L,
                             "Operating Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                             "Operating Physician Roles do not match" );

            DefaultAccount.AddPhysicianRelationship( newOprRelationship );

            newOprRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Operating().Role() );

            Assert.IsNotNull( newOprRelationship.Physician,
                              "New Operating Physician does not exist" );
            Assert.IsNotNull( newOprRelationship.PhysicianRole,
                              "New Operating Physician Role does not exist" );
            Assert.AreEqual( newOprRelationship.Physician.FirstName, "NewOperating",
                             "New Operating Physician's first name does not match" );
            Assert.AreEqual( newOprRelationship.Physician.LastName, "Physician",
                             "New Operating Physician's last name does not match" );
            Assert.AreEqual( newOprRelationship.Physician.PhysicianNumber, 14L,
                             "New Operating Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Operating().Role(), newOprRelationship.PhysicianRole.Role(),
                             "New Operating Physician Roles do not match" );

            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion

            #region Test and Replace PrimaryCare Relationship
            othRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( othRelationship.Physician,
                              "PrimaryCare Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole,
                              "PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( othRelationship.Physician.FirstName, "PrimaryCare",
                             "PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( othRelationship.Physician.LastName, "Physician",
                             "PrimaryCare Physician's last name does not match" );
            Assert.AreEqual( othRelationship.Physician.PhysicianNumber, 5L,
                             "PrimaryCare Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                             "PrimaryCare Physician Roles do not match" );

            DefaultAccount.AddPhysicianRelationship( newOthRelationship );

            newOthRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.PrimaryCare().Role() );

            Assert.IsNotNull( newOthRelationship.Physician, "New PrimaryCare Physician does not exist" );
            Assert.IsNotNull( newOthRelationship.PhysicianRole,
                              "New PrimaryCare Physician Role does not exist" );
            Assert.AreEqual( newOthRelationship.Physician.FirstName, "NewOther",
                             "New PrimaryCare Physician's first name does not match" );
            Assert.AreEqual( newOthRelationship.Physician.LastName, "Physician",
                             "New PrimaryCare Physician's last name does not match" );
            Assert.AreEqual( newOthRelationship.Physician.PhysicianNumber, 15L,
                             "New PrimaryCare Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.PrimaryCare().Role(), newOthRelationship.PhysicianRole.Role(),
                             "New PrimaryCare Physician Roles do not match" );

            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                              "There should be ten physician relationships" );
            #endregion

            #region Test and Replace Consulting Relationship
            ArrayList conRelationships = ( ArrayList )DefaultAccount.PhysicianRelationshipsWith(
                                                        PhysicianRole.Consulting().Role() );

            Assert.AreEqual( MAX_CONSULT_PHYS, conRelationships.Count,
                              "There should be five consulting physician relationships" );

            long physicianNumber = 1L;

            foreach ( PhysicianRelationship conRelationship in conRelationships )
            {
                Assert.IsNotNull( conRelationship, "Consulting Physician relationship does not exist" );
                Assert.AreEqual( conRelationship.Physician.FirstName, string.Concat( "Consulting", physicianNumber.ToString() ),
                                 "Consulting Physician's first name does not match" );
                Assert.AreEqual( conRelationship.Physician.PhysicianNumber, physicianNumber + 5L,
                                 "Consulting Physician's number does not match" );
                physicianNumber++;
            }

            conRelationships.Clear();
            DefaultAccount.AddPhysicianRelationship( newConRelationship );
            conRelationships = ( ArrayList )DefaultAccount.PhysicianRelationshipsWith(
                                              PhysicianRole.Consulting().Role() );

            Assert.AreEqual( MAX_CONSULT_PHYS, conRelationships.Count,
                              "There should be five consulting physician relationships" );

            Physician newConsultingPhysician = null;
            foreach ( PhysicianRelationship conRelationship in conRelationships )
            {
                if ( conRelationship.Physician.FirstName.Equals( "NewConsulting" ) &&
                    conRelationship.Physician.PhysicianNumber == 16L )
                {
                    newConsultingPhysician = conRelationship.Physician;
                    break;
                }
            }

            Assert.IsNotNull( newConsultingPhysician, "New Consulting Physician does not exist" );
            Assert.AreEqual( newConsultingPhysician.FirstName, "NewConsulting",
                             "New Consulting Physician's first name does not match" );

            Assert.AreEqual( newConsultingPhysician.PhysicianNumber, 16L,
                             "New Consulting Physician's number does not match" );

            Assert.AreEqual( 10, DefaultAccount.AllPhysicianRelationships.Count,
                             "There should be ten physician relationships" );
            #endregion
        }

        [Test]
        [Category( "Fast" )]
        public void TestRetirementDate()
        {
            Account account = new Account();
            Assert.IsFalse( account.IsRetired, "Account person should not be retired" );
            OccurrenceCode oc = new OccurrenceCode( 18, "Retirement Date",
                                                 new DateTime( 1987, 3, 4 ) ) 
                                                 { Code = OccurrenceCode.OCCURRENCECODE_RETIREDATE };
            account.AddOccurrenceCode( oc );
            Assert.IsTrue( account.IsRetired, "Account person should be retired" );
            Assert.AreEqual( new DateTime( 1987, 3, 4 ), account.RetirementDate );

        }

        [Test]
        [Category( "Fast" )]
        public void AddPhysicianWithRole()
        {
            //Consulting Physician
            Physician physician = new Physician
                { FirstName = "Consulting", LastName = "Physician", PhysicianNumber = 6L };
            PhysicianRelationship conRelationship = new PhysicianRelationship(
                PhysicianRole.Consulting(), physician );

            DefaultAccount.AddPhysicianWithRole( PhysicianRole.Consulting(), physician );

            Assert.AreEqual( 1, DefaultAccount.AllPhysicianRelationships.Count,
                             "There should be one physician relationship" );

            conRelationship = DefaultAccount.PhysicianRelationshipWithRole(
                PhysicianRole.Consulting().Role() );

            Assert.IsNotNull( conRelationship.Physician,
                              "Physician does not exist" );
            Assert.IsNotNull( conRelationship.PhysicianRole,
                              "Physician Role does not exist" );
            Assert.AreEqual( conRelationship.Physician.FirstName, "Consulting",
                             "Physician's first name does not match" );
            Assert.AreEqual( conRelationship.Physician.LastName, "Physician",
                             "Physician's last name does not match" );
            Assert.AreEqual( conRelationship.Physician.PhysicianNumber, 6L,
                             "Physician's number does not match" );
            Assert.AreEqual( PhysicianRole.Consulting().Role(),
                conRelationship.PhysicianRole.Role(),
                "Physician Roles do not match" );
        }

        [Test]
        [Category( "Fast" )]
        public void TestIsPatientTypeChangeable()
        {
            Account account = new Account { Activity = new AdmitNewbornActivity() };
            bool isPatientTypeChangeable = account.IsPatientTypeChangeable();
            Assert.IsFalse( isPatientTypeChangeable, "Patient Type is changeable for AdmitNewbornActivity" );
            account.Activity = new RegistrationActivity();
            isPatientTypeChangeable = account.IsPatientTypeChangeable();
            Assert.IsTrue( isPatientTypeChangeable, "Patient Type is not changeable for RegistrationActivity" );
            account.Activity = new MaintenanceActivity();
            isPatientTypeChangeable = account.IsPatientTypeChangeable();
            Assert.IsTrue( isPatientTypeChangeable, "Patient Type is changeable for MaintenanceActivity" );
            account.KindOfVisit = new VisitType( 0, DateTime.Now, "Inpatient", VisitType.INPATIENT );
            account.Location = new Location( "NS", "180", "A" );
            isPatientTypeChangeable = account.IsPatientTypeChangeable();
            Assert.IsFalse( isPatientTypeChangeable, "Patient Type is not changeable for MaintenanceActivity for inpatients" );
            account.KindOfVisit = new VisitType( 0, DateTime.Now, "Outpatient", VisitType.OUTPATIENT );
            account.Location = new Location( "NS", "180", "A" );
            isPatientTypeChangeable = account.IsPatientTypeChangeable();
            Assert.IsFalse( isPatientTypeChangeable, 
                "Patient Type is not changeable for MaintenanceActivity for Outpatient with Location seleected" );
        }

        [Test, ExpectedException( typeof( ArgumentNullException ) )]
        [Category( "Fast" )]
        public void TestAddNullConsentedResearchStudy_ShouldNotAddNullObjectToList()
        {
            Account account = new Account();
            account.AddConsentedResearchStudy( null );

            Assert.IsNotNull( account.ClinicalResearchStudies );
            Assert.IsTrue( account.ClinicalResearchStudies.Count() == 0 );
        }

        [Test]
        [Category( "Fast" )]
        public void TestAddConsentedResearchStudy_WhenCountIsLessThan10_ShouldAddResearchStudy()
        {
            Account account = new Account();
            ResearchStudy study1 = new ResearchStudy( "TEST001", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST002", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );

            ConsentedResearchStudy research1 = new ConsentedResearchStudy( study1, YesNoFlag.Yes );
            ConsentedResearchStudy research2 = new ConsentedResearchStudy( study2, YesNoFlag.No );

            account.AddConsentedResearchStudy( research1 );
            account.AddConsentedResearchStudy( research2 );

            Assert.IsNotNull( account.ClinicalResearchStudies );
            Assert.IsTrue( account.ClinicalResearchStudies.Count() == 2 );
            Assert.IsTrue( account.ClinicalResearchStudies.Contains( research1 ) );
            Assert.IsTrue( account.ClinicalResearchStudies.Contains( research2 ) );
        }

        [Test, ExpectedException( typeof( InvalidOperationException ) )]
        [Category( "Fast" )]
        public void TestAddConsentedResearchStudy_WhenCountIsEqualTo10_ShouldThrowInvalidOperationException()
        {
            Account account = GetAccountWithTenResearchStudies();
            ResearchStudy study11 = new ResearchStudy( "TEST011", "RESEARCH STUDY 11", "RESEARCH SPONSOR 11" );
            ConsentedResearchStudy research11 = new ConsentedResearchStudy( study11, YesNoFlag.Yes );

            account.AddConsentedResearchStudy( research11 );

            Assert.IsNotNull( account.ClinicalResearchStudies, "ClinicalResearchStudies list should not be null." );
            Assert.IsTrue( account.ClinicalResearchStudies.Count() == 10, "ClinicalResearchStudies Count should be 10." );
        }

        [Test]
        [Category( "Fast" )]
        public void TestCanAddConsentedResearchStudy_WhenCountIsEqualTo10_ShouldReturnFalse()
        {
            Account account = GetAccountWithTenResearchStudies();

            bool canAddConsentedResearchStudy = account.CanAddConsentedResearchStudy();

            Assert.IsNotNull( account.ClinicalResearchStudies, "ClinicalResearchStudies list should not be null." );
            Assert.IsTrue( account.ClinicalResearchStudies.Count() == 10, "ClinicalResearchStudies Count should be 10." );
            Assert.IsFalse( canAddConsentedResearchStudy, "Value should be False." );
        }

        [Test]
        [Category( "Fast" )]
        public void TestCanAddConsentedResearchStudy_WhenCountIsLessThan10_ShouldReturnTrue()
        {
            Account account = new Account();

            ResearchStudy study1 = new ResearchStudy( "TEST01", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST02", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );

            ConsentedResearchStudy research1 = new ConsentedResearchStudy( study1, YesNoFlag.Yes );
            ConsentedResearchStudy research2 = new ConsentedResearchStudy( study2, YesNoFlag.No );

            account.AddConsentedResearchStudy( research1 );
            account.AddConsentedResearchStudy( research2 );

            bool canAddConsentedResearchStudy = account.CanAddConsentedResearchStudy();

            Assert.IsNotNull( account.ClinicalResearchStudies, "ClinicalResearchStudies list should not be null." );
            Assert.IsTrue( account.ClinicalResearchStudies.Count() == 2, "ClinicalResearchStudies Count should be 2." );
            Assert.IsTrue( canAddConsentedResearchStudy, "Value should be True." );
        }
        [Test]
        public void TestSetDefaultInsurancePlan()
        {
            var account = GetAccountWithEmptyInsurance();
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                       OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.SetDefaultInsurancePlan();
            var plan = account.Insurance.CoverageFor(CoverageOrder.PRIMARY_OID).InsurancePlan;
            Assert.IsNotNull(plan, "default plan is not assigned when Insurance is null.");
            Assert.IsTrue(plan.PlanID == InsurancePlan.QUICK_ACCOUNTS_DEFAULT_INSURANCE_PLAN_ID, "Default plan assigned is Incorrect");
        }

        [Test]
        public void TestAddOccurrenceCode_PT_InPatient_HSV_SNF_ShouldAddDuplicateOccurrenceCodes50()
        {
            VisitType patientType = VisitType.Inpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "SUB_ACUTE_NV_B_HOLD",
                                                      HospitalService.SUB_ACUTE_NV_B_HOLD);
            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                       OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 4, "Should add Duplicate OccurrenceCode 50 and Occurrence codes Count should be 4.");

        }

        [Test]
        public void TestQUpdateOccurrenceCode_PT_InPatient_HSV_SNF_ShouldNotRemoveOccurrenceCode50s()
        {
            VisitType patientType = VisitType.Inpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "SUB_ACUTE_NV_B_HOLD",
                                                      HospitalService.SUB_ACUTE_NV_B_HOLD);
            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                       OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 4, "Occurrence codes Count should be 4.");
            account.RemoveOccurrenceCode50IfNotApplicable();
            Assert.IsNotNull(account.OccurrenceCodes, "Occurrence codes list should not be null.");
            Assert.IsTrue(account.OccurrenceCodes.Count == 4, "Occurrence codes Count should be 4.");
           
        }
        [Test]
        public void TestAddOccurrenceCode_PT_InPatient_HSV_NotSNF_ShouldNotAddDuplicateOccurrenceCodes50()
        {
            VisitType patientType = VisitType.Inpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "58",
                                                      HospitalService.HSV58);
            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                       OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 3, "Should not add Duplicate OccurrenceCode 50 and Occurrence codes Count should be 3.");

        }

        [Test]
        public void TestQUpdateOccurrenceCode_PT_InPatient_HSV_NotSNF_ShouldRemoveOccurrenceCode50s()
        {
            VisitType patientType = VisitType.Inpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "58",
                                                      HospitalService.HSV58);
            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                       OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 3, "Occurrence codes Count should be 3.");
            account.RemoveOccurrenceCode50IfNotApplicable();
            Assert.IsNotNull(account.OccurrenceCodes, "Occurrence codes list should not be null.");
            Assert.IsTrue(account.OccurrenceCodes.Count == 2, "Occurrence codes Count should be 2.");

        }

        [Test]
        public void TestAddOccurrenceCode_PT_OutPatient_HSV_NotSNF_ShouldNotAddDuplicateOccurrenceCodes50()
        {
            VisitType patientType = VisitType.Outpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "58",
                                                      HospitalService.HSV58);

            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                    OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                        OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));


            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 3,
                          "Should not add Duplicate Occurrence codes 50 and Occurrence codes Count should be 3.");
        }
       
        [Test]
        public void TestQUpdateOccurrenceCode_PT_OutPatient_HSV_NotSNF_ShouldRemoveOccurrenceCode50s()
        {
            VisitType patientType = VisitType.Outpatient;
            HospitalService HSV = new HospitalService(4, ReferenceValue.NEW_VERSION, "58",
                                                      HospitalService.HSV58);

            Account account = GetAccountWith(patientType, HSV);
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Auto",
                                                    OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "Illness",
                                                         OccurrenceCode.OCCURRENCECODE_ILLNESS));
            account.AddOccurrenceCode(new OccurrenceCode(0L, DateTime.Now, "RA_EOMB",
                                                         OccurrenceCode.OCCURENCECODE_DATE_OF_RA_EOMB));

            Debug.Assert(account.OccurrenceCodes != null, "account.OccurrenceCodes != null");
            Assert.IsTrue((account.OccurrenceCodes.Count) == 3, "Occurrence codes Count should be 3.");
            account.RemoveOccurrenceCode50IfNotApplicable();
            Assert.IsNotNull(account.OccurrenceCodes, "Occurrence codes list should not be null.");
            Assert.IsTrue(account.OccurrenceCodes.Count == 2, "Occurrence codes 50 will be removed and Count should be 2.");
        }

        [Test]
        public void Test_ContactPointsForCopyingToWithContext()
        {
            Patient patient = new Patient();

            var emergencyContact = new EmergencyContact();
            emergencyContact.Name = "Sam Thomas";
            Relationship relationship = new Relationship(new RelationshipType(0L, DateTime.Now, "Brother"), patient.GetType(), emergencyContact.GetType());
            emergencyContact.AddRelationship(relationship);
            emergencyContact.AddContactPoint(newPhysical);

            patient.AddContactPoint(newMailing);
            patient.AddContactPoint(newPhysical);

            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

            Account account = new Account
                                  {
                                      Activity = new EditPreMseActivity(),
                                      Patient = patient
                                  };
            Facility facility = facilityBroker.FacilityWith(FACILITY_CODE_DHF);
            account.Facility = facility;
            account.SetDefaultInsurancePlan();
            account.PrimaryInsured.AddContactPoint(newPhysical);
            account.EmergencyContact1 = emergencyContact;

            IDictionary contactPoints = account.ContactPointsForCopyingToWithContext(Address.PatientMailing);
            Assert.IsFalse(contactPoints.Contains("Insured - Primary"), "Mailing address context should not contain Insured Primary");
            Assert.IsTrue(contactPoints.Contains("Patient - Physical"), "Mailing address context should contain Patient - Physical");

            contactPoints = account.ContactPointsForCopyingToWithContext(Address.PatientPhysical);
            Assert.IsFalse(contactPoints.Contains("Insured - Primary"), "Physical address context should not contain Insured Primary");
            Assert.IsTrue(contactPoints.Contains("Patient - Mailing"), "Physical address context should contain Patient - Mailing");

            contactPoints = account.ContactPointsForCopyingToWithContext("EmergencyContact1");
            Assert.IsFalse(contactPoints.Contains("Insured - Primary"), "Emergency contact1 addresss context should not contain Insured Primary");
            Assert.IsTrue(contactPoints.Contains("Patient - Mailing"), "Emergency contact1 addresss context should contain Patient - Mailing");
            Assert.IsTrue(contactPoints.Contains("Patient - Physical"), "Emergency contact1 addresss context should contain Patient - Physical");
            Assert.IsFalse(contactPoints.Contains("EmergencyContact1"), "Emergency contact1 address context should not contain EmergencyContact1");

        }

        [Test]
        [Category("Fast")]
        public void Test_ShareDataWithPublicHIE_ShouldDefaultToYesNo()
        {
            var anAccount = new Account(PersistentModel.NEW_OID);
            var shareDatawithPublicHIE = anAccount.ShareDataWithPublicHieFlag.Code;
            Assert.AreEqual(YesNoFlag.CODE_NO, shareDatawithPublicHIE,
                "Share Data With Public HIE should default to No");
        }

        [Test]
        [Category("Fast")]
        public void Test_ShareDataWithPCP_ShouldDefaultToNo()
        {
            var anAccount = new Account(PersistentModel.NEW_OID);
            var shareDataWithPCP = anAccount.ShareDataWithPCPFlag.Code;
            Assert.AreEqual(YesNoFlag.CODE_NO, shareDataWithPCP, "Share Data With PCP should default to Yes(Y)");
        }

        #endregion

        #region Support Methods

        private static Account GetAccountWith(VisitType patientType, HospitalService HSV)
        {
            Account account = new Account();
            account.KindOfVisit = patientType;
            account.HospitalService = HSV;
          
            return account;
        }
         

        private static Account GetAccountWithTenResearchStudies()
        {
            Account account = new Account();

            ResearchStudy study1 = new ResearchStudy( "TEST01", "RESEARCH STUDY 1", "RESEARCH SPONSOR 1" );
            ResearchStudy study2 = new ResearchStudy( "TEST02", "RESEARCH STUDY 2", "RESEARCH SPONSOR 2" );
            ResearchStudy study3 = new ResearchStudy( "TEST03", "RESEARCH STUDY 3", "RESEARCH SPONSOR 3" );
            ResearchStudy study4 = new ResearchStudy( "TEST04", "RESEARCH STUDY 4", "RESEARCH SPONSOR 4" );
            ResearchStudy study5 = new ResearchStudy( "TEST05", "RESEARCH STUDY 5", "RESEARCH SPONSOR 5" );
            ResearchStudy study6 = new ResearchStudy( "TEST06", "RESEARCH STUDY 6", "RESEARCH SPONSOR 6" );
            ResearchStudy study7 = new ResearchStudy( "TEST07", "RESEARCH STUDY 7", "RESEARCH SPONSOR 7" );
            ResearchStudy study8 = new ResearchStudy( "TEST08", "RESEARCH STUDY 8", "RESEARCH SPONSOR 8" );
            ResearchStudy study9 = new ResearchStudy( "TEST08", "RESEARCH STUDY 9", "RESEARCH SPONSOR 9" );
            ResearchStudy study10 = new ResearchStudy( "TEST10", "RESEARCH STUDY 10", "RESEARCH SPONSOR 10" );

            ConsentedResearchStudy research1 = new ConsentedResearchStudy( study1, YesNoFlag.Yes );
            ConsentedResearchStudy research2 = new ConsentedResearchStudy( study2, YesNoFlag.No );
            ConsentedResearchStudy research3 = new ConsentedResearchStudy( study3, YesNoFlag.Yes );
            ConsentedResearchStudy research4 = new ConsentedResearchStudy( study4, YesNoFlag.Yes );
            ConsentedResearchStudy research5 = new ConsentedResearchStudy( study5, YesNoFlag.Yes );
            ConsentedResearchStudy research6 = new ConsentedResearchStudy( study6, YesNoFlag.Yes );
            ConsentedResearchStudy research7 = new ConsentedResearchStudy( study7, YesNoFlag.Yes );
            ConsentedResearchStudy research8 = new ConsentedResearchStudy( study8, YesNoFlag.Yes );
            ConsentedResearchStudy research9 = new ConsentedResearchStudy( study9, YesNoFlag.Yes );
            ConsentedResearchStudy research10 = new ConsentedResearchStudy( study10, YesNoFlag.Yes );

            account.AddConsentedResearchStudy( research1 );
            account.AddConsentedResearchStudy( research2 );
            account.AddConsentedResearchStudy( research3 );
            account.AddConsentedResearchStudy( research4 );
            account.AddConsentedResearchStudy( research5 );
            account.AddConsentedResearchStudy( research6 );
            account.AddConsentedResearchStudy( research7 );
            account.AddConsentedResearchStudy( research8 );
            account.AddConsentedResearchStudy( research9 );
            account.AddConsentedResearchStudy( research10 );
            return account;
        }
        private Account GetAccountWithEmptyInsurance()
        {
            Account account = new Account {Facility = FACILITYDHF, AdmitDate = DateTime.Today};
            return account;
        }
        private Account DefaultAccount { get; set; }

        #endregion

        #region Data Elements
       
        private const string
            PATIENT_F_NAME = "Sam",
            PATIENT_L_NAME = "Spade",
            PATIENT_MI = "L",

            GUARANTOR_F_NAME = "John",
            GUARANTOR_L_NAME = "Spade",
            GUARANTOR_MI = "D";

        private readonly SocialSecurityNumber
            PATIENT_SSN = new SocialSecurityNumber( "123121234" );
        private const long
            PATIENT_OID = 45L,
            PATIENT_MRN = 123456789;
        private readonly Name
            PATIENT_NAME = new Name( PATIENT_F_NAME, PATIENT_L_NAME, PATIENT_MI );
        private readonly DateTime
            PATIENT_DOB = new DateTime( 1955, 3, 5 );
        private readonly Gender PATIENT_SEX = new Gender( 0, DateTime.Now, "Male", "M" );

        private const string
            FACILILTY_NAME = "DELRAY TEST HOSPITAL",
            FACILITY_CODE = "DEL",
            FACILILTY_NAME_DHF = "DALLAS HOSPITAL",
            FACILITY_CODE_DHF = "DHF";
        private const int FACILITY_ID_DHF = 54;
        private readonly Facility
            FACILITY = new Facility( PersistentModel.NEW_OID,
                                           PersistentModel.NEW_VERSION,
                                           FACILILTY_NAME,
                                           FACILITY_CODE ),
            FACILITYDHF = new Facility(FACILITY_ID_DHF,
                                           PersistentModel.NEW_VERSION,
                                           FACILILTY_NAME_DHF,
                                          FACILITY_CODE_DHF );

        private const string ADDRESS1 = "335 Nicholson Dr.",
                             ADDRESS2 = "#303",
                             CITY = "Austin",
                             POSTALCODE = "60505";

        private readonly Address address = new Address( ADDRESS1,
                                                ADDRESS2,
                                                CITY,
                                                new ZipCode( POSTALCODE ),
                                                new State( 0L,
                                                           PersistentModel.NEW_VERSION,
                                                           "TEXAS",
                                                           "TX" ),
                                                new Country( 0L,
                                                             PersistentModel.NEW_VERSION,
                                                             "United States",
                                                             "USA" ),
                                                new County( 0L,
                                                            PersistentModel.NEW_VERSION,
                                                            "ORANGE",
                                                            "122" )
            );

        #endregion
    }
}