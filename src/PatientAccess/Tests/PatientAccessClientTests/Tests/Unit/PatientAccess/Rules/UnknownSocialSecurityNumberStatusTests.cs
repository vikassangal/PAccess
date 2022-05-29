using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;

namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for UnknownSocialSecurityNumberStatusTests
    /// </summary>
    [TestFixture]
    public class UnknownSocialSecurityNumberStatusTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFloridaSSN777777777_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("777777777");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn};
            var address = new Address("addr1", "addr2", "Orlando", null, State.Florida, null);
            patient.AddContactPoint(new ContactPoint(address,null,null,TypeOfContactPoint.NewPhysicalContactPointType()));
            var account = new Account { Patient = patient, Facility = FLORIDA_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFloridaSSNNon777777777_ShouldReturnTrue()
        {

           
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn};
            var address = new Address( "addr1", "addr2", "Orlando", null, State.Florida, null );
            patient.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Patient = patient, Facility = FLORIDA_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_WithCaliforniaSSN000000001_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("000000001");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn };
            
            var address = new Address( "addr1", "addr2", "Irvine", null, State.California, null );
            patient.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Patient = patient, Facility = CALIFORNIA_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithCaliforniaSSNNon000000001_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn };
            
            var address = new Address( "addr1", "addr2", "Irvine", null, State.California, null );
            patient.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Patient = patient, Facility = CALIFORNIA_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithOtherStateSSN999999999_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("999999999");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn };
             
            var address = new Address( "addr1", "addr2", "Plano", null, State.NonFloridaNonCalifornia, null );
            patient.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Patient = patient, Facility = TEXAS_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithOtherStateSSNNon999999999_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var patient = new Patient { SocialSecurityNumber = ssn };
            
            var address = new Address( "addr1", "addr2", "Plaon", null, State.NonFloridaNonCalifornia, null );
            patient.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Patient = patient, Facility = TEXAS_FACILITY };
            var ruleUnderTest = new UnknownSocialSecurityNumberStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        private Facility CALIFORNIA_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("ACO");
        private Facility TEXAS_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("DHF");
        private Facility FLORIDA_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("DEL");
  
        #endregion
    }
}
