using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.DemographicsViews;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.DemographicsViews
{
    [TestFixture]
    [Category("Fast")]
    public class PatientNamePresenterTests
    {
        #region Tests

        [Test]
        public void TestSetNewBornName_GenderIsM_ShouldSetNewBornNameAsExpected()
        {
            var gender = new Gender(0, DateTime.Now, "Male", "M");
            var name = new Name("SALLY", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYBSALLY");
        }

        [Test]
        public void TestSetNewBornName_GenderIsM_MotherFirstNameGreaterThan13_ShouldSetBornNameAfterTruncating()
        {
            var gender = new Gender(0, DateTime.Now, "Female", "F");
            var name = new Name("CHRISTABELLE", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYGCHRISTAB");
        }

        [Test]
        public void TestSetNewBornName_GenderIsF_ShouldSetNewBornNameAsExpected()
        {
            var gender = new Gender(0, DateTime.Now, "Female", "F");
            var name = new Name("SALLY", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYGSALLY");
        }
        [Test]
        public void TestSetNewBornName_GenderIsF_MotherFirstNameGreaterThan13_ShouldSetBornNameAfterTruncating()
        {
            var gender = new Gender(0, DateTime.Now, "Female", "F");
            var name = new Name("JOHANNESBURG", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYGJOHANNES");
        }
        [Test]
        public void TestSetNewBornName_GenderIsU_ShouldSetNewBornNameAsEmpty()
        {
            var gender = new Gender(0, DateTime.Now, "Unknown", "U");
            var name = new Name("SALLY", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "");
        }

        [Test]
        public void TestSetNewBornName_GenderIsU_MotherFirstNameGreaterThan13_ShouldSetBornNameAsEmpty()
        {
            var gender = new Gender(0, DateTime.Now, "Unknown", "U");
            var name = new Name("JOHANNESBURG", "WOOD", "M");
            var view = GetStubPatientNameView(gender, name);

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "");
        }

        [Test]
        public void TestSetNewBornName_GenderIsM_EmptyMothersName_ShouldSetNewBornNameAsExpected()
        {
            var gender = new Gender(0, DateTime.Now, "Male", "M");
            var view = GetStubPatientNameViewWithoutMothersAccount(gender, new Name(String.Empty, String.Empty, String.Empty));

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYB");
        }

        [Test]
        public void TestSetNewBornName_GenderIsF_EmptyMothersName_ShouldSetNewBornNameAsExpected()
        {
            var gender = new Gender(0, DateTime.Now, "Female", "F");
            var view = GetStubPatientNameViewWithoutMothersAccount(gender, new Name(String.Empty, String.Empty, String.Empty));

            view.PatientNamePresenter.AutoPopulateNewBornName();

            Assert.IsTrue(view.PatientFirstName == "BABYG");
        }

        #endregion

        #region Support Methods
        private static IPatientNameView GetStubPatientNameView(Gender gender, Name name)
        {


            var patient = new Patient { Name = name };
            var mothersAccount = new Account { Patient = patient };
            var newbornPatient = new Patient { MothersAccount = mothersAccount };

            var newBornAccount = new Account
            {
                Activity = new AdmitNewbornActivity(),
                AdmitDate = DateTime.Today,
                Patient = newbornPatient,

            };

            var view = MockRepository.GenerateStub<IPatientNameView>();
            var diagnosisViewPresenter = new PatientNamePresenter(view , new PatientNameFeatureManager());
            view.ModelAccount = newBornAccount;
            view.PatientFirstName = string.Empty;
            view.PatientGender = gender;
            view.PatientNamePresenter = diagnosisViewPresenter;

            return view;
        }
        private static IPatientNameView GetStubPatientNameViewWithoutMothersAccount(Gender gender, Name name)
        {


            var newbornPatient = new Patient();

            var newBornAccount = new Account
            {
                Activity = new AdmitNewbornActivity(),
                AdmitDate = DateTime.Today,
                Patient = newbornPatient,

            };

            var view = MockRepository.GenerateStub<IPatientNameView>();
            var diagnosisViewPresenter = new PatientNamePresenter(view, new PatientNameFeatureManager());
            view.ModelAccount = newBornAccount;
            view.PatientFirstName = string.Empty;
            view.PatientGender = gender;
            view.PatientNamePresenter = diagnosisViewPresenter;

            return view;
        }
        #endregion
    }
}
