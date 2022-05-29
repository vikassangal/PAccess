using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.UI;
using PatientAccess.UI.PhysicianSearchViews;

namespace Tests.Unit.PatientAccess.UI.PhysicianSearchViews
{
    [TestFixture]
    [Category( "Fast" )]
    public class PhysicianSearchFormViewTests
    {
          #region SetUp and TearDown PhysicianPatientsSearchCriteriaTests
        [TestFixtureSetUp()]
        public static void SetUpPhysicianPatientsSearchCriteriaTests()
        {
            Facility facility = new Facility(54,
                PersistentModel.NEW_VERSION,
                "DOCTORS HOSPITAL DALLAS",
                "DHF");
            User.GetCurrent().Facility = facility;
        }

        [TestFixtureTearDown()]
        public static void TearDownPhysicianPatientsSearchCriteriaTests()
        {
        }
     
        #endregion
        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBeNull()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.EMPLOYMENT;

            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());

            Assert.IsNull(physicianSearchForm.PhysicianRelationshipToView);
        }

        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBeOperatingPhysician()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.OPERATINGNONSTAFFPHYSICIAN;

            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());

            Assert.AreEqual(PhysicianRelationship.OPERATING_PHYSICIAN, physicianSearchForm.PhysicianRelationshipToView);
        }

        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBeAttendingPhysician()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.ATTENDINGNONSTAFFPHYSICIAN;

            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());

            Assert.AreEqual(PhysicianRelationship.ATTENDING_PHYSICIAN, physicianSearchForm.PhysicianRelationshipToView);

        }

        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBeAdmittingPhysician()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.ADMITTINGNONSTAFFPHYSICIAN;
            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());


            Assert.AreEqual(PhysicianRelationship.ADMITTING_PHYSICIAN, physicianSearchForm.PhysicianRelationshipToView);

        }

        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBePrimaryCarePhysician()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.PRIMARYCARENONSTAFFPHYSICIAN;

            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());

            Assert.AreEqual(PhysicianRelationship.PRIMARYCARE_PHYSICIAN, physicianSearchForm.PhysicianRelationshipToView);
        }

        [Test]
        public void TestGetPhysicianSearchForm_ScreenIndexIsEmployment_PhysicianRelationshipToViewShouldBeReferringPhysician()
        {
            const AccountView.ScreenIndexes screenIndexes = AccountView.ScreenIndexes.REFERRINGNONSTAFFPHYSICIAN;

            var physicianSearchForm = PhysicianSearchFormView.GetPhysicianSearchForm(screenIndexes, new Account());

            Assert.AreEqual(PhysicianRelationship.REFERRING_PHYSICIAN, physicianSearchForm.PhysicianRelationshipToView);

        }
    }
}
