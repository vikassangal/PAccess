using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI.CommonControls;
namespace Tests.Unit.PatientAccess.UI.CommonControls
{
    [TestFixture]
    public class SSNViewPopulatorTests
    {
        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsBaylorForDemographicsView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.DemographicsView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetBaylorFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();
            populator.Populate( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );
            Assert.IsTrue( statusSet.NewbornStatus, "Newborn status is not Found for Baylor facility, demographics view" );
            Assert.IsTrue( statusSet.RefusedStatus, "Refused status is not Found for Baylor facility, demographics view" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for Baylor facility, demographics view" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for Baylor facility, demographics view" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsBaylorForGuarantorView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.GuarantorView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetBaylorFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();
            populator.Populate( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );

            Assert.IsFalse( statusSet.NewbornStatus, "Newborn status is not Found for Baylor facility, guarantor view" );
            Assert.IsTrue( statusSet.RefusedStatus, "Refused status is not Found for Baylor facility, guarantor view" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for Baylor facility, guarantor view" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for Baylor facility, guarantor view" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsFloridaForDemographicsView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.DemographicsView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetFloridaFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();
            populator.Populate( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );

            Assert.IsTrue( statusSet.NewbornStatus, "Newborn status is not Found for Florida facility for Demographics view" );
            Assert.IsFalse( statusSet.RefusedStatus, "Refused status is not Found for Florida facility for Demographics view" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for Florida facility for Demographics view" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for Florida facility for Demographics view" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsFloridaForGuarantorView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.GuarantorView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetFloridaFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();
            populator.Populate( ssnView );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            Assert.IsFalse( statusSet.NewbornStatus, "Newborn status is not Found for Florida facility, Guarantor View" );
            Assert.IsFalse( statusSet.RefusedStatus, "Refused status is not Found for Florida facility, Guarantor View" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for Florida facility, Guarantor View" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for Florida facility, Guarantor View" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsNonBaylorNonFloridaForDemographicsView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.DemographicsView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetCaliforniaFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();

            populator.Populate( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );

            Assert.IsFalse( statusSet.NewbornStatus, "Newborn status is not Found for California facility For DemographicsView" );
            Assert.IsFalse( statusSet.RefusedStatus, "Refused status is not Found for California facility For DemographicsView" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for California facility For DemographicsView" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for California facility For DemographicsView" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsNonBaylorNonFloridaForGuarantorView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.GuarantorView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount( GetCaliforniaFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory( account );
            var populator = ssnFactory.GetPopulator();
            populator.Populate( ssnView );

            Assert.IsTrue( ssnView.SsnStatusCount > 0 );
            var statusSet = GetSSNStatusesFromSSNView( ssnView );

            Assert.IsFalse( statusSet.NewbornStatus, "Newborn status is not Found for California facility For Guarantor view" );
            Assert.IsFalse( statusSet.RefusedStatus, "Refused status is not Found for California facility For Guarantor view" );
            Assert.IsTrue( statusSet.UnknownStatus, "Unknown status is not Found for California facility For Guarantor view" );
            Assert.IsTrue( statusSet.NoneStatus, "None status is not Found for California facility For Guarantor view" );
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsSouthCarolinaWhenPatientAgeisLessThan2YearsForDemographicsViewNewbornStatusIsAvailable()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.DemographicsView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount(GetSouthCarolinaFacility(), DateTime.Today.Subtract( TimeSpan.FromDays(10) ) );
            ssnView.ModelAccount = account;
            var ssnFactory = new SsnPopulatorFactory(account);
            var populator = ssnFactory.GetPopulator();
            populator.Populate(ssnView);

            Assert.IsTrue(ssnView.SsnStatusCount > 0);
            var statusSet = GetSSNStatusesFromSSNView(ssnView);
            Assert.IsTrue(statusSet.NewbornStatus, "Newborn status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.RefusedStatus, "Refused status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.UnknownStatus, "Unknown status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.NoneStatus, "None status is not Found for SouthCarolina facility, demographics view");
        }
        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsSouthCarolinaWhenPatientAgeisMoreThan2YearsForDemographicsViewNewbornStatusIsNotAvailable()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.DemographicsView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount(GetSouthCarolinaFacility(), new DateTime( 2010,6,4 ));
            ssnView.ModelAccount = account;
            var ssnFactory = new SsnPopulatorFactory(account);
            var populator = ssnFactory.GetPopulator();
            populator.Populate(ssnView);

            Assert.IsTrue(ssnView.SsnStatusCount > 0);
            var statusSet = GetSSNStatusesFromSSNView(ssnView);
            Assert.IsFalse(statusSet.NewbornStatus, "Newborn status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.RefusedStatus, "Refused status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.UnknownStatus, "Unknown status is not Found for SouthCarolina facility, demographics view");
            Assert.IsTrue(statusSet.NoneStatus, "None status is not Found for SouthCarolina facility, demographics view");
        }

        [Test]
        public void TestPopulateSSNStatusCodes_WhenFacilityIsSouthCarolinaForGuarantorView()
        {
            var ssnView = new SSNControl { SsnContext = SsnViewContext.GuarantorView };
            ssnView.cmbSSNStatus.Items.Clear();

            var account = GetAccount(GetSouthCarolinaFacility(), new DateTime());
            var ssnFactory = new SsnPopulatorFactory(account);
            var populator = ssnFactory.GetPopulator();
            populator.Populate(ssnView);

            Assert.IsTrue(ssnView.SsnStatusCount > 0);
            var statusSet = GetSSNStatusesFromSSNView(ssnView);

            Assert.IsFalse(statusSet.NewbornStatus, "Newborn status is not Found for SouthCarolina facility, guarantor view");
            Assert.IsTrue(statusSet.RefusedStatus, "Refused status is not Found for SouthCarolina facility, guarantor view");
            Assert.IsTrue(statusSet.UnknownStatus, "Unknown status is not Found for SouthCarolina facility, guarantor view");
            Assert.IsTrue(statusSet.NoneStatus, "None status is not Found for SouthCarolina facility, guarantor view");
        }


        private SsnStatusSet GetSSNStatusesFromSSNView( SSNControl ssnView )
        {
            var ssnStatusSet = new SsnStatusSet();

            foreach ( SocialSecurityNumberStatus ssnStatus in ssnView.cmbSSNStatus.Items )
            {
                switch ( ssnStatus.Description.Trim().ToUpper() )
                {
                    case SocialSecurityNumberStatus.NEWBORN:
                        ssnStatusSet.NewbornStatus = true;
                        break;

                    case SocialSecurityNumberStatus.REFUSED:
                        ssnStatusSet.RefusedStatus = true;
                        break;

                    case SocialSecurityNumberStatus.UNKNOWN:
                        ssnStatusSet.UnknownStatus = true;
                        break;

                    case SocialSecurityNumberStatus.NONE:
                        ssnStatusSet.NoneStatus = true;
                        break;
                }
            }

            return ssnStatusSet;
        }

        private static Account GetAccount(Facility facility, DateTime dateOfBirth)
        {

            var account = new Account
                {
                    Patient = new Patient { DateOfBirth = dateOfBirth },
                    Facility = facility,
                    Activity = new RegistrationActivity(),
                    KindOfVisit =
                        new VisitType(0, PersistentModel.NEW_VERSION, VisitType.OUTPATIENT_DESC,
                                      VisitType.OUTPATIENT)
                };

            User.GetCurrent().Facility = facility;
            User.GetCurrent().PBAREmployeeID = "PACCUSER";
            User.GetCurrent().WorkstationID = "WorkstationID";
            account.Activity.AppUser = User.GetCurrent();

            return account;
        }

        private static Facility GetBaylorFacility()
        {
            var facility = new Facility( 54,
                                            PersistentModel.NEW_VERSION,
                                            "DOCTORS HOSPITAL DALLAS",
                                            "DHF" );
            var facilityState = new State( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Texas", "TX" );
            facility.FacilityState = facilityState;
            facility["IsBaylorFacility"] = true;
            facility.AddContactPoint( new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode( "75075" ),
                       facilityState,
                        Country.NewUnitedStatesCountry(),
                        new County( "1" ) ),
                    new PhoneNumber( "123", "1234567" ),
                    new EmailAddress( "someone@ps.net" ),
                    TypeOfContactPoint.NewPhysicalContactPointType() ) );

            return facility;
        }

        private static Facility GetFloridaFacility()
        {
            var facility = new Facility( 6,
                                            PersistentModel.NEW_VERSION,
                                            "DEL RAY",
                                            "DEL" );
            var facilityState = new State( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "Florida", "FL" );
            facility.FacilityState = facilityState;
            facility["IsBaylorFacility"] = null;
            facility.AddContactPoint( new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode( "75075" ),
                       facilityState,
                        Country.NewUnitedStatesCountry(),
                        new County( "1" ) ),
                    new PhoneNumber( "123", "1234567" ),
                    new EmailAddress( "someone@ps.net" ),
                    TypeOfContactPoint.NewPhysicalContactPointType() ) );
            return facility;
        }

        private static Facility GetCaliforniaFacility()
        {
            var facility = new Facility( 900,
                                            PersistentModel.NEW_VERSION,
                                            "ACO",
                                            "ACO" );
            var facilityState = new State( PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "California", "CA" );
            facility.FacilityState = facilityState;
            facility["IsBaylorFacility"] = null;
            facility.AddContactPoint( new ContactPoint(
                    new Address(
                        "Another Address (New Address)",
                        "Suite 1",
                        "Plano",
                        new ZipCode( "75075" ),
                       facilityState,
                        Country.NewUnitedStatesCountry(),
                        new County( "1" ) ),
                    new PhoneNumber( "123", "1234567" ),
                    new EmailAddress( "someone@ps.net" ),
                    TypeOfContactPoint.NewPhysicalContactPointType() ) );

            return facility;
        }

        private static Facility GetSouthCarolinaFacility()
        {
            var facility = new Facility(98,
                                            PersistentModel.NEW_VERSION,
                                            "ICE",
                                            "ICE");
            var facilityState = new State(PersistentModel.NEW_OID, PersistentModel.NEW_VERSION, "South Carolina", "SC");
            facility.FacilityState = facilityState;
            facility["IsSouthCarolina"] = true ;
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
    }

    internal class SsnStatusSet
    {
        public bool NewbornStatus { get; set; }
        public bool RefusedStatus { get; set; }
        public bool UnknownStatus { get; set; }
        public bool NoneStatus { get; set; }
    }
}