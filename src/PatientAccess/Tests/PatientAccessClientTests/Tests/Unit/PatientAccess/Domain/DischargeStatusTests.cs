using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class DischargeStatusTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown DischargeStatusTests
        [TestFixtureSetUp()]
        public static void SetUpDischargeStatusTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownDischargeStatusTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestDischargeStatus()
        {
            DischargeStatus notDischarged		= new DischargeStatus( DischargeStatus.NOT_DISCHARGED_OID, "NOTDISCHARGED" );
            DischargeStatus pendingDischarge    = new DischargeStatus( DischargeStatus.PENDING_DISCHARGE_OID, "PENDINGDISCHARGE" );
            DischargeStatus discharged			= new DischargeStatus( DischargeStatus.DISCHARGED_OID, "DISCHARGED" );

            Assert.AreEqual(
                notDischarged,
                DischargeStatus.NotDischarged()
                );

            Assert.AreEqual(
                pendingDischarge,
                DischargeStatus.PendingDischarge()
                );

            Assert.AreEqual(
                discharged,
                DischargeStatus.Discharged()
                );
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}