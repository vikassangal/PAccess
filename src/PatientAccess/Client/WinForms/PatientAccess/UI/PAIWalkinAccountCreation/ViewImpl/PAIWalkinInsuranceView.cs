using System;
using System.Collections;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.PAIWalkinAccountCreation.Presenters;
using PatientAccess.UI.PAIWalkinAccountCreation.Views;

namespace PatientAccess.UI.PAIWalkinAccountCreation.ViewImpl
{
    public delegate void CoverageDelegate( Coverage aCoverages );
    public delegate void CoveragesDelegate( ICollection coverages );

    [Serializable]
    public partial class PAIWalkinInsuranceView : ControlView, IPAIWalkinInsuranceView
    {
        #region Event Handlers


        public event CoverageDelegate CoverageResetClickedEvent;
        public event CoverageDelegate PlanSelectedEvent;
        public event CoveragesDelegate CoverageUpdatedEvent;

        private void PAIWalkinInsuranceViewOnLoad( object sender, EventArgs e )
        {
            btnEdit.Enabled = false;
            findAPlanView.IsPrimary = true;
        }

        private void FindAPlanViewOnPlanSelectedEvent( object sender, SelectInsuranceArgs e )
        {
            Presenter.FindAPlanViewPlanSelected( e );
        }

        private void ResetOnClick( object sender, EventArgs e )
        {
            if ( ModelCoverage != null )
            {
                CoverageResetClickedEvent( ModelCoverage );
            }

        }

        private void EditOnClick( object sender, EventArgs e )
        {
            Presenter.Edit();
        }

        #endregion

        #region Methods
        public void ResetView()
        {
            Presenter.ResetView();
        }

        public void ResetCoverage()
        {
            Presenter.ResetCoverage();
        }

        public void SetDefaultFocus()
        {
            findAPlanView.SetDefaultFocus();
        }

        public void FireCoverageUpdatedEvent()
        {
            if ( CoverageUpdatedEvent != null )
            {
                CoverageUpdatedEvent( Account.Insurance.Coverages );
            }
        }

        public void PlanSelected( Coverage coverage )
        {
            PlanSelectedEvent( coverage );
        }

        public void ResetFindPlanView()
        {
            findAPlanView.ResetView();
        }

        public override void UpdateView()
        {
            Presenter.UpdateView();
            if ( Account.HasInValidPrimaryPlanID )
            {
                findAPlanView.MtbPlanSearchEntry.UnMaskedText = Account.EMPIPrimaryInvalidPlanID;
            }
        }

        public override void UpdateModel()
        {
        }

        public void PopulatePlan( string planID, string planName )
        {
            PlanID = planID;
            PlanName = planName;
        }

        public void EnableEditButton()
        {
            btnEdit.Enabled = true;
        }

        public void DisableEditButton()
        {
            btnEdit.Enabled = false;
        }

        #endregion

        #region Properties

        public IPAIWalkinInsurancePresenter Presenter { private get; set; }

        public Coverage ModelCoverage
        {
            get
            {
                return (Coverage)Model;
            }
            set
            {
                Model = value;
            }
        }

        public InsuranceDetails InsuranceDetailsDialog
        {
            get
            {
                return insuranceDetailsDialog;
            }
            set
            {
                insuranceDetailsDialog = value;
            }
        }

        public Account PatientAccount
        {
            set { findAPlanView.PatientAccount = value; }
        }

        public Account Account
        {
            private get
            {
                return account;
            }
            set
            {
                account = value;
            }
        }

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        private string PlanID
        {
            set { lblPlanID.Text = value; }
        }

        private string PlanName
        {
            set { lblPlanName.Text = value; }
        }

        #endregion

        #region Construction and Finalization

        public PAIWalkinInsuranceView()
        {
            InitializeComponent();
            EnableThemesOn( this );
        }

        #endregion

        #region Data Elements

        private Account account;

        #endregion

        #region Constants
        #endregion
    }
}
