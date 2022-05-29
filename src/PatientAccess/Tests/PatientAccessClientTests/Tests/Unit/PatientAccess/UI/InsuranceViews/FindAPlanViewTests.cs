using NUnit.Framework;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews;

namespace Tests.Unit.PatientAccess.UI.InsuranceViews
{
    /// <summary>
    /// Summary description for FindAPlanViewTests
    /// </summary>
    [TestFixture]
    public class FindAPlanViewTests
    {
        [Test]
        public void TestEvaluateNoMedicarePrimaryPayorForAutoAccidentRule_WhenCoverageIsCommercial_ShouldNotFireEvent()
        {
            FindAPlanView findAPlanView = new FindAPlanView();

            Account account = new Account();
            account.Activity = new RegistrationActivity();
            Insurance insurance = new Insurance();
            CommercialCoverage commercialCoverage = new CommercialCoverage();
            commercialCoverage.CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION );
            insurance.AddCoverage(commercialCoverage);

            account.Insurance = insurance;

            Accident condition = new Accident { Kind = TypeOfAccident.NewAuto() };
            condition.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            account.Diagnosis.Condition = condition;

            findAPlanView.PatientAccount = account;
            if( RuleEngine.RulesToRun.Contains( typeof( NoMedicarePrimaryPayorForAutoAccident ) ) )
            {
                RuleEngine.RulesToRun.Remove( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
            }
            RuleEngine.RulesToRun.Add( typeof( NoMedicarePrimaryPayorForAutoAccident ),
                    new NoMedicarePrimaryPayorForAutoAccident() );

            bool result = findAPlanView.EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( null, delegate {});
            Assert.IsTrue( result );
        }

        [Test]
        public void TestEvaluateNoMedicarePrimaryPayorForAutoAccidentRule_WhenCoverageIsMedicare_ShouldFireEvent()
        {
            FindAPlanView findAPlanView = new FindAPlanView();

            Account account = new Account();
            account.Activity = new RegistrationActivity();
            Insurance insurance = new Insurance();
            GovernmentMedicareCoverage medicareCoverage = new GovernmentMedicareCoverage();
            medicareCoverage.CoverageOrder = new CoverageOrder( 1L, CoverageOrder.PRIMARY_DESCRIPTION );

            Payor p = new Payor();
            p.Code = "53";
            InsurancePlan insurancePlan = new GovernmentMedicareInsurancePlan();
            insurancePlan.Payor = p;
            insurancePlan.PlanSuffix = "54P";

            medicareCoverage.InsurancePlan = insurancePlan;

            insurance.AddCoverage( medicareCoverage );
            
            account.Insurance = insurance;

            Accident condition = new Accident { Kind = TypeOfAccident.NewAuto() };
            condition.Kind.OccurrenceCode.Code = OccurrenceCode.OCCURRENCECODE_ACCIDENT_AUTO;
            account.Diagnosis.Condition = condition;

            findAPlanView.PatientAccount = account;

            if( RuleEngine.RulesToRun.Contains( typeof( NoMedicarePrimaryPayorForAutoAccident ) ) )
            {
                RuleEngine.RulesToRun.Remove( typeof( NoMedicarePrimaryPayorForAutoAccident ) );
            }
            RuleEngine.RulesToRun.Add( typeof( NoMedicarePrimaryPayorForAutoAccident ),
                    new NoMedicarePrimaryPayorForAutoAccident() );

            bool result = findAPlanView.EvaluateNoMedicarePrimaryPayorForAutoAccidentRule( insurancePlan, delegate {} );
            Assert.IsFalse( result );
        }

        #region Private Methods
        #endregion

        #region Private Properties

        private RuleEngine RuleEngine
        {
            get
            {
                return RuleEngine.GetInstance();
            }
        }

        #endregion
    }
}
