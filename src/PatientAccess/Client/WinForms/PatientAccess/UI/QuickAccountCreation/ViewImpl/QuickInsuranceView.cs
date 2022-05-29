using System;
using System.Collections;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.UI.InsuranceViews;
using PatientAccess.UI.QuickAccountCreation.Presenters;
using PatientAccess.UI.QuickAccountCreation.Views;

namespace PatientAccess.UI.QuickAccountCreation.ViewImpl
{
    public delegate void CoverageDelegate( Coverage aCoverages );
    public delegate void CoveragesDelegate( ICollection coverages );

    [Serializable]
    public partial class QuickInsuranceView : ControlView, IQuickInsuranceView
    {
        #region Event Handlers


        public event CoverageDelegate CoverageResetClickedEvent;
        public event CoverageDelegate PlanSelectedEvent;
        public event CoveragesDelegate CoverageUpdatedEvent;

        private void QuickInsuranceViewOnLoad( object sender, EventArgs e )
        {
            btnEdit.Enabled = false;
            findAPlanView.IsPrimary = true;
        }

        private void FindAPlanViewOnPlanSelectedEvent( object sender, SelectInsuranceArgs e )
        {
            Presenter.FindAPlanViewPlanSelected( e );
        }

        public void PlanSelected( Coverage coverage )
        {
            PlanSelectedEvent( coverage );
        }

        public void FireCoverageUpdatedEvent()
        {
            if ( CoverageUpdatedEvent != null )
            {
                CoverageUpdatedEvent( Account.Insurance.Coverages );
            }
        }

        private void ResetOnClick( object sender, EventArgs e )
        {
            if ( ModelCoverage != null )
            {
                CoverageResetClickedEvent( ModelCoverage );
            }

        }

        public void ResetCoverage()
        {
            Presenter.ResetCoverage();
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

        public override void UpdateView()
        {
            Presenter.UpdateView();
            if ( account.HasInValidPrimaryPlanID )
            {
                findAPlanView.MtbPlanSearchEntry.UnMaskedText = account.EMPIPrimaryInvalidPlanID;
            }
        }

        public override void UpdateModel()
        {
        }

        public void SetDefaultFocus()
        {
            findAPlanView.SetDefaultFocus();
        }

        public void PopulatePlan( string planID, string planName )
        {
            PlanID = planID;
            PlanName = planName;
        }

        public Account PatientAccount
        {
            set { findAPlanView.PatientAccount = value; }
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
        public void ResetFindPlanView()

        {
            findAPlanView.ResetView();
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

        #endregion

        #region Private Methods

        #endregion

        #region Private Properties

        public IQuickInsurancePresenter Presenter { private get; set; }
 
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

        public QuickInsuranceView()
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
