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
    public class EthnicityViewPresenterTests
    {
        #region Constants and Variables

        private static IOriginBroker i_OriginBroker = null;
        public const long ACO_FACILITYID = 900;

        #endregion  

        #region SetUp and TearDown OriginBrokerTests
        [TestFixtureSetUp()]
        public static void SetUpOriginBrokerTests()
        {
            i_OriginBroker = BrokerFactory.BrokerOfType<IOriginBroker>();
        }

        #endregion
        [Test]
        public void TestUpdateModelIfEthnicityIsSelectedInEthnicityControl_ShouldUpdateEthnicity()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Ethnicity.ETHNICITY_PROPERTY);
            Ethnicity ethnicity = new Ethnicity { Code = "1", Description = "HISPANIC", ParentEthnicityCode = string.Empty };
            presenter.EthnictyCollection = i_OriginBroker.LoadEthnicities(ACO_FACILITYID);
            presenter.DescentCollection = i_OriginBroker.LoadDescent(ACO_FACILITYID);
            presenter.EthnicityDictionary = presenter.BuildEthnicityDictionary();
            presenter.BuildDescent();
            presenter.UpdateEthnicityAndDescentModelValue(ethnicity);
            Assert.AreEqual(ethnicity, presenter.ModelAccount.Patient.Ethnicity, "Patient Ethnicity should be updated.");
        }
        [Test]
        public void TestUpdateModelIfEthnicity2IsSelectedInEthnicity2Control_ShouldUpdateEthnicity2()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Ethnicity.ETHNICITY2_PROPERTY);
            Ethnicity ethnicity2 = new Ethnicity { Code = "2", Description = "NON-HISPANIC", ParentEthnicityCode = string.Empty };
            presenter.EthnictyCollection = i_OriginBroker.LoadEthnicities(ACO_FACILITYID);
            presenter.DescentCollection = i_OriginBroker.LoadDescent(ACO_FACILITYID);
            presenter.EthnicityDictionary = presenter.BuildEthnicityDictionary();
            presenter.BuildDescent();
            presenter.UpdateEthnicityAndDescentModelValue(ethnicity2);
            Assert.AreEqual(ethnicity2, presenter.ModelAccount.Patient.Ethnicity2, "Patient Ethnicity2 should be updated.");
        }
        [Test]
        public void TestUpdateModelIfDescentIsSelectedInEthnicityControl_ShouldUpdateDescent()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Ethnicity.ETHNICITY_PROPERTY);
            Ethnicity descent = new Ethnicity { Code = "102", Description = "MEXICAN", ParentEthnicityCode = "1" };
            presenter.EthnictyCollection = i_OriginBroker.LoadEthnicities(ACO_FACILITYID);
            presenter.DescentCollection = i_OriginBroker.LoadDescent(ACO_FACILITYID);
            presenter.EthnicityDictionary = presenter.BuildEthnicityDictionary();
            presenter.BuildDescent();
            presenter.UpdateEthnicityAndDescentModelValue(descent);
            Assert.AreEqual(descent, presenter.ModelAccount.Patient.Descent, "Patient descent should be updated.");
        }
        [Test]
        public void TestUpdateModelIfDescent2IsSelectedInEthnicity2Control_ShouldUpdateDescent2()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Ethnicity.ETHNICITY2_PROPERTY);
            Ethnicity descent2 = new Ethnicity { Code = "105", Description = "LATIN AMERICAN", ParentEthnicityCode = "1" };
            presenter.EthnictyCollection = i_OriginBroker.LoadEthnicities(ACO_FACILITYID);
            presenter.DescentCollection = i_OriginBroker.LoadDescent(ACO_FACILITYID);
            presenter.EthnicityDictionary = presenter.BuildEthnicityDictionary();
            presenter.BuildDescent();
            presenter.UpdateEthnicityAndDescentModelValue(descent2);
            Assert.AreEqual(descent2, presenter.ModelAccount.Patient.Descent2, "Patient descent2 should be updated.");
        }
        [Test]
        public void TestIfDescentBelongsToCorrectEthnicity()
        {

            var presenter = GetPresenterWithMockView(new RegistrationActivity(), VisitType.Inpatient, Ethnicity.ETHNICITY_PROPERTY);
            Ethnicity descent = new Ethnicity { Code = "105", Description = "LATIN AMERICAN", ParentEthnicityCode = "1" };
            presenter.EthnictyCollection = i_OriginBroker.LoadEthnicities(ACO_FACILITYID);
            presenter.DescentCollection = i_OriginBroker.LoadDescent(ACO_FACILITYID);
            presenter.EthnicityDictionary = presenter.BuildEthnicityDictionary();
            presenter.BuildDescent();
            presenter.UpdateEthnicityAndDescentModelValue(descent);
            Ethnicity parentethnicity = presenter.GetKeyEthnicityFromDictionary(descent.Code);
            if (parentethnicity != null)
                Assert.AreEqual(parentethnicity.Code, parentethnicity.ParentEthnicityCode, "Patient descent should belong to correct Ethnicity.");
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
        private EthnicityViewPresenter GetPresenterWithMockView(Activity activity, VisitType patientType, string ethnicityDescentControl)
        {
            var view = MockRepository.GenerateMock<IEthnicityView>();
            var account = GetAccount(activity, patientType);

            return new EthnicityViewPresenter(view, account, ethnicityDescentControl);
        }

    }
}
