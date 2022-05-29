using System;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class BedTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown BedTests
        [TestFixtureSetUp()]
        public static void SetUpBedTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownBedTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void TestBed()
        {
            Account anAccount = new Account() ;
            Bed bed = new Bed(1L,DateTime.Now,"Bed001","B001",new Accomodation(1L,DateTime.Now,"SPL","001"),true);
            
            anAccount.Location =  new Location(1L,DateTime.Now, "CA","CA",new NursingStation(1L,DateTime.Now,"NursingStation","NS"),new Room(1L,DateTime.Now,"Room","RM"),bed);
                
            Assert.AreEqual(
                anAccount.Location.Bed.Code ,
                "B001");
            Assert.AreEqual(
                anAccount.Location.Bed.Description ,
                "Bed001");
            Assert.AreEqual(
                anAccount.Location.Bed.IsOccupied ,
                true);
            

        }
        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}