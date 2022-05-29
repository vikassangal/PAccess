using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
namespace Tests.Unit.PatientAccess.Rules
{
    /// <summary>
    /// Summary description for UnknownGuarantorSSNStatusTests
    /// </summary>
    [TestFixture]
    public class UnknownGuarantorSSNStatusTests
    {
        #region Test Methods
        [Test]
        public void TestCanBeAppliedToWithInvalidContextType_ShouldReturnTrue()
        {
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            var inValidObjectType = new object();
            var actualResult = ruleUnderTest.CanBeAppliedTo( inValidObjectType );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedToWithNullContext_ShouldReturnTrue()
        {
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            var actualResult = ruleUnderTest.CanBeAppliedTo( null );
            Assert.IsTrue( actualResult );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFloridaSSN777777777_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("777777777");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
            var address = new Address( "addr1", "addr2", "Orlando", null, State.Florida, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor,  Facility = FLORIDA_FACILITY};
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithFloridaSSNNon777777777_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
           
            var address = new Address( "addr1", "addr2", "Orlando", null, State.Florida, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor , Facility = FLORIDA_FACILITY};
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        [Ignore()]
        public void TestCanBeAppliedTo_WithCaliforniaSSN000000001_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("000000001");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
             
            var address = new Address( "addr1", "addr2", "Irvine", null, State.California, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor, Facility = CALIFORNIA_FACILITY};
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithCaliforniaSSNNon000000001_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
             
            var address = new Address( "addr1", "addr2", "Irvine", null, State.California, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor, Facility = CALIFORNIA_FACILITY};
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithOtherStateSSN999999999_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("999999999");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
             
            var address = new Address( "addr1", "addr2", "Plano", null, State.NonFloridaNonCalifornia, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor, Facility = TEXAS_FACILITY};
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsFalse( ruleUnderTest.CanBeAppliedTo( account ) );
        }

        [Test]
        public void TestCanBeAppliedTo_WithOtherStateSSNNon999999999_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };
             
            var address = new Address( "addr1", "addr2", "Plaon", null, State.NonFloridaNonCalifornia, null );
            guarantor.AddContactPoint( new ContactPoint( address, null, null, TypeOfContactPoint.NewPhysicalContactPointType() ) );
            var account = new Account { Guarantor = guarantor, Facility = TEXAS_FACILITY };
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsTrue( ruleUnderTest.CanBeAppliedTo( account ) );
        }
        [Test]
        public void TestCanBeAppliedTo_WithSouthCarolinaSSN000000001_ShouldReturnFalse()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("000000001");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };

            var address = new Address("addr1", "addr2", "Irvine", null, State.California, null);
            guarantor.AddContactPoint(new ContactPoint(address, null, null, TypeOfContactPoint.NewPhysicalContactPointType()));
            var account = new Account { Guarantor = guarantor, Facility = GetSouthCarolinaFacility() };
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsFalse(ruleUnderTest.CanBeAppliedTo(account));
        }

        [Test]
        public void TestCanBeAppliedTo_WithSouthCarolinaSSNNon000000001_ShouldReturnTrue()
        {
            SocialSecurityNumber ssn = new SocialSecurityNumber("555555555");
            ssn.SSNStatus = SocialSecurityNumberStatus.UnknownSSNStatus;
            var guarantor = new Guarantor() { SocialSecurityNumber = ssn };

            var address = new Address("addr1", "addr2", "Irvine", null, State.California, null);
            guarantor.AddContactPoint(new ContactPoint(address, null, null, TypeOfContactPoint.NewPhysicalContactPointType()));
            var account = new Account { Guarantor = guarantor, Facility = GetSouthCarolinaFacility() };
            var ruleUnderTest = new UnknownGuarantorSSNStatus();
            Assert.IsTrue(ruleUnderTest.CanBeAppliedTo(account));
        }

        private static Facility GetSouthCarolinaFacility()
        {
            var facility = new Facility(99,
                                            PersistentModel.NEW_VERSION,
                                            "ICE",
                                            "ICE");
            var facilityState = new State(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "South carolina", "SC");
            facility.FacilityState = facilityState;
            facility["IsSouthCarolinaDefaultSSNFacility"] = true;
            facility.AddContactPoint(new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode("75075"),
                       facilityState,
                        Country.NewUnitedStatesCountry(),
                        new County("1")),
                    new PhoneNumber("123", "1234567"),
                    new EmailAddress("someone@ps.net"),
                    TypeOfContactPoint.NewPhysicalContactPointType()));

            return facility;
        }

        private Facility CALIFORNIA_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("ACO");
        private Facility TEXAS_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("DHF");
        private Facility FLORIDA_FACILITY = BrokerFactory.BrokerOfType<IFacilityBroker>().FacilityWith("DEL");
  
        #endregion

    }
}
