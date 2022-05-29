using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.BrokerInterfaces;
using PatientAccess.Domain;
using PatientAccess.UI.DemographicsViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DemographicsViews
{
    [TestFixture]
    [Category("Fast")]
    public class RaceViewPresenterTests
    {
        #region Constants and Variables

        private static IOriginBroker i_OriginBroker = null;
        public const long DHF_FACILITYID = 54;

        #endregion  

        #region SetUp and TearDown OriginBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpOriginBrokerTests()
        {
            i_OriginBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        }

        #endregion
        [Test]
        public void TestUpdateModelIfRaceIsSelectedInRaceControl_ShouldUpdateRace()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Race.RACENATIONALITY_CONTROL);
            Race race = new Race {Code = "5", Description = "Asian", ParentRaceCode = string.Empty};
            presenter.RaceCollection = i_OriginBroker.LoadRaces(DHF_FACILITYID);
            presenter.NationalityCollection = i_OriginBroker.LoadNationalities(DHF_FACILITYID);
            presenter.RaceNationalityDictionary = presenter.BuildRaceNationalityDictionary();
            presenter.BuildNationality();
            presenter.UpdateRaceAndNationalityModelValue(race);
            Assert.AreEqual(race, presenter.ModelAccount.Patient.Race, "Patient race should be updated.");
        }
        [Test]
        public void TestUpdateModelIfRaceIsSelectedInRace2Control_ShouldUpdateRace()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Race.RACENATIONALITY2_CONTROL);
            Race race = new Race { Code = "5", Description = "Asian", ParentRaceCode = string.Empty };
            presenter.RaceCollection = i_OriginBroker.LoadRaces(DHF_FACILITYID);
            presenter.NationalityCollection = i_OriginBroker.LoadNationalities(DHF_FACILITYID);
            presenter.RaceNationalityDictionary = presenter.BuildRaceNationalityDictionary();
            presenter.BuildNationality();
            presenter.UpdateRaceAndNationalityModelValue(race);
            Assert.AreEqual(race, presenter.ModelAccount.Patient.Race2, "Patient race2 should be updated.");
        }
        [Test]
        public void TestUpdateModelIfNationalityIsSelectedInRaceField_ShouldUpdateNationality()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Race.RACENATIONALITY_CONTROL);
            Race nationality = new Race { Code = "501", Description = "Asian Indian", ParentRaceCode = "5" };
            presenter.RaceCollection = i_OriginBroker.LoadRaces(DHF_FACILITYID);
            presenter.NationalityCollection = i_OriginBroker.LoadNationalities(DHF_FACILITYID);
            presenter.RaceNationalityDictionary = presenter.BuildRaceNationalityDictionary();
            presenter.BuildNationality();
            presenter.UpdateRaceAndNationalityModelValue(nationality);
            Assert.AreEqual(nationality, presenter.ModelAccount.Patient.Nationality, "Patient nationality should be updated.");
        }
        [Test]
        public void TestUpdateModelIfNationalityIsSelectedInRace2Field_ShouldUpdateNationality2()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Race.RACENATIONALITY2_CONTROL);
            Race nationality = new Race { Code = "501", Description = "Asian Indian", ParentRaceCode = "5" };
            presenter.RaceCollection = i_OriginBroker.LoadRaces(DHF_FACILITYID);
            presenter.NationalityCollection = i_OriginBroker.LoadNationalities(DHF_FACILITYID);
            presenter.RaceNationalityDictionary = presenter.BuildRaceNationalityDictionary();
            presenter.BuildNationality();
            presenter.UpdateRaceAndNationalityModelValue(nationality);
            Assert.AreEqual(nationality, presenter.ModelAccount.Patient.Nationality2, "Patient nationality2 should be updated.");
        }
        [Test]
        public void TestIfNationalityBelongsToCorrectRace()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Race.RACENATIONALITY_CONTROL);
            Race race = new Race { Code = "501", Description = "Asian Indian", ParentRaceCode = "5"};
            presenter.RaceCollection = i_OriginBroker.LoadRaces(DHF_FACILITYID);
            presenter.NationalityCollection = i_OriginBroker.LoadNationalities(DHF_FACILITYID);
            presenter.RaceNationalityDictionary = presenter.BuildRaceNationalityDictionary();
            presenter.BuildNationality();
            Race parentRace = presenter.GetKeyRaceFromDictionary(race.Code);
            if ( parentRace !=null )
            Assert.AreEqual(parentRace.Code, race.ParentRaceCode, "Patient nationality should belong to correct race.");
        }
        private static Account GetAccount(Activity activity, VisitType visitType)
        {
            var facility = new Facility(PersistentModel.NEW_OID,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");
           
            return new Account
            {
                Activity = activity,
                Facility = facility,
                AdmitDate = new DateTime(2019, 04, 02),
                AccountCreatedDate = DateTime.Now,
                KindOfVisit = visitType,
            };
        }
        private RaceViewPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, string raceNationalityControl)
        {
            var view = MockRepository.GenerateMock<IRaceView>();
            var account = GetAccount(activity, patientType);

            return new RaceViewPresenter(view, account, raceNationalityControl);
        }

    }
}
