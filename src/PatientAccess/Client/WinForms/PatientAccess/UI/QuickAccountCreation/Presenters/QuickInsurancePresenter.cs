using System;
using System.Windows.Forms;
using PatientAccess.Domain;
using PatientAccess.Domain.Parties;
using PatientAccess.Rules;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.QuickAccountCreation.ViewImpl;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.Presenters
{
    internal class QuickInsurancePresenter : IQuickInsurancePresenter
    {
        #region Constructors

        internal QuickInsurancePresenter( IQuickInsuranceView view, Account modelAccount, IRuleEngine ruleEngine )
        {
            View = view;
            Account = modelAccount;
            RuleEngine = ruleEngine;
        }

        #endregion Constructors

        #region Properties

        private IQuickInsuranceView View { get; set; }

        private Account Account { get; set; }

        private IRuleEngine RuleEngine { get; set; }

        #endregion Properties

        #region Public Methods

        public void UpdateView()
        {
            View.PatientAccount = Account;
            if ( View.ModelCoverage != null && View.ModelCoverage.CoverageOrder != null )
            {
                View.ModelCoverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "Primary" );
                if (View.ModelCoverage != null && View.ModelCoverage.InsurancePlan != null)
                {
                    View.PopulatePlan(View.ModelCoverage.InsurancePlan.PlanID, View.ModelCoverage.InsurancePlan.PlanName);
                }
                View.EnableEditButton();
            }

            else
            {
                View.DisableEditButton();
            }
        }

        public void ResetView()
        {
            View.ModelCoverage = null;
            View.PopulatePlan( String.Empty, String.Empty );
            View.DisableEditButton();
            View.ResetFindPlanView();
            UpdateView();
        }

        public void ResetCoverage()
        {
            ResetView();
            RuleEngine.EvaluateRule<MedicarePatientHasHMO>( Account.Insurance );
        }

        public void Edit()
        {
            View.InsuranceDetailsDialog = new InsuranceDetails();
            try
            {
                View.InsuranceDetailsDialog.insuranceDetailsView.Model_Coverage = View.ModelCoverage;
                View.InsuranceDetailsDialog.insuranceDetailsView.Account = Account;
                View.InsuranceDetailsDialog.insuranceDetailsView.Active_Tab = PLAN_DETAILS_PAGE;

                View.InsuranceDetailsDialog.UpdateView();

                View.ModelCoverage = View.InsuranceDetailsDialog.insuranceDetailsView.Model_Coverage;

                if ( View.InsuranceDetailsDialog.ShowDialog( (QuickInsuranceView)View ) == DialogResult.OK )
                {
                    UpdateView();
                }

                View.FireCoverageUpdatedEvent();
            }

            finally
            {
                View.InsuranceDetailsDialog.Dispose();
            }
        }

        public void FindAPlanViewPlanSelected( SelectInsuranceArgs args )
        {
            var insurancePlan = args.SelectedPlan as InsurancePlan;
            var employer = args.SelectedEmployer as Employer;
            var coverage = Coverage.CoverageFor( insurancePlan, new Insured() );
            coverage.CoverageOrder = new CoverageOrder( CoverageOrder.PRIMARY_OID, "Primary" );
            if ( coverage.Insured == null )
            {
                coverage.Insured = new Insured();
            }

            if ( coverage.Insured.Employment == null )
            {
                coverage.Insured.Employment = new Employment();
            }

            coverage.Insured.Employment.Employer = employer;
            if ( coverage.Insured.Employment.Employer != null )
            {
                var employerContactPoint = coverage.Insured.Employment.Employer.ContactPointWith( TypeOfContactPoint.NewEmployerContactPointType() );
                coverage.Insured.Employment.Employer.PartyContactPoint = employerContactPoint;
            }

            View.PlanSelected( coverage );
        }

        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

        #region Data Elements

        #endregion Data Elements

        #region Constants

        private const int PLAN_DETAILS_PAGE = 0;

        #endregion Constants
    }
}