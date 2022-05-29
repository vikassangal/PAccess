namespace PatientAccess.UI.RegulatoryViews.Presenters
{
    public interface IAuthorizePortalUserDetailPresenter
    {
        void UpdateView(); 
        void CheckAllFieldsValuesAreEntered(); 
        bool HasAllFieldsEntered();
        bool HasMissingEntries { get; }  
    }
}