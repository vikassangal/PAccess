using System;
using System.Collections.Generic;
using PatientAccess.Domain;
using PatientAccess.UI.CommonControls;
using PatientAccess.UI.RegulatoryViews.Presenters;

using PatientAccess.UI.RegulatoryViews.Views;


namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class AuthorizePortalUserView : TimeOutFormView, IAuthorizePortalUserView
    {
        private IAuthorizePortalUserPresenter authorizePortalUserPresenter;

        #region Properties

        public Account ModelAccount { get; set; }

        #endregion

        #region Construction and Finalization

        public AuthorizePortalUserView()
        {
            InitializeComponent();
            EnableThemesOn(this);
            this.CenterToScreen();
        }

        #endregion

        #region Private Methods

        private void btnSaveResponse_Click(object sender, EventArgs e)
        {
            if (authorizePortalUserPresenter.ValidateAuthorizePortalUser())
            {
                if (authorizePortalUserPresenter.HandleSaveResponse())
                {
                    CloseView();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseView();
        }

        #endregion

        #region Public Methods
        
        public void ShowAsDialog()
        {
            ShowDialog();
        }

        public void CloseView()
        {
            Close();
        }

        public override void UpdateView()
        {
            authorizePortalUserPresenter =
                new AuthorizePortalUserPresenter(this, new MessageBoxAdapter(), ModelAccount);
            authorizePortalUserPresenter.UpdateView();
        }


        public IDictionary<int, AuthorizedAdditionalPortalUser> AuthUserCollection { get; set; }

        public AuthorizePortalUserDetailView AuthorizePortalUserDetail0
        {
            get { return authorizePortalUserDetailView0; }
            set { authorizePortalUserDetailView0 = value; }
        }

        public AuthorizePortalUserDetailView AuthorizePortalUserDetail1
        {
            get { return authorizePortalUserDetailView1; }
            set { authorizePortalUserDetailView1 = value; }
        }

        public AuthorizePortalUserDetailView AuthorizePortalUserDetail2
        {
            get { return authorizePortalUserDetailView2; }
            set { authorizePortalUserDetailView2 = value; }
        }

        public AuthorizePortalUserDetailView AuthorizePortalUserDetail3
        {
            get { return authorizePortalUserDetailView3; }
            set { authorizePortalUserDetailView3 = value; }
        }

        #endregion

        #region Data Elements

        #endregion

        #region Constants

        #endregion
        
    }
}
