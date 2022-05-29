using System;
using Extensions.PersistenceCommon;
using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews;

namespace Tests.Unit.PatientAccess.UI.InsuranceViews

{
    /// <summary>
    /// Summary description for FinancialClassesViewTests
    /// </summary>
    [TestFixture]
    public class FinancialClassesViewTests
    {
        #region Test Methods
        [Test]
        public void TestEvaluateFinancialClassesViewAddMSPRelatedOccurenceCodes_WhenInsuranceMedicareandChangedToNotMedicare()
        {
            var account = GetAccountWithMedicareCoverage();
            var financialClassesView = GetStubFinancialClassesView(account);
            financialClassesView.UpdateView();
            OccurrenceCode i_OCCCode18 = new OccurrenceCode(18, "Retirement Date",
                                               new DateTime(1987, 3, 4)) { Code = OccurrenceCode.OCCURRENCECODE_RETIREDATE };
            Assert.IsTrue(account.OccurrenceCodes.Contains(i_OCCCode18),
                          "Retired Occurence code should be added during updateView of FinancialClassesView for MedicareInsurancePlan ");
        
            var newNonMedicareInsurance = GetNewInsurance();
            account.Insurance = newNonMedicareInsurance;
            financialClassesView = GetStubFinancialClassesView(account);
            financialClassesView.UpdateView();
            Assert.IsFalse(account.OccurrenceCodes.Contains(i_OCCCode18),
                          "Retired Occurence code should be removed during updateView of FinancialClassesView for NonMedicareInsurancePlan ");
        }

        [Test]
        public void TestUpdatePatientLiabilityForFinancialClass17_WhenFacilityIsEnabledAndFinancialClassIs17_And_AccountCreatedAfterImplementationDate()
        {
            var medicaidManagedFC17 = new FinancialClass { Code = "17", Description = "MCAID_MGD_CONTR_CODE" };
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            account.FinancialClass = medicaidManagedFC17;
            
            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Deductible = 100,
                CoPay = 10
            };
            var financialClassesView = GetStubFinancialClassesView(account);
         
            financialClassesView.Model_Account = account;
            financialClassesView.Model_Coverage = medicareCoverage;
            financialClassesView.selectedFinancialClass = medicaidManagedFC17;
            financialClassesView.UpdatePatientLiabilityForFinancialClass17();

           Assert.IsTrue(financialClassesView.Model_Coverage.Deductible == 0 && financialClassesView.Model_Coverage.CoPay==0,
               "Patient Liability value is not reset to 0");
        }
        [Test]
        public void TestUpdatePatientLiabilityForFinancialClass17_WhenFacilityIsEnabledAndFinancialClassIsNot17_And_AccountCreatedAfterImplementationDate()
        {
            var nonmedicaidManagedFC17 = new FinancialClass { Code = "18" };
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            account.FinancialClass = nonmedicaidManagedFC17;

            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Deductible = 100,
                CoPay = 10
            };
            var financialClassesView = GetStubFinancialClassesView(account);

            financialClassesView.Model_Account = account;
            financialClassesView.Model_Coverage = medicareCoverage;
            financialClassesView.selectedFinancialClass = nonmedicaidManagedFC17;
            financialClassesView.UpdatePatientLiabilityForFinancialClass17();

