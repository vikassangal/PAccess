using PatientAccess.Domain;
using PatientAccess.Rules;
using PatientAccess.UI.RegulatoryViews.Views;
using PatientAccess.Utilities;

namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public class AuthorizeAdditionalPortalUsersPresenter
    {
        #region Variables and Properties
        public Account Account { get; private set; }

        public readonly IAuthorizeAdditionalPortalUsersView AuthorizeAdditionalPortalUsersView;
       
        private readonly IAuthorizePortalUserFeatureManager AuthorizePortalUserFeatureManager;
            

        #endregion

        #region Constructors
        public AuthorizeAdditionalPortalUsersPresenter(IAuthorizeAdditionalPortalUsersView AuthUsersView, Account account,
            AuthorizePortalUserFeatureManager featureManager)
        {
            Guard.ThrowIfArgumentIsNull(AuthUsersView, "AuthorizeAdditionalPortalUsersView");
            Guard.ThrowIfArgumentIsNull(account, "Account");
            Guard.ThrowIfArgumentIsNull(featureManager, "AuthorizePortalUserFeatureManager");

            AuthorizeAdditionalPortalUsersView = AuthUsersView;
            Account = account;
            AuthorizePortalUserFeatureManager = featureManager;
            AuthorizeAdditionalPortalUsersView.AuthorizeAdditionalPortalUsersPresenter = this;

        }
        
        #endregion

        #region Public Methods
        public void AuthorizeAdditionalPortalUsersYes()
        {
            Account.AuthorizeAdditionalPortalUsers.SetYes();
            PopulateAuthorizeAdditionalPortalUsers();
            AuthorizeAdditionalPortalUsersView.ShowAuthorizeAdditionalPortalUserDetails();
        }
        public void AuthorizeAdditionalPortalUsersAsBlank()
        {
            Account.AuthorizeAdditionalPortalUsers.SetBlank();
        }

        public void AuthorizeAdditionalPortalUsersNo()
        {
            Account.AuthorizeAdditionalPortalUsers.SetNo();
        }

        public void UpdateView()
        {
            if (!IsAuthorizedPortalUserFeatureEnabled())
            {
                AuthorizeAdditionalPortalUsersView.HideAuthorizeAdditionalPortalUser();
            }
            else
            {
                AuthorizeAdditionalPortalUsersView.ShowAuthorizeAdditionalPortalUser();
                UpdateAuthorizeAdditionalPortalUsersOnTheView();
            }

        }

        public void EnableMe()
        {
            AuthorizeAdditionalPortalUsersView.EnableMe();
        }
        public void DisableMe()
        {
            AuthorizeAdditionalPortalUsersView.DisableMe();
        }

        public void UnSelectAuthorizeAdditionalPatientPortalUser()
        {
            AuthorizeAdditionalPortalUsersView.SetAuthorizeAdditionalPortalUserUnSelected();
        }

        #endregion

        #region Private Methods
        
        private void PopulateAuthorizeAdditionalPortalUsers()
        {
            AuthorizeAdditionalPortalUsersView.InitializeAuthorizedPortalUsersView();
            AuthorizeAdditionalPortalUsersView.AuthorizePortalUserView.ModelAccount = Account;
            AuthorizeAdditionalPortalUsersView.AuthorizePortalUserView.UpdateView();
        }
        
       
        private bool IsAuthorizedPortalUserFeatureEnabled()
        {
            return AuthorizePortalUserFeatureManager.ShouldFeatureBeEnabled(Account);
        }
        
       
        private void UpdateAuthorizeAdditionalPortalUsersOnTheView()
        {
            if (Account.AuthorizeAdditionalPortalUsers.IsYes)
            {
                AuthorizeAdditionalPortalUsersView.SetAuthorizeAdditionalPortalUserYes();
            }
            else if (Account.AuthorizeAdditionalPortalUsers.IsNo)
            {
                AuthorizeAdditionalPortalUsersView.SetAuthorizeAdditionalPortalUserNo();
            }
            else
            {
                AuthorizeAdditionalPortalUsersView.SetAuthorizeAdditionalPortalUserUnSelected();
            }
        }

        #endregion


    }
}
