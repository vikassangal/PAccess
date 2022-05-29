using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using IBM.Data.DB2.iSeries;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Persistence;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    [TestFixture()]
    public class EmployerPbarBrokerTests : AbstractBrokerTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown EmployerPbarBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpEmployerPbarBrokerTests()
        {
            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            i_facility = fb.FacilityWith(ACO_FACILITYID);

            dbConnection = new iDB2Connection();
            dbConnection.ConnectionString = i_facility.ConnectionSpec.ConnectionString;
            dbConnection.Open();

            //This is essentially cargo-culting from other tests...
            i_broker = BrokerFactory.BrokerOfType<IEmployerBroker>();
            CreateUser();
            IEmploymentStatusBroker esb = BrokerFactory.BrokerOfType<IEmploymentStatusBroker>();
            empStatus = esb.EmploymentStatusWith(ACO_FACILITYID, "1");
        }

        [TestFixtureTearDown()]
        public static void TearDownEmployerPbarBrokerTests()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
        #endregion

        #region Test Methods

        [Test()]
        public void TestGetEmployersWith()
        {
            SortedList employers = i_broker.AllEmployersWith(i_facility.Oid, "PE");
            Assert.IsNotNull(employers, "No Employers Found");
            Assert.IsTrue(employers.Count > 0, "No Employers found");
        }

        //Defect 9412 - PAS crashes when doing search for the employer. 
        // Search text as 'Team' is added to ACO under DVLA to reproduce this defect.
        [Test()]
        public void TestDuplicateEmployerKey()
        {
            SortedList employers = i_broker.AllEmployersWith( i_facility.Oid, "TEAM" );
            Assert.IsNotNull( employers, "No Employers Found" );
            Assert.IsTrue( employers.Count > 0, "No Employers found" );
        }
        [Test()]
        public void TestAddressForEmployer()
        {
            SortedList employers = i_broker.AllEmployersWith(i_facility.Oid, "NBC");

            EmployerProxy selectedEp = null;

            foreach (EmployerProxy ep in employers.Values)
            {
                if (ep.EmployerCode == 394)
                {
                    selectedEp = ep;
                    break;
                }
            }

            // CHECKS FOR Contact Points            
            Assert.IsNotNull(selectedEp, "Did not find EmployerProxy with EmployerCode = 394");
            Employer employer = i_broker.EmployerFor(selectedEp);
            ArrayList contactPoints = (ArrayList)employer.ContactPoints;
            Assert.IsTrue(contactPoints.Count > 0, "Wrong number of Contact Points found");

            Address address = ((EmployerContactPoint)contactPoints[0]).Address;
            if (address.Address1 != "ADDRESS") //removed OID dependency.
                address = ((ContactPoint)contactPoints[1]).Address;

            Assert.AreEqual("ADDRESS", address.Address1, "Address1 should be \"ADDRESS\"");
            Assert.AreEqual("CITY", address.City, "City should be \"CITY\"");
            Assert.AreEqual("99999", address.ZipCode.PostalCode, "PostalCode should be \"99999\"");

            Assert.IsNotNull(address.State, "There was no State");
            Assert.AreEqual("CA", address.State.Code, "State code Should me \"CA");
            Assert.AreEqual("CALIFORNIA", address.State.Description, "State  Should be \"CALIFORNIA");
        }

        [Test()]
        public void TestSelectEmployerByName()
        {
            long nocode = 0;
            Employer ep = i_broker.SelectEmployerByName(i_facility.Code, "WALMART");
            Assert.AreNotEqual(Convert.ToDecimal(ep.EmployerCode), Convert.ToDecimal(nocode), "No Employers named 'WALMART'");
            Assert.AreEqual(ep.Name, "WALMART", "Got back an employer OTHER THAN 'WALMART'");
        }

        [Test()]
        public void TestEmployerForPatient()
        {
            Patient patient = new Patient();
            patient.MedicalRecordNumber = 827741;

            patient = i_broker.EmployerFor(patient, i_facility.Oid);

            Assert.IsNotNull(patient.Employment, "Did not find employment for test person");
            Assert.IsNotNull(patient.Employment.Employer, "Did not find employer for test person");

            Console.WriteLine(patient.Employment.Employer.Name);
            Console.WriteLine(patient.Employment.Status.Description);
            Employment employment = patient.Employment;
            Assert.AreEqual(
                empStatus,
                employment.Status,
                "Missing or invalid Employment status found");
            Console.WriteLine(patient.Employment.Employer.PartyContactPoint.PhoneNumber.ToString());

        }

        [Test()]
        public void TestEmployerFor()
        {
            Guarantor guarantor = new Guarantor();

            // GOO - this is bogus, but the test is ignored, so the compile is successful

            guarantor = i_broker.EmployerFor(guarantor, 30049, i_facility.Oid);

            Assert.IsNotNull(guarantor.Employment, "Did not find employment for test person");
            Assert.IsNotNull(guarantor.Employment.Employer, "Did not find employer for test person");

            Employment employment = guarantor.Employment;
            Assert.AreEqual(
                empStatus,
                employment.Status,
                "Missing or invalid Employment status found");

            ContactPoint cp = guarantor.Employment.Employer.PartyContactPoint;
            Assert.IsNotNull(cp, "ContactPoint not loaded for person");
            Address addr = cp.Address;
            //Assert.AreEqual("Incorrect State found for Guarentors Employer","TX",addr.State.Code);
            //Assert.AreEqual("Incorrect Address found for Guarantors employer","12345 W Parker Road",addr.Address1);
            Assert.AreEqual("TX", addr.State.Code, "Incorrect State found for Guarentors Employer");
            //Assert.AreEqual("Incorrect Address found for Guarantors employer","UNIVERSITY DR",addr.Address1);
            Assert.AreEqual("2215 S FIRST ST", addr.Address1, "Incorrect Address found for Guarantors employer");

            IFacilityBroker fb = BrokerFactory.BrokerOfType<IFacilityBroker>();
            Facility facility = fb.FacilityWith("ICE");

            Patient patient = new Patient(
                0,
                Patient.NEW_VERSION,
                new Name("PHIL", "MCGRAW", string.Empty),
                789076,
                new DateTime(1954, 9, 15),
                new SocialSecurityNumber("405105604"),
                new Gender(0, DateTime.Now, "Male", "M"),
                facility
                );

            patient = i_broker.EmployerFor(patient, patient.Facility.Oid);
            Assert.AreEqual(
                string.Empty,
                patient.Employment.Employer.Industry,
                "industry should be ''");

            Assert.AreEqual(
                "HOST",
                patient.Employment.Occupation,
                "occupation should be 'HOST'");

            Assert.AreEqual(
                "123909",
                patient.Employment.EmployeeID,
                "employeeid should be employeeid");

            Assert.AreEqual(
                "323",
                patient.Employment.Employer.PartyContactPoint.PhoneNumber.AreaCode,
                "employer phone areacode should be 323");

            Assert.AreEqual(
                "4550930",
                patient.Employment.Employer.PartyContactPoint.PhoneNumber.Number,
                "employer phone number should be 4550930");
        }

        [Test()]
        public void TestAddEmployer()
        {
            string employerName = "NUNITTESTEMPLOYER";

            try
            {
                dbTransaction = dbConnection.BeginTransaction();

                long nocode = 0;
                Employer ep = new Employer();
                ep.Name = employerName;

                IEmployerBroker employerBroker = new EmployerPbarBroker(dbTransaction);
                int empnum = employerBroker.AddNewEmployer(ep, i_facility.FollowupUnit, i_facility.Code);

                Assert.AreNotEqual(
                    empnum,
                    -1,
                    "Something went horribly wrong with the returned employer number. Got back: {0}",
                    empnum);

                Employer ep2 = i_broker.SelectEmployerByName(i_facility.Code, ep.Name);

                Assert.AreNotEqual(Convert.ToDecimal(ep2.EmployerCode), Convert.ToDecimal(nocode), "No Employers named 'NUNITTESTEMPLOYER'");
                Assert.AreEqual(ep2.Name, ep.Name, "Got back an employer OTHER THAN '" + ep.Name + "'");

            }
            catch
            {
                Assert.Fail("New employer cannot be added to db2 database");
            }
            finally
            {
                dbTransaction.Rollback();
            }

            Employer ep3 = i_broker.SelectEmployerByName(i_facility.Code, employerName);

            Assert.AreEqual(0, ep3.EmployerCode, "Employer NOT deleted. Retrieved: {0}", ep3.EmployerCode);

        }

        [Test]
        public void TestGetAllEmployers()
        {
            var employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
            var facilityCode = i_facility.Code;
            var originalEmployerName = Guid.NewGuid().ToString();
            var employerToDelete = new Employer(01, DateTime.Now, originalEmployerName, "SomeNationalID", 02);

            try
            {
                employerBroker.AddEmployerForApproval(employerToDelete, facilityCode, "someUserID");
                var employerList = EmployerBroker.GetAllEmployersForApproval(i_facility.Code);
                Assert.IsNotNull(employerList, "Error - List is null.");
                Assert.IsTrue(employerList.Count > 0, "Returned employer list has length of 0.");
            }
            finally
            {
                employerBroker.DeleteEmployerForApproval(employerToDelete, facilityCode);
            }
        }

        [Test]
        public void TestAddEmployerForApproval()
        {
            var employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
            var facilityCode = i_facility.Code;
            var originalEmployerName = Guid.NewGuid().ToString();
            var employerToDelete = new Employer(01, DateTime.Now, originalEmployerName, "SomeNationalID", 02);
            var mangledEmployerName = Employer.ModifyPBAREmployerName(originalEmployerName);

            try
            {
                employerBroker.AddEmployerForApproval(employerToDelete, facilityCode, "someUserID");
                var employersRetreived = employerBroker.GetAllEmployersForApproval(facilityCode);
                var employerRtreived =
                    employersRetreived.Values.Where(x => x.Employer.Name == mangledEmployerName).First();
                Assert.IsTrue(employersRetreived.Values.Where(x => x.Employer.Name == mangledEmployerName).Count() == 1);
                Assert.IsTrue(employerRtreived.Employer.Oid != 0);
            }
            finally
            {
                employerBroker.DeleteEmployerForApproval(employerToDelete, facilityCode);
            }
        }

        [Test]
        public void TestSaveEmployersForApproval()
        {
            var employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
            IList<NewEmployerEntry> employersToSave = this.GetTwoStubEmployers();
            var mangledEmployerName1 = Employer.ModifyPBAREmployerName(employersToSave[0].Employer.Name);
            var mangledEmployerName2 = Employer.ModifyPBAREmployerName(employersToSave[1].Employer.Name);
            try
            {
                employerBroker.SaveEmployersForApproval(employersToSave, i_facility.Code);
                var employersRetreived = employerBroker.GetAllEmployersForApproval(i_facility.Code);

                Assert.IsTrue(employersRetreived.Values.Where(x => x.Employer.Name == mangledEmployerName1).Count() == 1);
                Assert.IsTrue(employersRetreived.Values.Where(x => x.Employer.Name == mangledEmployerName2).Count() == 1);
            }
            finally
            {
                employerBroker.DeleteEmployerForApproval(employersToSave[0].Employer, i_facility.Code);
                employerBroker.DeleteEmployerForApproval(employersToSave[1].Employer, i_facility.Code);
            }
        }


        private IList<NewEmployerEntry> GetTwoStubEmployers()
        {
            var employer1 = new Employer(01, DateTime.Now, Guid.NewGuid().ToString(), "SomeNationalID", 02);


            ContactPoint contactPoint =
                new ContactPoint(
                    new Address("2300 W Plano Parkway", "Suite 1", "Plano", new ZipCode("75075"), new State(),
                                Country.NewUnitedStatesCountry(), new County("1")),
                    new PhoneNumber("123", "1234567"), new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType());


            employer1.PartyContactPoint.Address = contactPoint.Address;
            employer1.PartyContactPoint.PhoneNumber= contactPoint.PhoneNumber;


            var employer2 = new Employer(02, DateTime.Now, Guid.NewGuid().ToString(), "SomeNationalID", 03);

            ContactPoint contactPoint2 =
                new ContactPoint(
                    new Address("2300 W Plano Parkway", "Suite 1", "Plano", new ZipCode("75075"), new State(),
                                Country.NewUnitedStatesCountry(), new County("1")),
                    new PhoneNumber("123", "1234567"), new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewEmployerContactPointType());
            
            employer2.PartyContactPoint.Address = contactPoint2.Address;
            employer2.PartyContactPoint.PhoneNumber = contactPoint2.PhoneNumber;
            return new List<NewEmployerEntry>
                       {
                           new NewEmployerEntry(employer1, "user"), 
                           new NewEmployerEntry(employer2, "user")
                       };
        }
        
        [Test]
        public void TestDeleteEmployerForApproval()
        {
            string originalEmployerName = Guid.NewGuid().ToString();
            var mangledEmployerName = Employer.ModifyPBAREmployerName(originalEmployerName);

            Employer employerToDelete = new Employer(01, DateTime.Now, originalEmployerName, "SomeNationalID", 02);
            var employerBroker = BrokerFactory.BrokerOfType<IEmployerBroker>();
            string facilityCode = i_facility.Code;
            employerToDelete.Oid = employerBroker.AddEmployerForApproval(employerToDelete, facilityCode, "someUserID");
            employerBroker.DeleteEmployerForApproval(employerToDelete, facilityCode);
            var employersRetreived = employerBroker.GetAllEmployersForApproval(facilityCode);

            Assert.IsTrue(employersRetreived.Values.Where(x => x.Employer.Name == mangledEmployerName).Count() == 0);
        }

        [Test]
        public void TestSaveNewEmployerAddress()
        {
            long EMPLOYER_CODE_WALMART = 371210;
            try
            {
                Employer employer = new Employer();
                employer.Name = "WALMART";

                State st = new State("TX");
                Country cntry = new Country("USA");

                Address address = new Address(
                    "Address1", "Address2", "City", new ZipCode("12345"), st, cntry);
                PhoneNumber phone = new PhoneNumber("999", "999999");
                EmailAddress email = new EmailAddress("PANUnitTest@PA.com");

                employer.EmployerCode = EMPLOYER_CODE_WALMART;
                EmployerContactPoint aContactPoint = new EmployerContactPoint(
                    address, phone, email, TypeOfContactPoint.NewEmployerContactPointType());
                employer.PartyContactPoint = aContactPoint;

                dbTransaction = dbConnection.BeginTransaction();
                IAddressBroker addressBroker = new AddressPBARBroker(dbTransaction);

                addressBroker.SaveNewEmployerAddressForApproval(employer, i_facility.Code);
                addressBroker.DeleteEmployerAddressForApproval(employer, i_facility.Code);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                dbTransaction.Rollback();
            }
        }

        #endregion

        #region Support Methods
        private static IEmployerBroker EmployerBroker
        {
            get
            {
                return i_broker;
            }
            set
            {
                i_broker = value;
            }
        }
        #endregion

        #region Data Elements

        private static IEmployerBroker i_broker;
        private static Facility i_facility;
        private static EmploymentStatus empStatus = null;

        private static IDbConnection dbConnection;
        private static IDbTransaction dbTransaction;

        #endregion
    }
}