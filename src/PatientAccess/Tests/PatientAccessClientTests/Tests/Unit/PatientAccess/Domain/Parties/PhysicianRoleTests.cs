using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;

namespace Tests.Unit.PatientAccess.Domain.Parties
{
    [TestFixture]
    [Category( "Fast" )]
    public class PhysicianRoleTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown PersonTests
        [TestFixtureSetUp()]
        public static void SetUpPhysicianRoleTests()
        {
            anAccount = new Account();
            anAccount.KindOfVisit.Code = VisitType.NON_PATIENT;
            anAccount.Activity = new PreRegistrationActivity();
            aPhysician = new Physician();
            aPhysician.ActiveInactiveFlag = "Active";
        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianRoleTests()
        {
            pr1 = null;
            pr2 = null;
            anAccount = null;
            aPhysician = null;
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestPhysicianRole()
        {
            pr1 = new PhysicianRole();
            pr1 = new PhysicianRole(ReferenceValue.NEW_OID, string.Empty);
            pr1 = new PhysicianRole(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION);
            pr1 = new PhysicianRole(ReferenceValue.NEW_OID, ReferenceValue.NEW_VERSION, string.Empty);
            
            pr1.ValidateFor(anAccount, aPhysician);
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(0, pr1.CompareTo(pr2));
        }

        [Test()]
        public void TestSpecificPhysicianRoles()
        {
            pr1 = PhysicianRole.Admitting();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(PhysicianRole.ADMITTING, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

            anAccount.KindOfVisit.Code = VisitType.OUTPATIENT;
            pr1.ValidateFor(anAccount, aPhysician);

            anAccount.KindOfVisit.Code = VisitType.RECURRING_PATIENT;
            pr1.ValidateFor(anAccount, aPhysician);

            anAccount.KindOfVisit.Code = VisitType.PREREG_PATIENT;
            pr1.ValidateFor(anAccount, aPhysician);

            aPhysician.AdmittingPrivileges = "Y";
            pr1.ValidateFor(anAccount, aPhysician);

            anAccount.Activity = new TransferOutToInActivity();
            pr1.ValidateFor(anAccount, aPhysician);
            anAccount.Activity = new PreMSERegisterActivity();

            anAccount.KindOfVisit.Code = VisitType.EMERGENCY_PATIENT;
            aPhysician.ActiveInactiveFlag = string.Empty;
            pr1.ValidateFor(anAccount, aPhysician);
            aPhysician.ActiveInactiveFlag = "Active";
            pr1.ValidateFor(anAccount, aPhysician);

            /* ---------------------------------------------------*/

            pr1 = PhysicianRole.Attending();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(PhysicianRole.ATTENDING, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

            anAccount.Activity = new TransferOutToInActivity();
            pr1.ValidateFor(anAccount, aPhysician);
            anAccount.Activity = new PreRegistrationActivity();
            anAccount.HospitalService.DayCare = "Y";
            pr1.ValidateFor(anAccount, aPhysician);
            aPhysician.ActiveInactiveFlag = string.Empty;
            anAccount.Activity = new PreMSERegisterActivity();
            pr1.ValidateFor(anAccount, aPhysician);
            aPhysician.ActiveInactiveFlag = "Active";

            /* ---------------------------------------------------*/

            pr1 = PhysicianRole.Consulting();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(PhysicianRole.CONSULTING, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

            /* ---------------------------------------------------*/

            pr1 = PhysicianRole.Operating();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(PhysicianRole.OPERATING, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

            /* ---------------------------------------------------*/

            pr1 = PhysicianRole.PrimaryCare();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            anAccount.FinancialClass = new FinancialClass{ Code = FinancialClass.MED_SCREEN_EXM_CODE };

            Assert.AreEqual(PhysicianRole.PRIMARYCARE, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

            /* ---------------------------------------------------*/

            pr1 = PhysicianRole.Referring();
            Assert.IsTrue(pr1.IsValidFor(anAccount, aPhysician));
            pr2 = pr1.Role();
            Assert.AreEqual(PhysicianRole.REFERRING, pr2.Oid);
            pr1.ValidateFor(anAccount, aPhysician);

        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private static PhysicianRole pr1 = null;
        private static PhysicianRole pr2 = null;
        private static Account anAccount = null;
        private static Physician aPhysician = null;
        #endregion
    }
}