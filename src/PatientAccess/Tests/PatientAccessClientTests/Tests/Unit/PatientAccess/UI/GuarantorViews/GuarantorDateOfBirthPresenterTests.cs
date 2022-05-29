using System;
using System.Windows.Forms;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI;
using PatientAccess.UI.GuarantorViews.Presenters;
using PatientAccess.UI.GuarantorViews.Views;
using PatientAccess.UI.HelperClasses;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.GuarantorViews
{
    [TestFixture]
    public class GuarantorDateOfBirthPresenterTests
    {
        [Test]
        public void TestHandleGuarantorDateOfBirth_WhenActivityIsReg_AdmitDateIsBeforeReleaseDate_InsuranceIsNull_GuarantorIsNull()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), null, null);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );
            presenter.HandleGuarantorDateOfBirth();
            view.AssertWasCalled(v => v.HideMe());
            view.AssertWasNotCalled(v => v.ShowMe()); 
        }

        [Test]
        public void TestHandleGuarantorDateOfBirth_WhenActivityIsReg_AdmitDateAfterRelease_InsuranceIsNull_GuarantorIsNull()
        {
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), null, null);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );
            presenter.HandleGuarantorDateOfBirth();
            view.AssertWasCalled(v => v.ShowMe()); 
        }

        [Test]
        public void TestHandleGuarantorDateOfBirth_WhenActivityIsReg_AdmitDateIsAfterRelease_InsuranceIsWorkersCompensation_GuarantorIsNull()
        {
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, null);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );
            presenter.HandleGuarantorDateOfBirth();
            view.AssertWasCalled(v => v.ShowMe()); 
        }

        [Test]
        public void TestHandleGuarantorDateOfBirth_WhenActivityIsAdmitNewBorn_AdmitDateIsAfterRelease_InsuranceIsNull_GuarantorIsEmployee()
        {
            var guarantor = GetGuarantor( RelationshipType.RELATIONSHIPTYPE_EMPLOYEE );
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), null, guarantor);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            presenter.HandleGuarantorDateOfBirth();
            view.AssertWasCalled(v => v.ShowMe());
        }

        [Test]
        public void TestUpdateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_InsuranceIsWorkersCompensation_GuarantorIsEmployee()
        {
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            presenter.HandleGuarantorDateOfBirth();
            view.AssertWasNotCalled(v => v.ShowMe());
            view.AssertWasCalled(v => v.HideMe()); 
        }

        [Test]
        public void TestUpdateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_InsuranceIsWorkersCompensation_GuarantorIsEmployee_DOBisReset()
        {
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            account.Guarantor.DateOfBirth = DATEOFBIRTH;
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            presenter.HandleGuarantorDateOfBirth();
            Assert.IsTrue(account.Guarantor.DateOfBirth == DateTime.MinValue);
        }

        [Test]
        public void TestUpdateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_InsuranceIsWorkersCompensation_GuarantorIsNotEmployee_DOBisNotReset()
        {
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_MOTHER);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            account.Guarantor.DateOfBirth = DATEOFBIRTH;
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            presenter.HandleGuarantorDateOfBirth();
            Assert.IsTrue(account.Guarantor.DateOfBirth == DATEOFBIRTH);
        }

        [Test]
        public void TestUpdateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_InsuranceIsNotWorkersCompensation_GuarantorIsEmployee_DOBisNotReset()
        {
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetselfpayCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            account.Guarantor.DateOfBirth = DATEOFBIRTH;
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            presenter.HandleGuarantorDateOfBirth();
            Assert.IsTrue(account.Guarantor.DateOfBirth == DATEOFBIRTH);
        }

        [Test]
        public void TestValidateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_DAOBIsInValid_ReturnsFalse()
        {
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();

            messageBoxAdapter.Expect(
                x => x.Show(Arg<string>.Is.Equal(UIErrorMessages.DOB_OUT_OF_RANGE),
                            Arg<string>.Is.Anything,
                            Arg<MessageBoxButtons>.Is.Anything,
                            Arg<MessageBoxIcon>.Is.Anything,
                            Arg<MessageBoxDefaultButton>.Is.Anything))
                            .Return(DialogResult.OK);

            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_EMPLOYEE);
            var insurance = new Insurance();
            insurance.AddCoverage(GetWorkersCompensationCoverage());
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );
            view.UnmaskedText = INVALID_DOB;
            Assert.IsFalse(presenter.ValidateDateOfBirth());
        }

        [Test]
        public void TestValidateGuarantorDateOfBirth_WhenActivityIsReg_AccountCreatedAfterRelease_DAOBIsValid_ReturnsTrue()
        { 
            var guarantor = GetGuarantor(RelationshipType.RELATIONSHIPTYPE_MOTHER);
            var insurance = new Insurance();
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), insurance, guarantor);
            var ruleEngine = MockRepository.GenerateStub<IRuleEngine>();
            var view = GetMockGuarantorDOBView();
            view.UnmaskedText = VALID_DOB;
            var messageBoxAdapter = MockRepository.GenerateMock<IMessageBoxAdapter>();
            var presenter = new GuarantorDateOfBirthPresenter(view, account, ruleEngine, messageBoxAdapter );

            Assert.IsTrue(presenter.ValidateDateOfBirth());
        }

        private static Account GetAccount(Activity activity, DateTime accountCreatedDate, Insurance insurance,
             Guarantor guarantor)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                Insurance = insurance,
                Guarantor = guarantor
            };
        }
        private static Coverage GetWorkersCompensationCoverage()
        {
            var workersCompCoverage = new WorkersCompensationCoverage()
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION)
            };

            return workersCompCoverage;
        }

        private static Guarantor GetGuarantor(string relType)
        {
            var guarantor = new Guarantor();

            var relationShip =
                new Relationship(
                    new RelationshipType(0, DateTime.MinValue, "Relationship", relType),
                    guarantor.GetType(), new Employer().GetType());

            guarantor.AddRelationship(relationShip);
            return guarantor;
        }
        private DateTime GetTestDateAfterFeatureStart()
        {
            return new GuarantorDateOfBirthFeatureManager().FeatureStartDate.AddDays(10);
        }
        private DateTime GetTestDateBeforeFeatureStart()
        {
            return new GuarantorDateOfBirthFeatureManager().FeatureStartDate.AddDays(-10);
        }
        private static IGuarantorDateOfBirthView GetMockGuarantorDOBView()
        {
            return MockRepository.GenerateStub<IGuarantorDateOfBirthView>();
        }

        private static Coverage GetselfpayCoverage()
        {
            var selfpayCoverage = new SelfPayCoverage()
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION)
            };

            return selfpayCoverage;
        }

        #region Constants
        private static readonly DateTime DATEOFBIRTH = new DateTime(1986, 3, 5);
        private readonly String VALID_DOB = "07252010";
        private readonly String INVALID_DOB = "01071725";
        #endregion
    }
}
