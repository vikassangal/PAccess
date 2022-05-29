using PatientAccess.UI.RegulatoryViews.Presenters;
using PatientAccess.UI.RegulatoryViews.ViewImpl;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IAuthorizeAdditionalPortalUsersView
    {
        void EnableMe();
        void DisableMe();
        void ShowAuthorizeAdditionalPortalUser();
        void HideAuthorizeAdditionalPortalUser();
        void SetAuthorizeAdditionalPortalUserYes();
        void SetAuthorizeAdditionalPortalUserNo();
        void SetAuthorizeAdditionalPortalUserUnSelected();
        void ShowAuthorizeAdditionalPortalUserDetails();
        AuthorizePortalUserView AuthorizePortalUserView { get; }
        void InitializeAuthorizedPortalUsersView();
        AuthorizeAdditionalPortalUsersPresenter AuthorizeAdditionalPortalUsersPresenter { get; set; }

    }
}