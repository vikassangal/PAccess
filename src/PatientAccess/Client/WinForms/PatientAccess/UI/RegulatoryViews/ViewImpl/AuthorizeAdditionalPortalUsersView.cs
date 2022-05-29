using System;
using System.Windows.Forms;
using Extensions.UI.Winforms;
using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.Views;

namespace PatientAccess.UI.RegulatoryViews.ViewImpl
{
    public partial class AuthorizeAdditionalPortalUsersView : ControlView, IAuthorizeAdditionalPortalUsersView
    {
        #region Variables and Properties

        public Account ModelAccount { get; set; }
        public AuthorizeAdditionalPortalUsersPresenter AuthorizeAdditionalPortalUsersPresenter { get; set; }

        #endregion


        #region Private Methods

        private void rbAuthorizeAdditionalPortalUsersYes_Click(object sender, EventArgs e)
        {
            if (rbAuthorizeAdditionalPortalUsersYes.Checked)
            {
                AuthorizeAdditionalPortalUsersPresenter.AuthorizeAdditionalPortalUsersYes();
            }
        }

        private void rbAuthorizeAdditionalPortalUsersYes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbAuthorizeAdditionalPortalUsersNo.Focus();
            }
        }

        private void rbAuthorizeAdditionalPortalUsersNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Left || e.KeyData == Keys.Right || e.KeyData == Keys.Up || e.KeyData == Keys.Down)
            {
                rbAuthorizeAdditionalPortalUsersYes.Focus();
            }
        }

       
        #endregion

        #region Private Methods and Properties
        public override void UpdateView()
        {
            AuthorizeAdditionalPortalUsersPresenter = new AuthorizeAdditionalPortalUsersPresenter(this,  ModelAccount,
                new AuthorizePortalUserFeatureManager());
            AuthorizeAdditionalPortalUsersPresenter.UpdateView();

        }
        public void InitializeAuthorizedPortalUsersView()
        {
            this.authorizePortalUserView = new PatientAccess.UI.RegulatoryViews.ViewImpl.AuthorizePortalUserView();
        }

        public void SetAuthorizeAdditionalPortalUserYes()
        {
            rbAuthorizeAdditionalPortalUsersYes.Checked = true;
        }

        public void SetAuthorizeAdditionalPortalUserNo()
        {
            rbAuthorizeAdditionalPortalUsersNo.Checked = true;
        }

        public void SetAuthorizeAdditionalPortalUserUnSelected()
        {
            rbAuthorizeAdditionalPortalUsersYes.Checked = false;
            rbAuthorizeAdditionalPortalUsersNo.Checked = false;
            AuthorizeAdditionalPortalUsersPresenter.AuthorizeAdditionalPortalUsersAsBlank();
            rbAuthorizeAdditionalPortalUsersYes.TabStop = true;
        }
        public void ShowAuthorizeAdditionalPortalUserDetails()
        {
            AuthorizePortalUserView.ShowDialog();

        }
        public void EnableMe()
        {
            Enabled = true;
        }
        public void DisableMe()
        {
            Enabled = false;
        }
        public void ShowAuthorizeAdditionalPortalUser()
        {
            lblAuthoizePatientPortalUser.Visible = true;
            rbAuthorizeAdditionalPortalUsersYes.Visible = true;
            rbAuthorizeAdditionalPortalUsersNo.Visible = true;
        }

        public void HideAuthorizeAdditionalPortalUser()
        {
            lblAuthoizePatientPortalUser.Visible = false;
            rbAuthorizeAdditionalPortalUsersYes.Visible = false;
            rbAuthorizeAdditionalPortalUsersNo.Visible = false;
        }

        public AuthorizeAdditionalPortalUsersView()
        {
            InitializeComponent();
        }

        public AuthorizePortalUserView AuthorizePortalUserView
        {
            get { return this.authorizePortalUserView; }
        }

        private void rbAuthorizeAdditionalPortalUsersNo_Click(object sender, EventArgs e)
        {
            if (rbAuthorizeAdditionalPortalUsersNo.Checked)
            {
                AuthorizeAdditionalPortalUsersPresenter.AuthorizeAdditionalPortalUsersNo();
            }
        }

        #endregion
       
    }
}
