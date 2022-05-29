using System;
using PatientAccess.Persistence.Utilities;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Utilities
{
    [TestFixture]
    [Category( "Fast" )]
    public class DateTimeUtiltiesTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown DateTimeUtiltiesTests
        [TestFixtureSetUp()]
        public static void SetUpDateTimeUtiltiesTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownDateTimeUtiltiesTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestDateToPBARNumber()
        {
            DateTime testDate = new DateTime(2004,10,4);
            int d = DateTimeUtilities.PackedDateFromDate(testDate);
            Assert.AreEqual( 100404, d, "Date Conversion should yield: 100404" );

            DateTime testDate2 = new DateTime(1996,3,14);
            int d2 = DateTimeUtilities.PackedDateFromDate(testDate2);
            Assert.AreEqual( 31496, d2, "Date Conversion should yield: 31496" );

            DateTime d3 = DateTimeUtilities.DateTimeFromString("0");
            Assert.AreEqual( d3, new DateTime(0), "Did not convert date of 0 correctly" );

            DateTime d4 = DateTimeUtilities.DateTimeFromString("03042004","1342");
            Assert.AreEqual( new DateTime(2004,3,4,13,42,0), d4,
                             "Did not convert Date correctly");

            DateTime d5 = DateTimeUtilities.DateTimeForYYYYMMDDFormat(20040304);
            Assert.AreEqual( new DateTime(2004,3,4,0,0,0), d5, 
                             "Did not convert Date correctly for YYYYMMDD");

            DateTime d6 = DateTimeUtilities.DateTimeForYYMMDDFormat(40304);
            Assert.AreEqual( new DateTime(2004,3,4,0,0,0), d6,
                             "Did not convert Date correctly for YYMMDD" );

            DateTime d7 = DateTimeUtilities.DateTimeForYYYYMMDDFormat("0    ");
            Assert.AreEqual( DateTime.MinValue, d7,
                             "Did not convert Date correctly for YYMMDD" );

            DateTime d8 = DateTimeUtilities.DateTimeForYYMMDDFormat(304);
            Assert.AreEqual( new DateTime(2000,3,4,0,0,0), d8,
                             "Did not convert Date correctly for YYMMDD" );

            DateTime d9 = DateTimeUtilities.DateTimeForYYMMDDFormat(11204);
            Assert.AreEqual( new DateTime(2001,12,4,0,0,0), d9,
                             "Did not convert Date correctly for YYMMDD" );

            DateTime d10 = DateTimeUtilities.DateTimeForYYMMDDFormat(-11204);
            Assert.AreEqual( new DateTime(2001,12,4,0,0,0), d10,
                             "Did not convert Date correctly for YYMMDD" );

        }
        
        [Test()]
        public void TestInvalidDates()
        {
            DateTime d1 = DateTimeUtilities.DateTimeForYYDDMMFormat(999999);
            Assert.AreEqual(DateTime.MinValue, d1, "Invalid Date did not return minDate");
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}