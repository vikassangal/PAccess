using System;
using Extensions.UI.Winforms;
using PatientAccess.Domain;

namespace PatientAccess.UI.InsuranceViews.AuthorizationViews
{
    public partial class InsuranceVerificationSummary : ControlView
    {
        #region Event Handlers
        #endregion

        #region Methods
        public override void UpdateView()
        {
            if( this.Model != null )
            {
                PopulateView();
            }
        }
        #endregion

        #region Properties
        public new Coverage Model
        {
            set
            {
                base.Model = value;
            }
            private get
            {
                return base.Model as Coverage;
            }
        }

        public Account Account
        {
            private get
            {
                return i_Account;
            }
            set
            {
                i_Account = value;
            }
        }
        #endregion

        #region Private Methods

        private void PopulateView()
        {
            this.lblPatientName.Text = Account.Patient.FormattedName;

            if( Account.Patient.DateOfBirth != DateTime.MinValue )
            {
                lblDOB.Text = String.Format( "{0:D2}/{1:D2}/{2:D4}",
                    Account.Patient.DateOfBirth.Month,
                    Account.Patient.DateOfBirth.Day,
                    Account.Patient.DateOfBirth.Year );
            }
            else
            {
                lblDOB.Text = string.Empty;
            }

            if( Account.AccountNumber != 0 )
            {
                lblAccount.Text = Account.AccountNumber.ToString();
            }
            if( Account.KindOfVisit != null )
            {
                lblPatientType.Text = Account.KindOfVisit.ToString();
            }

            if( Account.FinancialClass != null )
            {
                lblFinancialClass.Text = Account.FinancialClass.ToString();
            }
            if( Account.HospitalService != null )
            {
                lblHospitalService.Text = Account.HospitalService.ToString();
            }
            lblFacilityID.Text = Account.Facility.FederalTaxID;

            lblChiefComplaint.Text = Account.Diagnosis.ChiefComplaint;

            if( Model.InsurancePlan.GetType() == typeof( GovernmentMedicaidInsurancePlan ) )
            {
                lblMedicaidProvider.Text = Model.InsurancePlan.PlanID;
            }
            if ( Model.GetType().IsSubclassOf(typeof(CoverageGroup) ))
            {
                CoverageGroup coverageGroup = Model as CoverageGroup;

                if ( coverageGroup.Authorization.AuthorizationCompany != String.Empty )
                {
                    lblAuthorizationCompany.Text = coverageGroup.Authorization.AuthorizationCompany.Trim();
                }
                if ( coverageGroup.Authorization.AuthorizationPhone != null )
                {
                    lblPhone.Text = coverageGroup.Authorization.AuthorizationPhone.ToString();
                }
                if (coverageGroup.Authorization.PromptExt != null)
                {
                    lblPrompt.Text = coverageGroup.Authorization.PromptExt.Trim();
                }
            }
        }
        #endregion

        #region Private Properties
        #endregion

        #region Construction and Finalization
        public InsuranceVerificationSummary()
        {
            InitializeComponent();
        }
        #endregion

        #region Data Elements
        private Account i_Account = null;
        #endregion

        #region Constants
        #endregion


    }
}
