using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class NursingStationTests
    {
        #region Constants
        private const string TESTSITECODE = "01";
        #endregion

        #region SetUp and TearDown NursingStationTests
        [TestFixtureSetUp()]
        public static void SetUpNursingStationTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownNursingStationTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testNursingStation()
        {
            
            NursingStation ns = new NursingStation();
            ns.PreviousCensus =4;
            
            ns.AdmitToday = 4;
            ns.DischargeToday = 1;
            ns.DeathsToday = 0;
            ns.TransferredFromToday = 1;
            ns.TransferredToToday = 2;
            ns.AvailableBeds = 2; 
            ns.TotalBeds = 5;
            ns.TotalOccupiedBedsForMonth = 11;
            ns.TotalBedsForMonth = 20 ;
            ns.SiteCode = TESTSITECODE;
            int cc = ns.CurrentCensus ;
           
            Assert.AreEqual(
                cc ,
                8
                );
            Assert.AreEqual(
                ns.SiteCode,
                TESTSITECODE);
           
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}