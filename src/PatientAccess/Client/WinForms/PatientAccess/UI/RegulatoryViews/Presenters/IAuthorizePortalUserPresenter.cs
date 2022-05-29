namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public interface IAuthorizePortalUserPresenter
    {
        void UpdateView();
        bool HandleSaveResponse();
        bool ValidateAuthorizePortalUser();
        bool HasMissingEntriesInARow();
    }
}