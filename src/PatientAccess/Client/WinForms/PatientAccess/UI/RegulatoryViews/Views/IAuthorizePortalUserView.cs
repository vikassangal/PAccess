using PatientAccess.UI.RegulatoryViews.ViewImpl;

namespace PatientAccess.UI.RegulatoryViews.Views
{
    public interface IAuthorizePortalUserView
    {
        void UpdateView();
        void ShowAsDialog();
        void CloseView();
        AuthorizePortalUserDetailView AuthorizePortalUserDetail0 { get; set; }
        AuthorizePortalUserDetailView AuthorizePortalUserDetail1 { get; set; }
        AuthorizePortalUserDetailView AuthorizePortalUserDetail2 { get; set; }
        AuthorizePortalUserDetailView AuthorizePortalUserDetail3 { get; set; }
    }
}
