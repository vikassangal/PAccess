using System.Collections;
using NUnit.Framework;
using PatientAccess.Domain;

namespace Tests.Unit.PatientAccess.Domain
{
    [TestFixture]
    [Category( "Fast" )]
    public class RoomTests
    {
        #region Constants
        #endregion

        #region SetUp and TearDown RoomTests
        [TestFixtureSetUp()]
        public static void SetUpRoomTests()
        {
        }

        [TestFixtureTearDown()]
        public static void TearDownRoomTests()
        {
        }
        #endregion

        #region Test Methods
        [Test()]
        public void testRoom()
        {
           
            Room room1 = new Room(3,ReferenceValue.NEW_VERSION,"Room1","001");
            room1.RoomCondition = new RoomCondition(2L,ReferenceValue.NEW_VERSION,"Good","001");
           
            Assert.AreEqual(
                typeof(Room),
                room1.GetType()
                );
                 
            Assert.AreEqual(
                "Room1",
                room1.Description
                );
            Assert.AreEqual(
                "001",
                room1.Code
                );
            Assert.AreEqual(
                3,
                room1.Oid
                );
            Assert.AreEqual(
                "Good",
                room1.RoomCondition.Description
                );
            Assert.AreEqual(
                "001",
                room1.RoomCondition.Code
                );
            Room room2 = new Room(3,ReferenceValue.NEW_VERSION,"Room2","002");
            Room room3 = new Room(3,ReferenceValue.NEW_VERSION,"Room3","003");
            ArrayList rooms = new ArrayList();
            rooms.Add(room1);
            rooms.Add(room2);
            rooms.Add(room3);
            Assert.AreEqual(3,
                            rooms.Count                         
                );
            Assert.IsTrue(rooms.Contains(room2) );
        }

        #endregion

        #region Support Methods
        #endregion

        #region Data Elements
        #endregion
    }
}