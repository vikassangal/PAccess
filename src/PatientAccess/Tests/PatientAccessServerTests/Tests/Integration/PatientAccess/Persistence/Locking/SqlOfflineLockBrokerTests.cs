using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Locking;
using PatientAccess.Locking;
using PatientAccess.Persistence.Locking;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Locking
{
    [TestFixture]
    [Category( "Fast" )]
    public class SqlOfflineLockBrokerTests
    {
        [Test]
        public void TestAddLockEntry()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();

            DateTime timeBeforeAddingLock = DateTime.Now;

            //The SQL server 2005 datetime data type is only precise upto 3.33 milliseconds 
            //so adding a delay so that the lock is refreshed after no less than 1 second after it was created 
            //if this is not done then the test will report a failure even though the lock was properly refreshed
            Thread.Sleep( TimeSpan.FromSeconds( 1 ) );

            bool isEntryAdded = lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            DateTime timePrintForNewLock = GetTimePrintFor( lockHandle, lockOwner );

            Assert.IsTrue( isEntryAdded, "The lock entry should have been added" );
            Assert.IsTrue( timePrintForNewLock >= timeBeforeAddingLock, "The lock should have a time stamp" );
        }


        [Test]
        public void TestAddLockEntry_WhenLockEntryAlreadyExists_ShouldReturnTrue()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            var isEntryAdded = lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );

            Assert.IsTrue( isEntryAdded, "The method should return true as the entry already exists" );
        }

        [Test]
        public void TestAddLockEntry_WhenLockIsOwnedBySomeoneElse_ShouldReturnFalse()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();
            var otherUser = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            var isEntryAdded = lockBrokerUnderTest.AddLockEntry( lockHandle, otherUser, ResourceType.FacilityForNewEmployerManagementFeature );

            Assert.IsFalse( isEntryAdded, "The lock should not be acquired as it is owned by someone else" );
        }

        [Test]
        public void TestRemoveLockEntryWhenLockExists_ShouldThrowNotThrowException()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();

            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            lockBrokerUnderTest.RemoveLockEntry( lockHandle, lockOwner );
        }

        [Test]
        public void TestRemoveLockEntry_WhenLockDoesNotExist_ShouldNotThrowException()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();

            lockBrokerUnderTest.RemoveLockEntry( lockHandle, lockOwner );
        }

        [Test]
        public void TestDoesLockEntryExist()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            var lockEntryExists = lockBrokerUnderTest.DoesLockEntryExist( lockHandle, lockOwner );

            Assert.IsTrue( lockEntryExists, "The lock entry's existence should have been confirmed." );
        }

        [Test]
        public void TestDoesLockEntryExist_WithSameLockDifferentOwner_ShouldReturnFalse()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();
            var notLockOwner = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            var lockEntryExists = lockBrokerUnderTest.DoesLockEntryExist( lockHandle, notLockOwner );

            Assert.IsFalse( lockEntryExists, "The lock should not be reported as existing becuase it is owned by someone else" );
        }

        [Test]
        public void TestDoesLockEntryExist_WhenLockDoesNotExist_ShouldReturnFalse()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            var lockEntryExists = lockBrokerUnderTest.DoesLockEntryExist( lockHandle, lockOwner );
            Assert.IsFalse( lockEntryExists, "The lock entry's non-existence should have been confirmed." );
        }

        [Test]
        public void TestRefreshLock()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );

            var timeBeforeRefresh = GetTimePrintFor( lockHandle, lockOwner );

            //The SQL server 2005 datetime data type is only precise upto 3.33 milliseconds 
            //so adding a delay so that the lock is refreshed after no less than 1 second after it was created 
            //if this is not done then the test will report a failure even though the lock was properly refreshed
            Thread.Sleep( TimeSpan.FromSeconds( 1 ) );

            lockBrokerUnderTest.RefreshLock( lockHandle, lockOwner );

            var timeAfterRefresh = GetTimePrintFor( lockHandle, lockOwner );

            Assert.IsTrue( timeAfterRefresh > timeBeforeRefresh, "The lock should have been refreshed" );
        }

        [Test]
        [ExpectedException( typeof( LockingException ) )]
        public void TryRefreshingNonExistentLock_ShouldThrowException()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();

            lockBrokerUnderTest.RefreshLock( lockHandle, lockOwner );
        }

        [Test]
        [ExpectedException( typeof( LockingException ) )]
        public void TryRefreshingLockOwnedBySomeoneElse_ShouldThrowException()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();
            var someOneElse = Guid.NewGuid().ToString();


            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );
            lockBrokerUnderTest.RefreshLock( lockHandle, someOneElse );
        }

        [Test]
        public void TestCreationViaBrokerFactory()
        {
            IOfflineLockBroker broker = BrokerFactory.BrokerOfType<IOfflineLockBroker>();
            Assert.IsNotNull( broker, "Broker factory should return a valid broker object" );
        }

        [Test]
        public void TestGetLockHandlesForWhenAnEntryWithTheGivenLockTypeExists()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();
            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.OnlinePreregistrationSubmission );

            var results = lockBrokerUnderTest.GetLocksOfType( ResourceType.OnlinePreregistrationSubmission );

            Assert.IsTrue( results.Count() == 1 );
        }

        [Test]
        public void TestGetLockHandlesForWhenAnEntryWithTheGivenLockTypeDoesNotExist()
        {
            var lockBrokerUnderTest = new SqlOfflineLockBroker();
            var lockHandle = Guid.NewGuid().ToString();
            var lockOwner = Guid.NewGuid().ToString();
            lockBrokerUnderTest.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature );

            var results = lockBrokerUnderTest.GetLocksOfType( ResourceType.OnlinePreregistrationSubmission );

            Assert.IsTrue( results.Count() == 0 );
        }

        [TearDown]
        public void TearDown()
        {
            DeleteAllLockEntries();
        }

        [TestFixtureTearDown]
        public static void ClassCleanup()
        {
            DeleteAllLockEntries();
        }

        private static void DeleteAllLockEntries()
        {
            const string deleteAllLockEntriesSql = @"DELETE Locking.OFFLINELOCKS";
            var command = new SqlCommand
                                      {
                                          CommandText = deleteAllLockEntriesSql,
                                          CommandType = CommandType.Text,
                                          Connection = new SqlConnection( GetConnectionString() )
                                      };


            var connection = command.Connection;

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }

            finally
            {
                connection.Close();
                connection.Dispose();
                command.Dispose();
            }
        }


        private static DateTime GetTimePrintFor( string lockHandle, string lockOwner )
        {
            string timePrintSql = @"SELECT TIMEPRINT FROM Locking.OFFLINELOCKS WHERE HANDLE = '" + lockHandle + "' AND OWNER = '" + lockOwner + "'";
            SqlCommand command1 = new SqlCommand();

            command1.CommandText = timePrintSql;
            command1.CommandType = CommandType.Text;


            command1.Connection = new SqlConnection( GetConnectionString() );
            SqlConnection connection = command1.Connection;

            try
            {
                connection.Open();
                DateTime time = (DateTime)command1.ExecuteScalar();
                return time;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                command1.Dispose();
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
    }
}