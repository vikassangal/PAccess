using System;
using System.Globalization;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence
{
    /// <summary>
    /// Summary description for FacilityBrokerTests.
    /// </summary>
    [TestFixture()]
    public class TimeBrokerTests : AbstractBrokerTests
    {

        #region SetUp and TearDown TimeBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpTimeBrokerTests()
        {
            // 2007 times

            currentAppSvrTime1 = new DateTime( 2007, 3, 11, 1, 45, 0 );
            currentAppSvrTime2 = new DateTime( 2007, 3, 11, 3, 15, 0 );
            
            currentAppSvrTime4 = new DateTime( 2007, 11, 4, 3, 15, 0 );
            currentAppSvrTime5 = new DateTime( 2007, 11, 30, 3, 15, 0 );
  
            CultureInfo en = new CultureInfo("en-US");
            String format = "MM/dd/yyyy hh:mm:sszzz";
            String currAppSvrTime6 = "11/04/2007 01:45:00-06:00";  //test 1:45am standard time
            
            currentAppSvrTime6 = DateTime.ParseExact( currAppSvrTime6, format, en.DateTimeFormat, DateTimeStyles.AdjustToUniversal );
            
        }

        [TestFixtureTearDown()]
        public static void TearDownTimeBrokerTests()
        {
        }

        #endregion

        #region Test Methods
        [Test()]
        public void TestEasternTimeAt()
        {
           
            //test eastern facility----------------------------------------------------------------
            this.f = this.facilityBroker.FacilityWith( EASTERN_FACILITYID );
            this.facTime = this.timeBroker.TimeAt( this.f.GMTOffset, this.f.DSTOffset  );

            Assert.AreEqual(
                DateTime.Now.AddHours(1).ToString().Substring(1, 14),
                this.facTime.ToString().Substring(1, 14),
                "eastern fac DateTime is " );
        }

        [Test()]
        public void TestCentralTimeAt()
        { 
            //test central facility----------------------------------------------------------------
            this.f = this.facilityBroker.FacilityWith( CENTRAL_FACILITYID );
            this.f.GMTOffset = -6;
            this.f.DSTOffset = 1;
            this.facTime = this.timeBroker.TimeAt( this.f.GMTOffset, this.f.DSTOffset );

            Assert.AreEqual(
                DateTime.Now.ToString().Substring(1, 14),
                this.facTime.ToString().Substring(1, 14),
                "central fac DateTime is ");


            //    "central fac DateTime for time point3 is ");



        }        

        [Test()]
        public void TestMountainTimeAt()
        {
            //test mountain facility---------------------------------------------------------------
            this.f = this.facilityBroker.FacilityWith( MOUNTAIN_FACILITYID );
            this.f.GMTOffset = -7;
            this.f.DSTOffset = 0;

            // get mountain DSToffset-------------------------
            int mountainDSTOffset= 0;
            int currentAppSvrYear = DateTime.Now.Year;

            TimeZone localZone = TimeZone.CurrentTimeZone;
            DaylightTime daylight = localZone.GetDaylightChanges( currentAppSvrYear );

            DateTime appSvrDSTStartTime = daylight.Start;
            DateTime appSvrDSTEndTime = daylight.End;
                  
            if( DateTime.Now >= appSvrDSTStartTime &&  DateTime.Now < appSvrDSTEndTime )
            {
                mountainDSTOffset = -2;
            }
            else
            {
                mountainDSTOffset = -1;
            }
            //-------------------------------------------------    

            this.facTime = this.timeBroker.TimeAt( this.f.GMTOffset, this.f.DSTOffset );

            Assert.AreEqual(
                DateTime.Now.AddHours( mountainDSTOffset ).ToString().Substring(1, 14),
                this.facTime.ToString().Substring(1, 14),
                "mountain fac DateTime is ");
        }

        [Test()]
        public void TestPacificTimeAt()
        {
            //test pacific facility-------------------------------------------------------------------
            this.f = this.facilityBroker.FacilityWith( PACIFIC_FACILITYID );
            this.f.GMTOffset = -8;
            this.f.DSTOffset = 1;

            this.facTime = this.timeBroker.TimeAt( this.f.GMTOffset, this.f.DSTOffset );

            Assert.AreEqual(
                DateTime.Now.AddHours(-2).ToString().Substring(1, 14),
                this.facTime.ToString().Substring(1, 14),
                "pacific fac DateTime is ");
        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        private ITimeBroker timeBroker = BrokerFactory.BrokerOfType<ITimeBroker>();
        private IFacilityBroker facilityBroker = BrokerFactory.BrokerOfType<IFacilityBroker>();

        private DateTime facTime;
        private Facility f = null;

        private static  DateTime currentAppSvrTime1;
        private static  DateTime currentAppSvrTime2;
        private static  DateTime currentAppSvrTime4;
        private static  DateTime currentAppSvrTime5;            
        private static  DateTime currentAppSvrTime6;
        #endregion

        #region Constants
        private const int CENTRAL_FACILITYID = 98; //fake one
        private const int EASTERN_FACILITYID = 6;   //DEL
        private const int PACIFIC_FACILITYID = 98;  //fake one
        private const int MOUNTAIN_FACILITYID = 98;  //fake one
        #endregion
    }
}