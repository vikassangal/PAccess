using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    /// <summary>
    /// Summary description for VisitTypeTests.
    /// </summary>

    //TODO: Create XML summary comment for VisitTypeTests
    [TestFixture]
    [Category( "Fast" )]
    public class VisitTypeTests
    {
        #region Constants

        private const long   
            TESTOID                 = 234;

        private const string         
            TESTDESC                = "sdlkf8",
            TESTCODE                = "9dje";
        #endregion

        #region SetUp and TearDown VisitTypeTests
        [TestFixtureSetUp()]
        public static void SetUpVisitTypeTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownVisitTypeTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestVisitTypes()
        {
            VisitType vt = new VisitType(
                TESTOID,ReferenceValue.NEW_VERSION,
                TESTDESC,TESTCODE);
            Assert.IsNotNull(vt, "Create of VisitType failed");
            Assert.IsTrue(vt.Oid == TESTOID, "OID is incorrect");
            Assert.IsTrue(vt.Code == TESTCODE, "CODE is incorrect");
            Assert.IsTrue(vt.Description == TESTDESC, "Description is incorrect");

            vt = new VisitType(TESTOID, ReferenceValue.NEW_VERSION,
                               VisitType.INPATIENT,VisitType.INPATIENT);
            Assert.IsTrue(vt.Code == VisitType.INPATIENT, "Code is wrong for Inpatient");
            Assert.IsTrue(vt.IsInpatient,"Visit Type not Inpatient");

            vt = new VisitType(TESTOID,ReferenceValue.NEW_VERSION,
                               VisitType.EMERGENCY_PATIENT,VisitType.EMERGENCY_PATIENT);
            Assert.IsTrue(vt.IsEmergencyPatient,"Visit Type not EmergencyPatient");

            vt = new VisitType(TESTOID,ReferenceValue.NEW_VERSION,
                               VisitType.NON_PATIENT,VisitType.NON_PATIENT);
            Assert.IsTrue(vt.IsNonPatient,"Visit Type not NonPatient");

            vt = new VisitType(TESTOID,ReferenceValue.NEW_VERSION,
                               VisitType.PREREG_PATIENT,VisitType.PREREG_PATIENT);
            Assert.IsTrue(vt.IsPreRegistrationPatient,"Visit Type not PreRegPatient");

            vt = new VisitType(TESTOID,ReferenceValue.NEW_VERSION,
                               VisitType.RECURRING_PATIENT,VisitType.RECURRING_PATIENT);
            Assert.IsTrue(vt.IsRecurringPatient,"Visit Type not RecurringPatient");

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}