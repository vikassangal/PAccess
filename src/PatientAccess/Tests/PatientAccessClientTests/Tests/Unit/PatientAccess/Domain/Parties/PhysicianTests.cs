using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    /// <summary>
    /// Summary description for PhysicianTests.
    /// </summary>

    //TODO: Create XML summary comment for PhysicianTests
    [TestFixture]
    [Category( "Fast" )]
    public class PhysicianTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PhysicianTests
        [TestFixtureSetUp()]
        public static void SetUpPhysicianTests()
        {
            p1 = new Physician();
            p2 = new Physician();
        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianTests()
        {
            p1 = null;
            p2 = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestPhysician()
        {
            p1 = new Physician(123, ReferenceValue.NEW_VERSION);
            Assert.AreEqual("00123", p1.FormattedPhysicianNumber,
                            "Physician Numbers do not Match");

            p1 = new Physician(123, ReferenceValue.NEW_VERSION, "Marcus Welby");
            Assert.AreEqual("Marcus Welby", p1.ToString());
            
            p1 = new Physician(123, ReferenceValue.NEW_VERSION, "Marcus Welby", new Name("Marcus", "Welby", "T"));
            Assert.AreEqual("Welby, Marcus T", p1.ToString());
                       
            p2 = new Physician(123, ReferenceValue.NEW_VERSION, "Marcus Welby","Marcus","Welby","T");
            Assert.AreEqual(0, p1.CompareTo(p2));

            p1 = new Physician(123, ReferenceValue.NEW_VERSION, 10, 2, 2, 2, 2, 2);
            Assert.AreNotEqual(0, p1.CompareTo(p2));

            Assert.AreEqual(10, p1.TotalPatients, "Total patients should be ");
            Assert.AreEqual(2, p1.TotalAdmittingPatients, "Total admitting patients should be ");
            Assert.AreEqual(2, p1.TotalAttendingPatients, "Total attending patients should be ");
            Assert.AreEqual(2, p1.TotalConsultingPatients, "Total consulting patients should be ");
            Assert.AreEqual(2, p1.TotalOperatingPatients, "Total operating patients should be ");
            Assert.AreEqual(2, p1.TotalReferringPatients, "Total referring patients should be ");

            p1.Address = new Address();
            Assert.AreEqual(string.Empty, p1.Address.Address1);

            p1.Status = string.Empty;
            Assert.AreEqual(string.Empty, p1.Status);

            p1.AdmittingPrivileges = string.Empty;
            Assert.AreEqual(string.Empty, p1.AdmittingPrivileges);

            p1.ActiveInactiveFlag = string.Empty;
            Assert.AreEqual(string.Empty, p1.ActiveInactiveFlag);

            p1.DateActivated = DateTime.Today;
            Assert.AreEqual(DateTime.Today, p1.DateActivated);

            p1.DateInactivated = DateTime.Today;
            Assert.AreEqual(DateTime.Today, p1.DateInactivated);

            p1.DateExcluded = DateTime.Today;
            Assert.AreEqual(DateTime.Today, p1.DateExcluded);

            p1.ExcludedStatus = string.Empty;
            Assert.AreEqual(string.Empty, p1.ExcludedStatus);

            p1.Specialization = new Speciality();
            Assert.AreEqual(string.Empty, p1.Specialization.Description);

            p1.PhoneNumber = new PhoneNumber();
            Assert.AreEqual(string.Empty, p1.PhoneNumber.Number);

            p1.CellPhoneNumber = new PhoneNumber();
            Assert.AreEqual(string.Empty, p1.CellPhoneNumber.Number);

            p1.PagerNumber = new PhoneNumber();
            Assert.AreEqual(string.Empty, p1.PagerNumber.Number);

            p1.StateLicense = string.Empty;
            Assert.AreEqual(string.Empty, p1.StateLicense);

            p1.FederalLicense = new DriversLicense("123456789");
            Assert.AreEqual("123456789", p1.FederalLicense.Number);

            p1.Title = string.Empty;
            Assert.AreEqual(string.Empty, p1.Title);

            p1.UPIN = string.Empty;
            Assert.AreEqual(string.Empty, p1.UPIN);

            p1.PIN = 0L;
            Assert.AreEqual(0L, p1.PIN);

            p1.MedicalGroupNumber = string.Empty;
            Assert.AreEqual(string.Empty, p1.MedicalGroupNumber);

            p1.AdmitPrivilegeSuspendDate = DateTime.Today;
            Assert.AreEqual(DateTime.Today, p1.AdmitPrivilegeSuspendDate);

            
        }

        [Test()]
        public void TestNPI()
        {
            p1.NPI = string.Empty;
            Assert.AreEqual(string.Empty, p1.NPI);

            Assert.IsTrue(Physician.isValidNPI("808401689606790"), "CheckDigit for NPI 808401689606790 is valid");
            Assert.IsFalse(Physician.isValidNPI("808401111"), "CheckDigit for NPI 808401111 is invalid");
            Assert.IsFalse(Physician.isValidNPI("168960679S"), "CheckDigit for NPI 168960679S is invalid");
            Assert.IsFalse( Physician.isValidNPI( "1689606791" ), "CheckDigit for NPI 1689606791 should be invalid" );
            Assert.IsTrue(Physician.isValidNPI("1689606790"), "CheckDigit for NPI 1689606790 is valid");
            Assert.AreEqual('0', Physician.checkDigitNPI("1689606790"));
            
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static Physician p1 = null;
        private static Physician p2 = null;
        #endregion
    }
}