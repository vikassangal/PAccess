using PatientAccess.Domain;

namespace PatientAccess.UI.PreMSEViews
{

    /// <summary>
    /// 
    /// </summary>
    public interface IPreMseDemographicsPresenter
    {
        #region Operations
        void UpdateOtherLanguage( string otherLanguage );
        void SelectedLanguageChanged( Language language );
        void RegisterOtherLanguageRequiredRule();
        void UnRegisterOtherLanguageRequiredRule();
        #endregion Operations
    }
}