            Assert.IsFalse(financialClassesView.Model_Coverage.Deductible == 0 && financialClassesView.Model_Coverage.CoPay == 0,
                "Patient Liability value is reset to 0");
        }
        [Test]
        public void TestUpdatePatientLiabilityForFinancialClass17_WhenFacilityIsNotEnabledAndFinancialClassIs17_And_AccountCreatedAfterImplementationDate()
        {
            var medicaidManagedFC17 = new FinancialClass { Code = "17" };
            var account = GetAccount(new RegistrationActivity(), GetTestDateAfterFeatureStart(), VisitType.Inpatient);
            account.Facility = DHFFacility;
            account.FinancialClass = medicaidManagedFC17;

            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Deductible = 100,
                CoPay = 10
            };
            var financialClassesView = GetStubFinancialClassesView(account);

            financialClassesView.Model_Account = account;
            financialClassesView.Model_Coverage = medicareCoverage;
            financialClassesView.selectedFinancialClass = medicaidManagedFC17;
            financialClassesView.UpdatePatientLiabilityForFinancialClass17();

            Assert.IsFalse(financialClassesView.Model_Coverage.Deductible == 0 && financialClassesView.Model_Coverage.CoPay == 0,
                "Patient Liability value is reset to 0");
        }
        [Test]
        public void TestUpdatePatientLiabilityForFinancialClass17_WhenFacilityIsEnabledAndFinancialClassIs17_And_FinancialClassChanged_AccountCreatedBeforeImplementationDate()
        {
            var medicaidManagedFC17 = new FinancialClass { Code = "17", Description = "MCAID_MGD_CONTR_CODE" };
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            account.FinancialClass = medicaidManagedFC17;
            account.HasFinancialClassChanged = true;

            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Deductible = 100,
                CoPay = 10
            };
            var financialClassesView = GetStubFinancialClassesView(account);

            financialClassesView.Model_Account = account;
            financialClassesView.Model_Coverage = medicareCoverage;
            financialClassesView.selectedFinancialClass = medicaidManagedFC17;
            financialClassesView.UpdatePatientLiabilityForFinancialClass17();

            Assert.IsTrue(financialClassesView.Model_Coverage.Deductible == 0 && financialClassesView.Model_Coverage.CoPay == 0,
                "Patient Liability value is not reset to 0 when financial class changed to 17");
        }
        [Test]
        public void TestUpdatePatientLiabilityForFinancialClass17_WhenFacilityIsEnabledAndFinancialClassIs17_And_FinancialClassNotChanged_AccountCreatedBeforeImplementationDate()
        {
            var medicaidManagedFC17 = new FinancialClass { Code = "17", Description = "MCAID_MGD_CONTR_CODE" };
            var account = GetAccount(new RegistrationActivity(), GetTestDateBeforeFeatureStart(), VisitType.Inpatient);
            account.Facility = ICEFacility;
            account.FinancialClass = medicaidManagedFC17;
            account.HasFinancialClassChanged = false;

            var medicareCoverage = new GovernmentMedicareCoverage
            {
                CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION),
                Deductible = 100,
                CoPay = 10
            };
            var financialClassesView = GetStubFinancialClassesView(account);

            financialClassesView.Model_Account = account;
            financialClassesView.Model_Coverage = medicareCoverage;
            financialClassesView.selectedFinancialClass = medicaidManagedFC17;
            financialClassesView.UpdatePatientLiabilityForFinancialClass17();

            Assert.IsFalse(financialClassesView.Model_Coverage.Deductible == 0 && financialClassesView.Model_Coverage.CoPay == 0,
                "Patient Liability value is not reset to 0 when financial class not changed to 17");
        }
        private static Insurance  GetNewInsurance()
        {
              Insurance insurance = new Insurance();
            CommercialCoverage commercialCoverage = new CommercialCoverage();
            commercialCoverage.CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION);

            Payor p = new Payor();
            p.Code = "EN";
            InsurancePlan insurancePlan = new CommercialInsurancePlan();
            insurancePlan.Payor = p;
            insurancePlan.PlanSuffix = "901";
            commercialCoverage.InsurancePlan = insurancePlan;
            insurance.AddCoverage(commercialCoverage);
            return insurance;
        }
        private static FinancialClassesView GetStubFinancialClassesView(Account account)
        {
            var financialClassesView =new FinancialClassesView();
            financialClassesView.Model_Account = account;
            return financialClassesView;
        }
       
        private static Account GetAccount(Activity activity, DateTime accountCreatedDate,
            VisitType kindofVisit)
        {
            return new Account
            {
                Activity = activity,
                AccountCreatedDate = accountCreatedDate,
                KindOfVisit = kindofVisit

            };
        }
        private static Account GetAccountWithMedicareCoverage()
        {
            Account account = new Account();
            var facility = new Facility(54,
                                       PersistentModel.NEW_VERSION,
                                       "DOCTORS HOSPITAL DALLAS",
                                       "DHF");

            account.Facility = facility;
            User.SetCurrentUserTo(User.NewInstance());
            User.GetCurrent().Facility = account.Facility;
        
            account.Activity = new RegistrationActivity();
            Insurance insurance = new Insurance();
            GovernmentMedicareCoverage medicareCoverage = new GovernmentMedicareCoverage();
            medicareCoverage.CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION);

            Payor p = new Payor();
            p.Code = "53";
            InsurancePlan insurancePlan = new GovernmentMedicareInsurancePlan();
            insurancePlan.Payor = p;
            insurancePlan.PlanSuffix = "544";

            EmploymentStatus status = EmploymentStatus.NewRetired();
            Employment PatientEmployment = new Employment();
            PatientEmployment.Status = status;

            MedicareEntitlement medicareEntitlement = new AgeEntitlement(); 
             medicareEntitlement.PatientEmployment = PatientEmployment;
             account.MedicareSecondaryPayor.MedicareEntitlement = medicareEntitlement;

            medicareCoverage.InsurancePlan = insurancePlan;

            insurance.AddCoverage(medicareCoverage);

            account.Insurance = insurance;
             return account;
        }
        private static Account GetAccountWithMedicareCoverage(Facility facility, DateTime accountCreatedDate)
        {
            Account account = new Account();
            //var facility = new Facility(54,
            //    PersistentModel.NEW_VERSION,
            //    "DOCTORS HOSPITAL DALLAS",
            //    "DHF");

            account.Facility = facility;
            User.SetCurrentUserTo(User.NewInstance());
            User.GetCurrent().Facility = account.Facility;
            account.AccountCreatedDate = accountCreatedDate;
            account.Activity = new RegistrationActivity();
            Insurance insurance = new Insurance();
            GovernmentMedicareCoverage medicareCoverage = new GovernmentMedicareCoverage();
            medicareCoverage.CoverageOrder = new CoverageOrder(1L, CoverageOrder.PRIMARY_DESCRIPTION);

            Payor p = new Payor();
            p.Code = "53";
            InsurancePlan insurancePlan = new GovernmentMedicareInsurancePlan();
            insurancePlan.Payor = p;
            insurancePlan.PlanSuffix = "544";

            EmploymentStatus status = EmploymentStatus.NewRetired();
            Employment PatientEmployment = new Employment();
            PatientEmployment.Status = status;

            MedicareEntitlement medicareEntitlement = new AgeEntitlement();
            medicareEntitlement.PatientEmployment = PatientEmployment;
            account.MedicareSecondaryPayor.MedicareEntitlement = medicareEntitlement;

            medicareCoverage.InsurancePlan = insurancePlan;

            insurance.AddCoverage(medicareCoverage);

            account.Insurance = insurance;
            return account;
        }
        #endregion
        private Facility ICEFacility
        {
            get
            {
                var facility = new Facility(98,
                    PersistentModel.NEW_VERSION,
                    "ICE",
                    "ICE");
                facility["AutoCompleteNoLiabilityDueEnabled"] = true;
                return facility;
            }
        }

        private Facility DHFFacility
        {

            get
            {
                var facility = new Facility(54,
                    PersistentModel.NEW_VERSION,
                    "DHF",
                    "DHF");
                facility["AutoCompleteNoLiabilityDueEnabled"] = null;
                return facility;
            }
        }
        
        #region Constants
        private static DateTime GetTestDateAfterFeatureStart()
        {
            return new AutoCompleteNoLiabilityDueFeatureManager().FeatureStartDate.AddDays(10);
        }
        private static DateTime GetTestDateBeforeFeatureStart()
        {
            return new AutoCompleteNoLiabilityDueFeatureManager().FeatureStartDate.AddDays(-10);
        }
        #endregion
    }
}
