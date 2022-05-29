using System.Linq;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews;

namespace Tests.Unit.PatientAccess.UI.InsuranceViews
{
    [TestFixture]
    public class MspEvaluateFinancialClassesView
    {
        #region Test Methods
        [Test]
        public void WhenPatientNotEmployedSpouseNotEmployedGhpNo_ThenCondCode11IsNotAdded()
        {
            var account = GetAccountWithMedicareCoverage();
            var patientEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
            var spouseEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
            var groupHealthPlanCoverage = new YesNoFlag();
            groupHealthPlanCoverage.SetNo();
            var familyMemberGHPFlag = new YesNoFlag();
            groupHealthPlanCoverage.SetNo();
            SetDisabilityMspForAccount( account, patientEmployment,
                                       spouseEmployment,
                                       groupHealthPlanCoverage, familyMemberGHPFlag );

            var financialClassesView = GetStubFinancialClassesView( account );
            financialClassesView.UpdateView();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            var conditionCodeAdded = conditionCodes.Any(x => x.Code == ConditionCode.CONDITIONCODE_DISABILITY_NO_GHP);
            
            Assert.IsFalse( conditionCodeAdded );
        }

        [Test]
        public void WhenPatientEmployedSpouseNotEmployedGhpNo_ThenCondCode11IsAdded()
        {
            var account = GetAccountWithMedicareCoverage();
            var patientEmployment = new Employment { Status = EmploymentStatus.NewFullTimeEmployed() };
            var spouseEmployment = new Employment { Status = EmploymentStatus.NewNotEmployed() };
            var groupHealthPlanCoverage = new YesNoFlag();
            groupHealthPlanCoverage.SetNo();
            var familyMemberGHPFlag = new YesNoFlag();
            groupHealthPlanCoverage.SetNo();
            SetDisabilityMspForAccount( account, patientEmployment,
                                       spouseEmployment,
                                       groupHealthPlanCoverage, familyMemberGHPFlag );

            var financialClassesView = GetStubFinancialClassesView( account );
            financialClassesView.UpdateView();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            var conditionCodeAdded = conditionCodes.Any(x => x.Code == ConditionCode.CONDITIONCODE_DISABILITY_NO_GHP);
            Assert.IsTrue( conditionCodeAdded );
        }

        [Test]
        public void WhenPatientNotEmployedSpouseNotEmployedGhpYes_ThenCondCode11IsNotAdded()
        {
            var account = GetAccountWithMedicareCoverage();
            var patientEmployment = new Employment() { Status = EmploymentStatus.NewNotEmployed() };
            var spouseEmployment = new Employment() { Status = EmploymentStatus.NewNotEmployed() };
            var groupHealthPlanCoverage = new YesNoFlag();
            groupHealthPlanCoverage.SetYes();
            var familyMemberGHPFlag = new YesNoFlag();
            groupHealthPlanCoverage.SetNo();
            SetDisabilityMspForAccount( account, patientEmployment,
                                       spouseEmployment,
                                       groupHealthPlanCoverage, familyMemberGHPFlag );

            var financialClassesView = GetStubFinancialClassesView( account );
            financialClassesView.UpdateView();
            var conditionCodes = account.ConditionCodes.Cast<ConditionCode>();

            var conditionCodeAdded = conditionCodes.Any(x => x.Code == ConditionCode.CONDITIONCODE_DISABILITY_NO_GHP);
            Assert.IsFalse( conditionCodeAdded );
        }

        private static FinancialClassesView GetStubFinancialClassesView( Account account )
        {
            var financialClassesView = new FinancialClassesView();
            financialClassesView.Model_Account = account;
            return financialClassesView;
        }

        private static Account GetAccountWithMedicareCoverage()
        {
            Account account = new Account();
            var facility = new Facility( 54,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF" );

            account.Facility = facility;
            User.SetCurrentUserTo( User.NewInstance() );
            User.GetCurrent().Facility = account.Facility;

            account.Activity = new RegistrationActivity();
            Insurance insurance = new Insurance();
            GovernmentMedicareCoverage medicareCoverage = new GovernmentMedicareCoverage();
            medicareCoverage.CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION );

            Payor p = new Payor();
            p.Code = "53";
            InsurancePlan insurancePlan = new GovernmentMedicareInsurancePlan();
            insurancePlan.Payor = p;
            insurancePlan.PlanSuffix = "544";

            EmploymentStatus status = EmploymentStatus.NewRetired();
            Employment PatientEmployment = new Employment();
            PatientEmployment.Status = status;

            medicareCoverage.InsurancePlan = insurancePlan;
            insurance.AddCoverage( medicareCoverage );

            account.Insurance = insurance;
            return account;
        }

        private static void SetDisabilityMspForAccount( Account account, Employment patientEmployment, Employment spouseEmployment, YesNoFlag groupHealthPlanCoverage, YesNoFlag familyMemberGHPFlag )
        {
            var disabilityEntitlement = new DisabilityEntitlement
                                            {
                                                PatientEmployment = patientEmployment,
                                                SpouseEmployment = spouseEmployment,
                                                GroupHealthPlanCoverage = groupHealthPlanCoverage,
                                                FamilyMemberGHPFlag = familyMemberGHPFlag
                                            };


            account.MedicareSecondaryPayor.MedicareEntitlement = disabilityEntitlement;
        }
        #endregion
    }
}