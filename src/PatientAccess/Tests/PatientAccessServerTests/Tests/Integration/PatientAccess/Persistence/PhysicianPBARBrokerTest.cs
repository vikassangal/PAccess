using System.Collections;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for PhysicianBrokerTests.
    /// </summary>
    [TestFixture()]
    public class PhysicianPBARBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PhysicianBrokerTests

        [TestFixtureSetUp()]
        public static void SetUpPhysicianPBARBrokerTests()
        {            
            i_PhysicianBroker = BrokerFactory.BrokerOfType<IPhysicianBroker>();
            IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();
            AllPhysicianRoles = PhysicianRole.AllPhysicianRoles();//(ArrayList)PhysicianBroker.AllPhysicianRoles();
            i_ACOFacility = facilityBroker.FacilityWith( ACO_FACILITYID );

        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianPBARBrokerTests()
        {
        }

        #endregion

        #region Test Methods
        [Test()]
        public void AllPhysicianRolesTest()
        {
            Assert.AreEqual(
                "Admitting",
                ((PhysicianRole)AllPhysicianRoles[0]).Description,
                "First physician role should be ADMITTING"
                );

            Assert.AreEqual(
                "Attending",
                ((PhysicianRole)AllPhysicianRoles[1]).Description,
                "Second physician role should be ATTENDING"
                );

            Assert.AreEqual(
                "Consulting",
                ((PhysicianRole)AllPhysicianRoles[2]).Description,
                "Third physician role should be CONSULTING"
                );

            Assert.AreEqual(
                "Operating",
                ((PhysicianRole)AllPhysicianRoles[3]).Description,
                "Fourth physician role should be OPERATING"
                );

            Assert.AreEqual(
                "PrimaryCare",
                ((PhysicianRole)AllPhysicianRoles[4]).Description,
                "Fifth physician role should be PrimaryCare"
                );

            Assert.AreEqual(
                "Referring",
                ((PhysicianRole)AllPhysicianRoles[5]).Description,
                "Sixth physician role should be REFERRING"
                );
        }

        [Test]
        public void BuildAdmittingPhysicianRelationship()
        {
            PhysicianRelationship admRelationship = this.PhysicianBroker.BuildAdmittingPhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( admRelationship.Physician, "Admitting Physician does not exist" );
            Assert.IsNotNull( admRelationship.PhysicianRole, "Admitting Physician Role does not exist" );
            Assert.AreEqual( 
                admRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Admitting Physician's first name does not match");
            Assert.AreEqual( 
                admRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Admitting Physician's last name does not match" );
            Assert.AreEqual( 
                PhysicianRole.Admitting().Role(), admRelationship.PhysicianRole.Role(),
                "Admitting Physician Roles do not match");
        }

        [Test]
        public void BuildReferringPhysicianRelationship()
        {
            PhysicianRelationship refRelationship = this.PhysicianBroker.BuildReferringPhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( refRelationship.Physician, "Referring Physician does not exist" );
            Assert.IsNotNull( refRelationship.PhysicianRole, "Referring Physician Role does not exist" );
            Assert.AreEqual( 
                refRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Referring Physician's first name does not match");
            Assert.AreEqual(
                refRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Referring Physician's last name does not match");

            Assert.AreEqual( 
                PhysicianRole.Referring().Role(), refRelationship.PhysicianRole.Role(),
                "Referring Physician Roles do not match");
        }

        [Test]
        public void BuildAttendingPhysicianRelationship()
        {
            PhysicianRelationship attRelationship = this.PhysicianBroker.BuildAttendingPhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( attRelationship.Physician, "Attending Physician does not exist" );
            Assert.IsNotNull( attRelationship.PhysicianRole, "Attending Physician Role does not exist" );
            Assert.AreEqual( 
                attRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Attending Physician's first name does not match" );
            Assert.AreEqual( 
                attRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Attending Physician's last name does not match" );

            Assert.AreEqual( 
                PhysicianRole.Attending().Role(), attRelationship.PhysicianRole.Role(),
                "Attending Physician Roles do not match" );
        }

        [Test]
        public void BuildOperatingPhysicianRelationship()
        {
            PhysicianRelationship oprRelationship = this.PhysicianBroker.BuildOperatingPhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( oprRelationship.Physician, "Operating Physician does not exist" );
            Assert.IsNotNull( oprRelationship.PhysicianRole, "Operating Physician Role does not exist" );
            Assert.AreEqual( 
                oprRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Operating Physician's first name does not match");
            Assert.AreEqual( 
                oprRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Operating Physician's last name does not match" );

            Assert.AreEqual( 
                PhysicianRole.Operating().Role(), oprRelationship.PhysicianRole.Role(),
                "Operating Physician Roles do not match" );
        }

        [Test]
        public void BuildConsultingPhysicianRelationship()
        {
            PhysicianRelationship conRelationship = this.PhysicianBroker.BuildConsultingPhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( conRelationship.Physician, "Consulting Physician does not exist" );
            Assert.IsNotNull( conRelationship.PhysicianRole, "Consulting Physician Role does not exist" );
            Assert.AreEqual( 
                conRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Consulting Physician's first name does not match" );
            Assert.AreEqual( 
                conRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Consulting Physician's last name does not match" );

            Assert.AreEqual( 
                PhysicianRole.Consulting().Role(), conRelationship.PhysicianRole.Role(),
                "Consulting Physician Roles do not match" );
        }

        [Test]
        public void BuildPrimaryCarePhysicianRelationship()
        {
            PhysicianRelationship othRelationship = this.PhysicianBroker.BuildPrimaryCarePhysicianRelationship( 
                i_ACOFacility.Oid,
                FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull( othRelationship.Physician, "Consulting Physician does not exist" );
            Assert.IsNotNull( othRelationship.PhysicianRole, "Consulting Physician Role does not exist" );
            Assert.AreEqual( 
                othRelationship.Physician.FirstName, FIRST_PHYSICIAN_FIRST_NAME,
                "Consulting Physician's first name does not match" );
            Assert.AreEqual( 
                othRelationship.Physician.LastName, FIRST_PHYSICIAN_LAST_NAME,
                "Consulting Physician's last name does not match" );

            Assert.AreEqual( 
                PhysicianRole.PrimaryCare().Role(), othRelationship.PhysicianRole.Role(),
                "Consulting Physician Roles do not match" );
        }

        [Test]
        public void PhysicianRoleWith()
        {
            PhysicianRole admittingRole = new AdmittingPhysician();
            PhysicianRole referringRole = new ReferringPhysician();
            PhysicianRole attendingRole = new AttendingPhysician();
            PhysicianRole operatingRole = new OperatingPhysician();
            PhysicianRole consultingRole = new ConsultingPhysician();
            PhysicianRole otherRole = new PrimaryCarePhysician();

            Assert.AreEqual( 
                PhysicianRole.Admitting(),
                admittingRole.Role(),
                "Physician Roles do not match" );

            Assert.AreEqual( 
                PhysicianRole.Referring (), 
                referringRole.Role(), "Physician Roles do not match" );

            Assert.AreEqual( 
                PhysicianRole.Attending (), 
                attendingRole.Role(),
                "Physician Roles do not match" );

            Assert.AreEqual( 
                PhysicianRole.Operating (), 
                operatingRole.Role(),
                "Physician Roles do not match" );

            Assert.AreEqual( 
                PhysicianRole.Consulting (),
                consultingRole.Role(),
                "Physician Roles do not match" );

            Assert.AreEqual( 
                PhysicianRole.PrimaryCare (), 
                otherRole.Role(),
                "Physician Roles do not match" );
        }
    

        [Test]
        public void TestPhysiciansMatching()
        {
            PhysicianSearchCriteria criteria = 
                new PhysicianSearchCriteria( i_ACOFacility, "LEONA", "CHILA", 10 );
            ICollection physicians = 
                this.PhysicianBroker.PhysiciansMatching( criteria );
            Assert.IsNotNull( physicians, "Physicians list is empty" );
            Assert.IsTrue( physicians.Count >= 1, 
                           "There are no results from the Physicians Search");
            
        }

        [Test]
        public void TestSpecialityMatching()
        {
            Speciality specialty = new Speciality(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION,
                                                  "ANESTHESIOLOGY", "ANE");
            ICollection physicians = 
                this.PhysicianBroker.PhysiciansSpecialtyMatching( i_ACOFacility.Oid,specialty);
            Assert.IsNotNull( physicians, "Physicians list is empty" );
            
            Assert.IsTrue( physicians.Count >= 1,
                           "There are no results from the Physicians Search" );
            
        }

        [Test]
        public void TestPhysicianStatisticsFor()
        {
            Physician physician =
                this.PhysicianBroker.PhysicianStatisticsFor( i_ACOFacility.Oid, 1120 );

            Assert.IsNotNull( physician, "Physician object is empty" );


        }

        [Test]
        public void TestPhysicianDetailsFor()
        {
            Physician physician =
                this.PhysicianBroker.PhysicianDetails( i_ACOFacility.Oid, 73 );

            Assert.IsNotNull( physician, "Physician object is empty" );


        }

        [Test]
        public void TestAlllSpecialities()
        {            
            ICollection allPhysicianSpecs = this.PhysicianBroker.SpecialtiesFor(ACO_FACILITYID);
            Assert.IsNotNull( allPhysicianSpecs, "Specialities list is empty" );
            Assert.IsTrue( allPhysicianSpecs.Count >= 1, 
                           "There are no results from the Specialities Search" );
        }

        [Test]
        public void TestSpecialitiesForFacility()
        {            
            ArrayList facilityPhysicianList = ( ArrayList )this.PhysicianBroker.SpecialtiesFor( ACO_FACILITYID );
            Assert.IsNotNull( facilityPhysicianList, "Specialities list is empty" );
            Assert.IsTrue( facilityPhysicianList.Count >= 1,
                           "There are no results from the Specialities Search" );

        }

        [Test]
        public void TestSpecialityWithBlankCode()
        {            
            string blankCode= string.Empty;
            Speciality speciality = this.PhysicianBroker.SpecialityWith( ACO_FACILITYID ,blankCode );
            Assert.IsTrue( speciality.IsValid );
        }
        [Test]
        public void TestSpecialityWithInvalidCode()
        {            
            const string invalidCode = "XXX";
            Speciality speciality = this.PhysicianBroker.SpecialityWith( ACO_FACILITYID ,invalidCode );
            
            Assert.IsFalse( speciality.IsValid );
        }
        [Test]
        public void TestSpecialityWithValidCode()
        {            
            const string validAcc = "ANE";
            Speciality speciality = this.PhysicianBroker.SpecialityWith( ACO_FACILITYID ,validAcc );
            Assert.IsTrue( speciality.IsValid );
        }

        [Test]
        public void PhysicianWithPhysicianNumber()
        {
            Physician aPhysician = this.PhysicianBroker.PhysicianWith( ACO_FACILITYID, 30101L );

            Assert.IsNotNull( aPhysician, "Physician does not exist" );
        }

        [Test]
        public void TestVerifyPhysician()
        {
            Physician physician = this.PhysicianBroker.VerifyPhysicianWith( ACO_FACILITYID, FIRST_PHYSICIAN_NUMBER );
            Assert.IsNotNull(physician,"Did not find Physician with number of " + FIRST_PHYSICIAN_NUMBER );
        }

        [Test]
        public void TestVerifyPhysicianFail()
        {
            Physician physician = this.PhysicianBroker.VerifyPhysicianWith( ACO_FACILITYID, 99999999 );
            Assert.IsNotNull(physician,"Should not have found Physician with number of 99999999"  );
            Assert.AreEqual(physician.PhysicianNumber, 0, "Physician number should be 0 for invalid Physician");
        }

        #endregion
       
        #region Properties

        private static ArrayList AllPhysicianRoles
        {
            get
            {
                return i_AllPhysicianRoles;
            }
            set
            {
                i_AllPhysicianRoles = value;
            }
        }
        private IPhysicianBroker PhysicianBroker
        {
            get
            {
                return i_PhysicianBroker;
            }
        }
        #endregion
       

        #region Support Methods
        #endregion

        #region Data Elements
        
        private static  ArrayList i_AllPhysicianRoles;
        private static Facility i_ACOFacility;
        private static  IPhysicianBroker i_PhysicianBroker;

        private const string
            FIRST_PHYSICIAN_FIRST_NAME		= "GERRY",
            FIRST_PHYSICIAN_LAST_NAME		= "DICKMON";

        private const long
            FIRST_PHYSICIAN_NUMBER			= 3L;

        #endregion
    }
}