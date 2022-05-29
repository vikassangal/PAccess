using System;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews.MSP2;
using Rhino.Mocks;

namespace Tests.Unit.PatientAccess.UI.MedicareSecondaryPayor
{
    [TestFixture]
    [Category( "Fast" )]
    public class AgeEntitlementPage1Tests
    {
        private static readonly DateTime RETIREMENT_DATE = new DateTime( 2010, 01, 01 );
        private const string SPOUSE_EMPLOYMENT  = "SPOUSE";
        private const string PATIENT_EMPLOYMENT = "PATIENT";
        private const string DATE_FORMAT = "{0:D2}{1:D2}{2:D4}";

        [Test]
        public void PopulateSpouseEmploymentFields_WhenAccountIsNull_ShouldNotDisplaySpouseEmployment()
        {
            var presenter = new AgeEntitlementPage1Presenter( GetStubAgeEntitlementPage1(), null );

            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenMSPIsNull_ShouldNotDisplaySpouseEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor = null;
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenMedicareEntitlementIsNull_ShouldNotDisplaySpouseEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor.MedicareEntitlement = null;
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenNotAgeEntitlement_ShouldNotDisplaySpouseEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor.MedicareEntitlement = new ESRDEntitlement();
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenRetiredAndNoRetirementDate_ShouldDisplaySpouseOtherEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsTrue( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsTrue( view.SpouseRetirementDateText == string.Empty );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenRetiredWithRetirementDate_ShouldDisplaySpouseRetiredEmployment()
        {
            var presenter = GetPresenterWith( RETIREMENT_DATE, EmploymentStatus.NewRetired(), SPOUSE_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsTrue( view.SpouseRetired );
            Assert.AreEqual( view.SpouseRetirementDateText, String.Format( DATE_FORMAT, RETIREMENT_DATE.Month, RETIREMENT_DATE.Day, RETIREMENT_DATE.Year ) );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenFullTimeEmployed_ShouldDisplaySpouseFullOrPartTimeEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewFullTimeEmployed(), SPOUSE_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsTrue( view.SpouseEmployed );
            Assert.IsFalse( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulateSpouseEmploymentFields_WhenNeverEmployed_ShouldDisplaySpouseNeverEmployed()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewNotEmployed(), SPOUSE_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulateSpouseEmploymentFields();

            Assert.IsFalse( view.SpouseEmployed );
            Assert.IsTrue( view.SpouseNeverEmployed );
            Assert.IsFalse( view.SpouseOtherEmployed );
            Assert.IsFalse( view.SpouseRetired );
            Assert.IsNull( view.SpouseRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenAccountIsNull_ShouldNotDisplayPatientEmployment()
        {
            var presenter = new AgeEntitlementPage1Presenter( GetStubAgeEntitlementPage1(), null );

            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenMSPIsNull_ShouldNotDisplayPatientEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor = null;
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenMedicareEntitlementIsNull_ShouldNotDisplayPatientEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor.MedicareEntitlement = null;
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenNotAgeEntitlement_ShouldNotDisplayPatientEmployment()
        {
            AgeEntitlementPage1Presenter presenter =
                GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), SPOUSE_EMPLOYMENT );

            presenter.Account.MedicareSecondaryPayor.MedicareEntitlement = new ESRDEntitlement();
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenRetiredAndNoRetirementDate_ShouldDisplayPatientOtherEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewOtherOrUnknown(), PATIENT_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsTrue( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsTrue( view.PatientRetirementDateText == string.Empty );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenRetiredWithRetirementDate_ShouldDisplayPatientRetiredEmployment()
        {
            var presenter = GetPresenterWith( RETIREMENT_DATE, EmploymentStatus.NewRetired(), PATIENT_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsTrue( view.PatientRetired );
            Assert.IsTrue( view.PatientRetirementDateText == 
                String.Format( "{0:D2}{1:D2}{2:D4}", RETIREMENT_DATE.Month, RETIREMENT_DATE.Day, RETIREMENT_DATE.Year ) );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenFullTimeEmployed_ShouldDisplayPatientFullOrPartTimeEmployment()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewFullTimeEmployed(), PATIENT_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsTrue( view.PatientEmployed );
            Assert.IsFalse( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        [Test]
        public void PopulatePatientEmploymentFields_WhenNeverEmployed_ShouldDisplayPatientNeverEmployed()
        {
            var presenter = GetPresenterWith( DateTime.MinValue, EmploymentStatus.NewNotEmployed(), PATIENT_EMPLOYMENT );
            var view = presenter.View;

            presenter.PopulatePatientEmploymentFields();

            Assert.IsFalse( view.PatientEmployed );
            Assert.IsTrue( view.PatientNeverEmployed );
            Assert.IsFalse( view.PatientOtherEmployed );
            Assert.IsFalse( view.PatientRetired );
            Assert.IsNull( view.PatientRetirementDateText );
        }

        #region Private Methods

        private static AgeEntitlementPage1Presenter GetPresenterWith( DateTime retirementDate, EmploymentStatus empStatus, string employmentType )
        {
            Account account;
            if ( employmentType == SPOUSE_EMPLOYMENT )
            {
                account = GetAccountWithSpouseEmployment( retirementDate, empStatus );
            }
            else
            {
                account = GetAccountWithPatientEmployment( retirementDate, empStatus );
            }

            var view = MockRepository.GenerateStub<IAgeEntitlementPage1View>();
            return new AgeEntitlementPage1Presenter( view, account );
        }

        private static IAgeEntitlementPage1View GetStubAgeEntitlementPage1()
        {
            var ageEntitlementPage1 = MockRepository.GenerateStub<IAgeEntitlementPage1View>();
            return ageEntitlementPage1;
        }

        private static Account GetAccountWithSpouseEmployment( DateTime retirementDate, EmploymentStatus empStatus )
        {
            AgeEntitlement ageEntitlement = new AgeEntitlement
            {
                SpouseEmployment = new Employment
                {
                    Status = empStatus,
                    RetiredDate = retirementDate
                }
            };

            Account account = new Account { MedicareSecondaryPayor = { MedicareEntitlement = ageEntitlement } };

            return account;
        }

        private static Account GetAccountWithPatientEmployment( DateTime retirementDate, EmploymentStatus empStatus )
        {
            AgeEntitlement ageEntitlement = new AgeEntitlement
            {
                PatientEmployment = new Employment
                {
                    Status = empStatus,
                    RetiredDate = retirementDate
                }
            };

            Account account = new Account { MedicareSecondaryPayor = { MedicareEntitlement = ageEntitlement } };

            return account;
        }

        #endregion

    }
}
