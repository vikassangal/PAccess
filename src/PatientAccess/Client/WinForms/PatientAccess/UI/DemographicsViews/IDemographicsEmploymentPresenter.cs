using PatientAccess.Domain;

namespace PatientAccess.UI.DemographicsViews
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDemographicsEmploymentPresenter
    {
        #region Operations
        void UpdateOtherLanguage( string otherLanguage );
        void SelectedLanguageChanged( Language language );
        void RegisterOtherLanguageRequiredRule();
        void UnRegisterOtherLanguageRequiredRule();
        #endregion Operations
    }
}