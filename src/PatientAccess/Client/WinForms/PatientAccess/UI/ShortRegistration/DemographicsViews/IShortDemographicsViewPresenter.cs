using PatientAccess.Domain;

namespace PatientAccess.UI.ShortRegistration.DemographicsViews
{
    public interface IShortDemographicsViewPresenter
    {
        #region Operations
        void UpdateOtherLanguage( string otherLanguage );
        void SelectedLanguageChanged( Language language );
        void RegisterOtherLanguageRequiredRule();
        void UnRegisterOtherLanguageRequiredRule();
        void EvaluateOtherLanguageRequired();

        #endregion Operations
    }
}
