using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for AdmitTimePreferredTests
    /// </summary>
    [TestFixture]
    [Category( "Fast" )]
    public class LocationPreferredTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new LocationPreferred();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new LocationPreferred();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WhenContextIsNotAccount_ShouldReturnTrue()
        {
            var otherCoverage = new GovernmentOtherCoverage { CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION ) };
            var ruleUnderTest = new LocationPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( otherCoverage ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithLocation_ShouldReturnTrue()
        {
            var account = new Account
            {
                Location = new Location
                {
                    NursingStation = new NursingStation( 0, DateTime.Now, "testNursingDesc", "testNursingCode" ),
                    Room = new Room( 0, DateTime.Now, "testRoomDesc","testRoomCode" ),
                    Bed = new Bed( 0, DateTime.Now, "testBedDesc","testBedCode" )
                }
            };
            var ruleUnderTest = new LocationPreferred();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutNursingStation_ShouldReturnFalse()
        {
            var account = new Account
            {
                Location = new Location
                {
                    Room = new Room( 0, DateTime.Now, "testRoomDesc", "testRoomCode" ),
                    Bed = new Bed( 0, DateTime.Now, "testBedDesc", "testBedCode" )
                }
            };
            var ruleUnderTest = new LocationPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutRoom_ShouldReturnFalse()
        {
            var account = new Account
            {
                Location = new Location
                {
                    NursingStation = new NursingStation( 0, DateTime.Now, "testNursingDesc", "testNursingCode" ),
                    Bed = new Bed( 0, DateTime.Now, "testBedDesc", "testBedCode" )
                }
            };
            var ruleUnderTest = new LocationPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithoutBed_ShouldReturnFalse()
        {
            var account = new Account
            {
                Location = new Location
                {
                    NursingStation = new NursingStation( 0, DateTime.Now, "testNursingDesc", "testNursingCode" ),
                    Room = new Room( 0, DateTime.Now, "testRoomDesc", "testRoomCode" ),
                }
            };
            var ruleUnderTest = new LocationPreferred();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        #endregion

        #region Support Methods

        #endregion

        #region Constants

        #endregion
    }
}
