using System;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain.Locking;
using PatientAccess.Locking;
using Rhino.Mocks;
using NUnit.Framework;

namespace Tests.Integration.PatientAccess.Persistence.Locking
{
    /// <summary>
    /// Summary description for ApplicationLockManagerTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class NewEmployerManagementLockerTests
    {
        [Test]
        public void TestAcquireLock_WithUniqueHandleAndUniqueOwner_ShouldReturnTrue()
        {
            var mockLockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            mockLockBroker.Stub(x => x.AddLockEntry(lockHandle, lockOwner,ResourceType.FacilityForNewEmployerManagementFeature)).Return(true);
            
            var lockManagerUnderTest = new NewEmployerManagementLocker(mockLockBroker, lockHandle, lockOwner );
            
            var isLockAcquired = lockManagerUnderTest.AcquireLock();
            
            Assert.IsTrue(isLockAcquired, "The lock should have been acquired");
            
        }
        
        [Test]
        public void TestAcquireLock_WhenLockIsAlreadyOwnedByTheUser_ShouldReturnTrue()
        {
            var mockLockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            mockLockBroker.Stub( x => x.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature ) ).Return( true );
            var lockManagerUnderTest = new NewEmployerManagementLocker(mockLockBroker, lockHandle, lockOwner);
            
            lockManagerUnderTest.AcquireLock();
            var isLockAcquired = lockManagerUnderTest.AcquireLock();
            
            Assert.IsTrue(isLockAcquired, "The lock should have been acquired");
        }

        [Test]
        public void TestAcquireLock_WhenLockHasAlreadyBeenAcquiredBySomeOtherUser_ShouldReturnFalse()
        {
            var mockLockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            var anotherUser = Guid.NewGuid().ToString();

            mockLockBroker.Stub( x => x.AddLockEntry( lockHandle, anotherUser, ResourceType.FacilityForNewEmployerManagementFeature ) ).Return( false );

            var lockManagerUnderTest = new NewEmployerManagementLocker(mockLockBroker, lockHandle, lockOwner);
            
            lockManagerUnderTest.AcquireLock();

            var anotherLock = new NewEmployerManagementLocker(mockLockBroker, lockHandle, anotherUser);

            var isLockAcquired = anotherLock.AcquireLock();

            Assert.IsFalse(isLockAcquired, "The lock should not have been acquired");
        }

        [Test]
        public void TestVerifyLock_WhenLockIsWasProperlyAcquired_ShouldReturnTrue()
        {
            var mockLockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            mockLockBroker.Stub(x => x.DoesLockEntryExist(lockHandle, lockOwner)).Return(true);
            var lockManagerUnderTest = new NewEmployerManagementLocker(mockLockBroker, lockHandle, lockOwner);
            
            lockManagerUnderTest.AcquireLock();
            var lockExists = lockManagerUnderTest.IsLocked();

            Assert.IsTrue(lockExists, "The lock's existence should have been verified");
        }

        [Test]
        public void TestReleaseLock_WhenLockIsWasProperlyAcquired_ShouldNotThrowAnException()
        {
            var mockLockBroker = MockRepository.GenerateMock<IOfflineLockBroker>();
            string lockHandle = Guid.NewGuid().ToString();
            string lockOwner = Guid.NewGuid().ToString();
            mockLockBroker.Stub( x => x.AddLockEntry( lockHandle, lockOwner, ResourceType.FacilityForNewEmployerManagementFeature ) ).Return( true );
            var lockManagerUnderTest = new NewEmployerManagementLocker(mockLockBroker, lockHandle, lockOwner);
            
            lockManagerUnderTest.AcquireLock();
            lockManagerUnderTest.ReleaseLock();
        }
    }
}